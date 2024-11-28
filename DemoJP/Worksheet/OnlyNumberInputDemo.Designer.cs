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

namespace unvell.ReoGrid.Demo.WorksheetDemo
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
			this.grid.Size = new System.Drawing.Size(556, 378);
			this.grid.TabIndex = 0;
			this.grid.Text = "reoGridControl1";
			// 
			// chkOnlyNumeric
			// 
			this.chkOnlyNumeric.AutoSize = true;
			this.chkOnlyNumeric.Checked = true;
			this.chkOnlyNumeric.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkOnlyNumeric.Location = new System.Drawing.Point(14, 13);
			this.chkOnlyNumeric.Name = "chkOnlyNumeric";
			this.chkOnlyNumeric.Size = new System.Drawing.Size(104, 17);
			this.chkOnlyNumeric.TabIndex = 1;
			this.chkOnlyNumeric.Text = "数字のみを入力";
			this.chkOnlyNumeric.UseVisualStyleBackColor = true;
			// 
			// chkErrorPrompt
			// 
			this.chkErrorPrompt.AutoSize = true;
			this.chkErrorPrompt.Location = new System.Drawing.Point(14, 36);
			this.chkErrorPrompt.Name = "chkErrorPrompt";
			this.chkErrorPrompt.Size = new System.Drawing.Size(131, 17);
			this.chkErrorPrompt.TabIndex = 2;
			this.chkErrorPrompt.Text = "エラーメッセージを表示";
			this.chkErrorPrompt.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.chkErrorPrompt);
			this.panel1.Controls.Add(this.chkOnlyNumeric);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(556, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(240, 378);
			this.panel1.TabIndex = 3;
			// 
			// OnlyNumberInputDemo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.grid);
			this.Controls.Add(this.panel1);
			this.Name = "OnlyNumberInputDemo";
			this.Size = new System.Drawing.Size(796, 378);
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