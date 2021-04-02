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
using System.Windows.Forms;

using unvell.ReoGrid.Chart;

namespace unvell.ReoGrid.Demo.Charts
{
	public partial class BarChartDemo : UserControl
	{
		private Worksheet worksheet;

		public BarChartDemo()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			worksheet = this.grid.CurrentWorksheet;

			worksheet["A2"] = new object[,] {
					{ null, 2008, 2009, 2010, 2011, 2012 },
					{ "City 1", 3, 2, 4, 2, 6 },
					{ "City 2", 7, 5, 3, 6, 4 },
					{ "City 3", 13, 10, 9, 10, 9 },
					{ "Total",  "=SUM(B3:B5)", "=SUM(C3:C5)", "=SUM(D3:D5)", "=SUM(E3:E5)", "=SUM(F3:F5)" },
			};

			var dataRange = worksheet.Ranges["B3:F5"];
			var rowTitleRange = worksheet.Ranges["A3:A6"];
			var categoryNamesRange = worksheet.Ranges["B2:F2"];

			worksheet.AddHighlightRange(rowTitleRange);
			worksheet.AddHighlightRange(categoryNamesRange);
			worksheet.AddHighlightRange(dataRange);

			var chart = new Chart.BarChart
			{
				Location = new Graphics.Point(220, 160),
				Size = new Graphics.Size(400, 260),

				Title = "Bar Chart Sample",

				DataSource = new WorksheetChartDataSource(worksheet, rowTitleRange, dataRange)
				{
					CategoryNameRange = categoryNamesRange,
				},
			};

			worksheet.FloatingObjects.Add(chart);
		}

	}
}
