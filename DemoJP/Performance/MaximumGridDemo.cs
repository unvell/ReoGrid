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

namespace unvell.ReoGrid.Demo.PerformanceDemo
{
	/// <summary>
	/// 最大行列数のワークシートの性能
	/// </summary>
	public partial class MaximumGridDemo : UserControl
	{
		public MaximumGridDemo()
		{
			InitializeComponent();

			var worksheet = grid.CurrentWorksheet;

			// ワークシートのサイズを 1048576 x 32768 に変更
			worksheet.Resize(1048576, 32768);

			worksheet.MergeRange(1, 1, 1, 6);
			worksheet[1, 1] = "最大行列数 (1,048,576 x 32,768) のワークシートのデモです。";

			worksheet.MergeRange(3, 1, 2, 8);
			worksheet[3, 1] = "このワークシートでスクロール、拡大縮小、編集、選択範囲の移動などを操作して、動作のレスポンスを体験してください。";
		}
	}
}
