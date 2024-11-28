/*****************************************************************************
 * 
 * ReoGrid - .NET 表計算スプレッドシートコンポーネント
 * https://reogrid.net/jp
 *
 * ReoGrid 日本語版デモプロジェクトは MIT ライセンスでリリースされています。
 * 
 * このソフトウェアは無保証であり、このソフトウェアの使用により生じた直接・間接の損害に対し、
 * 著作権者は補償を含むあらゆる責任を負いません。 
 * 
 * Copyright (c) 2012-2016 unvell.com, All Rights Reserved.
 * https://www.unvell.com/jp
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
