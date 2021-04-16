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

#if DRAWING

using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Interaction;
using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.Views
{
	internal class DrawingViewport : Viewport
	{
		public DrawingViewport(IViewportController vc)
			: base(vc)
		{
		}
		
		public override void DrawView(CellDrawingContext dc)
		{
			this.sheet.drawingCanvas.ClipBounds = this.ViewBounds;
			this.sheet.drawingCanvas.Draw(dc);
		}

		public override void UpdateView()
		{
			base.UpdateView();

			this.sheet.drawingCanvas.ScaleX = this.scaleFactor;
			this.sheet.drawingCanvas.ScaleY = this.scaleFactor;
		}

		#region Find View
		public override IView GetViewByPoint(Point p)
		{
			var drawingCanvas = this.sheet.drawingCanvas;

			if (drawingCanvas == null
				|| drawingCanvas.Children == null
				|| drawingCanvas.Children.Count <= 0)
			{
				return null;
			}

			var vp = this.PointToView(p);

			for (int i = drawingCanvas.Children.Count - 1; i >= 0; i--)
			{
				var obj = drawingCanvas.Children[i];

				if (obj.Bounds.Contains(vp))
				{
					return this;
				}
			}

			return null;
		}
		#endregion // Find View

		#region UI Hanlders
		public override bool OnMouseDown(Point location, MouseButtons buttons)
		{
			return this.sheet.drawingCanvas.OnMouseDown(location, buttons);
		}

		public override bool OnMouseMove(Point location, MouseButtons buttons)
		{
			return this.sheet.drawingCanvas.OnMouseMove(location, buttons);
		}

		public override bool OnMouseUp(Point location, MouseButtons buttons)
		{
			return this.sheet.drawingCanvas.OnMouseUp(location, buttons);
		}

		public override bool OnMouseDoubleClick(Point location, MouseButtons buttons)
		{
			return this.sheet.drawingCanvas.OnMouseDoubleClick(location, buttons);
		}
		#endregion // UI Handlers
	}

}

#endif // DRAWING
