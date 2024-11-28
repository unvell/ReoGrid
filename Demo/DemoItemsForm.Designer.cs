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
            reportBugsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            projectHomepageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            panel1 = new System.Windows.Forms.Panel();
            panel2 = new System.Windows.Forms.Panel();
            lnkReset = new System.Windows.Forms.LinkLabel();
            labTitle = new System.Windows.Forms.Label();
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
            demoPanel.Size = new System.Drawing.Size(853, 919);
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
            menuStrip1.Size = new System.Drawing.Size(1282, 37);
            menuStrip1.TabIndex = 8;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { newEditorToolStripMenuItem, openFileToolStripMenuItem, toolStripMenuItem3, closeToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new System.Drawing.Size(54, 29);
            fileToolStripMenuItem.Text = "&File";
            // 
            // newEditorToolStripMenuItem
            // 
            newEditorToolStripMenuItem.Name = "newEditorToolStripMenuItem";
            newEditorToolStripMenuItem.Size = new System.Drawing.Size(213, 34);
            newEditorToolStripMenuItem.Text = "New Editor...";
            newEditorToolStripMenuItem.Click += newEditorToolStripMenuItem_Click;
            // 
            // openFileToolStripMenuItem
            // 
            openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            openFileToolStripMenuItem.Size = new System.Drawing.Size(213, 34);
            openFileToolStripMenuItem.Text = "Open File...";
            openFileToolStripMenuItem.Click += openFileToolStripMenuItem_Click;
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new System.Drawing.Size(210, 6);
            // 
            // closeToolStripMenuItem
            // 
            closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            closeToolStripMenuItem.Size = new System.Drawing.Size(213, 34);
            closeToolStripMenuItem.Text = "&Close";
            closeToolStripMenuItem.Click += closeToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { reportBugsToolStripMenuItem, toolStripMenuItem1, aboutToolStripMenuItem, projectHomepageToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new System.Drawing.Size(65, 29);
            helpToolStripMenuItem.Text = "&Help";
            // 
            // reportBugsToolStripMenuItem
            // 
            reportBugsToolStripMenuItem.Name = "reportBugsToolStripMenuItem";
            reportBugsToolStripMenuItem.Size = new System.Drawing.Size(274, 34);
            reportBugsToolStripMenuItem.Text = "Report Bugs...";
            reportBugsToolStripMenuItem.Click += reportBugToolStripMenuItem_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(271, 6);
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new System.Drawing.Size(274, 34);
            aboutToolStripMenuItem.Text = "&About...";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // projectHomepageToolStripMenuItem
            // 
            projectHomepageToolStripMenuItem.Name = "projectHomepageToolStripMenuItem";
            projectHomepageToolStripMenuItem.Size = new System.Drawing.Size(274, 34);
            projectHomepageToolStripMenuItem.Text = "Project Homepage...";
            projectHomepageToolStripMenuItem.Click += projectHomepageToolStripMenuItem_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(demoPanel);
            panel1.Controls.Add(panel2);
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(424, 43);
            panel1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            panel1.Name = "panel1";
            panel1.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            panel1.Size = new System.Drawing.Size(863, 993);
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
            panel2.Size = new System.Drawing.Size(853, 62);
            panel2.TabIndex = 13;
            // 
            // lnkReset
            // 
            lnkReset.AutoSize = true;
            lnkReset.Location = new System.Drawing.Point(600, 21);
            lnkReset.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            lnkReset.Name = "lnkReset";
            lnkReset.Size = new System.Drawing.Size(54, 25);
            lnkReset.TabIndex = 2;
            lnkReset.TabStop = true;
            lnkReset.Text = "Reset";
            lnkReset.LinkClicked += lnkReset_LinkClicked;
            // 
            // labTitle
            // 
            labTitle.Dock = System.Windows.Forms.DockStyle.Left;
            labTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            labTitle.Location = new System.Drawing.Point(7, 8);
            labTitle.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            labTitle.Name = "labTitle";
            labTitle.Size = new System.Drawing.Size(583, 46);
            labTitle.TabIndex = 0;
            labTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            ClientSize = new System.Drawing.Size(1292, 1042);
            Controls.Add(splitter2);
            Controls.Add(panel1);
            Controls.Add(tree);
            Controls.Add(menuStrip1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            Name = "DemoItemsForm";
            Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "ReoGrid Demo";
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
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label labTitle;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.LinkLabel lnkReset;
		private System.Windows.Forms.ToolStripMenuItem newEditorToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openFileToolStripMenuItem;
	}
}