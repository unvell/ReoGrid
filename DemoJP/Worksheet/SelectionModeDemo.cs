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

using System.Drawing;
using System.Windows.Forms;

namespace unvell.ReoGrid.Demo.WorksheetDemo
{
	public partial class SelectionModeDemo : UserControl
	{
		private Worksheet worksheet;

		public SelectionModeDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;

			// ワークシートのサイズを調整
			worksheet.Resize(50, 7);

			// ヘッダーの幅を調整
			worksheet.ColumnHeaders[0].Width = 100;
			worksheet.ColumnHeaders[1].Width = 500;

			// データを設定
			worksheet["A1"] = new object[,] {
				{ "Function", "Description" },
				{ "INDIRECT", "Returns the reference specified by a text string." },
				{ "ADDRESS", "You can use the ADDRESS function to obtain the address of a cell in a worksheet." },
				{ "COUNT", "The COUNT function counts the number of cells that contain numbers, and counts numbers within the list of arguments." },
				{ "COUNTIF", "The COUNTIF function counts the number of cells within a range that meet a single criterion that you specify." },
				{ "SUM", "Returns the sum of a set of values contained in a specified field on a query." },
				{ "SUMIF", "You use the SUMIF function to sum the values in a range that meet criteria that you specify." },
				{ "AVERAGE", "Returns the average (arithmetic mean) of the arguments." },
			};

			// 1行目にあるすべてのセルの背景色とテキストの色を設定（-1は全ての列）
			worksheet.SetRangeStyles(0, 0, 1, -1, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.BackColor | PlainStyleFlag.TextColor | PlainStyleFlag.FontStyleBold,

				BackColor = Color.DarkSeaGreen,
				TextColor = Color.DarkSlateBlue,
				Bold = true,
			});

			// 選択モードを「行」に設定
			worksheet.SelectionMode = WorksheetSelectionMode.Row;

			// テキスト表示のオーバーフローを禁止
			worksheet.SetSettings(WorksheetSettings.View_AllowCellTextOverflow, false);

			worksheet["B13"] = "選択モードは「行のみ」に設定しています";
		}
	}
}
