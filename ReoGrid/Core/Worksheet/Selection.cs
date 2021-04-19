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
using System.ComponentModel;

#if DEBUG
using System.Diagnostics;
#endif // DEBUG

#if EX_SCRIPT
using unvell.ReoScript;
#endif // EX_SCRIPT

using unvell.ReoGrid.Events;
using unvell.ReoGrid.Views;
using unvell.ReoGrid.Graphics;

namespace unvell.ReoGrid
{
	partial class Worksheet
	{
		#region Position
		internal CellPosition selStart = new CellPosition(0, 0);
		internal CellPosition selEnd = new CellPosition(0, 0);

		#region Focus & Hover

		internal CellPosition focusPos = new CellPosition(0, 0);

		/// <summary>
		/// The column focus pos goes when enter key pressed.
		/// </summary>
		private int focusReturnColumn = 0;

		/// <summary>
		/// Get or set current focused cell position.
		/// </summary>
		public CellPosition FocusPos
		{
			get
			{
				return this.focusPos;
			}
			set
			{
				// different with current focus pos
				if (this.focusPos != value)
				{
					var newFocusPos = this.FixPos(value);

					// not empty position
					if (!newFocusPos.IsEmpty)
					{
						var focusCell = cells[newFocusPos.Row, newFocusPos.Col];

						if (focusCell != null)
						{
							// new focus cell may be an invalid cell, need check it
							if (!focusCell.IsValidCell)
							{
								// if inside any merged cell, find the merge-start-cell
								newFocusPos = GetMergedCellOfRange(focusCell).InternalPos;
							}
						}
					}

					// compare to current focus position again
					if (this.focusPos != newFocusPos)
					{
						// if current focus position is not empty
						if (!focusPos.IsEmpty)
						{
							// get the cell, and invoke OnLostFocus if cell's has body
							var focusCell = cells[focusPos.Row, focusPos.Col];

							if (focusCell != null && focusCell.body != null)
							{
								focusCell.body.OnLostFocus();
							}
						}

						this.focusPos = newFocusPos;

						// invoke OnGotFocus on new focus position
						if (!this.focusPos.IsEmpty)
						{
							var focusCell = cells[focusPos.Row, focusPos.Col];

							if (focusCell != null && focusCell.body != null && focusCell.IsValidCell)
							{
								focusCell.body.OnGotFocus();
							}

							if (!this.selectionRange.Contains(this.focusPos))
							{
								SelectRange(this.focusPos.Row, this.FocusPos.Col, 1, 1);
							}
						}

						this.RequestInvalidate();

						FocusPosChanged?.Invoke(this, new CellPosEventArgs(focusPos));
					}
				}
			}
		}

		/// <summary>
		/// Raise when focus cell is changed
		/// </summary>
		public event EventHandler<CellPosEventArgs> FocusPosChanged;

		private FocusPosStyle focusPosStyle = FocusPosStyle.Default;

		/// <summary>
		/// Get or set focus position display style
		/// </summary>
		public FocusPosStyle FocusPosStyle
		{
			get
			{
				return this.focusPosStyle;
			}
			set
			{
				if (this.focusPosStyle != value)
				{
					RequestInvalidate();

					this.focusPosStyle = value;

					FocusPosStyleChanged?.Invoke(this, null);
				}
			}
		}

		/// <summary>
		/// Focus position style changed.
		/// </summary>
		public event EventHandler<EventArgs> FocusPosStyleChanged;

		internal CellPosition hoverPos;

		/// <summary>
		/// Cell when mouse moving and hover on
		/// </summary>
		public CellPosition HoverPos
		{
			get
			{
				return hoverPos;
			}

			internal set
			{
				if (hoverPos != value)
				{
					// raise cell mouse enter
					if (!hoverPos.IsEmpty)
					{
						CellMouseEventArgs evtArg = null;

						if (CellMouseLeave != null)
						{
							evtArg = new CellMouseEventArgs(this, hoverPos);
							CellMouseLeave(this, evtArg);
						}

						var cell = cells[hoverPos.Row, hoverPos.Col];

						if (cell != null)
						{
							if (!cell.IsValidCell)
							{
								cell = GetMergedCellOfRange(cell);
							}

							if (cell.body != null)
							{
								if (evtArg == null)
								{
									evtArg = new CellMouseEventArgs(this, cell);
								}

								bool processed = cell.body.OnMouseLeave(evtArg);
								if (processed) this.RequestInvalidate();
							}
						}
					}

					hoverPos = value;

					// raise cell mouse leave
					if (!hoverPos.IsEmpty)
					{
						CellMouseEventArgs evtArg = null;

						if (CellMouseEnter != null)
						{
							evtArg = new CellMouseEventArgs(this, hoverPos);
							CellMouseEnter(this, evtArg);
						}

						var cell = cells[hoverPos.Row, hoverPos.Col];

						if (cell != null)
						{
							if (!cell.IsValidCell)
							{
								cell = GetMergedCellOfRange(cell);
							}

							if (cell.body != null)
							{
								if (evtArg == null)
								{
									evtArg = new CellMouseEventArgs(this, cell);
									evtArg.Cell = cell;
								}

								bool processed = cell.body.OnMouseEnter(evtArg);
								if (processed) this.RequestInvalidate();
							}
						}
					}

					HoverPosChanged?.Invoke(this, new CellPosEventArgs(hoverPos));
				}
			}
		}

		/// <summary>
		/// Raise when hover cell is changed
		/// </summary>
		public event EventHandler<CellPosEventArgs> HoverPosChanged;

		#endregion // Focus & Hover

		internal RangePosition selectionRange = new RangePosition(0, 0, 1, 1);

		/// <summary>
		/// Current selection range of entire grid. If SelectionMode is None, the value of this property will be Empty.
		/// </summary>
		public RangePosition SelectionRange
		{
			get { return selectionRange; }
			set { SelectRange(value); }
		}
		#endregion // Position

		#region Mode & Style

		internal WorksheetSelectionMode selectionMode = WorksheetSelectionMode.Range;

		/// <summary>
		/// Get or set selection mode for worksheet.
		/// </summary>
		[DefaultValue(WorksheetSelectionMode.Range)]
		public WorksheetSelectionMode SelectionMode
		{
			get
			{
				return selectionMode;
			}
			set
			{
				if (selectionMode != value)
				{
					if (this.IsEditing)
					{
						this.EndEdit(EndEditReason.NormalFinish);
					}

					var oldSelectionMode = this.selectionMode;

					this.selectionMode = value;

					switch (oldSelectionMode)
					{
						case WorksheetSelectionMode.None:
							switch (value)
							{
								case WorksheetSelectionMode.Cell:
								case WorksheetSelectionMode.Range:
									#region None -> Cell/Range
									SelectRange(new RangePosition(0, 0, 1, 1));
									#endregion // None -> Cell/Range
									break;
							}
							break;

						default:
							switch (value)
							{
								case WorksheetSelectionMode.None:
									#region Any -> None
									this.selectionRange = RangePosition.Empty;
									this.focusPos = CellPosition.Empty;
									RequestInvalidate();
									#endregion // Any -> None
									break;

								case WorksheetSelectionMode.Cell:
									#region Any -> Cell
									SelectRange(this.selStart.Row, this.selStart.Col, 1, 1);
									#endregion // Any -> Cell
									break;

								case WorksheetSelectionMode.Range:
									SelectionRange = FixRangeSelection(this.selectionRange);
									break;
							}
							break;
					}

					switch (this.selectionMode)
					{
						case WorksheetSelectionMode.Row:
						case WorksheetSelectionMode.SingleRow:
							#region Any -> Row
							SelectRange(this.selectionRange.Row, 0, this.selectionRange.Rows, -1);
							#endregion // None -> Row
							break;

						case WorksheetSelectionMode.Column:
						case WorksheetSelectionMode.SingleColumn:
							#region Any -> Column
							SelectRange(0, this.selectionRange.Col, -1, this.selectionRange.Cols);
							#endregion // None -> Column
							break;
					}

					if (SelectionModeChanged != null)
					{
						SelectionModeChanged(this, null);
					}
				}
			}
		}

		private WorksheetSelectionStyle selectionStyle = WorksheetSelectionStyle.Default;

		/// <summary>
		/// Get or set the selection style for worksheet.
		/// </summary>
		[DefaultValue(WorksheetSelectionStyle.Default)]
		public WorksheetSelectionStyle SelectionStyle
		{
			get
			{
				return selectionStyle;
			}
			set
			{
				if (selectionStyle != value)
				{
					selectionStyle = value;
					RequestInvalidate();

					SelectionStyleChanged?.Invoke(this, null);
				}
			}
		}

		private SelectionForwardDirection selectionForwardDirection;

		/// <summary>
		/// Get or set focus cell moving direction.
		/// </summary>
		[DefaultValue(SelectionForwardDirection.Right)]
		public SelectionForwardDirection SelectionForwardDirection
		{
			get
			{
				return this.selectionForwardDirection;
			}
			set
			{
				if (this.selectionForwardDirection != value)
				{
					this.selectionForwardDirection = value;

					this.SelectionForwardDirectionChanged?.Invoke(this, null);
				}
			}
		}

		#endregion // Mode & Style

		#region Mouse Select
		internal void SelectRangeStartByMouse(Point location)
		{
			if (this.viewportController == null || this.viewportController.View == null)
			{
				return;
			}

#if WINFORM || WPF
			if (!Common.Toolkit.IsKeyDown(unvell.Common.Win32Lib.Win32.VKey.VK_SHIFT))
			{
#endif // WINFORM || WPF
			var viewport = this.viewportController.View.GetViewByPoint(location) as IRangeSelectableView;

			if (viewport == null)
			{
				viewport = this.viewportController.FocusView as IRangeSelectableView;
			}

			if (viewport != null)
			{
				Point vp = viewport.PointToView(location);

				var pos = CellsViewport.GetPosByPoint(viewport, vp);
				this.selEnd = this.selStart = pos;
			}
#if WINFORM || WPF
			}
#endif // WINFORM || WPF

			this.SelectRangeEndByMouse(location);
		}

		internal void SelectRangeEndByMouse(Point location)
		{
			if (this.viewportController == null || this.viewportController.View == null)
			{
				return;
			}

			var viewport = this.viewportController.View.GetViewByPoint(location) as IRangeSelectableView;

			if (viewport == null)
			{
				viewport = this.viewportController.FocusView as IRangeSelectableView;
			}

			if (viewport != null)
			{
				Point vp = viewport.PointToView(location);

				var startpos = this.selStart;
				var endpos = this.selEnd;

				#region Each Operation Status
				switch (this.operationStatus)
				{
					case OperationStatus.FullColumnSelect:
						{
							int col = -1;

							this.FindColumnByPosition(vp.X, out col);

							if (col > -1)
							{
								startpos = new CellPosition(0, startpos.Col);
								endpos = new CellPosition(this.rows.Count, col);
							}
						}
						break;

					case OperationStatus.FullRowSelect:
						{
							int row = -1;

							this.FindRowByPosition(vp.Y, out row);

							if (row > -1)
							{
								startpos = new CellPosition(startpos.Row, 0);
								endpos = new CellPosition(row, this.cols.Count);
							}
						}
						break;

					default:
						endpos = CellsViewport.GetPosByPoint(viewport, vp);
						break;
				}
				#endregion // Each Operation Status

				this.ApplyRangeSelection(startpos, endpos);
			}
		}
		#endregion // Mouse Select

		#region Select API

		/// <summary>
		/// Select specified range.
		/// </summary>
		/// <param name="range">Specified range to be selected</param>
		private RangePosition FixRangeSelection(RangePosition range)
		{
			if (range.IsEmpty) return RangePosition.Empty;

#if DEBUG
			Stopwatch stop = Stopwatch.StartNew();
#endif

			RangePosition fixedRange = FixRange(range);

			int minr = fixedRange.Row;
			int minc = fixedRange.Col;
			int maxr = fixedRange.EndRow;
			int maxc = fixedRange.EndCol;

			switch (this.selectionMode)
			{
				case WorksheetSelectionMode.Cell:
					maxr = minr = range.Row;
					maxc = minc = range.Col;
					break;

				case WorksheetSelectionMode.Row:
					minc = 0;
					maxc = this.cols.Count - 1;
					break;

				case WorksheetSelectionMode.Column:
					minr = 0;
					maxr = this.rows.Count - 1;
					break;
			}

			if ((this.selectionMode == WorksheetSelectionMode.Cell
				|| this.selectionMode == WorksheetSelectionMode.Range)
				&& ((fixedRange.Cols < this.cols.Count
				&& fixedRange.Rows < this.rows.Count)
				|| this.cols.Count == 1 || this.rows.Count == 1)
				)
			{
				#region Check and select the whole merged region
				//#if DEBUG
				//				if (!Toolkit.IsKeyDown(unvell.Common.Win32Lib.Win32.VKey.VK_CONTROL))
				//				{
				//#endif
				//
				// if there are any entire rows or columns selected (full == -1)
				// the selection bounds of merged range will not be checked.
				// any changes to the selection will also not be appiled to the range.
				//
				RangePosition checkedRange = CheckMergedRange(new RangePosition(minr, minc, maxr - minr + 1, maxc - minc + 1));

				minr = checkedRange.Row;
				minc = checkedRange.Col;
				maxr = checkedRange.EndRow;
				maxc = checkedRange.EndCol;

				//#if DEBUG
				//				}
				//#endif
				#endregion
			}

			int rows = maxr - minr + 1;
			int cols = maxc - minc + 1;

#if DEBUG
			stop.Stop();
			if (stop.ElapsedMilliseconds > 25)
			{
				Debug.WriteLine("select range takes " + stop.ElapsedMilliseconds + " ms.");
			}
#endif

			return new RangePosition(minr, minc, rows, cols);
		}

		private void MoveRangeSelection(CellPosition start, CellPosition end, bool appendSelect, bool scrollToSelectionEnd = true)
		{
			if (!appendSelect)
			{
				start = end;
			}

			this.ApplyRangeSelection(start, end, scrollToSelectionEnd: scrollToSelectionEnd);
		}

		private void ApplyRangeSelection(CellPosition start, CellPosition end, bool scrollToSelectionEnd = true)
		{
			bool processed = false;

			switch (this.operationStatus)
			{
				case OperationStatus.HighlightRangeCreate:
					{
						var fixedRange = FixRangeSelection(new RangePosition(start, end));

						if (this.focusHighlightRange == null)
						{
							// no focus highlight range, create one
							var refRange = AddHighlightRange(fixedRange);
							this.FocusHighlightRange = refRange;
						}
						else if (this.focusHighlightRange.Position != fixedRange)
						{
							// update size for current focus highlight range
							this.focusHighlightRange.Position = fixedRange;
							this.RequestInvalidate();
						}
					}

					processed = true;
					break;

				default:
					ChangeSelectionRange(start, end);

					processed = true;
					break;
			}

			if (processed)
			{
				if (this.HasSettings(WorksheetSettings.Behavior_ScrollToFocusCell)
					 //commented out before the case of entire row or column
					 //is checked inside NormalViewportController.ScrollToRange method
					 //issue #179
					//&& (this.selectionRange.Rows != this.rows.Count
					//&& this.selectionRange.Cols != this.cols.Count)
					&& scrollToSelectionEnd
					)
				{
					// skip to scroll if entire worksheet is selected
					if (!(start.Row == 0 && start.Col == 0
						&& selEnd.Row == this.rows.Count - 1 && selEnd.Col == this.cols.Count - 1))
					{
						this.ScrollToCell(selEnd);
					}
				}
			}
		}

		private void ChangeSelectionRange(CellPosition start, CellPosition end)
		{
			var range = FixRangeSelection(new RangePosition(start, end));

			// compare to current selection, only do this when selection was really changed.
			if (this.selectionRange != range)
			{
				if (this.BeforeSelectionRangeChange != null)
				{
					var arg = new BeforeSelectionChangeEventArgs(start, end);
					this.BeforeSelectionRangeChange(this, arg);

					if (arg.IsCancelled) return;

					if (start != arg.SelectionStart || end != arg.SelectionEnd)
					{
						start = arg.SelectionStart;
						end = arg.SelectionEnd;

						range = FixRangeSelection(new RangePosition(start, end));
					}
				}

				this.selectionRange = range;

				this.selStart = start;
				this.selEnd = end;

				//if (!range.Contains(selStart)) selStart = range.StartPos;
				//if (!range.Contains(selEnd)) selEnd = range.EndPos;

				// focus pos validations:
				//   1. focus pos must be inside selection range
				//   2. focus pos cannot stop at invalid cell (any part of merged cell)
				if (this.focusPos.IsEmpty
					|| !range.Contains(this.focusPos)
					|| !IsValidCell(this.focusPos))
				{
					var focusPos = selStart;

					// find first valid cell as focus pos
					for (int r = range.Row; r <= range.EndRow; r++)
					{
						for (int c = range.Col; c <= range.EndCol; c++)
						{
							var cell = this.cells[r, c];
							if (cell != null && (cell.Colspan <= 0 || cell.Rowspan <= 0)) continue;

							focusPos.Row = r;
							focusPos.Col = c;
							goto quit_loop;
						}
					}
					quit_loop:

					if (focusPos.Col < this.cols.Count
						&& focusPos.Row < this.rows.Count)
					{
						FocusPos = focusPos;
					}
				}

				// update focus return column
				this.focusReturnColumn = end.Col;
				
				if (this.operationStatus == OperationStatus.RangeSelect)
				{
					this.SelectionRangeChanging?.Invoke(this, new RangeEventArgs(this.selectionRange));

#if EX_SCRIPT
					// comment out this if you get performance problem when using script extension
					RaiseScriptEvent("onselectionchanging");
#endif
				}
				else
				{
					this.SelectionRangeChanged?.Invoke(this, new RangeEventArgs(this.selectionRange));

#if EX_SCRIPT
					RaiseScriptEvent("onselectionchange");
#endif
				}

				RequestInvalidate();
			}
		}

		/// <summary>
		/// Select speicifed range on spreadsheet
		/// </summary>
		/// <param name="address">address or name of specified range to be selected</param>
		public void SelectRange(string address)
		{
			// range address
			if (RangePosition.IsValidAddress(address))
			{
				SelectRange(new RangePosition(address));
			}
			// named range
			else if (RGUtility.IsValidName(address))
			{
				NamedRange refRange;
				if (registeredNamedRanges.TryGetValue(address, out refRange))
				{
					SelectRange(refRange);
				}
			}
		}

		/// <summary>
		/// Select speicifed range on spreadsheet
		/// </summary>
		/// <param name="pos1">Start position of specified range</param>
		/// <param name="pos2">End position of specified range</param>
		public void SelectRange(CellPosition pos1, CellPosition pos2)
		{
			SelectRange(new RangePosition(pos1, pos2));
		}

		/// <summary>
		/// Select specified range
		/// </summary>
		/// <param name="row">number of row</param>
		/// <param name="col">number of col</param>
		/// <param name="rows">number of rows to be selected</param>
		/// <param name="cols">number of columns to be selected</param>
		public void SelectRange(int row, int col, int rows, int cols)
		{
			SelectRange(new RangePosition(row, col, rows, cols));
		}

		/// <summary>
		/// Select speicifed range on spreadsheet
		/// </summary>
		/// <param name="range">range to be selected</param>
		public void SelectRange(RangePosition range)
		{
			if (range.IsEmpty || this.selectionMode == WorksheetSelectionMode.None) return;

			range = this.FixRange(range);

			// submit to select a range 
			ApplyRangeSelection(range.StartPos, range.EndPos, false);
		}

		/// <summary>
		/// Select entire sheet
		/// </summary>
		public void SelectAll()
		{
			if (IsEditing)
			{
				this.controlAdapter.EditControlSelectAll();
			}
			else
			{
				SelectRange(new RangePosition(0, 0, RowCount, ColumnCount));
			}
		}

		/// <summary>
		/// Select entire rows of columns form specified column
		/// </summary>
		/// <param name="col">number of column start to be selected</param>
		/// <param name="columns">numbers of column to be selected</param>
		public void SelectColumns(int col, int columns)
		{
			SelectRange(new RangePosition(0, col, this.rows.Count, columns));
		}

		/// <summary>
		/// Select entire column of rows from specified row
		/// </summary>
		/// <param name="row">number of row start to be selected</param>
		/// <param name="rows">numbers of row to be selected</param>
		public void SelectRows(int row, int rows)
		{
			SelectRange(new RangePosition(row, 0, rows, this.cols.Count));
		}

		#endregion // Select API

		#region Keyboard Move

		private void OnTabKeyPressed(bool shiftKeyDown)
		{
			if (this.selectionMode == WorksheetSelectionMode.None)
			{
				return;
			}

			var backupReturnCol = this.focusReturnColumn;

			if (!shiftKeyDown)
			{
				var endCol = this.selectionRange.Cols > 1 ? this.selectionRange.EndCol
					: this.cols.Count - 1;

				if (this.focusPos.Col < endCol)
				{
					MoveFocusRight();
				}
				else
				{
					var endRow = this.selectionRange.Rows > 1 ? this.selectionRange.EndRow
						: this.rows.Count - 1;

					if (this.focusPos.Row < endRow)
					{
						var startCol = this.selectionRange.Cols > 1 ? this.selectionRange.Col : 0;

						this.focusPos.Col = startCol;

						MoveFocusDown();
					}
				}
			}
			else
			{
				var startCol = this.selectionRange.Cols > 1 ? this.selectionRange.Col : 0;

				if (selEnd.Col > startCol)
				{
					MoveSelectionLeft();
				}
				else
				{
					var startRow = this.selectionRange.Rows > 1 ? this.selectionRange.Row : 0;

					if (selEnd.Row > startRow)
					{
						var endCol = this.selectionRange.Cols > 1 ? this.selectionRange.EndCol
							: this.cols.Count - 1;

						this.focusPos.Col = endCol;

						MoveSelectionUp();
					}
				}
			}

			this.focusReturnColumn = backupReturnCol;
		}

		private void OnEnterKeyPressed(bool shiftKeyDown)
		{
			if (this.selectionMode == WorksheetSelectionMode.None)
			{
				return;
			}

			if (!shiftKeyDown)
			{
				MoveSelectionForward();
			}
			else
			{
				MoveSelectionBackward();
			}
		}

		/// <summary>
		/// Move focus position rightward.
		/// </summary>
		/// <param name="autoReturn">Determines whether or not move to next column if reached end row.</param>
		public void MoveFocusRight(bool autoReturn = true)
		{
			if (this.selectionMode == WorksheetSelectionMode.None)
			{
				return;
			}

			this.FocusPos = this.FindNextMovableCellRight(this.focusPos, 
				this.RangeIsMergedCell(this.selectionRange) ? this.FixRange(RangePosition.EntireRange) : this.selectionRange,
				autoReturn);
		}

		/// <summary>
		/// Move focus position downward.
		/// </summary>
		/// <param name="autoReturn">Determines whether or not move to next row if reached end column.</param>
		public void MoveFocusDown(bool autoReturn = true)
		{
			if (this.selectionMode == WorksheetSelectionMode.None)
			{
				return;
			}

			this.FocusPos = FindNextMovableCellDown(this.focusPos,
				this.RangeIsMergedCell(this.selectionRange) ? this.FixRange(RangePosition.EntireRange) : this.selectionRange,
				autoReturn);
		}

		#region Move Utility

		private CellPosition FindNextMovableCellUp(CellPosition pos, int firstRow)
		{
			int row = pos.Row;

			// find next movable cell upward
			while (row > firstRow)
			{
				row--;

				var cell = this.cells[row, pos.Col];

				if (cell != null && !cell.MergeEndPos.IsEmpty
					&& row < cell.MergeEndPos.Row
					&& row >= cell.MergeStartPos.Row)
				{
					continue;
				}

				if (this.rows[row].InnerHeight > 0)
				{
					break;
				}
			}

			return new CellPosition(row, pos.Col);
		}

		private CellPosition FindNextMovableCellLeft(CellPosition pos, int firstCol)
		{
			int col = pos.Col;

			// find next movable cell leftward
			while (col > firstCol)
			{
				col--;

				var cell = this.cells[pos.Row, col];

				if (cell != null && !cell.MergeEndPos.IsEmpty
					&& col < cell.MergeEndPos.Col
					&& col >= cell.MergeStartPos.Col)
				{
					continue;
				}

				if (this.cols[col].InnerWidth > 0)
				{
					break;
				}
			}

			return new CellPosition(pos.Row, col);
		}

		private CellPosition FindNextMovableCellRight(CellPosition pos, RangePosition moveRange, bool autoReturn = true)
		{
			int col = pos.Col;

			var endCol = this.selectionRange.Cols > 1 ? this.selectionRange.EndCol : this.cols.Count - 1;

			if (col >= endCol)
			{
				var newpos = FindNextMovableCellDown(new CellPosition(pos.Row, moveRange.Col), moveRange, false);
				if (pos == newpos) return pos;

				pos = newpos;
			}

			int row = pos.Row;

			// find next movable cell rightward
			while (col < moveRange.EndCol)
			{
				col++;

				var cell = this.cells[row, col];

				if (cell != null && !cell.MergeEndPos.IsEmpty
					&& col <= cell.MergeEndPos.Col
					&& col > cell.MergeStartPos.Col)
				{
					continue;
				}

				if (this.cols[col].InnerWidth > 0)
				{
					break;
				}
			}

			return new CellPosition(pos.Row, col);
		}

		private CellPosition FindNextMovableCellDown(CellPosition pos, RangePosition moveRange, bool autoReturn = true)
		{
			int row = pos.Row;

			// find next movable cell downward
			while (row < moveRange.EndRow)
			{
				row++;

				var cell = this.cells[row, pos.Col];

				if (cell != null && !cell.MergeEndPos.IsEmpty
					&& row <= cell.MergeEndPos.Row
					&& row > cell.MergeStartPos.Row)
				{
					continue;
				}

				if (this.rows[row].InnerHeight > 0)
				{
					break;
				}
			}

			return new CellPosition(row, pos.Col);
		}

		#endregion // Move Utility

		/// <summary>
		/// Move forward selection
		/// </summary>
		public void MoveSelectionForward()
		{
			if (SelectionMovedForward != null)
			{
				var arg = new SelectionMovedForwardEventArgs();
				SelectionMovedForward(this, arg);
				if (arg.IsCancelled)
				{
					return;
				}
			}

#if EX_SCRIPT
			var scriptReturn = RaiseScriptEvent("onnextfocus");
			if (scriptReturn != null && !ScriptRunningMachine.GetBoolValue(scriptReturn))
			{
				return;
			}
#endif

			switch (selectionForwardDirection)
			{
				case SelectionForwardDirection.Right:
					{
						if (this.selEnd.Col < this.cols.Count - 1)
						{
							MoveSelectionRight();
						}
						else
						{
							if (this.selEnd.Row < this.rows.Count - 1)
							{
								this.selEnd.Col = 0;
								MoveSelectionDown();
							}
						}
					}
					break;

				case SelectionForwardDirection.Down:
					{
						if (this.selEnd.Row < this.rows.Count - 1)
						{
							this.selEnd.Col = this.focusReturnColumn;

							MoveSelectionDown();
						}
						else
						{
							if (this.selEnd.Col < this.cols.Count - 1)
							{
								this.selEnd.Row = 0;
								MoveSelectionRight();
							}
						}
					}
					break;
			}
		}

		/// <summary>
		/// Move backward selection
		/// </summary>
		public void MoveSelectionBackward()
		{
			if (SelectionMovedBackward != null)
			{
				var arg = new SelectionMovedBackwardEventArgs();
				SelectionMovedBackward(this, arg);
				if (arg.IsCancelled)
				{
					return;
				}
			}

#if EX_SCRIPT
			var scriptReturn = RaiseScriptEvent("onpreviousfocus");
			if (scriptReturn != null && !ScriptRunningMachine.GetBoolValue(scriptReturn))
			{
				return;
			}
#endif

			switch (selectionForwardDirection)
			{
				case SelectionForwardDirection.Right:
					{
						if (selEnd.Col > 0)
						{
							MoveSelectionLeft();
						}
					}
					break;

				case SelectionForwardDirection.Down:
					{
						if (selEnd.Row > 0)
						{
							MoveSelectionUp();
						}
					}
					break;
			}
		}

		/// <summary>
		/// Upward to move focus selection
		/// </summary>
		/// <param name="appendSelect">Decide whether or not perform an appending select (same as Shift key press down)</param>
		public void MoveSelectionUp(bool appendSelect = false)
		{
			if (this.selectionMode == WorksheetSelectionMode.None)
			{
				return;
			}

			int row = selEnd.Row;

			// downward to find next movable cell
			while (row > 0)
			{
				row--;

				var cell = this.cells[row, selEnd.Col];

				if (cell != null && !cell.MergeEndPos.IsEmpty
					&& row < cell.MergeEndPos.Row
					&& row >= cell.MergeStartPos.Row)
				{
					continue;
				}

				if (this.rows[row].InnerHeight > 0)
				{
					MoveRangeSelection(selStart, new CellPosition(row, selEnd.Col), appendSelect);
					break;
				}
			}
		}

		/// <summary>
		/// Downward to move focus selection
		/// </summary>
		/// <param name="appendSelect">Decide whether or not perform an appending select (same as Shift key press down)</param>
		public void MoveSelectionDown(bool appendSelect = false)
		{
			if (this.selectionMode == WorksheetSelectionMode.None)
			{
				return;
			}

			int row = selEnd.Row;

			// downward to find next movable cell
			while (row < this.rows.Count - 1)
			{
				row++;

				var cell = this.cells[row, selEnd.Col];

				if (cell != null && !cell.MergeEndPos.IsEmpty
					&& row <= cell.MergeEndPos.Row
					&& row > cell.MergeStartPos.Row)
				{
					continue;
				}

				if (this.rows[row].InnerHeight > 0)
				{
					MoveRangeSelection(this.selStart, new CellPosition(row, selEnd.Col), appendSelect);
					break;
				}
			}
		}

		/// <summary>
		/// Leftward to move focus selection
		/// </summary>
		/// <param name="appendSelect">Decide whether or not perform an appending select (same as Shift key press down)</param>
		public void MoveSelectionLeft(bool appendSelect = false)
		{
			if (this.selectionMode == WorksheetSelectionMode.None)
			{
				return;
			}

			int col = selEnd.Col;

			// downward to find next movable cell
			while (col > 0)
			{
				col--;

				var cell = this.cells[selEnd.Row, col];

				if (cell != null && !cell.MergeEndPos.IsEmpty
					&& col < cell.MergeEndPos.Col
					&& col >= cell.MergeStartPos.Col)
				{
					continue;
				}

				if (this.cols[col].InnerWidth > 0)
				{
					//selEnd.Col = col;
					MoveRangeSelection(this.selStart, new CellPosition(selEnd.Row, col), appendSelect);
					break;
				}
			}
		}

		/// <summary>
		/// Rightward to move focus selection
		/// </summary>
		/// <param name="appendSelect">Decide whether or not perform an appending select (same as Shift key press down)</param>
		public void MoveSelectionRight(bool appendSelect = false)
		{
			if (this.selectionMode == WorksheetSelectionMode.None)
			{
				return;
			}

			int col = selEnd.Col;

			// downward to find next movable cell
			while (col < this.cols.Count - 1)
			{
				col++;

				var cell = this.cells[selEnd.Row, col];

				if (cell != null && !cell.MergeEndPos.IsEmpty
					&& col <= cell.MergeEndPos.Col
					&& col > cell.MergeStartPos.Col)
				{
					continue;
				}

				if (this.cols[col].InnerWidth > 0)
				{
					MoveRangeSelection(selStart, new CellPosition(this.selEnd.Row, col), appendSelect);
					break;
				}
			}
		}

		/// <summary>
		/// Move selection to first cell of row or column which is specified by <code>rowOrColumn</code>
		/// </summary>
		/// <param name="rowOrColumn">specifies that move selection to first cell of row or column</param>
		/// <param name="appendSelect">Decide whether or not perform an appending select (same as Shift key press down)</param>
		public void MoveSelectionHome(RowOrColumn rowOrColumn, bool appendSelect = false)
		{
			if (this.selectionMode == WorksheetSelectionMode.None)
			{
				return;
			}

			//bool selectionChanged = false;
			CellPosition endpos = this.selEnd;

			if ((rowOrColumn & RowOrColumn.Row) == RowOrColumn.Row)
			{
				endpos.Row = 0;
			}

			if ((rowOrColumn & RowOrColumn.Column) == RowOrColumn.Column)
			{
				endpos.Col = 0;
			}

			if (endpos != this.selEnd)
			{
				MoveRangeSelection(this.selStart, endpos, appendSelect);
			}
		}

		/// <summary>
		/// Move selection to last cell of row or column which is specified by <code>rowOrColumn</code>
		/// </summary>
		/// <param name="rowOrColumn">specifies that move selection to the cell of row or column</param>
		/// <param name="appendSelect">Determines that whether or not to expand the current selection.</param>
		public void MoveSelectionEnd(RowOrColumn rowOrColumn, bool appendSelect = false)
		{
			if (this.selectionMode == WorksheetSelectionMode.None)
			{
				return;
			}

			//bool selectionChanged = false;
			var endpos = this.selEnd;

			if ((rowOrColumn & RowOrColumn.Row) == RowOrColumn.Row)
			{
				endpos.Row = this.rows.Count - 1;
			}

			if ((rowOrColumn & RowOrColumn.Column) == RowOrColumn.Column)
			{
				endpos.Col = this.cols.Count - 1;
			}

			if (endpos != this.selEnd)
			{
				MoveRangeSelection(this.selStart, endpos, appendSelect);
			}
		}

		/// <summary>
		/// Move selection to cell in next page vertically.
		/// </summary>
		/// <param name="appendSelect">When this value is true, the selection will be expanded to the cell in next page rather than moving it.</param>
		public void MoveSelectionPageDown(bool appendSelect = false)
		{
			if (this.selectionMode == WorksheetSelectionMode.None)
			{
				return;
			}

			int row = selEnd.Row;

			NormalViewportController nvc = viewportController as NormalViewportController;

			if (nvc != null)
			{
				IViewport viewport = nvc.FocusView as IViewport;

				if (viewport != null)
				{
					row += Math.Max(viewport.VisibleRegion.Rows - 1, 1);
				}
			}

			var pos = this.FixPos(new CellPosition(row, selEnd.Col));

			var cell = this.cells[pos.Row, pos.Col];

			if (cell != null)
			{
				cell = GetMergedCellOfRange(pos.Row, selEnd.Col);
				pos = cell.Position;
			}

			MoveRangeSelection(this.selStart, pos, appendSelect);
		}

		/// <summary>
		/// Move selection to cell in previous page vertically.
		/// </summary>
		/// <param name="appendSelect">When this value is true, the selection will be expanded to the cell in previous page rather than moving it.</param>
		public void MoveSelectionPageUp(bool appendSelect = false)
		{
			if (this.selectionMode == WorksheetSelectionMode.None)
			{
				return;
			}

			int row = selEnd.Row;

			NormalViewportController nvc = viewportController as NormalViewportController;

			if (nvc != null)
			{
				IViewport viewport = nvc.FocusView as IViewport;

				if (viewport != null)
				{
					row -= Math.Max(viewport.VisibleRegion.Rows - 1, 1);
				}
			}

			var pos = this.FixPos(new CellPosition(row, selEnd.Col));

			var cell = this.cells[pos.Row, pos.Col];

			if (cell != null)
			{
				cell = GetMergedCellOfRange(pos.Row, selEnd.Col);
				pos = cell.Position;
			}

			MoveRangeSelection(this.selStart, pos, appendSelect);
		}

		#endregion // Keyboard Move

		#region Events

		/// <summary>
		/// Event raised before selection range changing
		/// </summary>
		public event EventHandler<BeforeSelectionChangeEventArgs> BeforeSelectionRangeChange;
		/// <summary>
		/// Event raised on focus-selection-range changed
		/// </summary>
		public event EventHandler<RangeEventArgs> SelectionRangeChanged;
		/// <summary>
		/// Event raised on focus-selection-range is changing by mouse move
		/// </summary>
		public event EventHandler<RangeEventArgs> SelectionRangeChanging;
		/// <summary>
		/// Event raised on Selection-Mode change
		/// </summary>
		public event EventHandler SelectionModeChanged;
		/// <summary>
		/// Event raised on Selection-Style change
		/// </summary>
		public event EventHandler SelectionStyleChanged;
		/// <summary>
		/// Event raised on SelectionForwardDirection change
		/// </summary>
		public event EventHandler SelectionForwardDirectionChanged;

		/// <summary>
		/// Event raised when focus-selection move to next position
		/// </summary>
		public event EventHandler<SelectionMovedForwardEventArgs> SelectionMovedForward;

		/// <summary>
		/// Event raised when focus-selection move to previous position
		/// </summary>
		public event EventHandler<SelectionMovedBackwardEventArgs> SelectionMovedBackward;

		#endregion // Events
	}
}
