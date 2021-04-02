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

#if DRAWING

#if (WINFORM || WPF)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if WINFORM || ANDROID
using RGFloat = System.Single;
using RGFont = System.Drawing.Font;
using RGPen = System.Drawing.Pen;
using RGBrush = System.Drawing.SolidBrush;
#elif WPF
using RGFloat = System.Double;
using RGFont = System.Windows.Media.Typeface;
using RGPen = System.Windows.Media.Pen;
using RGBrush = System.Windows.Media.SolidColorBrush;
#endif // WINFORM

using unvell.Common;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Drawing.Text;
using System.Collections;

namespace unvell.ReoGrid.Drawing
{
	/// <summary>
	/// Represents a rich format text object that could be displayed inside cell or drawing objects.
	/// </summary>
	public sealed class RichText
	{
#if WINFORM
		internal System.Drawing.StringFormat sf = new System.Drawing.StringFormat(System.Drawing.StringFormat.GenericTypographic)
		{
			FormatFlags = System.Drawing.StringFormatFlags.NoWrap | System.Drawing.StringFormatFlags.MeasureTrailingSpaces,
		};
#endif // WINFORM

		private bool suspendUpdateText = false;

		/// <summary>
		/// Suspend update text when size, wrap-mode etc. properties changed.
		/// </summary>
		public void SuspendUpdateText()
		{
			this.suspendUpdateText = true;
		}

		/// <summary>
		/// Resume update text when size, wrap-mode etc. properties changed.
		/// </summary>
		public void ResumeUpdateText()
		{
			this.suspendUpdateText = false;
		}

		#region Constants
		internal const string BuiltInDefaultFontName = "Calibri";
		internal const RGFloat BuiltInDefaultFontSize = 10.25f;
		#endregion // Constants

		#region Size
		private Size size;

		/// <summary>
		/// Get or set the display area size. (in pixel)
		/// </summary>
		internal Size Size
		{
			get { return this.size; }
			set
			{
				if (this.size != value)
				{
					this.size = value;

					if (!this.suspendUpdateText)
					{
						this.UpdateText();
					}
				}
			}
		}
		#endregion // Size

		#region Measured Size
		internal Size measuredSize = new Size(0, 0);

		/// <summary>
		/// Get the actual size to display text.
		/// </summary>
		public Size TextSize
		{
			get
			{
				return this.measuredSize;
				//var p = this.paragraphs.Count <= 0 ? null : this.paragraphs[this.paragraphs.Count - 1];
				//return p = null ? new Size(0,0) : new Size(textMaxWidth, p.TextSize.
				//var lastLine = p.lines.Count <= 0 ? null : p.lines[p.lines.Count - 1];
				//return lastLine == null ? new Size(0, 0) : new Size(this.textMaxWidth, lastLine.Bottom);
			}
		}
		#endregion // Measured Text Size

		#region Default Values
		public string DefaultFontName { get; set; }

		public RGFloat DefaultFontSize { get; set; }

		public FontStyles DefaultFontStyles { get; set; }

		/// <summary>
		/// Get or set the default background color of text.
		/// </summary>
		public SolidColor DefaultBackColor { get; set; }

		/// <summary>
		/// Get or set the default text color.
		/// </summary>
		public SolidColor DefaultTextColor { get; set; }

		/// <summary>
		/// Get or set the default line height scale for every lines.
		/// </summary>
		public RGFloat DefaultLineHeight { get; set; }

		/// <summary>
		/// Determines the default horizontal alignment for paragraphs.
		/// This option may overwritten by settings from each paragraph.
		/// </summary>
		public ReoGridHorAlign DefaultHorizontalAlignment { get; set; }

		#endregion // Default Values

		/// <summary>
		/// Determines whether or not allow text displayed out of specified size.
		/// </summary>
		public bool Overflow { get; set; }

		private TextWrapMode textWrap = TextWrapMode.NoWrap;

		/// <summary>
		/// Determines the text wrap mode.
		/// </summary>
		public TextWrapMode TextWrap
		{
			get { return textWrap; }
			set
			{
				if (this.textWrap != value)
				{
					this.textWrap = value;

					if (!this.suspendUpdateText)
					{
						this.UpdateText();
					}
				}
			}
		}

		/// <summary>
		/// Get or set the vertical alignment mode.
		/// </summary>
		public ReoGridVerAlign VerticalAlignment { get; set; }

		/// <summary>
		/// Get or set the default paragraph spacing.
		/// </summary>
		public RGFloat DefaultParagraphSpacing { get; set; }

		/// <summary>
		/// Get or set the rotation angle. (-90° ~ 90°)
		/// </summary>
		internal float RotationAngle { get; set; }

		#region Constructors

		/// <summary>
		/// Create an instance of rich format text.
		/// </summary>
		public RichText()
		{
			this.DefaultFontName = RichText.BuiltInDefaultFontName;
			this.DefaultFontSize = RichText.BuiltInDefaultFontSize;
			this.DefaultFontStyles = FontStyles.Regular;
			this.DefaultTextColor = SolidColor.Black;
			this.DefaultBackColor = SolidColor.Transparent;
			this.DefaultLineHeight = 1.2f;
			this.DefaultHorizontalAlignment = ReoGridHorAlign.Left;
			this.DefaultParagraphSpacing = 1.5f;

			this.Overflow = false;
		}

#if WINFORM
		/// <summary>
		/// Dispose rich text object and attached resources.
		/// </summary>
		~RichText()
		{
			this.sf.Dispose();
		}
#endif // WINFORM

		/// <summary>
		/// Create an instance of rich format text with an specified initial display area size.
		/// </summary>
		/// <param name="size"></param>
		internal RichText(Size size)
		{
			this.size = size;
		}

		#endregion // Constructors

		#region Lines & Paragraphs
		private List<Paragraph> paragraphs = new List<Paragraph>();

		internal IEnumerable<Paragraph> Paragraphcs { get { return this.paragraphs; } }

		/// <summary>
		/// Append a new empty paragraph.
		/// </summary>
		/// <returns>The paragraph instance to be added.</returns>
		/// <param name="p">Paragraph to be added. Null to create a new empty paragraph.</param>
		internal void AddParagraph(Paragraph p = null)
		{
			if (p == null) p = new Paragraph(this);
			this.paragraphs.Add(p);
		}
		#endregion // Lines & Paragraphs

		#region Add Text
		/// <summary>
		/// Add text at end of current paragraph with specified styles.
		/// </summary>
		/// <param name="text">Text to be appended.</param>
		/// <param name="fontName">Font name of the text to be appended. Null to use last font name or default font name.</param>
		/// <param name="fontSize">Font size of the text to be appended. Null to use last font size or default font size.</param>
		/// <param name="fontStyles">Style of the text to be appended. Null to use last font style or default font style.</param>
		/// <param name="textColor">Color of the text to be appended. Null to use last text color or default text color.</param>
		/// <param name="backColor">Background color of text to be appended. Null to use last background color or default background color.</param>
		public void AddText(string text, string fontName = null, RGFloat? fontSize = null, FontStyles? fontStyles = null,
			SolidColor? textColor = null, SolidColor? backColor = null)
		{
			var lastPara = this.GetOrCreateLastParagraph();
			lastPara.AddText(text, fontName, fontSize, fontStyles, textColor, backColor);
		}

		/// <summary>
		/// End current paragraph, put following text into a new paragraph.
		/// </summary>
		/// <returns>Rich format text instance.</returns>
		public RichText NewLine()
		{
			var p = new Paragraph(this);
			this.AddParagraph(p);
			return this;
		}

		/// <summary>
		/// Append a text span with specified display format.
		/// </summary>
		/// <param name="text">Text to be appended.</param>
		/// <param name="fontName">Font name of the text to be appended. Null to use last font name or default font name.</param>
		/// <param name="fontSize">Font size of the text to be appended. Null to use last font size or default font size.</param>
		/// <param name="fontStyles">Style of the text to be appended. Null to use last font style or default font style.</param>
		/// <param name="textColor">Color of the text to be appended. Null to use last text color or default text color.</param>
		/// <param name="backColor">Background color of text to be appended. Null to use last background color or default background color.</param>
		/// <returns>Rich format text instance.</returns>
		public RichText Span(string text, string fontName = null, RGFloat? fontSize = null, FontStyles? fontStyles = null,
			SolidColor? textColor = null, SolidColor? backColor = null)
		{
			this.AddText(text, fontName, fontSize, fontStyles, textColor, backColor);
			return this;
		}

		public RichText Bold(string text, string fontName = null, RGFloat? fontSize = null,
			SolidColor? textColor = null, SolidColor? backColor = null)
		{
			var lastRun = this.GetOrCreateLastParagraph().GetOrCreateLastRun(fontName, fontSize, FontStyles.Bold, textColor, backColor);
			lastRun.AppendText(text);
			return this;
		}

		public RichText Italic(string text, string fontName = null, RGFloat? fontSize = null,
			SolidColor? textColor = null, SolidColor? backColor = null)
		{
			var lastRun = this.GetOrCreateLastParagraph().GetOrCreateLastRun(fontName, fontSize, FontStyles.Italic, textColor, backColor);
			lastRun.AppendText(text);
			return this;
		}

		public RichText Underline(string text, string fontName = null, RGFloat? fontSize = null,
			SolidColor? textColor = null, SolidColor? backColor = null)
		{
			var lastRun = this.GetOrCreateLastParagraph().GetOrCreateLastRun(fontName, fontSize, FontStyles.Underline, textColor, backColor);
			lastRun.AppendText(text);
			return this;
		}

		public RichText Regular(string text, string fontName = null, RGFloat? fontSize = null,
			SolidColor? textColor = null, SolidColor? backColor = null)
		{
			var lastRun = this.GetOrCreateLastParagraph().GetOrCreateLastRun(fontName, fontSize, FontStyles.Regular, textColor, backColor);
			lastRun.AppendText(text);
			return this;
		}

		public RichText Superscript(string text, string fontName = null, RGFloat? fontSize = null,
			SolidColor? textColor = null, SolidColor? backColor = null)
		{
			var lastRun = this.GetOrCreateLastParagraph().GetOrCreateLastRun(fontName, fontSize, FontStyles.Superscrit, textColor, backColor);
			lastRun.AppendText(text);
			return this;
		}

		public RichText Subscript(string text, string fontName = null, RGFloat? fontSize = null,
			SolidColor? textColor = null, SolidColor? backColor = null)
		{
			var lastRun = this.GetOrCreateLastParagraph().GetOrCreateLastRun(fontName, fontSize, FontStyles.Subscript, textColor, backColor);
			lastRun.AppendText(text);
			return this;
		}

		public RichText SetStyles(RGFloat? paragraphSpacing = null, RGFloat? lineHeight = null,
			ReoGridHorAlign? halign = null, string fontName = null, RGFloat? fontSize = null,
			FontStyles? fontStyles = null, SolidColor? textColor = null, SolidColor? backColor = null)
		{
			var p = GetOrCreateLastParagraph();

			p.ParagraphEndSpacing = paragraphSpacing ?? this.DefaultParagraphSpacing;
			p.LineHeight = lineHeight ?? this.DefaultLineHeight;
			p.HorizontalAlign = halign ?? this.DefaultHorizontalAlignment;

			var r = p.GetOrCreateLastRun(fontName, fontSize, fontStyles, textColor, backColor);

			return this;
		}

		private Paragraph GetOrCreateLastParagraph()
		{
			Paragraph lastPara = null;

			if (this.paragraphs.Count > 0)
			{
				lastPara = this.paragraphs[this.paragraphs.Count - 1];
			}
			else
			{
				lastPara = new Paragraph(this);
				this.AddParagraph(lastPara);
			}

			return lastPara;
		}

		#endregion // Add Text

		#region Draw
		/// <summary>
		/// Draw rich text at specified area.
		/// </summary>
		/// <param name="g">Graphics context.</param>
		/// <param name="bounds">Target area to draw rich text.</param>
		internal void Draw(IGraphics g, Rectangle bounds)
		{
			RGFloat x = bounds.Left + 2;
			RGFloat y = bounds.Top + 2;

			if (!this.Overflow)
			{
				g.PushClip(bounds);
			}

			switch (this.VerticalAlignment)
			{
				default:
				case ReoGridVerAlign.General:
				case ReoGridVerAlign.Bottom:
					y += (bounds.Height - this.measuredSize.Height) - 6;
					break;

				case ReoGridVerAlign.Middle:
					y += (bounds.Height - this.measuredSize.Height) / 2 - 2;
					break;

				case ReoGridVerAlign.Top:
					y++;
					break;
			}

			Run lastRun = null;

			RGBrush lastBrush = null;
			SolidColor lastColor = SolidColor.Transparent;

#if DEBUG1
			g.DrawRectangle(System.Drawing.Pens.DarkRed, x, y, x + measuredSize.Width, y + measuredSize.Height);
#endif // DEBUG

			foreach (var p in this.paragraphs)
			{
				foreach (var l in p.lines)
				{
#if DEBUG
					//g.DrawRectangle(System.Drawing.Pens.Blue, x, y + l.leftTop.Y, l.Width, l.Height);
#endif // DEBUG

					foreach (var b in l.boxes)
					{
						var r = b.Run;

						if (lastRun != r)
						{
							SolidColor textColor = r.TextColor;

							if (textColor.IsTransparent)
							{
								textColor = this.DefaultTextColor;
							}

							if (textColor != lastColor || lastBrush == null)
							{
								lastColor = textColor;

#if WINFORM || ANDROID
								if (lastBrush != null) lastBrush.Dispose();
#endif // WINFORM
								lastBrush = new RGBrush(lastColor);
							}

							lastRun = r;
						}

#if DEBUG
						//RGFloat baseLine = l.Top + l.Ascent;
						//g.DrawLine(System.Drawing.Pens.Blue, b.Left + 1, baseLine, b.Right - 1, baseLine);
#endif // DEBUG

						if ((r.FontStyles & FontStyles.Underline) == FontStyles.Underline)
						{
#if WINFORM
							using (var underlinePen = new RGPen(lastBrush.Color))
#elif WPF
							var underlinePen = new RGPen(new RGBrush(lastBrush.Color), 1);
#endif // WPF
							{
								RGFloat underlineY = l.Height + y + 1;
								g.PlatformGraphics.DrawLine(underlinePen, new Point(b.leftTop.X + x, underlineY), new Point(b.rightTop.X + x, underlineY));
							}
						}

						var tx = b.leftTop.X + x;
						var ty = b.leftTop.Y + y;

#if WINFORM || ANDROID
						g.PlatformGraphics.DrawString(b.Str, b.FontInfo.Font, lastBrush, tx, ty, this.sf);
#elif WPF
						var gr = new System.Windows.Media.GlyphRun(b.FontInfo.GlyphTypeface, 0, false, r.FontSize * 1.33d,
							new ushort[] { b.GlyphIndex },
							new System.Windows.Point(tx, ty),
							new double[] { b.Width }, null, null, null, null,
							null, null);

						g.PlatformGraphics.DrawGlyphRun(lastBrush, gr);
#endif // WPF
					}
				}

				//y += p.TextSize.Height + this.paragraphSpacing;
			}

			if (!this.Overflow)
			{
				g.PopClip();
			}
		}
		#endregion // Draw

		#region Update

#if DEBUG
		public long lastUpdateMS;
#endif // DEBUG

		internal void UpdateText()
		{
#if DEBUG
			var sw = System.Diagnostics.Stopwatch.StartNew();
#endif // DEBUG

			this.measuredSize = new Size(0, 0);

			Paragraph lastP = null;

			if (this.size.Width > 0 && this.size.Height > 0)
			{
				// RGFloat y = 0;
				var rs = new RelayoutSession(this.size);

				if (this.RotationAngle > 0)
				{
					rs.d = this.RotationAngle * 3.1415926f / 180f;
					rs.s = (float)Math.Sin(rs.d);
					rs.c = (float)Math.Cos(rs.d);
				}

				foreach (var p in this.paragraphs)
				{
					p.UpdateText(rs);

					if (p.lastLine != null)
					{
						var paraSpacing = p.lastLine.Height * (p.ParagraphEndSpacing - 1.0f);

						rs.startLoc.Y += paraSpacing;
					}

					lastP = p;
				}

				this.measuredSize = rs.measuredSize;
			}

#if DEBUG
			sw.Stop();
			this.lastUpdateMS = sw.ElapsedMilliseconds;
			if (this.lastUpdateMS > 10)
			{
				Logger.Log("richtext", "update all text takes " + this.lastUpdateMS + " ms.");
			}
#endif // DEBUG
		}

		private string textBuffer = string.Empty;
		#endregion // Update

		#region ToString
		internal bool sbDirty = true;

		private StringBuilder sb = null;

		/// <summary>
		/// Convert rich format text to plain text.
		/// </summary>
		/// <returns>Plain text converted from this rich format text.</returns>
		public override string ToString()
		{
			if (this.sbDirty)
			{
				if (this.sb == null)
				{
					this.sb = new StringBuilder(256);
				}
				else
				{
					this.sb.Remove(0, this.sb.Length);
				}

				foreach (var p in this.paragraphs)
				{
					if (sb.Length > 0)
					{
						sb.Append(Environment.NewLine);
					}

					foreach (var r in p.Runs)
					{
						sb.Append(r.Text);
					}
				}

				this.textBuffer = sb.ToString();
				this.sbDirty = false;
			}

			return this.textBuffer;
		}

		/// <summary>
		/// Convert rich format text to plain text.
		/// </summary>
		/// <returns>Plain text converted from this rich format text.</returns>
		public string ToPlainText()
		{
			return this.ToString();
		}
		#endregion // ToString
	}
}

namespace unvell.ReoGrid.Drawing.Text
{
	#region Paragraph
	/// <summary>
	/// Repersents a paragraph that is a part of rich format text.
	/// </summary>
	internal class Paragraph
	{
		#region Attributes
		private RichText rt;

		private List<Run> runs = new List<Run>();

		internal List<Run> Runs { get { return this.runs; } }

		public RGFloat ParagraphStartSpacing { get; set; }

		public RGFloat ParagraphEndSpacing { get; set; }

		#endregion // Attributes

		#region Alignments
		/// <summary>
		/// Get or set the horizontal alignment for this paragraph.
		/// </summary>
		public ReoGridHorAlign HorizontalAlign { get; set; }

		/// <summary>
		/// Get or set the vertical alignment for this paragraph.
		/// </summary>
		public ReoGridVerAlign VerticalAlign { get; set; }
		#endregion // Alignments

		#region Line Height
		/// <summary>
		/// Get or set the line height scale of this paragraph.
		/// </summary>
		public RGFloat LineHeight { get; set; }
		#endregion // Line Height

		#region Constructors
		/// <summary>
		/// Create new paragraph with initiali text.
		/// </summary>
		/// <param name="text">Text to be the content of paragraph created.</param>
		internal Paragraph(RichText rt, string text = null)
		{
			this.rt = rt;
			this.LineHeight = rt.DefaultLineHeight;
			this.ParagraphStartSpacing = 0;// reserved: rt.ParagraphSpacing;
			this.ParagraphEndSpacing = rt.DefaultParagraphSpacing;

			if (!string.IsNullOrEmpty(text))
			{
				this.AddText(text);
			}
		}
		#endregion // Constructors

		#region Add Text
		/// <summary>
		/// Append text at end of line in current paragraph.
		/// </summary>
		/// <param name="text">Text to be appended.</param>
		/// <param name="fontName">Font name of the text to be appended. Null to use last font name or default font name.</param>
		/// <param name="fontSize">Font size of the text to be appended. Null to use last font size or default font size.</param>
		/// <param name="fontStyles">Style of the text to be appended. Null to use last font style or default font style.</param>
		/// <param name="textColor">Color of the text to be appended. Null to use last text color or default text color.</param>
		/// <param name="backColor">Background color of text to be appended. Null to use last background color or default background color.</param>
		public void AddText(string text, string fontName = null, RGFloat? fontSize = null, FontStyles? fontStyles = null,
			SolidColor? textColor = null, SolidColor? backColor = null)
		{
			Run lastRun = this.GetOrCreateLastRun(fontName, fontSize, fontStyles, textColor, backColor);

			lastRun.Text += text;

			this.rt.sbDirty = true;
		}

		internal Run GetOrCreateLastRun(string fontName = null, RGFloat? fontSize = null, FontStyles? fontStyles = null,
			SolidColor? textColor = null, SolidColor? backColor = null)
		{
			Run newRun = null, lastRun = null;

			if (this.runs.Count > 0)
			{
				lastRun = this.runs[this.runs.Count - 1];

				if ((fontName == null || (lastRun.FontName == fontName))
					&& (fontSize == null || (lastRun.FontSize == fontSize))
					&& (fontStyles == null || (lastRun.FontStyles == fontStyles))
					&& (textColor == null || (lastRun.TextColor == textColor))
					&& (backColor == null || (lastRun.BackColor == backColor)))
				{
					newRun = lastRun;
				}
			}

			if (newRun == null)
			{
				newRun = new Run(this.rt,
					fontName == null ? (lastRun == null ? rt.DefaultFontName : lastRun.FontName) : fontName,
					fontSize == null ? (lastRun == null ? rt.DefaultFontSize : lastRun.FontSize) : (RGFloat)fontSize,
					fontStyles == null ? (lastRun == null ? rt.DefaultFontStyles : lastRun.FontStyles) : (FontStyles)fontStyles,
					textColor == null ? (lastRun == null ? rt.DefaultTextColor : lastRun.TextColor) : (SolidColor)textColor,
					backColor == null ? (lastRun == null ? rt.DefaultBackColor : lastRun.BackColor) : (SolidColor)backColor);

				this.runs.Add(newRun);
			}

			return newRun;
		}
		#endregion // Add Text

		#region Update
		internal List<Line> lines = new List<Line>();

		internal void UpdateText(RelayoutSession rs)
		{
			this.lines.Clear();

			this.lastLine = null;

			if (this.Runs != null && this.Runs.Count > 0 && rs.bounds.Width > 0)
			{
				rs.ParagraphReset();

				var line = new Line(rs.currentLoc.Y);
				//line.Top = rs.currentLoc.Y;

				Run prevRun = null, nextRun = null;
				BoxFontInfo curFontInfo = null;

				for (int ri = 0; ri < this.Runs.Count; ri++)
				{
					var r = this.Runs[ri];

					if (curFontInfo != r.FontInfo)
					{
						curFontInfo = r.FontInfo;

						if (rs.maxBaseline < curFontInfo.Ascent) rs.maxBaseline = curFontInfo.Ascent;
						if (rs.maxHeight < curFontInfo.LineHeight) rs.maxHeight = curFontInfo.LineHeight;
					}

					//prevRun = ri > 0 ? p.Runs[ri - 1] : null;
					nextRun = ri < this.Runs.Count - 1 ? this.Runs[ri + 1] : null;

					if (!string.IsNullOrEmpty(r.Text))
					{
						for (int i = 0; i < r.Text.Length; i++)
						{
							char c = r.Text[i];//.ToString();

#if WINFORM || ANDROID
							var b = new Box(r, c.ToString(), curFontInfo, r.TextSizes[i], r.FontInfo.LineHeight);
#elif WPF
							var b = new Box(r, c.ToString(), curFontInfo, r.GlyphIndexes[i], r.TextSizes[i]);
#endif // WPF

							rs.AddCachedBox(b);

							if (CJKHelper.IsBreakable(this, prevRun, nextRun, r, i))
							{
								this.CommitWord(ref line, rs, ref curFontInfo);
							}
						}
					}

					prevRun = r;
				}

				if (rs.cacheBoxesWidth > 0)
				{
					this.CommitWord(ref line, rs, ref curFontInfo);
				}

				if (line.boxes.Count > 0)
				{
					this.CommitLine(rs, line);
				}
			}
		}

		private void CommitWord(ref Line line, /*BoxGroup cachedBoxes, List<Box> wordBoxes, ref RGFloat cachedBoxesSize,*/
			RelayoutSession rs, ref BoxFontInfo fontInfo)
		{
			bool newline = false;

			if (this.rt.TextWrap != TextWrapMode.NoWrap)
			{
				var angle = this.rt.RotationAngle;

				if (angle == 0)
				{
					newline = rs.currentLoc.X + rs.cacheBoxesWidth > rs.bounds.Width;
				}
				else
				{
					var w = rs.cacheBoxesWidth;
					var h = rs.cacheBoxesMaxHeight;

					var s = rs.s;
					var c = rs.c;

					var leftTop = rs.currentLoc;
					var rightTop = new Point(leftTop.X + w * c, leftTop.Y + w * s);
					var leftBottom = new Point(leftTop.X - h * s, leftTop.Y + h * c);
					var rightBottom = new Point(rightTop.X - h * s, rightTop.Y + h * c);

					if (angle > 0)
					{
						var maxY = Math.Max(leftTop.Y, Math.Max(rightTop.Y, Math.Max(leftBottom.Y, rightBottom.Y)));
						var maxX = Math.Max(leftTop.X, Math.Max(rightTop.X, Math.Max(leftBottom.X, rightBottom.X)));

						newline = (maxX >= rs.bounds.Width || maxY > rs.bounds.Height);
					}
					else // angle < 0
					{
						var minY = Math.Min(leftTop.Y, Math.Min(rightTop.Y, Math.Min(leftBottom.Y, rightBottom.Y)));
						var maxX = Math.Max(leftTop.X, Math.Max(rightTop.X, Math.Max(leftBottom.X, rightBottom.X)));

						newline = (maxX > rs.bounds.Width || minY < -rs.bounds.Height);
					}
				}
			}

			if (newline)
			{
				CommitLine(rs, line);
				line = new Line(rs.currentLoc.Y);
			}

			if (line.Ascent < rs.maxBaseline) line.Ascent = rs.maxBaseline;
			if (line.Height < rs.maxHeight) line.Height = rs.maxHeight;

			foreach (var cb in rs.cacheBoxes)
			{
				if (rt.RotationAngle == 0)
				{
					cb.leftTop.X = rs.currentLoc.X;
					cb.rightTop.X = cb.leftTop.X + cb.Width;
					rs.currentLoc.X += cb.Width;
				}
				else
				{
					cb.leftTop = rs.currentLoc;
					rs.currentLoc.X += (float)(cb.Width * rs.c);
					rs.currentLoc.Y += (float)(cb.Width * rs.s);
				}

				line.boxes.Add(cb);
			}

			if (newline)
			{
				rs.maxBaseline = fontInfo.Ascent;
				rs.maxHeight = fontInfo.LineHeight;
			}

			rs.CacheBoxesReset();
		}

		internal Line lastLine = null;

		private void CommitLine(RelayoutSession rs, Line line)
		{
			var angle = this.rt.RotationAngle;

			#region Last Line Spacing
			// if there is last line, plus the line spacing height
			if (lastLine != null)
			{
				RGFloat lineSpacing = lastLine.Height * (this.LineHeight - 1);

				if (angle == 0)
				{
					rs.startLoc.Y += lineSpacing;
					//line.leftTop.Y += lineSpacing;
				}
				else
				{
					foreach (var box in line.boxes)
					{
						var a = (lineSpacing) * rs.s;
						var b = (lineSpacing) * rs.c / Math.Tan(rs.d);

						box.leftTop.X -= (float)(a + b);
					}
				}
			}
			#endregion // Last Line Spacing

			this.lines.Add(line);

			#region Horizontal Relayout Boxes
			var lineWidth = line.Width;

			RGFloat horAlignOffset = 0;

			var halign = this.HorizontalAlign;

			if (halign == ReoGridHorAlign.General)
			{
				halign = this.rt.DefaultHorizontalAlignment;
			}

			switch (halign)
			{
				default:
				case ReoGridHorAlign.Left:
					horAlignOffset = 0;
					break;

				case ReoGridHorAlign.Center:
					horAlignOffset = (rs.bounds.Width - lineWidth) / 2 - 3;
					break;

				case ReoGridHorAlign.Right:
					horAlignOffset = (rs.bounds.Width - lineWidth) - 4;
					break;
			}

			foreach (var b in line.boxes)
			{
				var r = b.Run;

				if (this.rt.RotationAngle == 0)
				{
					if ((r.FontStyles & FontStyles.Superscrit) == FontStyles.Superscrit)
					{
						b.leftTop.Y = line.Top - b.FontInfo.LineHeight / 2;
					}
					else if ((r.FontStyles & FontStyles.Subscript) == FontStyles.Subscript)
					{
						b.leftTop.Y = line.Top + line.Height - b.FontInfo.LineHeight / 2;
					}
					else
					{
						b.leftTop.Y = line.Top - b.FontInfo.LineHeight + line.Ascent;
					}
				}
				else
				{
					// TODO
					//b.leftTop.X = line.leftTop.Y - b.FontInfo.LineHeight + line.Ascent;
				}

				b.leftTop.X += horAlignOffset;
			}
			#endregion // Horizontal Relayout Boxes

			var lineHeight = line.Height;

			if (angle == 0)
			{
				rs.startLoc.Y += lineHeight;

				if (rs.measuredSize.Width < line.Width)
				{
					rs.measuredSize.Width = line.Width;
				}

				rs.measuredSize.Height = rs.startLoc.Y;
			}
			else
			{
				var a = (lineHeight) * rs.s;
				var b = (lineHeight) * rs.c / Math.Tan(rs.d);

				rs.startLoc.X -= (float)(a + b);

				rs.measuredSize.Width = 200;
				rs.measuredSize.Height = 200;

			}

			rs.currentLoc = rs.startLoc;

			lastLine = line;
		}

		#endregion // Update
	}

	class RelayoutSession
	{
		public Size bounds;

		public RelayoutSession(Size bounds)
		{
			this.bounds = bounds;
		}

		public RGFloat maxBaseline;
		public RGFloat maxHeight;

		public Point startLoc;
		public Point currentLoc;

		internal void ParagraphReset()
		{
			this.currentLoc = this.startLoc;
			this.maxBaseline = 0;
			this.maxHeight = 0;
		}

		public Size measuredSize;

		public float d, s, c;

		#region Cached Boxes
		public List<Box> cacheBoxes = new List<Box>();

		public RGFloat cacheBoxesWidth;

		public RGFloat cacheBoxesMaxHeight;

		public void AddCachedBox(Box box)
		{
			this.cacheBoxes.Add(box);

			this.cacheBoxesWidth += box.Width;

			if (this.cacheBoxesMaxHeight < box.Height)
			{
				this.cacheBoxesMaxHeight = box.Height;
			}
		}

		internal void CacheBoxesReset()
		{
			this.cacheBoxes.Clear();

			this.cacheBoxesWidth = 0;
			this.cacheBoxesMaxHeight = 0;
		}
		#endregion // Cached Boxes
	}
	#endregion // Paragraph

	#region Run
	internal class Run
	{
		private RichText rt;

#if WINFORM || ANDROID
		internal List<RGFloat> TextSizes { get; private set; }
#elif WPF
		internal List<double> TextSizes { get; private set; }
		internal List<ushort> GlyphIndexes { get; private set; }
#endif // WPF

		#region Font Info
		private BoxFontInfo fontInfo = null;

		internal BoxFontInfo FontInfo
		{
			get
			{
				if (this.fontInfo == null)
				{
					this.fontInfo = new BoxFontInfo();

					RGFloat renderFontSize = this.fontSize;

					if (renderFontSize < 6) renderFontSize = 6;

					if ((this.fontStyles & FontStyles.Superscrit) == FontStyles.Superscrit
						|| (this.FontStyles & FontStyles.Subscript) == FontStyles.Subscript)
					{
						renderFontSize *= 0.6f;
					}

#if WINFORM
					var font = new System.Drawing.Font(this.fontName, (float)renderFontSize,
						(System.Drawing.FontStyle)(this.fontStyles & ~(FontStyles.Superscrit | FontStyles.Subscript | FontStyles.Underline)));

					this.fontInfo.Font = font;

					float emHeight = font.FontFamily.GetEmHeight(font.Style);
					float ascent = font.FontFamily.GetCellAscent(font.Style);
					//float descent = font.FontFamily.GetCellDescent(font.Style);
					float lineSpacing = font.FontFamily.GetLineSpacing(font.Style);

					fontInfo.Ascent = font.Size * ascent / emHeight;
					fontInfo.LineHeight = font.Size * lineSpacing / emHeight;

#elif WPF
					var typeface = new System.Windows.Media.Typeface(
						new System.Windows.Media.FontFamily(this.fontName),
						PlatformUtility.ToWPFFontStyle(this.fontStyles),
						(this.fontStyles & FontStyles.Bold) == FontStyles.Bold ?
						System.Windows.FontWeights.Bold : System.Windows.FontWeights.Normal,
						System.Windows.FontStretches.Normal);

					System.Windows.Media.GlyphTypeface glyphTypeface;
					typeface.TryGetGlyphTypeface(out glyphTypeface);

					this.fontInfo.Typeface = typeface;
					this.fontInfo.GlyphTypeface = glyphTypeface;

					fontInfo.Ascent = typeface.FontFamily.Baseline;
					fontInfo.LineHeight = typeface.CapsHeight;
#endif // WPF

					//fontInfo.Height = font.Height;
				}

				return this.fontInfo;
			}
		}
		#endregion // Font Info

		private string fontName;
		public string FontName { get { return this.fontName; } set { this.fontName = value; this.fontInfo = null; } }

		private RGFloat fontSize;
		public RGFloat FontSize { get { return this.fontSize; } set { this.fontSize = value; this.fontInfo = null; } }

		private FontStyles fontStyles;
		public FontStyles FontStyles { get { return this.fontStyles; } set { this.fontStyles = value; this.fontInfo = null; } }

		public SolidColor TextColor { get; private set; }
		public SolidColor BackColor { get; private set; }

		public Run(RichText rt, string fontName, RGFloat fontSize, FontStyles fontStyles,
			SolidColor textColor, SolidColor backColor)
		{
			this.rt = rt;
			this.text = string.Empty;

			this.FontName = fontName;
			this.FontSize = fontSize;
			this.FontStyles = fontStyles;
			this.TextColor = textColor;
			this.BackColor = backColor;

#if WINFORM
			this.TextSizes = new List<RGFloat>();
#elif WPF
			this.TextSizes = new List<double>();
			this.GlyphIndexes = new List<ushort>();
#endif // WPF
		}

		#region Text
		private string text;
		public string Text
		{
			get { return this.text; }
			set
			{
				this.text = string.Empty;
				this.TextSizes.Clear();
				this.TextSizes.Capacity = this.text.Length;

				this.AppendText(value);
			}
		}

		public void AppendText(string text)
		{
			this.text += text;

#if WINFORM
			//foreach (var c in text)
			{
				var g = ResourcePoolManager.CachedGDIGraphics;

				if (g != null)
				{

					foreach (var c in text)
					{
						var size = g.MeasureString(c.ToString(), this.FontInfo.Font, 99999999, this.rt.sf);
						this.TextSizes.Add(size.Width);
					}

					//var regions = g.MeasureCharacterRanges(text, this.FontInfo.Font,
					//	new System.Drawing.RectangleF(0, 0, 999999, 99999), this.rt.sf);

					//foreach (var r in regions)
					//{
					//	this.TextSizes.Add(r.GetBounds(g).Width);
					//}
				}
			}
#elif WPF
			var glyphTypeface = this.FontInfo.GlyphTypeface;
			var size = this.fontSize * 1.33d;

			this.GlyphIndexes.Capacity = text.Length;

			for (int n = 0; n < text.Length; n++)
			{
				ushort glyphIndex = glyphTypeface.CharacterToGlyphMap[text[n]];
				GlyphIndexes.Add(glyphIndex);

				double width = glyphTypeface.AdvanceWidths[glyphIndex] * size;
				this.TextSizes.Add(width);
			}

#endif // WINFORM
		}
		#endregion // Text

		//internal RunTypes SpanType { get; set; }
	}

	enum RunTypes
	{
		Normal,
		Superscript,
		Subscript,

		Fraction,
	}
	#endregion // Run

	#region Line & Box
	internal class Line
	{
		internal RGFloat Height { get; set; }

		//internal Point leftTop;
		//internal Point leftBottom;
		//internal Point rightTop;
		//internal Point rightBottom;

		// only use for angle=0
		internal RGFloat Top { get; set; }
		//internal RGFloat Bottom { get { return this.Top + this.Height; } }

		internal List<Box> boxes = new List<Box>();
		//public List<Box> Boxes { get { return this.boxes; } }

		public Box LastBox { get { return this.boxes.Count == 0 ? null : this.boxes[this.boxes.Count - 1]; } }
		//public RGFloat Right { get { var lastBox = this.LastBox; return lastBox == null ? 0 : lastBox.rightTop.X; } }

		internal RGFloat Ascent { get; set; }

		public Line(RGFloat top)
		{
			this.Top = top;
		}

		public RGFloat Width
		{
			get
			{
				var lastBox = boxes.Count <= 0 ? null : boxes[boxes.Count - 1];
				return lastBox == null ? 0 : lastBox.rightTop.X;
			}
		}
	}

	internal class BoxFontInfo
	{
#if WINFORM
		public System.Drawing.Font Font { get; set; }
#elif WPF
		public System.Windows.Media.Typeface Typeface { get; set; }
		public System.Windows.Media.GlyphTypeface GlyphTypeface { get; set; }
#endif // WPF

		public RGFloat Ascent { get; set; }
		//public RGFloat Height { get; set; }
		public RGFloat LineHeight { get; set; }
	}

	internal class Box
	{
		public Run Run { get; private set; }

		public string Str { get; private set; }

		//internal RGFloat Left { get; set; }
		//internal RGFloat Right { get { return this.location.X + this.Width; } }
		//internal RGFloat Top { get; set; }

		internal RGFloat Width { get; private set; }
		internal RGFloat Height { get; private set; }

		internal BoxFontInfo FontInfo { get; private set; }

		internal Point leftTop;
		//internal Point leftBottom;
		internal Point rightTop;
		//internal Point rightBottom;

		private Box(Run run, string str)
		{
			this.Run = run;
			this.Str = str;
		}

#if WINFORM
		public Box(Run run, string str, BoxFontInfo fontInfo, RGFloat width, RGFloat height)
			: this(run, str)
		{
			this.FontInfo = fontInfo;
			this.Width = width;
			this.Height = height;
		}
#elif WPF

		internal ushort GlyphIndex { get; private set; }

		public Box(Run run, string str, BoxFontInfo fontInfo, ushort glyphIndex, double width)
			: this(run, str)
		{
			this.FontInfo = fontInfo;
			this.GlyphIndex = glyphIndex;
			this.Width = width;
		}
#endif // WINFORM

		public override string ToString()
		{
			return string.Format("Box[\"{0}\"]", this.Str);
		}
	}
	#endregion // Line & Box

	#region CJKHelper
	class CJKHelper
	{
		private static readonly string left_Symbols = "$（(「【[〔『＜《≪〈<［{｛";
		private static readonly string rightSymbols = "、。,.，：）)」】]〕』＞》≫〉>］}｝";

		public static bool IsLeftSymbol(char c)
		{
			return left_Symbols.IndexOf(c) >= 0;
		}

		public static bool IsRightSymbol(char c)
		{
			return rightSymbols.IndexOf(c) >= 0;
		}

		//public static bool IsDigit(char c)
		//{
		//	return c >= 48 && c <= 57;
		//}

		//public static bool IsAnsiLetter(char c)
		//{
		//	return c >= 65 && c <= 90 || c >= 97 && c <= 122;
		//}

		public static bool IsAnsiLetterOrDigit(char c)
		{
			return (c >= 48 && c <= 57)  // 0-9
				|| (c >= 65 && c <= 90)    // A-Z
				|| (c >= 97 && c <= 122);  // a-z
		}

		public static bool IsWhiteSpace(string str, int index)
		{
			char c = str[index];
			return c == 32 || c == 7;
		}

		public static bool IsBreakable(Paragraph p, Run prevRun, Run nextRun, Run r, int index)
		{
			string str = r.Text;
			int strlen = str.Length;

			if (strlen == 0) return true;

			char c = str[index];
			char prevC = (char)0, nextC = (char)0;

			if (index >= strlen - 1)
			{
				if (nextRun != null && nextRun.Text.Length > 0)
				{
					nextC = nextRun.Text[0];
				}
			}
			else
			{
				nextC = str[index + 1];
			}

			if ((c >= 48 && c <= 57)    // 0-9
				|| (c >= 65 && c <= 90)   // A-Z
				|| (c >= 97 && c <= 122)  // a-z
				)
			{
				if ((nextC >= 0x4E00 && nextC <= 0x9FFF) // CJK Unified Ideographs
					)
				{
					return !IsRightSymbol(nextC);
				}
				else
				{
					return false;
				}
			}

			if (c == '-' || c == '.' || c == '\'' || c == '"')
			{
				if (index <= 0 && prevRun != null && prevRun.Text.Length > 0)
				{
					prevC = prevRun.Text[prevRun.Text.Length - 1];
				}

				if (prevC != 0)
				{
					return !IsAnsiLetterOrDigit(prevC);
				}
				else if (nextC != 0)
				{
					return !IsAnsiLetterOrDigit(nextC);
				}
			}

			if (IsLeftSymbol(c))
			{
				return false;
			}

			if (IsRightSymbol(nextC))
			{
				return false;
			}

			return true;
		}
	}
	#endregion // CJKHelper
}

#endif // (WINFORM || WPF)

#endif // DRAWING