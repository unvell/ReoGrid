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
	public partial class AreaChartDemo : UserControl
	{
		public AreaChartDemo()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			var worksheet = this.grid.CurrentWorksheet;

			worksheet["A2"] = new object[,] {
				{ null, 2008, 2009, 2010, 2011, 2012 },
				{ "City 1", 3, 2, 4, 2, 6 },
				{ "City 2", 7, 5, 3, 6, 4 },
				{ "City 3", 13, 10, 9, 10, 9 },
				{ "Total",  "=SUM(B3:B5)", "=SUM(C3:C5)", "=SUM(D3:D5)", "=SUM(E3:E5)", "=SUM(F3:F5)" },
			};

			var dataRange = worksheet.Ranges["B3:F5"];
			var serialNamesRange = worksheet.Ranges["A3:A6"];
			var categoryNamesRange = worksheet.Ranges["B2:F2"];

			worksheet.AddHighlightRange(categoryNamesRange);
			worksheet.AddHighlightRange(serialNamesRange);
			worksheet.AddHighlightRange(dataRange);
			
			var chart = new Chart.AreaChart
			{
				Location = new Graphics.Point(220, 160),
				Size = new Graphics.Size(400, 260),

				Title = "Area Line Chart Sample",

				// Specify data source.
				// Data source is created from serial data and names for every serial data.
				DataSource = new WorksheetChartDataSource(worksheet, serialNamesRange, dataRange)
				{
					CategoryNameRange = categoryNamesRange,
				}
			};

			// Make all serial colors semi-transparent
			foreach (var style in chart.DataSerialStyles)
			{
				style.FillColor = new Graphics.SolidColor(100, style.FillColor.ToSolidColor());
			}

			worksheet.FloatingObjects.Add(chart);

		}
	}
}
