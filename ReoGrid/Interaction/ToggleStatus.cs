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
	/// Represent for the button status of mouse
	/// </summary>
	[Flags]
	public enum ToggleStatus
	{
		/// <summary>
		/// The button has its normal appearance (three-dimensional).
		/// </summary>
		Normal = 0,

		/// <summary>
		/// The button is inactive (grayed).
		/// </summary>
		Inactive = 0x100,

		/// <summary>
		/// The button appears pressed.
		/// </summary>
		Pushed = 0x200,

		/// <summary>
		/// The button has a checked or latched appearance. Use this appearance to show
		/// that a toggle button has been pressed.
		/// </summary>
		Checked = 0x400,

		/// <summary>
		/// The button has a flat, two-dimensional appearance.
		/// </summary>
		Flat = 0x4000,

		/// <summary>
		/// All flags except Normal are set.
		/// </summary>
		All = 0x4700,
	}
}
