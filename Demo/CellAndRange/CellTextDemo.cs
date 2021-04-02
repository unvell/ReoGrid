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
using unvell.ReoGrid.Graphics;

namespace unvell.ReoGrid.Demo.CellAndRange
{
	public partial class CellTextDemo : UserControl
	{
		private Worksheet sheet;

		public CellTextDemo()
		{
			InitializeComponent();

			sheet = grid.CurrentWorksheet;

			sheet.SetColumnsWidth(1, 1, 200);

			// demo 1
			this.SetCellNormalText();

			// demo 2
			this.AlignText();

			// demo 3
			this.SetNumberFormat();
		}

		/// <summary>
		/// Demo 1: Set normal text
		/// </summary>
		void SetCellNormalText()
		{
			sheet["B2"] = "This is normal text.";
			sheet.Ranges["B2:C2"].BorderOutside = RangeBorderStyle.BlackSolid;
		}

		/// <summary>
		/// Demo 3: Align text to right
		/// </summary>
		void AlignText()
		{
			sheet["B7"] = "Text aligned to right";
			sheet.Cells["B7"].Style.HAlign = ReoGridHorAlign.Right;
		}

		/// <summary>
		/// Demo 4: Set number format
		/// </summary>
		void SetNumberFormat()
		{
			sheet["B9"] = 12345.23456;
			sheet.Cells["B9"].DataFormat = DataFormat.CellDataFormatFlag.Number;
			sheet.Cells["B9"].DataFormatArgs = new DataFormat.NumberDataFormatter.NumberFormatArgs
			{
				DecimalPlaces = 2,
				UseSeparator = true
			};

			sheet["C9"] = "(Numbers align to right automatically)";
		}
	}
}
