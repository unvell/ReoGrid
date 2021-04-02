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

using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.Chart
{
	/// <summary>
	/// Represents line chart component.
	/// </summary>
	public class AreaChart : AxisChart
	{
		private AreaLineChartPlotView areaLineChartPlotView;

		/// <summary>
		/// Get plot view object of line chart component.
		/// </summary>
		public AreaLineChartPlotView AreaLineChartPlotView
		{
			get { return this.areaLineChartPlotView; }
			protected set { this.areaLineChartPlotView = value; }
		}

		/// <summary>
		/// Create line chart component instance.
		/// </summary>
		public AreaChart()
		{
			base.AddPlotViewLayer(this.areaLineChartPlotView = new AreaLineChartPlotView(this));
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

	public class AreaLineChartPlotView : LineChartPlotView
	{
		/// <summary>
		/// Create line chart plot view object.
		/// </summary>
		/// <param name="chart">Parent chart component instance.</param>
		public AreaLineChartPlotView(AxisChart chart)
			: base(chart)
		{
		}

		/// <summary>
		/// Render plot view region of line chart component.
		/// </summary>
		/// <param name="dc">Platform no-associated drawing context.</param>
		protected override void OnPaint(DrawingContext dc)
		{
			var axisChart = base.Chart as AxisChart;
			if (axisChart == null) return;

			var ds = Chart.DataSource;

			var g = dc.Graphics;
			var clientRect = this.ClientBounds;


#if WINFORM
			var path = new System.Drawing.Drawing2D.GraphicsPath();

			for (int r = 0; r < ds.SerialCount; r++)
			{
				var style = axisChart.DataSerialStyles[r];
				var lastPoint = new System.Drawing.PointF(axisChart.PlotColumnPoints[0], axisChart.ZeroHeight);

				for (int c = 0; c < ds.CategoryCount; c++)
				{
					var pt = axisChart.PlotDataPoints[r][c];

					System.Drawing.PointF point;

					if (pt.hasValue)
					{
						point = new System.Drawing.PointF(axisChart.PlotColumnPoints[c], axisChart.ZeroHeight - pt.value);
					}
					else
					{
						point = new System.Drawing.PointF(axisChart.PlotColumnPoints[c], axisChart.ZeroHeight);
					}

					path.AddLine(lastPoint, point);
					lastPoint = point;
				}

				var endPoint = new System.Drawing.PointF(axisChart.PlotColumnPoints[ds.CategoryCount - 1], axisChart.ZeroHeight);

				if (lastPoint != endPoint)
				{
					path.AddLine(lastPoint, endPoint);
				}

				path.CloseFigure();

				g.FillPath(style.FillColor, path);

				path.Reset();
			}

			path.Dispose();
#elif WPF


			for (int r = 0; r < ds.SerialCount; r++)
			{
				var style = axisChart.DataSerialStyles[r];

				var seg = new System.Windows.Media.PathFigure();

				seg.StartPoint = new System.Windows.Point(axisChart.PlotColumnPoints[0], axisChart.ZeroHeight);

				for (int c = 0; c < ds.CategoryCount; c++)
				{
					var pt = axisChart.PlotDataPoints[r][c];

					System.Windows.Point point;

					if (pt.hasValue)
					{
						point = new System.Windows.Point(axisChart.PlotColumnPoints[c], axisChart.ZeroHeight - pt.value);
					}
					else
					{
						point = new System.Windows.Point(axisChart.PlotColumnPoints[c], axisChart.ZeroHeight);
					}

					seg.Segments.Add(new System.Windows.Media.LineSegment(point, true));
				}

				var endPoint = new System.Windows.Point(axisChart.PlotColumnPoints[ds.CategoryCount - 1], axisChart.ZeroHeight);
				seg.Segments.Add(new System.Windows.Media.LineSegment(endPoint, true));

				seg.IsClosed = true;

				var path = new System.Windows.Media.PathGeometry();
				path.Figures.Add(seg);
				g.FillPath(style.LineColor, path);
			}

#endif // WPF
		}

	}
}

#endif // DRAWING
