/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * ReoGrid and ReoGrid Demo project is released under MIT license.
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace unvell.ReoGrid.Demo.Performance
{
	public partial class RowPerformanceDemo : UserControl
	{
		private Worksheet worksheet;

		public RowPerformanceDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;

			worksheet["E10"] = "Press buttons at right side to append rows.";
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
