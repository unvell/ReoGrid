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

namespace unvell.ReoGrid.Demo.CellAndRange
{
	partial class MergeCellsDemo
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
			this.btnMerge = new System.Windows.Forms.Button();
			this.btnUnmerge = new System.Windows.Forms.Button();
			this.btnMergeByScript = new System.Windows.Forms.Button();
			this.btnUnmergeByScript = new System.Windows.Forms.Button();
			this.btnSimulateException = new System.Windows.Forms.Button();
			this.grid = new unvell.ReoGrid.ReoGridControl();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.label3 = new System.Windows.Forms.Label();
			this.txtUnmergeScript = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.txtMergeScript = new System.Windows.Forms.TextBox();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnMerge
			// 
			this.btnMerge.Location = new System.Drawing.Point(12, 66);
			this.btnMerge.Name = "btnMerge";
			this.btnMerge.Size = new System.Drawing.Size(202, 35);
			this.btnMerge.TabIndex = 1;
			this.btnMerge.Text = "Merge Selected Range";
			this.btnMerge.UseVisualStyleBackColor = true;
			this.btnMerge.Click += new System.EventHandler(this.btnMerge_Click);
			// 
			// btnUnmerge
			// 
			this.btnUnmerge.Location = new System.Drawing.Point(12, 107);
			this.btnUnmerge.Name = "btnUnmerge";
			this.btnUnmerge.Size = new System.Drawing.Size(202, 35);
			this.btnUnmerge.TabIndex = 2;
			this.btnUnmerge.Text = "Unmerge Selected Range";
			this.btnUnmerge.UseVisualStyleBackColor = true;
			this.btnUnmerge.Click += new System.EventHandler(this.btnUnmerge_Click);
			// 
			// btnMergeByScript
			// 
			this.btnMergeByScript.Location = new System.Drawing.Point(26, 147);
			this.btnMergeByScript.Name = "btnMergeByScript";
			this.btnMergeByScript.Size = new System.Drawing.Size(167, 25);
			this.btnMergeByScript.TabIndex = 3;
			this.btnMergeByScript.Text = "Run";
			this.btnMergeByScript.UseVisualStyleBackColor = true;
			this.btnMergeByScript.Click += new System.EventHandler(this.btnMergeByScript_Click);
			// 
			// btnUnmergeByScript
			// 
			this.btnUnmergeByScript.Location = new System.Drawing.Point(400, 147);
			this.btnUnmergeByScript.Name = "btnUnmergeByScript";
			this.btnUnmergeByScript.Size = new System.Drawing.Size(167, 25);
			this.btnUnmergeByScript.TabIndex = 5;
			this.btnUnmergeByScript.Text = "Run";
			this.btnUnmergeByScript.UseVisualStyleBackColor = true;
			this.btnUnmergeByScript.Click += new System.EventHandler(this.btnUnmergeByScript_Click);
			// 
			// btnSimulateException
			// 
			this.btnSimulateException.Location = new System.Drawing.Point(12, 282);
			this.btnSimulateException.Name = "btnSimulateException";
			this.btnSimulateException.Size = new System.Drawing.Size(202, 35);
			this.btnSimulateException.TabIndex = 6;
			this.btnSimulateException.Text = "Cause RangeIntersectionException";
			this.btnSimulateException.UseVisualStyleBackColor = true;
			this.btnSimulateException.Click += new System.EventHandler(this.btnSimulateException_Click);
			// 
			// grid
			// 
			this.grid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.grid.ColumnHeaderContextMenuStrip = null;
			this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grid.LeadHeaderContextMenuStrip = null;
			this.grid.Location = new System.Drawing.Point(0, 0);
			this.grid.Name = "grid";
			this.grid.RowHeaderContextMenuStrip = null;
			this.grid.Script = null;
			this.grid.SheetTabContextMenuStrip = null;
			this.grid.SheetTabWidth = 400;
			this.grid.Size = new System.Drawing.Size(622, 405);
			this.grid.TabIndex = 0;
			this.grid.Text = "reoGridControl1";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.btnSimulateException);
			this.panel1.Controls.Add(this.btnUnmerge);
			this.panel1.Controls.Add(this.btnMerge);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(622, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(221, 405);
			this.panel1.TabIndex = 7;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(10, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(77, 13);
			this.label1.TabIndex = 7;
			this.label1.Text = "selected range";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.label3);
			this.panel2.Controls.Add(this.txtUnmergeScript);
			this.panel2.Controls.Add(this.label2);
			this.panel2.Controls.Add(this.txtMergeScript);
			this.panel2.Controls.Add(this.btnMergeByScript);
			this.panel2.Controls.Add(this.btnUnmergeByScript);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 405);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(843, 193);
			this.panel2.TabIndex = 8;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(398, 21);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(201, 13);
			this.label3.TabIndex = 9;
			this.label3.Text = "Unmerge selected range by runnng script";
			// 
			// txtUnmergeScript
			// 
			this.txtUnmergeScript.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtUnmergeScript.Location = new System.Drawing.Point(400, 41);
			this.txtUnmergeScript.Multiline = true;
			this.txtUnmergeScript.Name = "txtUnmergeScript";
			this.txtUnmergeScript.Size = new System.Drawing.Size(353, 99);
			this.txtUnmergeScript.TabIndex = 8;
			this.txtUnmergeScript.Text = "var sheet = workbook.currentWorksheet;\r\n\r\nsheet.unmergeRange(sheet.selection));";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(24, 21);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(190, 13);
			this.label2.TabIndex = 7;
			this.label2.Text = "Merge selected range by running script";
			// 
			// txtMergeScript
			// 
			this.txtMergeScript.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtMergeScript.Location = new System.Drawing.Point(26, 41);
			this.txtMergeScript.Multiline = true;
			this.txtMergeScript.Name = "txtMergeScript";
			this.txtMergeScript.Size = new System.Drawing.Size(353, 99);
			this.txtMergeScript.TabIndex = 6;
			this.txtMergeScript.Text = "var sheet = workbook.currentWorksheet;\r\n\r\nsheet.mergeRange(sheet.selection));";
			// 
			// MergeCellsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(843, 598);
			this.Controls.Add(this.grid);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.panel2);
			this.Name = "MergeCellsForm";
			this.Text = "MergeCellsForm";
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private ReoGridControl grid;
		private System.Windows.Forms.Button btnMerge;
		private System.Windows.Forms.Button btnUnmerge;
		private System.Windows.Forms.Button btnMergeByScript;
		private System.Windows.Forms.Button btnUnmergeByScript;
		private System.Windows.Forms.Button btnSimulateException;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtUnmergeScript;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtMergeScript;
		private System.Windows.Forms.Label label1;
	}
}