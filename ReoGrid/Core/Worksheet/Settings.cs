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

namespace unvell.ReoGrid
{
	/// <summary>
	/// Worksheet Settings
	/// </summary>
	/// <remarks>Refer to: https://reogrid.net/document/settings </remarks>
	[Flags]
	public enum WorksheetSettings : ulong
	{
		/// <summary>
		/// None settings.
		/// </summary>
		None = 0x000000000L,

		/// <summary>
		/// Default settings.
		/// </summary>
		Default = Edit_Default | Behavior_Default | View_Default | Formula_Default,

		#region Edit
		/// <summary>
		/// All default edit settings.
		/// </summary>
		Edit_Default = Edit_AutoFormatCell | Edit_FriendlyPercentInput 
			| Edit_AutoExpandRowHeight //| Edit_AutoAdjustColumnWidth
			| Edit_AllowAdjustRowHeight | Edit_AllowAdjustColumnWidth
			| Edit_DragSelectionToMoveCells
			| Edit_DragSelectionToFillSerial,

		/// <summary>
		/// Make worksheet read-only. Any changes are not allowed.
		/// </summary>
		Edit_Readonly = 0x00000001L,

		/// <summary>
		/// Allow data format after text editing by user.
		/// </summary>
		Edit_AutoFormatCell = 0x00000002L,

		/// <summary>
		/// Allow to put a '%' symbol at the end of text when start edit a cell that is set as percent data format.
		/// </summary>
		Edit_FriendlyPercentInput = 0x00000004L,

		/// <summary>
		/// Allow automatically adjust row height to fit largest cell.
		/// </summary>
		Edit_AutoExpandRowHeight = 0x00000008L,

		/// <summary>
		/// Allow user adjusts row height by mouse.
		/// </summary>
		Edit_AllowAdjustRowHeight = 0x00000010L,

		/// <summary>
		/// Allow automatically adjust column width to fit largest cell.
		/// </summary>
		Edit_AutoExpandColumnWidth = 0x00000020L,

		/// <summary>
		/// Allows user adjusts column width by mouse.
		/// </summary>
		Edit_AllowAdjustColumnWidth = 0x00000040L,

		/// <summary>
		/// Allow user drags and drops the selection to move cell content.
		/// </summary>
		Edit_DragSelectionToMoveCells = 0x00000080L,

		/// <summary>
		/// Allow user drags and drops selection to fill data serial, reuse formulas or clear data.
		/// </summary>
		Edit_DragSelectionToFillSerial = 0x00000100L,

		#endregion // Edit

		#region Behavior

		/// <summary>
		/// All behaivor settings.
		/// </summary>
		Behavior_Default = Behavior_DoubleClickToResizeHeader | Behavior_MouseWheelToScroll
			| Behavior_MouseWheelToZoom | Behavior_ShortcutKeyToZoom | Behavior_ScrollToFocusCell
			| Behavior_AllowUserChangingPageBreaks,

		/// <summary>
		/// Allow double click to make the headers fit to maximum text of cells. 
		/// (Behavior_DoubleClickToFitRowHeight & Behavior_DoubleClickToFitColumnWidth)
		/// </summary>
		Behavior_DoubleClickToResizeHeader = Behavior_DoubleClickToFitRowHeight | Behavior_DoubleClickToFitColumnWidth,

		/// <summary>
		/// Allow double click to make the row height fits to maximum text of cells.
		/// </summary>
		Behavior_DoubleClickToFitRowHeight = 0x00000200L,

		/// <summary>
		/// Allow double click to make the column width fits to maximum text of cells.
		/// </summary>
		Behavior_DoubleClickToFitColumnWidth = 0x00000400L,

		/// <summary>
		/// Allow to scroll spreadsheet by mouse wheeling.
		/// </summary>
		Behavior_MouseWheelToScroll = 0x00000800L,

		/// <summary>
		/// Allow user zooms worksheet by wheeling mouse.
		/// </summary>
		Behavior_MouseWheelToZoom = 0x00001000L,

		/// <summary>
		/// Allow user zooms worksheet by hotkeys. (ctrl + plus/minus)
		/// </summary>
		Behavior_ShortcutKeyToZoom = 0x00002000L,

		/// <summary>
		/// Allow user insert or adjust the page breaks by mouse.
		/// </summary>
		Behavior_AllowUserChangingPageBreaks = 0x00004000L,

		///// <summary>
		///// Allow user to move entire column by dragging mouse.
		///// </summary>
		//Behavior_DragToMoveColumnHeader = 0x00004000L,

		/// <summary>
		/// Allow always scroll worksheet automatically to make focus cell entirely visible.
		/// </summary>
		Behavior_ScrollToFocusCell = 0x00008000L,

		#endregion // Behavior

		#region View

		/// <summary>
		/// Default view settings (View_ShowHeaders | View_ShowGridLine | View_AllowShowOutlines)
		/// </summary>
		View_Default = View_ShowHeaders | View_ShowGridLine | View_ShowFrozenLine | View_AllowShowOutlines
			| View_AntialiasDrawing | View_AllowCellTextOverflow,

		/// <summary>
		/// Show column header.
		/// </summary>
		View_ShowColumnHeader = 0x00010000L,

		/// <summary>
		/// Show row header.
		/// </summary>
		View_ShowRowHeader = 0x00020000L,

		/// <summary>
		/// Show column header and row header.
		/// </summary>
		View_ShowHeaders = View_ShowColumnHeader | View_ShowRowHeader,

		/// <summary>
		/// Show Horizontal Ruler. (Reserved)
		/// </summary>
		View_ShowHorizontalRuler = 0x00040000L,

		/// <summary>
		/// Show Vertical Ruler. (Reserved)
		/// </summary>
		View_ShowVerticalRuler = 0x00080000L,

		/// <summary>
		/// Show rulers in horizontal and vertical direction. (Reserved)
		/// </summary>
		View_ShowRulers = View_ShowHorizontalRuler | View_ShowVerticalRuler,

		/// <summary>
		/// Show guide line.
		/// </summary>
		View_ShowGridLine = 0x00100000L,

		/// <summary>
		/// Allow to show outlines for rows.
		/// </summary>
		View_AllowShowRowOutlines = 0x00200000L,

		/// <summary>
		/// Allow to show outlines for columns.
		/// </summary>
		View_AllowShowColumnOutlines = 0x00400000L,

		/// <summary>
		/// Allow to show outlines on both row and column.
		/// </summary>
		View_AllowShowOutlines = View_AllowShowRowOutlines | View_AllowShowColumnOutlines,

		/// <summary>
		/// Allow cell text be displayed overflow the boundary of cell.
		/// </summary>
		View_ShowHiddenCellLine = 0x01000000L,

		/// <summary>
		/// Allow cell text be displayed overflow the boundary of cell.
		/// </summary>
		View_AllowCellTextOverflow = 0x02000000L,

		/// <summary>
		/// Enable to show pages boundaries for printing areas.
		/// </summary>
		View_ShowPageBreaks = 0x04000000L,

		/// <summary>
		/// Enable anti-alias drawing.
		/// </summary>
		View_AntialiasDrawing = 0x08000000L,

		/// <summary>
		/// Allow to show a gray solid line at frozen cells.
		/// </summary>
		View_ShowFrozenLine = 0x10000000L,

		#endregion // View

		#region Formula

		/// <summary>
		/// Default formula settings.
		/// </summary>
		Formula_Default = Formula_AutoUpdateReferenceCell 
			| Formula_AutoPickingCellAddress | Formula_AutoFormat
			,

		/// <summary>
		/// Allow to update formula reference cells automatically.
		/// </summary>
		Formula_AutoUpdateReferenceCell = 0x100000000L,

		/// <summary>
		/// Allow to pick an address from selected cell during formula editing. (Reserved)
		/// </summary>
		Formula_AutoPickingCellAddress = 0x200000000L,

		/// <summary>
		/// Allow to correct, format and rebuild formula automatically.
		/// </summary>
		Formula_AutoFormat = 0x400000000L,

		#endregion // Formula

	}

}
