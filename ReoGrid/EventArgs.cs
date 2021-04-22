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
 * ReoGrid is open source software released under MIT license.
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if WINFORM || ANDROID
using RGFloat = System.Single;
#elif WPF
using RGFloat = System.Double;
#elif iOS
using RGFloat = System.Double;
#endif // WPF

#if WINFORM
using RGKeys = System.Windows.Forms.Keys;
#endif // WINFORM

using unvell.Common;
using unvell.ReoGrid.DataFormat;
using unvell.ReoGrid.Views;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Interaction;
using unvell.ReoGrid.Main;

namespace unvell.ReoGrid.Events
{
	#region Workbook Arguments

	/// <summary>
	/// Common worksheet event arguments 
	/// </summary>
	public class WorksheetEventArgs : EventArgs
	{
		/// <summary>
		/// Instance of worksheet
		/// </summary>
		public Worksheet Worksheet { get; set; }

		/// <summary>
		/// Create common worksheet event arguments with specified instance of worksheet
		/// </summary>
		/// <param name="sheet">instance of worksheet related to the event</param>
		public WorksheetEventArgs(Worksheet sheet)
		{
			this.Worksheet = sheet;
		}
	}

	#region Actions

	/// <summary>
	/// Arguments of event which will be fired when action is performed by worksheet
	/// </summary>
	public class WorkbookActionEventArgs : EventArgs
	{
		/// <summary>
		/// Action is performed
		/// </summary>
		public IAction Action { get; private set; }

		/// <summary>
		/// Create this event argument with specified action
		/// </summary>
		/// <param name="action">instance of action</param>
		public WorkbookActionEventArgs(IAction action)
		{
			this.Action = action;
		}
	}

	/// <summary>
	/// Event argument for before action perform
	/// </summary>
	public class BeforeActionPerformEventArgs : WorkbookActionEventArgs
	{
		/// <summary>
		/// Determine whehter to abort perform current action
		/// </summary>
		public bool IsCancelled { get; set; }

		/// <summary>
		/// Create event argument with specified action
		/// </summary>
		/// <param name="action">Action to be performed</param>
		public BeforeActionPerformEventArgs(IAction action) : base(action)
		{
		}
	}

	#endregion // Actions

	#region Worksheet Managements

	/// <summary>
	/// Worksheet creating event argument
	/// </summary>
	public class WorksheetCreatedEventArgs : WorksheetEventArgs
	{
		/// <summary>
		/// Create this event argument with specified instance of worksheet
		/// </summary>
		/// <param name="sheet">instance of worksheet</param>
		public WorksheetCreatedEventArgs(Worksheet sheet)
			: base(sheet)
		{
		}
	}

	/// <summary>
	/// Worksheet inserting event argument
	/// </summary>
	public class WorksheetInsertedEventArgs : WorksheetEventArgs
	{
		/// <summary>
		/// Zero-based number of sheet is inserted
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// Create this event argument with specified worksheet
		/// </summary>
		/// <param name="sheet">instance of worksheet</param>
		public WorksheetInsertedEventArgs(Worksheet sheet)
			: base(sheet)
		{
		}
	}

	/// <summary>
	/// Worksheet removing event argument
	/// </summary>
	public class WorksheetRemovedEventArgs : WorksheetEventArgs
	{
		/// <summary>
		/// Index of worksheet in workbook before removing
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// Create this event argument with specified worksheet
		/// </summary>
		/// <param name="sheet">instance of worksheet</param>
		public WorksheetRemovedEventArgs(Worksheet sheet)
			: base(sheet)
		{
		}
	}

	/// <summary>
	/// Worksheet's name changing event argument
	/// </summary>
	public class WorksheetNameChangingEventArgs : WorksheetEventArgs
	{
		/// <summary>
		/// Get or set the new name used to instead of the old name of worksheet
		/// </summary>
		public string NewName { get; set; }

		/// <summary>
		/// Create this event argument with specified worksheet
		/// </summary>
		/// <param name="sheet">instance of worksheet</param>
		/// <param name="name">new name of worksheet</param>
		public WorksheetNameChangingEventArgs(Worksheet sheet, string name)
			: base(sheet)
		{
			this.NewName = name;
		}
	}

	/// <summary>
	/// Worksheet changing event argument
	/// </summary>
	public class CurrentWorksheetChangedEventArgs : WorksheetEventArgs
	{
		/// <summary>
		/// Zero-based number of sheet has changed
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// Create this event argument with specified worksheet
		/// </summary>
		/// <param name="sheet">instance of worksheet</param>
		public CurrentWorksheetChangedEventArgs(Worksheet sheet)
			: base(sheet)
		{
		}
	}

	/// <summary>
	/// Represents an event argument class for worksheet scrolling.
	/// </summary>
	public class WorksheetScrolledEventArgs : WorksheetEventArgs
	{
		/// <summary>
		/// Scrolled horizontal value.
		/// </summary>
		public RGFloat X { get; internal set; }

		/// <summary>
		/// Scrolled vertical value.
		/// </summary>
		public RGFloat Y { get; internal set; }

		/// <summary>
		/// Create the instance of this event argument.
		/// </summary>
		/// <param name="worksheet">The worksheet where event happened.</param>
		public WorksheetScrolledEventArgs(Worksheet worksheet)
			: base(worksheet)
		{
		}
	}

	#endregion // Worksheet Managements

	#endregion // Workbook Arguments

	#region Worksheet Arguments

	#region Cell Operations
	/// <summary>
	/// Position event argument on spreadsheet
	/// </summary>
	public class CellPosEventArgs : EventArgs
	{
		/// <summary>
		/// Zero-based two-dimensional coordinates to locate a cell on spreadsheet
		/// </summary>
		public CellPosition Position { get; set; }

		/// <summary>
		/// Construc this position event argument with specfieid position
		/// </summary>
		/// <param name="pos">zero-based two-dimensional coordinates on spreadsheet</param>
		public CellPosEventArgs(CellPosition pos)
		{
			this.Position = pos;
		}
	}

	/// <summary>
	/// Event raised on action was performed for any cells
	/// </summary>
	public class CellEventArgs : EventArgs
	{
		/// <summary>
		/// Get instance of current editing cell. This property may be null.
		/// </summary>
		public Cell Cell { get; protected set; }

		/// <summary>
		/// Create instance for CellEventArgs with specified cell.
		/// </summary>
		/// <param name="cell">Instance of current editing cell.</param>
		public CellEventArgs(Cell cell)
		{
			this.Cell = cell;
		}
	}
	#endregion // Cell Operations

	#region Mouse
	/// <summary>
	/// ReoGrid common mouse event argument
	/// </summary>
	public class WorksheetMouseEventArgs : EventArgs
	{
		/// <summary>
		/// Worksheet instance
		/// </summary>
		public Worksheet Worksheet { get; private set; }

		/// <summary>
		/// Pressed mouse buttons
		/// </summary>
		public MouseButtons Buttons { get; set; }

		/// <summary>
		/// Event source associated mouse position
		/// </summary>
		public Point RelativePosition { get; set; }

		/// <summary>
		/// Event source unassociated mouse position (Position to control)
		/// </summary>
		public Point AbsolutePosition { get; set; }

		/// <summary>
		/// Number of clicks
		/// </summary>
		public int Clicks { get; private set; }

		/// <summary>
		/// Delta value (only used in mouse wheel event)
		/// </summary>
		public int Delta { get; set; }

		/// <summary>
		/// Get or set whether to capture mouse from current event
		/// </summary>
		public bool Capture { get; set; }

		/// <summary>
		/// Get or set the cursor style during mouse over
		/// </summary>
		public CursorStyle CursorStyle { get; set; }

		/// <summary>
		/// Construct mouse event argument with specified parameters
		/// </summary>
		/// <param name="worksheet">worksheet instance</param>
		/// <param name="relativePosition">mouse relative position to current event owner</param>
		/// <param name="absolutePosition">mouse absolute position to spreadsheet control</param>
		/// <param name="buttons">pressed button flags</param>
		/// <param name="clicks">number of clicks</param>
		public WorksheetMouseEventArgs(Worksheet worksheet, Point relativePosition, Point absolutePosition,
			MouseButtons buttons, int clicks)
		{
			this.Worksheet = worksheet;
			this.Buttons = buttons;
			this.Clicks = clicks;
			this.RelativePosition = relativePosition;
			this.AbsolutePosition = absolutePosition;
		}

	}

	/// <summary>
	/// ReoGrid cell mouse event argument
	/// </summary>
	public class CellMouseEventArgs : WorksheetMouseEventArgs
	{
		/// <summary>
		/// Event source instance of cell. Note: this property may be null if cell has no data and style attached. 
		/// Check this property and create cell instance by CellPosition property.
		/// </summary>
		public Cell Cell { get; set; }

		/// <summary>
		/// Zero-based cell position
		/// </summary>
		public CellPosition CellPosition { get; set; }

		/// <summary>
		/// Create cell mouse event argument with specified parameters
		/// </summary>
		/// <param name="worksheet">worksheet instance</param>
		/// <param name="cellPosition">cell position</param>
		public CellMouseEventArgs(Worksheet worksheet, CellPosition cellPosition)
			: this(worksheet, null, cellPosition, new Point(0, 0), new Point(0, 0), MouseButtons.None, 0)
		{
		}

		/// <summary>
		/// Create cell mouse event argument with specified parameters
		/// </summary>
		/// <param name="worksheet">worksheet instance</param>
		/// <param name="cellPosition">cell position</param>
		/// <param name="relativePosition">relative mouse position (position in cell)</param>
		/// <param name="absolutePosition">absolute mouse position (position in spreadsheet)</param>
		/// <param name="buttons">pressed buttons</param>
		/// <param name="clicks">number of clicks</param>
		public CellMouseEventArgs(Worksheet worksheet, CellPosition cellPosition, Point relativePosition,
			Point absolutePosition, MouseButtons buttons, int clicks)
			: this(worksheet, null, cellPosition, relativePosition, absolutePosition, buttons, clicks)
		{
		}

		/// <summary>
		/// Create cell mouse event argument with specified parameters
		/// </summary>
		/// <param name="worksheet">worksheet instance</param>
		/// <param name="cell">cell instance</param>
		public CellMouseEventArgs(Worksheet worksheet, Cell cell)
			: this(worksheet, cell, cell.InternalPos, new Point(0, 0), new Point(0, 0), MouseButtons.None, 0)
		{
		}

		/// <summary>
		/// Create cell mouse event argument with specified parameters
		/// </summary>
		/// <param name="worksheet">worksheet instance</param>
		/// <param name="cell">cell instance</param>
		/// <param name="relativePosition">relative mouse position (position in cell)</param>
		/// <param name="absolutePosition">absolute mouse position (position in spreadsheet)</param>
		/// <param name="buttons">pressed buttons</param>
		/// <param name="clicks">number of clicks</param>
		public CellMouseEventArgs(Worksheet worksheet, Cell cell, Point relativePosition,
			Point absolutePosition, MouseButtons buttons, int clicks)
			: this(worksheet, cell, cell == null ? CellPosition.Empty : cell.InternalPos,
			relativePosition, absolutePosition, buttons, clicks)
		{
		}

		/// <summary>
		/// Create cell mouse event argument with specified parameters
		/// </summary>
		/// <param name="worksheet">worksheet instance</param>
		/// <param name="cell">cell instance</param>
		/// <param name="cellPosition">cell position</param>
		/// <param name="relativePosition">relative mouse position (position in cell)</param>
		/// <param name="absolutePosition">absolute mouse position (position in spreadsheet)</param>
		/// <param name="buttons">pressed buttons</param>
		/// <param name="clicks">number of clicks</param>
		public CellMouseEventArgs(Worksheet worksheet, Cell cell, CellPosition cellPosition,
			Point relativePosition, Point absolutePosition, MouseButtons buttons, int clicks)
			: base(worksheet, relativePosition, absolutePosition, buttons, clicks)
		{
			this.Cell = cell;
			this.CellPosition = cellPosition;
		}
	}
	#endregion // Mouse

	#region Keyborad
	/// <summary>
	/// Common key event argument
	/// </summary>
	public class WorksheetKeyEventArgs : EventArgs
	{
		public KeyCode KeyCode { get; set; }
	}

	/// <summary>
	/// Common key event argument for cells
	/// </summary>
	public class CellKeyDownEventArgs : WorksheetKeyEventArgs
	{
		/// <summary>
		/// Cell of event source
		/// </summary>
		public Cell Cell { get; set; }

		/// <summary>
		/// Position of cell of event source
		/// </summary>
		public CellPosition CellPosition { get; set; }
	}

	/// <summary>
	/// Event raised when user presses any key inside spreadsheet before built-in operations
	/// </summary>
	public class BeforeCellKeyDownEventArgs : CellKeyDownEventArgs
	{
		/// <summary>
		/// Determines whether or not should to cancel the following operations of this event.
		/// </summary>
		public bool IsCancelled { get; set; }
	}

	/// <summary>
	/// Event raised when user presses any key inside spreadsheet after built-in operations
	/// </summary>
	public class AfterCellKeyDownEventArgs : CellKeyDownEventArgs
	{
	}
	#endregion // Keyborad

	#region Editing
	/// <summary>
	/// Event raised after cell editing. Set 'NewData' property to a
	/// new value to change the data instead of edited value.
	/// </summary>
	public class CellAfterEditEventArgs : CellEventArgs
	{
		/// <summary>
		/// Set the data to new value instead of edited value.
		/// </summary>
		public object NewData { get; set; }

		/// <summary>
		/// Reason of edit operation ending. Set this property to restore 
		/// the data to the value of before editing.
		/// </summary>
		public EndEditReason EndReason { get; set; }

		/// <summary>
		/// When new data has been inputed, ReoGrid choose one formatter to 
		/// try to format the data. Set this property to force change the 
		/// formatter for the new data.
		/// </summary>
		public CellDataFormatFlag? DataFormat { get; set; }

		/// <summary>
		/// Create instance for CellAfterEditEventArgs
		/// </summary>
		/// <param name="cell">Cell edited by user</param>
		public CellAfterEditEventArgs(Cell cell) : base(cell) { }
	}

	/// <summary>
	/// Event raised before cell enter edit mode. Set 'IsCancelled' property force to stop default edit operation.
	/// </summary>
	public class CellBeforeEditEventArgs : CellEventArgs
	{
		/// <summary>
		/// Edit operation whether should be aborted.
		/// </summary>
		public bool IsCancelled { get; set; }
	
		/// <summary>
		/// Text will display in the input field, this text could be changed.
		/// </summary>
		public string EditText { get; set; }

		/// <summary>
		/// Create instance for CellBeforeEditEventArgs with specified cell.
		/// </summary>
		/// <param name="cell">Instance of cell will be edited by user.</param>
		public CellBeforeEditEventArgs(Cell cell) : base(cell) { }
	}

	/// <summary>
	/// Cell edit text.
	/// </summary>
	public class CellEditTextChangingEventArgs : CellEventArgs
	{
		/// <summary>
		/// Get or set the inputting text. Set new text to replace the text of user inputted.
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// Create event argument with specified cell.
		/// </summary>
		/// <param name="cell">instance of cell</param>
		public CellEditTextChangingEventArgs(Cell cell)
			: base(cell)
		{
		}
	}

	/// <summary>
	/// Event raised when unicode char was inputted during cell editing,
	/// replace the <code>InputChar</code> property to alter the input character.
	/// </summary>
	public class CellEditCharInputEventArgs : CellEventArgs
	{
		/// <summary>
		/// Get or set the input character.
		/// </summary>
		public int InputChar { get; set; }

		/// <summary>
		/// Get position of current editing text.
		/// </summary>
		public int CaretPositionInLine { get; private set; }

		/// <summary>
		/// Get line index of current editing text.
		/// </summary>
		public int LineIndex { get; private set; }

		/// <summary>
		/// Get current edit text inputted by user.
		/// </summary>
		public string InputText { get; private set; }

		internal CellEditCharInputEventArgs(Cell cell, string text, int @char, int caret, int line)
			: base(cell)
		{
			this.InputText = text;
			this.InputChar = @char;
			this.CaretPositionInLine = caret;
			this.LineIndex = line;
		}
	}
	#endregion // Editing

	#region Row Changes
	/// <summary>
	/// Base argument for events when worksheet row changed.
	/// </summary>
	public class WorksheetRowsEventArgs : EventArgs
	{
		/// <summary>
		/// Zero-based row index number.
		/// </summary>
		public int Row { get; private set; }

		/// <summary>
		/// Number of rows changed.
		/// </summary>
		public int Count { get; private set; }

		/// <summary>
		/// Create instance for RowEventArgs with specified row index number.
		/// </summary>
		/// <param name="row">Zero-based row index number.</param>
		internal WorksheetRowsEventArgs(int row, int count = 1)
		{
			this.Row = row;
			this.Count = count;
		}
	}

	/// <summary>
	/// Argument for event when row inserted into worksheet.
	/// </summary>
	public class RowsInsertedEventArgs : WorksheetRowsEventArgs
	{
		internal RowsInsertedEventArgs(int row, int count)
			: base(row, count)
		{
		}
	}

	/// <summary>
	/// Event raised when rows deleted from spreadsheet
	/// </summary>
	public class RowsDeletedEventArgs : WorksheetRowsEventArgs
	{
		/// <summary>
		/// Create instance for RowEventArgs with specified row index number.
		/// </summary>
		/// <param name="row">zero-based number of row start to delete</param>
		/// <param name="count">number of rows to be deleted</param>
		internal RowsDeletedEventArgs(int row, int count)
			: base(row, count)
		{
		}
	}

	/// <summary>
	/// Argument for event that will be raised when columns width is changed.
	/// </summary>
	public class RowsHeightChangedEventArgs : WorksheetRowsEventArgs
	{
		/// <summary>
		/// The new height that has been changed for rows.
		/// </summary>
		public int Height { get; private set; }

		internal RowsHeightChangedEventArgs(int index, int count, int height)
			: base(index, count)
		{
			this.Height = height;
		}
	}
	#endregion // Row Changes

	#region Column Changes
	/// <summary>
	/// Event raised when an action of column header was performed.
	/// </summary>
	public class WorksheetColumnsEventArgs : EventArgs
	{
		/// <summary>
		/// Zero-based number to insert columns
		/// </summary>
		public int Index { get; private set; }

		/// <summary>
		/// Indicates that how many columns has been inserted or appended
		/// </summary>
		public int Count { get; private set; }

		/// <summary>
		/// Create instead for ColumnEventArgs with specified column header number.
		/// </summary>
		/// <param name="index">Column index number.</param>
		/// <param name="count">Column count.</param>
		public WorksheetColumnsEventArgs(int index, int count) { this.Index = index; this.Count = count; }
	}

	/// <summary>
	/// Event raised when new column was inserted into a spreadsheet
	/// </summary>
	public class ColumnsInsertedEventArgs : WorksheetColumnsEventArgs
	{
		/// <summary>
		/// Create column inserted event argument
		/// </summary>
		/// <param name="index">Zero-based number of column start to insert</param>
		/// <param name="count">Number of columns to be inserted.</param>
		public ColumnsInsertedEventArgs(int index, int count)
			: base(index, count)
		{
		}
	}

	/// <summary>
	/// Event raised when columns deleted from spreadsheet
	/// </summary>
	public class ColumnsDeletedEventArgs : WorksheetColumnsEventArgs
	{
		/// <summary>
		/// Create column deleted event argument
		/// </summary>
		/// <param name="index">number of column start to delete</param>
		/// <param name="count">number of columns to be deleted</param>
		public ColumnsDeletedEventArgs(int index, int count)
			: base(index, count)
		{
		}
	}

	/// <summary>
	/// Argument for event that will be raised when columns width is changed.
	/// </summary>
	public class ColumnsWidthChangedEventArgs : WorksheetColumnsEventArgs
	{
		/// <summary>
		/// The new width changed for columns.
		/// </summary>
		public int Width { get; private set; }

		internal ColumnsWidthChangedEventArgs(int index, int count, int width)
			: base(index, count)
		{
			this.Width = width;
		}
	}

	#endregion // Column Changes

	#region Border Changes

	/// <summary>
	/// Event raised on border added to a range.
	/// </summary>
	public class BorderAddedEventArgs : RangeEventArgs
	{
		/// <summary>
		/// Position of border added.
		/// </summary>
		public BorderPositions Pos { get; set; }

		/// <summary>
		/// Style of border added.
		/// </summary>
		public RangeBorderStyle Style { get; set; }

		/// <summary>
		/// Create instance for BorderAddedEventArgs with specified range, 
		/// position of border and style of border.
		/// </summary>
		/// <param name="range"></param>
		/// <param name="pos"></param>
		/// <param name="style"></param>
		public BorderAddedEventArgs(RangePosition range, BorderPositions pos, RangeBorderStyle style)
			: base(range)
		{
			this.Pos = pos;
			this.Style = style;
		}
	}

	/// <summary>
	/// Event raised on border removed from a range.
	/// </summary>
	public class BorderRemovedEventArgs : RangeEventArgs
	{
		/// <summary>
		/// Position of border removed
		/// </summary>
		public BorderPositions Pos { get; set; }

		/// <summary>
		/// Create instance for BorderRemovedEventArgs with specified range and
		/// position of border.
		/// </summary>
		/// <param name="range"></param>
		/// <param name="pos"></param>
		public BorderRemovedEventArgs(RangePosition range, BorderPositions pos)
			: base(range)
		{
			this.Pos = pos;
		}
	}
	#endregion // Border Changes

	#region File IO
	/// <summary>
	/// Event raised on grid loaded from a stream.
	/// </summary>
	public class FileLoadedEventArgs : EventArgs
	{
		/// <summary>
		/// Full path of file. Available only grid was loaded from a file stream.
		/// </summary>
		public string Filename { get; set; }

		/// <summary>
		/// Create instance for FileSavedEventArgs with specified file path.
		/// </summary>
		/// <param name="filename">Full path of file</param>
		public FileLoadedEventArgs(string filename) { this.Filename = filename; }
	}

	/// <summary>
	/// Event raised on grid saved to a stream.
	/// </summary>
	public class FileSavedEventArgs : EventArgs
	{
		/// <summary>
		/// Full path of file. Available only grid be saved into a file stream.
		/// </summary>
		public string Filename { get; set; }

		/// <summary>
		/// Create instance for FileSavedEventArgs with specified file path.
		/// </summary>
		/// <param name="filename">Full path of file</param>
		public FileSavedEventArgs(string filename) { this.Filename = filename; }
	}
	#endregion // File IO

	#region Exception Notifications
	/// <summary>
	/// Event raised when any exceptions happen during built-in operations of worksheet.
	/// Such as Range copy/cut/move via built-in hotkeys.
	/// </summary>
	public class ExceptionHappenEventArgs : WorksheetEventArgs
	{
		/// <summary>
		/// Get or set the exception instance.
		/// </summary>
		public Exception Exception { get; set; }

		/// <summary>
		/// Create exception instance.
		/// </summary>
		/// <param name="sheet">Worksheet instance.</param>
		/// <param name="exception">Exception instance.</param>
		public ExceptionHappenEventArgs(Worksheet sheet, Exception exception)
			: base(sheet)
		{
			this.Exception = exception;
		}
	}
	#endregion // Exception Notifications

	#region Selection

	/// <summary>
	/// Event raised when selection moved to next position. 
	/// ReoGrid automatically move selection to right cell or below cell according
	/// to <code>SelectionForwardDirection</code> property of worksheet. 
	/// </summary>
	public class SelectionMovedForwardEventArgs : EventArgs
	{
		/// <summary>
		/// Decide whether to cancel current move operation.
		/// </summary>
		public bool IsCancelled { get; set; }

		/// <summary>
		/// Create instance of SelectionMovedForwardEventArgs with specified position.
		/// </summary>
		public SelectionMovedForwardEventArgs()
		{
		}
	}

	/// <summary>
	/// Event raised when selection moved to previous position. 
	/// ReoGrid automatically move selection to left cell or above cell according
	/// to <code>SelectionForwardDirection</code> property of worksheet. 
	/// </summary>
	public class SelectionMovedBackwardEventArgs : EventArgs
	{
		/// <summary>
		/// Decide whether to cancel current move operation.
		/// </summary>
		public bool IsCancelled { get; set; }

		/// <summary>
		/// Create instance of SelectionMovedBackwardEventArgs with specified position.
		/// </summary>
		public SelectionMovedBackwardEventArgs()
		{
		}
	}

	/// <summary>
	/// Argument class for event of BeforeSelectionChange
	/// </summary>
	public class BeforeSelectionChangeEventArgs : EventArgs
	{
		private CellPosition selectionStart;
		private CellPosition selectionEnd;

		/// <summary>
		/// Get or set selection start position
		/// </summary>
		public CellPosition SelectionStart { get { return this.selectionStart; } set { this.selectionStart = value; } }

		/// <summary>
		/// Get or set selection end position
		/// </summary>
		public CellPosition SelectionEnd { get { return this.selectionEnd; } set { this.selectionEnd = value; } }

		/// <summary>
		/// Get or set the start row of selection
		/// </summary>
		public int StartRow { get { return this.selectionStart.Row; } set { this.selectionStart.Row = value; } }

		/// <summary>
		/// Get or set this start column of selection
		/// </summary>
		public int StartCol { get { return this.selectionStart.Col; } set { this.selectionStart.Col = value; } }
		
		/// <summary>
		/// Get or set the end row of selection
		/// </summary>
		public int EndRow { get { return this.selectionEnd.Row; } set { this.selectionEnd.Row = value; } }

		/// <summary>
		/// Get or set the end column of selection
		/// </summary>
		public int EndCol { get { return this.selectionEnd.Col; } set { this.selectionEnd.Col = value; } }

		public bool IsCancelled { get; set; }

		/// <summary>
		/// Create this argument by specified selection start and end position
		/// </summary>
		/// <param name="selectionStart">The start position of selection</param>
		/// <param name="selectionEnd">The end position of selection</param>
		public BeforeSelectionChangeEventArgs(CellPosition selectionStart, CellPosition selectionEnd)
		{
			this.SelectionStart = selectionStart;
			this.SelectionEnd = selectionEnd;
		}
	}
	#endregion // Selection

	#region Range Operations

	/// <summary>
	/// Event raised on action was performed for range
	/// </summary>
	public class RangeEventArgs : EventArgs
	{
		/// <summary>
		/// Range of action performed
		/// </summary>
		public RangePosition Range { get; set; }

		/// <summary>
		/// Create instance for RangeEventArgs with specified range.
		/// </summary>
		/// <param name="range">Range of action performed</param>
		public RangeEventArgs(RangePosition range)
		{
			this.Range = range;
		}
	}
	
	/// <summary>
	/// Event raised when operation to be performed to range, this class has
	/// the property 'IsCancelled' it used to notify grid control to abort
	/// current operation.
	/// </summary>
	public class BeforeRangeOperationEventArgs : RangeEventArgs
	{
		/// <summary>
		/// Get or set the flag that be used to notify the grid 
		/// whether to abort current operation
		/// </summary>
		public bool IsCancelled { get; set; }

		/// <summary>
		/// Create instance of this class with specified range position
		/// </summary>
		/// <param name="range">Target range where performs the operation of this event</param>
		public BeforeRangeOperationEventArgs(RangePosition range) : base(range) { }
	}

	/// <summary>
	/// Event argument for copying or moving range by dragging mouse
	/// </summary>
	public class CopyOrMoveRangeEventArgs : EventArgs
	{
		/// <summary>
		/// Source range
		/// </summary>
		public RangePosition FromRange { get; set; }

		/// <summary>
		/// Target range
		/// </summary>
		public RangePosition ToRange { get; set; }

		/// <summary>
		/// Create event argument instance
		/// </summary>
		/// <param name="fromRange">Source range</param>
		/// <param name="toRange">Target range</param>
		public CopyOrMoveRangeEventArgs(RangePosition fromRange, RangePosition toRange)
		{
			this.FromRange = fromRange;
			this.ToRange = toRange;
		}
	}

	/// <summary>
	/// Event argument before copying or moving range by dragging mouse
	/// </summary>
	public class BeforeCopyOrMoveRangeEventArgs : CopyOrMoveRangeEventArgs
	{
		/// <summary>
		/// Cancelled flag used to notify control that abort the copy or move operation
		/// </summary>
		public bool IsCancelled { get; set; }

		/// <summary>
		/// Create event argument instance
		/// </summary>
		/// <param name="fromRange">Source range</param>
		/// <param name="toRange">Target range</param>
		public BeforeCopyOrMoveRangeEventArgs(RangePosition fromRange, RangePosition toRange)
			: base(fromRange, toRange)
		{
		}
	}

	
	/// <summary>
	/// Event raised when any errors happened during range operation
	/// </summary>
	public class RangeOperationErrorEventArgs : RangeEventArgs
	{
		/// <summary>
		/// The exception if happened during range operation
		/// </summary>
		public Exception Exception { get; set; }

		/// <summary>
		/// Construct instance with specified range
		/// </summary>
		/// <param name="range">Target range</param>
		/// <param name="ex">Additional exception associated to the range</param>
		public RangeOperationErrorEventArgs(RangePosition range, Exception ex)
			: base(range)
		{
			this.Exception = ex;
		}
	}
	#endregion // Range Operations

	#region Settings
	/// <summary>
	/// Event raised when control's settings has been changed
	/// </summary>
	public class SettingsChangedEventArgs : EventArgs
	{
		/// <summary>
		/// The setting flags what have been added
		/// </summary>
		public WorksheetSettings AddedSettings { get; set; }

		/// <summary>
		/// The setting flags what have been removed
		/// </summary>
		public WorksheetSettings RemovedSettings { get; set; }
	}
	#endregion // Settings

	#region Named Range
	/// <summary>
	/// Common named range event argument
	/// </summary>
	public class NamedRangeEventArgs : RangeEventArgs
	{
		/// <summary>
		/// Name of range
		/// </summary>
		public string Name {get;set;}

		/// <summary>
		/// Create named range event argument with specified parameters
		/// </summary>
		/// <param name="range">range as operation target</param>
		/// <param name="name">name of range</param>
		public NamedRangeEventArgs(RangePosition range, string name)
			: base(range)
		{
			this.Name = name;
		}
	}

	/// <summary>
	/// Event raised when named range has been added into spreadsheet
	/// </summary>
	public class NamedRangeAddedEventArgs : NamedRangeEventArgs
	{
		/// <summary>
		/// Named range instance
		/// </summary>
		public NamedRange NamedRange { get; private set; }

		/// <summary>
		/// Create event argument instance with named range instance
		/// </summary>
		/// <param name="namedRange">named range instance</param>
		public NamedRangeAddedEventArgs(NamedRange namedRange)
			: this(namedRange, namedRange.Name)
		{
			this.NamedRange = namedRange;
		}

		/// <summary>
		/// Create event argument instance with specified parameters
		/// </summary>
		/// <param name="range">spreadsheet range definition</param>
		/// <param name="name">the name of added range</param>
		public NamedRangeAddedEventArgs(RangePosition range, string name)
			: base(range, name)
		{
		}	
	}

	/// <summary>
	/// Event raised when named range has been deleted from spreadsheet
	/// </summary>
	public class NamedRangeUndefinedEventArgs : NamedRangeEventArgs
	{
		/// <summary>
		/// Construct event argument with specified parameters
		/// </summary>
		/// <param name="range">spreadsheet range definition</param>
		/// <param name="name">the name of deleted range</param>
		public NamedRangeUndefinedEventArgs(RangePosition range, string name)
			: base(range, name)
		{
		}
	}
	#endregion // Named Range

	#region Drawing
	/// <summary>
	/// Represents common event argument of drawing objects.
	/// </summary>
	public class DrawingEventArgs : EventArgs
	{
		/// <summary>
		/// Get the platform no-associated drawing context instance.
		/// </summary>
		public DrawingContext Context { get; private set; }

		/// <summary>
		/// Get the bounds of target rendering region.
		/// </summary>
		public Rectangle Bounds { get; private set; }

		internal DrawingEventArgs(DrawingContext dc, Rectangle bounds)
		{
			this.Context = dc;
			this.Bounds = bounds;
		}

	}
	#endregion // Drawing

	#endregion // Worksheet Arguments
}

