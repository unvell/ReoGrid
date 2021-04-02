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

namespace unvell.ReoGrid.Demo.Features
{
	public partial class PickRangeDemo : UserControl
	{
		private int currentRow = 0;

		public PickRangeDemo()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			StartPickRange();
		}

		private void StartPickRange()
		{
			grid.PickRange((inst, range) =>
			{
				// this delegate will be invoked after range picked by user
				//

				MessageBox.Show("User picked range: " + range.ToString());

				if (currentRow < grid.CurrentWorksheet.RowCount - 1)
				{
					grid.CurrentWorksheet[currentRow++, 0] = range.ToAddress();
				}

				// return true to pick range only once
				// return false to pick range again
				return false;

			}, 

			// cursor when user picking range on spreadsheet
			Cursors.Hand);
		}
	}
}
