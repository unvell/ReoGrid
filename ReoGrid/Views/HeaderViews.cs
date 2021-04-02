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

#if WINFORM || ANDROID
using RGFloat = System.Single;
#elif WPF || iOS
using RGFloat = System.Double;
#endif

using unvell.Common;

using unvell.ReoGrid.Actions;
using unvell.ReoGrid.Events;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Interaction;
using unvell.ReoGrid.Main;

namespace unvell.ReoGrid.Views
{

	#region LeadHeaderPart
	class LeadHeaderView : View
	{
		protected Worksheet sheet;

		public LeadHeaderView(ViewportController vc)
			: base(vc)
		{
			this.sheet = vc.worksheet;
		}

		#region Draw
		public override void Draw(CellDrawingContext dc)
		{
			if (bounds.Width <= 0 || bounds.Height <= 0 || sheet.controlAdapter == null) return;

			var g = dc.Graphics;
			var controlStyle = sheet.workbook.controlAdapter.ControlStyle;

			g.FillRectangle(bounds, controlStyle.Colors[ControlAppearanceColors.LeadHeadNormal]);

			var startColor = sheet.isLeadHeadSelected ?
					controlStyle.Colors[ControlAppearanceColors.LeadHeadIndicatorStart]
					: controlStyle.Colors[ControlAppearanceColors.LeadHeadSelected];

			var endColor = controlStyle.Colors[ControlAppearanceColors.LeadHeadIndicatorEnd];

			dc.Renderer.DrawLeadHeadArrow(bounds, startColor, endColor);
		}
		#endregion // Draw

		public override bool OnMouseDown(Point location, MouseButtons buttons)
		{
			// mouse down in LeadHead?
			switch (this.sheet.operationStatus)
			{
				case OperationStatus.Default:
					if (this.sheet.selectionMode != WorksheetSelectionMode.None)
					{
						this.sheet.SelectRange(RangePosition.EntireRange);

						// show context menu
						if (buttons == MouseButtons.Right)
						{
							this.sheet.controlAdapter.ShowContextMenuStrip(ViewTypes.LeadHeader, location);
						}

						return true;
					}
					break;
			}

			return false;
		}

		public override bool OnMouseMove(Point location, MouseButtons buttons)
		{
			this.sheet.controlAdapter.ChangeCursor(CursorStyle.Selection);

			return false;
		}
	}
	#endregion // LeadHeaderPart

	#region ColumnHeadPart
	class HeaderView : Viewport, IRangeSelectableView
	{
		protected RGFloat headerAdjustBackup = 0;

		public HeaderView(IViewportController vc)
			: base(vc)
		{
		}
	}

	class ColumnHeaderView : HeaderView
	{
		public ColumnHeaderView(IViewportController vc)
			: base(vc)
		{
			this.ScrollableDirections = ScrollDirection.Horizontal;
		}

		#region Draw
		public override void DrawView(CellDrawingContext dc)
		{
			var r = dc.Renderer;
			var g = dc.Graphics;

			if (bounds.Height <= 0 || sheet.controlAdapter == null)
			{
				return;
			}

			var controlStyle = sheet.workbook.controlAdapter.ControlStyle;

			r.BeginDrawHeaderText(this.scaleFactor);

			var splitterLinePen = r.GetPen(controlStyle.Colors[ControlAppearanceColors.RowHeadSplitter]);
			var headerTextBrush = r.GetBrush(controlStyle.Colors[ControlAppearanceColors.ColHeadText]);

			bool isFullColSelected = sheet.SelectionRange.Rows == sheet.RowCount;

			for (int i = visibleRegion.startCol; i <= visibleRegion.endCol; i++)
			{
				bool isSelected = i >= sheet.SelectionRange.Col && i <= sheet.SelectionRange.EndCol;

				ColumnHeader header = sheet.cols[i];

				RGFloat x = header.Left * this.scaleFactor;
				RGFloat width = header.InnerWidth * this.scaleFactor;

				if (!header.IsVisible)
				{
					g.DrawLine(splitterLinePen, x - 1, 0, x - 1, bounds.Bottom);
				}
				else
				{
					Rectangle rect = new Rectangle(x, 0, width, bounds.Height);

#if WINFORM || WPF
					g.FillRectangleLinear(controlStyle.GetColHeadStartColor(false, isSelected, isSelected && isFullColSelected, false),
						controlStyle.GetColHeadEndColor(false, isSelected, isSelected && isFullColSelected, false), 90f, rect);
#elif ANDROID
					g.FillRectangle(rect, controlStyle.GetRowHeadEndColor(false, isSelected, isSelected && isFullColSelected, false));
#endif // ANDROID

					g.DrawLine(splitterLinePen, x, 0, x, bounds.Height);

					var textBrush = header.TextColor != null ? dc.Renderer.GetBrush((SolidColor)header.TextColor) : headerTextBrush;

					if (textBrush == null)
					{
						textBrush = headerTextBrush;
					}

					r.DrawHeaderText(header.RenderText, textBrush, rect);

					if (header.Body != null)
					{
						g.PushTransform();
						g.TranslateTransform(rect.X, rect.Y);
						header.Body.OnPaint(dc, rect.Size);
						g.PopTransform();
					}
				}
			}

			RGFloat lx = sheet.cols[visibleRegion.endCol].Right * this.scaleFactor;
			g.DrawLine(splitterLinePen, lx, 0, lx, bounds.Height);

			//g.DrawLine(splitterLinePen, this.ViewLeft, bounds.Height, this.ViewLeft + bounds.Width, bounds.Height);

			// bottom line
			//if (!sheet.HasSettings(WorksheetSettings.View_ShowGuideLine))
			//{
			//	g.DrawLine(ViewLeft, bounds.Bottom,
			//		Math.Min((sheet.cols[sheet.cols.Count - 1].Right - ViewLeft) * this.scaleFactor + bounds.Left, bounds.Width),
			//		//ViewLeft+ bounds.Width,
			//		bounds.Bottom, controlStyle.Colors[ControlAppearanceColors.ColHeadSplitter]);
			//}
		}

		#endregion // Draw

		#region Mouse
		public override bool OnMouseDown(Point location, MouseButtons buttons)
		{
			bool isProcessed = false;

			switch (sheet.operationStatus)
			{
				case OperationStatus.Default:
					int col = -1;
					bool inSeparator = sheet.FindColumnByPosition(location.X, out col);

					if (col >= 0)
					{
						// adjust columns width
						if (inSeparator
							&& buttons == MouseButtons.Left
							&& sheet.HasSettings(WorksheetSettings.Edit_AllowAdjustColumnWidth))
						{
							sheet.currentColWidthChanging = col;
							sheet.operationStatus = OperationStatus.AdjustColumnWidth;
							sheet.controlAdapter.ChangeCursor(CursorStyle.ChangeColumnWidth);
							sheet.RequestInvalidate();

							this.headerAdjustBackup = sheet.headerAdjustNewValue = sheet.cols[sheet.currentColWidthChanging].InnerWidth;
							this.SetFocus();

							isProcessed = true;
						}

						if (!isProcessed)
						{
							var header = sheet.cols[col];

							if (header.Body != null)
							{
								// let body to decide the mouse behavior
								var arg = new WorksheetMouseEventArgs(sheet, new Point(
									((location.X - header.Left) * this.scaleFactor),
									(location.Y / this.scaleFactor)),
									new Point((location.X - header.Left) * this.scaleFactor + this.Left,
										location.Y / this.scaleFactor), buttons, 1);

								isProcessed = header.Body.OnMouseDown(
									new Size(header.InnerWidth * this.scaleFactor, sheet.colHeaderHeight), arg);
							}
						}

						if (!isProcessed
							// do not allow to select column if selection mode is null
							&& sheet.selectionMode != WorksheetSelectionMode.None)
						{
							bool isFullColSelected =
								(sheet.selectionMode == WorksheetSelectionMode.Range
								&& sheet.selectionRange.Rows == sheet.rows.Count
								&& sheet.selectionRange.ContainsColumn(col));

							// select whole column
							if ((!isFullColSelected || buttons == MouseButtons.Left))
							{
								sheet.operationStatus = OperationStatus.FullColumnSelect;
								sheet.controlAdapter.ChangeCursor(CursorStyle.FullColumnSelect);

								SetFocus();

								sheet.SelectRangeStartByMouse(this.PointToController(location));

								isProcessed = true;
							}
						}

						// show context menu
						if (buttons == MouseButtons.Right)
						{
							sheet.ControlAdapter.ShowContextMenuStrip(ViewTypes.ColumnHeader, this.PointToController(location));
						}
					}
					break;
			}

			return isProcessed;
		}

		public override bool OnMouseMove(Point location, MouseButtons buttons)
		{
			bool isProcessed = false;

			switch (sheet.operationStatus)
			{
				case OperationStatus.AdjustColumnWidth:
					if (sheet.currentColWidthChanging >= 0
					&& buttons == MouseButtons.Left)
					{
						ColumnHeader colHeader = sheet.cols[sheet.currentColWidthChanging];
						sheet.headerAdjustNewValue = location.X - colHeader.Left;
						if (sheet.headerAdjustNewValue < 0) sheet.headerAdjustNewValue = 0;

						this.sheet.controlAdapter.ChangeCursor(CursorStyle.ChangeColumnWidth);
						this.sheet.RequestInvalidate();
						isProcessed = true;
					}
					break;

				case OperationStatus.Default:
					{
						if (sheet.currentColWidthChanging == -1 && sheet.currentRowHeightChanging == -1)
						{
							int col = -1;

							// find the column index
							bool inline = sheet.FindColumnByPosition(location.X, out col)
									&& sheet.HasSettings(WorksheetSettings.Edit_AllowAdjustColumnWidth);

							if (col >= 0)
							{
								CursorStyle curStyle = inline ? CursorStyle.ChangeColumnWidth :
									(sheet.selectionMode == WorksheetSelectionMode.None ? CursorStyle.Selection : CursorStyle.FullColumnSelect);

								var header = sheet.cols[col];

								// check if header body exists
								if (header.Body != null)
								{
									// let cell's body decide the mouse behavior
									var arg = new WorksheetMouseEventArgs(sheet, new Point(
										((location.X - header.Left) * this.scaleFactor),
										(location.Y / this.scaleFactor)), location, buttons, 1)
									{
										CursorStyle = curStyle
									};

									isProcessed = header.Body.OnMouseMove(
										new Size(header.InnerWidth * this.scaleFactor, sheet.colHeaderHeight), arg);

									curStyle = arg.CursorStyle;
								}

								sheet.controlAdapter.ChangeCursor(curStyle);
							}
						}
					}
					break;

				case OperationStatus.FullColumnSelect:
				case OperationStatus.FullSingleColumnSelect:
					if (buttons == MouseButtons.Left)
					{
						sheet.controlAdapter.ChangeCursor(CursorStyle.FullColumnSelect);
						sheet.SelectRangeEndByMouse(this.PointToController(location));

						isProcessed = true;
					}
					break;
			}

			return isProcessed;
		}

		public override bool OnMouseUp(Point location, MouseButtons buttons)
		{
			switch (sheet.operationStatus)
			{
				case OperationStatus.AdjustColumnWidth:
					if (sheet.currentColWidthChanging > -1)
					{
						SetColumnsWidthAction setColsWidthAction;

						bool isFullColSelected = (sheet.selectionMode == WorksheetSelectionMode.Range
							&& sheet.selectionRange.Rows == sheet.rows.Count
							&& sheet.selectionRange.ContainsColumn(sheet.currentColWidthChanging));

						ushort targetWidth = (ushort)sheet.headerAdjustNewValue;

						if (targetWidth != this.headerAdjustBackup)
						{
							if (isFullColSelected)
							{
								setColsWidthAction = new SetColumnsWidthAction(sheet.selectionRange.Col, sheet.selectionRange.Cols, targetWidth);
							}
							else
							{
								setColsWidthAction = new SetColumnsWidthAction(sheet.currentColWidthChanging, 1, targetWidth);
							}

							sheet.DoAction(setColsWidthAction);
						}
					}

					sheet.currentColWidthChanging = -1;
					sheet.operationStatus = OperationStatus.Default;
					sheet.RequestInvalidate();

					this.headerAdjustBackup = sheet.headerAdjustNewValue = 0;
					this.FreeFocus();

					return true;

				case OperationStatus.FullColumnSelect:
					sheet.operationStatus = OperationStatus.Default;
					sheet.ControlAdapter.ChangeCursor(CursorStyle.Selection);
					this.FreeFocus();
					return true;
			}

			return false;
		}

		public override bool OnMouseDoubleClick(Point location, MouseButtons buttons)
		{
			int col = -1;
			bool inSeparator = sheet.FindColumnByPosition(location.X, out col);

			if (col >= 0)
			{
				// adjust columns width
				if (inSeparator
					&& buttons == MouseButtons.Left
					&& this.sheet.HasSettings(WorksheetSettings.Edit_AllowAdjustColumnWidth))
				{
					this.sheet.AutoFitColumnWidth(col, byAction: true);

					return true;
				}
			}

			return false;
		}
		#endregion // Mouse

		#region Utility
		public static Rectangle GetColHeaderBounds(Worksheet sheet, int col, Point position)
		{
			if (sheet == null) throw new ArgumentNullException("sheet");

			var viewportController = sheet.ViewportController;

			if (viewportController == null || viewportController.View == null)
			{
				throw new ArgumentNullException("viewportController");
			}

			//viewportController.Bounds

			IViewport view = viewportController.View.GetViewByPoint(position) as ColumnHeaderView;

			if (view == null)
			{
				throw new ArgumentNullException("Cannot found column header view from specified position");
			}

			if (view is ColumnHeaderView)
			{
				var header = sheet.RetrieveColumnHeader(col);

				RGFloat scaleFactor = sheet.renderScaleFactor;

				return new Rectangle(header.Left * scaleFactor + view.Left - view.ViewLeft,
					view.Top - view.ViewTop,
					header.InnerWidth * scaleFactor,
					sheet.colHeaderHeight * scaleFactor);
			}
			else
			{
				return new Rectangle();
			}
		}
		#endregion // Utility
	}
	#endregion // ColumnHeadPart

	#region RowHeaderView
	class RowHeaderView : HeaderView
	{
		public RowHeaderView(IViewportController vc)
			: base(vc)
		{
			this.ScrollableDirections = ScrollDirection.Vertical;
		}

		public override Point PointToView(Point p)
		{
			return base.PointToView(p);
		}

		#region Draw
		public override void DrawView(CellDrawingContext dc)
		{
			var g = dc.Renderer;

			if (bounds.Width <= 0 || sheet.controlAdapter == null)
			{
				return;
			}

			var controlStyle = sheet.workbook.controlAdapter.ControlStyle;

			g.BeginDrawHeaderText(this.scaleFactor);

			var splitterLinePen = dc.Renderer.GetPen(controlStyle.Colors[ControlAppearanceColors.RowHeadSplitter]);
			var defaultTextBrush = dc.Renderer.GetBrush(controlStyle.Colors[ControlAppearanceColors.RowHeadText]);

			bool isFullRowSelected = sheet.SelectionRange.Cols == sheet.ColumnCount;

			for (int i = visibleRegion.startRow; i <= visibleRegion.endRow; i++)
			{
				bool isSelected = i >= sheet.SelectionRange.Row && i <= sheet.SelectionRange.EndRow;

				RowHeader row = sheet.rows[i];
				RGFloat y = row.Top * this.scaleFactor;

				if (!row.IsVisible)
				{
					g.DrawLine(splitterLinePen, 0, y - 1, bounds.Width, y - 1);
				}
				else
				{
					Rectangle rect = new Rectangle(0, y, bounds.Width, row.InnerHeight * this.scaleFactor);

					if (rect.Height > 0)
					{
						g.FillRectangle(rect, controlStyle.GetRowHeadEndColor(false, isSelected, isSelected && isFullRowSelected, false));
						g.DrawLine(splitterLinePen, new Point(0, y), new Point(bounds.Width, y));

						var headerText = row.Text != null ? row.Text : (row.Row + 1).ToString();

						if (!string.IsNullOrEmpty(headerText))
						{
							var textBrush = row.TextColor != null ?
								dc.Renderer.GetBrush((SolidColor)row.TextColor) : defaultTextBrush;

							if (textBrush == null)
							{
								textBrush = defaultTextBrush;
							}

							g.DrawHeaderText(headerText, textBrush, rect);
						}

						if (row.Body != null)
						{
							g.PushTransform();
							g.TranslateTransform(rect.X, rect.Y);
							row.Body.OnPaint(dc, rect.Size);
							g.PopTransform();
						}
					}
				}
			}

			if (visibleRegion.endRow >= 0)
			{
				RGFloat ly = sheet.rows[visibleRegion.endRow].Bottom * this.scaleFactor;
				g.DrawLine(splitterLinePen, 0, ly, bounds.Width, ly);
			}

			// right line
			if (!sheet.HasSettings(WorksheetSettings.View_ShowGridLine))
			{
				dc.Graphics.DrawLine(dc.Renderer.GetPen(controlStyle.Colors[ControlAppearanceColors.RowHeadSplitter]),
					bounds.Right, bounds.Y, bounds.Right,
					Math.Min((sheet.rows[sheet.rows.Count - 1].Bottom - ViewTop) * this.scaleFactor + bounds.Top, bounds.Bottom));
			}
		}
		#endregion // Draw

		#region Mouse
		public override bool OnMouseDown(Point location, MouseButtons buttons)
		{
			bool isProcessed = false;

			int row = -1;

			switch (this.sheet.operationStatus)
			{
				case OperationStatus.Default:

					bool inSeparator = sheet.FindRowByPosition(location.Y, out row);

					if (row >= 0 && row < sheet.rows.Count)
					{
						if (inSeparator
							&& buttons == MouseButtons.Left
							&& sheet.HasSettings(WorksheetSettings.Edit_AllowAdjustRowHeight))
						{
							sheet.currentRowHeightChanging = row;
							sheet.operationStatus = OperationStatus.AdjustRowHeight;
							sheet.controlAdapter.ChangeCursor(CursorStyle.ChangeRowHeight);
							sheet.RequestInvalidate();

							this.headerAdjustBackup = sheet.headerAdjustNewValue = sheet.rows[sheet.currentRowHeightChanging].InnerHeight;
							this.SetFocus();

							isProcessed = true;
						}
						else if (sheet.selectionMode != WorksheetSelectionMode.None)
						{
							// check whether entire row is selected, select row if not
							bool isFullRowSelected = (sheet.selectionMode == WorksheetSelectionMode.Range
								&& sheet.selectionRange.Cols == sheet.cols.Count
								&& sheet.selectionRange.ContainsRow(row));

							if ((!isFullRowSelected || buttons == MouseButtons.Left))
							{
								sheet.operationStatus = OperationStatus.FullRowSelect;
								sheet.controlAdapter.ChangeCursor(CursorStyle.FullRowSelect);

								SetFocus();

								sheet.SelectRangeStartByMouse(this.PointToController(location));

								isProcessed = true;
							}

							if (buttons == MouseButtons.Right)
							{
								sheet.controlAdapter.ShowContextMenuStrip(ViewTypes.RowHeader, this.PointToController(location));
							}
						}
					}
					break;
			}

			return isProcessed;
		}

		public override bool OnMouseMove(Point location, MouseButtons buttons)
		{
			bool isProcessed = false;

			switch (sheet.operationStatus)
			{
				case OperationStatus.Default:
					if (buttons == MouseButtons.None
						&& sheet.HasSettings(WorksheetSettings.Edit_AllowAdjustRowHeight))
					{
						int row = -1;
						bool inline = sheet.FindRowByPosition(location.Y, out row);

						if (row >= 0) sheet.controlAdapter.ChangeCursor(inline ? CursorStyle.ChangeRowHeight :
							(sheet.selectionMode == WorksheetSelectionMode.None ? CursorStyle.PlatformDefault : CursorStyle.FullRowSelect));
					}
					break;

				case OperationStatus.AdjustRowHeight:
					if (buttons == MouseButtons.Left
						&& sheet.currentRowHeightChanging >= 0)
					{
						RowHeader rowHeader = sheet.rows[sheet.currentRowHeightChanging];
						sheet.headerAdjustNewValue = location.Y - rowHeader.Top;
						if (sheet.headerAdjustNewValue < 0) sheet.headerAdjustNewValue = 0;

						sheet.controlAdapter.ChangeCursor(CursorStyle.ChangeRowHeight);
						sheet.RequestInvalidate();
						isProcessed = true;
					}
					break;

				case OperationStatus.FullRowSelect:
					sheet.SelectRangeEndByMouse(this.PointToController(location));
					sheet.controlAdapter.ChangeCursor(CursorStyle.FullRowSelect);

					isProcessed = true;
					break;
			}

			return isProcessed;
		}

		public override bool OnMouseUp(Point location, MouseButtons buttons)
		{
			bool isProcessed = false;

			switch (sheet.operationStatus)
			{
				case OperationStatus.AdjustRowHeight:
					if (sheet.currentRowHeightChanging > -1)
					{
						SetRowsHeightAction setRowsHeightAction;

						bool isFullRowSelected = (sheet.selectionMode == WorksheetSelectionMode.Range
							&& sheet.selectionRange.Cols == sheet.cols.Count
							&& sheet.selectionRange.ContainsRow(sheet.currentRowHeightChanging));

						ushort targetHeight = (ushort)sheet.headerAdjustNewValue;

						if (targetHeight != this.headerAdjustBackup)
						{
							if (isFullRowSelected)
							{
								setRowsHeightAction = new SetRowsHeightAction(sheet.selectionRange.Row, sheet.selectionRange.Rows, targetHeight);
							}
							else
							{
								setRowsHeightAction = new SetRowsHeightAction(sheet.currentRowHeightChanging, 1, targetHeight);
							}

							sheet.DoAction(setRowsHeightAction);
						}

						sheet.currentRowHeightChanging = -1;
						sheet.operationStatus = OperationStatus.Default;
						this.headerAdjustBackup = sheet.headerAdjustNewValue = 0;

						sheet.RequestInvalidate();
						this.FreeFocus();
						isProcessed = true;
					}
					break;

				case OperationStatus.FullRowSelect:
				case OperationStatus.FullSingleRowSelect:
					sheet.operationStatus = OperationStatus.Default;
					sheet.controlAdapter.ChangeCursor(CursorStyle.Selection);

					this.FreeFocus();
					isProcessed = true;
					break;
			}

			return isProcessed;
		}

		public override bool OnMouseDoubleClick(Point location, MouseButtons buttons)
		{
			int row = -1;
			bool inSeparator = sheet.FindRowByPosition(location.Y, out row);

			if (row >= 0)
			{
				// adjust row height
				if (inSeparator
					&& buttons == MouseButtons.Left
					&& this.sheet.HasSettings(WorksheetSettings.Edit_AllowAdjustRowHeight))
				{
					this.sheet.AutoFitRowHeight(row, byAction: true);

					return true;
				}
			}

			return false;
		}
		#endregion // Mouse

	}
	#endregion // RowHeaderView

	#region XRuler (Reserved)
	class XRuler : View
	{
		public XRuler(IViewportController vc) : base(vc) { }
		public override void Draw(CellDrawingContext dc)
		{
			throw new NotImplementedException();
		}
	}
	#endregion // XRuler

	#region Header Space
	class SpaceView : View
	{
		public SpaceView() : base() { }

		public override void Draw(CellDrawingContext dc)
		{
			dc.Graphics.FillRectangle(bounds, Rendering.StaticResources.SystemColor_Control);
		}
	}
	#endregion // Header Space

}
