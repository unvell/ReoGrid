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

#if WINFORM || ANDROID
using RGFloat = System.Single;
#else
using RGFloat = System.Double;
#endif // WINFORM

using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Drawing.Shapes;
using unvell.ReoGrid.Drawing;

namespace unvell.ReoGrid.Chart
{
	/// <summary>
	/// Repersents pie chart component.
	/// </summary>
	public class PieChart : Chart
	{
		internal virtual PieChartDataInfo DataInfo { get; private set; }
		internal virtual List<RGFloat> PlotPieAngles { get; private set; }
		internal virtual PiePlotView PiePlotView { get; private set; }

		/// <summary>
		/// Creates pie chart instance.
		/// </summary>
		public PieChart()
		{
			this.DataInfo = new PieChartDataInfo();
			this.PlotPieAngles = new List<RGFloat>();
			
			this.AddPlotViewLayer(this.PiePlotView = CreatePlotViewInstance());
		}

		#region Legend
		protected override ChartLegend CreateChartLegend(LegendType type)
		{
			var chartLegend = base.CreateChartLegend(type);

			if (type == LegendType.PrimaryLegend)
			{
				chartLegend.LegendPosition = LegendPosition.Bottom;
			}

			return chartLegend;
		}
		#endregion // Legend

		#region Plot view instance
		/// <summary>
		/// Creates and returns pie plot view.
		/// </summary>
		/// <returns></returns>
		protected virtual PiePlotView CreatePlotViewInstance()
		{
			return new PiePlotView(this);
		}
		#endregion // Plot view instance

		#region Layout
		protected override Rectangle GetPlotViewBounds(Rectangle bodyBounds)
		{
			RGFloat minSize = Math.Min(bodyBounds.Width, bodyBounds.Height);

			return new Rectangle(bodyBounds.X + (bodyBounds.Width - minSize) / 2, 
				bodyBounds.Y + (bodyBounds.Height - minSize) / 2,
				minSize, minSize);
		}
		#endregion // Layout

		#region Data Serials
		//public override int GetSerialCount()
		//{
		//	return this.DataSource.CategoryCount;
		//}
		//public override string GetSerialName(int index)
		//{
		//	return this.DataSource == null ? string.Empty : this.DataSource.GetCategoryName(index);
		//}
		#endregion // Data Serials

		#region Update Draw Points
		//protected override int GetSerialStyleCount()
		//{
		//	var ds = this.DataSource;
		//	return ds == null ? 0 : ds.CategoryCount;
		//}

		/// <summary>
		/// Update data serial information.
		/// </summary>
		protected override void UpdatePlotData()
		{
			var ds = this.DataSource;

			if (ds == null) return;

			double sum = 0;

			if (ds != null && ds.SerialCount > 0)
			{
				for (int index = 0; index < ds.SerialCount; index++)
				{
					double? data = ds[index][0];

					if (data != null)
					{
						sum += (double)data;
					}
				}
			}

			this.DataInfo.Total = sum;

			this.UpdatePlotPoints();
		}

		/// <summary>
		/// Update plot calculation points.
		/// </summary>
		protected virtual void UpdatePlotPoints()
		{
			var ds = this.DataSource;

			if (ds != null)
			{
				int dataCount = ds.SerialCount;

				var clientRect = this.ClientBounds;
				RGFloat scale = (RGFloat)(360.0 / this.DataInfo.Total);

				for (int i = 0; i < dataCount; i++)
				{
					var data = ds[i][0];

					if (data != null)
					{
						RGFloat angle = (RGFloat)(data * scale);

						if (i >= this.PlotPieAngles.Count)
						{
							this.PlotPieAngles.Add(angle);
						}
						else
						{
							this.PlotPieAngles[i] = angle;
						}
					}
				}
			}
			else
			{
				this.PlotPieAngles.Clear();
			}

			if (this.PiePlotView != null)
			{
				this.PiePlotView.Invalidate();
			}
		}
		#endregion // Update Draw Points
	}
	
	/// <summary>
	/// Represents pie chart data information.
	/// </summary>
	public class PieChartDataInfo
	{
		public double Total { get; set; }
	}

	/// <summary>
	/// Represents pie plot view.
	/// </summary>
	public class PiePlotView : ChartPlotView
	{
		/// <summary>
		/// Create plot view object of pie 2d chart.
		/// </summary>
		/// <param name="chart">Pie chart instance.</param>
		public PiePlotView(Chart chart)
			: base(chart)
		{
			this.Chart.DataSourceChanged += Chart_DataSourceChanged;
			this.Chart.ChartDataChanged += Chart_DataSourceChanged;
		}

		~PiePlotView()
		{
			this.Chart.DataSourceChanged -= Chart_DataSourceChanged;
			this.Chart.ChartDataChanged -= Chart_DataSourceChanged;
		}

		void Chart_DataSourceChanged(object sender, EventArgs e)
		{
			this.UpdatePieShapes();
		}

		protected List<PieShape> PlotPieShapes = new List<PieShape>();

		protected virtual void UpdatePieShapes()
		{
			var pieChart = this.Chart as PieChart;
			if (pieChart == null) return;

			var ds = this.Chart.DataSource;
			if (ds != null)
			{
				var dataCount = ds.SerialCount;
				RGFloat currentAngle = 0;

				for (int i = 0; i < dataCount && i < pieChart.PlotPieAngles.Count; i++)
				{
					RGFloat angle = pieChart.PlotPieAngles[i];

					if (i >= this.PlotPieShapes.Count)
					{
						this.PlotPieShapes.Add(CreatePieShape(this.ClientBounds));
					}

					var pie = this.PlotPieShapes[i];
					pie.StartAngle = currentAngle;
					pie.SweepAngle = angle;
					pie.FillColor = pieChart.DataSerialStyles[i].FillColor;

					currentAngle += angle;
				}
			}
		}

		protected virtual PieShape CreatePieShape(Rectangle bounds)
		{
			return new PieShape()
			{
				Bounds = bounds,
				LineColor = SolidColor.Transparent,
			};
		}

		/// <summary>
		/// Render pie 2d plot view.
		/// </summary>
		/// <param name="dc">Platform no-associated drawing context instance.</param>
		protected override void OnPaint(Rendering.DrawingContext dc)
		{
			base.OnPaint(dc);

			foreach (var pieShape in this.PlotPieShapes)
			{
				pieShape.Draw(dc);
			}
		}
	}

	/// <summary>
	/// Repersents pie 2D chart component.
	/// </summary>
	public class Pie2DChart : PieChart
	{
	}

	/// <summary>
	/// Represents pie 2D plot view.
	/// </summary>
	public class Pie2DPlotView : PiePlotView
	{
		public Pie2DPlotView(Pie2DChart pieChart)
			: base(pieChart)
		{
		}
	}

	/// <summary>
	/// Repersents doughnut chart component.
	/// </summary>
	public class DoughnutChart : PieChart
	{
		/// <summary>
		/// Creates and returns doughnut plot view.
		/// </summary>
		protected override PiePlotView CreatePlotViewInstance()
		{
			return new DoughnutPlotView(this);
		}
	}

	/// <summary>
	/// Represents doughnut plot view.
	/// </summary>
	public class DoughnutPlotView : PiePlotView
	{
		public DoughnutPlotView(DoughnutChart chart)
			: base(chart)
		{
		}

		protected override PieShape CreatePieShape(Rectangle bounds)
		{
			return new Drawing.Shapes.SmartShapes.BlockArcShape
			{
				Bounds = bounds,
				LineColor = SolidColor.White,
			};
		}
	}
}

#endif // DRAWING