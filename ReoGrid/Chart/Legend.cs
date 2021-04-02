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

#if WINFORM || ANDROID
using RGFloat = System.Single;
#elif WPF
using RGFloat = System.Double;
#endif // WPF

using unvell.ReoGrid.Drawing;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.Chart
{
	/// <summary>
	/// Represents chart legend view.
	/// </summary>
	public class ChartLegend : DrawingComponent
	{
		/// <summary>
		/// Get the instance of owner chart.
		/// </summary>
		public virtual IChart Chart { get; protected set; }

		/// <summary>
		/// Create chart legend view.
		/// </summary>
		/// <param name="chart">Instance of owner chart.</param>
		public ChartLegend(IChart chart)
		{
			this.Chart = chart;

			this.LineColor = SolidColor.Transparent;
			this.FillColor = SolidColor.Transparent;
			this.FontSize *= 0.8f;
		}

		/// <summary>
		/// Get or set type of legend.
		/// </summary>
		public LegendType LegendType { get; set; }

		private LegendPosition legendPosition;

		/// <summary>
		/// Get or set the display position of legend.
		/// </summary>
		public LegendPosition LegendPosition
		{
			get { return this.legendPosition; }
			set
			{
				if (this.legendPosition != value)
				{
					this.legendPosition = value;

					if (this.Chart is Chart)
					{
						var chart = (Chart)this.Chart;
						chart.DirtyLayout();
					}
				}
			}
		}

		/*
		/// <summary>
		/// Render chart legend view.
		/// </summary>
		/// <param name="dc">Platform no-associated drawing context instance.</param>
		protected override void OnPaint(DrawingContext dc)
		{
			base.OnPaint(dc);
		
			var g = dc.Graphics;
			//var ds = this.Chart.DataSource;
			var clientRect = this.ClientBounds;

			int dataCount = this.Chart.GetSerialCount();

			Rectangle itemRect = new Rectangle(0, 0, this.ItemSize.Width, ItemSize.Height);
			Size smybolSize = this.GetSymbolSize();

			for (int index = 0; index < dataCount; index++)
			{
				string itemTitle = this.Chart.GetSerialName(index);

				Rectangle symbolRect = new Rectangle(itemRect.Left + 3, itemRect.Top + (itemRect.Height - smybolSize.Height) / 2,
					smybolSize.Width, smybolSize.Height);

				this.DrawSymbol(dc, index, symbolRect);

				if (itemTitle != null)
				{
					Rectangle textRect = new Rectangle(symbolRect.Right + 3, itemRect.Top,
						itemRect.Width - symbolRect.Width - 3, itemRect.Height);

					g.DrawText(itemTitle, this.FontName, this.FontSize, this.ForeColor, textRect, ReoGridHorAlign.Left, ReoGridVerAlign.Middle);
				}

				itemRect.X += itemRect.Width;

				if (itemRect.X + itemRect.Width > clientRect.Right)
				{
					itemRect.X = 0;
					itemRect.Y += ItemSize.Height;
				}
			}
		}
		*/
		
		/// <summary>
		/// Get default symbol size of chart legend.
		/// </summary>
		/// <param name="index">Index of serial in data source.</param>
		/// <returns>Symbol size of chart legend.</returns>
		protected virtual Size GetSymbolSize(int index)
		{
			return new Size(14, 14);
		}

		/// <summary>
		/// Measure serial label size.
		/// </summary>
		/// <param name="index">Index of serial in data source.</param>
		/// <returns>Measured size for serial label.</returns>
		protected virtual Size GetLabelSize(int index)
		{
			var ds = this.Chart.DataSource;

			if (ds == null) return Size.Zero;

			string label = ds[index].Label;

			return PlatformUtility.MeasureText(null, label, this.FontName, this.FontSize, this.FontStyles);
		}

		private Size layoutedSize = Size.Zero;

		/// <summary>
		/// Get measured legend view size.
		/// </summary>
		/// <returns>Measured size of legend view.</returns>
		public override Size GetPreferredSize()
		{
			return this.layoutedSize;
		}

		/// <summary>
		/// Layout all legned items.
		/// </summary>
		public virtual void MeasureSize(Rectangle parentClientRect)
		{
			var ds = this.Chart.DataSource;
			if (ds == null) return;

			int dataCount = ds.SerialCount;

			this.Children.Clear();

			RGFloat maxSymbolWidth = 0, maxSymbolHeight = 0, maxLabelWidth = 0, maxLabelHeight = 0;

			#region Measure Sizes
			for (int index = 0; index < dataCount; index++)
			{
				var legendItem = new ChartLegendItem(this, index);

				var symbolSize = this.GetSymbolSize(index);

				if (maxSymbolWidth < symbolSize.Width) maxSymbolWidth = symbolSize.Width;
				if (maxSymbolHeight < symbolSize.Height) maxSymbolHeight = symbolSize.Height;

				legendItem.SymbolBounds = new Rectangle(new Point(0, 0), symbolSize);

				var labelSize = this.GetLabelSize(index);

				// should +6, don't know why
				labelSize.Width += 6;

				if (maxLabelWidth < labelSize.Width) maxLabelWidth = labelSize.Width;
				if (maxLabelHeight < labelSize.Height) maxLabelHeight = labelSize.Height;

				legendItem.LabelBounds = new Rectangle(new Point(0, 0), labelSize);

				this.Children.Add(legendItem);
			}
			#endregion // Measure Sizes

			#region Layout
			const RGFloat symbolLabelSpacing = 4;

			var itemWidth = maxSymbolWidth + symbolLabelSpacing + maxLabelWidth;
			var itemHeight = Math.Max(maxSymbolHeight, maxLabelHeight);

			var clientRect = parentClientRect;
			RGFloat x = 0, y = 0, right = 0, bottom = 0;

			for (int index = 0; index < dataCount; index++)
			{
				var legendItem = this.Children[index] as ChartLegendItem;

				if (legendItem != null)
				{
					legendItem.SetSymbolLocation(0, (itemHeight - legendItem.SymbolBounds.Height) / 2);
					legendItem.SetLabelLocation(maxSymbolWidth + symbolLabelSpacing, (itemHeight - legendItem.LabelBounds.Height) / 2);

					legendItem.Bounds = new Rectangle(x, y, itemWidth, itemHeight);

					if (right < legendItem.Right) right = legendItem.Right;
					if (bottom < legendItem.Bottom) bottom = legendItem.Bottom;
				}

				x += itemWidth;

				const RGFloat itemSpacing = 10;

				if (this.LegendPosition == LegendPosition.Left || this.LegendPosition == LegendPosition.Right)
				{
					x = 0;
					y += itemHeight + itemSpacing;
				}
				else
				{
					x += itemSpacing;

					if (x > clientRect.Width)
					{
						x = 0;
						y += itemHeight + itemSpacing;
					}
				}
			}
			#endregion // Layout

			this.layoutedSize = new Size(right+10, bottom);
		}

	}

	/// <summary>
	/// Represents chart legend item.
	/// </summary>
	public class ChartLegendItem : DrawingObject
	{
		private Rectangle symbolBounds;
		public virtual Rectangle SymbolBounds { get { return this.symbolBounds; } set { this.symbolBounds = value; } }

		private Rectangle labelBounds;
		public virtual Rectangle LabelBounds { get { return this.labelBounds; } set { this.labelBounds = value; } }

		public virtual void SetSymbolLocation(RGFloat x, RGFloat y)
		{
			this.symbolBounds.X = x;
			this.symbolBounds.Y = y;
		}

		public virtual void SetLabelLocation(RGFloat x, RGFloat y)
		{
			this.labelBounds.X = x;
			this.labelBounds.Y = y;
		}

		public virtual ChartLegend ChartLegend { get; protected set; }

		public ChartLegendItem(ChartLegend chartLegend, int legendIndex)
		{
			this.ChartLegend = chartLegend;
			this.LegendIndex = legendIndex;
		}

		public virtual int LegendIndex { get; set; }

		protected override void OnPaint(DrawingContext dc)
		{
#if DEBUG
			//dc.Graphics.FillRectangle(this.ClientBounds, SolidColor.LightSteelBlue);
#endif // DEBUG

			if (this.symbolBounds.Width > 0 && this.symbolBounds.Height > 0)
			{
				this.OnPaintSymbol(dc);
			}

			if (this.labelBounds.Width > 0 && this.labelBounds.Height > 0)
			{
				this.OnPaintLabel(dc);
			}
		}

		/// <summary>
		/// Draw chart legend symbol.
		/// </summary>
		/// <param name="dc">Platform no-associated drawing context instance.</param>
		public virtual void OnPaintSymbol(DrawingContext dc)
		{
			var g = dc.Graphics;

			if (this.ChartLegend != null)
			{
				var legend = this.ChartLegend;

				if (legend.Chart != null)
				{
					var dss = legend.Chart.DataSerialStyles;

					if (dss != null)
					{
						var dsStyle = dss[LegendIndex];

						g.DrawAndFillRectangle(this.symbolBounds, dsStyle.LineColor, dsStyle.FillColor);
					}
				}
			}
		}

		/// <summary>
		/// Draw chart legend label.
		/// </summary>
		/// <param name="dc">Platform no-associated drawing context instance.</param>
		public virtual void OnPaintLabel(DrawingContext dc)
		{
			if (this.ChartLegend != null)
			{
				var legend = this.ChartLegend;

				if (legend.Chart != null && legend.Chart.DataSource != null)
				{
					var ds = legend.Chart.DataSource;

					string itemTitle = ds[LegendIndex].Label;

					if (!string.IsNullOrEmpty(itemTitle))
					{
#if DEBUG
						//dc.Graphics.FillRectangle(this.labelBounds, SolidColor.LightCoral);
#endif // DEBUG

						dc.Graphics.DrawText(itemTitle, this.FontName, this.FontSize, this.ForeColor, this.labelBounds,
							ReoGridHorAlign.Left, ReoGridVerAlign.Middle);
					}
				}
			}
		}
	}

	/// <summary>
	/// Legend type.
	/// </summary>
	public enum LegendType
	{
		/// <summary>
		/// Primary legend.
		/// </summary>
		PrimaryLegend,

		/// <summary>
		/// Secondary legend.
		/// </summary>
		SecondaryLegend,
	}

	/// <summary>
	/// Legend position.
	/// </summary>
	public enum LegendPosition
	{
		/// <summary>
		/// Right
		/// </summary>
		Right,

		/// <summary>
		/// Bottom
		/// </summary>
		Bottom,

		/// <summary>
		/// Left
		/// </summary>
		Left,

		/// <summary>
		/// Top
		/// </summary>
		Top,
	}
}

#endif // DRAWING