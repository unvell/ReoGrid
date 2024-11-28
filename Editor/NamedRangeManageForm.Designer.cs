/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net/
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * ReoGrid and ReoGridEditor is released under MIT license.
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

namespace unvell.ReoGrid.Editor
{
	partial class NamedRangeManageForm
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnClose = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.btnEdit = new System.Windows.Forms.Button();
			this.btnNew = new System.Windows.Forms.Button();
			this.labNamedRange = new System.Windows.Forms.Label();
			this.lstRanges = new System.Windows.Forms.ListView();
			this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colAddress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.btnClose);
			this.panel1.Controls.Add(this.btnDelete);
			this.panel1.Controls.Add(this.btnEdit);
			this.panel1.Controls.Add(this.btnNew);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(326, 4);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(118, 289);
			this.panel1.TabIndex = 0;
			// 
			// btnClose
			// 
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.Location = new System.Drawing.Point(5, 260);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(108, 26);
			this.btnClose.TabIndex = 3;
			this.btnClose.Text = "Close";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// btnDelete
			// 
			this.btnDelete.Location = new System.Drawing.Point(5, 113);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(108, 26);
			this.btnDelete.TabIndex = 2;
			this.btnDelete.Text = "&Delete";
			this.btnDelete.UseVisualStyleBackColor = true;
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// btnEdit
			// 
			this.btnEdit.Location = new System.Drawing.Point(5, 81);
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.Size = new System.Drawing.Size(108, 26);
			this.btnEdit.TabIndex = 1;
			this.btnEdit.Text = "&Edit";
			this.btnEdit.UseVisualStyleBackColor = true;
			this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			// 
			// btnNew
			// 
			this.btnNew.Location = new System.Drawing.Point(5, 49);
			this.btnNew.Name = "btnNew";
			this.btnNew.Size = new System.Drawing.Size(108, 26);
			this.btnNew.TabIndex = 0;
			this.btnNew.Text = "&New";
			this.btnNew.UseVisualStyleBackColor = true;
			this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
			// 
			// labNamedRange
			// 
			this.labNamedRange.Dock = System.Windows.Forms.DockStyle.Top;
			this.labNamedRange.Location = new System.Drawing.Point(4, 4);
			this.labNamedRange.Name = "labNamedRange";
			this.labNamedRange.Size = new System.Drawing.Size(322, 24);
			this.labNamedRange.TabIndex = 1;
			this.labNamedRange.Text = "Named ranges:";
			this.labNamedRange.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lstRanges
			// 
			this.lstRanges.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colAddress});
			this.lstRanges.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstRanges.FullRowSelect = true;
			this.lstRanges.HideSelection = false;
			this.lstRanges.Location = new System.Drawing.Point(4, 28);
			this.lstRanges.Name = "lstRanges";
			this.lstRanges.Size = new System.Drawing.Size(322, 265);
			this.lstRanges.TabIndex = 2;
			this.lstRanges.UseCompatibleStateImageBehavior = false;
			this.lstRanges.View = System.Windows.Forms.View.Details;
			// 
			// colName
			// 
			this.colName.Text = "Name";
			this.colName.Width = 120;
			// 
			// colAddress
			// 
			this.colAddress.Text = "Address";
			this.colAddress.Width = 120;
			// 
			// NamedRangeManageForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnClose;
			this.ClientSize = new System.Drawing.Size(448, 297);
			this.Controls.Add(this.lstRanges);
			this.Controls.Add(this.labNamedRange);
			this.Controls.Add(this.panel1);
			this.Name = "NamedRangeManageForm";
			this.Padding = new System.Windows.Forms.Padding(4);
			this.ShowIcon = false;
			this.Text = "Named Range Management";
			this.Load += new System.EventHandler(this.NamedRangeManageForm_Load);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.Button btnEdit;
		private System.Windows.Forms.Button btnNew;
		private System.Windows.Forms.Label labNamedRange;
		private System.Windows.Forms.ListView lstRanges;
		private System.Windows.Forms.ColumnHeader colName;
		private System.Windows.Forms.ColumnHeader colAddress;
		private System.Windows.Forms.Button btnClose;
	}
}