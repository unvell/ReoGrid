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

using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.CellTypes
{
	/// <summary>
	/// Represents an image button cell on worksheet.
	/// </summary>
	[Serializable]
	public class ImageButtonCell : ButtonCell
	{
		/// <summary>
		/// Image that displayed on button.
		/// </summary>
		public RGImage Image { get; set; }

		/// <summary>
		/// Create image button cell without image specified.
		/// </summary>
		public ImageButtonCell()
			: this(null)
		{
		}

		/// <summary>
		/// Create image button cell with specified image.
		/// </summary>
		/// <param name="image"></param>
		public ImageButtonCell(RGImage image)
		{
			this.Image = image;
		}

		/// <summary>
		/// Paint image button cell.
		/// </summary>
		/// <param name="dc">Platform non-associated drawing context.</param>
		public override void OnPaint(CellDrawingContext dc)
		{
			base.OnPaint(dc);

			if (this.Image != null)
			{
				RGFloat widthScale = Math.Min((Bounds.Width - 4) / this.Image.Width, 1);
				RGFloat heightScale = Math.Min((Bounds.Height - 4) / this.Image.Height, 1);

				RGFloat minScale = Math.Min(widthScale, heightScale);
				RGFloat imageScale = (RGFloat)Image.Height / Image.Width;
				RGFloat width = Image.Width * minScale;

				Rectangle r = new Rectangle(0, 0, width, imageScale * width);

				r.X = (Bounds.Width - r.Width) / 2;
				r.Y = (Bounds.Height - r.Height) / 2;

				if (this.IsPressed)
				{
					r.X++;
					r.Y++;
				}

				dc.Graphics.DrawImage(this.Image, r);
			}
		}

		/// <summary>
		/// Clone image button from this object.
		/// </summary>
		/// <returns>New instance of image button.</returns>
		public override ICellBody Clone()
		{
			return new ImageButtonCell(this.Image);
		}
	}
}
