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
	partial class CustomSelectionDemo
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
			this.chkTabToNextBlock = new System.Windows.Forms.CheckBox();
			this.chkLimitSelection = new System.Windows.Forms.CheckBox();
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
			this.grid.Size = new System.Drawing.Size(810, 609);
			this.grid.TabIndex = 0;
			this.grid.TabStop = false;
			this.grid.Text = "reoGridControl1";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.chkTabToNextBlock);
			this.panel1.Controls.Add(this.chkLimitSelection);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(810, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(204, 609);
			this.panel1.TabIndex = 1;
			// 
			// chkTabToNextBlock
			// 
			this.chkTabToNextBlock.AutoSize = true;
			this.chkTabToNextBlock.Checked = true;
			this.chkTabToNextBlock.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkTabToNextBlock.Location = new System.Drawing.Point(6, 47);
			this.chkTabToNextBlock.Name = "chkTabToNextBlock";
			this.chkTabToNextBlock.Size = new System.Drawing.Size(181, 17);
			this.chkTabToNextBlock.TabIndex = 0;
			this.chkTabToNextBlock.Text = "タブキーで入力エリアを切り替える";
			this.chkTabToNextBlock.UseVisualStyleBackColor = true;
			this.chkTabToNextBlock.CheckedChanged += new System.EventHandler(this.chkLimitSelection_CheckedChanged);
			// 
			// chkLimitSelection
			// 
			this.chkLimitSelection.AutoSize = true;
			this.chkLimitSelection.Checked = true;
			this.chkLimitSelection.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkLimitSelection.Location = new System.Drawing.Point(6, 22);
			this.chkLimitSelection.Name = "chkLimitSelection";
			this.chkLimitSelection.Size = new System.Drawing.Size(150, 17);
			this.chkLimitSelection.TabIndex = 0;
			this.chkLimitSelection.Text = "カスタマイズした選択モード";
			this.chkLimitSelection.UseVisualStyleBackColor = true;
			this.chkLimitSelection.CheckedChanged += new System.EventHandler(this.chkLimitSelection_CheckedChanged);
			// 
			// CustomSelectionDemo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.grid);
			this.Controls.Add(this.panel1);
			this.Name = "CustomSelectionDemo";
			this.Size = new System.Drawing.Size(1014, 609);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private ReoGridControl grid;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.CheckBox chkLimitSelection;
		private System.Windows.Forms.CheckBox chkTabToNextBlock;
	}
}