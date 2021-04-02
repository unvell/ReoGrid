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
	partial class FormulaBarControl
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
			this.leftPanel = new System.Windows.Forms.Panel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.splitterDown = new System.Windows.Forms.Splitter();
			this.splitterUp = new System.Windows.Forms.Splitter();
			this.panel3 = new System.Windows.Forms.Panel();
			this.txtFormula = new System.Windows.Forms.TextBox();
			this.addressField = new unvell.ReoGrid.Editor.AddressFieldControl();
			this.leftPanel.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.panel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// leftPanel
			// 
			this.leftPanel.BackColor = System.Drawing.SystemColors.Control;
			this.leftPanel.Controls.Add(this.panel1);
			this.leftPanel.Dock = System.Windows.Forms.DockStyle.Left;
			this.leftPanel.Location = new System.Drawing.Point(0, 1);
			this.leftPanel.Name = "leftPanel";
			this.leftPanel.Size = new System.Drawing.Size(251, 177);
			this.leftPanel.TabIndex = 5;
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.Control;
			this.panel1.Controls.Add(this.addressField);
			this.panel1.Controls.Add(this.panel2);
			this.panel1.Controls.Add(this.pictureBox1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.panel1.Size = new System.Drawing.Size(251, 21);
			this.panel1.TabIndex = 5;
			// 
			// panel2
			// 
			this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel2.Location = new System.Drawing.Point(180, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(41, 21);
			this.panel2.TabIndex = 10;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Right;
			this.pictureBox1.Image = global::unvell.ReoGrid.Editor.Properties.Resources.FunctionHS;
			this.pictureBox1.Location = new System.Drawing.Point(221, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(29, 21);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// splitterDown
			// 
			this.splitterDown.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitterDown.Location = new System.Drawing.Point(0, 178);
			this.splitterDown.Name = "splitterDown";
			this.splitterDown.Size = new System.Drawing.Size(781, 4);
			this.splitterDown.TabIndex = 9;
			this.splitterDown.TabStop = false;
			// 
			// splitterUp
			// 
			this.splitterUp.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitterUp.Location = new System.Drawing.Point(0, 0);
			this.splitterUp.Name = "splitterUp";
			this.splitterUp.Size = new System.Drawing.Size(781, 1);
			this.splitterUp.TabIndex = 10;
			this.splitterUp.TabStop = false;
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.txtFormula);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(251, 1);
			this.panel3.Name = "panel3";
			this.panel3.Padding = new System.Windows.Forms.Padding(3, 1, 1, 1);
			this.panel3.Size = new System.Drawing.Size(530, 177);
			this.panel3.TabIndex = 11;
			// 
			// txtFormula
			// 
			this.txtFormula.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtFormula.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtFormula.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtFormula.Location = new System.Drawing.Point(3, 1);
			this.txtFormula.Multiline = true;
			this.txtFormula.Name = "txtFormula";
			this.txtFormula.Size = new System.Drawing.Size(526, 175);
			this.txtFormula.TabIndex = 9;
			// 
			// addressField
			// 
			this.addressField.BackColor = System.Drawing.SystemColors.Window;
			this.addressField.Dock = System.Windows.Forms.DockStyle.Fill;
			this.addressField.GridControl = null;
			this.addressField.Location = new System.Drawing.Point(0, 0);
			this.addressField.Name = "addressField";
			this.addressField.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
			this.addressField.Size = new System.Drawing.Size(180, 21);
			this.addressField.TabIndex = 7;
			// 
			// FormulaBarControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.leftPanel);
			this.Controls.Add(this.splitterDown);
			this.Controls.Add(this.splitterUp);
			this.Name = "FormulaBarControl";
			this.Size = new System.Drawing.Size(781, 182);
			this.leftPanel.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel leftPanel;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private AddressFieldControl addressField;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Splitter splitterDown;
		private System.Windows.Forms.Splitter splitterUp;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.TextBox txtFormula;

	}
}
