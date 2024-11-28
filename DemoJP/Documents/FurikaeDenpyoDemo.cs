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
using unvell.ReoGrid.CellTypes;

namespace unvell.ReoGrid.Demo.DocumentDemo
{
	/// <summary>
	/// 振替伝票のデモ
	/// </summary>
	public partial class FurikaeDenpyoDemo : UserControl
	{
		private string filename = "_Templates\\Excel\\振替伝票.xlsx";

		public FurikaeDenpyoDemo()
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
				// 選択範囲のドラッグによる内容の移動を禁止する
				| WorksheetSettings.Edit_DragSelectionToMoveCells);
			
			// セルボタンを作成
			CreateSheetControls();
		}

		private void CreateSheetControls()
		{
			var sheet = reoGridControl.CurrentWorksheet;

			for (int i = 0; i < 5; i++)
			{
				int row = 8 + i * 2;

				var cell = sheet.Cells["F" + row];
				cell.Style.Indent = 3;
				cell.Body = new KamokuList();

				cell = sheet.Cells["O" + row];
				cell.Style.Indent = 3;
				cell.Body = new KamokuList();

				cell = sheet.Cells["I" + (row + 1)];
				cell.Style.Padding = new PaddingValue(3, 0, 12, 12);
				cell.Body = new CheckBoxCell(true);

				cell = sheet.Cells["R" + (row + 1)];
				cell.Style.Padding = new PaddingValue(3, 0, 12, 12);
				cell.Body = new CheckBoxCell();

				cell = sheet.Cells["I" + row];
				cell.Style.Padding = new PaddingValue(1, 1, 12, 12);
				var btn1 = new ButtonCell("...");
				btn1.Click += (s, e) => MessageBox.Show("借方の補助セルがクリックされた：" + cell.Address);
				cell.Body = btn1;

				cell = sheet.Cells["R" + row];
				cell.Style.Padding = new PaddingValue(1, 1, 12, 12);
				var btn2 = new ButtonCell("...");
				btn2.Click += (s, e) => MessageBox.Show("貸方の補助セルがクリックされた：" + cell.Address);
				cell.Body = btn2;
			}
		}

		/// <summary>
		/// 科目リストセル
		/// </summary>
		class KamokuList : DropdownCell
		{
			private KamokuPanel kamokuPanel;

			public KamokuList()
			{
				// 科目選択パネルを作成
				kamokuPanel = new KamokuPanel();
				
				// 科目が選択された際のイベントを処理
				kamokuPanel.KamokuCodeSelected += KamokuPanel_KamokuCodeSelected;

				// 科目選択パネルをドロップダウンパネルに追加
				this.DropdownControl = kamokuPanel;

				// ドロップダウンパネルの幅を440に設定
				this.MinimumDropdownWidth = 440;
			}

			private void KamokuPanel_KamokuCodeSelected(object sender, EventArgs e)
			{
				// リストから選択されたデータをセルに設定
				this.Cell.Data = kamokuPanel.SelectedKamokuCode;

				// ドロップダウンリストを閉じる
				this.PullUp();
			}
		}

	}

}