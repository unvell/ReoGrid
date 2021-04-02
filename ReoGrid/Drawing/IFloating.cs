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
#elif WPF
using RGFloat = System.Double;
#endif // WPF

using unvell.ReoGrid.Graphics;

namespace unvell.ReoGrid.Drawing
{
	/// <summary>
	/// Represents an floating object above worksheet
	/// </summary>
	public interface IFloatingObject
	{
		/// <summary>
		/// Get or set the bounds relative to the container of this floating object.
		/// </summary>
		Rectangle Bounds { get; set; }

		/// <summary>
		/// Get or set the position on X-coordinate.
		/// </summary>
		RGFloat X { get; set; }

		/// <summary>
		/// Get or set the position on Y-coordinate.
		/// </summary>
		RGFloat Y { get; set; }

		/// <summary>
		/// Get or set position on both X and Y coordinates.
		/// </summary>
		Point Location { get; set; }

		/// <summary>
		/// Get left position of bounds relative to the container of this floating object.
		/// </summary>
		RGFloat Left { get; }

		/// <summary>
		/// Get right position of bounds relative to the container of this floating object.
		/// </summary>
		RGFloat Right { get; }

		/// <summary>
		/// Get top position of bounds relative to the container of this floating object.
		/// </summary>
		RGFloat Top { get; }

		/// <summary>
		/// Get bottom position of bounds relative to the container of this floating object.
		/// </summary>
		RGFloat Bottom { get; }

		/// <summary>
		/// Get or set size of this floating object.
		/// </summary>
		Size Size { get; set; }

		/// <summary>
		/// Get and set the width of this floating object.
		/// </summary>
		RGFloat Width { get; set; }

		/// <summary>
		/// Get or set the height of this floating object.
		/// </summary>
		RGFloat Height { get; set; }

		/// <summary>
		/// Method invoked when Bounds property changed.
		/// </summary>
		/// <param name="oldRect">Previous bounds value before changing.</param>
		void OnBoundsChanged(Rectangle oldRect);
	}

	/// <summary>
	/// Collection of floating object
	/// </summary>
	/// <typeparam name="T">Type inherts from <seealso>IFloattingObject</seealso></typeparam>
	public interface IFloatingObjectCollection<T> : ICollection<T> where T : IFloatingObject
	{
		/// <summary>
		/// Access element from collection
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		T this[int index] { get; }

		/// <summary>
		/// Add all elements from another collection
		/// </summary>
		/// <param name="drawingObjects"></param>
		void AddRange(IEnumerable<T> drawingObjects);
	}

}

#endif // DRAWING