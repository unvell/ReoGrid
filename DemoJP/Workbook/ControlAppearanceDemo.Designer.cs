/*****************************************************************************
 * 
 * ReoGrid - .NET 表計算スプレッドシートコンポーネント
 * https://reogrid.net/jp
 *
 * ReoGrid 日本語版デモプロジェクトは MIT ライセンスでリリースされています。
 * 
 * このソフトウェアは無保証であり、このソフトウェアの使用により生じた直接・間接の損害に対し、
 * 著作権者は補償を含むあらゆる責任を負いません。 
 * 
 * Copyright (c) 2012-2016 unvell.com, All Rights Reserved.
 * https://www.unvell.com/jp
 * 
 ****************************************************************************/

namespace unvell.ReoGrid.Demo.WorkbookDemo
{
	partial class ControlAppearanceDemo
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
			this.reoGridControl = new unvell.ReoGrid.ReoGridControl();
			this.panel1 = new System.Windows.Forms.Panel();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.highlightColorPickerControl = new unvell.UIControls.ColorPickerControl();
			this.label3 = new System.Windows.Forms.Label();
			this.mainColorPickerControl = new unvell.UIControls.ColorPickerControl();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.rdoSnowWhite = new System.Windows.Forms.RadioButton();
			this.rdoLightGreen = new System.Windows.Forms.RadioButton();
			this.rdoGoldenSilver = new System.Windows.Forms.RadioButton();
			this.btnExportVBNET = new System.Windows.Forms.Button();
			this.btnExportCSharp = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// reoGridControl
			// 
			this.reoGridControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.reoGridControl.ColumnHeaderContextMenuStrip = null;
			this.reoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.reoGridControl.LeadHeaderContextMenuStrip = null;
			this.reoGridControl.Location = new System.Drawing.Point(0, 69);
			this.reoGridControl.Name = "reoGridControl";
			this.reoGridControl.RowHeaderContextMenuStrip = null;
			this.reoGridControl.Script = null;
			this.reoGridControl.SheetTabContextMenuStrip = null;
			this.reoGridControl.SheetTabNewButtonVisible = true;
			this.reoGridControl.SheetTabVisible = true;
			this.reoGridControl.SheetTabWidth = 400;
			this.reoGridControl.ShowScrollEndSpacing = true;
			this.reoGridControl.Size = new System.Drawing.Size(795, 453);
			this.reoGridControl.TabIndex = 2;
			this.reoGridControl.Text = "reoGridControl1";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.groupBox2);
			this.panel1.Controls.Add(this.groupBox1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(795, 69);
			this.panel1.TabIndex = 3;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.btnExportVBNET);
			this.groupBox2.Controls.Add(this.btnExportCSharp);
			this.groupBox2.Controls.Add(this.highlightColorPickerControl);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.mainColorPickerControl);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Location = new System.Drawing.Point(270, 4);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(522, 56);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "テーマ色から作成";
			// 
			// highlightColorPickerControl
			// 
			this.highlightColorPickerControl.Location = new System.Drawing.Point(170, 22);
			this.highlightColorPickerControl.Name = "highlightColorPickerControl";
			this.highlightColorPickerControl.ShowShadow = false;
			this.highlightColorPickerControl.Size = new System.Drawing.Size(39, 22);
			this.highlightColorPickerControl.SolidColor = System.Drawing.Color.YellowGreen;
			this.highlightColorPickerControl.TabIndex = 12;
			this.highlightColorPickerControl.Text = "colorPickerControl3";
			this.highlightColorPickerControl.ColorPicked += new System.EventHandler(this.highlightColorPickerControl_ColorPicked);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(126, 27);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(37, 13);
			this.label3.TabIndex = 11;
			this.label3.Text = "強調：";
			// 
			// mainColorPickerControl
			// 
			this.mainColorPickerControl.Location = new System.Drawing.Point(66, 22);
			this.mainColorPickerControl.Name = "mainColorPickerControl";
			this.mainColorPickerControl.ShowShadow = false;
			this.mainColorPickerControl.Size = new System.Drawing.Size(39, 22);
			this.mainColorPickerControl.SolidColor = System.Drawing.Color.Silver;
			this.mainColorPickerControl.TabIndex = 10;
			this.mainColorPickerControl.Text = "colorPickerControl2";
			this.mainColorPickerControl.ColorPicked += new System.EventHandler(this.mainColorPickerControl_ColorPicked);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(23, 27);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(42, 13);
			this.label2.TabIndex = 9;
			this.label2.Text = "メイン： ";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.rdoSnowWhite);
			this.groupBox1.Controls.Add(this.rdoLightGreen);
			this.groupBox1.Controls.Add(this.rdoGoldenSilver);
			this.groupBox1.Location = new System.Drawing.Point(9, 4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(255, 56);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "外観スタイルサンプル";
			// 
			// rdoSnowWhite
			// 
			this.rdoSnowWhite.AutoSize = true;
			this.rdoSnowWhite.Location = new System.Drawing.Point(187, 25);
			this.rdoSnowWhite.Name = "rdoSnowWhite";
			this.rdoSnowWhite.Size = new System.Drawing.Size(52, 17);
			this.rdoSnowWhite.TabIndex = 2;
			this.rdoSnowWhite.TabStop = true;
			this.rdoSnowWhite.Text = "スノー";
			this.rdoSnowWhite.UseVisualStyleBackColor = true;
			this.rdoSnowWhite.CheckedChanged += new System.EventHandler(this.rdoSnowWhite_CheckedChanged);
			// 
			// rdoLightGreen
			// 
			this.rdoLightGreen.AutoSize = true;
			this.rdoLightGreen.Location = new System.Drawing.Point(111, 25);
			this.rdoLightGreen.Name = "rdoLightGreen";
			this.rdoLightGreen.Size = new System.Drawing.Size(60, 17);
			this.rdoLightGreen.TabIndex = 1;
			this.rdoLightGreen.TabStop = true;
			this.rdoLightGreen.Text = "グリーン";
			this.rdoLightGreen.UseVisualStyleBackColor = true;
			this.rdoLightGreen.CheckedChanged += new System.EventHandler(this.rdoLightGreen_CheckedChanged);
			// 
			// rdoGoldenSilver
			// 
			this.rdoGoldenSilver.AutoSize = true;
			this.rdoGoldenSilver.Location = new System.Drawing.Point(23, 25);
			this.rdoGoldenSilver.Name = "rdoGoldenSilver";
			this.rdoGoldenSilver.Size = new System.Drawing.Size(73, 17);
			this.rdoGoldenSilver.TabIndex = 0;
			this.rdoGoldenSilver.TabStop = true;
			this.rdoGoldenSilver.Text = "ゴールデン";
			this.rdoGoldenSilver.UseVisualStyleBackColor = true;
			this.rdoGoldenSilver.CheckedChanged += new System.EventHandler(this.rdoGoldenSilver_CheckedChanged);
			// 
			// btnExportVBNET
			// 
			this.btnExportVBNET.Location = new System.Drawing.Point(368, 19);
			this.btnExportVBNET.Name = "btnExportVBNET";
			this.btnExportVBNET.Size = new System.Drawing.Size(133, 28);
			this.btnExportVBNET.TabIndex = 16;
			this.btnExportVBNET.Text = "VB.NETにエクスポート";
			this.btnExportVBNET.UseVisualStyleBackColor = true;
			this.btnExportVBNET.Click += new System.EventHandler(this.btnExportVBNET_Click);
			// 
			// btnExportCSharp
			// 
			this.btnExportCSharp.Location = new System.Drawing.Point(227, 19);
			this.btnExportCSharp.Name = "btnExportCSharp";
			this.btnExportCSharp.Size = new System.Drawing.Size(133, 28);
			this.btnExportCSharp.TabIndex = 15;
			this.btnExportCSharp.Text = "C#にエクスポート";
			this.btnExportCSharp.UseVisualStyleBackColor = true;
			this.btnExportCSharp.Click += new System.EventHandler(this.btnExportCSharp_Click);
			// 
			// ControlAppearanceDemo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.reoGridControl);
			this.Controls.Add(this.panel1);
			this.Name = "ControlAppearanceDemo";
			this.Size = new System.Drawing.Size(795, 522);
			this.panel1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private ReoGridControl reoGridControl;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton rdoSnowWhite;
		private System.Windows.Forms.RadioButton rdoLightGreen;
		private System.Windows.Forms.RadioButton rdoGoldenSilver;
		private System.Windows.Forms.GroupBox groupBox2;
		private UIControls.ColorPickerControl highlightColorPickerControl;
		private System.Windows.Forms.Label label3;
		private UIControls.ColorPickerControl mainColorPickerControl;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnExportVBNET;
		private System.Windows.Forms.Button btnExportCSharp;
	}
}
