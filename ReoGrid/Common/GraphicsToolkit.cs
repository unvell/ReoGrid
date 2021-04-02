/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net/
 *
 * Common Graphics Toolkit
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * Author: Jing Lu <jingwood at unvell.com>
 * 
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;

#if WINFORM
using RGPen = System.Drawing.Pen;
using RGPens = System.Drawing.Pens;
using RGBrushes = System.Drawing.Brushes;
using PlatformGraphics = System.Drawing.Graphics;
using WFRect = System.Drawing.Rectangle;

#elif WPF
using RGPen = System.Windows.Media.Pen;
using RGBrushes = System.Windows.Media.Brushes;
using PlatformGraphics = System.Windows.Media.DrawingContext;
#elif ANDROID
using PlatformGraphics = Android.Graphics.Canvas;
using RGPen = Android.Graphics.Paint;

#elif iOS
using PlatformGraphics = CoreGraphics.CGContext;
using RGPen = CoreGraphics.CGContext;

#endif // WPF

#if WINFORM || ANDROID
using RGFloat = System.Single;
#elif WPF
using RGFloat = System.Double;
#elif iOS
using RGFloat = System.Double;
#endif // WINFORM

using unvell.ReoGrid.Graphics;

namespace unvell.Common
{
	internal static class GraphicsToolkit
	{
		#region Calculation
		public static bool PointInRect(Rectangle rect, Point p)
		{
			return rect.Left <= p.X && rect.Top <= p.Y
				&& rect.Right >= p.X && rect.Bottom >= p.Y;
		}
		public static double DistancePointToLine(Point startPoint, Point endPoint, Point target)
		{
			return DistancePointToLine(startPoint.X, startPoint.Y,
				endPoint.X, endPoint.Y, target);
		}
		public static double DistancePointToLine(RGFloat x1, RGFloat y1, RGFloat x2, RGFloat y2, Point target)
		{
			double a = y2 - y1;
			double b = x1 - x2;
			double c = x2 * y1 - x1 * y2;
			return Math.Abs(a * target.X + b * target.Y + c) / Math.Sqrt(a * a + b * b);
		}

		public static double DistancePointToPolygonBound(Point[] points, Point p, double min = 9999999, bool firstReturn = false)
		{
			double mindis = min + 1;

			Point linePoint = points[points.Length - 1];

			foreach (var linePoint2 in points)
			{
				double dist = DistancePointToLine(linePoint, linePoint2, p);
				if (mindis > dist) mindis = dist;
				linePoint = linePoint2;
			}

			return mindis;
		}

		public static double DistancePointToRectBounds(Rectangle rect, Point p, double min = 9999999, bool firstReturn = false)
		{
			return DistancePointToPolygonBound(new Point[] {
				new Point(rect.Left,rect.Top),
				new	Point(rect.Right,rect.Top),
				new Point(rect.Right,rect.Bottom),
				new Point(rect.Left,rect.Bottom)}, p, min, firstReturn);
		}

		public static bool PointOnRectangleBounds(Rectangle rect, Point p, RGFloat borderWidth = 1, double min = 9999999)
		{
			rect.Inflate(borderWidth, borderWidth);
			return rect.Contains(p) && DistancePointToRectBounds(rect, p, min) <= borderWidth;
		}

		const double PIAngleDelta = Math.PI / 180.0;

		public static float AngleToArc(RGFloat width, RGFloat height, RGFloat angle)
		{
			return (float)(180.0 / Math.PI * Math.Atan2(
				Math.Sin(angle * PIAngleDelta) * height / width,
				Math.Cos(angle * PIAngleDelta)));
		}

		public static Point PointAtArc(Rectangle rect, RGFloat angle)
		{
			RGFloat radians = (RGFloat)((GraphicsToolkit.AngleToArc(
				rect.Width, rect.Height, angle)) * PIAngleDelta);

			RGFloat ww = rect.Width / 2;
			RGFloat hh = rect.Height / 2;

			RGFloat x = (RGFloat)Math.Sin(radians) * ww;
			RGFloat y = (RGFloat)Math.Cos(radians) * hh;

			return new Point(rect.X + ww + x, rect.Y + hh - y);
		}
		#endregion // Calculation

		#region Drawing
		public enum TriangleDirection { Left, Up, Right, Down, }

		public static void FillTriangle(PlatformGraphics g, RGFloat size, Point loc, TriangleDirection dir = TriangleDirection.Down)
		{

#if WINFORM
			var p = System.Drawing.Pens.Black;
#elif WPF
			var p = new System.Windows.Media.Pen(RGBrushes.Black, 1);
#elif ANDROID
			var p = new RGPen();
			p.Color = Android.Graphics.Color.Black;
#elif iOS
			var p = g;
#endif // WPF

			FillTriangle(g, size, loc, dir, p);
		}

		public static void FillTriangle(PlatformGraphics g, RGFloat size, Point loc, TriangleDirection dir, RGPen p)
		{
			RGFloat x = loc.X;
			RGFloat y = loc.Y;

			switch (dir)
			{
				case TriangleDirection.Up:
					loc.X -= size / 2;
					for (x = 0; x < size / 2; x++)
					{
#if WINFORM || WPF
						g.DrawLine(p, new Point(loc.X + x, y), new Point(loc.X + size - x - 1, y));
#elif ANDROID
						g.DrawLine(loc.X + x, y, loc.X + size - x - 1, y, p);
#endif
						y--;
					}
					break;

				case TriangleDirection.Down:
					loc.X -= size / 2 - 1;
					y--;
					for (x = 0; x < size / 2; x++)
					{
#if WINFORM || WPF
						g.DrawLine(p, new Point(loc.X + x, y), new Point(loc.X + size - x, y));
#elif ANDROID
						g.DrawLine(loc.X + x, y, loc.X + size - x, y, p);
#endif
						y++;
					}
					break;

				case TriangleDirection.Left:
					loc.Y -= size / 2;
					for (y = 0; y < size / 2; y++)
					{
#if WINFORM || WPF
						g.DrawLine(p, new Point(x, loc.Y + y), new Point(x, loc.Y + size - y - 1));
#elif ANDROID
						g.DrawLine(x, loc.Y + y, x, loc.Y + size - y - 1, p);
#endif
						x--;
					}
					break;

				case TriangleDirection.Right:
					loc.Y -= size / 2;
					for (y = 0; y < size / 2; y++)
					{
#if WINFORM || WPF
						g.DrawLine(p, new Point(x, loc.Y + y), new Point(x, loc.Y + size - y - 1));
#elif ANDROID
						g.DrawLine(x, loc.Y + y, x, loc.Y + size - y - 1, p);
#endif
						x++;
					}
					break;

			}
		}

#endregion

#if WINFORM

#region Toolkit
		public static void Draw3DButton(System.Drawing.Graphics g, System.Drawing.Rectangle rect, bool isPressed)
		{
			// background
			WFRect bgRect = rect;
			//bgRect.Inflate(-1, -1);
			bgRect.Offset(1, 1);
			g.FillRectangle(isPressed ? RGBrushes.Black : RGBrushes.White, bgRect);

			// outter frame
			g.DrawLine(System.Drawing.Pens.Black, rect.X + 1, rect.Y, rect.Right - 1, rect.Y);
			g.DrawLine(System.Drawing.Pens.Black, rect.X + 1, rect.Bottom, rect.Right - 1, rect.Bottom);
			g.DrawLine(System.Drawing.Pens.Black, rect.X, rect.Y + 1, rect.X, rect.Bottom - 1);
			g.DrawLine(System.Drawing.Pens.Black, rect.Right, rect.Y + 1, rect.Right, rect.Bottom - 1);

			// content
			WFRect bodyRect = rect;
			bodyRect.Inflate(-1, -1);
			bodyRect.Offset(1, 1);
			g.FillRectangle(System.Drawing.Brushes.LightGray, bodyRect);

			// shadow
			g.DrawLines(isPressed ? RGPens.White : System.Drawing.Pens.DimGray, new System.Drawing.Point[] {
				new System.Drawing.Point(rect.Left+1,rect.Bottom-1),
				new System.Drawing.Point(rect.Right-1,rect.Bottom-1),
				new System.Drawing.Point(rect.Right-1,rect.Top+1),
			});
		}
#endregion
#endif // WINFORM
		/*
				public static bool ConvertWebColor(string code, out SolidColor c)
				{
					if (code.StartsWith("#"))
					{
						code = code.Substring(1);
					}

					if (code.Length == 3)
					{
						c = new SolidColor(
							Convert.ToByte(code.Substring(0, 1), 16),
							Convert.ToByte(code.Substring(1, 1), 16),
							Convert.ToByte(code.Substring(2, 1), 16));

						return true;
					}
					else if (code.Length == 6)
					{
						c = new SolidColor(
							Convert.ToByte(code.Substring(0, 2), 16),
							Convert.ToByte(code.Substring(2, 2), 16),
							Convert.ToByte(code.Substring(4, 2), 16));

						return true;
					}
					else if (code.Length == 8)
					{
						c = new SolidColor(
							Convert.ToByte(code.Substring(0, 2), 16),
							Convert.ToByte(code.Substring(2, 2), 16),
							Convert.ToByte(code.Substring(3, 2), 16),
							Convert.ToByte(code.Substring(4, 2), 16));

						return true;
					}
					else
					{
						c = new SolidColor(0, 0, 0, 0);
						return false;
					}
				}
				*/
	}
}
