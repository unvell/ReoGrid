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
	partial class UpdateDataFormatDemo
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
			this.btnFormatAsNumber = new System.Windows.Forms.Button();
			this.grid = new unvell.ReoGrid.ReoGridControl();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnFormatAsNumber
			// 
			this.btnFormatAsNumber.Location = new System.Drawing.Point(12, 12);
			this.btnFormatAsNumber.Name = "btnFormatAsNumber";
			this.btnFormatAsNumber.Size = new System.Drawing.Size(211, 30);
			this.btnFormatAsNumber.TabIndex = 3;
			this.btnFormatAsNumber.Text = "Set to Number Format (0000.1234)";
			this.btnFormatAsNumber.UseVisualStyleBackColor = true;
			this.btnFormatAsNumber.Click += new System.EventHandler(this.btnFormatAsNumber_Click);
			// 
			// grid
			// 
			this.grid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grid.Location = new System.Drawing.Point(0, 0);
			this.grid.Name = "grid";
			this.grid.Size = new System.Drawing.Size(733, 571);
			this.grid.TabIndex = 2;
			this.grid.TabStop = true;
			this.grid.Text = "reoGridControl1";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(12, 84);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(211, 30);
			this.button1.TabIndex = 4;
			this.button1.Text = "Set to Percent (00.12%)";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(12, 48);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(211, 30);
			this.button2.TabIndex = 4;
			this.button2.Text = "Set to Date (yyyy/MM/dd)";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(12, 120);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(211, 30);
			this.button4.TabIndex = 5;
			this.button4.Text = "Set to Curreny ($ 000.0)";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(12, 156);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(211, 30);
			this.button3.TabIndex = 6;
			this.button3.Text = "Set to Text";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.button3);
			this.panel1.Controls.Add(this.button4);
			this.panel1.Controls.Add(this.button2);
			this.panel1.Controls.Add(this.button1);
			this.panel1.Controls.Add(this.btnFormatAsNumber);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(733, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(232, 571);
			this.panel1.TabIndex = 7;
			// 
			// DataFormatForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(965, 571);
			this.Controls.Add(this.grid);
			this.Controls.Add(this.panel1);
			this.Name = "DataFormatForm";
			this.Text = "DataFormatForm";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private ReoGridControl grid;
		private System.Windows.Forms.Button btnFormatAsNumber;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Panel panel1;
	}
}