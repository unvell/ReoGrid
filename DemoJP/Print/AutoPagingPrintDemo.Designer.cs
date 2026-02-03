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

namespace unvell.ReoGrid.Demo.PrintDemo
{
	partial class AutoPagingPrintDemo
  {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      panel1 = new System.Windows.Forms.Panel();
      chkShowPagingLines = new System.Windows.Forms.CheckBox();
      label1 = new System.Windows.Forms.Label();
      tckZoom = new System.Windows.Forms.TrackBar();
      labZoom = new System.Windows.Forms.Label();
      btnPrintSetup = new System.Windows.Forms.Button();
      btnSplitPages = new System.Windows.Forms.Button();
      btnPreview = new System.Windows.Forms.Button();
      grid = new ReoGridControl();
      panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)tckZoom).BeginInit();
      SuspendLayout();
      // 
      // panel1
      // 
      panel1.Controls.Add(chkShowPagingLines);
      panel1.Controls.Add(label1);
      panel1.Controls.Add(tckZoom);
      panel1.Controls.Add(labZoom);
      panel1.Controls.Add(btnPrintSetup);
      panel1.Controls.Add(btnSplitPages);
      panel1.Controls.Add(btnPreview);
      panel1.Dock = System.Windows.Forms.DockStyle.Top;
      panel1.Location = new System.Drawing.Point(0, 0);
      panel1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
      panel1.Name = "panel1";
      panel1.Size = new System.Drawing.Size(1513, 117);
      panel1.TabIndex = 2;
      // 
      // chkShowPagingLines
      // 
      chkShowPagingLines.AutoSize = true;
      chkShowPagingLines.Checked = true;
      chkShowPagingLines.CheckState = System.Windows.Forms.CheckState.Checked;
      chkShowPagingLines.Location = new System.Drawing.Point(282, 44);
      chkShowPagingLines.Name = "chkShowPagingLines";
      chkShowPagingLines.Size = new System.Drawing.Size(186, 29);
      chkShowPagingLines.TabIndex = 8;
      chkShowPagingLines.Text = "ページング線を表示";
      chkShowPagingLines.UseVisualStyleBackColor = true;
      chkShowPagingLines.CheckedChanged += chkShowPagingLines_CheckedChanged;
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Location = new System.Drawing.Point(758, 38);
      label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
      label1.Name = "label1";
      label1.Size = new System.Drawing.Size(344, 50);
      label1.TabIndex = 7;
      label1.Text = "ページのサイズを正しく表示するには、\r\nA4用紙をサポートするプリンターが必要です。";
      // 
      // tckZoom
      // 
      tckZoom.LargeChange = 20;
      tckZoom.Location = new System.Drawing.Point(1170, 42);
      tckZoom.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
      tckZoom.Maximum = 100;
      tckZoom.Minimum = 10;
      tckZoom.Name = "tckZoom";
      tckZoom.Size = new System.Drawing.Size(147, 69);
      tckZoom.SmallChange = 10;
      tckZoom.TabIndex = 6;
      tckZoom.TickFrequency = 10;
      tckZoom.Value = 50;
      tckZoom.Scroll += tckZoom_Scroll;
      // 
      // labZoom
      // 
      labZoom.AutoSize = true;
      labZoom.Location = new System.Drawing.Point(1327, 48);
      labZoom.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
      labZoom.Name = "labZoom";
      labZoom.Size = new System.Drawing.Size(47, 25);
      labZoom.TabIndex = 2;
      labZoom.Text = "50%";
      // 
      // btnPrintSetup
      // 
      btnPrintSetup.Location = new System.Drawing.Point(577, 65);
      btnPrintSetup.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
      btnPrintSetup.Name = "btnPrintSetup";
      btnPrintSetup.Size = new System.Drawing.Size(171, 38);
      btnPrintSetup.TabIndex = 1;
      btnPrintSetup.Text = "ページ設定(&S)...";
      btnPrintSetup.UseVisualStyleBackColor = true;
      btnPrintSetup.Click += btnPrintSetup_Click;
      // 
      // btnSplitPages
      // 
      btnSplitPages.Location = new System.Drawing.Point(31, 26);
      btnSplitPages.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
      btnSplitPages.Name = "btnSplitPages";
      btnSplitPages.Size = new System.Drawing.Size(212, 62);
      btnSplitPages.TabIndex = 0;
      btnSplitPages.Text = "自動ページング(S)...";
      btnSplitPages.UseVisualStyleBackColor = true;
      btnSplitPages.Click += btnSplitPages_Click;
      // 
      // btnPreview
      // 
      btnPreview.Location = new System.Drawing.Point(577, 21);
      btnPreview.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
      btnPreview.Name = "btnPreview";
      btnPreview.Size = new System.Drawing.Size(168, 38);
      btnPreview.TabIndex = 0;
      btnPreview.Text = "プレビュー(&P)...";
      btnPreview.UseVisualStyleBackColor = true;
      btnPreview.Click += btnPreview_Click;
      // 
      // grid
      // 
      grid.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
      grid.ColumnHeaderContextMenuStrip = null;
      grid.Dock = System.Windows.Forms.DockStyle.Fill;
      grid.LeadHeaderContextMenuStrip = null;
      grid.Location = new System.Drawing.Point(0, 117);
      grid.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
      grid.Name = "grid";
      grid.RowHeaderContextMenuStrip = null;
      grid.Script = null;
      grid.SheetTabContextMenuStrip = null;
      grid.SheetTabNewButtonVisible = true;
      grid.SheetTabVisible = true;
      grid.SheetTabWidth = 667;
      grid.ShowScrollEndSpacing = true;
      grid.Size = new System.Drawing.Size(1513, 975);
      grid.TabIndex = 1;
      grid.Text = "reoGridControl1";
      // 
      // AutoPagingPrintDemo
      // 
      AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
      AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      Controls.Add(grid);
      Controls.Add(panel1);
      Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
      Name = "AutoPagingPrintDemo";
      Size = new System.Drawing.Size(1513, 1092);
      panel1.ResumeLayout(false);
      panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)tckZoom).EndInit();
      ResumeLayout(false);

    }

    #endregion

    private ReoGridControl grid;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnPreview;
		private System.Windows.Forms.Button btnPrintSetup;
		private System.Windows.Forms.TrackBar tckZoom;
		private System.Windows.Forms.Label labZoom;
		private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btnSplitPages;
    private System.Windows.Forms.CheckBox chkShowPagingLines;
  }
}