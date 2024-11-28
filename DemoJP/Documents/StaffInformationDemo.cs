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
using unvell.ReoGrid.DemoJP.Properties;

namespace unvell.ReoGrid.Demo.DocumentDemo
{
	/// <summary>
	/// 職員情報のデモ
	/// </summary>
	public partial class StaffInformationDemo : UserControl
	{
		private string filename = "_Templates\\Excel\\職員情報.xlsx";

		public StaffInformationDemo()
		{
			InitializeComponent();

			foreach (Control item in panel2.Controls)
			{
				item.Cursor = Cursors.Hand;
				item.MouseEnter += Item_MouseEnter;
				item.MouseLeave += Item_MouseLeave;
			}
		}

		private void Item_MouseEnter(object sender, EventArgs e)
		{
			var item = sender as Control;
			if (item != null) {
				item.BackColor = System.Drawing.Color.Goldenrod;
			}
		}

		private void Item_MouseLeave(object sender, EventArgs e)
		{
			var item = sender as Control;
			if (item != null)
			{
				item.BackColor = System.Drawing.SystemColors.Window;
			}
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

			CreateSheetControls();
		}

		private static readonly Random rand = new Random();

		private void CreateSheetControls()
		{
			var sheet = reoGridControl.CurrentWorksheet;

			// 自動付番ボタンを追加
			var bangoButton = new ButtonCell("A");
			bangoButton.Click += (s, _e) => sheet["F3"] = rand.Next(100000) + 100000;
			sheet["K3"] = bangoButton;

			// 性別選択ラジオボタンを作成
			var seibetsuRadioButtonGroup = new RadioButtonGroup();
			var seibetsuOtoko = new RadioButtonCell() { RadioGroup = seibetsuRadioButtonGroup };
			var seibetsuOna = new RadioButtonCell() { RadioGroup = seibetsuRadioButtonGroup };
			var seibetsuFumei = new RadioButtonCell() { RadioGroup = seibetsuRadioButtonGroup };

			// ラジオボタンをワークシートに追加
			sheet["O6"] = seibetsuOtoko;
			sheet["Q6"] = seibetsuOna;
			sheet["S6"] = seibetsuFumei;

			// 性別選択ラジオボタンを作成
			var ketsuekiGataGroup = new RadioButtonGroup();
			var ketsuekiA = new RadioButtonCell() { RadioGroup = ketsuekiGataGroup };
			var ketsuekiB = new RadioButtonCell() { RadioGroup = ketsuekiGataGroup };
			var ketsuekiO = new RadioButtonCell() { RadioGroup = ketsuekiGataGroup };
			var ketsuekiAB = new RadioButtonCell() { RadioGroup = ketsuekiGataGroup };
			var ketsuekiFumei = new RadioButtonCell() { RadioGroup = ketsuekiGataGroup };

			// ラジオボタンをワークシートに追加
			sheet["F7"] = ketsuekiA;
			sheet["I7"] = ketsuekiB;
			sheet["L7"] = ketsuekiO;
			sheet["O7"] = ketsuekiAB;
			sheet["R7"] = ketsuekiFumei;

			var kokugaiSumiCheck = new CheckBoxCell();
			sheet["X11"] = kokugaiSumiCheck;

			// すべてのラジオボタンとチェックボックスのスタイルを設定
			foreach (var cell in sheet.Ranges["F6:X11"].Cells)
			{
				if (cell.Body is RadioButtonCell
					|| cell.Body is CheckBoxCell)
				{
					// 余白を設定
					cell.Style.Padding = new PaddingValue(2);

					// 真ん中に寄せる
					cell.Style.HAlign = ReoGridHorAlign.Center;
					cell.Style.VAlign = ReoGridVerAlign.Middle;
				}
			}

			// 国籍リストを追加
			var kokusekiList = new DropdownListCell("日本", "アメリカ", "イギリス", "ドイツ", "フランス", "イタリア", "中国");
			sheet["O8"] = kokusekiList;

			// 郵便番号検索ボタンを追加
			var yubinKensakuButton = new ButtonCell("検索");
			sheet["P12"] = yubinKensakuButton;

			// 入社日・退社日のカレンダーボタンを作成
			var calendarImage = Resources.Calendar_16;
			var nyusyaDay1Button = new ImageButtonCell(calendarImage);
			var taisyaDay1Button = new ImageButtonCell(calendarImage);
			var nyusyaDay2Button = new ImageButtonCell(calendarImage);
			var taisyaDay2Button = new ImageButtonCell(calendarImage);
			var nyusyaDay3Button = new ImageButtonCell(calendarImage);
			var taisyaDay3Button = new ImageButtonCell(calendarImage);

			// 入社日・退社日のカレンダーボタンをワークシートに追加
			sheet["P18"] = nyusyaDay1Button;
			sheet["AA18"] = taisyaDay1Button;
			sheet["P21"] = nyusyaDay2Button;
			sheet["AA21"] = taisyaDay2Button;
			sheet["P24"] = nyusyaDay3Button;
			sheet["AA24"] = taisyaDay3Button;
		}
	}

}