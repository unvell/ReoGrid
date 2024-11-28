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

using unvell.ReoGrid.DataFormat;

namespace unvell.ReoGrid.Demo.Performance
{
	public partial class UpdateDataFormatDemo : UserControl
	{
		private Worksheet worksheet;

		public UpdateDataFormatDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;

			worksheet.SetRows(100);

			for (int r = 0; r < 100; r++)
			{
				for (int c = 0; c < 20; c++)
				{
					worksheet[r, c] = (r + 1) * (c + 1);
				}
			}
		}

		private void btnFormatAsNumber_Click(object sender, EventArgs e)
		{
			worksheet.SetRangeDataFormat(RangePosition.EntireRange, CellDataFormatFlag.Number,
				new NumberDataFormatter.NumberFormatArgs()
				{
					// decimal digit places 0.1234
					DecimalPlaces = 4,
					
					// negative number style: ( 123) 
					NegativeStyle = NumberDataFormatter.NumberNegativeStyle.RedBrackets,

					// use separator: 123,456
					UseSeparator = true,
				});
		}

		private void button2_Click(object sender, EventArgs e)
		{
			worksheet.SetRangeDataFormat(RangePosition.EntireRange, CellDataFormatFlag.DateTime,
				new DateTimeDataFormatter.DateTimeFormatArgs
				{
					// culture
					CultureName = "en-US",

					// pattern
					Format = "yyyy/MM/dd",
				});
		}

		private void button1_Click(object sender, EventArgs e)
		{
			worksheet.SetRangeDataFormat(RangePosition.EntireRange, CellDataFormatFlag.Percent,
				new NumberDataFormatter.NumberFormatArgs
				{
					// decimal digit places
					DecimalPlaces = 2,
				});
		}

		private void button4_Click(object sender, EventArgs e)
		{
			worksheet.SetRangeDataFormat(RangePosition.EntireRange, CellDataFormatFlag.Currency,
				new CurrencyDataFormatter.CurrencyFormatArgs
				{
					// culture name
					CultureEnglishName = "en-US",

					// decimal digit places
					DecimalPlaces = 1,

					// symbol
					PrefixSymbol = "$",
				});
		}

		private void button3_Click(object sender, EventArgs e)
		{
			worksheet.SetRangeDataFormat(RangePosition.EntireRange, CellDataFormatFlag.Text, null);
		}
	}
}
