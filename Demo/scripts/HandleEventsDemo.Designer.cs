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

namespace unvell.ReoGrid.Demo.Scripts
{
	partial class HandleEventsDemo
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

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.chkOnload = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.chkSelectionMoveNext = new System.Windows.Forms.CheckBox();
			this.chkOnCut = new System.Windows.Forms.CheckBox();
			this.chkOnPaste = new System.Windows.Forms.CheckBox();
			this.chkOnCopy = new System.Windows.Forms.CheckBox();
			this.chkUnload = new System.Windows.Forms.CheckBox();
			this.chkCellDataChanged = new System.Windows.Forms.CheckBox();
			this.chkCellBeforeEdit = new System.Windows.Forms.CheckBox();
			this.chkSelectionChange = new System.Windows.Forms.CheckBox();
			this.chkCellMouseUp = new System.Windows.Forms.CheckBox();
			this.chkCellMouseDown = new System.Windows.Forms.CheckBox();
			this.btnReset = new System.Windows.Forms.Button();
			this.listbox1 = new System.Windows.Forms.ListBox();
			this.grid = new unvell.ReoGrid.ReoGridControl();
			this.panel1 = new System.Windows.Forms.Panel();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.groupBox1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// chkOnload
			// 
			this.chkOnload.AutoSize = true;
			this.chkOnload.Location = new System.Drawing.Point(18, 29);
			this.chkOnload.Name = "chkOnload";
			this.chkOnload.Size = new System.Drawing.Size(58, 17);
			this.chkOnload.TabIndex = 2;
			this.chkOnload.Text = "onload";
			this.chkOnload.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.chkSelectionMoveNext);
			this.groupBox1.Controls.Add(this.chkOnCut);
			this.groupBox1.Controls.Add(this.chkOnPaste);
			this.groupBox1.Controls.Add(this.chkOnCopy);
			this.groupBox1.Controls.Add(this.chkUnload);
			this.groupBox1.Controls.Add(this.chkCellDataChanged);
			this.groupBox1.Controls.Add(this.chkCellBeforeEdit);
			this.groupBox1.Controls.Add(this.chkSelectionChange);
			this.groupBox1.Controls.Add(this.chkCellMouseUp);
			this.groupBox1.Controls.Add(this.chkCellMouseDown);
			this.groupBox1.Controls.Add(this.chkOnload);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
			this.groupBox1.Location = new System.Drawing.Point(4, 4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(185, 388);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Events";
			// 
			// chkSelectionMoveNext
			// 
			this.chkSelectionMoveNext.AutoSize = true;
			this.chkSelectionMoveNext.Location = new System.Drawing.Point(18, 98);
			this.chkSelectionMoveNext.Name = "chkSelectionMoveNext";
			this.chkSelectionMoveNext.Size = new System.Drawing.Size(84, 17);
			this.chkSelectionMoveNext.TabIndex = 12;
			this.chkSelectionMoveNext.Text = "onnextfocus";
			this.chkSelectionMoveNext.UseVisualStyleBackColor = true;
			// 
			// chkOnCut
			// 
			this.chkOnCut.AutoSize = true;
			this.chkOnCut.Location = new System.Drawing.Point(18, 259);
			this.chkOnCut.Name = "chkOnCut";
			this.chkOnCut.Size = new System.Drawing.Size(53, 17);
			this.chkOnCut.TabIndex = 11;
			this.chkOnCut.Text = "oncut";
			this.chkOnCut.UseVisualStyleBackColor = true;
			// 
			// chkOnPaste
			// 
			this.chkOnPaste.AutoSize = true;
			this.chkOnPaste.Location = new System.Drawing.Point(18, 236);
			this.chkOnPaste.Name = "chkOnPaste";
			this.chkOnPaste.Size = new System.Drawing.Size(64, 17);
			this.chkOnPaste.TabIndex = 10;
			this.chkOnPaste.Text = "onpaste";
			this.chkOnPaste.UseVisualStyleBackColor = true;
			// 
			// chkOnCopy
			// 
			this.chkOnCopy.AutoSize = true;
			this.chkOnCopy.Location = new System.Drawing.Point(18, 213);
			this.chkOnCopy.Name = "chkOnCopy";
			this.chkOnCopy.Size = new System.Drawing.Size(61, 17);
			this.chkOnCopy.TabIndex = 9;
			this.chkOnCopy.Text = "oncopy";
			this.chkOnCopy.UseVisualStyleBackColor = true;
			// 
			// chkUnload
			// 
			this.chkUnload.AutoSize = true;
			this.chkUnload.Location = new System.Drawing.Point(18, 52);
			this.chkUnload.Name = "chkUnload";
			this.chkUnload.Size = new System.Drawing.Size(58, 17);
			this.chkUnload.TabIndex = 8;
			this.chkUnload.Text = "unload";
			this.chkUnload.UseVisualStyleBackColor = true;
			// 
			// chkCellDataChanged
			// 
			this.chkCellDataChanged.AutoSize = true;
			this.chkCellDataChanged.Location = new System.Drawing.Point(18, 190);
			this.chkCellDataChanged.Name = "chkCellDataChanged";
			this.chkCellDataChanged.Size = new System.Drawing.Size(95, 17);
			this.chkCellDataChanged.TabIndex = 7;
			this.chkCellDataChanged.Text = "ondatachange";
			this.chkCellDataChanged.UseVisualStyleBackColor = true;
			// 
			// chkCellBeforeEdit
			// 
			this.chkCellBeforeEdit.AutoSize = true;
			this.chkCellBeforeEdit.Location = new System.Drawing.Point(18, 167);
			this.chkCellBeforeEdit.Name = "chkCellBeforeEdit";
			this.chkCellBeforeEdit.Size = new System.Drawing.Size(71, 17);
			this.chkCellBeforeEdit.TabIndex = 6;
			this.chkCellBeforeEdit.Text = "oncelledit";
			this.chkCellBeforeEdit.UseVisualStyleBackColor = true;
			// 
			// chkSelectionChange
			// 
			this.chkSelectionChange.AutoSize = true;
			this.chkSelectionChange.Location = new System.Drawing.Point(18, 75);
			this.chkSelectionChange.Name = "chkSelectionChange";
			this.chkSelectionChange.Size = new System.Drawing.Size(116, 17);
			this.chkSelectionChange.TabIndex = 5;
			this.chkSelectionChange.Text = "onselectionchange";
			this.chkSelectionChange.UseVisualStyleBackColor = true;
			// 
			// chkCellMouseUp
			// 
			this.chkCellMouseUp.AutoSize = true;
			this.chkCellMouseUp.Location = new System.Drawing.Point(18, 144);
			this.chkCellMouseUp.Name = "chkCellMouseUp";
			this.chkCellMouseUp.Size = new System.Drawing.Size(81, 17);
			this.chkCellMouseUp.TabIndex = 4;
			this.chkCellMouseUp.Text = "onmouseup";
			this.chkCellMouseUp.UseVisualStyleBackColor = true;
			// 
			// chkCellMouseDown
			// 
			this.chkCellMouseDown.AutoSize = true;
			this.chkCellMouseDown.Location = new System.Drawing.Point(18, 121);
			this.chkCellMouseDown.Name = "chkCellMouseDown";
			this.chkCellMouseDown.Size = new System.Drawing.Size(95, 17);
			this.chkCellMouseDown.TabIndex = 3;
			this.chkCellMouseDown.Text = "onmousedown";
			this.chkCellMouseDown.UseVisualStyleBackColor = true;
			// 
			// btnReset
			// 
			this.btnReset.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.btnReset.Location = new System.Drawing.Point(4, 392);
			this.btnReset.Name = "btnReset";
			this.btnReset.Size = new System.Drawing.Size(451, 32);
			this.btnReset.TabIndex = 4;
			this.btnReset.Text = "Reset Control";
			this.btnReset.UseVisualStyleBackColor = true;
			this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
			// 
			// listbox1
			// 
			this.listbox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listbox1.FormattingEnabled = true;
			this.listbox1.Location = new System.Drawing.Point(189, 4);
			this.listbox1.Name = "listbox1";
			this.listbox1.Size = new System.Drawing.Size(266, 388);
			this.listbox1.TabIndex = 5;
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
			this.grid.SheetTabWidth = 382;
			this.grid.Size = new System.Drawing.Size(422, 428);
			this.grid.TabIndex = 1;
			this.grid.Text = "reoGridControl1";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.listbox1);
			this.panel1.Controls.Add(this.groupBox1);
			this.panel1.Controls.Add(this.btnReset);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(426, 0);
			this.panel1.Name = "panel1";
			this.panel1.Padding = new System.Windows.Forms.Padding(4);
			this.panel1.Size = new System.Drawing.Size(459, 428);
			this.panel1.TabIndex = 6;
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
			this.splitter1.Location = new System.Drawing.Point(422, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(4, 428);
			this.splitter1.TabIndex = 7;
			this.splitter1.TabStop = false;
			// 
			// HandleEventsDemo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.grid);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.panel1);
			this.Name = "HandleEventsDemo";
			this.Size = new System.Drawing.Size(885, 428);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private ReoGridControl grid;
		private System.Windows.Forms.CheckBox chkOnload;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button btnReset;
		private System.Windows.Forms.CheckBox chkCellMouseDown;
		private System.Windows.Forms.ListBox listbox1;
		private System.Windows.Forms.CheckBox chkCellMouseUp;
		private System.Windows.Forms.CheckBox chkSelectionChange;
		private System.Windows.Forms.CheckBox chkUnload;
		private System.Windows.Forms.CheckBox chkCellDataChanged;
		private System.Windows.Forms.CheckBox chkCellBeforeEdit;
		private System.Windows.Forms.CheckBox chkSelectionMoveNext;
		private System.Windows.Forms.CheckBox chkOnCut;
		private System.Windows.Forms.CheckBox chkOnPaste;
		private System.Windows.Forms.CheckBox chkOnCopy;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Splitter splitter1;
	}
}