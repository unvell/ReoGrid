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
	partial class ClipboardEventDemo
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
			this.chkPreventPasteEvent = new System.Windows.Forms.CheckBox();
			this.chkCustomizePaste = new System.Windows.Forms.CheckBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// grid
			// 
			this.grid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grid.Location = new System.Drawing.Point(0, 0);
			this.grid.Name = "grid";
			this.grid.Size = new System.Drawing.Size(680, 421);
			this.grid.TabIndex = 2;
			this.grid.TabStop = true;
			this.grid.Text = "reoGridControl1";
			// 
			// chkPreventPasteEvent
			// 
			this.chkPreventPasteEvent.AutoSize = true;
			this.chkPreventPasteEvent.Location = new System.Drawing.Point(22, 23);
			this.chkPreventPasteEvent.Name = "chkPreventPasteEvent";
			this.chkPreventPasteEvent.Size = new System.Drawing.Size(127, 16);
			this.chkPreventPasteEvent.TabIndex = 3;
			this.chkPreventPasteEvent.Text = "Prevent paste event";
			this.chkPreventPasteEvent.UseVisualStyleBackColor = true;
			// 
			// chkCustomizePaste
			// 
			this.chkCustomizePaste.AutoSize = true;
			this.chkCustomizePaste.Location = new System.Drawing.Point(22, 58);
			this.chkCustomizePaste.Name = "chkCustomizePaste";
			this.chkCustomizePaste.Size = new System.Drawing.Size(110, 16);
			this.chkCustomizePaste.TabIndex = 4;
			this.chkCustomizePaste.Text = "Customize Paste";
			this.chkCustomizePaste.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.chkCustomizePaste);
			this.panel1.Controls.Add(this.chkPreventPasteEvent);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(680, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(190, 421);
			this.panel1.TabIndex = 5;
			// 
			// ClipboardEventForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(870, 421);
			this.Controls.Add(this.grid);
			this.Controls.Add(this.panel1);
			this.Name = "ClipboardEventForm";
			this.Text = "ClipboardEventForm";
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private ReoGridControl grid;
		private System.Windows.Forms.CheckBox chkPreventPasteEvent;
		private System.Windows.Forms.CheckBox chkCustomizePaste;
		private System.Windows.Forms.Panel panel1;
	}
}