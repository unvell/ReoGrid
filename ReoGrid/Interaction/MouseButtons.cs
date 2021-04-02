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

namespace unvell.ReoGrid.Interaction
{
	/// <summary>
	/// Mouse button down status
	/// </summary>
	public enum MouseButtons
	{
		/// <summary>
		/// None
		/// </summary>
		None = 0,

		/// <summary>
		/// Left
		/// </summary>
		Left = 0x100000,

		/// <summary>
		/// Right
		/// </summary>
		Right = 0x200000,

		/// <summary>
		/// Middle
		/// </summary>
		Middle = 0x400000,
	}
}
