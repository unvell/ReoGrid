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
using unvell.ReoGrid.Core;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

using RGFloat = System.Double;
using RGPen = CoreGraphics.CGContext;
using RGBrush = CoreGraphics.CGContext;

namespace unvell.ReoGrid.iOS
{
	class Renderer : Graphics, IRenderer
	{
		public void BeginCappedLine(LineCapStyles startCap, Size startSize, LineCapStyles endCap, Size endSize, SolidColor color)
		{
			throw new NotImplementedException();
		}

		public void BeginDrawHeaderText(RGFloat scale)
		{
			throw new NotImplementedException();
		}

		public void BeginDrawLine(RGFloat width, SolidColor color)
		{
			throw new NotImplementedException();
		}

		public void DrawCappedLine(RGFloat x1, RGFloat y1, RGFloat x2, RGFloat y2)
		{
			throw new NotImplementedException();
		}

		public void DrawCellText(ReoGridCell cell, SolidColor textColor, DrawMode drawMode, RGFloat scale)
		{
			throw new NotImplementedException();
		}

		public void DrawHeaderText(string text, RGBrush brush, Rectangle rect)
		{
			throw new NotImplementedException();
		}

		public void DrawLeadHeadArrow(Rectangle bounds, SolidColor startColor, SolidColor endColor)
		{
			throw new NotImplementedException();
		}

		public void DrawLine(RGFloat x1, RGFloat y1, RGFloat x2, RGFloat y2)
		{
			throw new NotImplementedException();
		}

		public void DrawRunningFocusRect(RGFloat x, RGFloat y, RGFloat w, RGFloat h, SolidColor color, int runningOffset)
		{
			throw new NotImplementedException();
		}

		public void EndCappedLine()
		{
			throw new NotImplementedException();
		}

		public void EndDrawLine()
		{
			throw new NotImplementedException();
		}

		public RGBrush GetBrush(SolidColor color)
		{
			throw new NotImplementedException();
		}

		public RGPen GetPen(SolidColor color)
		{
			throw new NotImplementedException();
		}

		public Size MeasureTextSize(ReoGridCell cell, DrawMode drawMode, RGFloat scale)
		{
			throw new NotImplementedException();
		}

		public void ReleasePen(RGPen pen)
		{
			throw new NotImplementedException();
		}

		public void UpdateCellRenderFont(ReoGridCell cell, UpdateFontReason reason)
		{
			throw new NotImplementedException();
		}
	}
}

#endif // iOS