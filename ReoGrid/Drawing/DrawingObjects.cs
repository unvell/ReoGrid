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
using System.Collections.Generic;

#if WINFORM || ANDROID
using RGFloat = System.Single;
#elif WPF
using RGFloat = System.Double;
#endif // WINFORM

using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Interaction;

namespace unvell.ReoGrid.Drawing
{
	/// <summary>
	/// Represents basic drawing object.
	/// </summary>
	[Serializable]
	public abstract class FloatingObject : IFloatingObject
	{
		/// <summary>
		/// Create drawing object instance.
		/// </summary>
		public FloatingObject()
		{
			var size = GetPreferredSize();

			this.Size = size;
			this.innerWidth = size.Width;
			this.innerHeight = size.Height;
		}

		#region Attitudes

		internal protected RGFloat innerX = 0;
		internal protected RGFloat innerY = 0;
		internal protected RGFloat innerWidth = 160;
		internal protected RGFloat innerHeight = 100;

		#region Bounds

		/// <summary>
		/// Get or set X position.
		/// </summary>
		public RGFloat X
		{
			get { return this.innerX; }
			set
			{
				if (this.innerX != value)
				{
					Rectangle oldBounds = this.Bounds;
					this.innerX = value;
					this.InternalBoundsUpdate(oldBounds);
				}
			}
		}

		/// <summary>
		/// Get or set Y position.
		/// </summary>
		public RGFloat Y
		{
			get { return this.innerY; }
			set
			{
				if (this.innerY != value)
				{
					Rectangle oldBounds = this.Bounds;
					this.innerY = value;
					this.InternalBoundsUpdate(oldBounds);
				}
			}
		}

		/// <summary>
		/// Get or set object position.
		/// </summary>
		public Point Location
		{
			get { return new Point(this.innerX, this.innerY); }
			set
			{
				if (this.innerX != value.X || this.innerY != value.Y)
				{
					Rectangle oldBounds = this.Bounds;
					this.innerX = value.X;
					this.innerY = value.Y;
					this.InternalBoundsUpdate(oldBounds);
				}
			}
		}

		/// <summary>
		/// Get or set width.
		/// </summary>
		public RGFloat Width
		{
			get { return this.innerWidth; }
			set
			{
				if (this.innerWidth != value)
				{
					Rectangle oldBounds = this.Bounds;
					this.innerWidth = value;
					this.InternalBoundsUpdate(oldBounds);
				}
			}
		}

		/// <summary>
		/// Get or set height.
		/// </summary>
		public RGFloat Height
		{
			get { return this.innerHeight; }
			set
			{
				if (this.innerHeight != value)
				{
					Rectangle oldBounds = this.Bounds;
					this.innerHeight = value;
					this.InternalBoundsUpdate(oldBounds);
				}
			}
		}

		/// <summary>
		/// Get left position. (x-coordinate)
		/// </summary>
		public RGFloat Left { get { return this.innerX; } }

		/// <summary>
		/// Get top position. (y-coordinate)
		/// </summary>
		public RGFloat Top { get { return this.innerY; } }

		/// <summary>
		/// Get right position. (x-coordinate)
		/// </summary>
		public RGFloat Right { get { return this.innerX + this.innerWidth; } }

		/// <summary>
		/// Get bottom position. (y-coordinate)
		/// </summary>
		public RGFloat Bottom { get { return this.innerY + this.innerHeight; } }

		/// <summary>
		/// Get or set size.
		/// </summary>
		public Size Size
		{
			get { return new Size(this.innerWidth, this.innerHeight); }
			set
			{
				Rectangle oldBounds = this.Bounds;

				this.innerWidth = value.Width;
				this.innerHeight = value.Height;

				this.InternalBoundsUpdate(oldBounds);
			}
		}

		/// <summary>
		/// Get bounds position relative to object container.
		/// </summary>
		public Rectangle Bounds
		{
			get
			{
				return new Rectangle(this.innerX, this.innerY, this.innerWidth, this.innerHeight);
			}
			set
			{
				Rectangle oldBounds = this.Bounds;

				this.innerX = value.X;
				this.innerY = value.Y;
				this.innerWidth = value.Width;
				this.innerHeight = value.Height;

				this.InternalBoundsUpdate(oldBounds);
			}
		}

		#endregion // Bounds

		#endregion // Attitudes

		#region Events

		internal protected virtual void InternalBoundsUpdate(Rectangle oldBounds)
		{
			this.OnBoundsChanged(oldBounds);
		}

		/// <summary>
		/// This method will be invoked when bounds of drawing object is changed.
		/// </summary>
		/// <param name="oldRect"></param>
		public virtual void OnBoundsChanged(Rectangle oldRect) { }

		/// <summary>
		/// Get preferred size of drawing object. (Default is 160x100)
		/// </summary>
		/// <returns></returns>
		public virtual Size GetPreferredSize() { return new Size(160, 100); }

		/// <summary>
		/// Event raised when mouse pressed down inside this drawing object.
		/// </summary>
		public event EventHandler<MouseEventArgs> MouseDown;

		#endregion // Events

		#region UI Handle

		/// <summary>
		/// This method will be invoked when mouse button pressed down inside drawing object.
		/// </summary>
		/// <param name="location">Location relateved to drawing object.</param>
		/// <param name="buttons">Mouse button pressing status.</param>
		/// <returns>True if event has been handled; otherwise return false.</returns>
		public virtual bool OnMouseDown(Point location, MouseButtons buttons)
		{
			if (this.MouseDown != null)
			{
				this.MouseDown(this, new MouseEventArgs(location, buttons));
			}

			//this.IsSelection = true;

			//SetFocus();

			return true;
		}

		/// <summary>
		/// This method will be invoked when mouse moving inside drawing object.
		/// </summary>
		/// <param name="location">Location relateved to drawing object.</param>
		/// <param name="buttons">Mouse button pressing status.</param>
		/// <returns>True if event has been handled; otherwise return false.</returns>
		public virtual bool OnMouseMove(Point location, MouseButtons buttons) { return false; }

		/// <summary>
		/// This method will be invoked when mouse button released inside drawing object.
		/// </summary>
		/// <param name="location">Location relateved to drawing object.</param>
		/// <param name="buttons">Mouse button pressing status.</param>
		/// <returns>True if event has been handled; otherwise return false.</returns>
		public virtual bool OnMouseUp(Point location, MouseButtons buttons) { return false; }

		/// <summary>
		/// This method will be invoked when mouse button double clicked inside drawing object.
		/// </summary>
		/// <param name="location">Location relateved to drawing object.</param>
		/// <param name="buttons">Mouse button pressing status.</param>
		/// <returns>True if event has been handled; otherwise return false.</returns>
		public virtual bool OnMouseDoubleClick(Point location, MouseButtons buttons) { return false; }

		/// <summary>
		/// This method will be invoked when keyboard is pressed when drawing object is focused.
		/// </summary>
		/// <param name="keys">Virtual keyboard code to determine that which key is being pressed by user.</param>
		/// <returns>True if event has been handled; otherwise return false.</returns>
		public virtual bool OnKeyDown(KeyCode keys) { return false; }

		/// <summary>
		/// Make drawing object getting focus.
		/// </summary>
		public virtual void SetFocus() { }

		/// <summary>
		/// Make drawing object losing focus.
		/// </summary>
		public virtual void FreeFocus() { }

		#endregion // UI Handle

	}

	/// <summary>
	/// Represents user intervatable floating object.
	/// </summary>
	[Serializable]
	public abstract class SelectableFloatingObject : FloatingObject, IUserVisual, ISelectableVisual
	{
		/// <summary>
		/// Redraw this object.
		/// </summary>
		public abstract void Invalidate();

		#region Selection

		private bool isSelected;

		public event EventHandler SelectionChanged;

		/// <summary>
		/// Determine whether or not this drawing object is being selected.
		/// </summary>
		public virtual bool IsSelected
		{
			get { return this.isSelected; }
			set
			{
				if (this.isSelected != value)
				{
					this.isSelected = true;

					if (this.isSelected)
					{
						OnSelect();
					}
					else
					{
						OnDeselect();
					}

					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// This method will be invoked when drawing object is selected by user.
		/// </summary>
		public virtual void OnSelect()
		{
			if (this.SelectionChanged != null)
			{
				this.SelectionChanged(this, null);
			}
		}

		/// <summary>
		/// This method will be invoked when drawing object is no longer selected by user.
		/// </summary>
		public virtual void OnDeselect()
		{
			if (this.SelectionChanged != null)
			{
				this.SelectionChanged(this, null);
			}
		}

		#endregion // Selection

	}

	/// <summary>
	/// Represents drawing object.
	/// </summary>
	[Serializable]
	public class DrawingObject : SelectableFloatingObject, IDrawingObject, IThumbVisualObject 
	{
		#region Constructor

		/// <summary>
		/// Create drawing object instance.
		/// </summary>
		public DrawingObject()
		{
			this.clientBounds = this.Bounds;

			this.ScaleX = 1;
			this.ScaleY = 1;

			this.ForeColor = SolidColor.Black;
			this.LineColor = SolidColor.Black;
			this.FillColor = SolidColor.White;

			this.FontName = "Arial";
			this.FontSize = 8.25F;

			this.LineWidth = 1.0F;
		}

		#endregion // Constructor

		#region Container
		/// <summary>
		/// Get or set the container of this drawing object.
		/// </summary>
		public IDrawingContainer Container { get; set; }

		/// <summary>
		/// Invalidate the drawing object on rendering device.
		/// </summary>
		public override void Invalidate()
		{
			if (this.Container != null)
			{
				this.Container.Invalidate();
			}
		}
		#endregion // Container

		#region Attitudes

		#region Bounds

		private Rectangle clientBounds;

		/// <summary>
		/// Get client bounds position.
		/// </summary>
		public virtual Rectangle ClientBounds { get { return this.clientBounds; } }

		#endregion // Bounds

		private RGFloat halfWidth;
		private RGFloat halfHeight;

		/// <summary>
		/// Get the origin point of this object.
		/// </summary>
		public virtual Point OriginPoint { get; protected set; }

		//internal short InnerZOrder { get; set; }
		//public short ZOrder { get { return this.InnerZOrder; } set { this.InnerZOrder = value; } }

		/// <summary>
		/// Get or set the horizontal display scaling.
		/// </summary>
		public virtual RGFloat ScaleX { get; set; }

		/// <summary>
		/// Get or set the vertial display sacling.
		/// </summary>
		public virtual RGFloat ScaleY { get; set; }

		/// <summary>
		/// Get or set object rotation angle
		/// </summary>
		public virtual RGFloat RotateAngle { get; set; }

		private bool visible = true;

		/// <summary>
		/// Get or set visibility of this object.
		/// </summary>
		public bool Visible
		{
			get { return this.visible; }
			set
			{
				if (this.visible != value)
				{
					this.visible = value;
					this.Invalidate();
				}
			}
		}

		#endregion // Attitudes

		#region Styles

		internal PaddingValue Padding { get; set; }

		internal IColor FillColor { get; set; }
		internal SolidColor ForeColor { get; set; }
		internal SolidColor LineColor { get; set; }

		internal RGFloat LineWidth { get; set; }
		internal LineStyles LineStyle { get; set; }

		internal virtual string FontName { get; set; }
		internal virtual RGFloat FontSize { get; set; }
		internal virtual Text.FontStyles FontStyles { get; set; }

		private DrawingObjectStyle styleProxy = null;
		
		/// <summary>
		/// Get style object.
		/// </summary>
		public IDrawingObjectStyle Style
		{
			get
			{
				if (this.styleProxy == null)
				{
					this.styleProxy = new DrawingObjectStyle(this);
				}

				return this.styleProxy;
			}
		}

		#endregion // Styles

		#region Draw
		/// <summary>
		/// Render this drawing object.
		/// </summary>
		/// <param name="dc">Platform no-associated drawing context instance.</param>
		public virtual void Draw(DrawingContext dc)
		{
			var clientRect = this.ClientBounds;

			//if (clientRect.Width > 0 && clientRect.Height > 0)
			//{
				var g = dc.Graphics;

				if (this.innerX != 0 || this.innerY != 0 
					|| this.ScaleX != 1 || this.ScaleY != 1
					|| this.RotateAngle != 0)
				{
					g.PushTransform();

					if (this.ScaleX != 1 || this.ScaleY != 1)
					{
						g.ScaleTransform(this.ScaleX, this.ScaleY);
					}

					if (this.RotateAngle != 0)
					{
						g.TranslateTransform(this.innerX + this.halfWidth, this.innerY + this.halfHeight);
						g.RotateTransform(this.RotateAngle);
						g.TranslateTransform(-this.halfWidth, -this.halfHeight);
					}
					else
					{
						g.TranslateTransform(this.innerX, this.innerY);
					}

					OnPaint(dc);

					g.PopTransform();
				}
				else
				{
					OnPaint(dc);
				}
			//}

			if (this.IsSelected)
			{
				DrawSelection(dc);
			}
		}

		/// <summary>
		/// Render drawing object to graphics context.
		/// </summary>
		/// <param name="dc">Platform no-associated drawing context instance.</param>
		protected virtual void OnPaint(DrawingContext dc)
		{
			var g = dc.Graphics;

			var bounds = new Rectangle(0, 0, this.Width, this.Height);

			if (!this.FillColor.IsTransparent && !this.LineColor.IsTransparent)
			{
				g.DrawAndFillRectangle(bounds, this.LineColor, this.FillColor, this.LineWidth, this.LineStyle);
			}
			else if (!this.FillColor.IsTransparent)
			{
				g.FillRectangle(bounds, this.FillColor);
			}
			else if (this.LineColor.A > 0)
			{
				g.DrawRectangle(bounds, this.LineColor, this.LineWidth, this.LineStyle);
			}
		}

		/// <summary>
		/// Render when object is selected.
		/// </summary>
		/// <param name="dc">Platform no-associated drawing context instance</param>
		protected virtual void DrawSelection(DrawingContext dc)
		{
			var g = dc.Graphics;

			g.DrawRectangle(Bounds, SolidColor.DeepSkyBlue);

			if (this.resizePoints != null && this.resizePoints.Length > 0)
			{
				foreach (var ap in this.resizePoints)
				{
					var pt = ap.Point;
					g.DrawEllipse(SolidColor.SeaGreen, pt.X - 2, pt.Y - 2, 4, 4);
				}
			}
		}
		#endregion // Draw

		#region Events
		
		internal protected override void InternalBoundsUpdate(Rectangle oldBounds)
		{
			var rect = new Rectangle(this.Padding.Left, this.Padding.Top,
					this.innerWidth - this.Padding.Left - this.Padding.Right,
					this.innerHeight - this.Padding.Top - this.Padding.Bottom);

			if (rect.Width < 0) rect.Width = 0;
			if (rect.Height < 0) rect.Height = 0;

			this.clientBounds = rect;

			this.halfWidth = this.innerWidth / 2;
			this.halfHeight = this.innerHeight / 2;
			this.OriginPoint = new Point(halfWidth, halfHeight);

			base.InternalBoundsUpdate(oldBounds);
		}

		///// <summary>
		///// This method will be invoked when bounds of drawing object is changed.
		///// </summary>
		///// <param name="oldRect"></param>
		//public virtual void OnBoundsChanged(Rectangle oldRect) { }

		///// <summary>
		///// Get preferred size of drawing object. (Default is 100x100)
		///// </summary>
		///// <returns></returns>
		//public virtual Size GetPreferredSize() { return new Size(160, 100); }

		///// <summary>
		///// Event raised when mouse pressed down inside this drawing object.
		///// </summary>
		//public event EventHandler<MouseEventArgs> MouseDown;

		#endregion // Events

		#region UI Handle

		///// <summary>
		///// This method will be invoked when mouse button pressed down inside drawing object.
		///// </summary>
		///// <param name="location">Location relateved to drawing object.</param>
		///// <param name="buttons">Mouse button pressing status.</param>
		///// <returns>True if event has been handled; otherwise return false.</returns>
		//public virtual bool OnMouseDown(Point location, MouseButtons buttons)
		//{
		//	if (this.MouseDown != null)
		//	{
		//		this.MouseDown(this, new MouseEventArgs(location, buttons));
		//	}

		//	//this.IsSelection = true;

		//	//SetFocus();

		//	return true;
		//}

		///// <summary>
		///// This method will be invoked when mouse moving inside drawing object.
		///// </summary>
		///// <param name="location">Location relateved to drawing object.</param>
		///// <param name="buttons">Mouse button pressing status.</param>
		///// <returns>True if event has been handled; otherwise return false.</returns>
		//public virtual bool OnMouseMove(Point location, MouseButtons buttons) { return false; }

		///// <summary>
		///// This method will be invoked when mouse button released inside drawing object.
		///// </summary>
		///// <param name="location">Location relateved to drawing object.</param>
		///// <param name="buttons">Mouse button pressing status.</param>
		///// <returns>True if event has been handled; otherwise return false.</returns>
		//public virtual bool OnMouseUp(Point location, MouseButtons buttons) { return false; }

		///// <summary>
		///// This method will be invoked when mouse button double clicked inside drawing object.
		///// </summary>
		///// <param name="location">Location relateved to drawing object.</param>
		///// <param name="buttons">Mouse button pressing status.</param>
		///// <returns>True if event has been handled; otherwise return false.</returns>
		//public virtual bool OnMouseDoubleClick(Point location, MouseButtons buttons) { return false; }

		///// <summary>
		///// This method will be invoked when keyboard is pressed when drawing object is focused.
		///// </summary>
		///// <param name="keys">Virtual keyboard code to determine that which key is being pressed by user.</param>
		///// <returns>True if event has been handled; otherwise return false.</returns>
		//public virtual bool OnKeyDown(KeyCode keys) { return false; }

		///// <summary>
		///// Make drawing object getting focus.
		///// </summary>
		//public virtual void SetFocus() { }

		///// <summary>
		///// Make drawing object losing focus.
		///// </summary>
		//public virtual void FreeFocus() { }

		#endregion // UI Handle

		#region Selection

		/// <summary>
		/// This method will be invoked when drawing object is selected by user.
		/// </summary>
		public override void OnSelect()
		{
			this.UpdateResizeThumbPoints();

			base.OnSelect();
		}

		/// <summary>
		/// This method will be invoked when drawing object is no longer selected by user.
		/// </summary>
		public override void OnDeselect()
		{
			base.OnDeselect();
		}

		#endregion // Selection

		#region Thumb

		private ResizeThumb[] resizePoints = new ResizeThumb[8];

		/// <summary>
		/// Get thumb points.
		/// </summary>
		public IEnumerable<ResizeThumb> ThumbPoints
		{
			get { return resizePoints; }
		}

		internal protected void UpdateResizeThumbPoints()
		{
			resizePoints[0] = new ResizeThumb(ResizeThumbPosition.TopLeft, this.innerX, this.innerY);
			resizePoints[1] = new ResizeThumb(ResizeThumbPosition.Top, this.innerX + this.halfWidth, this.innerY);
			resizePoints[2] = new ResizeThumb(ResizeThumbPosition.TopRight, this.Right, this.innerY);
			resizePoints[3] = new ResizeThumb(ResizeThumbPosition.Left, this.innerX, this.innerY + this.halfHeight);
			resizePoints[4] = new ResizeThumb(ResizeThumbPosition.Right, this.Right, this.innerY + this.halfHeight);
			resizePoints[5] = new ResizeThumb(ResizeThumbPosition.BottomLeft, this.innerX, this.Bottom);
			resizePoints[6] = new ResizeThumb(ResizeThumbPosition.Bottom, this.innerX + this.halfWidth, this.Bottom);
			resizePoints[7] = new ResizeThumb(ResizeThumbPosition.BottomRight, this.Right, this.Bottom);
		}
		#endregion // Thumb

	}

	#region DrawingObjectCollection
	/// <summary>
	/// Represents collection of drawing objects.
	/// </summary>
	public class DrawingObjectCollection : IDrawingObjectCollection
	{
		/// <summary>
		/// Get container of drawing collection.
		/// </summary>
		public IDrawingContainer Owner { get; private set; }

		private List<IDrawingObject> list;

		/// <summary>
		/// Create collection of drawing object.
		/// </summary>
		/// <param name="owner">Container instance of collection.</param>
		internal DrawingObjectCollection(DrawingComponent owner)
		{
			this.Owner = owner;
			this.list = owner.drawingObjects;
		}

		/// <summary>
		/// Add drawing object item.
		/// </summary>
		/// <param name="item">Drawing object to be added.</param>
		public virtual void Add(IDrawingObject item)
		{
			item.Container = this.Owner;
			this.list.Add(item);
			this.Owner.OnChildAdded(item, this.list.Count - 1);
		}

		/// <summary>
		/// Get child object by specified index position.
		/// </summary>
		/// <param name="index">Index position in parent object container.</param>
		/// <returns>Child object from parent object container.</returns>
		public virtual IDrawingObject this[int index]
		{
			get { return this.list[index]; }
		}

		/// <summary>
		/// Remove all object from parent object container.
		/// </summary>
		public virtual void Clear()
		{
			foreach (var obj in this.list)
			{
				obj.Container = null;
			}

			this.list.Clear();
			this.Owner.OnChildrenClear();
		}

		/// <summary>
		/// Check whether a specified object is contained by this object container.
		/// </summary>
		/// <param name="item">Object to be checked.</param>
		/// <returns>True if specified object is contained by this object container; Otherwise return false.</returns>
		public bool Contains(IDrawingObject item)
		{
			return this.list.Contains(item);
		}

		public void CopyTo(IDrawingObject[] array, int arrayIndex)
		{
			this.list.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return this.list.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public virtual bool Remove(IDrawingObject item)
		{
			item.Container = null;
			return this.list.Remove(item);
		}

		IEnumerator<IDrawingObject> GetEnum()
		{
			return this.list.GetEnumerator();
		}

		public IEnumerator<IDrawingObject> GetEnumerator()
		{
			return this.GetEnum();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnum();
		}

		public virtual void AddRange(IEnumerable<IDrawingObject> drawingObjects)
		{
			this.list.AddRange(drawingObjects);

			int i = this.list.Count;

			foreach (var obj in drawingObjects)
			{
				obj.Container = this.Owner;

				this.Owner.OnChildAdded(obj, i++);
			}

		}
	}
	#endregion // DrawingObjectCollection

}

#endif // DRAWING