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

namespace unvell.ReoGrid.Demo.Features
{
	public partial class OutlineWithFreezeDemo : UserControl
	{
		public OutlineWithFreezeDemo()
		{
			InitializeComponent();

			var worksheet = grid.CurrentWorksheet;

			for (int i = 1; i < 9; i++)
			{
				worksheet.GroupRows(4, i);
			}
			for (int i = 1; i < 9; i++)
			{
				worksheet.GroupColumns(2, i);
			}

			worksheet.FreezeToCell(5, 5);

			worksheet[2, 2] = "Sample of outlines with frozen cells.";
		}
	}
}
