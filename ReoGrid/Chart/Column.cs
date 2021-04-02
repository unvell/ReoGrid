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

#if WINFORM || ANDROID
using RGFloat = System.Single;
#elif WPF
using RGFloat = System.Double;
#endif // WPF

using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.Chart
{
	/// <summary>
	/// Represents column chart component.
	/// </summary>
	public class ColumnChart : AxisChart
	{
		private ColumnChartPlotView chartPlotView;

		/// <summary>
		/// Get column chart plot view object.
		/// </summary>
		public ColumnChartPlotView ColumnChartPlotView
		{
			get { return this.chartPlotView; }
			protected set { this.chartPlotView = value; }
		}

		/// <summary>
		/// Create column chart instance.
		/// </summary>
		public ColumnChart()
		{
			base.AddPlotViewLayer(this.chartPlotView = CreateColumnChartPlotView());
		}

		/// <summary>
		/// Create and return the main plot view for this chart. 
		/// Derived classes specify their own plot view objects by overwriting this method.
		/// </summary>
		/// <returns>Plot view object for column-based charts.</returns>
		protected virtual ColumnChartPlotView CreateColumnChartPlotView()
		{
			return new ColumnChartPlotView(this);
    }

		/// <summary>
		/// Creates and returns column line chart legend instance.
		/// </summary>
		/// <returns>Instance of column line chart legend.</returns>
		protected override ChartLegend CreateChartLegend(LegendType type)
		{
			return new ColumnLineChartLegend(this);
		}
	}

	/// <summary>
	/// Represents column line chart legend.
	/// </summary>
	public class ColumnLineChartLegend : ChartLegend
	{
		/// <summary>
		/// Create column line chart legend.
		/// </summary>
		/// <param name="chart">Parent chart component.</param>
		internal ColumnLineChartLegend(IChart chart)
			: base(chart)
		{
		}

		/// <summary>
		/// Get default symbol size of chart legend.
		/// </summary>
		/// <returns>Symbol size of chart legend.</returns>
		protected override Size GetSymbolSize(int index)
		{
			return new Size(12, 12);
		}
	}

	/// <summary>
	/// Represents plot view object of column chart component.
	/// </summary>
	public class ColumnChartPlotView : ChartPlotView
	{
		/// <summary>
		/// Create column chart plot view object.
		/// </summary>
		/// <param name="chart">Owner chart instance.</param>
		public ColumnChartPlotView(AxisChart chart)
			: base(chart)
		{
		}

		/// <summary>
		/// Render the column chart plot view.
		/// </summary>
		/// <param name="dc">Platform no-associated drawing context instance.</param>
		protected override void OnPaint(DrawingContext dc)
		{
			base.OnPaint(dc);

			//var bottomAxis = axisChart.BottomAxisInfo;
			var clientRect = this.ClientBounds;
			var availableWidth = clientRect.Width * 0.7;

			if (availableWidth < 20)
			{
				return;
			}

			var axisChart = base.Chart as AxisChart;
			if (axisChart == null) return;

			var ds = Chart.DataSource;

			var rows = ds.SerialCount;
			var columns = ds.CategoryCount;

			var roundColumnWidth = availableWidth / columns;
			var roundColumnSpace = ((clientRect.Width - availableWidth) / (columns + 1));
			var singleColumnWidth = roundColumnWidth / rows;

			var ai = axisChart.PrimaryAxisInfo;

			double x = roundColumnSpace;

			var g = dc.Graphics;

			for (int c = 0; c < columns; c++)
			{
				for (int r = 0; r < ds.SerialCount; r++)
				{
					var pt = axisChart.PlotDataPoints[r][c];

					if (pt.hasValue)
					{
						var style = axisChart.DataSerialStyles[r];

						if (pt.value > 0)
						{
							g.DrawAndFillRectangle(new Rectangle(
									(RGFloat)x, axisChart.ZeroHeight - pt.value,
									(RGFloat)(singleColumnWidth - 1), pt.value), style.LineColor, style.FillColor);
						}
						else
						{
							g.DrawAndFillRectangle(new Rectangle(
								(RGFloat)x, axisChart.ZeroHeight,
								(RGFloat)(singleColumnWidth - 1), -pt.value), style.LineColor, style.FillColor);
						}
					}

					x += singleColumnWidth;
				}

				x += roundColumnSpace;
			}
		}
	}
}

#endif // DRAWING