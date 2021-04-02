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
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

#if iOS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CoreGraphics;
using unvell.ReoGrid.Graphics;

using PlatformGraphics = CoreGraphics.CGContext;
using RGFloat = System.Double;
using RGPen = CoreGraphics.CGContext;
using RGBrush = CoreGraphics.CGContext;
using RGPath = CoreGraphics.CGPath;
using RGImage = CoreGraphics.CGImage;
using RGTransform = CoreGraphics.CGAffineTransform;

namespace unvell.ReoGrid.iOS
{
	class Graphics : IGraphics
	{
		public bool IsAntialias
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public PlatformGraphics PlatformGraphics
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public void DrawAndFillRectangle(SolidColor lineColor, IColor fillColor, Rectangle rect)
		{
			throw new NotImplementedException();
		}

		public void DrawAndFillRectangle(SolidColor lineColor, IColor fillColor, Rectangle rect, RGFloat weight, LineStyles lineStyle)
		{
			throw new NotImplementedException();
		}

		public void DrawEllipse(RGPen pen, Rectangle rectangle)
		{
			throw new NotImplementedException();
		}

		public void DrawEllipse(SolidColor color, Rectangle rectangle)
		{
			throw new NotImplementedException();
		}

		public void DrawEllipse(SolidColor color, RGFloat x, RGFloat y, RGFloat width, RGFloat height)
		{
			throw new NotImplementedException();
		}

		public void DrawImage(RGImage image, Rectangle rect)
		{
			throw new NotImplementedException();
		}

		public void DrawImage(RGImage image, RGFloat x, RGFloat y, RGFloat width, RGFloat height)
		{
			throw new NotImplementedException();
		}

		public void DrawLine(RGPen p, Point startPoint, Point endPoint)
		{
			this.DrawLine(p, startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
		}

		public void DrawLine(Point startPoint, Point endPoint, SolidColor color)
		{
			throw new NotImplementedException();
		}

		public void DrawLine(RGPen p, RGFloat x1, RGFloat y1, RGFloat x2, RGFloat y2)
		{
			
		}

		public void DrawLine(Point startPoint, Point endPoint, SolidColor color, RGFloat width, LineStyles style)
		{
			throw new NotImplementedException();
		}

		public void DrawLine(RGFloat x1, RGFloat y1, RGFloat x2, RGFloat y2, SolidColor color)
		{
			throw new NotImplementedException();
		}

		public void DrawLine(RGFloat x1, RGFloat y1, RGFloat x2, RGFloat y2, SolidColor color, RGFloat width, LineStyles style)
		{
			throw new NotImplementedException();
		}

		public void DrawLines(Point[] points, int start, int length, SolidColor color, RGFloat width, LineStyles style)
		{
			throw new NotImplementedException();
		}

		public void DrawPath(SolidColor color, RGPath graphicsPath)
		{
			throw new NotImplementedException();
		}

		public void DrawRectangle(RGPen p, Rectangle rect)
		{
			throw new NotImplementedException();
		}

		public void DrawRectangle(Rectangle rect, SolidColor color)
		{
			throw new NotImplementedException();
		}

		public void DrawRectangle(Rectangle rect, SolidColor color, RGFloat width, LineStyles lineStyle)
		{
			throw new NotImplementedException();
		}

		public void DrawRectangle(RGPen p, RGFloat x, RGFloat y, RGFloat width, RGFloat height)
		{
			throw new NotImplementedException();
		}

		public void DrawRectangle(RGFloat x, RGFloat y, RGFloat width, RGFloat height, SolidColor color)
		{
			throw new NotImplementedException();
		}

		public void DrawText(string text, string fontName, RGFloat size, SolidColor color, Rectangle rect)
		{
			throw new NotImplementedException();
		}

		public void DrawText(string text, string fontName, RGFloat size, SolidColor color, Rectangle rect, ReoGridHorAlign halign, ReoGridVerAlign valign)
		{
			throw new NotImplementedException();
		}

		public void FillEllipse(RGBrush b, Rectangle rectangle)
		{
			throw new NotImplementedException();
		}

		public void FillEllipse(IColor fillColor, Rectangle rectangle)
		{
			throw new NotImplementedException();
		}

		public void FillEllipse(RGBrush b, RGFloat x, RGFloat y, RGFloat widht, RGFloat height)
		{
			throw new NotImplementedException();
		}

		public void FillPath(IColor color, RGPath graphicsPath)
		{
			throw new NotImplementedException();
		}

		public void FillRectangle(Rectangle rect, IColor color)
		{
			throw new NotImplementedException();
		}

		public void FillRectangle(HatchStyles style, SolidColor hatchColor, SolidColor bgColor, Rectangle rect)
		{
			throw new NotImplementedException();
		}

		public void FillRectangle(RGBrush b, RGFloat x, RGFloat y, RGFloat width, RGFloat height)
		{
			throw new NotImplementedException();
		}

		public void FillRectangle(RGFloat x, RGFloat y, RGFloat width, RGFloat height, IColor color)
		{
			throw new NotImplementedException();
		}

		public void FillRectangle(HatchStyles style, SolidColor hatchColor, SolidColor bgColor, RGFloat x, RGFloat y, RGFloat width, RGFloat height)
		{
			throw new NotImplementedException();
		}

		public void FillRectangleLinear(SolidColor startColor, SolidColor endColor, RGFloat angle, Rectangle rect)
		{
			throw new NotImplementedException();
		}

		public void PopClip()
		{
			throw new NotImplementedException();
		}

		public RGTransform PopTransform()
		{
			throw new NotImplementedException();
		}

		public void PushClip(Rectangle clip)
		{
			throw new NotImplementedException();
		}

		public void PushTransform()
		{
			throw new NotImplementedException();
		}

		public void PushTransform(RGTransform t)
		{
			throw new NotImplementedException();
		}

		public void Reset()
		{
			throw new NotImplementedException();
		}

		public void ResetTransform()
		{
			throw new NotImplementedException();
		}

		public void RotateTransform(RGFloat angle)
		{
			throw new NotImplementedException();
		}

		public void ScaleTransform(RGFloat sx, RGFloat sy)
		{
			throw new NotImplementedException();
		}

		public void TranslateTransform(RGFloat x, RGFloat y)
		{
			throw new NotImplementedException();
		}
	}
}

#endif // iOS