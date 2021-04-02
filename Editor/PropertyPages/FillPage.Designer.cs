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

using unvell.UIControls;

namespace unvell.ReoGrid.PropertyPages
{
	partial class FillPage
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
			this.grpSample = new System.Windows.Forms.GroupBox();
			this.labSample = new System.Windows.Forms.Label();
			this.labPatternColor = new System.Windows.Forms.Label();
			this.labBackgroundColor = new System.Windows.Forms.Label();
			this.labPatternStyle = new System.Windows.Forms.Label();
			this.grpPattern = new System.Windows.Forms.GroupBox();
			this.patternStyleComboBox = new unvell.UIControls.FillPatternComboBox();
			this.patternColorComboBox = new unvell.UIControls.ColorComboBox();
			this.colorPanel = new unvell.UIControls.ColorPickerPanel();
			this.grpSample.SuspendLayout();
			this.grpPattern.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpSample
			// 
			this.grpSample.Controls.Add(this.labSample);
			this.grpSample.Location = new System.Drawing.Point(13, 270);
			this.grpSample.Name = "grpSample";
			this.grpSample.Size = new System.Drawing.Size(485, 68);
			this.grpSample.TabIndex = 1;
			this.grpSample.TabStop = false;
			this.grpSample.Text = "Sample";
			// 
			// labSample
			// 
			this.labSample.Location = new System.Drawing.Point(11, 17);
			this.labSample.Name = "labSample";
			this.labSample.Size = new System.Drawing.Size(461, 39);
			this.labSample.TabIndex = 0;
			// 
			// labPatternColor
			// 
			this.labPatternColor.AutoSize = true;
			this.labPatternColor.Location = new System.Drawing.Point(28, 31);
			this.labPatternColor.Name = "labPatternColor";
			this.labPatternColor.Size = new System.Drawing.Size(71, 13);
			this.labPatternColor.TabIndex = 3;
			this.labPatternColor.Text = "Pattern &Color:";
			// 
			// labBackgroundColor
			// 
			this.labBackgroundColor.AutoSize = true;
			this.labBackgroundColor.Location = new System.Drawing.Point(10, 11);
			this.labBackgroundColor.Name = "labBackgroundColor";
			this.labBackgroundColor.Size = new System.Drawing.Size(95, 13);
			this.labBackgroundColor.TabIndex = 4;
			this.labBackgroundColor.Text = "&Background Color:";
			// 
			// labPatternStyle
			// 
			this.labPatternStyle.AutoSize = true;
			this.labPatternStyle.Location = new System.Drawing.Point(28, 91);
			this.labPatternStyle.Name = "labPatternStyle";
			this.labPatternStyle.Size = new System.Drawing.Size(70, 13);
			this.labPatternStyle.TabIndex = 3;
			this.labPatternStyle.Text = "&Pattern Style:";
			// 
			// grpPattern
			// 
			this.grpPattern.Controls.Add(this.patternStyleComboBox);
			this.grpPattern.Controls.Add(this.patternColorComboBox);
			this.grpPattern.Controls.Add(this.labPatternColor);
			this.grpPattern.Controls.Add(this.labPatternStyle);
			this.grpPattern.Location = new System.Drawing.Point(270, 31);
			this.grpPattern.Name = "grpPattern";
			this.grpPattern.Size = new System.Drawing.Size(228, 151);
			this.grpPattern.TabIndex = 6;
			this.grpPattern.TabStop = false;
			this.grpPattern.Text = "Fill Pattern";
			// 
			// patternStyleComboBox
			// 
			this.patternStyleComboBox.BackColor = System.Drawing.Color.White;
			this.patternStyleComboBox.CloseOnClick = false;
			this.patternStyleComboBox.dropdown = false;
			this.patternStyleComboBox.Location = new System.Drawing.Point(28, 107);
			this.patternStyleComboBox.Name = "patternStyleComboBox";
			this.patternStyleComboBox.PatternColor = System.Drawing.Color.Black;
			this.patternStyleComboBox.PatternStyle = System.Drawing.Drawing2D.HatchStyle.Horizontal;
			this.patternStyleComboBox.Size = new System.Drawing.Size(170, 23);
			this.patternStyleComboBox.TabIndex = 5;
			this.patternStyleComboBox.Text = "fillPatternComboBox1";
			// 
			// patternColorComboBox
			// 
			this.patternColorComboBox.BackColor = System.Drawing.Color.White;
			this.patternColorComboBox.CloseOnClick = true;
			this.patternColorComboBox.dropdown = false;
			this.patternColorComboBox.Location = new System.Drawing.Point(28, 51);
			this.patternColorComboBox.Name = "patternColorComboBox";
			this.patternColorComboBox.Size = new System.Drawing.Size(170, 23);
			this.patternColorComboBox.SolidColor = System.Drawing.Color.Black;
			this.patternColorComboBox.TabIndex = 2;
			this.patternColorComboBox.Text = "colorComboBox1";
			// 
			// colorPanel
			// 
			this.colorPanel.BackColor = System.Drawing.SystemColors.Window;
			this.colorPanel.Location = new System.Drawing.Point(13, 31);
			this.colorPanel.Margin = new System.Windows.Forms.Padding(1);
			this.colorPanel.Name = "colorPanel";
			this.colorPanel.Padding = new System.Windows.Forms.Padding(1);
			this.colorPanel.Size = new System.Drawing.Size(173, 230);
			this.colorPanel.SolidColor = System.Drawing.Color.Empty;
			this.colorPanel.TabIndex = 0;
			this.colorPanel.TabStop = false;
			this.colorPanel.Text = "colorPickPanel1";
			// 
			// FillPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.grpPattern);
			this.Controls.Add(this.labBackgroundColor);
			this.Controls.Add(this.grpSample);
			this.Controls.Add(this.colorPanel);
			this.Name = "FillPage";
			this.Size = new System.Drawing.Size(554, 359);
			this.grpSample.ResumeLayout(false);
			this.grpPattern.ResumeLayout(false);
			this.grpPattern.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private unvell.UIControls.ColorPickerPanel colorPanel;
		private System.Windows.Forms.GroupBox grpSample;
		private System.Windows.Forms.Label labSample;
		private ColorComboBox patternColorComboBox;
		private System.Windows.Forms.Label labPatternColor;
		private System.Windows.Forms.Label labBackgroundColor;
		private System.Windows.Forms.Label labPatternStyle;
		private FillPatternComboBox patternStyleComboBox;
		private System.Windows.Forms.GroupBox grpPattern;
	}
}
