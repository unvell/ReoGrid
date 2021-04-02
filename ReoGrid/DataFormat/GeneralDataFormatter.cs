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

using System;
using unvell.ReoGrid.Core;

namespace unvell.ReoGrid.DataFormat
{
	/// <summary>
	/// GeneralDataFormatter supports both Text and Numeric format.
	/// And format type can be switched after data changed by user inputing.
	/// </summary>
	internal class GeneralDataFormatter : IDataFormatter
	{
		public string FormatCell(Cell cell)
		{
			object data = cell.InnerData;

			// check numeric
			bool isNumeric = false;

			double value = 0;
			if (data is int)
			{
				value = (double)(int)data;
				isNumeric = true;
			}
			else if (data is double)
			{
				value = (double)data;
				isNumeric = true;
			}
			else if (data is float)
			{
				value = (double)(float)data;
				isNumeric = true;
			}
			else if (data is long)
			{
				value = (double)(long)data;
				isNumeric = true;
			}
			else if (data is short)
			{
				value = (double)(short)data;
				isNumeric = true;
			}
			else if (data is decimal)
			{
				value = (double)(decimal)data;
				isNumeric = true;
			}
			else if (data is string)
			{
				var str = (string)data;

				if (str.StartsWith(" ") || str.EndsWith(" "))
				{
					str = str.Trim();
				}

				isNumeric = double.TryParse(str, out value);

				if (isNumeric) cell.InnerData = value;
			}

			if (isNumeric)
			{
				if (cell.InnerStyle.HAlign == ReoGridHorAlign.General)
				{
					cell.RenderHorAlign = ReoGridRenderHorAlign.Right;
				}

				return Convert.ToString(value);
			}
			else
			{
				return null;
			}
		}

		public string[] Formats { get { return null; } }

		public bool PerformTestFormat()
		{
			return true;
		}
	}

}
