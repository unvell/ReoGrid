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

namespace unvell.ReoGrid.Demo.WorkbookDemo
{
	partial class MultisheetDemo
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
			this.components = new System.ComponentModel.Container();
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnSheetList = new unvell.ReoGrid.Demo.DropdownButton();
			this.btnRemoveSheet = new System.Windows.Forms.Button();
			this.btnAddWorksheet = new System.Windows.Forms.Button();
			this.sheetListContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.grid = new unvell.ReoGrid.ReoGridControl();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.btnSheetList);
			this.panel1.Controls.Add(this.btnRemoveSheet);
			this.panel1.Controls.Add(this.btnAddWorksheet);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(661, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(161, 564);
			this.panel1.TabIndex = 6;
			// 
			// btnSheetList
			// 
			this.btnSheetList.Location = new System.Drawing.Point(6, 86);
			this.btnSheetList.Name = "btnSheetList";
			this.btnSheetList.Size = new System.Drawing.Size(146, 26);
			this.btnSheetList.TabIndex = 1;
			this.btnSheetList.Text = "ワークシートリスト";
			this.btnSheetList.UseVisualStyleBackColor = true;
			this.btnSheetList.Click += new System.EventHandler(this.btnSheetList_Click);
			// 
			// btnRemoveSheet
			// 
			this.btnRemoveSheet.Location = new System.Drawing.Point(6, 44);
			this.btnRemoveSheet.Name = "btnRemoveSheet";
			this.btnRemoveSheet.Size = new System.Drawing.Size(146, 26);
			this.btnRemoveSheet.TabIndex = 0;
			this.btnRemoveSheet.Text = "シートを削除";
			this.btnRemoveSheet.UseVisualStyleBackColor = true;
			this.btnRemoveSheet.Click += new System.EventHandler(this.btnRemoveSheet_Click);
			// 
			// btnAddWorksheet
			// 
			this.btnAddWorksheet.Location = new System.Drawing.Point(6, 12);
			this.btnAddWorksheet.Name = "btnAddWorksheet";
			this.btnAddWorksheet.Size = new System.Drawing.Size(146, 26);
			this.btnAddWorksheet.TabIndex = 0;
			this.btnAddWorksheet.Text = "シートを追加";
			this.btnAddWorksheet.UseVisualStyleBackColor = true;
			this.btnAddWorksheet.Click += new System.EventHandler(this.btnAddWorksheet_Click);
			// 
			// sheetListContextMenuStrip
			// 
			this.sheetListContextMenuStrip.Name = "sheetListContextMenuStrip";
			this.sheetListContextMenuStrip.Size = new System.Drawing.Size(61, 4);
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
			this.grid.Size = new System.Drawing.Size(661, 564);
			this.grid.TabIndex = 2;
			this.grid.TabStop = false;
			this.grid.Text = "reoGridControl1";
			// 
			// MultisheetDemo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.grid);
			this.Controls.Add(this.panel1);
			this.Name = "MultisheetDemo";
			this.Size = new System.Drawing.Size(822, 564);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private ReoGridControl grid;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnAddWorksheet;
		private System.Windows.Forms.Button btnRemoveSheet;
		private System.Windows.Forms.ContextMenuStrip sheetListContextMenuStrip;
		private DropdownButton btnSheetList;
	}
}