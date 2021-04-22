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

using unvell.ReoGrid.Core;
using unvell.ReoGrid.Events;
using unvell.ReoGrid.Graphics;

#if WINFORM || WPF
//using HBorderArray = unvell.ReoGrid.Data.JaggedTreeArray<unvell.ReoGrid.Core.ReoGridHBorder>;
//using VBorderArray = unvell.ReoGrid.Data.JaggedTreeArray<unvell.ReoGrid.Core.ReoGridVBorder>;
using HBorderArray = unvell.ReoGrid.Data.Index4DArray<unvell.ReoGrid.Core.ReoGridHBorder>;
using VBorderArray = unvell.ReoGrid.Data.Index4DArray<unvell.ReoGrid.Core.ReoGridVBorder>;
#elif ANDROID || iOS
using HBorderArray = unvell.ReoGrid.Data.ReoGridHBorderArray;
using VBorderArray = unvell.ReoGrid.Data.ReoGridVBorderArray;
#endif // ANDROID

namespace unvell.ReoGrid.Core
{
	[Serializable]
	internal class ReoGridHBorder : BaseBorder
	{

		private HBorderOwnerPosition pos;

		internal HBorderOwnerPosition Pos
		{
			get { return pos; }
			set { pos = value; }
		}

		public static ReoGridHBorder Clone(ReoGridHBorder source)
		{
			return source == null ? null : new ReoGridHBorder
			{
				Span = source.Span,
				pos = source.Pos,
				Style = source.Style,
			};
		}
	}

	[Serializable]
	internal class ReoGridVBorder : BaseBorder
	{
		private VBorderOwnerPosition pos;

		internal VBorderOwnerPosition Pos
		{
			get { return pos; }
			set { pos = value; }
		}

		public static ReoGridVBorder Clone(ReoGridVBorder source)
		{
			return source == null ? null : new ReoGridVBorder
			{
				Span = source.Span,
				pos = source.Pos,
				Style = source.Style,
			};
		}
	}

	internal enum HBorderOwnerPosition : int
	{
		None = 0,

		All = Top | Bottom,
		Top = 1,
		Bottom = 2,
	}

	internal enum VBorderOwnerPosition : int
	{
		None = 0,

		All = Left | Right,
		Left = 1,
		Right = 2,
	}

	[Serializable]
	internal abstract class BaseBorder
	{
		public int Span { get; set; }

		public RangeBorderStyle Style { get; set; }
	}
}

namespace unvell.ReoGrid
{
	partial class Worksheet
	{
		internal HBorderArray hBorders = new HBorderArray();
		internal VBorderArray vBorders = new VBorderArray();

		#region Set Border

		/// <summary>
		/// Set borders to specified range
		/// </summary>
		/// <param name="addressOrName">range specified by address</param>
		/// <param name="pos">positions relative to the specified range to set borders</param>
		/// <param name="style">border style information</param>
		/// <exception cref="InvalidAddressException">throw if specified address or name is illegal</exception>
		public void SetRangeBorders(string addressOrName, BorderPositions pos, RangeBorderStyle style)
		{
			if (RangePosition.IsValidAddress(addressOrName))
			{
				SetRangeBorders(new RangePosition(addressOrName), pos, style);
			}
			else if (this.registeredNamedRanges.TryGetValue(addressOrName, out var namedRange))
			{
				SetRangeBorders(namedRange, pos, style);
			}
			else
				throw new InvalidAddressException(addressOrName);
		}

		/// <summary>
		/// Set borders to specified range
		/// </summary>
		/// <param name="row">number of start row</param>
		/// <param name="col">number of start column</param>
		/// <param name="rows">number of rows</param>
		/// <param name="cols">number of columns</param>
		/// <param name="pos">position around specified range to be set border</param>
		/// <param name="style">style of border to be set</param>
		public void SetRangeBorders(int row, int col, int rows, int cols, BorderPositions pos, RangeBorderStyle style)
		{
			this.SetRangeBorders(new RangePosition(row, col, rows, cols), pos, style);
		}

		/// <summary>
		/// Set border styles to specified range. Or set an empty border style to remove styles from specified range.
		/// </summary>
		/// <param name="range">Specified range to be set</param>
		/// <param name="pos">Style of which position in range should be setted</param>
		/// <see cref="BorderPositions"/>
		/// <param name="style">The style of border to be set</param>
		/// <see cref="RangeBorderStyle"/>
		public void SetRangeBorders(RangePosition range, BorderPositions pos, RangeBorderStyle style)
		{
			this.SetRangeBorders(FixRange(range), pos, style, true);
		}

		internal void SetRangeBorders(RangePosition range, BorderPositions pos, RangeBorderStyle style, bool invalidateWorksheet)
		{
			range = FixRange(range);

			int r1 = range.Row;
			int c1 = range.Col;
			int r2 = range.EndRow + 1;
			int c2 = range.EndCol + 1;

			if ((pos & BorderPositions.Right) == BorderPositions.Right
				&& c2 > this.cols.Count)
			{
				throw new ArgumentOutOfRangeException("Right side of range out of worksheet.");
			}

			if ((pos & BorderPositions.Bottom) == BorderPositions.Bottom
				&& r2 > this.rows.Count)
			{
				throw new ArgumentOutOfRangeException("Bottom side of range out of worksheet.");
			}

			#region Left and Right
			// vertical outside
			if ((pos & BorderPositions.Left) == BorderPositions.Left)
			{
				CutBeforeVBorder(r1, c1);
				SetVBorders(r1, c1, range.Rows, style, VBorderOwnerPosition.Left);
			}
			if ((pos & BorderPositions.Right) == BorderPositions.Right)
			{
				CutBeforeVBorder(r1, c2);
				SetVBorders(r1, c2, range.Rows, style, VBorderOwnerPosition.Right);
			}
			#endregion // Left and Right

			#region Top and Bottom
			// horzontial outside
			if ((pos & BorderPositions.Top) == BorderPositions.Top)
			{
				CutBeforeHBorder(r1, c1);
				SetHBorders(r1, c1, range.Cols, style, HBorderOwnerPosition.Top);
			}
			if ((pos & BorderPositions.Bottom) == BorderPositions.Bottom)
			{
				CutBeforeHBorder(r2, c1);
				SetHBorders(r2, c1, range.Cols, style, HBorderOwnerPosition.Bottom);
			}
			#endregion // Top and Bottom

			#region Inside
			// inside
			if ((pos & BorderPositions.InsideVertical) == BorderPositions.InsideVertical)
			{
				for (int c = c1 + 1; c < c2; c++)
				{
					CutBeforeVBorder(r1, c);
					SetVBorders(r1, c, range.Rows, style, VBorderOwnerPosition.All);
				}
			}
			if ((pos & BorderPositions.InsideHorizontal) == BorderPositions.InsideHorizontal)
			{
				for (int r = r1 + 1; r < r2; r++)
				{
					CutBeforeHBorder(r, c1);
					SetHBorders(r, c1, range.Cols, style, HBorderOwnerPosition.All);
				}
			}
			#endregion // Inside

			if (invalidateWorksheet)
			{
				RequestInvalidate();
			}

			// raise border added event
			if (!style.IsEmpty)
			{
				BorderAdded?.Invoke(this, new BorderAddedEventArgs(range, pos, style));
			}
			else
			{
				BorderRemoved?.Invoke(this, new BorderRemovedEventArgs(range, pos));
			}
		}

		/// <summary>
		/// Remove border style from specified range.
		/// </summary>
		/// <param name="range">Range to be removed</param>
		/// <param name="pos">Style of which position in range should be removed</param>
		public void RemoveRangeBorders(RangePosition range, BorderPositions pos)
		{
			SetRangeBorders(range, pos, RangeBorderStyle.Empty);
		}

		/// <summary>
		/// Event fired when any border styles be setted.
		/// </summary>
		public event EventHandler<BorderAddedEventArgs> BorderAdded;

		/// <summary>
		/// Event fired when any border styles be removed.
		/// </summary>
		public event EventHandler<BorderRemovedEventArgs> BorderRemoved;
		#endregion

		#region Get Border

		/// <summary>
		/// Get borders information from specified range identified by address or name.
		/// </summary>
		/// <param name="row">Number of row of the range to get borders.</param>
		/// <param name="col">Number of column of the range to get borders.</param>
		/// <param name="rows">Number of rows of the range to get borders.</param>
		/// <param name="cols">Number of columns of the range to get borders.</param>
		/// <param name="pos">Specifies where to get the borders around the range or cell. For example, 
		/// pass <code>BorderPositions.Outside</code> to get outside borders around specified address; 
		/// pass <code>BorderPositions.All</code> to get all borders from the range.</param>
		/// <param name="onlyCellsOwn">True to get the borders only owned by the range or cell.</param>
		/// <returns>Borders information object.</returns>
		public RangeBorderInfoSet GetRangeBorders(int row, int col, int rows, int cols,
			BorderPositions pos = BorderPositions.All, bool onlyCellsOwn = true)
		{
			return this.GetRangeBorders(new RangePosition(row, col, rows, cols), pos, onlyCellsOwn);
		}

		/// <summary>
		/// Get borders information from specified range identified by address or name
		/// </summary>
		/// <param name="addressOrName">An valid address or name to locate the range on worksheet</param>
		/// <param name="pos">Specifies where to get the borders around the range or cell. For example, 
		/// pass <code>BorderPositions.Outside</code> to get outside borders around specified address; 
		/// pass <code>BorderPositions.All</code> to get all borders from the range.</param>
		/// <param name="onlyCellsOwn">True to get the borders only owned by the range or cell.</param>
		/// <returns>Borders information object.</returns>
		public RangeBorderInfoSet GetRangeBorders(string addressOrName, BorderPositions pos = BorderPositions.All, bool onlyCellsOwn = true)
		{
			NamedRange namedRange;
			if (RangePosition.IsValidAddress(addressOrName))
			{
				return GetRangeBorders(new RangePosition(addressOrName), pos, onlyCellsOwn);
			}
			else if (NamedRange.IsValidName(addressOrName)
				&& this.registeredNamedRanges.TryGetValue(addressOrName, out namedRange))
			{
				return GetRangeBorders(namedRange, pos, onlyCellsOwn);
			}
			else
			{
				throw new InvalidAddressException(addressOrName);
			}
		}

		/// <summary>
		/// Get borders info from specified range.
		/// </summary>
		/// <param name="range">Range to get borders.</param>
		/// <param name="pos">Target position of range to get borders.</param>
		/// <param name="onlyCellsOwn">Indicates whether or not to get only the borders that belong to the cells in given range.</param>
		/// <returns>Borders info retrieved from specified range.</returns>
		public RangeBorderInfoSet GetRangeBorders(RangePosition range, BorderPositions pos = BorderPositions.All, bool onlyCellsOwn = true)
		{
			RangePosition fixedRange = FixRange(range);

			RangeBorderInfoSet borderInfo = new RangeBorderInfoSet();

			if (fixedRange.Rows == 0 || fixedRange.Cols == 0) return borderInfo;

			// top
			if (pos.Has(BorderPositions.Top))
			{
				borderInfo.Top = this.GetGridBorder(fixedRange.Row, fixedRange.Col, BorderPositions.Top, onlyCellsOwn);
				for (int c = fixedRange.Col + 1; c <= fixedRange.EndCol; c++)
				{
					if (borderInfo.Top != null)
					{
						if (hBorders[fixedRange.Row, c] == null
							|| hBorders[fixedRange.Row, c].Style != null
							&& !hBorders[fixedRange.Row, c].Style.Equals(borderInfo.Top))
						{
							borderInfo.NonUniformPos |= BorderPositions.Top;
							break;
						}
					}
					else
					{
						if (hBorders[fixedRange.Row, c] != null
							&& hBorders[fixedRange.Row, c].Style != null)
						{
							borderInfo.NonUniformPos |= BorderPositions.Top;
							break;
						}
					}
				}
			}

			// bottom
			if (pos.Has(BorderPositions.Bottom))
			{
				borderInfo.Bottom = this.GetGridBorder(fixedRange.EndRow, fixedRange.Col, BorderPositions.Bottom, onlyCellsOwn);
				if (borderInfo.Bottom != null)
				{
					for (int c = fixedRange.Col + 1; c <= fixedRange.EndCol; c++)
					{
						if (hBorders[fixedRange.EndRow + 1, c] == null
							|| hBorders[fixedRange.EndRow + 1, c].Style != null
							&& !hBorders[fixedRange.EndRow + 1, c].Style.Equals(borderInfo.Bottom))
						{
							borderInfo.NonUniformPos |= BorderPositions.Bottom;
							break;
						}
					}
				}
				else
				{
					for (int c = fixedRange.Col + 1; c <= fixedRange.EndCol; c++)
					{
						if (hBorders[fixedRange.EndRow + 1, c] != null
							&& hBorders[fixedRange.EndRow + 1, c].Style != null)
						{
							borderInfo.NonUniformPos |= BorderPositions.Bottom;
							break;
						}
					}
				}
			}

			// left
			if (pos.Has(BorderPositions.Left))
			{
				borderInfo.Left = this.GetGridBorder(fixedRange.Row, fixedRange.Col, BorderPositions.Left, onlyCellsOwn);
				if (borderInfo.Left != null)
				{
					for (int r = fixedRange.Row + 1; r < fixedRange.EndRow; r++)
					{
						if (vBorders[r, fixedRange.Col] == null
							|| vBorders[r, fixedRange.Col].Style != null
							&& !vBorders[r, fixedRange.Col].Style.Equals(borderInfo.Left))
						{
							borderInfo.NonUniformPos |= BorderPositions.Left;
							break;
						}
					}
				}
				else
				{
					for (int r = fixedRange.Row + 1; r <= fixedRange.EndRow; r++)
					{
						if (vBorders[r, fixedRange.Col] != null
							&& vBorders[r, fixedRange.Col].Style != null)
						{
							borderInfo.NonUniformPos |= BorderPositions.Left;
							break;
						}
					}
				}
			}

			// right
			if (pos.Has(BorderPositions.Right))
			{
				borderInfo.Right = this.GetGridBorder(fixedRange.Row, fixedRange.EndCol, BorderPositions.Right, onlyCellsOwn);
				if (borderInfo.Right != null)
				{
					for (int r = fixedRange.Row + 1; r < fixedRange.EndRow; r++)
					{
						if (vBorders[r, fixedRange.EndCol + 1] == null
							|| vBorders[r, fixedRange.EndCol + 1].Style != null
							&& !vBorders[r, fixedRange.EndCol + 1].Style.Equals(borderInfo.Right))
						{
							borderInfo.NonUniformPos |= BorderPositions.Right;
							break;
						}
					}
				}
				else
				{
					for (int r = fixedRange.Row + 1; r < fixedRange.EndRow; r++)
					{
						if (vBorders[r, fixedRange.EndCol + 1] != null
							&& vBorders[r, fixedRange.EndCol + 1].Style != null)
						{
							borderInfo.NonUniformPos |= BorderPositions.Right;
							break;
						}
					}
				}
			}

			bool hasSetted = false;

			if (pos.Has(BorderPositions.InsideHorizontal))
			{
				// inside horizontal
				int hbEndRow = Math.Min(fixedRange.EndRow, hBorders.MaxRow);
				int hbEndCol = Math.Min(fixedRange.EndCol, hBorders.MaxCol);

				for (int r = fixedRange.Row + 1; r <= hbEndRow; r++)
				{
					for (int c = fixedRange.Col; c <= hbEndCol;)
					{
						ReoGridHBorder hBorder = hBorders[r, c];
						if (hBorder != null)
						{
							if (hBorder.Span == 0)
							{
								c++;
								// ignore border in merged region
								continue;
							}
							else
								c += hBorder.Span;

							if (!hasSetted) borderInfo.InsideHorizontal = hBorder.Style;
						}
						else
						{
							c++;
							if (!hasSetted) borderInfo.InsideHorizontal = RangeBorderStyle.Empty;
						}

						if (hasSetted)
						{
							if (!((hBorder == null && borderInfo.InsideHorizontal.IsEmpty)
								|| (hBorder != null && !borderInfo.InsideHorizontal.IsEmpty
								&& hBorder.Style == borderInfo.InsideHorizontal)))
							{
								borderInfo.NonUniformPos |= BorderPositions.InsideHorizontal;
								r = fixedRange.EndRow;
								break;
							}
						}

						hasSetted = true;
					}
				}
			}

			hasSetted = false;

			if (pos.Has(BorderPositions.InsideVertical))
			{
				// inside vertical
				int vbEndRow = Math.Min(fixedRange.EndRow, vBorders.MaxRow);
				int vbEndCol = Math.Min(fixedRange.EndCol, vBorders.MaxCol);

				for (int c = fixedRange.Col + 1; c <= vbEndCol; c++)
				{
					for (int r = fixedRange.Row; r <= vbEndRow;)
					{
						ReoGridVBorder vBorder = vBorders[r, c];
						if (vBorder != null)
						{
							if (vBorder.Span == 0)
							{
								r++;
								// ignore border in merged region
								continue;
							}
							else
								r += vBorder.Span;

							if (!hasSetted) borderInfo.InsideVertical = vBorder.Style;
						}
						else
						{
							r++;
							if (!hasSetted) borderInfo.InsideVertical = RangeBorderStyle.Empty;
						}

						if (hasSetted)
						{
							if (!((vBorder == null && borderInfo.InsideVertical.IsEmpty)
								|| (vBorder != null && !borderInfo.InsideVertical.IsEmpty
								&& vBorder.Style == borderInfo.InsideVertical)))
							{
								borderInfo.NonUniformPos |= BorderPositions.InsideVertical;
								c = fixedRange.EndCol;
								break;
							}
						}

						hasSetted = true;
					}
				}
			}

			return borderInfo;
		}

		internal RangeBorderStyle GetGridBorder(int row, int col, BorderPositions pos, bool onlyGridOwn)
		{
			if (pos == BorderPositions.Top
				&& hBorders[row, col] != null
				&& (!onlyGridOwn
				|| (hBorders[row, col].Pos & HBorderOwnerPosition.Top) == HBorderOwnerPosition.Top))
			{
				return hBorders[row, col].Style;
			}
			else if (pos == BorderPositions.Bottom
				&& hBorders[row + 1, col] != null
				&& (!onlyGridOwn
				|| (hBorders[row + 1, col].Pos & HBorderOwnerPosition.Bottom) == HBorderOwnerPosition.Bottom))
			{
				return hBorders[row + 1, col].Style;
			}
			else if (pos == BorderPositions.Left
				&& vBorders[row, col] != null
				&& (!onlyGridOwn
				|| (vBorders[row, col].Pos & VBorderOwnerPosition.Left) == VBorderOwnerPosition.Left))
			{
				return vBorders[row, col].Style;
			}
			else if (pos == BorderPositions.Right
				&& vBorders[row, col + 1] != null
				&& (!onlyGridOwn
				|| (vBorders[row, col + 1].Pos & VBorderOwnerPosition.Right) == VBorderOwnerPosition.Right))
			{
				return vBorders[row, col + 1].Style;
			}
			else
				return RangeBorderStyle.Empty;
		}
		#endregion // Get Border

		#region Iterate Borders
		/// <summary>
		/// Iterate over all borders in specified range.
		/// </summary>
		/// <param name="scanDirections">Specifies that borders iterated on which direction, horizontal borders, 
		/// vertical borders or both horizontal and vertical borders.</param>
		/// <param name="range">The range to iterate over all border from.</param>
		/// <param name="iterator">Callback anonymous function that is used to iterate each returned border from range.</param>
		public void IterateBorders(RowOrColumn scanDirections, RangePosition range, Func<int, int, int, RangeBorderStyle, bool> iterator)
		{
			range = this.FixRange(range);

			if ((scanDirections & RowOrColumn.Row) == RowOrColumn.Row)
			{
				for (int r = range.Row; r <= range.EndRow; r++)
				{
					for (int c = range.Col; c <= range.EndCol;)
					{
						var border = this.hBorders[r, c];

						if (border != null)
						{
							if (border.Span > 0)
							{
								if (!iterator(r, c, border.Span, border.Style))
								{
									goto outLoopRow;
								}

								c += border.Span;
								continue;
							}
						}

						c++;
					}
				}
			}
			outLoopRow:

			if ((scanDirections & RowOrColumn.Column) == RowOrColumn.Column)
			{
				for (int c = range.Col; c <= range.EndCol; c++)
				{
					for (int r = range.Row; r <= range.EndRow;)
					{
						var border = this.vBorders[r, c];

						if (border != null)
						{
							if (border.Span > 0)
							{
								if (!iterator(r, c, border.Span, border.Style))
								{
									goto outLoopColumn;
								}

								r += border.Span;
								continue;
							}
						}

						r++;
					}
				}
			}
			outLoopColumn:
			return;
		}
		#endregion // Iterate Borders

		#region Internal Border Utilities

		#region Get & Retrieve

		internal ReoGridHBorder GetHBorder(int row, int col)
		{
			ReoGridHBorder border = hBorders[row, col];
			if (border == null)
			{
				border = hBorders[row, col] = new ReoGridHBorder();
			}
			return border;
		}
		internal ReoGridVBorder GetVBorder(int row, int col)
		{
			ReoGridVBorder border = vBorders[row, col];
			if (border == null)
			{
				border = vBorders[row, col] = new ReoGridVBorder();
			}
			return border;
		}

		internal ReoGridHBorder RetrieveHBorder(int row, int col)
		{
			return hBorders[row, col];
		}
		internal ReoGridVBorder RetrieveVBorder(int row, int col)
		{
			return vBorders[row, col];
		}

		internal ReoGridHBorder GetValidHBorderByPos(int row, int col, HBorderOwnerPosition pos)
		{
			Core.ReoGridHBorder b = hBorders[row, col];
			if (b == null || b.Span == 0)
				return null;
			else if ((b.Pos & pos) == pos)
				return b;
			else
				return null;
		}
		internal ReoGridVBorder GetValidVBorderByPos(int row, int col, VBorderOwnerPosition pos)
		{
			Core.ReoGridVBorder b = vBorders[row, col];
			if (b == null || b.Span == 0)
				return null;
			else if ((b.Pos & pos) == pos)
				return b;
			else
				return null;
		}

		#endregion // Get & Retrieve

		#region Set H Border
		private void CutBeforeHBorder(int row, int col)
		{
			if (col < 1) return;

			ReoGridHBorder prevBorder = hBorders[row, col - 1];
			if (prevBorder != null && prevBorder.Span > 1)
			{
				int c = col - 1;
				int offset = prevBorder.Span;
				while (c >= 0)
				{
					if (hBorders[row, c] == null || hBorders[row, c].Span <= 1) break;
					hBorders[row, c].Span -= offset - 1;
					c--;
				}
			}
		}

		private int FindSameHBorderLeft(int row, int col, RangeBorderStyle borderStyle)
		{
			if (col <= 0) return col;

			for (int c = col - 1; c >= 0; c--)
			{
				var hb = hBorders[row, c];

				if (hb == null)
					return c + 1;
				else if (hb.Style == null && borderStyle != null)
					return c + 1;
				//else if (hBorders[row, c].Border != null && borderStyle == null)
				//  return c + 1;
				else if (hb.Style != null && !hb.Style.Equals(borderStyle))
					return c + 1;
			}
			return 0;
		}

		private int FindSameHBorderRight(int row, int col, RangeBorderStyle borderStyle)
		{
			if (col > this.cols.Count - 1) return col;

			for (int c = col + 1; c < this.cols.Count; c++)
			{
				var hb = hBorders[row, c];

				if (hb == null)
					return c - 1;
				else if (hb == null && borderStyle != null)
					return c - 1;
				else if (!hb.Style.Equals(borderStyle))
					return c - 1;
			}

			return this.cols.Count - 1;
		}

		private void FillHBorders(int row, int col, int cols, RangeBorderStyle borderStyle, HBorderOwnerPosition pos)
		{
			int tc = col + cols;
			for (int c = col; c < tc; c++)
			{
				SetHBorder(row, c, tc - c, borderStyle, pos);
			}
		}

		private void SetHBorders(int row, int col, int cols, RangeBorderStyle borderStyle, HBorderOwnerPosition pos)
		{
			// todo: improve performance with inverted foreach

			// check whether the border does exist already
			//ReoGridHBorder border = hBorders[row, col];

			//if (border != null && cols <= border.Cols && border.Style == borderStyle)
			//{
			//	// don't set border if already exist in order to improvement performance
			//	return;
			//}

			int sc = col;
			int ec = col + cols - 1;

			if (borderStyle != null)
			{
				sc = FindSameHBorderLeft(row, col, borderStyle);
				ec = FindSameHBorderRight(row, col + cols - 1, borderStyle);
				cols = ec - sc + 1;
			}
			// find previous
			//	cols += ec - col;

			int c2 = sc + cols;

			// when the cols splitted by a merged cell, need find and skip it
			int tc = c2;
			int nextStartCol = -1;
			for (int k = sc; k < c2; k++)
			{
				if (row > 0)
				{
					Cell cell1 = cells[row - 1, k];
					Cell cell2 = cells[row, k];

					if (cell1 != null && cell2 != null
						&& !cell1.MergeEndPos.IsEmpty
						&& CellPosition.Equals(cell1.MergeStartPos, cell2.MergeStartPos))
					{
						tc = k;
						nextStartCol = cell2.MergeEndPos.Col + 1;
						break;
					}
				}
			}

			FillHBorders(row, sc, tc - sc, borderStyle, pos);

			// if border is splitted by merged cell, need skip it and set the remains
			if (nextStartCol != -1)
			{
				SetHBorders(row, nextStartCol, c2 - nextStartCol, borderStyle, pos);
			}
		}

		private void SetHBorder(int row, int col, int colspan, RangeBorderStyle borderStyle, HBorderOwnerPosition pos)
		{
			if (borderStyle == null)
			{
				// clear border
				hBorders[row, col] = null;
			}
			else
			{
				// set border style
				ReoGridHBorder hBorder = GetHBorder(row, col);
				hBorder.Span = colspan;
				hBorder.Style = borderStyle;

				// apply border owner pos
				// v088: no need remove pos, unreached case
				//if (row == 0) pos &= ~HBorderOwnerPosition.Bottom;
				//if (row == this.rows.Count) pos &= ~HBorderOwnerPosition.Top;
				hBorder.Pos |= pos;
			}
		}

		#endregion // Set H Border

		#region Set V Border
		private int FindSameVBorderTop(int row, int col, RangeBorderStyle borderStyle)
		{
			if (row <= 0) return row;

			for (int r = row - 1; r >= 0; r--)
			{
				var vb = vBorders[r, col];

				if (vb == null)
					return r + 1;
				else if (vb.Style == null && borderStyle != null)
					return r + 1;
				//else if (vBorders[r, col].Border != null && borderStyle == null)
				//  return r + 1;
				else if (vb.Style != null && !vb.Style.Equals(borderStyle))
					return r + 1;
			}

			return 0;
		}

		private int FindSameVBorderBottom(int row, int col, RangeBorderStyle borderStyle)
		{
			if (row > this.rows.Count - 1) return row;

			for (int r = row + 1; r < this.rows.Count; r++)
			{
				var vb = vBorders[r, col];

				if (vb == null)
					return r - 1;
				else if (vb.Style == null && borderStyle != null)
					return r - 1;
				else if (!vb.Style.Equals(borderStyle))
					return r - 1;
			}

			return this.rows.Count - 1;
		}

		private void FillVBorders(int row, int col, int rows, RangeBorderStyle borderStyle, VBorderOwnerPosition pos)
		{
			int tr = row + rows;
			for (int r = row; r < tr; r++)
			{
				SetVBorder(r, col, tr - r, borderStyle, pos);
			}
		}

		private void CutBeforeVBorder(int row, int col)
		{
			if (row < 1) return;

			ReoGridVBorder prevBorder = vBorders[row - 1, col];
			if (prevBorder != null && prevBorder.Span > 1)
			{
				int r = row - 1;
				int offset = prevBorder.Span;
				while (r >= 0)
				{
					if (vBorders[r, col] == null || vBorders[r, col].Span <= 1) break;
					vBorders[r, col].Span -= offset - 1;
					r--;
				}
			}
		}

		private void SetVBorders(int row, int col, int rows, RangeBorderStyle borderStyle, VBorderOwnerPosition pos)
		{
			int sr = row;
			int er = row + rows - 1;

			if (borderStyle != null)
			{
				// find previous
				sr = FindSameVBorderTop(row, col, borderStyle);
				er = FindSameVBorderBottom(row + rows - 1, col, borderStyle);
				rows = er - sr + 1;
			}

			int r2 = sr + rows;

			// when the cols splitted by a merged range, need find it
			int tr = r2;
			int nextStartRow = -1;
			for (int k = sr; k < r2; k++)
			{
				if (col > 0)
				{
					Cell cell1 = cells[k, col - 1];
					Cell cell2 = cells[k, col];

					if (cell1 != null && cell2 != null
												&& !cell1.MergeStartPos.IsEmpty
												&& CellPosition.Equals(cell1.MergeStartPos, cell2.MergeStartPos))
					{
						tr = k;
						nextStartRow = cell2.MergeEndPos.Row + 1;
						break;
					}
				}
			}

			FillVBorders(sr, col, tr - sr, borderStyle, pos);

			// if border splitted by merged range, set the remains
			if (nextStartRow != -1)
			{
				SetVBorders(nextStartRow, col, r2 - nextStartRow, borderStyle, pos);
			}
		}

		private void SetVBorder(int row, int col, int rowspan, RangeBorderStyle borderStyle, VBorderOwnerPosition pos)
		{
			if (borderStyle == null)
			{
				// clear border
				vBorders[row, col] = null;
			}
			else
			{
				// set border style
				ReoGridVBorder vBorder = GetVBorder(row, col);
				vBorder.Span = rowspan;
				vBorder.Style = borderStyle;

				// apply border owner pos
				// v088: no need remove pos, unreached case
				//if (col == 0) pos &= ~VBorderOwnerPosition.Right;
				//if (col == this.cols.Count) pos &= ~VBorderOwnerPosition.Left;
				vBorder.Pos |= pos;
			}
		}

		#endregion // Set V Border

		#endregion // Internal Border Utilities

	}

	#region ReoGridBorderPos

	/// <summary>
	/// Position of borders for a range or cell
	/// </summary>
	public enum BorderPositions : short
	{
		/// <summary>
		/// No border
		/// </summary>
		None = 0x0,

		/// <summary>
		/// Top border inside range or cell
		/// </summary>
		Top = 0x1,

		/// <summary>
		/// Bottom border inside range or cell
		/// </summary>
		Bottom = 0x2,

		/// <summary>
		/// Left side border inside range or cell
		/// </summary>
		Left = 0x4,

		/// <summary>
		/// Right side border inside range or cell
		/// </summary>
		Right = 0x8,

		/// <summary>
		/// Horizontal borders inside range or cell
		/// </summary>
		InsideHorizontal = 0x10,

		/// <summary>
		/// Vertical borders inside range or cell
		/// </summary>
		InsideVertical = 0x20,

		/// <summary>
		/// Slash lines inside cell (Reserved)
		/// </summary>
		Slash = 0x100,

		/// <summary>
		/// Backslash lines inside cell (Reserved)
		/// </summary>
		Backslash = 0x200,

		/// <summary>
		/// Borders in left and right side in range or cell
		/// </summary>
		LeftRight = Left | Right,

		/// <summary>
		/// Borders in top and bottom in range or cell
		/// </summary>
		TopBottom = Top | Bottom,

		/// <summary>
		/// Borders around range or cell
		/// </summary>
		Outside = Top | Bottom | Left | Right,

		/// <summary>
		/// Horizontal and vertical borders inside range or cell
		/// </summary>
		InsideAll = InsideHorizontal | InsideVertical,

		/// <summary>
		/// All borders belong to range or cell
		/// </summary>
		All = Outside | InsideAll,

		/// <summary>
		/// Cross line in single cell (Both Slash and Backslash, Reserved)
		/// </summary>
		X = Slash | Backslash,
	}

	#endregion // ReoGridBorderPos

	/// <summary>
	/// Line style of border.
	/// </summary>
	public enum BorderLineStyle : byte
	{
		/// <summary>
		/// None border
		/// </summary>
		None = 0,

		/// <summary>
		/// Solid border
		/// </summary>
		Solid = 1,

		/// <summary>
		/// Dotted border
		/// </summary>
		Dotted = 2,

		/// <summary>
		/// Dashed border
		/// </summary>
		Dashed = 3,

		/// <summary>
		/// Double line border (not supported in WPF version)
		/// </summary>
		DoubleLine = 4,

		/// <summary>
		/// Dashed (style 2) border (not supported in WPF version)
		/// </summary>
		Dashed2 = 5,

		/// <summary>
		/// Dashed (style 3) border
		/// </summary>
		DashDot = 6,

		/// <summary>
		/// Dashed (style 4) border
		/// </summary>
		DashDotDot = 7,

		/// <summary>
		/// Bold dashed (style 2) border
		/// </summary>
		BoldDashDot = 8,

		/// <summary>
		/// Bold dashed (style 3) border
		/// </summary>
		BoldDashDotDot = 9,

		/// <summary>
		/// Bold dashed border
		/// </summary>
		BoldDashed = 10,

		/// <summary>
		/// Bold dotted border
		/// </summary>
		BoldDotted = 11,

		/// <summary>
		/// Bold solid border
		/// </summary>
		BoldSolid = 12,

		/// <summary>
		/// Strong solid border (Bold x2)
		/// </summary>
		BoldSolidStrong = 13,
	}

	/// <summary>
	/// Represents a border style of range or cell.
	/// </summary>
	[Serializable]
	public struct RangeBorderStyle
	{
		/// <summary>
		/// No border
		/// </summary>
		public static readonly RangeBorderStyle Empty = new RangeBorderStyle
		{
			color = SolidColor.Transparent,
			style = BorderLineStyle.None,
		};

		private BorderLineStyle style;

		/// <summary>
		/// The border style
		/// </summary>
		public BorderLineStyle Style
		{
			get { return style; }
			set { style = value; }
		}

		private SolidColor color;

		/// <summary>
		/// Color of border
		/// </summary>
		public SolidColor Color
		{
			get { return color; }
			set { color = value; }
		}

		/// <summary>
		/// Determines whether this style is empty
		/// </summary>
		public bool IsEmpty
		{
			get { return this.Equals(Empty); }
		}

		/// <summary>
		/// Compare two border styles check whether they are same
		/// </summary>
		/// <param name="obj">Object to be compared</param>
		/// <returns>True if two object are same; otherwise return false.</returns>
		public override bool Equals(object obj)
		{
			if (obj == null && this.IsEmpty) return true;
			if (!(obj is RangeBorderStyle)) return false;

			RangeBorderStyle s2 = (RangeBorderStyle)obj;
			return style == s2.style && color == s2.color;
		}

		/// <summary>
		/// Compare two styles and check whether or not they are same.
		/// </summary>
		/// <param name="s1">First style to be compared.</param>
		/// <param name="s2">Second style to be compared.</param>
		/// <returns>Return true if two styles are same; Otherwise return false.</returns>
		public static bool operator ==(RangeBorderStyle s1, object s2)
		{
			return s1.Equals(s2);
		}

		/// <summary>
		/// Compare two styles and check whether or not they are not same.
		/// </summary>
		/// <param name="s1">First style to be compared.</param>
		/// <param name="s2">Second style to be compared.</param>
		/// <returns>Return true if two styles are same; Otherwise return false.</returns>
		public static bool operator !=(RangeBorderStyle s1, object s2)
		{
			return !s1.Equals(s2);
		}

		/// <summary>
		/// Return the hashcode of this object.
		/// </summary>
		/// <returns>Hashcode calculated from this object.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Convert style object into friendly description string.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("RangeBorderStyle[Color={0},Style={1}]", color, style);
		}

		/// <summary>
		/// Create range border style object with specified color and style.
		/// </summary>
		/// <param name="color">Color to display borders on worksheet.</param>
		/// <param name="style">Style to display borders on worksheet.</param>
		public RangeBorderStyle(SolidColor color, BorderLineStyle style)
		{
			this.color = color;
			this.style = style;
		}

		#region Predefine Border Styles

		/// <summary>
		/// Predefined border style of solid black border.
		/// </summary>
		public static readonly RangeBorderStyle BlackSolid = new RangeBorderStyle
		{
			Color = SolidColor.Black,
			Style = BorderLineStyle.Solid,
		};

		/// <summary>
		/// Predefined border style of solid black border.
		/// </summary>
		public static readonly RangeBorderStyle BlackBoldSolid = new RangeBorderStyle
		{
			Color = SolidColor.Black,
			Style = BorderLineStyle.BoldSolid,
		};

		/// <summary>
		/// Predefined border style of solid gray border.
		/// </summary>
		public static readonly RangeBorderStyle GraySolid = new RangeBorderStyle
		{
			Color = SolidColor.Gray,
			Style = BorderLineStyle.Solid,
		};

		/// <summary>
		/// Predefined border style of solid silver border.
		/// </summary>
		public static readonly RangeBorderStyle SilverSolid = new RangeBorderStyle
		{
			Color = SolidColor.Silver,
			Style = BorderLineStyle.Solid,
		};

		/// <summary>
		/// Predefined border style of dotted black border.
		/// </summary>
		public static readonly RangeBorderStyle BlackDotted = new RangeBorderStyle
		{
			Color = SolidColor.Black,
			Style = BorderLineStyle.Dotted,
		};

		/// <summary>
		/// Predefined border style of dotted gray border.
		/// </summary>
		public static readonly RangeBorderStyle GrayDotted = new RangeBorderStyle
		{
			Color = SolidColor.Gray,
			Style = BorderLineStyle.Dotted,
		};

		/// <summary>
		/// Predefined border style of dashed black border.
		/// </summary>
		public static readonly RangeBorderStyle BlackDash = new RangeBorderStyle
		{
			Color = SolidColor.Black,
			Style = BorderLineStyle.Dashed,
		};

		/// <summary>
		/// Predefined border style of dashed gray border.
		/// </summary>
		public static readonly RangeBorderStyle GrayDash = new RangeBorderStyle
		{
			Color = SolidColor.Gray,
			Style = BorderLineStyle.Dashed,
		};
		//#pragma warning disable 1591

		//		[Obsolete("use BlackSolid instead")]
		//		public static readonly RangeBorderStyle SolidBlack = BlackSolid;
		//		[Obsolete("use GraySolid instead")]
		//		public static readonly RangeBorderStyle SolidGray = GraySolid;
		//		[Obsolete("use BlackDotted instead")]
		//		public static readonly RangeBorderStyle DottedBlack = BlackDotted;
		//		[Obsolete("use GrayDotted instead")]
		//		public static readonly RangeBorderStyle DottedGray = GrayDotted;

		//#pragma warning restore 1591

		#endregion // Predefine Border Styles

	}

	/// <summary>
	/// This class contains the position and style information of a segment of border for a range.
	/// </summary>
	[Serializable]
	public struct RangeBorderInfo
	{
		private BorderPositions pos;

		/// <summary>
		/// Get or set the position of this border in a range.
		/// </summary>
		public BorderPositions Pos
		{
			get { return pos; }
			set { pos = value; }
		}

		private RangeBorderStyle style;

		/// <summary>
		/// Get or set the style of border in a range.
		/// </summary>
		public RangeBorderStyle Style
		{
			get { return style; }
			set { style = value; }
		}

		/// <summary>
		/// Create border information instance with specified position and style.
		/// </summary>
		/// <param name="pos">The position of border in a range.</param>
		/// <param name="style">The style of border in a range.</param>
		public RangeBorderInfo(BorderPositions pos, RangeBorderStyle style)
		{
			this.pos = pos;
			this.style = style;
		}
	}

	/// <summary>
	/// This class contains the information of all borders in specified range. 
	/// This class only be used as return value form <see cref="GetRangeBorders">GetRangeBorders</see> method.
	/// </summary>
	[Serializable]
	public class RangeBorderInfoSet
	{
		private BorderPositions hasNonUniformPos = BorderPositions.None;

		/// <summary>
		/// Borders at the positions are not same
		/// </summary>
		public BorderPositions NonUniformPos
		{
			get { return hasNonUniformPos; }
			set { hasNonUniformPos = value; }
		}

		/// <summary>
		/// Indicates whether the borders to each cells in a specified range are not same 
		/// </summary>
		/// <param name="pos">border position in range</param>
		/// <returns>true if borders at position are not same</returns>
		public bool IsNonUniform(BorderPositions pos)
		{
			return (hasNonUniformPos & pos) == pos;
		}

		private RangeBorderStyle top;

		/// <summary>
		/// Border style at top of range
		/// </summary>
		public RangeBorderStyle Top { get { return top; } set { top = value; } }

		private RangeBorderStyle right;

		/// <summary>
		/// Border style at right of range
		/// </summary>
		public RangeBorderStyle Right { get { return right; } set { right = value; } }

		private RangeBorderStyle bottom;

		/// <summary>
		/// Border style at bottom of range
		/// </summary>
		public RangeBorderStyle Bottom { get { return bottom; } set { bottom = value; } }

		private RangeBorderStyle left;

		/// <summary>
		/// Border  style at left of range
		/// </summary>
		public RangeBorderStyle Left { get { return left; } set { left = value; } }

		private RangeBorderStyle horizontal;

		/// <summary>
		/// Horizontal border style inside range
		/// </summary>
		public RangeBorderStyle InsideHorizontal { get { return horizontal; } set { horizontal = value; } }

		private RangeBorderStyle vertical;

		/// <summary>
		/// Vertical border style inside range
		/// </summary>
		public RangeBorderStyle InsideVertical { get { return vertical; } set { vertical = value; } }

		private RangeBorderStyle slash; // /

		/// <summary>
		/// Slash style inside range
		/// </summary>
		public RangeBorderStyle Slash { get { return slash; } set { slash = value; } }

		private RangeBorderStyle backslash;  // \

		/// <summary>
		/// Backslash style inside range
		/// </summary>
		public RangeBorderStyle Backslash { get { return backslash; } set { backslash = value; } }
	}

	/// <summary>
	/// Represents border property for cell instance.
	/// </summary>
	[Serializable]
	public class CellBorderProperty
	{
		private Cell cell;

		internal CellBorderProperty(Cell cell)
		{
			this.cell = cell;
		}

		private void CheckForOwnerAssociated()
		{
			if (this.cell == null)
			{
				throw new ReferenceObjectNotAssociatedException("Border property is not associated to any cells. Border might have been deleted from the cell. Try get this property from cell again.");
			}
		}

		/// <summary>
		/// Get or set left border style for cell.
		/// </summary>
		public RangeBorderStyle Left
		{
			get
			{
				CheckForOwnerAssociated();

				return this.cell.Worksheet.GetGridBorder(this.cell.Row, this.cell.Column, BorderPositions.Left, true);
			}
			set
			{
				CheckForOwnerAssociated();

				this.cell.Worksheet.SetRangeBorders(this.cell.PositionAsRange, BorderPositions.Left, value);
			}
		}

		/// <summary>
		/// Get or set top border style for cell.
		/// </summary>
		public RangeBorderStyle Top
		{
			get
			{
				CheckForOwnerAssociated();

				return this.cell.Worksheet.GetGridBorder(this.cell.Row, this.cell.Column, BorderPositions.Top, true);
			}
			set
			{
				CheckForOwnerAssociated();

				this.cell.Worksheet.SetRangeBorders(this.cell.PositionAsRange, BorderPositions.Top, value);
			}
		}

		/// <summary>
		/// Get or set right border style for cell.
		/// </summary>
		public RangeBorderStyle Right
		{
			get
			{
				CheckForOwnerAssociated();

				return this.cell.Worksheet.GetGridBorder(this.cell.Row, this.cell.Column, BorderPositions.Right, true);
			}
			set
			{
				CheckForOwnerAssociated();

				this.cell.Worksheet.SetRangeBorders(this.cell.PositionAsRange, BorderPositions.Right, value);
			}
		}

		/// <summary>
		/// Get or set bottom border style for cell.
		/// </summary>
		public RangeBorderStyle Bottom
		{
			get
			{
				CheckForOwnerAssociated();

				return this.cell.Worksheet.GetGridBorder(this.cell.Row, this.cell.Column, BorderPositions.Bottom, true);
			}
			set
			{
				CheckForOwnerAssociated();

				this.cell.Worksheet.SetRangeBorders(this.cell.PositionAsRange, BorderPositions.Bottom, value);
			}
		}

		/// <summary>
		/// Get or set all outside border styles for cell.
		/// </summary>
		public RangeBorderStyle Outside
		{
			get
			{
				CheckForOwnerAssociated();

				return this.cell.Worksheet.GetGridBorder(this.cell.Row, this.cell.Column, BorderPositions.Outside, true);
			}
			set
			{
				CheckForOwnerAssociated();

				this.cell.Worksheet.SetRangeBorders(this.cell.PositionAsRange, BorderPositions.Outside, value);
			}
		}

		/// <summary>
		/// Get or set all border styles for cell.
		/// </summary>
		public RangeBorderStyle All
		{
			get
			{
				CheckForOwnerAssociated();

				return this.cell.Worksheet.GetGridBorder(this.cell.Row, this.cell.Column, BorderPositions.All, true);
			}
			set
			{
				CheckForOwnerAssociated();

				this.cell.Worksheet.SetRangeBorders(this.cell.Row, this.cell.Column, 1, 1, BorderPositions.All, value);
			}
		}
	}

	/// <summary>
	/// Represents border property for range instance.
	/// </summary>
	public class RangeBorderProperty
	{
		private ReferenceRange range;

		internal RangeBorderProperty(ReferenceRange range)
		{
			this.range = range;
		}

		private void CheckForOwnerAssociated()
		{
			if (this.range == null)
			{
				throw new ReferenceObjectNotAssociatedException("Border property is not associated to any ranges. Border might have been deleted from range. Try get this property from range again.");
			}
		}

		private RangePosition Position { get { return this.range.Position; } }

		/// <summary>
		/// Get or set left border styles for range.
		/// </summary>
		public RangeBorderStyle Left
		{
			get
			{
				CheckForOwnerAssociated();

				return this.range.Worksheet.GetRangeBorders(this.Position, BorderPositions.Left, true).Left;
			}
			set
			{
				CheckForOwnerAssociated();

				this.range.Worksheet.SetRangeBorders(this.Position, BorderPositions.Left, value);
			}
		}

		/// <summary>
		/// Get or set top border styles for range.
		/// </summary>
		public RangeBorderStyle Top
		{
			get
			{
				CheckForOwnerAssociated();

				return this.range.Worksheet.GetRangeBorders(this.Position, BorderPositions.Top, true).Top;
			}
			set
			{
				CheckForOwnerAssociated();

				this.range.Worksheet.SetRangeBorders(this.Position, BorderPositions.Top, value);
			}
		}

		/// <summary>
		/// Get or set right border styles for range.
		/// </summary>
		public RangeBorderStyle Right
		{
			get
			{
				CheckForOwnerAssociated();

				return this.range.Worksheet.GetRangeBorders(this.Position, BorderPositions.Right, true).Right;
			}
			set
			{
				CheckForOwnerAssociated();

				this.range.Worksheet.SetRangeBorders(this.Position, BorderPositions.Right, value);
			}
		}

		/// <summary>
		/// Get or set bottom border styles for range.
		/// </summary>
		public RangeBorderStyle Bottom
		{
			get
			{
				CheckForOwnerAssociated();

				return this.range.Worksheet.GetRangeBorders(this.Position, BorderPositions.Bottom, true).Bottom;
			}
			set
			{
				CheckForOwnerAssociated();

				this.range.Worksheet.SetRangeBorders(this.Position, BorderPositions.Bottom, value);
			}
		}

		/// <summary>
		/// Get or set all inside borders style for range.
		/// </summary>
		public RangeBorderStyle Inside
		{
			get
			{
				CheckForOwnerAssociated();

				return this.range.Worksheet.GetRangeBorders(this.Position, BorderPositions.InsideAll, true).InsideHorizontal; // TODO: no outline available here
			}
			set
			{
				CheckForOwnerAssociated();

				this.range.Worksheet.SetRangeBorders(this.Position, BorderPositions.InsideAll, value);
			}
		}

		/// <summary>
		/// Get or set all horizontal border styles for range.
		/// </summary>
		public RangeBorderStyle InsideHorizontal
		{
			get
			{
				CheckForOwnerAssociated();

				return this.range.Worksheet.GetRangeBorders(this.Position, BorderPositions.InsideHorizontal, true).InsideHorizontal;
			}
			set
			{
				CheckForOwnerAssociated();

				this.range.Worksheet.SetRangeBorders(this.Position, BorderPositions.InsideHorizontal, value);
			}
		}

		/// <summary>
		/// Get or set all vertical border styles for range.
		/// </summary>
		public RangeBorderStyle InsideVertical
		{
			get
			{
				CheckForOwnerAssociated();

				return this.range.Worksheet.GetRangeBorders(this.Position, BorderPositions.InsideVertical, true).InsideVertical;
			}
			set
			{
				CheckForOwnerAssociated();

				this.range.Worksheet.SetRangeBorders(this.Position, BorderPositions.InsideVertical, value);
			}
		}

		/// <summary>
		/// Get or set all outside border styles for range.
		/// </summary>
		public RangeBorderStyle Outside
		{
			get
			{
				CheckForOwnerAssociated();

				return this.range.Worksheet.GetRangeBorders(this.Position, BorderPositions.Outside, true).Left; // TODO: no outline available here
			}
			set
			{
				CheckForOwnerAssociated();

				this.range.Worksheet.SetRangeBorders(this.Position, BorderPositions.Outside, value);
			}
		}

		/// <summary>
		/// Get or set all inside border styles for range.
		/// </summary>
		public RangeBorderStyle All
		{
			get
			{
				CheckForOwnerAssociated();

				return this.range.Worksheet.GetRangeBorders(this.Position, BorderPositions.All, true).Left; // TODO: no outline available here
			}
			set
			{
				CheckForOwnerAssociated();

				this.range.Worksheet.SetRangeBorders(this.Position, BorderPositions.All, value);
			}
		}


	}
}
