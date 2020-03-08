/*****************************************************************************
 * 
 * ReoGrid - Opensource .NET Spreadsheet Control
 * 
 * https://reogrid.net/
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * Thank you to all contributors!
 * 
 * (c) 2012-2020 Jingwood, unvell.com <jingwood at unvell.com>
 * 
 ****************************************************************************/

namespace unvell.ReoGrid.Actions
{
	/// <summary>
	/// Action for set styles to specified range
	/// </summary>
	public class SetRangeStyleAction : WorksheetReusableAction
	{
		private WorksheetRangeStyle style;

		/// <summary>
		/// Styles to be set
		/// </summary>
		public WorksheetRangeStyle Style { get { return style; } set { style = value; } }

		private WorksheetRangeStyle backupRootStyle = null;
		private WorksheetRangeStyle[] backupRowStyles = null;
		private WorksheetRangeStyle[] backupColStyles = null;
		private PartialGrid backupData = null;
		private bool isFullColSelected = false;
		private bool isFullRowSelected = false;
		private bool isFullGridSelected = false;

		/// <summary>
		/// Create an action to set styles into specified range
		/// </summary>
		/// <param name="row">Zero-based number of start row</param>
		/// <param name="col">Zero-based number of start column</param>
		/// <param name="rows">Number of rows in the range</param>
		/// <param name="cols">Number of columns in the range</param>
		/// <param name="style">Styles to be set</param>
		public SetRangeStyleAction(int row, int col, int rows, int cols, WorksheetRangeStyle style)
			: this(new RangePosition(row, col, rows, cols), style)
		{
		}

		/// <summary>
		/// Create an action to set styles into specified range
		/// </summary>
		/// <param name="address">Address to locate the cell or range on spreadsheet (Cannot specify named range for this method)</param>
		/// <param name="style">Styles to be set</param>
		/// <exception cref="InvalidAddressException">Throw if specified address or name is invalid</exception>
		public SetRangeStyleAction(string address, WorksheetRangeStyle style)
		{
			if (RangePosition.IsValidAddress(address))
			{
				this.Range = new RangePosition(address);
			}
			else
				throw new InvalidAddressException(address);

			this.style = style;
		}


		/// <summary>
		/// Create an action that perform set styles to specified range
		/// </summary>
		/// <param name="range">Range to be appiled this action</param>
		/// <param name="style">Style to be set to specified range</param>
		public SetRangeStyleAction(RangePosition range, WorksheetRangeStyle style)
			: base(range)
		{
			this.style = new WorksheetRangeStyle(style);
		}

		private RangePosition affectedRange;

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			backupData = Worksheet.GetPartialGrid(Range);

			affectedRange = this.Worksheet.FixRange(this.Range);

			int r1 = Range.Row;
			int c1 = Range.Col;
			int r2 = Range.EndRow;
			int c2 = Range.EndCol;

			int rowCount = Worksheet.RowCount;
			int colCount = Worksheet.ColumnCount;

			isFullColSelected = affectedRange.Rows == rowCount;
			isFullRowSelected = affectedRange.Cols == colCount;
			isFullGridSelected = isFullRowSelected && isFullColSelected;

			// update default styles
			if (isFullGridSelected)
			{
				backupRootStyle = WorksheetRangeStyle.Clone(Worksheet.RootStyle);

				this.backupRowStyles = new WorksheetRangeStyle[rowCount];
				this.backupColStyles = new WorksheetRangeStyle[colCount];

				// remote styles if it is already setted in full-row
				for (int r = 0; r < rowCount; r++)
				{
					RowHeader rowHead = Worksheet.RetrieveRowHeader(r);
					if (rowHead != null && rowHead.InnerStyle != null)
					{
						this.backupRowStyles[r] = WorksheetRangeStyle.Clone(rowHead.InnerStyle);
					}
				}

				// remote styles if it is already setted in full-col
				for (int c = 0; c < colCount; c++)
				{
					ColumnHeader colHead = Worksheet.RetrieveColumnHeader(c);
					if (colHead != null && colHead.InnerStyle != null)
					{
						this.backupColStyles[c] = WorksheetRangeStyle.Clone(colHead.InnerStyle);
					}
				}
			}
			else if (isFullRowSelected)
			{
				backupRowStyles = new WorksheetRangeStyle[r2 - r1 + 1];
				for (int r = r1; r <= r2; r++)
				{
					backupRowStyles[r - r1] = WorksheetRangeStyle.Clone(Worksheet.RetrieveRowHeader(r).InnerStyle);
				}
			}
			else if (isFullColSelected)
			{
				backupColStyles = new WorksheetRangeStyle[c2 - c1 + 1];
				for (int c = c1; c <= c2; c++)
				{
					backupColStyles[c - c1] = WorksheetRangeStyle.Clone(Worksheet.RetrieveColumnHeader(c).InnerStyle);
				}
			}

			Worksheet.SetRangeStyles(affectedRange, style);
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			if (this.isFullGridSelected)
			{
				Worksheet.RootStyle = WorksheetRangeStyle.Clone(backupRootStyle);

				// remote styles if it is already setted in full-row
				for (int r = 0; r < backupRowStyles.Length; r++)
				{
					if (backupRowStyles[r] != null)
					{
						RowHeader rowHead = Worksheet.RetrieveRowHeader(r);
						if (rowHead != null)
						{
							rowHead.InnerStyle = WorksheetRangeStyle.Clone(backupRowStyles[r]);
							//rowHead.InnerStyle.Flag = PlainStyleFlag.None;
							//rowHead.Style.BackColor = System.Drawing.Color.Empty;
						}
					}
				}

				// remote styles if it is already setted in full-col
				for (int c = 0; c < backupColStyles.Length; c++)
				{
					if (backupColStyles[c] != null)
					{
						ColumnHeader colHead = Worksheet.RetrieveColumnHeader(c);
						if (colHead != null)
						{
							colHead.InnerStyle = WorksheetRangeStyle.Clone(backupColStyles[c]);
							//colHead.InnerStyle.Flag = PlainStyleFlag.None;
							//colHead.Style.BackColor = System.Drawing.Color.Empty;
						}
					}
				}
			}
			else if (isFullRowSelected)
			{
				for (int r = 0; r < backupRowStyles.Length; r++)
				{
					RowHeader rowHead = Worksheet.RetrieveRowHeader(r + affectedRange.Row);
					if (rowHead != null) rowHead.InnerStyle = backupRowStyles[r];
				}
			}
			else if (isFullColSelected)
			{
				for (int c = 0; c < backupColStyles.Length; c++)
				{
					ColumnHeader colHead = Worksheet.RetrieveColumnHeader(c + affectedRange.Col);
					if (colHead != null) colHead.InnerStyle = backupColStyles[c];
				}
			}

			Worksheet.SetPartialGrid(affectedRange, backupData, PartialGridCopyFlag.CellStyle);
		}

		/// <summary>
		/// Returns friendly name for this action.
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Set Style";
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new SetRangeStyleAction(range, style);
		}
	}

}
