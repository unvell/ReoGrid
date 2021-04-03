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
using unvell.ReoGrid.Utility;

namespace unvell.ReoGrid.Graphics
{
	/// <summary>
	/// Color interface
	/// </summary>
	public interface IColor
	{
		/// <summary>
		/// Determines whether or not this color is fully transparent.
		/// </summary>
		bool IsTransparent { get; }

		/// <summary>
		/// Get solid color converted from this color interface.
		/// </summary>
		/// <returns></returns>
		SolidColor ToSolidColor();
	}

	/// <summary>
	/// Represents 32 bits ARGB format solid color. (0~255)
	/// </summary>
	[Serializable]
	public struct SolidColor : IColor
	{
		/// <summary>
		/// Get or set alpha value.
		/// </summary>
		public byte A { get; set; }

		/// <summary>
		/// Get or set red value.
		/// </summary>
		public byte R { get; set; }

		/// <summary>
		/// Get or set green value.
		/// </summary>
		public byte G { get; set; }

		/// <summary>
		/// Get or set blue value.
		/// </summary>
		public byte B { get; set; }

		/// <summary>
		/// Construct color value with specified ARGB value.
		/// </summary>
		/// <param name="a">Alpha value.</param>
		/// <param name="r">Red value.</param>
		/// <param name="g">Green value.</param>
		/// <param name="b">Blue value.</param>
		public SolidColor(int a, int r, int g, int b)
			: this()
		{
			if (a > 255) a = 255; else if (a < 0) a = 0;
			if (r > 255) r = 255; else if (r < 0) r = 0;
			if (g > 255) g = 255; else if (g < 0) g = 0;
			if (b > 255) b = 255; else if (b < 0) b = 0;

			this.A = (byte)a;
			this.R = (byte)r;
			this.G = (byte)g;
			this.B = (byte)b;
		}

		/// <summary>
		/// Construct color value with specified ARGB value. (0-255)
		/// </summary>
		/// <param name="a">Alpha value.</param>
		/// <param name="r">Red value.</param>
		/// <param name="g">Green value.</param>
		/// <param name="b">Blue value.</param>
		public SolidColor(byte a, byte r, byte g, byte b)
			: this()
		{
			this.A = a;
			this.R = r;
			this.G = g;
			this.B = b;
		}

		/// <summary>
		/// Construct color value with specified ARGB value. (0-255)
		/// </summary>
		/// <param name="r">Red value.</param>
		/// <param name="g">Green value.</param>
		/// <param name="b">Blue value.</param>
		public SolidColor(int r, int g, int b)
			: this()
		{
			this.A = 255;

			if (r > 255) r = 255; else if (r < 0) r = 0;
			if (g > 255) g = 255; else if (g < 0) g = 0;
			if (b > 255) b = 255; else if (b < 0) b = 0;

			this.R = (byte)r;
			this.G = (byte)g;
			this.B = (byte)b;
		}

		/// <summary>
		/// Construct color value with specified ARGB value. (0-255)
		/// </summary>
		/// <param name="r">Red value.</param>
		/// <param name="g">Green value.</param>
		/// <param name="b">Blue value.</param>
		public SolidColor(byte r, byte g, byte b)
			: this()
		{
			this.A = 255;
			this.R = r;
			this.G = g;
			this.B = b;
		}

		/// <summary>
		/// Create color with specified alpha factor and another solid color.
		/// </summary>
		/// <param name="alpha">Alpha factor used to decide the transparency of this color.</param>
		/// <param name="rgb">Another solid color used to create this color.</param>
		public SolidColor(int alpha, SolidColor rgb)
			: this()
		{
			if (alpha > 255) alpha = 255; else if (alpha < 0) alpha = 0;
			this.A = (byte)alpha;

			this.R = rgb.R;
			this.G = rgb.G;
			this.B = rgb.B;
		}

		/// <summary>
		/// Create color from hex format string. (e.g. AARRGGBB or #AARRGGBB)
		/// </summary>
		/// <param name="hex"></param>
		public SolidColor(string hex)
		{
			SolidColor color;
			Common.TextFormatHelper.DecodeColor(hex, out color);
			this = color;
		}

		/// <summary>
		/// Convert color value to 4 bytes integer value.
		/// </summary>
		/// <returns>Converted 4 bytes integer value.</returns>
		public int ToArgb()
		{
			return (this.A << 24) | (this.R << 16) | (this.G << 8) | (this.B << 0);
		}

		/// <summary>
		/// Check whether or not this color is transparent.
		/// </summary>
		public bool IsTransparent
		{
			get { return this.A == 0; }
		}

		/// <summary>
		/// Create color with specified ARGB values.
		/// </summary>
		/// <param name="a">Alpha value.</param>
		/// <param name="r">Red value.</param>
		/// <param name="g">Green value.</param>
		/// <param name="b">Blue value.</param>
		/// <returns>Converted color value.</returns>
		public static SolidColor FromArgb(int a, int r, int g, int b)
		{
			return new SolidColor(a, r, g, b);
		}

		/// <summary>
		/// Create color with specified ARGB values.
		/// </summary>
		/// <param name="a">Alpha value.</param>
		/// <param name="r">Red value.</param>
		/// <param name="g">Green value.</param>
		/// <param name="b">Blue value.</param>
		/// <returns>Converted color value.</returns>
		public static SolidColor FromArgb(byte a, byte r, byte g, byte b)
		{
			return new SolidColor(a, r, g, b);
		}

		/// <summary>
		/// Create color with specified RGB values.
		/// </summary>
		/// <param name="r">Red value.</param>
		/// <param name="g">Green value.</param>
		/// <param name="b">Blue value.</param>
		/// <returns>Converted color value.</returns>
		public static SolidColor FromArgb(int r, int g, int b)
		{
			return new SolidColor(r, g, b);
		}

		/// <summary>
		/// Create color with specified RGB values.
		/// </summary>
		/// <param name="r">Red value.</param>
		/// <param name="g">Green value.</param>
		/// <param name="b">Blue value.</param>
		/// <returns>Converted color value.</returns>
		public static SolidColor FromArgb(byte r, byte g, byte b)
		{
			return new SolidColor(r, g, b);
		}

		/// <summary>
		/// Construct color value with merged ARGB int value.
		/// </summary>
		/// <param name="argb">Merged ARGB int value to create color object.</param>
		public static SolidColor FromArgb(int argb)
		{
			return new SolidColor((byte)(argb >> 24),
				(byte)((argb >> 16) & 0xff),
				(byte)((argb >> 8) & 0xff),
				(byte)(argb & 0xff));
		}

		/// <summary>
		/// Construct color value with merged RGB int value.
		/// </summary>
		/// <param name="rgb">Merged RGB value to create color object.</param>
		public static SolidColor FromRGB(int rgb)
		{
			return new SolidColor((byte)255,
				(byte)((rgb >> 16) & 0xff),
				(byte)((rgb >> 8) & 0xff),
				(byte)(rgb & 0xff));
		}

		/// <summary>
		/// Convert to friendly description.
		/// </summary>
		/// <returns>Description of color.</returns>
		public override string ToString()
		{
			return string.Format("Color({0}, {1}, {2}, {3})", this.A, this.R, this.G, this.B);
		}

		/// <summary>
		/// Convert to friendly description. (e.g. A, R, G, B)
		/// </summary>
		/// <returns></returns>
		public string ToStringARGB()
		{
			return string.Format("{0}, {1}, {2}, {3}", this.A, this.R, this.G, this.B);
		}

		/// <summary>
		/// Convert to friendly description. (e.g. AABBCCDD)
		/// </summary>
		/// <returns></returns>
		public string ToStringHex()
		{
			return string.Format("{0:x2}{1:x2}{2:x2}{3:x2}", this.A, this.R, this.G, this.B);
		}

		/// <summary>
		/// Convert to solid color. (equal to self)
		/// </summary>
		/// <returns>Converted solid color.</returns>
		public SolidColor ToSolidColor()
		{
			return this;
		}

		private static Random rand;

		/// <summary>
		/// Randomly generate a color.
		/// </summary>
		/// <returns>New random solid color.</returns>
		public static SolidColor Randomly()
		{
			if (rand == null) rand = new Random();
			return new SolidColor((byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255));
		}

		/// <summary>
		/// Returns a lighter color of specified color.
		/// </summary>
		/// <param name="color">Original color to be changed.</param>
		/// <returns>Lighter color.</returns>
		public static SolidColor Light(SolidColor color)
		{
			return SetBrightness(0.3f, color);
		}

		/// <summary>
		/// Returns a darker color of specified color.
		/// </summary>
		/// <param name="color">Original color to be changed.</param>
		/// <returns>Darker color.</returns>
		public static SolidColor Dark(SolidColor color)
		{
			return SetBrightness(-0.3f, color);
		}

		/// <summary>
		/// Change color brightness.
		/// </summary>
		/// <param name="bright">Brightness value.</param>
		/// <param name="color">Color to be changed.</param>
		/// <returns>Color has been changed.</returns>
		public static SolidColor SetBrightness(float bright, SolidColor color)
		{
			HSLColor hsl = ColorUtility.RGBToHSL(color);
			hsl.L += bright;
			//if (hsl.L > 1f) hsl.L = 1f;
			return ColorUtility.HSLToRgb(hsl);
		}

		#region Operator Overrides

		/// <summary>
		/// Compare two colors and check whether they are same
		/// </summary>
		/// <param name="c1">First color to be compared</param>
		/// <param name="c2">Second color to be compared</param>
		/// <returns>True if specified two color are same</returns>
		public static bool operator ==(SolidColor c1, SolidColor c2)
		{
			return c1.A == c2.A && c1.R == c2.R && c1.G == c2.G && c1.B == c2.B;
		}

		/// <summary>
		/// Compare two colors and check whether they are not same
		/// </summary>
		/// <param name="c1">First color to be compared</param>
		/// <param name="c2">Second color to be compared</param>
		/// <returns>True if specified two color are not same</returns>
		public static bool operator !=(SolidColor c1, SolidColor c2)
		{
			return c1.A != c2.A || c1.R != c2.R || c1.G != c2.G || c1.B != c2.B;
		}

		/// <summary>
		/// Compare specified object whether is same as this object
		/// </summary>
		/// <param name="obj">Object to be compared</param>
		/// <returns>True if specified object are same as this object</returns>
		public override bool Equals(object obj)
		{
			if (obj is SolidColor)
			{
				var c2 = (SolidColor)obj;
				return this.A == c2.A && this.R == c2.R && this.G == c2.G && this.B == c2.B;
			}
#if WINFORM || ANDROID
			else if (obj is System.Drawing.Color)
			{
				var c2 = (System.Drawing.Color)obj;
				return this.A == c2.A && this.R == c2.R && this.G == c2.G && this.B == c2.B;
			}
#endif // WINFORM
#if WPF
			else if (obj is System.Windows.Media.Color)
			{
				var c2 = (System.Windows.Media.Color)obj;
				return this.A == c2.A && this.R == c2.R && this.G == c2.G && this.B == c2.B;;
			}
#endif // WPF
			else
				return false;
		}

		/// <summary>
		/// Get hash code
		/// </summary>
		/// <returns>Hash code</returns>
		public override int GetHashCode()
		{
			return this.ToArgb();
		}

		#endregion // Operator Overrides

		#region Platform Converters

		#region WinForm

#if WINFORM
		public static implicit operator SolidColor(System.Drawing.Color color)
		{
			return SolidColor.FromArgb(color.A, color.R, color.G, color.B);
		}

		/// <summary>
		/// Convert from System.Drawing.Color to ReoGrid color
		/// </summary>
		/// <param name="color">System.Drawing.Color value</param>
		/// <returns>Converted ReoGrid</returns>
		public static implicit operator System.Drawing.Color(SolidColor color)
		{
			return color.A == 0 && color.R ==0 && color.G == 0 && color.B == 0 
				? System.Drawing.Color.Empty : System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
		}

		/// <summary>
		/// Compare two colors and check whether they are not same
		/// </summary>
		/// <param name="c1">First color to be compared</param>
		/// <param name="c2">Second color to be compared</param>
		/// <returns>True if specified two color are not same</returns>
		public static bool operator ==(SolidColor c1, System.Drawing.Color c2)
		{
			return c1.A == 0 && c1.R == 0 && c1.G == 0 && c1.B == 0 && c2.IsEmpty
				|| c1.A == c2.A && c1.R == c2.R && c1.G == c2.G && c1.B == c2.B;
		}
#endif // WINFORM

#if WINFORM
		/// <summary>
		/// Compare two colors and check whether they are same
		/// </summary>
		/// <param name="c1">First color to be compared</param>
		/// <param name="c2">Second color to be compared</param>
		/// <returns>True if specified two color are same</returns>
		public static bool operator !=(SolidColor c1, System.Drawing.Color c2)
		{
			return !(c1 == c2);
		}

		///// <summary>
		///// Compare two colors and check whether they are not same
		///// </summary>
		///// <param name="c2">System.Drawing.Color to be compared</param>
		///// <returns>True if specified two color are not same</returns>
		//public bool Equals(System.Drawing.Color c2)
		//{
		//	return (this.A == 0 && this.R == 0 && this.G == 0 && this.B == 0 && c2.IsEmpty)
		//		|| (this.A == c2.A && this.R == c2.R && this.G == c2.G && this.B == c2.B);
		//}
#endif // WINFORM

		#endregion // WinForm

#if WPF
		public static implicit operator SolidColor(System.Windows.Media.Color color)
		{
			return SolidColor.FromArgb(color.A, color.R, color.G, color.B);
		}
		public static implicit operator System.Windows.Media.Color(SolidColor color)
		{
			return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
		}
#endif // WPF

#if ANDROID
		public static implicit operator SolidColor(Android.Graphics.Color color)
		{
			return SolidColor.FromArgb(color.A, color.R, color.G, color.B);
		}
		public static implicit operator Android.Graphics.Color(SolidColor color)
		{
			return new Android.Graphics.Color(color.R, color.G, color.B, color.A);
		}
#endif // ANDROID

#if iOS
		public static implicit operator SolidColor(UIKit.UIColor color)
		{
			nfloat r, g, b, a;
			color.GetRGBA(out r, out g, out b, out a);

			int rr, gg, bb, aa;

			rr = (int)Math.Round(r * 255);
			if (rr < 0) rr = 0; else if (rr > 255) rr = 255;
			gg = (int)Math.Round(g * 255);
			if (gg < 0) gg = 0; else if (gg > 255) gg = 255;
			bb = (int)Math.Round(b * 255);
			if (bb < 0) bb = 0; else if (bb > 255) bb = 255;
			aa = (int)Math.Round(a * 255);
			if (aa < 0) aa = 0; else if (aa > 255) aa = 255;

			return SolidColor.FromArgb(aa, rr, gg, bb);
		}
		public static implicit operator UIKit.UIColor(SolidColor color)
		{
			return new UIKit.UIColor(color.R / 255.0f, color.G / 255.0f,
				color.B / 255.0f, color.A / 255.0f);
		}
		public static implicit operator SolidColor(CoreGraphics.CGColor color)
		{
			return new UIKit.UIColor(color);
		}
		public static implicit operator CoreGraphics.CGColor(SolidColor color)
		{
			return new CoreGraphics.CGColor(color.R / 255.0f, color.G / 255.0f, 
				color.B / 255.0f, color.A / 255.0f);
		}
#endif // ANDROID
		#endregion // Platform Converters

		#region Predefined Colors
#pragma warning disable 1591

		//public static readonly Color Empty = new Color(0, 0, 0, 0);
		public static readonly SolidColor Transparent = new SolidColor(0, 0, 0, 0);

		public static readonly SolidColor White = new SolidColor(255, 255, 255);
		public static readonly SolidColor WhiteSmoke = new SolidColor(245, 245, 245);
		public static readonly SolidColor Gainsboro = new SolidColor(220, 220, 220);
		public static readonly SolidColor Silver = new SolidColor(192, 192, 192);
		public static readonly SolidColor Gray = new SolidColor(128, 128, 128);
		public static readonly SolidColor Black = new SolidColor(0, 0, 0);

		public static readonly SolidColor Red = new SolidColor(255, 0, 0);
		public static readonly SolidColor Green = new SolidColor(0, 255, 0);
		public static readonly SolidColor Blue = new SolidColor(0, 0, 255);

		public static readonly SolidColor Coral = new SolidColor(255, 127, 80);

		public static readonly SolidColor DarkBlue = new SolidColor(0, 0, 139);
		public static readonly SolidColor DarkOrange = new SolidColor(255, 140, 0);
		public static readonly SolidColor DeepSkyBlue = new SolidColor(0, 191, 255);

		public static readonly SolidColor Yellow = new SolidColor(255, 255, 0);
		public static readonly SolidColor Goldenrod = new SolidColor(218, 165, 32);
		public static readonly SolidColor Orange = new SolidColor(255, 165, 0);
		public static readonly SolidColor OrangeRed = new SolidColor(255, 69, 0);
		public static readonly SolidColor IndianRed = new SolidColor(205, 92, 92);

		public static readonly SolidColor LimeGreen = new SolidColor(50, 205, 50);
		public static readonly SolidColor LightGreen = new SolidColor(144, 238, 144);

		public static readonly SolidColor Lavender = new SolidColor(230, 230, 250);
		public static readonly SolidColor LemonChiffon = new SolidColor(255, 250, 205);
		public static readonly SolidColor LightCoral = new SolidColor(240, 128, 128);
		public static readonly SolidColor LightGoldenrodYellow = new SolidColor(250, 250, 210);
		public static readonly SolidColor LightYellow = new SolidColor(255, 255, 224);
		public static readonly SolidColor PaleGoldenrod = new SolidColor(238, 232, 170);

		public static readonly SolidColor LightSteelBlue = new SolidColor(176, 196, 222);
		public static readonly SolidColor Brown = new SolidColor(165, 42, 42);
		public static readonly SolidColor SeaGreen = new SolidColor(46, 139, 87);
		public static readonly SolidColor SteelBlue = new SolidColor(70, 130, 180);
		public static readonly SolidColor SkyBlue = new SolidColor(135, 206, 235);
		public static readonly SolidColor LightSkyBlue = new SolidColor(135, 206, 250);
		public static readonly SolidColor AliceBlue = new SolidColor(240, 248, 255);

		public static readonly SolidColor Purple = new SolidColor(128, 0, 128);
		public static readonly SolidColor Peru = new SolidColor(205, 133, 63);
		public static readonly SolidColor DarkOrchid = new SolidColor(153, 50, 204);

#pragma warning restore 1591
		#endregion Predefined Colors
	}

	/// <summary>
	/// Store the ARGB values of linear color.
	/// </summary>
	public struct LinearColor : IColor
	{
		/// <summary>
		/// Get or set the start color of linear fill region.
		/// </summary>
		public SolidColor StartColor { get; set; }

		/// <summary>
		/// Get or set the end color of linear fill region.
		/// </summary>
		public SolidColor EndColor { get; set; }

		/// <summary>
		/// Linear angle
		/// </summary>
		public float Angle { get; set; }

		/// <summary>
		/// Convert linear color to a solid color.
		/// </summary>
		/// <returns>Solid color converted from this linear color.</returns>
		public SolidColor ToSolidColor()
		{
			SolidColor c = new SolidColor();

			c.A = (byte)((StartColor.A + EndColor.A) / 2);
			c.R = (byte)((StartColor.R + EndColor.R) / 2);
			c.G = (byte)((StartColor.G + EndColor.G) / 2);
			c.B = (byte)((StartColor.B + EndColor.B) / 2);

			return c;
		}

		/// <summary>
		/// Check whether or not this color is transparent.
		/// </summary>
		public bool IsTransparent
		{
			get { return StartColor.IsTransparent && EndColor.IsTransparent; }
		}
	}
}
