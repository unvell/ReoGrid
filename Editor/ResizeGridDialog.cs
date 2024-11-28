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

using System;
using System.Windows.Forms;
using unvell.ReoGrid.Editor.LangRes;

namespace unvell.ReoGrid.Editor
{
	public partial class ResizeGridDialog : Form
	{
		public int Rows { get; set; }
		public int Cols { get; set; }

		public ResizeGridDialog()
		{
			InitializeComponent();

			numRows.KeyDown += numRows_KeyDown;
			numCols.KeyDown += numRows_KeyDown;

			SetupUILanguage();
		}

		private void SetupUILanguage()
		{
			this.Text = LangResource.Resize_Caption;

			this.labRows.Text = LangResource.Resize_Rows;
			this.labCols.Text = LangResource.Resize_Columns;
			this.labRowsRemark.Text = LangResource.Resize_Rows_Remark;
			this.labColsRemark.Text = LangResource.Resize_Columns_Remark;

			this.btnOK.Text = LangResource.Btn_OK;
			this.btnCancel.Text = LangResource.Btn_Cancel;
		}

		void numRows_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				btnOK.PerformClick();
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			numRows.Value = Rows;
			numCols.Value = Cols;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Rows = (int)numRows.Value;
			Cols = (int)numCols.Value;
		}
	}
}
