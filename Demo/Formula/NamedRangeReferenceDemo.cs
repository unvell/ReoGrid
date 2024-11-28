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

using unvell.ReoGrid.CellTypes;

namespace unvell.ReoGrid.Demo.Formula
{
	public partial class NamedRangeReferenceDemo : UserControl
	{
		public NamedRangeReferenceDemo()
		{
			InitializeComponent();

			var sheet = grid.CurrentWorksheet;

			sheet.SetColumnsWidth(1, 4, 100);

			sheet[1, 1] = "The red range has been defined as named range: myRange";

			// define a named range
			var myRange = sheet.DefineNamedRange("myRange", "B4:E6");

			// set border style for the range
			myRange.BorderOutside = new RangeBorderStyle
			{
				Color = Color.Red,
				Style = BorderLineStyle.Solid,
			};

			// set data for the range
			myRange.Data = new object[,] {
				{ 1, 2, 3, 4 },
				{ .1, .2, .3, .4 },
				{ "apple", "banana", "orange", "pear" },
			};

			sheet.Ranges["C8:E9"].Data = new object[,] {
				{ "SUM", "COUNT", "AVERAGE" },
				{ "=SUM(myRange)", "=COUNT(myRange)", "=AVERAGE(myRange)" },
			};
		}
	}
}
