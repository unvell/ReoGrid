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
 * Copyright (c) 2012-2023 Jingwood <jingwood at unvell.com>
 * Copyright (c) 2012-2023 unvell inc. All rights reserved.
 * 
 ****************************************************************************/

#if COMMENT

using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.Views
{
	internal class CommentViewport : Viewport
	{
		public CommentViewport(IViewportController vc)
			: base(vc)
		{
		}

		public override void DrawView(CellDrawingContext dc)
		{
			if (this.sheet.cellComments != null)
			{
				foreach (var comment in this.sheet.cellComments.Values)
				{
					
				}
			}
		}

		public override IView GetViewByPoint(Graphics.Point p)
		{
			return GetChildrenByPoint(p);
		}
	}
}

#endif // COMMENT