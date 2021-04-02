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
			this.sheetListContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.btnRemoveSheet = new System.Windows.Forms.Button();
			this.btnAddWorksheet = new System.Windows.Forms.Button();
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
			this.btnSheetList.Text = "Worksheet List";
			this.btnSheetList.UseVisualStyleBackColor = true;
			this.btnSheetList.Click += new System.EventHandler(this.btnSheetList_Click);
			// 
			// sheetListContextMenuStrip
			// 
			this.sheetListContextMenuStrip.Name = "sheetListContextMenuStrip";
			this.sheetListContextMenuStrip.Size = new System.Drawing.Size(61, 4);
			this.sheetListContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.sheetListContextMenuStrip_Opening);
			// 
			// btnRemoveSheet
			// 
			this.btnRemoveSheet.Location = new System.Drawing.Point(6, 44);
			this.btnRemoveSheet.Name = "btnRemoveSheet";
			this.btnRemoveSheet.Size = new System.Drawing.Size(146, 26);
			this.btnRemoveSheet.TabIndex = 0;
			this.btnRemoveSheet.Text = "Remove Current Sheet";
			this.btnRemoveSheet.UseVisualStyleBackColor = true;
			this.btnRemoveSheet.Click += new System.EventHandler(this.btnRemoveSheet_Click);
			// 
			// btnAddWorksheet
			// 
			this.btnAddWorksheet.Location = new System.Drawing.Point(6, 12);
			this.btnAddWorksheet.Name = "btnAddWorksheet";
			this.btnAddWorksheet.Size = new System.Drawing.Size(146, 26);
			this.btnAddWorksheet.TabIndex = 0;
			this.btnAddWorksheet.Text = "Append Sheet";
			this.btnAddWorksheet.UseVisualStyleBackColor = true;
			this.btnAddWorksheet.Click += new System.EventHandler(this.btnAddWorksheet_Click);
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
			this.grid.Size = new System.Drawing.Size(661, 564);
			this.grid.TabIndex = 2;
			this.grid.TabStop = false;
			this.grid.Text = "reoGridControl1";
			// 
			// MultisheetForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(822, 564);
			this.Controls.Add(this.grid);
			this.Controls.Add(this.panel1);
			this.Name = "MultisheetForm";
			this.Text = "Multisheet Demo";
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