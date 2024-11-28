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

using System.Windows.Forms;

using unvell.ReoGrid.Utility;

namespace unvell.ReoGrid.Demo.CellDemo
{
	/// <summary>
	/// セルの反復（IterateCells）の利用方法デモ
	/// </summary>
	public partial class IterateCellsDemo : UserControl
	{
		public IterateCellsDemo()
		{
			InitializeComponent();
		}

		// データを設定
		private void btnSetData_Click(object sender, System.EventArgs e)
		{
			var sheet = grid.CurrentWorksheet;

			sheet["B2"] = new object[,] {
				{ 1, 2, 3, 4 },
				{ 0.1, 0.2, 0.3, 0.4 },
				{ "リンゴ", "バナナ", "ミカン", "ナシ" },
			};
		}

		// 統計を行う
		private void btnCalcTotal_Click(object sender, System.EventArgs e)
		{
			var sheet = grid.CurrentWorksheet;

			var range = new RangePosition("B2:E4");

			double sumVal = 0, countVal = 0;

			sheet.IterateCells(range, (r, c, inCell) =>
			{
				// 数値を取得
				if (CellUtility.TryGetNumberData(inCell, out var data))
				{
					sumVal += data;
					countVal++;
				}

				// trueを返して反復を続ける
				return true;
			});

			// 計算結果を表示
			sheet["F6:G6"] = new object[] { "合計(SUM)：", sumVal };
			sheet["F7:G7"] = new object[] { "合計(COUNT)：", countVal };
		}
	}
}
