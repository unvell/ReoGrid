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
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace unvell.ReoGrid.Demo.Print
{
	public partial class PrintPreviewDemo : UserControl
	{
		private Worksheet worksheet;

		public PrintPreviewDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			// load the document spreadsheet which contains a printable content and print settings
			this.worksheet.Load(@"_Templates\RGF\printable_report.rgf");

			// set paper size to A4
			this.worksheet.PrintSettings.PaperName = "A4";
		
			// zoom worksheet display to 50%
			this.worksheet.ScaleFactor = 0.5f;
		}

		private void btnPreview_Click(object sender, EventArgs e)
		{
			// create print document
			var session = this.worksheet.CreatePrintSession();

			// show preview dialog
			using (PrintPreviewDialog ppd = new PrintPreviewDialog())
			{
				ppd.Document = session.PrintDocument;
				ppd.SetBounds(200, 200, 1024, 768);
				ppd.PrintPreviewControl.Zoom = 0.5d;
				ppd.PrintPreviewControl.Columns = 2;
				ppd.PrintPreviewControl.Rows = 2;
				ppd.ShowDialog(this);
			}
		}

		private void btnPrintSetup_Click(object sender, EventArgs e)
		{
			using (PageSetupDialog psd = new PageSetupDialog())
			{
				psd.PageSettings = (PageSettings)this.worksheet.PrintSettings.CreateSystemPageSettings();

				psd.AllowMargins = true;
				psd.AllowPrinter = true;
				psd.AllowPaper = true;
				psd.EnableMetric = true;

				if (psd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					this.worksheet.PrintSettings.ApplySystemPageSettings(psd.PageSettings);
					this.worksheet.ClearAllPageBreaks();
				}
			}
		}

		private void tckZoom_Scroll(object sender, EventArgs e)
		{
			this.worksheet.ScaleFactor = (float)tckZoom.Value / 100f;
			labZoom.Text = (this.worksheet.ScaleFactor * 100) + "%";
		}
	}
}
