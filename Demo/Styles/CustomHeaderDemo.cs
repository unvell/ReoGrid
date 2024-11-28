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
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;
using Point = unvell.ReoGrid.Graphics.Point;
using Rectangle = unvell.ReoGrid.Graphics.Rectangle;

namespace unvell.ReoGrid.Demo.Styles
{
	public partial class CustomHeaderDemo : UserControl
	{
		private class CustomHeaderBody : HeaderBody
		{
			public override void OnPaint(CellDrawingContext dc, unvell.ReoGrid.Graphics.Size headerSize)
			{
				dc.Graphics.FillRectangle(HatchStyles.OutlinedDiamond, Color.FromArgb(120, Color.BlueViolet), SolidColor.Transparent, new Rectangle(new Point(0, 0), headerSize));
			}
		}

		private Worksheet worksheet;

		public CustomHeaderDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;

			this.worksheet.Resize(10, 7);

			// set first header to checkbox type
			var checkBoxHeader = this.worksheet.ColumnHeaders[0];
			checkBoxHeader.Text = string.Empty;
			checkBoxHeader.DefaultCellBody = typeof(CheckBoxCell);
			checkBoxHeader.Width = 30;
			checkBoxHeader.Style.HorizontalAlign = ReoGridHorAlign.Center;
			checkBoxHeader.Style.VerticalAlign = ReoGridVerAlign.Middle;
			checkBoxHeader.Style.Padding = new PaddingValue(3);

			// set custom header body
			worksheet.ColumnHeaders[1].Body = new CustomHeaderBody();
			worksheet.RowHeaders[1].Body = new CustomHeaderBody();

			// set other headers
			worksheet.ColumnHeaders[1].Text = "Product";
			worksheet.ColumnHeaders[2].Text = "Unit Price";
			worksheet.ColumnHeaders[3].Text = "Discount";
			worksheet.ColumnHeaders[4].Text = "Quantity";
			worksheet.ColumnHeaders[5].Text = "Extended Price";
			worksheet.ColumnHeaders[6].Text = "Remark";

			// set width for columns (1-4)
			worksheet.SetColumnsWidth(1, 5, 100);

			// enable select only single cell
			worksheet.SelectionMode = WorksheetSelectionMode.Cell;

			// define a region as record range
			var recordRange = worksheet.DefineNamedRange("records", 0, 0, 4, 6);

			// apply the formula using a relative address in order to calculate each rows
			// extended price = (unit price - discount) * quantity
			var extendedPriceFormula = "=( INDIRECT(ADDRESS(ROW(), COLUMN()-3)) "
				+ " - INDIRECT(ADDRESS(ROW(), COLUMN()-2)) )"
				+ " * INDIRECT(ADDRESS(ROW(), COLUMN()-1) )";

			// fill data
			recordRange.Data = new object[,] {
				{ false, "CPU", "230", "0", "1", extendedPriceFormula },
				{ true, "Motherboard", "120", "5", "1", extendedPriceFormula },
				{ false, "Memory", "90", "0", "2", extendedPriceFormula },
				{ false, "VGA", "310", "0", "1", extendedPriceFormula },
			};

			// get formula from selected cell and show it
			worksheet.FocusPosChanged += (s, e) => toolStripStatusLabel1.Text = worksheet.GetCellFormula(worksheet.FocusPos);
		}
	}
}
