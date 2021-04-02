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
	partial class CellsEventDemo
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
			this.grid = new unvell.ReoGrid.ReoGridControl();
			this.panel1 = new System.Windows.Forms.Panel();
			this.listbox1 = new System.Windows.Forms.ListBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.chkSelectionRect = new System.Windows.Forms.CheckBox();
			this.chkGridLines = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.chkBackground = new System.Windows.Forms.CheckBox();
			this.chkHoverHighlight = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.chkMove = new System.Windows.Forms.CheckBox();
			this.chkUp = new System.Windows.Forms.CheckBox();
			this.chkDown = new System.Windows.Forms.CheckBox();
			this.chkLeave = new System.Windows.Forms.CheckBox();
			this.chkEnter = new System.Windows.Forms.CheckBox();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// grid
			// 
			this.grid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grid.Location = new System.Drawing.Point(0, 0);
			this.grid.Name = "grid";
			this.grid.Size = new System.Drawing.Size(556, 454);
			this.grid.TabIndex = 2;
			this.grid.TabStop = true;
			this.grid.Text = "reoGridControl1";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.listbox1);
			this.panel1.Controls.Add(this.panel2);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(559, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(378, 454);
			this.panel1.TabIndex = 7;
			// 
			// listbox1
			// 
			this.listbox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listbox1.FormattingEnabled = true;
			this.listbox1.Location = new System.Drawing.Point(151, 0);
			this.listbox1.Name = "listbox1";
			this.listbox1.Size = new System.Drawing.Size(227, 454);
			this.listbox1.TabIndex = 7;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.groupBox3);
			this.panel2.Controls.Add(this.groupBox2);
			this.panel2.Controls.Add(this.groupBox1);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(151, 454);
			this.panel2.TabIndex = 8;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.chkSelectionRect);
			this.groupBox3.Controls.Add(this.chkGridLines);
			this.groupBox3.Location = new System.Drawing.Point(6, 282);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(140, 86);
			this.groupBox3.TabIndex = 8;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Settings";
			// 
			// chkSelectionRect
			// 
			this.chkSelectionRect.AutoSize = true;
			this.chkSelectionRect.Checked = true;
			this.chkSelectionRect.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkSelectionRect.Location = new System.Drawing.Point(15, 54);
			this.chkSelectionRect.Name = "chkSelectionRect";
			this.chkSelectionRect.Size = new System.Drawing.Size(96, 17);
			this.chkSelectionRect.TabIndex = 7;
			this.chkSelectionRect.Text = "Selection Rect";
			this.chkSelectionRect.UseVisualStyleBackColor = true;
			// 
			// chkGridLines
			// 
			this.chkGridLines.AutoSize = true;
			this.chkGridLines.Checked = true;
			this.chkGridLines.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkGridLines.Location = new System.Drawing.Point(15, 30);
			this.chkGridLines.Name = "chkGridLines";
			this.chkGridLines.Size = new System.Drawing.Size(73, 17);
			this.chkGridLines.TabIndex = 6;
			this.chkGridLines.Text = "Grid Lines";
			this.chkGridLines.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.chkBackground);
			this.groupBox2.Controls.Add(this.chkHoverHighlight);
			this.groupBox2.Location = new System.Drawing.Point(6, 177);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(140, 99);
			this.groupBox2.TabIndex = 7;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Effect by event";
			// 
			// chkBackground
			// 
			this.chkBackground.AutoSize = true;
			this.chkBackground.Location = new System.Drawing.Point(15, 56);
			this.chkBackground.Name = "chkBackground";
			this.chkBackground.Size = new System.Drawing.Size(84, 17);
			this.chkBackground.TabIndex = 7;
			this.chkBackground.Text = "Background";
			this.chkBackground.UseVisualStyleBackColor = true;
			// 
			// chkHoverHighlight
			// 
			this.chkHoverHighlight.AutoSize = true;
			this.chkHoverHighlight.Location = new System.Drawing.Point(15, 33);
			this.chkHoverHighlight.Name = "chkHoverHighlight";
			this.chkHoverHighlight.Size = new System.Drawing.Size(99, 17);
			this.chkHoverHighlight.TabIndex = 6;
			this.chkHoverHighlight.Text = "Hover Highlight";
			this.chkHoverHighlight.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.chkMove);
			this.groupBox1.Controls.Add(this.chkUp);
			this.groupBox1.Controls.Add(this.chkDown);
			this.groupBox1.Controls.Add(this.chkLeave);
			this.groupBox1.Controls.Add(this.chkEnter);
			this.groupBox1.Location = new System.Drawing.Point(6, 9);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(139, 161);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Events";
			// 
			// chkMove
			// 
			this.chkMove.AutoSize = true;
			this.chkMove.Location = new System.Drawing.Point(15, 125);
			this.chkMove.Name = "chkMove";
			this.chkMove.Size = new System.Drawing.Size(102, 17);
			this.chkMove.TabIndex = 4;
			this.chkMove.Text = "CellMouseMove";
			this.chkMove.UseVisualStyleBackColor = true;
			// 
			// chkUp
			// 
			this.chkUp.AutoSize = true;
			this.chkUp.Location = new System.Drawing.Point(15, 101);
			this.chkUp.Name = "chkUp";
			this.chkUp.Size = new System.Drawing.Size(89, 17);
			this.chkUp.TabIndex = 3;
			this.chkUp.Text = "CellMouseUp";
			this.chkUp.UseVisualStyleBackColor = true;
			// 
			// chkDown
			// 
			this.chkDown.AutoSize = true;
			this.chkDown.Location = new System.Drawing.Point(15, 77);
			this.chkDown.Name = "chkDown";
			this.chkDown.Size = new System.Drawing.Size(103, 17);
			this.chkDown.TabIndex = 2;
			this.chkDown.Text = "CellMouseDown";
			this.chkDown.UseVisualStyleBackColor = true;
			// 
			// chkLeave
			// 
			this.chkLeave.AutoSize = true;
			this.chkLeave.Location = new System.Drawing.Point(15, 53);
			this.chkLeave.Name = "chkLeave";
			this.chkLeave.Size = new System.Drawing.Size(105, 17);
			this.chkLeave.TabIndex = 1;
			this.chkLeave.Text = "CellMouseLeave";
			this.chkLeave.UseVisualStyleBackColor = true;
			// 
			// chkEnter
			// 
			this.chkEnter.AutoSize = true;
			this.chkEnter.Location = new System.Drawing.Point(15, 29);
			this.chkEnter.Name = "chkEnter";
			this.chkEnter.Size = new System.Drawing.Size(100, 17);
			this.chkEnter.TabIndex = 0;
			this.chkEnter.Text = "CellMouseEnter";
			this.chkEnter.UseVisualStyleBackColor = true;
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
			this.splitter1.Location = new System.Drawing.Point(556, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 454);
			this.splitter1.TabIndex = 8;
			this.splitter1.TabStop = false;
			// 
			// CellsEventForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(937, 454);
			this.Controls.Add(this.grid);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.panel1);
			this.Name = "CellsEventForm";
			this.Text = "Cells Event";
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private ReoGridControl grid;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ListBox listbox1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.CheckBox chkMove;
		private System.Windows.Forms.CheckBox chkUp;
		private System.Windows.Forms.CheckBox chkDown;
		private System.Windows.Forms.CheckBox chkLeave;
		private System.Windows.Forms.CheckBox chkEnter;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox chkHoverHighlight;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox chkSelectionRect;
		private System.Windows.Forms.CheckBox chkGridLines;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox chkBackground;
	}
}