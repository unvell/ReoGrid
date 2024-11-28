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
	partial class BuiltInTypesDemo
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
			this.chkGridlines = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.chkSelectionNone = new System.Windows.Forms.RadioButton();
			this.chkSelectionCell = new System.Windows.Forms.RadioButton();
			this.chkSelectionRange = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.rdoNormal = new System.Windows.Forms.RadioButton();
			this.rdoFocus = new System.Windows.Forms.RadioButton();
			this.panel1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// grid
			// 
			this.grid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grid.Location = new System.Drawing.Point(0, 0);
			this.grid.Name = "grid";
			this.grid.Size = new System.Drawing.Size(751, 500);
			this.grid.TabIndex = 2;
			this.grid.Text = "reoGridControl1";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.groupBox2);
			this.panel1.Controls.Add(this.groupBox1);
			this.panel1.Controls.Add(this.chkGridlines);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(751, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(123, 500);
			this.panel1.TabIndex = 3;
			// 
			// chkGridlines
			// 
			this.chkGridlines.AutoSize = true;
			this.chkGridlines.Location = new System.Drawing.Point(16, 20);
			this.chkGridlines.Name = "chkGridlines";
			this.chkGridlines.Size = new System.Drawing.Size(73, 16);
			this.chkGridlines.TabIndex = 0;
			this.chkGridlines.Text = "Grid lines";
			this.chkGridlines.UseVisualStyleBackColor = true;
			this.chkGridlines.CheckedChanged += new System.EventHandler(this.chkGridlines_CheckedChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.chkSelectionRange);
			this.groupBox1.Controls.Add(this.chkSelectionCell);
			this.groupBox1.Controls.Add(this.chkSelectionNone);
			this.groupBox1.Location = new System.Drawing.Point(9, 51);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(102, 106);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Selection";
			// 
			// chkSelectionNone
			// 
			this.chkSelectionNone.AutoSize = true;
			this.chkSelectionNone.Location = new System.Drawing.Point(13, 26);
			this.chkSelectionNone.Name = "chkSelectionNone";
			this.chkSelectionNone.Size = new System.Drawing.Size(49, 16);
			this.chkSelectionNone.TabIndex = 0;
			this.chkSelectionNone.Text = "None";
			this.chkSelectionNone.UseVisualStyleBackColor = true;
			this.chkSelectionNone.CheckedChanged += new System.EventHandler(this.chkSelection_CheckedChanged);
			// 
			// chkSelectionCell
			// 
			this.chkSelectionCell.AutoSize = true;
			this.chkSelectionCell.Checked = true;
			this.chkSelectionCell.Location = new System.Drawing.Point(13, 48);
			this.chkSelectionCell.Name = "chkSelectionCell";
			this.chkSelectionCell.Size = new System.Drawing.Size(43, 16);
			this.chkSelectionCell.TabIndex = 1;
			this.chkSelectionCell.TabStop = true;
			this.chkSelectionCell.Text = "Cell";
			this.chkSelectionCell.UseVisualStyleBackColor = true;
			this.chkSelectionCell.CheckedChanged += new System.EventHandler(this.chkSelection_CheckedChanged);
			// 
			// chkSelectionRange
			// 
			this.chkSelectionRange.AutoSize = true;
			this.chkSelectionRange.Location = new System.Drawing.Point(13, 70);
			this.chkSelectionRange.Name = "chkSelectionRange";
			this.chkSelectionRange.Size = new System.Drawing.Size(55, 16);
			this.chkSelectionRange.TabIndex = 2;
			this.chkSelectionRange.Text = "Range";
			this.chkSelectionRange.UseVisualStyleBackColor = true;
			this.chkSelectionRange.CheckedChanged += new System.EventHandler(this.chkSelection_CheckedChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.rdoNormal);
			this.groupBox2.Controls.Add(this.rdoFocus);
			this.groupBox2.Location = new System.Drawing.Point(9, 170);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(102, 85);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Selection Style";
			// 
			// rdoNormal
			// 
			this.rdoNormal.AutoSize = true;
			this.rdoNormal.Location = new System.Drawing.Point(13, 51);
			this.rdoNormal.Name = "rdoNormal";
			this.rdoNormal.Size = new System.Drawing.Size(59, 16);
			this.rdoNormal.TabIndex = 1;
			this.rdoNormal.Text = "Normal";
			this.rdoNormal.UseVisualStyleBackColor = true;
			this.rdoNormal.CheckedChanged += new System.EventHandler(this.rdoNormal_CheckedChanged);
			// 
			// rdoFocus
			// 
			this.rdoFocus.AutoSize = true;
			this.rdoFocus.Checked = true;
			this.rdoFocus.Location = new System.Drawing.Point(13, 25);
			this.rdoFocus.Name = "rdoFocus";
			this.rdoFocus.Size = new System.Drawing.Size(54, 16);
			this.rdoFocus.TabIndex = 0;
			this.rdoFocus.TabStop = true;
			this.rdoFocus.Text = "Focus";
			this.rdoFocus.UseVisualStyleBackColor = true;
			this.rdoFocus.CheckedChanged += new System.EventHandler(this.rdoNormal_CheckedChanged);
			// 
			// BuiltInTypesForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(874, 500);
			this.Controls.Add(this.grid);
			this.Controls.Add(this.panel1);
			this.Name = "BuiltInTypesForm";
			this.Text = "Numeric Progress";
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private ReoGridControl grid;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.CheckBox chkGridlines;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton chkSelectionRange;
		private System.Windows.Forms.RadioButton chkSelectionCell;
		private System.Windows.Forms.RadioButton chkSelectionNone;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton rdoNormal;
		private System.Windows.Forms.RadioButton rdoFocus;
	}
}