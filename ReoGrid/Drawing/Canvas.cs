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

#if DRAWING

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Views;

namespace unvell.ReoGrid.Drawing
{
	internal interface IDrawingCanvas : IDrawingContainer
	{

	}

	internal class DrawingCanvas : DrawingComponent, IDrawingCanvas
	{
		public DrawingCanvas()
		{

		}
	}

	internal class WorksheetDrawingCanvas : DrawingCanvas
	{
		internal Worksheet Worksheet { get; set; }

		public WorksheetDrawingCanvas(Worksheet sheet)
		{
			this.Worksheet = sheet;
		}

		public override void Invalidate()
		{
			if (this.Worksheet != null)
			{
				this.Worksheet.RequestInvalidate();
			}
		}

        #region //-----custom-------
        public ImageObject GetImageObject(Point p)
        {
            foreach (ImageObject obj in this.drawingObjects)
            {
                if (obj.Bounds.Contains(p) && obj.Visible)
                    return obj;
            }
            return null;
        }
        public ImageObject GetSelectedImageObject()
        {
            foreach (ImageObject obj in this.drawingObjects)
            {
                if (obj.IsSelected && obj.Visible)
                    return obj;
            }
            return null;
        }

        public void DeleteSelectedImage()
        {
            ImageObject sobj = null;
            foreach (ImageObject obj in this.drawingObjects)
            {
                if (obj.IsSelected && obj.Visible)
                {
                    sobj = obj;
                    break;
                }
            }
            if (sobj != null)
                this.drawingObjects.Remove(sobj);
        }

        public void MoveSelectedLayer(bool isUp)
        {
            ImageObject sobj = null;
            foreach (ImageObject obj in this.drawingObjects)
            {
                if (obj.IsSelected && obj.Visible)
                {
                    sobj = obj;
                    break;
                }
            }
            if (sobj != null)
            {
                var origin = this.drawingObjects.IndexOf(sobj);
                if (isUp)
                {
                    if (origin > 0)
                        Utility.OtherUtility.SwapItems<IDrawingObject>(this.drawingObjects, origin, origin - 1);
                }
                else
                {
                    if (origin < this.drawingObjects.Count - 1)
                        Utility.OtherUtility.SwapItems<IDrawingObject>(this.drawingObjects, origin, origin + 1);
                }
            }
        }

        #endregion //-----custom-------

        /// <summary>
        /// Worksheet Drawing Canvas alwayas keep transparent and doesn't draw anything from itself
        /// </summary>
        /// <param name="dc">Platform no-associated drawing context instance.</param>
        protected override void OnPaint(DrawingContext dc)
		{
			dc.Graphics.IsAntialias = true;
          //draw backwards the last in  first to draw
            if (this.drawingObjects?.Count > 0)
            {
                for (int i = this.drawingObjects.Count - 1; i >= 0; i--)
                {
                    var child = this.drawingObjects[i];
                    if (child is DrawingObject)
                    {
                        var drawingObject = (DrawingObject)child;
                        if (!drawingObject.Visible) continue;
                    }

                    if (this.ClipBounds.Width > 0 || this.ClipBounds.Height > 0)
                    {
                        if (child.Bounds.IntersectWith(this.ClipBounds))
                        {
                            child.Draw(dc);
                        }
                    }
                    else
                    {
                        child.Draw(dc);
                    }
                }
            }

            dc.Graphics.IsAntialias = false;
		}

		internal WorksheetDrawingObjectCollection worksheetDrawingObjectCollection;

		public override IDrawingObjectCollection Children
		{
			get
			{
				if (this.worksheetDrawingObjectCollection == null)
				{
					this.worksheetDrawingObjectCollection = new WorksheetDrawingObjectCollection(this);
				}

				return this.worksheetDrawingObjectCollection;
			}
		}

		internal void Clear()
		{
			this.Children.Clear();
		}
	}

	internal class WorksheetDrawingObjectCollection : DrawingObjectCollection
	{
		private WorksheetDrawingCanvas owner;

		internal WorksheetDrawingObjectCollection(WorksheetDrawingCanvas owner)
			: base(owner)
		{
			this.owner = owner;
		}

		public override void Add(IDrawingObject item)
		{
			base.Add(item);

			if (this.owner.Worksheet != null) this.owner.Worksheet.RequestInvalidate();
		}

		public override void AddRange(IEnumerable<IDrawingObject> drawingObjects)
		{
			base.AddRange(drawingObjects);

			if (this.owner.Worksheet != null) this.owner.Worksheet.RequestInvalidate();
		}

		public override bool Remove(IDrawingObject item)
		{
			bool ret = base.Remove(item);

			if (ret && this.owner.Worksheet != null) this.owner.Worksheet.RequestInvalidate();

			return ret;
		}

		public override void Clear()
		{
			base.Clear();

			if (this.owner.Worksheet != null) this.owner.Worksheet.RequestInvalidate();
		}

		public override IDrawingObject this[int index]
		{
			get
			{
				var ret = base[index];

				if (this.owner.Worksheet != null) this.owner.Worksheet.RequestInvalidate();

				return ret;
			}
		}
	}
}

#endif // DRAWING