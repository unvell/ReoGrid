/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net/
 *
 * Plain Text Format Convert Utilies
 * 
 * - Convert between .NET objects and string what used in XML serialization
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Text.RegularExpressions;

using System.Reflection;
using System.Globalization;

using unvell.ReoGrid;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Drawing.Text;

namespace unvell.Common
{
	internal class TextFormatHelper
	{
		#region Encoding
		public static string EncodeRect(Rectangle rect)
		{
			return (string.Format("({0},{1},{2},{3})",
					rect.Left, rect.Top, rect.Width, rect.Height));
		}
		public static string EncodePadding(PaddingValue pad)
		{
			return (string.Format("({0},{1},{2},{3})",
					pad.Left, pad.Top, pad.Right, pad.Bottom));
		}
		public static string EncodeSize(Size size)
		{
			return (string.Format("({0},{1})", size.Width, size.Height));
		}
		public static string EncodeColor(SolidColor c)
		{
			return c.A == 255 ? (string.Format("#{0:x2}{1:x2}{2:x2}", c.R, c.G, c.B))
				: string.Format("#{0:x2}{1:x2}{2:x2}{3:x2}", c.A, c.R, c.G, c.B);
		}
		public static string EncodeColorNoAlpha(SolidColor c)
		{
			return string.Format("FF{0:x2}{1:x2}{2:x2}", c.R, c.G, c.B).ToUpper();
		}
		public static string EncodePoint(Point p)
		{
			return (string.Format("({0},{1})", p.X, p.Y));
		}
		public static string EncodePoints(IEnumerable<Point> points)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var p in points)
			{
				if (sb.Length > 0) sb.Append(",");
				sb.AppendFormat("{0} {1}",p.X, p.Y);
			}
			return sb.ToString();
		}
		public static string EncodePointsHex(IEnumerable<Point> points)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var p in points)
			{
				byte[] buf = BitConverter.GetBytes(p.X);
				byte[] buf2 = BitConverter.GetBytes(p.Y);
				sb.AppendFormat("{0:x1}{1:x1}{2:x1}{3:x1}{4:x1}{5:x1}{6:x1}{7:x1}",
					buf[0], buf[1], buf[2], buf[3],
					buf2[0], buf2[1], buf2[2], buf2[3]);
			}
			return sb.ToString();
		}

		public static string EncodeFontStyle(FontStyles fs)
		{
			StringBuilder sb = new StringBuilder();
			if ((fs & FontStyles.Bold) > 0)
			{
				sb.Append("blob");
			}
			if ((fs & FontStyles.Italic) > 0)
			{
				if (sb.Length > 0) sb.Append(" ");
				sb.Append("italic");
			}
			if ((fs & FontStyles.Strikethrough) > 0)
			{
				if (sb.Length > 0) sb.Append(" ");
				sb.Append("strikeout");
			}
			if ((fs & FontStyles.Underline) > 0)
			{
				if (sb.Length > 0) sb.Append(" ");
				sb.Append("underline");
			}
			
			return sb.Length == 0 ? "normal": sb.ToString();
		}

#if WINFROM
		public static string EncodeLineStyle(DashStyle ds)
		{
			switch (ds)
			{
				case (System.Drawing.Drawing2D.DashStyle.Dash):
					return "dash";
				case System.Drawing.Drawing2D.DashStyle.DashDot:
					return "dash-dot";
				case System.Drawing.Drawing2D.DashStyle.DashDotDot:
					return "dash-dot-dot";
				case System.Drawing.Drawing2D.DashStyle.Dot:
					return "dot";
				default:
				case System.Drawing.Drawing2D.DashStyle.Solid:
					return "solid";
			}
		}
		public static string EncodeLineCapStyle(System.Drawing.Drawing2D.LineCap cap)
		{
			switch (cap)
			{
				case LineCap.ArrowAnchor:
					return "arrow";
				case LineCap.DiamondAnchor:
					return "diamond";
				case LineCap.RoundAnchor:
					return "round";
				case LineCap.SquareAnchor:
					return "square";
				default:
				case LineCap.NoAnchor:
					return "none";
			}
		}
#endif // WINFROM
		public static string EncodeBool(bool p)
		{
			return p ? "true" : "false";
		}
		/// <summary>
		/// Encode a boolean value into string and compare to the default value, 
		/// if the value is the same as default value then return null (noting output to xml).
		/// </summary>
		/// <param name="p">boolean value to be encoded</param>
		/// <param name="def">default boolean value used to compare whether the value is same as default</param>
		/// <returns>encoded string of boolean value</returns>
		public static string EncodeBool(bool p, bool def)
		{
			return p == def ? null : EncodeBool(p);
		}
		public static string EncodeFloatArray(params float[] values)
		{
			StringBuilder sb = new StringBuilder();
			foreach (float v in values)
			{
				if (sb.Length > 0) sb.Append(",");
				sb.Append(v);
			}
			return sb.ToString();
		}
		public static string EncodeIntArray(IEnumerable<int> values)
		{
			StringBuilder sb = new StringBuilder();
			foreach (float v in values)
			{
				if (sb.Length > 0) sb.Append(",");
				sb.Append(v);
			}
			return sb.ToString();
		}
		#endregion

		#region Decoding
		public static Guid DecodeGuid(string data)
		{
			try
			{
				return new Guid(data);
			}
			catch { return Guid.Empty; }
		}
		public static readonly Regex RectRegex =
				new Regex(@"\(\s*([-\w.]+)\s*,\s*([-\w.]+)\s*,\s*([-\w.]+)\s*,\s*([-\w.]+)\)\s*");
		public static Rectangle DecodeRect(string data)
		{
			Match m = RectRegex.Match(data);
			if (m.Success)
			{
				return new Rectangle(GetPixelValue(m.Groups[1].Value),
					GetPixelValue(m.Groups[2].Value),
					GetPixelValue(m.Groups[3].Value),
					GetPixelValue(m.Groups[4].Value));
			}
			else
				return new Rectangle();
		}
		public static PaddingValue DecodePadding(string data)
		{
			Rectangle r = DecodeRect(data);
			return new PaddingValue((int)r.Left, (int)r.Top, (int)r.Width, (int)r.Height);
		}

		public static Size DecodeSize(string data)
		{
			var p = DecodePoint(data);
			return new Size(p.X, p.Y);
		}

		public static readonly Regex WebColorRegex = new
				Regex(@"\#?([0-9a-fA-F]{2})?([0-9a-fA-F]{2})([0-9a-fA-F]{2})([0-9a-fA-F]{2})");

		public static bool IsWebColorFormat(string data)
		{
			return WebColorRegex.IsMatch(data);
		}

		public static bool DecodeColor(string data, out SolidColor color)
		{
			if (!string.IsNullOrEmpty(data) && !data.Equals("none", StringComparison.InvariantCultureIgnoreCase))
			{
				Match m = WebColorRegex.Match(data);

				if (m.Success)
				{
					color = SolidColor.FromArgb(m.Groups[1].Value.Length > 0 ?
										Convert.ToByte(m.Groups[1].Value, 16) : (byte)255,
										Convert.ToByte(m.Groups[2].Value, 16),
										Convert.ToByte(m.Groups[3].Value, 16),
										Convert.ToByte(m.Groups[4].Value, 16));

					return true;
				}
#if WINFORM || WPF || ANDROID
				else
				{
					try
					{
#if WINFORM
						color = System.Drawing.Color.FromName(data);
#elif WPF
						color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(data);
#elif ANDROID
						var c = System.Drawing.Color.FromName(data);
						color = new SolidColor(c.R, c.G, c.B);

#endif // WPF
						return true;
					}
					catch { }
				}
#endif // WINFORM || WPF
			}

			color = new SolidColor();
			return false;
		}

		public static readonly Regex PointRegex = new Regex(@"\(\s*([-\w.]+)\s*,\s*([-\w.]+)\)\s*");

		public static bool IsRectFormat(string data)
		{
			return RectRegex.IsMatch(data);
		}

		public static Point DecodePoint(string data)
		{
			Match m = PointRegex.Match(data);
			if (m.Success)
			{
				return new Point(GetPixelValue(m.Groups[1].Value),
					GetPixelValue(m.Groups[2].Value));
			}
			else
				return new Point(0, 0); // todo
		}

#if WINFORM

		public static FontStyles DecodeFontStyle(string fontStyleStr)
		{
			FontStyles fs = FontStyles.Regular;
			string[] fsstr = fontStyleStr.Split(' ', ',');
			if (fsstr.Contains("blob"))
				fs |= FontStyles.Bold;
			else if (fsstr.Contains("italic"))
				fs |= FontStyles.Italic;
			else if (fsstr.Contains("strikeout"))
				fs |= FontStyles.Strikethrough;
			else if (fsstr.Contains("underline"))
				fs |= FontStyles.Underline;
			else
				fs = FontStyles.Regular;
			return fs;
		}

		public static System.Drawing.Drawing2D.DashStyle DecodeLineStyle(string data)
		{
			switch(data.Trim().ToLower())
			{
				case "dash":
					return System.Drawing.Drawing2D.DashStyle.Dash;
				case "dash-dot":
					return System.Drawing.Drawing2D.DashStyle.DashDot;
				case "dash-dot-dot":
					return System.Drawing.Drawing2D.DashStyle.DashDotDot;
				case "dot":
					return System.Drawing.Drawing2D.DashStyle.Dot;
				default:
				case "solid":
					return System.Drawing.Drawing2D.DashStyle.Solid;
			}
		}
		public static System.Drawing.Drawing2D.LineCap DecodeLineCapStyle(string data)
		{
			switch (data)
			{
				case "arrow":
					return System.Drawing.Drawing2D.LineCap.ArrowAnchor;
				case "diamond":
					return System.Drawing.Drawing2D.LineCap.DiamondAnchor;
				case "round":
					return System.Drawing.Drawing2D.LineCap.RoundAnchor;
				case "square":
					return System.Drawing.Drawing2D.LineCap.SquareAnchor;
				default:
				case "none":
					return System.Drawing.Drawing2D.LineCap.NoAnchor;
			}
		}
#endif // WINFORM

		public static readonly Regex pointArrayRegex = new Regex(@"(\d*\.?\d+)\s(\d*\.?\d+)");
		public static List<Point> DecodePoints(string data)
		{
			List<Point> pts = new List<Point>(5);
			foreach (Match m in pointArrayRegex.Matches(data))
			{
				pts.Add(new Point(GetFloatPixelValue(m.Groups[1].Value, 0f),
					GetFloatPixelValue(m.Groups[2].Value, 0f)));
			}
			return pts;
		}

		public static readonly Regex PathDataRegex = new Regex(@"(\w?)\(([-?\d+,?]+)\),?");

		public static float[] DecodeFloatArray(string str)
		{
			string[] items = str.Split(',');
			float[] values = new float[items.Length];
			for (int i = 0; i < items.Length; i++)
			{
				try
				{
					float.TryParse(items[i], out var f);
					values[i] = f;
				}
				catch { }
			}
			return values;
		}

		public static int[] DecodeIntArray(string str)
		{
			string[] items = str.Split(',');
			int[] values = new int[items.Length];
			for (int i = 0; i < items.Length; i++)
			{
				try
				{
					int.TryParse(items[i], out var f);
					values[i] = f;
				}
				catch { }
			}
			return values;
		}
#endregion

		public static float GetFloatValue(string str, float def)
		{
			float.TryParse(str, out def);
			return def;
		}
		public static float GetFloatValue(string str, float def, CultureInfo culture)
		{
			float.TryParse(str, NumberStyles.Any, culture, out def);
			return def;
		}
		public static float GetFloatPixelValue(string str, float def)
		{
			return GetFloatPixelValue(str, def, CultureInfo.CurrentCulture);
		}
		public static float GetFloatPixelValue(string str, float def, CultureInfo culture)
		{
			if (str == null) return def;
			str = str.Trim();
			if (str.EndsWith("px"))
				str = str.Substring(0, str.Length - 2).Trim();

			float.TryParse(str, NumberStyles.Any, culture, out var v);

			return v;
		}
		public static int GetPixelValue(string str)
		{
			return (int)GetPixelValue(str, 0);
		}
		public static int GetPixelValue(string str, CultureInfo culture)
		{
			return (int)GetPixelValue(str, 0, culture);
		}
		public static int GetPixelValue(string str, int def)
		{
			return (int)GetFloatPixelValue(str, (float)def, CultureInfo.CurrentCulture);
		}
		public static int GetPixelValue(string str, int def, CultureInfo culture)
		{
			return (int)GetFloatPixelValue(str, (float)def, culture);
		}

		public static float GetPercentValue(string str, float def)
		{
			if(string.IsNullOrEmpty(str)) return def;
			string p = str.Substring(0, str.Length - 1).Trim();
			return GetFloatValue(p, def) / 100f;
		}

		public static readonly Regex AttrRegex
			= new Regex(@"\s*([-_\w.]+)\s*:\s*((\'([^\']*)\')|([^;]*))\s*;?");
		public static void ParseDataAttribute(string attr, Action<string, string> a)
		{
			if (attr != null)
			{
				foreach (Match m in AttrRegex.Matches(attr))
				{
					string key = m.Groups[1].Value;
					string value = (m.Groups[5].Value != null
						&& m.Groups[5].Length > 0) ? m.Groups[5].Value : m.Groups[4].Value;

					a(key, value);
				}
			}
		}
		public static T CreateElementFromAttribute<T>(T obj, string attr) where T : new()
		{
			T t = (obj == null ? new T() : obj);

			ParseDataAttribute(attr, (key, value) =>
			{
				if (key.Length > 0 && value != null)
				{
					FieldInfo fi = t.GetType().GetField(key);
					if (fi != null)
					{
						fi.SetValue(t, value);
					}
					else
					{
						key = key.Substring(0, 1).ToUpper() + key.Substring(1);

						PropertyInfo pi = t.GetType().GetProperty(key);
						if (pi == null)
						{
							pi = t.GetType().GetProperties().FirstOrDefault(p =>
							{
								XmlAttributeAttribute[] attrs
									= p.GetCustomAttributes(typeof(XmlAttributeAttribute), true)
									as XmlAttributeAttribute[];
								return (attrs != null && attrs.Length > 0 && attrs[0] != null
								&& attrs[0].AttributeName.ToLower().Equals(key.ToLower()));
							});
						}
						if (pi != null)
						{
							pi.SetValue(t, value, null);
						}
					}
				}
			});

			return t;
		}
		public static string GenerateDataAttributeString(Dictionary<string, string> data)
		{
			StringBuilder sb = new StringBuilder();
			foreach (string key in data.Keys) {
				if (sb.Length > 0) sb.Append("; ");
				sb.Append(string.Format("{0}: {1}", key, data[key]));
			}
			return sb.ToString();
		}
		public static string GenerateDataAttributeString(params string[] data)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < data.Length; i += 2)
			{
				if (sb.Length > 0) sb.Append(" ");
				sb.Append(string.Format("{0}: {1};", data[i], data[i + 1]));
			}
			return sb.ToString();
		}

		public static bool IsSwitchOn(string value)
		{
			return !string.IsNullOrEmpty(value)
				&& (string.Equals(value, "1", StringComparison.CurrentCultureIgnoreCase)
				|| string.Equals(value, "true", StringComparison.CurrentCultureIgnoreCase)
				|| string.Equals(value, "yes", StringComparison.CurrentCultureIgnoreCase)
				|| string.Equals(value, "on", StringComparison.CurrentCultureIgnoreCase));
		}

		public static T LoadXml<T>(string filepath)
		{
			XmlSerializer xmlReader = new XmlSerializer(typeof(T));
			using (FileStream fs = new FileStream(filepath, FileMode.Open))
			{
				T t = (T)xmlReader.Deserialize(fs);
				return t;
			}
		}

	}
}
