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

#if DRAWING

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using unvell.ReoGrid.Rendering;
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

		/// <summary>
		/// Worksheet Drawing Canvas alwayas keep transparent and doesn't draw anything from itself
		/// </summary>
		/// <param name="dc">Platform no-associated drawing context instance.</param>
		protected override void OnPaint(DrawingContext dc)
		{
			dc.Graphics.IsAntialias = true;

			base.DrawChildren(dc);

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