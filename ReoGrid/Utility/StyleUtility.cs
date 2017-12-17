using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using unvell.Common;
using unvell.ReoGrid.Core;
using unvell.ReoGrid.Drawing.Text;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.XML;

namespace unvell.ReoGrid.Utility
{
	/// <summary>
	/// Range style utility
	/// </summary>
	public sealed class StyleUtility
	{
		/// <summary>
		/// Check whether or not the style set contains the specified item
		/// </summary>
		/// <param name="style">style set to be checked</param>
		/// <param name="flag">style item to be checked</param>
		/// <returns>true if the style set contains the specified item</returns>
		public static bool HasStyle(WorksheetRangeStyle style, PlainStyleFlag flag)
		{
			return (style.Flag & flag) == flag;
		}

		internal static PlainStyleFlag CheckDistinctStyle(WorksheetRangeStyle style, WorksheetRangeStyle referStyle)
		{
			if (style == null || style.Flag == PlainStyleFlag.None) return PlainStyleFlag.None;
			if (referStyle == null || referStyle.Flag == PlainStyleFlag.None) return PlainStyleFlag.None;

			//var returnStyle = new WorksheetRangeStyle(style);
			var distinctedFlag = style.Flag;

			if (StyleUtility.HasStyle(style, PlainStyleFlag.FillPatternColor)
				&& style.FillPatternColor == referStyle.FillPatternColor)
			{
				distinctedFlag &= ~PlainStyleFlag.FillPatternColor;
			}

			if (StyleUtility.HasStyle(style, PlainStyleFlag.FillPatternStyle)
				&& style.FillPatternStyle == referStyle.FillPatternStyle)
			{
				distinctedFlag &= ~PlainStyleFlag.FillPatternStyle;
			}

			if (StyleUtility.HasStyle(style, PlainStyleFlag.BackColor)
				&& style.BackColor == referStyle.BackColor)
			{
				distinctedFlag &= ~PlainStyleFlag.BackColor;
			}

			if (StyleUtility.HasStyle(style, PlainStyleFlag.TextColor)
				&& style.TextColor == referStyle.TextColor)
			{
				distinctedFlag &= ~PlainStyleFlag.TextColor;
			}

			if (StyleUtility.HasStyle(style, PlainStyleFlag.FontName)
				&& style.FontName == referStyle.FontName)
			{
				distinctedFlag &= ~PlainStyleFlag.FontName;
			}
			if (StyleUtility.HasStyle(style, PlainStyleFlag.FontSize)
				&& style.FontSize == referStyle.FontSize)
			{
				distinctedFlag &= ~PlainStyleFlag.FontSize;
			}

			if (StyleUtility.HasStyle(style, PlainStyleFlag.FontStyleBold)
				&& style.Bold == referStyle.Bold)
			{
				distinctedFlag &= ~PlainStyleFlag.FontStyleBold;
			}
			if (StyleUtility.HasStyle(style, PlainStyleFlag.FontStyleItalic)
				&& style.Italic == referStyle.Italic)
			{
				distinctedFlag &= ~PlainStyleFlag.FontStyleItalic;
			}
			if (StyleUtility.HasStyle(style, PlainStyleFlag.FontStyleStrikethrough)
				&& style.Strikethrough == referStyle.Strikethrough)
			{
				distinctedFlag &= ~PlainStyleFlag.FontStyleStrikethrough;
			}
			if (StyleUtility.HasStyle(style, PlainStyleFlag.FontStyleUnderline)
				&& style.Underline == referStyle.Underline)
			{
				distinctedFlag &= ~PlainStyleFlag.FontStyleUnderline;
			}

			if (StyleUtility.HasStyle(style, PlainStyleFlag.HorizontalAlign)
				&& style.HAlign == referStyle.HAlign)
			{
				distinctedFlag &= ~PlainStyleFlag.HorizontalAlign;
			}

			if (StyleUtility.HasStyle(style, PlainStyleFlag.VerticalAlign)
				&& style.VAlign == referStyle.VAlign)
			{
				distinctedFlag &= ~PlainStyleFlag.VerticalAlign;
			}

			if (StyleUtility.HasStyle(style, PlainStyleFlag.TextWrap)
				&& style.TextWrapMode == referStyle.TextWrapMode)
			{
				distinctedFlag &= ~PlainStyleFlag.TextWrap;
			}

			if (StyleUtility.HasStyle(style, PlainStyleFlag.Indent)
				&& style.Indent == referStyle.Indent)
			{
				distinctedFlag &= ~PlainStyleFlag.Indent;
			}

			if (StyleUtility.HasStyle(style, PlainStyleFlag.Padding)
				&& style.Padding == referStyle.Padding)
			{
				distinctedFlag &= ~PlainStyleFlag.Padding;
			}

			if (StyleUtility.HasStyle(style, PlainStyleFlag.RotationAngle)
				&& style.RotationAngle == referStyle.RotationAngle)
			{
				distinctedFlag &= ~PlainStyleFlag.RotationAngle;
			}

			return distinctedFlag;
		}

		internal static WorksheetRangeStyle DistinctStyle(WorksheetRangeStyle style, WorksheetRangeStyle referStyle)
		{
			if (style == null || style.Flag == PlainStyleFlag.None) return null;
			if (referStyle == null || referStyle.Flag == PlainStyleFlag.None) return new WorksheetRangeStyle(style);

			//var returnStyle = new WorksheetRangeStyle(style);
			var distinctedFlag = CheckDistinctStyle(style, referStyle);

			if (distinctedFlag == PlainStyleFlag.None)
			{
				return null;
			}
			else
			{
				var distinctedStyle = new WorksheetRangeStyle();
				StyleUtility.CopyStyle(style, distinctedStyle, distinctedFlag);
				return distinctedStyle;
			}
		}

		/// <summary>
		/// Remove repeated styles if it does same as default style.
		/// This function also can be used to create a default style for specified cell.
		/// </summary>
		/// <param name="grid">Instance of worksheet.</param>
		/// <param name="cell">The style from this cell will be checked.</param>
		/// <returns>Checked style, null if given cell or style of cell is null.</returns>
		internal static WorksheetRangeStyle CheckAndRemoveCellStyle(Worksheet grid, Cell cell)
		{
			if (cell.InnerStyle == null || cell.InnerStyle == null) return null;

			// if cell keeps parent style, then return null (means no cell own styles existed)
			if (cell.StyleParentKind == StyleParentKind.Root
				|| cell.StyleParentKind == StyleParentKind.Col
				|| cell.StyleParentKind == StyleParentKind.Row)
			{
				return null;
			}

			int row = cell.InternalRow;
			int col = cell.InternalCol;

			WorksheetRangeStyle style = cell.InnerStyle;

			StyleParentKind pKind = StyleParentKind.Own;
			WorksheetRangeStyle defaultStyle = FindCellParentStyle(grid, row, col, out pKind);

			return DistinctStyle(style, defaultStyle);
		}

		internal static void CopyStyle(WorksheetRangeStyle sourceStyle, WorksheetRangeStyle targetStyle)
		{
			CopyStyle(sourceStyle, targetStyle, sourceStyle.Flag);
		}

		internal static void CopyStyle(WorksheetRangeStyle sourceStyle, WorksheetRangeStyle targetStyle, PlainStyleFlag flag)
		{
			if ((flag & PlainStyleFlag.BackColor) == PlainStyleFlag.BackColor)
				targetStyle.BackColor = sourceStyle.BackColor;

			if ((flag & PlainStyleFlag.FillPatternColor) == PlainStyleFlag.FillPatternColor)
				targetStyle.FillPatternColor = sourceStyle.FillPatternColor;

			if ((flag & PlainStyleFlag.FillPatternStyle) == PlainStyleFlag.FillPatternStyle)
				targetStyle.FillPatternStyle = sourceStyle.FillPatternStyle;

			if ((flag & PlainStyleFlag.TextColor) == PlainStyleFlag.TextColor)
				targetStyle.TextColor = sourceStyle.TextColor;

			if ((flag & PlainStyleFlag.FontName) == PlainStyleFlag.FontName)
			{
				targetStyle.FontName = sourceStyle.FontName;
				System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(targetStyle.FontName));
			}

			if ((flag & PlainStyleFlag.FontSize) == PlainStyleFlag.FontSize)
				targetStyle.FontSize = sourceStyle.FontSize;

			if ((flag & PlainStyleFlag.FontStyleBold) == PlainStyleFlag.FontStyleBold)
				targetStyle.Bold = sourceStyle.Bold;

			if ((flag & PlainStyleFlag.FontStyleItalic) == PlainStyleFlag.FontStyleItalic)
				targetStyle.Italic = sourceStyle.Italic;

			if ((flag & PlainStyleFlag.FontStyleStrikethrough) == PlainStyleFlag.FontStyleStrikethrough)
				targetStyle.Strikethrough = sourceStyle.Strikethrough;

			if ((flag & PlainStyleFlag.FontStyleUnderline) == PlainStyleFlag.FontStyleUnderline)
				targetStyle.Underline = sourceStyle.Underline;

			if ((flag & PlainStyleFlag.HorizontalAlign) == PlainStyleFlag.HorizontalAlign)
				targetStyle.HAlign = sourceStyle.HAlign;

			if ((flag & PlainStyleFlag.VerticalAlign) == PlainStyleFlag.VerticalAlign)
				targetStyle.VAlign = sourceStyle.VAlign;

			if ((flag & PlainStyleFlag.TextWrap) == PlainStyleFlag.TextWrap)
				targetStyle.TextWrapMode = sourceStyle.TextWrapMode;

			if ((flag & PlainStyleFlag.Indent) == PlainStyleFlag.Indent)
				targetStyle.Indent = sourceStyle.Indent;

			if ((flag & PlainStyleFlag.Padding) == PlainStyleFlag.Padding)
				targetStyle.Padding = sourceStyle.Padding;

			if ((flag & PlainStyleFlag.RotationAngle) == PlainStyleFlag.RotationAngle)
				targetStyle.RotationAngle = sourceStyle.RotationAngle;

			targetStyle.Flag |= flag;
		}

		internal static WorksheetRangeStyle CreateMergedStyle(WorksheetRangeStyle style1, WorksheetRangeStyle style2)
		{
			var style = new WorksheetRangeStyle()
			{
				Flag = style1.Flag | style2.Flag,
			};

			var flag1 = style1.Flag;
			var flag2 = style2.Flag;

			if ((flag1 & PlainStyleFlag.BackColor) == PlainStyleFlag.BackColor)
				style.BackColor = style1.BackColor;
			else if ((flag2 & PlainStyleFlag.BackColor) == PlainStyleFlag.BackColor)
				style.BackColor = style2.BackColor;

			if ((flag1 & PlainStyleFlag.FillPatternColor) == PlainStyleFlag.FillPatternColor)
				style.FillPatternColor = style1.FillPatternColor;
			else if ((flag2 & PlainStyleFlag.FillPatternColor) == PlainStyleFlag.FillPatternColor)
				style.FillPatternColor = style2.FillPatternColor;

			if ((flag1 & PlainStyleFlag.FillPatternStyle) == PlainStyleFlag.FillPatternStyle)
				style.FillPatternStyle = style1.FillPatternStyle;
			else if ((flag2 & PlainStyleFlag.FillPatternStyle) == PlainStyleFlag.FillPatternStyle)
				style.FillPatternStyle = style2.FillPatternStyle;

			if ((flag1 & PlainStyleFlag.TextColor) == PlainStyleFlag.TextColor)
				style.TextColor = style1.TextColor;
			else if ((flag2 & PlainStyleFlag.TextColor) == PlainStyleFlag.TextColor)
				style.TextColor = style2.TextColor;

			if ((flag1 & PlainStyleFlag.FontName) == PlainStyleFlag.FontName)
				style.FontName = style1.FontName;
			else if ((flag2 & PlainStyleFlag.FontName) == PlainStyleFlag.FontName)
				style.FontName = style2.FontName;

			if ((flag1 & PlainStyleFlag.FontSize) == PlainStyleFlag.FontSize)
				style.FontSize = style1.FontSize;
			else if ((flag2 & PlainStyleFlag.FontSize) == PlainStyleFlag.FontSize)
				style.FontSize = style2.FontSize;

			if ((flag1 & PlainStyleFlag.FontStyleBold) == PlainStyleFlag.FontStyleBold)
				style.Bold = style1.Bold;
			else if ((flag2 & PlainStyleFlag.FontStyleBold) == PlainStyleFlag.FontStyleBold)
				style.Bold = style2.Bold;

			if ((flag1 & PlainStyleFlag.FontStyleItalic) == PlainStyleFlag.FontStyleItalic)
				style.Italic = style1.Italic;
			else if ((flag2 & PlainStyleFlag.FontStyleItalic) == PlainStyleFlag.FontStyleItalic)
				style.Italic = style2.Italic;

			if ((flag1 & PlainStyleFlag.FontStyleStrikethrough) == PlainStyleFlag.FontStyleStrikethrough)
				style.Strikethrough = style1.Strikethrough;
			else if ((flag2 & PlainStyleFlag.FontStyleStrikethrough) == PlainStyleFlag.FontStyleStrikethrough)
				style.Strikethrough = style2.Strikethrough;

			if ((flag1 & PlainStyleFlag.FontStyleUnderline) == PlainStyleFlag.FontStyleUnderline)
				style.Underline = style1.Underline;
			else if ((flag2 & PlainStyleFlag.FontStyleUnderline) == PlainStyleFlag.FontStyleUnderline)
				style.Underline = style2.Underline;

			if ((flag1 & PlainStyleFlag.HorizontalAlign) == PlainStyleFlag.HorizontalAlign)
				style.HAlign = style1.HAlign;
			else if ((flag2 & PlainStyleFlag.HorizontalAlign) == PlainStyleFlag.HorizontalAlign)
				style.HAlign = style2.HAlign;

			if ((flag1 & PlainStyleFlag.VerticalAlign) == PlainStyleFlag.VerticalAlign)
				style.VAlign = style1.VAlign;
			else if ((flag2 & PlainStyleFlag.VerticalAlign) == PlainStyleFlag.VerticalAlign)
				style.VAlign = style2.VAlign;

			if ((flag1 & PlainStyleFlag.TextWrap) == PlainStyleFlag.TextWrap)
				style.TextWrapMode = style1.TextWrapMode;
			else if ((flag2 & PlainStyleFlag.TextWrap) == PlainStyleFlag.TextWrap)
				style.TextWrapMode = style2.TextWrapMode;

			if ((flag1 & PlainStyleFlag.Indent) == PlainStyleFlag.Indent)
				style.Indent = style1.Indent;
			else if ((flag2 & PlainStyleFlag.Indent) == PlainStyleFlag.Indent)
				style.Indent = style2.Indent;

			if ((flag1 & PlainStyleFlag.Padding) == PlainStyleFlag.Padding)
				style.Padding = style1.Padding;
			else if ((flag2 & PlainStyleFlag.Padding) == PlainStyleFlag.Padding)
				style.Padding = style2.Padding;

			if ((flag1 & PlainStyleFlag.RotationAngle) == PlainStyleFlag.RotationAngle)
				style.RotationAngle = style1.RotationAngle;
			else if ((flag2 & PlainStyleFlag.RotationAngle) == PlainStyleFlag.RotationAngle)
				style.RotationAngle = style2.RotationAngle;

			return style;
		}

		internal static WorksheetRangeStyle FindCellParentStyle(Worksheet sheet, int row, int col, out StyleParentKind pKind)
		{
			RowHeader rowhead = sheet.RetrieveRowHeader(row);

			if (rowhead.InnerStyle != null)
			{
				pKind = StyleParentKind.Row;
				return rowhead.InnerStyle;
			}
			else
			{
				ColumnHeader colhead = sheet.RetrieveColumnHeader(col);

				if (colhead.InnerStyle != null)
				{
					pKind = StyleParentKind.Col;
					return colhead.InnerStyle;
				}
				else
				{
					pKind = StyleParentKind.Root;
					return sheet.RootStyle;
				}
			}
		}

		internal static void UpdateCellParentStyle(Worksheet grid, Cell cell)
		{
			RowHeader rowhead = grid.RetrieveRowHeader(cell.InternalRow);

			if (rowhead.InnerStyle != null)
			{
				cell.InnerStyle = rowhead.InnerStyle;
				cell.StyleParentKind = StyleParentKind.Row;
			}
			else
			{
				ColumnHeader colhead = grid.RetrieveColumnHeader(cell.InternalCol);

				if (colhead.InnerStyle != null)
				{
					cell.InnerStyle = colhead.InnerStyle;
					cell.StyleParentKind = StyleParentKind.Col;
				}
				else
				{
					cell.InnerStyle = grid.RootStyle;
					cell.StyleParentKind = StyleParentKind.Root;
				}
			}
		}

		internal static RGXmlCellStyle ConvertToXmlStyle(WorksheetRangeStyle style)
		{
			if (style == null || style.Flag == PlainStyleFlag.None) return null;

			RGXmlCellStyle xmlStyle = new RGXmlCellStyle();

			if (StyleUtility.HasStyle(style, PlainStyleFlag.BackColor))
			{
				xmlStyle.backColor = TextFormatHelper.EncodeColor(style.BackColor);
			}

			if (HasStyle(style, PlainStyleFlag.FillPattern))
			{
				RGXmlCellStyleFillPattern xmlFillPattern = new RGXmlCellStyleFillPattern()
				{
					color = TextFormatHelper.EncodeColor(style.FillPatternColor),
					patternStyleId = (int)style.FillPatternStyle,
				};
				xmlStyle.fillPattern = xmlFillPattern;
			}

			if (StyleUtility.HasStyle(style, PlainStyleFlag.TextColor))
				xmlStyle.textColor = TextFormatHelper.EncodeColor(style.TextColor);
			if (StyleUtility.HasStyle(style, PlainStyleFlag.FontName))
				xmlStyle.font = style.FontName;
			if (StyleUtility.HasStyle(style, PlainStyleFlag.FontSize))
				xmlStyle.fontSize = style.FontSize.ToString();
			if (StyleUtility.HasStyle(style, PlainStyleFlag.FontStyleBold))
				xmlStyle.bold = style.Bold.ToString().ToLower();
			if (StyleUtility.HasStyle(style, PlainStyleFlag.FontStyleItalic))
				xmlStyle.italic = style.Italic.ToString().ToLower();
			if (StyleUtility.HasStyle(style, PlainStyleFlag.FontStyleStrikethrough))
				xmlStyle.strikethrough = style.Strikethrough.ToString().ToLower();
			if (StyleUtility.HasStyle(style, PlainStyleFlag.FontStyleUnderline))
				xmlStyle.underline = style.Underline.ToString().ToLower();
			if (StyleUtility.HasStyle(style, PlainStyleFlag.HorizontalAlign))
				xmlStyle.hAlign = XmlFileFormatHelper.EncodeHorizontalAlign(style.HAlign);
			if (StyleUtility.HasStyle(style, PlainStyleFlag.VerticalAlign))
				xmlStyle.vAlign = XmlFileFormatHelper.EncodeVerticalAlign(style.VAlign);
			if (StyleUtility.HasStyle(style, PlainStyleFlag.TextWrap))
				xmlStyle.textWrap = XmlFileFormatHelper.EncodeTextWrapMode(style.TextWrapMode);
			if (StyleUtility.HasStyle(style, PlainStyleFlag.Indent))
				xmlStyle.indent = style.Indent.ToString();
			if (StyleUtility.HasStyle(style, PlainStyleFlag.Padding))
				xmlStyle.padding = TextFormatHelper.EncodePadding(style.Padding);
			if (StyleUtility.HasStyle(style, PlainStyleFlag.RotationAngle))
				xmlStyle.rotateAngle = style.RotationAngle.ToString();

			return xmlStyle;
		}

		internal static WorksheetRangeStyle ConvertFromXmlStyle(Worksheet grid, RGXmlCellStyle xmlStyle,
			CultureInfo culture)
		{
			WorksheetRangeStyle style = new WorksheetRangeStyle();

			if (xmlStyle == null) return style;

			// back color
			if (!string.IsNullOrEmpty(xmlStyle.backColor))
			{
				SolidColor color;

				if (TextFormatHelper.DecodeColor(xmlStyle.backColor, out color))
				{
					style.Flag |= PlainStyleFlag.BackColor;
					style.BackColor = color;
				}
			}

			// fill pattern
			if (xmlStyle.fillPattern != null)
			{
				SolidColor color;
				if (TextFormatHelper.DecodeColor(xmlStyle.fillPattern.color, out color))
				{
					style.Flag |= PlainStyleFlag.FillPattern;
					style.FillPatternColor = color;
					style.FillPatternStyle = (HatchStyles)xmlStyle.fillPattern.patternStyleId;
				}
			}

			// text color
			if (!string.IsNullOrEmpty(xmlStyle.textColor))
			{
				SolidColor color;
				if (TextFormatHelper.DecodeColor(xmlStyle.textColor, out color))
				{
					style.Flag |= PlainStyleFlag.TextColor;
					style.TextColor = color;
				}
			}

			// horizontal align
			if (!string.IsNullOrEmpty(xmlStyle.hAlign))
			{
				style.Flag |= PlainStyleFlag.HorizontalAlign;
				style.HAlign = XmlFileFormatHelper.DecodeHorizontalAlign(xmlStyle.hAlign);
			}
			// vertical align
			if (!string.IsNullOrEmpty(xmlStyle.vAlign))
			{
				style.Flag |= PlainStyleFlag.VerticalAlign;
				style.VAlign = XmlFileFormatHelper.DecodeVerticalAlign(xmlStyle.vAlign);
			}

			// font name
			if (!string.IsNullOrEmpty(xmlStyle.font))
			{
				style.Flag |= PlainStyleFlag.FontName;
				style.FontName = xmlStyle.font;
			}
			// font size
			if (xmlStyle.fontSize != null)
			{
				style.Flag |= PlainStyleFlag.FontSize;
				style.FontSize = TextFormatHelper.GetFloatValue(xmlStyle.fontSize, grid.RootStyle.FontSize, culture);
			}

			// bold
			if (xmlStyle.bold != null)
			{
				style.Flag |= PlainStyleFlag.FontStyleBold;
				style.Bold = xmlStyle.bold == "true";
			}
			// italic
			if (xmlStyle.italic != null)
			{
				style.Flag |= PlainStyleFlag.FontStyleItalic;
				style.Italic = xmlStyle.italic == "true";
			}
			// strikethrough
			if (xmlStyle.strikethrough != null)
			{
				style.Flag |= PlainStyleFlag.FontStyleStrikethrough;
				style.Strikethrough = xmlStyle.strikethrough == "true";
			}
			// underline
			if (xmlStyle.underline != null)
			{
				style.Flag |= PlainStyleFlag.FontStyleUnderline;
				style.Underline = xmlStyle.underline == "true";
			}

			// text-wrap
			if (!string.IsNullOrEmpty(xmlStyle.textWrap))
			{
				style.Flag |= PlainStyleFlag.TextWrap;
				style.TextWrapMode = XmlFileFormatHelper.DecodeTextWrapMode(xmlStyle.textWrap);
			}

			// padding
			if (!string.IsNullOrEmpty(xmlStyle.indent))
			{
				style.Flag |= PlainStyleFlag.Indent;

				int indent = TextFormatHelper.GetPixelValue(xmlStyle.indent, 0);
				if (indent > 0 && indent < 65535)
				{
					style.Indent = (ushort)indent;
				}
			}

			// padding
			if (!string.IsNullOrEmpty(xmlStyle.padding))
			{
				style.Flag |= PlainStyleFlag.Padding;
				style.Padding = TextFormatHelper.DecodePadding(xmlStyle.padding);
			}

			// rotate angle
			int angle;
			if (!string.IsNullOrEmpty(xmlStyle.rotateAngle) 
				&& int.TryParse(xmlStyle.rotateAngle, out angle))
			{
				style.Flag |= PlainStyleFlag.RotationAngle;
				style.RotationAngle = angle;
			}

			return style;
		}

		internal static void UpdateCellRenderAlign(Worksheet ctrl, Cell cell)
		{
			if (cell.InnerStyle != null && cell.InnerStyle.HAlign != ReoGridHorAlign.General)
			{
				switch (cell.InnerStyle.HAlign)
				{
					case ReoGridHorAlign.Left:
						cell.RenderHorAlign = ReoGridRenderHorAlign.Left;
						break;
					case ReoGridHorAlign.Center:
						cell.RenderHorAlign = ReoGridRenderHorAlign.Center;
						break;
					case ReoGridHorAlign.Right:
						cell.RenderHorAlign = ReoGridRenderHorAlign.Right;
						break;
				}
			}
		}

		internal bool Equals(WorksheetRangeStyle styleA, WorksheetRangeStyle styleB)
		{
			if (styleA == null && styleB != null
				|| styleA != null && styleB == null
				|| styleA.Flag != styleB.Flag)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.BackColor)
				&& styleA.BackColor != styleB.BackColor)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.FillPatternColor)
				&& styleA.FillPatternColor != styleB.FillPatternColor)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.FillPatternStyle)
				&& styleA.FillPatternStyle != styleB.FillPatternStyle)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.TextColor)
				&& styleA.TextColor != styleB.TextColor)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.FontName)
				&& styleA.FontName != styleB.FontName)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.FontSize)
				&& styleA.FontSize != styleB.FontSize)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.FontStyleBold)
				&& styleA.Bold != styleB.Bold)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.FontStyleItalic)
				&& styleA.Italic != styleB.Italic)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.FontStyleStrikethrough)
				&& styleA.Strikethrough != styleB.Strikethrough)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.FontStyleUnderline)
				&& styleA.Underline != styleB.Underline)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.HorizontalAlign)
				&& styleA.HAlign != styleB.HAlign)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.VerticalAlign)
				&& styleA.VAlign != styleB.VAlign)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.TextWrap)
				&& styleA.TextWrapMode != styleB.TextWrapMode)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.Indent)
				&& styleA.Indent != styleB.Indent)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.Padding)
				&& styleA.Padding != styleB.Padding)
				return false;

			if (styleA.HasStyle(PlainStyleFlag.RotationAngle)
				&& styleA.RotationAngle != styleB.RotationAngle)
				return false;

			return true;
		}

		/// <summary>
		/// Get single style from style set
		/// </summary>
		/// <param name="style">style set to find specified single style</param>
		/// <param name="flag">single style specifeid by this flag to be get</param>
		/// <returns>single style as object returned from style set</returns>
		public static object GetStyleItem(WorksheetRangeStyle style, PlainStyleFlag flag)
		{
			switch (flag)
			{
				case PlainStyleFlag.BackColor: return style.BackColor;
				case PlainStyleFlag.FillPatternColor: return style.FillPatternColor;
				case PlainStyleFlag.FillPatternStyle: return style.FillPatternStyle;
				case PlainStyleFlag.TextColor: return style.TextColor;
				case PlainStyleFlag.FontName: return style.FontName;
				case PlainStyleFlag.FontSize: return style.FontSize;
				case PlainStyleFlag.FontStyleBold: return style.Bold;
				case PlainStyleFlag.FontStyleItalic: return style.Italic;
				case PlainStyleFlag.FontStyleUnderline: return style.Underline;
				case PlainStyleFlag.FontStyleStrikethrough: return style.Strikethrough;
				case PlainStyleFlag.HorizontalAlign: return style.HAlign;
				case PlainStyleFlag.VerticalAlign: return style.VAlign;
				case PlainStyleFlag.TextWrap: return style.TextWrapMode;
				case PlainStyleFlag.Indent: return style.Indent;
				case PlainStyleFlag.Padding: return style.Padding;
				case PlainStyleFlag.RotationAngle: return style.RotationAngle;

				default: return null;
			}
		}

		/// <summary>
		/// Convert to union font style flag from worksheet styleset object.
		/// </summary>
		/// <param name="style"></param>
		/// <returns></returns>
		public static FontStyles CreateFontStyle(WorksheetRangeStyle style)
		{
			FontStyles fontStyle = FontStyles.Regular;

			if (style.HasStyle(PlainStyleFlag.FontStyleBold) && style.Bold) fontStyle |= FontStyles.Bold;
			if (style.HasStyle(PlainStyleFlag.FontStyleItalic) && style.Italic) fontStyle |= FontStyles.Italic;
			if (style.HasStyle(PlainStyleFlag.FontStyleStrikethrough) && style.Strikethrough) fontStyle |= FontStyles.Strikethrough;
			if (style.HasStyle(PlainStyleFlag.FontStyleUnderline) && style.Underline) fontStyle |= FontStyles.Underline;

			return fontStyle;
		}
	}

}
