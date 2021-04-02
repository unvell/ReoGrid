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

#if WINFORM
using RGFloat = System.Single;
using WFPaperSize = System.Drawing.Printing.PaperSize;
using WFPageSettings = System.Drawing.Printing.PageSettings;
#elif WPF
using RGFloat = System.Double;
#endif // WPF

namespace unvell.ReoGrid.Print
{
	/// <summary>
	/// Print settings for spreadsheet.
	/// </summary>
	public class PrintSettings : ICloneable
	{
		/// <summary>
		/// Get or set the name of output printer. (Null to use system default printer)
		/// </summary>
		public string PrinterName { get; set; }

		/// <summary>
		/// Determine the orientation to output the print pages.
		/// </summary>
		public PrintPageOrder PageOrder { get; set; }

		/// <summary>
		/// Determine whether or not to show the paper margins during preview and print.
		/// </summary>
		public bool ShowMargins { get; set; }

		/// <summary>
		/// Determines whether or not print grid lines.
		/// </summary>
		public bool ShowGridLines { get; set; }

		/// <summary>
		/// Scale factor for printing each pages (0.1f - 4f).
		/// This property might be changed by adjusting the break lines automatically.
		/// </summary>
		public RGFloat PageScaling { get; set; }

		///// <summary>
		///// Get or set paper size
		///// </summary>
		//public PaperSize PaperSize { get; set; }
		/// <summary>
		/// Get or set the name of this paper type.
		/// </summary>
		public string PaperName {get;set;}

		/// <summary>
		/// Determine the paper orientation (landscape or portrait).
		/// </summary>
		public bool Landscape { get; set; }

		/// <summary>
		/// Get or set the width of the paper, in hundredths of an inch. (only available when PaperSize is custom)
		/// </summary>
		public RGFloat PaperWidth { get; set; }

		/// <summary>
		/// Get or set the height of the paper, in hundredths of an inch. (only available when PaperSize is custom)
		/// </summary>
		public RGFloat PaperHeight { get; set; }

		/// <summary>
		/// Get or set the paper margins.
		/// </summary>
		public PageMargins Margins { get; set; }

		/// <summary>
		/// Construct print settings instance.
		/// </summary>
		public PrintSettings()
		{
			this.PageScaling = 1f;
			this.PageOrder = PrintPageOrder.DownThenOver;
			//this.PaperSize = PaperSize.Letter;
			this.PaperName = Print.PaperSize.Letter.ToString();
			this.PaperWidth = 8.5f;
			this.PaperHeight = 11f;
			this.Landscape = false;
			this.Margins = new PageMargins(1f);
		}

		/// <summary>
		/// Create a copy of current print settings object.
		/// </summary>
		/// <returns>Cloned print settings object.</returns>
		public object Clone()
		{
			return new PrintSettings()
			{
				PrinterName = this.PrinterName,
				PageOrder = this.PageOrder,
				PageScaling = this.PageScaling,
				ShowMargins = this.ShowMargins,
				ShowGridLines = this.ShowGridLines,
				//PaperSize = this.PaperSize,
				PaperName = PaperName,
				PaperWidth = this.PaperWidth,
				PaperHeight = this.PaperHeight,
				Landscape = this.Landscape,
				Margins = this.Margins,
			};
		}

#if WINFORM

		private WFPaperSize FindPaperSize(WFPageSettings wfps)
		{
			var coll = wfps.PrinterSettings.PaperSizes;
			var pSizeName = this.PaperName;

			for (int i = 0; i < coll.Count; i++)
			{
				var wfpsize = coll[i];
				if (string.Compare(wfpsize.PaperName, pSizeName, true) == 0)
				{
					return wfpsize;
				}
			}

			return null;
		}

		/// <summary>
		/// Create a Windows Forms platform page settings and store paper size into it
		/// </summary>
		/// <returns>Page settings used in Windows Form platform</returns>
		public System.Drawing.Printing.PageSettings CreateSystemPageSettings()
		{
			var ps = new System.Drawing.Printing.PageSettings()
			{
				Landscape = this.Landscape,
				Margins = this.Margins,
			};

			ps.PaperSize = FindPaperSize(ps);

			return ps;
		}

		/// <summary>
		/// Read pager size settings from a Windows Forms platform page settings.
		/// </summary>
		/// <param name="pageSettings">Page settings used in Windows Form platform.</param>
		public void ApplySystemPageSettings(System.Drawing.Printing.PageSettings wfps)
		{
			var wfpps = wfps.PaperSize;

			this.PaperName = wfpps.PaperName;
			this.PaperWidth = (RGFloat)Math.Round(wfpps.Width / 100f, 2);
			this.PaperHeight = (RGFloat)Math.Round(wfpps.Height / 100f, 2);

			this.Margins = wfps.Margins;
			this.Landscape = wfps.Landscape;
		}
#endif // WINFORM

	}

	#region Padding
	/// <summary>
	/// Page margin values (in hundredths of an inch)
	/// </summary>
	[Serializable]
	public struct PageMargins
	{
		/// <summary>
		/// Get or set top margin (in hundredths of an inch)
		/// </summary>
		public RGFloat Top { get; set; }

		/// <summary>
		/// Get or set bottom margin (in hundredths of an inch)
		/// </summary>
		public RGFloat Bottom { get; set; }

		/// <summary>
		/// Get or set left margin (in hundredths of an inch)
		/// </summary>
		public RGFloat Left { get; set; }

		/// <summary>
		/// Get or set right margin (in hundredths of an inch)
		/// </summary>
		public RGFloat Right { get; set; }

		/// <summary>
		/// Create and set all values with same value.
		/// </summary>
		/// <param name="all">Value applied to all margins.</param>
		public PageMargins(RGFloat all)
			: this()
		{
			this.Top = all;
			this.Bottom = all;
			this.Left = all;
			this.Right = all;
		}

		/// <summary>
		/// Create with every specified values.
		/// </summary>
		/// <param name="top">Top margin</param>
		/// <param name="bottom">Bottom margin</param>
		/// <param name="left">Left margin</param>
		/// <param name="right">Right margin</param>
		public PageMargins(RGFloat top, RGFloat bottom, RGFloat left, RGFloat right)
			: this()
		{
			this.Top = top;
			this.Bottom = bottom;
			this.Left = left;
			this.Right = right;
		}

		/// <summary>
		/// Predefined empty margin values. (All zero)
		/// </summary>
		public static readonly PageMargins Empty = new PageMargins(0);

		/// <summary>
		/// Compare two margin values whether are same.
		/// </summary>
		/// <param name="p1">Margin value 1 to be compared.</param>
		/// <param name="p2">Margin value 2 to be compared.</param>
		/// <returns>True if two margin values are same; otherwise return false.</returns>
		public static bool operator ==(PageMargins p1, PageMargins p2)
		{
			return p1.Left == p2.Left && p1.Top == p2.Top
				&& p1.Right == p2.Right && p1.Bottom == p2.Bottom;
		}

		/// <summary>
		/// Compare two margin values whether are not same.
		/// </summary>
		/// <param name="p1">Margin value 1 to be compared.</param>
		/// <param name="p2">Margin  value 2 to be compared.</param>
		/// <returns>True if two margin values are not same; otherwise return false.</returns>
		public static bool operator !=(PageMargins p1, PageMargins p2)
		{
			return p1.Left != p2.Left || p1.Top != p2.Top
				|| p1.Right != p2.Right || p1.Bottom != p2.Bottom;
		}

		/// <summary>
		/// Compare an object and check whether two margin value are same.
		/// </summary>
		/// <param name="obj">Another object to be checked.</param>
		/// <returns>True if two margin values are same; otherwise return false.</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is PageMargins)) return false;

			var obj2 = (PageMargins)obj;

			return this.Top == obj2.Top && this.Left == obj2.Left
				&& this.Right == obj2.Right && this.Bottom == obj2.Bottom;
		}

		/// <summary>
		/// Get hash code of this object.
		/// </summary>
		/// <returns>Hash code</returns>
		public override int GetHashCode()
		{
			return (int)(this.Top + this.Left * 10 + this.Right * 100 + this.Bottom * 1000);
		}

		/// <summary>
		/// Convert to Windows Form Margins type.
		/// </summary>
		/// <param name="pv"></param>
		/// <returns></returns>
		public static implicit operator System.Drawing.Printing.Margins(PageMargins pv)
		{
			return new System.Drawing.Printing.Margins(
				(int)Math.Round(pv.Left * 100),
				(int)Math.Round(pv.Right * 100),
				(int)Math.Round(pv.Top * 100),
				(int)Math.Round(pv.Bottom * 100));
		}

		/// <summary>
		/// Convert from Windows Form Margins type.
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		public static implicit operator PageMargins(System.Drawing.Printing.Margins m)
		{
			return new PageMargins(m.Top / 100f, m.Bottom / 100f, m.Left / 100f, m.Right / 100f);
		}
	}
	#endregion

}

#endif // PRINT