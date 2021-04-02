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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace unvell.ReoGrid.Demo.Welcome
{
	public partial class V100WhatsNew : UserControl
	{
		public V100WhatsNew()
		{
			InitializeComponent();

			// load Excel template file
			this.reoGridControl.Load("_Templates\\Excel\\Welcome-200.xlsx");

			var sheet1 = this.reoGridControl.Worksheets[0];

			// iterate to set hyperlink cells type
			sheet1.IterateCells("C15:J23", (row, col, cell) =>
			{
				if (cell.DisplayText.StartsWith("http:")
					|| cell.DisplayText.StartsWith("mailto:"))
				{
					cell.Body = new CellTypes.HyperlinkCell();
				}

				return true;
			});
		}
	}
}
