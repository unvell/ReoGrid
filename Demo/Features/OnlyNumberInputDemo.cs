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

using System.Windows.Forms;

using unvell.ReoGrid.Events;

namespace unvell.ReoGrid.Demo.Features
{
	public partial class OnlyNumberInputDemo : UserControl
	{
		private Worksheet worksheet;

		public OnlyNumberInputDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;

			worksheet.AfterCellEdit += grid_AfterCellEdit;

			chkErrorPrompt.CheckedChanged += (s, e) =>
			{
				if (!chkOnlyNumeric.Checked) chkOnlyNumeric.Checked = true;
			};
		}

		void grid_AfterCellEdit(object sender, CellAfterEditEventArgs e)
		{
			if (chkOnlyNumeric.Checked)
			{
				if (e.NewData == null || !float.TryParse(e.NewData.ToString(), out var val))
				{
					if (chkErrorPrompt.Checked)
					{
						MessageBox.Show("Only numeric is allowed.");
					}
				
					e.EndReason = EndEditReason.Cancel;
				}
				else
				{
					e.NewData = val;
				}
			}
		}
	}
}
