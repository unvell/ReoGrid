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

#if WINFORM || ANDROID
using RGFloat = System.Single;
using RGIntDouble = System.Int32;

#elif WPF
using RGFloat = System.Double;
using RGIntDouble = System.Double;

#elif iOS
using RGFloat = System.Double;
using RGIntDouble = System.Double;

#endif

using System.Collections.Generic;
using System.Diagnostics;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Interaction;
using unvell.ReoGrid.Main;

namespace unvell.ReoGrid.Views
{

	/// <summary>
	/// A view is a visual region which can be independent rendered on ReoGrid control.
	/// A view can contains multiple child views.
	/// </summary>
	interface IView : IUserVisual
	{
		IViewportController ViewportController { get; set; }

		Rectangle Bounds { get; set; }
		RGFloat Left { get; }
		RGFloat Top { get; }
		RGFloat Width { get; }
		RGFloat Height { get; }
		RGFloat Right { get; }
		RGFloat Bottom { get; }

		void UpdateView();

		RGFloat ScaleFactor { get; set; }

		bool PerformTransform { get; set; }
		void Draw(CellDrawingContext dc);
		void DrawChildren(CellDrawingContext dc);
		bool Visible { get; set; }

		Point PointToView(Point p);
		Point PointToController(Point p);
		IView GetViewByPoint(Point p);

		IList<IView> Children { get; set; }
	}

}