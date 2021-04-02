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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace unvell.ReoGrid.Views
{
	internal abstract class LayerViewport : Viewport
	{
		public LayerViewport(IViewportController vc)
			: base(vc)
		{
		}

		public override IView GetViewByPoint(Graphics.Point p)
		{
			return base.GetChildrenByPoint(p);
		}

		public override void UpdateView()
		{
			if (this.children != null)
			{
				foreach (var child in this.children)
				{
					child.Bounds = this.bounds;
					child.ScaleFactor = this.scaleFactor;

					if (child is IViewport)
					{
						var viewport = (IViewport)child;
						viewport.ViewStart = this.viewStart;
						viewport.VisibleRegion = this.visibleRegion;
						viewport.ScrollableDirections = this.ScrollableDirections;
					}

					child.UpdateView();
				}
			}
		}
	}

	class SheetViewport : LayerViewport
	{
		public SheetViewport(IViewportController vc)
			: base(vc)
		{
			this.children = new List<IView>(4)
				{
					new CellsViewport(vc) { PerformTransform = false },

#if DRAWING
					new DrawingViewport(vc) { PerformTransform = false },
#if COMMENT
					new CommentViewport(vc) { PerformTransform = false },
#endif // COMMENT
#endif // DRAWING

				new CellsForegroundView(vc) { PerformTransform = false },
				};
		}
	}

}

namespace unvell.ReoGrid
{
	using unvell.ReoGrid.Views;

	partial class Worksheet
	{
		internal void InitViewportController()
		{
			this.viewportController = new NormalViewportController(this);
		}
	}
}