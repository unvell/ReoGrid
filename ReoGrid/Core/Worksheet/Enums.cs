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

namespace unvell.ReoGrid
{
	/// <summary>
	/// Reason for ending of cell edit
	/// </summary>
	public enum EndEditReason
	{
		/// <summary>
		/// User edit has done normally
		/// </summary>
		NormalFinish,

		/// <summary>
		/// User has cancelled edit operation
		/// </summary>
		Cancel,
	}

	/// <summary>
	/// Represents selection mode for worksheet.
	/// </summary>
	public enum WorksheetSelectionMode
	{
		/// <summary>
		/// Do not allow to select anything on worksheet.
		/// </summary>
		None,

		/// <summary>
		/// Only allow to select single cell.
		/// </summary>
		Cell,

		/// <summary>
		/// Allow to select cell or ranges. (Default)
		/// </summary>
		Range,

		/// <summary>
		/// Always to select one or more entire rows at a time.
		/// </summary>
		Row,

		/// <summary>
		/// Always to select one or more entire columns at a time.
		/// </summary>
		Column,

		/// <summary>
		/// Allow to select only one row. (Reserved)
		/// </summary>
		SingleRow,

		/// <summary>
		/// Allow to select only one column. (Reserved)
		/// </summary>
		SingleColumn,
	}

	/// <summary>
	/// Represents selection style for worksheet.
	/// </summary>
	public enum WorksheetSelectionStyle
	{
		/// <summary>
		/// No selection will be drawn.
		/// </summary>
		None,

		/// <summary>
		/// Default selection style.
		/// </summary>
		Default,

		/// <summary>
		/// Windows classic focus rectangle style.
		/// </summary>
		FocusRect,
	}

	/// <summary>
	/// Selection Forward Direction for worksheet. When user finished cell edit,
	/// or Enter key is pressed on worksheet, the focus cell moves to next cell at right column.
	/// By changing this direction to change the moving direction of focus cell.
	/// </summary>
	public enum SelectionForwardDirection
	{
		/// <summary>
		/// Move to cell at right column.
		/// </summary>
		Right,

		/// <summary>
		/// Move to cell at below row.
		/// </summary>
		Down,
	}

	/// <summary>
	/// Determine the style to show focus cell
	/// </summary>
	public enum FocusPosStyle
	{
		/// <summary>
		/// Default style (cell with no background filled)
		/// </summary>
		Default,

		/// <summary>
		/// Nothing specical on focus cell
		/// </summary>
		None,
	}

	/// <summary>
	/// Behavior for spreadsheet operations
	/// </summary>
	internal enum OperationStatus
	{
		/// <summary>
		/// Change Selection Range, Edit cell, Move focus cell by keyboard and etc.
		/// </summary>
		Default,

		/// <summary>
		/// Selecting focus range by dragging mouse.
		/// </summary>
		RangeSelect,
		FullRowSelect,
		FullColumnSelect,
		FullSingleRowSelect,
		FullSingleColumnSelect,

		/// <summary>
		/// Adjust row height or column width.
		/// </summary>
		AdjustRowHeight,
		AdjustColumnWidth,

		/// <summary>
		/// Allow drag selection to fill serial data.
		/// </summary>
		DragSelectionFillSerial,

		/// <summary>
		/// Move a selection range by dragging mouse on border of selection.
		/// </summary>
		SelectionRangeMove,
		SelectionRangeMovePrepare,

		/// <summary>
		/// Picking a range during formula inputting.
		/// </summary>
		RangePicker,

		AdjustPageBreakRow,
		AdjustPageBreakColumn,

		HighlightRangeCreate,
		HighlightRangeSelect,
		HighlightRangeMove,
		HighlightRangeResize,

		CellBodyCapture,
	}

	/// <summary>
	/// Represents the frozen areas.
	/// </summary>
	public enum FreezeArea : byte
	{
		/// <summary>
		/// Do not freeze to any positions.
		/// </summary>
		None = 0,

		/// <summary>
		/// Freeze to left.
		/// </summary>
		Left = 1,

		/// <summary>
		/// Freeze to top.
		/// </summary>
		Top = 2,

		/// <summary>
		/// Freeze to right.
		/// </summary>
		Right = 3,

		/// <summary>
		/// Freeze to bottom.
		/// </summary>
		Bottom = 4,

		/// <summary>
		/// Freeze to left and top.
		/// </summary>
		LeftTop = 5,

		/// <summary>
		/// Freeze to left and bottom.
		/// </summary>
		LeftBottom = 6,

		/// <summary>
		/// Freeze to right and top.
		/// </summary>
		RightTop = 7,

		/// <summary>
		/// Freeze to right and bottom.
		/// </summary>
		RightBottom = 8,
	}

	/// <summary>
	/// View mode of current worksheet.
	/// </summary>
	public enum ReoGridViewMode
	{
		/// <summary>
		/// Normal view
		/// </summary>
		Normal,

		/// <summary>
		/// Page view
		/// </summary>
		PageView,

		/// <summary>
		/// Custom view (Reserved)
		/// </summary>
		Custom,
	}
}
