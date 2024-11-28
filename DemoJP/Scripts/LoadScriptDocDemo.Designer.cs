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

namespace unvell.ReoGrid.Demo.Scripts
{
	partial class LoadScriptDocDemo
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
			this.reoScriptEditorControl1 = new unvell.ReoScript.Editor.ReoScriptEditorControl();
			this.panel2 = new System.Windows.Forms.Panel();
			this.btnRun = new System.Windows.Forms.Button();
			this.btnStop = new System.Windows.Forms.Button();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// reoGridControl
			// 
			this.reoGridControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.reoGridControl.ColumnHeaderContextMenuStrip = null;
			this.reoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.reoGridControl.LeadHeaderContextMenuStrip = null;
			this.reoGridControl.Location = new System.Drawing.Point(0, 0);
			this.reoGridControl.Name = "reoGridControl";
			this.reoGridControl.RowHeaderContextMenuStrip = null;
			this.reoGridControl.Script = null;
			this.reoGridControl.SheetTabContextMenuStrip = null;
			this.reoGridControl.SheetTabNewButtonVisible = true;
			this.reoGridControl.SheetTabVisible = true;
			this.reoGridControl.SheetTabWidth = 274;
			this.reoGridControl.ShowScrollEndSpacing = true;
			this.reoGridControl.Size = new System.Drawing.Size(454, 535);
			this.reoGridControl.TabIndex = 1;
			this.reoGridControl.Text = "reoGridControl1";
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.Info;
			this.panel1.Controls.Add(this.reoScriptEditorControl1);
			this.panel1.Controls.Add(this.panel2);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(458, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(510, 535);
			this.panel1.TabIndex = 2;
			// 
			// reoScriptEditorControl1
			// 
			this.reoScriptEditorControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.reoScriptEditorControl1.Location = new System.Drawing.Point(0, 59);
			this.reoScriptEditorControl1.Name = "reoScriptEditorControl1";
			this.reoScriptEditorControl1.Size = new System.Drawing.Size(510, 476);
			this.reoScriptEditorControl1.TabIndex = 4;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.btnRun);
			this.panel2.Controls.Add(this.btnStop);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(510, 59);
			this.panel2.TabIndex = 5;
			// 
			// btnRun
			// 
			this.btnRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnRun.ForeColor = System.Drawing.Color.Green;
			this.btnRun.Location = new System.Drawing.Point(15, 16);
			this.btnRun.Name = "btnRun";
			this.btnRun.Size = new System.Drawing.Size(119, 31);
			this.btnRun.TabIndex = 1;
			this.btnRun.Text = "実行";
			this.btnRun.UseVisualStyleBackColor = true;
			this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
			// 
			// btnStop
			// 
			this.btnStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnStop.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.btnStop.Location = new System.Drawing.Point(145, 16);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(119, 31);
			this.btnStop.TabIndex = 2;
			this.btnStop.Text = "終了";
			this.btnStop.UseVisualStyleBackColor = true;
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
			this.splitter1.Location = new System.Drawing.Point(454, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(4, 535);
			this.splitter1.TabIndex = 3;
			this.splitter1.TabStop = false;
			// 
			// LoadScriptDocDemo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.reoGridControl);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.panel1);
			this.Name = "LoadScriptDocDemo";
			this.Size = new System.Drawing.Size(968, 535);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private ReoGridControl reoGridControl;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnRun;
		private System.Windows.Forms.Button btnStop;
		private ReoScript.Editor.ReoScriptEditorControl reoScriptEditorControl1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Splitter splitter1;
	}
}
