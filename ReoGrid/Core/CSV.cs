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
using System.IO;
using System.Text;

using unvell.ReoGrid.DataFormat;
using unvell.ReoGrid.Interaction;
using unvell.ReoGrid.IO;

namespace unvell.ReoGrid
{
	partial class Worksheet
	{
		#region Load

		/// <summary>
		/// Load CSV file into worksheet.
		/// </summary>
		/// <param name="path">File contains CSV data.</param>
		public void LoadCSV(string path)
		{
			LoadCSV(path, RangePosition.EntireRange);
		}

		/// <summary>
		/// Load CSV file into worksheet.
		/// </summary>
		/// <param name="path">File contains CSV data.</param>
		/// <param name="targetRange">The range used to fill loaded CSV data.</param>
		public void LoadCSV(string path, RangePosition targetRange)
		{
			LoadCSV(path, Encoding.Default, targetRange);
		}

		/// <summary>
		/// Load CSV file into worksheet.
		/// </summary>
		/// <param name="path">Path to load CSV file.</param>
		/// <param name="encoding">Encoding used to read and decode plain-text from file.</param>
		public void LoadCSV(string path, Encoding encoding)
		{
			LoadCSV(path, encoding, RangePosition.EntireRange);
		}

		/// <summary>
		/// Load CSV file into worksheet.
		/// </summary>
		/// <param name="path">Path to load CSV file.</param>
		/// <param name="encoding">Encoding used to read and decode plain-text from file.</param>
		/// <param name="targetRange">The range used to fill loaded CSV data.</param>
		public void LoadCSV(string path, Encoding encoding, RangePosition targetRange)
		{
			using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				LoadCSV(fs, encoding, targetRange);
			}
		}

		/// <summary>
		/// Load CSV data from stream into worksheet.
		/// </summary>
		/// <param name="s">Input stream to read CSV data.</param>
		public void LoadCSV(Stream s)
		{
			LoadCSV(s, Encoding.Default);
		}

		/// <summary>
		/// Load CSV data from stream into worksheet.
		/// </summary>
		/// <param name="s">Input stream to read CSV data.</param>
		/// <param name="targetRange">The range used to fill loaded CSV data.</param>
		public void LoadCSV(Stream s, RangePosition targetRange)
		{
			LoadCSV(s, Encoding.Default, targetRange);
		}

		/// <summary>
		/// Load CSV data from stream into worksheet.
		/// </summary>
		/// <param name="s">Input stream to read CSV data.</param>
		/// <param name="encoding">Text encoding used to read and decode plain-text from stream.</param>
		public void LoadCSV(Stream s, Encoding encoding)
		{
			LoadCSV(s, encoding, RangePosition.EntireRange);
		}

		/// <summary>
		/// Load CSV data from stream into worksheet.
		/// </summary>
		/// <param name="s">Input stream to read CSV data.</param>
		/// <param name="encoding">Text encoding used to read and decode plain-text from stream.</param>
		/// <param name="targetRange">The range used to fill loaded CSV data.</param>
		public void LoadCSV(Stream s, Encoding encoding, RangePosition targetRange)
		{
			LoadCSV(s, encoding, targetRange, targetRange.IsEntire ? true : false, 256);
		}

		/// <summary>
		/// Load CSV data from stream into worksheet.
		/// </summary>
		/// <param name="s">Input stream to read CSV data.</param>
		/// <param name="encoding">Text encoding used to read and decode plain-text from stream.</param>
		/// <param name="targetRange">The range used to fill loaded CSV data.</param>
		/// <param name="autoSpread">decide whether or not to append rows or columns automatically to fill csv data</param>
		/// <param name="bufferLines">decide how many lines int the buffer to read and fill csv data</param>
		public void LoadCSV(Stream s, Encoding encoding, RangePosition targetRange, bool autoSpread, int bufferLines)
		{
			this.controlAdapter.ChangeCursor(CursorStyle.Busy);

			try
			{
				CSVFileFormatProvider csvProvider = new CSVFileFormatProvider();

				var arg = new CSVFormatArgument
				{
					AutoSpread = autoSpread,
					BufferLines = bufferLines,
					TargetRange = targetRange,
				};

				Clear();

				csvProvider.Load(this.workbook, s, encoding, arg);
			}
			finally
			{
				this.controlAdapter.ChangeCursor(CursorStyle.PlatformDefault);
			}
		}

		#endregion // Load

		#region Export

		/// <summary>
		/// Export spreadsheet as CSV format from specified number of rows.
		/// </summary>
		/// <param name="path">File path to write CSV format as stream.</param>
		/// <param name="startRow">Number of rows start to export data, 
		/// this property is useful to skip the headers on top of worksheet.</param>
		/// <param name="encoding">Text encoding during output text in CSV format.</param>
		public void ExportAsCSV(string path, int startRow = 0, Encoding encoding = null)
		{
			ExportAsCSV(path, new RangePosition(startRow, 0, -1, -1), encoding);
		}

		/// <summary>
		/// Export spreadsheet as CSV format from specified range.
		/// </summary>
		/// <param name="path">File path to write CSV format as stream.</param>
		/// <param name="addressOrName">Range to be output from this worksheet, specified by address or name.</param>
		/// <param name="encoding">Text encoding during output text in CSV format.</param>
		public void ExportAsCSV(string path, string addressOrName, Encoding encoding = null)
		{
			NamedRange namedRange;

			if (RangePosition.IsValidAddress(addressOrName))
			{
				this.ExportAsCSV(path, new RangePosition(addressOrName), encoding);
			}
			else if (this.TryGetNamedRange(addressOrName, out namedRange))
			{
				this.ExportAsCSV(path, namedRange, encoding);
			}
			else
			{
				throw new InvalidAddressException(addressOrName);
			}
		}

		/// <summary>
		/// Export spreadsheet as CSV format from specified range.
		/// </summary>
		/// <param name="path">File path to write CSV format as stream.</param>
		/// <param name="range">Range to be output from this worksheet.</param>
		/// <param name="encoding">Text encoding during output text in CSV format.</param>
		public void ExportAsCSV(string path, RangePosition range, Encoding encoding = null)
		{
			using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				this.ExportAsCSV(fs, range, encoding);
			}
		}

		/// <summary>
		/// Export spreadsheet as CSV format from specified number of rows.
		/// </summary>
		/// <param name="s">Stream to write CSV format as stream.</param>
		/// <param name="startRow">Number of rows start to export data, 
		/// this property is useful to skip the headers on top of worksheet.</param>
		/// <param name="encoding">Text encoding during output text in CSV format</param>
		public void ExportAsCSV(Stream s, int startRow = 0, Encoding encoding = null)
		{
			this.ExportAsCSV(s, new RangePosition(startRow, 0, -1, -1), encoding);
		}

		/// <summary>
		/// Export spreadsheet as CSV format from specified range.
		/// </summary>
		/// <param name="s">Stream to write CSV format as stream.</param>
		/// <param name="addressOrName">Range to be output from this worksheet, specified by address or name.</param>
		/// <param name="encoding">Text encoding during output text in CSV format.</param>
		public void ExportAsCSV(Stream s, string addressOrName, Encoding encoding = null)
		{
			NamedRange namedRange;

			if (RangePosition.IsValidAddress(addressOrName))
			{
				ExportAsCSV(s, new RangePosition(addressOrName), encoding);
			}
			else if (this.TryGetNamedRange(addressOrName, out namedRange))
			{
				ExportAsCSV(s, namedRange, encoding);
			}
			else
			{
				throw new InvalidAddressException(addressOrName);
			}
		}

		/// <summary>
		/// Export spreadsheet as CSV format from specified range.
		/// </summary>
		/// <param name="s">Stream to write CSV format as stream.</param>
		/// <param name="range">Range to be output from this worksheet.</param>
		/// <param name="encoding">Text encoding during output text in CSV format.</param>
		public void ExportAsCSV(Stream s, RangePosition range, Encoding encoding = null)
		{
			range = FixRange(range);

			int maxRow = Math.Min(range.EndRow, this.MaxContentRow);
			int maxCol = Math.Min(range.EndCol, this.MaxContentCol);

			if (encoding == null) encoding = Encoding.Default;

			using (var sw = new StreamWriter(s, encoding))
			{
				StringBuilder sb = new StringBuilder();

				for (int r = range.Row; r <= maxRow; r++)
				{
					if (sb.Length > 0)
					{
						sw.WriteLine(sb.ToString());
						sb.Length = 0;
					}

					for (int c = range.Col; c <= maxCol;)
					{
						if (sb.Length > 0)
						{
							sb.Append(',');
						}

						var cell = this.GetCell(r, c);
						if (cell == null || !cell.IsValidCell)
						{
							sb.Append(',');
							c++;
						}
						else
						{
							var data = cell.Data;
							string str = null;
							bool quota = false;

							if (data is string)
							{
								str = (string)data;

								if (str.IndexOf('"') >= 0)
								{
									quota = true;
								}
							}
							else
							{
								str = Convert.ToString(data);
							}

							if (!quota)
							{
								if (cell.DataFormat == CellDataFormatFlag.Text)
								{
									quota = true;
								}
							}

							if (quota)
							{
								sb.Append('"');
								sb.Append(str);
								sb.Append('"');
							}
							else
							{
								sb.Append(str);
							}

							c += cell.Colspan;
						}
					}
				}

				if (sb.Length > 0)
				{
					sw.WriteLine(sb.ToString());
					sb.Length = 0;
				}
			}
		}

		#endregion // Export
	}
}
