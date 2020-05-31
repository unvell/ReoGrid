﻿/*****************************************************************************
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
		
		#region Draw
		public override void DrawView(CellDrawingContext dc)
		{
			this.sheet.drawingCanvas.ClipBounds = this.ViewBounds;
			this.sheet.drawingCanvas.Draw(dc);
		}
		#endregion // Draw

		#region Update
		public override void UpdateView()
		{
			base.UpdateView();

			this.sheet.drawingCanvas.ScaleX = this.scaleFactor;
			this.sheet.drawingCanvas.ScaleY = this.scaleFactor;
		}
		#endregion // Update

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

        public override void FreeFocus()
        {
            //所有的子image全部失去focus
            this.sheet.drawingCanvas.SetChildrenLostFocus();
            base.FreeFocus();
        }

        public override bool OnMouseDown(Point location, MouseButtons buttons)
		{
            #region //-------custom-------
            this.SetFocus();
            if (buttons == MouseButtons.Right)
            {
                bool locked = false;
                var obj = sheet.drawingCanvas.GetImageObject(location);
                if (obj != null)
                    locked = obj.IsLocked;
                sheet.controlAdapter.ShowContextMenuStrip(locked ? ViewTypes.FloatingImgLocked : ViewTypes.FloatingImg, PointToController(location));
            }
            #endregion
            return this.sheet.drawingCanvas.OnMouseDown(location, buttons);
		}

		public override bool OnMouseMove(Point location, MouseButtons buttons)
		{
            #region //---------custom--------
            //if view is not focused then return false to stop send the move event
            if (this.ViewportController.FocusView == this)
                return this.sheet.drawingCanvas.OnMouseMove(location, buttons);
            return false;
        // return this.sheet.drawingCanvas.OnMouseMove(location, buttons);
            #endregion //---------custom--------
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
