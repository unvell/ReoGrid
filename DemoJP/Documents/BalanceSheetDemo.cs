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
	/// 貸借対照表のデモ
	/// </summary>
	public partial class BalanceSheetDemo : UserControl
	{
		private string filename = "_Templates\\Excel\\貸借対照表.xlsx";

		public BalanceSheetDemo()
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

			var sheet = reoGridControl.CurrentWorksheet;

			// ワークシートの設定を変更
			sheet.DisableSettings(
				// 列の幅の調整を禁止
				WorksheetSettings.Edit_AllowAdjustColumnWidth 
				// 行の高さの調整を禁止
				| WorksheetSettings.Edit_AllowAdjustRowHeight
				// 行列のヘッダーを非表示
				| WorksheetSettings.View_ShowHeaders
				// 選択範囲のドラッグによる内容の移動を禁止する
				| WorksheetSettings.Edit_DragSelectionToMoveCells);
		
		}

	}

}