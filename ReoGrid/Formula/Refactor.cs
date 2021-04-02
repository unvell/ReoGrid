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
using System.Linq;
using System.Text;

using unvell.ReoGrid.Formula;

namespace unvell.ReoGrid
{
	partial class Worksheet
	{
	}
}

namespace unvell.ReoGrid.Formula
{
	class FormulaRefactor
	{
		#region Generator
		/// <summary>
		/// Generate formula from a syntax tree in memory
		/// </summary>
		/// <param name="node">root node of syntax tree used to generate formula</param>
		/// <returns>formula generated from a syntax tree</returns>
		internal static string Generate(string input, STNode node)
		{
			return Generate(input, node, FormulaFormatFlag.Default);
		}

		/// <summary>
		/// Generate formula form a syntax tree in memory by specified format styles
		/// </summary>
		/// <param name="node">root node of syntax tree used to generate formula</param>
		/// <param name="flag">generating format style flags</param>
		/// <returns>formula generated from a syntax tree</returns>
		internal static string Generate(string input, STNode node, FormulaFormatFlag flag)
		{
			StringBuilder sb = new StringBuilder();
			GenerateExpression(sb, input, node, flag);
			if (sb[sb.Length - 1] == ' ') sb.Remove(sb.Length - 1, 1);
			return sb.ToString();
		}

		private static void GenerateExpression(StringBuilder sb, string input, STNode node, FormulaFormatFlag flag)
		{
			switch (node.Type)
			{
				default:
					sb.Append(node.ToString());
					break;

				case STNodeType.CELL:
					sb.Append(((STCellNode)node).Position.ToAddress());
					break;

				case STNodeType.RANGE:
					sb.Append(((STRangeNode)node).Range.ToAddress());
					break;

				case STNodeType.NUMBER:
					sb.Append(((STNumberNode)node).Value);
					break;

				case STNodeType.STRING:
					sb.Append(((STStringNode)node).Text);
					break;

				case STNodeType.IDENTIFIER:
					sb.Append(((STIdentifierNode)node).Identifier);
					break;

				case STNodeType.ADD: GenerateOperatorString(sb, input, "+", node, flag); break;
				case STNodeType.SUB: GenerateOperatorString(sb, input, "-", node, flag); break;
				case STNodeType.MUL: GenerateOperatorString(sb, input, "*", node, flag); break;
				case STNodeType.DIV: GenerateOperatorString(sb, input, "/", node, flag); break;
				case STNodeType.POW: GenerateOperatorString(sb, input, "^", node, flag); break;
				case STNodeType.GREAT_THAN: GenerateOperatorString(sb, input, ">", node, flag); break;
				case STNodeType.GREAT_EQUALS: GenerateOperatorString(sb, input, ">=", node, flag); break;
				case STNodeType.LESS_THAN: GenerateOperatorString(sb, input, "<", node, flag); break;
				case STNodeType.LESS_EQUALS: GenerateOperatorString(sb, input, "<=", node, flag); break;
				case STNodeType.EQUALS: GenerateOperatorString(sb, input, "=", node, flag); break;
				case STNodeType.CONNECT: GenerateOperatorString(sb, input, "&", node, flag); break;

				case STNodeType.UNARY_PERCENT:
					GenerateExpression(sb, input, node.Children[0], flag);
					AddSpaceIfFlag(sb, flag, FormulaFormatFlag.SpaceBeforePercent);
					sb.Append('%');
					break;

				case STNodeType.FUNCTION_CALL:
					var funNode = (STFunctionNode)node;
					string funName = funNode.Name;
					if ((flag & FormulaFormatFlag.FunctionNameAutoUppercase) == FormulaFormatFlag.FunctionNameAutoUppercase)
						sb.Append(funName.ToUpper());
					else
						sb.Append(funName);
					AddSpaceIfFlag(sb, flag, FormulaFormatFlag.SpaceAfterFunctionName);
					sb.Append('(');
					AddSpaceIfFlag(sb, flag, FormulaFormatFlag.SpaceBeforeParameterList);
					for (int i = 0; i < node.Children.Count; i++)
					{
						if (i > 0)
						{
							AddSpaceIfFlag(sb, flag, FormulaFormatFlag.SpaceBeforeComma);
							sb.Append(',');
							AddSpaceIfFlag(sb, flag, FormulaFormatFlag.SpaceAfterComma);
						}
						GenerateExpression(sb, input, node.Children[i], flag);
					}
					AddSpaceIfFlag(sb, flag, FormulaFormatFlag.SpaceAfterParameterList);
					sb.Append(')');
					break;

				case STNodeType.SUB_EXPR:
					sb.Append('(');
					AddSpaceIfFlag(sb, flag, FormulaFormatFlag.SpaceBeforeSubExpression);
					GenerateExpression(sb, input, node.Children[0], flag);
					AddSpaceIfFlag(sb, flag, FormulaFormatFlag.SpaceAfterSubExpression);
					sb.Append(')');
					break;

				case STNodeType.UNARY_MINUS:
					sb.Append('-');
					AddSpaceIfFlag(sb, flag, FormulaFormatFlag.SpaceAfterMinus);
					GenerateExpression(sb, input, node.Children[0], flag);
					break;
			}
		}

		private static void AddSpaceIfFlag(StringBuilder sb, FormulaFormatFlag flag, FormulaFormatFlag target)
		{
			if ((flag & target) == target && (sb.Length == 0 || sb[sb.Length - 1] != ' '))
			{
				sb.Append(' ');
			}
		}

		private static void GenerateOperatorString(StringBuilder sb, string input, string op, STNode node, FormulaFormatFlag flag)
		{
			var left = node.Children[0];
			var right = node.Children[1];

			GenerateExpression(sb, input, left, flag);
			if ((flag & FormulaFormatFlag.SpaceBeforeOperator) == FormulaFormatFlag.SpaceBeforeOperator) sb.Append(' ');
			sb.Append(op);
			if ((flag & FormulaFormatFlag.SpaceAfterOperator) == FormulaFormatFlag.SpaceAfterOperator) sb.Append(' ');
			GenerateExpression(sb, input, right, flag);
		}
		#endregion // Generator

		#region Reuse
		internal static void Reuse(Worksheet sheet, CellPosition fromPosition, RangePosition toRange)
		{
			fromPosition = sheet.FixPos(fromPosition);
			toRange = sheet.FixRange(toRange);

			var cell = sheet.cells[fromPosition.Row, fromPosition.Col];

			#region Arguments Check
			if (cell == null
				|| string.IsNullOrEmpty(cell.InnerFormula)
				|| cell.FormulaTree == null)
			{
				throw new InvalidOperationException("cannot found formula from specified position, try reset formula for the cell again");
			}

			if (cell.formulaStatus != Formula.FormulaStatus.Normal)
			{
				throw new InvalidOperationException("formula in specified cell contains errors, correct the formula firstly");
			}

			if (toRange.Contains(fromPosition))
			{
				throw new ArgumentException("toRange should not contain the position of the formula to be reused");
			}
			#endregion // Arguments Check

			var rs = new ReplacableString(cell.InnerFormula);
			STNode node = cell.FormulaTree;

			Stack<List<Cell>> dirtyCells = new Stack<List<Cell>>();

			for (int r = toRange.Row; r <= toRange.EndRow; r++)
			{
				for (int c = toRange.Col; c <= toRange.EndCol;)
				{
					var toCell = sheet.CreateAndGetCell(r, c);

					if (toCell.Colspan <= 0)
					{
						c++;
						continue;
					}

					FormulaRefactor.CopyFormula(fromPosition, node, toCell, rs, dirtyCells);
					
					c += cell.Colspan;
				}
			}
		}

		internal static void CopyFormula(CellPosition fromPosition, STNode fromNode, Cell toCell, 
			ReplacableString rs, Stack<List<Cell>> dirtyCells)
		{
			var sheet = toCell.Worksheet;

			STNode node2 = (STNode)fromNode.Clone();

			int r = toCell.Row;
			int c = toCell.Column;

			rs.Restore();

			#region Rebuilt Formula
			STNode.RecursivelyIterate(node2, n =>
			{
				switch (n.Type)
				{
					case STNodeType.CELL:
						#region Cell Offset
						{
							var refCellNode = (STCellNode)n;
							var newPos = refCellNode.Position;

							#region Calc Offset
							// B2: =A1
							// B3: =A2
							if (newPos.RowProperty == PositionProperty.Relative)
							{
								newPos.Row += r - fromPosition.Row;
							}

							if (newPos.ColumnProperty == PositionProperty.Relative)
							{
								newPos.Col += c - fromPosition.Col;
							}
							#endregion // Calc Offset

							if (newPos.Row < 0 || newPos.Col < 0
								|| newPos.Row >= sheet.rows.Count || newPos.Col >= sheet.cols.Count)
							{
								toCell.formulaStatus = FormulaStatus.InvalidReference;
							}

							refCellNode.Position = newPos;

							n.Start += rs.Offset;
							int diff = rs.Replace(n.Start, n.Length, newPos.ToAddress());
							n.Length += diff;
						}
						break;
					#endregion // Cell Offset

					case STNodeType.RANGE:
						#region Range Offset
						{
							var refRangeNode = (STRangeNode)n;
							var newRange = refRangeNode.Range;

							#region Calc Offset
							int diffRow = r - fromPosition.Row;
							int diffCol = c - fromPosition.Col;

							if (newRange.StartRowProperty == PositionProperty.Relative)
							{
								newRange.Row += diffRow;
							}

							if (newRange.StartColumnProperty == PositionProperty.Relative)
							{
								newRange.Col += diffCol;
							}

							//if (newRange.EndRowProperty == PositionProperty.Relative)
							//{
							//	newRange.EndRow += diffRow;
							//}

							//if (newRange.EndColumnProperty == PositionProperty.Relative)
							//{
							//	newRange.EndCol += diffCol;
							//}
							#endregion // Calc Offset

							if (newRange.Row < 0 || newRange.Col < 0
								|| newRange.Row >= sheet.rows.Count || newRange.Col >= sheet.cols.Count
								|| newRange.EndRow < 0 || newRange.EndCol < 0
								|| newRange.EndRow >= sheet.rows.Count || newRange.EndCol >= sheet.cols.Count)
							{
								toCell.formulaStatus = FormulaStatus.InvalidReference;
							}

							refRangeNode.Range = newRange;

							n.Start += rs.Offset;
							int diff = rs.Replace(n.Start, n.Length, newRange.ToAddress());
							n.Length += diff;
						}
						break;
						#endregion // Range Offset
				}
			});
			#endregion // Rebuilt Formula

			#region Update To New Cell

			toCell.InnerFormula = rs.ToString();

#if FORMULA_FORMAT
					toCell.InnerFormula = Generate(cell.InnerFormula, node2);
#endif // FORMULA_FORMAT

			sheet.SetCellFormula(toCell, node2);

			if (toCell.formulaStatus == FormulaStatus.Normal)
			{
				sheet.RecalcCell(toCell, dirtyCells);
			}
			#endregion // Update To New Cell

		}
		#endregion // Reuse
	}

	class ReplacableString
	{
		private string original;

		private StringBuilder sb;

		internal ReplacableString(string str)
		{
			this.original = str;

			this.Restore();
		}

		private int offset = 0;

		internal int Offset { get { return this.offset; } }

		public int Replace(int from, int len, string newstr)
		{
			this.sb.Remove(from, len);
			this.sb.Insert(from, newstr);

			int diff = newstr.Length - len;
			this.offset += diff;
			return diff;
		}

		public override string ToString()
		{
			return sb.ToString();
		}

		public void Restore()
		{
			if (this.sb == null)
			{
				this.sb = new StringBuilder(this.original, 256);
			}
			else
			{
				this.sb.Length = 0;
				this.sb.Append(this.original);
			}
			this.offset = 0;
		}
	}
}

#endif // FORMULA