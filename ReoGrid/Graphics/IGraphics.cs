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

#pragma warning disable 1591

#if WINFORM
using RGFloat = System.Single;

using RGPen = System.Drawing.Pen;
using RGBrush = System.Drawing.Brush;

using RGPath = System.Drawing.Drawing2D.GraphicsPath;
using RGImage = System.Drawing.Image;

using PlatformGraphics = System.Drawing.Graphics;
using RGTransform = System.Drawing.Drawing2D.Matrix;

#elif ANDROID
using RGFloat = System.Single;
using PlatformGraphics = Android.Graphics.Canvas;
using RGPen = Android.Graphics.Paint;
using RGBrush = Android.Graphics.Paint;
using RGPath = Android.Graphics.Path;
using RGImage = Android.Graphics.Picture;
using RGTransform = Android.Graphics.Matrix;

#elif WPF

using RGFloat = System.Double;

using RGPath = System.Windows.Media.Geometry;
using RGImage = System.Windows.Media.ImageSource;

using RGPen = System.Windows.Media.Pen;
using RGBrush = System.Windows.Media.Brush;

using PlatformGraphics = System.Windows.Media.DrawingContext;
using RGTransform = System.Windows.Media.Matrix;

#elif iOS
using RGFloat = System.Double;
using PlatformGraphics = CoreGraphics.CGContext;
using RGPen = CoreGraphics.CGContext;
using RGBrush = CoreGraphics.CGContext;
using RGPath = CoreGraphics.CGPath;
using RGImage = CoreGraphics.CGImage;
using RGTransform = CoreGraphics.CGAffineTransform;

#endif // WPF

using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Drawing.Text;

namespace unvell.ReoGrid.Graphics
{
	/// <summary>
	/// Represents abstract cross-platform drawing context.
	/// </summary>
	public interface IGraphics
	{
		PlatformGraphics PlatformGraphics { get; set; }

		void DrawLine(RGFloat x1, RGFloat y1, RGFloat x2, RGFloat y2, SolidColor color);
		void DrawLine(RGFloat x1, RGFloat y1, RGFloat x2, RGFloat y2, SolidColor color, RGFloat width, LineStyles style);
		void DrawLine(Point startPoint, Point endPoint, SolidColor color);
		void DrawLine(Point startPoint, Point endPoint, SolidColor color, RGFloat width, LineStyles style);
		//void DrawLine(SolidColor color, Point startPoint, Point endPoint, RGFloat width, LineStyles style, LineCapStyles startCap, LineCapStyles endCap);
		void DrawLine(RGPen p, RGFloat x1, RGFloat y1, RGFloat x2, RGFloat y2);
		void DrawLine(RGPen p, Point startPoint, Point endPoint);
		void DrawLines(Point[] points, int start, int length, SolidColor color, RGFloat width, LineStyles style);

		void DrawRectangle(Rectangle rect, SolidColor color);
		void DrawRectangle(Rectangle rect, SolidColor color, RGFloat width, LineStyles lineStyle);
		void DrawRectangle(RGFloat x, RGFloat y, RGFloat width, RGFloat height, SolidColor color);
		void DrawRectangle(RGPen p, Rectangle rect);
		void DrawRectangle(RGPen p, RGFloat x, RGFloat y, RGFloat width, RGFloat height);

		void FillRectangle(HatchStyles style, SolidColor hatchColor, SolidColor bgColor, Rectangle rect);
		void FillRectangle(HatchStyles style, SolidColor hatchColor, SolidColor bgColor, RGFloat x, RGFloat y, RGFloat width, RGFloat height);
		void FillRectangle(Rectangle rect, IColor color);
		void FillRectangle(RGFloat x, RGFloat y, RGFloat width, RGFloat height, IColor color);
		void FillRectangle(RGBrush b, RGFloat x, RGFloat y, RGFloat width, RGFloat height);
		void FillRectangleLinear(SolidColor startColor, SolidColor endColor, RGFloat angle, Rectangle rect);

		void DrawAndFillRectangle(Rectangle rect, SolidColor lineColor, IColor fillColor);
		void DrawAndFillRectangle(Rectangle rect, SolidColor lineColor, IColor fillColor, RGFloat weight, LineStyles lineStyle);

		void DrawEllipse(SolidColor color, Rectangle rectangle);
		void DrawEllipse(SolidColor color, RGFloat x, RGFloat y, RGFloat width, RGFloat height);
		void DrawEllipse(RGPen pen, Rectangle rectangle);
		void FillEllipse(IColor fillColor, Rectangle rectangle);
		void FillEllipse(RGBrush b, Rectangle rectangle);
		void FillEllipse(RGBrush b, RGFloat x, RGFloat y, RGFloat widht, RGFloat height);

		void DrawPolygon(SolidColor color, RGFloat lineWidth, LineStyles lineStyle, params Point[] points);
		void FillPolygon(IColor color, params Point[] points);

		void FillPath(IColor color, RGPath graphicsPath);
		void DrawPath(SolidColor color, RGPath graphicsPath);

		void DrawImage(RGImage image, RGFloat x, RGFloat y, RGFloat width, RGFloat height);
		void DrawImage(RGImage image, Rectangle rect);

		void DrawText(string text, string fontName, RGFloat size, SolidColor color, Rectangle rect, 
			ReoGridHorAlign halign = ReoGridHorAlign.Center, ReoGridVerAlign valign = ReoGridVerAlign.Middle);

		//Graphics.Size MeasureText(string text, string fontName, RGFloat size, Size areaSize);
		//void FillPolygon(RGPointF[] points, RGColor startColor, RGColor endColor, RGFloat angle, RGRectF rect);

		void ScaleTransform(RGFloat sx, RGFloat sy);
		void TranslateTransform(RGFloat x, RGFloat y);
		void RotateTransform(RGFloat angle);
		void ResetTransform();

		void PushClip(Rectangle clip);
		void PopClip();

		void PushTransform();
		void PushTransform(RGTransform t);
		RGTransform PopTransform();

		bool IsAntialias { get; set; }

		void Reset();
	}

	struct LineCap
	{
		public LineCapStyles StartStyle { get; set; }
		public LineCapStyles EndStyle { get; set; }

		public Graphics.Size StartSize { get; set; }
		public Graphics.Size EndSize { get; set; }

		public SolidColor StartColor { get; set; }
		public SolidColor EndColor { get; set; }
	}


}

#pragma warning restore 1591
