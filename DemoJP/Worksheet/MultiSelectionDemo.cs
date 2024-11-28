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

namespace unvell.ReoGrid.Demo.WorksheetDemo
{
	/// <summary>
	/// マルチ選択の実装のデモ
	/// </summary>
	public partial class MultiSelectionDemo : UserControl
	{
		private Worksheet worksheet;

		public MultiSelectionDemo()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			worksheet = grid.CurrentWorksheet;

			// シートサイズを変更
			worksheet.Resize(50, 7);

			// 一つ目の列の幅を調整
			worksheet.ColumnHeaders[0].Width = 20;

			// 一つ目の列のデフォルトセル型をチェックボックスに変更
			worksheet.ColumnHeaders[0].DefaultCellBody = typeof(CheckBoxCell);

			// 他の列の幅を調整
			worksheet.ColumnHeaders[1].Width = 100;
			worksheet.ColumnHeaders[2].Width = 500;
			
			// データを設定
			worksheet["A1"] = new object[,] {
				{ null, "Function", "Description" },
				{ false, "INDIRECT", "Returns the reference specified by a text string." },
				{ false, "ADDRESS", "You can use the ADDRESS function to obtain the address of a cell in a worksheet." },
				{ false, "COUNT", "The COUNT function counts the number of cells that contain numbers, and counts numbers within the list of arguments." },
				{ false, "COUNTIF", "The COUNTIF function counts the number of cells within a range that meet a single criterion that you specify." },
				{ false, "SUM", "Returns the sum of a set of values contained in a specified field on a query." },
				{ false, "SUMIF", "You use the SUMIF function to sum the values in a range that meet criteria that you specify." },
				{ false, "AVERAGE", "Returns the average (arithmetic mean) of the arguments." },
				{ false, "LOOKUP", "The LOOKUP function returns a value either from a one-row or one-column range or from an array." },
				{ false, "ROWS", "Returns the number of rows in a reference or array." },
				{ false, "COLUMNS", "Returns the number of columns in an array or reference." },
				{ false, "INDEX", "Returns a value or the reference to a value from within a table or range." },
				{ false, "CEILING", "Returns number rounded up, away from zero, to the nearest multiple of significance." },
				{ false, "LEN", "Returns the number of characters in a text string." },
				{ false, "LENB", "Returns the number of bytes used to represent the characters in a text string." },
				{ false, "ROUND", "Round a number to the nearest number." },
				{ null, null, null },
				{ null, "セルにクリックしてみてください...", null },
			};

			// 一つ目の行のスタイルを設定 (-1 means entire row or column)
			var rowStyle = worksheet.RowHeaders[0].Style;
			rowStyle.BackColor = Color.DarkSeaGreen;
			rowStyle.TextColor = Color.DarkSlateBlue;
			rowStyle.Bold = true;

			// ドラッグ機能を禁止
			worksheet.DisableSettings(WorksheetSettings.Edit_DragSelectionToMoveCells);

			// ヘッダーをすべて非表示に設定
			worksheet.DisableSettings(WorksheetSettings.View_ShowHeaders);

			// 範囲選択機能を禁止
			worksheet.SelectionMode = WorksheetSelectionMode.None;

			// セルでマウスが押された際のイベントを処理
			worksheet.CellMouseUp += Worksheet_CellMouseUp;

			// セルでマウスがリリースされる際のイベントを処理
			worksheet.CellDataChanged += Worksheet_CellDataChanged;
		}

		public void SelectRow(int row, bool rowChecked)
		{
			var rowRange = new RangePosition(row, 0, 1, worksheet.Columns);

			// 行の背景色を設定
			worksheet.SetRangeStyles(rowRange, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.BackColor,
				BackColor = rowChecked ? Color.SkyBlue : Color.Empty,
			});

			worksheet[row, 0] = rowChecked;
		}
		
		/// <summary>
		/// イベントの二重発生を防ぐためのフラグ
		/// </summary>
		private bool inUpdate = false;

		private void Worksheet_CellMouseUp(object sender, Events.CellMouseEventArgs e)
		{
			if (inUpdate) return;

			int row = e.CellPosition.Row;

			if (row > 0 && row < 16)
			{
				inUpdate = true;

				// チェックボックスのステータスを取得
				bool rowSelected = worksheet.GetCellData<bool>(row, 0);

				if (rowSelected)
				{
					SelectRow(row, false);
				}
				else
				{
					SelectRow(row, true);
				}

				inUpdate = false;
			}
		}

		private void Worksheet_CellDataChanged(object sender, Events.CellEventArgs e)
		{
			if (inUpdate) return;

			var pos = e.Cell.Position;

			inUpdate = true;

			// チェックボックスの位置を確認
			if (pos.Col == 0)
			{
				// 全て選択のチェックボックスの位置を確認
				if (pos.Row == 0)
				{
					// チェックボックスのステータスを取得
					bool checkboxChecked = e.Cell.GetData<bool>();

					// 最初の行から最後の行まで、すべての行を選択
					for (int r = 1; r < 16; r++)
					{
						SelectRow(r, checkboxChecked);
					}
				}
				else
				{
					// 該当行のみを選択
					SelectRow(pos.Row, e.Cell.GetData<bool>());
				}
			}

			inUpdate = false;
		}
	}
}
