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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using unvell.ReoGrid.Chart;

namespace unvell.ReoGrid.Demo.Documents
{
	public partial class ProjectBuildReportDemo : UserControl
	{
		public ProjectBuildReportDemo()
		{
			InitializeComponent();

			// get first worksheet instance
			var worksheet = reoGridControl.Worksheets[0];

			// load tepmlate from RGF file.
			// RGF file is a file format that contains worksheet information, 
			// such as data, styles, borders, formula and etc, RGF file can 
			// be saved and loaded by ReoGrid and ReoGridEditor.
			//
			// https://reogrid.net/document/rgf-format
			//
			worksheet.LoadRGF("_Templates\\RGF\\project_building_report.rgf");

			worksheet.Ranges["Q5"].Data = new object[,] {
				{ null, "Unit Test", "Check Style", "Document Gen", },
				{"Warnings", 141, 216, 53},
				{"Errors", 51, 18, 7},
			};

			worksheet.FloatingObjects.Add(new LineChart()
			{
				Title = "Warnings and Errors",
				DataSource = new WorksheetChartDataSource(worksheet, "Q6:Q7", "R6:T7")
				{
					CategoryNameRange = new RangePosition("S6:T6"),
				},

				Location = new Graphics.Point(40, 150),
				Size = new Graphics.Size(360, 220),
			});

			worksheet.FloatingObjects.Add(new Chart.LineChart()
			{
				Title = "Warnings and Errors",
				DataSource = new WorksheetChartDataSource(worksheet, "Q6:Q7", "R6:T7")
				{
					CategoryNameRange = new RangePosition("R6:T6"),
				},

				Location = new Graphics.Point(460, 150),
				Size = new Graphics.Size(360, 220),
			});
		}
	}
}
