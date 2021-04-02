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

using unvell.ReoGrid.Drawing;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.Chart
{
	internal class ChartTitle : DrawingObject
	{
		public IChart Chart { get; set; }

		public ChartTitle(IChart chart)
		{
			this.Chart = chart;

			//this.ForeColor = SolidColor.Transparent;
			this.FontSize += 5.0F;
		}

		/// <summary>
		/// Render chart title view.
		/// </summary>
		/// <param name="dc">Platform no-associated drawing context instance.</param>
		protected override void OnPaint(DrawingContext dc)
		{
			//base.OnPaint(dc);

			var g = dc.Graphics;

			g.DrawText(Chart.Title, this.FontName, this.FontSize,
				unvell.ReoGrid.Rendering.StaticResources.SystemColor_WindowText, this.ClientBounds,
				ReoGridHorAlign.Center, ReoGridVerAlign.Middle);
		}
	}
}

#endif // DRAWING