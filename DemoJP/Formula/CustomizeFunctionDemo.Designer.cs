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

namespace unvell.ReoGrid.Demo.FormulaDemo
{
	partial class CustomizeFunctionDemo
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
			this.btnAddReoScriptFunctionInCSharp = new System.Windows.Forms.Button();
			this.grid = new unvell.ReoGrid.ReoGridControl();
			this.btnAddReoScriptFunctionInScript = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label5 = new System.Windows.Forms.Label();
			this.addCustomizeFunction = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnAddReoScriptFunctionInCSharp
			// 
			this.btnAddReoScriptFunctionInCSharp.Font = new System.Drawing.Font("MS PGothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnAddReoScriptFunctionInCSharp.Location = new System.Drawing.Point(73, 190);
			this.btnAddReoScriptFunctionInCSharp.Name = "btnAddReoScriptFunctionInCSharp";
			this.btnAddReoScriptFunctionInCSharp.Size = new System.Drawing.Size(228, 25);
			this.btnAddReoScriptFunctionInCSharp.TabIndex = 4;
			this.btnAddReoScriptFunctionInCSharp.Text = "C#で \'func2\' 関数を作成";
			this.btnAddReoScriptFunctionInCSharp.UseVisualStyleBackColor = true;
			this.btnAddReoScriptFunctionInCSharp.Click += new System.EventHandler(this.btnAddReoScriptFunction_Click);
			// 
			// grid
			// 
			this.grid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.grid.ColumnHeaderContextMenuStrip = null;
			this.grid.Dock = System.Windows.Forms.DockStyle.Left;
			this.grid.Font = new System.Drawing.Font("MS PGothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.grid.LeadHeaderContextMenuStrip = null;
			this.grid.Location = new System.Drawing.Point(2, 2);
			this.grid.Name = "grid";
			this.grid.RowHeaderContextMenuStrip = null;
			this.grid.Script = null;
			this.grid.SheetTabContextMenuStrip = null;
			this.grid.SheetTabNewButtonVisible = true;
			this.grid.SheetTabVisible = true;
			this.grid.SheetTabWidth = 331;
			this.grid.ShowScrollEndSpacing = true;
			this.grid.Size = new System.Drawing.Size(442, 429);
			this.grid.TabIndex = 3;
			this.grid.Text = "reoGridControl1";
			// 
			// btnAddReoScriptFunctionInScript
			// 
			this.btnAddReoScriptFunctionInScript.Font = new System.Drawing.Font("MS PGothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnAddReoScriptFunctionInScript.Location = new System.Drawing.Point(73, 221);
			this.btnAddReoScriptFunctionInScript.Name = "btnAddReoScriptFunctionInScript";
			this.btnAddReoScriptFunctionInScript.Size = new System.Drawing.Size(228, 25);
			this.btnAddReoScriptFunctionInScript.TabIndex = 5;
			this.btnAddReoScriptFunctionInScript.Text = "スクリプトで \'func3\' 関数を作成";
			this.btnAddReoScriptFunctionInScript.UseVisualStyleBackColor = true;
			this.btnAddReoScriptFunctionInScript.Click += new System.EventHandler(this.btnAddReoScriptFunctionInScript_Click);
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.label1.Font = new System.Drawing.Font("MS PGothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(76, 130);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(218, 45);
			this.label1.TabIndex = 6;
			this.label1.Text = "function func2(data) {\r\n  return \'[\' + data + \']\';\r\n}";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.btnAddReoScriptFunctionInScript);
			this.groupBox1.Controls.Add(this.btnAddReoScriptFunctionInCSharp);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.groupBox1.Font = new System.Drawing.Font("MS PGothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox1.Location = new System.Drawing.Point(444, 173);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
			this.groupBox1.Size = new System.Drawing.Size(371, 258);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "スクリプト言語を利用してカスタマイズした関数を作成";
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("MS PGothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(17, 89);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(336, 27);
			this.label3.TabIndex = 7;
			this.label3.Text = "※この機能は拡張機能版でご利用頂けます。";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("MS PGothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(17, 23);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(336, 63);
			this.label2.TabIndex = 6;
			this.label2.Text = "拡張機能版を利用している場合、ReoScript 言語の実行が可能になります。ReoScript 言語を利用して作成した関数は、数式計算でも利用できます。サンプル" +
    "は以下の通りです。";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.addCustomizeFunction);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Font = new System.Drawing.Font("MS PGothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox2.Location = new System.Drawing.Point(444, 2);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(371, 171);
			this.groupBox2.TabIndex = 8;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "カスタマイズした関数を作成";
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("MS PGothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(17, 25);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(336, 64);
			this.label5.TabIndex = 6;
			this.label5.Text = "カスタマイズした関数を追加するには FormulaExtension クラスを利用すると追加できます。詳しくは下のドキュメントをご覧ください。\r\n";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// addCustomizeFunction
			// 
			this.addCustomizeFunction.Font = new System.Drawing.Font("MS PGothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.addCustomizeFunction.Location = new System.Drawing.Point(73, 109);
			this.addCustomizeFunction.Name = "addCustomizeFunction";
			this.addCustomizeFunction.Size = new System.Drawing.Size(228, 25);
			this.addCustomizeFunction.TabIndex = 4;
			this.addCustomizeFunction.Text = "\'func1\' 関数を作成";
			this.addCustomizeFunction.UseVisualStyleBackColor = true;
			this.addCustomizeFunction.Click += new System.EventHandler(this.btnAddCustomFunction_Click);
			// 
			// CustomizeFunctionDemo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.grid);
			this.Font = new System.Drawing.Font("MS PGothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "CustomizeFunctionDemo";
			this.Padding = new System.Windows.Forms.Padding(2);
			this.Size = new System.Drawing.Size(817, 433);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnAddReoScriptFunctionInCSharp;
		private ReoGridControl grid;
		private System.Windows.Forms.Button btnAddReoScriptFunctionInScript;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button addCustomizeFunction;
	}
}