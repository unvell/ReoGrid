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

using unvell.ReoGrid.Chart;

namespace unvell.ReoGrid.Demo.Charts
{
	public partial class DoughnutChartDemo : UserControl
	{
		public DoughnutChartDemo()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			var worksheet = grid.CurrentWorksheet;

			worksheet["A2"] = new object[,] {
					{ null, 2008, 2009, 2010, 2011, 2012 },
					{ "City 1", 3, 2, 4, 2, 6 },
			};

			var dataRange = worksheet.Ranges["B3:F3"];
			var titleRange = worksheet.Ranges["B2:F2"];

			worksheet.AddHighlightRange(dataRange);
			worksheet.AddHighlightRange(titleRange);

			Chart.Chart c1 = new Chart.DoughnutChart
			{
				Location = new Graphics.Point(220, 160),
				Size = new Graphics.Size(400, 260),

				Title = "Doughnut Chart Sample",
				DataSource = new Chart.WorksheetChartDataSource(worksheet, titleRange, dataRange, RowOrColumn.Column),
			};

			worksheet.FloatingObjects.Add(c1);

		}

	}
}
