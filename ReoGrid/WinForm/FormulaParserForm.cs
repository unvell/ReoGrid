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

#if DEBUG && WINFORM && FORMULA

using System;
using System.Text;
using System.Windows.Forms;

using unvell.ReoGrid;
using unvell.ReoGrid.Formula;

namespace FormulaParserTest
{
#if DEBUG
	public
#endif
	partial class FormulaParserForm : Form
	{
		public IWorkbook Workbook { get; set; }

		public Cell Cell { get; set; }

		public FormulaParserForm(Cell cell)
		{
			this.Cell = cell;
			this.Workbook = cell.Worksheet == null ? null : cell.Worksheet.workbook;

			InitializeComponent();

			txtInput.TextChanged += (s, e) => ConstructSyntaxTree();

			syntaxTree.AfterSelect += syntaxTree_AfterSelect;

			txtRangeAddress.TextChanged += (s, e) => UpdateNodeValueAndGenerateFormula(txtRangeAddress.Text);
			txtCellAddress.TextChanged += (s, e) => UpdateNodeValueAndGenerateFormula(txtCellAddress.Text);
			txtName.TextChanged += (s, e) => UpdateNodeValueAndGenerateFormula(txtName.Text);
			txtConnectLeft.TextChanged += (s, e) => UpdateNodeValueAndGenerateFormula(txtConnectLeft.Text);
			txtConnectRight.TextChanged += (s, e) => UpdateNodeValueAndGenerateFormula(txtConnectRight.Text);
	
			chkBeforeOperator.CheckedChanged += (s, e) => UpdateFlagAndGenerateFormula();
			chkAfterOperator.CheckedChanged += (s, e) => UpdateFlagAndGenerateFormula();

			chkBeforeComma.CheckedChanged += (s, e) => UpdateFlagAndGenerateFormula();
			chkAfterComma.CheckedChanged += (s, e) => UpdateFlagAndGenerateFormula();

			chkBeforeParamList.CheckedChanged += (s, e) => UpdateFlagAndGenerateFormula();
			chkAfterParamList.CheckedChanged += (s, e) => UpdateFlagAndGenerateFormula();
			
			chkBeforeSubExpr.CheckedChanged += (s, e) => UpdateFlagAndGenerateFormula();
			chkAfterSubExpr.CheckedChanged += (s, e) => UpdateFlagAndGenerateFormula();

			chkBeforePercent.CheckedChanged += (s, e) => UpdateFlagAndGenerateFormula();
			chkAfterMinus.CheckedChanged += (s, e) => UpdateFlagAndGenerateFormula();

			chkSpaceAfterFunctionName.CheckedChanged += (s, e) => UpdateFlagAndGenerateFormula();
			chkFunctionNameUppercase.CheckedChanged += (s, e) => UpdateFlagAndGenerateFormula();

			linkClearAll.Click += (s, e) => ClearAllFormatCheck();
			linkRestoreToDefault.Click += (s, e) => SetFormatStylesByFlag(FormulaFormatFlag.Default);
			linkComfortable.Click += (s, e) => SetFormatStylesByFlag(FormulaFormatFlag.Comfortable);
			linkCompact.Click += (s, e) => SetFormatStylesByFlag(FormulaFormatFlag.Compact);

			syntaxTree.MouseDown += (s, e) => syntaxTree.SelectedNode = syntaxTree.GetNodeAt(e.Location);
		
		}

		GroupBox group = null;

		private STNode currentSyntaxTree;

		void syntaxTree_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (group != null)
			{
				group.Visible = false;
			}

			if (e.Node.Tag is STNode)
			{
				var node = (STNode)e.Node.Tag;

				txtInput.SelectionStart = node.Start;
				txtInput.SelectionLength = node.Length;

				isUpdatingUI = true;

				switch (node.Type)
				{
					case STNodeType.RANGE:
						{
							group = groupRange;
							txtRangeAddress.Text = ((STRangeNode)node).Range.ToAddress();
						}
						break;
					case STNodeType.CELL:
						{
							group = groupCell;
							txtCellAddress.Text = ((STCellNode)node).Position.ToAddress();
						}
						break;
					case STNodeType.FUNCTION_CALL:
						{
							var funNode = (STFunctionNode)node;
							group = groupFunCall;
							txtFunName.Text = funNode.Name;
							lstArgs.Items.Clear();
							for (int i = 1; i < node.Children.Count; i++)
							{
								lstArgs.Items.Add(node.Children[i].ToString());
							}
						}
						break;
					case STNodeType.IDENTIFIER:
						{
							group = groupName;
							txtName.Text = ((STIdentifierNode)node).Identifier;
						}
						break;
					case STNodeType.CONNECT:
						{
							group = groupConnect;
							txtConnectLeft.Text = node.Children[0].ToString();
							txtConnectRight.Text = node.Children[1].ToString();
						}
						break;

					default: group = null;  break;
				}

				isUpdatingUI = false;

				if (group != null)
				{
					group.Dock = DockStyle.Fill;
					group.Visible = true;
					group.BringToFront();
				}
			}
		}

		private void UpdateNodeValueAndGenerateFormula(string value)
		{
			//if (!isUpdatingUI && syntaxTree.SelectedNode.Tag is STNode)
			//{
			//	var node = (STNode)syntaxTree.SelectedNode.Tag;
			//	node.Value = (STnode) value;
			//	syntaxTree.SelectedNode.ForeColor = Color.Green;
			//	GenerateFormula();
			//}
		}

		private void ConstructSyntaxTree()
		{
			syntaxTree.Nodes.Clear();
			txtErrorMessage.Text = string.Empty;
			txtGeneratedFormula.Text = string.Empty;

			try
			{
				currentSyntaxTree = Parser.Parse(this.Workbook as Workbook, this.Cell, txtInput.Text);

				if (currentSyntaxTree != null)
				{
					syntaxTree.Nodes.Add(currentSyntaxTree.Type.ToString() 
						+ ": " + currentSyntaxTree.ToString()).Tag = currentSyntaxTree;

					if (currentSyntaxTree.Children != null)
					{
						foreach (var node in currentSyntaxTree.Children)
						{
							AddTreeNode(syntaxTree.Nodes[0], node);
						}
					}

					syntaxTree.ExpandAll();
				}

				GenerateFormula();
			}
			catch(FormulaParseException ex)
			{
				StringBuilder sb = new StringBuilder();
				//sb.AppendLine(txtInput.Text);
				sb.Append(new string(' ', ex.Index));
				sb.AppendLine("^");
				sb.Append(ex.Message);

				txtErrorMessage.Text = sb.ToString();

				currentSyntaxTree = null;
			}
		}

		private void AddTreeNode(TreeNode parent, STNode node)
		{
			var newNode = parent.Nodes.Add(node.Type.ToString() + ": " + node.ToString());
			newNode.Tag = node;

			if (node.Children != null)
			{
				foreach (var child in node.Children)
				{
					AddTreeNode(newNode, child);
				}
			}
		}

		private void GenerateFormula()
		{
			if (currentSyntaxTree != null)
			{
				FormulaFormatFlag flag = GetCurrentFormatSetting();

				string formula = FormulaRefactor.Generate(txtGeneratedFormula.Text, currentSyntaxTree, flag);

				txtGeneratedFormula.Text = formula;
			}
			else
			{
				txtGeneratedFormula.Text = string.Empty;
			}
		}

		private FormulaFormatFlag GetCurrentFormatSetting()
		{
			FormulaFormatFlag flag = FormulaFormatFlag.None;

			if (chkBeforeOperator.Checked) flag |= FormulaFormatFlag.SpaceBeforeOperator;
			if (chkAfterOperator.Checked) flag |= FormulaFormatFlag.SpaceAfterOperator;
		
			if (chkBeforeComma.Checked) flag |= FormulaFormatFlag.SpaceBeforeComma;
			if (chkAfterComma.Checked) flag |= FormulaFormatFlag.SpaceAfterComma;
		
			if (chkBeforeParamList.Checked) flag |= FormulaFormatFlag.SpaceBeforeParameterList;
			if (chkAfterParamList.Checked) flag |= FormulaFormatFlag.SpaceAfterParameterList;

			if (chkBeforeSubExpr.Checked) flag |= FormulaFormatFlag.SpaceBeforeSubExpression;
			if (chkAfterSubExpr.Checked) flag |= FormulaFormatFlag.SpaceAfterSubExpression;

			if (chkBeforePercent.Checked) flag |= FormulaFormatFlag.SpaceBeforePercent;
			if (chkAfterMinus.Checked) flag |= FormulaFormatFlag.SpaceAfterMinus;

			if (chkFunctionNameUppercase.Checked) flag |= FormulaFormatFlag.FunctionNameAutoUppercase;
			if (chkSpaceAfterFunctionName.Checked) flag |= FormulaFormatFlag.SpaceAfterFunctionName;

			return flag;
		}

		private void UpdateFlagAndGenerateFormula()
		{
			// if now updating the value of UI controls, do not generate formula
			if (!this.isUpdatingUI)
			{
				GenerateFormula();
			}
		}

		private bool isUpdatingUI = false;

		private void ClearAllFormatCheck()
		{
			isUpdatingUI = true;

			chkBeforeOperator.Checked = false;
			chkAfterOperator.Checked = false;

			chkBeforeComma.Checked = false;
			chkAfterComma.Checked = false;

			chkBeforeParamList.Checked = false;
			chkAfterParamList.Checked = false;

			chkBeforeSubExpr.Checked = false;
			chkAfterSubExpr.Checked = false;

			chkBeforePercent.Checked = false;
			chkAfterMinus.Checked = false;

			chkFunctionNameUppercase.Checked = false;
			chkSpaceAfterFunctionName.Checked = false;

			isUpdatingUI = false;

			GenerateFormula();
		}

		private void SetFormatStylesByFlag(FormulaFormatFlag flag)
		{
			isUpdatingUI = true;

			chkBeforeOperator.Checked = flag.Has(FormulaFormatFlag.SpaceBeforeOperator);
			chkAfterOperator.Checked = flag.Has(FormulaFormatFlag.SpaceAfterOperator);

			chkBeforeComma.Checked = flag.Has(FormulaFormatFlag.SpaceBeforeComma);
			chkAfterComma.Checked = flag.Has(FormulaFormatFlag.SpaceAfterComma);

			chkBeforeParamList.Checked = flag.Has(FormulaFormatFlag.SpaceBeforeParameterList);
			chkAfterParamList.Checked = flag.Has(FormulaFormatFlag.SpaceAfterParameterList);

			chkBeforeSubExpr.Checked = flag.Has(FormulaFormatFlag.SpaceBeforeSubExpression);
			chkAfterSubExpr.Checked = flag.Has(FormulaFormatFlag.SpaceAfterSubExpression);

			chkBeforePercent.Checked = flag.Has(FormulaFormatFlag.SpaceBeforePercent);
			chkAfterMinus.Checked = flag.Has(FormulaFormatFlag.SpaceAfterMinus);

			chkFunctionNameUppercase.Checked = flag.Has(FormulaFormatFlag.FunctionNameAutoUppercase);
			chkSpaceAfterFunctionName.Checked = flag.Has(FormulaFormatFlag.SpaceAfterFunctionName);

			isUpdatingUI = false;

			GenerateFormula();
		}

		protected override void OnLoad(EventArgs e)
		{
			SetFormatStylesByFlag(FormulaFormatFlag.Default);

			ConstructSyntaxTree();
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			txtInput.Focus();
		}

		public static void TestRef(IWorkbook wb)
		{
			var sheet = wb.Worksheets[0];

			sheet["A8:A9"] = new object[] { 1 };
			sheet.AutoFillSerial("A8:A8", "A9:A11");

			sheet["B8:B9"] = new object[] { 1, 2 };
			sheet.AutoFillSerial("B8:B8", "B10:B11");
			
			sheet.Cells["C8"].Formula = "(A8+SUM(A8:B8))*2";

			sheet.AutoFillSerial("C8", "C9:C11");

			//FormulaRefactor.Reuse(sheet, new ReoGridPos("B1"), new ReoGridRange("B2:D5"));
		}
	}

	static class FormatFormuleFlagExtension
	{
		public static bool Has(this FormulaFormatFlag flag, FormulaFormatFlag target)
		{
			return (flag & target) == target;
		}
	}

}

#endif // DEBUG && WINFORM && FORMULA
