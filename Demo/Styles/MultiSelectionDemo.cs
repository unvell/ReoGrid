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
	public partial class MultiSelectionDemo : UserControl
	{
		private Worksheet worksheet;

		public MultiSelectionDemo()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			worksheet = grid.CurrentWorksheet;

			// resize spreadsheet
			worksheet.Resize(50, 7);

			// set first column to checkbox column
			worksheet.ColumnHeaders[0].Width = 20;
			worksheet.ColumnHeaders[0].DefaultCellBody = typeof(CheckBoxCell);

			// change header width
			worksheet.ColumnHeaders[1].Width = 100;
			worksheet.ColumnHeaders[2].Width = 500;
			
			// fill data
			worksheet["A1"] = new object[,] {
				{ null, "Function", "Description" },
				{ false, "INDIRECT", "Returns the reference specified by a text string." },
				{ false, "ADDRESS", "You can use the ADDRESS function to obtain the address of a cell in a worksheet." },
				{ false, "COUNT", "The COUNT function counts the number of cells that contain numbers, and counts numbers within the list of arguments." },
				{ false, "COUNTIF", "The COUNTIF function counts the number of cells within a range that meet a single criterion that you specify." },
				{ false, "SUM", "Returns the sum of a set of values contained in a specified field on a query." },
				{ false, "SUMIF", "You use the SUMIF function to sum the values in a range that meet criteria that you specify." },
				{ false, "AVERAGE", "Returns the average (arithmetic mean) of the arguments." },
				{ false, "LOOKUP", "The LOOKUP function returns a value either from a one-row or one-column range or from an array." },
				{ false, "ROWS", "Returns the number of rows in a reference or array." },
				{ false, "COLUMNS", "Returns the number of columns in an array or reference." },
				{ false, "INDEX", "Returns a value or the reference to a value from within a table or range." },
				{ false, "CEILING", "Returns number rounded up, away from zero, to the nearest multiple of significance." },
				{ false, "LEN", "Returns the number of characters in a text string." },
				{ false, "LENB", "Returns the number of bytes used to represent the characters in a text string." },
				{ false, "ROUND", "Round a number to the nearest number." },
				{ null, null, null },
				{ null, "Try click on cells...", null },
			};

			// set first row style (-1 means entire row or column)
			worksheet.SetRangeStyles(0, 0, 1, -1, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.BackColor | PlainStyleFlag.TextColor | PlainStyleFlag.FontStyleBold,

				BackColor = Color.DarkSeaGreen,
				TextColor = Color.DarkSlateBlue,
				Bold = true,
			});

			// disable dragging cells
			worksheet.DisableSettings(WorksheetSettings.Edit_DragSelectionToMoveCells);

			// hide headers
			worksheet.DisableSettings(WorksheetSettings.View_ShowHeaders);

			// disable default selection
			worksheet.SelectionMode = WorksheetSelectionMode.None;

			// handle the mouse down event to set style for selected row
			worksheet.CellMouseUp += Worksheet_CellMouseUp;

			// handle cell data changed to select/unselect all rows
			worksheet.CellDataChanged += Worksheet_CellDataChanged;
		}

		public void SelectRow(int row, bool rowChecked)
		{
			var rowRange = new RangePosition(row, 0, 1, worksheet.Columns);

			// remove back color for row
			worksheet.SetRangeStyles(rowRange, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.BackColor,
				BackColor = rowChecked ? Color.SkyBlue : Color.Empty,
			});

			worksheet[row, 0] = rowChecked;
		}
		
		/// <summary>
		/// A flag to avoid event of event
		/// </summary>
		private bool inUpdate = false;

		private void Worksheet_CellMouseUp(object sender, Events.CellMouseEventArgs e)
		{
			if (inUpdate) return;

			int row = e.CellPosition.Row;

			if (row > 0 && row < 16)
			{
				inUpdate = true;

				bool rowSelected = worksheet.GetCellData<bool>(row, 0);

				if (rowSelected)
				{
					SelectRow(row, false);
				}
				else
				{
					SelectRow(row, true);
				}

				inUpdate = false;
			}
		}

		private void Worksheet_CellDataChanged(object sender, Events.CellEventArgs e)
		{
			if (inUpdate) return;

			var pos = e.Cell.Position;

			inUpdate = true;

			// all checkbox at first column
			if (pos.Col == 0)
			{
				// select all checkbox at first row
				if (pos.Row == 0)
				{
					bool checkboxChecked = e.Cell.GetData<bool>();

					for (int r = 1; r < 16; r++)
					{
						SelectRow(r, checkboxChecked);
					}
				}
				else
				{
					SelectRow(pos.Row, e.Cell.GetData<bool>());
				}
			}

			inUpdate = false;
		}
	}
}
