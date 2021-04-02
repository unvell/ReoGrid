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
	partial class ControlAppearanceEditorForm
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
			this.btnExportCSharp = new System.Windows.Forms.Button();
			this.lstColors = new System.Windows.Forms.ListBox();
			this.btnReset = new System.Windows.Forms.Button();
			this.chkUseSystemHighlight = new System.Windows.Forms.CheckBox();
			this.labHighligh = new System.Windows.Forms.Label();
			this.labMain = new System.Windows.Forms.Label();
			this.numSelectionWidth = new System.Windows.Forms.NumericUpDown();
			this.labSelectionBorderWidth = new System.Windows.Forms.Label();
			this.labPixel = new System.Windows.Forms.Label();
			this.btnExportVBNET = new System.Windows.Forms.Button();
			this.highlightColorPickerControl = new unvell.UIControls.ColorPickerControl();
			this.mainColorPickerControl = new unvell.UIControls.ColorPickerControl();
			this.colorPickerControl = new unvell.UIControls.ColorComboBox();
			((System.ComponentModel.ISupportInitialize)(this.numSelectionWidth)).BeginInit();
			this.SuspendLayout();
			// 
			// btnExportCSharp
			// 
			this.btnExportCSharp.Location = new System.Drawing.Point(280, 166);
			this.btnExportCSharp.Name = "btnExportCSharp";
			this.btnExportCSharp.Size = new System.Drawing.Size(128, 28);
			this.btnExportCSharp.TabIndex = 7;
			this.btnExportCSharp.Text = "Export C#";
			this.btnExportCSharp.UseVisualStyleBackColor = true;
			this.btnExportCSharp.Click += new System.EventHandler(this.btnExportCSharp_Click);
			// 
			// lstColors
			// 
			this.lstColors.FormattingEnabled = true;
			this.lstColors.Location = new System.Drawing.Point(13, 54);
			this.lstColors.Name = "lstColors";
			this.lstColors.Size = new System.Drawing.Size(231, 147);
			this.lstColors.TabIndex = 0;
			// 
			// btnReset
			// 
			this.btnReset.Location = new System.Drawing.Point(280, 131);
			this.btnReset.Name = "btnReset";
			this.btnReset.Size = new System.Drawing.Size(128, 29);
			this.btnReset.TabIndex = 10;
			this.btnReset.Text = "Reset to Default";
			this.btnReset.UseVisualStyleBackColor = true;
			this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
			// 
			// chkUseSystemHighlight
			// 
			this.chkUseSystemHighlight.AutoSize = true;
			this.chkUseSystemHighlight.Location = new System.Drawing.Point(265, 20);
			this.chkUseSystemHighlight.Name = "chkUseSystemHighlight";
			this.chkUseSystemHighlight.Size = new System.Drawing.Size(153, 17);
			this.chkUseSystemHighlight.TabIndex = 9;
			this.chkUseSystemHighlight.Text = "Use System Highlight Color";
			this.chkUseSystemHighlight.UseVisualStyleBackColor = true;
			this.chkUseSystemHighlight.CheckedChanged += new System.EventHandler(this.chkUseSystemHighlight_CheckedChanged);
			// 
			// labHighligh
			// 
			this.labHighligh.AutoSize = true;
			this.labHighligh.Location = new System.Drawing.Point(135, 22);
			this.labHighligh.Name = "labHighligh";
			this.labHighligh.Size = new System.Drawing.Size(54, 13);
			this.labHighligh.TabIndex = 7;
			this.labHighligh.Text = "Highlight: ";
			// 
			// labMain
			// 
			this.labMain.AutoSize = true;
			this.labMain.Location = new System.Drawing.Point(10, 22);
			this.labMain.Name = "labMain";
			this.labMain.Size = new System.Drawing.Size(36, 13);
			this.labMain.TabIndex = 5;
			this.labMain.Text = "Main: ";
			// 
			// numSelectionWidth
			// 
			this.numSelectionWidth.Location = new System.Drawing.Point(297, 81);
			this.numSelectionWidth.Name = "numSelectionWidth";
			this.numSelectionWidth.Size = new System.Drawing.Size(54, 20);
			this.numSelectionWidth.TabIndex = 11;
			this.numSelectionWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// labSelectionBorderWidth
			// 
			this.labSelectionBorderWidth.AutoSize = true;
			this.labSelectionBorderWidth.Location = new System.Drawing.Point(281, 59);
			this.labSelectionBorderWidth.Name = "labSelectionBorderWidth";
			this.labSelectionBorderWidth.Size = new System.Drawing.Size(119, 13);
			this.labSelectionBorderWidth.TabIndex = 12;
			this.labSelectionBorderWidth.Text = "Selection Border Width:";
			// 
			// labPixel
			// 
			this.labPixel.AutoSize = true;
			this.labPixel.Location = new System.Drawing.Point(357, 87);
			this.labPixel.Name = "labPixel";
			this.labPixel.Size = new System.Drawing.Size(29, 13);
			this.labPixel.TabIndex = 13;
			this.labPixel.Text = "Pixel";
			// 
			// btnExportVBNET
			// 
			this.btnExportVBNET.Location = new System.Drawing.Point(280, 200);
			this.btnExportVBNET.Name = "btnExportVBNET";
			this.btnExportVBNET.Size = new System.Drawing.Size(128, 28);
			this.btnExportVBNET.TabIndex = 14;
			this.btnExportVBNET.Text = "Export VB.NET";
			this.btnExportVBNET.UseVisualStyleBackColor = true;
			this.btnExportVBNET.Click += new System.EventHandler(this.btnExportVBNET_Click);
			// 
			// highlightColorPickerControl
			// 
			this.highlightColorPickerControl.Location = new System.Drawing.Point(204, 17);
			this.highlightColorPickerControl.Name = "highlightColorPickerControl";
			this.highlightColorPickerControl.ShowShadow = false;
			this.highlightColorPickerControl.Size = new System.Drawing.Size(40, 23);
			this.highlightColorPickerControl.SolidColor = System.Drawing.Color.YellowGreen;
			this.highlightColorPickerControl.TabIndex = 8;
			this.highlightColorPickerControl.Text = "colorPickerControl3";
			this.highlightColorPickerControl.ColorPicked += new System.EventHandler(this.highlightColorPickerControl_ColorPicked);
			// 
			// mainColorPickerControl
			// 
			this.mainColorPickerControl.Location = new System.Drawing.Point(74, 17);
			this.mainColorPickerControl.Name = "mainColorPickerControl";
			this.mainColorPickerControl.ShowShadow = false;
			this.mainColorPickerControl.Size = new System.Drawing.Size(40, 23);
			this.mainColorPickerControl.SolidColor = System.Drawing.Color.Silver;
			this.mainColorPickerControl.TabIndex = 6;
			this.mainColorPickerControl.Text = "colorPickerControl2";
			this.mainColorPickerControl.ColorPicked += new System.EventHandler(this.mainColorPickerControl_ColorPicked);
			// 
			// colorPickerControl
			// 
			this.colorPickerControl.BackColor = System.Drawing.Color.White;
			this.colorPickerControl.CloseOnClick = true;
			this.colorPickerControl.dropdown = false;
			this.colorPickerControl.Location = new System.Drawing.Point(13, 207);
			this.colorPickerControl.Name = "colorPickerControl";
			this.colorPickerControl.Size = new System.Drawing.Size(231, 23);
			this.colorPickerControl.SolidColor = System.Drawing.Color.Empty;
			this.colorPickerControl.TabIndex = 1;
			this.colorPickerControl.Text = "colorComboBox1";
			// 
			// ControlAppearanceEditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(439, 244);
			this.Controls.Add(this.btnExportVBNET);
			this.Controls.Add(this.labPixel);
			this.Controls.Add(this.labSelectionBorderWidth);
			this.Controls.Add(this.numSelectionWidth);
			this.Controls.Add(this.btnExportCSharp);
			this.Controls.Add(this.highlightColorPickerControl);
			this.Controls.Add(this.chkUseSystemHighlight);
			this.Controls.Add(this.labHighligh);
			this.Controls.Add(this.btnReset);
			this.Controls.Add(this.mainColorPickerControl);
			this.Controls.Add(this.labMain);
			this.Controls.Add(this.colorPickerControl);
			this.Controls.Add(this.lstColors);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ControlAppearanceEditorForm";
			this.Text = "Control Appearance Editor";
			((System.ComponentModel.ISupportInitialize)(this.numSelectionWidth)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox lstColors;
		private System.Windows.Forms.CheckBox chkUseSystemHighlight;
		private unvell.UIControls.ColorPickerControl highlightColorPickerControl;
		private System.Windows.Forms.Label labHighligh;
		private unvell.UIControls.ColorPickerControl mainColorPickerControl;
		private System.Windows.Forms.Label labMain;
		private System.Windows.Forms.Button btnExportCSharp;
		private System.Windows.Forms.Button btnReset;
		private unvell.UIControls.ColorComboBox colorPickerControl;
		private System.Windows.Forms.NumericUpDown numSelectionWidth;
		private System.Windows.Forms.Label labSelectionBorderWidth;
		private System.Windows.Forms.Label labPixel;
		private System.Windows.Forms.Button btnExportVBNET;
	}
}