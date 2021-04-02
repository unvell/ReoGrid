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

#if WPF
//#define GRID_GUIDELINE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

using unvell.Common;
using unvell.ReoGrid.Graphics;

using Point = unvell.ReoGrid.Graphics.Point;
using WPFPoint = System.Windows.Point;
using RGFont = System.Windows.Media.Typeface;

using DrawingContext = unvell.ReoGrid.Rendering.DrawingContext;
using WPFDrawingContext = System.Windows.Media.DrawingContext;
using unvell.ReoGrid.Drawing.Text;

namespace unvell.ReoGrid.Rendering
{
	#region Graphics
	internal class WPFGraphics : IGraphics
	{
		protected ResourcePoolManager resourceManager = new ResourcePoolManager();

		public ResourcePoolManager ResourcePoolManager
		{
			get { return this.resourceManager; }
		}

		private WPFDrawingContext g = null;

		public WPFGraphics()
		{
		}

		public WPFDrawingContext PlatformGraphics { get { return g; } set { this.g = value; } }

		#region Line
		public void DrawLine(Pen p, double x1, double y1, double x2, double y2)
		{
			DrawLine(p, new Point(x1, y1), new Point(x2, y2));
		}

		public void DrawLine(Pen p, Point startPoint, Point endPoint)
		{
#if !GRID_GUIDELINE
			double halfPenWidth = p.Thickness / 2;

			// Create a guidelines set
			System.Windows.Media.GuidelineSet guidelines = new System.Windows.Media.GuidelineSet();

			guidelines.GuidelinesX.Add(startPoint.X + halfPenWidth);
			guidelines.GuidelinesY.Add(startPoint.Y + halfPenWidth);

			g.PushGuidelineSet(guidelines);
#endif // GRID_GUIDELINE

			g.DrawLine(p, (System.Windows.Point)startPoint, (System.Windows.Point)endPoint);

#if !GRID_GUIDELINE
			g.Pop();
#endif // GRID_GUIDELINE
		}

		public void DrawLine(Point startPoint, Point endPoint, SolidColor color)
		{
			this.g.DrawLine(this.resourceManager.GetPen(color), (System.Windows.Point)startPoint, (System.Windows.Point)endPoint);
		}

		public void DrawLine(double x1, double y1, double x2, double y2, SolidColor color)
		{
			var pen = this.resourceManager.GetPen(color);
			this.DrawLine(pen, x1, y1, x2, y2);
		}

		public void DrawLine(double x1, double y1, double x2, double y2, SolidColor color, double width, LineStyles style)
		{
			var p = this.resourceManager.GetPen(color, width, ToWPFDashStyle(style));

			if (p != null)
			{
				g.DrawLine(p, new System.Windows.Point(x1, y1), new System.Windows.Point(x2, y2));
			}
		}

		public void DrawLine(Point startPoint, Point endPoint, SolidColor color, double width, LineStyles style)
		{
			var p = this.resourceManager.GetPen(color, width, ToWPFDashStyle(style));

			if (p != null)
			{
				g.DrawLine(p, startPoint, endPoint);
			}
		}

		public void DrawLines(Point[] points, int start, int length, SolidColor color, double width, LineStyles style)
		{
			if (!color.IsTransparent && length > 1)
			{
				var p = this.resourceManager.GetPen(color, width, ToWPFDashStyle(style));

				if (p != null)
				{
					var geo = new PathGeometry();
					for (int i = 1, k = start + 1; i < length; i++, k++)
					{
						geo.AddGeometry(new LineGeometry(points[k - 1], points[k]));
					}
					g.DrawGeometry(null, p, geo);
				}
			}
		}

		//public void DrawLine(SolidColor color, Point startPoint, Point endPoint, double width, LineStyles style, LineCapStyles startCap, LineCapStyles endCap)
		//{
		//	var b = this.resourceManager.GetBrush(color);

		//	var p = new Pen(b, width);

		//	if (startCap == LineCapStyles.Arrow)
		//	{
		//		p.StartLineCap = PenLineCap.Triangle;
		//	}

		//	if (endCap == LineCapStyles.Arrow)
		//	{
		//		p.EndLineCap = PenLineCap.Triangle;
		//	}

		//	this.g.DrawLine(p, startPoint, endPoint);
		//}
		#endregion // Line

		#region Rectangle
		public void DrawRectangle(Pen p, Rectangle rect)
		{
			g.DrawRectangle(null, p, rect);
		}

		public void DrawRectangle(Pen p, double x, double y, double w, double h)
		{
			g.DrawRectangle(null, p, new Rect(x, y, w, h));
		}

		public void DrawRectangle(Rectangle rect, SolidColor color)
		{
			var p = this.resourceManager.GetPen(color);
			if (p != null) this.g.DrawRectangle(null, p, (System.Windows.Rect)rect);
		}

		public void DrawRectangle(double x, double y, double width, double height, SolidColor color)
		{
			var p = this.resourceManager.GetPen(color);
			this.g.DrawRectangle(null, p, new Rect(x, y, width, height));
		}

		public void FillRectangle(HatchStyles style, SolidColor hatchColor, SolidColor bgColor, Rectangle rect)
		{
			// TODO
		}

		public void FillRectangle(HatchStyles style, SolidColor hatchColor, SolidColor bgColor, double x, double y, double width, double height)
		{
			// TODO
		}

		public void FillRectangle(Rectangle rect, IColor color)
		{
			if (color is SolidColor)
			{
				this.g.DrawRectangle(this.resourceManager.GetBrush((SolidColor)color), null, (System.Windows.Rect)rect);
			}
		}

		public void FillRectangle(double x, double y, double width, double height, IColor color)
		{
			if (color is SolidColor)
			{
				this.g.DrawRectangle(this.resourceManager.GetBrush((SolidColor)color), null, new Rect(x, y, width, height));
			}
		}

		public void FillRectangle(Brush b, double x, double y, double width, double height)
		{
			this.g.DrawRectangle(b, null, new Rect(x, y, width, height));
		}

		public void FillRectangleLinear(SolidColor color1, SolidColor color2, double angle, Rectangle rect)
		{
			LinearGradientBrush lgb = new LinearGradientBrush(color1, color2, angle);
			g.DrawRectangle(lgb, null, (System.Windows.Rect)rect);
		}


		public void DrawRectangle(Rectangle rect, SolidColor color, double width, LineStyles lineStyle)
		{
			var p = this.resourceManager.GetPen(color, width, ToWPFDashStyle(lineStyle));
			if (p != null)
			{
				this.g.DrawRectangle(null, p, rect);
			}
		}

		public void DrawAndFillRectangle(Rectangle rect, SolidColor lineColor, IColor fillColor)
		{
			if (fillColor is SolidColor)
			{
				this.g.DrawRectangle(this.resourceManager.GetBrush((SolidColor)fillColor),
					this.resourceManager.GetPen(lineColor), (System.Windows.Rect)rect);
			}
		}

		public void DrawAndFillRectangle(Rectangle rect, SolidColor lineColor, IColor fillColor, double width, LineStyles lineStyle)
		{
			var p = this.resourceManager.GetPen(lineColor, width, ToWPFDashStyle(lineStyle));
			var b = this.resourceManager.GetBrush(fillColor.ToSolidColor());

			if (p != null && b != null)
			{
				this.g.DrawRectangle(b, p, rect);
			}
		}
		#endregion // Rectangle

		public void DrawImage(ImageSource image, double x, double y, double width, double height)
		{
			g.DrawImage(image, new Rect(x, y, width, height));
		}

		public void DrawImage(ImageSource image, Rectangle bounds)
		{
			g.DrawImage(image, (System.Windows.Rect)bounds);
		}

		public void FillPolygon(Point[] points, SolidColor startColor, SolidColor endColor, double angle, Rectangle rect)
		{
			WPFPoint[] pts = new WPFPoint[points.Length];
			for (int i = 0; i < pts.Length; i++)
			{
				pts[i] = (System.Windows.Point)points[i];
			}

			StreamGeometry streamGeometry = new StreamGeometry();
			using (StreamGeometryContext geometryContext = streamGeometry.Open())
			{
				geometryContext.PolyLineTo(pts, true, true);
			}

			LinearGradientBrush lgb = new LinearGradientBrush(startColor, endColor, angle);
			g.DrawGeometry(lgb, null, streamGeometry);
		}

		#region Text

		public void DrawText(string text, string fontName, double size, SolidColor color, Rectangle rect)
		{
			DrawText(text, fontName, size, color, rect, ReoGridHorAlign.Left, ReoGridVerAlign.Top);
		}

		public void DrawText(string text, string fontName, double size, SolidColor color, Rectangle rect, ReoGridHorAlign halign, ReoGridVerAlign valign)
		{
			if (rect.Width > 0 && rect.Height > 0 && !string.IsNullOrEmpty(text))
			{
				FormattedText ft = new FormattedText(text, System.Threading.Thread.CurrentThread.CurrentCulture,
					 FlowDirection.LeftToRight, this.resourceManager.GetTypeface(fontName),
					 size * PlatformUtility.GetDPI() / 72.0,
					 this.resourceManager.GetBrush(color));

				ft.MaxTextWidth = rect.Width;
				ft.MaxTextHeight = rect.Height;

				switch (halign)
				{
					default:
						break;

					case ReoGridHorAlign.Left:
						ft.TextAlignment = TextAlignment.Left;
						break;

					case ReoGridHorAlign.Center:
						ft.TextAlignment = TextAlignment.Center;
						break;

					case ReoGridHorAlign.Right:
						ft.TextAlignment = TextAlignment.Right;
						break;
				}

				switch (valign)
				{
					default:
						break;

					case ReoGridVerAlign.Middle:
						rect.Y += (rect.Height - ft.Height) / 2;
						break;

					case ReoGridVerAlign.Bottom:
						rect.Y += (rect.Height - ft.Height);
						break;
				}

				g.DrawText(ft, rect.Location);
			}
		}

		public Graphics.Size MeasureText(string text, string fontName, double fontSize, Graphics.Size displayArea)
		{
			// in WPF environment do not measure text, use FormattedText instead
			return new Graphics.Size(0, 0);
		}

		#endregion // Text

		#region Clip
		public void PushClip(Rectangle clipRect)
		{
			this.g.PushClip(new RectangleGeometry((System.Windows.Rect)clipRect));
		}

		public void PopClip()
		{
			this.g.Pop();
		}
		#endregion // Clip

		#region Transform

		private Stack<MatrixTransform> transformStack = new Stack<MatrixTransform>();

		public void PushTransform()
		{
			this.PushTransform(Matrix.Identity);
		}

		public void PushTransform(Matrix m)
		{
			var mt = new MatrixTransform(m);
			this.transformStack.Push(mt);
			this.g.PushTransform(mt);
		}

		Matrix IGraphics.PopTransform()
		{
			this.g.Pop();
			return this.transformStack.Pop().Matrix;
		}

		public Matrix PopTransform()
		{
			this.g.Pop();
			return this.transformStack.Pop().Matrix;
		}

		public void TranslateTransform(double x, double y)
		{
			if (transformStack.Count > 0)
			{
				var mt = transformStack.Peek();
				var m2 = new Matrix();
				m2.Translate(x, y);
				mt.Matrix = m2 * mt.Matrix;
			}
		}

		public void ScaleTransform(double x, double y)
		{
			if (x != 0 && y != 0
				&& x != 1 && y != 1
				&& transformStack.Count > 0)
			{
				var mt = transformStack.Peek();
				var m2 = new Matrix();
				m2.Scale(x, y);
				mt.Matrix = m2 * mt.Matrix;
			}
		}

		public void RotateTransform(double angle)
		{
			if (transformStack.Count > 0)
			{
				var mt = transformStack.Peek();
				var m = mt.Matrix;
				var m2 = new Matrix();
				m2.RotateAt(angle, m.OffsetX, m.OffsetY);
				mt.Matrix = m2 * mt.Matrix;
			}
		}

		public void ResetTransform()
		{
			if (transformStack.Count > 0)
			{
				var mt = transformStack.Peek();
				mt.Matrix = Matrix.Identity;
			}
		}
		#endregion // Transform

		#region Ellipse

		public void DrawEllipse(SolidColor color, Rectangle rectangle)
		{
			var p = this.resourceManager.GetPen(color);
			if (p != null)
			{
				this.g.DrawEllipse(null, p, new Point(rectangle.X + rectangle.Width / 2,
						rectangle.Y + rectangle.Height / 2), rectangle.Width, rectangle.Height);
			}
		}

		public void DrawEllipse(SolidColor color, double x, double y, double width, double height)
		{
			var p = this.resourceManager.GetPen(color);
			if (p != null)
			{
				this.g.DrawEllipse(null, p, new Point(x, y), width, height);
			}
		}

		public void DrawEllipse(Pen pen, Rectangle rectangle)
		{
			this.g.DrawEllipse(null, pen, rectangle.Location, rectangle.Width, rectangle.Height);
		}

		public void FillEllipse(Brush b, Rectangle rectangle)
		{
			this.g.DrawEllipse(b, null, rectangle.Location, rectangle.Width, rectangle.Height);
		}

		public void FillEllipse(Brush b, double x, double y, double width, double height)
		{
			this.g.DrawEllipse(b, null, new Point(x, y), width, height);
		}

		#endregion // Ellipse

		#region Polygon
		public void DrawPolygon(SolidColor color, double width, LineStyles style, params Graphics.Point[] points)
		{
			this.DrawLines(points, 0, points.Length, color, width, style);
		}

		public void FillPolygon(IColor color, params Graphics.Point[] points)
		{
			if (!color.IsTransparent)
			{
				var geo = new PathGeometry();

				for (int i = 1, k = 1; i < points.Length; i++, k++)
				{
					geo.AddGeometry(new LineGeometry(points[k - 1], points[k]));
				}

				g.DrawGeometry(new SolidColorBrush(color.ToSolidColor()), null, geo);
			}
		}
		#endregion // Polygon

		#region Utility
		public bool IsAntialias { get { return true; } set { } }

		public void Reset()
		{
			this.transformStack.Clear();
		}

		internal void SetPlatformGraphics(WPFDrawingContext dc)
		{
			this.g = dc;
		}
		#endregion // Utility

		internal System.Windows.Media.DashStyle ToWPFDashStyle(LineStyles style)
		{
			switch (style)
			{
				default:
				case LineStyles.Solid: return System.Windows.Media.DashStyles.Solid;
				case LineStyles.Dot: return System.Windows.Media.DashStyles.Dot;
				case LineStyles.Dash: return System.Windows.Media.DashStyles.Dash;
				case LineStyles.DashDot: return System.Windows.Media.DashStyles.DashDot;
				case LineStyles.DashDotDot: return System.Windows.Media.DashStyles.DashDotDot;
			}
		}

		#region Path
		public void FillPath(IColor color, Geometry graphicsPath)
		{
			var b = this.resourceManager.GetBrush(color.ToSolidColor());
			if (b != null) this.g.DrawGeometry(b, null, graphicsPath);
		}

		public void DrawPath(SolidColor color, Geometry graphicsPath)
		{
			var p = this.resourceManager.GetPen(color);
			if (p != null) this.g.DrawGeometry(null, p, graphicsPath);
		}
		#endregion // Path



		public void FillEllipse(IColor fillColor, Rectangle rect)
		{
			var b = this.resourceManager.GetBrush(fillColor.ToSolidColor());

			if (b != null)
			{
				this.g.DrawEllipse(b, null, rect.Origin, rect.Width, rect.Height);
			}
		}

	}
	#endregion // Graphics

	#region Renderer
	internal class WPFRenderer : WPFGraphics, IRenderer
	{
		protected System.Windows.Media.Typeface headerTextTypeface;

		internal WPFRenderer()
		{
			this.headerTextTypeface = unvell.ReoGrid.Rendering.PlatformUtility.GetFontDefaultTypeface(System.Windows.SystemFonts.SmallCaptionFontFamily);
		}

		public Graphics.Size MeasureCellText(Cell cell, DrawMode drawMode, double scale)
		{
			if (cell.InnerStyle.RotationAngle != 0)
			{
				System.Windows.Media.Matrix m = System.Windows.Media.Matrix.Identity;

				double hw = cell.formattedText.Width * 0.5, hh = cell.formattedText.Height * 0.5;
				WPFPoint p1 = new WPFPoint(-hw, -hh), p2 = new WPFPoint(hw, hh);
				m.Rotate(cell.InnerStyle.RotationAngle);
				p1 *= m; p2 *= m;
				return new Graphics.Size(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));
			}
			else
			{
				return new Graphics.Size(cell.formattedText.Width, cell.formattedText.Height);
			}
		}

		public void DrawCellText(Cell cell, SolidColor textColor, DrawMode drawMode, double scale)
		{
			var sheet = cell.Worksheet;

			if (sheet == null) return;

			//if (cell.formattedText == null)
			//{
			//	sheet.UpdateCellFont(cell);
			//}

			if (cell.InnerStyle.RotationAngle != 0)
			{
				System.Windows.Media.Matrix m = System.Windows.Media.Matrix.Identity;
				m.Rotate(cell.InnerStyle.RotationAngle);
				m.Translate(cell.Bounds.OriginX * sheet.ScaleFactor, cell.Bounds.OriginY * sheet.ScaleFactor);

				this.PushTransform(m);
				this.PlatformGraphics.DrawText(cell.formattedText, new WPFPoint(-cell.formattedText.Width * 0.5, -cell.formattedText.Height * 0.5));
				this.PopTransform();
			}
			else
			{
				this.PlatformGraphics.DrawText(cell.formattedText, cell.TextBounds.Location);
			}
		}

		private static Color DecideTextColor(Cell cell) {
			var sheet = cell.Worksheet;
			var controlStyle = sheet.controlAdapter.ControlStyle;
			SolidColor textColor;

			if (!cell.RenderColor.IsTransparent)
			{
				textColor = cell.RenderColor;
			}
			else if (cell.InnerStyle.HasStyle(PlainStyleFlag.TextColor))
			{
				// cell text color, specified by SetRangeStyle
				textColor = cell.InnerStyle.TextColor;
			}
			else if (!controlStyle.TryGetColor(ControlAppearanceColors.GridText, out textColor))
			{
				// default cell text color
				textColor = SolidColor.Black;
			}

			return textColor;
		}

		public void UpdateCellRenderFont(Cell cell, Core.UpdateFontReason reason)
		{
			var sheet = cell.Worksheet;
			if (sheet == null || sheet.controlAdapter == null) return;

			double dpi = PlatformUtility.GetDPI();
			double fontSize = cell.InnerStyle.FontSize * sheet.renderScaleFactor * dpi / 72.0;

			if (cell.formattedText == null || cell.formattedText.Text != cell.InnerDisplay)
			{
				SolidColor textColor = DecideTextColor(cell);

				cell.formattedText = new System.Windows.Media.FormattedText(cell.InnerDisplay,
					System.Globalization.CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight,
					base.resourceManager.GetTypeface(cell.InnerStyle.FontName),
					fontSize,
					base.resourceManager.GetBrush(textColor));
			}
			else if (reason == Core.UpdateFontReason.FontChanged || reason == Core.UpdateFontReason.ScaleChanged)
			{
				cell.formattedText.SetFontFamily(cell.InnerStyle.FontName);
				cell.formattedText.SetFontSize(fontSize);
			}
			else if (reason == Core.UpdateFontReason.TextColorChanged)
			{
				SolidColor textColor = DecideTextColor(cell);
				cell.formattedText.SetForegroundBrush(resourceManager.GetBrush(textColor));
			}

			var ft = cell.formattedText;

			if (reason == Core.UpdateFontReason.FontChanged || reason == Core.UpdateFontReason.ScaleChanged)
			{
				ft.SetFontWeight(
					cell.InnerStyle.Bold ? System.Windows.FontWeights.Bold : System.Windows.FontWeights.Normal);

				ft.SetFontStyle(PlatformUtility.ToWPFFontStyle(cell.InnerStyle.fontStyles));

				ft.SetTextDecorations(PlatformUtility.ToWPFFontDecorations(cell.InnerStyle.fontStyles));
			}
		}

		public void DrawRunningFocusRect(double x, double y, double w, double h, SolidColor color, int runningOffset)
		{

		}

		private Pen capLinePen = null;

		public void BeginCappedLine(LineCapStyles startCap, Graphics.Size startSize, LineCapStyles endCap, Graphics.Size endSize,
			SolidColor color, double width)
		{
			capLinePen = new Pen(new SolidColorBrush(color), width);
			capLinePen.StartLineCap = PlatformUtility.ToWPFLineCap(startCap);
			capLinePen.EndLineCap = PlatformUtility.ToWPFLineCap(endCap);
		}

		private LineCap lineCap;

		public void DrawCappedLine(double x1, double y1, double x2, double y2)
		{
			if (this.capLinePen != null)
			{
				base.DrawLine(this.capLinePen, x1, y1, x2, y2);
			}
		}

		public void EndCappedLine()
		{
			this.capLinePen = null;
		}

		private Pen cachePen = null;

		public void BeginDrawLine(double width, SolidColor color)
		{
			cachePen = new Pen(new SolidColorBrush(color), width);
		}

		public void DrawLine(double x1, double y1, double x2, double y2)
		{
			base.DrawLine(this.cachePen, new WPFPoint(x1, y1), new WPFPoint(x2, y2));
		}

		public void EndDrawLine()
		{
		}

		public void DrawLeadHeadArrow(Rectangle bounds, SolidColor startColor, SolidColor endColor)
		{
		}

		public Pen GetPen(SolidColor color)
		{
			return this.resourceManager.GetPen(color);
		}

		public void ReleasePen(Pen pen) { }

		public Brush GetBrush(SolidColor color)
		{
			return this.resourceManager.GetBrush(color);
		}

		public RGFont GetFont(string name, double size, Drawing.Text.FontStyles style)
		{
			return this.resourceManager.GetTypeface(name, FontWeights.Normal, System.Windows.FontStyles.Normal, FontStretches.Normal);
		}

		private double headerTextScale = 9d;

		public void BeginDrawHeaderText(double scale)
		{
			this.headerTextScale = 9d * scale;
		}

		public void DrawHeaderText(string text, Brush brush, Rectangle rect)
		{
			var ft = new System.Windows.Media.FormattedText(text,
				System.Globalization.CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight,
				this.headerTextTypeface, headerTextScale / 72f * 96f, brush);

			base.PlatformGraphics.DrawText(ft,
				new Point(rect.X + (rect.Width - ft.Width) / 2, rect.Y + (rect.Height - ft.Height) / 2));
		}

		public ResourcePoolManager GetResourcePoolManager
		{
			get { return this.resourceManager; }
		}
	}

	#endregion // Renderer

}

#endif // WPF