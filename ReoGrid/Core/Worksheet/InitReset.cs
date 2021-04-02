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

#if DEBUG
using System.Diagnostics;
#endif // DEBUG

#if WINFORM || WPF
//using CellArray = unvell.ReoGrid.Data.JaggedTreeArray<unvell.ReoGrid.ReoGridCell>;
//using HBorderArray = unvell.ReoGrid.Data.JaggedTreeArray<unvell.ReoGrid.Core.ReoGridHBorder>;
//using VBorderArray = unvell.ReoGrid.Data.JaggedTreeArray<unvell.ReoGrid.Core.ReoGridVBorder>;
using CellArray = unvell.ReoGrid.Data.Index4DArray<unvell.ReoGrid.Cell>;
using HBorderArray = unvell.ReoGrid.Data.Index4DArray<unvell.ReoGrid.Core.ReoGridHBorder>;
using VBorderArray = unvell.ReoGrid.Data.Index4DArray<unvell.ReoGrid.Core.ReoGridVBorder>;
#elif ANDROID || iOS
using CellArray = unvell.ReoGrid.Data.ReoGridCellArray;
using HBorderArray = unvell.ReoGrid.Data.ReoGridHBorderArray;
using VBorderArray = unvell.ReoGrid.Data.ReoGridVBorderArray;
#endif // ANDROID

using unvell.ReoGrid.Main;

namespace unvell.ReoGrid
{
	partial class Worksheet
	{

		private void InitGrid()
		{
			InitGrid(DefaultRows, DefaultCols);
		}

		private void InitGrid(int rows, int cols)
		{
#if DEBUG
			Stopwatch sw = Stopwatch.StartNew();
			Debug.WriteLine("start creating worksheet...");
#endif // DEBUG

			this.SuspendUIUpdates();

			// resize spreadsheet to specified size
			this.Resize(rows, cols);

			if (this.controlAdapter != null)
			{
				this.renderScaleFactor = this._scaleFactor + this.controlAdapter.BaseScale;

				var scv = this.viewportController as Views.IScalableViewportController;

				if (scv != null)
				{
					scv.ScaleFactor = this.renderScaleFactor;
				}
			}

			// restore root style
			this.RootStyle = new WorksheetRangeStyle(DefaultStyle);

			// initialize default settings
			this.settings = WorksheetSettings.Default;

			// reset selection 
			this.selectionRange = new RangePosition(0, 0, 1, 1);

#if PRINT
			// clear print settings
			if (this.printSettings != null) this.printSettings = null;
#endif // PRINT

#if DRAWING
			// drawing object
			this.drawingCanvas = new Drawing.WorksheetDrawingCanvas(this);
#endif // DRAWING

			this.ResumeUIUpdates();

			if (this.viewportController != null)
			{
				// reste viewport controller
				this.viewportController.Reset();
			}

#if EX_SCRIPT
			//settings |=
			//	// auto run script if loaded from file
			//		WorkbookSettings.Script_AutoRunOnload
			//	// confirm to user whether allow to run script after loaded from file
			//	| WorkbookSettings.Script_PromptBeforeAutoRun;

			//InitSRM();
			//this.worksheetObj = null;

			RaiseScriptEvent("onload");
#endif // EX_SCRIPT

#if DEBUG
			sw.Stop();
			long ms = sw.ElapsedMilliseconds;
			if (ms > 10)
			{
				Debug.WriteLine("creating worksheet done: " + ms + " ms.");
			}
#endif // DEBUG
		}

		internal void Clear()
		{
			// hidden edit textbox
			EndEdit(EndEditReason.Cancel);

			// clear editing flag 
			endEditProcessing = false;

			// reset ActionManager
			if (controlAdapter != null)
			{
				var actionSupportedControl = controlAdapter.ControlInstance as IActionControl;

        if (actionSupportedControl != null)
				{
					actionSupportedControl.ClearActionHistoryForWorksheet(this);
				}
			}

#if OUTLINE
			// clear row outlines
			if (this.outlines != null)
			{
				ClearOutlines(RowOrColumn.Row | RowOrColumn.Column);
			}
#endif // OUTLINE

			// clear named ranges
			registeredNamedRanges.Clear();

			// clear highlight ranges
			if (highlightRanges != null)
			{
				highlightRanges.Clear();
			}

#if PRINT
			// clear page breaks
			if (this.pageBreakRows != null) this.pageBreakRows.Clear();
			if (this.pageBreakCols != null) this.pageBreakCols.Clear();

			if (this.userPageBreakRows != null) this.userPageBreakRows.Clear();
			if (this.userPageBreakCols != null) this.userPageBreakCols.Clear();

			this.printableRange = RangePosition.Empty;
			this.printSettings = null;
#endif // PRINT

#if DRAWING
			// drawing objects
			if (this.drawingCanvas != null)
			{
				this.drawingCanvas.Children.Clear();
			}
#endif // DRAWING

			// reset default width and height 
			this.defaultColumnWidth = InitDefaultColumnWidth;
			this.defaultRowHeight = InitDefaultRowHeight;

			// clear root style
			this.RootStyle = new WorksheetRangeStyle(DefaultStyle);

			// clear focus highlight ranges
			this.FocusHighlightRange = null;

			// restore to default operation mode
			this.operationStatus = OperationStatus.Default;

			// restore settings
			this.settings = WorksheetSettings.Default;

			if (this.SettingsChanged != null)
			{
				this.SettingsChanged(this, null);
			}

#if FORMULA
			// clear formula referenced cells and ranges
			formulaRanges.Clear();

			// clear trace lines
			if (this.traceDependentArrows != null)
			{
				this.traceDependentArrows.Clear();
			}
#endif // FORMULA

#if EX_SCRIPT
			if (Srm != null)
			{
				RaiseScriptEvent("unload");
			}
#endif // EX_SCRIPT

			// unfreeze rows and columns
			CellPosition pos = this.FreezePos;
			if (pos.Row > 0 || pos.Col > 0)
			{
				Unfreeze();
			}

			if (this.viewportController != null)
			{
				// reset viewport controller
				viewportController.Reset();
			}

			// TODO: release objects inside cells and borders
			cells = new CellArray();
			hBorders = new HBorderArray();
			vBorders = new VBorderArray();

			// clear header & index
			this.rows.Clear();
			this.cols.Clear();

			// reset max row and column indexes
			this.maxRowHeader = -1;
			this.maxColumnHeader = -1;

			// reset highlight range color counter
			this.rangeHighlightColorCounter = 0;
		}

		/// <summary>
		/// Reset control to default status.
		/// </summary>
		public void Reset()
		{
			Reset(DefaultRows, DefaultCols);
		}

		/// <summary>
		/// Reset control and initialize to specified size
		/// </summary>
		/// <param name="rows">number of rows to be set after resting</param>
		/// <param name="cols">number of columns to be set after reseting</param>
		public void Reset(int rows, int cols)
		{
			// cancel editing mode
			this.EndEdit(EndEditReason.Cancel);

			// reset scale factor, need this?
			this._scaleFactor = 1f;

			// clear grid
			this.Clear();

			// clear all actions belongs to this worksheet
			if (this.controlAdapter != null)
			{
				var actionSupportedControl = this.controlAdapter.ControlInstance as IActionControl;

				if (actionSupportedControl != null)
				{
					actionSupportedControl.ClearActionHistoryForWorksheet(this);
				}
			}

			// restore default cell size
			this.defaultRowHeight = Worksheet.InitDefaultRowHeight;
			this.defaultColumnWidth = Worksheet.InitDefaultColumnWidth;

			// restore row header panel width
			this.userRowHeaderWidth = false;

			// restore UI
			this.settings = WorksheetSettings.View_Default;

			// init grid
			this.InitGrid(rows, cols);

			// repaint
			this.RequestInvalidate();

			// raise reseting event
			if (this.Resetted != null) this.Resetted(this, null);
		}

	}
}
