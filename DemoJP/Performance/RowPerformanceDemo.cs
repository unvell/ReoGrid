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
using System.Diagnostics;
using System.Windows.Forms;

namespace unvell.ReoGrid.Demo.PerformanceDemo
{
	/// <summary>
	/// データの追加性能のデモ
	/// </summary>
	public partial class RowPerformanceDemo : UserControl
	{
		private Worksheet worksheet;

		public RowPerformanceDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;

			worksheet["E10"] = "順番に右側のボタンをクリックしてください。";
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Stopwatch sw = Stopwatch.StartNew();
			worksheet.SetRows(10000);
			sw.Stop();
			MessageBox.Show(sw.ElapsedMilliseconds + " ms.");

			button2.Enabled = true;
			button3.Enabled = true;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Stopwatch sw = Stopwatch.StartNew();
			for (int i = 0; i < Math.Min(worksheet.RowCount, 10000); i++)
			{
				worksheet[i, 0] = (i + 1);
			}
			sw.Stop();
			MessageBox.Show(sw.ElapsedMilliseconds + " ms.");

			button3.Enabled = true;
		}

		private void button3_Click(object sender, EventArgs e)
		{
			Stopwatch sw = Stopwatch.StartNew();
			worksheet.SetRows(100000);
			sw.Stop();
			MessageBox.Show(sw.ElapsedMilliseconds + " ms.");

			button4.Enabled = true;
			button5.Enabled = true;
		}

		private void button4_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;

			try
			{
				Stopwatch sw = Stopwatch.StartNew();
				for (int i = 0; i < worksheet.RowCount; i++)
				{
					worksheet[i, 1] = (i + 1);
				}
				sw.Stop();
				MessageBox.Show(sw.ElapsedMilliseconds + " ms.");
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void button5_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;

			try
			{
				worksheet.SuspendDataChangedEvents();

				Stopwatch sw = Stopwatch.StartNew();
				for (int i = 0; i < worksheet.RowCount; i++)
				{
					worksheet[i, 2] = (i + 1);
				}
				sw.Stop();
				MessageBox.Show(sw.ElapsedMilliseconds + " ms.");
			}
			finally
			{
				worksheet.ResumeDataChangedEvents();
		
				Cursor = Cursors.Default;
			}
		}


	}
}
