/*****************************************************************************
 * 
 * ReoGrid - .NET 表計算スプレッドシートコンポーネント
 * https://reogrid.net/jp
 *
 * ReoGrid 日本語版デモプロジェクトは MIT ライセンスでリリースされています。
 * 
 * このソフトウェアは無保証であり、このソフトウェアの使用により生じた直接・間接の損害に対し、
 * 著作権者は補償を含むあらゆる責任を負いません。 
 * 
 * Copyright (c) 2012-2016 unvell.com, All Rights Reserved.
 * https://www.unvell.com/jp
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

namespace unvell.ReoGrid.Demo.DocumentDemo
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
