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

using unvell.ReoGrid.Drawing;
using unvell.ReoGrid.Graphics;
using System.Collections;

namespace unvell.ReoGrid.Chart
{
	/// <summary>
	/// Data point styles for line plot view.
	/// </summary>
	public enum DataPointStyles
	{
		/// <summary>
		/// None (Default style)
		/// </summary>
		None,

		/// <summary>
		/// Triangle
		/// </summary>
		Triangle,

		/// <summary>
		/// Square
		/// </summary>
		Square,

		/// <summary>
		/// Ellipse Outline
		/// </summary>
		EllipseOutline,
		
		/// <summary>
		/// Ellipse Filled
		/// </summary>
		EllipseFilled,
	}

	/// <summary>
	/// Represents the interface of data serial style.
	/// </summary>
	public interface IDataSerialStyle : IDrawingLineObjectStyle
	{
		//DataPointStyles DataPointStyle { get; set; }
	}

	//internal class BaseDataSerialStyle : IDataSerialStyle
	//{
	//	public IColor FillColor { get; set; }
	//	public SolidColor LineColor { get; set; }
	//	public RGFloat LineWidth { get; set; }
	//	public LineStyles LineStyle { get; set; }
	//	public LineCapStyles StartCap { get; set; }
	//	public LineCapStyles EndCap { get; set; }
	//	public SolidColor TextColor { get; set; }
	//}

	internal class DataSerialStyle : IDataSerialStyle
	{
		internal Chart Chart { get; private set; }

		public DataSerialStyle(Chart chart)
		{
			this.Chart = chart;
		}

		private IColor fillColor;

		public IColor FillColor
		{
			get
			{
				return this.fillColor;
			}
			set
			{
				if (this.fillColor != value || !this.fillColor.Equals(value))
				{
					this.fillColor = value;

					if (this.Chart != null) this.Chart.Invalidate();
				}
			}
		}

		private SolidColor lineColor;

		public SolidColor LineColor
		{
			get
			{
				return lineColor;
			}
			set
			{
				if (lineColor != value)
				{
					this.lineColor = value;

					if (this.Chart != null) this.Chart.Invalidate();
				}
			}
		}

		private RGFloat lineWeight = 2f;

		public RGFloat LineWidth
		{
			get
			{
				return lineWeight;
			}
			set
			{
				if (lineWeight != value)
				{
					lineWeight = value;

					if (this.Chart != null) this.Chart.Invalidate();
				}
			}
		}

		private LineStyles lineStyle = LineStyles.Solid;

		public LineStyles LineStyle
		{
			get
			{
				return this.lineStyle;
			}
			set
			{
				if (this.lineStyle != value)
				{
					this.lineStyle = value;

					if (this.Chart != null) this.Chart.Invalidate();
				}
			}
		}

		private LineCapStyles startCap = LineCapStyles.None;

		public LineCapStyles StartCap
		{
			get
			{
				return this.startCap;
			}
			set
			{
				if (this.startCap != value)
				{
					this.startCap = value;

					if (this.Chart != null) this.Chart.Invalidate();
				}
			}
		}

		public LineCapStyles endCap = LineCapStyles.None;

		public LineCapStyles EndCap
		{
			get
			{
				return this.endCap;
			}
			set
			{
				if (this.endCap != value)
				{
					this.endCap = value;

					if (this.Chart != null) this.Chart.Invalidate();
				}
			}
		}

		//private SolidColor textColor;

		//public SolidColor TextColor
		//{
		//	get
		//	{
		//		return this.textColor;
		//	}
		//	set
		//	{
		//		if (this.textColor != value)
		//		{
		//			this.textColor = value;

		//			if (this.Chart != null) this.Chart.Invalidate();
		//		}
		//	}
		//}
	}

	/// <summary>
	/// Represents a collection of data serial styles.
	/// </summary>
	public sealed class DataSerialStyleCollection : IEnumerable<IDataSerialStyle>
	{
		internal Chart Chart { get; set; }

		internal DataSerialStyle defaultDataSerialStyle;

		internal DataSerialStyleCollection(Chart chart)
		{
			this.Chart = chart;

			this.defaultDataSerialStyle = new DataSerialStyle(this.Chart)
			{
				FillColor = SolidColor.Transparent,
				LineColor = SolidColor.Black,
				LineWidth = 1f,
				StartCap = LineCapStyles.None,
				EndCap = LineCapStyles.None,
			};
		}

		/// <summary>
		/// Get or set the seiral style for data specified by zero-based index.
		/// </summary>
		/// <param name="index">Zero-based index of data to get style.</param>
		/// <returns>Data serial style object of data.</returns>
		public IDataSerialStyle this[int index]
		{
			get
			{
				return this.Chart.serialStyles[index];
			}
		}

		internal IEnumerator<IDataSerialStyle> GetEnum()
		{
			foreach (var style in this.Chart.serialStyles)
			{
				yield return style;
			}
		}

		public IEnumerator<IDataSerialStyle> GetEnumerator()
		{
			return this.GetEnum();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnum();
		}
	}
}

#endif // DRAWING