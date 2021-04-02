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
	partial class ResizeGridDialog
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
			this.labRows = new System.Windows.Forms.Label();
			this.labCols = new System.Windows.Forms.Label();
			this.numRows = new System.Windows.Forms.NumericUpDown();
			this.numCols = new System.Windows.Forms.NumericUpDown();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.labRowsRemark = new System.Windows.Forms.Label();
			this.labColsRemark = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.numRows)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numCols)).BeginInit();
			this.SuspendLayout();
			// 
			// labRows
			// 
			this.labRows.AutoSize = true;
			this.labRows.Location = new System.Drawing.Point(21, 25);
			this.labRows.Name = "labRows";
			this.labRows.Size = new System.Drawing.Size(37, 13);
			this.labRows.TabIndex = 0;
			this.labRows.Text = "Rows:";
			// 
			// labCols
			// 
			this.labCols.AutoSize = true;
			this.labCols.Location = new System.Drawing.Point(21, 51);
			this.labCols.Name = "labCols";
			this.labCols.Size = new System.Drawing.Size(30, 13);
			this.labCols.TabIndex = 3;
			this.labCols.Text = "Cols:";
			// 
			// numRows
			// 
			this.numRows.Location = new System.Drawing.Point(77, 23);
			this.numRows.Maximum = new decimal(new int[] {
            1048576,
            0,
            0,
            0});
			this.numRows.Name = "numRows";
			this.numRows.Size = new System.Drawing.Size(71, 20);
			this.numRows.TabIndex = 1;
			// 
			// numCols
			// 
			this.numCols.Location = new System.Drawing.Point(77, 49);
			this.numCols.Maximum = new decimal(new int[] {
            32768,
            0,
            0,
            0});
			this.numCols.Name = "numCols";
			this.numCols.Size = new System.Drawing.Size(71, 20);
			this.numCols.TabIndex = 4;
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(149, 99);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 6;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.button1_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(230, 99);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 7;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// labRowsRemark
			// 
			this.labRowsRemark.AutoSize = true;
			this.labRowsRemark.Location = new System.Drawing.Point(154, 25);
			this.labRowsRemark.Name = "labRowsRemark";
			this.labRowsRemark.Size = new System.Drawing.Size(80, 13);
			this.labRowsRemark.TabIndex = 2;
			this.labRowsRemark.Text = "(max: 1048576)";
			// 
			// labColsRemark
			// 
			this.labColsRemark.AutoSize = true;
			this.labColsRemark.Location = new System.Drawing.Point(154, 51);
			this.labColsRemark.Name = "labColsRemark";
			this.labColsRemark.Size = new System.Drawing.Size(68, 13);
			this.labColsRemark.TabIndex = 5;
			this.labColsRemark.Text = "(max: 32768)";
			// 
			// ResizeGridDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(326, 134);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.numCols);
			this.Controls.Add(this.numRows);
			this.Controls.Add(this.labCols);
			this.Controls.Add(this.labColsRemark);
			this.Controls.Add(this.labRowsRemark);
			this.Controls.Add(this.labRows);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ResizeGridDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Resize - ReoGrid";
			((System.ComponentModel.ISupportInitialize)(this.numRows)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numCols)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labRows;
		private System.Windows.Forms.Label labCols;
		private System.Windows.Forms.NumericUpDown numRows;
		private System.Windows.Forms.NumericUpDown numCols;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label labRowsRemark;
		private System.Windows.Forms.Label labColsRemark;
	}
}