/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net/
 *
 * Resource Pool Manager 
 *  - manage reusable GDI+ objects, images and other resources
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

#if WINFORM || WPF
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using WFFont = System.Drawing.Font;
using WFFontStyle = System.Drawing.FontStyle;
using WFGraphics = System.Drawing.Graphics;

#if WINFORM

using RGFloat = System.Single;

using RGDashStyle = System.Drawing.Drawing2D.DashStyle;
using RGDashStyles = System.Drawing.Drawing2D.DashStyle;

using RGPen = System.Drawing.Pen;
using RGSolidBrush = System.Drawing.SolidBrush;

using HatchBrush = System.Drawing.Drawing2D.HatchBrush;
using HatchStyle = System.Drawing.Drawing2D.HatchStyle;

#elif WPF

using RGFloat = System.Double;

using RGPen = System.Windows.Media.Pen;
using RGSolidBrush = System.Windows.Media.SolidColorBrush;
using RGBrushes = System.Windows.Media.Brushes;
using RGDashStyle = System.Windows.Media.DashStyle;
using RGDashStyles = System.Windows.Media.DashStyles;

#endif // WPF

using unvell.ReoGrid.Graphics;

namespace unvell.Common
{
	internal sealed class ResourcePoolManager : IDisposable
	{
		//private static readonly ResourcePoolManager instance = new ResourcePoolManager();
		//public static ResourcePoolManager Instance { get { return instance; } }

		internal ResourcePoolManager()
		{
			Logger.Log("resource pool", "create resource pool...");
		}

#region Brush
#if WINFORM || WPF
		private Dictionary<SolidColor, RGSolidBrush> cachedBrushes = new Dictionary<SolidColor, RGSolidBrush>();

		public RGSolidBrush GetBrush(SolidColor color)
		{
			if (color.A == 0) return null;

			lock (cachedBrushes)
			{
				if (cachedBrushes.TryGetValue(color, out var b))
				{
					return b;
				}
				else
				{
					b = new RGSolidBrush(color);
					cachedBrushes.Add(color, b);

					if ((cachedBrushes.Count % 10) == 0)
					{
						Logger.Log("resource pool", "solid brush count: " + cachedBrushes.Count);
					}

					return b;
				}
			}
		}
#endif // WINFORM || WPF

#if WINFORM
		private Dictionary<HatchStyleBrushInfo, HatchBrush> hatchBrushes = new Dictionary<HatchStyleBrushInfo, HatchBrush>();

		public HatchBrush GetHatchBrush(HatchStyle style, SolidColor foreColor, SolidColor backColor)
		{
			HatchStyleBrushInfo info = new HatchStyleBrushInfo(style, foreColor, backColor);

			lock (this.hatchBrushes)
			{
				if (hatchBrushes.TryGetValue(info, out var hb))
				{
					return hb;
				}
				else
				{
					HatchBrush b = new HatchBrush(style, foreColor, backColor);
					hatchBrushes.Add(info, b);

					Logger.Log("resource pool", "add hatch brush, count: " + hatchBrushes.Count);
					return b;
				}
			}
		}
		private struct HatchStyleBrushInfo
		{
			internal HatchStyle style;
			internal SolidColor foreColor;
			internal SolidColor backgroundColor;

			public HatchStyleBrushInfo(HatchStyle style, SolidColor foreColor, SolidColor backgroundColor)
			{
				this.style = style;
				this.foreColor = foreColor;
				this.backgroundColor = backgroundColor;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is HatchStyleBrushInfo)) return false;

				HatchStyleBrushInfo right = (HatchStyleBrushInfo)obj;
				return (this.style == right.style
					&& this.foreColor == right.foreColor
					&& this.backgroundColor == right.backgroundColor);
			}

			public static bool operator ==(HatchStyleBrushInfo left, HatchStyleBrushInfo right)
			{
				return left.Equals(right);

				// type converted from class
				//if (left == null && right == null) return true;
				//if (left == null || right == null) return false;

				//if (left == null)
				//	return right.Equals(left);
				//else
				//	return left.Equals(right);
			}

			public static bool operator !=(HatchStyleBrushInfo left, HatchStyleBrushInfo right)
			{
				return !(left == right);
			}

			public override int GetHashCode()
			{
				return (short)style * (foreColor.ToArgb() + backgroundColor.ToArgb());
			}
		}
#endif // WINFORM
#endregion Brush

#region Pen
		private Dictionary<SolidColor, List<RGPen>> cachedPens = new Dictionary<SolidColor, List<RGPen>>();
		public RGPen GetPen(SolidColor color)
		{
			return GetPen(color, 1, RGDashStyles.Solid);
		}
		public RGPen GetPen(SolidColor color, RGFloat weight, RGDashStyle style)
		{
			if (color.A == 0) return null;

			RGPen pen;
			List<RGPen> penlist;

			lock (cachedPens)
			{
				if (!cachedPens.TryGetValue(color, out penlist))
				{
					penlist = cachedPens[color] = new List<RGPen>();
#if WINFORM
					penlist.Add(pen = new RGPen(color, weight));
#elif WPF
				penlist.Add(pen = new RGPen(new RGSolidBrush(color), weight));
#endif // WPF

					pen.DashStyle = style;

					if ((cachedPens.Count % 10) == 0)
					{
						Logger.Log("resource pool", "wf pen count: " + cachedPens.Count);
					}
				}
				else
				{
					lock (penlist)
					{
#if WINFORM
						pen = penlist.FirstOrDefault(p => p.Width == weight && p.DashStyle == style);
#elif WPF
						pen = penlist.FirstOrDefault(p => p.Thickness == weight && p.DashStyle == style);
#endif // WPF
					}

					if (pen == null)
					{
#if WINFORM
						penlist.Add(pen = new RGPen(color, weight));
#elif WPF
					penlist.Add(pen = new RGPen(new RGSolidBrush(color), weight));
#endif // WPF
						pen.DashStyle = style;

						if ((cachedPens.Count % 10) == 0)
						{
							Logger.Log("resource pool", "pen count: " + cachedPens.Count);
						}
					}
				}
			}

			return pen;
		}
#endregion // Pen

#region Font

		private Dictionary<string, List<WFFont>> fonts = new Dictionary<string, List<WFFont>>();

#if WINFORM
		public WFFont GetFont(string familyName, float emSize, WFFontStyle wfs)
		{

#elif WPF
		public WFFont GetFont(string familyName, double emSizeD, WFFontStyle wfs)
		{
			float emSize = (float)emSizeD;
#endif // WPF

#if DEBUG
			Stopwatch sw = Stopwatch.StartNew();
#endif // DEBUG

			if (string.IsNullOrEmpty(familyName))
			{
				familyName = System.Drawing.SystemFonts.DefaultFont.FontFamily.Name;
			}

			WFFont font = null;
			List<WFFont> fontGroup = null;
			System.Drawing.FontFamily family = null;

			lock (this.fonts)
			{
				if (this.fonts.TryGetValue(familyName, out fontGroup))
				{
					if (fontGroup.Count > 0)
					{
						family = fontGroup[0].FontFamily;
					}

					lock (fontGroup)
					{
						font = fontGroup.FirstOrDefault(f => f.Size == emSize && f.Style == wfs);
					}
				}
			}

			if (font != null) return font;

			if (family == null)
			{
				try
				{
					family = new System.Drawing.FontFamily(familyName);
				}
				catch (ArgumentException ex)
				{
					//throw new FontNotFoundException(ex.ParamName);
					family = System.Drawing.SystemFonts.DefaultFont.FontFamily;
					Logger.Log("resource pool", "font family error: " + familyName + ": " + ex.Message);
				}

				if (!family.IsStyleAvailable(wfs))
				{
					try
					{
						wfs = FindFirstAvailableFontStyle(family);
					}
					catch
					{
						return System.Drawing.SystemFonts.DefaultFont;
					}
				}
			}

			lock (this.fonts)
			{
				if (fonts.TryGetValue(family.Name, out fontGroup))
				{
					lock (fontGroup)
					{
						font = fontGroup.FirstOrDefault(f => f.Size == emSize && f.Style == wfs);
					}
				}
			}

			if (font == null)
			{
				font = new WFFont(family, emSize, wfs);

				if (fontGroup == null)
				{
					lock (this.fonts)
					{
						fonts.Add(family.Name, fontGroup = new List<WFFont> { font });
					}
					Logger.Log("resource pool", "font resource group added. font groups: " + fonts.Count);
				}
				else
				{
					lock (fontGroup)
					{
						fontGroup.Add(font);
					}
					Logger.Log("resource pool", "font resource added. fonts: " + fontGroup.Count);
				}

			}

#if DEBUG
			sw.Stop();
			long ms = sw.ElapsedMilliseconds;
			if (ms > 10)
			{
				Debug.WriteLine("resource pool: font scan: " + sw.ElapsedMilliseconds + " ms.");
			}
#endif // DEBUG
			return font;
		}

		private static WFFontStyle FindFirstAvailableFontStyle(System.Drawing.FontFamily ff)
		{
			if (ff.IsStyleAvailable(WFFontStyle.Regular))
				return WFFontStyle.Regular;
			else if (ff.IsStyleAvailable(WFFontStyle.Bold))
				return WFFontStyle.Bold;
			else if (ff.IsStyleAvailable(WFFontStyle.Italic))
				return WFFontStyle.Italic;
			else if (ff.IsStyleAvailable(WFFontStyle.Strikeout))
				return WFFontStyle.Strikeout;
			else if (ff.IsStyleAvailable(WFFontStyle.Underline))
				return WFFontStyle.Underline;
			else
			{
				Logger.Log("resource pool", "no available font style found: " + ff.Name);
				throw new NoAvailableFontStyleException();
			}
		}

		internal class NoAvailableFontStyleException : Exception
		{
		}

#if WPF

		private Dictionary<string, System.Windows.Media.FontFamily> fontFamilies
			= new Dictionary<string, System.Windows.Media.FontFamily>();

		public System.Windows.Media.FontFamily GetFontFamily(string name)
		{
			System.Windows.Media.FontFamily ff = null;
			this.fontFamilies.TryGetValue(name, out ff);
			if (ff == null)
			{
				ff = new System.Windows.Media.FontFamily(name);
				this.fontFamilies[name] = ff;
			}
			return ff;
		}

		private Dictionary<string, List<System.Windows.Media.Typeface>> typefaces 
			= new Dictionary<string, List<System.Windows.Media.Typeface>>();

		public System.Windows.Media.Typeface GetTypeface(string name)
		{
			return GetTypeface(name, System.Windows.FontWeights.Regular, System.Windows.FontStyles.Normal,
				System.Windows.FontStretches.Normal);
		}

		public System.Windows.Media.Typeface GetTypeface(string name, System.Windows.FontWeight weight, 
			System.Windows.FontStyle style, System.Windows.FontStretch stretch)
		{
			List<System.Windows.Media.Typeface> list;

			if (!typefaces.TryGetValue(name, out list))
			{
				this.typefaces[name] = list = new List<System.Windows.Media.Typeface>();
			}

			var typeface = list.FirstOrDefault(t=>t.Weight == weight && t.Style == style);
			if (typeface == null)
			{
				list.Add(typeface = new System.Windows.Media.Typeface(new System.Windows.Media.FontFamily(name), style, weight, stretch));
			}

			return typeface;
		}
#endif // WPF

#endregion // Font

#region Image
#if WINFORM && IMAGE_POOL
		private Dictionary<Guid, ImageResource> images 
			= new Dictionary<Guid, ImageResource>();
		public ImageResource GetImageResource(Guid id)
		{
			return images.Values.FirstOrDefault(i => i.ResId.Equals(id));
		}
		public ImageResource GetImage(string fullPath)
		{
			ImageResource res = images.Values.FirstOrDefault(
				i => i.FullPath != null &&
					i.FullPath.ToLower().Equals(fullPath.ToLower()));
			if (res != null)
			{
				if (res.Image != null) res.Image.Dispose();
				res.Image = Image.FromFile(fullPath);
				return res;
			}
			else
			{
				Image image;
				try
				{
					image = Image.FromFile(fullPath);
				}
				catch(Exception ex) {
					Logger.Log("resource pool", "add image file failed: " + ex.Message);
					return null;
				}

				return AddImage(Guid.NewGuid(), image, fullPath);
			}
		}
		public ImageResource AddImage(Guid id, Image image, string fullPath)
		{
			ImageResource res;

			if (!images.TryGetValue(id, out res))
			{
				images.Add(id, res = new ImageResource()
				{
					ResId = id,
					FullPath = fullPath,
				});

				Logger.Log("resource pool", "image added. count: " + images.Count);
			}

			if (res.Image != null)
			{
				res.Image.Dispose();
			}

			res.Image = image;

			return res;
		}
#endif
#endregion

#region Graphics

		private static System.Drawing.Bitmap bitmapForCachedGDIGraphics;
		private static WFGraphics cachedGDIGraphics;
		public static WFGraphics CachedGDIGraphics
		{
			get
			{
				if (cachedGDIGraphics == null)
				{
					bitmapForCachedGDIGraphics = new System.Drawing.Bitmap(1, 1);
					cachedGDIGraphics = WFGraphics.FromImage(bitmapForCachedGDIGraphics);
				}

				return cachedGDIGraphics;
			}
		}

#endregion // Graphics

#region FormattedText

#endregion // FormattedText

		internal void ReleaseAllResources()
		{
			Logger.Log("resource pool", "release all resources...");

			int count = 
				cachedPens.Count +

#if WINFORM
				hatchBrushes.Count + fonts.Values.Sum(f => f.Count) +
#endif
				/*images.Count +*/ cachedBrushes.Count
#if WPF
				+ typefaces.Sum(t=>t.Value.Count)
#endif
				;

			// pens
			foreach (var plist in cachedPens.Values)
			{
#if WINFORM
				foreach (var p in plist) p.Dispose();
#endif // WINFORM
				plist.Clear();
			}

			cachedPens.Clear();

#if WINFORM
			// fonts
			foreach (var fl in fonts.Values)
			{
				foreach (var f in fl)
				{
					f.FontFamily.Dispose();
					f.Dispose();
				}
				fl.Clear();
			}

			fonts.Clear();

			foreach (var hb in this.hatchBrushes.Values)
			{
				hb.Dispose();
			}

			hatchBrushes.Clear();

			foreach (var sb in this.cachedBrushes.Values)
			{
				sb.Dispose();
			}
#elif WPF
			foreach (var list in typefaces)
			{
				list.Value.Clear();
			}
#endif // WPF

			cachedBrushes.Clear();

#if WINFORM

			//if (cachedGDIGraphics != null) cachedGDIGraphics.Dispose();
			//if (bitmapForCachedGDIGraphics != null) bitmapForCachedGDIGraphics.Dispose();
#endif // WINFORM

			Logger.Log("resource pool", count + " objects released.");
		}

		public void Dispose()
		{
			ReleaseAllResources();
		}
	}
}

#endif // WINFORM || WPF
