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
 * Author: Jing <lujing at unvell.com>
 *
 * Copyright (c) 2012-2016 Jing <lujing at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Linq;

using unvell.ReoGrid.Data;

#if DEBUG
using System.Diagnostics;
#endif

#if WINFORM || ANDROID
using RGFloat = System.Single;
using RGIntDouble = System.Int32;
#elif WPF || iOS
using RGFloat = System.Double;
using RGIntDouble = System.Double;
#endif

using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Main;

namespace unvell.ReoGrid.Views
{
	/// <summary>
	/// Standard view controller for normal scene of control
	/// </summary>
	class NormalViewportController : ViewportController, IFreezableViewportController,
		IScrollableViewportController, IScalableViewportController
	{
		#region Constructor
		private LeadHeaderView leadHeadPart;

		// four-header (two columns, two rows)
		private RowHeaderView rowHeaderPart1 = null;
		private RowHeaderView rowHeaderPart2 = null;
		private ColumnHeaderView colHeaderPart2 = null;
		private ColumnHeaderView colHeaderPart1 = null;

		// four-viewport, any of one could be set as main-viewport, and others will be forzen
		private SheetViewport leftTopViewport = null;
		private SheetViewport leftBottomViewport = null;
		private SheetViewport rightBottomViewport = null;
		private SheetViewport rightTopViewport = null;

		// main-viewport used to decide the value of scrollbar
		internal SheetViewport mainViewport = null;

		private SpaceView rightBottomSpace;

		public NormalViewportController(Worksheet sheet)
			: base(sheet)
		{
			sheet.ViewportController = this;

			// space
			this.AddView(rightBottomSpace = new SpaceView());

			// unfreezed
			this.AddView(leadHeadPart = new LeadHeaderView(this));
			this.AddView(colHeaderPart2 = new ColumnHeaderView(this));
			this.AddView(rowHeaderPart2 = new RowHeaderView(this));
			this.AddView(rightBottomViewport = new SheetViewport(this));

			// default settings of viewports
			this.rightBottomViewport.ScrollableDirections = ScrollDirection.Both;

			this.FocusView = rightBottomViewport;
			this.mainViewport = rightBottomViewport;
		}
		#endregion // Constructor

		#region Visibility Management
		public override void SetViewVisible(ViewTypes viewFlag, bool visible)
		{
			base.SetViewVisible(viewFlag, visible);

			bool rowHeadVisible = IsViewVisible(ViewTypes.RowHeader);
			bool colHeadVisible = IsViewVisible(ViewTypes.ColumnHeader);

			bool rowOutlineHeadVisible = IsViewVisible(ViewTypes.RowOutline | ViewTypes.ColumnHeader);
			bool outlineColHeadVisible = IsViewVisible(ViewTypes.ColOutline | ViewTypes.RowHeader);

			#region Column Head
			if ((viewFlag & ViewTypes.ColumnHeader) == ViewTypes.ColumnHeader)
			{
				if (visible)
				{
					colHeaderPart2.Visible = true;
					colHeaderPart2.VisibleRegion = this.rightBottomViewport.VisibleRegion;

					if (this.FreezePos.Col > 0)
					{
						colHeaderPart1.Visible = true;
						colHeaderPart1.VisibleRegion = frozenVisibleRegion;
					}
				}
				else
				{
					colHeaderPart2.Visible = false;

					if (colHeaderPart1 != null) colHeaderPart1.Visible = false;
				}
			}
			#endregion // Column Head

			#region Row Head
			if ((viewFlag & ViewTypes.RowHeader) == ViewTypes.RowHeader)
			{
				if (visible)
				{
					rowHeaderPart2.Visible = true;
					rowHeaderPart2.VisibleRegion = this.rightBottomViewport.VisibleRegion;

					if (this.FreezePos.Row > 0)
					{
						rowHeaderPart1.Visible = true;
						rowHeaderPart1.VisibleRegion = frozenVisibleRegion;
					}
				}
				else
				{
					rowHeaderPart2.Visible = false;

					if (rowHeaderPart1 != null) rowHeaderPart1.Visible = false;
				}
			}
			#endregion

			leadHeadPart.Visible = IsViewVisible(ViewTypes.LeadHeader);

#if OUTLINE
			bool rowOutlineVisible = IsViewVisible(ViewTypes.RowOutline);
			bool colOutlineVisible = IsViewVisible(ViewTypes.ColOutline);

			// row outline
			if (rowOutlineVisible)
			{
				if (visible && this.rowOutlinePart2 == null)
				{
					this.rowOutlinePart2 = new RowOutlineView(this);

					// set view start position
					this.rowOutlinePart2.ViewTop = this.rowHeaderPart2.ViewTop;

					this.AddView(this.rowOutlinePart2);
				}

				if (rowOutlinePart2 != null) rowOutlinePart2.Visible = rowOutlineVisible;
				if (rowOutlinePart1 != null)
				{
					rowOutlinePart1.Visible = this.FreezePos.IsEmpty && rowOutlinePart2.Visible;
				}
			}
			else
			{
				if (rowOutlinePart2 != null) rowOutlinePart2.Visible = false;
				if (rowOutlinePart1 != null) rowOutlinePart1.Visible = false;
			}

			// row outline header
			if (rowOutlineHeadVisible)
			{
				if (this.rowOutlineHeadPart == null)
				{
					this.rowOutlineHeadPart = new RowOutlineHeaderView(this);
					this.AddView(this.rowOutlineHeadPart);
				}

				this.rowOutlineHeadPart.Visible = true;
			}
			else if (this.rowOutlineHeadPart != null)
			{
				this.rowOutlineHeadPart.Visible = false;
			}

			// column outline
			if (colOutlineVisible)
			{
				if (visible && this.colOutlinePart2 == null)
				{
					this.colOutlinePart2 = new ColumnOutlinePart(this);

					// set view start position
					this.colOutlinePart2.ViewLeft = this.colHeaderPart2.ViewLeft;

					this.AddView(this.colOutlinePart2);
				}

				if (colOutlinePart2 != null) colOutlinePart2.Visible = colOutlineVisible;
				if (colOutlinePart1 != null) colOutlinePart1.Visible = colOutlineVisible;
			}
			else
			{
				if (colOutlinePart2 != null) colOutlinePart2.Visible = false;
				if (colOutlinePart1 != null) colOutlinePart1.Visible = false;
			}

			// column outline header
			if (outlineColHeadVisible)
			{
				if (this.colOutlineHeadPart == null)
				{
					this.colOutlineHeadPart = new ColumnOutlineHeadPart(this);
					this.AddView(this.colOutlineHeadPart);
				}

				this.colOutlineHeadPart.Visible = true;
			}
			else if (this.colOutlineHeadPart != null)
			{
				this.colOutlineHeadPart.Visible = false;
			}

			// outline space
			if (rowOutlineVisible && colOutlineVisible)
			{
				if (this.outlineLeftTopSpace == null)
				{
					this.outlineLeftTopSpace = new OutlineLeftTopSpace(this);
					this.AddView(outlineLeftTopSpace);
				}

				this.outlineLeftTopSpace.Visible = visible;
			}
			else if (this.outlineLeftTopSpace != null)
			{
				this.outlineLeftTopSpace.Visible = false;
			}
#endif // OUTLINE
		}

		private Rectangle GetGridScaleBounds(CellPosition pos)
		{
			return GetGridScaleBounds(pos.Row, pos.Col);
		}
		private Rectangle GetGridScaleBounds(int row, int col)
		{
			var freezePos = this.worksheet.FreezePos;

			var rowHead = worksheet.rows[freezePos.Row];
			var colHead = worksheet.cols[freezePos.Col];

			return new Rectangle(colHead.Left * this.ScaleFactor, rowHead.Top * this.ScaleFactor,
				colHead.InnerWidth * this.ScaleFactor + 1, rowHead.InnerHeight * this.ScaleFactor + 1);
		}
		private Rectangle GetRangeScaledBounds(GridRegion region)
		{
			var startRowHead = worksheet.rows[region.startRow];
			var startColHead = worksheet.cols[region.startCol];

			var endRowHead = worksheet.rows[region.endRow];
			var endColHead = worksheet.cols[region.endCol];

			RGFloat x1 = startColHead.Left * this.ScaleFactor;
			RGFloat y1 = startRowHead.Top * this.ScaleFactor;
			RGFloat x2 = endColHead.Right * this.ScaleFactor;
			RGFloat y2 = endRowHead.Bottom * this.ScaleFactor;

			return new Rectangle(x1, y1, x2 - x1, y2 - y1);
		}

		public override IView FocusView
		{
			get
			{
				return base.FocusView;
			}

			set
			{
				base.FocusView = value;

				if (value == null)
				{
					base.FocusView = this.rightBottomViewport;
				}
			}
		}
		#endregion // Visibility Management

		#region Update

		GridRegion frozenVisibleRegion = GridRegion.Empty;
		GridRegion mainVisibleRegion = GridRegion.Empty;

		protected virtual void UpdateVisibleRegion()
		{
			// update right bottom visible region
			UpdateViewportVisibleRegion(this.rightBottomViewport, NormalViewportController.GetVisibleRegion(this.rightBottomViewport));

			var freezePos = this.worksheet.FreezePos;

			if (freezePos.Row > 0 || freezePos.Col > 0)
			{
				// update left top visible region
				UpdateViewportVisibleRegion(this.leftTopViewport, NormalViewportController.GetVisibleRegion(this.leftTopViewport));

				// update left bottom visible region
				UpdateViewportVisibleRegion(this.leftBottomViewport, new GridRegion(
					this.rightBottomViewport.VisibleRegion.startRow,
					this.leftTopViewport.VisibleRegion.startCol,
					this.rightBottomViewport.VisibleRegion.endRow,
					this.leftTopViewport.VisibleRegion.endCol));

				// update right top visible region
				UpdateViewportVisibleRegion(this.rightTopViewport, new GridRegion(
					this.leftTopViewport.VisibleRegion.startRow,
					this.rightBottomViewport.VisibleRegion.startCol,
					this.leftTopViewport.VisibleRegion.endRow,
					this.rightBottomViewport.VisibleRegion.endCol));
			}

			// frozen headers always are synchronized to left top viewport
			if (this.leftTopViewport != null)
			{
				this.colHeaderPart1.VisibleRegion = this.leftTopViewport.VisibleRegion;
				this.rowHeaderPart1.VisibleRegion = this.leftTopViewport.VisibleRegion;
			}

			// normal headers always are synchronized to right bottom viewport
			this.colHeaderPart2.VisibleRegion = this.rightBottomViewport.VisibleRegion;
			this.rowHeaderPart2.VisibleRegion = this.rightBottomViewport.VisibleRegion;
		}

		private void UpdateViewportVisibleRegion(Viewport viewport, GridRegion range)
		{
			var oldVisible = viewport.VisibleRegion;
			viewport.VisibleRegion = range;

			UpdateNewVisibleRegionTexts(viewport.VisibleRegion, oldVisible);
		}

		// TODO: Need performance improvement
		private void UpdateNewVisibleRegionTexts(GridRegion region, GridRegion oldVisibleRegion)
		{
#if DEBUG
			Stopwatch sw = Stopwatch.StartNew();
#endif

			// TODO: Need performance improvement
			//       do not perform this during visible region updating
			//
			// end of visible region updating
			this.worksheet.cells.Iterate(region.startRow, region.startCol, region.Rows, region.Cols, true, (r, c, cell) =>
			{
				var rowHeader = this.worksheet.rows[r];
				if (rowHeader.InnerHeight <= 0) return region.Cols;

				int cspan = cell.Colspan;
				if (cspan <= 0) return 1;

				if (cell.RenderScaleFactor != this.ScaleFactor
						&& !string.IsNullOrEmpty(cell.DisplayText))
				{
					worksheet.UpdateCellFont(cell, Core.UpdateFontReason.ScaleChanged);
					cell.RenderScaleFactor = this.ScaleFactor;
				}

				return (cspan <= 0) ? 1 : cspan;
			});

#if DEBUG
			sw.Stop();
			long ms = sw.ElapsedMilliseconds;

			if (ms > 10)
			{
				Debug.WriteLine("update new visible region text takes " + ms + " ms.");
			}
#endif // DEBUG
		}

		#region Visible Region Update
		/// <summary>
		/// Update visible region for viewport. Visible region decides how many rows and columns 
		/// of cells (from...to) will be displayed.
		/// </summary>
		internal static GridRegion GetVisibleRegion(Viewport viewport)
		{
#if DEBUG
			Stopwatch watch = Stopwatch.StartNew();
#endif

			var sheet = viewport.Worksheet;

			RGFloat scale = sheet.renderScaleFactor;

			Point viewStart = viewport.ViewStart;

			GridRegion region = GridRegion.Empty;

			RGFloat scaledViewLeft = viewStart.X;
			RGFloat scaledViewTop = viewStart.Y;
			RGFloat scaledViewRight = viewStart.X + viewport.Width / scale;
			RGFloat scaledViewBottom = viewStart.Y + viewport.Height / scale;

			// begin visible region updating
			if (viewport.Height > 0 && sheet.rows.Count > 0)
			{
				float contentBottom = sheet.rows.Last().Bottom;

				if (scaledViewTop > contentBottom)
				{
					region.startRow = sheet.RowCount - 1;
				}
				else
				{
					int index = sheet.rows.Count / 2;

					ArrayHelper.QuickFind(index, 0, sheet.rows.Count, (rindex) =>
					{
						RowHeader row = sheet.rows[rindex];

						float top = row.Top;
						float bottom = row.Bottom;

						if (scaledViewTop >= top && scaledViewTop <= bottom)
						{
							region.startRow = rindex;
							return 0;
						}
						else if (scaledViewTop < top)
							return -1;
						else if (scaledViewTop > bottom)
							return 1;
						else
							throw new InvalidOperationException();      // this case should not be reached
					});
				}

				if (scaledViewBottom > contentBottom)
				{
					region.endRow = sheet.rows.Count - 1;
				}
				else
				{
					int index = sheet.rows.Count / 2;

					ArrayHelper.QuickFind(index, 0, sheet.rows.Count, (rindex) =>
					{
						RowHeader row = sheet.rows[rindex];

						float top = row.Top;
						float btn = row.Bottom;

						if (scaledViewBottom >= top && scaledViewBottom <= btn)
						{
							region.endRow = rindex;
							return 0;
						}
						else if (scaledViewBottom < top)
							return -1;
						else if (scaledViewBottom > btn)
							return 1;
						else
							throw new InvalidOperationException();      // this case should not be reached
					});
				}
			}

			if (viewport.Width > 0 && sheet.cols.Count > 0)
			{
				float contentRight = sheet.cols.Last().Right;

				if (scaledViewLeft > contentRight)
				{
					region.startCol = sheet.cols.Count - 1;
				}
				else
				{
					int index = sheet.cols.Count / 2;

					ArrayHelper.QuickFind(index, 0, sheet.cols.Count, (cindex) =>
					{
						ColumnHeader col = sheet.cols[cindex];

						float left = col.Left;
						float rgt = col.Right;

						if (scaledViewLeft >= left && scaledViewLeft <= rgt)
						{
							region.startCol = cindex;
							return 0;
						}
						else if (scaledViewLeft < left)
							return -1;
						else if (scaledViewLeft > rgt)
							return 1;
						else
							throw new InvalidOperationException();      // this case should not be reached
					});
				}

				if (scaledViewRight > contentRight)
				{
					region.endCol = sheet.cols.Count - 1;
				}
				else
				{
					int index = sheet.cols.Count / 2;

					ArrayHelper.QuickFind(index, 0, sheet.cols.Count, (cindex) =>
					{
						ColumnHeader col = sheet.cols[cindex];

						float left = col.Left;
						float rgt = col.Right;

						if (scaledViewRight >= left && scaledViewRight <= rgt)
						{
							region.endCol = cindex;
							return 0;
						}
						else if (scaledViewRight < left)
							return -1;
						else if (scaledViewRight > rgt)
							return 1;
						else
							throw new InvalidOperationException();      // this case should not reach
					});
				}
			}


#if DEBUG
			Debug.Assert(region.endRow >= region.startRow);
			Debug.Assert(region.endCol >= region.startCol);

			watch.Stop();

			// for unsual visible region
			// when over 200 rows or columns were setted as visible region,
			// we need check for whether the algorithm above has any mistake.
			if (region.Rows > 200 || region.Cols > 200)
			{
				Debug.WriteLine(string.Format("unusual visible region detected: [row: {0} - {1}, col: {2} - {3}]: {4} ms.",
					region.startRow, region.endRow, region.startCol, region.endCol,
					watch.ElapsedMilliseconds));
			}

			if (watch.ElapsedMilliseconds > 15)
			{
				Debug.WriteLine("update visible region takes " + watch.ElapsedMilliseconds + " ms.");
			}
#endif

			return region;
		}
		#endregion // Visible Region Update

		public override void UpdateController()
		{
#if DEBUG
			Stopwatch sw = Stopwatch.StartNew();
#endif
			bool colOutlineVisible = IsViewVisible(ViewTypes.ColOutline);
			bool rowOutlineVisible = IsViewVisible(ViewTypes.RowOutline);

			bool colheadVisible = IsViewVisible(ViewTypes.ColumnHeader);
			bool rowheadVisible = IsViewVisible(ViewTypes.RowHeader);
			bool leadheadVisible = IsViewVisible(ViewTypes.LeadHeader);

			var freezePos = this.worksheet.FreezePos;
			bool isFrozen = freezePos.Row > 0 || freezePos.Col > 0;

			Rectangle outlineSpaceRect = new Rectangle(Bounds.X, Bounds.Y, 0, 0);
			Rectangle leadheadRect = new Rectangle(0, 0, 0, 0);
			Rectangle contentRect = new Rectangle(0, 0, 0, 0);

			RGFloat scale = this.ScaleFactor;

			if (colheadVisible)
			{
				leadheadRect.Height = (int)Math.Round(worksheet.colHeaderHeight * scale);
			}

			if (rowheadVisible)
			{
				leadheadRect.Width = (int)Math.Round(worksheet.rowHeaderWidth * scale);
			}

#if OUTLINE

			RGFloat minOutlineButtonScale = (Worksheet.OutlineButtonSize + 3) * Math.Min(scale, 1f);

			if (colOutlineVisible)
			{
				var outlines = worksheet.outlines[RowOrColumn.Column];

				if (outlines != null)
				{
					// 1
					outlineSpaceRect.Height = (int)Math.Round(outlines.Count * minOutlineButtonScale);
				}
				else
				{
					colOutlineVisible = false;
				}
			}

			if (rowOutlineVisible)
			{
				var outlines = worksheet.outlines[RowOrColumn.Row];

				if (outlines != null)
				{
					// 2
					outlineSpaceRect.Width = (int)Math.Round(outlines.Count * minOutlineButtonScale);
				}
				else
				{
					rowOutlineVisible = false;
				}
			}
#endif // OUTLINE

			leadheadRect.X = this.view.Left + outlineSpaceRect.Width;
			leadheadRect.Y = this.view.Top + outlineSpaceRect.Height;
			if (leadheadVisible) leadHeadPart.Bounds = leadheadRect;

			// cells display range boundary
			RGFloat contentWidth = this.view.Right - leadheadRect.Right;
			RGFloat contentHeight = this.view.Bottom - leadheadRect.Bottom;

			if (contentWidth < 0) contentWidth = 0;
			if (contentHeight < 0) contentHeight = 0;

			contentRect = new Rectangle(leadheadRect.Right, leadheadRect.Bottom, contentWidth, contentHeight);

#if OUTLINE
			if (colOutlineVisible)
			{
				if (rowheadVisible)
				{
					this.colOutlineHeadPart.Bounds = new Rectangle(leadheadRect.X, outlineSpaceRect.Y,
						leadheadRect.Width, outlineSpaceRect.Height);
				}

				this.colOutlinePart2.Bounds = new Rectangle(leadheadRect.Right, outlineSpaceRect.Y,
					contentRect.Width, outlineSpaceRect.Height);
			}

			if (rowOutlineVisible)
			{
				if (colheadVisible)
				{
					this.rowOutlineHeadPart.Bounds = new Rectangle(outlineSpaceRect.X,
						outlineSpaceRect.Bottom, outlineSpaceRect.Width, leadheadRect.Height);
				}
			}
#endif // OUTLINE

			Rectangle rightBottomRect = new Rectangle(0, 0, 0, 0);

			if (isFrozen)
			{
				#region Forzen Bounds Layout
				Point origin = new Point(0, 0);
				Rectangle leftTopRect = new Rectangle(0, 0, 0, 0);

				var freezeArea = this.worksheet.FreezeArea;
				Rectangle freezeBounds;

				switch (freezeArea)
				{
					default:
					case ReoGrid.FreezeArea.LeftTop:
						Rectangle gridLoc = GetGridScaleBounds(freezePos);
						origin = new Point(contentRect.X + gridLoc.X, contentRect.Y + gridLoc.Y);
						break;

					case ReoGrid.FreezeArea.RightBottom:
						freezeBounds = GetRangeScaledBounds(new GridRegion(
							freezePos.Row, freezePos.Col,
							this.worksheet.RowCount - 1, this.worksheet.ColumnCount - 1));

						origin = new Point(contentRect.Right - freezeBounds.Width, contentRect.Bottom - freezeBounds.Height);
						break;

					case ReoGrid.FreezeArea.LeftBottom:
						freezeBounds = GetRangeScaledBounds(new GridRegion(
							freezePos.Row, freezePos.Col,
							this.worksheet.RowCount - 1, this.worksheet.ColumnCount - 1));

						origin = new Point(contentRect.X + freezeBounds.X, contentRect.Bottom - freezeBounds.Height);
						break;

					case ReoGrid.FreezeArea.RightTop:
						freezeBounds = GetRangeScaledBounds(new GridRegion(
							freezePos.Row, freezePos.Col,
							this.worksheet.RowCount - 1, this.worksheet.ColumnCount - 1));

						origin = new Point(contentRect.Right - freezeBounds.Width, contentRect.Y + freezeBounds.Y);
						break;
				}

				if (origin.X < contentRect.X) origin.X = contentRect.X;
				if (origin.Y < contentRect.Y) origin.Y = contentRect.Y;
				if (origin.X > contentRect.Right) origin.X = contentRect.Right;
				if (origin.Y > contentRect.Bottom) origin.Y = contentRect.Bottom;

				// set left top
				leftTopRect = new Rectangle(contentRect.X, contentRect.Y, origin.X - contentRect.X, origin.Y - contentRect.Y);

				// set right bottom
				rightBottomRect = new Rectangle(leftTopRect.Right, leftTopRect.Bottom,
					contentRect.Width - leftTopRect.Width, contentRect.Height - leftTopRect.Height);

				// viewports
				this.leftTopViewport.Bounds = leftTopRect;
				this.leftBottomViewport.Bounds = new Rectangle(leftTopRect.X, rightBottomRect.Y, leftTopRect.Width, rightBottomRect.Height);
				this.rightTopViewport.Bounds = new Rectangle(rightBottomRect.X, leftTopRect.Y, rightBottomRect.Width, leftTopRect.Height);

				// column header
				this.colHeaderPart1.Bounds = new Rectangle(this.leftTopViewport.Left, this.leadHeadPart.Top,
					this.leftTopViewport.Width, worksheet.colHeaderHeight * scale);

				// row header
				this.rowHeaderPart1.Bounds = new Rectangle(this.leadHeadPart.Left, this.leftTopViewport.Top,
					worksheet.rowHeaderWidth * scale, this.leftTopViewport.Height);

#if OUTLINE
				CreateOutlineHeaderViewIfNotExist();

				// column outline
				if (colOutlineVisible)
				{
					this.colOutlinePart1.Bounds = new Rectangle(leftTopRect.X, outlineSpaceRect.Y,
						leftTopRect.Width, outlineSpaceRect.Height);
				}

				// row outline
				if (rowOutlineVisible)
				{
					this.rowOutlinePart1.Bounds = new Rectangle(outlineSpaceRect.X, leftTopRect.Y,
						outlineSpaceRect.Width, leftTopRect.Height);
				}
#endif // OUTLINE
				#endregion // Forzen Bounds Layout
			}
			else
			{
				rightBottomRect = contentRect;
			}

			this.rightBottomViewport.Bounds = rightBottomRect;

			this.colHeaderPart2.Bounds = new Rectangle(rightBottomRect.X, leadheadRect.Y, rightBottomRect.Width, leadheadRect.Height);
			this.rowHeaderPart2.Bounds = new Rectangle(leadheadRect.X, rightBottomRect.Y, leadheadRect.Width, rightBottomRect.Height);

#if OUTLINE
			if (rowOutlineVisible)
			{
				this.rowOutlinePart2.Bounds = new Rectangle(outlineSpaceRect.X, rightBottomRect.Y, outlineSpaceRect.Width, rightBottomRect.Height);
			}

			if (colOutlineVisible)
			{
				this.colOutlinePart2.Bounds = new Rectangle(rightBottomRect.X, outlineSpaceRect.Y, rightBottomRect.Width, outlineSpaceRect.Height);
			}

			if (rowOutlineVisible && colOutlineVisible)
			{
				outlineLeftTopSpace.Bounds = outlineSpaceRect;
			}
#endif // OUTLINE

			//if (this.mainViewport.Width < 0) this.mainViewport.Width = 0;
			//if (this.mainViewport.Height < 0) this.mainViewport.Height = 0;

#if WINFORM || ANDROID
			this.worksheet.controlAdapter.ScrollBarHorizontalLargeChange = this.scrollHorLarge = (int)Math.Round(this.view.Width);
			this.worksheet.controlAdapter.ScrollBarVerticalLargeChange = this.scrollVerLarge = (int)Math.Round(this.view.Height);
#elif WPF
			this.worksheet.controlAdapter.ScrollBarHorizontalLargeChange = this.scrollHorLarge = this.view.Width;
			this.worksheet.controlAdapter.ScrollBarVerticalLargeChange = this.scrollVerLarge = this.view.Height;
#endif // WPF

			this.UpdateVisibleRegion();
			this.UpdateScrollBarSize();

			// synchronize scale factor
			if (this.View != null && this.View.Children != null)
			{
				foreach (var child in this.View.Children)
				{
					child.ScaleFactor = this.View.ScaleFactor;
				}
			}

			this.view.UpdateView();

			this.worksheet.RequestInvalidate();

#if DEBUG
			sw.Stop();
			long ms = sw.ElapsedMilliseconds;
			if (ms > 0)
			{
				Debug.WriteLine("update viewport bounds done: " + ms + " ms.");
			}
#endif
		}

		public override void Reset()
		{
			var viewport = this.view as IViewport;

			if (viewport != null)
			{
				viewport.ViewStart = new Point(0, 0);
			}

			//this.view.ScaleFactor = 1f;
			this.view.UpdateView();

			mainVisibleRegion = GridRegion.Empty;
			frozenVisibleRegion = GridRegion.Empty;

			var adapter = this.worksheet.controlAdapter;
			adapter.ScrollBarHorizontalMinimum = 0;
			adapter.ScrollBarVerticalMinimum = 0;
			adapter.ScrollBarHorizontalValue = 0;
			adapter.ScrollBarVerticalValue = 0;
		}

		#endregion // Update

		#region Scroll

		public void HorizontalScroll(RGIntDouble value)
		{
			if (this.mainViewport.ViewLeft != value)
			{
				this.ScrollViews(ScrollDirection.Horizontal, value - this.mainViewport.ViewLeft, 0);
			}
		}

		public void VerticalScroll(RGIntDouble value)
		{
			if (this.mainViewport.ViewTop != value)
			{
				this.ScrollViews(ScrollDirection.Vertical, 0, value - this.mainViewport.ViewTop);
			}
		}

		private RGIntDouble scrollHorMin;
		private RGIntDouble scrollHorMax;
		private RGIntDouble scrollHorLarge;
		private RGFloat scrollHorValue;

		private RGIntDouble scrollVerMin;
		private RGIntDouble scrollVerMax;
		private RGIntDouble scrollVerLarge;
		private RGFloat scrollVerValue;

		public virtual void ScrollViews(ScrollDirection dir, RGFloat x, RGFloat y)
		{
			if (x == 0 && y == 0) return;
			if (RGFloat.IsNaN(x) || RGFloat.IsNaN(y)) return;

			scrollHorValue = this.mainViewport.ViewLeft;
			scrollVerValue = this.mainViewport.ViewTop;

			var apt = this.worksheet.controlAdapter;

			if ((dir & ScrollDirection.Horizontal) == ScrollDirection.Horizontal)
			{
				if (scrollHorValue + x > scrollHorMax - scrollHorLarge)
					x = scrollHorMax - scrollHorLarge - scrollHorValue;
				if (scrollHorValue + x < scrollHorMin)
					x = scrollHorMin - scrollHorValue;
			}

			if ((dir & ScrollDirection.Vertical) == ScrollDirection.Vertical)
			{
				if (scrollVerValue + y > scrollVerMax - scrollVerLarge)
					y = scrollVerMax - scrollVerLarge - scrollVerValue;
				if (scrollVerValue + y < scrollVerMin)
					y = scrollVerMin - scrollVerValue;
			}

			if (x == 0 && y == 0) return;

			// if Control is in edit mode, it is necessary to finish the edit mode
			if (worksheet.IsEditing)
			{
				worksheet.EndEdit(EndEditReason.NormalFinish);
			}

			foreach (var v in this.view.Children)
			{
				if (v is IViewport)
				{
					IViewport viewpart = v as IViewport;

#if WIN32_SCROLL
						bool scrolled = false;
						Rectangle scrolledRect = Rectangle.Empty;
#endif

					if (viewpart.ScrollableDirections == dir)
					{

#if WIN32_SCROLL
							viewpart.Scroll(left, top);
#else
						viewpart.Scroll(x, y);
#endif
					}
					else
					{
						if ((viewpart.ScrollableDirections & ScrollDirection.Horizontal) == ScrollDirection.Horizontal
							&& (dir & ScrollDirection.Horizontal) == ScrollDirection.Horizontal
							&& x != 0)
						{
#if WIN32_SCROLL
								viewpart.Scroll(x, 0);
#else
							viewpart.Scroll(x, 0);
#endif
						}

						if ((viewpart.ScrollableDirections & ScrollDirection.Vertical) == ScrollDirection.Vertical
						 && (dir & ScrollDirection.Vertical) == ScrollDirection.Vertical
						 && y != 0)
						{
#if WIN32_SCROLL
								viewpart.Scroll(0, y);
#else
							viewpart.Scroll(0, y);
#endif
						}
					}
				}
			}

			// TODO: Performance Optimization: update visible region by offset 
			this.UpdateVisibleRegion();

			this.view.UpdateView();

#if WIN32_SCROLL
				var	updateRect = bounds;

				if (left > 0)
				{
					updateRect.X = updateRect.Right - left;
					updateRect.Width = left;
				}
				else if (left < 0)
				{
					updateRect.Width = -left;
				}

				if (top > 0)
				{
					updateRect.Y = updateRect.Bottom - top;
					updateRect.Height = top;
				}
				else if (top < 0)
				{
					updateRect.Height = -top;
				}

				grid.Invalidate(updateRect);
#else
			worksheet.RequestInvalidate();
#endif

			scrollHorValue += x;
			scrollVerValue += y;

#if WINFORM || ANDROID
			this.worksheet.controlAdapter.ScrollBarHorizontalValue = (int)Math.Round(scrollHorValue);
			this.worksheet.controlAdapter.ScrollBarVerticalValue = (int)Math.Round(scrollVerValue);
#elif WPF
			this.worksheet.ControlAdapter.ScrollBarHorizontalValue = scrollHorValue;
			this.worksheet.ControlAdapter.ScrollBarVerticalValue = scrollVerValue;
#endif // WPF

			if (x != 0 || y != 0)
			{
				if (this.worksheet.workbook != null)
				{
					this.worksheet.workbook.RaiseWorksheetScrolledEvent(this.worksheet, x, y);
				}
			}
		}

		public virtual void ScrollToRange(RangePosition range, CellPosition basePos)
		{
			var view = this.FocusView as Viewport;

			if (view != null)
			{
				Rectangle rect = this.worksheet.GetScaledRangeBounds(range);

				//var rect = this.worksheet.GetGridBounds(basePos.Row, basePos.Col);
				//rect.Width /= this.worksheet.scaleFactor;
				//rect.Height /= this.worksheet.scaleFactor;

				RGFloat scale = this.ScaleFactor;

				double top = view.ViewTop * scale;
				double bottom = view.ViewTop * scale + view.Height;
				double left = view.ViewLeft * scale;
				double right = view.ViewLeft * scale + view.Width;

				double offsetX = 0, offsetY = 0;

				if (rect.Height < view.Height)
				{
					// skip to scroll y if entire row is selected
					if ((view.ScrollableDirections & ScrollDirection.Vertical) == ScrollDirection.Vertical)
					{
						if (range.Rows < this.worksheet.rows.Count)
						{
							if (rect.Y < top/* && (range.Row <= view.VisibleRegion.startRow)*/)
							{
								offsetY = (rect.Y - top) / this.ScaleFactor;
							}
							else if (rect.Bottom >= bottom/* && (range.EndRow >= view.VisibleRegion.endRow)*/)
							{
								offsetY = (rect.Bottom - bottom) / this.ScaleFactor + 1;
							}
						}
					}
				}

				if (rect.Width < view.Width)
				{
					// skip to scroll x if entire column is selected
					if ((view.ScrollableDirections & ScrollDirection.Horizontal) == ScrollDirection.Horizontal)
					{
						if (range.Cols < this.worksheet.cols.Count)
						{
							if (rect.X < left /*&& (range.Col <= view.VisibleRegion.startCol)*/)
							{
								offsetX = (rect.X - left) / this.ScaleFactor;
							}
							else if (rect.Right >= right/* && (range.EndCol >= view.VisibleRegion.endCol)*/)
							{
								offsetX = (rect.Right - right) / this.ScaleFactor + 1;
							}
						}
					}
				}

				if (offsetX != 0 || offsetY != 0)
				{
					this.ScrollViews(ScrollDirection.Both, (int)Math.Round(offsetX), (int)Math.Round(offsetY));
				}
			}
		}

		private void UpdateScrollBarSize()
		{
			RGFloat scale = this.ScaleFactor;
			RGFloat width = 0, height = 0;

			if (worksheet.cols.Count > 0)
			{
				width = worksheet.cols[worksheet.cols.Count - 1].Right + mainViewport.Width - mainViewport.Width / scale + 1;
			}

			if (worksheet.rows.Count > 0)
			{
				height = worksheet.rows[worksheet.rows.Count - 1].Bottom + mainViewport.Height - mainViewport.Height / scale + 1;
			}

			if (this.worksheet.controlAdapter != null
				&& this.worksheet.controlAdapter.ControlInstance != null
				&& this.worksheet.controlAdapter.ControlInstance.ShowScrollEndSpacing
				&& this.worksheet.FreezeArea == FreezeArea.None)
			{
				width += 100 / scale;
				height += 100 / scale;
			}

			int maxHorizontal = (int)(Math.Round(width + this.mainViewport.Left));
			int maxVertical = Math.Max(0, (int)(Math.Round(height + this.mainViewport.Top)));

#if WINFORM || ANDROID
			int offHor = maxHorizontal - this.scrollHorMax;
			int offVer = maxVertical - this.scrollVerMax;
#elif WPF
				int offHor = (int)Math.Round(maxHorizontal - this.scrollHorMax);
				int offVer = (int)Math.Round(maxVertical - this.scrollVerMax);
#elif ANDROID || iOS
			RGFloat offHor = maxHorizontal - this.scrollHorMax;
			RGFloat offVer = maxVertical - this.scrollVerMax;

#endif // WPF
			if (offHor > 0) offHor = 0;
			if (offVer > 0) offVer = 0;

			if (offHor < 0 || offVer < 0)
			{
				ScrollViews(ScrollDirection.Both, offHor, offVer);
			}

			this.worksheet.controlAdapter.ScrollBarHorizontalMaximum = this.scrollHorMax = maxHorizontal;
			this.worksheet.controlAdapter.ScrollBarVerticalMaximum = this.scrollVerMax = maxVertical;
		}

		public void SynchronizeScrollBar()
		{
			if (this.scrollHorValue < this.scrollHorMin)
				this.scrollHorValue = this.scrollHorMin;
			else if (this.scrollHorValue > this.scrollHorMax)
				this.scrollHorValue = this.scrollHorMax;

			if (this.scrollVerValue < this.scrollVerMin)
				this.scrollVerValue = this.scrollVerMin;
			else if (this.scrollVerValue > this.scrollVerMax)
				this.scrollVerValue = this.scrollVerMax;

			this.worksheet.controlAdapter.ScrollBarHorizontalMaximum = this.scrollHorMax;
			this.worksheet.controlAdapter.ScrollBarVerticalMaximum = this.scrollVerMax;

			this.worksheet.controlAdapter.ScrollBarHorizontalMinimum = this.scrollHorMin;
			this.worksheet.controlAdapter.ScrollBarVerticalMinimum = this.scrollVerMin;

			this.worksheet.controlAdapter.ScrollBarHorizontalLargeChange = this.scrollHorLarge;
			this.worksheet.controlAdapter.ScrollBarVerticalLargeChange = this.scrollVerLarge;

			this.worksheet.controlAdapter.ScrollBarHorizontalValue = (int)Math.Round(this.scrollHorValue);
			this.worksheet.controlAdapter.ScrollBarVerticalValue = (int)Math.Round(this.scrollVerValue);
		}
		#endregion // Scroll

		#region Freeze

		//private CellPosition freezePos = new CellPosition(0, 0);

		public CellPosition FreezePos
		{
			get { return this.worksheet.FreezePos; }
			//set { freezePos = value; }
		}

		//private Point lastFreezePoint = new Point(0, 0);

		//public FreezeArea FreezeArea { get; set; }

		//internal void Freeze(CellPosition pos)
		//{
		//	Freeze(pos.Row, pos.Col);
		//}

		public void Freeze(/*int row, int col, FreezeArea position = ReoGrid.FreezeArea.LeftTop*/)
		{
			int row = this.worksheet.FreezePos.Row;
			int col = this.worksheet.FreezePos.Col;
			//var area = this.worksheet.FreezeArea;
			//this.freezePos.Row = row;
			//this.freezePos.Col = col;
			//this.FreezeArea = position;

			// origin freeze-viewports 
			Rectangle gridLoc = worksheet.GetGridBounds(row, col);

			if (row == 0 && col == 0)
			{
				// restore main-viewport to right-bottom-viewport
				this.mainViewport = this.rightBottomViewport;

				// restore right-bottom viewport settings
				this.rightBottomViewport.ViewStart = new Point(0, 0);

				// restore headers viewport settings
				this.rowHeaderPart2.ScrollableDirections = ScrollDirection.Vertical;
				this.colHeaderPart2.ScrollableDirections = ScrollDirection.Horizontal;

				// hide freeze viewports
				if (this.leftTopViewport != null)
				{
					this.leftTopViewport.Visible =
						this.leftBottomViewport.Visible =
						this.rightTopViewport.Visible =
						false;
				}

				if (rowHeaderPart1 != null) rowHeaderPart1.Visible = false;
				if (colHeaderPart1 != null) colHeaderPart1.Visible = false;
#if OUTLINE
				if (rowOutlinePart1 != null) rowOutlinePart1.Visible = false;
				if (colOutlinePart1 != null) colOutlinePart1.Visible = false;
#endif // OUTLINE
			}
			else
			{
				// right-top cells-viewport
				if (rightTopViewport == null) AddView(rightTopViewport = new SheetViewport(this));

				// left-bottom cells-viewport
				if (leftBottomViewport == null) AddView(leftBottomViewport = new SheetViewport(this));

				// headers (use InsertPart instead of AddPart to decide the z-orders of viewparts)
				if (colHeaderPart1 == null) AddView(colHeaderPart1 = new ColumnHeaderView(this));
				if (rowHeaderPart1 == null) AddView(rowHeaderPart1 = new RowHeaderView(this));

				CreateOutlineHeaderViewIfNotExist();

				// left-top cells-viewport
				if (leftTopViewport == null) AddView(leftTopViewport = new SheetViewport(this));

				// set up viewports view start postion
				leftTopViewport.ViewStart = new Point(0, 0);
				leftBottomViewport.ViewStart = new Point(0, gridLoc.Y);
				rightTopViewport.ViewStart = new Point(gridLoc.X, 0);
				rightBottomViewport.ViewStart = new Point(gridLoc.X, gridLoc.Y);

				// set up viewports by selected freeze position
				switch (this.worksheet.FreezeArea)
				{
					default:
					case ReoGrid.FreezeArea.LeftTop:
						this.leftTopViewport.ScrollableDirections = ScrollDirection.None;
						this.leftBottomViewport.ScrollableDirections = ScrollDirection.Vertical;
						this.rightTopViewport.ScrollableDirections = ScrollDirection.Horizontal;

						this.colHeaderPart1.ScrollableDirections = ScrollDirection.None;
						this.rowHeaderPart1.ScrollableDirections = ScrollDirection.None;
						this.colHeaderPart2.ScrollableDirections = ScrollDirection.Horizontal;
						this.rowHeaderPart2.ScrollableDirections = ScrollDirection.Vertical;

						this.mainViewport = rightBottomViewport;
						break;

					case ReoGrid.FreezeArea.RightBottom:
						this.leftBottomViewport.ScrollableDirections = ScrollDirection.Horizontal;
						this.rightTopViewport.ScrollableDirections = ScrollDirection.Vertical;
						this.rightBottomViewport.ScrollableDirections = ScrollDirection.None;

						this.colHeaderPart1.ScrollableDirections = ScrollDirection.Horizontal;
						this.rowHeaderPart1.ScrollableDirections = ScrollDirection.Vertical;
						this.colHeaderPart2.ScrollableDirections = ScrollDirection.None;
						this.rowHeaderPart2.ScrollableDirections = ScrollDirection.None;

						this.mainViewport = this.leftTopViewport;
						break;

					case ReoGrid.FreezeArea.LeftBottom:
						this.leftTopViewport.ScrollableDirections = ScrollDirection.Vertical;
						this.leftBottomViewport.ScrollableDirections = ScrollDirection.None;
						this.rightBottomViewport.ScrollableDirections = ScrollDirection.Horizontal;

						this.colHeaderPart1.ScrollableDirections = ScrollDirection.None;
						this.rowHeaderPart1.ScrollableDirections = ScrollDirection.Vertical;
						this.colHeaderPart2.ScrollableDirections = ScrollDirection.Horizontal;
						this.rowHeaderPart2.ScrollableDirections = ScrollDirection.None;

						this.mainViewport = this.rightTopViewport;
						break;

					case ReoGrid.FreezeArea.RightTop:
						this.leftTopViewport.ScrollableDirections = ScrollDirection.Horizontal;
						this.rightTopViewport.ScrollableDirections = ScrollDirection.None;
						this.rightBottomViewport.ScrollableDirections = ScrollDirection.Vertical;

						this.colHeaderPart1.ScrollableDirections = ScrollDirection.Horizontal;
						this.rowHeaderPart1.ScrollableDirections = ScrollDirection.None;
						this.colHeaderPart2.ScrollableDirections = ScrollDirection.None;
						this.rowHeaderPart2.ScrollableDirections = ScrollDirection.Vertical;

						this.mainViewport = this.leftBottomViewport;
						break;
				}

				leftTopViewport.Visible =
					leftBottomViewport.Visible =
					rightTopViewport.Visible =
					true;

				this.colHeaderPart1.Visible = colHeaderPart2.Visible;
				this.rowHeaderPart1.Visible = rowHeaderPart2.Visible;

#if OUTLINE
				if (this.colOutlinePart1 != null && this.colOutlinePart2 != null)
				{
					this.colOutlinePart1.Visible = colOutlinePart2.Visible;
				}
				if (this.rowOutlinePart1 != null && this.rowOutlinePart2 != null)
				{
					this.rowOutlinePart1.Visible = rowOutlinePart2.Visible;
				}
#endif // OUTLINE

			}

			// Scrollable direction for main viewport always should be Both
			this.mainViewport.ScrollableDirections = ScrollDirection.Both;

			if (this.rowHeaderPart1 != null)
			{
				this.rowHeaderPart1.ViewTop = this.leftTopViewport.ViewTop;
			}
			if (this.colHeaderPart1 != null)
			{
				this.colHeaderPart1.ViewLeft = this.leftTopViewport.ViewLeft;
			}

			this.colHeaderPart2.ViewLeft = gridLoc.X;
			this.rowHeaderPart2.ViewTop = gridLoc.Y;

			#region Outline
#if OUTLINE
			if (colOutlinePart2 != null)
			{
				colOutlinePart2.ViewLeft = gridLoc.X;
				colOutlinePart2.ScrollableDirections = colHeaderPart2.ScrollableDirections;

				if (colOutlinePart1 != null)
				{
					this.colOutlinePart1.ViewStart = this.colHeaderPart1.ViewStart;
					this.colOutlinePart1.ScrollableDirections = this.colHeaderPart1.ScrollableDirections;
				}
			}

			if (rowOutlinePart2 != null)
			{
				rowOutlinePart2.ViewTop = gridLoc.Y;
				rowOutlinePart2.ScrollableDirections = rowHeaderPart2.ScrollableDirections;

				if (rowOutlinePart1 != null)
				{
					this.rowOutlinePart1.ViewStart = this.rowHeaderPart1.ViewStart;
					this.rowOutlinePart1.ScrollableDirections = this.rowHeaderPart1.ScrollableDirections;
				}
			}
#endif // OUTLINE
			#endregion // Outline

			// scroll bars start at view-start of the main-viewport
			this.worksheet.controlAdapter.ScrollBarHorizontalMinimum = this.scrollHorMin = (int)Math.Round(this.mainViewport.ViewLeft);
			this.worksheet.controlAdapter.ScrollBarVerticalMinimum = this.scrollVerMin = (int)Math.Round(this.mainViewport.ViewTop);

			int hlc = (int)(this.view.Width - gridLoc.X);
			if (hlc < 0) hlc = 0;
			int vlc = (int)(this.view.Height - gridLoc.Y);
			if (vlc < 0) vlc = 0;

			this.worksheet.controlAdapter.ScrollBarHorizontalLargeChange = this.scrollHorLarge = hlc;
			this.worksheet.controlAdapter.ScrollBarVerticalLargeChange = this.scrollVerLarge = vlc;

			UpdateController();
		}

		private void CreateOutlineHeaderViewIfNotExist()
		{
#if OUTLINE
			// outline-views 
			if (rowOutlinePart1 == null && rowOutlinePart2 != null)
			{
				AddView(rowOutlinePart1 = new RowOutlineView(this));

				// move row outline header part to the topmost
				this.RemoveView(this.rowOutlineHeadPart);
				AddView(this.rowOutlineHeadPart);
			}

			if (colOutlinePart1 == null && colOutlinePart2 != null)
			{
				AddView(colOutlinePart1 = new ColumnOutlinePart(this));

				// move column outline header part to the topmost
				this.RemoveView(this.colOutlineHeadPart);
				AddView(this.colOutlineHeadPart);

				// adjust z-index of outline left top corner space
				if (this.outlineLeftTopSpace != null)
				{
					this.RemoveView(this.outlineLeftTopSpace);
					AddView(this.outlineLeftTopSpace);
				}
			}
#endif // OUTLINE
		}

		#endregion // Freeze

		#region Outline
#if OUTLINE
		private RowOutlineView rowOutlinePart2;
		private RowOutlineView rowOutlinePart1;
		private RowOutlineHeaderView rowOutlineHeadPart;
		private ColumnOutlinePart colOutlinePart2;
		private ColumnOutlinePart colOutlinePart1;
		private ColumnOutlineHeadPart colOutlineHeadPart;
		private OutlineLeftTopSpace outlineLeftTopSpace;
#endif // OUTLINE
		#endregion // Outline

		#region Draw
		public override void Draw(CellDrawingContext dc)
		{
			base.Draw(dc);

			var g = dc.Graphics;

			#region Freeze Split Line
			if (this.view != null
				&& this.worksheet.HasSettings(WorksheetSettings.View_ShowFrozenLine))
			{
				var freezePos = this.worksheet.FreezePos;

				if (freezePos.Col > 0)
				{
					g.DrawLine(leftBottomViewport.Right, this.view.Top, leftBottomViewport.Right, this.view.Bottom, SolidColor.Gray);
				}

				if (freezePos.Row > 0)
				{
					g.DrawLine(this.view.Left, rightTopViewport.Bottom, this.view.Right, rightTopViewport.Bottom, SolidColor.Gray);
				}
			}
			#endregion // Freeze Split Line
		}
		#endregion // Draw
	}
}
