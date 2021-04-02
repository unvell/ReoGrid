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

#if WINFORM || ANDROID
using RGFloat = System.Single;
#elif WPF
using RGFloat = System.Double;
#endif // WPF

using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Graphics;

namespace unvell.ReoGrid.Drawing.Shapes
{
	/// <summary>
	/// Represents straight line connection.
	/// </summary>
	public class Line : DrawingObject
	{
		private Point startPoint;

		/// <summary>
		/// Get or set the start position.
		/// </summary>
		public Point StartPoint
		{
			get { return this.startPoint; }
			set
			{
				if (this.startPoint != value)
				{
					this.startPoint = value;
					this.UpdateBoundsByTwoPoints();
				}
			}
		}

		private Point endPoint;

		/// <summary>
		/// Get or set the end position.
		/// </summary>
		public Point EndPoint
		{
			get { return this.endPoint; }
			set
			{
				if (this.endPoint != value)
				{
					this.endPoint = value;
					this.UpdateBoundsByTwoPoints();
				}
			}
		}

		/// <summary>
		/// Update bounds by start and end positions.
		/// </summary>
		protected virtual void UpdateBoundsByTwoPoints()
		{
			this.Bounds = new Rectangle(this.startPoint, this.endPoint);
			this.Invalidate();
		}

		/// <summary>
		/// Render line object to graphics context.
		/// </summary>
		/// <param name="dc">Platform no-associated drawing context instance.</param>
		public override void Draw(DrawingContext dc)
		{
			var r = dc.Renderer;

			if (this.StartCap != LineCapStyles.None || this.EndCap != LineCapStyles.None)
			{
				RGFloat capSize = (RGFloat)System.Math.Max(System.Math.Pow(this.LineWidth, 0.75d), 4.2d);

				r.BeginCappedLine(this.StartCap, new Size(capSize, capSize),
					this.EndCap, new Size(capSize, capSize), this.LineColor, this.LineWidth);

				r.DrawCappedLine(this.startPoint.X, this.startPoint.Y, this.endPoint.X, this.endPoint.Y);

				r.EndCappedLine();
			}
			else
			{
				r.DrawLine(this.startPoint, this.endPoint, this.LineColor, this.LineWidth, this.LineStyle);
			}
		}

		internal LineCapStyles StartCap { get; set; }
		internal LineCapStyles EndCap { get; set; }

		private IDrawingLineObjectStyle lineStyleWrapper;

		/// <summary>
		/// Get line style object.
		/// </summary>
		public new IDrawingLineObjectStyle Style
		{
			get
			{
				if (this.lineStyleWrapper == null)
				{
					this.lineStyleWrapper = new DrawingLineObjectStyle(this);
				}

				return this.lineStyleWrapper;
			}
		}
	}

	class DrawingLineObjectStyle : DrawingObjectStyle, IDrawingLineObjectStyle
	{
		private Line line;

		public DrawingLineObjectStyle(Line line)
			: base(line)
		{
			this.line = line;
		}

		public LineCapStyles StartCap
		{
			get { return line.StartCap; } set { line.StartCap = value; }
		}

		public LineCapStyles EndCap
		{
			get { return line.EndCap; } set { line.EndCap = value; }
		}
	}
}

#endif // DRAWING