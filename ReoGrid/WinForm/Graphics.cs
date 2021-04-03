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

#if WINFORM

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

using unvell.Common;
using unvell.ReoGrid.Graphics;

using WFFontStyle = System.Drawing.FontStyle;
using PlatformGraphics = System.Drawing.Graphics;

using WFPointF = System.Drawing.PointF;
using WFRectF = System.Drawing.RectangleF;

using Point = unvell.ReoGrid.Graphics.Point;
using Rectangle = unvell.ReoGrid.Graphics.Rectangle;

using unvell.ReoGrid.Core;
using unvell.ReoGrid.Utility;
using unvell.ReoGrid.Drawing.Text;
using unvell.ReoGrid.Rendering;
using DrawMode = unvell.ReoGrid.Rendering.DrawMode;

namespace unvell.ReoGrid.WinForm
{
	#region GDIGraphics
	internal class GDIGraphics : IGraphics
	{
		protected ResourcePoolManager resourceManager = new ResourcePoolManager();

		#region Platform Graphics
		private System.Drawing.Graphics g = null;

		public PlatformGraphics PlatformGraphics { get { return this.g; } set { this.g = value; } }
		#endregion // Platform Graphics

		protected StringFormat sf;

		public GDIGraphics()
		{
			this.sf = new StringFormat(StringFormat.GenericTypographic)
			{
				FormatFlags = StringFormatFlags.MeasureTrailingSpaces
			};
		}

		public GDIGraphics(PlatformGraphics g)
			: this()
		{
			this.g = g;
		}

		#region Line
		public void DrawLine(float x1, float y1, float x2, float y2, SolidColor color)
		{
			if (color.A > 0) this.g.DrawLine(this.resourceManager.GetPen(color), x1, y1, x2, y2);
		}
		public void DrawLine(Point startPoint, Point endPoint, SolidColor color)
		{
			if (color.A > 0) this.g.DrawLine(this.resourceManager.GetPen(color), startPoint, endPoint);
		}
		public void DrawLine(float x1, float y1, float x2, float y2, SolidColor color, float width, LineStyles style)
		{
			if (color.A > 0)
			{
				var p = this.resourceManager.GetPen(color, width, ToGDILineStyle(style));

				if (p != null)
				{
					g.DrawLine(p, x1, y1, x2, y2);
				}
			}
		}
		public void DrawLine(Point startPoint, Point endPoint, SolidColor color, float width, LineStyles style)
		{
			if (color.A > 0)
			{
				var p = this.resourceManager.GetPen(color, width, ToGDILineStyle(style));
				if (p != null)
				{
					this.g.DrawLine(p, startPoint, endPoint);
				}
			}
		}
		public void DrawLine(Pen p, float x1, float y1, float x2, float y2) { this.g.DrawLine(p, x1, y1, x2, y2); }
		public void DrawLine(Pen p, Point startPoint, Point endPoint) { this.g.DrawLine(p, startPoint, endPoint); }
		public void DrawLines(Point[] points, int start, int length, SolidColor color, float width, LineStyles style)
		{
			if (color.A < 0) return;
			var p = this.resourceManager.GetPen(color, width, ToGDILineStyle(style));
			if (p == null) return;

			WFPointF[] pt = new WFPointF[length];
			for (int i = 0, k = start; i < pt.Length; i++, k++)
			{
				pt[i] = points[k];
			}

			this.g.DrawLines(p, pt);
		}

		//public void DrawLine(SolidColor color, Point startPoint, Point endPoint, float width,
		//	LineStyles style, LineCapStyles startCap, LineCapStyles endCap)
		//{
		//	using (Pen p = new Pen(color, width))
		//	{
		//		if (startCap == LineCapStyles.Arrow)
		//		{
		//			p.StartCap = System.Drawing.Drawing2D.LineCap.Custom;
		//			p.CustomStartCap = new AdjustableArrowCap(width + 3, width + 3);
		//		}

		//		if (endCap == LineCapStyles.Arrow)
		//		{
		//			p.EndCap = System.Drawing.Drawing2D.LineCap.Custom;
		//			p.CustomEndCap = new AdjustableArrowCap(width + 3, width + 3);
		//		}

		//		this.g.DrawLine(p, startPoint, endPoint);
		//	}
		//}
		#endregion // Line

		#region Rectangle
		public void DrawRectangle(Rectangle rect, SolidColor color)
		{
			if (color.A > 0) this.g.DrawRectangle(this.resourceManager.GetPen(color), rect.X, rect.Y, rect.Width, rect.Height);
		}
		public void DrawRectangle(float x, float y, float width, float height, SolidColor color)
		{
			if (color.A > 0) this.g.DrawRectangle(this.resourceManager.GetPen(color), x, y, width, height);
		}
		public void DrawRectangle(Pen p, Rectangle rect) { this.g.DrawRectangle(p, rect.X, rect.Y, rect.Width, rect.Height); }
		public void DrawRectangle(Pen p, float x, float y, float width, float height) { this.g.DrawRectangle(p, x, y, width, height); }
		public void DrawRectangle(Rectangle rect, SolidColor color, float width, LineStyles lineStyle)
		{
			var p = this.resourceManager.GetPen(color, width, ToGDILineStyle(lineStyle));
			this.g.DrawRectangle(p, rect.X, rect.Y, rect.Width, rect.Height);
		}

		public void FillRectangle(HatchStyles style, SolidColor hatchColor, SolidColor bgColor, Rectangle rect)
		{
			this.g.FillRectangle(this.resourceManager.GetHatchBrush((HatchStyle)style, hatchColor, bgColor), rect);
		}
		public void FillRectangle(HatchStyles style, SolidColor hatchColor, SolidColor bgColor, float x, float y, float width, float height)
		{
			this.g.FillRectangle(this.resourceManager.GetHatchBrush((HatchStyle)style, hatchColor, bgColor), x, y, width, height);
		}
		public void FillRectangle(Brush b, float x, float y, float w, float h) { this.g.FillRectangle(b, x, y, w, h); }
		public void FillRectangle(HatchStyle style, SolidColor hatchColor, SolidColor bgColor, Rectangle rect)
		{
			this.FillRectangle(style, hatchColor, bgColor, rect.X, rect.Y, rect.Width, rect.Height);
		}
		public void FillRectangle(HatchStyle style, SolidColor hatchColor, SolidColor bgColor, float x, float y, float width, float height)
		{
			System.Drawing.Drawing2D.HatchBrush hb = this.resourceManager.GetHatchBrush(style, hatchColor, bgColor);
			this.g.FillRectangle(hb, x, y, width, height);
		}
		public void FillRectangle(Rectangle rect, IColor color)
		{
			if (color is SolidColor)
			{
				Brush b = this.resourceManager.GetBrush((SolidColor)color);
				if (b != null)
				{
					this.g.FillRectangle(b, rect);
				}
			}
		}
		public void FillRectangle(float x, float y, float w, float h, IColor color)
		{
			if (color is SolidColor)
			{
				Brush b = this.resourceManager.GetBrush((SolidColor)color);
				if (b != null) this.g.FillRectangle(b, x, y, w, h);
			}
		}

		public void DrawAndFillRectangle(Rectangle rect, SolidColor lineColor, IColor fillColor)
		{
			FillRectangle(rect, fillColor);
			DrawRectangle(rect, lineColor);
		}

		public void DrawAndFillRectangle(Rectangle rect, SolidColor lineColor, IColor fillColor, float weight, LineStyles lineStyle)
		{
			FillRectangle(rect, fillColor);
			DrawRectangle(rect, lineColor, weight, lineStyle);
		}

		public void FillRectangleLinear(SolidColor startColor, SolidColor endColor, float angle, Rectangle rect)
		{
			using (System.Drawing.Drawing2D.LinearGradientBrush lgb =
				new System.Drawing.Drawing2D.LinearGradientBrush(rect, startColor, endColor, angle))
			{
				this.g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
				this.g.FillRectangle(lgb, rect);
				this.g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Default;
			}
		}
		#endregion // Rectangle

		#region Image
		public void DrawImage(Image image, float x, float y, float width, float height) { this.g.DrawImage(image, x, y, width, height); }
		public void DrawImage(Image image, Rectangle bounds) { this.g.DrawImage(image, bounds); }
		#endregion // Image

		#region Ellipse
		public void DrawEllipse(SolidColor color, Rectangle rectangle)
		{
			if (!color.IsTransparent)
			{
				g.DrawEllipse(this.resourceManager.GetPen(color), rectangle);
			}
		}
		public void DrawEllipse(SolidColor color, float x, float y, float width, float height)
		{
			if (!color.IsTransparent)
			{
				g.DrawEllipse(this.resourceManager.GetPen(color), x, y, width, height);
			}
		}
		public void DrawEllipse(Pen pen, Rectangle rect) { this.g.DrawEllipse(pen, rect); }
		public void FillEllipse(IColor fillColor, Rectangle rectangle)
		{
			var b = this.resourceManager.GetBrush(fillColor.ToSolidColor());

			if (b != null)
			{
				this.g.FillEllipse(b, rectangle);
			}
		}
		public void FillEllipse(Brush b, Rectangle rect) { this.g.FillEllipse(b, rect); }
		public void FillEllipse(Brush b, float x, float y, float widht, float height) { this.g.FillEllipse(b, x, y, widht, height); }
		#endregion // Ellipse

		#region Polygon

		public void DrawPolygon(SolidColor color, float lineWidth, LineStyles lineStyle, params Point[] points)
		{
			if (!color.IsTransparent)
			{
				var p = resourceManager.GetPen(color, lineWidth, ToGDILineStyle(lineStyle));

				var wfpoints = new WFPointF[points.Length];
				for (int i = 0; i < points.Length; i++)
				{
					wfpoints[i] = points[i];
				}

				this.g.DrawPolygon(p, wfpoints);
			}
		}

		public void FillPolygon(IColor color, params Point[] points)
		{
			if (!color.IsTransparent)
			{
				var b = resourceManager.GetBrush(color.ToSolidColor());

				var wfpoints = new WFPointF[points.Length];
				for (int i = 0; i < points.Length; i++)
				{
					wfpoints[i] = points[i];
				}

				this.g.FillPolygon(b, wfpoints);
			}
		}

		#endregion // Polygon

		#region Path
		public void FillPath(IColor color, GraphicsPath graphicsPath)
		{
			if (!color.IsTransparent)
			{
				var b = this.resourceManager.GetBrush(color.ToSolidColor());
				if (b != null) g.FillPath(b, graphicsPath);
			}
		}

		public void DrawPath(SolidColor color, GraphicsPath graphicsPath)
		{
			if (!color.IsTransparent)
			{
				var p = this.resourceManager.GetPen(color.ToSolidColor());
				if (p != null) g.DrawPath(p, graphicsPath);
			}
		}
		#endregion // Path

		#region Text
		//public void DrawText(string text, string fontName, float size, SolidColor color, Rectangle rect)
		//{
		//	if (color.IsTransparent)
		//	{
		//		return;
		//	}

		//	var font = this.resourceManager.GetFont(fontName, size, WFFontStyle.Regular);

		//	if (font != null)
		//	{
		//		lock (this.sf)
		//		{
		//			sf.Alignment = StringAlignment.Near;
		//			sf.LineAlignment = StringAlignment.Near;

		//			g.DrawString(text, font, this.resourceManager.GetBrush(color), rect, sf);
		//		}
		//	}
		//}

		public void DrawText(string text, string fontName, float size, SolidColor color, Rectangle rect,
			ReoGridHorAlign halign, ReoGridVerAlign valign)
		{
			if (color.IsTransparent)
			{
				return;
			}

			var font = this.resourceManager.GetFont(fontName, size, WFFontStyle.Regular);

			if (font != null)
			{
				lock (this.sf)
				{
					switch (halign)
					{
						default: sf.Alignment = StringAlignment.Near; break;
						case ReoGridHorAlign.Center: sf.Alignment = StringAlignment.Center; break;
						case ReoGridHorAlign.Right: sf.Alignment = StringAlignment.Far; break;
					}

					switch (valign)
					{
						default: sf.LineAlignment = StringAlignment.Near; break;
						case ReoGridVerAlign.Middle: sf.LineAlignment = StringAlignment.Center; break;
						case ReoGridVerAlign.Bottom: sf.LineAlignment = StringAlignment.Far; break;
					}

					g.DrawString(text, font, this.resourceManager.GetBrush(color), rect, sf);
				}
			}
		}

		//public Graphics.Size MeasureText(string text, string fontName, float size, Graphics.Size areaSize)
		//{
		//	var font = this.resourceManager.GetFont(fontName, size, WFFontStyle.Regular);

		//	if (font == null)
		//	{
		//		return Graphics.Size.Zero;
		//	}

		//	lock (sf)
		//	{
		//		return g.MeasureString(text, font, areaSize, sf);
		//	}
		//}

		#endregion // Text

		#region Clip
		private Stack<WFRectF> clipStack = new Stack<WFRectF>();

		public void PushClip(Rectangle clip)
		{
			clipStack.Push(this.g.ClipBounds);
			this.g.SetClip(clip, CombineMode.Intersect);
		}

		public void PopClip()
		{
			WFRectF cb = clipStack.Pop();
			this.g.SetClip(cb);
		}
		#endregion // Clip

		#region Transform
		private Stack<Matrix> transformStack = new Stack<Matrix>();

		/// <summary>
		/// Push a new transform matrix into stack. (Backup current transform matrix)
		/// </summary>
		public void PushTransform()
		{
			this.PushTransform(new Matrix());
		}

		/// <summary>
		/// Push specified transform matrix into stack. (Backup current transform matrix)
		/// </summary>
		public void PushTransform(Matrix m)
		{
			var t = this.g.Transform;

			this.transformStack.Push(t);

			m = m.Clone();
			m.Multiply(t);

			this.g.Transform = m;
		}

		/// <summary>
		/// Pop the last transform matrix from stack. (Restore transform matrix to previous status)
		/// </summary>
		public Matrix PopTransform()
		{
			Matrix m = this.transformStack.Pop();
			this.g.Transform = m;
			return m;
		}

		/// <summary>
		/// Scale current transform matrix.
		/// </summary>
		/// <param name="sx">X-factor to be scaled.</param>
		/// <param name="sy">Y-factor to be scaled.</param>
		public void ScaleTransform(float sx, float sy)
		{
			if (sx != 0 && sy != 0)
			{
				this.g.ScaleTransform(sx, sy);
			}
		}

		/// <summary>
		/// Translate current transform matrix.
		/// </summary>
		/// <param name="x">X-offset value to be translated.</param>
		/// <param name="y">Y-offset value to be translated.</param>
		public void TranslateTransform(float x, float y)
		{
			this.g.TranslateTransform(x, y);
		}

		/// <summary>
		/// Rotate current transform matrix by specified angle.
		/// </summary>
		/// <param name="angle"></param>
		public void RotateTransform(float angle)
		{
			this.g.RotateTransform(angle);
		}

		/// <summary>
		/// Reset current transform matrix. (Load identity)
		/// </summary>
		public void ResetTransform()
		{
			this.g.ResetTransform();
		}
		#endregion // Transform

		#region Utility
		public bool IsAntialias
		{
			get { return this.g.SmoothingMode == SmoothingMode.AntiAlias; }
			set { this.g.SmoothingMode = value ? SmoothingMode.AntiAlias : SmoothingMode.Default; }
		}

		public void Reset()
		{
			this.transformStack.Clear();
			this.clipStack.Clear();
		}

		internal static System.Drawing.Drawing2D.DashStyle ToGDILineStyle(LineStyles dashStyle)
		{
			switch (dashStyle)
			{
				default:
				case LineStyles.Solid: return System.Drawing.Drawing2D.DashStyle.Solid;
				case LineStyles.Dash: return System.Drawing.Drawing2D.DashStyle.Dash;
				case LineStyles.Dot: return System.Drawing.Drawing2D.DashStyle.Dot;
				case LineStyles.DashDot: return System.Drawing.Drawing2D.DashStyle.DashDot;
				case LineStyles.DashDotDot: return System.Drawing.Drawing2D.DashStyle.DashDotDot;
			}
		}
		#endregion // Utility

	}
	#endregion // GDIGraphics

	#region GDIRenderer
	internal class GDIRenderer : GDIGraphics, IRenderer
	{
		private System.Drawing.Graphics cachedGraphics;

		private StringFormat headerSf;

		internal static readonly System.Drawing.Font HeaderFont = new System.Drawing.Font(
			System.Drawing.SystemFonts.DefaultFont.Name, 8f, System.Drawing.FontStyle.Regular);

		internal GDIRenderer(System.Drawing.Graphics cg)
		{
			this.cachedGraphics = cg;

			this.headerSf = new System.Drawing.StringFormat
			{
				LineAlignment = System.Drawing.StringAlignment.Center,
				Alignment = System.Drawing.StringAlignment.Center,
				Trimming = System.Drawing.StringTrimming.None,
				FormatFlags = System.Drawing.StringFormatFlags.LineLimit,
			};
		}

		public void DrawRunningFocusRect(float x, float y, float w, float h, SolidColor color, int runnerOffset)
		{
			using (var p = new Pen(color))
			{
				p.DashStyle = DashStyle.Custom;
				p.DashPattern = new float[] { 3f, 4f, 3f };
				p.DashOffset = runnerOffset;

				base.PlatformGraphics.DrawRectangle(p, x, y, w, h);
			}
		}

		#region Capped Line
		private AdjustableArrowCap cappedStartArrowCap;
		private AdjustableArrowCap cappedEndArrowCap;
		private Pen cappedLinePen;
		private Graphics.LineCap lineCap;

		private void CreateArrowCap(ref AdjustableArrowCap arrowCap, Graphics.Size size)
		{
			if (arrowCap == null)
			{
				arrowCap = new System.Drawing.Drawing2D.AdjustableArrowCap(size.Width, size.Height);
			}
			else
			{
				arrowCap.Width = size.Width;
				arrowCap.Height = size.Height;
			}
		}

		public void BeginCappedLine(LineCapStyles startStyle, Graphics.Size startSize,
			LineCapStyles endStyle, Graphics.Size endSize, SolidColor color, float width)
		{
			lineCap.StartStyle = startStyle;
			lineCap.StartSize = startSize;
			lineCap.EndStyle = endStyle;
			lineCap.EndSize = endSize;
			lineCap.StartColor = color;
			lineCap.EndColor = color;

			if ((startStyle == LineCapStyles.None && endStyle == LineCapStyles.None))
			{
				return;
			}

			PlatformGraphics.SmoothingMode = SmoothingMode.AntiAlias;

			if (cappedLinePen == null)
			{
				this.cappedLinePen = new Pen(color, width);
			}
			else
			{
				this.cappedLinePen.Color = color;
				this.cappedLinePen.Width = width;
			}

			if (startStyle == LineCapStyles.Arrow)
			{
				CreateArrowCap(ref this.cappedStartArrowCap, this.lineCap.StartSize);
				cappedLinePen.CustomStartCap = this.cappedStartArrowCap;
			}
			else
			{
				this.cappedLinePen.StartCap = System.Drawing.Drawing2D.LineCap.NoAnchor;
			}

			if (endStyle == LineCapStyles.Arrow)
			{
				CreateArrowCap(ref this.cappedEndArrowCap, this.lineCap.EndSize);
				cappedLinePen.CustomEndCap = this.cappedEndArrowCap;
			}
			else
			{
				this.cappedLinePen.EndCap = System.Drawing.Drawing2D.LineCap.NoAnchor;
			}
		}

		private void DrawEllipseForCappedLine(float x, float y, Graphics.Size size)
		{
			float ellipseSize = size.Width + size.Height / 2;
			float halfOfEllipse = ellipseSize / 2f;

			using (var b = new SolidBrush(this.cappedLinePen.Color))
			{
				PlatformGraphics.FillEllipse(b, x - halfOfEllipse, y - halfOfEllipse, ellipseSize, ellipseSize);
			}
		}

		public void DrawCappedLine(float x1, float y1, float x2, float y2)
		{
			PlatformGraphics.DrawLine(this.cappedLinePen, x1, y1, x2, y2);

			if (this.lineCap.StartStyle == LineCapStyles.Ellipse)
			{
				this.DrawEllipseForCappedLine(x1, y1, this.lineCap.StartSize);
			}

			if (this.lineCap.EndStyle == LineCapStyles.Ellipse)
			{
				this.DrawEllipseForCappedLine(x2, y2, this.lineCap.EndSize);
			}
		}

		public void EndCappedLine()
		{
			PlatformGraphics.SmoothingMode = SmoothingMode.Default;
		}
		#endregion // Capped Line

		#region Line
		private Pen linePen;

		public void BeginDrawLine(float width, SolidColor color)
		{
			if (linePen == null)
			{
				linePen = new Pen(color, width);
			}
			else
			{
				linePen.Color = color;
				linePen.Width = width;
			}
		}

		public void DrawLine(float x1, float y1, float x2, float y2)
		{
			PlatformGraphics.DrawLine(this.linePen, x1, y1, x2, y2);
		}

		public void EndDrawLine()
		{
		}

		#endregion Line

		#region Cell
		public void DrawCellText(Cell cell, SolidColor textColor, DrawMode drawMode, float scale)
		{
			var sheet = cell.Worksheet;

			if (sheet == null) return;

			var b = this.GetBrush(textColor);
			if (b == null) return;

			Rectangle textBounds;
			System.Drawing.Font scaledFont;

			#region Determine text bounds
			switch (drawMode)
			{
				default:
				case DrawMode.View:
					textBounds = cell.TextBounds;
					scaledFont = cell.RenderFont;
					break;

				case DrawMode.Preview:
				case DrawMode.Print:
					textBounds = cell.PrintTextBounds;
					scaledFont = this.resourceManager.GetFont(cell.RenderFont.Name,
						cell.InnerStyle.FontSize * scale, cell.RenderFont.Style);
					break;
			}
			#endregion // Determine text bounds

			lock (this.sf)
			{
				#region Set sf wrap
				if (cell.InnerStyle.TextWrapMode == TextWrapMode.NoWrap)
				{
					sf.FormatFlags |= System.Drawing.StringFormatFlags.NoWrap;
				}
				else
				{
					sf.FormatFlags &= ~System.Drawing.StringFormatFlags.NoWrap;
				}
				#endregion // Set sf wrap

				var g = base.PlatformGraphics;

				#region Rotate text
				if (cell.InnerStyle.RotationAngle != 0)
				{
#if DEBUG1
					g.DrawRectangle(Pens.Red, (System.Drawing.Rectangle)textBounds);
#endif // DEBUG

					this.PushTransform();

					this.TranslateTransform(textBounds.OriginX, textBounds.OriginY);
					this.RotateTransform(-cell.InnerStyle.RotationAngle);

					sf.LineAlignment = StringAlignment.Center;
					sf.Alignment = StringAlignment.Center;

					g.DrawString(cell.DisplayText, scaledFont, b, 0, 0, this.sf);

					this.PopTransform();
				}
				else
				#endregion // Rotate text
				{
					#region Align StringFormat
					switch (cell.RenderHorAlign)
					{
						default:
						case ReoGridRenderHorAlign.Left:
							sf.Alignment = System.Drawing.StringAlignment.Near;
							break;

						case ReoGridRenderHorAlign.Center:
							sf.Alignment = System.Drawing.StringAlignment.Center;
							break;

						case ReoGridRenderHorAlign.Right:
							sf.Alignment = System.Drawing.StringAlignment.Far;
							break;
					}

					switch (cell.InnerStyle.VAlign)
					{
						case ReoGridVerAlign.Top:
							sf.LineAlignment = System.Drawing.StringAlignment.Near;
							break;

						default:
						case ReoGridVerAlign.Middle:
							sf.LineAlignment = System.Drawing.StringAlignment.Center;
							break;

						case ReoGridVerAlign.Bottom:
							sf.LineAlignment = System.Drawing.StringAlignment.Far;
							break;
					}
					#endregion // Align StringFormat

					g.DrawString(cell.DisplayText, scaledFont, b, textBounds, this.sf);
				}
			}
		}

		public void UpdateCellRenderFont(Cell cell, Core.UpdateFontReason reason)
		{
			var sheet = cell.Worksheet;

			if (sheet == null) return;

			var style = cell.InnerStyle;

			var fontStyle = StyleUtility.CreateFontStyle(style);

			// unknown bugs happened here (several times)
			// cell.Style is null (cell.Style.FontSize is zero)
			if (style.FontSize <= 0) style.FontSize = 6f;

			float fontSize = (float)Math.Round(style.FontSize * sheet.renderScaleFactor, 1);

			var renderFont = cell.RenderFont;

			if (renderFont == null
				|| renderFont.Name != style.FontName
				|| renderFont.Size != fontSize
				|| renderFont.Style != (System.Drawing.FontStyle)fontStyle)
			{
				cell.RenderFont = this.resourceManager.GetFont(style.FontName, fontSize,
					(System.Drawing.FontStyle)fontStyle);
			}
		}

		public Graphics.Size MeasureCellText(Cell cell, DrawMode drawMode, float scale)
		{
			var sheet = cell.Worksheet;

			if (sheet == null) return Graphics.Size.Zero;

			WorksheetRangeStyle style = cell.InnerStyle;

			int fieldWidth = 0;

			double s = 0, c = 0;

			//if (sf == null) sf = new System.Drawing.StringFormat(System.Drawing.StringFormat.GenericTypographic);

			if (cell.Style.RotationAngle != 0)
			{
				double d = (style.RotationAngle * Math.PI / 180.0d);
				s = Math.Sin(d);
				c = Math.Cos(d);
			}

			lock (sf)
			{
				// merged cell need word break automatically
				if (style.TextWrapMode == TextWrapMode.NoWrap)
				{
					// no word break
					fieldWidth = 9999999; // TODO: avoid magic number
					sf.FormatFlags |= System.Drawing.StringFormatFlags.NoWrap;
				}
				else
				{
					// get cell available width
					float cellWidth = 0;

					if (cell.Style.RotationAngle != 0)
					{
						cellWidth = (float)(Math.Abs(cell.Bounds.Width * c) + Math.Abs(cell.Bounds.Height * s));
					}
					else
					{
						cellWidth = cell.Bounds.Width;

						if (cell.InnerStyle != null)
						{
							if ((cell.InnerStyle.Flag & PlainStyleFlag.Indent) == PlainStyleFlag.Indent)
							{
								cellWidth -= (int)(cell.InnerStyle.Indent * sheet.IndentSize);
							}
						}

						// border width
						cellWidth -= 2;
					}

					// word break
					fieldWidth = (int)Math.Round(cellWidth * scale);
					sf.FormatFlags &= ~System.Drawing.StringFormatFlags.NoWrap;
				}

				if (cell.FontDirty)
				{
					sheet.UpdateCellRenderFont(this, cell, drawMode, UpdateFontReason.FontChanged);
				}

				var g = this.cachedGraphics;

				System.Drawing.Font scaledFont;
				switch (drawMode)
				{
					default:
					case DrawMode.View:
						scaledFont = cell.RenderFont;
						break;

					case DrawMode.Preview:
					case DrawMode.Print:
						scaledFont = this.resourceManager.GetFont(cell.RenderFont.Name,
							style.FontSize * scale, cell.RenderFont.Style);
						g = this.PlatformGraphics;
						System.Diagnostics.Debug.Assert(g != null);
						break;
				}

				SizeF size = g.MeasureString(cell.DisplayText, scaledFont, fieldWidth, sf);
				size.Height++;

				if (style.RotationAngle != 0)
				{
					float w = (float)(Math.Abs(size.Width * c) + Math.Abs(size.Height * s));
					float h = (float)(Math.Abs(size.Width * s) + Math.Abs(size.Height * c));

					size = new SizeF(w + 1, h + 1);
				}

				return size;
			}
		}

		#endregion // Cell

		#region Header

		private Font scaledHeaderFont;


		public void BeginDrawHeaderText(float scale)
		{
			scaledHeaderFont = this.resourceManager.GetFont(HeaderFont.Name, HeaderFont.Size * scale, FontStyle.Regular);
		}

		public void DrawHeaderText(string text, System.Drawing.Brush brush, Rectangle rect)
		{
			base.PlatformGraphics.DrawString(text, scaledHeaderFont, brush, rect, this.headerSf);
		}

		#endregion // Header

		public void DrawLeadHeadArrow(Graphics.Rectangle bounds, SolidColor startColor, SolidColor endColor)
		{
			using (System.Drawing.Drawing2D.GraphicsPath leadHeadPath = new System.Drawing.Drawing2D.GraphicsPath())
			{
				leadHeadPath.AddLines(new System.Drawing.PointF[] { new Point(bounds.Right - 4, bounds.Y + 4),
						new Point(bounds.Right - 4, bounds.Bottom - 4),
						new Point(bounds.Right - bounds.Height + 4, bounds.Bottom - 4)});

				leadHeadPath.CloseAllFigures();

				using (System.Drawing.Drawing2D.LinearGradientBrush lgb
					= new System.Drawing.Drawing2D.LinearGradientBrush(bounds, startColor, endColor, 90f))
				{
					base.PlatformGraphics.FillPath(lgb, leadHeadPath);
				}
			}
		}

		public Pen GetPen(SolidColor color)
		{
			return this.resourceManager.GetPen(color);
		}

		public void ReleasePen(Pen pen)
		{
		}

		public Brush GetBrush(SolidColor color)
		{
			return this.resourceManager.GetBrush(color);
		}

		public System.Drawing.Font GetFont(string name, float size, FontStyles style)
		{
			return this.resourceManager.GetFont(name, size, PlatformUtility.ToWFFontStyle(style));
		}

		public ResourcePoolManager ResourcePoolManager
		{
			get { return this.resourceManager; }
		}

		public void Dispose()
		{
			if (this.cappedLinePen != null)
			{
				this.cappedLinePen.Dispose();
				this.cappedLinePen = null;
			}

			if (this.cappedStartArrowCap != null)
			{
				this.cappedStartArrowCap.Dispose();
				this.cappedStartArrowCap = null;
			}

			if (this.cappedEndArrowCap != null)
			{
				this.cappedEndArrowCap.Dispose();
				this.cappedEndArrowCap = null;
			}

			if (this.linePen != null)
			{
				this.linePen.Dispose();
			}

			if (this.cachedGraphics != null)
			{
				this.cachedGraphics.Dispose();
				this.cachedGraphics = null;
			}

			if (this.sf != null)
			{
				this.sf.Dispose();
				this.sf = null;
			}

			if (this.headerSf != null)
			{
				this.headerSf.Dispose();
				this.headerSf = null;
			}
		}
	}
	#endregion // GDIRenderer
}

#endif // WINFORM
