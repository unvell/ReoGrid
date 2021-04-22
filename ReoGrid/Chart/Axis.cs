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
#else
using RGFloat = System.Double;
#endif // WINFORM

using unvell.ReoGrid.Data;
using unvell.ReoGrid.Drawing;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.Chart
{
	/// <summary>
	/// Axis data information for axis-based chart.
	/// </summary>
	public class AxisDataInfo
	{
		/// <summary>
		/// Get or set the plot vertial levels.
		/// </summary>
		public int Levels { get; set; }

		/// <summary>
		/// Get or set axis scaler.
		/// </summary>
		public int Scaler { get; set; }

		/// <summary>
		/// Get or set axis minimum value.
		/// </summary>
		public double Minimum { get; set; }

		/// <summary>
		/// Get or set axis maximum value.
		/// </summary>
		public double Maximum { get; set; }

		/// <summary>
		/// Specifies that whether or not to decide the axis minimum value automatically by scanning data.
		/// </summary>
		public bool AutoMinimum { get; set; }

		/// <summary>
		/// Specifies that whether or not to decide the axis maximum value automatically by scanning data.
		/// </summary>
		public bool AutoMaximum { get; set; }

		/// <summary>
		/// Get or set the axis large stride value.
		/// </summary>
		public double LargeStride { get; set; }

		/// <summary>
		/// Get or set the axis small stride value.
		/// </summary>
		public double SmallStride { get; set; }

	}

	/// <summary>
	/// Axis Types
	/// </summary>
	public enum AxisTypes
	{
		/// <summary>
		/// Primary axis
		/// </summary>
		Primary,
		
		/// <summary>
		/// Secondary axis
		/// </summary>
		Secondary,
	}

	/// <summary>
	/// Axis Orientations
	/// </summary>
	public enum AxisOrientation
	{
		/// <summary>
		/// Horizontal
		/// </summary>
		Horizontal,

		/// <summary>
		/// Vertical
		/// </summary>
		Vertical,
	}

	internal struct PlotPointRow : IEnumerable<PlotPointColumn>
	{
		public PlotPointColumn[] columns;

		public PlotPointColumn this[int index]
		{
			get { return columns[index]; }
			set { columns[index] = value; }
		}

		public int Length
		{
			get { return this.columns.Length; }
		}

		public IEnumerator<PlotPointColumn> GetEnumerator()
		{
			foreach (var col in this.columns)
			{
				yield return col;
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.columns.GetEnumerator();
		}
	}

	internal struct PlotPointColumn
	{
		public bool hasValue;
		public RGFloat value;

		public static readonly PlotPointColumn Nil = new PlotPointColumn { hasValue = false, value = 0 };

		public static implicit operator PlotPointColumn(RGFloat value)
		{
			return new PlotPointColumn { hasValue = true, value = value };
		}
	}

	#region On-Axis Views
	public abstract class AxisInfoView : DrawingObject
	{
		public AxisChart Chart { get; protected set; }

		public AxisTypes AxisType { get; protected set; }

		public AxisInfoView(AxisChart chart, AxisTypes axisType)
		{
			this.Chart = chart;
			this.AxisType = axisType;

			this.FillColor = SolidColor.Transparent;
			this.LineColor = SolidColor.Transparent;
			this.FontSize *= 0.9f;
		}

		public AxisInfoView(AxisChart chart)
			: this(chart, AxisTypes.Primary)
		{
		}
	}

	public class AxisCategoryLabelView : AxisInfoView
	{
		private AxisOrientation orientation;

		public AxisCategoryLabelView(AxisChart chart, AxisTypes axisType = AxisTypes.Primary, AxisOrientation orientation = AxisOrientation.Vertical)
			: base(chart, axisType)
		{
			this.orientation = orientation;
		}

		/// <summary>
		/// Render axis information view.
		/// </summary>
		/// <param name="dc">Platform no-associated drawing context instance.</param>
		protected override void OnPaint(DrawingContext dc)
		{
			base.OnPaint(dc);

			if (this.Chart == null) return;

			var ai = this.AxisType == AxisTypes.Primary ?
				this.Chart.PrimaryAxisInfo : this.Chart.SecondaryAxisInfo;

			if (ai == null) return;

			var g = dc.Graphics;

			var ds = this.Chart.DataSource;
			var clientRect = this.ClientBounds;

			RGFloat fontHeight = (RGFloat)(this.FontSize * PlatformUtility.GetDPI() / 72.0) + 4;

			double rowValue = ai.Minimum;

			if (orientation == AxisOrientation.Vertical)
			{
				RGFloat stepY = (clientRect.Height - fontHeight) / ai.Levels;
				var textRect = new Rectangle(0, clientRect.Bottom - fontHeight, clientRect.Width, fontHeight);

				for (int level = 0; level <= ai.Levels; level++)
				{
					g.DrawText(Math.Round(rowValue, Math.Abs(ai.Scaler)).ToString(), this.FontName, this.FontSize, this.ForeColor, textRect, ReoGridHorAlign.Right, ReoGridVerAlign.Middle);

					textRect.Y -= stepY;
					rowValue += Math.Round(ai.LargeStride, Math.Abs(ai.Scaler));
				}
			}
			else if (orientation == AxisOrientation.Horizontal)
			{
				RGFloat columnWidth = clientRect.Width / ai.Levels;
				var textRect = new Rectangle(clientRect.Left - (columnWidth / 2), clientRect.Top, columnWidth, clientRect.Height);

				for (int level = 0; level <= ai.Levels; level ++)
				{
					g.DrawText(Math.Round(rowValue, Math.Abs(ai.Scaler)).ToString(), this.FontName, this.FontSize, this.ForeColor, textRect, ReoGridHorAlign.Center, ReoGridVerAlign.Top);

					textRect.X += columnWidth;
					rowValue += Math.Round(ai.LargeStride, Math.Abs(ai.Scaler));
				}
			}
		}
	}

	public class AxisSerialLabelView : AxisInfoView
	{
		private AxisOrientation orientation;

		public AxisSerialLabelView(AxisChart chart, AxisTypes axisType = AxisTypes.Primary, AxisOrientation orientation = AxisOrientation.Horizontal)
			: base(chart, axisType)
		{
			this.orientation = orientation;
		}

		/// <summary>
		/// Render data label view.
		/// </summary>
		/// <param name="dc">Platform no-associated drawing context instance.</param>
		protected override void OnPaint(DrawingContext dc)
		{
			base.OnPaint(dc);

			if (this.Chart == null) return;

			var ai = this.AxisType == AxisTypes.Primary ?
				this.Chart.PrimaryAxisInfo : this.Chart.SecondaryAxisInfo;

			if (ai == null) return;

			var g = dc.Graphics;

			var ds = this.Chart.DataSource;
			var clientRect = this.ClientBounds;

			var titles = new Dictionary<int, string>();
			var boxes = new List<Size>();

			int dataCount = ds.CategoryCount;
			double rowValue = ai.Minimum;

			for (int i = 0; i < dataCount; i++)
			{
				var title = ds.GetCategoryName(i);

				if (!string.IsNullOrEmpty(title))
				{
					titles[i] = title;
					boxes.Add(PlatformUtility.MeasureText(dc.Renderer, title, this.FontName, this.FontSize, Drawing.Text.FontStyles.Regular));
				}
				else
				{
					boxes.Add(new Size(0, 0));
				}
			}

			if (orientation == AxisOrientation.Horizontal)
			{
				RGFloat columnWidth = (clientRect.Width) / dataCount;

				var maxWidth = boxes.Max(b => b.Width);
				var showableColumns = clientRect.Width / maxWidth;

				int showTitleStride = (int)Math.Ceiling(dataCount / showableColumns);
				if (showTitleStride < 1) showTitleStride = 1;

				RGFloat stepX = clientRect.Width / dataCount;

				for (int i = 0; i < dataCount; i += showTitleStride)
				{
					if (titles.TryGetValue(i, out var text) && !string.IsNullOrEmpty(text))
					{
						var size = boxes[i];
						var textRect = new Rectangle(columnWidth * i, 0, columnWidth, clientRect.Height);

						g.DrawText(text, this.FontName, this.FontSize, this.ForeColor, textRect, ReoGridHorAlign.Center, ReoGridVerAlign.Middle);
					}
				}
			}
			else if (orientation == AxisOrientation.Vertical)
			{
				RGFloat rowHeight = (clientRect.Height) / dataCount;

				var maxHeight = boxes.Max(b => b.Height);
				var showableRows = clientRect.Width / maxHeight;

				int showTitleStride = (int)Math.Ceiling(dataCount / showableRows);
				if (showTitleStride < 1) showTitleStride = 1;

				for (int i = 0; i < dataCount; i += showTitleStride)
				{
					if (titles.TryGetValue(i, out var text) && !string.IsNullOrEmpty(text))
					{
						var size = boxes[i];
						var textRect = new Rectangle(0, rowHeight * i, clientRect.Width, rowHeight);

						g.DrawText(text, this.FontName, this.FontSize, this.ForeColor, textRect, ReoGridHorAlign.Center, ReoGridVerAlign.Middle);
					}
				}
			}
		}
	}
	#endregion // On-Axis Views

	#region Axis Plot Background View
	public class AxisGuideLinePlotView : ChartPlotView
	{
		public AxisGuideLinePlotView(AxisChart chart)
			: base(chart)
		{
			this.LineColor = SolidColor.Silver;
		}

		/// <summary>
		/// Render axis plot view.
		/// </summary>
		/// <param name="dc">Platform unassociated drawing context instance.</param>
		protected override void OnPaint(DrawingContext dc)
		{
			var axisChart = this.Chart as AxisChart;
			if (axisChart == null) return;

			var g = dc.Graphics;
			var clientRect = this.ClientBounds;

			if (axisChart.ShowHorizontalGuideLines)
			{
				var ai = axisChart.PrimaryAxisInfo;

				RGFloat stepY = clientRect.Height / ai.Levels;
				RGFloat y = clientRect.Bottom;

				for (int level = 0; level <= ai.Levels; level++)
				{
					g.DrawLine(clientRect.X, y, clientRect.Right, y, this.LineColor);
					y -= stepY;
				}
			}

			if (axisChart.ShowVerticalGuideLines)
			{
				var ai = axisChart.PrimaryAxisInfo;

				RGFloat stepX = clientRect.Width / ai.Levels;
				RGFloat x = clientRect.Left;

				for (int level = 0; level <= ai.Levels; level++)
				{
					g.DrawLine(x, clientRect.Top, x, clientRect.Bottom, this.LineColor);
					x += stepX;
				}
			}
		}
	}
	#endregion // Axis Plot Background View
}

#endif // DRAWING