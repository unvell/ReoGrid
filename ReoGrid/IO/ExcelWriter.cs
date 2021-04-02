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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using unvell.ReoGrid.Utility;
using unvell.ReoGrid.IO.OpenXML.Schema;

using RGWorkbook = unvell.ReoGrid.IWorkbook;
using RGWorksheet = unvell.ReoGrid.Worksheet;
using unvell.ReoGrid.DataFormat;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Graphics;

namespace unvell.ReoGrid.IO.OpenXML
{
	#region Writer

	internal sealed class ExcelWriter
	{
		public static void WriteStream(RGWorkbook rgWorkbook, Stream stream)
		{
			//Document doc = new Document();
			var doc = Document.CreateOnStream(stream);

			WriteDefaultStyles(doc, rgWorkbook);

			var workbook = doc.Workbook;

			foreach (var rgSheet in rgWorkbook.Worksheets)
			{
				WriteWorksheet(doc, rgSheet);

				if (rgSheet.NamedRanges.Count > 0)
				{
					if (workbook.definedNames == null)
					{
						workbook.definedNames = new List<DefinedName>();
					}

					foreach (var range in rgSheet.NamedRanges)
					{
						workbook.definedNames.Add(new DefinedName
						{
							name = range.Name,
							address = rgSheet.Name + "!" + range.Position.ToAbsoluteAddress(),
						});
					}
				}
			}

			doc.Flush();
		}

		#region Fill

		private static int WriteFill(Document doc, SolidColor c)
		{
			var styles = doc.Stylesheet;

			var existedIndex = styles.fills.FindIndex(f =>
				f.patternFill.patternType == "solid" && f.patternFill.foregroundColor._rgColor == c);

			if (existedIndex >= 0)
			{
				return existedIndex;
			}

			styles.fills.Add(new Fill
			{
				patternFill = new PatternFill
				{
					patternType = "solid",
					foregroundColor = new ColorValue(c) { _rgColor = c },
					backgroundColor = new ColorValue { indexed = "64" },
				},
			});

			return styles.fills.Count - 1;
		}

		#endregion // Fill

		#region Font

		private static int WriteFont(Document doc, WorksheetRangeStyle rgStyle)
		{
			var styles = doc.Stylesheet;

			var flag = rgStyle.Flag;
			var fontName = rgStyle.FontName;
			var fontSize = rgStyle.FontSize;

			var existedIndex = styles.fonts.FindIndex(f =>
				((flag & PlainStyleFlag.FontName) == PlainStyleFlag.FontName
				&& f.name != null && string.Equals(f.name.value, fontName, StringComparison.CurrentCultureIgnoreCase))
				&& ((flag & PlainStyleFlag.FontSize) == PlainStyleFlag.FontSize && f.size != null && f._size == fontSize)
				&& (((flag & PlainStyleFlag.FontStyleBold) == PlainStyleFlag.FontStyleBold && f.bold != null && f.bold != null)
					|| ((flag & PlainStyleFlag.FontStyleBold) != PlainStyleFlag.FontStyleBold && f.bold == null))
				&& (((flag & PlainStyleFlag.FontStyleItalic) == PlainStyleFlag.FontStyleItalic && f.italic != null && f.italic != null)
					|| ((flag & PlainStyleFlag.FontStyleItalic) != PlainStyleFlag.FontStyleItalic && f.italic == null))
				&& (((flag & PlainStyleFlag.FontStyleStrikethrough) == PlainStyleFlag.FontStyleStrikethrough && f.strikethrough != null && f.strikethrough != null)
					|| ((flag & PlainStyleFlag.FontStyleStrikethrough) != PlainStyleFlag.FontStyleStrikethrough && f.strikethrough == null))
				&& (((flag & PlainStyleFlag.FontStyleUnderline) == PlainStyleFlag.FontStyleUnderline && f.underline != null && f.underline != null)
					|| ((flag & PlainStyleFlag.FontStyleUnderline) != PlainStyleFlag.FontStyleUnderline && f.underline == null))
				&& (((flag & PlainStyleFlag.TextColor) == PlainStyleFlag.TextColor && f.color != null
					&& f.color._rgColor != null && f.color._rgColor.Value == rgStyle.TextColor)
					|| ((flag & PlainStyleFlag.TextColor) != PlainStyleFlag.TextColor && f.color == null))
				);

			if (existedIndex >= 0)
			{
				return existedIndex;
			}

			styles.fonts.Add(new Schema.Font(rgStyle));

			return styles.fonts.Count - 1;
		}

		#endregion // Font

		#region Border
		#region Border Style Convert
		private static string ConvertToExcelBorderStyle(BorderLineStyle bls)
		{
			switch (bls)
			{
				default:

				case BorderLineStyle.Solid:
					return "thin";
				case BorderLineStyle.BoldSolid:
					return "medium";
				case BorderLineStyle.BoldSolidStrong:
					return "thick";

				case BorderLineStyle.Dotted:
					return "hair";
				case BorderLineStyle.BoldDotted:
					return "mediumDashDotDot";

				case BorderLineStyle.DoubleLine:
					return "double";

				case BorderLineStyle.Dashed:
					return "dashed";
				case BorderLineStyle.BoldDashDot:
					return "mediumDashed";
				case BorderLineStyle.BoldDashed:
					return "mediumDashDot";
				case BorderLineStyle.BoldDashDotDot:
					return "mediumDashDotDot";

				case BorderLineStyle.Dashed2:
					return "dotted";
				case BorderLineStyle.DashDot:
					return "dashDotDot";
				case BorderLineStyle.DashDotDot:
					return "slantDashDot";

					//case BorderLineStyle.Dotted:
					//	return "hair"; // todo: what's this?
			}
		}
		#endregion // Border Style Convert

		private static int WriteBorder(Document doc, RangeBorderStyle? top, RangeBorderStyle? bottom,
			RangeBorderStyle? left, RangeBorderStyle? right)
		{
			var styles = doc.Stylesheet;

			var existedIndex = styles.borders.FindIndex(_b
					=> ((_b.top != null && top != null && _b._top == top.Value) || (_b.top == null && top == null))
					&& ((_b.bottom != null && bottom != null && _b._bottom == bottom.Value) || (_b.bottom == null && bottom == null))
					&& ((_b.left != null && left != null && _b._left == left.Value) || (_b.left == null && left == null))
					&& ((_b.right != null && right != null && _b._right == right.Value) || (_b.right == null && right == null)));

			if (existedIndex >= 0)
			{
				return existedIndex;
			}

			var b = new Border();

			if (top != null)
			{
				b.top = new SideBorder
				{
					color = new ColorValue(top.Value.Color),
					style = ConvertToExcelBorderStyle(top.Value.Style),
				};
				b._top = top.Value;
			}

			if (bottom != null)
			{
				b.bottom = new SideBorder
				{
					color = new ColorValue(bottom.Value.Color),
					style = ConvertToExcelBorderStyle(bottom.Value.Style),
				};
				b._bottom = bottom.Value;
			}

			if (left != null)
			{
				b.left = new SideBorder
				{
					color = new ColorValue(left.Value.Color),
					style = ConvertToExcelBorderStyle(left.Value.Style),
				};
				b._left = left.Value;
			}

			if (right != null)
			{
				b.right = new SideBorder
				{
					color = new ColorValue(right.Value.Color),
					style = ConvertToExcelBorderStyle(right.Value.Style),
				};
				b._right = right.Value;
			}

			styles.borders.Add(b);

			return styles.borders.Count - 1;
		}
		#endregion // Border

		#region Data Format
		private static string ConvertToExcelNumberPattern(NumberDataFormatter.INumberFormatArgs arg,
			string prefix = null, string postfix = null)
		{
			//NumberDataFormatter.NumberFormatArgs narg;

			#region Decimal Digit Places
			StringBuilder sbDigits = new StringBuilder();

			if (arg.UseSeparator)
			{
				sbDigits.Append("#,##");
			}

			sbDigits.Append('0');

			if (arg.DecimalPlaces > 0)
			{
				sbDigits.Append('.');

				for (int i = 0; i < arg.DecimalPlaces; i++)
				{
					sbDigits.Append('0');
				}
			}

			string digits = sbDigits.ToString();
			#endregion // Decimal Digit Places

			if (!string.IsNullOrEmpty(prefix))
			{
				prefix = "\"" + prefix + "\"";
			}

			if (!string.IsNullOrEmpty(postfix))
			{
				postfix = "\"" + postfix + "\"";
			}

			StringBuilder sb = new StringBuilder();

			if (!string.IsNullOrEmpty(prefix)) sb.Append(prefix);
			sb.Append(digits);
			if (!string.IsNullOrEmpty(postfix)) sb.Append(postfix);
			sb.Append(';');

			#region Negative part
			switch (arg.NegativeStyle)
			{
				default:
				case NumberDataFormatter.NumberNegativeStyle.Default:
					if (!string.IsNullOrEmpty(prefix)) sb.Append(prefix);
					sb.Append('-');
					sb.Append(digits);
					if (!string.IsNullOrEmpty(postfix)) sb.Append(postfix);
					break;

				case NumberDataFormatter.NumberNegativeStyle.Red:
				case NumberDataFormatter.NumberNegativeStyle.RedMinus:
					if (!string.IsNullOrEmpty(prefix)) sb.Append(prefix);
					sb.Append("[Red]");
					if ((arg.NegativeStyle & NumberDataFormatter.NumberNegativeStyle.Minus) == NumberDataFormatter.NumberNegativeStyle.Minus) sb.Append("-");
					sb.Append(digits);
					if (!string.IsNullOrEmpty(postfix)) sb.Append(postfix);
					break;

				case NumberDataFormatter.NumberNegativeStyle.Brackets:
				case NumberDataFormatter.NumberNegativeStyle.BracketsMinus:
					if (!string.IsNullOrEmpty(prefix)) sb.Append(prefix);
					sb.Append('(');
					if ((arg.NegativeStyle & NumberDataFormatter.NumberNegativeStyle.Minus) == NumberDataFormatter.NumberNegativeStyle.Minus) sb.Append("-");
          sb.Append(digits);
					sb.Append(')');
					if (!string.IsNullOrEmpty(postfix)) sb.Append(postfix);
					break;

				case NumberDataFormatter.NumberNegativeStyle.RedBrackets:
				case NumberDataFormatter.NumberNegativeStyle.RedBracketsMinus:
					if (!string.IsNullOrEmpty(prefix)) sb.Append(prefix);
					sb.Append("[Red](");
					if ((arg.NegativeStyle & NumberDataFormatter.NumberNegativeStyle.Minus) == NumberDataFormatter.NumberNegativeStyle.Minus) sb.Append("-");
					sb.Append(digits);
					sb.Append(')');
					if (!string.IsNullOrEmpty(postfix)) sb.Append(postfix);
					break;

#if LANG_JP
				case NumberDataFormatter.NumberNegativeStyle.Prefix_Sankaku:
					if (!string.IsNullOrEmpty(prefix)) sb.Append(prefix);
					sb.Append("▲");
					sb.Append(digits);
					if (!string.IsNullOrEmpty(postfix)) sb.Append(postfix);
					break;
#endif // LANG_JP
			}

			sb.Append(';');
			#endregion // Negative part

			if (!string.IsNullOrEmpty(prefix)) sb.Append(prefix);
			sb.Append(digits);
			if (!string.IsNullOrEmpty(postfix)) sb.Append(postfix);

			return sb.ToString();
		}

		private const int BaseUserNumberFormatId = 165;

		private static int WriteNumberFormat(CellDataFormatFlag flag, object arg, Stylesheet styles)
		{
			if (styles.numberFormats == null)
			{
				styles.numberFormats = new NumberFormatCollection();
			}

			NumberDataFormatter.NumberFormatArgs narg;
			CurrencyDataFormatter.CurrencyFormatArgs carg;
			DateTimeDataFormatter.DateTimeFormatArgs dtarg;

			switch (flag)
			{
				case CellDataFormatFlag.Number:
					#region Number
					{
						if (arg == null) return 1;

						narg = (NumberDataFormatter.NumberFormatArgs)arg;

						int id = 0;

						for (int i = 0; i < styles.numberFormats.list.Count; i++)
						{
							var numFmt = styles.numberFormats.list[i];

							if (numFmt._iarg.Equals(narg))
							{
								id = i + BaseUserNumberFormatId;
								break;
							}
						}

						if (id > 0)
						{
							return id;
						}

						id = styles.numberFormats.Count + BaseUserNumberFormatId;

						string prefix = null, postfix = null;

						if (narg.NegativeStyle == NumberDataFormatter.NumberNegativeStyle.CustomSymbol)
						{
							prefix = narg.CustomNegativePrefix;
							postfix = narg.CustomNegativePostfix;
						}

						string formatCode = ConvertToExcelNumberPattern(narg, prefix, postfix);

						styles.numberFormats.Add(new NumberFormat
						{
							_iarg = narg,
							formatId = id,
							formatCode = formatCode,
						});

						return id;
					}
				#endregion Number

				case CellDataFormatFlag.Percent:
					#region Percent
					if (arg == null)
					{
						return 9;
					}
					else
					{
						narg = (NumberDataFormatter.NumberFormatArgs)arg;
						if (narg.DecimalPlaces == 0)
							return 9;
						else
							return 10;
					}
				#endregion // Percent

				case CellDataFormatFlag.Currency:
					#region Currency
					{
						if (arg == null)
						{
							return 1;
						}

						carg = (CurrencyDataFormatter.CurrencyFormatArgs)arg;

						int id = 0;

						for (int i = 0; i < styles.numberFormats.list.Count; i++)
						{
							var numFmt = styles.numberFormats.list[i];

							if (numFmt._iarg.Equals(arg))
							{
								id = i + BaseUserNumberFormatId;
								break;
							}
						}

						if (id > 0)
						{
							return id;
						}

						var currencyPattern = ConvertToExcelNumberPattern(carg, carg.PrefixSymbol, carg.PostfixSymbol);

						id = styles.numberFormats.Count + BaseUserNumberFormatId;

						styles.numberFormats.Add(new NumberFormat
						{
							_iarg = carg,
							formatId = id,
							formatCode = currencyPattern,
						});

						return id;
					}
				#endregion // Currency

				case CellDataFormatFlag.DateTime:
					#region DateTime
					if (arg == null)
					{
						return 14;
					}

					dtarg = (DateTimeDataFormatter.DateTimeFormatArgs)arg;

					switch (dtarg.Format)
					{
						case "yyyy/MM/dd": return 14;
						case "d-MMM-yy": return 15;
						case "d-MMM": return 16;
						case "MMM-yy": return 17;
						case "h:mm tt": return 18;
						case "h:mm:ss tt": return 19;
						case "h:mm": return 20;
						case "H:mm:ss": return 21;
						case "M/d/yy h:mm": return 22;
						case "mm:ss": return 45;
						case "h:mm:ss": return 46;
						case "mmss.f": return 47;

						default:
							{
								int id = styles.numberFormats.Count + BaseUserNumberFormatId;

								styles.numberFormats.Add(new NumberFormat
								{
									_iarg = dtarg,
									formatId = id,
									formatCode = dtarg.Format,
								});

								return id;
							}
					}
				#endregion // DateTime

				case CellDataFormatFlag.Text:
					return 49;

			}

			return -1;
		}

		#endregion // Data Format

		#region Style

		private static string ConvertToExcelHorAlign(ReoGridHorAlign halign)
		{
			switch (halign)
			{
				default:
				case ReoGridHorAlign.General:
					return "general";
				case ReoGridHorAlign.Left:
					return "left";
				case ReoGridHorAlign.Center:
					return "center";
				case ReoGridHorAlign.Right:
					return "right";
			}
		}

		private static string ConvertToExcelVerAlign(ReoGridVerAlign valign)
		{
			switch (valign)
			{
				default:
				case ReoGridVerAlign.General:
					return "general";
				case ReoGridVerAlign.Top:
					return "top";
				case ReoGridVerAlign.Middle:
					return "center";
				case ReoGridVerAlign.Bottom:
					return "bottom";
			}
		}

		private static int WriteStyle(Document doc, WorksheetRangeStyle rgStyle,
			RangeBorderStyle? top = null, RangeBorderStyle? bottom = null,
			RangeBorderStyle? left = null, RangeBorderStyle? right = null,
			CellDataFormatFlag dataFormatFlag = CellDataFormatFlag.General, object dataFormatArg = null)
		{
			var styles = doc.Stylesheet;

			string fillId = null, fontId = null, borderId = null, numberFormatId = null;

			if (rgStyle != null && (rgStyle.Flag & PlainStyleFlag.BackColor) == PlainStyleFlag.BackColor)
			{
				fillId = WriteFill(doc, rgStyle.BackColor).ToString();
			}

			if (rgStyle != null && (rgStyle.Flag & (PlainStyleFlag.FontAll | PlainStyleFlag.TextColor)) > 0)
			{
				fontId = WriteFont(doc, rgStyle).ToString();
			}

			if (top != null || bottom != null || left != null || right != null)
			{
				borderId = WriteBorder(doc, top, bottom, left, right).ToString();
			}

			if (dataFormatFlag != CellDataFormatFlag.General)
			{
				int numFmt = WriteNumberFormat(dataFormatFlag, dataFormatArg, doc.Stylesheet);

				if (numFmt > 0)
				{
					numberFormatId = numFmt.ToString();
				}
			}

			bool horAlign = false, verAlign = false, textWrap = false, textRotate = false;
			float? indent = null;

			if (rgStyle != null)
			{
				if ((rgStyle.Flag & PlainStyleFlag.HorizontalAlign) == PlainStyleFlag.HorizontalAlign
					&& rgStyle.HAlign != ReoGridHorAlign.General)
				{
					horAlign = true;

					if ((rgStyle.Flag & PlainStyleFlag.Indent) == PlainStyleFlag.Indent)
					{
						indent = rgStyle.Indent;
					}
				}

				if ((rgStyle.Flag & PlainStyleFlag.VerticalAlign) == PlainStyleFlag.VerticalAlign
					// Excel treats that 'General' and 'Bottom' are the same
					&& rgStyle.VAlign != ReoGridVerAlign.General
					&& rgStyle.VAlign != ReoGridVerAlign.Bottom)
				{
					verAlign = true;
				}

				if ((rgStyle.Flag & PlainStyleFlag.TextWrap) == PlainStyleFlag.TextWrap
					&& rgStyle.TextWrapMode != TextWrapMode.NoWrap)
				{
					textWrap = true;
				}

				if ((rgStyle.Flag & PlainStyleFlag.RotationAngle) == PlainStyleFlag.RotationAngle
					&& rgStyle.RotationAngle != 0)
				{
					textRotate = true;
				}
			}

			int existedStyleIndex = styles.cellFormats.FindIndex(s =>

				s.fillId == fillId
				&& s.fontId == fontId
				&& s.numberFormatId == numberFormatId

				&& s.borderId == borderId

				&& ((horAlign && s.alignment != null && s.alignment._horAlign == rgStyle.HAlign)
				|| (!horAlign && (s.alignment == null || s.alignment.horizontal == null)))

				&& ((verAlign && s.alignment != null && s.alignment._verAlign == rgStyle.VAlign)
				|| (!verAlign && (s.alignment == null || s.alignment.vertical == null)))

				&& ((textWrap && s.alignment != null && s.alignment.wrapText == "1")
				|| (!textWrap && (s.alignment == null || s.alignment.wrapText == null)))

				&& ((textRotate && s.alignment != null && s.alignment._rotateAngle == (int)rgStyle.RotationAngle)
				|| (!textRotate && (s.alignment == null || s.alignment.textRotation == null)))

				);

			if (existedStyleIndex >= 0)
			{
				return existedStyleIndex;
			}

			var style = new CellFormat()
			{
				fillId = fillId,
				applyFill = (fillId != null) ? "1" : null,
				fontId = fontId,
				applyFont = (fontId != null) ? "1" : null,
				borderId = borderId,
				applyBorder = (borderId != null) ? "1" : null,
				numberFormatId = numberFormatId,
				applyNumberFormat = !string.IsNullOrEmpty(numberFormatId) ? "1" : null,
			};

			if (horAlign || verAlign || textWrap || textRotate)
			{
				var align = new Alignment();

				style.alignment = align;

				if (horAlign)
				{
					align.horizontal = ConvertToExcelHorAlign(rgStyle.HAlign);
					align._horAlign = rgStyle.HAlign;
					if (rgStyle.HAlign == ReoGridHorAlign.Left)
					{
						align.indent = Convert.ToString(rgStyle.Indent);
					}
				}

				if (verAlign)
				{
					align.vertical = ConvertToExcelVerAlign(rgStyle.VAlign);
					align._verAlign = rgStyle.VAlign;
				}

				if (textWrap)
				{
					align.wrapText = "1";
				}

				if (textRotate)
				{
					align._rotateAngle = (int)rgStyle.RotationAngle;
					align.textRotation = (rgStyle.RotationAngle < 0 ? Math.Abs(rgStyle.RotationAngle - 90) : rgStyle.RotationAngle).ToString();
				}

				style.applyAlignment = "true";
			}

			System.Diagnostics.Debug.Assert(fillId != null || fontId != null || numberFormatId != null || borderId != null || !horAlign || !verAlign);

			styles.cellFormats.Add(style);

			return styles.cellFormats.Count - 1;
		}

		private static void WriteDefaultStyles(Document doc, RGWorkbook rgWorkbook)
		{
			doc.Stylesheet = doc.CreateStyles();

			var style = doc.Stylesheet;
			var defaulStyle = rgWorkbook.Worksheets[0].RootStyle;

			style.fonts.Add(new Schema.Font(defaulStyle));
			style.fills.Add(new Fill { patternFill = new PatternFill { patternType = "none" } });
			style.fills.Add(new Fill { patternFill = new PatternFill { patternType = "gray125" } });
			style.borders.Add(new Border
			{
				left = new SideBorder(),
				right = new SideBorder(),
				top = new SideBorder(),
				bottom = new SideBorder(),
				diagonal = new SideBorder(),
			});
			style.cellFormats.Add(new CellFormat
			{
				xfId = "0",
				numberFormatId = "0",
				fontId = "0",
				fillId = "0",
				borderId = "0",
			});
			style.cellStyleFormats.Add(new CellFormat
			{
				numberFormatId = "0",
				fontId = "0",
				fillId = "0",
				borderId = "0",
			});
			style.cellStyles.Add(new CellStyle { name = "Normal", xfId = "0", builtinId = "0", });

			#region Indexed Colors

			//style.colors = new IndexedColors { indexedColors = new List<ColorValue>() };
			//foreach (var c in IndexedColorTable.colors)
			//{
			//	style.colors.indexedColors.Add(new ColorValue { rgb = c.ToString("X6") });
			//}

			#endregion // Indexed Colors
		}

		#endregion // Style

		#region Shared Strings
		private static int AddSharedString(Document doc, string str)
		{
			if (doc.SharedStrings == null)
			{
				doc.SharedStrings = doc.CreateSharedStrings();
			}

			var id = doc.SharedStrings.items.FindIndex(s => s.text != null && s.text.val == str);

			if (id < 0)
			{
				doc.SharedStrings.items.Add(new SharedStringItem { text = new ElementText(str) });
				id = doc.SharedStrings.items.Count - 1;
			}

			return id;
		}

#if DRAWING
		private static int AddSharedString(Document doc, Drawing.RichText rt)
		{
			if (doc.SharedStrings == null)
			{
				doc.SharedStrings = doc.CreateSharedStrings();
			}

			var id = doc.SharedStrings.items.FindIndex(s => s._rt == rt);

			if (id < 0)
			{
				var ssi = new SharedStringItem()
				{
					runs = new List<Run>(),
					_rt = rt,
				};

				Run lastRun = null;

				foreach (var p in rt.Paragraphcs)
				{
					if (lastRun != null)
					{
						lastRun.text.innerText += Environment.NewLine;
						lastRun = null;
					}

					foreach (var r in p.Runs)
					{
		#region Run Properties
						var rpr = new RunProperty
						{
							color = new ColorValue(r.TextColor),
							font = r.FontName,
							//family = "1",
							size = r.FontSize.ToString(EnglishCulture),
							//schema = "minor",
						};

						if ((r.FontStyles & Drawing.Text.FontStyles.Strikethrough) == Drawing.Text.FontStyles.Strikethrough)
						{
							rpr.strike = new ElementValue<string>();
						}

						if ((r.FontStyles & Drawing.Text.FontStyles.Bold) == Drawing.Text.FontStyles.Bold)
						{
							rpr.b = new ElementValue<string>();
						}

						if ((r.FontStyles & Drawing.Text.FontStyles.Italic) == Drawing.Text.FontStyles.Italic)
						{
							rpr.i = new ElementValue<string>();
						}

						if ((r.FontStyles & Drawing.Text.FontStyles.Underline) == Drawing.Text.FontStyles.Underline)
						{
							rpr.u = new ElementValue<string>();
						}

						if ((r.FontStyles & Drawing.Text.FontStyles.Superscrit) == Drawing.Text.FontStyles.Superscrit)
						{
							rpr.vertAlign = "superscript";
						}
						else if ((r.FontStyles & Drawing.Text.FontStyles.Subscript) == Drawing.Text.FontStyles.Subscript)
						{
							rpr.vertAlign = "subscript";
						}
		#endregion // Run Properties

						ssi.runs.Add(lastRun = new Run
						{
							property = rpr,

							text = new Text
							{
								innerText = r.Text,
								space = (r.Text.StartsWith(" ") || r.Text.EndsWith(" ")) ? "preserve" : null,
							},
						});
					}
				}

				doc.SharedStrings.items.Add(ssi);

				id = doc.SharedStrings.items.Count - 1;
			}

			return id;
		}
#endif // DRAWING
		#endregion // Shared Strings

		#region Drawing Objects
#if DRAWING

		private static CellAnchor CreateCellAnchorByLocation(Worksheet rgSheet, Point p)
		{
			var dpi = PlatformUtility.GetDPI();

			int row = 0, col = 0;

			rgSheet.FindColumnByPosition(p.X, out col);
			rgSheet.FindRowByPosition(p.Y, out row);

			var colHeader = rgSheet.RetrieveColumnHeader(col);
			int colOffset = MeasureToolkit.PixelToEMU(p.X - colHeader.Left, dpi);

			var rowHeader = rgSheet.RetrieveRowHeader(row);
			int rowOffset = MeasureToolkit.PixelToEMU(p.Y - rowHeader.Top, dpi);

			return new CellAnchor
			{
				col = col,
				colOff = colOffset,
				row = row,
				rowOff = rowOffset
			};
		}

		private static ShapeProperty CreateShapeProperty(ReoGrid.Drawing.DrawingObject image)
		{
			var dpi = PlatformUtility.GetDPI();

			return new ShapeProperty
			{
				transform = new Transform
				{
					offset = new Offset
					{
						x = MeasureToolkit.PixelToEMU(image.X, dpi),
						y = MeasureToolkit.PixelToEMU(image.Y, dpi),
					},
					extents = new Extents
					{
						cx = MeasureToolkit.PixelToEMU(image.Right, dpi),
						cy = MeasureToolkit.PixelToEMU(image.Bottom, dpi),
					},
				},

				prstGeom = new PresetGeometry
				{
					presetType = "rect",
					avList = new List<ShapeGuide>(),
				},
			};
		}

		private static void WriteImage(Document doc, Schema.Worksheet sheet, Schema.Drawing drawing,
			Worksheet rgSheet, unvell.ReoGrid.Drawing.ImageObject image)
		{
			if (drawing.twoCellAnchors == null)
			{
				drawing.twoCellAnchors = new List<TwoCellAnchor>();
			}

			string typeName = image.GetFriendlyTypeName();

			int typeObjCount = 0;

			drawing._typeObjectCount.TryGetValue(typeName, out typeObjCount);
			typeObjCount++;

			drawing._typeObjectCount[typeName] = typeObjCount;

			var twoCellAnchor = new Schema.TwoCellAnchor
			{
				from = CreateCellAnchorByLocation(rgSheet, image.Location),
				to = CreateCellAnchorByLocation(rgSheet, new Point(image.Right, image.Bottom)),

				pic = new Pic
				{
					nvPicPr = new NvPicProperty
					{
						cNvPr = new NonVisualProp
						{
							id = drawing._drawingObjectCount++,
							name = typeName + " " + typeObjCount,
						},

						cNvPicPr = new CNvPicProperty
						{
							picLocks = new PicLocks(),
						}
					},

					blipFill = new BlipFill
					{
						blip = doc.AddMediaImage(sheet, drawing, image),

						stretch = new Stretch
						{
							fillRect = new FillRect(),
						},
					},

					prop = CreateShapeProperty(image),
				},

				clientData = new ClientData(),
			};

			drawing.twoCellAnchors.Add(twoCellAnchor);
		}
#endif // DRAWING
		#endregion // Drawing Objects

		#region Worksheet
		internal static readonly System.Globalization.CultureInfo EnglishCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");

		private static void WriteWorksheet(Document doc, RGWorksheet rgSheet)
		{
			if (rgSheet.Rows == 0 || rgSheet.Columns == 0)
			{
#if DEBUG
				System.Diagnostics.Debug.Assert(false, "rows or columns on worksheet must not be zero.");
#endif
				return;
			}

			var sheet = doc.Workbook.CreateWorksheet(rgSheet.Name);

			var dpix = PlatformUtility.GetDPI();
			const float fixedCharWidth = 7f; // todo: get from default font
			const float columnWidthPad = 0f; // todo: openxml std: 5f

			int maxRows = Math.Max(rgSheet.MaxContentRow, 1);
			int maxCols = Math.Max(rgSheet.MaxContentCol, 1);

			#region Sheet Properties

			var dimensionRange = new RangePosition(0, 0, maxRows + 1, maxCols + 1);

			sheet.dimension = new Dimension
			{
				address = dimensionRange.IsSingleCell ? dimensionRange.StartPos.ToRelativeAddress() : dimensionRange.ToRelativeAddress(),
			};

			sheet.sheetViews = new List<SheetView>(new SheetView[]
				{
					new SheetView
					{
						tabSelected = rgSheet.controlAdapter == null ? (rgSheet.workbook.GetWorksheetIndex(rgSheet) == 0 ? "1": "0")
							: (rgSheet == rgSheet.controlAdapter.ControlInstance.CurrentWorksheet ? "1" : "0"),
						workbookViewId = "0",
						//selection = new Selection { activeCell = "A1", sqref = "A1", },
						showGridLines = rgSheet.HasSettings(WorksheetSettings.View_ShowGridLine) ? null : "0",
						showRowColHeaders = rgSheet.settings.HasAny(WorksheetSettings.View_ShowHeaders) ? null :"0",
						zoomScale = rgSheet.ScaleFactor == 1 ? null : (rgSheet.ScaleFactor * 100).ToString(EnglishCulture),
					},
				});

			var frozenPos = rgSheet.FreezePos;

			if (frozenPos.Row > 0 || frozenPos.Col > 0)
			{
				sheet.sheetViews[0].pane = new Pane
				{
					activePane = "bottomRight",
					state = "frozen",
					topLeftCell = frozenPos.ToRelativeAddress(),
					xSplit = frozenPos.Col.ToString(),
					ySplit = frozenPos.Row.ToString(),
				};
			}

			if (rgSheet.defaultRowHeight != Worksheet.InitDefaultRowHeight)
			{
				sheet.sheetFormatProperty = new SheetFormatProperty
				{
					defaultRowHeight = (rgSheet.defaultRowHeight * 72f / dpix).ToString(EnglishCulture),
					customHeight = "1",
				};
			}

			#endregion // Sheet Properties

			#region Columns

			var firstColHeader = rgSheet.RetrieveColumnHeader(0);

			int lastColIndex = 0;
			int lastColWidth = firstColHeader.InnerWidth;

			int lastColStyleId;

			if (firstColHeader.InnerStyle != null && !firstColHeader.InnerStyle.Equals(rgSheet.RootStyle))
			{
				lastColStyleId = WriteStyle(doc, firstColHeader.InnerStyle);
			}
			else
			{
				lastColStyleId = -1;
			}

			bool lastColAutoWidth = firstColHeader.IsAutoWidth;

			for (int i = 1; i <= maxCols; i++)
			{
				var colHeader = rgSheet.RetrieveColumnHeader(i);

				int colStyleId = -1;

				if (colHeader.InnerStyle != null && !colHeader.InnerStyle.Equals(rgSheet.RootStyle))
				{
					colStyleId = WriteStyle(doc, colHeader.InnerStyle);
				}

				int colWidth = colHeader.InnerWidth;

				if (colWidth != lastColWidth || colStyleId != lastColStyleId
					|| colHeader.IsAutoWidth != lastColAutoWidth)
				{
					bool customWidth = lastColWidth != rgSheet.defaultColumnWidth;

					if (sheet.cols == null)
					{
						sheet.cols = new List<Column>();
					}

					var pixel = lastColWidth - columnWidthPad;

					sheet.cols.Add(new Column
					{
						min = lastColIndex + 1,
						max = i,
						width = Math.Truncate((pixel) / fixedCharWidth * 100.0 + 0.5) / 100.0,
						customWidth = !lastColAutoWidth ? "1" : null,
						style = lastColStyleId >= 0 ? lastColStyleId.ToString() : null,
					});

					lastColWidth = colWidth;
					lastColIndex = i;
					lastColStyleId = colStyleId;
					lastColAutoWidth = colHeader.IsAutoWidth;
				}
			}

			if (lastColIndex > 0 && lastColIndex <= maxCols)
			{
				if (lastColAutoWidth /* default is !lastColAutoWidth, TODO: snyc with WorkwheetSettings.Edit_Default */
					|| lastColWidth != rgSheet.defaultColumnWidth
					|| lastColStyleId >= 0)
				{
					if (sheet.cols == null)
					{
						sheet.cols = new List<Column>();
					}

					sheet.cols.Add(new Column
					{
						min = lastColIndex + 1,
						max = maxCols + 1,
						width = Math.Truncate((lastColWidth - columnWidthPad) / fixedCharWidth * 100.0 + 0.5) / 100.0,
						customWidth = !lastColAutoWidth ? "1" : null,
						style = lastColStyleId >= 0 ? lastColStyleId.ToString() : null,
					});

					lastColIndex = maxCols + 1;
				}
			}

			if (lastColIndex > 0 && lastColIndex < rgSheet.Columns)
			{
				if (sheet.cols == null)
				{
					sheet.cols = new List<Column>();
				}

				sheet.cols.Add(new Column
				{
					min = lastColIndex + 1,
					max = rgSheet.Columns,
					width = Math.Truncate((rgSheet.defaultColumnWidth - columnWidthPad) / fixedCharWidth * 100.0 + 0.5) / 100.0,
				});
			}

			#endregion // Columns

			RowHeader rowHeader = null;
			//int lastRow = -1;

			Row row = null;

			//#if WINFORM || WPF
			//			const int colPageSize = unvell.ReoGrid.Data.JaggedTreeArray<ReoGridCell>.ColSize;
			//#elif ANDROID || iOS
			//			const int colPageSize = unvell.ReoGrid.Data.ReoGridCellArray.ColSize;
			//#endif // ANDROID

			for (int r = 0; r <= maxRows; r++)
			{
				#region Row

				rowHeader = rgSheet.RetrieveRowHeader(r);

				if (rowHeader.InnerHeight != rgSheet.defaultRowHeight
					|| !rowHeader.IsAutoHeight
					|| rowHeader.InnerStyle != null)
				{
					row = new Row()
					{
						index = (r + 1),
						//spans = "1:1",
						//dyDescent = "0.25",

						hidden = !rowHeader.IsVisible ? "1" : null,
					};

					//if (rowHeader.InnerHeight != rgSheet.defaultRowHeight) row.customHeight = "1";
					if (!rowHeader.IsAutoHeight) row.customHeight = "1";

					row.height = (rowHeader.InnerHeight * 72f / dpix).ToString(EnglishCulture);

					if (rowHeader.InnerStyle != null)
					{
						row.styleIndex = WriteStyle(doc, rowHeader.InnerStyle).ToString();
						row.customFormat = "1";
					}
				}

				#endregion Row

				for (int c = 0; c <= maxCols;)
				{
					//if (rgSheet.IsPageNull(r, c))
					//{
					//	c += colPageSize - c % colPageSize;
					//	continue;
					//}

					var rgCell = rgSheet.GetCell(r, c);

					int spans = 1;
					Schema.Cell cell = null;
					WorksheetRangeStyle rgStyle = null;
					CellDataFormatFlag dflag = CellDataFormatFlag.General;
					object dfarg = null;

					#region Cell
					// not an valid cell, skip it spans
					if (rgCell != null)
					{
						if (!rgCell.IsValidCell && !rgCell.IsStartMergedCell)
						{
							spans = rgCell.MergeEndPos.Col - c;
							if (spans <= 0) spans = 1;
						}
						else
						{
							cell = new Schema.Cell
							{
								address = RGUtility.ToAddress(r, c),
							};

							#region Data
							// data
							var data = rgCell.InnerData;

							bool hasFormula = rgCell.HasFormula;


							if (CellUtility.IsNumberData(data))
							{
								cell.value = new ElementText(Convert.ToString(data, EnglishCulture));
							}
#if DRAWING
							else if (data is Drawing.RichText)
							{
								int sharedStrId = AddSharedString(doc, (Drawing.RichText)data);
								cell.value = new ElementText(sharedStrId.ToString());
								cell.dataType = "s";
							}
#endif // DRAWING
							else if (data is DateTime)
							{
								var dt = (DateTime)data;
								double days = (dt - DateTimeDataFormatter.BaseStartDate).TotalDays + 1;
								if (days > 59) days++;

								cell.value = new ElementText(Convert.ToString(days, EnglishCulture));
							}
							else if (data != null)
							{
								var str = data is string ? (string)data : Convert.ToString(data);

								if (str.Length > 0)
								{
									if (hasFormula)
									{
										cell.dataType = "str";
										cell.value = new ElementText(str);
									}
									else if (data != null)
									{
										int sharedStrId = AddSharedString(doc, str);
										cell.value = new ElementText(sharedStrId.ToString());
										cell.dataType = "s";
									}
								}
							}

#endregion // Data

#region Formula
							// formula
							if (hasFormula)
							{
								cell.formula = new Schema.Formula { val = rgCell.InnerFormula };
							}
#endregion // Formula

#region Data Format
							if (rgCell.DataFormat != DataFormat.CellDataFormatFlag.General)
							{
								dflag = rgCell.DataFormat;
								dfarg = rgCell.DataFormatArgs;
							}
#endregion // Data Format

							// cell styles
							switch (rgCell.StyleParentKind)
							{
								case Core.StyleParentKind.Root:
									rgStyle = rgSheet.RootStyle;
									break;
								case Core.StyleParentKind.Row:
									rgStyle = rowHeader.InnerStyle;
									break;
								case Core.StyleParentKind.Col:
									{
										var colHeader = rgSheet.RetrieveColumnHeader(c);
										rgStyle = colHeader.InnerStyle;
									}
									break;
								default:
								case Core.StyleParentKind.Own:
									rgStyle = rgCell.InnerStyle;
									break;
							}

							// merged cell
							if (rgCell.IsStartMergedCell && (rgCell.Colspan > 1 || rgCell.Rowspan > 1))
							{
								if (sheet.mergeCells == null)
								{
									sheet.mergeCells = new List<MergeCell>();
								}

								sheet.mergeCells.Add(new MergeCell { address = new RangePosition(rgCell.MergeStartPos, rgCell.MergeEndPos).ToAddress() });

								spans = rgCell.Colspan;
							}
						}
					}
#endregion // Cell

#region Border

					var top = rgSheet.GetValidHBorderByPos(r, c, Core.HBorderOwnerPosition.Top);
					var bottom = rgSheet.GetValidHBorderByPos(r + 1, c, Core.HBorderOwnerPosition.Bottom);
					var left = rgSheet.GetValidVBorderByPos(r, c, Core.VBorderOwnerPosition.Left);
					var right = rgSheet.GetValidVBorderByPos(r, c + 1, Core.VBorderOwnerPosition.Right);

#endregion // Border

#region Style
					if (rgStyle == null && top == null && bottom == null && left == null && right == null)
					{
						c += spans;
						continue;
					}

					if (cell == null)
					{
						cell = new Schema.Cell
						{
							address = RGUtility.ToAddress(r, c),
						};
					}

					//if (rgCell.StyleParentKind == Core.StyleParentKind.Own
					//	|| rgCell.StyleParentKind == Core.StyleParentKind.Range)
					//{
					cell.styleIndex = WriteStyle(doc, rgStyle,
						top != null ? (RangeBorderStyle?)top.Style : null,
						bottom != null ? (RangeBorderStyle?)bottom.Style : null,
						left != null ? (RangeBorderStyle?)left.Style : null,
						right != null ? (RangeBorderStyle?)right.Style : null,
						dflag, dfarg).ToString();
					//}

#endregion Style

#region Add into row

					if (row == null)
					{
						row = new Row()
						{
							index = (r + 1),
							//spans = "1:1",
							//dyDescent = "0.25",
						};
					}

					if (row.cells == null)
					{
						row.cells = new List<Schema.Cell>();
					}

					row.cells.Add(cell);

#endregion // Add into row

					c++;
				}

				if (row != null && ((row.cells != null && row.cells.Count > 0) || row.height != null))
				{
					sheet.rows.Add(row);
				}

				row = null;
			}

#region Floating Objects
#if DRAWING
			if (rgSheet.FloatingObjects != null && rgSheet.FloatingObjects.Count > 0)
			{
				var drawing = doc.CreateDrawing(sheet);

				foreach (var obj in rgSheet.FloatingObjects)
				{
					if (obj is Drawing.ImageObject)
					{
						var img = (Drawing.ImageObject)obj;

						if (img.Image != null)
						{
							WriteImage(doc, sheet, drawing, rgSheet, (Drawing.ImageObject)obj);
						}
					}
				}
			}
#endif // DRAWING
#endregion // Floating Objects
		}
#endregion // Worksheet
	}

#endregion // Writer

#region Document
	internal partial class Document : OpenXMLFile
	{
		internal ContentType contentType;

		internal AppProperties appProp;

		internal CoreProperties coreProp;

		internal static Document CreateOnStream(Stream stream)
		{
			Document doc = new Document()
			{
				//zipArchive = NET35ZipArchiveFactory.OpenOnStream(stream, FileMode.Create, FileAccess.Write, true),
				zipArchive = MZipArchiveFactory.CreateOnStream(stream),
				_relationFile = new Relationships("_rels/.rels"),
			};

			doc.contentType = new ContentType()
			{
				Defaults = new List<ContentTypeDefaultItem>()
				{
					new ContentTypeDefaultItem { Extension = "rels", ContentType = OpenXMLContentTypes.Relationships_ },
					new ContentTypeDefaultItem { Extension = "xml", ContentType = OpenXMLContentTypes.XML___________ },
				},

				Overrides = new List<ContentTypeOverrideItem>(),
			};

			doc.CreateWorkbook();

			doc.CreateCoreProperties();

			doc.CreateAppProperties();

			return doc;
		}

		internal Schema.Workbook CreateWorkbook()
		{
			this.Workbook = new Schema.Workbook
			{
				_doc = this,
				_xmlTarget = "xl/workbook.xml",
				_relationFile = new Relationships("xl/_rels/workbook.xml.rels"),

				fileVersion = new FileVersion(),
				workbookPr = new WorkbookProperties(),
				bookViews = new BookViews
				{
					workbookView = new WorkbookView()
					{
						xWindow = "0",
						yWindow = "0",
						windowWidth = "16384",
						windowHeight = "8192",
					}
				},
			};

			this.Workbook._resId = this.AddRelationship(OpenXMLRelationTypes.xl_workbook______, this.Workbook._xmlTarget);

			this.contentType.Overrides.Add(new ContentTypeOverrideItem
			{
				PartName = "/" + this.Workbook._xmlTarget,
				ContentType = OpenXMLContentTypes.Workbook______,
			});

			return this.Workbook;
		}

		internal Stylesheet CreateStyles()
		{
			this.Stylesheet = new Stylesheet()
			{
				_xmlTarget = "xl/styles.xml",

				borders = new BorderCollection(),
				cellFormats = new CellFormatCollection(),
				colors = new IndexedColors(),
				fills = new FillCollection(),
				fonts = new FontCollection(),
				//numberFormats = new List<NumberFormat>(),
				cellStyles = new CellStyleCollection(),
				cellStyleFormats = new CellFormatCollection(),
			};

			this.Workbook.AddRelationship(OpenXMLRelationTypes.styles___________, "styles.xml");

			this.contentType.Overrides.Add(new ContentTypeOverrideItem
			{
				PartName = "/" + this.Stylesheet._xmlTarget,
				ContentType = OpenXMLContentTypes.Styles________,
			});

			return this.Stylesheet;
		}

		internal SharedStrings CreateSharedStrings()
		{
			this.SharedStrings = new SharedStrings
			{
				_xmlTarget = "xl/sharedStrings.xml",
				items = new List<SharedStringItem>(),
			};

			this.Workbook.AddRelationship(OpenXMLRelationTypes.shared_strings___, "sharedStrings.xml");

			this.contentType.Overrides.Add(new ContentTypeOverrideItem
			{
				PartName = "/" + this.SharedStrings._xmlTarget,
				ContentType = OpenXMLContentTypes.SharedStrings_,
			});

			return this.SharedStrings;
		}

#region Drawing & Images
#if DRAWING
		private int drawingFileCount = 0;

		internal IO.OpenXML.Schema.Drawing CreateDrawing(Schema.Worksheet sheet)
		{
			string drawingFileName = string.Format("drawing{0}.xml", ++drawingFileCount);

			var drawing = new IO.OpenXML.Schema.Drawing()
			{
				_xmlTarget = "xl/drawings/" + drawingFileName,
				_rsTarget = "xl/drawings/_rels/" + drawingFileName + ".rels",

				_typeObjectCount = new Dictionary<string, int>(),
			};

			string resId = sheet.AddRelationship(OpenXMLRelationTypes.drawing__________, "../drawings/" + drawingFileName);

			this.contentType.Overrides.Add(new ContentTypeOverrideItem
			{
				PartName = "/xl/drawings/" + drawingFileName,
				ContentType = OpenXMLContentTypes.Drawing_______,
			});

			sheet.drawing = new SheetDrawing { id = resId, _instance = drawing };

			return drawing;
		}

		private int mediaImageCount = 0;

		internal Blip AddMediaImage(Schema.Worksheet sheet, Schema.Drawing drawing, Drawing.ImageObject image)
		{
			var imageFileName = string.Format("image{0}.png", ++mediaImageCount);

			if (drawing._images == null)
			{
				drawing._images = new List<Blip>();
			}

			var resId = drawing.AddRelationship(OpenXMLRelationTypes.image____________, "../media/" + imageFileName);

			var blip = new Blip
			{
				_imageObject = image,
				_imageFileName = imageFileName,
				embedId = resId,
			};

			drawing._images.Add(blip);

			if (!this.contentType.Defaults.Any(ct => ct.Extension == "png"))
			{
				this.contentType.Defaults.Add(new ContentTypeDefaultItem
				{
					Extension = "png",
					ContentType = "image/png",
				});
			}

			return blip;
		}
#endif // DRAWING
#endregion // Drawing & Images

		internal void CreateCoreProperties()
		{
			this.coreProp = new CoreProperties()
			{
				creator = new InnerTextElement(string.Empty),
				lastModifiedBy = new InnerTextElement(string.Empty),
				modified = new OpenXMLDateTime(DateTime.Now),
				created = new OpenXMLDateTime(DateTime.Now),
				_xmlTarget = "docProps/core.xml",
			};

			this.coreProp._resId = this.AddRelationship(OpenXMLRelationTypes.docProps_core____, this.coreProp._xmlTarget);

			this.contentType.Overrides.Add(new ContentTypeOverrideItem
			{
				PartName = "/" + this.coreProp._xmlTarget,
				ContentType = OpenXMLContentTypes.CoreProperties,
			});
		}

		internal void CreateAppProperties()
		{
			//var cas = typeof(IWorkbook).Assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyVersionAttribute), false);

			this.appProp = new AppProperties
			{
				//headingPairs = new HeadingPairs
				//{
				//	vector = new Vector
				//	{
				//		size = "2",
				//		baseType = "variant",
				//		variants = new List<Variant>{
				//			new Variant{
				//				lpstr=new InnerTextElement( "Worksheets"),
				//			},
				//			new	Variant{
				//				i4=new InnerTextElement("1"),
				//			},
				//		},
				//	},
				//},

				//titlesOfParts = new TitlesOfParts
				//{
				//	vector = new Vector
				//	{
				//		size = "1",
				//		baseType = "lpstr",
				//		lpstr = new InnerTextElement("Sheet1"),
				//	},
				//},

				_xmlTarget = "docProps/app.xml",
			};

			this.appProp._resId = this.AddRelationship(OpenXMLRelationTypes.docProps_app_____, this.appProp._xmlTarget);

			this.contentType.Overrides.Add(new ContentTypeOverrideItem
			{
				PartName = "/" + this.appProp._xmlTarget,
				ContentType = OpenXMLContentTypes.App___________,
			});
		}

#if DEBUG1
		private void WriteDebugFile<T>(string filename, T obj)
		{
			var testPath = "..\\..\\..\\samples\\" + filename + ".xml";

			using (var fs = new FileStream(testPath, FileMode.Create, FileAccess.Write))
			{
				XMLHelper.SaveXML(fs, obj);
			}
		}
#endif // DEBUG

		private void WriteFile<T>(string path, T obj)
		{
			var stream = new MemoryStream();
			{
				XMLHelper.SaveXML(stream, obj);
				stream.Position = 0;

				IZipEntry ctEntry = this.zipArchive.AddFile(path, stream);

				//var s = ctEntry.CreateStream();
				//{
				//}
			}
		}

		private void WriteOpenXMLFile<T>(T obj) where T : OpenXMLFile
		{
			if (obj._relationFile != null)
			{
				WriteFile(obj._relationFile._xmlTarget, obj._relationFile);
			}

			WriteFile(obj._xmlTarget, obj);
		}

		public void Flush()
		{
			// Content type
			WriteFile(ContentType._xmlTarget, contentType);

			// .rels
			WriteFile(this._relationFile._xmlTarget, this._relationFile);

#region Workbook
			WriteOpenXMLFile(this.Workbook);
#endregion Workbook

#region SharedStrings
			if (this.SharedStrings != null)
			{
				this.SharedStrings.count = this.SharedStrings.items.Count.ToString();
				this.SharedStrings.uniqueCount = this.SharedStrings.items.Count.ToString();

				WriteFile(this.SharedStrings._xmlTarget, this.SharedStrings);
			}
#endregion // SharedStrings

#region Theme1.xml
			//this.Workbook.AddRelationship(OpenXMLRelationTypes.theme____________, "theme/theme1.xml");
			//IZipFileEntry themeEntry = this.zipArchive.AddFile("xl/theme/theme1.xml");
			//using (var s = new StreamWriter(themeEntry.CreateStream()))
			//{
			//	s.Write(unvell.ReoGrid.Properties.Resources.theme1);
			//}
			//this.contentType.Overrides.Add(new ContentTypeOverrideItem
			//{
			//	PartName = "/xl/theme/theme1.xml",
			//	ContentType = OpenXMLContentTypes.Theme_________
			//});
#endregion // Theme1.xml

#region Styles
#if DEBUG1
			WriteDebugFile("styles", this.Stylesheet);
#endif // DEBUG

			WriteFile(this.Stylesheet._xmlTarget, this.Stylesheet);
			//IZipFileEntry ctEntry2 = this.zipArchive.AddFile(this.Stylesheet._xmlTarget);
			//using (var s = ctEntry2.CreateStream())
			//{
			//	using(var sw=new StreamWriter(s))
			//	{
			//		sw.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\r\n<styleSheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\" mc:Ignorable=\"x14ac\" xmlns:x14ac=\"http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac\">\r\n\t<fonts count=\"1\" x14ac:knownFonts=\"1\">\r\n\t\t<font>\r\n\t\t\t<sz val=\"11\"/>\r\n\t\t\t<color theme=\"1\"/>\r\n\t\t\t<name val=\"Calibri\"/>\r\n\t\t\t<family val=\"2\"/>\r\n\t\t\t<scheme val=\"minor\"/>\r\n\t\t</font>\r\n\t</fonts>\r\n\t<fills count=\"2\">\r\n\t\t<fill>\r\n\t\t\t<patternFill patternType=\"none\"/>\r\n\t\t</fill>\r\n\t\t<fill>\r\n\t\t\t<patternFill patternType=\"gray125\"/>\r\n\t\t</fill>\r\n\t</fills>\r\n\t<borders count=\"1\">\r\n\t\t<border>\r\n\t\t\t<left/>\r\n\t\t\t<right/>\r\n\t\t\t<top/>\r\n\t\t\t<bottom/>\r\n\t\t\t<diagonal/>\r\n\t\t</border>\r\n\t</borders>\r\n\t<cellStyleXfs count=\"1\">\r\n\t\t<xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\"/>\r\n\t</cellStyleXfs>\r\n\t<cellXfs count=\"1\">\r\n\t\t<xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\" xfId=\"0\"/>\r\n\t</cellXfs>\r\n\t<cellStyles count=\"1\">\r\n\t\t<cellStyle name=\"Normal\" xfId=\"0\" builtinId=\"0\"/>\r\n\t</cellStyles>\r\n\t<dxfs count=\"0\"/>\r\n\t<tableStyles count=\"0\" defaultTableStyle=\"TableStyleMedium2\" defaultPivotStyle=\"PivotStyleMedium9\"/>\r\n\t<extLst>\r\n\t\t<ext uri=\"{EB79DEF2-80B8-43e5-95BD-54CBDDF9020C}\" xmlns:x14=\"http://schemas.microsoft.com/office/spreadsheetml/2009/9/main\">\r\n\t\t\t<x14:slicerStyles defaultSlicerStyle=\"SlicerStyleLight1\"/>\r\n\t\t</ext>\r\n\t\t<ext uri=\"{9260A510-F301-46a8-8635-F512D64BE5F5}\" xmlns:x15=\"http://schemas.microsoft.com/office/spreadsheetml/2010/11/main\">\r\n\t\t\t<x15:timelineStyles defaultTimelineStyle=\"TimeSlicerStyleLight1\"/>\r\n\t\t</ext>\r\n\t</extLst>\r\n</styleSheet>");
			//	}
			//}
#endregion // Styles

#region Worksheets
			foreach (var sheet in this.Workbook.sheets)
			{
#region Drawings
#if DRAWING
				if (sheet._instance.drawing != null
					&& sheet._instance.drawing._instance != null)
				{
					var drawing = sheet._instance.drawing._instance;
					WriteOpenXMLFile(drawing);

#region Floating Images
					if (drawing._images != null)
					{
						foreach (var blip in drawing._images)
						{
							//streamCache.Position = 0;

							var stream = new MemoryStream(4096);
							//var stream = ctEntry.CreateStream();
							{
								var image = blip._imageObject.Image;
#if WINFORM
								image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
#elif WPF
								var encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
								encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create((System.Windows.Media.Imaging.BitmapSource)image));
								encoder.Save(stream);
#endif // WPF
							}

							stream.Position = 0;

							IZipEntry ctEntry = this.zipArchive.AddFile("xl/media/" + blip._imageFileName, stream);

						}
					}
#endregion // Floating Images
				}
#endif // DRAWING
#endregion // Drawings

#if DEBUG1
				WriteDebugFile("output\\sheet1", sheet._instance);
#endif
				WriteOpenXMLFile(sheet._instance);
			}
#endregion Worksheets

#region CoreProperties
			if (this.coreProp != null)
			{
				WriteFile(this.coreProp._xmlTarget, this.coreProp);
			}
#endregion // CoreProperties

#region AppProperties
			if (this.appProp != null)
			{
				WriteFile(this.appProp._xmlTarget, this.appProp);
			}
#endregion // AppProperties

			this.zipArchive.Flush();
			this.zipArchive.Close();
		}
	}
#endregion // Document
}

namespace unvell.ReoGrid.IO.OpenXML.Schema
{
#region Workbook
	partial class Workbook
	{
		internal Document _doc;

		internal Schema.Worksheet CreateWorksheet(string sheetName)
		{
			if (this.sheets == null)
			{
				this.sheets = new List<WorkbookSheet>();
			}

			int sheetId = this.sheets.Count + 1;

			string sheetFileName = string.Format("sheet{0}.xml", sheetId);
			string rsTarget = "worksheets/" + sheetFileName;

			var sheet = new Schema.Worksheet()
			{
				_xmlTarget = "xl/" + rsTarget,
				_rsTarget = "xl/worksheets/_rels/" + sheetFileName + ".rels",

				rows = new List<Row>(),
			};

			var sheetIndex = new WorkbookSheet
			{
				_instance = sheet,
				sheetId = sheetId.ToString(),
				name = sheetName,
			};

			sheetIndex.resId = this.AddRelationship(OpenXMLRelationTypes.worksheets_sheet_, rsTarget);
			this.sheets.Add(sheetIndex);

			this._doc.contentType.Overrides.Add(new ContentTypeOverrideItem
			{
				PartName = "/" + sheet._xmlTarget,
				ContentType = OpenXMLContentTypes.Worksheet_____,
			});

			return sheet;
		}
	}
#endregion // Workbook

#region Relationships
	partial class OpenXMLFile
	{
		internal string GetAvailableRelationId()
		{
			if (this._relationFile == null
				|| this._relationFile.relations == null
				|| this._relationFile.relations.Count == 0)
				return "rId1";

			int index = this._relationFile.relations.Count + 1;
			string rId = null;

			while (this._relationFile.relations.Any(s => s.id.Equals((rId = "rId" + index), StringComparison.CurrentCultureIgnoreCase)))
			{
				index++;
			}

			return rId;
		}

		internal string AddRelationship(string type, string target)
		{
			if (this._relationFile == null)
			{
				this._relationFile = new Relationships(this._rsTarget);
			}

			if (this._relationFile.relations == null)
			{
				this._relationFile.relations = new List<Relationship>();
			}

			string rid = GetAvailableRelationId();

			this._relationFile.relations.Add(new Relationship { id = rid, type = type, target = target });

			return rid;
		}
	}
#endregion // Relationships
}
