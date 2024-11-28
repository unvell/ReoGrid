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
	/// フィルターとデータ並び替えのデモ
	/// https://reogrid.net/jp/document/worksheet/filter-and-sort/
	/// </summary>
	public partial class ColumnFilterAndSortDemo : UserControl
	{
		private Worksheet worksheet;

		public ColumnFilterAndSortDemo()
		{
			InitializeComponent();

			worksheet = grid.CurrentWorksheet;

			// CSVファイルからデータを読み込む
			worksheet.LoadCSV("_Templates\\csv\\13TOKYO.CSV");

			// A列からO列まで、フィルターを作成する
			worksheet.CreateColumnFilter("A", "O");
		}

	}
}
