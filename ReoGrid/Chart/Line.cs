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

using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.Chart
{
	/// <summary>
	/// Represents line chart component.
	/// </summary>
	public class LineChart : AxisChart
	{
		private LineChartPlotView lineChartPlotView;

		/// <summary>
		/// Get plot view object of line chart component.
		/// </summary>
		public LineChartPlotView LineChartPlotView
		{
			get { return this.lineChartPlotView; }
			protected set { this.lineChartPlotView = value; }
		}

		/// <summary>
		/// Create line chart component instance.
		/// </summary>
		public LineChart()
		{
			base.AddPlotViewLayer(this.lineChartPlotView = new LineChartPlotView(this));
		}

		/// <summary>
		/// Creates and returns line chart legend instance.
		/// </summary>
		/// <returns>Instance of line chart legend.</returns>
		protected override ChartLegend CreateChartLegend(LegendType type)
		{
			return new LineChartLegend(this);
		}
	}

	/// <summary>
	/// Represents line chart legend.
	/// </summary>
	public class LineChartLegend : ChartLegend
	{
		/// <summary>
		/// Create line chart legend.
		/// </summary>
		/// <param name="chart">Parent chart component.</param>
		public LineChartLegend(IChart chart)
			: base(chart)
		{
		}

		/// <summary>
		/// Get symbol size of chart legend.
		/// </summary>
		/// <param name="index">Index of serial in data source.</param>
		/// <returns>Symbol size of chart legend.</returns>
		protected override Size GetSymbolSize(int index)
		{
			return new Size(25, 3);
		}
	}

	/// <summary>
	/// Represents plot view object of line chart component.
	/// </summary>
	public class LineChartPlotView : ChartPlotView
	{
		/// <summary>
		/// Create line chart plot view object.
		/// </summary>
		/// <param name="chart">Parent chart component instance.</param>
		public LineChartPlotView(AxisChart chart)
			: base(chart)
		{
		}

		/// <summary>
		/// Render plot view of line chart component.
		/// </summary>
		/// <param name="dc">Platform no-associated drawing context.</param>
		protected override void OnPaint(DrawingContext dc)
		{
			base.OnPaint(dc);

			var axisChart = base.Chart as AxisChart;
			if (axisChart == null) return;

			var ai = axisChart.PrimaryAxisInfo;
			if (double.IsNaN(ai.Levels) || ai.Levels <= 0)
			{
				return;
			}

			//var bottomAxis = axisChart.BottomAxisInfo;
			var ds = Chart.DataSource;

			var g = dc.Graphics;

			var clientRect = this.ClientBounds;
			//var parentClientRect = axisChart.PlotViewContainer.ClientBounds;
			//var scaleX = clientRect.Width / parentClientRect.Width;
			//var scaleY = clientRect.Height / parentClientRect.Height;

			var columns = new Point[ds.CategoryCount];

			for (int r = 0; r < ds.SerialCount; r++)
			{
				var style = axisChart.DataSerialStyles[r];
				int lastIndex = 0;

				for (int c = 0; c < ds.CategoryCount; c++)
				{
					var pt = axisChart.PlotDataPoints[r][c];

					int count = c - lastIndex;

					if (pt.hasValue)
					{
						columns[c] = new Point(axisChart.PlotColumnPoints[c]/* * scaleX*/, axisChart.ZeroHeight - pt.value/* * scaleY*/);
					}

					bool last = c >= ds.CategoryCount - 1;

					if ((!pt.hasValue) || (last && count > 0))
					{
						if (count > 1 || (last && count > 0))
						{
							if (last) count++;

							g.DrawLines(columns, lastIndex, count, style.LineColor, style.LineWidth, style.LineStyle);
						}

						lastIndex = c + 1;
					}
				}
			}
		}
	}

}

#endif // DRAWING