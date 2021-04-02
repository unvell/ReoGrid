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

#if DEBUG
using System.Diagnostics;
#endif // DEBUG

#if WINFORM
using PlatformGraphics = System.Drawing.Graphics;
#elif WPF
using PlatformGraphics = System.Windows.Media.DrawingContext;
#endif // WPF

using unvell.ReoGrid.Views;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.Print
{
	/// <summary>
	/// Represents a print session to print worksheets.
	/// </summary>
	public partial class PrintSession : IDisposable
	{
		internal List<Worksheet> worksheets = new List<Worksheet>();

		private PrintSessionWorksheetCollection worksheetCollection = null;

		/// <summary>
		/// Get the collection of worksheet that will be printed out.
		/// </summary>
		public ICollection<Worksheet> Worksheets
		{
			get
			{
				if (this.worksheetCollection == null)
				{
					this.worksheetCollection = new PrintSessionWorksheetCollection(this);
				}

				return this.worksheetCollection;
			}
		}

		/// <summary>
		/// Get or set the zero-based index of current worksheet.
		/// </summary>
		internal int CurrentWorksheetIndex { get; set; }

		/// <summary>
		/// Get current worksheet instance.
		/// </summary>
		internal Worksheet CurrentWorksheet { get; set; }

		internal int CurrentRowIndex { get; set; }
		internal int CurrentColIndex { get; set; }

		internal Rectangle CurrentPaperBounds { get; set; }
		internal PrintSettings CurrentPrintSettings { get; set; }

		internal PrintSession()
		{
			this.CurrentWorksheetIndex = -1;
		}

		/// <summary>
		/// Get whether current is in the operation of print or preview
		/// </summary>
		public bool IsPrinting { get; internal set; }

		internal CellDrawingContext DrawingContext { get; set; }
		internal SheetViewport PrintViewport { get; set; }

		private ViewportController printViewportController;

		#region NextWorksheet
		internal void NextWorksheet()
		{
			this.CurrentWorksheetIndex++;

			if (this.CurrentWorksheetIndex < 0 || this.CurrentWorksheetIndex >= this.worksheets.Count)
			{
				// no more worksheets to print
				this.CurrentWorksheet = null;
				return;
			}

			this.CurrentWorksheet = this.worksheets[this.CurrentWorksheetIndex];

			if (this.CurrentWorksheet == null) return;

			var sheet = this.CurrentWorksheet;

			if (sheet.pageBreakRows == null || sheet.pageBreakCols == null
				|| sheet.pageBreakRows.Count == 0 || sheet.pageBreakCols.Count == 0)
			{
				sheet.AutoSplitPage();
			}

			if (sheet.pageBreakRows == null || sheet.pageBreakRows == null
				|| sheet.pageBreakRows.Count == 0 || sheet.pageBreakRows.Count == 0)
			{
				// no content found on this worksheet, skip to next
				NextWorksheet();
				return;
			}

			this.DrawingContext = new CellDrawingContext(sheet, DrawMode.Print);
			this.DrawingContext.FullCellClip = true;

			this.CurrentPrintSettings = sheet.PrintSettings;

			if (this.CurrentPrintSettings == null)
			{
				this.CurrentPrintSettings = new PrintSettings();
			}

			this.CurrentColIndex = 0;
			this.CurrentRowIndex = 0;

			this.CurrentPaperBounds = sheet.GetPaperPrintBounds();

			var currentPS = sheet.PrintSettings;

#if WINFORM
			this.currentGDIPageSettings = sheet.CreateGDIPageSettings();
#endif // WINFORM

		}
		#endregion // NextWorksheet

		#region NextPage
		internal bool hasMorePages = false;

		internal void NextPage(PlatformGraphics pg)
		{
			this.hasMorePages = false;

			var sheet = this.CurrentWorksheet;
			if (sheet == null) return;

			// out of print areas
			if (this.CurrentRowIndex >= sheet.pageBreakRows.Count
				&& this.CurrentColIndex >= sheet.pageBreakCols.Count)
			{
				return;
			}

			int row = sheet.pageBreakRows[this.CurrentRowIndex];
			int col = sheet.pageBreakCols[this.CurrentColIndex];

			int endRow = sheet.pageBreakRows[this.CurrentRowIndex + 1];
			int endCol = sheet.pageBreakCols[this.CurrentColIndex + 1];

			switch (this.CurrentPrintSettings.PageOrder)
			{
				default:
				case PrintPageOrder.DownThenOver:
					{
						if (this.CurrentRowIndex < sheet.pageBreakRows.Count - 2)
						{
							this.CurrentRowIndex++;

							this.hasMorePages = true;
						}
						else
						{
							if (this.CurrentColIndex < sheet.pageBreakCols.Count - 2)
							{
								this.CurrentRowIndex = 0;
								this.CurrentColIndex++;

								this.hasMorePages = true;
							}
							else
							{
								this.hasMorePages = false;
							}
						}
					}
					break;

				case PrintPageOrder.OverThenDown:
					{
						if (this.CurrentColIndex < sheet.pageBreakCols.Count - 2)
						{
							this.CurrentColIndex++;

							this.hasMorePages = true;
						}
						else
						{
							if (this.CurrentRowIndex < sheet.pageBreakRows.Count - 2)
							{
								this.CurrentColIndex = 0;
								this.CurrentRowIndex++;

								this.hasMorePages = true;
							}
							else
							{
								this.hasMorePages = false;
							}
						}
					}
					break;
			}

			if (this.DrawingContext.Graphics == null)
			{
#if WINFORM
				this.DrawingContext.Graphics = new unvell.ReoGrid.WinForm.GDIRenderer(pg);
#endif // WINFORM
			}
			else
			{
				this.DrawingContext.Graphics.Reset();
			}

			var ig = this.DrawingContext.Renderer;
			ig.PlatformGraphics = pg;

#if DEBUG
			Debug.WriteLine(string.Format("print page {0,3},{1,3} - {2,3},{3,3}", row, col, endRow, endCol));
#endif // DEBUG

			GridRegion gr = new GridRegion(row, col, endRow, endCol);

			if (this.printViewportController == null)
			{
				this.printViewportController = new ViewportController(sheet);
			}
			else
			{
				this.printViewportController.worksheet = sheet;
			}

			this.PrintViewport = new SheetViewport(this.printViewportController);
			this.PrintViewport.Bounds = this.CurrentPaperBounds;

			// refresh cells text boundary
			sheet.IterateCells(gr.ToRange(), (_unused_r, _unused_c, cell) =>
			{
				sheet.UpdateCellTextBounds(ig, cell, DrawMode.Print, this.CurrentPrintSettings.PageScaling, Core.UpdateFontReason.ScaleChanged);
				return true;
			});

			this.PrintViewport.VisibleRegion = gr;
			this.PrintViewport.ScaleFactor = this.CurrentPrintSettings.PageScaling;
			this.PrintViewport.ViewStart = new Point(sheet.cols[col].Left, sheet.rows[row].Top);
			this.PrintViewport.UpdateView();

			this.PrintViewport.Draw(this.DrawingContext);

			if (this.CurrentPrintSettings.ShowMargins)
			{
				var m = this.CurrentPaperBounds;

				var p = this.DrawingContext.Renderer.GetPen(SolidColor.Gray);

				lock (p)
				{
					ig.DrawLine(p, m.X - 50, m.Y, m.X, m.Y);
					ig.DrawLine(p, m.Right + 50, m.Y, m.Right, m.Y);

					ig.DrawLine(p, m.X - 50, m.Bottom, m.X, m.Bottom);
					ig.DrawLine(p, m.Right + 50, m.Bottom, m.Right, m.Bottom);

					ig.DrawLine(p, m.X, m.Y - 50, m.X, m.Y);
					ig.DrawLine(p, m.X, m.Bottom + 50, m.X, m.Bottom);

					ig.DrawLine(p, m.Right, m.Y - 50, m.Right, m.Y);
					ig.DrawLine(p, m.Right, m.Bottom + 50, m.Right, m.Bottom);
				}
			}

			if (!this.hasMorePages)
			{
				this.NextWorksheet();

				this.hasMorePages = this.CurrentWorksheet != null;
			}
		}
		#endregion // NextPage
	}

	internal class PrintSessionWorksheetCollection : ICollection<Worksheet>
	{
		private PrintSession session;

		internal PrintSessionWorksheetCollection(PrintSession session)
		{
			this.session = session;
		}

		public void Add(Worksheet worksheet)
		{
			CheckInPrinting();
			session.worksheets.Add(worksheet);
		}

		public void Clear()
		{
			CheckInPrinting();
		}

		public bool Contains(Worksheet worksheet)
		{
			return this.session.worksheets.Contains(worksheet);
		}

		public void CopyTo(Worksheet[] array, int arrayIndex)
		{
			this.session.worksheets.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return this.session.worksheets.Count; }
		}

		public bool IsReadOnly
		{
			get { return this.session.IsPrinting; }
		}

		public bool Remove(Worksheet worksheet)
		{
			CheckInPrinting();

			return this.session.worksheets.Remove(worksheet);
		}

		private void CheckInPrinting()
		{
			if (session.IsPrinting)
			{
				throw new InvalidOperationException("Cannot modify worksheet collection during printing.");
			}
		}

		public IEnumerator<Worksheet> GetEnumerator()
		{
			return session.worksheets.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return session.worksheets.GetEnumerator();
		}
	}
}

#endif // PRINT