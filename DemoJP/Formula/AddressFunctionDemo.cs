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

namespace unvell.ReoGrid.Demo.FormulaDemo
{
	/// <summary>
	/// カスタマイズしたヘッダーのデモ
	/// </summary>
	public partial class AddressFunctionDemo : UserControl
	{
		private Worksheet worksheet;

		public AddressFunctionDemo()
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
			worksheet.ColumnHeaders[1].Text = "Product";
			worksheet.ColumnHeaders[2].Text = "Unit Price";
			worksheet.ColumnHeaders[3].Text = "Discount";
			worksheet.ColumnHeaders[4].Text = "Quantity";
			worksheet.ColumnHeaders[5].Text = "Extended Price";
			worksheet.ColumnHeaders[6].Text = "Remark";

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
				{ false, "CPU", "230", "0", "1", extendedPriceFormula },
				{ true, "Motherboard", "120", "5", "1", extendedPriceFormula },
				{ false, "Memory", "90", "0", "2", extendedPriceFormula },
				{ false, "VGA", "310", "0", "1", extendedPriceFormula },
			};

			// 選択したセルの計算式を表示
			worksheet.FocusPosChanged += (s, e) => toolStripStatusLabel1.Text = worksheet.GetCellFormula(worksheet.FocusPos);
		}
	}
}
