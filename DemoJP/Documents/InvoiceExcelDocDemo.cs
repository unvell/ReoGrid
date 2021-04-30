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

namespace unvell.ReoGrid.Demo.DocumentDemo
{
	/// <summary>
	/// 請求書のデモ
	/// </summary>
	public partial class InvoiceExcelDocDemo : UserControl
	{
		private string filename = "_Templates\\Excel\\請求書.xlsx";

		public InvoiceExcelDocDemo()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			// Excelファイルからスプレッドシートを読み込む
			// https://reogrid.net/jp/document/excel-format/
			// 
			reoGridControl.Load(filename);

			// セルボタンを作成
			MakeButton();
		}

		private void MakeButton()
		{
			var worksheet = reoGridControl.Worksheets[0];
			
			// L16:M16範囲を結合
			worksheet.Ranges["L17:M17"].Merge();

			// セルボタンを作成
			var button = new CellTypes.ButtonCell("Excelで開く");

			// クリックした場合 Excel で「請求書」を表示する
			button.Click += (s, e) => RGUtility.OpenFileOrLink(filename);

			// セルボタンをワークシートに置く
			worksheet["L17"] = button;
		}
	}
}
