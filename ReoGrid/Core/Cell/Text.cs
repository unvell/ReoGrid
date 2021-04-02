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
using System.Linq;
using System.Text;
using unvell.ReoGrid.Core;
using unvell.ReoGrid.Graphics;

#if WINFORM || ANDROID
using RGFloat = System.Single;
#elif WPF
using RGFloat = System.Double;
#elif iOS
using RGFloat = System.Double;
#endif // WPF

namespace unvell.ReoGrid
{
	partial class Worksheet
	{
	}

	partial class Cell
	{
		internal bool FontDirty { get; set; }

		private Rectangle textBounds;

		/// <summary>
		/// text boundary for display
		/// </summary>
		internal Rectangle TextBounds { get { return this.textBounds; } set { this.textBounds = value; } }

		internal RGFloat TextBoundsTop { get { return textBounds.Y; } set { this.textBounds.Y = value; } }
		internal RGFloat TextBoundsLeft { get { return textBounds.X; } set { this.textBounds.X = value; } }
		internal RGFloat TextBoundsWidth { get { return textBounds.Width; } set { this.textBounds.Width = value; } }
		internal RGFloat TextBoundsHeight { get { return textBounds.Height; } set { this.textBounds.Height = value; } }

		internal Rectangle PrintTextBounds { get; set; }

		/// <summary>
		/// Horizontal alignement for display
		/// </summary>
		internal ReoGridRenderHorAlign RenderHorAlign { get; set; }

		/// <summary>
		/// Column span if text larger than the cell it inside
		/// </summary>
		internal short RenderTextColumnSpan { get; set; }

		//private SolidColor renderColor = null;
		/// <summary>
		/// Get the render color of cell text. Render color is the final color that used to render the text on the worksheet.
		/// Whatever cell style with text color is specified, negative numbers may displayed as red.
		/// This property cannot be changed directly. 
		/// To change text color, set cell style with text color by call SetRangeStyles method, or change the Cell.Style.TextColor property.
		/// </summary>
		public SolidColor RenderColor { get; internal set; }

		//public SolidColor? GetRenderColor() { return renderColor; }

		internal RGFloat RenderScaleFactor { get; set; }

	}
}
