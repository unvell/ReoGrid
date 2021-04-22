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
#elif WPF
using RGFloat = System.Double;
#endif // WPF

using unvell.ReoGrid.Drawing;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Interaction;

namespace unvell.ReoGrid.Chart
{
	/// <summary>
	/// Represents chart drawing component.
	/// </summary>
	public abstract class Chart : DrawingComponent, IChart//, ILegendSupportedChart
	{
		/// <summary>
		/// Get or set the title string object.
		/// </summary>
		public virtual IDrawingObject TitleView { get; set; }

		/// <summary>
		/// Get or set the title of chart.
		/// </summary>
		public virtual string Title { get; set; }

		#region Constructor
		/// <summary>
		/// Create chart instance.
		/// </summary>
		protected Chart()
		{
			this.Title = "Chart";

			// border line color
			this.LineColor = SolidColor.Silver;
			this.Padding = new PaddingValue(10);

			// body
			this.Children.Add(this.PlotViewContainer = new DrawingComponent()
			{
				FillColor = SolidColor.Transparent,
				LineColor = SolidColor.Transparent,
			});

			// title
			this.Children.Add(this.TitleView = new ChartTitle(this));

			// legend
			this.Children.Add(this.PrimaryLegend = CreateChartLegend(LegendType.PrimaryLegend));
		}
		#endregion // Constructor

		#region Layout

		public override Size GetPreferredSize()
		{
			return new Size(400, 260);
		}

		/// <summary>
		/// Relayout this view.
		/// </summary>
		/// <param name="oldRect">Bounds rectangle before changed.</param>
		public override void OnBoundsChanged(Rectangle oldRect)
		{
			base.OnBoundsChanged(oldRect);

			this.layoutDirty = true;
		}

		private bool layoutDirty = true;

		internal void DirtyLayout()
		{
			this.layoutDirty = true;
		}

		/// <summary>
		/// Update children view bounds.
		/// </summary>
		protected virtual void UpdateLayout()
		{
			var clientRect = this.ClientBounds;

			const RGFloat titlePlotSpacing = 20;
			const RGFloat plotLegendSpacing = 10;

			var titleBounds = GetTitleBounds();

			var bodyBounds = new Rectangle(clientRect.X, titleBounds.Bottom + titlePlotSpacing,
				clientRect.Width, clientRect.Height - titleBounds.Bottom - titlePlotSpacing);

			if (this.showLegend)
			{
				var legendSize = Size.Zero;

				if (this.PrimaryLegend != null)
				{
					this.PrimaryLegend.MeasureSize(clientRect);

					this.PrimaryLegend.Bounds = this.GetLegendBounds(bodyBounds, LegendType.PrimaryLegend, this.PrimaryLegend.LegendPosition);
					legendSize = this.PrimaryLegend.Size;

					switch (this.PrimaryLegend.LegendPosition)
					{
						case LegendPosition.Left:
							bodyBounds.X += legendSize.Width + plotLegendSpacing;
							break;

						default:
						case LegendPosition.Right:
							bodyBounds.Width -= legendSize.Width + plotLegendSpacing;
							break;

						case LegendPosition.Top:
							bodyBounds.Y += legendSize.Height + plotLegendSpacing;
							break;

						case LegendPosition.Bottom:
							bodyBounds.Height -= legendSize.Height + plotLegendSpacing;
							break;
					}
				}
			}

			if (this.PlotViewContainer != null)
			{
				this.PlotViewContainer.Bounds = this.GetPlotViewBounds(bodyBounds);
			}

			if (this.TitleView != null) this.TitleView.Bounds = titleBounds;

			this.layoutDirty = false;
		}

			/// <summary>
			/// Get default title bounds.
			/// </summary>
			/// <returns>Rectangle of title bounds.</returns>
		protected virtual Rectangle GetTitleBounds()
		{
			var titleRect = this.ClientBounds;
			titleRect.Height = 30;
			return titleRect;
		}

		/// <summary>
		/// Get default body bounds.
		/// </summary>
		/// <returns>Rectangle of body bounds.</returns>
		protected virtual Rectangle GetPlotViewBounds(Rectangle bodyBounds)
		{
			return bodyBounds;
		}

		/// <summary>
		/// Get legend view bounds.
		/// </summary>
		/// <returns></returns>
		protected virtual Rectangle GetLegendBounds(Rectangle plotViewBounds, LegendType type, LegendPosition position)
		{
			if (!this.showLegend)
			{
				return new Rectangle(0, 0, 0, 0);
			}

			var clientRect = this.ClientBounds;

			Size legendSize = Size.Zero;

			switch (type)
			{
				default:
				case LegendType.PrimaryLegend:
					legendSize = this.PrimaryLegend.GetPreferredSize();
					break;
			}

			switch (position)
			{
				case LegendPosition.Left:
					return new Rectangle(0,
						plotViewBounds.Y + (plotViewBounds.Height - legendSize.Height) / 2,
							legendSize.Width, legendSize.Height);

				default:
				case LegendPosition.Right:
					return new Rectangle(clientRect.Right - legendSize.Width,
						plotViewBounds.Y + (plotViewBounds.Height - legendSize.Height) / 2,
							legendSize.Width, legendSize.Height);

				case LegendPosition.Top:
					return new Rectangle(plotViewBounds.X + (plotViewBounds.Width - legendSize.Width) / 2,
						0, legendSize.Width, legendSize.Height);

				case LegendPosition.Bottom:
					return new Rectangle(plotViewBounds.X + (plotViewBounds.Width - legendSize.Width) / 2, 
						plotViewBounds.Bottom - legendSize.Height, legendSize.Width, legendSize.Height);
			}
		}
		#endregion // Layout

		#region Paint
		protected override void OnPaint(DrawingContext dc)
		{
			if (this.layoutDirty)
			{
				this.UpdateLayout();
			}

			base.OnPaint(dc);
		}
		#endregion // Paint

		#region Data Source
		///// <summary>
		///// Specifies that whether or not allow to swap the row and column from specified data range.
		///// </summary>
		//public virtual bool SwapDataRowColumn { get; set; }

		private WorksheetChartDataSource dataSource;

		/// <summary>
		/// Get or set chart data source.
		/// </summary>
		public virtual WorksheetChartDataSource DataSource
		{
			get { return this.dataSource; }
			set
			{
				if (this.dataSource != null)
				{
					this.dataSource.DataChanged -= dataSource_DataChanged;
					//this.dataSource.DataRangeChanged -= dataSource_DataRangeChanged;
				}

				this.dataSource = value;

				if (this.dataSource != null)
				{
					this.dataSource.DataChanged += dataSource_DataChanged;
					//this.dataSource.DataRangeChanged += dataSource_DataRangeChanged;
				}

				this.ResetDataSerialStyles();
				OnDataSourceChanged();
			}
		}

		void dataSource_DataChanged(object sender, EventArgs e)
		{
			this.OnDataChanged();
		}

		//void dataSource_DataRangeChanged(object sender, EventArgs e)
		//{
		//	this.OnDataSourceChanged();
		//}

		/// <summary>
		/// This method will be invoked when data from data source is changed.
		/// </summary>
		protected virtual void OnDataChanged()
		{
			this.UpdatePlotData();

			this.UpdateLayout();

			if (this.ChartDataChanged != null)
			{
				this.ChartDataChanged(this, null);
			}
		}

		/// <summary>
		/// This event will be invoked when chart data from data source is changed.
		/// </summary>
		public event EventHandler ChartDataChanged;

		/// <summary>
		/// This method will be invoked when chart data source is changed.
		/// </summary>
		protected virtual void OnDataSourceChanged()
		{
			this.UpdatePlotData();

			this.UpdateLayout();

			if (this.DataSourceChanged != null)
			{
				this.DataSourceChanged(this, null);
			}
		}

		/// <summary>
		/// This event will be invoked when chart data source is changed.
		/// </summary>
		public event EventHandler DataSourceChanged;

		/// <summary>
		/// Update chart when data source or data range has been changed.
		/// </summary>
		protected virtual void UpdatePlotData()
		{
			// empty
		}

		#endregion // Data Source

		#region Plot View Children
		/// <summary>
		/// Get the chart plot view component.
		/// </summary>
		public virtual IDrawingContainer PlotViewContainer
		{ get; private set; }

		/// <summary>
		/// Add chart plot view object.
		/// </summary>
		/// <param name="view">Chart plot view object.</param>
		protected virtual void AddPlotViewLayer(IPlotView view)
		{
			if (this.PlotViewContainer != null)
			{
				view.Bounds = this.PlotViewContainer.ClientBounds;
				this.PlotViewContainer.Children.Add(view);
			}

			this.Invalidate();
		}
		#endregion // Children Body

		#region Legend

		private bool showLegend = true;

		/// <summary>
		/// Get or set whether or not to show legend view.
		/// </summary>
		public bool ShowLegend
		{
			get { return this.showLegend; }
			set
			{
				if (this.showLegend != value)
				{
					this.showLegend = value;

					if (this.PrimaryLegend != null)
					{
						this.PrimaryLegend.Visible = value;
					}

					this.DirtyLayout();
				}
			}
		}

		/// <summary>
		/// Get or set the primary legend object.
		/// </summary>
	public virtual ChartLegend PrimaryLegend
		{
			get;
			set;
		}

		//private LegendPosition primaryLegendPosition;

		//public LegendPosition PrimaryLegendPosition
		//{
		//	get
		//	{
		//		return this.primaryLegendPosition;
		//	}
		//	set
		//	{
		//		if (this.primaryLegendPosition != value)
		//		{
		//			this.primaryLegendPosition = value;

		//			this.UpdateChart();
		//			this.Invalidate();
		//		}
		//	}
		//}

		/// <summary>
		/// Create chart legend.
		/// </summary>
		/// <returns>Instance of chart legend.</returns>
		protected virtual ChartLegend CreateChartLegend(LegendType type)
		{
			return new ChartLegend(this);
		}

		#endregion // Legend Symbol

		#region Data Serial
		internal List<DataSerialStyle> serialStyles = new List<DataSerialStyle>();

		/// <summary>
		/// Reset data serial to row default styles.
		/// </summary>
		protected virtual void ResetDataSerialStyles()
		{
			var ds = this.dataSource;
			if (ds == null) return;

			int dataSerialCount = this.dataSource.SerialCount;

			if (this.dataSource == null || dataSerialCount <= 0)
			{
				this.serialStyles.Clear();
			}

			while (this.serialStyles.Count < dataSerialCount)
			{
				this.serialStyles.Add(new DataSerialStyle(this)
				{
					FillColor = ChartUtility.GetDefaultDataSerialFillColor(this.serialStyles.Count),
					LineColor = ChartUtility.GetDefaultDataSerialFillColor(this.serialStyles.Count),
					LineWidth = 2f,
				});
			}
		}

		///// <summary>
		///// Get the number of serial styles that is used for this chart.
		///// </summary>
		///// <returns>Number of serial styles.</returns>
		//protected virtual int GetSerialStyleCount()
		//{
		//	return this.dataSource.SerialCount;
		//}

		private DataSerialStyleCollection dataSerialStyleCollection = null;

		/// <summary>
		/// Get data serial styles.
		/// </summary>
		public virtual DataSerialStyleCollection DataSerialStyles
		{
			get
			{
				if (this.dataSerialStyleCollection == null)
				{
					this.dataSerialStyleCollection = new DataSerialStyleCollection(this);
				}

				return this.dataSerialStyleCollection;
			}
		}

		///// <summary>
		///// Get number of data serials from data source.
		///// </summary>
		///// <returns>Number of data serials.</returns>
		//public virtual int GetSerialCount()
		//{
		//	return this.DataSource == null ? 0 : this.DataSource.SerialCount;
		//}

		///// <summary>
		///// Get name of specified data serial.
		///// </summary>
		///// <param name="index">Zero-based number of data serial to get name.</param>
		///// <returns>Name in string of specified data serial.</returns>
		//public virtual string GetSerialName(int index)
		//{
		//	return this.DataSource == null ? string.Empty : this.DataSource[index].Label;
		//}

		#endregion // Data Serial

		#region Mouse
		/// <summary>
		/// Handles the mouse down event.
		/// </summary>
		/// <param name="location">Relative location of mouse button press-down.</param>
		/// <param name="button">Determines that which mouse button is pressed down.</param>
		/// <returns>True if event has been handled; Otherwise false.</returns>
		public override bool OnMouseDown(Point location, MouseButtons button)
		{
			return base.OnMouseDown(location, button);
		}
		#endregion // Mouse
	}

	/// <summary>
	/// Represents axis-based chart component. 
	/// This is an abstract class that should be implemented by other axis-based chart classes.
	/// </summary>
	public abstract class AxisChart : Chart
	{
		#region Attributes
		/// <summary>
		/// Get or set the primary axis information set.
		/// </summary>
		public virtual AxisDataInfo PrimaryAxisInfo
		{ get; set; }

		/// <summary>
		/// Get or set the secondary axis information set.
		/// </summary>
		public virtual AxisDataInfo SecondaryAxisInfo
		{ get; set; }

		/// <summary>
		/// Get or set the primary axis view object.
		/// </summary>
		public virtual AxisInfoView HorizontalAxisInfoView
		{ get; set; }

		/// <summary>
		/// Get or set the data label view object.
		/// </summary>
		public virtual AxisInfoView VerticalAxisInfoView
		{ get; set; }

		/// <summary>
		/// Get or set the grid line background view object.
		/// </summary>
		public virtual AxisGuideLinePlotView GuideLineBackgroundView
		{ get; set; }

		/// <summary>
		/// Specifies that whether or not allow to display the horizontal guide lines.
		/// </summary>
		public virtual bool ShowHorizontalGuideLines { get; set; }

		/// <summary>
		/// Specifies that whether or not allow to display the vertical guide lines.
		/// </summary>
		public virtual bool ShowVerticalGuideLines { get; set; }
		#endregion // Attributes

		#region Constructor

		/// <summary>
		/// Create axis-based chart instance.
		/// </summary>
		public AxisChart()
		{
			this.ShowHorizontalGuideLines = true;
			this.ShowVerticalGuideLines = false;

			this.PrimaryAxisInfo = new AxisDataInfo();
			this.SecondaryAxisInfo = null;// = new AxisRulerInfo();

			var bodyBounds = this.PlotViewContainer.Bounds;
			var clientRect = this.ClientBounds;

			base.AddPlotViewLayer(this.GuideLineBackgroundView = this.CreateGuideLineBackgroundView(bodyBounds));
			this.Children.Add(this.HorizontalAxisInfoView = this.CreatePrimaryAxisSerialLabelView(bodyBounds));
			this.Children.Add(this.VerticalAxisInfoView = this.CreatePrimaryAxisCategoryLabelView(bodyBounds));
		}

		protected virtual AxisGuideLinePlotView CreateGuideLineBackgroundView(Rectangle bodyBounds)
		{
			return new AxisGuideLinePlotView(this)
			{
				Size = new Size(bodyBounds.Width, bodyBounds.Height),
			};
		}

		protected virtual AxisInfoView CreatePrimaryAxisSerialLabelView(Rectangle bodyBounds)
		{
			return new AxisSerialLabelView(this, AxisTypes.Primary)
			{
				Bounds = GetDefaultHorizontalAxisInfoViewBounds(bodyBounds),
			};
		}

		protected virtual Rectangle GetDefaultHorizontalAxisInfoViewBounds(Rectangle bodyBounds)
		{
			return new Rectangle(bodyBounds.X, bodyBounds.Bottom + 6, bodyBounds.Width, 24);
		}

		/// <summary>
		/// Create default primary axis category label view.
		/// </summary>
		/// <param name="bodyBounds"></param>
		/// <returns></returns>
		protected virtual AxisInfoView CreatePrimaryAxisCategoryLabelView(Rectangle bodyBounds)
		{
			return new AxisCategoryLabelView(this, AxisTypes.Primary)
			{
				Bounds = GetDefaultVerticalAxisInfoViewBounds(bodyBounds),
			};
		}

		/// <summary>
		/// Return default vertical axis bounds.
		/// </summary>
		/// <param name="bodyBounds">Bounds of chart body.</param>
		/// <returns>Vertical axis bounds rectangle.</returns>
		protected virtual Rectangle GetDefaultVerticalAxisInfoViewBounds(Rectangle bodyBounds)
		{
			return new Rectangle(bodyBounds.X - 35, bodyBounds.Y - 5, 30, bodyBounds.Height + 10);
		}
		
		#endregion // Constructor

		#region Data Changes

		/// <summary>
		/// This method will be invoked when data source of this chart is changed.
		/// </summary>
		protected override void OnDataSourceChanged()
		{
			base.OnDataSourceChanged();

			this.ResetDrawPoints();
			this.UpdateDrawPoints();
		}

		/// <summary>
		/// This method will be invoked when data from the data source of this chart is changed.
		/// </summary>
		protected override void OnDataChanged()
		{
			base.OnDataChanged();

			this.UpdateDrawPoints();
		}

		#endregion // Data Changes

		#region Update Draw Points

		/// <summary>
		/// Update chart data information.
		/// </summary>
		protected override void UpdatePlotData()
		{
			var ds = this.DataSource;
			if (ds == null) return;

			double minData = 0;
			double maxData = 0;
			bool first = true;

			for (int r = 0; r < ds.SerialCount; r++)
			{
				for (int c = 0; c < ds.CategoryCount; c++)
				{
					double? data = ds[r][c];

					if (data != null)
					{
						if (first)
						{
							minData = (double)data;
							maxData = minData;

							first = false;
						}
						else
						{
							if (minData > data) minData = (double)data;
							if (maxData < data) maxData = (double)data;
						}
					}
				}
			}

			//var ai = !SwapDataRowColumn ? PrimaryAxisInfo : SecondaryAxisInfo;
			this.UpdateAxisInfo(this.PrimaryAxisInfo, minData, maxData);
		}

		/// <summary>
		/// Update specified axis information.
		/// </summary>
		/// <param name="ai">Axis information set.</param>
		/// <param name="minData">Minimum value scanned from data range.</param>
		/// <param name="maxData">Maximum value scanned from data range.</param>
		protected virtual void UpdateAxisInfo(AxisDataInfo ai, double minData, double maxData)
		{
			var clientRect = this.PlotViewContainer;

			double range = maxData - minData;

			ai.Levels = (int)Math.Ceiling(clientRect.Height / 30f);

			// when clientRect is zero, nothing to do
			if (double.IsNaN(ai.Levels))
			{
				return;
			}

			if (minData == maxData)
			{
				if (maxData == 0)
					maxData = ai.Levels;
				else
					minData = 0;
			}

			double stride = ChartUtility.CalcLevelStride(minData, maxData, ai.Levels, out var scaler);
			ai.Scaler = scaler;

			double m;

			if (!ai.AutoMinimum)
			{
				if (this.AxisOriginToZero(minData, maxData, range))
				{
					ai.Minimum = 0;
				}
				else
				{
					m = minData % stride;
					if (m == 0)
					{
						if (minData == 0)
						{
							ai.Minimum = minData;
						}
						else
						{
							ai.Minimum = minData - stride;
						}
					}
					else
					{
						if (minData < 0)
						{
							ai.Minimum = minData - stride - m;
						}
						else
						{
							ai.Minimum = minData - m;
						}
					}
				}
			}

			if (!ai.AutoMaximum)
			{
				m = maxData % stride;
				if (m == 0)
				{
					ai.Maximum = maxData + stride;
				}
				else
				{
					ai.Maximum = maxData - m + stride;
				}
			}

			ai.Levels = (int)Math.Round((ai.Maximum - ai.Minimum) / stride);

			ai.LargeStride = stride;
		}

		/// <summary>
		/// Measure axis ruler information.
		/// </summary>
		/// <param name="info">Specified axis data information set.</param>
		/// <param name="data">Data to be measured.</param>
		protected virtual void MeasureAxisRuler(AxisDataInfo info, double data)
		{
			if (info.Minimum > data) info.Minimum = data;
			if (info.Maximum < data) info.Maximum = data;

			info.LargeStride = (info.Maximum - info.Minimum) / 5;
		}

		/// <summary>
		/// Determines that whether or not allow to set axis minimum value to a non-zero position automatically.
		/// </summary>
		/// <param name="minData">Minimum data scanned from data source.</param>
		/// <param name="maxData">Maximum data scanned from data source.</param>
		/// <param name="range">Data range.</param>
		/// <returns>True to set axis minimum value; Otherwise false.</returns>
		protected virtual bool AxisOriginToZero(double minData, double maxData, double range)
		{
			return (maxData > 0 && minData > 0
					|| maxData < 0 && minData < 0)
					&& Math.Abs(minData) < range;
		}

		/// <summary>
		/// Get the origin value of width related to this view object.
		/// </summary>
		public RGFloat ZeroWidth { get; protected set; }

		/// <summary>
		/// Get the origin value of height related to this view object.
		/// </summary>
		public RGFloat ZeroHeight { get; protected set; }

		private PlotPointRow[] platRowPoints = null;
		internal PlotPointRow[] PlotDataPoints { get { return this.platRowPoints; } }

		private RGFloat[] platColPoints = null;
		internal RGFloat[] PlotColumnPoints { get { return this.platColPoints; } }

		/// <summary>
		/// Reset plot drawing points.
		/// </summary>
		protected virtual void ResetDrawPoints()
		{
			var ds = this.DataSource;

			if (ds != null)
			{
				#region Row

				if (this.platRowPoints == null)
				{
					this.platRowPoints = new PlotPointRow[ds.SerialCount];
				}
				else
				{
					if (this.platRowPoints.Length != ds.SerialCount)
					{
						Array.Resize(ref this.platColPoints, ds.SerialCount);
					}
				}

				for (int r = 0; r < ds.SerialCount; r++)
				{
					this.platRowPoints[r].columns = new PlotPointColumn[ds.CategoryCount];
				}

				#endregion // Row

				#region Columns

				if (this.platColPoints == null)
				{
					this.platColPoints = new RGFloat[ds.CategoryCount];
				}
				else
				{
					if (this.platColPoints.Length != ds.CategoryCount)
					{
						Array.Resize(ref this.platColPoints, ds.CategoryCount);
					}

					for (int i = 0; i < ds.CategoryCount; i++)
					{
						this.platColPoints[i] = 0;
					}
				}

				#endregion // Columns
			}
			else
			{
				#region Reset
				for (int r = 0; r < this.platRowPoints.Length; r++)
				{
					var row = this.platRowPoints[r];

					for (int c = 0; c < row.Length; c++)
					{
						this.platRowPoints[r][c] = 0;
					}
				}

				for (int i = 0; i < this.platColPoints.Length; i++)
				{
					this.platColPoints[i] = 0;
				}
				#endregion // Reset
			}
		}

		/// <summary>
		/// Update plot drawing points.
		/// </summary>
		protected virtual void UpdateDrawPoints()
		{
			var ds = this.DataSource;

			if (ds != null)
			{
				//var ai = !this.SwapDataRowColumn ? this.PrimaryAxisInfo : this.SecondaryAxisInfo;
				var ai = this.PrimaryAxisInfo;

				if (!double.IsNaN(ai.Levels))
				{
					int serialCount = ds.SerialCount;
					int categoryCount = ds.CategoryCount;

					var total = (ai.Maximum - ai.Minimum);

					var clientSize = this.PlotViewContainer.Size;

					RGFloat scaleX = clientSize.Width / categoryCount;
					double scaleY = clientSize.Height / total;

					this.ZeroHeight = (RGFloat)(clientSize.Height + clientSize.Height * ai.Minimum / total);
					this.ZeroWidth = (RGFloat)(clientSize.Width * ai.Minimum / total);

					var colHalf = clientSize.Width / categoryCount / 2;

					for (int r = 0; r < serialCount; r++)
					{
						var serial = ds.GetSerial(r);

						for (int c = 0; c < categoryCount; c++)
						{
							if (r == 0)
							{
								this.platColPoints[c] = colHalf + (float)c * scaleX;
							}

							var data = serial[c];

							if (data == null)
							{
								this.platRowPoints[r][c] = PlotPointColumn.Nil;
							}
							else
							{
								this.platRowPoints[r][c] = (RGFloat)((data) * scaleY);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// This method will be invoked when chart bounds changed.
		/// </summary>
		/// <param name="oldRect">Old bounds rectangle.</param>
		public override void OnBoundsChanged(Rectangle oldRect)
		{
			base.OnBoundsChanged(oldRect);

			this.UpdatePlotData();
			this.UpdateDrawPoints();

			//if (this.PlotViewContainer != null)
			//{
			//	foreach (var child in this.PlotViewContainer.Children)
			//	{
			//		child.Bounds = this.PlotViewContainer.Bounds;
			//	}
			//}
		}

		#endregion // Update Draw Points

		#region Layout

		/// <summary>
		/// Update all children bounds.
		/// </summary>
		protected override void UpdateLayout()
		{
			base.UpdateLayout();

			//if (this.PlotViewContainer != null)
			//{
			//	var bodyBounds = this.PlotViewContainer.Bounds;

			//	if (this.HorizontalAxisInfoView != null)
			//	{
			//		this.HorizontalAxisInfoView.Bounds = GetDefaultHorizontalAxisInfoViewBounds(bodyBounds);
			//	}

			//	this.UpdateChart();
			//	this.UpdateDrawPoints();
			//}

			this.GuideLineBackgroundView.Bounds = this.PlotViewContainer.ClientBounds;

			UpdateAxisLabelViewLayout(this.PlotViewContainer.Bounds);
		}

		protected virtual void UpdateAxisLabelViewLayout(Rectangle plotRect)
		{
			const RGFloat spacing = 10;

			this.VerticalAxisInfoView.Bounds = new Rectangle(this.ClientBounds.X, plotRect.Y - 5, 30, plotRect.Height + 10);
			this.HorizontalAxisInfoView.Bounds = new Rectangle(plotRect.X, plotRect.Bottom + spacing, plotRect.Width, 10);
		}

		protected override Rectangle GetPlotViewBounds(Rectangle bodyBounds)
		{
			var rect = base.GetPlotViewBounds(bodyBounds);

			const RGFloat spacing = 10;

			return new Rectangle(rect.X + 30 + spacing, rect.Y, rect.Width - 30 - spacing, rect.Height - 10);
		}

		#endregion // Layout
	}

}

#endif // DRAWING