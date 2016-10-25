/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * http://reogrid.net/
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * Author: Jing Lu <lujing at unvell.com>

 * Copyright (c) 2012-2016 Jing <lujing at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;

using unvell.ReoGrid.Formula;
using unvell.ReoGrid.Utility;

namespace unvell.ReoGrid
{
	partial class Worksheet
	{
		public void AutoFillSerial(string fromAddressOrName, string toAddressOrName)
		{
			NamedRange fromNRange, toNRange;
			RangePosition fromRange, toRange;

			#region fromRange
			if (this.TryGetNamedRange(fromAddressOrName, out fromNRange))
			{
				fromRange = fromNRange.Position;
			}
			else if (RangePosition.IsValidAddress(fromAddressOrName))
			{
				fromRange = new RangePosition(fromAddressOrName);
			}
			else
			{
				throw new InvalidAddressException(fromAddressOrName);
			}
			#endregion // fromRange

			#region toRange
			if (this.TryGetNamedRange(toAddressOrName, out toNRange))
			{
				toRange = toNRange.Position;
			}
			else if (RangePosition.IsValidAddress(toAddressOrName))
			{
				toRange = new RangePosition(toAddressOrName);
			}
			else
			{
				throw new InvalidAddressException(toAddressOrName);
			}
			#endregion // toRange

			this.AutoFillSerial(fromRange, toRange);
		}

		public void AutoFillSerial(RangePosition fromRange, RangePosition toRange)
		{
			fromRange = this.FixRange(fromRange);
			toRange = this.FixRange(toRange);

			#region Arguments Check
			if (fromRange.IntersectWith(toRange))
			{
				throw new ArgumentException("fromRange and toRange cannot being intersected.");
			}

			if (toRange != CheckMergedRange(toRange))
			{
				throw new ArgumentException("cannot change a part of merged range.");
			}
			#endregion // Arguments Check

			if (fromRange.Col == toRange.Col && fromRange.Cols == toRange.Cols)
			{
				#region Vertical Fill
				for (int c = toRange.Col; c <= toRange.EndCol; c++)
				{
					double diff = 1;

					#region Calc diff
					if (fromRange.Rows > 1)
					{
						for (int r = fromRange.Row; r < fromRange.EndRow; r++)
						{
							double val1 = 0;

							if (!this.TryGetNumberData(r, c, out val1))
							{
								break;
							}

							double val2;

							if (this.TryGetNumberData(r + 1, c, out val2))
							{
								if (r == fromRange.Row)
								{
									diff = (val2 - val1);
								}
								else
								{
									diff = (diff + (val2 - val1)) / 2;
								}
							}
						}
					}
					#endregion // Calc diff

					#region Up to Down
					for (int toRow = toRange.Row, index = 0; toRow < toRange.EndRow + 1; index++)
					{
						Cell toCell = this.cells[toRow, c];

						if (toCell != null && toCell.Rowspan < 0)
						{
							toRow++;
							continue;
						}

						CellPosition fromPos = new CellPosition(fromRange.Row + (index % fromRange.Rows), c);

						Cell fromCell = this.cells[fromPos.Row, fromPos.Col];

						if (fromCell == null || fromCell.Rowspan <= 0)
						{
							this[toRow, c] = null;
							toRow++;
							continue;
						}

						if (fromCell != null && !string.IsNullOrEmpty(fromCell.InnerFormula))
						{
							#region Fill Formula
							FormulaRefactor.Reuse(this, fromPos, new RangePosition(toRow, c, 1, 1));
							#endregion // Fill Formula
						}
						else
						{
							#region Fill Number
							double refValue = 0;

							if (CellUtility.TryGetNumberData(fromCell.Data, out refValue))
							{
								this[toRow, c] = refValue + diff * (toRow - fromPos.Row);
							}
							#endregion // Fill Number
						}

						toRow += Math.Max(fromCell.Rowspan, toCell == null ? 1 : toCell.Rowspan);
					}
					#endregion // Up to Down
				}
				#endregion // Vertical Fill
			}
			else if (fromRange.Row == toRange.Row && fromRange.Rows == toRange.Rows)
			{
				#region Horizontal Fill
				for (int r = toRange.Row; r <= toRange.EndRow; r++)
				{
					double diff = 1;

					#region Calc diff
					if (fromRange.Cols > 1)
					{
						for (int c = fromRange.Col; r < fromRange.EndCol; c++)
						{
							double val1 = 0;

							if (!this.TryGetNumberData(r, c, out val1))
							{
								break;
							}

							double val2;

							if (this.TryGetNumberData(r, c + 1, out val2))
							{
								if (c == fromRange.Col)
								{
									diff = (val2 - val1);
								}
								else
								{
									diff = (diff + (val2 - val1)) / 2;
								}
							}
						}
					}
					#endregion // Calc diff

					#region Left to Right
					for (int toCol = toRange.Col, index = 0; toCol < toRange.EndCol + 1; index++)
					{
						Cell toCell = this.cells[r, toCol];

						if (toCell != null && toCell.Colspan < 0)
						{
							toCol++;
							continue;
						}

						CellPosition fromPos = new CellPosition(r, fromRange.Col + (index % fromRange.Cols));

						Cell fromCell = this.cells[fromPos.Row, fromPos.Col];

						if (fromCell == null || fromCell.Colspan <= 0)
						{
							this[r, toCol] = null;
							toCol++;
							continue;
						}

						if (fromCell != null && !string.IsNullOrEmpty(fromCell.InnerFormula))
						{
							#region Fill Formula
							FormulaRefactor.Reuse(this, fromPos, new RangePosition(r, toCol, 1, 1));
							#endregion // Fill Formula
						}
						else
						{
							#region Fill Number
							double refValue = 0;

							if (CellUtility.TryGetNumberData(fromCell.Data, out refValue))
							{
								this[r, toCol] = refValue + diff * (toCol - fromPos.Col);
							}
							#endregion // Fill Number
						}

						toCol += Math.Max(fromCell.Colspan, toCell == null ? 1 : toCell.Colspan);
					}
					#endregion // Left to Right
				}
				#endregion // Vertical Fill
			}
			else
				throw new InvalidOperationException("The fromRange and toRange must be having same number of rows or same number of columns.");
		}
	}
}
