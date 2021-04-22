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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using unvell.ReoGrid.Core;
using unvell.ReoGrid.Graphics;

namespace unvell.ReoGrid.DataFormat
{
	/// <summary>
	/// Cell data format type
	/// </summary>
	public enum CellDataFormatFlag
	{
		/// <summary>
		/// Auto format type (compliant with Text and Number)
		/// </summary>
		General,

		/// <summary>
		/// Number Type
		/// </summary>
		Number,

		/// <summary>
		/// Date and Time Type
		/// </summary>
		DateTime,

		/// <summary>
		/// Percent Type
		/// </summary>
		Percent,

		/// <summary>
		/// Currency Type
		/// </summary>
		Currency,

		/// <summary>
		/// String
		/// </summary>
		Text,

		/// <summary>
		/// User custom data formatter
		/// </summary>
		Custom,
	}

	[Serializable]
	internal sealed class DataFormatAttribute : Attribute
	{
		private CellDataFormatFlag formatFlag;
		public DataFormatAttribute(CellDataFormatFlag formatFlag)
		{
			this.formatFlag = formatFlag;
		}

		public bool PerformTypeTest { get; set; }
	}

	/// <summary>
	/// Data format provider interface
	/// </summary>
	public interface IDataFormatter
	{
		/// <summary>
		/// Format data stored in specified cell.
		/// </summary>
		/// <param name="cell">Instance of cell to be formatted.</param>
		/// <returns>Return non-empty string if formatting was performed successfully; Otherwise return null.</returns>
		string FormatCell(Cell cell);

		/// <summary>
		/// Indicate that whether or not to check the data type before format.
		/// </summary>
		/// <returns>True to perform test; Otherwise return false.</returns>
		bool PerformTestFormat();
	}

	/// <summary>
	/// Data Formatter Manager
	/// </summary>
	public sealed class DataFormatterManager
	{
		private static DataFormatterManager instance;

		/// <summary>
		/// Instance for this class
		/// </summary>
		public static DataFormatterManager Instance
		{
			get
			{
				if (instance == null) instance = new DataFormatterManager();
				return instance;
			}
		}

		private Dictionary<CellDataFormatFlag, IDataFormatter> dataFormatters = new Dictionary<CellDataFormatFlag, IDataFormatter>();

		/// <summary>
		/// Built-in data formatters
		/// </summary>
		public Dictionary<CellDataFormatFlag, IDataFormatter> DataFormatters
		{
			get { return dataFormatters; }
			set { dataFormatters = value; }
		}

		private DataFormatterManager()
		{
			// add data formatter by this order to decide format detecting priority
			// by default General Data Formatter is first formatter
			dataFormatters.Add(CellDataFormatFlag.General, new GeneralDataFormatter());
			dataFormatters.Add(CellDataFormatFlag.Number, new NumberDataFormatter());
			dataFormatters.Add(CellDataFormatFlag.DateTime, new DateTimeDataFormatter());
			dataFormatters.Add(CellDataFormatFlag.Percent, new PercentDataFormatter());
			dataFormatters.Add(CellDataFormatFlag.Currency, new CurrencyDataFormatter());
			dataFormatters.Add(CellDataFormatFlag.Text, new TextDataFormatter());
		}

		internal void FormatCell(Cell cell)
		{
			// clear cell render color
			// render color used for draw a negative number
			cell.RenderColor = SolidColor.Transparent;

			if (cell.DataFormat == CellDataFormatFlag.General)
			{
				bool found = false;
				string formattedText = null;

				foreach (CellDataFormatFlag flag in dataFormatters.Keys)
				{
					var formatter = dataFormatters[flag];

					if (formatter.PerformTestFormat()
						&& (formattedText = dataFormatters[flag].FormatCell(cell)) != null)
					{
						cell.DataFormat = flag;
						cell.InnerDisplay = formattedText;
						found = true;
						break;
					}
				}

				if (!found)
				{
					if (cell.InnerData is string)
					{
						cell.InnerDisplay = (string)cell.InnerData;
					}
					else
					{
						cell.InnerDisplay = Convert.ToString(cell.InnerData);
					}

					// if horizontal-align is auto self-adapt, 
					// set the render alignment to left for string type
					if (cell.InnerStyle.HAlign == ReoGridHorAlign.General)
					{
						cell.RenderHorAlign = ReoGridRenderHorAlign.Left;
					}
				}

			}
			else
			{
				if (DataFormatters.TryGetValue(cell.DataFormat, out var formatter))
				{
					string formattedText = DataFormatters[cell.DataFormat].FormatCell(cell);

					if (formattedText == null)
					{
						formattedText = DataFormatters[CellDataFormatFlag.Text].FormatCell(cell);
					}

					cell.InnerDisplay = formattedText;
				}
			}
		}
	}
}
