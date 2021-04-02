/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net/
 *
 * Font Settings Control Panel - Windows Standard Font Dialog-like control
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

using System.Windows.Forms;
using unvell.UIControls;

namespace unvell.UIControls
{
	partial class FontSettingsControl
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
			this.labFont = new System.Windows.Forms.Label();
			this.txtFont = new System.Windows.Forms.TextBox();
			this.labStyle = new System.Windows.Forms.Label();
			this.txtStyle = new System.Windows.Forms.TextBox();
			this.labSize = new System.Windows.Forms.Label();
			this.txtSize = new System.Windows.Forms.TextBox();
			this.styleList = new System.Windows.Forms.ListBox();
			this.sizeList = new System.Windows.Forms.ListBox();
			this.labColor = new System.Windows.Forms.Label();
			this.sampleGroup = new System.Windows.Forms.GroupBox();
			this.labSample = new System.Windows.Forms.Label();
			this.chkStrikeout = new System.Windows.Forms.CheckBox();
			this.chkUnderline = new System.Windows.Forms.CheckBox();
			this.fontColor = new unvell.UIControls.ColorComboBox();
			this.fontList = new unvell.UIControls.FontListBox();
			this.sampleGroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// labFont
			// 
			this.labFont.AutoSize = true;
			this.labFont.Location = new System.Drawing.Point(10, 10);
			this.labFont.Name = "labFont";
			this.labFont.Size = new System.Drawing.Size(28, 12);
			this.labFont.TabIndex = 0;
			this.labFont.Text = "Font";
			// 
			// txtFont
			// 
			this.txtFont.Location = new System.Drawing.Point(13, 25);
			this.txtFont.Name = "txtFont";
			this.txtFont.Size = new System.Drawing.Size(250, 19);
			this.txtFont.TabIndex = 2;
			// 
			// labStyle
			// 
			this.labStyle.AutoSize = true;
			this.labStyle.Location = new System.Drawing.Point(268, 10);
			this.labStyle.Name = "labStyle";
			this.labStyle.Size = new System.Drawing.Size(31, 12);
			this.labStyle.TabIndex = 0;
			this.labStyle.Text = "Style";
			// 
			// txtStyle
			// 
			this.txtStyle.Location = new System.Drawing.Point(271, 25);
			this.txtStyle.Name = "txtStyle";
			this.txtStyle.Size = new System.Drawing.Size(115, 19);
			this.txtStyle.TabIndex = 2;
			// 
			// labSize
			// 
			this.labSize.AutoSize = true;
			this.labSize.Location = new System.Drawing.Point(392, 10);
			this.labSize.Name = "labSize";
			this.labSize.Size = new System.Drawing.Size(26, 12);
			this.labSize.TabIndex = 0;
			this.labSize.Text = "Size";
			// 
			// txtSize
			// 
			this.txtSize.Location = new System.Drawing.Point(395, 25);
			this.txtSize.Name = "txtSize";
			this.txtSize.Size = new System.Drawing.Size(76, 19);
			this.txtSize.TabIndex = 2;
			// 
			// styleList
			// 
			this.styleList.FormattingEnabled = true;
			this.styleList.ItemHeight = 12;
			this.styleList.Location = new System.Drawing.Point(271, 44);
			this.styleList.Name = "styleList";
			this.styleList.Size = new System.Drawing.Size(115, 112);
			this.styleList.TabIndex = 3;
			// 
			// sizeList
			// 
			this.sizeList.FormattingEnabled = true;
			this.sizeList.ItemHeight = 12;
			this.sizeList.Location = new System.Drawing.Point(395, 44);
			this.sizeList.Name = "sizeList";
			this.sizeList.Size = new System.Drawing.Size(76, 112);
			this.sizeList.TabIndex = 4;
			// 
			// labColor
			// 
			this.labColor.Location = new System.Drawing.Point(271, 171);
			this.labColor.Name = "labColor";
			this.labColor.Size = new System.Drawing.Size(57, 21);
			this.labColor.TabIndex = 6;
			this.labColor.Text = "Color:";
			this.labColor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// sampleGroup
			// 
			this.sampleGroup.Controls.Add(this.labSample);
			this.sampleGroup.Location = new System.Drawing.Point(114, 209);
			this.sampleGroup.Name = "sampleGroup";
			this.sampleGroup.Size = new System.Drawing.Size(357, 81);
			this.sampleGroup.TabIndex = 7;
			this.sampleGroup.TabStop = false;
			this.sampleGroup.Text = "Sample";
			// 
			// labSample
			// 
			this.labSample.Location = new System.Drawing.Point(6, 17);
			this.labSample.Name = "labSample";
			this.labSample.Size = new System.Drawing.Size(345, 59);
			this.labSample.TabIndex = 0;
			this.labSample.Text = "ReoGrid サンプル";
			// 
			// chkStrikeout
			// 
			this.chkStrikeout.AutoSize = true;
			this.chkStrikeout.Location = new System.Drawing.Point(16, 209);
			this.chkStrikeout.Name = "chkStrikeout";
			this.chkStrikeout.Size = new System.Drawing.Size(92, 16);
			this.chkStrikeout.TabIndex = 8;
			this.chkStrikeout.Text = "&Strikethrough";
			this.chkStrikeout.UseVisualStyleBackColor = true;
			// 
			// chkUnderline
			// 
			this.chkUnderline.AutoSize = true;
			this.chkUnderline.Location = new System.Drawing.Point(16, 230);
			this.chkUnderline.Name = "chkUnderline";
			this.chkUnderline.Size = new System.Drawing.Size(72, 16);
			this.chkUnderline.TabIndex = 9;
			this.chkUnderline.Text = "Underline";
			this.chkUnderline.UseVisualStyleBackColor = true;
			// 
			// fontColor
			// 
			this.fontColor.BackColor = System.Drawing.Color.White;
			this.fontColor.dropdown = false;
			this.fontColor.Location = new System.Drawing.Point(334, 171);
			this.fontColor.Name = "fontColor";
			this.fontColor.Size = new System.Drawing.Size(137, 21);
			this.fontColor.SolidColor = System.Drawing.Color.Empty;
			this.fontColor.TabIndex = 5;
			this.fontColor.Text = "colorComboBox1";
			// 
			// fontList
			// 
			this.fontList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.fontList.FormattingEnabled = true;
			this.fontList.ItemHeight = 26;
			this.fontList.Location = new System.Drawing.Point(13, 44);
			this.fontList.Name = "fontList";
			this.fontList.Size = new System.Drawing.Size(250, 134);
			this.fontList.TabIndex = 1;
			// 
			// FontSettingsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.chkUnderline);
			this.Controls.Add(this.chkStrikeout);
			this.Controls.Add(this.sampleGroup);
			this.Controls.Add(this.labColor);
			this.Controls.Add(this.fontColor);
			this.Controls.Add(this.sizeList);
			this.Controls.Add(this.styleList);
			this.Controls.Add(this.txtSize);
			this.Controls.Add(this.txtStyle);
			this.Controls.Add(this.labSize);
			this.Controls.Add(this.txtFont);
			this.Controls.Add(this.labStyle);
			this.Controls.Add(this.fontList);
			this.Controls.Add(this.labFont);
			this.Name = "FontSettingsControl";
			this.Size = new System.Drawing.Size(536, 320);
			this.sampleGroup.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labFont;
		private FontListBox fontList;
		private System.Windows.Forms.TextBox txtFont;
		private System.Windows.Forms.Label labStyle;
		private System.Windows.Forms.TextBox txtStyle;
		private System.Windows.Forms.Label labSize;
		private System.Windows.Forms.TextBox txtSize;
		private System.Windows.Forms.ListBox styleList;
		private System.Windows.Forms.ListBox sizeList;
		private unvell.UIControls.ColorComboBox fontColor;
		private System.Windows.Forms.Label labColor;
		private System.Windows.Forms.GroupBox sampleGroup;
		private System.Windows.Forms.CheckBox chkStrikeout;
		private System.Windows.Forms.CheckBox chkUnderline;
		private Label labSample;
	}
}
