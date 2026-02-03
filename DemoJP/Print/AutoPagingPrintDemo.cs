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
using System.Drawing.Printing;
using System.Windows.Forms;

namespace unvell.ReoGrid.Demo.PrintDemo
{
  public partial class AutoPagingPrintDemo : UserControl
  {
    private Worksheet worksheet;

    public AutoPagingPrintDemo()
    {
      InitializeComponent();

      this.worksheet = grid.CurrentWorksheet;
    }

    int ROW_COUNT = 300;

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      // load the document spreadsheet which contains a printable content and print settings
      this.worksheet.SetRows(ROW_COUNT + 100);

      for (int r = 0; r < ROW_COUNT; r++)
      {
        for (int c = 0; c < 20; c++)
        {
          this.worksheet[r, c] = $"R{r + 1}C{c + 1}";
        }
      }

      this.worksheet.PrintSettings.PaperName = "A4";
      this.worksheet.PrintSettings.ShowGridLines = true;

      this.worksheet.ScaleFactor = 1f;
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

    private void chkShowPagingLines_CheckedChanged(object sender, EventArgs e)
    {
      this.worksheet.SetSettings(WorksheetSettings.View_ShowPageBreaks, chkShowPagingLines.Checked);
    }

    private void btnSplitPages_Click(object sender, EventArgs e)
    {
      this.worksheet.AutoSplitPage();
      this.worksheet.SetSettings(WorksheetSettings.View_ShowPageBreaks, chkShowPagingLines.Checked);
    }
  }
}
