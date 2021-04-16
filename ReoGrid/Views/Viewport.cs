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

		public virtual GridRegion VisibleRegion
		{
			get { return visibleRegion; }
			set { visibleRegion = value; }
		}
		#endregion

		#region View
		protected Point viewStart;
		public virtual Point ViewStart { get { return viewStart; } set { viewStart = value; } }
		public virtual RGFloat ViewTop { get { return viewStart.Y; } set { viewStart.Y = value; } }
		public virtual RGFloat ViewLeft { get { return viewStart.X; } set { viewStart.X = value; } }
		//public virtual RGFloat ViewRight { get { return viewStart.X  + bounds.Width / this.scaleFactor; } }
		//public virtual RGFloat ViewBottom { get { return viewStart.Y + bounds.Height / this.scaleFactor; } }
		public virtual Rectangle ViewBounds
		{
			get
			{
				return new Rectangle(this.viewStart.X, this.viewStart.Y, this.bounds.Width / this.scaleFactor, this.bounds.Height / this.scaleFactor);
			}
		}

		private ScrollDirection scrollableDirections = ScrollDirection.None;
		public virtual ScrollDirection ScrollableDirections { get { return scrollableDirections; } set { scrollableDirections = value; } }

		public virtual void Scroll(RGFloat offX, RGFloat offY)
		{
			ViewTop += offY;
			ViewLeft += offX;

			if (ViewTop < 0) ViewTop = 0;
			if (ViewLeft < 0) ViewLeft = 0;
		}

		#endregion

		#region Point transform

		public override Point PointToView(Point p)
		{
			return new Point(
				(p.X - bounds.Left + viewStart.X * this.scaleFactor) / this.scaleFactor,
				(p.Y - bounds.Top + viewStart.Y * this.scaleFactor) / this.scaleFactor);
		}

		public override Point PointToController(Point p)
		{
			return new Point(
				(p.X - viewStart.X) * this.scaleFactor + bounds.Left,
				(p.Y - viewStart.Y) * this.scaleFactor + bounds.Top);
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
				g.TranslateTransform(bounds.Left - viewStart.X * this.scaleFactor, bounds.Top - viewStart.Y * this.scaleFactor);
			}

			DrawView(dc);

			if (this.PerformTransform)
			{
				g.PopTransform();
				g.PopClip();
			}
			
#if VP_DEBUG && WINFORM
				dc.Graphics.PlatformGraphics.DrawString(string.Format("VR {0},{1}-{2},{3} VS X{4},Y{5}\nSD {6}", this.visibleRegion.startRow,
					this.visibleRegion.startCol, this.visibleRegion.endRow, this.visibleRegion.endCol, this.ViewLeft, this.ViewTop,
					this.ScrollableDirections.ToString()),
					System.Drawing.SystemFonts.DialogFont, System.Drawing.Brushes.Blue, this.Left + 1, this.Top + 
					((this is CellsViewport) ? 30 : this.Height / 2));
#endif // VP_DEBUG && WINFORM

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

