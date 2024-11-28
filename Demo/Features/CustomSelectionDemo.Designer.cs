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
			this.chkLimitSelection = new System.Windows.Forms.CheckBox();
			this.chkTabToNextBlock = new System.Windows.Forms.CheckBox();
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
			this.grid.SheetTabWidth = 400;
			this.grid.Size = new System.Drawing.Size(822, 609);
			this.grid.TabIndex = 0;
			this.grid.TabStop = false;
			this.grid.Text = "reoGridControl1";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.chkTabToNextBlock);
			this.panel1.Controls.Add(this.chkLimitSelection);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(822, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(192, 609);
			this.panel1.TabIndex = 1;
			// 
			// chkLimitSelection
			// 
			this.chkLimitSelection.AutoSize = true;
			this.chkLimitSelection.Checked = true;
			this.chkLimitSelection.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkLimitSelection.Location = new System.Drawing.Point(6, 22);
			this.chkLimitSelection.Name = "chkLimitSelection";
			this.chkLimitSelection.Size = new System.Drawing.Size(172, 17);
			this.chkLimitSelection.TabIndex = 0;
			this.chkLimitSelection.Text = "Limit selection inside input cells";
			this.chkLimitSelection.UseVisualStyleBackColor = true;
			this.chkLimitSelection.CheckedChanged += new System.EventHandler(this.chkLimitSelection_CheckedChanged);
			// 
			// chkTabToNextBlock
			// 
			this.chkTabToNextBlock.AutoSize = true;
			this.chkTabToNextBlock.Checked = true;
			this.chkTabToNextBlock.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkTabToNextBlock.Location = new System.Drawing.Point(6, 57);
			this.chkTabToNextBlock.Name = "chkTabToNextBlock";
			this.chkTabToNextBlock.Size = new System.Drawing.Size(109, 17);
			this.chkTabToNextBlock.TabIndex = 0;
			this.chkTabToNextBlock.Text = "Tab to next block";
			this.chkTabToNextBlock.UseVisualStyleBackColor = true;
			this.chkTabToNextBlock.CheckedChanged += new System.EventHandler(this.chkLimitSelection_CheckedChanged);
			// 
			// CustomSelectionForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1014, 609);
			this.Controls.Add(this.grid);
			this.Controls.Add(this.panel1);
			this.Name = "CustomSelectionForm";
			this.Text = "Customize Selection";
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