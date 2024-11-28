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
	partial class RunFunctionForm
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
			this.labFunctions = new System.Windows.Forms.Label();
			this.functionList = new System.Windows.Forms.ListBox();
			this.btnRun = new System.Windows.Forms.Button();
			this.btnClose = new System.Windows.Forms.Button();
			this.chkCloseAfterRun = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// labFunctions
			// 
			this.labFunctions.AutoSize = true;
			this.labFunctions.Location = new System.Drawing.Point(12, 12);
			this.labFunctions.Name = "labFunctions";
			this.labFunctions.Size = new System.Drawing.Size(56, 13);
			this.labFunctions.TabIndex = 0;
			this.labFunctions.Text = "Functions:";
			// 
			// functionList
			// 
			this.functionList.FormattingEnabled = true;
			this.functionList.Location = new System.Drawing.Point(12, 28);
			this.functionList.Name = "functionList";
			this.functionList.Size = new System.Drawing.Size(267, 238);
			this.functionList.TabIndex = 1;
			// 
			// btnRun
			// 
			this.btnRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnRun.Location = new System.Drawing.Point(285, 28);
			this.btnRun.Name = "btnRun";
			this.btnRun.Size = new System.Drawing.Size(75, 23);
			this.btnRun.TabIndex = 2;
			this.btnRun.Text = "Run";
			this.btnRun.UseVisualStyleBackColor = true;
			this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
			// 
			// btnClose
			// 
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.Location = new System.Drawing.Point(285, 57);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(75, 23);
			this.btnClose.TabIndex = 2;
			this.btnClose.Text = "Close";
			this.btnClose.UseVisualStyleBackColor = true;
			// 
			// chkCloseAfterRun
			// 
			this.chkCloseAfterRun.AutoSize = true;
			this.chkCloseAfterRun.Checked = true;
			this.chkCloseAfterRun.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkCloseAfterRun.Location = new System.Drawing.Point(12, 272);
			this.chkCloseAfterRun.Name = "chkCloseAfterRun";
			this.chkCloseAfterRun.Size = new System.Drawing.Size(172, 17);
			this.chkCloseAfterRun.TabIndex = 3;
			this.chkCloseAfterRun.Text = "Close this window after running";
			this.chkCloseAfterRun.UseVisualStyleBackColor = true;
			// 
			// RunFunctionForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnClose;
			this.ClientSize = new System.Drawing.Size(372, 299);
			this.Controls.Add(this.chkCloseAfterRun);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.btnRun);
			this.Controls.Add(this.functionList);
			this.Controls.Add(this.labFunctions);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "RunFunctionForm";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Run Function - ReoGrid";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labFunctions;
		private System.Windows.Forms.ListBox functionList;
		private System.Windows.Forms.Button btnRun;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.CheckBox chkCloseAfterRun;
	}
}