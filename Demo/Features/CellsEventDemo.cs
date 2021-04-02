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
using System.Linq;
using System.Text;
using System.Windows.Forms;

using unvell.ReoGrid.Events;

namespace unvell.ReoGrid.Demo.Features
{
	public partial class CellsEventDemo : UserControl
	{
		private Worksheet worksheet;

		public CellsEventDemo()
		{
			InitializeComponent();

			worksheet = grid.CurrentWorksheet;

			chkEnter.CheckedChanged += UpdateEventBind;
			chkLeave.CheckedChanged += UpdateEventBind;
			chkDown.CheckedChanged += UpdateEventBind;
			chkUp.CheckedChanged += UpdateEventBind;
			chkMove.CheckedChanged += UpdateEventBind;
			chkHoverHighlight.CheckedChanged += UpdateEventBind;
			chkBackground.CheckedChanged += UpdateEventBind;

			chkGridLines.CheckedChanged += (s, e) => worksheet.SetSettings(
				WorksheetSettings.View_ShowGridLine, chkGridLines.Checked);

			chkSelectionRect.CheckedChanged += (s, e) => worksheet.SelectionMode =
				chkSelectionRect.Checked ? WorksheetSelectionMode.Range : WorksheetSelectionMode.None;
		}

		private void UpdateEventBind(object sender, EventArgs e)
		{
			worksheet.CellMouseEnter -= Grid_CellMouseEnter;
			worksheet.CellMouseLeave -= Grid_CellMouseLeave;
			worksheet.CellMouseDown -= Grid_CellMouseDown;
			worksheet.CellMouseUp -= Grid_CellMouseUp;
			worksheet.CellMouseMove -= Grid_CellMouseMove;

			if (chkEnter.Checked || chkHoverHighlight.Checked || chkBackground.Checked) worksheet.CellMouseEnter += Grid_CellMouseEnter;
			if (chkLeave.Checked || chkHoverHighlight.Checked || chkBackground.Checked) worksheet.CellMouseLeave += Grid_CellMouseLeave;
			if (chkDown.Checked) worksheet.CellMouseDown += Grid_CellMouseDown;
			if (chkUp.Checked) worksheet.CellMouseUp += Grid_CellMouseUp;
			if (chkMove.Checked) worksheet.CellMouseMove += Grid_CellMouseMove;
		}

		private void Grid_CellMouseEnter(object sender, CellMouseEventArgs e)
		{
			if(chkEnter.Checked) Log("cell mouse enter: " + e.CellPosition);

			if (chkHoverHighlight.Checked)
			{
				worksheet.SetRangeBorders(new RangePosition(e.CellPosition, e.CellPosition), BorderPositions.Outside, RangeBorderStyle.GraySolid);
			}

			if (chkBackground.Checked)
			{
				worksheet.SetRangeStyles(new RangePosition(e.CellPosition, e.CellPosition), new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.BackColor,
					BackColor = Color.Silver,
				});
			}
		}

		private void Grid_CellMouseLeave(object sender, CellMouseEventArgs e)
		{
			if (chkLeave.Checked) Log("cell mouse leave: " + e.CellPosition);

			if (chkHoverHighlight.Checked)
			{
				worksheet.RemoveRangeBorders(new RangePosition(e.CellPosition, e.CellPosition), BorderPositions.Outside);
			}

			if (chkBackground.Checked)
			{
				worksheet.RemoveRangeStyles(new RangePosition(e.CellPosition, e.CellPosition), PlainStyleFlag.BackColor);
			}
		}

		private void Grid_CellMouseDown(object sender, CellMouseEventArgs e)
		{
			Log("cell mouse down: " + e.CellPosition + ", " + e.RelativePosition);
		}
		private void Grid_CellMouseUp(object sender, CellMouseEventArgs e)
		{
			Log("cell mouse up: " + e.CellPosition + ", " + e.RelativePosition);
		}
		private void Grid_CellMouseMove(object sender, CellMouseEventArgs e)
		{
			Log("cell mouse move: " + e.CellPosition + ", " + e.RelativePosition);
		}

		private void Log(string msg)
		{
			listbox1.Items.Add(msg);
			listbox1.SelectedIndex = listbox1.Items.Count - 1;
		}
	}
}
