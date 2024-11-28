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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace unvell.ReoGrid.Demo.Features
{
	public partial class MultisheetDemo : UserControl
	{
		private Worksheet worksheet;

		public MultisheetDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;
		}

		private static readonly Random rand = new Random();

		private static readonly WorksheetRangeStyle grayBackgroundStyle = new WorksheetRangeStyle
		{
			Flag = PlainStyleFlag.BackColor,
		};

		private WorksheetRangeStyle GetRandomBackColorStyle()
		{
			grayBackgroundStyle.BackColor = Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255));
			return grayBackgroundStyle;
		}

		private void btnAddWorksheet_Click(object sender, EventArgs e)
		{
			// create worksheet
			var newSheet = this.grid.CreateWorksheet();

			// set worksheet background color
			newSheet.SetRangeStyles(RangePosition.EntireRange, GetRandomBackColorStyle());

			// add worksheet into workbook
			this.grid.AddWorksheet(newSheet);

			// set worksheet as current focus
			grid.CurrentWorksheet = newSheet;
		}

		private void btnRemoveSheet_Click(object sender, EventArgs e)
		{
			if (grid.Worksheets.Count <= 1)
			{
				// cannot remove the last one
				MessageBox.Show("The last worksheet cannot be removed.");
			}
			else
			{
				// remove current focus worksheet
				grid.RemoveWorksheet(grid.CurrentWorksheet);
			}
		}

		private void btnSheetList_Click(object sender, EventArgs e)
		{
			// remove old menu items
			foreach (ToolStripMenuItem oldMenuItem in this.sheetListContextMenuStrip.Items)
			{
				oldMenuItem.Click -= menuItem_Click;
			}

			this.sheetListContextMenuStrip.Items.Clear();

			// add menu item for every worksheet
			foreach (var sheet in this.grid.Worksheets)
			{
				ToolStripMenuItem menuItem = new ToolStripMenuItem(sheet.Name)
				{
					Tag = sheet,
					Checked = (sheet == this.grid.CurrentWorksheet),
				};

				menuItem.Click += menuItem_Click;

				this.sheetListContextMenuStrip.Items.Add(menuItem);
			}

			sheetListContextMenuStrip.Show(this.btnSheetList, new Point(0, this.btnSheetList.Height));
		}

		private void sheetListContextMenuStrip_Opening(object sender, CancelEventArgs e)
		{
			//// remove old menu items
			//foreach (ToolStripMenuItem oldMenuItem in this.sheetListContextMenuStrip.Items)
			//{
			//	oldMenuItem.Click -= menuItem_Click;
			//}

			//this.sheetListContextMenuStrip.Items.Clear();

			//// add menu item for every worksheet
			//foreach (var sheet in this.grid.Worksheets)
			//{
			//	ToolStripMenuItem menuItem = new ToolStripMenuItem(sheet.Name)
			//	{
			//		Tag = sheet,
			//	};

			//	menuItem.Click += menuItem_Click;

			//	this.sheetListContextMenuStrip.Items.Add(menuItem);
			//}
		}

		void menuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
			this.grid.CurrentWorksheet = (Worksheet)menuItem.Tag;
		}

	}

}
