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
	public partial class Pie2DChartDemo : UserControl
	{
		public Pie2DChartDemo()
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
			};

			var dataRange = worksheet.Ranges["B3:F3"];
			var titleRange = worksheet.Ranges["B2:F2"];

			worksheet.AddHighlightRange(dataRange);
			worksheet.AddHighlightRange(titleRange);

			Chart.Chart c1 = new Pie2DChart
			{
				Location = new Graphics.Point(220, 160),
				Size = new Graphics.Size(400, 260),

				Title = "2D Pie Chart Sample",
				DataSource = new Chart.WorksheetChartDataSource(worksheet, titleRange, dataRange, RowOrColumn.Column),
			};

			worksheet.FloatingObjects.Add(c1);

		}

	}
}
