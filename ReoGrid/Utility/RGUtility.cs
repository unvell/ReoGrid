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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace unvell.ReoGrid
{
	public static class RGUtility
	{
		internal static readonly Regex CellReferenceRegex = new Regex(
			"^\\s*(?:(?<abs_col>\\$)?(?<col>[A-Z]+)(?<abs_row>\\$)?(?<row>[0-9]+))\\s*$", 
			RegexOptions.Compiled | RegexOptions.Singleline);

		internal static readonly Regex RangeReferenceRegex = new Regex(
			"^\\s*(?:(?<abs_from_col>\\$)?(?<from_col>[A-Z]+)(?<abs_from_row>\\$)?(?<from_row>[0-9]+):(?<abs_to_col>\\$)?(?<to_col>[A-Z]+)(?<abs_to_row>\\$)?(?<to_row>[0-9]+))\\s*$",
			RegexOptions.Compiled | RegexOptions.Singleline);

		internal static readonly Regex SingleAbsoulteRangeRegex = new Regex(
			"^\\s*(" +
				"((?<abs_from_row>\\$)?(?<from_row>[0-9]+):(?<abs_to_row>\\$)?(?<to_row>[0-9]+))|" +
				"((?<abs_from_col>\\$)?(?<from_col>[A-Z]+):(?<abs_to_col>\\$)?(?<to_col>[A-Z]+)))\\s*$",
				//"((?:\\$(?<from_col>[A-Z]+)\\$(?<from_row>[0-9]+):\\$(?<to_col>[A-Z]+)\\$(?<to_row>[0-9]+))))\\s*$",
			RegexOptions.Compiled | RegexOptions.Singleline);

		//internal static readonly Regex RangeReferenceRegex = new Regex(
		//	//"(?:\"[^\"]*\\s*\")|(?:(?<from_col>[A-Z]+)(?<from_row>[0-9]+):(?<to_col>[A-Z]+)(?<to_row>[0-9]+))",
		//	"(?:(?<from_col>[A-Z]+)(?<from_row>[0-9]+):(?<to_col>[A-Z]+)(?<to_row>[0-9]+))",
		//	RegexOptions.Compiled | RegexOptions.Singleline);

		internal static readonly Regex NameRegex = new Regex("^\\s*[A-Za-z0-9_$]+\\s*$",
			RegexOptions.Compiled | RegexOptions.Singleline);

		private const string AlphaChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		private const int AlphaCharsLength = 26;

		/// <summary>
		/// Get alphabet of number (eg. A is 1 and 30 is AD)
		/// </summary>
		/// <param name="a">number to be converted</param>
		/// <returns>alphabet of number</returns>
		public static string GetAlphaChar(long a)
		{
			char[] v = new char[10];

			int i = 9;
			while (a >= AlphaCharsLength)
			{
				v[i] = AlphaChars[((int)(a % AlphaCharsLength))];
				a = a / AlphaCharsLength - 1;

				i--;
			}

			v[i] = AlphaChars[((int)(a % AlphaCharsLength))];
			return new string(v, i, 10 - i);
		}

		/// <summary>
		/// Get number of alphabet from string (eg. A is 1 and AD is 30)
		/// </summary>
		/// <param name="address">alphabet to be converted</param>
		/// <returns>number of alphabet</returns>
		public static int GetNumberOfChar(string address)
		{
			if (string.IsNullOrEmpty(address) || address.Length < 1 || address.Any(c => c < 'A' || c > 'Z'))
			{
				throw new ArgumentException("cannot convert into number of index from empty address", "id");
			}
			else
			{
				int idx = address[0] - AlphaChars[0] + 1;
				for (int i = 1; i < address.Length; i++)
				{
					idx *= AlphaCharsLength;
					idx += address[i] - AlphaChars[0] + 1;
				}
				return idx - 1;
			}
		}

		/// <summary>
		/// Check whether specified string is an valid address to locate cell or range 
		/// </summary>
		/// <param name="address">address for cell or range</param>
		public static bool IsValidAddress(string address)
		{
			return CellPosition.IsValidAddress(address) || RangePosition.IsValidAddress(address);
		}

		/// <summary>
		/// Chck whether specified string is valid name to define a range
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static bool IsValidName(string name)
		{
			return NameRegex.IsMatch(name);
		}

		/// <summary>
		/// Parse tabbed string into regular array
		/// </summary>
		/// <param name="str">string to be parsed</param>
		/// <returns>parsed regular array</returns>
		public static object[,] ParseTabbedString(string str)
		{
			int rows = 0, cols = 0;
			
			string[] lines = str.Split(new string[] { "\n" }, StringSplitOptions.None);

			int len = lines.Length;
			
			while (string.IsNullOrEmpty(lines[len - 1]))
			{
				len--;
			}

			for (int r = 0; r < len; r++)
			{
				string line = lines[r];
				
				if (line.EndsWith("\n")) line = line.Substring(0, line.Length - 1);
				if (line.EndsWith("\r")) line = line.Substring(0, line.Length - 1);

				string[] tabs = line.Split('\t');
				cols = Math.Max(cols, tabs.Length);
				rows++;
			}

			object[,] arr = new object[rows, cols];

			for (int r = 0; r < lines.Length; r++)
			{
				string line = lines[r];
			
				if (line.EndsWith("\n")) line = line.Substring(0, line.Length - 1);
				if (line.EndsWith("\r")) line = line.Substring(0, line.Length - 1);

				if (line.Length > 0)
				{
					string[] tabs = line.Split('\t');
					cols = Math.Max(cols, tabs.Length);

					for (int c = 0; c < tabs.Length; c++)
					{
						string text = tabs[c];

						if (text.StartsWith("\"") && text.EndsWith("\""))
						{
							text = text.Substring(1, text.Length - 2);
						}

						arr[r, c] = text;
					}
					rows++;
				}
			}

			return arr;
		}

		#region ToAddress
		/// <summary>
		/// Convert position or range into address string
		/// </summary>
		/// <param name="row">Zero-based index number of row</param>
		/// <param name="col">Zero-based index number of column</param>
		/// <param name="absNum">Determine that which R1C1 format should be used.<br/>
		/// <ul>
		/// <li>1: [Absolute Row][Absolute Col] R1C1</li>
		/// <li>2: [Absolute Row][Relative Col] R1C[1]</li>
		/// <li>3: [Relative Row][Absolute Col] R[1]C1</li>
		/// <li>4: [Relative Row][Relative Col] R[1]C[1]</li>
		/// </ul>
		/// </param>
		/// <returns>position or range in address string</returns>
		public static string ToAddress(int row, int col, int absNum)
		{
			return ToAddress(row, col, 1, 1, absNum, false);
		}

		/// <summary>
		/// Convert position or range into address string
		/// </summary>
		/// <param name="row">Zero-based index number of row</param>
		/// <param name="col">Zero-based index number of column</param>
		/// <param name="a1style">true to use A1 style; false use the R1C1 style</param>
		/// <returns>position or range in address string</returns>
		public static string ToAddress(int row, int col, bool a1style)
		{
			return ToAddress(row, col, 1, 1, a1style);
		}

		/// <summary>
		/// Convert position or range into address string
		/// </summary>
		/// <param name="row">Zero-based index number of row</param>
		/// <param name="col">Zero-based index number of column</param>
		/// <param name="absNum">Determine that which R1C1 format should be used.<br/>
		/// <ul>
		/// <li>1: [Absolute Row][Absolute Col] R1C1</li>
		/// <li>2: [Absolute Row][Relative Col] R1C[1]</li>
		/// <li>3: [Relative Row][Absolute Col] R[1]C1</li>
		/// <li>4: [Relative Row][Relative Col] R[1]C[1]</li>
		/// </ul>
		/// </param>
		/// <param name="a1style">true to use A1 style; false use the R1C1 style</param>
		/// <returns>position or range in address string</returns>
		public static string ToAddress(int row, int col, int absNum, bool a1style)
		{
			return ToAddress(row, col, 1, 1, 0, a1style);
		}

		/// <summary>
		/// Convert position or range into address string
		/// </summary>
		/// <param name="row">Zero-based index number of row</param>
		/// <param name="col">Zero-based index number of column</param>
		/// <param name="rows">Zero-based number of rows</param>
		/// <param name="cols">Zero-based number of columns</param>
		/// <param name="a1style">true to use A1 style; false use the R1C1 style</param>
		/// <returns>position or range in address string</returns>
		public static string ToAddress(int row, int col, int rows = 1, int cols = 1, bool a1style = true)
		{
			return ToAddress(row, col, rows, cols, 0, a1style);
		}

		/// <summary>
		/// Convert position or range into address stringConvert position or range into address string
		/// </summary>
		/// <param name="row">Zero-based index number of row</param>
		/// <param name="col">Zero-based index number of column</param>
		/// <param name="rows">Zero-based number of rows</param>
		/// <param name="cols">Zero-based number of columns</param>
		/// <param name="absNum">Determine that which R1C1 format should be used.<br/>
		/// <ul>
		/// <li>1: [Absolute Row][Absolute Col] R1C1</li>
		/// <li>2: [Absolute Row][Relative Col] R1C[1]</li>
		/// <li>3: [Relative Row][Absolute Col] R[1]C1</li>
		/// <li>4: [Relative Row][Relative Col] R[1]C[1]</li>
		/// </ul>
		/// </param>
		/// <param name="a1style">true to use A1 style; false use the R1C1 style</param>
		/// <returns>position or range in address string</returns>
		public static string ToAddress(int row, int col, int rows, int cols, int absNum, bool a1style)
		{
			if (rows <= 1 && cols <= 1)
			{
				// pos
				if (a1style)
				{
					return (RGUtility.GetAlphaChar(col) + (row + 1));
				}
				else
				{
					switch (absNum)
					{
						default:
						case 1: // absolute row, absolute col
							return string.Format("R{0}C{1}", row, col);
						case 2: // absolute row, relative col
							return string.Format("R{0}C[{1}]", row, col);
						case 3: // relative row, absolute col
							return string.Format("R[{0}]C{1}", row, col);
						case 4: // relative row, relative col
							return string.Format("R[{0}]C[{1}]", row, col);
					}
				}
			}
			else
			{
				// range

				int toRow = row + rows - 1;
				int toCol = col + cols - 1;

				return ToAddress(row, col, absNum, a1style) + ":" + ToAddress(row, col, absNum, a1style);
			}
		}

		/// <summary>
		/// Convert range address into cell address style if the range is a merged cell (A1:A1 => A1)
		/// </summary>
		/// <param name="sheet">Worksheet instance used to check whther or not the range is a merged cell</param>
		/// <param name="range">Range to be converted</param>
		/// <returns>Single cell address if convert is successful; otherwise return the range address</returns>
		public static string ToSingleAddressIfPossible(Worksheet sheet, RangePosition range)
		{
			return (sheet.IsMergedCell(range)) ? range.StartPos.ToAddress() : range.ToAddress();
		}
		#endregion // ToAddress

		public static System.Diagnostics.Process OpenFileOrLink(string url, string args = null)
		{
			return System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url)
			{
				UseShellExecute = true,
				Arguments = args,
				Verb = "open"
			});
		}
	}
}
