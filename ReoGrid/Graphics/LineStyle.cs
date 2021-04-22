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

namespace unvell.ReoGrid.Graphics
{
	/// <summary>
	/// Represents line styles.
	/// </summary>
	public enum LineStyles : byte
	{
		/// <summary>
		/// Solid
		/// </summary>
		Solid = 1,

		/// <summary>
		/// Dashed
		/// </summary>
		Dash = 2,

		/// <summary>
		/// Dotted
		/// </summary>
		Dot = 3,

		/// <summary>
		/// Dashed dot
		/// </summary>
		DashDot = 4,

		/// <summary>
		/// Dashed double dot
		/// </summary>
		DashDotDot = 5,
	}

	/// <summary>
	/// Represents line cap styles.
	/// </summary>
	public enum LineCapStyles : byte
	{
		/// <summary>
		/// None
		/// </summary>
		None = 0,

		/// <summary>
		/// Arrow
		/// </summary>
		Arrow = 1,

		/// <summary>
		/// Ellipse
		/// </summary>
		Ellipse = 2,

		/// <summary>
		/// Round
		/// </summary>
		Round = 3,
	}
}
