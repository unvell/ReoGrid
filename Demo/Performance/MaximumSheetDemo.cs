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

using System.Windows.Forms;

namespace unvell.ReoGrid.Demo.Performance
{
	public partial class MaximumSheetDemo : UserControl
	{
		public MaximumSheetDemo()
		{
			InitializeComponent();

			var worksheet = grid.CurrentWorksheet;

			worksheet.Resize(1048576, 32768);

			worksheet.MergeRange(1, 1, 1, 6);
			worksheet[1, 1] = "This is sample for maximum cells. (1,048,576 x 32,768)";

			worksheet.MergeRange(3, 1, 2, 8);
			worksheet[3, 1] = "Try scroll, zoom, edit in anywhere or adjust the row and column size.";
		}
	}
}
