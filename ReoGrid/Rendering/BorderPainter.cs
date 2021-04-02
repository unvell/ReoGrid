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
 * Author: Jing Lu <jingwood at unvell.com>
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;

using unvell.Common;
using unvell.ReoGrid.Graphics;

#if WINFORM
using RGPointF = System.Drawing.PointF;
using RGFloat = System.Single;
using RGColors = System.Drawing.Color;
using RGPen = System.Drawing.Pen;
using RGPenColor = System.Drawing.Color;
using RGDashStyles = System.Drawing.Drawing2D.DashStyle;
using PlatformGraphics = System.Drawing.Graphics;
#elif WPF
using RGPointF = System.Windows.Point;
using RGFloat = System.Double;
using RGPen = System.Windows.Media.Pen;
using RGPenColor = System.Windows.Media.Brushes;
using RGDashStyles = System.Windows.Media.DashStyles;
using RGSolidBrush = System.Windows.Media.SolidColorBrush;
using PlatformGraphics = System.Windows.Media.DrawingContext;
#elif ANDROID
using Android.Graphics;
using PlatformGraphics = Android.Graphics.Canvas;
using RGPen = Android.Graphics.Paint;
using RGColor = Android.Graphics.Color;
using RGPenColor = Android.Graphics.Color;
using RGFloat = System.Single;
using RGPointF = Android.Graphics.PointF;
#elif iOS
using CoreGraphics;
using RGFloat = System.Double;
using PlatformGraphics = CoreGraphics.CGContext;
using RGPen = CoreGraphics.CGContext;

#endif // ANDROID

namespace unvell.ReoGrid.Rendering
{
	/// <summary>
	/// Draw borders at the specified location.
	/// </summary>
#if WINFORM
	public 
#endif // WINFORM
	sealed class BorderPainter : IDisposable
	{
		private static BorderPainter instance;

		/// <summary>
		/// Get BorderPainter instance
		/// </summary>
		public static BorderPainter Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new BorderPainter();
				}

				return instance;
			}
		}

#if WINFORM || WPF || ANDROID
		private readonly RGPen[] pens = new RGPen[14];

		private BorderPainter()
		{

			RGPen p;

			// Solid
#if WINFORM || WPF
			p = new RGPen(RGPenColor.Black, 1);
#elif ANDROID
			p = new RGPen();
			p.Color = Android.Graphics.Color.Black;
			p.SetPathEffect(new DashPathEffect(new float[]{ 1, 1 }, 1));
#endif // ANDROID
			pens[(byte)BorderLineStyle.Solid] = p;

			// Dahsed
#if WINFORM || WPF
			p = new RGPen(RGPenColor.Black, 1);
			p.DashStyle = RGDashStyles.Dash;
#elif ANDROID
			p = new RGPen();
			p.Color = Android.Graphics.Color.Black;
			p.SetPathEffect(new DashPathEffect(new float[] { 1, 1 }, 1));
#endif // ANDROID
			pens[(byte)BorderLineStyle.Dashed] = p;

			// Dotted
#if WINFORM || WPF
			p = new RGPen(RGPenColor.Black, 1);
			p.DashStyle = RGDashStyles.Dot;
#elif ANDROID
			p = new RGPen();
			p.Color = Android.Graphics.Color.Black;
			p.SetPathEffect(new DashPathEffect(new float[] { 1, 1 }, 1));
#endif // ANDROID
			pens[(byte)BorderLineStyle.Dotted] = p;

			// DoubleLine
#if WINFORM || WPF
			p = new RGPen(RGPenColor.Black, 3);
#if WINFORM
			p.CompoundArray = new float[] { 0f, 0.2f, 0.8f, 1f };
#endif
#elif ANDROID
			p = new RGPen();
			p.Color = Android.Graphics.Color.Black;
			p.SetPathEffect(new DashPathEffect(new float[] { 1, 1 }, 1));
#endif // ANDROID
			pens[(byte)BorderLineStyle.DoubleLine] = p;

			// Dashed2
#if WINFORM || WPF
			p = new RGPen(RGPenColor.Black, 1);
#if WINFORM
			p.DashPattern = new float[] { 2f, 2f };
#endif
#elif ANDROID
			p = new RGPen();
			p.Color = Android.Graphics.Color.Black;
			p.SetPathEffect(new DashPathEffect(new float[] { 1, 1 }, 1));
#endif // ANDROID
			pens[(byte)BorderLineStyle.Dashed2] = p;

			// DashDot
#if WINFORM || WPF
			p = new RGPen(RGPenColor.Black, 1);
#if WINFORM
			p.DashPattern = new float[] { 10f, 3f, 3f, 3f };
#elif WPF
			p.DashStyle = RGDashStyles.DashDot;
#endif
#elif ANDROID
			p = new RGPen();
			p.Color = Android.Graphics.Color.Black;
			p.SetPathEffect(new DashPathEffect(new float[] { 1, 1 }, 1));
#endif // ANDROID
			pens[(byte)BorderLineStyle.DashDot] = p;

			// DashDotDot
#if WINFORM || WPF
			p = new RGPen(RGPenColor.Black, 1);
#if WINFORM
			p.DashPattern = new float[] { 10f, 3f, 3f, 3f, 3f, 3f };
#elif WPF
			p.DashStyle = RGDashStyles.DashDotDot;
#endif
#elif ANDROID
			p = new RGPen();
			p.Color = Android.Graphics.Color.Black;
			p.SetPathEffect(new DashPathEffect(new float[] { 1, 1 }, 1));
#endif // ANDROID

			pens[(byte)BorderLineStyle.DashDotDot] = p;

			// BoldDashDot
#if WINFORM || WPF
			p = new RGPen(RGPenColor.Black, 2);
			p.DashStyle = RGDashStyles.DashDot;
#elif ANDROID
			p = new RGPen();
			p.Color = Android.Graphics.Color.Black;
			p.SetPathEffect(new DashPathEffect(new float[] { 1, 1 }, 1));
#endif // ANDROID
			pens[(byte)BorderLineStyle.BoldDashDot] = p;

			// BoldDashDotDot
#if WINFORM || WPF
			p = new RGPen(RGPenColor.Black, 2);
			p.DashStyle = RGDashStyles.DashDotDot;
#elif ANDROID
			p = new RGPen();
			p.Color = Android.Graphics.Color.Black;
			p.SetPathEffect(new DashPathEffect(new float[] { 1, 1 }, 1));
#endif // ANDROID
			pens[(byte)BorderLineStyle.BoldDashDotDot] = p;

			// BoldDotted
#if WINFORM || WPF
			p = new RGPen(RGPenColor.Black, 2);
			p.DashStyle = RGDashStyles.Dot;
#elif ANDROID
			p = new RGPen();
			p.Color = Android.Graphics.Color.Black;
			p.SetPathEffect(new DashPathEffect(new float[] { 1, 1 }, 1));
#endif // ANDROID
			pens[(byte)BorderLineStyle.BoldDotted] = p;

			// BoldDashed
#if WINFORM || WPF
			p = new RGPen(RGPenColor.Black, 2);
			p.DashStyle = RGDashStyles.Dash;
#elif ANDROID
			p = new RGPen();
			p.Color = Android.Graphics.Color.Black;
			p.SetPathEffect(new DashPathEffect(new float[] { 1, 1 }, 1));
#endif // ANDROID
			pens[(byte)BorderLineStyle.BoldDashed] = p;

			// BoldSolid
#if WINFORM || WPF
			p = new RGPen(RGPenColor.Black, 2);
#elif ANDROID
			p = new RGPen();
			p.Color = Android.Graphics.Color.Black;
			p.SetPathEffect(new DashPathEffect(new float[] { 1, 1 }, 1));
#endif // ANDROID
			pens[(byte)BorderLineStyle.BoldSolid] = p;

			// BoldSolidStrong
#if WINFORM || WPF
			p = new RGPen(RGPenColor.Black, 3);
#elif ANDROID
			p = new RGPen();
			p.Color = Android.Graphics.Color.Black;
			p.SetPathEffect(new DashPathEffect(new float[] { 1, 1 }, 1));
#endif // ANDROID
			pens[(byte)BorderLineStyle.BoldSolidStrong] = p;

		}

		// endif // WINFORM || WPF || ANDROID
#elif iOS
		private BorderPainter()
		{
		}
#endif // iOS

		/// <summary>
		/// Draw border at specified location
		/// </summary>
		/// <param name="g">instance for graphics object</param>
		/// <param name="x">x coordinate of start point</param>
		/// <param name="y">y coordinate of start point</param>
		/// <param name="x2">x coordinate of end point</param>
		/// <param name="y2">y coordinate of end point</param>
		/// <param name="style">style instance of border</param>
		public void DrawLine(PlatformGraphics g, RGFloat x, RGFloat y, RGFloat x2, RGFloat y2, RangeBorderStyle style)
		{
			DrawLine(g, x, y, x2, y2, style.Style, style.Color);
		}


		/// <summary>
		/// Draw border at specified position.
		/// </summary>
		/// <param name="g">Instance for graphics object.</param>
		/// <param name="x">X coordinate of start point.</param>
		/// <param name="y">Y coordinate of start point.</param>
		/// <param name="x2">X coordinate of end point.</param>
		/// <param name="y2">Y coordinate of end point.</param>
		/// <param name="style">Style flag of border.</param>
		/// <param name="color">Color of border.</param>
		/// <param name="bgPen">Fill pen used when drawing double outline.</param>
		public void DrawLine(PlatformGraphics g, RGFloat x, RGFloat y, RGFloat x2, RGFloat y2, BorderLineStyle style,
			SolidColor color, RGPen bgPen = null)
		{
			if (style == BorderLineStyle.None) return;

#if WINFORM || WPF || ANDROID


#if WINFORM
				RGPen p = pens[(byte)style];

			lock (p)
			{
				p.Color = color;
				p.StartCap = System.Drawing.Drawing2D.LineCap.Square;
				p.EndCap = System.Drawing.Drawing2D.LineCap.Square;
				g.DrawLine(p, new RGPointF(x, y), new RGPointF(x2, y2));
			}
#elif WPF
			// get template pen from cache list
			var tp = pens[(byte)style];

			// create new WPF pen
			var p = new RGPen(new RGSolidBrush(color), tp.Thickness);
			// copy the pen style from template
			p.DashStyle = tp.DashStyle;

			p.StartLineCap = System.Windows.Media.PenLineCap.Square;
			p.EndLineCap = System.Windows.Media.PenLineCap.Square;

			System.Windows.Media.GuidelineSet gs = new System.Windows.Media.GuidelineSet();
			double halfPenWidth = p.Thickness / 2;
			gs.GuidelinesX.Add(x + halfPenWidth);
			gs.GuidelinesY.Add(y + halfPenWidth);
			gs.GuidelinesX.Add(x2 + halfPenWidth);
			gs.GuidelinesY.Add(y2 + halfPenWidth);
			g.PushGuidelineSet(gs);
				g.DrawLine(p, new RGPointF(x, y), new RGPointF(x2, y2));
#elif ANDROID
				g.DrawLine(x, y, x2, y2, p);
#endif

	

			if (style == BorderLineStyle.DoubleLine && bgPen != null)
			{
				lock (bgPen)
				{
#if WINFORM || WPF
					g.DrawLine(bgPen, new RGPointF(x, y), new RGPointF(x2, y2));
#elif ANDROID
					g.DrawLine(x, y, x2, y2, bgPen);
#endif // WPF
				}
			}

#if WPF
			g.Pop();
#endif // WPF

//#endif // WINFORM || WPF || ANDROID
#elif iOS
			using (var path = new CGPath())
			{
				path.AddLines(new CGPoint[] { new CGPoint(x, y), new CGPoint(x2, y2) });

				switch (style)
				{
					default:
					case BorderLineStyle.Solid:
						g.AddPath(path);
						g.SetStrokeColor(color);
						g.DrawPath(CGPathDrawingMode.Stroke);
						break;
				}
			}
#endif // iOS

		}

		/// <summary>
		/// Release all cached objects.
		/// </summary>
		public void Dispose()
		{
#if WINFORM || ANDROID
			for (int i = 1; i < pens.Length; i++)
			{
				pens[i].Dispose();
				pens[i] = null;
			}

#endif
		}

	}
}
