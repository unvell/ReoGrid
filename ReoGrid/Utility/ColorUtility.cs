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

using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.Utility
{

	static internal class ColorUtility
	{
		/// <summary>
		/// Source: http://ciintelligence.blogspot.jp/2012/02/converting-excel-theme-color-and-tint.html
		/// </summary>
		/// <param name="rgbColor"></param>
		/// <returns></returns>
		public static HSLColor RGBToHSL(SolidColor rgbColor)
		{
			var hslColor = new HSLColor();

			float r = (float)rgbColor.R / 255f;
			float g = (float)rgbColor.G / 255f;
			float b = (float)rgbColor.B / 255f;
			float a = (float)rgbColor.A / 255f;
			float min = Math.Min(r, Math.Min(g, b));
			float max = Math.Max(r, Math.Max(g, b));
			float delta = max - min;

			hslColor.A = a;

			if (max == min)
			{
				hslColor.H = 0;
				hslColor.S = 0;
				hslColor.L = max;
				return hslColor;
			}

			hslColor.L = (min + max) / 2f;

			if (hslColor.L < 0.5f)
			{
				hslColor.S = delta / (max + min);
			}
			else
			{
				hslColor.S = delta / (2.0f - max - min);
			}

			if (r == max) hslColor.H = (g - b) / delta;
			if (g == max) hslColor.H = 2.0f + (b - r) / delta;
			if (b == max) hslColor.H = 4.0f + (r - g) / delta;

			hslColor.H *= 60f;
			if (hslColor.H < 0) hslColor.H += 360f;

			return hslColor;
		}

		/// <summary>
		/// Source: http://ciintelligence.blogspot.jp/2012/02/converting-excel-theme-color-and-tint.html
		/// </summary>
		/// <param name="hslColor"></param>
		/// <returns></returns>
		public static SolidColor HSLToRgb(HSLColor hslColor)
		{
			var rgbColor = new SolidColor();

			if (hslColor.S == 0)
			{
				rgbColor = SolidColor.FromArgb((byte)(hslColor.A * 255f), (byte)(hslColor.L * 255f),
					(byte)(hslColor.L * 255f), (byte)(hslColor.L * 255f));

				return rgbColor;
			}

			float t1;

			if (hslColor.L < 0.5f)
			{
				t1 = (float)(hslColor.L * (1.0f + hslColor.S));
			}

			else
			{
				t1 = (float)(hslColor.L + hslColor.S - (hslColor.L * hslColor.S));
			}

			float t2 = 2.0f * hslColor.L - t1;
			float h = hslColor.H / 360f;
			float tR = h + (1.0f / 3.0f);
			float r = SetColor(t1, t2, tR);
			float tG = h;
			float g = SetColor(t1, t2, tG);
			float tB = h - (1.0f / 3.0f);
			float b = SetColor(t1, t2, tB);
			rgbColor = SolidColor.FromArgb((int)(hslColor.A * 255), (int)(r * 255), (int)(g * 255), (int)(b * 255));
			return rgbColor;

		}

		/// <summary>
		/// Source: http://ciintelligence.blogspot.jp/2012/02/converting-excel-theme-color-and-tint.html
		/// </summary>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		/// <param name="t3"></param>
		/// <returns></returns>
		private static float SetColor(float t1, float t2, float t3)
		{
			if (t3 < 0) t3 += 1.0f;
			if (t3 > 1) t3 -= 1.0f;

			float color;

			if (6.0f * t3 < 1.0f)
			{
				color = t2 + (t1 - t2) * 6.0f * t3;
			}

			else if (2.0f * t3 < 1f)
			{
				color = t1;
			}

			else if (3.0f * t3 < 2f)
			{
				color = t2 + (t1 - t2) * ((2.0f / 3.0f) - t3) * 6.0f;
			}
			else
			{
				color = t2;
			}

			// Set return value 
			return color;
		}

		/// <summary>
		/// Source: http://ciintelligence.blogspot.jp/2012/02/converting-excel-theme-color-and-tint.html
		/// </summary>
		/// <param name="tint"></param>
		/// <param name="lum"></param>
		/// <returns></returns>
		public static float CalculateFinalLumValue(float tint, float lum)
		{
			float lum1 = 0;

			if (tint < 0)
			{
				lum1 = lum * (1.0f + tint);
			}
			else
			{
				lum1 = lum * (1.0f - tint) + (255f - 255f * (1.0f - tint));
			}

			return lum1;
		}

		public static SolidColor LightColor(SolidColor color) { return ChangeColorBrightness(color, 0.1f); }
		public static SolidColor LightLightColor(SolidColor color) { return ChangeColorBrightness(color, 0.2f); }
		public static SolidColor LightLightLightColor(SolidColor color) { return ChangeColorBrightness(color, 0.3f); }

		public static SolidColor DarkColor(SolidColor color) { return ChangeColorBrightness(color, -0.1f); }
		public static SolidColor DarkDarkColor(SolidColor color) { return ChangeColorBrightness(color, -0.2f); }
		public static SolidColor DarkDarkDarkColor(SolidColor color) { return ChangeColorBrightness(color, -0.3f); }

		public static SolidColor ChangeColorBrightness(SolidColor color, float brightnessFactor)
		{
			var hsl = ColorUtility.RGBToHSL(color);
			hsl.L += brightnessFactor;

			if (hsl.L > 1) hsl.L = 1;
			else if (hsl.L < 0) hsl.L = 0;

			return ColorUtility.HSLToRgb(hsl);
			//short r = color.R;
			//short g = color.G;
			//short b = color.B;

			//r = (short)(r * brightnessFactor + 10*brightnessFactor);
			//g = (short)(g * brightnessFactor + 10 * brightnessFactor);
			//b = (short)(b * brightnessFactor + 10 * brightnessFactor);

			//if (r < 0) r = 0;
			//if (g < 0) g = 0;
			//if (b < 0) b = 0;
			//if (r > 255) r = 255;
			//if (g > 255) g = 255;
			//if (b > 255) b = 255;

			//return new SolidColor(255, r, g, b);
		}

		public static SolidColor FromAlphaColor(byte alpha, SolidColor color)
		{
			return new SolidColor(alpha, color.R, color.G, color.B);
		}

		public static SolidColor FromARGBValue(long value)
		{
			SolidColor c = SolidColor.FromArgb((byte)((value >> 24) & 0xff),
				(byte)((value >> 16) & 0xff), (byte)((value >> 8) & 0xff), (byte)((value & 0xff)));

			System.Diagnostics.Debug.Assert(c.A != 0);

			return c;
		}

	}

	internal struct HSLColor
	{
		public float A;
		public float H;
		public float L;
		public float S;

		public override string ToString()
		{
			return string.Format("HSL({0},{1},{2},{3})", this.A, this.H, this.L, this.S);
		}
	}

}
