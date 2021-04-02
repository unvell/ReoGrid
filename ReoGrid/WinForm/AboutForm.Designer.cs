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
 * Author: Jing Lu <jingwood at unvell.com>
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

#if WINFORM

namespace unvell.ReoGrid.WinForm
{
	partial class AboutForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
			this.label1 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.lnkHP = new System.Windows.Forms.LinkLabel();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.labVersion = new System.Windows.Forms.Label();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(8, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(251, 18);
			this.label1.TabIndex = 0;
			this.label1.Text = "ReoGrid - .NET Spreadsheet Control";
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(465, 319);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 25);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			// 
			// lnkHP
			// 
			this.lnkHP.AutoSize = true;
			this.lnkHP.Location = new System.Drawing.Point(9, 44);
			this.lnkHP.Name = "lnkHP";
			this.lnkHP.Size = new System.Drawing.Size(102, 13);
			this.lnkHP.TabIndex = 2;
			this.lnkHP.TabStop = true;
			this.lnkHP.Text = "https://reogrid.net";
			// 
			// textBox1
			// 
			this.textBox1.BackColor = System.Drawing.SystemColors.Control;
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox1.Location = new System.Drawing.Point(12, 261);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(425, 92);
			this.textBox1.TabIndex = 3;
			this.textBox1.Text = resources.GetString("textBox1.Text");
			// 
			// labVersion
			// 
			this.labVersion.AutoSize = true;
			this.labVersion.Location = new System.Drawing.Point(289, 17);
			this.labVersion.Name = "labVersion";
			this.labVersion.Size = new System.Drawing.Size(68, 13);
			this.labVersion.TabIndex = 4;
			this.labVersion.Text = "version 0.8.5";
			// 
			// textBox2
			// 
			this.textBox2.BackColor = System.Drawing.SystemColors.Window;
			this.textBox2.Location = new System.Drawing.Point(11, 73);
			this.textBox2.Multiline = true;
			this.textBox2.Name = "textBox2";
			this.textBox2.ReadOnly = true;
			this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBox2.Size = new System.Drawing.Size(529, 178);
			this.textBox2.TabIndex = 3;
			this.textBox2.Text = resources.GetString("textBox2.Text");
			// 
			// AboutForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(552, 359);
			this.Controls.Add(this.labVersion);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.lnkHP);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.LinkLabel lnkHP;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label labVersion;
		private System.Windows.Forms.TextBox textBox2;
	}
}

#endif // WINFORM