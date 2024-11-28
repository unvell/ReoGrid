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

using System.Drawing;
using System.Windows.Forms;

namespace unvell.ReoGrid.Demo.Formula
{
	public partial class FormulaDemo : UserControl
	{
		private Worksheet worksheet;

		public FormulaDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;

			worksheet.SetColumnsWidth(1, 4, 100);

			worksheet["B2"] = "Calculation expression with cell references:";

			worksheet["B4"] = new object[] { "Const Value", "Const Value", null, "Formula (=B5+C5)" };
			worksheet["B5"] = new object[] { "10", "20", null, "=B5+C5" };

			worksheet["B7"] = "Function call: ROUND function returns a nearest number with a specified number of digits.";
			worksheet["B8"] = new object[] { "123.64", "=ROUND(B8)", null, "1234.56789", "=ROUND(E8, 2)" };

			worksheet["B10"] = "Range reference: using SUM function";
			worksheet["B11"] = new object[,] {
				{ 1, 2, 3, 4, null, null },
				{ 5, 6, 7, 8, null, "=SUM(B12:E13)" },
			};

			// set 4 cells background color to Yellow
			worksheet.Cells["E5"].Style.BackColor = Color.LightYellow;
			worksheet.Cells["C8"].Style.BackColor = Color.LightYellow;
			worksheet.Cells["F8"].Style.BackColor = Color.LightYellow;
			worksheet.Cells["G12"].Style.BackColor = Color.LightYellow;

			// handle event of focus pos changing to show formula from a cell
			worksheet.FocusPosChanged += (s, e) =>
			{
				var formula = worksheet.GetCellFormula(worksheet.FocusPos);
				txtFormula.Text = string.IsNullOrEmpty(formula) ? "Select a cell which has a formula to be displayed." : formula;
			};

			// handle enter key to update formula for focus cell from formula textbox
			this.txtFormula.KeyDown += (s, e) =>
			{
				if (e.KeyCode == Keys.Enter)
				{
					worksheet[worksheet.FocusPos] = txtFormula.Text;
					txtFormula.SelectAll();
				}
			};
		}
	}
}
