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
	/// 折れ線グラフデモ
	/// </summary>
	public partial class LineChartDemo : UserControl
	{
		public LineChartDemo()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			var worksheet = this.grid.CurrentWorksheet;

			// データ
			worksheet["A2"] = new object[,] {
				{ null, 2008, 2009, 2010, 2011, 2012 },
				{ "札幌", 3, 2, 4, 2, 6 },
				{ "名古屋", 7, 5, 3, 6, 4 },
				{ "東京", 13, 10, 9, 10, 9 },
				{ "合計",  "=SUM(B3:B5)", "=SUM(C3:C5)", "=SUM(D3:D5)", "=SUM(E3:E5)", "=SUM(F3:F5)" },
			};

			// 範囲を定義
			var dataRange = worksheet.Ranges["B3:F5"];
			var serialNamesRange = worksheet.Ranges["A3:A6"];
			var categoryNamesRange = worksheet.Ranges["B2:F2"];

			// 範囲を強調
			worksheet.AddHighlightRange(categoryNamesRange);
			worksheet.AddHighlightRange(serialNamesRange);
			worksheet.AddHighlightRange(dataRange);

			var c1 = new Chart.LineChart
			{
				// 位置
				Location = new Graphics.Point(220, 160),

				// サイズ
				Size = new Graphics.Size(400, 260),

				// グラフタイトル
				Title = "折れ線グラフ",

				// データソース
				DataSource = new WorksheetChartDataSource(worksheet // ワークシートインスタンス
				, serialNamesRange		// データ名の範囲
				, dataRange						// データの範囲
				)
				{
					CategoryNameRange = categoryNamesRange,   // グラフの目盛り
				}
			};

			// グラフオブジェクトをワークシートに追加
			worksheet.FloatingObjects.Add(c1);
		}

	}
}
