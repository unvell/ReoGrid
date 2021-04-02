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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace unvell.ReoGrid.IO
{
	internal static class CSVFormat
	{
		public const int DEFAULT_READ_BUFFER_LINES = 512;
		
		private static Regex lineRegex = new Regex("\\s*(\\\"(?<item>[^\\\"]*)\\\"|(?<item>[^,]*))\\s*,?", RegexOptions.Compiled);

		public static void Read(Stream stream, Worksheet sheet, RangePosition targetRange, 
			Encoding encoding = null, int bufferLines = DEFAULT_READ_BUFFER_LINES, bool autoSpread = true)
		{
			targetRange = sheet.FixRange(targetRange);

			string[] lines = new string[bufferLines];
			List<object>[] bufferLineList = new List<object>[bufferLines];

			for (int i = 0; i < bufferLineList.Length; i++)
			{
				bufferLineList[i] = new List<object>(256);
			}

#if DEBUG
			var sw = System.Diagnostics.Stopwatch.StartNew();
#endif

			int row = targetRange.Row;
			int totalReadLines = 0;

			using (var sr = new StreamReader(stream, encoding))
			{
				sheet.SuspendDataChangedEvents();
				int maxCols = 0;

				try
				{
					bool finished = false;
					while (!finished)
					{
						int readLines = 0;

						for (; readLines < lines.Length; readLines++)
						{
							var line = sr.ReadLine();
							if (line == null)
							{
								finished = true;
								break;
							}

							lines[readLines] = line;

							totalReadLines++;
							if (!autoSpread && totalReadLines > targetRange.Rows)
							{
								finished = true;
								break;
							}
						}

						if (autoSpread && row + readLines > sheet.RowCount)
						{
							int appendRows = bufferLines - (sheet.RowCount % bufferLines);
							if (appendRows <= 0) appendRows = bufferLines;
							sheet.AppendRows(appendRows);
						}

						for (int i = 0; i < readLines; i++)
						{
							var line = lines[i];

							var toBuffer = bufferLineList[i];
							toBuffer.Clear();

							foreach (Match m in lineRegex.Matches(line))
							{
								toBuffer.Add(m.Groups["item"].Value);

								if (toBuffer.Count >= targetRange.Cols) break;
							}

							if (maxCols < toBuffer.Count) maxCols = toBuffer.Count;

							if (autoSpread && maxCols >= sheet.ColumnCount)
							{
								sheet.SetCols(maxCols + 1);
							}
						}

						sheet.SetRangeData(row, targetRange.Col, readLines, maxCols, bufferLineList);
						row += readLines;
					}
				}
				finally
				{
					sheet.ResumeDataChangedEvents();
				}

				sheet.RaiseRangeDataChangedEvent(new RangePosition(
					targetRange.Row, targetRange.Col, maxCols, totalReadLines));
			}

#if DEBUG
			sw.Stop();
			System.Diagnostics.Debug.WriteLine("load csv file: " + sw.ElapsedMilliseconds + " ms, rows: " + row);
#endif
		}
	}

	#region CSV File Provider
	internal class CSVFileFormatProvider : IFileFormatProvider
	{
		public bool IsValidFormat(string file)
		{
			return System.IO.Path.GetExtension(file).Equals(".csv", StringComparison.CurrentCultureIgnoreCase);
		}

		public bool IsValidFormat(Stream s)
		{
			throw new NotSupportedException();
		}

		public void Load(IWorkbook workbook, Stream stream, Encoding encoding, object arg)
		{
			bool autoSpread = true;
			int bufferLines = CSVFormat.DEFAULT_READ_BUFFER_LINES;
			RangePosition targetRange = RangePosition.EntireRange;

			CSVFormatArgument csvArg = arg as CSVFormatArgument;

			if (csvArg != null)
			{
				autoSpread = csvArg.AutoSpread;
				bufferLines = csvArg.BufferLines;
				targetRange = csvArg.TargetRange;
			}
		
			Worksheet sheet = null;

			if (workbook.Worksheets.Count == 0)
			{
				sheet = workbook.CreateWorksheet("Sheet1");
				workbook.Worksheets.Add(sheet);
			}
			else
			{
				while (workbook.Worksheets.Count > 1)
				{
					workbook.Worksheets.RemoveAt(workbook.Worksheets.Count - 1);
				}

				sheet = workbook.Worksheets[0];
				sheet.Reset();
			}

			CSVFormat.Read(stream, sheet, targetRange, encoding, bufferLines, autoSpread);
		}

		public void Save(IWorkbook workbook, Stream stream, Encoding encoding, object arg)
		{
			throw new NotSupportedException("Saving entire workbook as CSV is not supported, use Worksheet.ExportAsCSV instead.");
			//int fromRow = 0, fromCol = 0, toRow = 0, toCol = 0;

			//if (args != null)
			//{
			//	object arg;
			//	if (args.TryGetValue("fromRow", out arg)) fromRow = (int)arg;
			//	if (args.TryGetValue("fromCol", out arg)) fromCol = (int)arg;
			//	if (args.TryGetValue("toRow", out arg)) toRow = (int)arg;
			//	if (args.TryGetValue("toCol", out arg)) toCol = (int)arg;
			//}
		}
	}

	/// <summary>
	/// Arguments for loading and saving CSV format.
	/// </summary>
	public class CSVFormatArgument
	{
		/// <summary>
		/// Determines whether or not allow to expand worksheet to load more data from CSV file. (Default is True)
		/// </summary>
		public bool AutoSpread { get; set; }

		/// <summary>
		/// Determines how many rows read from CSV file one time. (Default is CSVFormat.DEFAULT_READ_BUFFER_LINES = 512)
		/// </summary>
		public int BufferLines { get; set; }

		/// <summary>
		/// Determines the default worksheet name if CSV file loaded into a new workbook. (Default is "Sheet1")
		/// </summary>
		public string SheetName { get; set; }

		/// <summary>
		/// Determines where to start import CSV data on worksheet.
		/// </summary>
		public RangePosition TargetRange { get; set; }

		/// <summary>
		/// Create the argument object instance.
		/// </summary>
		public CSVFormatArgument()
		{
			this.AutoSpread = true;
			this.BufferLines = CSVFormat.DEFAULT_READ_BUFFER_LINES;
			this.SheetName = "Sheet1";
			this.TargetRange = RangePosition.EntireRange;
		}
	}
	#endregion // CSV File Provider

}
