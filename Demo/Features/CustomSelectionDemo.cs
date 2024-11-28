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

using unvell.ReoGrid.Interaction;

namespace unvell.ReoGrid.Demo.Features
{
	public partial class CustomSelectionDemo : UserControl
	{
		private Worksheet worksheet;

		public CustomSelectionDemo()
		{
			InitializeComponent();

			string path = "_Templates\\RGF\\order_sample.rgf";

			// disable multiple sheet switching UI
			this.grid.SetSettings(WorkbookSettings.View_ShowSheetTabControl, false);
	
			// get default worksheet
			this.worksheet = this.grid.CurrentWorksheet;

			// load template from file
			this.worksheet.Load(path);
			
			// disable dragging mouse to move range feature
			this.worksheet.SetSettings(WorksheetSettings.Edit_DragSelectionToMoveCells, false);

			// handle the event before selection range changing
			this.worksheet.BeforeSelectionRangeChange += worksheet_BeforeSelectionRangeChange;

			// handle user key
			this.worksheet.BeforeCellKeyDown += worksheet_BeforeCellKeyDown;

			// set all valid ranges background
			foreach (var addr in this.validRanges)
			{
				this.worksheet.SetRangeStyles(addr, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.BackColor,
					BackColor = Color.LightYellow,
				});
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			this.grid.Focus();
		}

		/// <summary>
		/// A list that selection range only be allowed inside
		/// </summary>
		private List<string> validRanges = new List<string>() 
		{
			"A1:A2", "A5:A8", "G2:G3", "A11:A15", "D11:D15", "A18:G18",
			"A21:G35", "A38:D42", "G36:G41", "F46:G46", "A49:G49",
		};

		void worksheet_BeforeSelectionRangeChange(object sender, Events.BeforeSelectionChangeEventArgs e)
		{
			if (chkLimitSelection.Checked)
			{
				// Check the start and end position of the new selection range 
				// Abort operations if any selection position not be included in the list defined above
				e.IsCancelled = !validRanges.Any(vr =>
				{
					var range = new RangePosition(vr);
					return range.Contains(e.SelectionStart) && range.Contains(e.SelectionEnd);
				});
			}
		}

		void worksheet_BeforeCellKeyDown(object sender, Events.BeforeCellKeyDownEventArgs e)
		{
			// we just customize the key process if checkbox is enabled
			if (chkLimitSelection.Checked)
			{
				// redefine Tab key to switch between different input block
				if (chkTabToNextBlock.Checked
					&& (e.KeyCode | KeyCode.Tab) == KeyCode.Tab)
				{
					int index = this.validRanges.FindIndex(vr => new RangePosition(vr).Contains(e.CellPosition));

					index++;
					if (index >= this.validRanges.Count) index = 0;

					this.worksheet.SelectionRange = new RangePosition(validRanges[index]);
				}
				else
				{
					// Check the start and end position of the new selection range 
					// Abort operations if any selection position not be included in the list defined above
					e.IsCancelled = !validRanges.Any(vr => new RangePosition(vr).Contains(e.CellPosition));
				}
			}
		}

		private void chkLimitSelection_CheckedChanged(object sender, EventArgs e)
		{
			if (this.chkLimitSelection.Checked)
			{
				var firstRange = new RangePosition(validRanges[0]);

				// reset selection to make sure that is inside valid ranges
				this.worksheet.SelectionRange = new RangePosition(firstRange.Row, firstRange.Col, 1, 1);
			}

			// Tab-to-Next-Block just available when customize selection is enabled
			this.chkTabToNextBlock.Enabled = this.chkLimitSelection.Checked;
		}
	}
}
