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
using System.Drawing;
using System.Windows.Forms;

using unvell.ReoGrid.CellTypes;
using unvell.ReoGrid.DemoJP.Properties;

namespace unvell.ReoGrid.Demo.CellTypeDemo
{
	/// <summary>
	/// 内蔵セル型のデモ
	/// </summary>
	public partial class BuiltInCellTypesDemo : UserControl
	{
		private Worksheet worksheet;

		public BuiltInCellTypesDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;

			// ワークシート全体のスタイルを設定
			worksheet.SetRangeStyles(RangePosition.EntireRange, new WorksheetRangeStyle
			{
				// フォント名と縦位置の揃え方を設定
				Flag = PlainStyleFlag.FontName | PlainStyleFlag.VerticalAlign,
				FontName = "Arial",
				VAlign = ReoGridVerAlign.Middle,
			});

			// グリッドラインを非表示
			worksheet.SetSettings(WorksheetSettings.View_ShowGridLine | WorksheetSettings.Edit_DragSelectionToMoveCells, false);

			// 選択モードを単一セルのみに設定
			worksheet.SelectionMode = WorksheetSelectionMode.Cell;

			// 選択スタイルを Focus に設定
			worksheet.SelectionStyle = WorksheetSelectionStyle.FocusRect;

			var grayTextStyle = new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.TextColor,
				TextColor = Color.DimGray
			};

			worksheet.MergeRange(1, 1, 1, 6); // B2:G2

			worksheet.SetRangeStyles(1, 1, 1, 6, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.TextColor | PlainStyleFlag.FontSize,
				TextColor = Color.DarkGreen,
				FontSize = 18,
			});

			worksheet[1, 1] = "内蔵セル型";

			// 列幅を調整
			worksheet.SetColumnsWidth(1, 1, 100);
			worksheet.SetColumnsWidth(2, 1, 30);
			worksheet.SetColumnsWidth(3, 1, 100);
			worksheet.SetColumnsWidth(6, 2, 65);

			// ボタン
			worksheet.MergeRange(3, 2, 1, 2); // C4:D4
			var btn = new ButtonCell("ボタン");
			worksheet[3, 1] = new object[] { "ボタン: ", btn };
			btn.Click += (s, e) => ShowText("ボタンがクリックされた。");

			// リンク
			worksheet.MergeRange(5, 2, 1, 3); // C6:E6
			var link = new HyperlinkCell("https://www.google.com") { AutoNavigate = false };
			worksheet[5, 1] = new object[] { "ハイパーリンク", link };
			link.Click += (s, e) => RGUtility.OpenFileOrLink(worksheet.GetCellText(5, 2));

			// チェックボックス
			var checkbox = new CheckBoxCell();
			worksheet.SetRangeStyles(8, 2, 1, 1, grayTextStyle); // C9:C9
			worksheet[7, 1] = new object[] { "チェックボックス", checkbox, "テキストは別のセルを利用します" };
			worksheet[8, 2] = "(セル型の編集はキーボードでも変更できます)";
			checkbox.CheckChanged += (s, e) => ShowText("チェックステータスが変更された： " + checkbox.IsChecked.ToString());

			// ラジオボタン
			worksheet[10, 1] = "Radio Button";  // B11
			var radioGroup = new RadioButtonGroup(); // ラジオボタングループを作成
			worksheet[10, 2] = new object[,] {  // C11
				{new RadioButtonCell() { RadioGroup = radioGroup }, "リンゴ"},
				{new RadioButtonCell() { RadioGroup = radioGroup }, "ミカン"},
				{new RadioButtonCell() { RadioGroup = radioGroup }, "バナナ"}
			};
			radioGroup.RadioButtons.ForEach(rb => rb.CheckChanged += (s, e) =>
				ShowText("ラジオボタンのステータスが変更された：" + worksheet[rb.Cell.Row, rb.Cell.Column + 1]));
			worksheet[10, 2] = true;
			worksheet[13, 2] = "(RadioGroup に複数のラジオボタンを追加するとお互いに切り替えることができます)";
			worksheet.SetRangeStyles(13, 2, 1, 1, grayTextStyle);   //C14

			// ドロップダウンリスト 
			worksheet.MergeRange(15, 2, 1, 3);   // C16:E16
			var dropdown = new DropdownListCell("リンゴ", "ミカン", "バナナ", "ナシ", "カボチャ", "チェリー", "ココナッツ");
			worksheet[15, 1] = new object[] { "ドロップダウン", dropdown };
			worksheet.SetRangeBorders(15, 2, 1, 3, BorderPositions.Outside, RangeBorderStyle.GraySolid);
			dropdown.SelectedItemChanged += (s, e) => ShowText("ドロップダウンの項目が選択された：" + dropdown.SelectedItem);

			// イメージ
			worksheet.MergeRange(2, 6, 5, 2);  // G3:H7
			worksheet[2, 6] = new ImageCell(Resources.computer_laptop);

			// イベント情報
			worksheet.SetRangeBorders("A20:J20", BorderPositions.Top, RangeBorderStyle.GraySolid);
		}

		private void ShowText(string text)
		{
			worksheet["A20"] = text;
		}

		private void chkGridlines_CheckedChanged(object sender, EventArgs e)
		{
			// グリッド線を表示／非表示
			worksheet.SetSettings(WorksheetSettings.View_ShowGridLine, chkGridlines.Checked);
		}

		private void chkSelection_CheckedChanged(object sender, EventArgs e)
		{
			// 選択モードを変更
			if (chkSelectionNone.Checked)
			{
				worksheet.SelectionMode = WorksheetSelectionMode.None;
			}
			else if (chkSelectionRange.Checked)
			{
				worksheet.SelectionMode = WorksheetSelectionMode.Range;
			}
			else if (chkSelectionCell.Checked)
			{
				worksheet.SelectionMode = WorksheetSelectionMode.Cell;
			}
		}

		private void rdoNormal_CheckedChanged(object sender, EventArgs e)
		{
			// 選択スタイルを変更
			if (rdoFocus.Checked)
			{
				worksheet.SelectionStyle = WorksheetSelectionStyle.FocusRect;
			}
			else if (rdoNormal.Checked)
			{
				worksheet.SelectionStyle = WorksheetSelectionStyle.Default;
			}
		}
	}
}
