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

using unvell.ReoGrid.Events;

namespace unvell.ReoGrid.Demo.WorksheetDemo
{
	/// <summary>
	/// クリップボードの利用方法のデモ
	/// </summary>
	public partial class ClipboardEventDemo : UserControl
	{
		private Worksheet worksheet;

		public ClipboardEventDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;

			worksheet[1, 1] = new object[,] {
				{ "あ","い","う" },
				{ "2012", "2013", "2014" },
				{ 2012, 2013, 2014 },
			};

			worksheet.BeforePaste += grid_BeforePaste;
		}

		void grid_BeforePaste(object sender, BeforeRangeOperationEventArgs e)
		{
			if (chkPreventPasteEvent.Checked || chkCustomizePaste.Checked)
			{
				e.IsCancelled = true;

				if (chkCustomizePaste.Checked)
				{
					string text = Clipboard.GetText();

					object[,] data = RGUtility.ParseTabbedString(text);

					// 解析したデータを指定範囲に格納 
					var applyRange = new RangePosition(worksheet.SelectionRange.Row,
						worksheet.SelectionRange.Col,
						data.GetLength(0), data.GetLength(1));

					worksheet.SetRangeData(applyRange, data);

					worksheet.SetRangeStyles(applyRange, new WorksheetRangeStyle
					{
						Flag = PlainStyleFlag.BackAll,
						BackColor = Color.Yellow,
					});
				}
			}
		}
	}
}
