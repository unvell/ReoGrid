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
	partial class SetEditableRangeDemo
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
			this.btnSetEditableRange = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// grid
			// 
			this.grid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grid.Location = new System.Drawing.Point(0, 0);
			this.grid.Name = "grid";
			this.grid.Size = new System.Drawing.Size(654, 376);
			this.grid.TabIndex = 1;
			this.grid.TabStop = true;
			this.grid.Text = "reoGridControl1";
			// 
			// btnSetEditableRange
			// 
			this.btnSetEditableRange.Location = new System.Drawing.Point(10, 31);
			this.btnSetEditableRange.Name = "btnSetEditableRange";
			this.btnSetEditableRange.Size = new System.Drawing.Size(162, 28);
			this.btnSetEditableRange.TabIndex = 2;
			this.btnSetEditableRange.Text = "Set Editable Range";
			this.btnSetEditableRange.UseVisualStyleBackColor = true;
			this.btnSetEditableRange.Click += new System.EventHandler(this.btnSetEditableRange_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.btnSetEditableRange);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(654, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(185, 376);
			this.panel1.TabIndex = 3;
			// 
			// SetEditableRangeForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(839, 376);
			this.Controls.Add(this.grid);
			this.Controls.Add(this.panel1);
			this.Name = "SetEditableRangeForm";
			this.Text = "SpecifyEditableRangeForm";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private ReoGridControl grid;
		private System.Windows.Forms.Button btnSetEditableRange;
		private System.Windows.Forms.Panel panel1;
	}
}