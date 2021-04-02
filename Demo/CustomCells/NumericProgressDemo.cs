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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using unvell.ReoGrid.CellTypes;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.Demo.CustomCells
{
	public partial class NumericProgressDemo : UserControl
	{
		private Worksheet worksheet;

		public NumericProgressDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;

			var rand = new Random();

			worksheet[1, 2] = "Try change the value below: ";

			for (int r = 3; r < 8; r++)
			{
				// set customize cell body
				worksheet[r, 2] = new NumericProgressCell();

				// set formula into cell which is used to get data from another cell
				worksheet[r, 2] = "=" + new CellPosition(r, 3).ToAddress(); // e.g. D3

				// generate a random value
				worksheet[r, 3] = Math.Round(rand.NextDouble(), 2);
			}

			// set data format as percent
			worksheet.SetRangeDataFormat(3, 3, 5, 2, DataFormat.CellDataFormatFlag.Percent,
				new DataFormat.NumberDataFormatter.NumberFormatArgs
				{
					DecimalPlaces = 0,
				});

			// change selection forward direction to down
			worksheet.SelectionForwardDirection = SelectionForwardDirection.Down;

			// put focus on cell
			worksheet.FocusPos = new CellPosition(3, 3);

			// link
			worksheet.MergeRange(12, 0, 1, 7);
			worksheet[11, 0] = "More info about Custom Cell:";
			worksheet[12, 0] = new HyperlinkCell(
				"https://reogrid.net/document/Custom%20Cell", true);
		}
	}

	internal class NumericProgressCell : CellBody
	{
		public override void OnPaint(CellDrawingContext dc)
		{
			double value = Cell.Worksheet.GetCellData<double>(this.Cell.Position);

			int width = (int)(Math.Round(value * Bounds.Width));

			if (width > 0)
			{
				System.Drawing.Graphics g = dc.Graphics.PlatformGraphics;

				Rectangle rect = new Rectangle(Bounds.Left, Bounds.Top + 1, width, Bounds.Height - 1);

				using (LinearGradientBrush lgb = new LinearGradientBrush(rect, SolidColor.Coral, SolidColor.IndianRed, 90f))
				{
					g.PixelOffsetMode = PixelOffsetMode.Half;
					g.FillRectangle(lgb, rect);
					g.PixelOffsetMode = PixelOffsetMode.Default;
				}
			}
		}
	}
}
