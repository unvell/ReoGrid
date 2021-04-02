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

namespace unvell.ReoGrid.Demo.Scripts
{
	partial class ReoQueryScriptDemo
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.reoGridControl = new unvell.ReoGrid.ReoGridControl();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.btnRun = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// reoGridControl
			// 
			this.reoGridControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.reoGridControl.ColumnHeaderContextMenuStrip = null;
			this.reoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.reoGridControl.LeadHeaderContextMenuStrip = null;
			this.reoGridControl.Location = new System.Drawing.Point(0, 0);
			this.reoGridControl.Name = "reoGridControl";
			this.reoGridControl.RowHeaderContextMenuStrip = null;
			this.reoGridControl.Script = null;
			this.reoGridControl.SheetTabContextMenuStrip = null;
			this.reoGridControl.SheetTabWidth = 251;
			this.reoGridControl.Size = new System.Drawing.Size(403, 645);
			this.reoGridControl.TabIndex = 2;
			this.reoGridControl.Text = "reoGridControl1";
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
			this.splitter1.Location = new System.Drawing.Point(399, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(4, 645);
			this.splitter1.TabIndex = 3;
			this.splitter1.TabStop = false;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.panel2);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(403, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(464, 645);
			this.panel1.TabIndex = 4;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.btnRun);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(464, 39);
			this.panel2.TabIndex = 6;
			// 
			// btnRun
			// 
			this.btnRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnRun.ForeColor = System.Drawing.Color.Green;
			this.btnRun.Location = new System.Drawing.Point(6, 5);
			this.btnRun.Name = "btnRun";
			this.btnRun.Size = new System.Drawing.Size(92, 31);
			this.btnRun.TabIndex = 0;
			this.btnRun.Text = "Run";
			this.btnRun.UseVisualStyleBackColor = true;
			this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
			// 
			// ReoQueryScriptDemo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.reoGridControl);
			this.Controls.Add(this.panel1);
			this.Name = "ReoQueryScriptDemo";
			this.Size = new System.Drawing.Size(867, 645);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private ReoGridControl reoGridControl;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button btnRun;
	}
}
