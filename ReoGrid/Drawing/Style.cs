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
using System.Linq;
using System.Text;

#if WINFORM || ANDROID
using RGFloat = System.Single;
#else
using RGFloat = System.Double;
#endif // WINFORM

using unvell.ReoGrid.Graphics;

namespace unvell.ReoGrid.Drawing
{
	/// <summary>
	/// Represents the horizontal alignment for drawing objects.
	/// </summary>
	public enum HorizontalAlignment
	{
		Left,
		Center,
		Right,
	}

	/// <summary>
	/// Represents the vertical alignment for drawing objects.
	/// </summary>
	public enum VerticalAlignment
	{
		Top,
		Middle,
		Bottom,
	}

	/// <summary>
	/// Represents the interface of drawing object style set.
	/// </summary>
	public interface IDrawingObjectStyle
	{
		/// <summary>
		/// Get or set fill background color.
		/// </summary>
		IColor FillColor { get; set; }

		/// <summary>
		/// Get or set line color.
		/// </summary>
		SolidColor LineColor { get; set; }

		/// <summary>
		/// Get or set line width.
		/// </summary>
		RGFloat LineWidth { get; set; }

		/// <summary>
		/// Get or set line style.
		/// </summary>
		LineStyles LineStyle { get; set; }
	}

	/// <summary>
	/// Represents the interface of line object style.
	/// </summary>
	public interface IDrawingLineObjectStyle : IDrawingObjectStyle
	{
		/// <summary>
		/// Get or set the start cap style.
		/// </summary>
		LineCapStyles StartCap { get; set; }

		/// <summary>
		/// Get or set the end cap style.
		/// </summary>
		LineCapStyles EndCap { get; set; }
	}

	/// <summary>
	/// Represents the interface of drawing component style.
	/// </summary>
	public interface IDrawingComponentStyle : IDrawingObjectStyle
	{
		/// <summary>
		/// Get or set padding.
		/// </summary>
		PaddingValue Padding { get; set; }
	}

	///// <summary>
	///// Represents reference style of drawing objects.
	///// </summary>
	//public abstract class DrawingReferenceStyle : IDrawingObjectStyle
	//{
	//	/// <summary>
	//	/// Get the instance of owner drawing object.
	//	/// </summary>
	//	public DrawingObject OwnerObject { get; private set; }

	//	/// <summary>
	//	/// Create instance of reference style by specified owner drawing object.
	//	/// </summary>
	//	/// <param name="owner">Owner drawing object.</param>
	//	protected DrawingReferenceStyle(DrawingObject owner)
	//	{
	//		this.OwnerObject = owner;
	//	}

	//	/// <summary>
	//	/// Validate whether or not the reference to owner drawing object is still valid.
	//	/// </summary>
	//	protected void ValidateReferenceOwner()
	//	{
	//		if (this.OwnerObject == null)
	//		{
	//			throw new ReferenceObjectNotAssociatedException("Drawing object style has not valid owner.");
	//		}
	//	}

	//	/// <summary>
	//	/// Get or set fill background color.
	//	/// </summary>
	//	public abstract IColor FillColor { get; set; }

	//	/// <summary>
	//	/// Get or set line color.
	//	/// </summary>
	//	public abstract SolidColor LineColor { get; set; }

	//	/// <summary>
	//	/// Get or set line width.
	//	/// </summary>
	//	public abstract RGFloat LineWidth { get; set; }

	//	/// <summary>
	//	/// Get or set line style.
	//	/// </summary>
	//	public abstract LineStyles LineStyle { get; set; }

	//	///// <summary>
	//	///// Get or set the color of text.
	//	///// </summary>
	//	//public abstract SolidColor TextColor { get; set; }
	//}

	/// <summary>
	/// Represents the implementation of drawing objects style set.
	/// </summary>
	public class DrawingObjectStyle : IDrawingObjectStyle
	{
		/// <summary>
		/// Get the instance of owner drawing object.
		/// </summary>
		public DrawingObject OwnerObject { get; private set; }

		internal DrawingObjectStyle(DrawingObject owner)
		{
			this.OwnerObject = owner;
		}

		protected void ValidateReferenceOwner()
		{
			if (this.OwnerObject == null)
			{
				throw new ReferenceObjectNotAssociatedException("Drawing object style has not valid owner.");
			}
		}

		#region FillColor
		/// <summary>
		/// Get or set fill background color.
		/// </summary>
		public IColor FillColor
		{
			get
			{
				ValidateReferenceOwner();

				return this.OwnerObject.FillColor;
			}
			set
			{
				ValidateReferenceOwner();

				this.OwnerObject.FillColor = value;
				this.OwnerObject.Invalidate();
			}
		}
		#endregion // FillColor

		#region LineColor
		/// <summary>
		/// Get or set line color.
		/// </summary>
		public SolidColor LineColor
		{
			get
			{
				ValidateReferenceOwner();

				return this.OwnerObject.LineColor;
			}
			set
			{
				ValidateReferenceOwner();

				this.OwnerObject.LineColor = value;
				this.OwnerObject.Invalidate();
			}
		}
		#endregion // LineColor

		#region LineWidth
		/// <summary>
		/// Get or set line width.
		/// </summary>
		public RGFloat LineWidth
		{
			get
			{
				ValidateReferenceOwner();

				return this.OwnerObject.LineWidth;
			}
			set
			{
				ValidateReferenceOwner();

				this.OwnerObject.LineWidth = value;
				this.OwnerObject.Invalidate();
			}
		}
		#endregion // LineWidth

		#region LineStyle
		/// <summary>
		/// Get or set line style.
		/// </summary>
		public LineStyles LineStyle
		{
			get
			{
				ValidateReferenceOwner();

				return this.OwnerObject.LineStyle;
			}
			set
			{
				ValidateReferenceOwner();

				this.OwnerObject.LineStyle = value;
				this.OwnerObject.Invalidate();
			}
		}
		#endregion // LineStyle

	}

	/// <summary>
	/// Represents 
	/// </summary>
	public class DrawingComponentStyle : DrawingObjectStyle, IDrawingComponentStyle
	{
		internal DrawingComponentStyle(DrawingComponent owner)
			: base(owner)
		{
		}

		/// <summary>
		/// Get or set padding.
		/// </summary>
		public PaddingValue Padding
		{
			get
			{
				ValidateReferenceOwner();

				return this.OwnerObject.Padding;
			}
			set
			{
				ValidateReferenceOwner();

				var oldBounds = this.OwnerObject.Bounds;
				this.OwnerObject.Padding = value;
				this.OwnerObject.InternalBoundsUpdate(oldBounds);
				this.OwnerObject.Invalidate();
			}
		}
	}
}

#endif // DRAWING