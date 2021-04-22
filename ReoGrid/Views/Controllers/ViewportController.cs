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
 * Author: Jingwood <jingwood at unvell.com>
 *
 * Copyright (c) 2012-2021 Jingwood, unvell.com, all rights reserved.
 * 
 ****************************************************************************/

#define VP_DEBUG

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
}

