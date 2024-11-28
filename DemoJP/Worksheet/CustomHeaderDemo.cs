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

using unvell.ReoGrid.CellTypes;

namespace unvell.ReoGrid.Demo.WorksheetDemo
{
	/// <summary>
	/// カスタマイズしたヘッダーのデモ
	/// </summary>
	public partial class CustomHeaderDemo : UserControl
	{
		private Worksheet worksheet;

		public CustomHeaderDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;

			this.worksheet.Resize(10, 7);

			// 一つ目列のデフォルトセル型を「チェックボックスセル」に設定
			var checkBoxHeader = this.worksheet.ColumnHeaders[0];
			checkBoxHeader.Text = string.Empty;
			checkBoxHeader.DefaultCellBody = typeof(CheckBoxCell);
			checkBoxHeader.Width = 30;
			checkBoxHeader.Style.HorizontalAlign = ReoGridHorAlign.Center;
			checkBoxHeader.Style.VerticalAlign = ReoGridVerAlign.Middle;
			checkBoxHeader.Style.Padding = new PaddingValue(3);

			// ヘッダーテキストを設定
			worksheet.ColumnHeaders[1].Text = "商品";
			worksheet.ColumnHeaders[2].Text = "単価";
			worksheet.ColumnHeaders[3].Text = "値引き";
			worksheet.ColumnHeaders[4].Text = "数量";
			worksheet.ColumnHeaders[5].Text = "小計";
			worksheet.ColumnHeaders[6].Text = "備考";

			// 列幅を調整する
			worksheet.SetColumnsWidth(1, 5, 100);

			// 選択モードをセルに変更
			worksheet.SelectionMode = WorksheetSelectionMode.Cell;

			// 名前範囲recordsを定義
			var recordRange = worksheet.DefineNamedRange("records", 0, 0, 4, 6);

			// 計算式を作成
			// extended price = (unit price - discount) * quantity
			var extendedPriceFormula = "=( INDIRECT(ADDRESS(ROW(), COLUMN()-3)) "
				+ " - INDIRECT(ADDRESS(ROW(), COLUMN()-2)) )"
				+ " * INDIRECT(ADDRESS(ROW(), COLUMN()-1) )";

			// データをワークシートに設定
			recordRange.Data = new object[,] {
				{ false, "メモ帳", 980, 0, 2, extendedPriceFormula },
				{ true, "ボールペン", 120, 10, 5, extendedPriceFormula },
				{ false, "A4用紙", 420, 0, 3, extendedPriceFormula },
				{ false, "業務用ハンドラベラー", 1180, 0, 1, extendedPriceFormula },
			};

			// 選択したセルの計算式を表示
			worksheet.FocusPosChanged += (s, e) => toolStripStatusLabel1.Text = worksheet.GetCellFormula(worksheet.FocusPos);
		}
	}
}
