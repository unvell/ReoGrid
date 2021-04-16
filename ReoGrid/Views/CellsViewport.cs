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

#if DEBUG
using System.Diagnostics;
#endif

#if EX_SCRIPT
using unvell.ReoScript;
using unvell.ReoGrid.Script;
#endif // EX_SCRIPT

#if WINFORM || ANDROID
using RGFloat = System.Single;
#else
using RGFloat = System.Double;
#endif // WINFORM

using unvell.Common;

#if WINFORM || WPF
using unvell.Common.Win32Lib;
#endif // WINFORM || WPF

using unvell.ReoGrid.Core;
using unvell.ReoGrid.Utility;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Events;
using unvell.ReoGrid.Actions;
using unvell.ReoGrid.Data;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Interaction;

namespace unvell.ReoGrid.Views
{
	class CellsViewport : Viewport, IRangeSelectableView
	{
		public CellsViewport(IViewportController vc)
			: base(vc)
		{
		}

		#region Draw

		#region DrawView
		public override void DrawView(CellDrawingContext dc)
		{
			if (sheet.rows.Count <= 0 || sheet.cols.Count < 0)
			{
				return;
			}

			if (

				// view mode
				(sheet.HasSettings(WorksheetSettings.View_ShowGridLine)
				&& dc.DrawMode == DrawMode.View
				// zoom < 40% will not display grid lines
				&& this.scaleFactor >= 0.4f
				)

#if PRINT
				// print/preview mode
				|| ((dc.DrawMode == DrawMode.Print || dc.DrawMode == DrawMode.Preview)
				&& sheet.PrintSettings != null && sheet.PrintSettings.ShowGridLines)
#endif // PRINT

				)
			{
				DrawGuideLines(dc);
			}

			DrawContent(dc);

			DrawSelection(dc);
		}
		#endregion // DrawView

		internal void DrawContent(CellDrawingContext dc)
		{
			#region Cells

			int toRow = visibleRegion.endRow + (dc.FullCellClip ? 0 : 1);
			int toCol = visibleRegion.endCol + (dc.FullCellClip ? 0 : 1);

			#region Background-only Cells
			var drawedCells = new System.Collections.Generic.List<Cell>(5);

			for (int r = visibleRegion.startRow; r < toRow; r++)
			{
				RowHeader rowHead = sheet.rows[r];
				if (rowHead.InnerHeight <= 0) continue;

				for (int c = visibleRegion.startCol; c < toCol;)
				{
					Cell cell = sheet.cells[r, c];

					if (cell == null)
					{
						DrawCellBackground(dc, r, c, cell);
						c++;
					}
					else if (string.IsNullOrEmpty(cell.DisplayText))
					{
						if (cell.Rowspan == 1 && cell.Colspan == 1)
						{
							DrawCellBackground(dc, r, c, cell);
							c++;
						}
						else if (
							cell.IsStartMergedCell
							|| (cell.IsEndMergedCell
								&& !visibleRegion.Contains(cell.MergeStartPos)))
						{
							DrawCellBackground(dc, cell.MergeStartPos.Row, cell.MergeStartPos.Col, sheet.GetCell(cell.MergeStartPos));
#if DEBUG
							Debug.Assert(cell.MergeEndPos.Col >= c);
#endif // DEBUG
							c = cell.MergeEndPos.Col + 1;

						}
						// merged cell is outside of visible region should also to be drawn
						else if (
							//cell.InternalRow == visibleRegion.startRow
							//&& cell.InternalCol == visibleRegion.startCol && 
							cell.Rowspan == 0 || cell.Colspan == 0
							&& (cell.MergeStartPos.Row < visibleRegion.startRow
							&& cell.MergeEndPos.Row > visibleRegion.endRow
							|| cell.MergeStartPos.Col < visibleRegion.startCol
							&& cell.MergeEndPos.Col > visibleRegion.endCol))
						{
							var mergedStartCell = sheet.GetCell(cell.MergeStartPos);

							if (!drawedCells.Contains(mergedStartCell))
							{
								DrawCellBackground(dc, mergedStartCell.Row, mergedStartCell.Column, mergedStartCell);
								drawedCells.Add(mergedStartCell);
							}

#if DEBUG
							Debug.Assert(cell.MergeEndPos.Col >= c);
#endif // DEBUG

							c = cell.MergeEndPos.Col + 1;
						}
						else
							c++;
					}
					else
						c++;
				}
			}

			#endregion // Background-only Cells

			#region Display Text Cells

			drawedCells.Clear();

			for (int r = visibleRegion.startRow; r < toRow && r <= sheet.cells.MaxRow; r++)
			{
				RowHeader rowHead = sheet.rows[r];
				if (rowHead.InnerHeight <= 0) continue;

				for (int c = visibleRegion.startCol; c < toCol && c <= sheet.cells.MaxCol;)
				{
					Cell cell = sheet.cells[r, c];

					// draw cell onyl when cell's instance existing
					// and bounds of cell must be > 1 (minimum is 1, including one pixel border)
					if (cell != null && cell.Width > 1 && cell.Height > 1)
					{
						bool hasContent = !string.IsNullOrEmpty(cell.DisplayText) || cell.body != null;

						// single cell
						if (cell.Rowspan == 1 && cell.Colspan == 1 && hasContent)
						{
							DrawCell(dc, cell);
							c++;
						}

						// merged cell start
						else if (cell.IsStartMergedCell && hasContent)
						{
							DrawCell(dc, cell);
							c = cell.MergeEndPos.Col + 1;
						}

						// merged cell end
						else if (cell.IsEndMergedCell
							&& !visibleRegion.Contains(cell.MergeStartPos)
							// don't check hasContent because it is the current cell,
							// we should check and draw merged start cell
							//&& hasContent
							)
						{
							var mergedStartCell = sheet.GetCell(cell.MergeStartPos);

							if (!string.IsNullOrEmpty(mergedStartCell.DisplayText) || mergedStartCell.body != null)
							{
								DrawCell(dc, sheet.GetCell(cell.MergeStartPos));
							}
							c = cell.MergeEndPos.Col + 1;
						}

						// merged cell is outside of visible region should also to be drawn
						else if (
							//cell.InternalRow == visibleRegion.startRow
							//&& cell.InternalCol == visibleRegion.startCol &&
							(cell.MergeStartPos.Row < visibleRegion.startRow
							&& cell.MergeEndPos.Row > visibleRegion.endRow
							|| cell.MergeStartPos.Col < visibleRegion.startCol
							&& cell.MergeEndPos.Col > visibleRegion.endCol))
						{
							var mergedStartCell = sheet.GetCell(cell.MergeStartPos);

							if (!drawedCells.Contains(mergedStartCell))
							{
								if (!string.IsNullOrEmpty(mergedStartCell.DisplayText) || mergedStartCell.body != null)
								{
									DrawCell(dc, mergedStartCell);
								}

								drawedCells.Add(mergedStartCell);
							}

							c = cell.MergeEndPos.Col + 1;
						}
						else c++;
					}
					else c++;
				}
			}
			#endregion // Display Text Cells

			#endregion // Cells

#if DEBUG
			Stopwatch sw = new Stopwatch();
			sw.Reset();
			sw.Start();
#endif // DEBUG

			#region Vertical Borders
			int rightColBoundary = visibleRegion.endCol + (dc.FullCellClip ? 0 : 1);

			for (int c = visibleRegion.startCol; c <= rightColBoundary; c++)
			{
				int x = c == sheet.cols.Count ? sheet.cols[c - 1].Right : sheet.cols[c].Left;

				if (c < sheet.cols.Count)
				{
					// skip invisible vertical borders
					var colHeader = sheet.cols[c];
					if (!colHeader.IsVisible) continue;
				}

				for (int r = visibleRegion.startRow; r <= visibleRegion.endRow;)
				{
					int y = r == sheet.rows.Count ? sheet.rows[r - 1].Bottom : sheet.rows[r].Top;

					ReoGridVBorder cellBorder = sheet.vBorders[r, c];
					if (cellBorder != null && cellBorder.Span > 0 && cellBorder.Style != null)
					{
						int endRow = r + Math.Min(cellBorder.Span - 1, visibleRegion.endRow);

						if (dc.FullCellClip && endRow >= visibleRegion.endRow - 1) endRow = visibleRegion.endRow - 1;

						int y2 = sheet.rows[endRow].Bottom;

						BorderPainter.Instance.DrawLine(dc.Graphics.PlatformGraphics, x * this.scaleFactor, y * this.scaleFactor,
							x * this.scaleFactor, y2 * this.scaleFactor, cellBorder.Style);

						r += cellBorder.Span;
					}
					else
						r++;
				}
			}
			#endregion

			#region Horizontal Borders
			int rightRowBoundary = visibleRegion.endRow + (dc.FullCellClip ? 0 : 1);

			for (int r = visibleRegion.startRow; r <= rightRowBoundary; r++)
			{
				if (r < sheet.rows.Count)
				{
					// skip invisible horizontal borders
					var rowHeader = sheet.rows[r];
					if (!rowHeader.IsVisible) continue;
				}

				int y = r == sheet.rows.Count ? sheet.rows[r - 1].Bottom : sheet.rows[r].Top;

				for (int c = visibleRegion.startCol; c <= visibleRegion.endCol;)
				{
					int x = c == sheet.cols.Count ? sheet.cols[c - 1].Right : sheet.cols[c].Left;

					ReoGridHBorder cellBorder = sheet.hBorders[r, c];
					if (cellBorder != null && cellBorder.Span > 0 && cellBorder.Style != null)
					{
						int endCol = c + Math.Min(cellBorder.Span - 1, visibleRegion.endCol);

						if (dc.FullCellClip && endCol >= visibleRegion.endCol - 1) endCol = visibleRegion.endCol - 1;

						int x2 = sheet.cols[endCol].Right;

						BorderPainter.Instance.DrawLine(dc.Graphics.PlatformGraphics, x * this.scaleFactor, y * this.scaleFactor,
							x2 * this.scaleFactor, y * this.scaleFactor, cellBorder.Style);

						c += cellBorder.Span;
					}
					else
						c++;
				}
			}
			#endregion

#if DEBUG
			sw.Stop();
			if (sw.ElapsedMilliseconds > 1000)
			{
				Debug.WriteLine($"draw border ({sw.ElapsedMilliseconds} ms.)");
			}
#endif // DEBUG

			#region View Mode Visible

			if (dc.DrawMode == DrawMode.View)
			{
				#region Print Breaks
#if PRINT
				if (this.sheet.HasSettings(WorksheetSettings.View_ShowPageBreaks)
					&& this.sheet.pageBreakRows != null && this.sheet.pageBreakCols != null
					&& this.sheet.pageBreakRows.Count > 0 && this.sheet.pageBreakCols.Count > 0)
				{
					RGFloat minX = this.sheet.cols[this.sheet.pageBreakCols[0]].Left * this.scaleFactor;
					RGFloat minY = this.sheet.rows[this.sheet.pageBreakRows[0]].Top * this.scaleFactor;
					RGFloat maxX = this.sheet.cols[this.sheet.pageBreakCols[this.sheet.pageBreakCols.Count - 1] - 1].Right * this.scaleFactor;
					RGFloat maxY = this.sheet.rows[this.sheet.pageBreakRows[this.sheet.pageBreakRows.Count - 1] - 1].Bottom * this.scaleFactor;

					foreach (int row in this.sheet.pageBreakRows)
					{
						RGFloat y = (row >= this.sheet.rows.Count ? this.sheet.rows[row - 1].Bottom : this.sheet.rows[row].Top) * this.scaleFactor;

						bool isUserPageSplitter = this.sheet.userPageBreakRows != null && this.sheet.userPageBreakRows.Contains(row);

						dc.Graphics.DrawLine(Math.Max(this.ScrollViewLeft * this.scaleFactor, minX), y,
							Math.Min(this.ScrollViewLeft * this.scaleFactor + bounds.Width, maxX), y,
							SolidColor.Blue, 2f, isUserPageSplitter ? LineStyles.Solid : LineStyles.Dash);
					}

					foreach (int col in this.sheet.pageBreakCols)
					{
						RGFloat x = (col >= this.sheet.cols.Count ? this.sheet.cols[col - 1].Right : this.sheet.cols[col].Left) * this.scaleFactor;

						bool isUserPageSplitter = this.sheet.userPageBreakCols != null && this.sheet.userPageBreakCols.Contains(col);

						dc.Graphics.DrawLine(x, Math.Max(this.ScrollViewTop * this.scaleFactor, minY),
							x, Math.Min(this.ScrollViewTop * this.scaleFactor + bounds.Height, maxY),
							SolidColor.Blue, 2f, isUserPageSplitter ? LineStyles.Solid : LineStyles.Dash);
					}
				}
#endif // PRINT
				#endregion // Print Breaks

				#region Break Lines Adjusting
				if (this.sheet.pageBreakAdjustCol > -1 && this.sheet.pageBreakAdjustFocusIndex > -1)
				{
					RGFloat x = 0;

					if (this.sheet.pageBreakAdjustFocusIndex < this.sheet.cols.Count)
						x = this.sheet.cols[this.sheet.pageBreakAdjustFocusIndex].Left;
					else
						x = this.sheet.cols[this.sheet.cols.Count - 1].Right;

					x *= this.scaleFactor;

					dc.Graphics.FillRectangle(HatchStyles.Percent50, SolidColor.Gray, SolidColor.Transparent,
						x - 1, ScrollViewTop * this.scaleFactor, 3, (ScrollViewTop + Height));
				}

				if (this.sheet.pageBreakAdjustRow > -1 && this.sheet.pageBreakAdjustFocusIndex > -1)
				{
					RGFloat y = 0;

					if (this.sheet.pageBreakAdjustFocusIndex < this.sheet.rows.Count)
					{
						y = this.sheet.rows[this.sheet.pageBreakAdjustFocusIndex].Top;
					}
					else
					{
						y = this.sheet.rows[this.sheet.rows.Count - 1].Bottom;
					}

					y *= this.scaleFactor;

					dc.Graphics.FillRectangle(HatchStyles.Percent50, SolidColor.Gray, SolidColor.Transparent,
						ScrollViewLeft * this.scaleFactor, y - 1, (ScrollViewLeft + Width), 3);
				}
				#endregion // Break Lines Adjusting

				#region Highlight & Focus Ranges
				if (sheet.highlightRanges != null)
				{
					foreach (var range in sheet.highlightRanges)
					{
						// is visible?
						if (range.HighlightColor.A > 0 && visibleRegion.IsOverlay(range))
						{
							DrawHighlightRange(dc, range);
						}
					}
				}

				var focusHR = sheet.focusHighlightRange;

				if (focusHR != null && focusHR.HighlightColor.A > 0
					&& visibleRegion.IsOverlay(focusHR))
				{
					var rect = GetScaledAndClippedRangeRect(this, focusHR.StartPos, focusHR.EndPos, 1f);
					rect.Inflate(-1, -1);

					dc.Renderer.DrawRunningFocusRect(rect.X, rect.Y, rect.Right, rect.Bottom,
						focusHR.HighlightColor, focusHR.RunnerOffset);

					focusHR.RunnerOffset += 2;

					if (focusHR.RunnerOffset > 9)
					{
						focusHR.RunnerOffset = 0;
					}
				}

				#endregion // Highlight & Focus Ranges
			}

			#endregion // View Mode Visible

			#region Trace Precedents & Dependents
#if FORMULA
			if (sheet.traceDependentArrows != null && sheet.traceDependentArrows.Count > 0)
			{
				var r = dc.Renderer;

				RGFloat ellipseSize = 4 * this.scaleFactor;
				RGFloat halfOfEllipse = ellipseSize / 2 + 1;

				r.BeginCappedLine(LineCapStyles.Ellipse, new Size(ellipseSize - 1, ellipseSize - 1),
					 LineCapStyles.Arrow, new Size(halfOfEllipse, ellipseSize), SolidColor.Blue, 1);

				foreach (var fromCell in sheet.traceDependentArrows.Keys)
				{
					var lines = sheet.traceDependentArrows[fromCell];

					foreach (var pl in lines)
					{
						if (visibleRegion.Contains(fromCell.InternalPos)
							&& visibleRegion.Contains(pl.InternalPos))
						{
							Point startPoint = GetScaledTracePoint(fromCell.InternalPos);
							Point endPoint = GetScaledTracePoint(pl.InternalPos);

							r.DrawCappedLine(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
						}
					}
				}

				r.EndCappedLine();
			}
#endif // FORMULA
			#endregion // Trace Precedents & Dependents
		}

		#region Clip Utility
		private Point GetScaledTracePoint(CellPosition startPos)
		{
			Rectangle startBounds = sheet.GetCellBounds(startPos);

			Point startPoint = startBounds.Location;
			startPoint.X += Math.Min(startBounds.Width, 30) / 2;
			startPoint.Y += Math.Min(startBounds.Height, 30) / 2;
			startPoint.X *= this.scaleFactor;
			startPoint.Y *= this.scaleFactor;

			return startPoint;
		}

		internal static Rectangle GetScaledAndClippedRangeRect(IViewport view, CellPosition startPos, CellPosition endPos, float borderWidth)
		{
			var sheet = view.ViewportController.Worksheet;
			Rectangle rangeRect = sheet.GetRangeBounds(startPos, endPos);

			var rowHead = sheet.rows[startPos.Row];
			var colHead = sheet.cols[startPos.Col];
			var toRowHead = sheet.rows[endPos.Row];
			var toColHead = sheet.cols[endPos.Col];

			int width = toColHead.Right - colHead.Left;
			int height = toRowHead.Bottom - rowHead.Top;

			Rectangle scaledRangeRect = new Rectangle(
					(colHead.Left) * view.ScaleFactor,
					(rowHead.Top) * view.ScaleFactor,
					((width) * view.ScaleFactor),
					((height) * view.ScaleFactor));

			return GetClippedRangeRect(view, scaledRangeRect, borderWidth);
		}

		static Rectangle GetClippedRangeRect(IViewport view, Rectangle scaledRangeRect, float borderWidth)
		{
			RGFloat scaledViewTop = view.ScrollViewTop * view.ScaleFactor;
			RGFloat scaledViewLeft = view.ScrollViewLeft * view.ScaleFactor;

			RGFloat viewBottom = view.Height + scaledViewTop + borderWidth; // 3: max select range border overflow
			RGFloat viewRight = view.Width + scaledViewLeft + borderWidth;

			// top
			if (scaledRangeRect.Y < scaledViewTop - borderWidth)
			{
				RGFloat h = scaledRangeRect.Height - scaledViewTop + scaledRangeRect.Y + borderWidth;
				if (h < 0) h = 0;
				scaledRangeRect.Height = h;
				scaledRangeRect.Y = scaledViewTop - borderWidth;
			}

			// left
			if (scaledRangeRect.X < scaledViewLeft - borderWidth)
			{
				RGFloat w = scaledRangeRect.Width - scaledViewLeft + scaledRangeRect.X + borderWidth;
				if (w < 0) w = 0;
				scaledRangeRect.Width = w;
				scaledRangeRect.X = scaledViewLeft - borderWidth;
			}

			// bottom
			if (scaledRangeRect.Bottom > viewBottom)
			{
				RGFloat h = viewBottom - scaledRangeRect.Y;
				if (h < 0) h = 0;
				scaledRangeRect.Height = h;
			}

			// right
			if (scaledRangeRect.Right > viewRight)
			{
				RGFloat w = viewRight - scaledRangeRect.X;
				if (w < 0) w = 0;
				scaledRangeRect.Width = w;
			}

			return scaledRangeRect;
		}
		#endregion // Clip Utility

		#region Draw Highlight Range
		private void DrawHighlightRange(CellDrawingContext dc, HighlightRange range)
		{
			var g = dc.Graphics;

			SolidColor color = range.HighlightColor;
			float weight = range.Hover ? 2f : 1f;

			// convert to view rectangle
			Rectangle scaledRange = sheet.GetScaledRangeBounds(range);
			Rectangle clippedRange = GetClippedRangeRect(this, scaledRange, weight);

			g.DrawRectangle(clippedRange, color, weight, LineStyles.Solid);

			g.FillRectangle(scaledRange.X - 1, scaledRange.Y - 1, 5, 5, color);
			g.FillRectangle(scaledRange.Right - 3, scaledRange.Y - 1, 5, 5, color);
			g.FillRectangle(scaledRange.X - 1, scaledRange.Bottom - 3, 5, 5, color);
			g.FillRectangle(scaledRange.Right - 3, scaledRange.Bottom - 3, 5, 5, color);

		}
		#endregion // Draw Highlight Range

		#region Draw Guide Lines
		private void DrawGuideLines(CellDrawingContext dc)
		{
			var render = dc.Renderer;

			int endRow = visibleRegion.endRow + (dc.FullCellClip ? 0 : 1);
			int endCol = visibleRegion.endCol + (dc.FullCellClip ? 0 : 1);

			render.BeginDrawLine(1, sheet.controlAdapter.ControlStyle.Colors[ControlAppearanceColors.GridLine]);

			#region Horizontal line
			// horizontal line
			for (int r = visibleRegion.startRow; r <= endRow; r++)
			{
				float y = r >= sheet.rows.Count ? sheet.rows[sheet.rows.Count - 1].Bottom : sheet.rows[r].Top;
				RGFloat scaledY = y * this.scaleFactor;

				for (int c = visibleRegion.startCol; c < endCol; c++)
				{
					// skip horizontal border - line start
					ReoGridHBorder cellBorder = null;

					int x = sheet.cols[c].Left;
					int x2 = x;// sheet.cols[c].Right;

					// skip horizontal border - line end
					while (c < endCol)
					{
						cellBorder = sheet.hBorders[r, c];

						if (cellBorder != null && cellBorder.Span >= 0)
						{
							break;
						}

						if (r > 0)
						{
							Cell cell = this.sheet.cells[r, c];

							if (cell != null && cell.InnerStyle.BackColor.A > 0)
							{
								break;
							}
						}

						c++;
					}

					x2 = (c == 0 ? x : sheet.cols[c - 1].Right);
					render.DrawLine(x * this.scaleFactor, scaledY, x2 * this.scaleFactor, scaledY);
				}
			}
			#endregion // Horizontal line

			#region Vertical line
			// vertical line
			for (int c = visibleRegion.startCol; c <= endCol; c++)
			{
				float x = c == sheet.cols.Count ? sheet.cols[c - 1].Right : sheet.cols[c].Left;
				RGFloat scaledX = x * this.scaleFactor;

				for (int r = visibleRegion.startRow; r < endRow; r++)
				{
					ReoGridVBorder cellBorder = null;

					int y = sheet.rows[r].Top;
					int y2 = y;// sheet.rows[r].Bottom;

					while (r < endRow)
					{
						cellBorder = sheet.vBorders[r, c];

						if (cellBorder != null && cellBorder.Span >= 0)
						{
							break;
						}

						if (c > 0)
						{
							Cell cell = this.sheet.cells[r, c];
							if (cell != null && cell.InnerStyle.BackColor.A > 0)
							{
								break;
							}
						}

						r++;
					}

					y2 = r == 0 ? y : sheet.rows[r - 1].Bottom;
					render.DrawLine(scaledX, y * this.scaleFactor, scaledX, y2 * this.scaleFactor);
				}
			}
			#endregion // Vertical line

			render.EndDrawLine();
		}
		#endregion // Draw Gridlines

		#region Draw Cells

		#region DrawCell Entry
		private void DrawCell(CellDrawingContext dc, Cell cell)
		{
			if (cell == null) return;

			if (cell.IsMergedCell && (cell.Width <= 1 || cell.Height <= 1))
			{
				return;
			}

			if (cell.body != null)
			{
				dc.Cell = cell;

				var g = dc.Graphics;

				g.PushTransform();

				if (this.scaleFactor != 1f)
				{
					g.ScaleTransform(this.scaleFactor, this.scaleFactor);
				}

				g.TranslateTransform(dc.Cell.Left, dc.Cell.Top);

				cell.body.OnPaint(dc);

				g.PopTransform();
			}
			else
			{
				if (!string.IsNullOrEmpty(cell.DisplayText))
				{
					DrawCellBackground(dc, cell.InternalRow, cell.InternalCol, cell);

					DrawCellText(dc, cell);
				}
			}
		}
		#endregion // DrawCell Entry

		#region DrawCell Text
		internal void DrawCellText(CellDrawingContext dc, Cell cell)
		{
			var g = dc.Graphics;

#if FORMULA && DRAWING
			var rt = cell.Data as Drawing.RichText;

			if (rt != null)
			{
			#region Rich Text

			#region Determine clip region
				bool needWidthClip = (cell.IsMergedCell || cell.InnerStyle.TextWrapMode == TextWrapMode.WordBreak);

				if (!needWidthClip && dc.AllowCellClip)
				{
					if (cell.InternalCol < this.sheet.cols.Count - 1)
					{
						if (cell.RenderHorAlign == ReoGridRenderHorAlign.Left
							|| cell.RenderHorAlign == ReoGridRenderHorAlign.Center)
						{
							var nextCell = sheet.cells[cell.InternalRow, cell.InternalCol + 1];

							needWidthClip = nextCell != null
								&& rt.TextSize.Width > cell.Width
								&& !string.IsNullOrEmpty(nextCell.DisplayText);
						}
					}

					if (!needWidthClip
						&& cell.InternalCol > 0
						&& (cell.RenderHorAlign == ReoGridRenderHorAlign.Right
							|| cell.RenderHorAlign == ReoGridRenderHorAlign.Center))
					{
						var prevCell = sheet.cells[cell.InternalRow, cell.InternalCol - 1];

						needWidthClip = prevCell != null
							&& rt.TextSize.Width > cell.Width
							&& !string.IsNullOrEmpty(prevCell.DisplayText);
					}
				}
			#endregion // Determine clip region

				var rtBounds = cell.Bounds * this.scaleFactor;

				//if (!needWidthClip)
				//{
				//	if (cell.InnerStyle != null)
				//	{
				//		var horAlign = cell.InnerStyle.HasStyle(PlainStyleFlag.HorizontalAlign)
				//			? cell.InnerStyle.HAlign : ReoGridHorAlign.General;

				//		switch (horAlign)
				//		{
				//			case ReoGridHorAlign.General:
				//			case ReoGridHorAlign.Left:
				//				break;

				//			case ReoGridHorAlign.Center:
				//				rtBounds.X += (cell.Width - rt.TextSize.Width) / 2;
				//				break;

				//			case ReoGridHorAlign.Right:
				//				rtBounds.X = (cell.Right - rt.TextSize.Width) - 4;
				//				break;
				//		}
				//	}

				//	rtBounds.Width = rt.TextSize.Width + 2;
				//}

				rt.Draw(g, rtBounds);
			#endregion // Rich Text
			}
			else
#endif // FORMULA && DRAWING
			{
				#region Plain Text

				#region Determine text color
				SolidColor textColor;

				if (!cell.RenderColor.IsTransparent)
				{
					// render color, used to render negative number, specified by data formatter
					textColor = cell.RenderColor;
				}
				else if (cell.InnerStyle.HasStyle(PlainStyleFlag.TextColor))
				{
					// cell text color, specified by SetRangeStyle
					textColor = cell.InnerStyle.TextColor;
				}
				// default cell text color
				else if (!sheet.controlAdapter.ControlStyle.TryGetColor(ControlAppearanceColors.GridText, out textColor))
				{
					// default built-in text
					textColor = SolidColor.Black;
				}

				if (cell.FontDirty)
				{
					sheet.UpdateCellFont(cell);
				}

				#endregion // Determine text color

				#region Determine clip region

				RGFloat cellScaledWidth = cell.Width * this.scaleFactor;
				RGFloat cellScaledHeight = (float)Math.Floor(cell.Height * this.scaleFactor) - 1;

				Rectangle clipRect = new Rectangle(this.ScrollViewLeft * this.scaleFactor, cell.Top * this.scaleFactor, this.Width, cellScaledHeight);

				bool needWidthClip = cell.IsMergedCell || cell.InnerStyle.TextWrapMode == TextWrapMode.WordBreak || dc.AllowCellClip;

				if (!needWidthClip)
				{
					if (cell.InternalCol < this.sheet.cols.Count - 1)
					{
						if (cell.RenderHorAlign == ReoGridRenderHorAlign.Left
							|| cell.RenderHorAlign == ReoGridRenderHorAlign.Center)
						{
							var nextCell = sheet.cells[cell.InternalRow, cell.InternalCol + 1];

							needWidthClip = nextCell != null
								&& cell.TextBounds.Right > cell.Right
								&& !string.IsNullOrEmpty(nextCell.DisplayText);
						}
					}

					if (!needWidthClip
						&& cell.InternalCol > 0
						&& (cell.RenderHorAlign == ReoGridRenderHorAlign.Right
							|| cell.RenderHorAlign == ReoGridRenderHorAlign.Center))
					{
						var prevCell = sheet.cells[cell.InternalRow, cell.InternalCol - 1];

						needWidthClip = prevCell != null
							&& prevCell.TextBounds.Left < cell.Left
							&& !string.IsNullOrEmpty(prevCell.DisplayText);
					}
				}

				if (needWidthClip)
				{
					clipRect = cell.Bounds;
					clipRect.X *= this.scaleFactor;
					clipRect.Y *= this.scaleFactor;
					clipRect.Width = cellScaledWidth;
					clipRect.Height = cellScaledHeight;
				}
				else
				{
					needWidthClip = cell.TextBoundsHeight > cellScaledHeight;
				}

				if (needWidthClip)
				{
					g.PushClip(clipRect);

					//dc.Renderer.DrawRectangle(cell.PrintTextBounds, SolidColor.Blue);
				}

				#endregion // Determine clip region

				dc.Renderer.DrawCellText(cell, textColor, dc.DrawMode, this.scaleFactor);

				#region clip region
				if (needWidthClip)
				{
					dc.Graphics.PopClip();
				}
				#endregion clip region

				#endregion // Plain Text
			}
		}
		#endregion // DrawCell Text

		#region DrawCell Background
		internal void DrawCellBackground(CellDrawingContext dc, int row, int col, Cell cell, bool refPosition = false)
		{
			WorksheetRangeStyle style;

			if (cell == null)
			{
				StyleParentKind pKind = StyleParentKind.Own;
				style = StyleUtility.FindCellParentStyle(this.sheet, row, col, out pKind);
			}
			else
			{
				style = cell.InnerStyle;
			}

			if (style.BackColor.A > 0)
			{
				var startPos = new CellPosition(row, col);

				Rectangle rect = cell == null ?
					GetScaledAndClippedRangeRect(this, startPos, startPos, 1)
					: GetScaledAndClippedRangeRect(this, startPos,
					new CellPosition(row + cell.Rowspan - 1, col + cell.Colspan - 1), 1);

				if (cell != null && refPosition)
				{
					rect.Location = new Point(0, 0);
				}

				if (rect.Width > 0 && rect.Height > 0)
				{
					var g = dc.Graphics;

					if (style.FillPatternColor.A > 0)
					{
						g.FillRectangle(style.FillPatternStyle, style.FillPatternColor, style.BackColor, rect);
					}
					else
					{
						g.FillRectangle(rect, style.BackColor);
					}
				}
			}
		}
		#endregion // DrawCell Background

		#endregion // Draw Cells

		#region Draw Selection
		private void DrawSelection(CellDrawingContext dc)
		{
			// selection
			if (!sheet.SelectionRange.IsEmpty
				&& dc.DrawMode == DrawMode.View
				&& sheet.SelectionStyle != WorksheetSelectionStyle.None)
			{
				var g = dc.Graphics;
				var controlStyle = sheet.workbook.controlAdapter.ControlStyle;

				var selectionBorderWidth = controlStyle.SelectionBorderWidth;

				var scaledSelectionRect = GetScaledAndClippedRangeRect(this,
					sheet.SelectionRange.StartPos, sheet.SelectionRange.EndPos, selectionBorderWidth);

				if (scaledSelectionRect.Width > 0 || scaledSelectionRect.Height > 0)
				{
					SolidColor selectionFillColor = controlStyle.Colors[ControlAppearanceColors.SelectionFill];

					if (sheet.SelectionStyle == WorksheetSelectionStyle.Default)
					{
						var range = this.sheet.GetRangeIfMergedCell(this.sheet.focusPos);
						var scaledFocusPosRect = GetScaledAndClippedRangeRect(this, range.StartPos, range.EndPos, 0);

						SolidColor selectionBorderColor = controlStyle.Colors[ControlAppearanceColors.SelectionBorder];

#if WINFORM
						if (sheet.FocusPosStyle == FocusPosStyle.Default)
						{
							g.PushClip(g.PlatformGraphics.ClipBounds);
							g.PlatformGraphics.SetClip(scaledFocusPosRect, System.Drawing.Drawing2D.CombineMode.Exclude);
							g.FillRectangle(scaledSelectionRect, selectionFillColor);
							g.PopClip();
						}
						else
						{
							g.FillRectangle(scaledSelectionRect, selectionFillColor);
						}
#elif WPF
						g.FillRectangle(scaledSelectionRect, selectionFillColor);
#endif // WPF

						if (selectionBorderColor.A > 0)
						{
							g.DrawRectangle(scaledSelectionRect, selectionBorderColor, selectionBorderWidth, LineStyles.Solid);
						}
					}
					else if (this.sheet.SelectionStyle == WorksheetSelectionStyle.FocusRect)
					{
						g.DrawRectangle(scaledSelectionRect, SolidColor.Black, 1, LineStyles.Dot);
					}

					if (sheet.HasSettings(WorksheetSettings.Edit_DragSelectionToFillSerial))
					{
						var sheetBackColor = controlStyle.Colors[ControlAppearanceColors.GridBackground];

						var thumbRect = new Rectangle(scaledSelectionRect.Right - selectionBorderWidth,
							scaledSelectionRect.Bottom - selectionBorderWidth,
							selectionBorderWidth + 2, selectionBorderWidth + 2);

						g.DrawRectangle(thumbRect, sheetBackColor);
					}
				}
			}
		}
		#endregion // Draw Selection

		#endregion // Draw

		#region Mouse

		public override bool OnMouseDown(Point location, MouseButtons buttons)
		{
			bool isProcessed = false;

			if (!isProcessed
				&& sheet.selectionMode != WorksheetSelectionMode.None
				&& !sheet.HasSettings(WorksheetSettings.Edit_Readonly))
			{
				if (!isProcessed
					&& sheet.HasSettings(WorksheetSettings.Edit_DragSelectionToFillSerial))
				{
					#region Hit Selection Drag
					if (CellsViewport.SelectDragCornerHitTest(this.sheet, location))
					{
						this.sheet.operationStatus = OperationStatus.DragSelectionFillSerial;
						this.sheet.lastMouseMoving = location;
						this.sheet.draggingSelectionRange = sheet.selectionRange;
						this.sheet.focusMovingRangeOffset = sheet.selectionRange.EndPos;

						this.sheet.RequestInvalidate();
						isProcessed = true;
					}
					#endregion // Hit Selection Drag
				}

				if (!isProcessed
					&& sheet.HasSettings(WorksheetSettings.Edit_DragSelectionToMoveCells))
				{
					#region Hit Selection Move
					Rectangle selBounds = sheet.GetRangePhysicsBounds(sheet.selectionRange);
					selBounds.Width--;
					selBounds.Height--;

					if (GraphicsToolkit.PointOnRectangleBounds(selBounds, location, 2 / this.scaleFactor))
					{
						sheet.draggingSelectionRange = sheet.selectionRange;
						sheet.operationStatus = OperationStatus.SelectionRangeMovePrepare;

						// set offset position (from selection left-top corner to mouse current location)
						var pos = CellsViewport.GetPosByPoint(this, location);

						// set offset
						sheet.lastMouseMoving.Y = sheet.selectionRange.Row;
						sheet.lastMouseMoving.X = sheet.selectionRange.Col;

						// make hover position inside selection range
						if (pos.Row < sheet.selectionRange.Row) pos.Row = sheet.selectionRange.Row;
						if (pos.Col < sheet.selectionRange.Col) pos.Col = sheet.selectionRange.Col;
						if (pos.Row > sheet.selectionRange.EndRow) pos.Row = sheet.selectionRange.EndRow;
						if (pos.Col > sheet.selectionRange.EndCol) pos.Col = sheet.selectionRange.EndCol;

						// set offset
						sheet.focusMovingRangeOffset.Row = pos.Row - sheet.selectionRange.Row;
						sheet.focusMovingRangeOffset.Col = pos.Col - sheet.selectionRange.Col;

						sheet.RequestInvalidate();
						this.SetFocus();
						isProcessed = true;
					}
					#endregion // Hit Selection Move
				}
			}

			#region Hit Print Breaks
#if PRINT
			// process page break lines adjusting
			if (!isProcessed
				&& sheet.HasSettings(
				// when the page breaks are showing
				WorksheetSettings.View_ShowPageBreaks |
				// when the user inserting or adjusting the page breaks is allowed
				WorksheetSettings.Behavior_AllowUserChangingPageBreaks)
				//&& !sheet.HasSettings(WorksheetSettings.Edit_Readonly)
				)
			{
				int splitCol = sheet.FindBreakIndexOfColumnByPixel(location);
				if (splitCol >= 0)
				{
					sheet.pageBreakAdjustCol = splitCol;
					sheet.pageBreakAdjustFocusIndex = sheet.pageBreakCols[splitCol];
					sheet.lastMouseMoving.X = sheet.pageBreakAdjustFocusIndex;

					sheet.operationStatus = OperationStatus.AdjustPageBreakColumn;
					sheet.RequestInvalidate();
					this.SetFocus();
					isProcessed = true;
				}

				if (!isProcessed)
				{
					int splitRow = sheet.FindBreakIndexOfRowByPixel(location);
					if (splitRow >= 0)
					{
						sheet.pageBreakAdjustRow = splitRow;
						sheet.pageBreakAdjustFocusIndex = sheet.pageBreakRows[splitRow];
						sheet.lastMouseMoving.Y = sheet.pageBreakAdjustFocusIndex;

						sheet.operationStatus = OperationStatus.AdjustPageBreakRow;
						sheet.RequestInvalidate();
						this.SetFocus();
						isProcessed = true;
					}
				}
			}
#endif // PRINT
			#endregion // Hit Print Breaks

			#region Hit Cells
			if (!isProcessed)
			{
				int row = CellsViewport.GetRowByPoint(this, location.Y);
#if DEBUG
				Debug.Assert(row >= 0 && row < sheet.rows.Count);
#endif // DEBUG

				if (row != -1) // in valid rows
				{
					int col = CellsViewport.GetColByPoint(this, location.X);
#if DEBUG
					Debug.Assert(col >= 0 && col < sheet.cols.Count);
#endif // DEBUG

					if (col != -1) // in valid cols
					{
						CellPosition pos = new CellPosition(row, col);

						var cell = sheet.cells[row, col];

						if (cell != null || sheet.HasCellMouseDown)
						{
							if (cell != null && !cell.IsValidCell)
							{
								cell = sheet.GetMergedCellOfRange(cell);
							}

							if ((cell != null && cell.body != null) || sheet.HasCellMouseDown)
							{
								var cellRect = sheet.GetCellBounds(pos);

								var evtArg = new CellMouseEventArgs(sheet, cell, pos, new Point(
									location.X - cellRect.Left,
									location.Y - cellRect.Top), location, buttons, 1);

								sheet.RaiseCellMouseDown(evtArg);

								if (cell != null && cell.body != null)
								{
									if (cell.body.OnMouseDown(evtArg))
									{
										isProcessed = true;

										// if cell body has processed any mouse down event,
										// it is necessary to cancel double click event to Control instance.
										//
										// this flag use to notify Control to ignore the double click event once.
										sheet.IgnoreMouseDoubleClick = true;

										sheet.RequestInvalidate();

										if (cell.body.AutoCaptureMouse() || evtArg.Capture)
										{
											this.SetFocus();
											sheet.mouseCapturedCell = cell;
											sheet.operationStatus = OperationStatus.CellBodyCapture;
										}
									}
								}
							}
						}

#if EX_SCRIPT
						object scriptReturn = sheet.RaiseScriptEvent("onmousedown", RSUtility.CreatePosObject(pos));
						if (scriptReturn != null && !ScriptRunningMachine.GetBoolValue(scriptReturn))
						{
							return true;
						}
#endif // EX_SCRIPT

						if (!isProcessed)
						{
							this.SetFocus();

							#region Range Selection
							//else
							// do not change focus cell if selection mode is null
							if (sheet.selectionMode != WorksheetSelectionMode.None)
							{
								if (
									// mouse left button to start new selection session
									buttons == MouseButtons.Left
									// or mouse right button to show context-menu, that starts also new selection session
									|| !sheet.selectionRange.Contains(row, col))
								{
									// if mouse left button pressed, change operation status to free range selection
									switch (sheet.selectionMode)
									{
										case WorksheetSelectionMode.Row:
											sheet.operationStatus = OperationStatus.FullRowSelect;
											break;

										case WorksheetSelectionMode.SingleRow:
											sheet.operationStatus = OperationStatus.FullSingleRowSelect;
											break;

										case WorksheetSelectionMode.Column:
											sheet.operationStatus = OperationStatus.FullColumnSelect;
											break;

										case WorksheetSelectionMode.SingleColumn:
											sheet.operationStatus = OperationStatus.FullSingleColumnSelect;
											break;

										case WorksheetSelectionMode.Range:
											sheet.operationStatus = OperationStatus.RangeSelect;
											break;
									}

									sheet.SelectRangeStartByMouse(this.PointToController(location));
								}
							}
							#endregion // Cell Selection

							if (buttons == MouseButtons.Right)
							{
								sheet.controlAdapter.ShowContextMenuStrip(ViewTypes.None, PointToController(location));
							}

							// block other processes
							isProcessed = true;
						}
					}
				}
			}
			#endregion // Hit Cells

			return isProcessed;
		}

		public override bool OnMouseMove(Point location, MouseButtons buttons)
		{
			bool isProcessed = false;

			switch (sheet.operationStatus)
			{
				case OperationStatus.CellBodyCapture:
					#region Cell Body Capture
					if (sheet.mouseCapturedCell != null && sheet.mouseCapturedCell.body != null)
					{
						int rowTop = sheet.rows[sheet.mouseCapturedCell.InternalRow].Top;
						int colLeft = sheet.cols[sheet.mouseCapturedCell.InternalCol].Left;

						var evtArg = new CellMouseEventArgs(sheet, sheet.mouseCapturedCell, new Point(
								(location.X - colLeft),
								(location.Y - rowTop)), location, buttons, 1);

						isProcessed = sheet.mouseCapturedCell.body.OnMouseMove(evtArg);
					}
					#endregion // Cell Body Capture
					break;

				case OperationStatus.Default:
					#region Default Cells Hover
					{
						bool cursorChanged = false;

						if (sheet.selectionMode != WorksheetSelectionMode.None
							&& !sheet.HasSettings(WorksheetSettings.Edit_Readonly))
						{
							#region Hover - Check to drag serial
							if (sheet.HasSettings(WorksheetSettings.Edit_DragSelectionToFillSerial))
							{
								if (CellsViewport.SelectDragCornerHitTest(this.sheet, location))
								{
									sheet.controlAdapter.ChangeCursor(CursorStyle.Cross);
									cursorChanged = true;
									isProcessed = true;
								}
							}
							#endregion // Hover - Check to drag selection

							#region Hover - Check to move selection
							if (!isProcessed
								&& sheet.HasSettings(WorksheetSettings.Edit_DragSelectionToMoveCells))
							{
								Rectangle selBounds = sheet.GetRangePhysicsBounds(sheet.SelectionRange);
								selBounds.Width--;
								selBounds.Height--;

								bool selHover = (GraphicsToolkit.PointOnRectangleBounds(selBounds, location, 2 / this.scaleFactor));

								if (selHover)
								{
									sheet.controlAdapter.ChangeCursor(CursorStyle.Move);
									cursorChanged = true;
									isProcessed = true;
								}
							}
							#endregion // Hover - Selection range

							#region Hover - Highlight ranges
							// process highlight range hover
							if (!isProcessed
								&& sheet.highlightRanges != null)
							{
								foreach (var refRange in sheet.highlightRanges)
								{
									Rectangle scaledRangeRect = sheet.GetRangePhysicsBounds(refRange);
									scaledRangeRect.Width--;
									scaledRangeRect.Height--;

									bool hover = (GraphicsToolkit.PointOnRectangleBounds(scaledRangeRect, location, 2f));

									if (hover)
									{
										sheet.controlAdapter.ChangeCursor(CursorStyle.Move);
										cursorChanged = true;
									}

									if (hover != refRange.Hover)
									{
										refRange.Hover = hover;
										isProcessed = true;
										sheet.RequestInvalidate();
										break;
									}
								}
							}
							#endregion // Hover - Highlight ranges
						}

						#region Cell Hover - Page breaks
#if PRINT
						// process page break lines hover
						if (!isProcessed
							&& sheet.HasSettings(
							// when the page breaks are showing
							WorksheetSettings.View_ShowPageBreaks |
							// when the user inserting or adjusting page breaks is allowed
							WorksheetSettings.Behavior_AllowUserChangingPageBreaks)
							//&& !sheet.HasSettings(WorksheetSettings.Edit_Readonly)
							)
						{
							int splitCol = sheet.FindBreakIndexOfColumnByPixel(location);
							if (splitCol >= 0)
							{
								sheet.controlAdapter.ChangeCursor(CursorStyle.ResizeHorizontal);
								cursorChanged = true;
								isProcessed = true;
							}

							int splitRow = sheet.FindBreakIndexOfRowByPixel(location);
							if (splitRow >= 0)
							{
								sheet.controlAdapter.ChangeCursor(CursorStyle.ResizeVertical);
								cursorChanged = true;
								isProcessed = true;
							}
						}
#endif // PRINT
						#endregion // Hover - Page breaks

						#region Cell Hover - Cells
						// process cells hover
						if (!isProcessed)
						{
							CellPosition newHoverPos = CellsViewport.GetPosByPoint(this, location);
							if (newHoverPos != sheet.hoverPos)
							{
								sheet.HoverPos = newHoverPos;
							}

							if (!sheet.hoverPos.IsEmpty)
							{
								var cell = sheet.cells[sheet.hoverPos.Row, sheet.hoverPos.Col];

								if (cell != null || sheet.HasCellMouseMove)
								{
									if (cell != null && !cell.IsValidCell)
									{
										cell = sheet.GetMergedCellOfRange(cell);
									}

									if ((cell != null && cell.body != null) || sheet.HasCellMouseMove)
									{
										var cellRect = sheet.GetCellBounds(sheet.hoverPos);

										var evtArg = new CellMouseEventArgs(sheet, cell, sheet.hoverPos, new Point(
												(location.X - cellRect.Left),
												(location.Y - cellRect.Top)), location, buttons, 1);

										sheet.RaiseCellMouseMove(evtArg);

										if (cell != null && cell.body != null)
										{
											cell.body.OnMouseMove(evtArg);
										}

										if (evtArg.CursorStyle != CursorStyle.PlatformDefault)
										{
											cursorChanged = true;
											sheet.controlAdapter.ChangeCursor(evtArg.CursorStyle);
										}
									}
								}
							}

							if (!cursorChanged)
							{
								sheet.controlAdapter.ChangeCursor(CursorStyle.Selection);
							}
						}
						#endregion // Cell Hover - Cells
					}
					#endregion // Default Cells Hover
					break;

				case OperationStatus.AdjustColumnWidth:
				case OperationStatus.AdjustRowHeight:
					// do nothing
					break;

				case OperationStatus.SelectionRangeMovePrepare:
					#region Ready to move selection
					// prepare to move selection
					if (buttons == MouseButtons.Left
						&& sheet.HasSettings(WorksheetSettings.Edit_DragSelectionToMoveCells)
						&& sheet.draggingSelectionRange != RangePosition.Empty)
					{
						sheet.operationStatus = OperationStatus.SelectionRangeMove;
						isProcessed = true;
					}
					#endregion // Ready to move selection
					break;

				case OperationStatus.SelectionRangeMove:
					#region Selection Range Move
					{
						var pos = CellsViewport.GetPosByPoint(this, location);

						// reuse lastMoseMoving (compare Point to ReoGridPos)
						if (sheet.lastMouseMoving.Y != pos.Row || sheet.lastMouseMoving.X != pos.Col)
						{
							sheet.lastMouseMoving.Y = pos.Row;
							sheet.lastMouseMoving.X = pos.Col;

							sheet.draggingSelectionRange = new RangePosition(pos.Row - sheet.focusMovingRangeOffset.Row,
								pos.Col - sheet.focusMovingRangeOffset.Col,
								sheet.selectionRange.Rows, sheet.selectionRange.Cols);

							if (sheet.draggingSelectionRange.Row < 0) sheet.draggingSelectionRange.Row = 0;
							if (sheet.draggingSelectionRange.Col < 0) sheet.draggingSelectionRange.Col = 0;

							// keep range inside spreadsheet - row
							if (sheet.draggingSelectionRange.EndRow >= sheet.RowCount)
							{
								sheet.draggingSelectionRange.Row = sheet.RowCount - sheet.draggingSelectionRange.Rows;
							}

							// keep range inside spreadsheet - col
							if (sheet.draggingSelectionRange.EndCol >= sheet.ColumnCount)
							{
								sheet.draggingSelectionRange.Col = sheet.ColumnCount - sheet.draggingSelectionRange.Cols;
							}

							sheet.ScrollToCell(pos);
							sheet.RequestInvalidate();
						}

						isProcessed = true;
					}
					#endregion // Selection Range Move
					break;

				case OperationStatus.DragSelectionFillSerial:
					#region Selection Range Drag
					{
						var pos = CellsViewport.GetPosByPoint(this, location);

						// reuse lastMoseMoving (compare Point to ReoGridPos)
						if (sheet.focusMovingRangeOffset != pos)
						{
							sheet.focusMovingRangeOffset = pos;

							int minRow = Math.Min(sheet.selectionRange.Row, pos.Row);
							int minCol = Math.Min(sheet.selectionRange.Col, pos.Col);
							int maxRow = Math.Max(sheet.selectionRange.EndRow, pos.Row);
							int maxCol = Math.Max(sheet.selectionRange.EndCol, pos.Col);

							var selLoc = sheet.GetRangePhysicsBounds(sheet.selectionRange);

							bool horizontal = true;

							if (location.X <= selLoc.X)
							{
								if (location.Y < selLoc.Y)
								{
									// left top
									horizontal = Math.Abs(selLoc.X - location.X) > Math.Abs(selLoc.Y - location.Y);
								}
								else if (location.Y > selLoc.Bottom)
								{
									// left bottom
									horizontal = Math.Abs(selLoc.X - location.X) > Math.Abs(selLoc.Bottom - location.Y);
								}
							}
							else if (location.X >= selLoc.Right)
							{
								if (location.Y < selLoc.Y)
								{
									// right top
									horizontal = Math.Abs(selLoc.Right - location.X) > Math.Abs(selLoc.Y - location.Y);
								}
								else if (location.Y > selLoc.Bottom)
								{
									// right bottom
									horizontal = Math.Abs(selLoc.Right - location.X) > Math.Abs(selLoc.Bottom - location.Y);
								}
							}
							else
							{
								horizontal = false;
							}

							if (horizontal)
							{
								minRow = sheet.selectionRange.Row;
								maxRow = sheet.selectionRange.EndRow;
							}
							else
							{
								minCol = sheet.selectionRange.Col;
								maxCol = sheet.selectionRange.EndCol;
							}

							sheet.draggingSelectionRange = sheet.FixRange(new RangePosition(minRow, minCol, maxRow - minRow + 1, maxCol - minCol + 1));
							sheet.ScrollToCell(pos);

							sheet.RequestInvalidate();
						}

						isProcessed = true;
					}
					#endregion // Selection Range Drag
					break;

#if PRINT
				case OperationStatus.AdjustPageBreakRow:
				#region Page Break Row
					if (buttons == MouseButtons.Left
						&& sheet.pageBreakAdjustRow > -1)
					{
						int rowIndex = sheet.FindRowIndexMiddle(location.Y);
						int index = sheet.FixPageBreakRowIndex(sheet.pageBreakAdjustRow, rowIndex);

						if (sheet.lastMouseMoving.Y != index)
						{
							sheet.pageBreakAdjustFocusIndex = index;
							sheet.lastMouseMoving.Y = index;

							sheet.RequestInvalidate();
						}

						isProcessed = true;
					}
				#endregion // Page Break Row
					break;

				case OperationStatus.AdjustPageBreakColumn:
				#region Page Break Column
					if (buttons == MouseButtons.Left
						&& sheet.pageBreakAdjustCol > -1)
					{
						int colIndex = sheet.FindColIndexMiddle(location.X);
						int index = sheet.FixPageBreakColIndex(sheet.pageBreakAdjustCol, colIndex);

						if (sheet.lastMouseMoving.X != index)
						{
							sheet.pageBreakAdjustFocusIndex = index;
							sheet.lastMouseMoving.X = index;

							sheet.RequestInvalidate();
						}

						isProcessed = true;
					}
				#endregion // Page Break Column
					break;
#endif // PRINT

				case OperationStatus.RangeSelect:
				case OperationStatus.FullRowSelect:
				case OperationStatus.FullColumnSelect:
					#region Range Select
					if (buttons == MouseButtons.Left)
					{
						var sp = this.PointToController(location);
						sheet.SelectRangeEndByMouse(sp);
					}
					#endregion // Range Select
					break;

				default:
					sheet.controlAdapter.ChangeCursor(CursorStyle.PlatformDefault);
					break;
			}

			return isProcessed;
		}

		public override bool OnMouseUp(Point location, MouseButtons buttons)
		{
			bool isProcessed = false;

			switch (sheet.operationStatus)
			{
				case OperationStatus.CellBodyCapture:
					#region CellBodyCapture
					if (sheet.mouseCapturedCell != null)
					{
						if (sheet.mouseCapturedCell.body != null)
						{
							int rowTop = sheet.rows[sheet.mouseCapturedCell.InternalRow].Top;
							int colLeft = sheet.cols[sheet.mouseCapturedCell.InternalCol].Left;

							var evtArg = new CellMouseEventArgs(sheet, sheet.mouseCapturedCell, new Point(
									(location.X - colLeft),
									(location.Y - rowTop)), location, buttons, 1);

							isProcessed = sheet.mouseCapturedCell.body.OnMouseUp(evtArg);

							if (isProcessed)
							{
								sheet.RequestInvalidate();
							}
						}
						sheet.mouseCapturedCell = null;
					}
					sheet.operationStatus = OperationStatus.Default;
					#endregion // CellBodyCapture
					break;

				case OperationStatus.SelectionRangeMovePrepare:
					#region Abort Selection Range Move
					sheet.operationStatus = OperationStatus.Default;
					sheet.RequestInvalidate();
					isProcessed = true;
					#endregion // Abort Selection Range Move
					break;

				case OperationStatus.SelectionRangeMove:
					#region Submit Selection Range Move
					if (sheet.selectionRange != sheet.draggingSelectionRange)
					{
						var fromRange = sheet.selectionRange;
						var toRange = sheet.draggingSelectionRange;

						try
						{
							if (this.sheet.CheckRangeReadonly(fromRange))
							{
								throw new RangeContainsReadonlyCellsException(fromRange);
							}

							if (this.sheet.CheckRangeReadonly(toRange))
							{
								throw new RangeContainsReadonlyCellsException(toRange);
							}

							BaseWorksheetAction action;

							if (PlatformUtility.IsKeyDown(KeyCode.ControlKey))
							{
								action = new CopyRangeAction(fromRange, toRange.StartPos);
							}
							else
							{
								action = new MoveRangeAction(fromRange, toRange.StartPos);
							}

							sheet.DoAction(action);
						}
						catch (Exception ex)
						{
							sheet.NotifyExceptionHappen(ex);
						}
					}

					sheet.focusMovingRangeOffset = CellPosition.Empty;
					sheet.draggingSelectionRange = RangePosition.Empty;
					sheet.operationStatus = OperationStatus.Default;
					sheet.controlAdapter.ChangeCursor(CursorStyle.PlatformDefault);
					sheet.RequestInvalidate();
					isProcessed = true;
					#endregion // Submit Selection Range Move
					break;

#if FORMULA
				case OperationStatus.DragSelectionFillSerial:
					#region Submit Selection Drag
					sheet.operationStatus = OperationStatus.Default;

					if (sheet.draggingSelectionRange.Rows > sheet.selectionRange.Rows
						|| sheet.draggingSelectionRange.Cols > sheet.selectionRange.Cols)
					{
						RangePosition targetRange = RangePosition.Empty;

						if (sheet.draggingSelectionRange.Rows == sheet.selectionRange.Rows)
						{
							targetRange = new RangePosition(
								sheet.draggingSelectionRange.Row,
								sheet.draggingSelectionRange.Col + sheet.selectionRange.Cols,
								sheet.draggingSelectionRange.Rows,
								sheet.draggingSelectionRange.Cols - sheet.selectionRange.Cols);
						}
						else if (sheet.draggingSelectionRange.Cols == sheet.selectionRange.Cols)
						{
							targetRange = new RangePosition(
								sheet.draggingSelectionRange.Row + sheet.selectionRange.Rows,
								sheet.draggingSelectionRange.Col,
								sheet.draggingSelectionRange.Rows - sheet.selectionRange.Rows,
								sheet.draggingSelectionRange.Cols);
						}

						if (targetRange != RangePosition.Empty)
						{
							sheet.DoAction(new AutoFillSerialAction(sheet.SelectionRange, targetRange));
						}
					}

					sheet.RequestInvalidate();
					isProcessed = true;
					#endregion // Submit Selection Drag
					break;
#endif // FORMULA

				case OperationStatus.RangeSelect:
				case OperationStatus.FullRowSelect:
				case OperationStatus.FullColumnSelect:
					#region Change Selection Range
					{
						CellPosition pos = CellsViewport.GetPosByPoint(this, location);

						if (sheet.lastChangedSelectionRange != sheet.selectionRange)
						{
							sheet.lastChangedSelectionRange = sheet.selectionRange;
							sheet.selEnd = pos;

#if WINFORM || WPF
							//if (sheet.controlAdapter.ControlInstance is IRangePickableControl)
							//{
							if (sheet.whenRangePicked != null)
							{
								if (sheet.whenRangePicked(sheet, sheet.selectionRange))
								{
									sheet.EndPickRange();
								}
							}
							//}
#endif // WINFORM || WPF

							sheet.RaiseSelectionRangeChanged(new RangeEventArgs(sheet.selectionRange));

#if EX_SCRIPT
							object scriptReturn = sheet.RaiseScriptEvent("onmouseup", RSUtility.CreatePosObject(sheet.selEnd));

							// run if script return true or nothing
							if (scriptReturn == null || ScriptRunningMachine.GetBoolValue(scriptReturn))
							{
								sheet.RaiseScriptEvent("onselectionchange");
							}
#endif // EX_SCRIPT
						}

						{
							int row = pos.Row;
							int col = pos.Col;

							var cell = sheet.cells[row, col];

							if ((cell != null && cell.body != null) || sheet.HasCellMouseUp)
							{
								int rowTop = sheet.rows[row].Top;
								int colLeft = sheet.cols[col].Left;

								var evtArg = new CellMouseEventArgs(sheet, cell, pos, new Point(
									(location.X - colLeft),
									(location.Y - rowTop)), location, buttons, 1);

								sheet.RaiseCellMouseUp(evtArg);

								if (cell != null && cell.body != null)
								{
									isProcessed = cell.body.OnMouseUp(evtArg);
								}
							}
						}

						sheet.operationStatus = OperationStatus.Default;
						isProcessed = true;
					}
					#endregion // Change Selection Range
					break;

#if PRINT
				case OperationStatus.AdjustPageBreakColumn:
				#region Adjust Page Break Column
					if (sheet.pageBreakAdjustCol > -1)
					{
						//SetPageBreakColIndex(this.pageBreakAdjustCol, this.commonMouseMoveColIndex);

						int oldIndex = sheet.pageBreakCols[sheet.pageBreakAdjustCol];

						if (oldIndex >= 0)
						{
							if (oldIndex != sheet.pageBreakAdjustFocusIndex)
							{
								sheet.ChangeColumnPageBreak(oldIndex, sheet.pageBreakAdjustFocusIndex);
							}
							else
							{
								sheet.RequestInvalidate();
							}
						}
					}
					sheet.pageBreakAdjustCol = -1;
					sheet.pageBreakAdjustFocusIndex = -1;
					sheet.operationStatus = OperationStatus.Default;
					isProcessed = true;
				#endregion // Adjust Page Break Column
					break;

				case OperationStatus.AdjustPageBreakRow:
				#region Adjust Page Break Row
					if (sheet.pageBreakAdjustRow > -1)
					{
						int oldIndex = sheet.pageBreakRows[sheet.pageBreakAdjustRow];

						if (oldIndex >= 0)
						{
							if (oldIndex != sheet.pageBreakAdjustFocusIndex)
							{
								sheet.ChangeRowPageBreak(oldIndex, sheet.pageBreakAdjustFocusIndex);
							}
							else
							{
								sheet.RequestInvalidate();
							}
						}
					}
					sheet.pageBreakAdjustRow = -1;
					sheet.pageBreakAdjustFocusIndex = -1;
					sheet.operationStatus = OperationStatus.Default;
					isProcessed = true;
				#endregion // Adjust Page Break Row
					break;
#endif // PRINT

				default:
					#region Call Event CellMouseUp
					{
						CellPosition pos = CellsViewport.GetPosByPoint(this, location);

						int row = pos.Row;
						int col = pos.Col;

						var cell = sheet.cells[row, col];

						if ((cell != null && cell.body != null) || sheet.HasCellMouseUp)
						{
							int rowTop = sheet.rows[row].Top;
							int colLeft = sheet.cols[col].Left;

							var evtArg = new CellMouseEventArgs(sheet, cell, pos, new Point(
								(location.X - colLeft),
								(location.Y - rowTop)), location, buttons, 1);

							sheet.RaiseCellMouseUp(evtArg);

							if (cell != null && cell.body != null)
							{
								isProcessed = cell.body.OnMouseUp(evtArg);
							}
						}
					}
					#endregion // Call Event CellMouseUp
					break;
			}

			return isProcessed;
		}

		#region DoubleClick
		public override bool OnMouseDoubleClick(Point location, MouseButtons buttons)
		{
			if (!sheet.focusPos.IsEmpty)
			{
				var pos = CellsViewport.GetPosByPoint(this, location);

				if (!pos.IsEmpty
					&& pos.Row < sheet.rows.Count
					&& pos.Col < sheet.cols.Count)
				{
					var cell = sheet.cells[pos.Row, pos.Col];

					if (cell != null && !cell.IsValidCell)
					{
						pos = cell.MergeStartPos;
					}
				}

				if (this.sheet.focusPos == pos)
				{
					this.sheet.StartEdit();

					this.sheet.controlAdapter.EditControlApplySystemMouseDown();

					return true;
				}
			}

			return false;
		}
		#endregion // DoubleClick

		#endregion // Mouse

		#region Utility
		internal static int GetColByPoint(IViewport view, RGFloat x)
		{
			Worksheet sheet = view.ViewportController.Worksheet;

			if (sheet.cols.Count <= 0 || x < sheet.cols[0].Right) return 0;

			var visibleRegion = view.VisibleRegion;

			// view only contain one column
			if (visibleRegion.endCol <= visibleRegion.startCol)
			{
				return visibleRegion.startCol;
			}

			// binary search to find the column which contains the give position
			return ArrayHelper.QuickFind((visibleRegion.endCol - visibleRegion.startCol + 1) / 2,
				0, sheet.cols.Count - 1, i =>
				{
					var colHeader = sheet.cols[i];

					if (colHeader.Right < x)
						return 1;
					else if (colHeader.Left > x)
						return -1;
					else
						return 0;
				});
		}

		internal static int GetRowByPoint(IViewport view, RGFloat y)
		{
			Worksheet sheet = view.ViewportController.Worksheet;

			if (sheet.rows.Count <= 0 || y < sheet.rows[0].Bottom) return 0;

			var visibleRegion = view.VisibleRegion;

			// view only contain one row
			if (visibleRegion.endRow <= visibleRegion.startRow)
			{
				return visibleRegion.startRow;
			}

#if DEBUG
			Stopwatch sw = Stopwatch.StartNew();
			try
			{
#endif
			// binary search to find the row which contains the give position
			return ArrayHelper.QuickFind((visibleRegion.endRow - visibleRegion.startRow + 1) / 2,
				0, sheet.rows.Count - 1, i =>
				{
					var rowHeader = sheet.rows[i];

					if (rowHeader.Bottom < y)
						return 1;
					else if (rowHeader.Top > y)
						return -1;
					else
						return 0;
				});

#if DEBUG
			}
			finally
			{
				sw.Stop();
				long ms = sw.ElapsedMilliseconds;
				if (ms > 1)
				{
					Debug.WriteLine("finding row index takes " + ms + " ms.");
				}
			}
#endif
		}

		public static CellPosition GetPosByPoint(IViewport view, Point p)
		{
			return new CellPosition(GetRowByPoint(view, p.Y), GetColByPoint(view, p.X));
		}

		/// <summary>
		/// Transform position of specified cell into the position on control
		/// </summary>
		/// <param name="view">Source view of the specified cell position.</param>
		/// <param name="pos">Cell position to be converted.</param>
		/// <param name="p">Output point of the cell position related to grid control.</param>
		/// <returns>True if conversion is successful; Otherwise return false.</returns>
		public static bool TryGetCellPositionToControl(IView view, CellPosition pos, out Point p)
		{
			if (view == null)
			{
				p = new Point();
				return false;
			}

			var sheet = view.ViewportController.Worksheet;

			if (sheet == null)
			{
				p = new Point();
				return false;
			}

			pos = sheet.FixPos(pos);

			IViewport viewport = view as IViewport;

			if (viewport == null)
			{
				p = new Point(sheet.cols[pos.Col].Left * view.ScaleFactor + view.Left,
					sheet.rows[pos.Row].Top * view.ScaleFactor + view.Top);
			}
			else
			{
				p = new Point(sheet.cols[pos.Col].Left * view.ScaleFactor + viewport.Left - viewport.ScrollViewLeft * view.ScaleFactor,
					sheet.rows[pos.Row].Top * view.ScaleFactor + viewport.Top - viewport.ScrollViewTop * view.ScaleFactor);
			}

			return true;
		}

		internal static bool SelectDragCornerHitTest(Worksheet sheet, Point location)
		{
			Rectangle selBounds = sheet.GetRangePhysicsBounds(sheet.SelectionRange);
			selBounds.Width--;
			selBounds.Height--;

			var selectionBorderWidth = sheet.controlAdapter.ControlStyle.SelectionBorderWidth;

			var thumbRect = new Rectangle(selBounds.Right - selectionBorderWidth,
				selBounds.Bottom - selectionBorderWidth,
				selectionBorderWidth + 2, selectionBorderWidth + 2);

			return thumbRect.Contains(location);
		}

		public override string ToString()
		{
			return string.Format("CellsViewport[{0}]", this.ViewBounds);
		}
		#endregion // Utility
	}
}
