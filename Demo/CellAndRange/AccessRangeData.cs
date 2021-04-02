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

namespace unvell.ReoGrid.Demo.CellAndRange
{
	public partial class AccessRangeData : UserControl
	{
		private Worksheet sheet;

		public AccessRangeData()
		{
			InitializeComponent();

			sheet = grid.CurrentWorksheet;

			this.SetRangeFromTwoDimensionalArray();
		}

		void SetRangeFromTwoDimensionalArray()
		{
			sheet["A1"] = new object[,] { 
				{ 1, 2, 3, 4, 5},
				{ "A", "B", "C", "D", "E"},
				{ DateTime.Now, new DateTime(2015, 3, 26), "=A1*2", "=NOW()", null },
			};
		}


	}
}
