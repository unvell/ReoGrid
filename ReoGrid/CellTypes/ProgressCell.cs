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
	#region Progress
	/// <summary>
	/// Representation for a button of cell body
	/// </summary>
	[Serializable]
	public class ProgressCell : CellBody
	{
		/// <summary>
		/// Get or set the top color.
		/// </summary>
		public SolidColor TopColor { get; set; }

		/// <summary>
		/// Get or set the bottom color.
		/// </summary>
		public SolidColor BottomColor { get; set; }

		/// <summary>
		/// Create progress cell body.
		/// </summary>
		public ProgressCell()
		{
			TopColor = SolidColor.LightSkyBlue;
			BottomColor = SolidColor.SkyBlue;
		}

		/// <summary>
		/// Render the progress cell body.
		/// </summary>
		/// <param name="dc"></param>
		public override void OnPaint(CellDrawingContext dc)
		{
			double value = this.Cell.GetData<double>();

			if (value > 0)
			{
				var g = dc.Graphics.PlatformGraphics;

				Rectangle rect = new Rectangle(Bounds.Left, Bounds.Top + 1, (RGFloat)(Bounds.Width * value), Bounds.Height - 1);

				if (rect.Width > 0 && rect.Height > 0)
				{
					dc.Graphics.FillRectangleLinear(TopColor, BottomColor, 90f, rect);
				}
			}
		}

		/// <summary>
		/// Clone a progress bar from this object.
		/// </summary>
		/// <returns>New instance of progress bar.</returns>
		public override ICellBody Clone()
		{
			return new ProgressCell();
		}
	}
	#endregion // Progress

}
