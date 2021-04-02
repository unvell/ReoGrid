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
	partial class DebugForm
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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.labTop = new System.Windows.Forms.Label();
			this.labCenter = new System.Windows.Forms.Label();
			this.labRight = new System.Windows.Forms.Label();
			this.labDown = new System.Windows.Forms.Label();
			this.labLeft = new System.Windows.Forms.Label();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.labBorderTop = new System.Windows.Forms.Label();
			this.labCenter2 = new System.Windows.Forms.Label();
			this.labBorderRight = new System.Windows.Forms.Label();
			this.labBorderDown = new System.Windows.Forms.Label();
			this.labBorderLeft = new System.Windows.Forms.Label();
			this.tabPage5 = new System.Windows.Forms.TabPage();
			this.layout = new unvell.ReoGrid.WinForm.RGCellLayoutGraphControl();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.labFormatArgs = new System.Windows.Forms.Label();
			this.labFormat = new System.Windows.Forms.Label();
			this.tabPage4 = new System.Windows.Forms.TabPage();
			this.txtLog = new System.Windows.Forms.TextBox();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.statusToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.tabPage5.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.tabPage4.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage5);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Controls.Add(this.tabPage4);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(2, 2);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(342, 246);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.tableLayoutPanel1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(334, 220);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Grid";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 3;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.26072F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.26072F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.47856F));
			this.tableLayoutPanel1.Controls.Add(this.labTop, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.labCenter, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.labRight, 2, 1);
			this.tableLayoutPanel1.Controls.Add(this.labDown, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.labLeft, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.26072F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.26072F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.47856F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(334, 220);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// labTop
			// 
			this.labTop.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labTop.ForeColor = System.Drawing.SystemColors.GrayText;
			this.labTop.Location = new System.Drawing.Point(114, 0);
			this.labTop.Name = "labTop";
			this.labTop.Size = new System.Drawing.Size(105, 73);
			this.labTop.TabIndex = 2;
			this.labTop.Text = "label2";
			this.labTop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labCenter
			// 
			this.labCenter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labCenter.Location = new System.Drawing.Point(114, 73);
			this.labCenter.Name = "labCenter";
			this.labCenter.Size = new System.Drawing.Size(105, 73);
			this.labCenter.TabIndex = 3;
			this.labCenter.Text = "label3";
			this.labCenter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labRight
			// 
			this.labRight.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labRight.ForeColor = System.Drawing.SystemColors.GrayText;
			this.labRight.Location = new System.Drawing.Point(225, 73);
			this.labRight.Name = "labRight";
			this.labRight.Size = new System.Drawing.Size(106, 73);
			this.labRight.TabIndex = 4;
			this.labRight.Text = "label4";
			this.labRight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labDown
			// 
			this.labDown.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labDown.ForeColor = System.Drawing.SystemColors.GrayText;
			this.labDown.Location = new System.Drawing.Point(114, 146);
			this.labDown.Name = "labDown";
			this.labDown.Size = new System.Drawing.Size(105, 74);
			this.labDown.TabIndex = 5;
			this.labDown.Text = "label5";
			this.labDown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labLeft
			// 
			this.labLeft.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labLeft.ForeColor = System.Drawing.SystemColors.GrayText;
			this.labLeft.Location = new System.Drawing.Point(3, 73);
			this.labLeft.Name = "labLeft";
			this.labLeft.Size = new System.Drawing.Size(105, 73);
			this.labLeft.TabIndex = 1;
			this.labLeft.Text = "label1";
			this.labLeft.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.tableLayoutPanel2);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(334, 220);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Border";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 3;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.26072F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.26072F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.47856F));
			this.tableLayoutPanel2.Controls.Add(this.labBorderTop, 1, 0);
			this.tableLayoutPanel2.Controls.Add(this.labCenter2, 1, 1);
			this.tableLayoutPanel2.Controls.Add(this.labBorderRight, 2, 1);
			this.tableLayoutPanel2.Controls.Add(this.labBorderDown, 1, 2);
			this.tableLayoutPanel2.Controls.Add(this.labBorderLeft, 0, 1);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 3;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.26072F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.26072F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.47856F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(334, 220);
			this.tableLayoutPanel2.TabIndex = 2;
			// 
			// labBorderTop
			// 
			this.labBorderTop.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labBorderTop.Location = new System.Drawing.Point(114, 0);
			this.labBorderTop.Name = "labBorderTop";
			this.labBorderTop.Size = new System.Drawing.Size(105, 73);
			this.labBorderTop.TabIndex = 2;
			this.labBorderTop.Text = "label2";
			this.labBorderTop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labCenter2
			// 
			this.labCenter2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labCenter2.ForeColor = System.Drawing.SystemColors.GrayText;
			this.labCenter2.Location = new System.Drawing.Point(114, 73);
			this.labCenter2.Name = "labCenter2";
			this.labCenter2.Size = new System.Drawing.Size(105, 73);
			this.labCenter2.TabIndex = 3;
			this.labCenter2.Text = "label3";
			this.labCenter2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labBorderRight
			// 
			this.labBorderRight.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labBorderRight.Location = new System.Drawing.Point(225, 73);
			this.labBorderRight.Name = "labBorderRight";
			this.labBorderRight.Size = new System.Drawing.Size(106, 73);
			this.labBorderRight.TabIndex = 4;
			this.labBorderRight.Text = "label4";
			this.labBorderRight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labBorderDown
			// 
			this.labBorderDown.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labBorderDown.Location = new System.Drawing.Point(114, 146);
			this.labBorderDown.Name = "labBorderDown";
			this.labBorderDown.Size = new System.Drawing.Size(105, 74);
			this.labBorderDown.TabIndex = 5;
			this.labBorderDown.Text = "label5";
			this.labBorderDown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labBorderLeft
			// 
			this.labBorderLeft.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labBorderLeft.Location = new System.Drawing.Point(3, 73);
			this.labBorderLeft.Name = "labBorderLeft";
			this.labBorderLeft.Size = new System.Drawing.Size(105, 73);
			this.labBorderLeft.TabIndex = 1;
			this.labBorderLeft.Text = "label1";
			this.labBorderLeft.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// tabPage5
			// 
			this.tabPage5.Controls.Add(this.layout);
			this.tabPage5.Controls.Add(this.label7);
			this.tabPage5.Controls.Add(this.label6);
			this.tabPage5.Location = new System.Drawing.Point(4, 22);
			this.tabPage5.Name = "tabPage5";
			this.tabPage5.Size = new System.Drawing.Size(334, 220);
			this.tabPage5.TabIndex = 4;
			this.tabPage5.Text = "Layout";
			this.tabPage5.UseVisualStyleBackColor = true;
			// 
			// layout
			// 
			this.layout.BackColor = System.Drawing.Color.White;
			this.layout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layout.Grid = null;
			this.layout.Location = new System.Drawing.Point(0, 20);
			this.layout.Name = "layout";
			this.layout.Size = new System.Drawing.Size(334, 180);
			this.layout.TabIndex = 2;
			this.layout.Text = "rgCellLayoutGraphControl1";
			// 
			// label7
			// 
			this.label7.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.label7.Location = new System.Drawing.Point(0, 200);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(334, 20);
			this.label7.TabIndex = 1;
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label6
			// 
			this.label6.Dock = System.Windows.Forms.DockStyle.Top;
			this.label6.Location = new System.Drawing.Point(0, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(334, 20);
			this.label6.TabIndex = 0;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.labFormatArgs);
			this.tabPage3.Controls.Add(this.labFormat);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(334, 220);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Format";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// labFormatArgs
			// 
			this.labFormatArgs.AutoSize = true;
			this.labFormatArgs.Location = new System.Drawing.Point(15, 42);
			this.labFormatArgs.Name = "labFormatArgs";
			this.labFormatArgs.Size = new System.Drawing.Size(0, 13);
			this.labFormatArgs.TabIndex = 1;
			// 
			// labFormat
			// 
			this.labFormat.AutoSize = true;
			this.labFormat.Location = new System.Drawing.Point(15, 17);
			this.labFormat.Name = "labFormat";
			this.labFormat.Size = new System.Drawing.Size(0, 13);
			this.labFormat.TabIndex = 0;
			// 
			// tabPage4
			// 
			this.tabPage4.Controls.Add(this.txtLog);
			this.tabPage4.Location = new System.Drawing.Point(4, 22);
			this.tabPage4.Name = "tabPage4";
			this.tabPage4.Size = new System.Drawing.Size(334, 220);
			this.tabPage4.TabIndex = 3;
			this.tabPage4.Text = "Valids";
			this.tabPage4.UseVisualStyleBackColor = true;
			// 
			// txtLog
			// 
			this.txtLog.BackColor = System.Drawing.SystemColors.Window;
			this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtLog.Location = new System.Drawing.Point(0, 0);
			this.txtLog.Multiline = true;
			this.txtLog.Name = "txtLog";
			this.txtLog.ReadOnly = true;
			this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtLog.Size = new System.Drawing.Size(334, 220);
			this.txtLog.TabIndex = 0;
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusToolStripStatusLabel});
			this.statusStrip1.Location = new System.Drawing.Point(2, 248);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(342, 22);
			this.statusStrip1.TabIndex = 1;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// statusToolStripStatusLabel
			// 
			this.statusToolStripStatusLabel.Name = "statusToolStripStatusLabel";
			this.statusToolStripStatusLabel.Size = new System.Drawing.Size(327, 17);
			this.statusToolStripStatusLabel.Spring = true;
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.Location = new System.Drawing.Point(112, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(103, 71);
			this.label1.TabIndex = 2;
			this.label1.Text = "label2";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label2.Location = new System.Drawing.Point(112, 71);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(103, 71);
			this.label2.TabIndex = 3;
			this.label2.Text = "label3";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label3
			// 
			this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label3.Location = new System.Drawing.Point(221, 71);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(104, 71);
			this.label3.TabIndex = 4;
			this.label3.Text = "label4";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label4
			// 
			this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label4.Location = new System.Drawing.Point(112, 142);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(103, 72);
			this.label4.TabIndex = 5;
			this.label4.Text = "label5";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label5
			// 
			this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label5.Location = new System.Drawing.Point(3, 71);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(103, 71);
			this.label5.TabIndex = 1;
			this.label5.Text = "label1";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// DebugForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(346, 272);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.statusStrip1);
			this.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "DebugForm";
			this.Padding = new System.Windows.Forms.Padding(2);
			this.ShowInTaskbar = false;
			this.Text = "ReoGrid Debug Info";
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tabPage5.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.tabPage3.PerformLayout();
			this.tabPage4.ResumeLayout(false);
			this.tabPage4.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label labTop;
		private System.Windows.Forms.Label labCenter;
		private System.Windows.Forms.Label labRight;
		private System.Windows.Forms.Label labDown;
		private System.Windows.Forms.Label labLeft;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel statusToolStripStatusLabel;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Label labBorderTop;
		private System.Windows.Forms.Label labCenter2;
		private System.Windows.Forms.Label labBorderRight;
		private System.Windows.Forms.Label labBorderDown;
		private System.Windows.Forms.Label labBorderLeft;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.Label labFormatArgs;
		private System.Windows.Forms.Label labFormat;
		private System.Windows.Forms.TabPage tabPage4;
		private System.Windows.Forms.TextBox txtLog;
		private System.Windows.Forms.TabPage tabPage5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private RGCellLayoutGraphControl layout;
	}
}

#endif // WINFORM && DEBUG