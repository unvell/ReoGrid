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
	partial class FormatPage
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
			this.formatList = new System.Windows.Forms.ListBox();
			this.labCategory = new System.Windows.Forms.Label();
			this.grpSample = new System.Windows.Forms.GroupBox();
			this.labSample = new System.Windows.Forms.Label();
			this.labType = new System.Windows.Forms.Label();
			this.datetimeFormatList = new System.Windows.Forms.ListBox();
			this.labLocale = new System.Windows.Forms.Label();
			this.datetimeLocationList = new System.Windows.Forms.ComboBox();
			this.labDecimalPlacesNum = new System.Windows.Forms.Label();
			this.numberDecimalPlaces = new System.Windows.Forms.NumericUpDown();
			this.labNegativeNumbersNum = new System.Windows.Forms.Label();
			this.numberPanel = new System.Windows.Forms.Panel();
			this.chkNumberUseSeparator = new System.Windows.Forms.CheckBox();
			this.currencyPanel = new System.Windows.Forms.Panel();
			this.currencySymbolList = new System.Windows.Forms.ComboBox();
			this.labSymbol = new System.Windows.Forms.Label();
			this.labNegativeNumberCurrency = new System.Windows.Forms.Label();
			this.currencyDecimalPlaces = new System.Windows.Forms.NumericUpDown();
			this.labDecimalPlacesCurrency = new System.Windows.Forms.Label();
			this.datetimePanel = new System.Windows.Forms.Panel();
			this.txtDatetimeFormat = new System.Windows.Forms.TextBox();
			this.labDateTimePattern = new System.Windows.Forms.Label();
			this.percentPanel = new System.Windows.Forms.Panel();
			this.percentDecimalPlaces = new System.Windows.Forms.NumericUpDown();
			this.labDecimalPlacesPercent = new System.Windows.Forms.Label();
			this.currencyNegativeStyleList = new unvell.UIControls.ColoredListBox();
			this.numberNegativeStyleList = new unvell.UIControls.ColoredListBox();
			this.grpSample.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numberDecimalPlaces)).BeginInit();
			this.numberPanel.SuspendLayout();
			this.currencyPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.currencyDecimalPlaces)).BeginInit();
			this.datetimePanel.SuspendLayout();
			this.percentPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.percentDecimalPlaces)).BeginInit();
			this.SuspendLayout();
			// 
			// formatList
			// 
			this.formatList.FormattingEnabled = true;
			this.formatList.Location = new System.Drawing.Point(16, 35);
			this.formatList.Name = "formatList";
			this.formatList.ScrollAlwaysVisible = true;
			this.formatList.Size = new System.Drawing.Size(133, 277);
			this.formatList.TabIndex = 3;
			// 
			// labCategory
			// 
			this.labCategory.AutoSize = true;
			this.labCategory.Location = new System.Drawing.Point(13, 13);
			this.labCategory.Name = "labCategory";
			this.labCategory.Size = new System.Drawing.Size(52, 13);
			this.labCategory.TabIndex = 2;
			this.labCategory.Text = "Category:";
			// 
			// grpSample
			// 
			this.grpSample.Controls.Add(this.labSample);
			this.grpSample.Location = new System.Drawing.Point(165, 18);
			this.grpSample.Name = "grpSample";
			this.grpSample.Size = new System.Drawing.Size(377, 50);
			this.grpSample.TabIndex = 4;
			this.grpSample.TabStop = false;
			this.grpSample.Text = "Sample";
			// 
			// labSample
			// 
			this.labSample.Location = new System.Drawing.Point(15, 23);
			this.labSample.Name = "labSample";
			this.labSample.Size = new System.Drawing.Size(349, 20);
			this.labSample.TabIndex = 0;
			// 
			// labType
			// 
			this.labType.AutoSize = true;
			this.labType.Location = new System.Drawing.Point(4, 29);
			this.labType.Name = "labType";
			this.labType.Size = new System.Drawing.Size(34, 13);
			this.labType.TabIndex = 2;
			this.labType.Text = "&Type:";
			// 
			// datetimeFormatList
			// 
			this.datetimeFormatList.FormattingEnabled = true;
			this.datetimeFormatList.Location = new System.Drawing.Point(6, 46);
			this.datetimeFormatList.Name = "datetimeFormatList";
			this.datetimeFormatList.ScrollAlwaysVisible = true;
			this.datetimeFormatList.Size = new System.Drawing.Size(377, 134);
			this.datetimeFormatList.TabIndex = 3;
			// 
			// labLocale
			// 
			this.labLocale.AutoSize = true;
			this.labLocale.Location = new System.Drawing.Point(3, 189);
			this.labLocale.Name = "labLocale";
			this.labLocale.Size = new System.Drawing.Size(88, 13);
			this.labLocale.TabIndex = 2;
			this.labLocale.Text = "&Locale (location):";
			// 
			// datetimeLocationList
			// 
			this.datetimeLocationList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.datetimeLocationList.FormattingEnabled = true;
			this.datetimeLocationList.Location = new System.Drawing.Point(6, 206);
			this.datetimeLocationList.Name = "datetimeLocationList";
			this.datetimeLocationList.Size = new System.Drawing.Size(377, 21);
			this.datetimeLocationList.TabIndex = 5;
			// 
			// labDecimalPlacesNum
			// 
			this.labDecimalPlacesNum.AutoSize = true;
			this.labDecimalPlacesNum.Location = new System.Drawing.Point(3, 5);
			this.labDecimalPlacesNum.Name = "labDecimalPlacesNum";
			this.labDecimalPlacesNum.Size = new System.Drawing.Size(83, 13);
			this.labDecimalPlacesNum.TabIndex = 6;
			this.labDecimalPlacesNum.Text = "&Decimal Places:";
			// 
			// numberDecimalPlaces
			// 
			this.numberDecimalPlaces.Location = new System.Drawing.Point(125, 3);
			this.numberDecimalPlaces.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
			this.numberDecimalPlaces.Name = "numberDecimalPlaces";
			this.numberDecimalPlaces.Size = new System.Drawing.Size(59, 20);
			this.numberDecimalPlaces.TabIndex = 7;
			this.numberDecimalPlaces.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			// 
			// labNegativeNumbersNum
			// 
			this.labNegativeNumbersNum.AutoSize = true;
			this.labNegativeNumbersNum.Location = new System.Drawing.Point(3, 59);
			this.labNegativeNumbersNum.Name = "labNegativeNumbersNum";
			this.labNegativeNumbersNum.Size = new System.Drawing.Size(96, 13);
			this.labNegativeNumbersNum.TabIndex = 6;
			this.labNegativeNumbersNum.Text = "&Negative numbers:";
			// 
			// numberPanel
			// 
			this.numberPanel.Controls.Add(this.labNegativeNumbersNum);
			this.numberPanel.Controls.Add(this.numberNegativeStyleList);
			this.numberPanel.Controls.Add(this.chkNumberUseSeparator);
			this.numberPanel.Controls.Add(this.numberDecimalPlaces);
			this.numberPanel.Controls.Add(this.labDecimalPlacesNum);
			this.numberPanel.Location = new System.Drawing.Point(668, 3);
			this.numberPanel.Name = "numberPanel";
			this.numberPanel.Size = new System.Drawing.Size(393, 199);
			this.numberPanel.TabIndex = 8;
			// 
			// chkNumberUseSeparator
			// 
			this.chkNumberUseSeparator.AutoSize = true;
			this.chkNumberUseSeparator.Checked = true;
			this.chkNumberUseSeparator.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkNumberUseSeparator.Location = new System.Drawing.Point(6, 33);
			this.chkNumberUseSeparator.Name = "chkNumberUseSeparator";
			this.chkNumberUseSeparator.Size = new System.Drawing.Size(133, 17);
			this.chkNumberUseSeparator.TabIndex = 8;
			this.chkNumberUseSeparator.Text = "&Use 1000 Separator (,)";
			this.chkNumberUseSeparator.UseVisualStyleBackColor = true;
			// 
			// currencyPanel
			// 
			this.currencyPanel.Controls.Add(this.currencySymbolList);
			this.currencyPanel.Controls.Add(this.labSymbol);
			this.currencyPanel.Controls.Add(this.labNegativeNumberCurrency);
			this.currencyPanel.Controls.Add(this.currencyNegativeStyleList);
			this.currencyPanel.Controls.Add(this.currencyDecimalPlaces);
			this.currencyPanel.Controls.Add(this.labDecimalPlacesCurrency);
			this.currencyPanel.Location = new System.Drawing.Point(668, 209);
			this.currencyPanel.Name = "currencyPanel";
			this.currencyPanel.Size = new System.Drawing.Size(393, 184);
			this.currencyPanel.TabIndex = 8;
			// 
			// currencySymbolList
			// 
			this.currencySymbolList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.currencySymbolList.FormattingEnabled = true;
			this.currencySymbolList.Location = new System.Drawing.Point(53, 30);
			this.currencySymbolList.Name = "currencySymbolList";
			this.currencySymbolList.Size = new System.Drawing.Size(330, 21);
			this.currencySymbolList.TabIndex = 11;
			// 
			// labSymbol
			// 
			this.labSymbol.AutoSize = true;
			this.labSymbol.Location = new System.Drawing.Point(3, 33);
			this.labSymbol.Name = "labSymbol";
			this.labSymbol.Size = new System.Drawing.Size(44, 13);
			this.labSymbol.TabIndex = 10;
			this.labSymbol.Text = "&Symbol:";
			// 
			// labNegativeNumberCurrency
			// 
			this.labNegativeNumberCurrency.AutoSize = true;
			this.labNegativeNumberCurrency.Location = new System.Drawing.Point(3, 59);
			this.labNegativeNumberCurrency.Name = "labNegativeNumberCurrency";
			this.labNegativeNumberCurrency.Size = new System.Drawing.Size(96, 13);
			this.labNegativeNumberCurrency.TabIndex = 6;
			this.labNegativeNumberCurrency.Text = "&Negative numbers:";
			// 
			// currencyDecimalPlaces
			// 
			this.currencyDecimalPlaces.Location = new System.Drawing.Point(125, 3);
			this.currencyDecimalPlaces.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
			this.currencyDecimalPlaces.Name = "currencyDecimalPlaces";
			this.currencyDecimalPlaces.Size = new System.Drawing.Size(59, 20);
			this.currencyDecimalPlaces.TabIndex = 7;
			this.currencyDecimalPlaces.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			// 
			// labDecimalPlacesCurrency
			// 
			this.labDecimalPlacesCurrency.AutoSize = true;
			this.labDecimalPlacesCurrency.Location = new System.Drawing.Point(3, 5);
			this.labDecimalPlacesCurrency.Name = "labDecimalPlacesCurrency";
			this.labDecimalPlacesCurrency.Size = new System.Drawing.Size(83, 13);
			this.labDecimalPlacesCurrency.TabIndex = 6;
			this.labDecimalPlacesCurrency.Text = "&Decimal Places:";
			// 
			// datetimePanel
			// 
			this.datetimePanel.Controls.Add(this.txtDatetimeFormat);
			this.datetimePanel.Controls.Add(this.labDateTimePattern);
			this.datetimePanel.Controls.Add(this.datetimeFormatList);
			this.datetimePanel.Controls.Add(this.labLocale);
			this.datetimePanel.Controls.Add(this.datetimeLocationList);
			this.datetimePanel.Controls.Add(this.labType);
			this.datetimePanel.Location = new System.Drawing.Point(668, 400);
			this.datetimePanel.Name = "datetimePanel";
			this.datetimePanel.Size = new System.Drawing.Size(393, 237);
			this.datetimePanel.TabIndex = 8;
			// 
			// txtDatetimeFormat
			// 
			this.txtDatetimeFormat.Location = new System.Drawing.Point(84, 2);
			this.txtDatetimeFormat.Name = "txtDatetimeFormat";
			this.txtDatetimeFormat.Size = new System.Drawing.Size(299, 20);
			this.txtDatetimeFormat.TabIndex = 7;
			// 
			// labDateTimePattern
			// 
			this.labDateTimePattern.AutoSize = true;
			this.labDateTimePattern.Location = new System.Drawing.Point(4, 5);
			this.labDateTimePattern.Name = "labDateTimePattern";
			this.labDateTimePattern.Size = new System.Drawing.Size(44, 13);
			this.labDateTimePattern.TabIndex = 6;
			this.labDateTimePattern.Text = "&Pattern:";
			// 
			// percentPanel
			// 
			this.percentPanel.Controls.Add(this.percentDecimalPlaces);
			this.percentPanel.Controls.Add(this.labDecimalPlacesPercent);
			this.percentPanel.Location = new System.Drawing.Point(668, 644);
			this.percentPanel.Name = "percentPanel";
			this.percentPanel.Size = new System.Drawing.Size(393, 38);
			this.percentPanel.TabIndex = 8;
			// 
			// percentDecimalPlaces
			// 
			this.percentDecimalPlaces.Location = new System.Drawing.Point(125, 3);
			this.percentDecimalPlaces.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
			this.percentDecimalPlaces.Name = "percentDecimalPlaces";
			this.percentDecimalPlaces.Size = new System.Drawing.Size(59, 20);
			this.percentDecimalPlaces.TabIndex = 7;
			// 
			// labDecimalPlacesPercent
			// 
			this.labDecimalPlacesPercent.AutoSize = true;
			this.labDecimalPlacesPercent.Location = new System.Drawing.Point(3, 5);
			this.labDecimalPlacesPercent.Name = "labDecimalPlacesPercent";
			this.labDecimalPlacesPercent.Size = new System.Drawing.Size(83, 13);
			this.labDecimalPlacesPercent.TabIndex = 6;
			this.labDecimalPlacesPercent.Text = "&Decimal Places:";
			// 
			// currencyNegativeStyleList
			// 
			this.currencyNegativeStyleList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.currencyNegativeStyleList.FormattingEnabled = true;
			this.currencyNegativeStyleList.Location = new System.Drawing.Point(6, 76);
			this.currencyNegativeStyleList.Name = "currencyNegativeStyleList";
			this.currencyNegativeStyleList.Size = new System.Drawing.Size(377, 95);
			this.currencyNegativeStyleList.TabIndex = 9;
			// 
			// numberNegativeStyleList
			// 
			this.numberNegativeStyleList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.numberNegativeStyleList.FormattingEnabled = true;
			this.numberNegativeStyleList.Location = new System.Drawing.Point(6, 76);
			this.numberNegativeStyleList.Name = "numberNegativeStyleList";
			this.numberNegativeStyleList.Size = new System.Drawing.Size(377, 95);
			this.numberNegativeStyleList.TabIndex = 9;
			// 
			// FormatPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.datetimePanel);
			this.Controls.Add(this.percentPanel);
			this.Controls.Add(this.currencyPanel);
			this.Controls.Add(this.numberPanel);
			this.Controls.Add(this.grpSample);
			this.Controls.Add(this.formatList);
			this.Controls.Add(this.labCategory);
			this.Name = "FormatPage";
			this.Size = new System.Drawing.Size(1076, 707);
			this.grpSample.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numberDecimalPlaces)).EndInit();
			this.numberPanel.ResumeLayout(false);
			this.numberPanel.PerformLayout();
			this.currencyPanel.ResumeLayout(false);
			this.currencyPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.currencyDecimalPlaces)).EndInit();
			this.datetimePanel.ResumeLayout(false);
			this.datetimePanel.PerformLayout();
			this.percentPanel.ResumeLayout(false);
			this.percentPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.percentDecimalPlaces)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox formatList;
		private System.Windows.Forms.Label labCategory;
		private System.Windows.Forms.GroupBox grpSample;
		private System.Windows.Forms.Label labSample;
		private System.Windows.Forms.Label labType;
		private System.Windows.Forms.ListBox datetimeFormatList;
		private System.Windows.Forms.Label labLocale;
		private System.Windows.Forms.ComboBox datetimeLocationList;
		private System.Windows.Forms.Label labDecimalPlacesNum;
		private System.Windows.Forms.Label labNegativeNumbersNum;
		private System.Windows.Forms.NumericUpDown numberDecimalPlaces;
		private System.Windows.Forms.Panel numberPanel;
		private System.Windows.Forms.CheckBox chkNumberUseSeparator;
		private ColoredListBox numberNegativeStyleList;
		private System.Windows.Forms.Panel currencyPanel;
		private System.Windows.Forms.Label labNegativeNumberCurrency;
		private ColoredListBox currencyNegativeStyleList;
		private System.Windows.Forms.NumericUpDown currencyDecimalPlaces;
		private System.Windows.Forms.Label labDecimalPlacesCurrency;
		private System.Windows.Forms.ComboBox currencySymbolList;
		private System.Windows.Forms.Label labSymbol;
		private System.Windows.Forms.Panel datetimePanel;
		private System.Windows.Forms.Panel percentPanel;
		private System.Windows.Forms.NumericUpDown percentDecimalPlaces;
		private System.Windows.Forms.Label labDecimalPlacesPercent;
		private System.Windows.Forms.TextBox txtDatetimeFormat;
		private System.Windows.Forms.Label labDateTimePattern;
	}
}
