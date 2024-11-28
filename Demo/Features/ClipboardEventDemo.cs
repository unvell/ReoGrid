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
	public partial class ClipboardEventDemo : UserControl
	{
		private Worksheet worksheet;

		public ClipboardEventDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;

			worksheet[1, 1] = new object[,]{
				{ "a","b","c"},
				{ "2012", "2013", "2014"},
				{ 2012, 2013, 2014},
			};

			worksheet.BeforePaste += grid_BeforePaste;
		}

		void grid_BeforePaste(object sender, BeforeRangeOperationEventArgs e)
		{
			if (chkPreventPasteEvent.Checked || chkCustomizePaste.Checked)
			{
				e.IsCancelled = true;

				if (chkCustomizePaste.Checked)
				{
					string text = Clipboard.GetText();

					object[,] data = RGUtility.ParseTabbedString(text);

					// set a new range 
					var applyRange = new RangePosition(worksheet.SelectionRange.Row,
						worksheet.SelectionRange.Col,
						data.GetLength(0), data.GetLength(1));

					worksheet.SetRangeData(applyRange, data);

					worksheet.SetRangeStyles(applyRange, new WorksheetRangeStyle
					{
						Flag = PlainStyleFlag.BackAll,
						BackColor = Color.Yellow,
					});
				}
			}
		}
	}
}
