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
using System.Windows.Forms;

namespace unvell.ReoGrid.Demo.Features
{
	public partial class SynchronizeScrollDemo : UserControl
	{
		public SynchronizeScrollDemo()
		{
			InitializeComponent();

			// Load tepmlates from RGF file.
			//
			// RGF file is a file format that contains worksheet information, 
			// such as data, styles, borders, formula and etc, RGF file can 
			// be saved and loaded by ReoGrid and ReoGridEditor.
			//
			// https://reogrid.net/document/rgf-format
			// 
			reoGridControl1.CurrentWorksheet.LoadRGF("_Templates\\RGF\\order_sample.rgf");
			reoGridControl2.CurrentWorksheet.LoadRGF("_Templates\\RGF\\order_sample.rgf");

			// Sync scroll from control 1 to control 2
			reoGridControl1.WorksheetScrolled += (s, e) =>
			{
				if (!this.inScrolling)
				{
					this.inScrolling = true;
					reoGridControl2.ScrollCurrentWorksheet(e.X, e.Y);
					this.inScrolling = false;
				}
			};

			// Sync scroll from control 2 to control 1
			reoGridControl2.WorksheetScrolled += (s, e) =>
			{
				if (!this.inScrolling)
				{
					this.inScrolling = true;
					reoGridControl1.ScrollCurrentWorksheet(e.X, e.Y);
					this.inScrolling = false;
				}
			};
		}

		/// <summary>
		/// Flag to avoid scroll two controls recursively
		/// </summary>
		private bool inScrolling = false;

		void reoGridControl1_WorksheetScrolled(object sender, Events.WorksheetScrolledEventArgs e)
		{
			throw new NotImplementedException();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			reoGridControl1.Width = (this.ClientRectangle.Width - panel1.Width - splitter1.Width) / 2;
		}

		private void btnScrollToTop_Click(object sender, EventArgs e)
		{
			timer2.Enabled = false;
			timer1.Enabled = true;
		}

		private void btnScrollToBottom_Click(object sender, EventArgs e)
		{
			timer1.Enabled = false;
			timer2.Enabled = true;
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			reoGridControl1.ScrollCurrentWorksheet(0, -2);
		}

		private void timer2_Tick(object sender, EventArgs e)
		{
			reoGridControl1.ScrollCurrentWorksheet(0, 2);
		}


	}
}
