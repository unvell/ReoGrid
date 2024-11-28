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

namespace unvell.ReoGrid.Demo.CellDemo
{
	partial class IterateCellsDemo
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
			this.grid = new unvell.ReoGrid.ReoGridControl();
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnCalcTotal = new System.Windows.Forms.Button();
			this.btnSetData = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// grid
			// 
			this.grid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.grid.ColumnHeaderContextMenuStrip = null;
			this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grid.LeadHeaderContextMenuStrip = null;
			this.grid.Location = new System.Drawing.Point(0, 54);
			this.grid.Margin = new System.Windows.Forms.Padding(4);
			this.grid.Name = "grid";
			this.grid.RowHeaderContextMenuStrip = null;
			this.grid.Script = null;
			this.grid.SheetTabContextMenuStrip = null;
			this.grid.SheetTabNewButtonVisible = true;
			this.grid.SheetTabVisible = true;
			this.grid.SheetTabWidth = 533;
			this.grid.ShowScrollEndSpacing = true;
			this.grid.Size = new System.Drawing.Size(724, 571);
			this.grid.TabIndex = 0;
			this.grid.Text = "reoGridControl1";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.btnCalcTotal);
			this.panel1.Controls.Add(this.btnSetData);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(724, 54);
			this.panel1.Margin = new System.Windows.Forms.Padding(4);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(167, 571);
			this.panel1.TabIndex = 1;
			// 
			// btnCalcTotal
			// 
			this.btnCalcTotal.Location = new System.Drawing.Point(15, 55);
			this.btnCalcTotal.Margin = new System.Windows.Forms.Padding(4);
			this.btnCalcTotal.Name = "btnCalcTotal";
			this.btnCalcTotal.Size = new System.Drawing.Size(134, 31);
			this.btnCalcTotal.TabIndex = 0;
			this.btnCalcTotal.Text = "２．統計を行う";
			this.btnCalcTotal.UseVisualStyleBackColor = true;
			this.btnCalcTotal.Click += new System.EventHandler(this.btnCalcTotal_Click);
			// 
			// btnSetData
			// 
			this.btnSetData.Location = new System.Drawing.Point(15, 16);
			this.btnSetData.Margin = new System.Windows.Forms.Padding(4);
			this.btnSetData.Name = "btnSetData";
			this.btnSetData.Size = new System.Drawing.Size(134, 31);
			this.btnSetData.TabIndex = 0;
			this.btnSetData.Text = "１．データを設定";
			this.btnSetData.UseVisualStyleBackColor = true;
			this.btnSetData.Click += new System.EventHandler(this.btnSetData_Click);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.label1);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Margin = new System.Windows.Forms.Padding(4);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(891, 54);
			this.panel2.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Padding = new System.Windows.Forms.Padding(8, 7, 8, 7);
			this.label1.Size = new System.Drawing.Size(891, 54);
			this.label1.TabIndex = 0;
			this.label1.Text = "セルの反復 IterateCells メソッドを利用して指定した範囲からセルのデータを統計できます。IterateCells メソッドについて詳しくは、下側のドキ" +
    "ュメントをご覧ください。";
			// 
			// IterateCellsDemo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.grid);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.panel2);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "IterateCellsDemo";
			this.Size = new System.Drawing.Size(891, 625);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private ReoGridControl grid;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnSetData;
		private System.Windows.Forms.Button btnCalcTotal;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label1;
	}
}