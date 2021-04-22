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

namespace unvell.ReoGrid.Interaction
{
	/// <summary>
	/// Cursor style 
	/// </summary>
	public enum CursorStyle : byte
	{
		/// <summary>
		/// Default (Auto)
		/// </summary>
		PlatformDefault,

		/// <summary>
		/// Hand
		/// </summary>
		Hand,

		/// <summary>
		/// Range Selection
		/// </summary>
		Selection,

		/// <summary>
		/// Full Row Selector
		/// </summary>
		FullRowSelect,

		/// <summary>
		/// Full Column Selector
		/// </summary>
		FullColumnSelect,

		/// <summary>
		/// Entire worksheet Selector
		/// </summary>
		EntireSheet,

		/// <summary>
		/// Move object
		/// </summary>
		Move,

		/// <summary>
		/// Copy object
		/// </summary>
		Copy,

		/// <summary>
		/// Change Column Width
		/// </summary>
		ChangeColumnWidth,

		/// <summary>
		/// Change Row Height
		/// </summary>
		ChangeRowHeight,

		/// <summary>
		/// Horizontal Resize
		/// </summary>
		ResizeHorizontal,

		/// <summary>
		/// Vertical Resize
		/// </summary>
		ResizeVertical,

		/// <summary>
		/// Busy (Waiting)
		/// </summary>
		Busy,

		/// <summary>
		/// Cross Cursor
		/// </summary>
		Cross,
	}
}
