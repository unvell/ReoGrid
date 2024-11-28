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
	/// ドーナツグラフデモ
	/// </summary>
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

			// ソースコードのコメントと説明は「折れ線グラフ（LineChartDemo.cs）」をご覧ください。

			worksheet["A2"] = new object[,] {
					{ null, 2008,	2009,	2010,	2011,	2012 },
					{ "名古屋", 3, 2, 4, 2, 6 },
			};

			var dataRange = worksheet.Ranges["B3:F3"];
			var titleRange = worksheet.Ranges["B2:F2"];

			worksheet.AddHighlightRange(dataRange);
			worksheet.AddHighlightRange(titleRange);

			Chart.Chart c1 = new Chart.DoughnutChart
			{
				Location = new Graphics.Point(220, 160),
				Size = new Graphics.Size(400, 260),

				Title = "ドーナツグラフ",
				DataSource = new WorksheetChartDataSource(worksheet		// ワークシートインスタンス
				, titleRange		// データ系列名の範囲
				, dataRange			// データ範囲
				, RowOrColumn.Column // データ範囲から系列を作成する方向（Row＝行、Column＝列）
				),
			};

			worksheet.FloatingObjects.Add(c1);

		}

	}
}
