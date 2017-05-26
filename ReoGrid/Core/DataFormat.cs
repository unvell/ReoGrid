/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * http://reogrid.net/
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * Author: Jing <lujing at unvell.com>
 *
 * Copyright (c) 2012-2016 Jing <lujing at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

using unvell.ReoGrid.Core;
using unvell.ReoGrid.DataFormat;

namespace unvell.ReoGrid
{
	partial class Worksheet
	{
		#region Set
		/// <summary>
		/// Set data format for specified range.
		/// </summary>
		/// <param name="addressOrName">Address or name to locate the cell and range on spreadsheet to be set.</param>
		/// <param name="format">Flag specifies that which format will be used.</param>
		/// <param name="dataFormatArgs">Argument to specify the details for different format.</param>
		/// <exception cref="InvalidAddressException">Throw if specified address or name is illegal.</exception>
		public void SetRangeDataFormat(string addressOrName, CellDataFormatFlag format, object dataFormatArgs = null)
		{
			NamedRange namedRange;

			if (RangePosition.IsValidAddress(addressOrName))
			{
				SetRangeDataFormat(new RangePosition(addressOrName), format, dataFormatArgs);
			}
			else if (this.registeredNamedRanges.TryGetValue(addressOrName, out namedRange))
			{
				SetRangeDataFormat(namedRange, format, dataFormatArgs);
			}
			else
				throw new InvalidAddressException(addressOrName);
		}

		/// <summary>
		/// Set data format for specified range.
		/// </summary>
		/// <param name="row">Number of row to locate the range.</param>
		/// <param name="col">Number of column to locate the range.</param>
		/// <param name="rows">Number of rows contained in the range.</param>
		/// <param name="cols">Number of columns contained in the range.</param>
		/// <param name="format">Flag specifies that which format will be used.</param>
		/// <param name="dataFormatArgs">Argument to specify the details for different format.</param>
		public void SetRangeDataFormat(int row, int col, int rows, int cols, CellDataFormatFlag format, object dataFormatArgs = null)
		{
			SetRangeDataFormat(new RangePosition(row, col, rows, cols), format, dataFormatArgs);
		}

		/// <summary>
		/// Set data format for specified range.
		/// </summary>
		/// <param name="range">Range to be set.</param>
		/// <param name="format">Flag specifies that which format will be used.</param>
		/// <param name="dataFormatArgs">Argument to specify the details for different format.</param>
		public void SetRangeDataFormat(RangePosition range, CellDataFormatFlag format, object dataFormatArgs = null)
		{
			RangePosition fixedRange = FixRange(range);

			int rend = fixedRange.EndRow;
			int cend = fixedRange.EndCol;

#if FORMULA
			List<Cell> formulaDirtyCells = new List<Cell>(10);
#else
			List<Cell> formulaDirtyCells = null;
#endif

			for (int r = fixedRange.Row; r <= rend; r++)
			{
				for (int c = fixedRange.Col; c <= cend;)
				{
					Cell cell = CreateAndGetCell(r, c);

					SetCellDataFormat(cell, format, ref dataFormatArgs, formulaDirtyCells);

					c += cell.Colspan > 1 ? cell.Colspan : 1;
				}
			}

#if FORMULA
			foreach (var cell in formulaDirtyCells)
			{
				RecalcCell(cell);
			}
#endif

			RequestInvalidate();
		}

		internal void SetCellDataFormat(CellPosition pos, CellDataFormatFlag format, ref object dataFormatArgs)
		{
			SetCellDataFormat(CreateAndGetCell(pos), format, ref dataFormatArgs);
		}

		internal void SetCellDataFormat(Cell cell, CellDataFormatFlag format,
			ref object dataFormatArgs, List<Cell> formulaDirtyCells = null)
		{
			cell.DataFormat = format;
			cell.DataFormatArgs = dataFormatArgs;

			//string oldDisplay = cell.Display;

			DataFormatterManager.Instance.FormatCell(cell);

			unvell.ReoGrid.Utility.StyleUtility.UpdateCellRenderAlign(this, cell);
			UpdateCellTextBounds(cell);

#if FORMULA
			if (formulaDirtyCells != null)
			{
				// reference ranges (including cells)
				foreach (var referencedRange in formulaRanges)
				{
					if (referencedRange.Value.Any(rr => rr.Contains(cell.InternalPos)))
					{
						if (!formulaDirtyCells.Contains(referencedRange.Key))
						{
							formulaDirtyCells.Add(referencedRange.Key);
						}
					}
				}
			}
#endif
		}
		#endregion // Set

		#region Get
		public CellDataFormatFlag GetCellDataFormat(string addressOrName, out object dataFormatArgs)
		{
			RangePosition namedRange;

			if (CellPosition.IsValidAddress(addressOrName))
			{
				return this.GetCellDataFormat(new CellPosition(addressOrName), out dataFormatArgs);
			}
			else if (this.TryGetNamedRangePosition(addressOrName, out namedRange))
			{
				return this.GetCellDataFormat(namedRange.StartPos, out dataFormatArgs);
			}
			else
				throw new InvalidAddressException(addressOrName);
		}

		public CellDataFormatFlag GetCellDataFormat(CellPosition pos, out object dataFormatArgs)
		{
			pos = this.FixPos(pos);

			var cell = this.cells[pos.Row, pos.Col];

			if (cell == null)
			{
				dataFormatArgs = null;
				return CellDataFormatFlag.General;
			}

			return GetCellDataFormat(cell, out dataFormatArgs);
		}

		internal CellDataFormatFlag GetCellDataFormat(Cell cell, out object dataFormatArgs)
		{
			dataFormatArgs = cell.DataFormatArgs;
			return cell.DataFormat;
		}
		#endregion // Get

		#region Delete
		/// <summary>
		/// Delete data format settings from specified range
		/// </summary>
		/// <param name="range">Range to be remove formats</param>
		public void DeleteRangeDataFormat(RangePosition range)
		{
			var fixedRange = this.FixRange(range);

			for (int r = fixedRange.Row; r <= fixedRange.EndRow; r++)
			{
				for (int c = fixedRange.Col; c <= fixedRange.EndCol;)
				{
					Cell cell = this.cells[r, c];

					if (cell == null)
					{
						c++;
					}
					else
					{
						// clear data format flags
						cell.DataFormat = CellDataFormatFlag.General;
						cell.DataFormatArgs = null;

						if (cell.IsValidCell)
						{
							// reformat cell
							DataFormatterManager.Instance.FormatCell(cell);

							// update cell render alignemnt (Number aligned to right might be restored to left)
							unvell.ReoGrid.Utility.StyleUtility.UpdateCellRenderAlign(this, cell);

							// update text bounds
							UpdateCellTextBounds(cell);

							c += cell.Colspan > 1 ? cell.Colspan : 1;
						}
						else
						{
							c++;
						}
					}
				}
			}
		}
		#endregion // Delete
	}
}

namespace unvell.ReoGrid.DataFormat
{
	using System.Threading;
	using System.Globalization;

	using Utility;
	using Graphics;

	#region Defines
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
				IDataFormatter formatter;

				if (DataFormatters.TryGetValue(cell.DataFormat, out formatter))
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
	#endregion

	#region General
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
	#endregion General

	#region Number
	/// <summary>
	/// Number Formatter used to format data as numeric format.
	/// Available also to format data with different negative styles.
	/// </summary>
	[DataFormat(CellDataFormatFlag.Number)]
	public class NumberDataFormatter : IDataFormatter
	{
		/// <summary>
		/// Format given cell
		/// </summary>
		/// <param name="cell">Instance of cell to be formatted</param>
		/// <returns></returns>
		public string FormatCell(Cell cell)
		{
			object data = cell.InnerData;

			// check numeric
			bool isNumeric = false;

			double value = 0;

			if (data is double)
			{
				value = (double)data;
				isNumeric = true;
			}
			else if (data is int)
			{
				value = (double)(int)data;
				isNumeric = true;
			}
			else if (data is long)
			{
				value = (double)(long)data;
			}
			else if (data is float)
			{
				value = (double)(float)data;
				isNumeric = true;
			}
			else if (data is decimal)
			{
				value = (double)(decimal)data;
				isNumeric = true;
			}
			else if (data is string)
			{
				string strdata = (data as string).Trim();

				isNumeric = double.TryParse(strdata, out value);

				if (!isNumeric) isNumeric = double.TryParse(strdata.Replace(",", ""), out value);

				if (isNumeric) cell.InnerData = value;
			}
			else if (data is DateTime)
			{
				value = ((DateTime)data - new DateTime(1900, 1, 1)).TotalDays;
				isNumeric = true;
			}

			if (isNumeric)
			{
				string prefix = null;
				string postfix = null;

				INumberFormatArgs arg = cell.DataFormatArgs as INumberFormatArgs;

				var numberPart = FormatNumberCellAndGetPattern(cell, ref value, arg);

				if (arg is NumberFormatArgs)
				{
					NumberFormatArgs nargs = (NumberFormatArgs)cell.DataFormatArgs;
					prefix = nargs.CustomNegativePrefix;
					postfix = nargs.CustomNegativePostfix;
				}

				if (arg != null
					&& (arg.NegativeStyle & NumberNegativeStyle.CustomSymbol) == NumberNegativeStyle.CustomSymbol)
				{
					numberPart = (value < 0) ? (prefix + numberPart + postfix) : numberPart;
				}

				return value.ToString(numberPart);
			}
			else
			{
				return null;
			}
		}

		internal static string FormatNumberCellAndGetPattern(Cell cell, ref double value, INumberFormatArgs arg)
		{
			if (cell.InnerStyle.HAlign == ReoGridHorAlign.General)
			{
				cell.RenderHorAlign = ReoGridRenderHorAlign.Right;
			}

			short decimals = 2;
			bool useSeparator = true;
			NumberNegativeStyle negativeStyle = NumberNegativeStyle.Default;

			if (arg != null)
			{
				decimals = arg.DecimalPlaces;
				useSeparator = arg.UseSeparator;
				negativeStyle = arg.NegativeStyle;
			}

			if (value < 0)
			{
				if ((negativeStyle & NumberNegativeStyle.Red) == NumberNegativeStyle.Red)
				{
					cell.RenderColor = SolidColor.Red;
				}
				else
				{
					cell.RenderColor = SolidColor.Transparent;
				}
			}

			// decimal places
			string decimalPlacePart = new string('0', decimals);

			// number
			string numberPart = (useSeparator ? "#,##0." : "0.") + decimalPlacePart;
			if ((negativeStyle & NumberNegativeStyle.Brackets) == NumberNegativeStyle.Brackets)
			{
				numberPart = (value < 0) ? ("(" + numberPart + ")") : numberPart;
			}
			else if ((negativeStyle & NumberNegativeStyle.Prefix_Sankaku) == NumberNegativeStyle.Prefix_Sankaku)
			{
				numberPart = (value < 0) ? ("▲ " + numberPart) : numberPart;
			}

			// negative
			if ((negativeStyle & NumberNegativeStyle.Minus) == 0)
			{
				value = Math.Abs(value);
			}

			return numberPart;
		}

		/// <summary>
		/// Predefined format argument with using separator and two decimal digits (e.g. 1,234.56)
		/// </summary>
		public static NumberFormatArgs TwoDecimalDigitsArgument
		{
			get
			{
				return new NumberFormatArgs
				{
					DecimalPlaces = 2,
					UseSeparator = true,
				};
			}
		}

		/// <summary>
		/// Predefined format argument with using separator but without decimal digits (e.g. 1,234)
		/// </summary>
		public static NumberFormatArgs NoDecimalDigitsArgument
		{
			get
			{
				return new NumberFormatArgs
				{
					DecimalPlaces = 0,
					UseSeparator = true,
				};
			}
		}

		/// <summary>
		/// Represents an interface for all number formats.
		/// </summary>
		public interface INumberFormatArgs
		{
			/// <summary>
			/// Get or set the digis places for number.
			/// </summary>
			short DecimalPlaces { get; set; }

			/// <summary>
			/// Get or set the negative number styles.
			/// </summary>
			NumberNegativeStyle NegativeStyle { get; set; }

			/// <summary>
			/// Determines that whether or not show the separators in numbers.
			/// </summary>
			bool UseSeparator { get; set; }

			/// <summary>
			/// Prefix symbol before negative numbers. (Requires that <code>NegativeStyle</code> set to <code>Custom</code>)
			/// </summary>
			string CustomNegativePrefix { get; set; }

			/// <summary>
			/// Postfix symbol after negative numbers. (Requires that <code>NegativeStyle</code> set to <code>Custom</code>)
			/// </summary>
			string CustomNegativePostfix { get; set; }
		}

		/// <summary>
		/// Represents number format arguments.
		/// </summary>
		[Serializable]
		public class NumberFormatArgs : INumberFormatArgs
		{
			/// <summary>
			/// Create number format arguments.
			/// </summary>
			public NumberFormatArgs()
			{
				this.DecimalPlaces = 2;
				this.NegativeStyle = NumberNegativeStyle.Minus;
			}

			/// <summary>
			/// Number of decimal places.
			/// </summary>
			public short DecimalPlaces { get; set; }

			/// <summary>
			/// Determine the negative number style. (Minus, Red and Brackets)
			/// </summary>
			public NumberNegativeStyle NegativeStyle { get; set; }

			/// <summary>
			/// Determine whether to use a separator to split number every 3 digits.
			/// </summary>
			public bool UseSeparator { get; set; }

			/// <summary>
			/// Prefix symbol before negative numbers. (Requires that <code>NegativeStyle</code> set to <code>Custom</code>)
			/// </summary>
			public string CustomNegativePrefix { get; set; }

			/// <summary>
			/// Postfix symbol after negative numbers. (Requires that <code>NegativeStyle</code> set to <code>Custom</code>)
			/// </summary>
			public string CustomNegativePostfix { get; set; }

			/// <summary>
			/// Compare to another argument instance of NumberFormatArgs.
			/// </summary>
			/// <param name="obj">Another instance to be compared.</param>
			/// <returns>true if two argument object are same.</returns>
			public override bool Equals(object obj)
			{
				if (!(obj is NumberFormatArgs)) return false;
				NumberFormatArgs o = (NumberFormatArgs)obj;
				return this.DecimalPlaces == o.DecimalPlaces
					&& this.NegativeStyle == o.NegativeStyle
					&& this.UseSeparator == o.UseSeparator
					&& this.CustomNegativePrefix == o.CustomNegativePrefix
					&& this.CustomNegativePostfix == o.CustomNegativePostfix
					;
			}

			/// <summary>
			/// Returns the hash code for this instance.
			/// </summary>
			/// <returns></returns>
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}
		}

		/// <summary>
		/// Represents the negative number styles.
		/// </summary>
		/// <example>
		/// Default							:		-1234.56
		/// Minus								:		-1234.56
		/// Red									:		<span style='color:red;'>1,234.56</span>
		/// RedMinus						:		<span style='color:red;'>-1,234.56</span>
		/// Brackets						:		(1,234.56)
		/// RedBrackets					:		<span style='color:red;'>(1,234.56)</span>
		/// RedBracketsMinus		:		<span style='color:red;'>(-1,234.56)</span>
		/// JapaneseSankaku			:		▲ 1,234.00
		/// JapaneseRedSankaku	:		<span style='color:red;'>▲ 1,234.00</span>
		/// </example>
		[Flags]
		public enum NumberNegativeStyle : byte
		{
			/// <summary>
			/// Regular negative style. (e.g. -1234.56)
			/// </summary>
			Default = Minus,

			/// <summary>
			/// Regular negative style. (e.g. -1234.56)
			/// </summary>
			Minus = 0x1,

			/// <summary>
			/// Negative number displayed as red without prefix symbol. (e.g. <span style='color:red;'>1234.56</span>)
			/// </summary>
			Red = 0x2,

			/// <summary>
			/// Negative number surrounded by brackets. (e.g. (1234.56))
			/// </summary>
			Brackets = 0x4,

			/// <summary>
			/// Negative number displayed as red. Equals (Minus | Red) (e.g. <span style='color:red;'>-1234.56</span>)
			/// </summary>
			RedMinus = Minus | Red,

			/// <summary>
			/// Negative number displayed with prefix symbol and brackets. Equals (Minus | Brackets) 
			/// (e.g. <span style='color:red;'>-1234.56</span>)
			/// </summary>
			BracketsMinus = Minus | Brackets,

			/// <summary>
			/// Negative number surrounded by brackets and displayed as red.
			/// Equals (Red | Brackets) e.g. <span style='color:red;'>(1234.56)</span>
			/// </summary>
			RedBrackets = Brackets | Red,

			/// <summary>
			/// Negative number displayed as red with prefix symbol, surrounded by brackets.
			/// Equals (Minus | Red | Brackets). (e.g. <span style='color:red;'>(-1234.56)</span>)
			/// </summary>
			RedBracketsMinus = Minus | Red | Brackets,

#if LANG_JP
			/// <summary>
			/// Negative number with Sankaku symbol prefix. (Japanese negative number style e.g. ▲ 1,234.00)
			/// </summary>
			Prefix_Sankaku = 0x8,
#endif // LANG_JP

			/// <summary>
			/// Set custom prefix and/or postfix for negative numbers.
			/// </summary>
			CustomSymbol = 0xf0,
		}
		/*
		internal enum NumberNegativeStyle : byte
		{
			Minus = 0x1,
			Red = 0x2,
			Brackets = 0x4,

			MinusRed = Minus | Red,
			BracketsRed = Brackets | Red,

			// todo: support culture prefix negative symbol
		}*/

		/// <summary>
		/// Determine whether or not to perform a test when cell is not be set to use current format.
		/// </summary>
		/// <returns>True to perform test; False to do not perform test.</returns>
		public bool PerformTestFormat()
		{
			return true;
		}
	}
#endregion // Number

#region Text
	internal class TextDataFormatter : IDataFormatter
	{
		public string FormatCell(Cell cell)
		{
			if (cell.InnerStyle.HAlign == ReoGridHorAlign.General)
			{
				cell.RenderHorAlign = ReoGridRenderHorAlign.Left;
			}

			return Convert.ToString(cell.InnerData);
		}

		public bool PerformTestFormat()
		{
			return false;
		}
	}
#endregion // Text

#region DateTime
	/// <summary>
	/// Datetime data formatter
	/// </summary>
	public class DateTimeDataFormatter : IDataFormatter
	{
		private static DateTime baseStartDate = new DateTime(1900, 1, 1);

		/// <summary>
		/// Base start time used to calculcate the date from a number value
		/// </summary>
		public static DateTime BaseStartDate { get { return baseStartDate; } set { baseStartDate = value; } }

		/// <summary>
		/// Format cell
		/// </summary>
		/// <param name="cell">cell to be formatted</param>
		/// <returns>Formatted text used to display as cell content</returns>
		public string FormatCell(Cell cell)
		{
			object data = cell.InnerData;

			bool isFormat = false;
			double number;
			DateTime value = baseStartDate;
			string formattedText = null;

			if (data is DateTime)
			{
				value = (DateTime)data;
				isFormat = true;
			}
			else if (CellUtility.TryGetNumberData(data, out number))
			{
				try
				{
					// Excel/Lotus 2/29/1900 bug   
					// original post: http://stackoverflow.com/questions/4538321/reading-datetime-value-from-excel-sheet
					value = DateTime.FromOADate(number);

					isFormat = true;
				}
				catch { }
			}
			else
			{
				string strdata = (data is string ? (string)data : Convert.ToString(data));

				double days = 0;
				if (double.TryParse(strdata, out days))
				{
					try
					{
						value = value.AddDays(days);
						isFormat = true;
					}
					catch { }
				}
				else
				{
					isFormat = (DateTime.TryParse(strdata, out value));
				}
			}

			if (isFormat)
			{
				if (cell.InnerStyle.HAlign == ReoGridHorAlign.General)
				{
					cell.RenderHorAlign = ReoGridRenderHorAlign.Right;
				}

				CultureInfo culture = null;

				string pattern = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;

				if (cell.DataFormatArgs != null && cell.DataFormatArgs is DateTimeFormatArgs)
				{
					DateTimeFormatArgs dargs = (DateTimeFormatArgs)cell.DataFormatArgs;

					if (pattern == null || pattern == String.Empty)
					{
						pattern = dargs.Format;
					}

					culture = (dargs.CultureName == null
						|| string.Equals(dargs.CultureName, Thread.CurrentThread.CurrentCulture.Name))
						? Thread.CurrentThread.CurrentCulture : new CultureInfo(dargs.CultureName);
				}
				else
				{
					culture = System.Threading.Thread.CurrentThread.CurrentCulture;
					cell.DataFormatArgs = new DateTimeFormatArgs { Format = pattern, CultureName = culture.Name };
				}
					
				if (culture.Name.StartsWith("ja") && pattern.Contains("g"))
				{
					culture = new CultureInfo("ja-JP", true);
					culture.DateTimeFormat.Calendar = new JapaneseCalendar();
				}

				try
				{
					switch (pattern)
					{
						case "d":
							formattedText = value.Day.ToString();
							break;

						default:
							formattedText = value.ToString(pattern, culture);
							break;
					}
				}
				catch
				{
					formattedText = Convert.ToString(value);
				}
			}

			return isFormat ? formattedText : null;
		}

		/// <summary>
		/// Represents the argument that is used during format a cell as data time.
		/// </summary>
		[Serializable]
		public struct DateTimeFormatArgs
		{
			private string format;
			/// <summary>
			/// Get or set the date time pattern. (Standard .NET datetime pattern is supported, e.g.: yyyy/MM/dd)
			/// </summary>
			public string Format { get { return format; } set { format = value; } }

			private string cultureName;
			/// <summary>
			/// Get or set the culture name that is used to format datetime according to localization settings.
			/// </summary>
			public string CultureName { get { return cultureName; } set { cultureName = value; } }

			/// <summary>
			/// Compare to another object, check whether or not two objects are same.
			/// </summary>
			/// <param name="obj">Another object to be compared.</param>
			/// <returns>True if two objects are same; Otherwise return false.</returns>
			public override bool Equals(object obj)
			{
				if (!(obj is DateTimeFormatArgs)) return false;
				DateTimeFormatArgs o = (DateTimeFormatArgs)obj;
				return format.Equals(o.format)
					&& cultureName.Equals(o.cultureName);
			}

			/// <summary>
			/// Get the hash code of this argument object.
			/// </summary>
			/// <returns>Hash code of argument object.</returns>
			public override int GetHashCode()
			{
				return format.GetHashCode() ^ cultureName.GetHashCode();
			}
		}

		/// <summary>
		/// Determines whether or not to perform a test when target cell is not set as datetime format.
		/// </summary>
		/// <returns></returns>
		public bool PerformTestFormat()
		{
			return true;
		}
	}
#endregion // DateTime

#region Percent
	/// <summary>
	/// Percent data formatter
	/// </summary>
	public class PercentDataFormatter : IDataFormatter
	{
		public string FormatCell(Cell cell)
		{
			object data = cell.InnerData;

			double percent = 0;
			bool isFormat = false;
			short digits = 0;
			string formattedText = null;

			if (data is double)
			{
				percent = (double)data;
				isFormat = true;
				digits = 9;
			}
			else if (data is DateTime)
			{
				percent = ((DateTime)data - new DateTime(1900, 1, 1)).TotalDays;
				isFormat = true;
				digits = 0;
			}
			else
			{
				string str = Convert.ToString(data);
				if (str.Length > 1 && str.EndsWith("%"))
				{
					// string ends with "%"
					str = str.Substring(0, str.Length - 1);

					isFormat = double.TryParse(str, out percent);

					if (isFormat)
					{
						percent /= 100d;

						int decimalDigits = (short)str.LastIndexOf(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
						if (decimalDigits >= 0)
						{
							digits = (short)(str.Length - 1 - decimalDigits);
						}
					}
				}
				else
				{
					// string ends without "%"
					isFormat = double.TryParse(str, out percent);

					if (isFormat)
					{
						int decimalDigits = (short)str.LastIndexOf(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
						if (decimalDigits >= 0)
						{
							digits = (short)(str.Length - 1 - decimalDigits);
						}
					}
					else
					{
						// try convert from datetime
						DateTime date = new DateTime(1900, 1, 1);
						if (DateTime.TryParse(str, out date))
						{
							percent = (date - new DateTime(1900, 1, 1)).TotalDays;
							isFormat = true;
						}
					}
				}

				if (isFormat) cell.InnerData = percent;
			}

			if (isFormat)
			{
				//if (cell.DataFormatArgs != null && cell.DataFormatArgs is NumberDataFormatter.NumberFormatArgs)
				//{
				//	digits = ((NumberDataFormatter.NumberFormatArgs)cell.DataFormatArgs).DecimalPlaces;
				//}
				//else
				//{
				//	cell.DataFormatArgs = new NumberDataFormatter.NumberFormatArgs { DecimalPlaces = digits };
				//}

				//string decimalPlacePart = new string('0', digits);

				//formattedText = (percent * 100).ToString("0." + decimalPlacePart) + "%";

				//if (cell.InnerStyle.HAlign == ReoGridHorAlign.General)
				//{
				//	cell.RenderHorAlign = ReoGridRenderHorAlign.Right;
				//}
				var format = NumberDataFormatter.FormatNumberCellAndGetPattern(cell, ref percent,
					cell.DataFormatArgs as NumberDataFormatter.INumberFormatArgs);

				return percent.ToString(format + "%");
			}

			return isFormat ? formattedText : null;
		}


		//[Serializable]
		[Obsolete("use NumberDataFormatter.NumberFormatArgs instead")]
		public struct PercentFormatArgs
		{
			///// <summary>
			///// Get or set the decimal places
			///// </summary>
			//public short DecimalPlaces { get; set; }

			///// <summary>
			///// Determine whether or not to display the number using decimal mark
			///// </summary>
			//public bool UseSeparator { get; set; }

			///// <summary>
			///// Compare two objects, check whether or not they are same
			///// </summary>
			///// <param name="obj">Another object to be checked with this</param>
			///// <returns>True if two objects are same</returns>
			//public override bool Equals(object obj)
			//{
			//	if (!(obj is PercentFormatArgs)) return false;
			//	PercentFormatArgs o = (PercentFormatArgs)obj;
			//	return this.DecimalPlaces.Equals(o.DecimalPlaces)
			//		&& this.UseSeparator == o.UseSeparator;
			//}

			///// <summary>
			///// Get the hash code of this object
			///// </summary>
			///// <returns></returns>
			//public override int GetHashCode()
			//{
			//	return base.GetHashCode();
			//}
		}

		/// <summary>
		/// Perform a format check
		/// </summary>
		/// <returns>true if the data is in this format</returns>
		public bool PerformTestFormat()
		{
			return true;
		}
	}
#endregion // Percent

#region Currency
	/// <summary>
	/// Currency data formatter
	/// </summary>
	public class CurrencyDataFormatter : IDataFormatter
	{
		/// <summary>
		/// Format specified cell
		/// </summary>
		/// <param name="cell">cell instance</param>
		/// <returns>true if cell has been formatted</returns>
		public string FormatCell(Cell cell)
		{
			bool isFormat = false;

			object data = cell.InnerData;
			double currency = double.NaN;

			if (data is double)
			{
				isFormat = true;
				currency = (double)data;
			}
			else if (data is DateTime)
			{
				currency = (new DateTime(1900, 1, 1) - (DateTime)data).TotalDays;
				isFormat = true;
			}
			else
			{
				string str = Convert.ToString(data).Trim();
				string number = string.Empty;

				if (str.StartsWith("$"))
				{
					number = str.Substring(1);
					if (double.TryParse(number, out currency))
					{
						isFormat = true;
						cell.InnerData = currency;
					}
				}
				else
				{
					DateTime date = new DateTime(1900, 1, 1);
					if (DateTime.TryParse(str, out date))
					{
						currency = (date - new DateTime(1900, 1, 1)).TotalDays;
						isFormat = true;
					}
					else
					{
						isFormat = double.TryParse(str, out currency);
					}
				}
			}

			if (isFormat)
			{
				if (cell.InnerStyle.HAlign == ReoGridHorAlign.General)
				{
					cell.RenderHorAlign = ReoGridRenderHorAlign.Right;
				}

				string prefixSymbol = null, postfixSymbol = null;
				short decimals = 2;
				NumberDataFormatter.NumberNegativeStyle negativeStyle = NumberDataFormatter.NumberNegativeStyle.Default;
				string prefix = null;
				string postfix = null;

				if (cell.DataFormatArgs != null && cell.DataFormatArgs is CurrencyFormatArgs)
				{
					CurrencyFormatArgs args = (CurrencyFormatArgs)cell.DataFormatArgs;
					prefixSymbol = args.PrefixSymbol;
					postfixSymbol = args.PostfixSymbol;
					decimals = args.DecimalPlaces;
					negativeStyle = args.NegativeStyle;
					prefix = args.CustomNegativePrefix;
					postfix = args.CustomNegativePostfix;
				}
				//else
				//{
				//	var culture = Thread.CurrentThread.CurrentCulture;

				//	switch (culture.NumberFormat.CurrencyPositivePattern)
				//	{
				//		case 0: prefixSymbol = culture.NumberFormat.CurrencySymbol; postfixSymbol = null; break;
				//		case 1: prefixSymbol = null; postfixSymbol = culture.NumberFormat.CurrencySymbol; break;
				//		case 2: prefixSymbol = " " + culture.NumberFormat.CurrencySymbol; postfixSymbol = null; break;
				//		case 3: prefixSymbol = null; postfixSymbol = " " + culture.NumberFormat.CurrencySymbol; break;
				//	}

				//	cell.DataFormatArgs = new CurrencyFormatArgs { PrefixSymbol = prefixSymbol, PostfixSymbol = postfixSymbol, DecimalPlaces = decimals };
				//}

				if (currency < 0)
				{
					if ((negativeStyle & NumberDataFormatter.NumberNegativeStyle.Red) == NumberDataFormatter.NumberNegativeStyle.Red)
						cell.RenderColor = SolidColor.Red;
					else
						cell.RenderColor = SolidColor.Transparent;
				}

				// decimal places
				string decimalPlacePart = new string('0', decimals);

				// number
				string numberPartFormat = prefixSymbol + "#,##0." + decimalPlacePart + postfixSymbol;

				if ((negativeStyle & NumberDataFormatter.NumberNegativeStyle.Brackets) == NumberDataFormatter.NumberNegativeStyle.Brackets)
				{
					numberPartFormat = (currency < 0) ? ("(" + numberPartFormat + ")") : numberPartFormat;
				}
				else if ((negativeStyle & NumberDataFormatter.NumberNegativeStyle.Prefix_Sankaku) == NumberDataFormatter.NumberNegativeStyle.Prefix_Sankaku)
				{
					numberPartFormat = (currency < 0) ? ("▲ " + numberPartFormat) : numberPartFormat;
				}
				else if ((negativeStyle & NumberDataFormatter.NumberNegativeStyle.CustomSymbol) == NumberDataFormatter.NumberNegativeStyle.CustomSymbol)
				{
					numberPartFormat = (currency < 0) ? (prefix + numberPartFormat + postfix) : numberPartFormat;
				}

				// negative
				if ((negativeStyle & NumberDataFormatter.NumberNegativeStyle.Minus) == 0)
				{
					currency = Math.Abs(currency);
				}

				return currency.ToString(numberPartFormat);
			}
			else
				return null;
		}

		/// <summary>
		/// Represents arguments of currency data format.
		/// </summary>
		[Serializable]
		public class CurrencyFormatArgs : NumberDataFormatter.NumberFormatArgs
		{
			/// <summary>
			/// Currency symbol that displayed before currency number.
			/// </summary>
			public string PrefixSymbol { get; set; }

			/// <summary>
			/// Currency symbol that displayed after currency number.
			/// </summary>
			public string PostfixSymbol { get; set; }

			/// <summary>
			/// Culture name in English. (e.g. en-US)
			/// </summary>
			public string CultureEnglishName { get; set; }

			/// <summary>
			/// Check whether or not two objects are same.
			/// </summary>
			/// <param name="obj">Another object to be compared.</param>
			/// <returns>True if two objects are same; Otherwise return false.</returns>
			public override bool Equals(object obj)
			{
				if (!(obj is CurrencyFormatArgs)) return false;

				CurrencyFormatArgs o = (CurrencyFormatArgs)obj;

				return PrefixSymbol == o.PrefixSymbol
					&& PostfixSymbol == o.PostfixSymbol
					&& string.Compare(CultureEnglishName, o.CultureEnglishName, true) == 0
					&& base.Equals(obj);
			}

			/// <summary>
			/// Get hash code
			/// </summary>
			/// <returns></returns>
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}
		}

		/// <summary>
		/// Determine whether or not to perform format test
		/// </summary>
		/// <returns>True to perform test; False to abort</returns>
		public bool PerformTestFormat()
		{
			return true;
		}
	}

#endregion // Currency
}