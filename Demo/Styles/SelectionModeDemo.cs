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

namespace unvell.ReoGrid.Demo.Styles
{
	public partial class SelectionModeDemo : UserControl
	{
		private Worksheet worksheet;

		public SelectionModeDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;

			// resize spreadsheet
			worksheet.Resize(50, 7);

			// change header width
			worksheet.ColumnHeaders[0].Width = 100;
			worksheet.ColumnHeaders[1].Width = 500;

			// fill data
			worksheet["A1"] = new object[,] {
				{ "Function", "Description" },
				{ "INDIRECT", "Returns the reference specified by a text string." },
				{ "ADDRESS", "You can use the ADDRESS function to obtain the address of a cell in a worksheet." },
				{ "COUNT", "The COUNT function counts the number of cells that contain numbers, and counts numbers within the list of arguments." },
				{ "COUNTIF", "The COUNTIF function counts the number of cells within a range that meet a single criterion that you specify." },
				{ "SUM", "Returns the sum of a set of values contained in a specified field on a query." },
				{ "SUMIF", "You use the SUMIF function to sum the values in a range that meet criteria that you specify." },
				{ "AVERAGE", "Returns the average (arithmetic mean) of the arguments." },
				{ "LOOKUP", "The LOOKUP function returns a value either from a one-row or one-column range or from an array." },
				{ "ROWS", "Returns the number of rows in a reference or array." },
				{ "COLUMNS", "Returns the number of columns in an array or reference." },
				{ "INDEX", "Returns a value or the reference to a value from within a table or range." },
				{ "CEILING", "Returns number rounded up, away from zero, to the nearest multiple of significance." },
				{ "LEN", "Returns the number of characters in a text string." },
				{ "LENB", "Returns the number of bytes used to represent the characters in a text string." },
				{ "ROUND", "Round a number to the nearest number." },
			};

			// set first row style (-1 means entire row or column)
			worksheet.SetRangeStyles(0, 0, 1, -1, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.BackColor | PlainStyleFlag.TextColor | PlainStyleFlag.FontStyleBold,

				BackColor = Color.DarkSeaGreen,
				TextColor = Color.DarkSlateBlue,
				Bold = true,
			});

			// set cursor style for cells to default cursor
			worksheet.SelectionMode = WorksheetSelectionMode.Row;

			// disable cell text overflow
			worksheet.SetSettings(WorksheetSettings.View_AllowCellTextOverflow, false);
		}
	}
}
