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

#if PRINT

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#if WINFORM
using RGFloat = System.Single;

#elif WPF
using RGFloat = System.Double;

#endif // WPF

using unvell.ReoGrid.Print;
using unvell.ReoGrid.Data;
using unvell.ReoGrid.Utility;
using unvell.ReoGrid.Graphics;

namespace unvell.ReoGrid.Print
{
	#region PrintPageOrder
	/// <summary>
	/// Paging order decide print each pages in what order
	/// </summary>
	public enum PrintPageOrder
	{
		/// <summary>
		/// First down, then over (Vertical Paging) (Default)
		/// </summary>
		DownThenOver,

		/// <summary>
		/// First over, then down (Horizontal Paging)
		/// </summary>
		OverThenDown,
	}
	#endregion // PrintPageOrder
}

namespace unvell.ReoGrid
{
	public partial class Worksheet
	{
		#region Settings and Properties
		//private PageSettings pageSettings;

		///// <summary>
		///// Get or set the page settings
		///// </summary>
		//public PageSettings PageSettings
		//{
		//	get
		//	{
		//		if (this.pageSettings == null)
		//		{
		//			this.pageSettings = new PageSettings();
		//		}
		//		return this.pageSettings;
		//	}
		//	set
		//	{
		//		if (value == null)
		//		{
		//			throw new ArgumentNullException("Cannot set page settings to null");
		//		}

		//		if (this.pageSettings == null
		//			|| this.pageSettings.PaperSize != value.PaperSize
		//			|| this.pageSettings.Landscape != value.Landscape)
		//		{
		//			this.PrintSettings.PageScaling = 1f;

		//			if (this.MaxContentRow > 0 && this.MaxContentCol == 0)
		//			{
		//				this.AutoSplitPage();
		//			}
		//		}

		//		this.pageSettings = value;
		//	}
		//}

		private PrintSettings printSettings;

		/// <summary>
		/// Ger or set ReoGrid spreadsheet print settings.
		/// </summary>
		public PrintSettings PrintSettings
		{
			get
			{
				if (this.printSettings == null)
				{
					this.printSettings = new PrintSettings();
				}

				return this.printSettings;
			}
			set
			{
				this.printSettings = value;

				// TODO: need refresh page breaks?
				//
				if (!this.printableRange.IsEmpty)
				{
					this.AutoSplitPage();
				}
			}
		}

		private RangePosition printableRange = RangePosition.Empty;

		/// <summary>
		/// Get or set printable range
		/// </summary>
		public RangePosition PrintableRange
		{
			get { return this.printableRange; }
			set
			{
				value = FixRange(value);

				// if new printable region is empty, clear user break indexes
				if (value.IsEmpty)
				{
					this.userPageBreakCols.Clear();
					this.userPageBreakRows.Clear();
				}
				else if (this.printableRange != value)
				{
					this.printableRange = value;

					// remove indexes which is out of the range (left and right)
					if (this.userPageBreakCols != null)
					{
						for (int i = 0; i < this.userPageBreakCols.Count; i++)
						{
							if (this.userPageBreakCols[i] < this.printableRange.Col)
							{
								this.userPageBreakCols.RemoveAt(i);
							}
							else if (this.userPageBreakCols[i] > this.printableRange.EndCol + 1)
							{
								this.userPageBreakCols.RemoveAt(i);
							}
						}
					}

					// remove indexes which is out of the range (top and bottom)
					if (this.userPageBreakRows != null)
					{
						for (int i = 0; i < this.userPageBreakRows.Count; i++)
						{
							if (this.userPageBreakRows[i] < this.printableRange.Row)
							{
								this.userPageBreakRows.RemoveAt(i);
							}
							else if (this.userPageBreakRows[i] > this.printableRange.EndRow + 1)
							{
								this.userPageBreakRows.RemoveAt(i);
							}
						}
					}

					this.RaisePrintableRangeChangedEvent();
				}
			}
		}
		#endregion // Settings and Properties

		#region Paging

		internal List<int> pageBreakRows = null, pageBreakCols = null;
		internal List<int> userPageBreakRows = null, userPageBreakCols = null;

		#region Collection Provider

		private RowPageBreakIndexCollection pageBreakRowCollection;

		/// <summary>
		/// Get collection of row page-break indexes
		/// </summary>
		public RowPageBreakIndexCollection RowPageBreaks
		{
			get
			{
				if (this.pageBreakRowCollection == null)
				{
					this.pageBreakRowCollection = new RowPageBreakIndexCollection(this);
				}

				return this.pageBreakRowCollection;
			}
		}

		/// <summary>
		/// Collection of row page-break indexes
		/// </summary>
		public class RowPageBreakIndexCollection : ICollection<int>
		{
			private Worksheet Worksheet { get; set; }

			internal RowPageBreakIndexCollection(Worksheet sheet)
			{
				this.Worksheet = sheet;
			}

			private List<int> ValidPageBreakArray {
				get
				{
					return (this.Worksheet.pageBreakRows != null && this.Worksheet.pageBreakRows.Count > 1)
						? this.Worksheet.pageBreakRows : this.Worksheet.userPageBreakRows;
				}
			}

			private IEnumerator<int> GetEnum()
			{
				if (this.Worksheet == null)
				{
					throw new ReferenceObjectNotAssociatedException("Collection of row page breaks must be created from valid instance of ReoGrid control.");
				}

				return ValidPageBreakArray.GetEnumerator();
			}

			/// <summary>
			/// Get enumerator of row page break indexes
			/// </summary>
			/// <returns></returns>
			public IEnumerator<int> GetEnumerator()
			{
				return this.GetEnum();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this.GetEnum();
			}

			/// <summary>
			/// Insert a new index
			/// </summary>
			/// <param name="rowIndex">row index to be inserted</param>
			public void Add(int rowIndex)
			{
				if (this.Worksheet == null)
				{
					throw new ReferenceObjectNotAssociatedException("Collection of row page break must be created from valid grid control.");
				}

				this.Worksheet.InsertRowPageBreak(rowIndex);
			}

			/// <summary>
			/// Clear all row break indexes
			/// </summary>
			public void Clear()
			{
				if (this.Worksheet == null)
				{
					throw new ReferenceObjectNotAssociatedException("Collection of row page break must be created from valid grid control.");
				}

				this.Worksheet.ClearRowPageBreaks();
			}

			/// <summary>
			/// Check whether or not the specified index is contained in current collection
			/// </summary>
			/// <param name="index">index to be checked</param>
			/// <returns></returns>
			public bool Contains(int index)
			{
				return ValidPageBreakArray == null ? false : ValidPageBreakArray.Contains(index);
			}

			/// <summary>
			/// Copy elements to another array
			/// </summary>
			/// <param name="array">Array to put elements</param>
			/// <param name="arrayIndex">Start index to copy</param>
			public void CopyTo(int[] array, int arrayIndex)
			{
				ValidPageBreakArray.CopyTo(array, arrayIndex);
			}

			/// <summary>
			/// Get the number of elements
			/// </summary>
			public int Count
			{
				get { return ValidPageBreakArray.Count; }
			}

			/// <summary>
			/// Check whether or not this collection is read-only
			/// </summary>
			public bool IsReadOnly
			{
				get { return false; }
			}

			/// <summary>
			/// Remove element by specified index
			/// </summary>
			/// <param name="rowIndex">Number of element to be removed</param>
			/// <returns>True if element was found and removed successfully</returns>
			public bool Remove(int rowIndex)
			{
				if (this.Worksheet == null)
				{
					throw new ReferenceObjectNotAssociatedException("Collection of row page break must be created from valid grid control.");
				}

				try
				{
					this.Worksheet.RemoveRowPageBreak(rowIndex);
					return true;
				}
				catch
				{
					return false;
				}
			}

			/// <summary>
			/// Get or set page-break by zero-based index
			/// </summary>
			/// <param name="index">Index to get page-break index</param>
			/// <returns></returns>
			public int this[int index]
			{
				get
				{
					return ValidPageBreakArray[index];
				}
				set
				{
					this.Worksheet.ChangeRowPageBreak(index, value);
				}
			}
		}

		private ColumnPageBreakIndexCollection pageBreakColumnCollection;

		/// <summary>
		/// Get collection of column page break indexes
		/// </summary>
		public ColumnPageBreakIndexCollection ColumnPageBreaks
		{
			get
			{
				if (this.pageBreakColumnCollection == null)
				{
					this.pageBreakColumnCollection = new ColumnPageBreakIndexCollection(this);
				}

				return this.pageBreakColumnCollection;
			}
		}

		/// <summary>
		/// Collection of column page break indexes
		/// </summary>
		public class ColumnPageBreakIndexCollection : ICollection<int>
		{
			private Worksheet Worksheet { get; set; }

			internal ColumnPageBreakIndexCollection(Worksheet sheet)
			{
				this.Worksheet = sheet;
			}

			private List<int> ValidPageBreakArray
			{
				get
				{
					return (this.Worksheet.pageBreakCols != null && this.Worksheet.pageBreakCols.Count > 1)
						? this.Worksheet.pageBreakCols : this.Worksheet.userPageBreakCols;
				}
			}

			private IEnumerator<int> GetEnum()
			{
				if (this.Worksheet == null)
				{
					throw new ReferenceObjectNotAssociatedException("Collection of column page breaks must be created from valid instance of ReoGrid control.");
				}

				this.Worksheet.CheckAndInitPrintableRegion();

				return ValidPageBreakArray.GetEnumerator();
			}

			/// <summary>
			/// Get the enumerator of current collection
			/// </summary>
			/// <returns>enumerator of current collection</returns>
			public IEnumerator<int> GetEnumerator()
			{
				return this.GetEnum();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this.GetEnum();
			}

			/// <summary>
			/// Add a column page break
			/// </summary>
			/// <param name="columnIndex">number of column to be added as page break</param>
			public void Add(int columnIndex)
			{
				if (this.Worksheet == null)
				{
					throw new ReferenceObjectNotAssociatedException("Collection of column page breaks must be created from valid instance of ReoGrid control.");
				}

				this.Worksheet.InsertColumnPageBreak(columnIndex);
			}

			/// <summary>
			/// Clear all column page breaks
			/// </summary>
			public void Clear()
			{
				if (this.Worksheet == null)
				{
					throw new ReferenceObjectNotAssociatedException("Collection of column page breaks must be created from valid instance of ReoGrid control.");
				}

				this.Worksheet.ClearColumnPageBreaks();
			}

			/// <summary>
			/// Check whether or not a column page break has already added
			/// </summary>
			/// <param name="index">number of column index to be checked</param>
			/// <returns>true if the page break has been added, otherwise return false</returns>
			public bool Contains(int index)
			{
				return ValidPageBreakArray == null ? false : ValidPageBreakArray.Contains(index);
			}

			/// <summary>
			/// Copy all column page breaks into another array
			/// </summary>
			/// <param name="array">array to be save the page breaks</param>
			/// <param name="arrayIndex">number of index to start copy the array</param>
			public void CopyTo(int[] array, int arrayIndex)
			{
				ValidPageBreakArray.CopyTo(array, arrayIndex);
			}

			/// <summary>
			/// Get the column page break count
			/// </summary>
			public int Count
			{
				get { return ValidPageBreakArray.Count; }
			}

			/// <summary>
			/// Check whether or not current column page break is read-only
			/// </summary>
			public bool IsReadOnly
			{
				get { return false; }
			}

			/// <summary>
			/// Remove a column page break from spreadsheet
			/// </summary>
			/// <param name="columnIndex">number of column index to be removed</param>
			/// <returns></returns>
			public bool Remove(int columnIndex)
			{
				if (this.Worksheet == null)
				{
					throw new ReferenceObjectNotAssociatedException("Collection of column page breaks must be created from valid instance of ReoGrid control.");
				}

				try
				{
					this.Worksheet.RemoveColumnPageBreak(columnIndex);
					return true;
				}
				catch
				{
					return false;
				}
			}

			/// <summary>
			/// Get or set the column page break
			/// </summary>
			/// <param name="index">number of column index to be added or removed</param>
			/// <returns>number of column index returned from current spreadsheet</returns>
			public int this[int index]
			{
				get
				{
					return ValidPageBreakArray[index];
				}
				set
				{
					if (ValidPageBreakArray.Count == 0)
						throw new PageBreakNotFoundException(index);

					this.Worksheet.ChangeColumnPageBreak(index, value);
				}
			}
		}

		#endregion // Collection Provider

		/// <summary>
		/// Event raised when printable range is changed
		/// </summary>
		public event EventHandler PrintableRangeChanged;

		/// <summary>
		/// Reset all page breaks
		/// </summary>
		public void ResetAllPageBreaks()
		{
			// clear printable range
			this.printableRange = RangePosition.Empty;

			// remove all user specified page breaks
			ClearUserPageBreaks();

			// perform auot-split to create new page breaks
			AutoSplitPage();
		}

		/// <summary>
		/// Split spreadsheet into multiple pages automatically according to given paper size.
		/// </summary>
		/// <remarks>
		/// This method itself works very fast, but will be slower if a remote printer is specified 
		/// as target printer. Fetching paper size from a remote printer will spend more than 100ms. 
		/// (depending on the network environment)
		/// </remarks>
		public void AutoSplitPage()
		{
			if (this.MaxContentRow < 0 || this.MaxContentCol < 0)
			{
				throw new NoPrintableContentException("No printable content found on this spreadsheet.");
			}

#if DEBUG
			Stopwatch sw = Stopwatch.StartNew();
#endif

			if (this.printSettings == null)
			{
				this.printSettings = new PrintSettings();
			}

			if (this.pageBreakRows == null)
			{
				this.pageBreakRows = new List<int>();
			}
			else
			{
				this.pageBreakRows.Clear();
			}

			if (this.pageBreakCols == null)
			{
				this.pageBreakCols = new List<int>();
			}
			else
			{
				this.pageBreakCols.Clear();
			}

			CheckAndInitPrintableRegion();

			Rectangle printContentBounds = this.GetRangePhysicsBounds(this.printableRange);

			this.pageBreakRows.Add(this.printableRange.Row);
			this.pageBreakCols.Add(this.printableRange.Col);

			int maxRow = printableRange.EndRow + 1;
			int maxCol = printableRange.EndCol + 1;

			RGFloat scaling = this.printSettings == null ? 1f : (this.printSettings.PageScaling);
			Rectangle paperBounds = this.GetPaperPrintBounds();
			RGFloat paperWidth = paperBounds.Width / scaling;
			RGFloat paperHeight = paperBounds.Height / scaling;

			double x = printContentBounds.Left + paperWidth, y = printContentBounds.Top + paperHeight;

			int row = this.PrintableRange.Row, col = this.PrintableRange.Col;

			int splitRow = 0, splitCol = 0;

			while (y < printContentBounds.Bottom || row < maxRow)
			{
				splitRow = ArrayHelper.QuickFind(row + 1, maxRow, i =>
				{
					var rowHeader = this.rows[i];

					if (rowHeader.Bottom < y)
						return 1;
					else if (rowHeader.Top > y)
						return -1;
					else
						return 0;
				});

				if (this.userPageBreakRows != null && this.userPageBreakRows.Count > 0)
				{
					int nextUserRow = this.userPageBreakRows.FirstOrDefault(ur => ur > row);
					if (nextUserRow != 0 && nextUserRow < splitRow)
					{
						splitRow = nextUserRow;
					}
				}

				if (splitRow >= maxRow)
				{
					pageBreakRows.Add(splitRow);
					break;
				}

				pageBreakRows.Add(splitRow);
				row = splitRow;
				y = this.rows[splitRow].Top + paperHeight;
			}

			while (x < printContentBounds.Right || col < maxCol)
			{
				splitCol = ArrayHelper.QuickFind(col + 1, maxCol, i =>
				{
					var colHeader = this.cols[i];

					if (colHeader.Right < x)
						return 1;
					else if (colHeader.Left > x)
						return -1;
					else
						return 0;
				});

				if (this.userPageBreakCols != null && this.userPageBreakCols.Count > 0)
				{
					int nextUserCol = this.userPageBreakCols.FirstOrDefault(uc => uc > col);
					if (nextUserCol != 0 && nextUserCol < splitCol)
					{
						splitCol = nextUserCol;
					}
				}

				if (splitCol >= maxCol)
				{
					pageBreakCols.Add(splitCol);
					break;
				}

				pageBreakCols.Add(splitCol);
				col = splitCol;

				x = this.cols[splitCol].Left + paperWidth;
			}

			this.RequestInvalidate();

#if DEBUG
			sw.Stop();
			long ms = sw.ElapsedMilliseconds;

			this._Debug_Validate_PrintPageBreaks();

			if (ms > 10)
			{
				Debug.WriteLine("Auto split page takes " + ms + " ms.");
			}
#endif
		}

		internal void CheckAndInitPrintableRegion()
		{
			bool updateUserCols = false, updateUserRows = false;

			if (this.printableRange.IsEmpty)
			{
				//maxRow = Math.Min(this.rows.Count, Math.Max(cells.MaxRow, Math.Max(hBorders.MaxRow, vBorders.MaxRow)));
				//maxCol = Math.Min(this.cols.Count, Math.Max(cells.MaxCol, Math.Max(hBorders.MaxCol, vBorders.MaxCol)));

				updateUserCols = true;
				updateUserRows = true;

				this.PrintableRange = new RangePosition(0, 0, this.MaxContentRow + 1, this.MaxContentCol + 1);
			}
			else
			{
				if (this.userPageBreakCols == null || this.userPageBreakCols.Count < 2)
				{
					updateUserCols = true;
				}

				if (this.userPageBreakRows == null || this.userPageBreakRows.Count < 2)
				{
					updateUserRows = true;
				}
			}

			if (updateUserCols)
			{
				if (this.userPageBreakCols == null)
					this.userPageBreakCols = new List<int>();
				else
					this.userPageBreakCols.Clear();

				this.userPageBreakCols.Add(this.printableRange.Col);
				this.userPageBreakCols.Add(this.printableRange.EndCol + 1);
			}

			if (updateUserRows)
			{
				if (this.userPageBreakRows == null)
					this.userPageBreakRows = new List<int>();
				else
					this.userPageBreakRows.Clear();

				this.userPageBreakRows.Add(this.printableRange.Row);
				this.userPageBreakRows.Add(this.printableRange.EndRow + 1);
			}
		}

		internal void ClearUserPageBreaks()
		{
			if (this.userPageBreakRows == null)
				this.userPageBreakRows = new List<int>();
			else
				this.userPageBreakRows.Clear();

			if (this.userPageBreakCols == null)
				this.userPageBreakCols = new List<int>();
			else
				this.userPageBreakCols.Clear();
		}

		#region Find break index by position

		internal int FindBreakIndexOfRowByPixel(Point p)
		{
#if DEBUG
			Debug.Assert(pageBreakCols != null);
#endif

			// return 'not found' when page breaks were not initialized
			if (pageBreakRows == null || pageBreakRows.Count <= 0
				|| pageBreakCols == null || pageBreakCols.Count <= 0)
				return -1;

			// check left
			float left = this.cols[this.pageBreakCols[0]].Left;
			if (p.X < left) return -1;

			// check right
			int rightIndex = this.pageBreakCols[this.pageBreakCols.Count - 1];
			if (rightIndex >= this.cols.Count) rightIndex = this.cols.Count;
			float right = rightIndex < this.cols.Count ? this.cols[rightIndex].Left : this.cols[rightIndex - 1].Right;
			if (p.X > right) return -1;

			for (int i = 0; i < this.pageBreakRows.Count; i++)
			{
				if (IsSplitRowHoverByMouse(i, p.Y)) return i;
			}

			return -1;
		}

		internal int FindBreakIndexOfColumnByPixel(Point p)
		{
#if DEBUG
			Debug.Assert(pageBreakRows != null);
			Debug.Assert(pageBreakCols != null);
#endif

			// return 'not found' when page breaks were not initialized
			if (pageBreakRows == null || pageBreakRows.Count <= 0
				|| pageBreakCols == null || pageBreakCols.Count <= 0)
				return -1;

			// check top
			float top = this.rows[this.pageBreakRows[0]].Top;
			if (p.Y < top) return -1;

			// check bottom
			int bottomIndex = this.pageBreakRows[this.pageBreakRows.Count - 1];
			if (bottomIndex >= this.rows.Count) bottomIndex = this.rows.Count - 1;
			float bottom = bottomIndex < this.rows.Count ? this.rows[bottomIndex].Bottom : this.rows[bottomIndex - 1].Bottom;
			if (p.Y > bottom) return -1;

			for (int i = 0; i < this.pageBreakCols.Count; i++)
			{
				if (IsSplitColHoverByMouse(i, p.X)) return i;
			}

			return -1;
		}

		internal bool IsSplitRowHoverByMouse(int breakIndex, RGFloat y)
		{
			int row = this.pageBreakRows[breakIndex];

			int split = 0;
			if (row < this.rows.Count)
			{
				var header = this.rows[row];
				split = header.Top;
			}
			else
			{
				var header = this.rows[row - 1];
				split = header.Bottom;
			}

			RGFloat scaledThumb = 2f / this.renderScaleFactor;
			return split - scaledThumb <= y && split + scaledThumb > y;
		}

		internal bool IsSplitColHoverByMouse(int breakIndex, RGFloat x)
		{
			int col = this.pageBreakCols[breakIndex];

			int split = 0;
			if (col < this.cols.Count)
			{
				var header = this.cols[col];
				split = header.Left;
			}
			else
			{
				var header = this.cols[col - 1];
				split = header.Right;
			}

			RGFloat scaledThumb = 2f / this.renderScaleFactor;
			return split - scaledThumb <= x && split + scaledThumb > x;
		}

		#endregion // Find break indexes by position

		/// <summary>
		/// Adjust the page scale to fit every page print range automatically
		/// in order to make page cells can be printed out properly.
		/// </summary>
		public void AutoSetMaximumScaleForPages()
		{
			if (this.printSettings == null)
			{
				this.printSettings = new PrintSettings();
			}

			Rectangle paperBounds = this.GetPaperPrintBounds();

			RGFloat minScale = 1f;

			IteratePrintPages(this.printSettings.PageOrder, range =>
			{
				Size sizef = GetRangePhysicsBounds(range).Size;

				RGFloat scaleX = ((float)paperBounds.Width / sizef.Width);
				RGFloat scaleY = ((float)paperBounds.Height / sizef.Height);

				if (minScale > scaleX) minScale = scaleX;
				if (minScale > scaleY) minScale = scaleY;

				return true;
			});

			this.printSettings.PageScaling = minScale;

#if DEBUG
			this._Debug_Validate_PrintPageBreaks();

			Debug.WriteLine("Print scale set to " + this.printSettings.PageScaling + "f.");
#endif
		}
	
		#endregion // Paging

		#region Print

		/// <summary>
		/// Create print session to print this worksheet.
		/// </summary>
		/// <returns>Print session created from this worksheet.</returns>
		/// <exception cref="NoPrintableContentException">Exception will be thrown if nothing found to be printed.</exception>
		public PrintSession CreatePrintSession()
		{
			if (this.pageBreakRows == null || this.pageBreakCols == null
				|| this.pageBreakRows.Count == 0 || this.pageBreakCols.Count == 0)
			{
				AutoSplitPage();
			}

			if (this.pageBreakRows == null || this.pageBreakRows == null
				|| this.pageBreakRows.Count == 0 || this.pageBreakRows.Count == 0)
			{
				throw new NoPrintableContentException();
			}

			if (this.printSettings == null)
			{
				this.printSettings = new PrintSettings();
			}

			PrintSession session = new PrintSession();

			session.worksheets.Add(this);

			session.Init();

			return session;
		}

		internal Rectangle GetPaperPrintBounds()
		{
			RGFloat dpix = 100f, dpiy = 100f;

			Size size = GetPaperSizeInPixel(dpix, dpiy);

			//RGFloat dpi = unvell.ReoGrid.Rendering.PlatformUtility.GetDPI();

			var marginLeft = MeasureToolkit.InchToPixel(this.printSettings.Margins.Left, dpix);
			var marginRight = MeasureToolkit.InchToPixel(this.printSettings.Margins.Right, dpix);
			var marginTop = MeasureToolkit.InchToPixel(this.printSettings.Margins.Top, dpiy);
			var marginBottom = MeasureToolkit.InchToPixel(this.printSettings.Margins.Bottom, dpiy);

			return new Rectangle(
				(int)Math.Floor(marginLeft), (int)Math.Floor(marginTop),
					(int)Math.Ceiling(size.Width - marginLeft - marginRight),
					(int)Math.Ceiling(size.Height - marginTop - marginBottom));
		}

		internal Size GetPaperSizeInPixel(RGFloat dpix, RGFloat dpiy)
		{
			if (this.printSettings == null)
			{
				this.printSettings = new PrintSettings();
			}

			RGFloat paperWidthInch = this.printSettings.PaperWidth;
			RGFloat paperHeightInch = this.printSettings.PaperHeight;

			int width = (int)Math.Ceiling(MeasureToolkit.InchToPixel(paperWidthInch, dpix));
			int height = (int)Math.Ceiling(MeasureToolkit.InchToPixel(paperHeightInch, dpiy));

			return this.printSettings.Landscape ? new Size(height, width) : new Size(width, height);
		}

		internal System.Drawing.Printing.PageSettings CreateGDIPageSettings()
		{
			return new System.Drawing.Printing.PageSettings()
			{
				PaperSize = new System.Drawing.Printing.PaperSize(
					this.printSettings.PaperName,
					(int)Math.Round(this.printSettings.PaperWidth * 100f), 
					(int)Math.Round(this.printSettings.PaperHeight * 100f)),

				Landscape = this.printSettings.Landscape,
				Margins = this.printSettings.Margins,
			};
		}
		#endregion // Page

		#region Interate Pages
		/// <summary>
		/// Iterate over all page break ranges
		/// </summary>
		/// <param name="iterator">delegate function to iterate over each pages</param>
		public void IteratePrintPages(Func<RangePosition, bool> iterator)
		{
			IteratePrintPages(PrintPageOrder.DownThenOver, iterator);
		}

		/// <summary>
		/// Iterate over all page break ranges
		/// </summary>
		/// <param name="pageOrder">order for iterating pages (down then over, over then down)</param>
		/// <param name="iterator">delegate function to iterate over each pages</param>
		public void IteratePrintPages(PrintPageOrder pageOrder, Func<RangePosition, bool> iterator)
		{
			int rowIndex = 0, colIndex = 0;

			while (true)
			{
				int startRow = this.pageBreakRows[rowIndex];
				int startCol = this.pageBreakCols[colIndex];
				int endRow = this.pageBreakRows[rowIndex + 1] - 1;
				int endCol = this.pageBreakCols[colIndex + 1] - 1;

				if (!iterator(new RangePosition(startRow, startCol, endRow - startRow + 1, endCol - startCol + 1)))
					break;

				if (pageOrder == PrintPageOrder.DownThenOver)
				{
					if (rowIndex < this.pageBreakRows.Count - 2)
					{
						rowIndex++;
					}
					else
					{
						if (colIndex < this.pageBreakCols.Count - 2)
						{
							rowIndex = 0;
							colIndex++;
						}
						else
						{
							break;
						}
					}
				}
				else if (pageOrder == PrintPageOrder.OverThenDown)
				{
					if (colIndex < this.pageBreakCols.Count - 2)
					{
						colIndex++;
					}
					else
					{
						if (rowIndex < this.pageBreakRows.Count - 2)
						{
							colIndex = 0;
							rowIndex++;
						}
						else
						{
							break;
						}
					}
				}
				else
					break;
			}
		}

		#endregion // Interate Pages

		#region Page Break Management API
		/// <summary>
		/// Change (Move) a column page break index from specified position to a new position.
		/// </summary>
		/// <param name="oldIndex">Zero-based number of column to find the page break that will be moved.</param>
		/// <param name="newIndex">Zero-based number of column to put column page break.</param>
		/// <param name="refreshPageBreaks">Indicates whether or not to resplit all pages after changing. (Default is true)</param>
		public void ChangeColumnPageBreak(int oldIndex, int newIndex, bool refreshPageBreaks = true)
		{
			if (this.pageBreakCols == null || this.pageBreakCols.Count < 2)
			{
				throw new InvalidOperationException("No page breaks exist. Try call AutoSplitPage method firstly.");
			}

			if (this.pageBreakCols.IndexOf(oldIndex) < 0
				&& (this.userPageBreakCols == null || this.userPageBreakCols.IndexOf(oldIndex) < 0))
			{
				throw new PageBreakNotFoundException(oldIndex);
			}

			if (newIndex < 0 || newIndex > this.cols.Count)
			{
				throw new IndexOutOfRangeException("Specified index out of the available range on spreadsheet");
			}

			if ((oldIndex != this.printableRange.Col && newIndex < this.printableRange.Col)
				|| (oldIndex != this.printableRange.EndCol + 1 && newIndex > this.printableRange.EndCol + 1))
			{
				throw new ArgumentException("Cannot move to the new position that is out of the printable range. Change PrintableRange property firstly.", "newIndex");
			}

			// for adjust the maximum scaling
			this.pageBreakCols.RemoveAll(bc => bc >= oldIndex && bc <= newIndex);
			this.pageBreakCols.Add(newIndex);
			this.pageBreakCols.Sort();

			bool changeLeftBound = oldIndex == this.userPageBreakCols.First();
			bool changeRightBound = oldIndex == this.userPageBreakRows.Last();

			this.userPageBreakCols.RemoveAll(uc => uc == oldIndex || uc == newIndex);

			this.userPageBreakCols.Add(newIndex);
			this.userPageBreakCols.Sort();

			if (this.userPageBreakCols.Last() == newIndex)
			{
				this.printableRange.EndCol = newIndex - 1;
			}
			else if (this.userPageBreakCols.First() == newIndex)
			{
				this.printableRange.Cols -= newIndex - this.printableRange.Col;
				this.printableRange.Col = newIndex;

				Debug.Assert(this.printableRange.Col >= 0);
				Debug.Assert(this.printableRange.EndCol <= this.cols.Count - 1);
			}

			if (!changeLeftBound && !changeRightBound)
			{
				AutoSetMaximumScaleForPages();
			}

			if (refreshPageBreaks) AutoSplitPage();
		}

		/// <summary>
		/// Move a specified row page break index from specified position to a new position.
		/// </summary>
		/// <param name="oldIndex">Zero-based number of row to find the page break that will be moved.</param>
		/// <param name="newIndex">Zero-based number of row to put row page break.</param>
		/// <param name="refreshPageBreaks">Indicates whether or not to resplit all pages after changing. (Default is true)</param>
		public void ChangeRowPageBreak(int oldIndex, int newIndex, bool refreshPageBreaks = true)
		{
			if (this.pageBreakRows == null || this.pageBreakRows.Count < 2)
			{
				throw new InvalidOperationException("No page breaks exist. Try call AutoSplitPage method firstly.");
			}

			if (this.pageBreakRows.IndexOf(oldIndex) < 0
				&& (this.userPageBreakRows == null || this.userPageBreakRows.IndexOf(oldIndex) < 0))
			{
				throw new PageBreakNotFoundException(oldIndex);
			}

			if (newIndex < 0 || newIndex > this.rows.Count)
			{
				throw new IndexOutOfRangeException("Specified index out of the available range on worksheet");
			}

			if ((oldIndex != this.printableRange.Row && newIndex < this.printableRange.Row)
				|| (oldIndex != this.printableRange.EndRow + 1 && newIndex > this.printableRange.EndRow + 1))
			{
				throw new ArgumentException("Cannot move to the new position that is out of the printable range. Change PrintableRange property firstly.", "newIndex");
			}

			// for adjust the maximum scaling
			this.pageBreakRows.RemoveAll(br => br >= oldIndex && br <= newIndex);
			this.pageBreakRows.Add(newIndex);
			this.pageBreakRows.Sort();

			bool changeTopBound = oldIndex == this.userPageBreakCols.First();
			bool changeBottomBound = oldIndex == this.userPageBreakRows.Last();

			this.userPageBreakRows.RemoveAll(uc => uc == oldIndex || uc == newIndex);

			this.userPageBreakRows.Add(newIndex);
			this.userPageBreakRows.Sort();

			if (this.userPageBreakRows.Last() == newIndex)
			{
				this.printableRange.EndRow = newIndex - 1;
			}
			else if (this.userPageBreakRows.First() == newIndex)
			{
				this.printableRange.Rows -= newIndex - this.printableRange.Row;
				this.printableRange.Row = newIndex;

#if DEBUG
				Debug.Assert(this.printableRange.Row >= 0);
				Debug.Assert(this.printableRange.Rows <= this.rows.Count - 1);
#endif
			}

			if (!changeTopBound && !changeBottomBound)
			{
				AutoSetMaximumScaleForPages();
			}

			if (refreshPageBreaks) AutoSplitPage();
		}

		/// <summary>
		/// Insert row page break before specified column
		/// </summary>
		/// <param name="columnIndex">zero-based number of row to insert page break</param>
		/// <param name="refreshPageBreaks">indicates that whether allow to update others page 
		/// break and adjust the page scale automatically. (default is true)</param>
		public void InsertColumnPageBreak(int columnIndex, bool refreshPageBreaks = true)
		{
			if (this.userPageBreakCols == null || this.userPageBreakCols.Count < 2)
			{
				CheckAndInitPrintableRegion();
			}

			if (!this.userPageBreakCols.Contains(columnIndex))
			{
				this.userPageBreakCols.Add(columnIndex);
				this.userPageBreakCols.Sort();

				// if inserted page-break index outside current printable range,
				// expend the printable range
				if (this.printableRange.EndCol < columnIndex - 1)
				{
					this.printableRange.EndCol = columnIndex - 1;
				}

				if (refreshPageBreaks) this.AutoSplitPage();
			}
		}

		/// <summary>
		/// Insert row page break before specified row
		/// </summary>
		/// <param name="rowIndex">zero-based number of row to insert page break</param>
		/// <param name="refreshPageBreaks">indicates that whether allow to update others page 
		/// break and adjust the page scale automatically. (default is true)</param>
		public void InsertRowPageBreak(int rowIndex, bool refreshPageBreaks = true)
		{
			if (this.userPageBreakRows == null || this.userPageBreakRows.Count < 2)
			{
				CheckAndInitPrintableRegion();
			}

			if (!this.userPageBreakRows.Contains(rowIndex))
			{
				this.userPageBreakRows.Add(rowIndex);
				this.userPageBreakRows.Sort();

				// if inserted page-break index outside current printable range,
				// expend the printable range
				if (this.printableRange.EndRow < rowIndex - 1)
				{
					this.printableRange.EndRow = rowIndex - 1;
				}

				if (refreshPageBreaks) this.AutoSplitPage();
			}
		}

		/// <summary>
		/// Remove column page break from specified number of column
		/// </summary>
		/// <param name="columnIndex">zero-based number of column to remove specified page break</param>
		public void RemoveColumnPageBreak(int columnIndex)
		{
			if ((this.pageBreakCols == null || !this.pageBreakCols.Contains(columnIndex))
				&& (this.userPageBreakCols == null || !this.userPageBreakCols.Contains(columnIndex)))
			{
				throw new PageBreakNotFoundException(columnIndex);
			}

			if (this.userPageBreakCols == null || this.userPageBreakCols.Count <= 2)
			{
				throw new PageBreakCannotRemoveException(columnIndex);
			}

			// for adjust the maximum scaling
			this.pageBreakCols.RemoveAll(br => br == columnIndex);
			this.pageBreakCols.Sort();

			this.userPageBreakCols.RemoveAll(uc => uc == columnIndex || uc == columnIndex);

			AutoSetMaximumScaleForPages();
			AutoSplitPage();
		}

		/// <summary>
		/// Remove row page break from specified number of row
		/// </summary>
		/// <param name="rowIndex">zero-based number of row to remove specified page break</param>
		public void RemoveRowPageBreak(int rowIndex)
		{
			if ((this.pageBreakRows == null || !this.pageBreakRows.Contains(rowIndex))
				&& (this.userPageBreakRows == null || !this.userPageBreakRows.Contains(rowIndex)))
			{
				throw new PageBreakNotFoundException(rowIndex);
			}

			if (this.userPageBreakRows == null || this.userPageBreakRows.Count <= 2)
			{
				throw new PageBreakCannotRemoveException(rowIndex);
			}

			// for adjust the maximum scaling
			this.pageBreakRows.RemoveAll(br => br == rowIndex);
			this.pageBreakRows.Sort();

			this.userPageBreakRows.RemoveAll(uc => uc == rowIndex || uc == rowIndex);

			AutoSetMaximumScaleForPages();
			AutoSplitPage();
		}

		/// <summary>
		/// Clear all column page breaks
		/// </summary>
		public void ClearColumnPageBreaks()
		{
			if (this.userPageBreakCols != null && this.userPageBreakCols.Count > 2)
			{
				while (this.userPageBreakCols.Count > 2)
				{
					this.userPageBreakCols.RemoveAt(1);
				}

				this.userPageBreakCols.Sort();
				this.AutoSplitPage();
			}
		}

		/// <summary>
		/// Clear all row page breaks
		/// </summary>
		public void ClearRowPageBreaks()
		{
			if (this.userPageBreakRows != null && this.userPageBreakRows.Count > 2)
			{
				while (this.userPageBreakRows.Count > 2)
				{
					this.userPageBreakRows.RemoveAt(1);
				}

				this.userPageBreakRows.Sort();
				this.AutoSplitPage();
			}
		}

		/// <summary>
		/// Clear all page breaks
		/// </summary>
		public void ClearAllPageBreaks()
		{
			if (this.userPageBreakCols != null) this.userPageBreakCols.Clear();
			if (this.userPageBreakRows != null) this.userPageBreakRows.Clear();

			if (MaxContentRow > 0 && MaxContentCol > 0)
			{
				this.AutoSplitPage();
			}
		}

		/// <summary>
		/// Get number of printable pages
		/// </summary>
		/// <returns>Number of pages</returns>
		public int PrintPageCounts
		{
			get
			{
				return (this.pageBreakCols == null || this.pageBreakCols.Count == 0
					|| this.pageBreakRows == null || this.pageBreakRows.Count == 0)
					? 0 : (this.pageBreakCols.Count - 1) * (this.pageBreakRows.Count - 1);
			}
		}
		#endregion // Page Break Management API

		#region Events
		private void RaisePrintableRangeChangedEvent()
		{
			if (this.PrintableRangeChanged != null)
			{
				this.PrintableRangeChanged(this, null);
			}
		}
		#endregion // Events

		#region UI
		internal int FixPageBreakColIndex(int index, int col)
		{
			if (index == 0)
			{
				// right boundary
				int maxCol = this.printableRange.IsEmpty ? this.rows.Count : this.printableRange.EndCol;

				if (col >= maxCol) col = maxCol;
			}
			else if (index == this.pageBreakCols.Count - 1)
			{
				// left boundary
				int minCol = this.printableRange.IsEmpty ? 0 : this.printableRange.Col;

				if (col <= minCol) col = minCol + 1;
			}
			else
			{
				// right boundary
				int maxCol = this.printableRange.EndCol;
				if (col > maxCol + 1)
					col = maxCol + 1;
				else
				{
					// left boundary
					int minCol = this.printableRange.Col;
					if (col < minCol) col = minCol;
				}
			}

			return col;
		}

		internal int FixPageBreakRowIndex(int index, int row)
		{
			if (index == 0)
			{
				// bottom boundary
				int maxRow = this.printableRange.IsEmpty ? this.rows.Count : this.printableRange.EndRow;

				if (row >= maxRow) row = maxRow;
			}
			else if (index == this.pageBreakRows.Count - 1)
			{
				// top boundary
				int minRow = this.printableRange.IsEmpty ? 0 : this.printableRange.Row;

				if (row <= minRow) row = minRow + 1;
			}
			else
			{
				// bottom boundary
				int maxRow = this.printableRange.EndRow;
				if (row > maxRow + 1)
					row = maxRow + 1;
				else
				{
					// left boundary
					int minRow = this.printableRange.Row;
					if (row < minRow) row = minRow;
				}
			}

			return row;
		}

		#endregion // UI

		#region Debug Validation
#if DEBUG
		public void _Debug_Validate_PrintPageBreaks()
		{
			if(this.printSettings==null)
				throw new Exception("print settings is null");
				
			//if (this.pageSettings == null)
			//	throw new Exception("page settings is null");

			//if (this.printSettings == null)
			//	throw new Exception("print settings is null");

			if (this.printableRange.IsEmpty)
				throw new Exception("no printable range initialized");

			if (this.userPageBreakCols == null || this.userPageBreakCols.Count < 2)
				throw new Exception("user column page breaks invalid");

			if (this.userPageBreakRows == null || this.userPageBreakRows.Count < 2)
				throw new Exception("user row page breaks invalid");

			if (this.userPageBreakCols[0] != this.printableRange.Col)
				throw new Exception("user col left != printable range");

			if (this.userPageBreakCols.Last() != this.printableRange.EndCol + 1)
				throw new Exception("user col right != printable range");

			if (this.userPageBreakRows[0] != this.printableRange.Row)
				throw new Exception("user row left != printable range");

			if (this.userPageBreakRows.Last() != this.printableRange.EndRow + 1)
				throw new Exception("user row right != printable range");

			// compare page breaks and use page breaks
			foreach (var ubc in this.userPageBreakCols)
			{
				if (!this.pageBreakCols.Contains(ubc))
					throw new Exception("col user page break not containing in page breaks");
			}

			foreach (var ubr in this.userPageBreakRows)
			{
				if (!this.pageBreakRows.Contains(ubr))
					throw new Exception("row user page break not containing in page breaks");
			}

			Func<List<int>, bool> validate_array = (a) =>
			{
				var b = new List<int>(a.Distinct());
				b.Sort();

				if (a.Count != b.Count) return false;

				for (int i = 0; i < a.Count; i++)
				{
					if (a[i] != b[i]) return false;
				}

				return true;
			};

			if (!validate_array(this.pageBreakCols))
				throw new Exception("col page break contains invalid index");

			if (!validate_array(this.pageBreakRows))
				throw new Exception("row page break contains invalid index");

			if (!validate_array(this.userPageBreakCols))
				throw new Exception("user col page break contains invalid index");

			if (!validate_array(this.userPageBreakRows))
				throw new Exception("user row page break contains invalid index");

			if (this.printSettings != null && this.printSettings.PageScaling < 0f)
				throw new Exception("page scale < 0");

			Rectangle paperSize = this.GetPaperPrintBounds();
			RGFloat paperWidth = paperSize.Width;
			RGFloat paperHeight = paperSize.Height;
			RGFloat scale = this.printSettings == null ? 1f : this.printSettings.PageScaling;

			int pageCount = 0;

			pageCount = (this.pageBreakRows.Count -1)*( this.pageBreakCols.Count-1);
			
			//
			// Allow page width out of paper's width
			//
			//IteratePrintPages(range =>
			//{
			//	RGRectF bounds = this.GetRangeBounds(range);

			//	if (bounds.Width * scale > paperWidth)
			//		throw new Exception("page width out of paper");

			//	if (bounds.Height * scale > paperHeight)
			//		throw new Exception("page height out of paper");

			//	pageCount++;
			//	return true;
			//});

			int expect = this.PrintPageCounts;

			if (pageCount != expect)
				throw new Exception(string.Format("page count incorrect, expect {0} but {1}", expect, pageCount));
		}
#endif // DEBUG
		#endregion // Debug Validation
	}

}

#endif // PRINT