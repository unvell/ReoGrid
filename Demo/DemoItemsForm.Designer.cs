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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DemoItemsForm));
			this.demoPanel = new System.Windows.Forms.Panel();
			this.tree = new System.Windows.Forms.TreeView();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
			this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.reportBugsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.projectHomepageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.lnkReset = new System.Windows.Forms.LinkLabel();
			this.labTitle = new System.Windows.Forms.Label();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.web = new System.Windows.Forms.WebBrowser();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.menuStrip1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// demoPanel
			// 
			this.demoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.demoPanel.Location = new System.Drawing.Point(3, 35);
			this.demoPanel.Name = "demoPanel";
			this.demoPanel.Size = new System.Drawing.Size(511, 129);
			this.demoPanel.TabIndex = 0;
			// 
			// tree
			// 
			this.tree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tree.Dock = System.Windows.Forms.DockStyle.Left;
			this.tree.HideSelection = false;
			this.tree.ImageIndex = 0;
			this.tree.ImageList = this.imageList1;
			this.tree.Location = new System.Drawing.Point(3, 27);
			this.tree.Name = "tree";
			this.tree.SelectedImageIndex = 0;
			this.tree.Size = new System.Drawing.Size(252, 512);
			this.tree.TabIndex = 1;
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "DialogGroup_5846_16x.png");
			this.imageList1.Images.SetKeyName(1, "Table_748.png");
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(3, 3);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(769, 24);
			this.menuStrip1.TabIndex = 8;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newEditorToolStripMenuItem,
            this.openFileToolStripMenuItem,
            this.toolStripMenuItem3,
            this.closeToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// newEditorToolStripMenuItem
			// 
			this.newEditorToolStripMenuItem.Name = "newEditorToolStripMenuItem";
			this.newEditorToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.newEditorToolStripMenuItem.Text = "New Editor...";
			this.newEditorToolStripMenuItem.Click += new System.EventHandler(this.newEditorToolStripMenuItem_Click);
			// 
			// openFileToolStripMenuItem
			// 
			this.openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
			this.openFileToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.openFileToolStripMenuItem.Text = "Open File...";
			this.openFileToolStripMenuItem.Click += new System.EventHandler(this.openFileToolStripMenuItem_Click);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(149, 6);
			// 
			// closeToolStripMenuItem
			// 
			this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
			this.closeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.closeToolStripMenuItem.Text = "&Close";
			this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reportBugsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.aboutToolStripMenuItem,
            this.projectHomepageToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = "&Help";
			// 
			// reportBugsToolStripMenuItem
			// 
			this.reportBugsToolStripMenuItem.Name = "reportBugsToolStripMenuItem";
			this.reportBugsToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
			this.reportBugsToolStripMenuItem.Text = "Report Bugs...";
			this.reportBugsToolStripMenuItem.Click += new System.EventHandler(this.reportBugToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(179, 6);
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
			this.aboutToolStripMenuItem.Text = "&About...";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// projectHomepageToolStripMenuItem
			// 
			this.projectHomepageToolStripMenuItem.Name = "projectHomepageToolStripMenuItem";
			this.projectHomepageToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
			this.projectHomepageToolStripMenuItem.Text = "Project Homepage...";
			this.projectHomepageToolStripMenuItem.Click += new System.EventHandler(this.projectHomepageToolStripMenuItem_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.demoPanel);
			this.panel1.Controls.Add(this.panel2);
			this.panel1.Controls.Add(this.splitter1);
			this.panel1.Controls.Add(this.web);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(255, 27);
			this.panel1.Name = "panel1";
			this.panel1.Padding = new System.Windows.Forms.Padding(3);
			this.panel1.Size = new System.Drawing.Size(517, 512);
			this.panel1.TabIndex = 9;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.lnkReset);
			this.panel2.Controls.Add(this.labTitle);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(3, 3);
			this.panel2.Name = "panel2";
			this.panel2.Padding = new System.Windows.Forms.Padding(4);
			this.panel2.Size = new System.Drawing.Size(511, 32);
			this.panel2.TabIndex = 13;
			// 
			// lnkReset
			// 
			this.lnkReset.AutoSize = true;
			this.lnkReset.Location = new System.Drawing.Point(360, 11);
			this.lnkReset.Name = "lnkReset";
			this.lnkReset.Size = new System.Drawing.Size(35, 13);
			this.lnkReset.TabIndex = 2;
			this.lnkReset.TabStop = true;
			this.lnkReset.Text = "Reset";
			this.lnkReset.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkReset_LinkClicked);
			// 
			// labTitle
			// 
			this.labTitle.Dock = System.Windows.Forms.DockStyle.Left;
			this.labTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labTitle.Location = new System.Drawing.Point(4, 4);
			this.labTitle.Name = "labTitle";
			this.labTitle.Size = new System.Drawing.Size(350, 24);
			this.labTitle.TabIndex = 0;
			this.labTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter1.Location = new System.Drawing.Point(3, 164);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(511, 4);
			this.splitter1.TabIndex = 12;
			this.splitter1.TabStop = false;
			// 
			// web
			// 
			this.web.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.web.Location = new System.Drawing.Point(3, 168);
			this.web.MinimumSize = new System.Drawing.Size(20, 20);
			this.web.Name = "web";
			this.web.Size = new System.Drawing.Size(511, 341);
			this.web.TabIndex = 1;
			this.web.Url = new System.Uri("", System.UriKind.Relative);
			// 
			// splitter2
			// 
			this.splitter2.Location = new System.Drawing.Point(255, 27);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(4, 512);
			this.splitter2.TabIndex = 10;
			this.splitter2.TabStop = false;
			// 
			// DemoItemsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(775, 542);
			this.Controls.Add(this.splitter2);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.tree);
			this.Controls.Add(this.menuStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "DemoItemsForm";
			this.Padding = new System.Windows.Forms.Padding(3);
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ReoGrid Demo";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

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
	}
}