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
}

