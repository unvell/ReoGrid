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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace unvell.ReoGrid.Views
{
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