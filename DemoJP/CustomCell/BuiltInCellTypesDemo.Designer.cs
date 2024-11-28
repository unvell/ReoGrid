/*****************************************************************************
 * 
 * ReoGrid - .NET 表計算スプレッドシートコンポーネント
 * https://reogrid.net/jp
 *
 * ReoGrid 日本語版デモプロジェクトは MIT ライセンスでリリースされています。
 * 
 * このソフトウェアは無保証であり、このソフトウェアの使用により生じた直接・間接の損害に対し、
 * 著作権者は補償を含むあらゆる責任を負いません。 
 * 
 * Copyright (c) 2012-2016 unvell.com, All Rights Reserved.
 * https://www.unvell.com/jp
 * 
 ****************************************************************************/

namespace unvell.ReoGrid.Demo.CellTypeDemo
{
	partial class BuiltInCellTypesDemo
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
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.rdoNormal = new System.Windows.Forms.RadioButton();
			this.rdoFocus = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.chkSelectionRange = new System.Windows.Forms.RadioButton();
			this.chkSelectionCell = new System.Windows.Forms.RadioButton();
			this.chkSelectionNone = new System.Windows.Forms.RadioButton();
			this.chkGridlines = new System.Windows.Forms.CheckBox();
			this.panel1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
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
			this.grid.SheetTabNewButtonVisible = true;
			this.grid.SheetTabVisible = true;
			this.grid.SheetTabWidth = 400;
			this.grid.ShowScrollEndSpacing = true;
			this.grid.Size = new System.Drawing.Size(751, 542);
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
			this.panel1.Size = new System.Drawing.Size(123, 542);
			this.panel1.TabIndex = 3;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.rdoNormal);
			this.groupBox2.Controls.Add(this.rdoFocus);
			this.groupBox2.Location = new System.Drawing.Point(9, 184);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(102, 92);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "選択スタイル";
			// 
			// rdoNormal
			// 
			this.rdoNormal.AutoSize = true;
			this.rdoNormal.Location = new System.Drawing.Point(13, 55);
			this.rdoNormal.Name = "rdoNormal";
			this.rdoNormal.Size = new System.Drawing.Size(69, 17);
			this.rdoNormal.TabIndex = 1;
			this.rdoNormal.Text = "デフォルト";
			this.rdoNormal.UseVisualStyleBackColor = true;
			this.rdoNormal.CheckedChanged += new System.EventHandler(this.rdoNormal_CheckedChanged);
			// 
			// rdoFocus
			// 
			this.rdoFocus.AutoSize = true;
			this.rdoFocus.Checked = true;
			this.rdoFocus.Location = new System.Drawing.Point(13, 27);
			this.rdoFocus.Name = "rdoFocus";
			this.rdoFocus.Size = new System.Drawing.Size(69, 17);
			this.rdoFocus.TabIndex = 0;
			this.rdoFocus.TabStop = true;
			this.rdoFocus.Text = "フォーカス";
			this.rdoFocus.UseVisualStyleBackColor = true;
			this.rdoFocus.CheckedChanged += new System.EventHandler(this.rdoNormal_CheckedChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.chkSelectionRange);
			this.groupBox1.Controls.Add(this.chkSelectionCell);
			this.groupBox1.Controls.Add(this.chkSelectionNone);
			this.groupBox1.Location = new System.Drawing.Point(9, 55);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(102, 115);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "選択モード";
			// 
			// chkSelectionRange
			// 
			this.chkSelectionRange.AutoSize = true;
			this.chkSelectionRange.Location = new System.Drawing.Point(13, 76);
			this.chkSelectionRange.Name = "chkSelectionRange";
			this.chkSelectionRange.Size = new System.Drawing.Size(49, 17);
			this.chkSelectionRange.TabIndex = 2;
			this.chkSelectionRange.Text = "範囲";
			this.chkSelectionRange.UseVisualStyleBackColor = true;
			this.chkSelectionRange.CheckedChanged += new System.EventHandler(this.chkSelection_CheckedChanged);
			// 
			// chkSelectionCell
			// 
			this.chkSelectionCell.AutoSize = true;
			this.chkSelectionCell.Checked = true;
			this.chkSelectionCell.Location = new System.Drawing.Point(13, 52);
			this.chkSelectionCell.Name = "chkSelectionCell";
			this.chkSelectionCell.Size = new System.Drawing.Size(45, 17);
			this.chkSelectionCell.TabIndex = 1;
			this.chkSelectionCell.TabStop = true;
			this.chkSelectionCell.Text = "セル";
			this.chkSelectionCell.UseVisualStyleBackColor = true;
			this.chkSelectionCell.CheckedChanged += new System.EventHandler(this.chkSelection_CheckedChanged);
			// 
			// chkSelectionNone
			// 
			this.chkSelectionNone.AutoSize = true;
			this.chkSelectionNone.Location = new System.Drawing.Point(13, 28);
			this.chkSelectionNone.Name = "chkSelectionNone";
			this.chkSelectionNone.Size = new System.Drawing.Size(49, 17);
			this.chkSelectionNone.TabIndex = 0;
			this.chkSelectionNone.Text = "無効";
			this.chkSelectionNone.UseVisualStyleBackColor = true;
			this.chkSelectionNone.CheckedChanged += new System.EventHandler(this.chkSelection_CheckedChanged);
			// 
			// chkGridlines
			// 
			this.chkGridlines.AutoSize = true;
			this.chkGridlines.Location = new System.Drawing.Point(16, 22);
			this.chkGridlines.Name = "chkGridlines";
			this.chkGridlines.Size = new System.Drawing.Size(70, 17);
			this.chkGridlines.TabIndex = 0;
			this.chkGridlines.Text = "グリッド線";
			this.chkGridlines.UseVisualStyleBackColor = true;
			this.chkGridlines.CheckedChanged += new System.EventHandler(this.chkGridlines_CheckedChanged);
			// 
			// BuiltInCellTypesDemo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.grid);
			this.Controls.Add(this.panel1);
			this.Name = "BuiltInCellTypesDemo";
			this.Size = new System.Drawing.Size(874, 542);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
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