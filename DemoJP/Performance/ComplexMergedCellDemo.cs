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
	/// 複雑なワークシートの性能のデモ
	/// </summary>
	public partial class ComplexMergedCellDemo : UserControl
	{
		public ComplexMergedCellDemo()
		{
			InitializeComponent();

			var worksheet = grid.CurrentWorksheet;

			worksheet.LoadRGF("_Templates\\RGF\\merged_range.rgf");

			worksheet["B20"] = "ReoGrid は高効率なアルゴリズムで結合したセルのメモリ管理と描画を実現しているため、複雑なワークシートでも高速で動作できます。";
			worksheet["B21"] = "結合したセルを選択したり、編集したりして、そのレスポンスを体感してください。";
		}
	}
}
