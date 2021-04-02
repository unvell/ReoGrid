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

using unvell.ReoGrid.Actions;

namespace unvell.ReoGrid.Demo.Features
{
	public partial class SetEditableRangeDemo : UserControl
	{
		public SetEditableRangeDemo()
		{
			InitializeComponent();

			grid.CurrentWorksheet["B2"] = "Click button at right side to set editable range.";
		}

		private void btnSetEditableRange_Click(object sender, EventArgs ee)
		{
			var editableRange = new RangePosition("B5:E6");

			var worksheet = grid.CurrentWorksheet;

			worksheet.SetRangeBorders(editableRange, BorderPositions.Outside, RangeBorderStyle.BlackSolid);

			worksheet["B4"] = "Edit only be allowed in this range:";
			worksheet.BeforeCellEdit += (s, e) => e.IsCancelled = !editableRange.Contains(e.Cell.Position);
		}

	}
}
