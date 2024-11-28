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
	partial class OnlyNumberInputDemo
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
			this.chkOnlyNumeric = new System.Windows.Forms.CheckBox();
			this.chkErrorPrompt = new System.Windows.Forms.CheckBox();
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
			this.grid.Size = new System.Drawing.Size(556, 349);
			this.grid.TabIndex = 0;
			this.grid.TabStop = true;
			this.grid.Text = "reoGridControl1";
			// 
			// chkOnlyNumeric
			// 
			this.chkOnlyNumeric.AutoSize = true;
			this.chkOnlyNumeric.Checked = true;
			this.chkOnlyNumeric.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkOnlyNumeric.Location = new System.Drawing.Point(14, 12);
			this.chkOnlyNumeric.Name = "chkOnlyNumeric";
			this.chkOnlyNumeric.Size = new System.Drawing.Size(208, 16);
			this.chkOnlyNumeric.TabIndex = 1;
			this.chkOnlyNumeric.Text = "All cells only allow to input numeric";
			this.chkOnlyNumeric.UseVisualStyleBackColor = true;
			// 
			// chkErrorPrompt
			// 
			this.chkErrorPrompt.AutoSize = true;
			this.chkErrorPrompt.Location = new System.Drawing.Point(14, 33);
			this.chkErrorPrompt.Name = "chkErrorPrompt";
			this.chkErrorPrompt.Size = new System.Drawing.Size(100, 16);
			this.chkErrorPrompt.TabIndex = 2;
			this.chkErrorPrompt.Text = "Show message";
			this.chkErrorPrompt.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.chkErrorPrompt);
			this.panel1.Controls.Add(this.chkOnlyNumeric);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(556, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(240, 349);
			this.panel1.TabIndex = 3;
			// 
			// OnlyNumberInputForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(796, 349);
			this.Controls.Add(this.grid);
			this.Controls.Add(this.panel1);
			this.Name = "OnlyNumberInputForm";
			this.Text = "OnlyNumberInputForm";
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private ReoGridControl grid;
		private System.Windows.Forms.CheckBox chkOnlyNumeric;
		private System.Windows.Forms.CheckBox chkErrorPrompt;
		private System.Windows.Forms.Panel panel1;
	}
}