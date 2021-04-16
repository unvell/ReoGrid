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

#if DEBUG
using System.Diagnostics;
#endif

#if EX_SCRIPT
using unvell.ReoScript;
using unvell.ReoGrid.Script;
#endif // EX_SCRIPT

#if WINFORM || ANDROID
using RGFloat = System.Single;
#else
using RGFloat = System.Double;
#endif // WINFORM

using unvell.Common;

#if WINFORM || WPF
using unvell.Common.Win32Lib;
#endif // WINFORM || WPF

using unvell.ReoGrid.Core;
using unvell.ReoGrid.Utility;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Events;
using unvell.ReoGrid.Actions;
using unvell.ReoGrid.Data;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Interaction;

namespace unvell.ReoGrid.Views
{
	class CellsForegroundView : Viewport
	{
		public CellsForegroundView(IViewportController vc) : base(vc) { }

		public override IView GetViewByPoint(Point p)
		{
			// return null to always avoid making this view actived
			return null;
		}

		public override void Draw(CellDrawingContext dc)
		{
			var sheet = this.ViewportController.Worksheet;
			if (sheet == null || sheet.controlAdapter == null) return;

			var g = dc.Graphics;
			var controlStyle = sheet.workbook.controlAdapter.ControlStyle;

			switch (sheet.operationStatus)
			{
				case OperationStatus.AdjustColumnWidth:
					#region Draw Column Header Adjust Line
					if (sheet.currentColWidthChanging >= 0)
					{
						ColumnHeader col = sheet.cols[sheet.currentColWidthChanging];

						RGFloat left = col.Left * this.scaleFactor;// -ViewLeft * this.scaleFactor;
						RGFloat right = (col.Left + sheet.headerAdjustNewValue) * this.scaleFactor;// -ViewLeft * this.scaleFactor;
						RGFloat top = ScrollViewTop * this.scaleFactor;
						RGFloat bottom = ScrollViewTop * this.scaleFactor + this.Height;

						g.DrawLine(left, top, left, bottom, SolidColor.Black, 1, LineStyles.Dot);
						g.DrawLine(right, top, right, bottom, SolidColor.Black, 1, LineStyles.Dot);
					}
					#endregion // Draw Column Header Adjust Line
					break;

				case OperationStatus.AdjustRowHeight:
					#region Draw Row Header Adjust Line
					if (sheet.currentRowHeightChanging >= 0)
					{
						RowHeader row = sheet.rows[sheet.currentRowHeightChanging];

						RGFloat top = row.Top * this.scaleFactor;
						RGFloat bottom = (row.Top + sheet.headerAdjustNewValue) * this.scaleFactor;
						RGFloat left = ScrollViewLeft * this.scaleFactor;
						RGFloat right = ScrollViewLeft * this.scaleFactor + this.Width;

						g.DrawLine(left, top, right, top, SolidColor.Black, 1, LineStyles.Dot);
						g.DrawLine(left, bottom, right, bottom, SolidColor.Black, 1, LineStyles.Dot);
					}
					#endregion // Draw Row Header Adjust Line
					break;

				case OperationStatus.DragSelectionFillSerial:
				case OperationStatus.SelectionRangeMovePrepare:
				case OperationStatus.SelectionRangeMove:
					#region Selection Moving
					if (sheet.draggingSelectionRange != RangePosition.Empty
						&& dc.DrawMode == DrawMode.View
						&& sheet.HasSettings(WorksheetSettings.Edit_DragSelectionToMoveCells))
					{
						var scaledSelectionMovingRect = CellsViewport.GetScaledAndClippedRangeRect(this,
							sheet.draggingSelectionRange.StartPos,
							sheet.draggingSelectionRange.EndPos,
							controlStyle.SelectionBorderWidth);

						scaledSelectionMovingRect.Offset(-1, -1);

						SolidColor selectionBorderColor = controlStyle.Colors[ControlAppearanceColors.SelectionBorder];

						dc.Graphics.DrawRectangle(scaledSelectionMovingRect,
							ColorUtility.FromAlphaColor(255, selectionBorderColor),
							controlStyle.SelectionBorderWidth, LineStyles.Solid);
					}
					#endregion // Selection Moving
					break;
			}

		}
	}
}
