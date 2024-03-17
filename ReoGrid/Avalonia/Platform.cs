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
 * Author: Jingwood <jingwood at unvell.com>
 *
 * Copyright (c) 2012-2023 Jingwood <jingwood at unvell.com>
 * Copyright (c) 2012-2023 unvell inc. All rights reserved.
 * 
 ****************************************************************************/

#if AVALONIA

using Avalonia.Controls;
using Avalonia.Media;
using System;
#if !AVALONIA
using System.Drawing;
#endif
using System.Globalization;
using System.Linq;
using unvell.Common;
using unvell.ReoGrid.Drawing.Text;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Interaction;
using FontFamily = Avalonia.Media.FontFamily;
using FontStyle = Avalonia.Media.FontStyle;
using Pen = Avalonia.Media.Pen;

namespace unvell.ReoGrid
{
	partial class Cell
	{
		[NonSerialized]
		internal FormattedText formattedText;
	}
}

namespace unvell.ReoGrid.Rendering
{
	#region PlatformUtility
	partial class PlatformUtility
	{
		internal static bool IsKeyDown(KeyCode key)
		{
		//	return Toolkit.IsKeyDown((Common.Win32Lib.Win32.VKey)key);
        return false;
        }

		private static double lastGetDPI = 0;

		public static double GetDPI()
		{
			if (lastGetDPI == 0)
			{
				//Window win = new Window();
				//PresentationSource source = PresentationSource.FromVisual(win);
				

				//if (source != null)
				//{
				//	lastGetDPI = 96.0 * source.CompositionTarget.TransformToDevice.M11;
				//}

				//win.Close();

				if (lastGetDPI == 0) lastGetDPI = 96;
			}

			return lastGetDPI;
		}

		public static Typeface? GetFontDefaultTypeface(FontFamily ff)
		{
			var typefaces =  ff.FamilyNames;
			if (typefaces.Count > 0)
			{
				var iterator = typefaces.GetEnumerator();
				if (iterator.MoveNext())
				{
					return resourcePoolManager.GetTypeface(iterator.Current);
				}
			}

			return null;
		}

		//public static (Typeface?, IGlyphTypeface) FindTypefaceContainsCharacter(char ch, CultureInfo ci = null)
		//{
		//	if (ci == null)
		//	{
		//		ci = CultureInfo.CurrentCulture;
		//	}

		//	foreach (var font in Avalonia.Media.Fonts.SystemFontFamilies)
		//	{
		//		foreach (var typeface in font.GetTypefaces())
		//		{
		//			if (typeface.TryGetGlyphTypeface(out var glyphTypeface))
		//			{
		//				if (glyphTypeface.FaceNames.ContainsKey(ci))
		//				{
		//					if (glyphTypeface.CharacterToGlyphMap.ContainsKey(ch))
		//					{
		//						return (typeface, glyphTypeface);
		//					}
		//				}
		//			}
		//		}
		//	}

		//	return (null, null);
		//}

        private static ResourcePoolManager resourcePoolManager = new ResourcePoolManager();

		internal static Graphics.Size MeasureText(IRenderer r, string text, string fontName, double fontSize, Drawing.Text.FontStyles style)
		{
			ResourcePoolManager resManager;
            Typeface? typeface = null;

			if (r == null)
			{
				if (resourcePoolManager == null) resourcePoolManager = new ResourcePoolManager();

				resManager = resourcePoolManager;
			}
			else
			{
				resManager = r.ResourcePoolManager;
			}

			typeface = resManager.GetTypeface(fontName, FontWeight.Regular, ToAvaloniaFontStyle(style), FontStretch.Normal);

			if (typeface == null)
			{
				return Graphics.Size.Zero;
			}

			typeface = new Avalonia.Media.Typeface(
						new Avalonia.Media.FontFamily(fontName),
						PlatformUtility.ToAvaloniaFontStyle(style),
						(style & FontStyles.Bold) == FontStyles.Bold ?
						FontWeight.Bold : FontWeight.Normal,
						FontStretch.Normal);

			IGlyphTypeface glyphTypeface;

            double width = 0;
			double height = 0;
			if (FontManager.Current.TryGetGlyphTypeface(typeface??Typeface.Default, out glyphTypeface))
			{
				//fontInfo.Ascent = typeface.FontFamily.Baseline;
				//fontInfo.LineHeight = typeface.CapsHeight;

				var size = fontSize * 1.33d;

				var glyphIndexs = text.Select(ch => glyphTypeface.GetGlyph(ch)).ToArray() ;

                GlyphRun run = new GlyphRun(glyphTypeface, size,text.AsMemory(), glyphIndexs);
				width = run.Bounds.Size.Width;
				height = run.Bounds.Size.Height * 1.33d;

				//run.Bounds.Size;
				//this.GlyphIndexes.Capacity = text.Length;

				//for (int n = 0; n < text.Length; n++)
				//{
				//	ushort glyphIndex = glyphTypeface.GetGlyphs(text[n]);
				//	//GlyphIndexes.Add(glyphIndex);

				//	double width = glyphTypeface.GetGlyphAdvance(glyphIndex) * size;
				//	//this.TextSizes.Add(width);

				//	totalWidth += width;
				//}
			}

			return new Graphics.Size(width, height);
		}

		public static FontStyle ToAvaloniaFontStyle(unvell.ReoGrid.Drawing.Text.FontStyles textStyle)
		{
			if ((textStyle & Drawing.Text.FontStyles.Italic) == Drawing.Text.FontStyles.Italic)
			{
				return FontStyle.Italic;
			}
			else
				return FontStyle.Normal;
		}

		public static TextDecorationCollection ToWPFFontDecorations(unvell.ReoGrid.Drawing.Text.FontStyles textStyle)
		{
			var decorations = new TextDecorationCollection();

			if ((textStyle & Drawing.Text.FontStyles.Underline) == Drawing.Text.FontStyles.Underline)
			{
				decorations.AddRange(TextDecorations.Underline);
			}

			if ((textStyle & Drawing.Text.FontStyles.Strikethrough) == Drawing.Text.FontStyles.Strikethrough)
			{
				decorations.AddRange(TextDecorations.Strikethrough);
			}

			return decorations;
		}

		public static PenLineCap ToWPFLineCap(unvell.ReoGrid.Graphics.LineCapStyles capStyle)
		{
			switch (capStyle)
			{
				default:
				case LineCapStyles.None:
					return PenLineCap.Flat;

				case LineCapStyles.Arrow:
					return PenLineCap.Round;

				case LineCapStyles.Round:
				case LineCapStyles.Ellipse:
					return PenLineCap.Round;
			}
		}
	}
	#endregion // PlatformUtility

	#region StaticResources
	partial class StaticResources
	{
		//private static string systemDefaultFontName = null;
		//internal static string SystemDefaultFontName
		//{
		//	get
		//	{
		//		if (systemDefaultFontName == null)
		//		{
		//			var names = System.Windows.SystemFonts.MessageFontFamily.FamilyNames;

		//			systemDefaultFontName = names.Count > 0 ? names[System.Windows.Markup.XmlLanguage.GetLanguage(string.Empty)] : string.Empty;
		//			//var typeface = ResourcePoolManager.Instance.GetTypeface(
		//		}

		//		return systemDefaultFontName;
		//	}
		//}
		//internal static double SystemDefaultFontSize = System.Drawing.SystemFonts.DefaultFont.Size * 72.0 / PlatformUtility.GetDPI();

		internal static readonly SolidColor SystemColor_Highlight = SystemColors.Highlight;
		internal static readonly SolidColor SystemColor_Window = SystemColors.Window;
		internal static readonly SolidColor SystemColor_WindowText = SystemColors.WindowText;
		internal static readonly SolidColor SystemColor_Control = SystemColors.Control;
		internal static readonly SolidColor SystemColor_ControlLight = SystemColors.ControlLight;
		internal static readonly SolidColor SystemColor_ControlDark = SystemColors.ControlDark;
	}
	#endregion // StaticResources
}

#endif
