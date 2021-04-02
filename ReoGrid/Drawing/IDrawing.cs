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

using unvell.ReoGrid.Interaction;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.Drawing
{
	/// <summary>
	/// Represents interface of drawing objects.
	/// </summary>
	public interface IDrawingObject : IFloatingObject, IUserVisual
	{
		#region Attitudes
		/// <summary>
		/// Get or set the client bounds of drawing object.
		/// </summary>
		Rectangle ClientBounds { get; }

		/// <summary>
		/// Get or set horizontal scaling of drawing object.
		/// </summary>
		RGFloat ScaleX { get; set; }

		/// <summary>
		/// Get or set vertical scaling of drawing object.
		/// </summary>
		RGFloat ScaleY { get; set; }

		/// <summary>
		/// Get or set the rotation angle of drawing object.
		/// </summary>
		RGFloat RotateAngle { get; set; }
		#endregion // Attitudes

		/// <summary>
		/// Get or set the container of drawing object.
		/// </summary>
		IDrawingContainer Container { get; set; }

		/// <summary>
		/// Access the style set of drawing object.
		/// </summary>
		IDrawingObjectStyle Style { get; }

		/// <summary>
		/// Render this drawing object.
		/// </summary>
		/// <param name="dc">Platform no-associated drawing context instance.</param>
		void Draw(DrawingContext dc);
	}

	/// <summary>
	/// Represents collection of drawing object.
	/// </summary>
	public interface IDrawingObjectCollection : IFloatingObjectCollection<IDrawingObject>
	{
	}
}

#endif // DRAWING