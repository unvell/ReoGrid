/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * ReoGrid and ReoGrid Demo project is released under MIT license.
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using unvell.ReoGrid.CellTypes;
using unvell.ReoGrid.Demo.Properties;

namespace unvell.ReoGrid.Demo.CustomCells
{
	public partial class BuiltInTypesDemo : UserControl
	{
		private Worksheet worksheet;

		public BuiltInTypesDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;

			// set default sheet style
			worksheet.SetRangeStyles(RangePosition.EntireRange, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.FontName | PlainStyleFlag.VerticalAlign,
				FontName = "Arial",
				VAlign = ReoGridVerAlign.Middle,
			});

			worksheet.SetSettings(WorksheetSettings.View_ShowGridLine |
				 WorksheetSettings.Edit_DragSelectionToMoveCells, false);
			worksheet.SelectionMode = WorksheetSelectionMode.Cell;
			worksheet.SelectionStyle = WorksheetSelectionStyle.FocusRect;

			var middleStyle = new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.Padding | PlainStyleFlag.HorizontalAlign,
				Padding = new PaddingValue(2),
				HAlign = ReoGridHorAlign.Center,
			};

			var grayTextStyle = new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.TextColor,
				TextColor = Color.DimGray
			};

			worksheet.MergeRange(1, 1, 1, 6);

			worksheet.SetRangeStyles(1, 1, 1, 6, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.TextColor | PlainStyleFlag.FontSize,
				TextColor = Color.DarkGreen,
				FontSize = 18,
			});

			worksheet[1, 1] = "Built-in Cell Bodies";

			worksheet.SetColumnsWidth(1, 1, 100);
			worksheet.SetColumnsWidth(2, 1, 30);
			worksheet.SetColumnsWidth(3, 1, 100);
			worksheet.SetColumnsWidth(6, 2, 65);

			// button
			worksheet.MergeRange(3, 2, 1, 2);
			var btn = new ButtonCell("Hello");
			worksheet[3, 1] = new object[] { "Button: ", btn };
			btn.Click += (s, e) => ShowText("Button clicked.");

			// link
			worksheet.MergeRange(5, 2, 1, 3);
			var link = new HyperlinkCell("http://www.google.com") { AutoNavigate = false };
			worksheet[5, 1] = new object[] { "Hyperlink", link };
			link.Click += (s, e) =>
			{
				try
				{
					RGUtility.OpenFileOrLink(worksheet.GetCellText(5, 2));
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString());
				}
			};

		 // checkbox
			var checkbox = new CheckBoxCell();
			worksheet.SetRangeStyles(7, 2, 1, 1, middleStyle);
			worksheet.SetRangeStyles(8, 2, 1, 1, grayTextStyle);
			worksheet[7, 1] = new object[] { "Check box", checkbox, "Auto destroy after 5 minutes." };
			worksheet[8, 2] = "(Keyboard is also supported to change the status of control)";
			checkbox.CheckChanged += (s, e) => ShowText("Check box switch to " + checkbox.IsChecked.ToString());

			// radio & radio group
			worksheet[10, 1] = "Radio Button";
			worksheet.SetRangeStyles(10, 2, 3, 1, middleStyle);
			var radioGroup = new RadioButtonGroup();
			worksheet[10, 2] = new object[,] {
				{new RadioButtonCell() { RadioGroup = radioGroup }, "Apple"},
				{new RadioButtonCell() { RadioGroup = radioGroup }, "Orange"},
				{new RadioButtonCell() { RadioGroup = radioGroup }, "Banana"}
			};
			radioGroup.RadioButtons.ForEach(rb => rb.CheckChanged += (s, e) =>
				ShowText("Radio button selected: " + worksheet[rb.Cell.Row, rb.Cell.Column + 1]));
			worksheet[10, 2] = true;
			worksheet[13, 2] = "(By adding radio buttons into same RadioGroup to make them toggle each other automatically)";
			worksheet.SetRangeStyles(13, 2, 1, 1, grayTextStyle);

			// dropdown 
			worksheet.MergeRange(15, 2, 1, 3);
			var dropdown = new DropdownListCell("Apple", "Orange", "Banana", "Pear", "Pumpkin", "Cherry", "Coconut");
			worksheet[15, 1] = new object[] { "Dropdown", dropdown };
			worksheet.SetRangeBorders(15, 2, 1, 3, BorderPositions.Outside, RangeBorderStyle.GraySolid);
			dropdown.SelectedItemChanged += (s, e) => ShowText("Dropdown list selected: " + dropdown.SelectedItem);

			// image
			worksheet.MergeRange(2, 6, 5, 2);
			worksheet[2, 6] = new ImageCell(Resources.computer_laptop);

			// information cell
			worksheet.SetRangeBorders(19, 0, 1, 10, BorderPositions.Top, RangeBorderStyle.GraySolid);
		}

		private void ShowText(string text)
		{
			worksheet[19, 0] = text;
		}

		private void chkGridlines_CheckedChanged(object sender, EventArgs e)
		{
			worksheet.SetSettings(WorksheetSettings.View_ShowGridLine, chkGridlines.Checked);
		}

		private void chkSelection_CheckedChanged(object sender, EventArgs e)
		{
			if (chkSelectionNone.Checked)
			{
				worksheet.SelectionMode = WorksheetSelectionMode.None;
			}
			else if (chkSelectionRange.Checked)
			{
				worksheet.SelectionMode = WorksheetSelectionMode.Range;
			}
			else if (chkSelectionCell.Checked)
			{
				worksheet.SelectionMode = WorksheetSelectionMode.Cell;
			}
		}

		private void rdoNormal_CheckedChanged(object sender, EventArgs e)
		{
			if (rdoFocus.Checked)
			{
				worksheet.SelectionStyle = WorksheetSelectionStyle.FocusRect;
			}
			else if (rdoNormal.Checked)
			{
				worksheet.SelectionStyle = WorksheetSelectionStyle.Default;
			}
		}
	}
}
