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
using unvell.ReoGrid.Graphics;

namespace unvell.ReoGrid.Chart
{
	internal class ChartUtility
	{
		public static double CalcLevelStride(double min, double max, int count, out int scaler)
		{
			double avg = (max - min) / count;
			scaler = (int)( Math.Floor(Math.Log10(avg)));
			double multi = Math.Pow(10, scaler);
			double m = avg % multi;
			return (m == 0) ? avg : (avg - m + multi);
		}

		#region Data Serial Colors
		public const int MaxDataSerials = 100;

		public static List<SolidColor> defaultDataSerialColors = null;

		public static SolidColor GetDefaultDataSerialFillColor(int index)
		{
			if (defaultDataSerialColors == null)
			{
				defaultDataSerialColors = new List<SolidColor>()
				{
					new SolidColor(91,155,213),
					new SolidColor(237,125,49),
					new SolidColor(165, 165, 165),
					new SolidColor(255,192,0),
					new SolidColor(22,191,177),
					new SolidColor(165,196,86),
					new SolidColor(69,35,163),
					new SolidColor(212,98,117),
					new SolidColor(241,131,151),
					new SolidColor(208,199,6),
					new SolidColor(255,50,50),
				};
			}

			if (index >= defaultDataSerialColors.Count)
			{
				if (index < MaxDataSerials)
				{
					for (int i = defaultDataSerialColors.Count; i <= index; i++)
					{
						defaultDataSerialColors.Add(SolidColor.Randomly());
					}
				}
				else
				{
					index = index % MaxDataSerials;
				}
			}

			return defaultDataSerialColors[index];
		}
		#endregion // Shared Row Colors

	}
}

#endif // DRAWING