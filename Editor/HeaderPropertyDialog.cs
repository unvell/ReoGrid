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

using unvell.ReoGrid.CellTypes;
using unvell.ReoGrid.Editor.LangRes;

namespace unvell.ReoGrid.Editor
{
	internal partial class HeaderPropertyDialog : Form
	{
		public RowOrColumn RowOrColumn { get; set; }

		public string HeaderText { get; set; }

		public Color HeaderTextColor { get; set; }

		public Type DefaultCellBody { get; set; }

		public int RowHeaderWidth { get; set; }

		public bool AutoFitToCell { get; set; }

		public HeaderPropertyDialog(RowOrColumn rowOrColumn)
		{
			this.RowOrColumn = rowOrColumn;

			InitializeComponent();

			SetupUILanguage();

			this.cmbCellBody.Items.Add("(null)");

			foreach (var cellType in CellTypesManager.CellTypes)
			{
				var name = cellType.Key;
				if (name.EndsWith("Cell")) name = name.Substring(0, name.Length - 4);

				cmbCellBody.Items.Add(new CellBodyItem
				{
					Name = name,
					Type = cellType.Value,
				});
			}

			if (this.RowOrColumn == ReoGrid.RowOrColumn.Column)
			{
				labRowHeaderWidth.Visible = numRowHeaderWidth.Visible = numRowHeaderWidth.Enabled = false;
			}
			else
			{
				labRowHeaderWidth.Visible = numRowHeaderWidth.Visible = numRowHeaderWidth.Enabled = true;
			}

			this.txtHeaderText.KeyDown += (s, e) =>
			{
				if (e.KeyCode == Keys.Enter) btnOK.PerformClick();
			};
		}

		void SetupUILanguage()
		{
			this.Text = LangResource.HeaderProperty_Caption;

			this.labText.Text = LangResource.HeaderProperty_Text;
			this.labTextColor.Text = LangResource.HeaderProperty_Text_Color;
			this.chkAutoFit.Text = LangResource.HeaderProperty_Auto_Fit_To_Cell;
			this.labRowHeaderWidth.Text = LangResource.HeaderProperty_Panel_Width;
			this.labCellBody.Text = LangResource.HeaderProperty_Default_Column_Type;

			this.btnOK.Text = btnOK.Text;
			this.btnCancel.Text = btnCancel.Text;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			this.txtHeaderText.Text = HeaderText;
			this.colorComboBox1.SolidColor = HeaderTextColor;
			this.numRowHeaderWidth.Value = RowHeaderWidth;
			this.chkAutoFit.Checked = AutoFitToCell;

			if (this.DefaultCellBody != null)
			{
				for (int i = 0; i < this.cmbCellBody.Items.Count; i++)
				{
					if (this.DefaultCellBody == this.cmbCellBody.Items[i])
					{
						this.cmbCellBody.SelectedIndex = i;
						break;
					}
				}
			}
			else if (this.cmbCellBody.Items.Count > 0)
			{
				this.cmbCellBody.SelectedIndex = 0;
			}
		}

		private class CellBodyItem
		{
			public Type Type { get; set; }

			public string Name { get; set; }

			public override string ToString()
			{
				return this.Name;
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			this.Text = (this.RowOrColumn == ReoGrid.RowOrColumn.Row ? LangResource.Row : LangResource.Column) + " " + LangResource.HeaderProperty_Caption;

			HeaderText = txtHeaderText.Text;
			HeaderTextColor = colorComboBox1.SolidColor;
			RowHeaderWidth = (int)numRowHeaderWidth.Value;
			AutoFitToCell = this.chkAutoFit.Checked;

			DefaultCellBody = (cmbCellBody.SelectedItem == null || cmbCellBody.SelectedIndex <= 0) ? null :
				((CellBodyItem)cmbCellBody.SelectedItem).Type;
		}
	}
}
