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
 * Author: Jingwood <jingwood at unvell.com>
 *
 * Copyright (c) 2012-2021 Jingwood, unvell.com, all rights reserved.
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

	#region ColumnHeadPart
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

				return new Rectangle(header.Left * scaleFactor + view.Left - view.ScrollViewLeft,
					view.Top - view.ScrollViewTop,
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


}
