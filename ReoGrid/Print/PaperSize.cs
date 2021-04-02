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

#if PRINT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Utility;

#if WINFORM
using RGFloat = System.Single;
using RGSizeF = System.Drawing.SizeF;
#elif WPF
using RGFloat = System.Double;
using RGSizeF = System.Windows.Size;
#endif // WPF

namespace unvell.ReoGrid.Print
{
	/// <summary>
	/// Manages the paper size list for printing spreadsheet.
	/// </summary>
	public sealed class PaperManager
	{
		/// <summary>
		/// Collection of paper size.
		/// </summary>
		public static readonly Dictionary<string, RGSizeF> PaperSizesInch = new Dictionary<string, RGSizeF>() {
			{"A0", new RGSizeF(33.11f, 46.81f)},
			{"A1", new RGSizeF(23.39f, 33.11f)},
			{"A2", new RGSizeF(16.54f, 23.39f)},
			{"A3", new RGSizeF(11.69f, 16.54f)},
			{"A4", new RGSizeF(8.27f, 11.69f)},
			{"A5", new RGSizeF(5.83f, 8.27f)},
			{"A6", new RGSizeF(4.13f, 5.83f)},
			{"A7", new RGSizeF(2.91f, 4.13f)},
			{"A8", new RGSizeF(2.05f, 2.91f)},
			//{PaperSize.A9, new RGSizeF(37f, 52f)},
			//{PaperSize.A10, new RGSizeF(26f, 37f)},
			
			{"B0", new RGSizeF(39.37f, 55.67f)},
			{"B1", new RGSizeF(27.83f, 39.37f)},
			{"B2", new RGSizeF(19.69f, 27.83f)},
			{"B3", new RGSizeF(13.90f, 19.69f)},
			{"B4", new RGSizeF(9.84f, 13.90f)},
			{"B5", new RGSizeF(6.93f, 9.84f)},
			{"B6", new RGSizeF(4.92f, 6.93f)},
			{"B7", new RGSizeF(3.46f, 4.92f)},
			{"B8", new RGSizeF(2.44f, 3.46f)},
			{"B9", new RGSizeF(1.73f, 2.44f)},
			{"B10", new RGSizeF(1.22f, 1.73f)},

			{"C2", new RGSizeF(25.51f, 18.03f)},
			{"C3", new RGSizeF(18.03f, 12.76f)},
			{"C4", new RGSizeF(12.76f, 9.02f)},
			{"C5", new RGSizeF(9.02f, 6.38f)},
			{"C6", new RGSizeF(6.38f, 4.49f)},

			{"D0", new RGSizeF(42.91f, 30.35f)},

			{"SRA0", new RGSizeF(50.39f, 35.43f)},
			{"SRA1", new RGSizeF(35.43f, 25.20f)},
			{"SRA2", new RGSizeF(25.20f, 17.72f)},
			{"SRA3", new RGSizeF(17.72f, 12.60f)},
			{"SRA4", new RGSizeF(12.60f, 8.86f)},
			{"RA0", new RGSizeF(48.03f, 33.86f)},
			{"RA1", new RGSizeF(33.86f, 24.02f)},
			{"RA2", new RGSizeF(24.02f, 16.93f)},

			{"Letter", new RGSizeF(8.5f, 11f)},
			{"Legal", new RGSizeF(8.5f, 14f)},
			{"Ledger", new RGSizeF(11f, 17f)},
			{"Tabloid", new RGSizeF(17f, 11f)},
			{"Executive", new RGSizeF(7.25f, 10.55f)},
			{"ANSI_C", new RGSizeF(22f, 17f)},
			{"ANSI_D", new RGSizeF(34f, 22f)},
			{"ANSI_E", new RGSizeF(44f, 34f)},

			//{PaperSize.GovernmentLetter, new RGSizeF(203.2f, 266.7f)},
			
			{"JIS_B0", new RGSizeF(40.55f, 57.32f)},
			{"JIS_B1", new RGSizeF(28.66f, 40.55f)},
			{"JIS_B2", new RGSizeF(20.28f, 28.66f)},
			{"JIS_B3", new RGSizeF(14.33f, 20.28f)},
			{"JIS_B4", new RGSizeF(10.12f, 14.33f)},
			{"JIS_B5", new RGSizeF(7.17f, 10.12f)},
			{"JIS_B6", new RGSizeF(5.04f, 7.17f)},
			{"JIS_B7", new RGSizeF(3.58f, 5.04f)},
			{"JIS_B8", new RGSizeF(2.52f, 3.58f)},
			{"JIS_B9", new RGSizeF(1.77f, 2.52f)},
			{"JIS_B10", new RGSizeF(1.26f, 1.77f)},
			{"JIS_B11", new RGSizeF(0.87f, 1.26f)},
			{"JIS_B12", new RGSizeF(0.63f, 0.87f)},

			{"Custom", new RGSizeF(8.27f, 11.69f)},
		};

		// a4           210mm x 297mm
		//      inchs   8.2677 in x 11.6929
		//internal static RGSizeF GetA4Pixel()
		//{
		//	RGFloat dpi = PlatformUtility.GetDPI();
		//	return new RGSizeF(dpi * 8.2677f, dpi * 11.6929f);
		//}

		internal static RGSizeF GetPaperSizeValue(PaperSize psize)
		{
			RGSizeF size;

			if (PaperSizesInch.TryGetValue(psize.ToString(), out size))
			{
				return size;
				//return new RGSizeF(MeasureToolkit.CMToInch(size.Width / 10f), MeasureToolkit.CMToInch(size.Height / 10f));
			}
			else
			{
				//RGFloat dpi = PlatformUtility.GetDPI();
				// return default Letter
				return new RGSizeF(8.5f, 11f);
			}
		}
	}

	// ISO Paper Size
	/// <summary>
	/// Perdefiend paper size
	/// </summary>
	public enum PaperSize
	{
		/// <summary>
		/// Detect the default paper size from specified printer
		/// </summary>
		Auto,

#pragma warning disable 1591
		A0, A1, A2, A3, A4, A5, A6, A7, A8, //A9, A10,
		B0, B1, B2, B3, B4, B5, B6, B7, B8, B9, B10,
		C2, C3, C4, C5, C6,
		D0,
		SRA0, SRA1, SRA2, SRA3, SRA4, RA0, RA1, RA2,

		JIS_B0, JIS_B1, JIS_B2, JIS_B3, JIS_B4, JIS_B5,
		JIS_B6, JIS_B7, JIS_B8, JIS_B9, JIS_B10, JIS_B11, JIS_B12,

		/// <summary>
		/// ANSI Letter (8.5 x 11 inches, 216 mm x 279 mm)
		/// </summary>
		Letter,

		Legal, Ledger, Tabloid, Executive, ANSI_C, ANSI_D, ANSI_E,
#pragma warning restore 1591

		///// <summary>
		///// Government Letter Size (203 mm x 267 mm)
		///// </summary>
		//GovernmentLetter,
		/// <summary>
		/// Specify to use customize pager size
		/// </summary>
		Custom,	
	}

}

#endif // PRINT