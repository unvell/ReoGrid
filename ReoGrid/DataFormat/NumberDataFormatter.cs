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
using unvell.ReoGrid.DataFormat;
using unvell.ReoGrid.Graphics;

namespace unvell.ReoGrid.DataFormat
{
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
}
