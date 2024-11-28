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

namespace unvell.ReoGrid.Demo.WorksheetDemo
{
	/// <summary>
	/// アウトラインのデモ
	/// </summary>
	public partial class OutlineDemo : UserControl
	{
		private Worksheet worksheet;

		public OutlineDemo()
		{
			InitializeComponent();

			// 現在表示中のワークシートを取得
			worksheet = grid.CurrentWorksheet;

			// アウトラインを利用するテンプレートファイルを読み込む
			worksheet.LoadRGF("_Templates\\RGF\\outline.rgf");
		}

	}
}
