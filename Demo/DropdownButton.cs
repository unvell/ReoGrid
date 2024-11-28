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

using System.Drawing;
using System.Windows.Forms;

namespace unvell.ReoGrid.Demo
{
	class DropdownButton : Button
	{
		protected override void OnPaint(PaintEventArgs pevent)
		{
			base.OnPaint(pevent);

			const float size = 7;

			PointF loc = new PointF(Width - 14, Height / 2 - 1);

			float x = loc.X;
			float y = loc.Y;

			loc.X -= size / 2 - 1;
			y--;
			for (x = 0; x < size / 2; x++)
			{
				pevent.Graphics.DrawLine(Pens.Black, loc.X + x, y, loc.X + size - x, y);
				y++;
			}
		}
	}

	class NewLabelButton : Button
	{
		private static readonly Font font = new Font("Small Fonts", 5.9f, FontStyle.Regular);

		protected override void OnPaint(PaintEventArgs pevent)
		{
			base.OnPaint(pevent);

			pevent.Graphics.DrawString("new!", font, Brushes.Red, this.ClientRectangle.Right - 20, 0);
		}
	}
}
