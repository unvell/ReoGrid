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

namespace unvell.ReoGrid.Drawing.Shapes
{
	/// <summary>
	/// Represents an abstract shape drawing object.
	/// </summary>
	public abstract class ShapeObject : DrawingObject
	{
		#region Text
		private string text;

		/// <summary>
		/// Get or set the text displayed inside this shape.
		/// </summary>
		public string Text
		{
			get
			{
				if (this.richText != null)
				{
					return this.richText.ToPlainText();
				}
				else
				{
					return this.text;
				}
			}
			set
			{
				if (this.text != value)
				{
					this.text = value;
					this.Invalidate();
				}

				this.richText = null;
			}
		}

		private RichText richText;

		/// <summary>
		/// Get or set rich format text.
		/// </summary>
		public RichText RichText
		{
			get { return this.richText; }
			set
			{
				this.richText = value;
				this.Invalidate();
			}
		}
		#endregion // Text

		#region Style Attributes
		internal SolidColor TextColor;

		internal HorizontalAlignment HorizontalAlignment { get; set; }

		internal VerticalAlignment VerticalAlignment { get; set; }

		private DrawingShapeObjectStyle styleProxy = null;
		
		/// <summary>
		/// Get style object.
		/// </summary>
		public new IDrawingShapeObjectStyle Style
		{
			get
			{
				if (this.styleProxy == null)
				{
					this.styleProxy = new DrawingShapeObjectStyle(this);
				}

				return this.styleProxy;
			}
		}
		#endregion // Style Attributes

		#region TextBounds
		/// <summary>
		/// Get the text bounds for display text inside this shape.
		/// </summary>
		protected virtual Rectangle TextBounds
		{
			get
			{
				var rect = this.ClientBounds;
				rect.Inflate(-5, -5);
				return rect;
			}
		}
		#endregion // TextBounds

		#region Paint
		/// <summary>
		/// Render shape object to graphics context.
		/// </summary>
		/// <param name="dc">Platform no-associated drawing context instance.</param>
		protected override void OnPaint(DrawingContext dc)
		{
			base.OnPaint(dc);
			this.OnPaintText(dc);
		}

		/// <summary>
		/// Paint text inside this shape.
		/// </summary>
		/// <param name="dc">Instance of cross-platform drawing context.</param>
		protected virtual void OnPaintText(DrawingContext dc)
		{
			if (this.richText != null)
			{
				//var rect = new Rectangle(this.ClientBounds.X, 0, this.ClientBounds.Width, this.Height);
				this.richText.Draw(dc.Graphics, this.ClientBounds);
			}
			else
			{

			if (!string.IsNullOrEmpty(this.text))
			{
				dc.Graphics.DrawText(this.text, this.FontName, this.FontSize, this.ForeColor, this.TextBounds,
					 ReoGridHorAlign.Center, ReoGridVerAlign.Middle);
			}

			}
		}
		#endregion // Paint

		public override void OnBoundsChanged(Rectangle oldRect)
		{
			base.OnBoundsChanged(oldRect);

			if (this.richText != null)
			{
				this.richText.Size = this.Bounds.Size;
			}
		}
	}

	/// <summary>
	/// Interface of drawing shape object.
	/// </summary>
	public interface IDrawingShapeObjectStyle : IDrawingObjectStyle
	{
		/// <summary>
		/// Get or set the color of text.
		/// </summary>
		SolidColor TextColor { get; set; }

		/// <summary>
		/// Get or set the horizontal alignment for text.
		/// </summary>
		HorizontalAlignment HorizontalAlignment { get; set; }

		/// <summary>
		/// Get or set the vertical alignment for text.
		/// </summary>
		VerticalAlignment VerticalAlignment { get; set; }
	}

	/// <summary>
	/// Represents style object for shape object.
	/// </summary>
	public class DrawingShapeObjectStyle : DrawingObjectStyle, IDrawingShapeObjectStyle
	{
		/// <summary>
		/// Get the instance of owner drawing object.
		/// </summary>
		public Shapes.ShapeObject ShapeObject { get; private set; }

		#region Style Attributes
		/// <summary>
		/// Get or set the text color.
		/// </summary>
		public SolidColor TextColor
		{
			get
			{
				ValidateReferenceOwner();

				return this.ShapeObject.TextColor;
			}
			set
			{
				ValidateReferenceOwner();

				this.ShapeObject.TextColor = value;
				this.ShapeObject.Invalidate();
			}
		}

		public HorizontalAlignment HorizontalAlignment
		{
			get
			{
				ValidateReferenceOwner();

				return this.ShapeObject.HorizontalAlignment;
			}
			set
			{
				ValidateReferenceOwner();

				this.ShapeObject.HorizontalAlignment = value;
				this.ShapeObject.Invalidate();
			}
		}

		public VerticalAlignment VerticalAlignment
		{
			get
			{
				ValidateReferenceOwner();

				return this.ShapeObject.VerticalAlignment;
			}
			set
			{
				ValidateReferenceOwner();

				this.ShapeObject.VerticalAlignment = value;
				this.ShapeObject.Invalidate();
			}
		}
		#endregion // Style Attributes

		internal DrawingShapeObjectStyle(Shapes.ShapeObject owner)
			: base(owner)
		{
			this.ShapeObject = owner;
		}

	}
}

#endif // DRAWING