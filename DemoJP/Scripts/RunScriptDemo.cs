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

namespace unvell.ReoGrid.Demo.Scripts
{
	/// <summary>
	/// スクリプト言語の実行のデモ
	/// </summary>
	public partial class RunScriptDemo : UserControl
	{
		private Worksheet worksheet;

		public RunScriptDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;

			worksheet.Resize(14, 6);

			for (int r = 0; r < 10; r++)
			{
				for (int c = 0; c < 5; c++)
				{
					worksheet[r, c] = (r + 1) * (c + 1);
				}
			}
		}

		private void btnHelloworld_Click(object sender, EventArgs e)
		{
#if EX_SCRIPT
			this.grid.RunScript("alert('hello world');");
#else
			MessageBox.Show("Script execution is not available in this edition.");
#endif
		}

		private void button1_Click(object sender, EventArgs e)
		{
#if EX_SCRIPT
			var script = @"

var sheet = workbook.currentWorksheet;

var range = sheet.selection.range;

alert('current selection: ' + range);

";

			this.grid.RunScript(script);
#else
			MessageBox.Show("Script execution is not available in this edition.");
#endif
		}

		private void button2_Click(object sender, EventArgs e)
		{
#if EX_SCRIPT
			var script = @"

var sheet = workbook.currentWorksheet;

var pos = sheet.selection.pos;

var cell = sheet.getCell(pos);

cell.style.backgroundColor = 'darkgreen';

";
			this.grid.RunScript(script);
#else
			MessageBox.Show("Script execution is not available in this edition.");
#endif
		}

	}
}
