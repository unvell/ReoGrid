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
using System.Diagnostics;

#if WINFORM || ANDROID
using RGIntDouble = System.Int32;
using RGFloat = System.Single;

#elif WPF
using RGFloat = System.Double;
using RGIntDouble = System.Double;

#elif iOS
using RGFloat = System.Double;
using RGIntDouble = System.Double;

#endif // WPF

using unvell.Common;

using unvell.ReoGrid.Core;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Events;
using unvell.ReoGrid.Utility;
using unvell.ReoGrid.Graphics;

namespace unvell.ReoGrid.Core
{
	/// <summary>
	/// Cell horizontal alignment for render. (cell-auto-format)
	/// </summary>
	[Serializable]
	internal enum ReoGridRenderHorAlign
	{
		Left, Center, Right,
	}

	internal enum StyleParentKind : byte
	{
		Root,
		Col,
		Row,
		Range,
		Own,
	}

	enum UpdateFontReason
	{
		FontChanged,
		ScaleChanged,
		TextColorChanged,
	}
}

namespace unvell.ReoGrid
{
	partial class Worksheet
	{
		#region Set Style

		/// <summary>
		/// Set styles to each cells inside specified range
		/// </summary>
		/// <param name="addressOrName">address or name to locate the cell or range on spreadsheet</param>
		/// <param name="style">styles to be set</param>
		/// <exception cref="InvalidAddressException">throw if specified address or name is illegal</exception>
		public void SetRangeStyles(string addressOrName, WorksheetRangeStyle style)
		{
			NamedRange namedRange;

			if (RangePosition.IsValidAddress(addressOrName))
			{
				SetRangeStyles(new RangePosition(addressOrName), style);
			}
			else if (this.registeredNamedRanges.TryGetValue(addressOrName, out namedRange))
			{
				SetRangeStyles(namedRange, style);
			}
			else
				throw new InvalidAddressException(addressOrName);
		}

		/// <summary>
		/// Set styles to each cells inside specified range
		/// </summary>
		/// <param name="row">number of row of specified range</param>
		/// <param name="col">number of col of specified range</param>
		/// <param name="rows">number of rows inside specified range</param>
		/// <param name="cols">number of columns inside specified range</param>
		/// <param name="style">styles to be set</param>
		public void SetRangeStyles(int row, int col, int rows, int cols, WorksheetRangeStyle style)
		{
			this.SetRangeStyles(new RangePosition(row, col, rows, cols), style);
		}

		/// <summary>
		/// Set styles to each cells inside specified range
		/// </summary>
		/// <param name="range">specified range to the styles</param>
		/// <param name="style">styles to be set</param>
		public void SetRangeStyles(RangePosition range, WorksheetRangeStyle style)
		{
			if (this.currentEditingCell != null)
			{
				EndEdit(EndEditReason.NormalFinish);
			}

			RangePosition fixedRange = FixRange(range);

			int r1 = fixedRange.Row;
			int c1 = fixedRange.Col;
			int r2 = fixedRange.EndRow;
			int c2 = fixedRange.EndCol;

			bool isColStyle = fixedRange.Rows == this.rows.Count;
			bool isRowStyle = fixedRange.Cols == this.cols.Count;
			bool isRootStyle = isRowStyle && isColStyle;

			bool isRange = !isColStyle && !isRowStyle;

			int maxRow, maxCol;
			if (isColStyle && r2 > (maxRow = MaxContentRow)) r2 = maxRow;
			if (isRowStyle && c2 > (maxCol = MaxContentCol)) c2 = maxCol;

			StyleParentKind pkind = StyleParentKind.Own;

			// update default styles
			if (isRootStyle)
			{
				#region All headers style updating
				StyleUtility.CopyStyle(style, this.RootStyle);

				// update styles which has been set into row headers
				for (int r = 0; r < this.rows.Count; r++)
				{
					RowHeader rowHead = rows[r];

					if (rowHead != null && rowHead.InnerStyle != null)
					{
						StyleUtility.CopyStyle(style, rowHead.InnerStyle);
					}
				}

				// update styles which has been set into column headers
				for (int c = 0; c < this.cols.Count; c++)
				{
					ColumnHeader colHead = cols[c];

					//if (colHead != null && colHead.InnerStyle != null)
					//{
					//	unvell.ReoGrid.Utility.StyleUtility.CopyStyle(style, colHead.InnerStyle);
					//}
					if (colHead != null)
					{
						colHead.InnerStyle = null;
					}
				}
				#endregion

				pkind = StyleParentKind.Root;
			}
			else if (isRowStyle)
			{
				#region Rows in range updating
				// Rows in range updating
				for (int r = r1; r <= r2; r++)
				{
					var rowHeader = this.rows[r];

					if (rowHeader.InnerStyle == null)
					{
						rowHeader.InnerStyle = StyleUtility.CreateMergedStyle(style, this.RootStyle);
					}
					else
					{
						StyleUtility.CopyStyle(style, rowHeader.InnerStyle);
					}
				}
				#endregion // Rows in range updating

				pkind = StyleParentKind.Row;
			}
			else if (isColStyle)
			{
				#region Columns in range updating
				// Columns in range updating
				for (int c = c1; c <= c2; c++)
				{
					var colHeader = this.cols[c];

					if (colHeader.InnerStyle == null)
					{
						colHeader.InnerStyle = StyleUtility.CreateMergedStyle(style, RootStyle);
					}
					else
					{
						StyleUtility.CopyStyle(style, colHeader.InnerStyle);
					}
				}
				#endregion // Columns in range updating

				pkind = StyleParentKind.Col;
			}

			WorksheetRangeStyle rowStyle = null;
			WorksheetRangeStyle colStyle = null;

			// update cells
			for (int r = r1; r <= r2; r++)
			{
				rowStyle = null;

				for (int c = c1; c <= c2; c++)
				{
					Cell cell = cells[r, c];
					colStyle = null;

					if (cell != null)
					{
						if (
							cell.IsValidCell

							&&

							// if is a part of merged cell, check whether all rows or columns is selected
							// if all rows or columns is selected, skip set styles
							((!isRowStyle && !isColStyle)
							|| (r1 <= cell.InternalRow && r2 >= cell.MergeEndPos.Row
							&& c1 <= cell.InternalCol && c2 >= cell.MergeEndPos.Col))
							)
						{
							if (pkind == StyleParentKind.Row)
							{
								if (cell.StyleParentKind == StyleParentKind.Col)
								{
									SetCellStyle(cell, style, StyleParentKind.Own);
								}
								else
								{
									if (rowStyle == null)
									{
										rowStyle = this.rows[r].InnerStyle;
									}

									SetCellStyle(cell, style, pkind, rowStyle);
								}
							}
							else if (pkind == StyleParentKind.Col)
							{
								if (colStyle == null)
								{
									colStyle = this.cols[c].InnerStyle;
								}
								SetCellStyle(cell, style, pkind, colStyle);
							}
							else
							{
								SetCellStyle(cell, style, pkind, this.RootStyle);
							}
						}
					}
					else
					{

						// allow to create cells
						if (isRange)
						{
							cell = CreateCell(r, c, false);
							SetCellStyle(cell, style, StyleParentKind.Own);
						}
						// if full grid style then skip all null cells
						else if (isRootStyle)
						{
							continue;
						}
						// if the column of cell has styles, compare to row style
						else if (isColStyle)
						{
							if (rowStyle == null)
							{
								rowStyle = this.rows[r].InnerStyle;
							}

							// if row has style, then create cell, else skip creating null cell
							if (rowStyle != null)
							// full column selected but the row of cell has also style,
							// row style has the higher priority than the column style,
							// so it is need to create instance of cell to 
							// get highest priority for cell styles
							{
								SetCellStyle(CreateCell(r, c, false), style, StyleParentKind.Own);
							}
						}
					}
				}
			}

			if (RangeStyleChanged != null)
			{
				RangeStyleChanged(this, new RangeEventArgs(fixedRange));
			}

			RequestInvalidate();
		}

		internal void SetCellStyleOwn(Cell cell, WorksheetRangeStyle style)
		{
			this.SetCellStyle(cell, style, StyleParentKind.Own);
		}

		internal void SetCellStyleOwn(CellPosition pos, WorksheetRangeStyle style)
		{
			this.SetCellStyleOwn(pos.Row, pos.Col, style);
		}

		/// <summary>
		/// Set style to cell specified by row and col index
		/// </summary>
		/// <param name="row">index to row</param>
		/// <param name="col">index to col</param>
		/// <param name="style">style will be copied</param>
		internal void SetCellStyleOwn(int row, int col, WorksheetRangeStyle style)
		{
			this.SetCellStyle(CreateAndGetCell(row, col), style, StyleParentKind.Own);
		}

		private void SetCellStyle(Cell cell, WorksheetRangeStyle style,
			StyleParentKind parentKind, WorksheetRangeStyle parentStyle = null)
		{
			// do nothing if cell is a part of merged range
			if (cell.Rowspan == 0 || cell.Colspan == 0) return;

			if (cell.StyleParentKind == StyleParentKind.Own
			|| parentKind == StyleParentKind.Own)
			{
				if (cell.StyleParentKind != StyleParentKind.Own)
				{
					cell.CreateOwnStyle();
				}

				StyleUtility.CopyStyle(style, cell.InnerStyle);

				// auto remove fill pattern when pattern color is empty
				if ((cell.InnerStyle.Flag & PlainStyleFlag.FillPattern) == PlainStyleFlag.FillPattern
					&& cell.InnerStyle.FillPatternColor.ToArgb() == 0)
				{
					cell.InnerStyle.Flag &= ~PlainStyleFlag.FillPattern;
				}

				// auto remove background color when backcolor is empty
				if ((cell.InnerStyle.Flag & PlainStyleFlag.BackColor) == PlainStyleFlag.BackColor
					&& cell.InnerStyle.BackColor.ToArgb() == 0)
				{
					cell.InnerStyle.Flag &= ~PlainStyleFlag.BackColor;
				}
			}
			else
			{
				cell.InnerStyle = parentStyle != null ? parentStyle : style;
				cell.StyleParentKind = parentKind;
			}

			// update render text align when data format changed
			unvell.ReoGrid.Utility.StyleUtility.UpdateCellRenderAlign(this, cell);

			if (!string.IsNullOrEmpty(cell.DisplayText))
			{
				// when font changed, cell's scaled font need be updated.
				if (style.Flag.HasAny(PlainStyleFlag.FontAll))
				{
					// update cell font and text's bounds
					UpdateCellFont(cell);
				}
				// when font is not changed but alignment is changed, only update the bounds of text
				else if (style.Flag.HasAny(PlainStyleFlag.HorizontalAlign
					| PlainStyleFlag.VerticalAlign
					| PlainStyleFlag.TextWrap
					| PlainStyleFlag.Indent
					| PlainStyleFlag.RotationAngle))
				{
					UpdateCellTextBounds(cell);
				}
#if WPF
				else if (style.Flag.Has(PlainStyleFlag.TextColor))
				{
					UpdateCellFont(cell, UpdateFontReason.TextColorChanged);
				}
#endif // WPF
			}
			//else
			//{
			//	cell.RenderFont = null;
			//}

			// update cell bounds
			if (style.Flag.Has(PlainStyleFlag.Padding))
			{
				cell.UpdateContentBounds();
			}

			// update cell body alignment
			if (cell.body != null && style.Flag.HasAny(PlainStyleFlag.AlignAll))
			{
				cell.body.OnBoundsChanged();
			}
		}

		/// <summary>
		/// Event raised on style of range changed
		/// </summary>
		public event EventHandler<RangeEventArgs> RangeStyleChanged;

		#endregion // Set Style

		#region Remove Style

		/// <summary>
		/// Remove specified styles from a range specified by address or name
		/// </summary>
		/// <param name="addressOrName">Address or name to locate range from spreadsheet</param>
		/// <param name="flags">Styles specified by this flags to be removed</param>
		public void RemoveRangeStyles(string addressOrName, PlainStyleFlag flags)
		{
			if (RangePosition.IsValidAddress(addressOrName))
			{
				this.RemoveRangeStyles(new RangePosition(addressOrName), flags);
			}
			else if (this.registeredNamedRanges.TryGetValue(addressOrName, out var namedRange))
			{
				this.RemoveRangeStyles(namedRange, flags);
			}
			else
				throw new InvalidAddressException(addressOrName);
		}

		/// <summary>
		/// Remove specified styles from a specified range
		/// </summary>
		/// <param name="range">Range to be remove styles</param>
		/// <param name="flags">Styles specified by this flags to be removed</param>
		public void RemoveRangeStyles(RangePosition range, PlainStyleFlag flags)
		{
			RangePosition fixedRange = FixRange(range);

			int startRow = fixedRange.Row;
			int startCol = fixedRange.Col;
			int endRow = fixedRange.EndRow;
			int endCol = fixedRange.EndCol;

			bool isFullColSelected = fixedRange.Rows == this.rows.Count;
			bool isFullRowSelected = fixedRange.Cols == this.cols.Count;
			bool isFullGridSelected = isFullRowSelected && isFullColSelected;

			bool canCreateCell = !isFullColSelected && !isFullRowSelected;

			// update default styles
			if (isFullGridSelected)
			{
				this.RootStyle.Flag &= ~flags;

				// remote styles if it is already setted in full-row
				for (int r = 0; r < this.rows.Count; r++)
				{
					var rowStyle = rows[r].InnerStyle;
					if (rowStyle != null) rowStyle.Flag &= ~flags;
				}

				// remote styles if it is already setted in full-col
				for (int c = 0; c < this.cols.Count; c++)
				{
					var colStyle = cols[c].InnerStyle;
					if (colStyle != null) colStyle.Flag &= ~flags;
				}
			}
			else if (isFullRowSelected)
			{
				for (int r = startRow; r <= endRow; r++)
				{
					var rowStyle = rows[r].InnerStyle;
					if (rowStyle != null) rowStyle.Flag &= ~flags;
				}
			}
			else if (isFullColSelected)
			{
				for (int c = startCol; c <= endCol; c++)
				{
					var colStyle = cols[c].InnerStyle;
					if (colStyle != null) colStyle.Flag &= ~flags;
				}
			}

			for (int r = startRow; r <= endRow; r++)
			{
				for (int c = startCol; c <= endCol; )
				{
					var cell = this.cells[r, c];

					if (cell == null)
					{
						c++;
					}
					else if (cell.Rowspan == 1 && cell.Colspan == 1)
					{
						RemoveCellStyle(cell, flags);
						c++;
					}
					else if (cell.IsStartMergedCell
						// only set merged cell if selection contains the merged entire range
						&& startRow <= cell.MergeStartPos.Row && endRow >= cell.MergeEndPos.Row
						&& startCol <= cell.MergeStartPos.Col && endCol >= cell.MergeEndPos.Col)
					{
						RemoveCellStyle(cell, flags);
						c += cell.Colspan;
					}
					else if (!cell.MergeStartPos.IsEmpty)
					{
						c = cell.MergeEndPos.Col + 1;
					}
				}
			}

			RequestInvalidate();
		}

		private void RemoveCellStyle(Cell cell, PlainStyleFlag flags)
		{
			// backup cell flags, copy the items from parent style by this flags
			var pFlag = cell.StyleParentKind;

			// cell style references to root style
			if (pFlag == StyleParentKind.Root)
			{
				var distinctedStyle = StyleUtility.CheckDistinctStyle(this.RootStyle, Worksheet.DefaultStyle);

				if (distinctedStyle == PlainStyleFlag.None)
				{
					// root style doesn't have own styles, no need to remove any styles
					return;
				}
			}

			// Parent style of cell
			WorksheetRangeStyle pStyle = null;
			StyleParentKind newPKind = StyleParentKind.Root;

			RowHeader rowhead = this.rows[cell.Row];
			ColumnHeader colhead = this.cols[cell.Column];

			// find parent style
			if (rowhead.InnerStyle != null)
			{
				pStyle = rowhead.InnerStyle;
				newPKind = StyleParentKind.Row;
			}
			else
			{
				if (colhead.InnerStyle != null)
				{
					pStyle = colhead.InnerStyle;
					newPKind = StyleParentKind.Col;
				}
				else
				{
					pStyle = this.RootStyle;
					newPKind = StyleParentKind.Root;
				}
			}

			if (pFlag != StyleParentKind.Own)
			{
				cell.InnerStyle = new WorksheetRangeStyle(pStyle);
				cell.InnerStyle.Flag &= ~flags;
			}
			else
			{
				cell.InnerStyle.Flag &= ~flags;

				// cell with own styles all have been removed
				// restore the cell to parent reference
				if (cell.InnerStyle.Flag == PlainStyleFlag.None)
				{
					cell.InnerStyle = pStyle;
					cell.StyleParentKind = newPKind;

					return;
				}
				else
				{
					// remove style values
					if ((flags & PlainStyleFlag.BackColor) == PlainStyleFlag.BackColor)
					{
						cell.InnerStyle.BackColor = SolidColor.Transparent;
					}

				}
			}
			// remove style items by removing-flags

			switch (pFlag)
			{
				case StyleParentKind.Row:
					if (colhead.InnerStyle != null)
						pStyle = colhead.InnerStyle;
					else
						pStyle = this.RootStyle;
					break;

				default:
					pStyle = this.RootStyle;
					break;
			}

			// copy style items from parent cell
			// copy all items by cellFlags in order to restore the style items from parent
			var newFlags = (flags & pStyle.Flag);

			if (newFlags != PlainStyleFlag.None)
			{
				Utility.StyleUtility.CopyStyle(pStyle, cell.InnerStyle, newFlags);
			}

			// copy flags from parent style
			//cell.InnerStyle.Flag = pStyle.Flag;

			cell.StyleParentKind = StyleParentKind.Own;

			if ((flags & (/*PlainStyleFlag.AlignAll  // may don't need this  |*/
				PlainStyleFlag.TextWrap |
#if WINFORM || WPF || iOS
				PlainStyleFlag.FontAll
#elif ANDROID
				PlainStyleFlag.FontName | PlainStyleFlag.FontStyleAll
#endif // ANDROID
				)) > 0)
			{
				cell.FontDirty = true;
			}
		}

		#endregion // Remove Style

		#region Get Style

		/// <summary>
		/// Get style of specified range.
		/// </summary>
		/// <param name="range">The range to get style.</param>
		/// <returns>Style info of specified range.</returns>
		public WorksheetRangeStyle GetRangeStyles(RangePosition range)
		{
			RangePosition fixedRange = FixRange(range);

			return this.GetCellStyles(range.StartPos);
		}

		internal object GetRangeStyle(int row, int col, int rows, int cols, PlainStyleFlag flag)
		{
			// TODO: return range's style instead of cell 
			return GetCellStyleItem(row, col, flag);
		}

		/// <summary>
		/// Get style from cell by specified position.
		/// </summary>
		/// <param name="address">Address to locate a cell to get its style.</param>
		/// <returns>Style set of cell retrieved from specified position.</returns>
		public WorksheetRangeStyle GetCellStyles(string address)
		{
			if (!CellPosition.IsValidAddress(address))
			{
				throw new InvalidAddressException(address);
			}

			return this.GetCellStyles(new CellPosition(address));
		}

		/// <summary>
		/// Get style of single cell.
		/// </summary>
		/// <param name="pos">Position of cell to get.</param>
		/// <returns>Style of cell in the specified position.</returns>
		public WorksheetRangeStyle GetCellStyles(CellPosition pos)
		{
			return GetCellStyles(pos.Row, pos.Col);
		}

		/// <summary>
		/// Get style of specified cell without creating its instance.
		/// </summary>
		/// <param name="row">Index of row of specified cell.</param>
		/// <param name="col">Index of column of specified cell.</param>
		/// <returns>Style of cell from specified position.</returns>
		public WorksheetRangeStyle GetCellStyles(int row, int col)
		{
			Cell cell = cells[row, col];
			StyleParentKind pKind = StyleParentKind.Own;
			if (cell == null)
				return StyleUtility.FindCellParentStyle(this, row, col, out pKind);
			else
				return new WorksheetRangeStyle(cell.InnerStyle);
		}

		/// <summary>
		/// Get single style item from specified cell
		/// </summary>
		/// <param name="row">Zero-based number of row</param>
		/// <param name="col">Zero-based number of column</param>
		/// <param name="flag">Specified style item to be get</param>
		/// <returns>Style item value</returns>
		public object GetCellStyleItem(int row, int col, PlainStyleFlag flag)
		{
			Cell cell = cells[row, col];

			StyleParentKind pKind = StyleParentKind.Own;

			WorksheetRangeStyle style = (cell == null) ?
				unvell.ReoGrid.Utility.StyleUtility.FindCellParentStyle(this, row, col, out pKind) : cell.InnerStyle;

			return unvell.ReoGrid.Utility.StyleUtility.GetStyleItem(style, flag);
		}
		#endregion

		#region UpdateCellBounds
		private void UpdateCellBounds(Cell cell)
		{
#if DEBUG
			Debug.Assert(cell.Rowspan >= 1 && cell.Colspan >= 1);
#else
			if (cell.Rowspan < 1 || cell.Colspan < 1) return;
#endif
			cell.Bounds = GetRangeBounds(cell.InternalRow, cell.InternalCol, cell.Rowspan, cell.Colspan);
			UpdateCellTextBounds(cell);
			cell.UpdateContentBounds();
		}
		#endregion // UpdateCellBounds

		#region Update Font & Text

		internal void UpdateCellFont(Cell cell, UpdateFontReason reason = UpdateFontReason.FontChanged)
		{
			UpdateCellRenderFont(null, cell, DrawMode.View, reason);
		}

		internal void UpdateCellRenderFont(IRenderer ir, Cell cell, DrawMode drawMode, UpdateFontReason reason)
		{
			if (this.controlAdapter == null || cell.InnerStyle == null)
			{
				return;
			}

			// cell doesn't contain any text, clear font dirty flag and return
			if (string.IsNullOrEmpty(cell.InnerDisplay))
			{
				// can't use below sentence, that makes RenderFont property null due to unknown reasons
				//cell.FontDirty = false;
				return;
			}

#if DRAWING
			// rich text object doesn't need update font
			if (!(cell.Data is Drawing.RichText))
			{
#endif // DRAWING

				if (ir == null) ir = this.controlAdapter.Renderer;

				ir.UpdateCellRenderFont(cell, reason);

#if DRAWING
			}
#endif // DRAWING

			cell.FontDirty = false;

			UpdateCellTextBounds(ir, cell, drawMode, reason);
		}

		/// <summary>
		/// Update Cell Text Bounds for View/Edit mode
		/// </summary>
		/// <param name="cell"></param>
		/// <param name="updateRowHeight"></param>
		internal void UpdateCellTextBounds(Cell cell)
		{
			if (cell.FontDirty)
			{
				this.UpdateCellFont(cell);
			}
			else
			{
				this.UpdateCellTextBounds(null, cell, DrawMode.View, UpdateFontReason.FontChanged);
			}
		}

		internal void UpdateCellTextBounds(IRenderer ig, Cell cell, DrawMode drawMode, UpdateFontReason reason)
		{
			this.UpdateCellTextBounds(ig, cell, drawMode, this.renderScaleFactor, reason);
		}

		/// <summary>
		/// Update cell text bounds. 
		/// need to call this method when content of cell is changed, contains styles like align, font, etc.
		/// 
		/// if cell's display property is null, this method does nothing.
		/// </summary>
		/// <param name="ig">The graphics device used to calculate bounds. Null to use default graphic device.</param>
		/// <param name="cell">The target cell will be updated.</param>
		/// <param name="drawMode">Draw mode</param>
		/// <param name="scaleFactor">Scale factor of current worksheet</param>
		internal void UpdateCellTextBounds(IRenderer ig, Cell cell, DrawMode drawMode, RGFloat scaleFactor, UpdateFontReason reason)
		{
			if (cell == null || string.IsNullOrEmpty(cell.DisplayText)) return;

			if (ig == null && this.controlAdapter != null)
			{
				ig = this.controlAdapter.Renderer;
			}

			if (ig == null) return;

			Size oldSize;
			Size size;

#if DRAWING
			if (cell.Data is Drawing.RichText)
			{
				var rt = (Drawing.RichText)cell.Data;

				oldSize = rt.TextSize;

				rt.TextWrap = cell.InnerStyle.TextWrapMode;
				rt.DefaultHorizontalAlignment = cell.Style.HAlign;
				rt.VerticalAlignment = cell.Style.VAlign;

				rt.Size = new Size(cell.Width - cell.InnerStyle.Indent, cell.Height);

				size = rt.TextSize;
				return;
			}
			else
			{
#endif // DRAWING

			oldSize = cell.TextBounds.Size;

			#region Plain Text Measure Size
			size = ig.MeasureCellText(cell, drawMode, scaleFactor);

			if (size.Width <= 0 || size.Height <= 0) return;

			// FIXME: get incorrect size if CJK fonts
			size.Width += 2;
			size.Height += 1;
			#endregion // Plain Text Measure Size

			Rectangle cellBounds = cell.Bounds;

			RGFloat cellWidth = cellBounds.Width * scaleFactor;

#if WINFORM

				if (cell.InnerStyle.HAlign == ReoGridHorAlign.DistributedIndent)
				{
					size.Width--;

					if (drawMode == DrawMode.View)
					{
						cell.DistributedIndentSpacing = ((cellWidth - size.Width - 3) / (cell.DisplayText.Length - 1)) - 1;
						if (cell.DistributedIndentSpacing < 0) cell.DistributedIndentSpacing = 0;
					}
					else
					{
						cell.DistributedIndentSpacingPrint = ((cellWidth - size.Width - 3) / (cell.DisplayText.Length - 1)) - 1;
						if (cell.DistributedIndentSpacingPrint < 0) cell.DistributedIndentSpacingPrint = 0;
					}

					cell.RenderHorAlign = ReoGridRenderHorAlign.Center;
					if (size.Width < cellWidth - 1) size.Width = (float)(Math.Round(cellWidth - 1));
				}

#elif WPF

			if (cell.InnerStyle.TextWrapMode != TextWrapMode.NoWrap)
			{
				cell.formattedText.MaxTextWidth = cellWidth;
			}

#endif // WPF

			#region Update Text Size Cache
			RGFloat x = 0;
			RGFloat y = 0;

			float indent = cell.InnerStyle.Indent;

			switch (cell.RenderHorAlign)
			{
				default:
				case ReoGridRenderHorAlign.Left:
					x = cellBounds.Left * scaleFactor + 2 + (indent * this.indentSize);
					break;

				case ReoGridRenderHorAlign.Center:
					x = (cellBounds.Left * scaleFactor + cellWidth / 2 - size.Width / 2);
					break;

				case ReoGridRenderHorAlign.Right:
					x = cellBounds.Right * scaleFactor - 3 - size.Width - (indent * this.indentSize);
					break;
			}

			switch (cell.InnerStyle.VAlign)
			{
				case ReoGridVerAlign.Top:
					y = cellBounds.Top * scaleFactor + 1;
					break;

				case ReoGridVerAlign.Middle:
					y = (cellBounds.Top * scaleFactor + (cellBounds.Height * scaleFactor) / 2 - (size.Height) / 2);
					break;

				default:
				case ReoGridVerAlign.General:
				case ReoGridVerAlign.Bottom:
					y = cellBounds.Bottom * scaleFactor - 1 - size.Height;
					break;
			}

			switch (drawMode)
			{
				default:
				case DrawMode.View:
					cell.TextBounds = new Rectangle(x, y, size.Width, size.Height);
					break;

				case DrawMode.Preview:
				case DrawMode.Print:
					cell.PrintTextBounds = new Rectangle(x, y, size.Width, size.Height);
					break;
			}
			#endregion // Update Text Size Cache

#if DRAWING
			}
#endif // DRAWING

			if (drawMode == DrawMode.View 
				&& reason != UpdateFontReason.ScaleChanged)
			{
				if (size.Height > oldSize.Height 
					&& this.settings.Has(WorksheetSettings.Edit_AutoExpandRowHeight))
				{
					var rowHeader = this.rows[cell.Row];

					if (rowHeader.IsVisible && rowHeader.IsAutoHeight)
					{
						cell.ExpandRowHeight();
					}
				}

				if (size.Width > oldSize.Width
					&& this.settings.Has(WorksheetSettings.Edit_AutoExpandColumnWidth))
				{
					var colHeader = this.cols[cell.Column];

					if (colHeader.IsVisible && colHeader.IsAutoWidth)
					{
						cell.ExpandColumnWidth();
					}
				}
			}
		}

		/// <summary>
		/// Make the text of cells in specified range larger or smaller.
		/// </summary>
		/// <param name="range">The spcified range.</param>
		/// <param name="stepHandler">Iterator callback to handle how to make text larger or smaller.</param>
		public void StepRangeFont(RangePosition range, Func<float, float> stepHandler)
		{
			RangePosition fixedRange = FixRange(range);

			bool enableAdjustRowHeight = this.settings.Has(WorksheetSettings.Edit_AllowAdjustRowHeight
				| WorksheetSettings.Edit_AutoExpandRowHeight);

			RowHeader rowHeader = null;

			this.IterateCells(fixedRange, (r, c, cell) =>
			{
				cell.CreateOwnStyle();

				float newSize = stepHandler(cell.InnerStyle.FontSize);

				if (enableAdjustRowHeight && newSize > cell.InnerStyle.FontSize)
				{
					cell.InnerStyle.FontSize = newSize;

					if (rowHeader == null || rowHeader.Index != r)
					{
						rowHeader = this.rows[r];

						if (rowHeader.IsAutoHeight)
						{
							cell.ExpandRowHeight();
						}
					}
				}
				else
				{
					cell.InnerStyle.FontSize = newSize;
				}

				cell.FontDirty = true;

				return true;
			});

			RequestInvalidate();
		}

		#endregion // Update Font & Text

		internal WorksheetRangeStyle RootStyle { get; set; }
	}

	#region Alignment
	/// <summary>
	/// Cell horizontal alignment (default: General)
	/// </summary>
	[Serializable]
	public enum ReoGridHorAlign
	{
		/// <summary>
		/// General horizontal alignment (Spreadsheet decides the alignment automatically)
		/// </summary>
		General,

		/// <summary>
		/// Left
		/// </summary>
		Left,

		/// <summary>
		/// Center
		/// </summary>
		Center,

		/// <summary>
		/// Right
		/// </summary>
		Right,

		/// <summary>
		/// Distributed to fill the space of cell
		/// </summary>
		DistributedIndent,
	}

	/// <summary>
	/// Cell vertical alignment (default: Middle)
	/// </summary>
	[Serializable]
	public enum ReoGridVerAlign
	{
		/// <summary>
		/// Default
		/// </summary>
		General,

		/// <summary>
		/// Top
		/// </summary>
		Top,

		/// <summary>
		/// Middle
		/// </summary>
		Middle,

		/// <summary>
		/// Bottom
		/// </summary>
		Bottom,
	}
	#endregion // Alignment

	#region enum PlainStyleFlag
	/// <summary>
	/// Key of cell style item
	/// </summary>
	public enum PlainStyleFlag : long
	{
		/// <summary>
		/// None style will be added or removed
		/// </summary>
		None = 0,

		/// <summary>
		/// Font name
		/// </summary>
		FontName = 0x1,

		/// <summary>
		/// Font size
		/// </summary>
		FontSize = 0x2,

		/// <summary>
		/// Font bold
		/// </summary>
		FontStyleBold = 0x4,

		/// <summary>
		/// Font italic
		/// </summary>
		FontStyleItalic = 0x8,

		/// <summary>
		/// Font strikethrough
		/// </summary>
		FontStyleStrikethrough = 0x10,

		/// <summary>
		/// Font underline
		/// </summary>
		FontStyleUnderline = 0x20,

		/// <summary>
		/// Text color
		/// </summary>
		TextColor = 0x40,

		/// <summary>
		/// Background color
		/// </summary>
		BackColor = 0x80,

		/// <summary>
		/// Line color (Reserved)
		/// </summary>
		LineColor = 0x100,

		/// <summary>
		/// Line style (Reserved)
		/// </summary>
		LineStyle = 0x200,

		/// <summary>
		/// Line weight (Reserved)
		/// </summary>
		LineWeight = 0x400,

		/// <summary>
		/// Line start cap (Reserved)
		/// </summary>
		LineStartCap = 0x800,

		/// <summary>
		/// Line end cap (Reserved)
		/// </summary>
		LineEndCap = 0x1000,

		/// <summary>
		/// Horizontal alignements
		/// </summary>
		HorizontalAlign = 0x2000,

		/// <summary>
		/// Vertical alignement
		/// </summary>
		VerticalAlign = 0x4000,

		/// <summary>
		/// Background pattern color (not supported in WPF version)
		/// </summary>
		FillPatternColor = 0x80000,

		/// <summary>
		/// Background pattern style (not supported in WPF version)
		/// </summary>
		FillPatternStyle = 0x100000,

		/// <summary>
		/// Text wrap (word-break mode)
		/// </summary>
		TextWrap = 0x200000,

		/// <summary>
		/// Padding
		/// </summary>
		Indent = 0x400000,

		/// <summary>
		/// Padding
		/// </summary>
		Padding = 0x800000,

		/// <summary>
		/// Rotation angle for cell text
		/// </summary>
		RotationAngle = 0x1000000,

		/// <summary>
		/// [Union flag] All flags of font style
		/// </summary>
		FontStyleAll = FontStyleBold | FontStyleItalic
			| FontStyleStrikethrough | FontStyleUnderline,

		/// <summary>
		/// [Union flag] All font styles (name, size and style)
		/// </summary>
		FontAll = FontName | FontSize | FontStyleAll,

		/// <summary>
		/// [Union flag] All line styles (color, style, weight and caps)
		/// </summary>
		LineAll = LineColor | LineStyle | LineWeight | LineStartCap | LineEndCap,

		/// <summary>
		/// [Union flag] All layout styles (Text-wrap, padding and angle)
		/// </summary>
		LayoutAll = TextWrap | Padding | RotationAngle,

		/// <summary>
		/// [Union flag] Both horizontal and vertical alignments
		/// </summary>
		AlignAll = HorizontalAlign | VerticalAlign,

		/// <summary>
		/// [Union flag] Background pattern (color and style)
		/// </summary>
		FillPattern = FillPatternColor | FillPatternStyle,

		/// <summary>
		/// [Union flag] All background styles (color and pattern)
		/// </summary>
		BackAll = BackColor | FillPattern,

		/// <summary>
		/// [Union flag] All styles
		/// </summary>
		All = FontAll | TextColor | BackAll | LineAll | AlignAll | LayoutAll,
	}
	#endregion // PlainStyleFlag

	#region enum TextWrapMode
	/// <summary>
	/// Text-wrap mode of cell
	/// </summary>
	public enum TextWrapMode
	{
		/// <summary>
		/// No break (default)
		/// </summary>
		NoWrap,

		/// <summary>
		/// Normal word break
		/// </summary>
		WordBreak,

		/// <summary>
		/// Break enabled for all characters
		/// </summary>
		BreakAll,
	}
	#endregion // TextWrapMode

	#region Padding
	/// <summary>
	/// Padding value struct
	/// </summary>
	[Serializable]
	public struct PaddingValue
	{
		/// <summary>
		/// Get or set top padding
		/// </summary>
		public RGIntDouble Top { get; set; }

		/// <summary>
		/// Get or set bottom padding
		/// </summary>
		public RGIntDouble Bottom { get; set; }

		/// <summary>
		/// Get or set left padding
		/// </summary>
		public RGIntDouble Left { get; set; }

		/// <summary>
		/// Get or set right padding
		/// </summary>
		public RGIntDouble Right { get; set; }

		/// <summary>
		/// Create padding and set all values with same specified value.
		/// </summary>
		/// <param name="all">Value applied to all padding.</param>
		public PaddingValue(RGIntDouble all)
			: this(all, all, all, all)
		{
		}

		/// <summary>
		/// Create padding with every specified values. (in pixel)
		/// </summary>
		/// <param name="top">Top padding.</param>
		/// <param name="bottom">Bottom padding.</param>
		/// <param name="left">Left padding.</param>
		/// <param name="right">Right padding.</param>
		public PaddingValue(RGIntDouble top, RGIntDouble bottom, RGIntDouble left, RGIntDouble right)
			: this()
		{
			this.Top = top;
			this.Bottom = bottom;
			this.Left = left;
			this.Right = right;
		}

		/// <summary>
		/// Predefined empty padding value
		/// </summary>
		public static readonly PaddingValue Empty = new PaddingValue(0);

		/// <summary>
		/// Compare two padding values whether are same
		/// </summary>
		/// <param name="p1">Padding value 1 to be compared</param>
		/// <param name="p2">Padding value 2 to be compared</param>
		/// <returns>True if two padding values are same; otherwise return false</returns>
		public static bool operator ==(PaddingValue p1, PaddingValue p2)
		{
			return p1.Left == p2.Left && p1.Top == p2.Top
				&& p1.Right == p2.Right && p1.Bottom == p2.Bottom;
		}

		/// <summary>
		/// Compare two padding values whether are not same
		/// </summary>
		/// <param name="p1">Padding value 1 to be compared</param>
		/// <param name="p2">Padding value 2 to be compared</param>
		/// <returns>True if two padding values are not same; otherwise return false</returns>
		public static bool operator !=(PaddingValue p1, PaddingValue p2)
		{
			return p1.Left != p2.Left || p1.Top != p2.Top
				|| p1.Right != p2.Right || p1.Bottom != p2.Bottom;
		}

		/// <summary>
		/// Compare an object and check whether two padding value are same
		/// </summary>
		/// <param name="obj">Another object to be checked</param>
		/// <returns>True if two padding values are same; otherwise return false</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is PaddingValue)) return false;

			var obj2 = (PaddingValue)obj;

			return this.Top == obj2.Top && this.Left == obj2.Left
				&& this.Right == obj2.Right && this.Bottom == obj2.Bottom;
		}

		/// <summary>
		/// Get hash code of this object
		/// </summary>
		/// <returns>Hash code</returns>
		public override int GetHashCode()
		{
			return (int)(this.Top + this.Left * 2 + this.Right * 3 + this.Bottom * 4);
		}

		public override string ToString()
		{
			return string.Format("[{0}, {1}, {2}, {3}]", this.Top, this.Bottom, this.Left, this.Right);
		}
	}
	#endregion

	#region WorksheetRangeStyle

	/// <summary>
	/// Styles of range or cells. By specifying PlainStyleFlag to determine 
	/// what styles should be used in this set.
	/// </summary>
	[Serializable]
	public class WorksheetRangeStyle
	{
		/// <summary>
		/// Get or set the styles flag that indicates what styles are contained in this style set
		/// </summary>
		public PlainStyleFlag Flag { get; set; }

		/// <summary>
		/// Get or set background color 
		/// </summary>
		public SolidColor BackColor { get; set; }

		/// <summary>
		/// Get or set backgrond pattern color.
		/// When set pattern color or pattern style, the background color should also be set.
		/// </summary>
		public SolidColor FillPatternColor { get; set; }

		/// <summary>
		/// Get or set background pattern style.
		/// When set pattern color or pattern style, the background color should also be set.
		/// </summary>
		public HatchStyles FillPatternStyle { get; set; }

		/// <summary>
		/// Get or set text color
		/// </summary>
		public SolidColor TextColor { get; set; }

		/// <summary>
		/// Get or set font name
		/// </summary>
		public string FontName { get; set; }

		/// <summary>
		/// Get or set font size
		/// </summary>
		public float FontSize { get; set; }

		internal Drawing.Text.FontStyles fontStyles = Drawing.Text.FontStyles.Regular;

		/// <summary>
		/// Get or set bold style
		/// </summary>
		public bool Bold
		{
			get { return (this.fontStyles & Drawing.Text.FontStyles.Bold) == Drawing.Text.FontStyles.Bold; }
			set
			{
				if (value)
					this.fontStyles |= Drawing.Text.FontStyles.Bold;
				else
					this.fontStyles &= ~Drawing.Text.FontStyles.Bold;
			}
		}

		/// <summary>
		/// Get or set italic style
		/// </summary>
		public bool Italic
		{
			get { return (this.fontStyles & Drawing.Text.FontStyles.Italic) == Drawing.Text.FontStyles.Italic; }
			set
			{
				if (value)
					this.fontStyles |= Drawing.Text.FontStyles.Italic;
				else
					this.fontStyles &= ~Drawing.Text.FontStyles.Italic;
			}
		}

		/// <summary>
		/// Get or set strikethrough style
		/// </summary>
		public bool Strikethrough
		{
			get { return (this.fontStyles & Drawing.Text.FontStyles.Strikethrough) == Drawing.Text.FontStyles.Strikethrough; }
			set
			{
				if (value)
					this.fontStyles |= Drawing.Text.FontStyles.Strikethrough;
				else
					this.fontStyles &= ~Drawing.Text.FontStyles.Strikethrough;
			}
		}

		/// <summary>
		/// Get or set underline style
		/// </summary>
		public bool Underline
		{
			get { return (this.fontStyles & Drawing.Text.FontStyles.Underline) == Drawing.Text.FontStyles.Underline; }
			set
			{
				if (value)
					this.fontStyles |= Drawing.Text.FontStyles.Underline;
				else
					this.fontStyles &= ~Drawing.Text.FontStyles.Underline;
			}
		}

		/// <summary>
		/// Get or set horizontal alignment
		/// </summary>
		public ReoGridHorAlign HAlign { get; set; }

		/// <summary>
		/// Get or set vertical alignment
		/// </summary>
		public ReoGridVerAlign VAlign { get; set; }

		/// <summary>
		/// Get or set text-wrap mode
		/// </summary>
		public TextWrapMode TextWrapMode { get; set; }

		/// <summary>
		/// Get or set text indent (0-65535)
		/// </summary>
		public ushort Indent { get; set; }

		/// <summary>
		/// Get or set padding of cell.
		/// </summary>
		public PaddingValue Padding { get; set; }

		/// <summary>
		/// Get or set rotate angle.
		/// </summary>
		public float RotationAngle { get; set; }

		/// <summary>
		/// Create an empty style set.
		/// </summary>
		public WorksheetRangeStyle() { }

		/// <summary>
		/// Create style set by copying from another one.
		/// </summary>
		/// <param name="source">Another style set to be copied.</param>
		public WorksheetRangeStyle(WorksheetRangeStyle source) { CopyFrom(source); }

		/// <summary>
		/// Clone style set from specified another style set.
		/// </summary>
		/// <param name="source">Another style to be cloned.</param>
		/// <returns>New cloned style set.</returns>
		public static WorksheetRangeStyle Clone(WorksheetRangeStyle source)
		{
			return source == null ? source : new WorksheetRangeStyle(source);
		}

		/// <summary>
		/// Copy styles from another specified one.
		/// </summary>
		/// <param name="s">Style to be copied.</param>
		public void CopyFrom(WorksheetRangeStyle s)
		{
			StyleUtility.CopyStyle(s, this);
		}

		/// <summary>
		/// Predefined empty style set.
		/// </summary>
		public static WorksheetRangeStyle Empty = new WorksheetRangeStyle();

		/// <summary>
		/// Check two styles and compare whether or not they are same.
		/// </summary>
		/// <param name="obj">Another style object compared to this object.</param>
		/// <returns>True if thay are same; otherwise return false.</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is WorksheetRangeStyle)) return false;

			WorksheetRangeStyle s2 = (WorksheetRangeStyle)obj;

			if (this.Flag != s2.Flag) return false;

			if ((this.Flag & PlainStyleFlag.HorizontalAlign) == PlainStyleFlag.HorizontalAlign
				&& this.HAlign != s2.HAlign) return false;
			if ((this.Flag & PlainStyleFlag.VerticalAlign) == PlainStyleFlag.VerticalAlign
				&& this.VAlign != s2.VAlign) return false;
			if ((this.Flag & PlainStyleFlag.BackColor) == PlainStyleFlag.BackColor
				&& this.BackColor != s2.BackColor) return false;
			if ((this.Flag & PlainStyleFlag.FillPatternColor) == PlainStyleFlag.FillPatternColor
				&& this.FillPatternColor != s2.FillPatternColor) return false;
			if ((this.Flag & PlainStyleFlag.FillPatternStyle) == PlainStyleFlag.FillPatternStyle
				&& this.FillPatternStyle != s2.FillPatternStyle) return false;
			if ((this.Flag & PlainStyleFlag.TextColor) == PlainStyleFlag.TextColor
				&& this.TextColor != s2.TextColor) return false;
			if ((this.Flag & PlainStyleFlag.FontName) == PlainStyleFlag.FontName
				&& this.FontName != s2.FontName) return false;
			if ((this.Flag & PlainStyleFlag.FontSize) == PlainStyleFlag.FontSize
				&& this.FontSize != s2.FontSize) return false;
			if ((this.Flag & PlainStyleFlag.FontStyleBold) == PlainStyleFlag.FontStyleBold
				&& this.Bold != s2.Bold) return false;
			if ((this.Flag & PlainStyleFlag.FontStyleItalic) == PlainStyleFlag.FontStyleItalic
				&& this.Italic != s2.Italic) return false;
			if ((this.Flag & PlainStyleFlag.FontStyleStrikethrough) == PlainStyleFlag.FontStyleStrikethrough
				&& this.Strikethrough != s2.Strikethrough) return false;
			if ((this.Flag & PlainStyleFlag.FontStyleUnderline) == PlainStyleFlag.FontStyleUnderline
				&& this.Underline != s2.Underline) return false;
			if ((this.Flag & PlainStyleFlag.TextWrap) == PlainStyleFlag.TextWrap
				&& this.TextWrapMode != s2.TextWrapMode) return false;
			if ((this.Flag & PlainStyleFlag.Indent) == PlainStyleFlag.Indent
				&& this.Indent != s2.Indent) return false;
			if ((this.Flag & PlainStyleFlag.Padding) == PlainStyleFlag.Padding
				&& this.Padding != s2.Padding) return false;
			if ((this.Flag & PlainStyleFlag.RotationAngle) == PlainStyleFlag.RotationAngle
				&& this.RotationAngle != s2.RotationAngle) return false;

			return true;
		}

		/// <summary>
		/// Get hash code of this object.
		/// </summary>
		/// <returns>hash code</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Check whether this set of style contains specified style item.
		/// </summary>
		/// <param name="flag">Style item to be checked.</param>
		/// <returns>Ture if this set contains specified style item.</returns>
		public bool HasStyle(PlainStyleFlag flag)
		{
			return (this.Flag & flag) == flag;
		}

		/// <summary>
		/// Check whether this set of style contains any of one of specified style items.
		/// </summary>
		/// <param name="flag">Style items to be checked.</param>
		/// <returns>True if this set contains any one of specified items.</returns>
		public bool HasAny(PlainStyleFlag flag)
		{
			return (this.Flag & flag) > 0;
		}
	}

	#endregion // ReoGridStyleObject

	#region ReferenceStyle
	/// <summary>
	/// Referenced style instance to cell of range
	/// </summary>
	public abstract class ReferenceStyle
	{
		private Worksheet sheet;

		/// <summary>
		/// Get worksheet instance
		/// </summary>
		public Worksheet Worksheet { get { return this.sheet; } }

		internal ReferenceStyle(Worksheet sheet)
		{
			this.sheet = sheet;
		}

		internal virtual void SetStyle(RangePosition range, WorksheetRangeStyle style)
		{
			SetStyle(range.Row, range.Col, range.Rows, range.Cols, style);
		}

		internal virtual void SetStyle(int row, int col, int rows, int cols, WorksheetRangeStyle style)
		{
			this.Worksheet.SetRangeStyles(row, col, rows, cols, style);
		}

		//TODO: reduce create style object
		internal virtual void SetStyle<T>(RangePosition range, PlainStyleFlag flag, T value)
		{
			SetStyle<T>(range.Row, range.Col, range.Rows, range.Cols, flag, value);
		}

		//TODO: reduce create style object
		internal virtual void SetStyle<T>(int row, int col, int rows, int cols, PlainStyleFlag flag, T value)
		{
			var style = new WorksheetRangeStyle
			{
				Flag = flag,
			};

			switch (flag)
			{
				case PlainStyleFlag.BackColor: style.BackColor = (SolidColor)(object)value; break;
				case PlainStyleFlag.TextColor: style.TextColor = (SolidColor)(object)value; break;
				case PlainStyleFlag.TextWrap: style.TextWrapMode = (TextWrapMode)(object)value; break;
				case PlainStyleFlag.Indent: style.Indent = (ushort)(object)value; break;
				case PlainStyleFlag.FillPatternColor: style.FillPatternColor = (SolidColor)(object)value; break;
				case PlainStyleFlag.FillPatternStyle: style.FillPatternStyle = (HatchStyles)(object)value; break;
				case PlainStyleFlag.Padding: style.Padding = (PaddingValue)(object)value; break;
				case PlainStyleFlag.FontName: style.FontName = (string)(object)value; break;
				case PlainStyleFlag.FontSize: style.FontSize = (float)(object)value; break;
				case PlainStyleFlag.FontStyleBold: style.Bold = (bool)(object)value; break;
				case PlainStyleFlag.FontStyleItalic: style.Italic = (bool)(object)value; break;
				case PlainStyleFlag.FontStyleUnderline: style.Underline = (bool)(object)value; break;
				case PlainStyleFlag.FontStyleStrikethrough: style.Strikethrough = (bool)(object)value; break;
				case PlainStyleFlag.HorizontalAlign: style.HAlign = (ReoGridHorAlign)(object)value; break;
				case PlainStyleFlag.VerticalAlign: style.VAlign = (ReoGridVerAlign)(object)value; break;
				case PlainStyleFlag.RotationAngle: style.RotationAngle = (int)(object)value; break;
			}

			this.Worksheet.SetRangeStyles(row, col, rows, cols, style);
		}

		internal virtual T GetStyle<T>(RangePosition range, PlainStyleFlag flag)
		{
			return GetStyle<T>(range.Row, range.Col, range.Rows, range.Cols, flag);
		}

		internal virtual T GetStyle<T>(int row, int col, int rows, int cols, PlainStyleFlag flag)
		{
			Type type = typeof(T);

			return (T)Convert.ChangeType(this.Worksheet.GetRangeStyle(row, col, rows, cols, flag), type);
		}

		internal virtual void CheckForReferenceOwner(object owner)
		{
			if (this.Worksheet == null || owner == null)
			{
				throw new ReferenceObjectNotAssociatedException("Reference style must be associated with an instance of owner.");
			}
		}

		/// <summary>
		/// Convert style reference to style object.
		/// </summary>
		/// <param name="refStyle">Style reference to be converted.</param>
		/// <returns>Style object converted from style reference.</returns>
		public static implicit operator WorksheetRangeStyle(ReferenceStyle refStyle)
		{
			return new WorksheetRangeStyle();
		}
	}
	#endregion // ReferenceStyle

	#region ReferenceRangeStyle
	/// <summary>
	/// Range reference to spreadsheet
	/// </summary>
	public class ReferenceRangeStyle : ReferenceStyle
	{
		private ReferenceRange range;

		internal ReferenceRange Range { get { return this.range; } }

		internal ReferenceRangeStyle(Worksheet grid, ReferenceRange range)
			: base(grid)
		{
			this.range = range;
		}

		/// <summary>
		/// Get or set the background color to entire range
		/// </summary>
		public SolidColor BackColor
		{
			get
			{
				CheckForReferenceOwner(this.range);

				return GetStyle<SolidColor>(this.range, PlainStyleFlag.BackColor);
			}
			set
			{
				CheckForReferenceOwner(this.range);

				SetStyle(this.range, PlainStyleFlag.BackColor, value);
			}
		}

		/// <summary>
		/// Get or set the text color to entire range
		/// </summary>
		public SolidColor TextColor
		{
			get
			{
				CheckForReferenceOwner(this.range);

				return GetStyle<SolidColor>(this.range, PlainStyleFlag.TextColor);
			}
			set
			{
				CheckForReferenceOwner(this.range);

				SetStyle(this.range, PlainStyleFlag.TextColor, value);
			}
		}

		/// <summary>
		/// Get or set the font name to entire range
		/// </summary>
		public string FontName
		{
			get
			{
				CheckForReferenceOwner(this.range);

				return GetStyle<string>(this.range, PlainStyleFlag.FontName);
			}
			set
			{
				CheckForReferenceOwner(this.range);

				SetStyle(this.range, PlainStyleFlag.FontName, value);
			}
		}

		/// <summary>
		/// Get or set the font size to entire range
		/// </summary>
		public float FontSize
		{
			get
			{
				CheckForReferenceOwner(this.range);

				return GetStyle<float>(this.range, PlainStyleFlag.FontSize);
			}
			set
			{
				CheckForReferenceOwner(this.range);

				SetStyle(this.range, PlainStyleFlag.FontSize, value);
			}
		}

		/// <summary>
		/// Get or set bold font style to entire range
		/// </summary>
		public bool Bold
		{
			get
			{
				CheckForReferenceOwner(this.range);

				return GetStyle<bool>(this.range, PlainStyleFlag.FontStyleBold);
			}
			set
			{
				CheckForReferenceOwner(this.range);

				SetStyle(this.range, PlainStyleFlag.FontStyleBold, value);
			}
		}

		/// <summary>
		/// Get or set italic font style to entire range
		/// </summary>
		public bool Italic
		{
			get
			{
				CheckForReferenceOwner(this.range);

				return GetStyle<bool>(this.range, PlainStyleFlag.FontStyleItalic);
			}
			set
			{
				CheckForReferenceOwner(this.range);

				SetStyle(this.range, PlainStyleFlag.FontStyleItalic, value);
			}
		}

		/// <summary>
		/// Get or set underline font style to entire range
		/// </summary>
		public bool Underline
		{
			get
			{
				CheckForReferenceOwner(this.range);

				return GetStyle<bool>(this.range, PlainStyleFlag.FontStyleUnderline);
			}
			set
			{
				CheckForReferenceOwner(this.range);

				SetStyle(this.range, PlainStyleFlag.FontStyleUnderline, value);
			}
		}

		/// <summary>
		/// Get or set the strikethrough to entire range
		/// </summary>
		public bool Strikethrough
		{
			get
			{
				CheckForReferenceOwner(this.range);

				return GetStyle<bool>(this.range, PlainStyleFlag.FontStyleStrikethrough);
			}
			set
			{
				CheckForReferenceOwner(this.range);

				SetStyle(this.range, PlainStyleFlag.FontStyleStrikethrough, value);
			}
		}

		/// <summary>
		/// Get or set the horizontal alignment to entire range
		/// </summary>
		public ReoGridHorAlign HorizontalAlign
		{
			get
			{
				CheckForReferenceOwner(this.range);

				return GetStyle<ReoGridHorAlign>(this.range, PlainStyleFlag.HorizontalAlign);
			}
			set
			{
				CheckForReferenceOwner(this.range);

				SetStyle(this.range, PlainStyleFlag.HorizontalAlign, value);
			}
		}

		/// <summary>
		/// Get or set the vertical alignment to entire range
		/// </summary>
		public ReoGridVerAlign VerticalAlign
		{
			get
			{
				CheckForReferenceOwner(this.range);

				return GetStyle<ReoGridVerAlign>(this.range, PlainStyleFlag.VerticalAlign);
			}
			set
			{
				CheckForReferenceOwner(this.range);

				SetStyle(this.range, PlainStyleFlag.VerticalAlign, value);
			}
		}

		/// <summary>
		/// Get or set the padding to entire range
		/// </summary>
		public PaddingValue Padding
		{
			get
			{
				CheckForReferenceOwner(this.range);

				return GetStyle<PaddingValue>(this.range, PlainStyleFlag.Padding);
			}
			set
			{
				CheckForReferenceOwner(this.range);

				SetStyle(this.range, PlainStyleFlag.Padding, value);
			}
		}

		/// <summary>
		/// Get or set the text-wrap style to entire range
		/// </summary>
		public TextWrapMode TextWrap
		{
			get
			{
				CheckForReferenceOwner(this.range);

				return GetStyle<TextWrapMode>(this.range, PlainStyleFlag.TextWrap);
			}
			set
			{
				CheckForReferenceOwner(this.range);

				SetStyle(this.range, PlainStyleFlag.TextWrap, value);
			}
		}

		/// <summary>
		/// Get or set the cell indent
		/// </summary>
		public ushort Indent
		{
			get
			{
				CheckForReferenceOwner(this.range);

				return GetStyle<ushort>(this.range, PlainStyleFlag.Indent);
			}
			set
			{
				CheckForReferenceOwner(this.range);

				this.SetStyle(this.range, PlainStyleFlag.Padding, value);
			}
		}
	}
	#endregion // ReferenceRangeStyle

	#region ColumnHeaderStyle
	/// <summary>
	/// Referenced style for column header
	/// </summary>
	public class ColumnHeaderStyle : ReferenceStyle
	{
		/// <summary>
		/// Column header instance
		/// </summary>
		private ColumnHeader columnHeader;

		internal ColumnHeaderStyle(Worksheet grid, ColumnHeader columnHeader)
			: base(grid)
		{
			this.columnHeader = columnHeader;
		}

		/// <summary>
		/// Get or set horizontal alignment for all cells on this row
		/// </summary>
		public bool Bold
		{
			get
			{
				CheckForReferenceOwner(this.columnHeader);

				return (bool)this.Worksheet.GetRangeStyle(0, this.columnHeader.Index, -1, 1, PlainStyleFlag.FontStyleBold);
			}
			set
			{
				CheckForReferenceOwner(this.columnHeader);

				//TODO: reduce create style object
				base.SetStyle(0, this.columnHeader.Index, -1, 1, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.FontStyleBold,
					Bold = value
				});
			}
		}

		/// <summary>
		/// Get or set horizontal alignment for all cells on this row
		/// </summary>
		public bool Italic
		{
			get
			{
				CheckForReferenceOwner(this.columnHeader);

				return (bool)this.Worksheet.GetRangeStyle(0, this.columnHeader.Index, -1, 1, PlainStyleFlag.FontStyleItalic);
			}
			set
			{
				CheckForReferenceOwner(this.columnHeader);

				//TODO: reduce create style object
				base.SetStyle(0, this.columnHeader.Index, -1, 1, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.FontStyleItalic,
					Italic = value
				});
			}
		}

		/// <summary>
		/// Get or set horizontal alignment for all cells on this row
		/// </summary>
		public bool Strikethrough
		{
			get
			{
				CheckForReferenceOwner(this.columnHeader);

				return (bool)this.Worksheet.GetRangeStyle(0, this.columnHeader.Index, -1, 1, PlainStyleFlag.FontStyleStrikethrough);
			}
			set
			{
				CheckForReferenceOwner(this.columnHeader);

				//TODO: reduce create style object
				base.SetStyle(0, this.columnHeader.Index, -1, 1, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.FontStyleStrikethrough,
					Strikethrough = value
				});
			}
		}

		/// <summary>
		/// Get or set horizontal alignment for all cells on this row
		/// </summary>
		public bool Underline
		{
			get
			{
				CheckForReferenceOwner(this.columnHeader);

				return (bool)this.Worksheet.GetRangeStyle(0, this.columnHeader.Index, -1, 1, PlainStyleFlag.FontStyleUnderline);
			}
			set
			{
				CheckForReferenceOwner(this.columnHeader);

				//TODO: reduce create style object
				base.SetStyle(0, this.columnHeader.Index, -1, 1, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.FontStyleUnderline,
					Underline = value
				});
			}
		}

		/// <summary>
		/// Get or set horizontal alignment for all cells on this column
		/// </summary>
		public ReoGridHorAlign HorizontalAlign
		{
			get
			{
				CheckForReferenceOwner(this.columnHeader);

				return (ReoGridHorAlign)this.Worksheet.GetRangeStyle(0, this.columnHeader.Index, -1, 1, PlainStyleFlag.HorizontalAlign);
			}
			set
			{
				CheckForReferenceOwner(this.columnHeader);

				//TODO: reduce create style object
				base.SetStyle(0, this.columnHeader.Index, -1, 1, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.HorizontalAlign,
					HAlign = value
				});
			}
		}

		/// <summary>
		/// Get or set horizontal alignment for all cells on this column
		/// </summary>
		public ReoGridVerAlign VerticalAlign
		{
			get
			{
				CheckForReferenceOwner(this.columnHeader);

				return (ReoGridVerAlign)this.Worksheet.GetRangeStyle(0, this.columnHeader.Index, -1, 1, PlainStyleFlag.VerticalAlign);
			}
			set
			{
				CheckForReferenceOwner(this.columnHeader);

				//TODO: reduce create style object
				base.SetStyle(0, this.columnHeader.Index, -1, 1, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.VerticalAlign,
					VAlign = value
				});
			}
		}

		/// <summary>
		/// Get or set padding for all cells on this column
		/// </summary>
		public PaddingValue Padding
		{
			get
			{
				CheckForReferenceOwner(this.columnHeader);

				return (PaddingValue)this.Worksheet.GetRangeStyle(0, this.columnHeader.Index, -1, 1, PlainStyleFlag.Padding);
			}
			set
			{
				CheckForReferenceOwner(this.columnHeader);

				//TODO: reduce create style object
				base.SetStyle(0, this.columnHeader.Index, -1, 1,
					new WorksheetRangeStyle
					{
						Flag = PlainStyleFlag.Padding,
						Padding = value
					});
			}
		}

		/// <summary>
		/// Get or set background color for all cells on this column
		/// </summary>
		public SolidColor BackColor
		{
			get
			{
				CheckForReferenceOwner(this.columnHeader);

				return (SolidColor)this.Worksheet.GetRangeStyle(0, this.columnHeader.Index, -1, 1, PlainStyleFlag.BackColor);
			}
			set
			{
				CheckForReferenceOwner(this.columnHeader);

				//TODO: reduce create style object
				base.SetStyle(0, this.columnHeader.Index, -1, 1,
					new WorksheetRangeStyle
					{
						Flag = PlainStyleFlag.BackColor,
						BackColor = value
					});
			}
		}

		/// <summary>
		/// Get or set background color for all cells on this column
		/// </summary>
		public SolidColor TextColor
		{
			get
			{
				CheckForReferenceOwner(this.columnHeader);

				return (SolidColor)this.Worksheet.GetRangeStyle(0, this.columnHeader.Index, -1, 1, PlainStyleFlag.TextColor);
			}
			set
			{
				CheckForReferenceOwner(this.columnHeader);

				//TODO: reduce create style object
				base.SetStyle(0, this.columnHeader.Index, -1, 1,
					new WorksheetRangeStyle
					{
						Flag = PlainStyleFlag.TextColor,
						TextColor = value
					});
			}
		}
	}
	#endregion // ColumnHeaderStyle

	#region RowHeaderStyle
	/// <summary>
	/// Refereced style for row header
	/// </summary>
	public class RowHeaderStyle : ReferenceStyle
	{
		private RowHeader rowHeader;

		internal RowHeaderStyle(Worksheet grid, RowHeader rowHeader)
			: base(grid)
		{
			this.rowHeader = rowHeader;
		}

		/// <summary>
		/// Get or set horizontal alignment for all cells on this row
		/// </summary>
		public bool Bold
		{
			get
			{
				CheckForReferenceOwner(this.rowHeader);

				return (bool)this.Worksheet.GetRangeStyle(this.rowHeader.Index, 0, 1, -1, PlainStyleFlag.FontStyleBold);
			}
			set
			{
				CheckForReferenceOwner(this.rowHeader);

				//TODO: reduce create style object
				base.SetStyle(this.rowHeader.Index, 0, 1, -1, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.FontStyleBold,
					Bold = value
				});
			}
		}

		/// <summary>
		/// Get or set horizontal alignment for all cells on this row
		/// </summary>
		public bool Italic
		{
			get
			{
				CheckForReferenceOwner(this.rowHeader);

				return (bool)this.Worksheet.GetRangeStyle(this.rowHeader.Index, 0, 1, -1, PlainStyleFlag.FontStyleItalic);
			}
			set
			{
				CheckForReferenceOwner(this.rowHeader);

				//TODO: reduce create style object
				base.SetStyle(this.rowHeader.Index, 0, 1, -1, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.FontStyleItalic,
					Italic = value
				});
			}
		}

		/// <summary>
		/// Get or set horizontal alignment for all cells on this row
		/// </summary>
		public bool Strikethrough
		{
			get
			{
				CheckForReferenceOwner(this.rowHeader);

				return (bool)this.Worksheet.GetRangeStyle(this.rowHeader.Index, 0, 1, -1, PlainStyleFlag.FontStyleStrikethrough);
			}
			set
			{
				CheckForReferenceOwner(this.rowHeader);

				//TODO: reduce create style object
				base.SetStyle(this.rowHeader.Index, 0, 1, -1, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.FontStyleStrikethrough,
					Strikethrough = value
				});
			}
		}

		/// <summary>
		/// Get or set horizontal alignment for all cells on this row
		/// </summary>
		public bool Underline
		{
			get
			{
				CheckForReferenceOwner(this.rowHeader);

				return (bool)this.Worksheet.GetRangeStyle(this.rowHeader.Index, 0, 1, -1, PlainStyleFlag.FontStyleUnderline);
			}
			set
			{
				CheckForReferenceOwner(this.rowHeader);

				//TODO: reduce create style object
				base.SetStyle(this.rowHeader.Index, 0, 1, -1, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.FontStyleUnderline,
					Underline = value
				});
			}
		}

		/// <summary>
		/// Get or set horizontal alignment for all cells on this row
		/// </summary>
		public ReoGridHorAlign HorizontalAlign
		{
			get
			{
				CheckForReferenceOwner(this.rowHeader);

				return (ReoGridHorAlign)this.Worksheet.GetRangeStyle(this.rowHeader.Index, 0, 1, -1, PlainStyleFlag.HorizontalAlign);
			}
			set
			{
				CheckForReferenceOwner(this.rowHeader);

				//TODO: reduce create style object
				base.SetStyle(this.rowHeader.Index, 0, 1, -1, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.HorizontalAlign,
					HAlign = value
				});
			}
		}

		/// <summary>
		/// Get or set horizontal alignment for all cells on this row
		/// </summary>
		public ReoGridVerAlign VerticalAlign
		{
			get
			{
				CheckForReferenceOwner(this.rowHeader);

				return (ReoGridVerAlign)this.Worksheet.GetRangeStyle(this.rowHeader.Index, 0, 1, -1, PlainStyleFlag.VerticalAlign);
			}
			set
			{
				CheckForReferenceOwner(this.rowHeader);

				//TODO: reduce create style object
				base.SetStyle(this.rowHeader.Index, 0, 1, -1, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.VerticalAlign,
					VAlign = value
				});
			}
		}

		/// <summary>
		/// Get or set padding for all cells on this row
		/// </summary>
		public PaddingValue Padding
		{
			get
			{
				CheckForReferenceOwner(this.rowHeader);

				return (PaddingValue)this.Worksheet.GetRangeStyle(this.rowHeader.Index, 0, 1, -1, PlainStyleFlag.Padding);
			}
			set
			{
				CheckForReferenceOwner(this.rowHeader);

				//TODO: reduce create style object
				base.SetStyle(this.rowHeader.Index, 0, 1, -1,
					new WorksheetRangeStyle
					{
						Flag = PlainStyleFlag.Padding,
						Padding = value
					});
			}
		}

		/// <summary>
		/// Get or set background color for all cells on this row
		/// </summary>
		public SolidColor BackColor
		{
			get
			{
				CheckForReferenceOwner(this.rowHeader);

				return (SolidColor)this.Worksheet.GetRangeStyle(this.rowHeader.Index, 0, 1, -1, PlainStyleFlag.BackColor);
			}
			set
			{
				CheckForReferenceOwner(this.rowHeader);

				//TODO: reduce create style object
				base.SetStyle(this.rowHeader.Index, 0, 1, -1,
					new WorksheetRangeStyle
					{
						Flag = PlainStyleFlag.BackColor,
						BackColor = value
					});
			}
		}

		/// <summary>
		/// Get or set background color for all cells on this row
		/// </summary>
		public SolidColor TextColor
		{
			get
			{
				CheckForReferenceOwner(this.rowHeader);

				return (SolidColor)this.Worksheet.GetRangeStyle(this.rowHeader.Index, 0, 1, -1, PlainStyleFlag.TextColor);
			}
			set
			{
				CheckForReferenceOwner(this.rowHeader);

				//TODO: reduce create style object
				base.SetStyle(this.rowHeader.Index, 0, 1, -1,
					new WorksheetRangeStyle
					{
						Flag = PlainStyleFlag.TextColor,
						TextColor = value
					});
			}
		}

	}
	#endregion // RowHeaderStyle

	#region ReferenceCellStyle
	/// <summary>
	/// Referenced cell style
	/// </summary>
	public class ReferenceCellStyle : ReferenceStyle
	{
		/// <summary>
		/// Referenced cell instance
		/// </summary>
		public Cell Cell { get; private set; }

		/// <summary>
		/// Create referenced cell style
		/// </summary>
		/// <param name="sheet"></param>
		/// <param name="cell"></param>
		public ReferenceCellStyle(Cell cell)
			: base(cell.Worksheet)
		{
			this.Cell = cell;
		}

		/// <summary>
		/// Get or set cell background color.
		/// </summary>
		public SolidColor BackColor
		{
			get
			{
				CheckReferenceValidity();
				return this.Cell.InnerStyle.BackColor;
			}
			set
			{
				CheckReferenceValidity();
				this.Worksheet.SetCellStyleOwn(this.Cell, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.BackColor,
					BackColor = value,
				});
			}
		}

		/// <summary>
		/// Get or set the horizontal alignment for the cell content.
		/// </summary>
		public ReoGridHorAlign HAlign
		{
			get
			{
				CheckReferenceValidity();
				return this.Cell.InnerStyle.HAlign;
			}
			set
			{
				CheckReferenceValidity();
				this.Worksheet.SetCellStyleOwn(this.Cell, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.HorizontalAlign,
					HAlign = value,
				});
			}
		}

		/// <summary>
		/// Get or set the vertical alignment for the cell content.
		/// </summary>
		public ReoGridVerAlign VAlign
		{
			get
			{
				CheckReferenceValidity();
				return this.Cell.InnerStyle.VAlign;
			}
			set
			{
				CheckReferenceValidity();
				this.Worksheet.SetCellStyleOwn(this.Cell, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.VerticalAlign,
					VAlign = value,
				});
			}
		}

		/// <summary>
		/// Get or set text color of cell.
		/// </summary>
		public SolidColor TextColor
		{
			get
			{
				CheckReferenceValidity();

				//SolidColor textColor;

				//if (!this.Cell.RenderColor.IsTransparent)
				//{
				//	// render color, used to render negative number, specified by data formatter
				//	textColor = this.Cell.RenderColor;
				//}
				//else if (this.Cell.InnerStyle.HasStyle(PlainStyleFlag.TextColor))
				//{
				//	// cell text color, specified by SetRangeStyle
				//	textColor = this.Cell.InnerStyle.TextColor;
				//}
				//// default cell text color
				//else if (!this.Cell.Worksheet.controlAdapter.ControlStyle.TryGetColor(
				//	ControlAppearanceColors.GridText, out textColor))
				//{
				//	// default built-in text
				//	textColor = SolidColor.Black;
				//}

				return this.Cell.InnerStyle.TextColor;
			}
			set
			{
				CheckReferenceValidity();
				this.Worksheet.SetCellStyleOwn(this.Cell, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.TextColor,
					TextColor = value,
				});
			}
		}

		/// <summary>
		/// Get or set font name of cell.
		/// </summary>
		public string FontName
		{
			get
			{
				CheckReferenceValidity();
				return this.Cell.InnerStyle.FontName;
			}
			set
			{
				CheckReferenceValidity();
				this.Worksheet.SetCellStyleOwn(this.Cell, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.FontName,
					FontName = value,
				});
			}
		}

		/// <summary>
		/// Get or set font name of cell.
		/// </summary>
		public float FontSize
		{
			get
			{
				CheckReferenceValidity();
				return this.Cell.InnerStyle.FontSize;
			}
			set
			{
				CheckReferenceValidity();

				this.Worksheet.SetCellStyleOwn(this.Cell, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.FontSize,
					FontSize = value,
				});

				this.Worksheet.RequestInvalidate();
			}
		}

		/// <summary>
		/// Determine whether or not the font style is bold.
		/// </summary>
		public bool Bold
		{
			get
			{
				CheckReferenceValidity();
				return this.Cell.InnerStyle.Bold;
			}
			set
			{
				CheckReferenceValidity();

				this.Worksheet.SetCellStyleOwn(this.Cell, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.FontStyleBold,
					Bold = value,
				});

				this.Worksheet.RequestInvalidate();
			}
		}

		/// <summary>
		/// Determine whether or not the font style is italic.
		/// </summary>
		public bool Italic
		{
			get
			{
				CheckReferenceValidity();
				return this.Cell.InnerStyle.Italic;
			}
			set
			{
				CheckReferenceValidity();

				this.Worksheet.SetCellStyleOwn(this.Cell, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.FontStyleItalic,
					Italic = value,
				});

				this.Worksheet.RequestInvalidate();
			}
		}

		/// <summary>
		/// Determine whether or not the font style has strikethrough.
		/// </summary>
		public bool Strikethrough
		{
			get
			{
				CheckReferenceValidity();
				return this.Cell.InnerStyle.Strikethrough;
			}
			set
			{
				CheckReferenceValidity();

				this.Worksheet.SetCellStyleOwn(this.Cell, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.FontStyleStrikethrough,
					Strikethrough = value,
				});

				this.Worksheet.RequestInvalidate();
			}
		}

		/// <summary>
		/// Determine whether or not the font style has underline.
		/// </summary>
		public bool Underline
		{
			get
			{
				CheckReferenceValidity();
				return this.Cell.InnerStyle.Underline;
			}
			set
			{
				CheckReferenceValidity();

				this.Worksheet.SetCellStyleOwn(this.Cell, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.FontStyleUnderline,
					Underline = value,
				});
			}
		}

		/// <summary>
		/// Get or set the cell text-wrap mode.
		/// </summary>
		public TextWrapMode TextWrap
		{
			get
			{
				CheckReferenceValidity();
				return this.Cell.InnerStyle.TextWrapMode;
			}
			set
			{
				CheckReferenceValidity();
				this.Worksheet.SetCellStyleOwn(this.Cell, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.TextWrap,
					TextWrapMode = value,
				});
			}
		}

		/// <summary>
		/// Get or set cell indent.
		/// </summary>
		public ushort Indent
		{
			get
			{
				CheckReferenceValidity();
				return this.Cell.InnerStyle.Indent;
			}
			set
			{
				CheckReferenceValidity();
				this.Worksheet.SetCellStyleOwn(this.Cell, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.Indent,
					Indent = value,
				});
			}
		}

		/// <summary>
		/// Get or set padding of cell layout.
		/// </summary>
		public PaddingValue Padding
		{
			get
			{
				CheckReferenceValidity();
				return this.Cell.InnerStyle.Padding;
			}
			set
			{
				CheckReferenceValidity();
				this.Worksheet.SetCellStyleOwn(this.Cell, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.Padding,
					Padding = value,
				});
			}
		}

		/// <summary>
		/// Get or set text rotation angle. (-90° ~ 90°)
		/// </summary>
		public float RotationAngle
		{
			get
			{
				CheckReferenceValidity();
				return this.Cell.InnerStyle.RotationAngle;
			}
			set
			{
				CheckReferenceValidity();
				this.Worksheet.SetCellStyleOwn(this.Cell, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.RotationAngle,
					RotationAngle = value,
				});
			}
		}

		private void CheckReferenceValidity()
		{
			if (this.Cell == null || this.Worksheet == null)
			{
				throw new ReferenceObjectNotAssociatedException("ReferenceCellStyle must be associated to an valid cell and grid control.");
			}
		}
	}
	#endregion // ReferenceCellStyle
}
