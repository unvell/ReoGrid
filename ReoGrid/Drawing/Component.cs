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

using System.Collections.Generic;

#if WINFORM || ANDROID
using RGFloat = System.Single;
#elif WPF
using RGFloat = System.Double;
#endif

using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Interaction;
using System;

namespace unvell.ReoGrid.Drawing
{
	/// <summary>
	/// Represents container of drawing objects.
	/// </summary>
	public interface IDrawingContainer : IDrawingObject
	{
		/// <summary>
		/// Collection of children objects in this container
		/// </summary>
		IDrawingObjectCollection Children { get; }

		/// <summary>
		/// Clip bounds for rendering final object
		/// </summary>
		Rectangle ClipBounds { get; set; }

		/// <summary>
		/// Method invoked when child object added.
		/// </summary>
		/// <param name="child">Instance of child object has been added.</param>
		/// <param name="index">The index of child object in container.</param>
		void OnChildAdded(IDrawingObject child, int index);

		/// <summary>
		/// Method invoked when child object removed from container.
		/// </summary>
		/// <param name="child">Instance of child object has been removed.</param>
		/// <param name="index">The index of child object in container.</param>
		void OnChildRemoved(IDrawingObject child, int index);

		/// <summary>
		/// Method invoked when all child objects has been removed.
		/// </summary>
		void OnChildrenClear();
	}

	/// <summary>
	/// Represents drawing component object that contains others drawing objects.
	/// </summary>
	[Serializable]
	public class DrawingComponent : DrawingObject, IDrawingContainer
	{
		#region Draw
		/// <summary>
		/// Get or set the clip bounds to render final object
		/// </summary>
		public virtual Rectangle ClipBounds { get; set; }
		
		/// <summary>
		/// Render drawing object to graphics context.
		/// </summary>
		/// <param name="dc">Platform no-associated drawing context instance.</param>
		protected override void OnPaint(DrawingContext dc)
		{
			base.OnPaint(dc);
		
			DrawChildren(dc);
		}

		/// <summary>
		/// Render children objects to graphics context.
		/// </summary>
		/// <param name="dc">Platform no-associated drawing context instance.</param>
		protected virtual void DrawChildren(DrawingContext dc)
		{
			foreach (var child in this.drawingObjects)
			{
				if (child is DrawingObject)
				{
					var drawingObject = (DrawingObject)child;
					if (!drawingObject.Visible) continue;
				}

				if (this.ClipBounds.Width > 0 || this.ClipBounds.Height > 0)
				{
					if (child.Bounds.IntersectWith(this.ClipBounds))
					{
						child.Draw(dc);
					}
#if DEBUG
					else
					{
					}
#endif // DEBUG
				}
				else
				{
					child.Draw(dc);
				}
			}
		}
		#endregion // Draw

		#region Children Management
		internal List<IDrawingObject> drawingObjects = new List<IDrawingObject>();

		private DrawingObjectCollection drawingObjectCollection = null;

		public virtual IDrawingObjectCollection Children
		{
			get
			{
				if (this.drawingObjectCollection == null)
				{
					this.drawingObjectCollection = new DrawingObjectCollection(this);
				}

				return this.drawingObjectCollection;
			}
		}

		public virtual void OnChildAdded(IDrawingObject child, int index) { }

		public virtual void OnChildRemoved(IDrawingObject child, int index) { }

		public virtual void OnChildrenClear() { }
		#endregion // Children Management

		#region Style
		private DrawingComponentStyle style;

		/// <summary>
		/// Get style object.
		/// </summary>
		public new IDrawingComponentStyle Style
		{
			get
			{
				if (this.style == null)
				{
					this.style = new DrawingComponentStyle(this);
				}

				return this.style;
			}
		}
		#endregion // Style

		#region Resize
		public override void OnBoundsChanged(Rectangle oldRect)
		{
			base.OnBoundsChanged(oldRect);

			//RGFloat offsetX = this.innerX - oldRect.X;
			//RGFloat offsetY = this.innerY - oldRect.Y;

			//if (offsetX != 0 || offsetY != 0)
			//{
			//	this.OffsetChildren(offsetX, offsetY);
			//}

			//return;
			RGFloat scaleX = oldRect.Width == 0 ? 1 : this.Width / oldRect.Width;
			RGFloat scaleY = oldRect.Height == 0 ? 1 : this.Height / oldRect.Height;

			if (scaleX != 1 || scaleY != 1)
			{
				this.ScaleChildren(scaleX, scaleY);
			}
		}
		
		protected void ScaleChildren(RGFloat scaleX, RGFloat scaleY)
		{
			if (this.Children != null)
			{
				foreach (var child in this.Children)
				{
					var oldBounds = child.Bounds;

					child.Bounds = new Rectangle(child.X * scaleX, child.Y * scaleY,
						child.Width * scaleX, child.Height * scaleY);

					//child.OnBoundsChanged(oldBounds);

#if DEBUG
					System.Diagnostics.Debug.Assert(!double.IsInfinity(child.Width)
						&& !double.IsInfinity(child.Height));
#endif // DEBUG
				}
			}
		}
		#endregion // Resize

		#region UI Handles
		public override bool OnMouseDown(Point location, MouseButtons button)
		{
			bool isProcessed = false;

			foreach (var obj in this.drawingObjects)
			{
				if (obj.Bounds.Contains(location))
				{
					isProcessed = obj.OnMouseDown(new Point(location.X - obj.X, location.Y - obj.Y), button);
					if (isProcessed) break;
				}
			}

			if (!isProcessed)
			{
				isProcessed = base.OnMouseDown(location, button);
			}

			return isProcessed;
		}
		#endregion // UI Handles
	}

	/*
	/// <summary>
	/// Represents layout drawing component.
	/// </summary>
	public class LayoutDrawingComponent : DrawingComponent
	{
		/// <summary>
		/// Get or set layout horizontal alignment.
		/// </summary>
		public HorizontalAlignment LayoutHorizontalAlignment { get; set; }

		/// <summary>
		/// Get or set layout vertical alignment.
		/// </summary>
		public VerticalAlignment LayoutVerticalAlignment { get; set; }

		/// <summary>
		/// Get or set the layout manager.
		/// </summary>
		public LayoutManager LayoutManager { get; set; }

		public override void OnBoundsChanged(Rectangle oldRect)
		{
			base.OnBoundsChanged(oldRect);

			this.Relayout();
		}

		public virtual void Relayout()
		{
			this.Layout(this.ClientBounds);
		}

		public virtual void Layout(Rectangle parentRect)
		{
			if (this.LayoutManager != null)
			{
				this.LayoutManager.LayoutChildren();
			}
		}

		public override void OnChildAdded(IDrawingObject child, int index)
		{
			base.OnChildAdded(child, index);

			if (this.LayoutManager != null && child is LayoutDrawingComponent)
			{
				this.LayoutManager.LayoutChild((LayoutDrawingComponent)child);
			}
		}
	}

	public class AlignmentLayoutDrawingComponent : LayoutDrawingComponent
	{
		public AlignmentLayoutDrawingComponent()
		{
			this.LayoutManager = new AlignmentLayoutManager(this);
		}
	}

	public abstract class LayoutManager
	{
		public DrawingComponent Component
		{
			get; private set;
		}

		public LayoutManager(DrawingComponent component)
		{
			this.Component = component;
		}

		public abstract void LayoutChildren();

		public abstract void LayoutChild(LayoutDrawingComponent child);
	}

	public class AlignmentLayoutManager : LayoutManager
	{
		public AlignmentLayoutManager(DrawingComponent component)
			: base(component)
		{
		}

		public override void LayoutChildren()
		{
			if (this.Component == null) return;

			var clientRect = this.Component.ClientBounds;

			foreach (var child in this.Component.Children)
			{
				if (child is LayoutDrawingComponent)
				{
					this.LayoutChild((LayoutDrawingComponent)child);
				}
			}
		}

		public override void LayoutChild(LayoutDrawingComponent child)
		{
			var clientRect = this.Component.ClientBounds;

			child.Layout(this.Component.ClientBounds);

			RGFloat x = child.X, y = child.Y;

			switch (child.LayoutHorizontalAlignment)
			{
				case HorizontalAlignment.Left:
					child.X = clientRect.X;
					break;

				default:
				case HorizontalAlignment.Center:
					child.X = (clientRect.Width - child.Width) / 2;
					break;

				case HorizontalAlignment.Right:
					child.X = clientRect.Width - child.Width;
					break;
			}

			switch (child.LayoutVerticalAlignment)
			{
				case VerticalAlignment.Top:
					child.Y = clientRect.Y;
					break;

				default:
				case VerticalAlignment.Middle:
					child.Y = clientRect.Y + (clientRect.Height - child.Height) / 2;
					break;

				case VerticalAlignment.Bottom:
					child.Y = (clientRect.Bottom - child.Height);
					break;
			}
		}
	}
	*/
}

#endif // DRAWING