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

namespace unvell.ReoGrid.Drawing.Shapes.SmartShapes
{
	/// <summary>
	/// Represents a block arc smart shape object.
	/// </summary>
	public class BlockArcShape : PieShape
	{
		#region Attributes
		private RGFloat arcWidth = 50;

		/// <summary>
		/// Get or set the width of block arc.
		/// </summary>
		public virtual RGFloat ArcWidth
		{
			get { return this.arcWidth; }
			set
			{
				if (this.arcWidth != value)
				{
					this.arcWidth = value;
					UpdatePath();
				}
			}
		}

		#endregion // Attributes

		protected override void UpdatePath()
		{
			var clientRect = this.ClientBounds;

			RGFloat s = Math.Min(this.arcWidth, Math.Min(clientRect.Width - 1, clientRect.Height - 1));

#if WINFORM
			Path.Reset();

			if (this.SweepAngle > 0 && clientRect.Width > 0 && clientRect.Height > 0)
			{
				var startAngle = this.StartAngle - 90;

				Path.AddArc(0, 0, clientRect.Width, clientRect.Height, startAngle, this.SweepAngle);
				Path.AddArc(s / 2, s / 2, clientRect.Width - s, clientRect.Height - s, startAngle + this.SweepAngle, -this.SweepAngle);
				Path.CloseAllFigures();
			}
#elif WPF

			Path.Clear();

			if (this.SweepAngle > 0)
			{
				System.Windows.Media.PathFigure pf = new System.Windows.Media.PathFigure();
			
				pf.Segments.Add(new System.Windows.Media.LineSegment(this.OriginPoint, false));
				pf.Segments.Add(new System.Windows.Media.ArcSegment(new System.Windows.Point(0, 0),
					new System.Windows.Size(this.Width, this.Height), this.SweepAngle, true, System.Windows.Media.SweepDirection.Clockwise, false));

				Path.Figures.Add(pf);
			}

#elif ANDROID
#endif // WINFORM
		}
	}
}

#endif // DRAWING