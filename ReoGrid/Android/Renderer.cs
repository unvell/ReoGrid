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

#if ANDROID

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;
using unvell.ReoGrid.Drawing.Text;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.AndroidOS
{
	class AndroidRenderer : AndroidGraphics, IRenderer
	{
		private Typeface headerTypeface = null;

		internal AndroidRenderer()
		{
			this.headerTypeface = Typeface.Create("Arial", TypefaceStyle.Normal);
		}

		#region Capped Line

		public void BeginCappedLine(LineCapStyles startCap, Size startSize, LineCapStyles endCap, Size endSize, SolidColor color)
		{
		}

		public void DrawCappedLine(float x1, float y1, float x2, float y2)
		{
		}

		public void EndCappedLine()
		{
		}

		#endregion // Capped Line

		#region Line

		private Paint linePaint;

		public void BeginDrawLine(float width, SolidColor color)
		{
			if (this.linePaint == null)
			{
				this.linePaint = new Paint();
			}

			linePaint.StrokeWidth = width;
			linePaint.Color = color;
		}

	
		public void DrawLine(float x1, float y1, float x2, float y2)
		{
			this.canvas.DrawLine(x1, y1, x2, y2, this.linePaint);
		}

		public void EndDrawLine()
		{
			if (this.linePaint != null)
			{
				this.linePaint.Dispose();
				this.linePaint = null;
			}
		}

		#endregion // Line

		public void DrawRunningFocusRect(float x, float y, float w, float h, SolidColor color, int runningOffset)
		{

		}

		#region Cell

		public void DrawCellText(ReoGridCell cell, SolidColor textColor, DrawMode drawMode, float scale)
		{
			var sheet = cell.Worksheet;
			if (sheet == null) return;

			var loc = cell.RenderTextBounds.Location;

			//using (var typeface = Typeface.Create(cell.InnerStyle.FontName, GetTypefaceStyle(cell.InnerStyle.fontStyles)))
			//{
			using (var p = new Paint())
			{
				p.Color = textColor;
				p.SetTypeface(cell.renderFont);
				p.TextSize = cell.InnerStyle.FontSize * scale;

				base.PlatformGraphics.DrawText(cell.DisplayText, loc.X, loc.Y + cell.RenderTextBounds.Height, p);
			}
			//}
		}

		public Size MeasureTextSize(ReoGridCell cell, DrawMode drawMode, float scale)
		{
			//using (var typeface = Typeface.Create(cell.InnerStyle.FontName, GetTypefaceStyle(cell.InnerStyle.fontStyles)))
			//{
			using (var p = new Paint())
			{
				Rect bounds = new Rect();
				p.SetTypeface(cell.renderFont);
				p.TextSize = cell.InnerStyle.FontSize * scale;

				var str = cell.DisplayText;

				p.GetTextBounds(str, 0, str.Length, bounds);

				if (str.EndsWith(" "))
				{
					int spaceWidth = (int)Math.Round(p.MeasureText(" "));

					for (int i = cell.DisplayText.Length - 1; i >= 0; i--)
					{
						if (str[i] == ' ')
							bounds.Right += spaceWidth;
						else
							break;
					}
				}

				return new Size(bounds.Width(), bounds.Height());
			}
			//}
		}

		public void UpdateCellRenderFont(ReoGridCell cell, Core.UpdateFontReason reason)
		{
			if (reason == Core.UpdateFontReason.FontChanged)
			{
				if (cell.renderFont != null)
				{
					lock (cell.renderFont)
					{
						cell.renderFont.Dispose();
					}
				}

				cell.renderFont = Typeface.Create(cell.InnerStyle.FontName, GetTypefaceStyle(cell.InnerStyle.fontStyles));
			}
		}

		#endregion Cell

		#region Header

		private float scaledHeaderTextSize;

		public void BeginDrawHeaderText(float scale)
		{
			this.scaledHeaderTextSize = 9f * scale;
    }

		public void DrawHeaderText(string text, Paint brush, Rectangle rect)
		{
			base.DrawText(text, brush, this.headerTypeface, this.scaledHeaderTextSize,
				rect, ReoGridHorAlign.Center, ReoGridVerAlign.Middle);
		}

		public void EndDrawHeaderText(Paint brush)
		{
			if (brush != null)
			{
				brush.Dispose();
			}
		}

		public void DrawLeadHeadArrow(Rectangle bounds, SolidColor startColor, SolidColor endColor)
		{
			
		}

		#endregion // Header

		public Paint GetPen(SolidColor color)
		{
			var p = new Paint();
			p.Color = color;
			return p;
		}

		public void ReleasePen(Paint pen)
		{
			pen.Dispose();
		}

		public Paint GetBrush(SolidColor color)
		{
			var p = new Paint();
			p.Color = color;
			return p;
		}

		public void ReleaseBrush(Paint p)
		{
			p.Dispose();
		}

		public Typeface GetFont(string name, float size, FontStyles style)
		{
			return Typeface.Create(name, TypefaceStyle.Normal); // TODO
		}

		#region Utility
		private static TypefaceStyle GetTypefaceStyle(FontStyles styles)
		{
			TypefaceStyle ts = TypefaceStyle.Normal;

			if ((styles & FontStyles.Bold) == FontStyles.Bold)
			{
				ts |= TypefaceStyle.Bold;
			}

			if ((styles & FontStyles.Italic) == FontStyles.Italic)
			{
				ts |= TypefaceStyle.Italic;
			}

			return ts;
		}
		#endregion // Utility
	}
}

#endif // ANDROID 