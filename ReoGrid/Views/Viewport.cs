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
	
	#region ViewFlags
	
	internal enum ViewTypes
	{
		None = 0x0,
		Cells = 0x1,

		ColumnHeader = 0x2,
		RowHeader = 0x4,
		LeadHeader = ColumnHeader | RowHeader,

		ColOutline = 0x10,
		RowOutline = 0x20,
		Outlines = ColOutline | RowOutline,
	}

	#endregion // ViewFlags

	#region GridRegion
	internal struct GridRegion
	{
		internal int startRow;
		internal int endRow;
		internal int startCol;
		internal int endCol;
		internal static readonly GridRegion Empty = new GridRegion()
		{
			startRow = 0,
			startCol = 0,
			endRow = 0,
			endCol = 0
		};
		public GridRegion(int startRow, int startCol, int endRow, int endCol)
		{
			this.startRow = startRow;
			this.startCol = startCol;
			this.endRow = endRow;
			this.endCol = endCol;
		}
		public bool Contains(CellPosition pos)
		{
			return Contains(pos.Row, pos.Col);
		}
		public bool Contains(int row, int col)
		{
			return this.startRow <= row && this.endRow >= row && this.startCol <= col && this.endCol >= col;
		}
		public bool Contains(RangePosition range)
		{
			return range.Row >= this.startRow && range.Col >= this.startCol
				&& range.EndRow <= this.endRow && range.EndCol <= this.endCol;
		}
		public bool Intersect(RangePosition range)
		{
			return (range.Row < this.startRow && range.EndRow > this.startRow)
				|| (range.Row < this.endRow && range.EndRow > this.endRow)
				|| (range.Col < this.startCol && range.EndCol > this.startCol)
				|| (range.Col < this.endCol && range.EndCol > this.endCol);
		}
		public bool IsOverlay(RangePosition range)
		{
			return Contains(range) || Intersect(range);
		}
		public override bool Equals(object obj)
		{
			if ((obj as GridRegion?) == null) return false;

			GridRegion gr2 = (GridRegion)obj;
			return startRow == gr2.startRow && startCol == gr2.startCol
				&& endRow == gr2.endRow && endCol == gr2.endCol;
		}
		public override int GetHashCode()
		{
			return startRow ^ startCol ^ endRow ^ endCol;
		}
		public static bool operator ==(GridRegion gr1, GridRegion gr2)
		{
			return gr1.Equals(gr2);
		}
		public static bool operator !=(GridRegion gr1, GridRegion gr2)
		{
			return !gr1.Equals(gr2);
		}
		public bool IsEmpty { get { return this.Equals(Empty); } }
		public int Rows { get { return endRow - startRow + 1; } set { endRow = startRow + value - 1; } }
		public int Cols { get { return endCol - startCol + 1; } set { endCol = startCol + value - 1; } }

		public override string ToString()
		{
			return string.Format("VisibleRegion[{0},{1}-{2},{3}]", startRow, startCol, endRow, endCol);
		}

		/// <summary>
		/// Convert into range struct
		/// </summary>
		/// <returns></returns>
		public RangePosition ToRange()
		{
			return new RangePosition(startRow, startCol, endRow - startRow + 1, endCol - startCol + 1);
		}
	}
	#endregion // GridRegion

	#region IView

	interface IView : IUserVisual
	{
		IViewportController ViewportController { get; set; }

		Rectangle Bounds { get; set; }
		RGFloat Left { get; }
		RGFloat Top { get;  }
		RGFloat Width { get;  }
		RGFloat Height { get;  }
		RGFloat Right { get; }
		RGFloat Bottom { get; }

		void UpdateView();

		RGFloat ScaleFactor { get; set; }

		bool PerformTransform { get; set; }
		void Draw(CellDrawingContext dc);
		void DrawChildren(CellDrawingContext dc);
		bool Visible { get; set; }

		Point PointToView(Point p);
		Point PointToController(Point p);
		IView GetViewByPoint(Point p);

		IList<IView> Children { get; set; }
	}

	#endregion // IView

	#region View

	internal class View : IView
	{
		private IViewportController viewportController;

		public IViewportController ViewportController
		{
			get { return this.viewportController; }
			set { this.viewportController = value; }
		}

		public View()
		{
			this.Visible = true;
			this.PerformTransform = true;
		}

		public View(IViewportController vc)
			: this()
		{
			this.viewportController = vc;
		}

		#region Bounds
		protected Rectangle bounds = new Rectangle(0, 0, 0, 0);
		public virtual Rectangle Bounds
		{
			get { return bounds; }
			set { bounds = value; }
		}
		public virtual RGFloat Top { get { return bounds.Top; } set { bounds.Y = value; } }
		public virtual RGFloat Left { get { return bounds.Left; } set { bounds.X = value; } }
		public virtual RGFloat Right { get { return bounds.Right; } }
		public virtual RGFloat Bottom { get { return bounds.Bottom; } }
		public virtual RGFloat Width { get { return bounds.Width; } set { bounds.Width = value; } }
		public virtual RGFloat Height { get { return bounds.Height; } set { bounds.Height = value; } }
		#endregion

		protected RGFloat scaleFactor = 1f;
		public virtual RGFloat ScaleFactor { get { return scaleFactor; } set { scaleFactor = value; } }

		public virtual bool Visible { get; set; }

		public virtual bool PerformTransform { get; set; }

		public virtual void Draw(CellDrawingContext dc)
		{
			this.DrawChildren(dc);
		}

		public virtual void DrawChildren(CellDrawingContext dc)
		{
			if (this.children != null)
			{
				foreach (var view in this.children)
				{
					if (view.Visible)
					{
						dc.CurrentView = view;
						view.Draw(dc);
						dc.CurrentView = null;
					}
				}
			}
		}

		public virtual Point PointToView(Point p)
		{
			return new Point(
				(p.X - bounds.Left) / this.scaleFactor,
				(p.Y - bounds.Top) / this.scaleFactor);
		}
		public virtual Point PointToController(Point p)
		{
			return new Point(
					p.X * this.scaleFactor + bounds.Left,
					p.Y * this.scaleFactor + bounds.Top);
		}

		public virtual IView GetViewByPoint(Point p)
		{
			IView child = this.GetChildrenByPoint(p);

			if (child != null)
			{
				return child;
			}
			else
			{
				return this.bounds.Contains(p) ? this : null;
			}
		}

		public virtual IView GetChildrenByPoint(Point p)
		{
			if (this.children != null
				&& this.children.Count > 0)
			{
				for (int i = this.children.Count - 1; i >= 0; i--)
				{
					var child = this.children[i];
					if (!child.Visible) continue;

					var view = child.GetViewByPoint(p);

					if (view != null)
					{
						return view;
					}
				}
			}

			return null;
		}

		public virtual void Invalidate()
		{
			if (this.viewportController != null)
			{
				this.viewportController.Invalidate();
			}
		}

		public virtual bool OnMouseDown(Point location, MouseButtons buttons) { return false; }

		public virtual bool OnMouseMove(Point location, MouseButtons buttons) { return false; }

		public virtual bool OnMouseUp(Point location, MouseButtons buttons) { return false; }

		public virtual bool OnMouseDoubleClick(Point location, MouseButtons buttons) { return false; }

		public virtual bool OnKeyDown(KeyCode key) { return false; }

		public virtual void UpdateView() 
		{
			if (this.children != null)
			{
				foreach (var child in this.children)
				{
					child.UpdateView();
				}
			}
		} 

		public virtual void SetFocus() { this.ViewportController.FocusView = this; }
		public virtual void FreeFocus() { this.ViewportController.FocusView = null; }
	
		public IViewport Parent { get; set; }

		protected IList<IView> children = null;
		public IList<IView> Children { get { return this.children; } set { this.children = value; } }
	}

	#endregion // View

	#region IViewport
	interface IViewport : IView
	{
		Point ViewStart { get; set; }
		RGFloat ViewTop { get; }
		RGFloat ViewLeft { get; }
		//RGFloat ViewRight { get; }
		//RGFloat ViewBottom { get; }

		ScrollDirection ScrollableDirections { get; set; }
		void Scroll(RGFloat offX, RGFloat offY);

		GridRegion VisibleRegion { get; set; }
	}
	#endregion // IViewport

	#region Viewport

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

	#endregion // Viewport

	#region ViewportController Interfaces

	internal interface IViewportController : IUserVisual, IVisualController
	{
		Worksheet Worksheet { get; }

		Rectangle Bounds { get; set; }

		IView View { get; }
		IView FocusView { get; set; }
		
		void Draw(CellDrawingContext dc);

		void UpdateController();
		void Reset();

		void SetViewVisible(ViewTypes view, bool visible);
	}

	/// <summary>
	/// Interface for freezable ViewportController
	/// </summary>
	internal interface IFreezableViewportController
	{
		/// <summary>
		/// Freeze to specified cell and position.
		/// </summary>
		///// <param name="row">Number of row to start freeze.</param>
		///// <param name="col">Number of column to start freeze.</param>
		///// <param name="resetViewStart">Determines whether or not to reset the view start position.</param>
		///// <param name="position">Freeze position that decides the frozen view position.</param>
		void Freeze(/*int row, int col, FreezeArea position = ReoGrid.FreezeArea.LeftTop*/);

		///// <summary>
		///// Get or set position of freezing viewport.
		///// </summary>
		//FreezeArea FreezePosition { get; }

		///// <summary>
		///// Get current frozen position.
		///// </summary>
		//CellPosition FreezePos { get; }
	}

	internal interface IScrollableViewportController
	{
		void HorizontalScroll(RGIntDouble value);

		void VerticalScroll(RGIntDouble value);

		void ScrollViews(ScrollDirection dir, RGFloat x, RGFloat y);

		void ScrollToRange(RangePosition range, CellPosition pos);

		void SynchronizeScrollBar();
	}

	internal interface IScalableViewportController
	{
		RGFloat ScaleFactor { get; set; }
	}

	#endregion // ViewportController Interfaces

	#region ViewportController Implements

	#region Abstract ViewportController

	internal class ViewportController : IViewportController
	{
		#region Constructor

		internal Worksheet worksheet;

		public Worksheet Worksheet { get { return this.worksheet; } }

		public ViewportController(Worksheet sheet)
		{
			this.worksheet = sheet;

			this.view = new View(this);
			this.view.Children = new List<IView>();
		}

		#endregion // Constructor

		#region Bounds

		//private RGRect bounds;
		public virtual Rectangle Bounds { get { return this.view.Bounds; } set { this.view.Bounds = value; } }

		//public virtual RGSize Size { get { return this.Bounds.Size; }  }
		//public virtual RGIntDouble Left { get { return this.Bounds.X; } }
		//public virtual RGIntDouble Top { get { return this.Bounds.Y; } }
		//public virtual RGIntDouble Width { get { return this.Bounds.Width; } }
		//public virtual RGIntDouble Height { get { return this.Bounds.Height; } }
		//public virtual RGIntDouble Right { get { return this.Bounds.Right; } }
		//public virtual RGIntDouble Bottom { get { return this.Bounds.Bottom; } }

		public virtual RGFloat ScaleFactor
		{
			get { return this.View == null ? 1f : this.View.ScaleFactor; }
			set { if (this.View != null) this.View.ScaleFactor = value; }
		}
		#endregion // Bounds

		#region Viewport Management
		protected IView view;

		public virtual IView View { get { return this.view; } set { this.view = value; } }

		internal virtual void AddView(IView view)
		{
			this.view.Children.Add(view);
			view.ViewportController = this;
		}

		internal virtual void InsertView(IView before, IView viewport)
		{
			IList<IView> views = this.view.Children;

			int index = views.IndexOf(before);
			if (index > 0 && index < views.Count)
			{
				views.Insert(index, viewport);
			}
			else
			{
				views.Add(viewport);
			}
		}

		internal virtual void InsertView(int index, IView viewport)
		{
			this.view.Children.Insert(index, viewport);
		}

		internal virtual bool RemoveView(IView view)
		{
			if (this.view.Children.Remove(view))
			{
				view.ViewportController = null;
				return true;
			}
			else
				return false;
		}

		protected ViewTypes viewsVisible = ViewTypes.LeadHeader;

		internal bool IsViewVisible(ViewTypes head)
		{
			return (viewsVisible & head) == head;
		}

		public virtual void SetViewVisible(ViewTypes head, bool visible)
		{
			if (visible)
			{
				viewsVisible |= head;
			}
			else
			{
				viewsVisible &= ~head;
			}
		}

		#endregion // Viewport Management

		#region Update
		public virtual void UpdateController() { }

		public virtual void Reset() { }

		public virtual void Invalidate()
		{
			if (this.worksheet != null)
			{
				this.worksheet.RequestInvalidate();
			}
		}
		#endregion // Update

		#region Draw
		public virtual void Draw(CellDrawingContext dc)
		{
			if (this.view.Visible && this.view.Width > 0 && this.view.Height > 0)
			{
				view.Draw(dc);
			}

			this.worksheet.viewDirty = false;
		}
		#endregion // Draw

		#region Focus
		
		public virtual IView FocusView { get; set; }

		public virtual IUserVisual FocusVisual { get; set; }

		#endregion // View Point Evalution

		#region UI Handle
		public virtual bool OnMouseDown(Point location, MouseButtons buttons)
		{
			bool isProcessed = false;

			if (!isProcessed)
			{
				var targetView = this.view.GetViewByPoint(location);

				if (targetView != null)
				{
					isProcessed = targetView.OnMouseDown(targetView.PointToView(location), buttons);
				}
			}

			return isProcessed;
		}

		public virtual bool OnMouseMove(Point location, MouseButtons buttons)
		{
			bool isProcessed = false;

			if (this.FocusView != null)
			{
				this.FocusView.OnMouseMove(this.FocusView.PointToView(location), buttons);
			}

			if (!isProcessed)
			{
				var targetView = this.view.GetViewByPoint(location);

				if (targetView != null)
				{
					isProcessed = targetView.OnMouseMove(targetView.PointToView(location), buttons);
				}
			}

			return isProcessed;
		}

		public virtual bool OnMouseUp(Point location, MouseButtons buttons)
		{
			bool isProcessed = false;

			if (this.FocusView != null)
			{
				isProcessed = this.FocusView.OnMouseUp(this.FocusView.PointToView(location), buttons);
			}

			return isProcessed;
		}

		public virtual bool OnMouseDoubleClick(Point location, MouseButtons buttons)
		{
			bool isProcessed = false;

			var targetView = this.FocusView != null ? this.FocusView
				: this.view.GetViewByPoint(location);

			if (targetView != null)
			{
				isProcessed = targetView.OnMouseDoubleClick(targetView.PointToView(location), buttons);
			}

			return isProcessed;
		}

		public virtual bool OnKeyDown(KeyCode key) { return false; }

		public virtual void SetFocus() { }

		public virtual void FreeFocus() { }
		#endregion // Mouse

	}
	#endregion // Abstract ViewportController

	#region PageLayout ViewportController
	//class PageLayoutViewportController : AbstractViewportController
	//{
	//	public PageLayoutViewportController(Worksheet grid, RGRect bounds) : base(grid) { }

	//	public override void OnBoundsChange()
	//	{
	//		throw new NotImplementedException();
	//	}

	//	public override void ScrollViews(ScrollDirection dir, RGFloat x, RGFloat y)
	//	{
	//		throw new NotImplementedException();
	//	}
	//}
	#endregion // PageLayout ViewportController

	#endregion // ViewportController Implements
}

