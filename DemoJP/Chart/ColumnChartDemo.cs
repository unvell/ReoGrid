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
using System.Windows.Forms;

using unvell.ReoGrid.Chart;

namespace unvell.ReoGrid.Demo.ChartDemo
{
	/// <summary>
	/// 縦棒グラフデモ
	/// </summary>
	public partial class ColumnChartDemo : UserControl
	{
		private Worksheet worksheet;

		public ColumnChartDemo()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			worksheet = this.grid.CurrentWorksheet;

			// ソースコードのコメントと説明は「折れ線グラフ（LineChartDemo.cs）」をご覧ください。

			worksheet["A2"] = new object[,] {
					{ null, 2008,	2009,	2010,	2011,	2012 },
					{ "札幌", 3, 2, 4, 2, 6 },
					{ "名古屋", 7, 5, 3, 6, 4 },
					{ "東京", 13,	10,	9, 10, 9 },
					{ "合計",   "=SUM(B3:B5)", "=SUM(C3:C5)", "=SUM(D3:D5)", "=SUM(E3:E5)", "=SUM(F3:F5)" },
			};

			var dataRange = worksheet.Ranges["B3:F5"];
			var rowTitleRange = worksheet.Ranges["A3:A6"];
			var categoryNamesRange = worksheet.Ranges["B2:F2"];

			worksheet.AddHighlightRange(rowTitleRange);
			worksheet.AddHighlightRange(categoryNamesRange);
			worksheet.AddHighlightRange(dataRange);

			var c1 = new ColumnChart
			{
				Location = new Graphics.Point(220, 160),
				Size = new Graphics.Size(400, 260),

				Title = "縦棒グラフ",
				DataSource = new WorksheetChartDataSource(worksheet, rowTitleRange, dataRange)
				{
					CategoryNameRange = categoryNamesRange,
				},
			};

			worksheet.FloatingObjects.Add(c1);

		}

	}
}
