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
 * ReoGrid and ReoGridEditor is released under MIT license.
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;

using unvell.Common;
using unvell.Common.Win32Lib;

namespace unvell.Common
{
	/// <summary>
	/// Font toolkit
	/// </summary>
	internal static class FontUIToolkit
	{
		internal static bool Has(this FontStyle flag, FontStyle check)
		{
			return (flag & check) == check;
		}
		internal static bool HasAny(this FontStyle flag, FontStyle check)
		{
			return (flag & check) > 0;
		}
		
		/// <summary>
		/// Join specified font style names
		/// </summary>
		/// <param name="style">font style flag</param>
		/// <param name="regularText">name for regular style</param>
		/// <param name="italicText">name for italic style</param>
		/// <param name="boldText">name for bold style</param>
		/// <returns>joined font style</returns>
		public static string GetFontStyleName(FontStyle style, string regularText,
			string italicText, string boldText)
		{
			List<string> names = new List<string>();

			if (style.Has(FontStyle.Bold))
			{
				names.Add(boldText);
			}
			if (style.Has(FontStyle.Italic))
			{
				names.Add(italicText);
			}

			if (names.Count == 0)
				return regularText;
			else
				return string.Join(" ", names.ToArray());
		}

		/// <summary>
		/// Convert font name string to font style flags
		/// </summary>
		/// <param name="text">font style names</param>
		/// <param name="italicText">name for italic style</param>
		/// <param name="boldText">name for bold style</param>
		/// <returns>font style flag</returns>
		public static FontStyle GetFontStyleByName(string text, string italicText, string boldText)
		{
			string[] styleNames = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			FontStyle fs = FontStyle.Regular;

			foreach (string style in styleNames)
			{
				if (string.Compare(style, italicText, true) == 0)
				{
					fs |= FontStyle.Italic;
				}
				else if (string.Compare(style, boldText, true) == 0)
				{
					fs |= FontStyle.Bold;
				}
			}

			return fs;
		}

		public static readonly float[] FontSizeList = new float[] {
            5f, 6f, 7f, 8f, 9f, 10f, 10.5f, 11f, 11.5f, 12f, 12.5f, 14f, 16f, 18f,
            20f, 22f, 24f, 26f, 28f, 30f, 32f, 34f, 38f, 46f, 58f, 64f, 78f, 92f};

		private static readonly float FixedDrawFontSize = 14f;

		public static void DrawFontItem(Graphics g, FontFamilyInfo fontFamily,
			Rectangle rect, bool isSelected)
		{
			using (StringFormat sf = new StringFormat()
			{
				Alignment = StringAlignment.Near,
				LineAlignment = StringAlignment.Center,
			})
			{
				sf.FormatFlags |= StringFormatFlags.NoWrap;

				using (FontFamily ff = new FontFamily(fontFamily.CultureName))
				{
					Font font = null;

					// some fonts are not support Regular style, we need find 
					// a style which is supported by current font

					if (ff.IsStyleAvailable(FontStyle.Regular))
						font = new Font(fontFamily.CultureName, FixedDrawFontSize, FontStyle.Regular);
					else if (ff.IsStyleAvailable(FontStyle.Bold))
						font = new Font(fontFamily.CultureName, FixedDrawFontSize, FontStyle.Bold);
					else if (ff.IsStyleAvailable(FontStyle.Italic))
						font = new Font(fontFamily.CultureName, FixedDrawFontSize, FontStyle.Italic);
					else if (ff.IsStyleAvailable(FontStyle.Strikeout))
						font = new Font(fontFamily.CultureName, FixedDrawFontSize, FontStyle.Strikeout);
					else if (ff.IsStyleAvailable(FontStyle.Underline))
						font = new Font(fontFamily.CultureName, FixedDrawFontSize, FontStyle.Underline);

					if (font != null)
					{
						g.DrawString(font.Name, font,
							isSelected ? SystemBrushes.HighlightText : Brushes.Black, rect, sf);

						font.Dispose();
					}
				}
			}
		}

		public static float GetLargerSize(float size)
		{
			return (size >= FontUIToolkit.FontSizeList.Max()) ? size : FontUIToolkit.FontSizeList.Where(fs => fs > size).Min();
		}

		public static float GetSmallerSize(float size)
		{
			return (size >= FontUIToolkit.FontSizeList.Max()) ? size : FontUIToolkit.FontSizeList.Where(fs => fs < size).Max();
		}

		public static Font GetFontIfExisted(string name, float size, FontStyle style)
		{
			Font f = null;
			try
			{
				FontFamily ff = new FontFamily(name);
				if (!ff.IsStyleAvailable(style)) style = FontStyle.Regular;
				f = new Font(ff, size, style);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("info: " + ex.Message, "font toolkit");

				f = SystemFonts.DefaultFont;
			}
			return f;
		}

		public static FontFamily GetFontFamilyIfExisted(string name)
		{
			try
			{
				return new FontFamily(name);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("font family does not exist: " + ex.Message, "font toolkit");

				return null;
			}
		}

		public static bool AreCloseSize(float s1, float s2)
		{
			return Math.Abs(s1 - s2) < 0.3f;
		}
	}

	public class FontFamilyInfo
	{
		private string[] names;
		public string[] Names { get { return names; } }

		public FontFamily FontFamily { get; private set; }

		public FontFamilyInfo(FontFamily fontFamily, params string[] names)
		{
			this.FontFamily = fontFamily;
			this.cultureName = fontFamily.Name;
			this.names = names;
		}

		private string cultureName;

		public string CultureName
		{
			get
			{
				return cultureName; // TODO: get culture name reference to OS
			}
		}

		public bool IsFamilyName(string name)
		{
			return names == null || names.Any(n => n.StartsWith(name, StringComparison.CurrentCultureIgnoreCase));
		}

		public override string ToString()
		{
			return cultureName;
		}
	}

	public class FontInfo
	{
		private FontFamilyInfo fontFamilyInfo;

		public FontFamilyInfo FontFamilyInfo
		{
			get { return fontFamilyInfo; }
			set { fontFamilyInfo = value; }
		}

		public string Name { get { return fontFamilyInfo.FontFamily.Name; } }

		private float size;
		public float Size { get { return size; } set { size = value; } }

		private FontStyle style;
		public FontStyle Style { get { return style; } set { style = value; } }
		
		public FontInfo(Font font)
			: this(font.Name, font.Size, font.Style)
		{
		}

		public FontInfo(FontInfo proto)
		{
			this.fontFamilyInfo = proto.fontFamilyInfo;
			this.size = proto.size;
			this.style = proto.style;
		}

		public FontInfo(string name, float size, FontStyle style)
			: this(new FontFamilyInfo(new FontFamily(name)), size, style)
		{
		}

		public FontInfo(FontFamilyInfo fontFamily, float size, FontStyle style)
		{
#if DEBUG
			Debug.Assert(fontFamilyInfo != null);
#endif

			this.fontFamilyInfo = fontFamily;
			this.size = size;
			this.style = style;
		}

		public override string ToString()
		{
			return Name;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is FontInfo)) return false;
			FontInfo f2 = (FontInfo)obj;
			return string.Equals(Name, f2.Name, StringComparison.CurrentCultureIgnoreCase)
				&& size == f2.size && style == f2.style;
		}

		public bool Equals(string name, float size, FontStyle fontStyle)
		{
			return string.Equals(Name, name, StringComparison.CurrentCultureIgnoreCase)
				&& this.size == size
				&& this.style == fontStyle;
		}

		public bool Equals(Font font)
		{
			return font != null && Equals(font.Name, font.Size, font.Style);
		}

		public override int GetHashCode()
		{
			return fontFamilyInfo.GetHashCode() ^ (int)size ^ (int)style;
		}
	}

}
