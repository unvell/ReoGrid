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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

#if WINFORM
using System.Windows.Forms;
using RGFloat = System.Single;
using RGImage = System.Drawing.Image;
#else
using RGFloat = System.Double;
using RGImage = System.Windows.Media.ImageSource;
#endif // WINFORM

using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.CellTypes
{
	/// <summary>
	/// Representation for an image of cell body
	/// </summary>
	public class ImageCell : CellBody
	{
		/// <summary>
		/// Get or set the image to be displayed in cell
		/// </summary>
		public RGImage Image { get; set; }

		#region Constructor
		/// <summary>
		/// Create image cell object.
		/// </summary>
		public ImageCell() { }

		/// <summary>
		/// Construct image cell-body to show a specified image
		/// </summary>
		/// <param name="image">Image to be displayed</param>
		public ImageCell(RGImage image)
			: this(image, default(ImageCellViewMode))
		{
		}

		/// <summary>
		/// Construct image cell-body to show a image by specified display-method
		/// </summary>
		/// <param name="image">Image to be displayed</param>
		/// <param name="viewMode">View mode decides how to display a image inside a cell</param>
		public ImageCell(RGImage image, ImageCellViewMode viewMode)
		{
			this.Image = image;
			this.viewMode = viewMode;
		}
		#endregion // Constructor

		#region ViewMode
		protected ImageCellViewMode viewMode;

		/// <summary>
		/// Set or get the view mode of this image cell
		/// </summary>
		public ImageCellViewMode ViewMode
		{
			get
			{
				return this.viewMode;
			}
			set
			{
				if (this.viewMode != value)
				{
					this.viewMode = value;

					if (base.Cell != null && base.Cell.Worksheet != null)
					{
						base.Cell.Worksheet.RequestInvalidate();
					}
				}
			}
		}
		#endregion // ViewMode

		#region OnPaint
		/// <summary>
		/// Render the image cell body.
		/// </summary>
		/// <param name="dc">Platform no-associated drawing context instance.</param>
		public override void OnPaint(CellDrawingContext dc)
		{
			if (Image != null)
			{
				RGFloat x = Bounds.X, y = Bounds.Y, width = 0, height = 0;
				bool needClip = false;

				switch (this.viewMode)
				{
					default:
					case ImageCellViewMode.Stretch:
						width = Bounds.Width;
						height = Bounds.Height;
						break;

					case ImageCellViewMode.Zoom:
						RGFloat widthRatio = (RGFloat)Bounds.Width / Image.Width;
						RGFloat heightRatio = (RGFloat)Bounds.Height / Image.Height;
						RGFloat minRatio = Math.Min(widthRatio, heightRatio);
						width = minRatio * Image.Width;
						height = minRatio * Image.Height;
						break;

					case ImageCellViewMode.Clip:
						width = Image.Width;
						height = Image.Height;

						if (width > Bounds.Width || height > Bounds.Height) needClip = true;
						break;
				}

				switch (Cell.Style.HAlign)
				{
					default:
					case ReoGridHorAlign.Left:
						x = Bounds.X;
						break;

					case ReoGridHorAlign.Center:
						x = (Bounds.Width - width) / 2;
						break;

					case ReoGridHorAlign.Right:
						x = Bounds.Width - width;
						break;
				}

				switch (Cell.Style.VAlign)
				{
					default:
					case ReoGridVerAlign.Top:
						y = Bounds.Y;
						break;

					case ReoGridVerAlign.Middle:
						y = (Bounds.Height - height) / 2;
						break;

					case ReoGridVerAlign.Bottom:
						y = Bounds.Height - height;
						break;
				}

				var g = dc.Graphics;

				if (needClip)
				{
					g.PushClip(Bounds);
				}

				g.DrawImage(Image, x, y, width, height);

				if (needClip)
				{
					g.PopClip();
				}
			}

			dc.DrawCellText();
		}
		#endregion // OnPaint

		public override ICellBody Clone()
		{
			return new ImageCell(this.Image);
		}
	}

	#region ImageCellViewMode
	/// <summary>
	/// Image dispaly method in ImageCell-body
	/// </summary>
	public enum ImageCellViewMode
	{
		/// <summary>
		/// Fill to cell boundary. (default)
		/// </summary>
		Stretch,

		/// <summary>
		/// Lock aspect ratio to fit cell boundary.
		/// </summary>
		Zoom,

		/// <summary>
		/// Keep original image size and clip to fill the cell.
		/// </summary>
		Clip,
	}
	#endregion // ImageCellViewMode
}
