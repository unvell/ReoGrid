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

using unvell.ReoGrid.Data;
using unvell.ReoGrid.Drawing;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Events;
using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.Chart
{
	/// <summary>
	/// Interface of chart component
	/// </summary>
	public interface IChart : IDrawingContainer
	{
		/// <summary>
		/// Get or set the title of chart
		/// </summary>
		string Title { get; set; }

		/// <summary>
		/// Get or set the data source of chart
		/// </summary>
		WorksheetChartDataSource DataSource { get; set; }

		/// <summary>
		/// Determine the style of data serial
		/// </summary>
		DataSerialStyleCollection DataSerialStyles { get; }

		///// <summary>
		///// Get number of data serials from data source.
		///// </summary>
		///// <returns>Number of data serials.</returns>
		//int GetSerialCount();

		///// <summary>
		///// Get name of specified data serial.
		///// </summary>
		///// <param name="index">Zero-based number of data serial to get name.</param>
		///// <returns>Name in string of specified data serial.</returns>
		//string GetSerialName(int index);

		///// <summary>
		///// Update chart legend. Update the title, legend and plot view positions.
		///// </summary>
		//void UpdateLayout();
	}

	///// <summary>
	///// Represents legend supported chart interface.
	///// </summary>
	//public interface ILegendSupportedChart
	//{
	//	/// <summary>
	//	/// Get default symbol size of chart legend.
	//	/// </summary>
	//	/// <returns>Symbol size of chart legend.</returns>
	//	Size GetLegendSymbolSize();

	//	/// <summary>
	//	/// Draw legend symbol for every data rows
	//	/// </summary>
	//	/// <param name="dc">Current instance of drawing context.</param>
	//	/// <param name="index">Number of row in data source.</param>
	//	/// <param name="bounds">Symbol bounds relative to legend view.</param>
	//	void DrawLegendSymbol(DrawingContext dc, int index, Rectangle bounds);
	//}

	/// <summary>
	/// Event arguments for drawing context in Chart
	/// </summary>
	public class ChartDrawingEventArgs : DrawingEventArgs
	{
		/// <summary>
		/// Get the instance of current chart
		/// </summary>
	public IChart Chart { get; private set; }

		internal ChartDrawingEventArgs(IChart chart, DrawingContext dc, Rectangle bounds)
			: base(dc, bounds)
		{
			this.Chart = chart;
		}
	}

}

#endif // DRAWING
