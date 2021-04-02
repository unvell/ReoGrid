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

namespace unvell.ReoGrid.Demo.CustomCells
{
	partial class SlideCellDemo
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
			this.chkDisableSelection = new System.Windows.Forms.CheckBox();
			this.chkShowGridLines = new System.Windows.Forms.CheckBox();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// grid
			// 
			this.grid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grid.Location = new System.Drawing.Point(0, 0);
			this.grid.Name = "grid";
			this.grid.Size = new System.Drawing.Size(666, 406);
			this.grid.TabIndex = 3;
			this.grid.Text = "reoGridControl1";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.chkDisableSelection);
			this.panel1.Controls.Add(this.chkShowGridLines);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(666, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(144, 406);
			this.panel1.TabIndex = 4;
			// 
			// chkDisableSelection
			// 
			this.chkDisableSelection.AutoSize = true;
			this.chkDisableSelection.Location = new System.Drawing.Point(13, 68);
			this.chkDisableSelection.Name = "chkDisableSelection";
			this.chkDisableSelection.Size = new System.Drawing.Size(113, 16);
			this.chkDisableSelection.TabIndex = 1;
			this.chkDisableSelection.Text = "Disable Selection";
			this.chkDisableSelection.UseVisualStyleBackColor = true;
			// 
			// chkShowGridLines
			// 
			this.chkShowGridLines.AutoSize = true;
			this.chkShowGridLines.Checked = true;
			this.chkShowGridLines.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkShowGridLines.Location = new System.Drawing.Point(13, 31);
			this.chkShowGridLines.Name = "chkShowGridLines";
			this.chkShowGridLines.Size = new System.Drawing.Size(107, 16);
			this.chkShowGridLines.TabIndex = 0;
			this.chkShowGridLines.Text = "Show Grid Lines";
			this.chkShowGridLines.UseVisualStyleBackColor = true;
			// 
			// SlideCellForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(810, 406);
			this.Controls.Add(this.grid);
			this.Controls.Add(this.panel1);
			this.Name = "SlideCellForm";
			this.Text = "SlideCellForm";
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private ReoGridControl grid;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.CheckBox chkShowGridLines;
		private System.Windows.Forms.CheckBox chkDisableSelection;
	}
}