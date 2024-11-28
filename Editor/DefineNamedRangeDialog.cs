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
	public partial class DefineNamedRangeDialog : Form
	{
		public DefineNamedRangeDialog()
		{
			InitializeComponent();

			txtName.KeyDown += (s, e) =>
			{
				if (e.KeyCode == Keys.Enter)
				{
					btnOK.PerformClick();
				}
			};

			txtRange.KeyDown += (s, e) =>
			{
				if (e.KeyCode == Keys.Enter)
				{
					btnOK.PerformClick();
				}
			};

			SetupUILanguage();
		}

		void SetupUILanguage()
		{
			this.Text = LangResource.DefineNamedRange_Caption;

			this.labName.Text = LangResource.DefineNamedRange_Name;
			this.labComment.Text = LangResource.DefineNamedRange_Comment;
			this.labRange.Text = LangResource.DefineNamedRange_Range;

			this.btnOK.Text = LangResource.Btn_OK;
			this.btnCancel.Text = LangResource.Btn_Cancel;
		}

		public string RangeName { get; set; }

		public string Comment { get; set; }

		public RangePosition Range { get; set; }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			txtName.Text = RangeName;
			txtComment.Text = Comment;
			
			if (!Range.IsEmpty)
			{
				txtRange.Text = Range.IsSingleCell ? Range.StartPos.ToAddress() : Range.ToAddress();
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtName.Text))
			{
				MessageBox.Show(LangRes.LangResource.Msg_Input_Name_Of_Range);
				txtName.Focus();
				return;
			}

			if (string.IsNullOrEmpty(txtRange.Text)
				|| !RangePosition.IsValidAddress(txtRange.Text))
			{
				MessageBox.Show(LangRes.LangResource.Msg_Invalid_Address_Of_Range);
				txtRange.Focus();
				return;
			}

			RangeName = txtName.Text;
			Comment = txtComment.Text;

			Range = new RangePosition(txtRange.Text);

			DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}
	}
}
