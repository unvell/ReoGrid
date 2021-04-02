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

namespace unvell.ReoGrid.Demo.Scripts
{
	public partial class RunScriptDemo : UserControl
	{
		private Worksheet worksheet;

		public RunScriptDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;

			worksheet.SetRows(14);
			worksheet.SetCols(6);

			for (int r = 0; r < 10; r++)
			{
				for (int c = 0; c < 5; c++)
				{
					worksheet[r, c] = (r + 1) * (c + 1);
				}
			}
		}

		private void btnHelloworld_Click(object sender, EventArgs e)
		{
			this.grid.RunScript("alert('hello world');");
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var script = @"

var sheet = workbook.currentWorksheet;

var range = sheet.selection.range;

alert('current selection: ' + range);

";

			this.grid.RunScript(script);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			var script = @"

var sheet = workbook.currentWorksheet;

var pos = sheet.selection.pos;

var cell = sheet.getCell(pos);

cell.style.backgroundColor = 'darkgreen';

";
			this.grid.RunScript(script);
		}

	}
}
