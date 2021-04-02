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

namespace unvell.ReoGrid.Demo.Performance
{
	partial class RowPerformanceDemo
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
			this.button4 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.button5 = new System.Windows.Forms.Button();
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
			this.grid.Size = new System.Drawing.Size(630, 699);
			this.grid.TabIndex = 8;
			this.grid.Text = "reoGridControl1";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.button5);
			this.panel1.Controls.Add(this.button4);
			this.panel1.Controls.Add(this.button3);
			this.panel1.Controls.Add(this.button2);
			this.panel1.Controls.Add(this.button1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(630, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(188, 699);
			this.panel1.TabIndex = 9;
			// 
			// button4
			// 
			this.button4.Enabled = false;
			this.button4.Location = new System.Drawing.Point(6, 133);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(175, 37);
			this.button4.TabIndex = 6;
			this.button4.Text = "Fill 100,000 rows\r\n";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// button3
			// 
			this.button3.Enabled = false;
			this.button3.Location = new System.Drawing.Point(6, 90);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(175, 37);
			this.button3.TabIndex = 5;
			this.button3.Text = "Append to 100,000 rows";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// button2
			// 
			this.button2.Enabled = false;
			this.button2.Location = new System.Drawing.Point(6, 49);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(175, 35);
			this.button2.TabIndex = 4;
			this.button2.Text = "Fill 10000 rows";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(6, 8);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(175, 35);
			this.button1.TabIndex = 3;
			this.button1.Text = "Append to 10000 rows";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button5
			// 
			this.button5.Enabled = false;
			this.button5.Location = new System.Drawing.Point(6, 176);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(175, 46);
			this.button5.TabIndex = 7;
			this.button5.Text = "Fill 100,000 rows\r\n (with suspending events)";
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new System.EventHandler(this.button5_Click);
			// 
			// ManyRowTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(818, 699);
			this.Controls.Add(this.grid);
			this.Controls.Add(this.panel1);
			this.Name = "ManyRowTestForm";
			this.Text = "Many Row Test Form";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private ReoGridControl grid;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button5;
	}
}