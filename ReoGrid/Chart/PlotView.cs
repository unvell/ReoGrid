﻿/*****************************************************************************
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

#if DRAWING

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if WINFORM || ANDROID
using RGFloat = System.Single;

#elif WPF
using RGFloat = System.Double;

#endif // WPF

using unvell.ReoGrid.Data;
using unvell.ReoGrid.Drawing;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.Chart
{
	/// <summary>
	/// Chart Plot View 
	/// </summary>
	public interface IPlotView : IDrawingObject
	{
	}

	/// <summary>
	/// Represents common chart plot view.
	/// </summary>
	public class ChartPlotView : DrawingObject, IPlotView
	{
		/// <summary>
		/// Get or set the owner chart to this plot view.
		/// </summary>
		public Chart Chart { get; set; }

		/// <summary>
		/// Create common chart plot view object.
		/// </summary>
		/// <param name="chart">Owner chart instance.</param>
		public ChartPlotView(Chart chart)
		{
			this.Chart = chart;

			this.FillColor = SolidColor.Transparent;
			this.LineColor = SolidColor.Transparent;
		}
	}


}

#endif // DRAWING