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
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace unvell.ReoGrid.Demo.CellAndRange
{
	public partial class MergeCellsDemo : UserControl
	{
		private Worksheet worksheet;

		public MergeCellsDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;

			worksheet.SelectionRangeChanged += (s, e) =>
			{
				label1.Text = "Selected range: " + worksheet.SelectionRange.ToString();
			};
		}

		private void btnMerge_Click(object sender, EventArgs e)
		{
			if (worksheet.SelectionRange.Rows == 1 && worksheet.SelectionRange.Cols == 1)
			{
				MessageBox.Show("Selected range must contain at least two cells.");
			}
			else
			{
				try
				{
					worksheet.MergeRange(worksheet.SelectionRange);
				}
				catch (RangeIntersectionException)
				{
					MessageBox.Show("Cannot merge an intersected range.");
				}
			}
		}

		private void btnUnmerge_Click(object sender, EventArgs e)
		{
			worksheet.UnmergeRange(worksheet.SelectionRange);
		}

		private void btnMergeByScript_Click(object sender, EventArgs e)
		{
			if (worksheet.SelectionRange.Rows == 1 && worksheet.SelectionRange.Cols == 1)
			{
				MessageBox.Show("Selected range must contain at least two cells.");
			}
			else
			{
				try
				{
					this.grid.RunScript(txtMergeScript.Text);
				}
				catch (Exception ex)
				{
					MessageBox.Show("Script Error: " + ex.Message);
				}
			}
		}

		private void btnUnmergeByScript_Click(object sender, EventArgs e)
		{
			try
			{
				this.grid.RunScript(txtUnmergeScript.Text);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Script Error: " + ex.Message);
			}
		}

		private void btnSimulateException_Click(object sender, EventArgs e)
		{
			worksheet.Reset();

			try
			{
				// try to merge an intersected range will get an exception
				worksheet.MergeRange(new RangePosition(2, 2, 5, 5));
				worksheet.MergeRange(new RangePosition(3, 3, 5, 5));
			}
			catch(RangeIntersectionException)
			{
				MessageBox.Show("Exception RangeIntersectionException catched!\n\n Cannot change part of a range.");
			}
		}
	}
}
