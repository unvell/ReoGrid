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
					Math.Min((sheet.rows[sheet.rows.Count - 1].Bottom - ScrollViewTop) * this.scaleFactor + bounds.Top, bounds.Bottom));
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

}
