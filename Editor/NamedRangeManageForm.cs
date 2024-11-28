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
	public partial class NamedRangeManageForm : Form
	{
		private ReoGridControl grid;

		public NamedRangeManageForm(ReoGridControl grid)
		{
			InitializeComponent();

			SetupUILanguage();

			this.grid = grid;
		}

		void SetupUILanguage()
		{
			this.Text = LangResource.NamedRangeManage_Caption;
			this.labNamedRange.Text = LangResource.NamedRangeManage_Named_Ranges;

			this.colName.Text = LangResource.Name;
			this.colAddress.Text = LangResource.Address;

			this.btnNew.Text = LangResource.NamedRangeManage_New;
			this.btnEdit.Text = LangResource.Btn_Edit;
			this.btnDelete.Text = LangResource.Btn_Delete;
			this.btnClose.Text = LangResource.Btn_Close;
		}

		private void NamedRangeManageForm_Load(object sender, EventArgs _unused)
		{
			if (this.grid.CurrentWorksheet == null)
			{
				MessageBox.Show(LangRes.LangResource.Msg_No_Current_Worksheet);
				return;
			}

			foreach (var name in this.grid.CurrentWorksheet.GetAllNamedRanges())
			{
				var range = this.grid.CurrentWorksheet.GetNamedRange(name);

				var item = new ListViewItem(new string[] { range.Name, range.Position.ToAddress() }) { Tag = range };
				this.lstRanges.Items.Add(item);
			}

			btnEdit.Enabled = btnDelete.Enabled = this.lstRanges.Items.Count > 0;

			this.lstRanges.SelectedIndexChanged += (s, e) =>
				{
					if (this.lstRanges.SelectedItems.Count > 0)
					{
						var item = this.lstRanges.SelectedItems[0];
						if (item != null)
						{
							var range = item.Tag as NamedRange;
							if (range != null && range.Worksheet == this.grid.CurrentWorksheet)
							{
								this.grid.CurrentWorksheet.SelectionRange = range;
							}
						}

						btnEdit.Enabled = btnDelete.Enabled = true;
					}
					else
					{
						btnEdit.Enabled = btnDelete.Enabled = false;
					}
				};
		}

		private void btnNew_Click(object sender, EventArgs e)
		{
			using (var dlg = new DefineNamedRangeDialog())
			{
				if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					NamedRange range = DefineNamedRange(this, this.grid.CurrentWorksheet,
						dlg.RangeName, dlg.Comment, dlg.Range);

					if (range != null)
					{
						lstRanges.Items.Add(new ListViewItem(new string[] { range.Name, range.Position.ToAddress() }) { Tag = range });

						if (range.Worksheet == this.grid.CurrentWorksheet)
						{
							this.grid.CurrentWorksheet.SelectionRange = range;
						}
					}
				}
			}
		}

		private void btnEdit_Click(object sender, EventArgs e)
		{
			if (this.lstRanges.SelectedItems.Count > 0)
			{
				var item = this.lstRanges.SelectedItems[0];
				var range = item.Tag as NamedRange;

				if (range != null)
				{
					using (var dlg = new DefineNamedRangeDialog())
					{
						dlg.RangeName = range.Name;
						dlg.Range = range;

						if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
						{
							range.Name = dlg.RangeName;
							range.Comment = dlg.Comment;
							range.Position = dlg.Range;

							item.Text = dlg.RangeName;
							item.SubItems[1].Text = dlg.Range.ToAddress();
							this.grid.CurrentWorksheet.SelectionRange = range;

							if (range.Worksheet == this.grid.CurrentWorksheet)
							{
								this.grid.CurrentWorksheet.SelectionRange = range;
							}
						}
					}
				}
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (this.lstRanges.SelectedItems.Count > 0)
			{
				var item = this.lstRanges.SelectedItems[0];
				var range = item.Tag as NamedRange;

				if (range != null)
				{
					if (MessageBox.Show(this, LangRes.LangResource.Msg_Delete_Named_Range_Confirm, LangRes.LangResource.Msg_Delete_Named_Range_Confirm_Title,
						 MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
					{
						if (this.grid.CurrentWorksheet.UndefineNamedRange(range.Name))
						{
							this.lstRanges.Items.Remove(item);
						}
					}
				}
			}
		}

		internal static NamedRange DefineNamedRange(IWin32Window owner, Worksheet sheet, string name, string comment, RangePosition range)
		{
			NamedRange namedRange = null;

			try
			{
				namedRange = sheet.DefineNamedRange(name, range);
				namedRange.Comment = comment;
			}
			catch
			{
				MessageBox.Show(owner, LangRes.LangResource.Msg_Create_Named_Range_Failed,
					LangRes.LangResource.Msg_Create_Named_Range_Failed_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}

			return namedRange;
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			Close();
		}

	}
}
