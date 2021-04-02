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

#if DRAWING

using System;

#if WINFORM
using RGFloat = System.Single;
#else
using RGFloat = System.Double;
#endif // WINFORM

using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Graphics;

namespace unvell.ReoGrid.Drawing.Shapes
{
	#region Diamond
	/// <summary>
	/// Represents diamond shape object.
	/// </summary>
	public class DiamondShape : ShapeObject
	{
		private Point[] points = new Point[4];

		/// <summary>
		/// Render diamond shape to graphics context.
		/// </summary>
		/// <param name="dc">Platform no-associated drawing context.</param>
		protected override void OnPaint(DrawingContext dc)
		{
			var g = dc.Graphics;

			var clientRect = this.ClientBounds;

			if (clientRect.Width > 0 && clientRect.Height > 0)
			{
				var w = this.Bounds.Width;
				var h = this.Bounds.Height;
				var hw = w / 2f;
				var hh = h / 2f;

				points[0] = new Point(hw, 0);
				points[1] = new Point(w, hh);
				points[2] = new Point(hw, h);
				points[3] = new Point(0, hh);

				if (!this.FillColor.IsTransparent)
				{
					g.FillPolygon(this.FillColor, points);
				}

				if (!this.LineColor.IsTransparent)
				{
					g.IsAntialias = true;

					g.DrawPolygon(this.LineColor, this.LineWidth, this.LineStyle, points);

					g.IsAntialias = false;
				}
			}

			base.OnPaintText(dc);
		}
	}
	#endregion // Diamond

}

#endif // DRAWING