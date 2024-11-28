/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * ReoGrid and ReoGrid Demo project is released under MIT license.
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

namespace unvell.ReoGrid.Demo.Formula
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomizeFunctionDemo));
			this.btnAddReoScriptFunctionInCSharp = new System.Windows.Forms.Button();
			this.grid = new unvell.ReoGrid.ReoGridControl();
			this.btnAddReoScriptFunctionInScript = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.addCustomizeFunction = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnAddReoScriptFunctionInCSharp
			// 
			this.btnAddReoScriptFunctionInCSharp.Location = new System.Drawing.Point(73, 206);
			this.btnAddReoScriptFunctionInCSharp.Name = "btnAddReoScriptFunctionInCSharp";
			this.btnAddReoScriptFunctionInCSharp.Size = new System.Drawing.Size(228, 27);
			this.btnAddReoScriptFunctionInCSharp.TabIndex = 4;
			this.btnAddReoScriptFunctionInCSharp.Text = "Add \'func2\' Function in C#";
			this.btnAddReoScriptFunctionInCSharp.UseVisualStyleBackColor = true;
			this.btnAddReoScriptFunctionInCSharp.Click += new System.EventHandler(this.btnAddReoScriptFunction_Click);
			// 
			// grid
			// 
			this.grid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.grid.ColumnHeaderContextMenuStrip = null;
			this.grid.Dock = System.Windows.Forms.DockStyle.Left;
			this.grid.LeadHeaderContextMenuStrip = null;
			this.grid.Location = new System.Drawing.Point(0, 0);
			this.grid.Name = "grid";
			this.grid.RowHeaderContextMenuStrip = null;
			this.grid.Script = null;
			this.grid.SheetTabContextMenuStrip = null;
			this.grid.SheetTabWidth = 331;
			this.grid.Size = new System.Drawing.Size(442, 469);
			this.grid.TabIndex = 3;
			this.grid.Text = "reoGridControl1";
			// 
			// btnAddReoScriptFunctionInScript
			// 
			this.btnAddReoScriptFunctionInScript.Location = new System.Drawing.Point(73, 239);
			this.btnAddReoScriptFunctionInScript.Name = "btnAddReoScriptFunctionInScript";
			this.btnAddReoScriptFunctionInScript.Size = new System.Drawing.Size(228, 27);
			this.btnAddReoScriptFunctionInScript.TabIndex = 5;
			this.btnAddReoScriptFunctionInScript.Text = "Add \'func3\' Function in Script";
			this.btnAddReoScriptFunctionInScript.UseVisualStyleBackColor = true;
			this.btnAddReoScriptFunctionInScript.Click += new System.EventHandler(this.btnAddReoScriptFunctionInScript_Click);
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.label1.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(76, 145);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(218, 49);
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
			this.groupBox1.Location = new System.Drawing.Point(442, 190);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(375, 279);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Customize function in ReoScript extension";
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(17, 110);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(336, 29);
			this.label3.TabIndex = 7;
			this.label3.Text = "The buttons below perform adding ReoScript function.";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(17, 21);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(336, 81);
			this.label2.TabIndex = 6;
			this.label2.Text = resources.GetString("label2.Text");
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.addCustomizeFunction);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(442, 0);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(375, 190);
			this.groupBox2.TabIndex = 8;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Cutomize function supported by ReoGrid core";
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(17, 145);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(336, 29);
			this.label4.TabIndex = 7;
			this.label4.Text = "Note: this feature doesn\'t require the ReoScript module.\r\n";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(17, 27);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(336, 69);
			this.label5.TabIndex = 6;
			this.label5.Text = "The single DLL (Standard Release Package) supports the formula calculation featur" +
    "e from 0.8.8, to add a cutomize function, use the FormulaExtension class.\r\n";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// addCustomizeFunction
			// 
			this.addCustomizeFunction.Location = new System.Drawing.Point(73, 103);
			this.addCustomizeFunction.Name = "addCustomizeFunction";
			this.addCustomizeFunction.Size = new System.Drawing.Size(228, 27);
			this.addCustomizeFunction.TabIndex = 4;
			this.addCustomizeFunction.Text = "Add \'func1\' Function";
			this.addCustomizeFunction.UseVisualStyleBackColor = true;
			this.addCustomizeFunction.Click += new System.EventHandler(this.btnAddCustomFunction_Click);
			// 
			// AddCustomizeFunctionForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(817, 469);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.grid);
			this.Name = "AddCustomizeFunctionForm";
			this.Text = "AddCustomizeFunctionForm";
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
		private System.Windows.Forms.Label label4;
	}
}