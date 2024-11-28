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

namespace unvell.ReoGrid.Demo.Features
{
	partial class SynchronizeScrollDemo
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnScrollToBottom = new System.Windows.Forms.Button();
			this.btnScrollToTop = new System.Windows.Forms.Button();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.timer2 = new System.Windows.Forms.Timer(this.components);
			this.reoGridControl2 = new unvell.ReoGrid.ReoGridControl();
			this.reoGridControl1 = new unvell.ReoGrid.ReoGridControl();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.btnScrollToBottom);
			this.panel1.Controls.Add(this.btnScrollToTop);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(704, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(177, 608);
			this.panel1.TabIndex = 6;
			// 
			// btnScrollToBottom
			// 
			this.btnScrollToBottom.Location = new System.Drawing.Point(17, 53);
			this.btnScrollToBottom.Name = "btnScrollToBottom";
			this.btnScrollToBottom.Size = new System.Drawing.Size(143, 35);
			this.btnScrollToBottom.TabIndex = 1;
			this.btnScrollToBottom.Text = "Scroll to Bottom";
			this.btnScrollToBottom.UseVisualStyleBackColor = true;
			this.btnScrollToBottom.Click += new System.EventHandler(this.btnScrollToBottom_Click);
			// 
			// btnScrollToTop
			// 
			this.btnScrollToTop.Location = new System.Drawing.Point(17, 12);
			this.btnScrollToTop.Name = "btnScrollToTop";
			this.btnScrollToTop.Size = new System.Drawing.Size(143, 35);
			this.btnScrollToTop.TabIndex = 0;
			this.btnScrollToTop.Text = "Scroll to Top";
			this.btnScrollToTop.UseVisualStyleBackColor = true;
			this.btnScrollToTop.Click += new System.EventHandler(this.btnScrollToTop_Click);
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(379, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(4, 608);
			this.splitter1.TabIndex = 8;
			this.splitter1.TabStop = false;
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// timer2
			// 
			this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
			// 
			// reoGridControl2
			// 
			this.reoGridControl2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.reoGridControl2.ColumnHeaderContextMenuStrip = null;
			this.reoGridControl2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.reoGridControl2.LeadHeaderContextMenuStrip = null;
			this.reoGridControl2.Location = new System.Drawing.Point(383, 0);
			this.reoGridControl2.Name = "reoGridControl2";
			this.reoGridControl2.RowHeaderContextMenuStrip = null;
			this.reoGridControl2.Script = null;
			this.reoGridControl2.SheetTabContextMenuStrip = null;
			this.reoGridControl2.SheetTabNewButtonVisible = false;
			this.reoGridControl2.SheetTabWidth = 246;
			this.reoGridControl2.SheetTabNewButtonVisible = false;
			this.reoGridControl2.Size = new System.Drawing.Size(321, 608);
			this.reoGridControl2.TabIndex = 2;
			this.reoGridControl2.TabStop = false;
			this.reoGridControl2.Text = "reoGridControl1";
			// 
			// reoGridControl1
			// 
			this.reoGridControl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.reoGridControl1.ColumnHeaderContextMenuStrip = null;
			this.reoGridControl1.Dock = System.Windows.Forms.DockStyle.Left;
			this.reoGridControl1.LeadHeaderContextMenuStrip = null;
			this.reoGridControl1.Location = new System.Drawing.Point(0, 0);
			this.reoGridControl1.Name = "reoGridControl1";
			this.reoGridControl1.RowHeaderContextMenuStrip = null;
			this.reoGridControl1.Script = null;
			this.reoGridControl1.SheetTabContextMenuStrip = null;
			this.reoGridControl1.SheetTabNewButtonVisible = false;
			this.reoGridControl1.SheetTabWidth = 300;
			this.reoGridControl1.SheetTabNewButtonVisible = false;
			this.reoGridControl1.Size = new System.Drawing.Size(379, 608);
			this.reoGridControl1.TabIndex = 7;
			this.reoGridControl1.TabStop = false;
			this.reoGridControl1.Text = "reoGridControl2";
			// 
			// SynchronizeScrollDemo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.reoGridControl2);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.reoGridControl1);
			this.Controls.Add(this.panel1);
			this.Name = "SynchronizeScrollDemo";
			this.Size = new System.Drawing.Size(881, 608);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private ReoGridControl reoGridControl2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnScrollToBottom;
		private System.Windows.Forms.Button btnScrollToTop;
		private ReoGridControl reoGridControl1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Timer timer2;
	}
}