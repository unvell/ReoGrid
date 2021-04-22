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

//#define VP_DEBUG

using System.Collections.Generic;
using System.Diagnostics;

#if WINFORM || ANDROID
using RGFloat = System.Single;
using RGIntDouble = System.Int32;

#elif WPF
using RGFloat = System.Double;
using RGIntDouble = System.Double;

#elif iOS
using RGFloat = System.Double;
using RGIntDouble = System.Double;

#endif

using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Interaction;
using unvell.ReoGrid.Main;

namespace unvell.ReoGrid.Views
{
	internal abstract class Viewport : View, IViewport
	{
		protected Worksheet sheet;

		internal Worksheet Worksheet { get { return this.sheet; } }

		public Viewport(IViewportController vc)
			: base(vc)
		{
			this.sheet = vc.Worksheet;
		}

		#region Visible Region
		protected GridRegion visibleRegion;

		/// <summary>
		/// View window for cell address, decides how many cells are visible for this viewport.
		/// </summary>
		public virtual GridRegion VisibleRegion
		{
			get { return visibleRegion; }
			set { visibleRegion = value; }
		}
		#endregion

		#region View window
		protected Point viewStart;

		/// <summary>
		/// View window start position. (Scroll position)
		/// </summary>
		public virtual Point ViewStart { get { return viewStart; } set { viewStart = value; } }

		/// <summary>
		/// Top position of view window. (Vertial scroll position)
		/// </summary>
		public virtual RGFloat ViewTop { get { return viewStart.Y; } set { viewStart.Y = value; } }

		/// <summary>
		/// Left position of view window. (Horizontal scroll position)
		/// </summary>
		public virtual RGFloat ViewLeft { get { return viewStart.X; } set { viewStart.X = value; } }
		//public virtual RGFloat ViewRight { get { return viewStart.X  + bounds.Width / this.scaleFactor; } }
		//public virtual RGFloat ViewBottom { get { return viewStart.Y + bounds.Height / this.scaleFactor; } }

		/// <summary>
		/// The bounds of view window, starts from scroll position, ends at scroll position + window size.
		/// </summary>
		public virtual Rectangle ViewBounds
		{
			get
			{
				return new Rectangle(this.ScrollViewLeft, this.ScrollViewTop, this.bounds.Width / this.scaleFactor, this.bounds.Height / this.scaleFactor);
			}
		}

		public virtual ScrollDirection ScrollableDirections { get; set; } = ScrollDirection.None;

		public RGFloat ScrollX { get; set; }
		public RGFloat ScrollY { get; set; }

		public RGFloat ScrollViewLeft { get { return this.viewStart.X + ScrollX; } }
		public RGFloat ScrollViewTop { get { return this.viewStart.Y + ScrollY; } }

		public virtual void Scroll(RGFloat offX, RGFloat offY)
		{
			ScrollX += offX;
			ScrollY += offY;

			if (ScrollX < 0) ScrollX = 0;
			if (ScrollY < 0) ScrollY = 0;
		}

		public virtual void ScrollTo(RGFloat x, RGFloat y)
		{
			if (x >= 0 && (this.ScrollableDirections & ScrollDirection.Horizontal) == ScrollDirection.Horizontal) ScrollX = x;
			if (y >= 0 && (this.ScrollableDirections & ScrollDirection.Vertical) == ScrollDirection.Vertical) ScrollY = y;

			if (ScrollX < 0) ScrollX = 0;
			if (ScrollY < 0) ScrollY = 0;
		}


		#endregion // View window

		#region Point transform

		public override Point PointToView(Point p)
		{
			return new Point(
				(p.X - bounds.Left + ScrollViewLeft * this.scaleFactor) / this.scaleFactor,
				(p.Y - bounds.Top + ScrollViewTop * this.scaleFactor) / this.scaleFactor);
		}

		public override Point PointToController(Point p)
		{
			return new Point(
				(p.X - ScrollViewLeft) * this.scaleFactor + bounds.Left,
				(p.Y - ScrollViewTop) * this.scaleFactor + bounds.Top);
		}

		#endregion // Point transform

		#region Draw

		public override void Draw(CellDrawingContext dc)
		{
#if DEBUG
			Stopwatch sw = Stopwatch.StartNew();
#endif

			if (!Visible //|| visibleGridRegion == GridRegion.Empty
				|| bounds.Width <= 0 || bounds.Height <= 0) return;

			//bool needClip = this.Parent == null
			//	|| this.bounds != this.Parent.Bounds;

			//bool needTranslate = this.Parent == null
			//	|| this.viewStart.X != this.Parent.ViewLeft
			//	|| this.ViewStart.Y != this.Parent.ViewTop;

			var g = dc.Graphics;

			if (PerformTransform)
			{
				g.PushClip(this.bounds);
				g.PushTransform();
				g.TranslateTransform(bounds.Left - ScrollViewLeft * this.scaleFactor, bounds.Top - ScrollViewTop * this.scaleFactor);
			}

			DrawView(dc);

			if (this.PerformTransform)
			{
				g.PopTransform();
				g.PopClip();
			}

#if VP_DEBUG
#if WINFORM
			if (this is SheetViewport
				|| this is ColumnHeaderView
				//|| this is RowHeaderView
				|| this is RowOutlineView)
			{
				//var rect = this.bounds;
				//rect.Width--;
				//rect.Height--;
				//dc.Graphics.DrawRectangle(this.bounds, this is SheetViewport ? SolidColor.Blue : SolidColor.Purple);

				var msg = $"{ this.GetType().Name }\n" +
					$"{visibleRegion.ToRange()}\n" +
					$"{this.ViewLeft}, {this.ViewTop}, ({ScrollX}, {ScrollY}), {this.Width}, {this.Height}\n" +
					$"{this.ScrollableDirections}";

				dc.Graphics.PlatformGraphics.DrawString(msg,
						System.Drawing.SystemFonts.DefaultFont, System.Drawing.Brushes.Blue, this.Left + Width / 2, Top + Height / 2);
			}
#elif WPF
			var msg = string.Format("VR {0},{1}-{2},{3} VS X{4},Y{5}\nSD {6}", this.visibleRegion.startRow,
				this.visibleRegion.startCol, this.visibleRegion.endRow, this.visibleRegion.endCol, this.ViewLeft, this.ViewTop,
				this.ScrollableDirections.ToString());

			var ft = new System.Windows.Media.FormattedText(msg, System.Globalization.CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, 
				new System.Windows.Media.Typeface("Arial"), 12, System.Windows.Media.Brushes.Blue, 96);

			dc.Graphics.PlatformGraphics.DrawText(ft, new System.Windows.Point(this.Left + 1, this.Top + ((this is CellsViewport) ? 30 : this.Height / 2)));
#endif // WPF
#endif // VP_DEBUG

#if DEBUG
			sw.Stop();
			if (sw.ElapsedMilliseconds > 20)
			{
				Debug.WriteLine("draw viewport takes " + sw.ElapsedMilliseconds + " ms. visible region: rows: " + visibleRegion.Rows + ", cols: " + visibleRegion.Cols);
			}
#endif // Debug

		}

		public virtual void DrawView(CellDrawingContext dc)
		{
			this.DrawChildren(dc);
		}

		#endregion // Draw
	
	}

}

