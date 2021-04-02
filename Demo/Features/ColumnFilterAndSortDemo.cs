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

namespace unvell.ReoGrid.Demo.Features
{
	public partial class ColumnFilterAndSortDemo : UserControl
	{
		private Worksheet worksheet;

		public ColumnFilterAndSortDemo()
		{
			InitializeComponent();

			// get current worksheet
			worksheet = grid.CurrentWorksheet;

			// load data from csv file
			worksheet.LoadCSV("_Templates\\csv\\zip_code_sample.csv");

			// create filter and sort user interface, from A column to O column
			worksheet.CreateColumnFilter("A", "O");
		}

	}
}
