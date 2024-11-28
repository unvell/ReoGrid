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
using System.Drawing;
using System.Windows.Forms;

namespace unvell.ReoGrid.Editor
{
	public partial class SetWidthOrHeightDialog : Form
	{
		private int value;

		public int Value
		{
			get { return value; }
			set { this.value = value; }
		}

		public SetWidthOrHeightDialog(RowOrColumn rowOrColumn)
		{
			InitializeComponent();

			StartPosition = FormStartPosition.Manual;

			if (rowOrColumn == RowOrColumn.Row)
			{
				Text = LangRes.LangResource.SetHeaderSize_Caption_Row;
				label1.Text = LangRes.LangResource.SetHeaderSize_Caption_Row;
			}
			else
			{
				Text = LangRes.LangResource.SetHeaderSize_Caption_Column;
				label1.Text = LangRes.LangResource.SetHeaderSize_Caption_Column;
			}

			btnOK.Text = LangRes.LangResource.Btn_OK;
			btnCancel.Text = LangRes.LangResource.Btn_Cancel;

			numericUpDown1.KeyDown += new KeyEventHandler(numericUpDown1_KeyDown);
		}

		void numericUpDown1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				btnOK.PerformClick();
			}
			else if (e.KeyCode == Keys.Escape)
			{
				DialogResult = DialogResult.Cancel;
				Close();
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			numericUpDown1.Value = value;
			numericUpDown1.Select(0, numericUpDown1.Value.ToString().Length);

			Point p = Cursor.Position;
			p.Offset(-115, -61);
			Location = p;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			value = (int)numericUpDown1.Value;
		}
	}
}
