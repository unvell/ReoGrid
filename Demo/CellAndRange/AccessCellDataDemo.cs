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
	public partial class AccessCellDataDemo : UserControl
	{
		private Worksheet sheet;

		public AccessCellDataDemo()
		{
			InitializeComponent();

			sheet = grid.CurrentWorksheet;

			sheet.SetColumnsWidth(0, 2, 120);

			this.SetAndGetData();
			this.SetAndGetString();
			this.SetAndGetFormula();
			this.SetByCellInstance();
		}

		void SetAndGetData()
		{
			// set data
			sheet["A1"] = 10;

			// get and set data
			sheet["B1"] = sheet["A1"];
		}

		void SetAndGetString()
		{
			sheet["A2"] = "This is a string sample.";
			sheet["B2"] = "String length: " + sheet.GetCellData<string>("A2").Length;
		}

		void SetAndGetFormula()
		{
			sheet["A3"] = "=A1*2";
			sheet["B3"] = "A3 Formula is =" + sheet.GetCellFormula("A3");
		}

		void SetByCellInstance()
		{
			var cell1 = sheet.Cells["A4"];
			cell1.Data = 20;

			var cell2 = sheet.Cells["B4"];
			cell2.Data = cell1.GetData<double>() * 2;
		}

		void SetByIndex()
		{
			sheet[4, 0] = "A5";
			
			var cell = sheet.Cells[4, 1];
			cell.Data = cell.Address;
		}
	}
}
