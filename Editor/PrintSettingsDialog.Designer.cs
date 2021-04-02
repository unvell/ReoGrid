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
	partial class PrintSettingsDialog
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
			this.grpPaper = new System.Windows.Forms.GroupBox();
			this.btnPaperSetup = new System.Windows.Forms.Button();
			this.txtPaperOrientation = new System.Windows.Forms.TextBox();
			this.txtPaperSize = new System.Windows.Forms.TextBox();
			this.labOrientation = new System.Windows.Forms.Label();
			this.labPaperSize = new System.Windows.Forms.Label();
			this.grpScaling = new System.Windows.Forms.GroupBox();
			this.numScale = new System.Windows.Forms.NumericUpDown();
			this.labPercentNormalSize = new System.Windows.Forms.Label();
			this.labAdjustTo = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.grpPagingOrder = new System.Windows.Forms.GroupBox();
			this.rdoDownThenOver = new System.Windows.Forms.RadioButton();
			this.rdoOverThenDown = new System.Windows.Forms.RadioButton();
			this.grpVisibleSettings = new System.Windows.Forms.GroupBox();
			this.chkShowMargins = new System.Windows.Forms.CheckBox();
			this.chkPrintGridLines = new System.Windows.Forms.CheckBox();
			this.grpPaper.SuspendLayout();
			this.grpScaling.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numScale)).BeginInit();
			this.grpPagingOrder.SuspendLayout();
			this.grpVisibleSettings.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpPaper
			// 
			this.grpPaper.Controls.Add(this.btnPaperSetup);
			this.grpPaper.Controls.Add(this.txtPaperOrientation);
			this.grpPaper.Controls.Add(this.txtPaperSize);
			this.grpPaper.Controls.Add(this.labOrientation);
			this.grpPaper.Controls.Add(this.labPaperSize);
			this.grpPaper.Location = new System.Drawing.Point(12, 13);
			this.grpPaper.Name = "grpPaper";
			this.grpPaper.Size = new System.Drawing.Size(300, 124);
			this.grpPaper.TabIndex = 0;
			this.grpPaper.TabStop = false;
			this.grpPaper.Text = "Paper";
			// 
			// btnPaperSetup
			// 
			this.btnPaperSetup.Location = new System.Drawing.Point(208, 84);
			this.btnPaperSetup.Name = "btnPaperSetup";
			this.btnPaperSetup.Size = new System.Drawing.Size(75, 25);
			this.btnPaperSetup.TabIndex = 2;
			this.btnPaperSetup.Text = "Setup";
			this.btnPaperSetup.UseVisualStyleBackColor = true;
			this.btnPaperSetup.Click += new System.EventHandler(this.btnPaperSetup_Click);
			// 
			// txtPaperOrientation
			// 
			this.txtPaperOrientation.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtPaperOrientation.Location = new System.Drawing.Point(100, 59);
			this.txtPaperOrientation.Name = "txtPaperOrientation";
			this.txtPaperOrientation.ReadOnly = true;
			this.txtPaperOrientation.Size = new System.Drawing.Size(180, 13);
			this.txtPaperOrientation.TabIndex = 1;
			// 
			// txtPaperSize
			// 
			this.txtPaperSize.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtPaperSize.Location = new System.Drawing.Point(100, 31);
			this.txtPaperSize.Name = "txtPaperSize";
			this.txtPaperSize.ReadOnly = true;
			this.txtPaperSize.Size = new System.Drawing.Size(180, 13);
			this.txtPaperSize.TabIndex = 1;
			// 
			// labOrientation
			// 
			this.labOrientation.AutoSize = true;
			this.labOrientation.Location = new System.Drawing.Point(21, 59);
			this.labOrientation.Name = "labOrientation";
			this.labOrientation.Size = new System.Drawing.Size(61, 13);
			this.labOrientation.TabIndex = 0;
			this.labOrientation.Text = "Orientation:";
			// 
			// labPaperSize
			// 
			this.labPaperSize.AutoSize = true;
			this.labPaperSize.Location = new System.Drawing.Point(21, 31);
			this.labPaperSize.Name = "labPaperSize";
			this.labPaperSize.Size = new System.Drawing.Size(61, 13);
			this.labPaperSize.TabIndex = 0;
			this.labPaperSize.Text = "Paper Size:";
			// 
			// grpScaling
			// 
			this.grpScaling.Controls.Add(this.numScale);
			this.grpScaling.Controls.Add(this.labPercentNormalSize);
			this.grpScaling.Controls.Add(this.labAdjustTo);
			this.grpScaling.Location = new System.Drawing.Point(12, 229);
			this.grpScaling.Name = "grpScaling";
			this.grpScaling.Size = new System.Drawing.Size(300, 66);
			this.grpScaling.TabIndex = 0;
			this.grpScaling.TabStop = false;
			this.grpScaling.Text = "Scaling";
			// 
			// numScale
			// 
			this.numScale.Location = new System.Drawing.Point(94, 31);
			this.numScale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numScale.Name = "numScale";
			this.numScale.Size = new System.Drawing.Size(56, 20);
			this.numScale.TabIndex = 1;
			this.numScale.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			// 
			// labPercentNormalSize
			// 
			this.labPercentNormalSize.AutoSize = true;
			this.labPercentNormalSize.Location = new System.Drawing.Point(160, 33);
			this.labPercentNormalSize.Name = "labPercentNormalSize";
			this.labPercentNormalSize.Size = new System.Drawing.Size(70, 13);
			this.labPercentNormalSize.TabIndex = 0;
			this.labPercentNormalSize.Text = "% normal size";
			// 
			// labAdjustTo
			// 
			this.labAdjustTo.AutoSize = true;
			this.labAdjustTo.Location = new System.Drawing.Point(32, 33);
			this.labAdjustTo.Name = "labAdjustTo";
			this.labAdjustTo.Size = new System.Drawing.Size(51, 13);
			this.labAdjustTo.TabIndex = 0;
			this.labAdjustTo.Text = "Adjust to ";
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(343, 301);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 25);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(424, 301);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 25);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// grpPagingOrder
			// 
			this.grpPagingOrder.Controls.Add(this.rdoDownThenOver);
			this.grpPagingOrder.Controls.Add(this.rdoOverThenDown);
			this.grpPagingOrder.Location = new System.Drawing.Point(12, 143);
			this.grpPagingOrder.Name = "grpPagingOrder";
			this.grpPagingOrder.Size = new System.Drawing.Size(300, 80);
			this.grpPagingOrder.TabIndex = 2;
			this.grpPagingOrder.TabStop = false;
			this.grpPagingOrder.Text = "Paging Order";
			// 
			// rdoDownThenOver
			// 
			this.rdoDownThenOver.AutoSize = true;
			this.rdoDownThenOver.Location = new System.Drawing.Point(35, 38);
			this.rdoDownThenOver.Name = "rdoDownThenOver";
			this.rdoDownThenOver.Size = new System.Drawing.Size(104, 17);
			this.rdoDownThenOver.TabIndex = 0;
			this.rdoDownThenOver.TabStop = true;
			this.rdoDownThenOver.Text = "Down, then over";
			this.rdoDownThenOver.UseVisualStyleBackColor = true;
			// 
			// rdoOverThenDown
			// 
			this.rdoOverThenDown.AutoSize = true;
			this.rdoOverThenDown.Location = new System.Drawing.Point(163, 38);
			this.rdoOverThenDown.Name = "rdoOverThenDown";
			this.rdoOverThenDown.Size = new System.Drawing.Size(104, 17);
			this.rdoOverThenDown.TabIndex = 0;
			this.rdoOverThenDown.TabStop = true;
			this.rdoOverThenDown.Text = "Over, then down";
			this.rdoOverThenDown.UseVisualStyleBackColor = true;
			// 
			// grpVisibleSettings
			// 
			this.grpVisibleSettings.Controls.Add(this.chkShowMargins);
			this.grpVisibleSettings.Controls.Add(this.chkPrintGridLines);
			this.grpVisibleSettings.Location = new System.Drawing.Point(318, 13);
			this.grpVisibleSettings.Name = "grpVisibleSettings";
			this.grpVisibleSettings.Size = new System.Drawing.Size(181, 124);
			this.grpVisibleSettings.TabIndex = 3;
			this.grpVisibleSettings.TabStop = false;
			this.grpVisibleSettings.Text = "Visible Settings";
			// 
			// chkShowMargins
			// 
			this.chkShowMargins.AutoSize = true;
			this.chkShowMargins.Location = new System.Drawing.Point(18, 30);
			this.chkShowMargins.Name = "chkShowMargins";
			this.chkShowMargins.Size = new System.Drawing.Size(93, 17);
			this.chkShowMargins.TabIndex = 1;
			this.chkShowMargins.Text = "Show Margins";
			this.chkShowMargins.UseVisualStyleBackColor = true;
			// 
			// chkPrintGridLines
			// 
			this.chkPrintGridLines.AutoSize = true;
			this.chkPrintGridLines.Location = new System.Drawing.Point(18, 53);
			this.chkPrintGridLines.Name = "chkPrintGridLines";
			this.chkPrintGridLines.Size = new System.Drawing.Size(103, 17);
			this.chkPrintGridLines.TabIndex = 0;
			this.chkPrintGridLines.Text = "Show Grid Lines";
			this.chkPrintGridLines.UseVisualStyleBackColor = true;
			// 
			// PrintSettingsDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(511, 335);
			this.ControlBox = false;
			this.Controls.Add(this.grpVisibleSettings);
			this.Controls.Add(this.grpPagingOrder);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.grpScaling);
			this.Controls.Add(this.grpPaper);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PrintSettingsDialog";
			this.Text = "Print Settings";
			this.grpPaper.ResumeLayout(false);
			this.grpPaper.PerformLayout();
			this.grpScaling.ResumeLayout(false);
			this.grpScaling.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numScale)).EndInit();
			this.grpPagingOrder.ResumeLayout(false);
			this.grpPagingOrder.PerformLayout();
			this.grpVisibleSettings.ResumeLayout(false);
			this.grpVisibleSettings.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox grpPaper;
		private System.Windows.Forms.Button btnPaperSetup;
		private System.Windows.Forms.TextBox txtPaperOrientation;
		private System.Windows.Forms.TextBox txtPaperSize;
		private System.Windows.Forms.Label labOrientation;
		private System.Windows.Forms.Label labPaperSize;
		private System.Windows.Forms.GroupBox grpScaling;
		private System.Windows.Forms.NumericUpDown numScale;
		private System.Windows.Forms.Label labAdjustTo;
		private System.Windows.Forms.Label labPercentNormalSize;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.GroupBox grpPagingOrder;
		private System.Windows.Forms.RadioButton rdoDownThenOver;
		private System.Windows.Forms.RadioButton rdoOverThenDown;
		private System.Windows.Forms.GroupBox grpVisibleSettings;
		private System.Windows.Forms.CheckBox chkPrintGridLines;
		private System.Windows.Forms.CheckBox chkShowMargins;
	}
}