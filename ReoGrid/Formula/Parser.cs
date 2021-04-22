/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net/
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * Author: Jing Lu <jingwood at unvell.com>
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

#if FORMULA

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace unvell.ReoGrid.Formula
{
	#region Parser
	/// <summary>
	/// Excel-compatible formula syntax parser
	/// </summary>
	static class Parser
	{
		#region Parser API
		/// <summary>
		/// Convert formula to syntax tree.
		/// </summary>
		/// <param name="workbook">Workbook instance.</param>
		/// <param name="cell">Cell instance.</param>
		/// <param name="input">Formula to be converted.</param>
		/// <returns>syntax tree constructed from specified formula.</returns>
		public static STNode Parse(IWorkbook workbook, Cell cell, string input)
		{
			ExcelFormulaLexer lexer = new ExcelFormulaLexer(workbook, cell, input);
			var node = ReadExpr(lexer);

			if (lexer.CurrentToken != null && lexer.CurrentToken.Success)
			{
				throw CreateException(lexer, "unexpect token: " + lexer.CurrentToken.Value);
			}

			return node;
		}
		#endregion // Parser API

		#region Parsers
		private static STNode ReadExpr(ExcelFormulaLexer lexer)
		{
			var node = ReadCompare(lexer);
			return node;
		}

		private static STNode ReadCompare(ExcelFormulaLexer lexer)
		{
			var node = ReadConnect(lexer);
			if (node == null) return null;

			STNodeType type = STNodeType.NONE;

			if (lexer.IsToken("=") || lexer.IsToken("=="))
				type = STNodeType.EQUALS;
			else if (lexer.IsToken("<>") || lexer.IsToken("!="))
				type = STNodeType.NOT_EQUALS;
			else if (lexer.IsToken(">"))
				type = STNodeType.GREAT_THAN;
			else if (lexer.IsToken("<"))
				type = STNodeType.LESS_THAN;
			else if (lexer.IsToken(">="))
				type = STNodeType.GREAT_EQUALS;
			else if (lexer.IsToken("<="))
				type = STNodeType.LESS_EQUALS;
			else
				return node;

			lexer.NextToken();

			var right = ReadExpr(lexer);
			if (right == null) throw CreateException(lexer, "expect expression");

			node = CreateNode(lexer, type, node.Start, lexer.CommittedLength - node.Start,
				new List<STNode> { node, right });

			return node;
		}

		internal static STNode ParseInterCompareExp(Cell cell, string input)
		{
			ExcelFormulaLexer lexer = new ExcelFormulaLexer(
				cell.Worksheet == null ? null : cell.Worksheet.workbook, cell, input);

			STNodeType type = STNodeType.NONE;

			if (lexer.IsToken("=") || lexer.IsToken("=="))
				type = STNodeType.EQUALS;
			else if (lexer.IsToken("<>") || lexer.IsToken("!="))
				type = STNodeType.NOT_EQUALS;
			else if (lexer.IsToken(">"))
				type = STNodeType.GREAT_THAN;
			else if (lexer.IsToken("<"))
				type = STNodeType.LESS_THAN;
			else if (lexer.IsToken(">="))
				type = STNodeType.GREAT_EQUALS;
			else if (lexer.IsToken("<="))
				type = STNodeType.LESS_EQUALS;

			if (type != STNodeType.NONE)
			{
				lexer.NextToken();

				var right = ReadExpr(lexer);
				if (right != null)
				{
					return CreateNode(lexer, type, 0, lexer.CommittedLength, new List<STNode> { null, right });
				}
			}

			STNode node = ReadConnect(lexer);

			return new STNode(STNodeType.EQUALS, 0, input.Length, new List<STNode> { null,
				(node != null && node.Type != STNodeType.IDENTIFIER) ? node : 
				new STStringNode(input, 0, input.Length) });
		}

		private static STNode ReadConnect(ExcelFormulaLexer lexer)
		{
			var node = ReadAdd(lexer);
			if (node == null) return null;

			while (lexer.SkipToken("&"))
			{
				var right = ReadAdd(lexer);
				if (right == null) throw CreateException(lexer, "expect expression");

				node = CreateNode(lexer, STNodeType.CONNECT, node.Start, lexer.CommittedLength - node.Start,
					new List<STNode> { node, right });
			}

			return node;
		}

		private static STNode ReadAdd(ExcelFormulaLexer lexer)
		{
			var node = ReadMul(lexer);
			if (node == null) return null;

			while (true)
			{
				STNodeType type = STNodeType.NONE;

				if (lexer.IsToken("+"))
					type = STNodeType.ADD;
				else if (lexer.IsToken("-"))
					type = STNodeType.SUB;
				else
					break;

				lexer.NextToken();

				var right = ReadMul(lexer);
				if (right == null) throw CreateException(lexer, "expect expression");

				node = CreateNode(lexer, type, node.Start, lexer.CommittedLength - node.Start,
					new List<STNode> { node, right });
			}

			return node;
		}

		private static STNode ReadMul(ExcelFormulaLexer lexer)
		{
			var node = ReadExponent(lexer);
			if (node == null) return node;

			while (true)
			{
				STNodeType type = STNodeType.NONE;

				if (lexer.IsToken("*"))
					type = STNodeType.MUL;
				else if (lexer.IsToken("/"))
					type = STNodeType.DIV;
				else
					break;

				lexer.NextToken();

				var right = ReadExponent(lexer);
				if (right == null) throw CreateException(lexer, "expect expression");

				node = CreateNode(lexer, type, node.Start, lexer.CommittedLength - node.Start,
					new List<STNode> { node, right });
			}

			return node;
		}

		private static STNode ReadExponent(ExcelFormulaLexer lexer)
		{
			var node = ReadPercent(lexer);
			if (node == null) return node;

			while (lexer.SkipToken("^"))
			{
				var right = ReadPercent(lexer);
				if (right == null) throw CreateException(lexer, "expect expression");

				node = CreateNode(lexer, STNodeType.POW, node.Start, lexer.CommittedLength - node.Start,
					new List<STNode> { node, right });
			}

			return node;
		}

		private static STNode ReadPercent(ExcelFormulaLexer lexer)
		{
			var node = ReadMinus(lexer);
			if (node == null) return null;

			while (lexer.SkipToken("%"))
			{
				node = CreateNode(lexer, STNodeType.UNARY_PERCENT, node.Start, lexer.CommittedLength - node.Start,
					new List<STNode>() { node });
			}

			return node;
		}

		private static STNode ReadMinus(ExcelFormulaLexer lexer)
		{
			if (!lexer.IsToken("-")) return ReadFunctionCall(lexer);

			int start = lexer.Index;
			lexer.NextToken();

			var node = ReadFunctionCall(lexer);
			if (node == null) throw CreateException(lexer, "expect expression");

			return CreateNode(lexer, STNodeType.UNARY_MINUS, start, lexer.CommittedLength - start, new List<STNode> { node });
		}

		private static STNode ReadFunctionCall(ExcelFormulaLexer lexer)
		{
			var id = ReadSheetName(lexer);
			if (id == null) return null;

			string funName = null;

			// process function name like 'LOG10'
			if (id.Type == STNodeType.CELL)
			{
				Group mg = null;

				if (lexer.CurrentToken == null
					|| (mg = lexer.CurrentToken.Groups["token"]) == null
					|| !mg.Success
					|| mg.Value != "(")
					return id;

				funName = lexer.Input.Substring(id.Start, id.Length);
				lexer.NextToken();
			}
			else
			{
				if (id.Type != STNodeType.IDENTIFIER
					|| !lexer.SkipToken("(")) return id;

				funName = ((STIdentifierNode)id).Identifier;
			}

			var parameterList = ReadParameterList(lexer);

			if (!lexer.SkipToken(")")) throw CreateException(lexer, "expect )");

			return new STFunctionNode(funName, id.Start, lexer.CommittedLength - id.Start, parameterList);
		}

		private static List<STNode> ReadParameterList(ExcelFormulaLexer lexer)
		{
			List<STNode> nodes = new List<STNode>();

			int i = 0;
			while (true)
			{
				STNode node = ReadExpr(lexer);
				nodes.Add(node);
				i++;

				if (!lexer.SkipToken(FormulaExtension.ParameterSeparator)) break;
			}

			while (nodes.Count > 0 && nodes[nodes.Count - 1] == null)
			{
				nodes.RemoveAt(nodes.Count - 1);
			}

			return nodes.Count == 0 ? null : nodes;
		}

		private static STNode ReadSheetName(ExcelFormulaLexer lexer)
		{
			var node = ReadPrimary(lexer);
			if (node == null) return null;

			// neither a sheet name nor a function scope
			if (node.Type != STNodeType.IDENTIFIER)
			{
				return node;
			}

			if (lexer.SkipToken("!"))
			{
				var id = ReadPrimary(lexer);

				if (id.Type != STNodeType.CELL
					&& id.Type != STNodeType.RANGE
					&& id.Type != STNodeType.IDENTIFIER)
				{
					throw CreateException(lexer, "expect Cell/Range/Name reference");
				}

				var sheetName = ((STIdentifierNode)node).Identifier;

				IWorkbook workbook = lexer.Workbook;

				if (workbook == null
					&& lexer.Cell != null && lexer.Cell.Worksheet != null && lexer.Cell.Worksheet.workbook != null)
				{
					workbook = lexer.Cell.Worksheet.workbook;
				}

				// only set worksheet reference if cell or worksheet associated
				if (workbook != null)
				{
					switch (id.Type)
					{
						case STNodeType.CELL:
							{
								STCellNode cellNode = (STCellNode)id;
								cellNode.Worksheet = workbook.GetWorksheetByName(sheetName);
							}
							break;

						case STNodeType.RANGE:
							{
								STRangeNode rangeNode = (STRangeNode)id;
								rangeNode.Worksheet = workbook.GetWorksheetByName(sheetName);
							}
							break;

						case STNodeType.IDENTIFIER:
							{
								STIdentifierNode idNode = (STIdentifierNode)id;
								idNode.Worksheet = workbook.GetWorksheetByName(sheetName);
							}
							break;
					}
				}

				return id;
			}
			else if (lexer.SkipToken("."))
			{
				var id = ReadPrimary(lexer);

				if (id.Type != STNodeType.IDENTIFIER)
				{
					throw CreateException(lexer, "expect identifier");
				}

				return id;
			}
			else
			{
				return node;
			}
		}

		private static STNode ReadPrimary(ExcelFormulaLexer lexer)
		{
			STNode node;

			if (CommitMatchNode(lexer, "string", STNodeType.STRING, out node)
				|| CommitMatchNode(lexer, "identifier", STNodeType.IDENTIFIER, out node)
				|| CommitMatchNode(lexer, "number", STNodeType.NUMBER, out node)
				|| CommitMatchNode(lexer, "cell", STNodeType.CELL, out node)
				|| CommitMatchNode(lexer, "range", STNodeType.RANGE, out node)
				|| CommitMatchNode(lexer, "true", STNodeType.TRUE, out node)
				|| CommitMatchNode(lexer, "false", STNodeType.FALSE, out node)
				|| CommitMatchNode(lexer, "union_ranges", STNodeType.INTERSECTION, out node))
			{
				return node;
			}
			//else if (lexer.IsMatch("abs_cell"))
			//{
			//	var g = lexer.CurrentToken;
				
			//	int col = RGUtility.GetNumberOfChar(g.Groups["col"].Value) - 1;
			//	int row = int.Parse(g.Groups["row"].Value) - 1;
				
			//	lexer.NextToken();

			//	return new STCellNode(lexer.Cell.Worksheet, new ReoGridPos(row, col), STNodeType.CELL, g.Index, g.Length);
			//}
			else if (lexer.IsToken("("))
			{
				int start = lexer.Index;

				lexer.NextToken();

				var expr = ReadExpr(lexer);
				if (expr == null) throw CreateException(lexer, "expect expression");

				if (!lexer.SkipToken(")")) throw CreateException(lexer, "expect )");

				return CreateNode(lexer, STNodeType.SUB_EXPR, start, lexer.CommittedLength - start,
					new List<STNode>() { expr });
			}
			else
				return null;
		}

		private static FormulaParseException CreateException(ExcelFormulaLexer lexer, string msg)
		{
			return new FormulaParseException(msg, lexer.CommittedLength);
		}

		#region Commit & STNode Constructions
		private static bool CommitMatchNode(ExcelFormulaLexer lexer, string groupName, STNodeType type, out STNode node)
		{
			if (lexer.IsMatch(groupName))
			{
				var g = lexer.CurrentToken.Groups[groupName];
				node = CommitRunAndCreateNode(lexer, type, g.Index, g.Length, null);
				return true;
			}
			else
			{
				node = null;
				return false;
			}
		}

		private static STNode CommitRunAndCreateNode(ExcelFormulaLexer lexer, STNodeType type, 
			int start, int len, List<STNode> nodes)
		{
			lexer.NextToken();
			return CreateNode(lexer, type, start, len, nodes);
		}

		private static STNode CreateNode(ExcelFormulaLexer lexer, STNodeType type)
		{
			return new STNode(type, lexer.CurrentToken.Index, lexer.CurrentToken.Length, null);
		}

		private static STNode CreateNode(ExcelFormulaLexer lexer, STNodeType type, int start, int len, List<STNode> nodes)
		{
			switch (type)
			{
				case STNodeType.NUMBER:
					string text = lexer.Input.Substring(start, len);
					return double.TryParse(text, out var v) ? new STNumberNode(v, start, len) : null;

				case STNodeType.IDENTIFIER:
					return new STIdentifierNode(lexer.Cell == null ? null : lexer.Cell.Worksheet, lexer.Input.Substring(start, len), start, len);

				case STNodeType.STRING:
					return new STStringNode(lexer.Input, start, len);

				case STNodeType.RANGE:
					return new STRangeNode(null, new RangePosition(lexer.Input.Substring(start, len)), start, len);

				case STNodeType.CELL:
					return new STCellNode(null, new CellPosition(lexer.Input.Substring(start, len)), type, start, len);

				default:
					return new STNode(type, start, len, nodes);
			}
		}
		#endregion // Commit & STNode Constructions

		#endregion // Parsers

		#region Dump Nodes
		///// <summary>
		///// Recursive to iterate a syntax tree and dump its all of nodes
		///// </summary>
		///// <param name="node">root node of syntax tree to be outputted</param>
		///// <param name="originalInput">Origional input sentence in string</param>
		///// <returns>output string including the information of each nodes from a syntax tree</returns>
		//static string DumpTree(STNode node, string originalInput)
		//{
		//	StringBuilder sb = new StringBuilder();
		//	node.DumpTree(sb, originalInput);
		//	return sb.ToString();
		//}
		#endregion // Dump Nodes
	}
	#endregion Parser

	#region Lexer
	internal class ExcelFormulaLexer
	{
		public IWorkbook Workbook { get; set; }
		public Cell Cell { get; set; }

		private static readonly Regex TokenRegex = new Regex(
			"\\s*((?<string>\"(?:\"\"|[^\"])*\")|(?<union_ranges>[A-Z]+[0-9]+:[A-Z]+[0-9]+(\\s[A-Z]+[0-9]+:[A-Z]+[0-9]+)+)"
			+ "|(?<range>\\$?[A-Z]+\\$?[0-9]*:\\$?[A-Z]+\\$?[0-9]*)"
			+ "|(?<cell>\\$?[A-Z]+\\$?[0-9]+)"
			+ "|(?<token>-)|(?<number>\\-?\\d*\\" + FormulaExtension.NumberDecimalSeparator + "?\\d+)"
			+ "|(?<true>(?i)TRUE)|(?<false>(?i)FALSE)|(?<identifier>\\w+)"
			+ "|(?<token>\\=\\=|\\<\\>|\\<\\=|\\>\\=|\\<\\>|\\=|\\!|[\\=\\.\\"
			+ FormulaExtension.ParameterSeparator // ,
			+ "\\+\\-\\*\\/\\%\\<\\>\\(\\)\\&\\^]))",
			RegexOptions.Compiled);

		public string Input { get; set; }

		public int Start { get; set; }
		public int Length { get; set; }

		public int Index
		{
			get
			{
				return this.match == null ? 0 : (
					this.match.Groups["token"].Success ? this.match.Groups["token"].Index : this.match.Index);
			}
		}

		public int CommittedLength { get; set; }

		private Match match;

		public Match CurrentToken { get { return this.match; } }

		public ExcelFormulaLexer(IWorkbook workbook, Cell cell, string input)
			: this(workbook, cell, input, 0, input.Length)
		{
		}

		public ExcelFormulaLexer(IWorkbook workbook, Cell cell, string input, int start, int length)
		{
			this.Workbook = workbook;
			this.Cell = cell;
			this.Input = input;
			this.Start = start;
			this.Length = length;

			Reset();
		}

		public void NextToken()
		{
			if (this.match != null)
			{
				CommittedLength += this.match.Length;

				if (CommittedLength >= Length)
				{
					this.match = null;
				}
				else
				{
					this.match = this.match.NextMatch();
				}
			}
			else
			{
				this.match = null;
			}
		}

		public bool SkipToken(string value)
		{
			return SkipToken("token", value);
		}

		public bool SkipToken(string groupName, string value)
		{
			if (IsMatch(groupName, value))
			{
				NextToken();
				return true;
			}
			else
				return false;
		}

		public bool IsToken(string token)
		{
			return IsMatch("token", token);
		}

		public bool IsMatch(string groupName)
		{
			return IsMatch(groupName, null);
		}

		public bool IsMatch(string groupName, string value)
		{
			return this.match != null && this.match.Groups[groupName].Success
				&& (value == null || this.match.Groups[groupName].Value.Equals(value));
		}

		public void Reset()
		{
			this.CommittedLength = 0;
			this.match = TokenRegex.Match(this.Input, this.Start);
		}
	}
	#endregion

	#region Nodes

	#region STNode
	class STNode : IEnumerable<STNode>, ICloneable //: IReferenceNode
	{
		#region Attributes
		/// <summary>
		/// Children nodes
		/// </summary>
		public List<STNode> Children { get; set; }

		/// <summary>
		/// Type of node
		/// </summary>
		public STNodeType Type { get; set; }

		/// <summary>
		/// Start index from an input string
		/// </summary>
		public int Start { get; set; }

		/// <summary>
		/// Length of value in an input string
		/// </summary>
		public int Length { get; set; }
		#endregion // Attributes

		#region Constructors
		/// <summary>
		/// Construct STNode by specified arguments
		/// </summary>
		/// <param name="type">Type of node</param>
		/// <param name="start">Start index from an input string</param>
		/// <param name="len">Length of value in an input string</param>
		public STNode(STNodeType type, int start, int len)
			: this(type, start, len, null)
		{
		}

		/// <summary>
		/// Construct STNode by specified arguments
		/// </summary>
		/// <param name="type">Type of node</param>
		/// <param name="start">Start index from an input string</param>
		/// <param name="len">Length of value in an input string</param>
		/// <param name="children">Children nodes of tree from this node</param>
		public STNode(STNodeType type, int start, int len, List<STNode> children)
		{
			this.Start = start;
			this.Type = type;
			this.Length = len;
			this.Children = children;
		}
		#endregion // Constructors

		#region Dump
		///// <summary>
		///// Dump tree into string
		///// </summary>
		///// <param name="sb">Memory buffer to store the result string</param>
		///// <param name="originalInput">Original input string</param>
		///// <param name="indents">Number of spaces used to indent at beginning of a line</param>
		//public void DumpTree(StringBuilder sb, string originalInput, int indents = 0)
		//{
		//	sb.Append(new string(' ', indents));
		//	sb.Append(this.Type.ToString());
		//	sb.Append(": ");
		//	sb.AppendLine(originalInput.Substring(this.Start, this.Length));

		//	if (Children != null)
		//	{
		//		foreach (var c in Children)
		//		{
		//			indents += 2;
		//			c.DumpTree(sb, originalInput, indents);
		//			indents -= 2;
		//		}
		//	}
		//}
		#endregion // Dump

		/// <summary>
		/// Get child node from specified position
		/// </summary>
		/// <param name="index">zero-based number of child to be get</param>
		/// <returns>child node from specified position</returns>
		public STNode this[int index]
		{
			get
			{
				return this.Children[index];
			}
			set
			{
				this.Children[index] = value;
			}
		}

		#region Enum
		private IEnumerator<STNode> GetEnum()
		{
			if (this.Children != null && this.Children.Count > 0)
			{
				foreach (var child in Children)
				{
					yield return child;
				}
			}
		}

		public IEnumerator<STNode> GetEnumerator()
		{
			return this.GetEnum();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnum();
		}
		#endregion // Enum

		#region Iteration
		internal static void RecursivelyIterate(STNode node, Action<STNode> iterator)
		{
			if (node.Children != null && node.Children.Count > 0)
			{
				foreach (var child in node)
				{
					RecursivelyIterate(child, iterator);
				}
			}
			else
			{
				iterator(node);
			}
		}
		#endregion // Iteration

		public override string ToString()
		{
			return string.Format("Node[{0}-{1}]", this.Start, this.Start + this.Length);
		}

		/// <summary>
		/// Create a copy from this stnode.
		/// </summary>
		/// <returns>The copied stnode.</returns>
		public virtual object Clone()
		{
			var node = new STNode(this.Type, this.Start, this.Length);

			if (this.Children != null && this.Children.Count > 0)
			{
				node.Children = new List<STNode>();

				foreach (var child in this.Children)
				{
					node.Children.Add((STNode)child.Clone());
				}
			}

			return node;
		}
	}
	#endregion // STNode

	#region NumberNode
	class STNumberNode : STNode
	{
		public double Value { get; set; }
		public STNumberNode(double value, int start, int len)
			: base(STNodeType.NUMBER, start, len)
		{
			this.Value = value;
		}

		public override object Clone()
		{
			return new STNumberNode(this.Value, this.Start, this.Length);
		}
	}
	#endregion NumberNode

	#region StringNode
	class STStringNode : STNode
	{
		internal string Text { get; set; }

		public STStringNode(string input, int start, int len)
			: base(STNodeType.STRING, start, len)
		{
			if (input != null)
			{
				if (input[start] == '"' || input[start] == '\'')
				{
					this.Text = input.Substring(start + 1, len - 2);
				}
				else
				{
					this.Text = input.Substring(start, len);
				}
			}
		}

		public override object Clone()
		{
			return new STStringNode(null, this.Start, this.Length) { Text = this.Text };
		}
	}
	#endregion StringNode

	#region IdentifierNode
	class STIdentifierNode : STNode
	{
		public Worksheet Worksheet { get; set; }
	
		public string Identifier { get; set; }

		public STIdentifierNode(Worksheet worksheet, string identifier, int start, int len)
			: base(STNodeType.IDENTIFIER, start, len)
		{
			this.Worksheet = worksheet;
			this.Identifier = identifier;
		}

		public override object Clone()
		{
			return new STIdentifierNode(this.Worksheet, this.Identifier, this.Start, this.Length);
		}
	}
	#endregion IdentifierNode

	#region FunctionNode
	class STFunctionNode : STNode
	{
		public string Name { get; set; }

		public STFunctionNode(string name, int start, int len, List<STNode> children)
			: base(STNodeType.FUNCTION_CALL, start, len, children) { this.Name = name; }

		public override object Clone()
		{
			List<STNode> newChildren = null;

			if (this.Children != null && this.Children.Count > 0)
			{
				newChildren = new List<STNode>(this.Children.Count);
				foreach (var child in this.Children)
				{
					newChildren.Add((STNode)child.Clone());
				}
			}

			return new STFunctionNode(this.Name, this.Start, this.Length, newChildren);
		}
	}
	#endregion FunctionNode

	#region CellNode
	class STCellNode : STNode
	{
		public Worksheet Worksheet { get; set; }

		public CellPosition Position { get; set; }

		public STCellNode(Worksheet worksheet, CellPosition pos, STNodeType type, int start, int len)
			: base(type, start, len)
		{
			this.Worksheet = worksheet;
			this.Position = pos;
		}

		public override object Clone()
		{
			return new STCellNode(this.Worksheet, this.Position, this.Type, this.Start, this.Length);
		}
	}
	#endregion CellNode

	#region RangeNode
	class STRangeNode : STNode
	{
		public Worksheet Worksheet { get; set; }
	
		public RangePosition Range { get; set; }

		public STRangeNode(Worksheet worksheet, RangePosition range, int start, int len)
			: base(STNodeType.RANGE, start, len)
		{
			this.Worksheet = worksheet;
			this.Range = range;
		}

		public override object Clone()
		{
			return new STRangeNode(this.Worksheet, this.Range, this.Start, this.Length);
		}
	}
	#endregion RangeNode

	#region ValueNode
	class STValueNode : STNode
	{
		public FormulaValue Value { get; set; }

		public STValueNode(FormulaValue value)
			: base(STNodeType._FORMULA_VALUE, 0, 0)
		{
			this.Value = value;
		}

		public override object Clone()
		{
			return new STValueNode(this.Value);
		}
	}
	#endregion ValueNode

	/// <summary>
	/// Determine the kind of type for node in a tree.
	/// </summary>
	enum STNodeType
	{
		NONE,

		CONNECT, UNION_RANGES, INTERSECTION,

		NUMBER,
		STRING,
		TRUE, FALSE,
		IDENTIFIER, SHEET_NAME,
		RANGE, CELL, 
		FUNCTION_CALL, FUN_EXT_DOT,

		ADD, SUB, MUL, DIV, POW,

		EQUALS, NOT_EQUALS,
		GREAT_THAN, LESS_THAN,
		GREAT_EQUALS, LESS_EQUALS,

		UNARY_MINUS, UNARY_PERCENT,

		SUB_EXPR,
		_FORMULA_VALUE,
	}

	#endregion Nodes

	#region FormulaParseException
	/// <summary>
	/// Exception thrown when any errors happen during formula parsing and evaluation.
	/// </summary>
	[Serializable]
	public class FormulaParseException : Exception
	{
		/// <summary>
		/// Zero-based number of character of the position error happened
		/// </summary>
		public int Index { get; private set; }

		/// <summary>
		/// Create exception with specified message and the position in formula where error happens.
		/// </summary>
		/// <param name="message">additional message to describe the error</param>
		/// <param name="index">zero-based number of character of the position error happened</param>
		public FormulaParseException(string message, int index)
			: base(message)
		{
			this.Index = index;
		}
	}
	#endregion // FormulaParseException

	#region FormulaFormatFlag
	enum FormulaFormatFlag : short
	{
		None,

		SpaceBeforeOperator = 0x1,
		SpaceAfterOperator = 0x2,
		SpaceBetweenOperator = SpaceBeforeOperator | SpaceAfterOperator,

		SpaceBeforeParameterList = 0x4,
		SpaceAfterParameterList = 0x8,
		SpaceBetweenParameterList = SpaceBeforeParameterList | SpaceAfterParameterList,

		SpaceBeforeComma = 0x10,
		SpaceAfterComma = 0x20,
		SpaceBetweenComma = SpaceBeforeComma | SpaceAfterComma,

		SpaceBeforeSubExpression = 0x40,
		SpaceAfterSubExpression = 0x80,
		SpaceBetweenSubExpression = SpaceBeforeSubExpression | SpaceAfterSubExpression,

		SpaceAfterFunctionName = 0x100,
		FunctionNameAutoUppercase = 0x200,

		SpaceBeforePercent = 0x400,
		SpaceAfterMinus = 0x800,

		// Predefined styles

		Default = SpaceBetweenOperator | SpaceAfterComma | FunctionNameAutoUppercase,

		Comfortable = SpaceBetweenOperator | SpaceBetweenParameterList | SpaceBetweenComma
			| SpaceBetweenSubExpression | SpaceAfterFunctionName | FunctionNameAutoUppercase
			| SpaceBeforePercent | SpaceAfterMinus,

		Compact = FunctionNameAutoUppercase,
	}
	#endregion // FormulaFormatFlag
}

#endif // FORMULA
