/*****************************************************************************
 * 
 * ReoGrid - .NET 表計算スプレッドシートコンポーネント
 * 
 * https://reogrid.net/jp
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * ReoGrid 日本語版デモプロジェクトは BSD ライセンスでリリースされています。
 * Copyright (c) 2012-2016 https://www.unvell.com/jp All Rights Reserved.
 * 
 ****************************************************************************/

namespace unvell.ReoGrid.Demo
{
	partial class DemoItemsForm
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
      components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DemoItemsForm));
      demoPanel = new System.Windows.Forms.Panel();
      tree = new System.Windows.Forms.TreeView();
      imageList1 = new System.Windows.Forms.ImageList(components);
      menuStrip1 = new System.Windows.Forms.MenuStrip();
      fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      newEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      openFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
      closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      projectHomepageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      downloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      documentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
      reportBugsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
      aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      panel1 = new System.Windows.Forms.Panel();
      panel2 = new System.Windows.Forms.Panel();
      lnkReset = new System.Windows.Forms.LinkLabel();
      labTitle = new System.Windows.Forms.Label();
      splitter1 = new System.Windows.Forms.Splitter();
      web = new System.Windows.Forms.WebBrowser();
      splitter2 = new System.Windows.Forms.Splitter();
      menuStrip1.SuspendLayout();
      panel1.SuspendLayout();
      panel2.SuspendLayout();
      SuspendLayout();
      // 
      // demoPanel
      // 
      demoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      demoPanel.Location = new System.Drawing.Point(5, 68);
      demoPanel.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
      demoPanel.Name = "demoPanel";
      demoPanel.Size = new System.Drawing.Size(963, 255);
      demoPanel.TabIndex = 0;
      // 
      // tree
      // 
      tree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      tree.Dock = System.Windows.Forms.DockStyle.Left;
      tree.HideSelection = false;
      tree.ImageIndex = 0;
      tree.ImageList = imageList1;
      tree.Location = new System.Drawing.Point(5, 43);
      tree.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
      tree.Name = "tree";
      tree.SelectedImageIndex = 0;
      tree.Size = new System.Drawing.Size(419, 993);
      tree.TabIndex = 1;
      // 
      // imageList1
      // 
      imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      imageList1.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageList1.ImageStream");
      imageList1.TransparentColor = System.Drawing.Color.Transparent;
      imageList1.Images.SetKeyName(0, "DialogGroup_5846_16x.png");
      imageList1.Images.SetKeyName(1, "Table_748.png");
      // 
      // menuStrip1
      // 
      menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
      menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, helpToolStripMenuItem });
      menuStrip1.Location = new System.Drawing.Point(5, 6);
      menuStrip1.Name = "menuStrip1";
      menuStrip1.Padding = new System.Windows.Forms.Padding(10, 4, 0, 4);
      menuStrip1.Size = new System.Drawing.Size(1392, 37);
      menuStrip1.TabIndex = 8;
      menuStrip1.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { newEditorToolStripMenuItem, openFileToolStripMenuItem, toolStripMenuItem3, closeToolStripMenuItem });
      fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      fileToolStripMenuItem.Size = new System.Drawing.Size(101, 29);
      fileToolStripMenuItem.Text = "ファイル(&F)";
      // 
      // newEditorToolStripMenuItem
      // 
      newEditorToolStripMenuItem.Name = "newEditorToolStripMenuItem";
      newEditorToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E;
      newEditorToolStripMenuItem.Size = new System.Drawing.Size(355, 34);
      newEditorToolStripMenuItem.Text = "エディタ(&E)...";
      newEditorToolStripMenuItem.Click += newEditorToolStripMenuItem_Click;
      // 
      // openFileToolStripMenuItem
      // 
      openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
      openFileToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O;
      openFileToolStripMenuItem.Size = new System.Drawing.Size(355, 34);
      openFileToolStripMenuItem.Text = "ファイルを読み込み(&O)...";
      openFileToolStripMenuItem.Click += openFileToolStripMenuItem_Click;
      // 
      // toolStripMenuItem3
      // 
      toolStripMenuItem3.Name = "toolStripMenuItem3";
      toolStripMenuItem3.Size = new System.Drawing.Size(352, 6);
      // 
      // closeToolStripMenuItem
      // 
      closeToolStripMenuItem.Name = "closeToolStripMenuItem";
      closeToolStripMenuItem.Size = new System.Drawing.Size(355, 34);
      closeToolStripMenuItem.Text = "閉じる(&C)";
      closeToolStripMenuItem.Click += closeToolStripMenuItem_Click;
      // 
      // helpToolStripMenuItem
      // 
      helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { projectHomepageToolStripMenuItem, downloadToolStripMenuItem, documentsToolStripMenuItem, toolStripMenuItem2, reportBugsToolStripMenuItem, toolStripMenuItem1, aboutToolStripMenuItem });
      helpToolStripMenuItem.Name = "helpToolStripMenuItem";
      helpToolStripMenuItem.Size = new System.Drawing.Size(97, 29);
      helpToolStripMenuItem.Text = "ヘルプ(&H)";
      // 
      // projectHomepageToolStripMenuItem
      // 
      projectHomepageToolStripMenuItem.Name = "projectHomepageToolStripMenuItem";
      projectHomepageToolStripMenuItem.Size = new System.Drawing.Size(291, 34);
      projectHomepageToolStripMenuItem.Text = "ホームページ(&H)...";
      projectHomepageToolStripMenuItem.Click += projectHomepageToolStripMenuItem_Click;
      // 
      // downloadToolStripMenuItem
      // 
      downloadToolStripMenuItem.Name = "downloadToolStripMenuItem";
      downloadToolStripMenuItem.Size = new System.Drawing.Size(291, 34);
      downloadToolStripMenuItem.Text = "ダウンロード(&D)...";
      downloadToolStripMenuItem.Click += downloadToolStripMenuItem_Click;
      // 
      // documentsToolStripMenuItem
      // 
      documentsToolStripMenuItem.Name = "documentsToolStripMenuItem";
      documentsToolStripMenuItem.Size = new System.Drawing.Size(291, 34);
      documentsToolStripMenuItem.Text = "ドキュメント(&C)...";
      // 
      // toolStripMenuItem2
      // 
      toolStripMenuItem2.Name = "toolStripMenuItem2";
      toolStripMenuItem2.Size = new System.Drawing.Size(288, 6);
      // 
      // reportBugsToolStripMenuItem
      // 
      reportBugsToolStripMenuItem.Name = "reportBugsToolStripMenuItem";
      reportBugsToolStripMenuItem.Size = new System.Drawing.Size(291, 34);
      reportBugsToolStripMenuItem.Text = "バグ報告（英語）(&B)...";
      reportBugsToolStripMenuItem.Click += reportBugToolStripMenuItem_Click;
      // 
      // toolStripMenuItem1
      // 
      toolStripMenuItem1.Name = "toolStripMenuItem1";
      toolStripMenuItem1.Size = new System.Drawing.Size(288, 6);
      // 
      // aboutToolStripMenuItem
      // 
      aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
      aboutToolStripMenuItem.Size = new System.Drawing.Size(291, 34);
      aboutToolStripMenuItem.Text = "ReoGrid について(&A)...";
      aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
      // 
      // panel1
      // 
      panel1.Controls.Add(demoPanel);
      panel1.Controls.Add(panel2);
      panel1.Controls.Add(splitter1);
      panel1.Controls.Add(web);
      panel1.Dock = System.Windows.Forms.DockStyle.Fill;
      panel1.Location = new System.Drawing.Point(424, 43);
      panel1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
      panel1.Name = "panel1";
      panel1.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
      panel1.Size = new System.Drawing.Size(973, 993);
      panel1.TabIndex = 9;
      // 
      // panel2
      // 
      panel2.Controls.Add(lnkReset);
      panel2.Controls.Add(labTitle);
      panel2.Dock = System.Windows.Forms.DockStyle.Top;
      panel2.Location = new System.Drawing.Point(5, 6);
      panel2.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
      panel2.Name = "panel2";
      panel2.Padding = new System.Windows.Forms.Padding(7, 8, 7, 8);
      panel2.Size = new System.Drawing.Size(963, 62);
      panel2.TabIndex = 13;
      // 
      // lnkReset
      // 
      lnkReset.Dock = System.Windows.Forms.DockStyle.Left;
      lnkReset.Location = new System.Drawing.Point(7, 8);
      lnkReset.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
      lnkReset.Name = "lnkReset";
      lnkReset.Size = new System.Drawing.Size(108, 46);
      lnkReset.TabIndex = 2;
      lnkReset.TabStop = true;
      lnkReset.Text = "再読み込み";
      lnkReset.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      lnkReset.LinkClicked += lnkReset_LinkClicked;
      // 
      // labTitle
      // 
      labTitle.AutoSize = true;
      labTitle.Dock = System.Windows.Forms.DockStyle.Left;
      labTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
      labTitle.Location = new System.Drawing.Point(7, 8);
      labTitle.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
      labTitle.Name = "labTitle";
      labTitle.Size = new System.Drawing.Size(0, 29);
      labTitle.TabIndex = 0;
      labTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // splitter1
      // 
      splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
      splitter1.Location = new System.Drawing.Point(5, 323);
      splitter1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
      splitter1.Name = "splitter1";
      splitter1.Size = new System.Drawing.Size(963, 8);
      splitter1.TabIndex = 12;
      splitter1.TabStop = false;
      // 
      // web
      // 
      web.Dock = System.Windows.Forms.DockStyle.Bottom;
      web.Location = new System.Drawing.Point(5, 331);
      web.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
      web.MinimumSize = new System.Drawing.Size(33, 38);
      web.Name = "web";
      web.Size = new System.Drawing.Size(963, 656);
      web.TabIndex = 1;
      web.Url = new System.Uri("about:blank", System.UriKind.Absolute);
      // 
      // splitter2
      // 
      splitter2.Location = new System.Drawing.Point(424, 43);
      splitter2.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
      splitter2.Name = "splitter2";
      splitter2.Size = new System.Drawing.Size(7, 993);
      splitter2.TabIndex = 10;
      splitter2.TabStop = false;
      // 
      // DemoItemsForm
      // 
      AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
      AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      ClientSize = new System.Drawing.Size(1402, 1042);
      Controls.Add(splitter2);
      Controls.Add(panel1);
      Controls.Add(tree);
      Controls.Add(menuStrip1);
      Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
      Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
      Name = "DemoItemsForm";
      Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
      StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      Text = "ReoGrid デモプロジェクト";
      menuStrip1.ResumeLayout(false);
      menuStrip1.PerformLayout();
      panel1.ResumeLayout(false);
      panel2.ResumeLayout(false);
      panel2.PerformLayout();
      ResumeLayout(false);
      PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel demoPanel;
		private System.Windows.Forms.TreeView tree;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem reportBugsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem projectHomepageToolStripMenuItem;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.WebBrowser web;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label labTitle;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.LinkLabel lnkReset;
		private System.Windows.Forms.ToolStripMenuItem newEditorToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openFileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem downloadToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem documentsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
	}
}