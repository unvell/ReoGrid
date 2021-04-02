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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace unvell.Common
{
	internal static class GraphicsToolkit
	{
		#region Calculation
		public static bool PointInRect(RectangleF rect, PointF p)
		{
			return rect.Left <= p.X && rect.Top <= p.Y
				&& rect.Right >= p.X && rect.Bottom >= p.Y;
		}
		#endregion // Calculation

		#region Drawing
		public enum TriangleDirection
		{
			Left,
			Up,
			Right,
			Down,
		}
		public static void FillTriangle(Graphics g, int width, Point loc)
		{
			FillTriangle(g, width, loc, TriangleDirection.Down);
		}
		public static void FillTriangle(Graphics g, float size, PointF loc, TriangleDirection dir)
		{
			FillTriangle(g, size, loc, dir, Pens.Black);
		}
		public static void FillTriangle(Graphics g, float size, PointF loc, TriangleDirection dir, Pen p)
		{
			float x = loc.X;
			float y = loc.Y;

			switch (dir)
			{
				case TriangleDirection.Up:
					loc.X -= size / 2;
					for (x = 0; x < size / 2; x++)
					{
						g.DrawLine(p, loc.X + x, y, loc.X + size - x - 1, y);
						y--;
					}
					break;

				case TriangleDirection.Down:
					loc.X -= size / 2 - 1;
					y--;
					for (x = 0; x < size / 2; x++)
					{
						g.DrawLine(p, loc.X + x, y, loc.X + size - x, y);
						y++;
					}
					break;

				case TriangleDirection.Left:
					loc.Y -= size / 2;
					for (y = 0; y < size / 2; y++)
					{
						g.DrawLine(p, x, loc.Y + y, x, loc.Y + size - y - 1);
						x--;
					}
					break;

				case TriangleDirection.Right:
					loc.Y -= size / 2;
					for (y = 0; y < size / 2; y++)
					{
						g.DrawLine(p, x, loc.Y + y, x, loc.Y + size - y - 1);
						x++;
					}
					break;

			}
		}

		public static void DrawTransparentBlock(Graphics g, Rectangle rect)
		{
			g.SetClip(rect);

			int u = 0, k = 0;
			for (int y = rect.Top; y < rect.Bottom; y += 5)
			{
				u = k++;
				for (int x = rect.Left; x < rect.Right; x += 5)
				{
					g.FillRectangle((u++ % 2) == 1 ? Brushes.White : Brushes.Gainsboro, x, y, 5, 5);
				}
			}

			g.ResetClip();
		}
		#endregion

		#region Toolkit
		public static void Draw3DButton(Graphics g, Rectangle rect, bool isPressed)
		{
			// background
			Rectangle bgRect = rect;
			//bgRect.Inflate(-1, -1);
			bgRect.Offset(1, 1);
			g.FillRectangle(isPressed ? Brushes.Black : Brushes.White, bgRect);

			// outter frame
			g.DrawLine(Pens.Black, rect.X + 1, rect.Y, rect.Right - 1, rect.Y);
			g.DrawLine(Pens.Black, rect.X + 1, rect.Bottom, rect.Right - 1, rect.Bottom);
			g.DrawLine(Pens.Black, rect.X, rect.Y + 1, rect.X, rect.Bottom - 1);
			g.DrawLine(Pens.Black, rect.Right, rect.Y + 1, rect.Right, rect.Bottom - 1);

			// content
			Rectangle bodyRect = rect;
			bodyRect.Inflate(-1, -1);
			bodyRect.Offset(1, 1);
			g.FillRectangle(Brushes.LightGray, bodyRect);

			// shadow
			g.DrawLines(isPressed ? Pens.White : Pens.DimGray, new Point[] {
				new Point(rect.Left+1,rect.Bottom-1),
				new Point(rect.Right-1,rect.Bottom-1),
				new Point(rect.Right-1,rect.Top+1),
			});
		}
		public static Color ConvertWebColor(string code)
		{
			if (code.StartsWith("#"))
			{
				code = code.Substring(1);
			}

			if (code.Length == 3)
			{
				return Color.FromArgb(
					Convert.ToInt32(code.Substring(0, 1), 16),
					Convert.ToInt32(code.Substring(1, 1), 16),
					Convert.ToInt32(code.Substring(2, 1), 16));
			}
			if (code.Length == 6)
			{
				return Color.FromArgb(
					Convert.ToInt32(code.Substring(0, 2), 16),
					Convert.ToInt32(code.Substring(2, 2), 16),
					Convert.ToInt32(code.Substring(4, 2), 16));
			}
			else if (code.Length == 8)
			{
				return Color.FromArgb(
					Convert.ToInt32(code.Substring(0, 2), 16),
					Convert.ToInt32(code.Substring(2, 2), 16),
					Convert.ToInt32(code.Substring(3, 2), 16),
					Convert.ToInt32(code.Substring(4, 2), 16));
			}
			else
				return Color.Empty;
		}

		#endregion
	}
}
