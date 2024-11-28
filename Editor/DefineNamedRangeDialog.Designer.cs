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
	partial class DefineNamedRangeDialog
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
			this.labName = new System.Windows.Forms.Label();
			this.txtName = new System.Windows.Forms.TextBox();
			this.txtComment = new System.Windows.Forms.TextBox();
			this.labComment = new System.Windows.Forms.Label();
			this.txtRange = new System.Windows.Forms.TextBox();
			this.labRange = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// labName
			// 
			this.labName.AutoSize = true;
			this.labName.Location = new System.Drawing.Point(13, 21);
			this.labName.Name = "labName";
			this.labName.Size = new System.Drawing.Size(38, 13);
			this.labName.TabIndex = 0;
			this.labName.Text = "Name:";
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(79, 18);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(197, 20);
			this.txtName.TabIndex = 1;
			// 
			// txtComment
			// 
			this.txtComment.Location = new System.Drawing.Point(79, 44);
			this.txtComment.Multiline = true;
			this.txtComment.Name = "txtComment";
			this.txtComment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtComment.Size = new System.Drawing.Size(197, 58);
			this.txtComment.TabIndex = 3;
			// 
			// labComment
			// 
			this.labComment.AutoSize = true;
			this.labComment.Location = new System.Drawing.Point(13, 47);
			this.labComment.Name = "labComment";
			this.labComment.Size = new System.Drawing.Size(54, 13);
			this.labComment.TabIndex = 2;
			this.labComment.Text = "Comment:";
			// 
			// txtRange
			// 
			this.txtRange.Location = new System.Drawing.Point(79, 108);
			this.txtRange.Name = "txtRange";
			this.txtRange.Size = new System.Drawing.Size(197, 20);
			this.txtRange.TabIndex = 5;
			// 
			// labRange
			// 
			this.labRange.AutoSize = true;
			this.labRange.Location = new System.Drawing.Point(13, 111);
			this.labRange.Name = "labRange";
			this.labRange.Size = new System.Drawing.Size(42, 13);
			this.labRange.TabIndex = 4;
			this.labRange.Text = "Range:";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(122, 140);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(74, 24);
			this.btnOK.TabIndex = 6;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(202, 140);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(74, 24);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// DefineNamedRangeDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(288, 176);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.txtRange);
			this.Controls.Add(this.labRange);
			this.Controls.Add(this.txtComment);
			this.Controls.Add(this.labComment);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.labName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "DefineNamedRangeDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "New Name";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labName;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.TextBox txtComment;
		private System.Windows.Forms.Label labComment;
		private System.Windows.Forms.TextBox txtRange;
		private System.Windows.Forms.Label labRange;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
	}
}