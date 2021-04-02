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

#if WPF

using System;
using System.Windows;
using System.Windows.Media;

using unvell.Common;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Interaction;

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
			return Toolkit.IsKeyDown((Common.Win32Lib.Win32.VKey)key);
		}

		private static double lastGetDPI = 0;

		public static double GetDPI()
		{
			if (lastGetDPI == 0)
			{
				Window win = new Window();
				PresentationSource source = PresentationSource.FromVisual(win);

				if (source != null)
				{
					lastGetDPI = 96.0 * source.CompositionTarget.TransformToDevice.M11;
				}

				win.Close();

				if (lastGetDPI == 0) lastGetDPI = 96;
			}

			return lastGetDPI;
		}

		public static Typeface GetFontDefaultTypeface(FontFamily ff)
		{
			var typefaces = ff.GetTypefaces();
			if (typefaces.Count > 0)
			{
				var iterator = typefaces.GetEnumerator();
				if (iterator.MoveNext())
				{
					return iterator.Current;
				}
			}

			return null;
		}

		private static ResourcePoolManager resourcePoolManager;// = new ResourcePoolManager();

		internal static Graphics.Size MeasureText(IRenderer r, string text, string fontName, double fontSize, Drawing.Text.FontStyles style)
		{
			ResourcePoolManager resManager;
			Typeface typeface = null;

			if (r == null)
			{
				if (resourcePoolManager == null) resourcePoolManager = new ResourcePoolManager();

				resManager = resourcePoolManager;
			}
			else
			{
				resManager = r.ResourcePoolManager;
			}

			typeface = resManager.GetTypeface(fontName, FontWeights.Regular, ToWPFFontStyle(style), FontStretches.Normal);

			if (typeface == null)
			{
				return Graphics.Size.Zero;
			}

			//var typeface = new System.Windows.Media.Typeface(
			//			new System.Windows.Media.FontFamily(fontName),
			//			PlatformUtility.ToWPFFontStyle(this.fontStyles),
			//			(this.fontStyles & FontStyles.Bold) == FontStyles.Bold ?
			//			System.Windows.FontWeights.Bold : System.Windows.FontWeights.Normal,
			//			System.Windows.FontStretches.Normal);

			System.Windows.Media.GlyphTypeface glyphTypeface;

			double totalWidth = 0;

			if (typeface.TryGetGlyphTypeface(out glyphTypeface))
			{
				//fontInfo.Ascent = typeface.FontFamily.Baseline;
				//fontInfo.LineHeight = typeface.CapsHeight;

				var size = fontSize * 1.33d;

				//this.GlyphIndexes.Capacity = text.Length;

				for (int n = 0; n < text.Length; n++)
				{
					ushort glyphIndex = glyphTypeface.CharacterToGlyphMap[text[n]];
					//GlyphIndexes.Add(glyphIndex);

					double width = glyphTypeface.AdvanceWidths[glyphIndex] * size;
					//this.TextSizes.Add(width);

					totalWidth += width;
				}
			}

			return new Graphics.Size(totalWidth, typeface.CapsHeight);
		}

		public static System.Windows.FontStyle ToWPFFontStyle(unvell.ReoGrid.Drawing.Text.FontStyles textStyle)
		{
			if ((textStyle & Drawing.Text.FontStyles.Italic) == Drawing.Text.FontStyles.Italic)
			{
				return System.Windows.FontStyles.Italic;
			}
			else
				return System.Windows.FontStyles.Normal;
		}

		public static TextDecorationCollection ToWPFFontDecorations(unvell.ReoGrid.Drawing.Text.FontStyles textStyle)
		{
			var decorations = new TextDecorationCollection();

			if ((textStyle & Drawing.Text.FontStyles.Underline) == Drawing.Text.FontStyles.Underline)
			{
				decorations.Add(TextDecorations.Underline);
			}

			if ((textStyle & Drawing.Text.FontStyles.Strikethrough) == Drawing.Text.FontStyles.Strikethrough)
			{
				decorations.Add(TextDecorations.Strikethrough);
			}

			return decorations;
		}

		public static System.Windows.Media.PenLineCap ToWPFLineCap(unvell.ReoGrid.Graphics.LineCapStyles capStyle)
		{
			switch (capStyle)
			{
				default:
				case LineCapStyles.None:
					return PenLineCap.Flat;

				case LineCapStyles.Arrow:
					return PenLineCap.Triangle;

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

		internal static readonly SolidColor SystemColor_Highlight = System.Windows.SystemColors.HighlightColor;
		internal static readonly SolidColor SystemColor_Window = System.Windows.SystemColors.WindowColor;
		internal static readonly SolidColor SystemColor_WindowText = System.Windows.SystemColors.WindowTextColor;
		internal static readonly SolidColor SystemColor_Control = System.Windows.SystemColors.ControlColor;
		internal static readonly SolidColor SystemColor_ControlLight = System.Windows.SystemColors.ControlLightColor;
		internal static readonly SolidColor SystemColor_ControlDark = System.Windows.SystemColors.ControlDarkColor;

		internal static readonly Pen Gray = new Pen(new SolidColorBrush(Colors.Gray), 1f);
	}
	#endregion // StaticResources
}

#endif
