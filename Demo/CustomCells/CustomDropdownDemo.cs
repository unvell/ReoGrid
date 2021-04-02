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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using unvell.ReoGrid.CellTypes;
using unvell.ReoGrid.Demo.Properties;

namespace unvell.ReoGrid.Demo.CustomCells
{
	public partial class CustomDropdownDemo : UserControl
	{
		private Worksheet worksheet;

		public CustomDropdownDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;

			// set sheet default style
			worksheet.SetRangeStyles(RangePosition.EntireRange, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.FontName | PlainStyleFlag.VerticalAlign,
				FontName = "Arial",
				VAlign = ReoGridVerAlign.Middle,
			});

			worksheet.ColumnHeaders["C"].Width = 120;

			worksheet["B3"] = "Item 1:";
			worksheet["C3"] = "Choose...";
			worksheet["C3"] = new ListViewDropdownCell();
			worksheet.Ranges["C3"].BorderOutside = RangeBorderStyle.GraySolid;

			worksheet["B5"] = "Item 2:";
			worksheet["C5"] = "Choose...";
			worksheet["C5"] = new ListViewDropdownCell();
			worksheet.Ranges["C5"].BorderOutside = RangeBorderStyle.GraySolid;
		}

		private void chkGridlines_CheckedChanged(object sender, EventArgs e)
		{
			worksheet.SetSettings(WorksheetSettings.View_ShowGridLine, chkGridlines.Checked);
		}
	}

	class ListViewDropdownCell : DropdownCell
	{
		private ListView listView;

		public ListViewDropdownCell()
		{
			// create listview
			this.listView = new ListView()
			{
				BorderStyle = System.Windows.Forms.BorderStyle.None,
				View = View.Details,
				FullRowSelect = true,
			};

			// set dropdown control
			this.DropdownControl = listView;

			// add listview columns
			this.listView.Columns.Add("Column 1", 120);
			this.listView.Columns.Add("Column 2", 120);

			// add groups and items
			var group1 = listView.Groups.Add("grp1", "Group 1");
			listView.Items.Add(new ListViewItem(new string[] { "Item 1.1", "Subitem 1.1" }) { Group = group1 });
			listView.Items.Add(new ListViewItem(new string[] { "Item 1.2", "Subitem 1.2" }) { Group = group1 });
			listView.Items.Add(new ListViewItem(new string[] { "Item 1.3", "Subitem 1.3" }) { Group = group1 });

			var group2 = listView.Groups.Add("grp2", "Group 2");
			listView.Items.Add(new ListViewItem(new string[] { "Item 2.1", "Subitem 2.1" }) { Group = group2 });
			listView.Items.Add(new ListViewItem(new string[] { "Item 2.2", "Subitem 2.2" }) { Group = group2 });
			listView.Items.Add(new ListViewItem(new string[] { "Item 2.3", "Subitem 2.3" }) { Group = group2 });

			// enlarge the dropdown panel
			this.MinimumDropdownWidth = 300;

			// add click event handler
			this.listView.Click += listView_Click;

		}

		void listView_Click(object sender, EventArgs e)
		{
			if (this.listView.SelectedItems.Count > 0)
			{
				this.Cell.Data = this.listView.SelectedItems[0].Text;
				PullUp();
			}
		}
	}
}
