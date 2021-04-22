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
 * Copyright (c) 2012-2016 Jing <lujing at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

#if WINFORM || ANDROID
using RGFloat = System.Single;
#elif WPF
using RGFloat = System.Double;
#endif // WPF

using RGWorkbook = unvell.ReoGrid.IWorkbook;
using RGWorksheet = unvell.ReoGrid.Worksheet;

using unvell.Common;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.DataFormat;
using unvell.ReoGrid.Utility;
using unvell.ReoGrid.IO.OpenXML.Schema;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Drawing;

namespace unvell.ReoGrid.IO.OpenXML
{
	#region Reader

	internal sealed class ExcelReader
	{
		#region Read Stream
		public static void ReadStream(RGWorkbook rgWorkbook, Stream stream)
		{
#if DEBUG
			Stopwatch sw = Stopwatch.StartNew();
#endif

			var document = Document.ReadFromStream(stream);
			if (document.Workbook.sheets.Count == 0) throw new ReoGridLoadException("Specified Excel file does not contain any valid sheets.");

			rgWorkbook.Worksheets.Clear();

			// create all worksheets
			foreach (var sheet in document.Workbook.sheets)
			{
				var rgSheet = rgWorkbook.CreateWorksheet(sheet.name);
				rgWorkbook.AddWorksheet(rgSheet);
			}

			// load all defined names
			foreach (var definedName in document.Workbook.definedNames)
			{
				if (string.IsNullOrEmpty(definedName.name))
				{
					continue;
				}

				ReadNamedRange(rgWorkbook, document.Workbook, definedName);
			}

			// load all worksheets
			foreach (var sheet in document.Workbook.sheets)
			{
				LoadWorksheet(rgWorkbook, document, sheet);
			}

#if DEBUG
			sw.Stop();
			long ms = sw.ElapsedMilliseconds;
			if (ms > 20)
			{
				Logger.Log("file format", "reading excel format takes " + ms + " ms.");
			}
#endif // DEBUG
		}
		#endregion // Read Stream

		#region Named range

		private static void ReadNamedRangeAddr(RGWorkbook rgBook, string name, string addr, bool worksheetRange)
		{
			int sheetSplit = addr.IndexOf('!');
			string sheetName = null;

			if (sheetSplit >= 0)
			{
				sheetName = addr.Substring(0, sheetSplit);
				addr = addr.Substring(sheetSplit + 1);
			}

			if (!string.IsNullOrEmpty(sheetName))
			{
				if ((sheetName[0] == '\'' && sheetName[0] == '"')
					|| (sheetName[sheetName.Length - 1] == '\'' && sheetName[sheetName.Length - 1] == '"'))
				{
					sheetName = sheetName.Substring(1, sheetName.Length - 2);
				}
			}

			var rgSheet = rgBook.GetWorksheetByName(sheetName);

			if (rgSheet != null)
			{
				if (!RangePosition.IsValidAddress(addr))
				{
					rgBook.NotifyExceptionHappen(rgSheet, new InvalidAddressException(addr));
				}
				else
				{
					// TODO: real workbook scope name
					rgSheet.DefineNamedRange(name, addr,
						worksheetRange ? NamedRangeScope.Worksheet : NamedRangeScope.Workbook);
				}
			}
		}

		private static void ReadNamedRange(RGWorkbook rgBook, Schema.Workbook workbook, Schema.DefinedName defName)
		{
			var name = defName.name;

			int index = name.IndexOf('.');

			if (index == 0)
			{
				return;
			}
			else if (index > 0)
			{
				string space = name.Substring(0, index);
				name = name.Substring(index + 1);

				switch (space)
				{
					case "_xlnm":

						break;
				}
			}
			else if (!string.IsNullOrEmpty(defName.address))
			{
				// TODO: real workbook scope name
				ReadNamedRangeAddr(rgBook, name, defName.address, !string.IsNullOrEmpty(defName.localSheetId));
			}
		}

		#endregion // Named range

		#region Worksheet
		private static void LoadWorksheet(RGWorkbook rgWorkbook, Document doc, WorkbookSheet sheetIndex)
		{
			RGWorksheet rgSheet = rgWorkbook.GetWorksheetByName(sheetIndex.name);

			var sheet = doc.LoadRelationResourceById<Schema.Worksheet>(doc.Workbook, sheetIndex.resId);

			const float fixedCharWidth = 7.0f; //ResourcePoolManager.Instance.GetFont("Arial", 10f, System.Drawing.FontStyle.Regular).SizeInPoints;

			#region SheetView
			var sheetView = sheet.sheetViews.FirstOrDefault() as SheetView;

			if (sheetView != null)
			{
				if (sheetView.showGridLines != null)
				{
					rgSheet.SetSettings(WorksheetSettings.View_ShowGridLine, OpenXMLUtility.IsTrue(sheetView.showGridLines));
				}

				if (sheetView.showRowColHeaders != null)
				{
					rgSheet.SetSettings(WorksheetSettings.View_ShowHeaders, OpenXMLUtility.IsTrue(sheetView.showRowColHeaders));
				}

				if (sheetView.zoomScale != null)
				{
					double zoom = 100;
					if (double.TryParse(sheetView.zoomScale, out zoom))
					{
						rgSheet.ScaleFactor = (float)(zoom / 100f);
					}
				}
			}
			#endregion // SheetView

			double dpi = PlatformUtility.GetDPI();

			ushort defaultRowHeight = rgSheet.defaultRowHeight;

			#region SheetFormatProperty
			if (sheet.sheetFormatProperty != null)
			{
				if (sheet.sheetFormatProperty.defaultRowHeight != null)
				{
					double defRowHeight = 4f;

					if (double.TryParse(sheet.sheetFormatProperty.defaultRowHeight, System.Globalization.NumberStyles.Number,
						ExcelWriter.EnglishCulture, out defRowHeight))
					{
						defaultRowHeight = (ushort)Math.Round(defRowHeight * dpi / 72.0);
						rgSheet.defaultRowHeight = defaultRowHeight;
					}
				}

				if (sheet.sheetFormatProperty.defaultColumnWidth != null)
				{
					if (double.TryParse(sheet.sheetFormatProperty.defaultColumnWidth, System.Globalization.NumberStyles.Number,
						ExcelWriter.EnglishCulture, out var defColumnWidth))
					{
						ushort pixelWidth = (ushort)Math.Truncate(((256 * defColumnWidth + Math.Truncate(128 / fixedCharWidth)) / 256) * fixedCharWidth);

						rgSheet.defaultColumnWidth = pixelWidth;
						rgSheet.SetColumnsWidth(0, rgSheet.ColumnCount, c => rgSheet.defaultColumnWidth, false, false);
					}
				}

			}
			#endregion // SheetFormatProperty

			#region Resize
			// resize to dimension 

			int sheetRowCount = sheet.rows.Count;
			int sheetColCount = sheet.cols.Count;

			if (sheet.dimension != null && !string.IsNullOrEmpty(sheet.dimension.address))
			{
				RangePosition contentRange = new RangePosition(sheet.dimension.address);

				sheetRowCount = Math.Max(sheetRowCount, contentRange.EndRow + 1);
				sheetColCount = Math.Max(sheetColCount, contentRange.EndCol + 1);
			}

			if (rgSheet.RowCount < sheetRowCount)
			{
				rgSheet.Rows = sheetRowCount;
			}

			if (rgSheet.ColumnCount < sheetColCount)
			{
				rgSheet.Columns = sheetColCount;
			}
			#endregion // Resize

			#region Columns
			// columns
			if (sheet.cols != null)
			{
				foreach (Column col in sheet.cols)
				{
					//width = Truncate([{Number of Characters} * {Maximum Digit Width} + {5 pixel padding}] / {Maximum Digit Width} * 256) / 256

					//ushort pixelWidth = (ushort)Math.Truncate(((256 * col.width + Math.Truncate(128 / fixedCharWidth)) / 256) * fixedCharWidth);
					//ushort excelWidth = (ushort)( ( col.width - 12) / 7d + 1);
					//OpenXmlWidth = (Pixels - 12 + 5) / 7d + 1;//From pixels to inches.


					ushort pixelWidth = (ushort)Math.Truncate(((256 * col.width + Math.Truncate(128 / fixedCharWidth)) / 256) * fixedCharWidth);

					int startCol = col.min - 1;
					int count = col.max - col.min + 1;

					if (startCol + count > rgSheet.ColumnCount) count = rgSheet.ColumnCount - startCol;

					rgSheet.SetColumnsWidth(startCol, count, pixelWidth);

					for (int i = startCol; i < startCol + count; i++)
					{
						var header = rgSheet.GetColumnHeader(i);

						header.IsAutoWidth = !OpenXMLUtility.IsTrue(col.customWidth);
					}
				}
			}
			#endregion // Columns

			// stylesheet
			var styles = doc.LoadEntryFile<Stylesheet>(doc.Workbook._path, "styles.xml");
			doc.Stylesheet = styles;

			// fonts
			var fonts = styles.fonts;

			// borders
			var borders = styles.borders;

			// fills
			var fills = styles.fills;

			// cell formats
			var cellFormats = styles.cellFormats;

			// data
			SharedStrings sharedStringTable = doc.ReadSharedStringTable();

			var defaultFont = fonts.list.ElementAtOrDefault(0) as Schema.Font;
			if (defaultFont != null)
			{
				SetStyleFont(doc, rgSheet.RootStyle, defaultFont);
			}

#if DEBUG
			Stopwatch sw = Stopwatch.StartNew();
			Logger.Log("excel format", "begin iterating cells...");

			Stopwatch swBorder = new Stopwatch();
			Stopwatch swStyle = new Stopwatch();
			Stopwatch swValue = new Stopwatch();
			Stopwatch swFormula = new Stopwatch();
			Stopwatch swRowStyle = new Stopwatch();
			Stopwatch swRowHeight = new Stopwatch();
			Stopwatch swDrawing = new Stopwatch();

			int cellCount = 0;
#endif

			Dictionary<int, SharedFormulaInfo> sharedFormulas = new Dictionary<int, SharedFormulaInfo>();

			int rowTop = rgSheet.GetRowHeader(0).Top;
			int lastRowIndex = -1;

			int maxHBorderRow = -1, maxHBorderCol = -1;
			int maxVBorderRow = -1, maxVBorderCol = -1;

			#region Rows

			foreach (Row row in sheet.rows)
			{
				int rowIndex = row.index - 1;

				#region Row Height

#if DEBUG
				swRowHeight.Start();
#endif // DEBUG
				if (rowIndex - lastRowIndex > 1)
				{
					//rgSheet.SetRowsHeight(lastRowIndex + 1, rowIndex - lastRowIndex - 1, defaultRowHeight);
					for (int i = lastRowIndex + 1; i < rowIndex; i++)
					{
						var passedRowHeader = rgSheet.GetRowHeader(i);
						passedRowHeader.Top = rowTop;
						passedRowHeader.InnerHeight = defaultRowHeight;
						rowTop += defaultRowHeight;
					}

					//rowTop += (rowIndex - lastRowIndex - 1) * rgSheet.defaultRowHeight;
					lastRowIndex = rowIndex - 1;

				}

				// row height
				double rowHeight = 0;
				bool isHidden = OpenXMLUtility.IsTrue(row.hidden);

				RowHeader rowHeader;

				if (//row.customHeight == "1"
						//&& 
					!string.IsNullOrEmpty(row.height) && double.TryParse(row.height, out rowHeight))
				{
					rowHeader = rgSheet.GetRowHeader(rowIndex);
					ushort height = (ushort)Math.Round(rowHeight * dpi / 72f);

					rowHeader.Top = rowTop;

					if (isHidden)
					{
						rowHeader.LastHeight = height;
						rowHeader.InnerHeight = 0;
					}
					else
					{
						rowHeader.InnerHeight = height;
						rowTop += height;
					}
				}
				else
				{
					rowHeader = rgSheet.GetRowHeader(rowIndex);

					rowHeader.Top = rowTop;

					if (isHidden)
					{
						rowHeader.LastHeight = defaultRowHeight;
						rowHeader.InnerHeight = 0;
					}
					else
					{
						rowHeader.InnerHeight = defaultRowHeight;
						rowTop += defaultRowHeight;
					}
				}

				rowHeader.IsAutoHeight = !OpenXMLUtility.IsTrue(row.customHeight);

				lastRowIndex = rowIndex;

#if DEBUG
				swRowHeight.Stop();
#endif // DEBUG

				#endregion // Row Height

				#region Row Style
#if DEBUG
				swRowStyle.Start();
#endif // DEBUG

				WorksheetRangeStyle rowStyle = null;

				int styleIndex = -1;

				// row style
				if (!string.IsNullOrEmpty(row.styleIndex) && int.TryParse(row.styleIndex, out styleIndex))
				{
					var style = cellFormats.list.ElementAtOrDefault(styleIndex) as CellFormat;

					if (style != null)
					{
						if (!style._preprocessed)
						{
							PreprocessCellStyle(doc, rgSheet, style, fonts, fills);
						}

						if (style._cachedStyleSet != null)
						{
							rowStyle = style._cachedStyleSet;
							rgSheet.RetrieveRowHeader(row.index - 1).InnerStyle = new WorksheetRangeStyle(rowStyle);
						}

						if (//style.applyBorder == "1" && 
							!string.IsNullOrEmpty(style.borderId))
						{
							if (int.TryParse(style.borderId, out var id)
								&& id >= 0 && id < borders.Count)
							{
								var border = borders[id];

								if (border != null)
								{
									if (!border._preprocessed)
									{
										PreprocessCellBorders(doc, border);
									}

									if (border._hasTop)
									{
										for (int c = 0; c < rgSheet.Columns; c++)
										{
											var hb = rgSheet.GetHBorder(rowIndex, c);
											hb.Pos |= Core.HBorderOwnerPosition.Top;
											hb.Style = border._top;
											hb.Span = 1;
										}

										if (maxHBorderRow < rowIndex) maxHBorderRow = rowIndex;
										maxHBorderCol = rgSheet.Columns;
									}

									if (border._hasBottom)
									{
										for (int c = 0; c < rgSheet.Columns; c++)
										{
											var hb = rgSheet.GetHBorder(rowIndex + 1, c);
											hb.Pos |= Core.HBorderOwnerPosition.Bottom;
											hb.Style = border._bottom;
											hb.Span = 1;
										}

										if (maxHBorderRow < rowIndex + 1) maxHBorderRow = rowIndex + 1;
										maxHBorderCol = rgSheet.Columns;
									}

									if (border._hasLeft)
									{
										for (int c = 0; c < rgSheet.Columns; c++)
										{
											var vb = rgSheet.GetVBorder(rowIndex, c);
											vb.Pos |= Core.VBorderOwnerPosition.Left;
											vb.Style = border._left;
											vb.Span = 1;
										}

										if (maxVBorderRow < rowIndex) maxVBorderRow = rowIndex;
										maxVBorderCol = rgSheet.Columns;
									}

									if (border._hasRight)
									{
										for (int c = 0; c < rgSheet.Columns; c++)
										{
											var vb = rgSheet.GetVBorder(rowIndex, c + 1);
											vb.Pos |= Core.VBorderOwnerPosition.Right;
											vb.Style = border._right;
											vb.Span = 1;
										}

										if (maxVBorderRow < rowIndex) maxVBorderRow = rowIndex;
										maxVBorderCol = rgSheet.Columns + 1;
									}
								}
							}
						}
					}
				}

#if DEBUG
				swRowStyle.Stop();
#endif // DEBUG

				#endregion // Row Style

				foreach (Schema.Cell cell in row.cells)
				{
					CellPosition pos = new CellPosition(cell.address);
					Cell rgCell = rgSheet.cells[pos.Row, pos.Col];

					if (rgCell == null)
					{
						rgCell = rgSheet.CreateCell(pos.Row, pos.Col);
						rgSheet.cells[pos.Row, pos.Col] = rgCell;
					}
					else
					{
						//var rowHeader = rgSheet.rows[pos.Row];
						rgCell.Top = rowHeader.Top;
						rgCell.Height = rowHeader.InnerHeight + 1;
					}

#if DEBUG
					Debug.Assert(rgCell.Height == rgSheet.GetRowHeader(pos.Row).InnerHeight + 1);
#endif // DEBUG

					CellFormat style = null;

					#region Cell Style, Border

					if (!string.IsNullOrEmpty(cell.styleIndex) && int.TryParse(cell.styleIndex, out styleIndex))
					{
#if DEBUG
						swStyle.Start();
#endif // DEBUG
						style = cellFormats.list.ElementAtOrDefault(styleIndex) as CellFormat;
#if DEBUG
						swStyle.Stop();
#endif // DEBUG

						if (style != null)
						{
#if DEBUG
							swStyle.Start();
#endif // DEBUG

							//cell._style = style;

							if (!style._preprocessed)
							{
								PreprocessCellStyle(doc, rgSheet, style, fonts, fills);
							}
#if DEBUG
							swStyle.Stop();
#endif // DEBUG

							if (style._cachedStyleSet != null)
							{
#if DEBUG
								swStyle.Start();
#endif // DEBUG
								if (rowStyle != null && rowStyle != style._cachedStyleSet)
								{
									rgCell.InnerStyle = StyleUtility.CreateMergedStyle(style._cachedStyleSet, rowStyle);
									rgCell.StyleParentKind = Core.StyleParentKind.Own;
									StyleUtility.UpdateCellRenderAlign(rgSheet, rgCell);
								}
								else
								{
									rgCell.InnerStyle = style._cachedStyleSet;
									rgCell.StyleParentKind = Core.StyleParentKind.Range;
									StyleUtility.UpdateCellRenderAlign(rgSheet, rgCell);
								}
#if DEBUG
								swStyle.Stop();
#endif // DEBUG
							}

							#region Border
#if DEBUG
							swBorder.Start();
#endif // DEBUG

							int borderId = 0;

							if (style != null
								&& int.TryParse(style.borderId, out borderId)
								&& borderId < borders.Count
								&& (//style.applyBorder == "1" || 
								borderId > 0))
							{
								var border = borders[borderId];

								if (border != null)
								{
									if (!border._preprocessed)
									{
										PreprocessCellBorders(doc, border);
									}

									if (border._hasTop)
									{
										var hb = rgSheet.GetHBorder(pos.Row, pos.Col);
										hb.Pos |= Core.HBorderOwnerPosition.Top;
										hb.Style = border._top;
										hb.Span = 1;

										if (maxHBorderRow < pos.Row) maxHBorderRow = pos.Row;
										if (maxHBorderCol < pos.Col) maxHBorderCol = pos.Col;
									}

									if (border._hasBottom)
									{
										var hb = rgSheet.GetHBorder(pos.Row + 1, pos.Col);
										hb.Pos |= Core.HBorderOwnerPosition.Bottom;
										hb.Style = border._bottom;
										hb.Span = 1;

										if (maxHBorderRow < pos.Row + 1) maxHBorderRow = pos.Row + 1;
										if (maxHBorderCol < pos.Col) maxHBorderCol = pos.Col;
									}

									if (border._hasLeft)
									{
										var vb = rgSheet.GetVBorder(pos.Row, pos.Col);
										vb.Pos |= Core.VBorderOwnerPosition.Left;
										vb.Style = border._left;
										vb.Span = 1;

										if (maxVBorderRow < pos.Row) maxVBorderRow = pos.Row;
										if (maxVBorderCol < pos.Col) maxVBorderCol = pos.Col;
									}

									if (border._hasRight)
									{
										var vb = rgSheet.GetVBorder(pos.Row, pos.Col + 1);
										vb.Pos |= Core.VBorderOwnerPosition.Right;
										vb.Style = border._right;
										vb.Span = 1;

										if (maxVBorderRow < pos.Row) maxVBorderRow = pos.Row;
										if (maxVBorderCol < pos.Col + 1) maxVBorderCol = pos.Col + 1;
									}
								}
							}
#if DEBUG
							swBorder.Stop();
#endif // DEBUG
							#endregion // Border
						}

					}

					#endregion // Cell Style, Border

					#region Cell Value
#if DEBUG
					swValue.Start();
#endif // DEBUG

					if (cell.value != null && !string.IsNullOrEmpty(cell.value.val))
					{
						if (cell.dataType == "s")
						{
							if (sharedStringTable != null)
							{
								int sharedTextIndex = 0;

								if (int.TryParse(cell.value.val, out sharedTextIndex))
								{
									var ssitem = sharedStringTable.items[sharedTextIndex];

									if (ssitem.text != null)
									{
										rgCell.InnerData = ssitem.text.val;
										rgCell.DataFormat = CellDataFormatFlag.Text;
									}
									else if (ssitem.runs != null)
									{
#if DRAWING
										rgCell.InnerData = CreateRichTextFromRuns(doc, new Paragraph[] { new Paragraph { runs = ssitem.runs } });
#endif // DRAWING
									}
								}
							}
						}
						else if (cell.dataType == "b")
						{
							rgCell.InnerData = OpenXMLUtility.IsTrue(cell.value.val);
						}
						else
						{
							rgCell.InnerData = cell.value.val;
						}
					}

#if DEBUG
					swValue.Stop();
#endif // DEBUG

#endregion // Cell Value

#region Data Format

					if (style != null)
					{
						int numFormatId = 0;

						if (rgCell.DataFormat == CellDataFormatFlag.General
							//&& style.applyNumberFormat == "1" 
							&& !string.IsNullOrEmpty(style.numberFormatId)
							&& int.TryParse(style.numberFormatId, out numFormatId))
						{
							rgCell.DataFormat = SetRGSheetDataFormat(rgSheet, rgCell, numFormatId, styles);
						}
					}

					if (rgCell.DataFormat == CellDataFormatFlag.DateTime)
					{
						rgCell.Data = DataFormatterManager.Instance.DataFormatters[CellDataFormatFlag.DateTime].FormatCell(rgCell);
					}
					else
					{
						DataFormatterManager.Instance.FormatCell(rgCell);
					}

#endregion // Data Format

#region Cell Formula
#if FORMULA

#if DEBUG
					swFormula.Start();
#endif // DEBUG

					var formula = cell.formula;

					if (formula != null)
					{
						string cellFormula = null;

						if (formula.type == "shared")
						{
							if (int.TryParse(formula.sharedIndex, out var sharedIndex))
							{
								if (!string.IsNullOrEmpty(formula.val))
								{
									// parent
									cellFormula = formula.val;

									sharedFormulas[sharedIndex] = new SharedFormulaInfo { pos = pos, formula = cellFormula };
								}
								else
								{
									var sharedFormulaInfo = sharedFormulas[sharedIndex];

									// children
									unvell.ReoGrid.Formula.FormulaRefactor.Reuse(rgSheet, sharedFormulaInfo.pos, new RangePosition(pos));
								}
							}
						}
						else
						{
							cellFormula = formula.val;
						}

						if (cellFormula != null)
						{
							try
							{
								rgSheet.SetCellFormula(rgCell, cellFormula);
							}
							catch { }
						}
					}

#if DEBUG
					swFormula.Stop();
#endif // DEBUG
#endif // FORMULA

#endregion // Cell Formula
				}

#if DEBUG
				cellCount++;
#endif // DEBUG
			}

#endregion // Rows

#if DEBUG
			swRowHeight.Start();
#endif // DEBUG

#region Normalize Row Heights
			//int offset = 0;
			for (int i = lastRowIndex + 1; i < rgSheet.Rows; i++)
			{
				var rowHeader = rgSheet.GetRowHeader(i);
				rowHeader.Top = rowTop;
				rowHeader.InnerHeight = defaultRowHeight;
				rowTop += defaultRowHeight;
			}
#endregion // Normalize Row Heights

#if DEBUG
			swRowHeight.Stop();
#endif

#region Merge
			// merge
			if (sheet.mergeCells != null)
			{
				foreach (MergeCell mcell in sheet.mergeCells)
				{
					rgSheet.MergeRange(new RangePosition(mcell.address), updateUIAndEvent: false);
				}
			}
#endregion // Merge

#region Border

#if DEBUG
			swBorder.Start();
#endif // DEBUG

			// horizontal normalize
			RangeBorderStyle lastBorder = RangeBorderStyle.Empty;
			Core.HBorderOwnerPosition lastHPos = Core.HBorderOwnerPosition.None;
			int borderSpans = 0;

			for (int r = maxHBorderRow; r >= 0; r--)
			{
				lastBorder = RangeBorderStyle.Empty;
				lastHPos = Core.HBorderOwnerPosition.None;
				borderSpans = 1;

				for (int c = maxHBorderCol; c >= 0; c--)
				{
					var hb = rgSheet.RetrieveHBorder(r, c);

					if (hb != null)
					{
						if (hb.Span > 0 && hb.Style.Equals(lastBorder))
						{
							hb.Span += borderSpans;
							hb.Pos |= lastHPos;
							borderSpans++;
						}
						else
						{
							lastBorder = hb.Style;
							lastHPos = hb.Pos;
							borderSpans = 1;
						}
					}
					else
					{
						lastBorder = RangeBorderStyle.Empty;
						lastHPos = Core.HBorderOwnerPosition.None;
						borderSpans = 1;
					}
				}
			}

			lastBorder = RangeBorderStyle.Empty;
			Core.VBorderOwnerPosition lastVPos = Core.VBorderOwnerPosition.None;
			borderSpans = 1;

			// vertical normalize
			for (int c = maxVBorderCol; c >= 0; c--)
			{
				lastBorder = RangeBorderStyle.Empty;
				lastVPos = Core.VBorderOwnerPosition.None;
				borderSpans = 1;

				for (int r = maxVBorderRow; r >= 0; r--)
				{
					var vb = rgSheet.RetrieveVBorder(r, c);

					if (vb != null)
					{
						if (vb.Span > 0 && vb.Style.Equals(lastBorder))
						{
							vb.Span += borderSpans;
							vb.Pos |= lastVPos;
							borderSpans++;
						}
						else
						{
							lastBorder = vb.Style;
							lastVPos = Core.VBorderOwnerPosition.None;
							borderSpans = 1;
						}
					}
					else
					{
						lastBorder = RangeBorderStyle.Empty;
						lastVPos = Core.VBorderOwnerPosition.None;
						borderSpans = 1;
					}
				}
			}

#if DEBUG
			swBorder.Stop();
#endif // DEBUG

#endregion // Border

#region Drawing
#if DRAWING
			if (sheet.drawing != null)
			{
#if DEBUG
				swDrawing.Start();
#endif // DEBUG

				var drawingFile = doc.LoadRelationResourceById<Schema.Drawing>(sheet, sheet.drawing.id);

				if (drawingFile != null)
				{
					LoadDrawingObjects(doc, sheet, rgSheet, drawingFile);
				}

#if DEBUG
				swDrawing.Stop();
#endif // DEBUG
			}
#endif // DRAWING
#endregion // Drawing

#region Print Settings
#if PRINT
			if (sheet.pageMargins != null)
			{
				rgSheet.PrintSettings.Margins = new Print.PageMargins(sheet.pageMargins.top,
					sheet.pageMargins.bottom, sheet.pageMargins.left, sheet.pageMargins.bottom);
			}

			if (sheet.pageSetup != null)
			{
				rgSheet.PrintSettings.Landscape = sheet.pageSetup.orientation == "portrait";
			}

			if (sheet.rowBreaks != null && sheet.rowBreaks.breaks != null)
			{
				foreach (var rb in sheet.rowBreaks.breaks)
				{
					rgSheet.InsertRowPageBreak(rb.id, false);
				}
			}

			if (sheet.colBreaks != null && sheet.colBreaks.breaks != null)
			{
				foreach (var cb in sheet.colBreaks.breaks)
				{
					rgSheet.InsertColumnPageBreak(cb.id, false);
				}
			}
#endif // PRINT
#endregion // Print Settings

#region Freeze
			//#if FREEZE
			if (sheetView != null && sheetView.pane != null)
			{
				int freezeRow = 0, freezeCol = 0;

				if (sheetView.pane.ySplit != null)
				{
					int.TryParse(sheetView.pane.ySplit, out freezeRow);
				}
				if (sheetView.pane.xSplit != null)
				{
					int.TryParse(sheetView.pane.xSplit, out freezeCol);
				}

				if (freezeRow > 0 || freezeCol > 0)
				{
					rgSheet.FreezeToCell(new CellPosition(freezeRow, freezeCol));
				}
			}
			//#endif // FREEZE
#endregion // Freeze

			rgSheet.UpdateViewportController();

#if DEBUG
			sw.Stop();
			Logger.Log("excel format", "end iterating. cells: {0}, {1} ms.", cellCount, sw.ElapsedMilliseconds);

			Debug.WriteLine(string.Format("Row  Height  : {0} ms.", swRowHeight.ElapsedMilliseconds));
			Debug.WriteLine(string.Format("Row  Style   : {0} ms.", swRowStyle.ElapsedMilliseconds));
			Debug.WriteLine(string.Format("Cell Style   : {0} ms.", swStyle.ElapsedMilliseconds));
			Debug.WriteLine(string.Format("Cell Border  : {0} ms.", swBorder.ElapsedMilliseconds));
			Debug.WriteLine(string.Format("Cell Value   : {0} ms.", swValue.ElapsedMilliseconds));
			Debug.WriteLine(string.Format("Cell Formula : {0} ms.", swFormula.ElapsedMilliseconds));
			Debug.WriteLine(string.Format("Drawing      : {0} ms.", swDrawing.ElapsedMilliseconds));

#if WINFORM
			rgSheet._Debug_Validate_All();
#endif // WINFORM
#endif // DEBUG

		}
#endregion // Worksheet

#region Style
		private static void SetStyleFont(Document doc, WorksheetRangeStyle styleset, Schema.Font font)
		{
			SolidColor tempColor = new SolidColor();

			if (font.name != null && !string.IsNullOrEmpty(font.name.value))
			{
				styleset.Flag |= PlainStyleFlag.FontName;
				styleset.FontName = font.name.value;
			}

			float v;
			if (font.size != null && float.TryParse(font.size, System.Globalization.NumberStyles.Float,
				ExcelWriter.EnglishCulture, out v))
			{
				styleset.Flag |= PlainStyleFlag.FontSize;
				styleset.FontSize = v;
			}

			if (font.bold != null)
			{
				styleset.Flag |= PlainStyleFlag.FontStyleBold;
				styleset.Bold = true;
			}

			if (font.italic != null)
			{
				styleset.Flag |= PlainStyleFlag.FontStyleItalic;
				styleset.Italic = true;
			}

			if (font.strikethrough != null)
			{
				styleset.Flag |= PlainStyleFlag.FontStyleStrikethrough;
				styleset.Strikethrough = true;
			}

			if (font.strikethrough != null)
			{
				styleset.Flag |= PlainStyleFlag.FontStyleStrikethrough;
				styleset.Strikethrough = true;
			}

			if (font.underline != null)
			{
				styleset.Flag |= PlainStyleFlag.FontStyleUnderline;
				styleset.Underline = true;
			}

			if (ConvertFromIndexedColor(doc, font.color, ref tempColor))
			{
				styleset.Flag |= PlainStyleFlag.TextColor;
				styleset.TextColor = tempColor;
			}
		}

		private static void PreprocessCellStyle(Document doc, RGWorksheet rgSheet, CellFormat style,
			FontCollection fonts, FillCollection fills)
		{
			WorksheetRangeStyle styleset = new WorksheetRangeStyle(rgSheet.RootStyle);
			SolidColor tempColor = new SolidColor();

#region Font
			if (//style.applyFont == "1" && 
				!string.IsNullOrEmpty(style.fontId))
			{
				var font = fonts.list.ElementAtOrDefault(int.Parse(style.fontId)) as Schema.Font;

				if (font != null)
				{
					SetStyleFont(doc, styleset, font);
				}
			}
#endregion // Font

#region Fill

			if (//OpenXMLUtility.IsTrue(style.applyFill) &&
				!string.IsNullOrEmpty(style.fillId))
			{
				var fill = fills.list.ElementAtOrDefault(int.Parse(style.fillId)) as Fill;

				if (fill != null && fill.patternFill != null && fill.patternFill.patternType != null)
				{
					switch (fill.patternFill.patternType)
					{
						case "solid":
							// solid pattern only use foregroundColor
							if (ConvertFromIndexedColor(doc, fill.patternFill.foregroundColor, ref tempColor))
							{
								styleset.Flag |= PlainStyleFlag.BackColor;
								styleset.BackColor = tempColor;
							}
							break;

						case "darkHorizontal":
						case "darkVertical":
						case "darkDown":
						case "darkUp":
						case "darkGrid":

						case "lightHorizontal":
						case "lightVertical":
						case "lightDown":
						case "lightUp":
						case "lightGrid":
						case "lightTrellis":

						case "darkTrellis":
							SolidColor foreColor = SolidColor.Transparent;
							tempColor = SolidColor.White;
							if (ConvertFromIndexedColor(doc, fill.patternFill.foregroundColor, ref foreColor)
								|| ConvertFromIndexedColor(doc, fill.patternFill.backgroundColor, ref tempColor))
							{
								styleset.Flag |= PlainStyleFlag.FillPattern | PlainStyleFlag.BackColor;
								styleset.FillPatternStyle = HatchStyles.DottedDiamond;
								styleset.FillPatternColor = foreColor;
								styleset.BackColor = tempColor;
							}
							break;
					}
				}
			}
#endregion // Fill

#region Alignment
			if (//style.applyAlignment == "1" && 
				style.alignment != null)
			{
				// vertical alignment
				if (!string.IsNullOrEmpty(style.alignment.horizontal))
				{
					ReoGridHorAlign halign = ReoGridHorAlign.Left;
					switch (style.alignment.horizontal)
					{
						default:
						case "left":
							halign = ReoGridHorAlign.Left;
							break;

						case "center":
							halign = ReoGridHorAlign.Center;
							break;

						case "right":
							halign = ReoGridHorAlign.Right;
							break;
					}

					styleset.Flag |= PlainStyleFlag.HorizontalAlign;
					styleset.HAlign = halign;
				}

				// horizontal alignment
				//
				// Excel does not have default vertical alignment,
				// should always enter this switch
				//if (!string.IsNullOrEmpty(style.alignment.vertical))
				//{
				ReoGridVerAlign valign = ReoGridVerAlign.General;
				switch (style.alignment.vertical)
				{
					// Excel doesn't have general style
					// Default is bottom
					//
					//case "general":
					//	valign = ReoGridVerAlign.General;
					//	break;

					case "top":
						valign = ReoGridVerAlign.Top;
						break;

					case "center":
						valign = ReoGridVerAlign.Middle;
						break;

					default:
					case "bottom":
						valign = ReoGridVerAlign.Bottom;
						break;
				}

				styleset.Flag |= PlainStyleFlag.VerticalAlign;
				styleset.VAlign = valign;

				// text wrap
				if (TextFormatHelper.IsSwitchOn(style.alignment.wrapText))
				{
					styleset.Flag |= PlainStyleFlag.TextWrap;
					styleset.TextWrapMode = TextWrapMode.WordBreak;
				}

				// text rotation
				if (!string.IsNullOrEmpty(style.alignment.textRotation))
				{
					if (int.TryParse(style.alignment.textRotation, out var angle))
					{
						styleset.Flag |= PlainStyleFlag.RotationAngle;

						if (angle > 90) angle = 90 - angle;

						styleset.RotationAngle = angle;
					}
				}

				// indent
				if (!string.IsNullOrEmpty(style.alignment.indent))
				{
					if (ushort.TryParse(style.alignment.indent, out var indent))
					{
						styleset.Indent = indent;
					}
				}
			}
#endregion // Alignment

			if (styleset.Flag != PlainStyleFlag.None)
			{
				style._cachedStyleSet = styleset;
			}

			style._preprocessed = true;
		}
#endregion // Style

#region Color
		private static bool ConvertFromIndexedColor(Document doc, ColorValue color, ref SolidColor rgColor)
		{
			if (color == null) return false;

			int rgbValue;

			if (!string.IsNullOrEmpty(color.rgb)
				&& int.TryParse(color.rgb, System.Globalization.NumberStyles.AllowHexSpecifier, null, out rgbValue))
			{
				rgColor = SolidColor.FromRGB(rgbValue);
				return true;
			}

			int themeIndex;
			double tint;

			if (!string.IsNullOrEmpty(color.theme)
				&& int.TryParse(color.theme, out themeIndex)
				&& themeIndex >= 0)
			{
				var theme = doc.Themesheet;

				if (theme != null)
				{
					if (theme.elements != null
						&& theme.elements.clrScheme != null)
					{
						switch (themeIndex)
						{
							case 0: rgColor = SolidColor.White; break;// doc.ConvertFromCompColor(theme.elements.clrScheme.dk1); break;
							case 1: rgColor = SolidColor.Black; break;// doc.ConvertFromCompColor(theme.elements.clrScheme.lt1); break;
							case 2: rgColor = doc.ConvertFromCompColor(theme.elements.clrScheme.dk2); break;
							case 3: rgColor = doc.ConvertFromCompColor(theme.elements.clrScheme.lt2); break;
							case 4: rgColor = doc.ConvertFromCompColor(theme.elements.clrScheme.accent1); break;
							case 5: rgColor = doc.ConvertFromCompColor(theme.elements.clrScheme.accent2); break;
							case 6: rgColor = doc.ConvertFromCompColor(theme.elements.clrScheme.accent3); break;
							case 7: rgColor = doc.ConvertFromCompColor(theme.elements.clrScheme.accent4); break;
							case 8: rgColor = doc.ConvertFromCompColor(theme.elements.clrScheme.accent5); break;
							case 9: rgColor = doc.ConvertFromCompColor(theme.elements.clrScheme.accent6); break;
							case 10: rgColor = doc.ConvertFromCompColor(theme.elements.clrScheme.hlink); break;
							case 11: rgColor = doc.ConvertFromCompColor(theme.elements.clrScheme.folHlink); break;
							default: rgColor = SolidColor.Black; break;
						}

						if (double.TryParse(color.tint, out tint))
						{
							HSLColor hlsColor = ColorUtility.RGBToHSL(rgColor);
							hlsColor.L = ColorUtility.CalculateFinalLumValue((float)tint, (float)hlsColor.L * 255f) / 255f;
							rgColor = ColorUtility.HSLToRgb(hlsColor);
						}

						return true;
					}
				}
			}

			int colorIndex;

			if (!string.IsNullOrEmpty(color.indexed)
				&& int.TryParse(color.indexed, out colorIndex)
				&& colorIndex >= 0)
			{
				switch (colorIndex)
				{
					case 64: // System Foreground
						rgColor = StaticResources.SystemColor_WindowText;
						return true;

					case 65: // System Background
						rgColor = StaticResources.SystemColor_Window;
						return true;

					default:
						{
							if (doc.Stylesheet.colors != null
								&& doc.Stylesheet.colors.indexedColors != null
								&& colorIndex < doc.Stylesheet.colors.indexedColors.Count
								&& int.TryParse(doc.Stylesheet.colors.indexedColors[colorIndex].rgb,
									System.Globalization.NumberStyles.AllowHexSpecifier, null, out rgbValue))
							{
								rgColor = SolidColor.FromArgb(rgbValue);
								return true;
							}
							else if (colorIndex < IndexedColorTable.colors.Length)
							{
								rgColor = SolidColor.FromRGB(IndexedColorTable.colors[colorIndex]);
								return true;
							}
						}
						break;
				}
			}

			return false;
		}
#endregion // Color

#region Border
		private static void PreprocessCellBorders(Document doc, Border border)
		{
			border._hasTop = ConvertFromExcelBorder(doc, border.top, ref border._top);
			border._hasBottom = ConvertFromExcelBorder(doc, border.bottom, ref border._bottom);
			border._hasLeft = ConvertFromExcelBorder(doc, border.left, ref border._left);
			border._hasRight = ConvertFromExcelBorder(doc, border.right, ref border._right);
			border._preprocessed = true;
		}

		private static void SetRGRangeBorders(Document doc, RGWorksheet rgSheet, RangePosition range, Border border)
		{
			RangeBorderStyle posStyle = new RangeBorderStyle();

			if (ConvertFromExcelBorder(doc, border.top, ref posStyle))
			{
				rgSheet.SetRangeBorders(range, BorderPositions.Top, posStyle, false);
			}

			if (ConvertFromExcelBorder(doc, border.bottom, ref posStyle))
			{
				rgSheet.SetRangeBorders(range, BorderPositions.Bottom, posStyle, false);
			}

			if (ConvertFromExcelBorder(doc, border.left, ref posStyle))
			{
				rgSheet.SetRangeBorders(range, BorderPositions.Left, posStyle, false);
			}

			if (ConvertFromExcelBorder(doc, border.right, ref posStyle))
			{
				rgSheet.SetRangeBorders(range, BorderPositions.Right, posStyle, false);
			}
		}

		private static bool ConvertFromExcelBorder(Document doc, SideBorder sideBorder, ref RangeBorderStyle rgStyle)
		{
			if (sideBorder != null && !string.IsNullOrEmpty(sideBorder.style))
			{
				SolidColor color = new SolidColor();

				rgStyle.Style = ConvertFromExcelBorderStyle(sideBorder.style);

				if (sideBorder.color == null)
				{
					rgStyle.Color = SolidColor.Black;
					return true;
				}
				else if (ConvertFromIndexedColor(doc, sideBorder.color, ref color))
				{
					rgStyle.Color = color;
					return true;
				}
				else if (OpenXMLUtility.IsTrue(sideBorder.color.auto))
				{
					rgStyle.Color = SolidColor.Black;
					return true;
				}
			}

			return false;
		}

		private static BorderLineStyle ConvertFromExcelBorderStyle(string ebStyle)
		{
			switch (ebStyle)
			{
				default:
				case "thin":
					return BorderLineStyle.Solid;
				case "medium":
					return BorderLineStyle.BoldSolid;
				case "thick":
					return BorderLineStyle.BoldSolidStrong;

				case "hair":
					return BorderLineStyle.Dotted;

				case "dashed":
					return BorderLineStyle.Dashed;
				case "dotted":
					return BorderLineStyle.Dashed2;
				case "double":
					return BorderLineStyle.DoubleLine;

				case "mediumDashed":
					return BorderLineStyle.BoldDashed;
				case "mediumDashDot":
					return BorderLineStyle.BoldDashDot;
				case "mediumDashDotDot":
					return BorderLineStyle.BoldDashDotDot;

				case "dashDot":
					return BorderLineStyle.DashDot;
				case "dashDotDot":
					return BorderLineStyle.DashDotDot;

				case "slantDashDot": // not supported in ReoGrid yet
					return BorderLineStyle.BoldDashDotDot;

			}
		}
#endregion

#region Data Format

		//private static Regex numberFormatRegex = new Regex("0*\\.{[0]+}");

		private static Regex currencyFormatRegex = new Regex(@"([^\\\s]*)\\?(\s*)\[\$([^(\-|\])]+)-?[^\]]*\]\\?(\s*)([^\\\s]*)", RegexOptions.Compiled);

		private static NumberDataFormatter.INumberFormatArgs ReadNumberFormatArgs(string pattern, NumberDataFormatter.INumberFormatArgs arg)
		{
			if (pattern.StartsWith("[Red]", StringComparison.CurrentCultureIgnoreCase))
			{
				// add red style
				arg.NegativeStyle |= NumberDataFormatter.NumberNegativeStyle.Red;
				// remove minus symbol
				arg.NegativeStyle &= ~NumberDataFormatter.NumberNegativeStyle.Minus;

				pattern = pattern.Substring(5);
			}

			if (pattern.StartsWith("\""))
			{
				int index = pattern.IndexOf('"', 1);

				string prefix = pattern.Substring(1, index - 1);

				if (prefix == "▲ ")
				{
					// add sankaku symbol
					arg.NegativeStyle |= NumberDataFormatter.NumberNegativeStyle.Prefix_Sankaku;

					// remove minus symbol
					arg.NegativeStyle &= ~NumberDataFormatter.NumberNegativeStyle.Minus;
				}
				else
				{
					arg.CustomNegativePrefix = prefix;
				}

				pattern = pattern.Substring(index + 1);
			}

			if (pattern.StartsWith("\\(") && pattern.EndsWith("\\)"))
			{
				// add bracket style
				arg.NegativeStyle |= NumberDataFormatter.NumberNegativeStyle.Brackets;

				// remove minus symbol
				arg.NegativeStyle &= ~NumberDataFormatter.NumberNegativeStyle.Minus;

				pattern = pattern.Substring(2, pattern.Length - 4);
			}

			var culture = System.Threading.Thread.CurrentThread.CurrentCulture;

			int len = pattern.Length;

			int decimalSeparatorIndex = pattern.LastIndexOf(culture.NumberFormat.NumberDecimalSeparator, len - 1);

			if (decimalSeparatorIndex >= 0 && decimalSeparatorIndex < len - 1)
			{
				arg.DecimalPlaces = (short)(len - 1 - decimalSeparatorIndex);
			}
			else
			{
				arg.DecimalPlaces = 0;
			}

			arg.UseSeparator = (pattern.IndexOf(culture.NumberFormat.NumberGroupSeparator) > 0);

			return arg;
		}

		enum NumberFormatParseStatus
		{
			Segment,
			InString,
		}

		enum NumberFormatSegmentDefines
		{
			Positive = 0,
			Negative = 1,
			Zero = 2,
			Text = 3,
		}

		private static CellDataFormatFlag SetRGSheetDataFormat(RGWorksheet rgSheet, Cell cell, int formatId, Stylesheet styles)
		{
			CellDataFormatFlag flag = CellDataFormatFlag.General;

			if (BuiltinNumberFormats.SetFromExcelBuiltinFormat(rgSheet, cell, formatId, out flag))
			{
				return flag;
			}

			if (styles.numberFormats != null)
			{
				var numFormat = styles.numberFormats.list.FirstOrDefault(nf => nf.formatId == formatId) as NumberFormat;

				if (numFormat != null)
				{
					string[] patterns = numFormat.formatCode.Split(';');

					if (patterns != null && patterns.Length > 0)
					{
						object arg = null;
						Match currencyMatch = null;

						var pattern = patterns[0];

						if (pattern.StartsWith("\"$\""))
						{
							flag = CellDataFormatFlag.Currency;

							var carg = (CurrencyDataFormatter.CurrencyFormatArgs)ReadNumberFormatArgs(
								pattern.Substring(3), new CurrencyDataFormatter.CurrencyFormatArgs());

							carg.PrefixSymbol = "$";

							arg = carg;
						}
						// #,##0.00 [$-419E] #,##0.00
						else if ((currencyMatch = currencyFormatRegex.Match(pattern)).Success)
						{
							#region Currency
							flag = CellDataFormatFlag.Currency;
							var carg = new CurrencyDataFormatter.CurrencyFormatArgs();

							if (currencyMatch.Groups[1].Length > 0)
							{
								if (currencyMatch.Groups[3].Success)
								{
									carg.PostfixSymbol = currencyMatch.Groups[3].Value;
								}

								if (currencyMatch.Groups[2].Length > 0)
								{
									carg.PostfixSymbol = currencyMatch.Groups[2].Value + carg.PostfixSymbol;
								}

								carg = (CurrencyDataFormatter.CurrencyFormatArgs)ReadNumberFormatArgs(currencyMatch.Groups[1].Value, carg);
							}
							else if (currencyMatch.Groups[5].Length > 0)
							{
								if (currencyMatch.Groups[3].Success)
								{
									carg.PrefixSymbol = currencyMatch.Groups[3].Value;
								}

								if (currencyMatch.Groups[4].Length > 0)
								{
									carg.PrefixSymbol += currencyMatch.Groups[4].Value;
								}

								carg = (CurrencyDataFormatter.CurrencyFormatArgs)ReadNumberFormatArgs(currencyMatch.Groups[5].Value, carg);
							}

							arg = carg;
							#endregion // Currency
						}
						else if (pattern.EndsWith("%"))
						{
							#region Percent
							flag = CellDataFormatFlag.Percent;

							pattern = pattern.Substring(0, pattern.Length - 1);

							arg = ReadNumberFormatArgs(pattern, new NumberDataFormatter.NumberFormatArgs());
							#endregion // Percent
						}
						else if (pattern.Any(c => c == 'm' || c == 'h' || c == 's' || c == 'y' || c == 'd'))
						{
							pattern = pattern.Replace("yyyy/mm", "yyyy/MM").Replace("mm/yy", "MM/yy")
											 .Replace("mm/d", "MM/d").Replace("m/d", "M/d")
											 .Replace("d/mm", "d/MM").Replace("d/m", "d/M")
											 .Replace("aaa", "ddd");

							flag = CellDataFormatFlag.DateTime;

							arg = new DateTimeDataFormatter.DateTimeFormatArgs
							{
								Format = pattern,
							};
						}
						else
						{
							flag = CellDataFormatFlag.Number;
							arg = ReadNumberFormatArgs(patterns.Length > 1 ? patterns[1] : patterns[0], new NumberDataFormatter.NumberFormatArgs());
						}

						if (flag != CellDataFormatFlag.General)
						{
							//rgSheet.SetCellDataFormat(cell, flag, ref arg, );
							cell.DataFormat = flag;
							cell.DataFormatArgs = arg;
						}
					}
				}
			}

			return flag;
		}

#endregion

#region Drawing
#if DRAWING
		private static void LoadDrawingObjects(Document doc, Schema.Worksheet sheet, RGWorksheet rgSheet, Schema.Drawing drawingFile)
		{
			foreach (var archor in drawingFile.twoCellAnchors)
			{
				DrawingObject obj = null;

				if (archor.pic != null)
				{
					obj = LoadImage(doc, rgSheet, archor.pic, drawingFile);
				}
				else
				if (archor.shape != null)
				{
					obj = LoadShape(doc, rgSheet, archor.shape);
				}
				else if (archor.cxnShape != null)
				{
					obj = LoadShape(doc, rgSheet, archor.cxnShape);
				}
				else if (archor.graphcFrame != null)
				{
					obj = LoadGraphic(doc, rgSheet, drawingFile, archor.graphcFrame);
				}

				if (obj != null)
				{
					obj.Bounds = GetDrawingBounds(rgSheet, archor);

					rgSheet.FloatingObjects.Add(obj);
				}
			}
		}

		private static void SetDrawingObjectStyle(Document doc, DrawingObject obj, Schema.Shape shape)
		{
			var theme = doc.Themesheet;

			if (theme == null || theme.elements == null
				|| theme.elements.fmtScheme == null)
			{
				return;
			}

			var style = shape.style;
			var prop = shape.prop;

			// line color
			bool overrideFill = false;
			bool overrideLineWeight = false;
			bool overrideLineStyle = false;
			bool overrideLineColor = false;

			if (prop != null)
			{
#region Line
				if (prop.line != null)
				{
					if (prop.line.solidFill != null)
					{
						obj.LineColor = doc.ConvertFromCompColor(prop.line.solidFill);
						overrideLineColor = true;
					}
					else if (prop.line.noFill != null)
					{
						obj.LineColor = SolidColor.Transparent;
						overrideLineColor = true;
					}

					if (prop.line.weight != null)
					{
						if (int.TryParse(prop.line.weight, out var weight))
						{
							obj.LineWidth = MeasureToolkit.EMUToPixel(weight, PlatformUtility.GetDPI());
							overrideLineWeight = true;
						}
					}

					if (prop.line.prstDash != null)
					{
						Graphics.LineStyles lineStyle;

						if (ConvertFromDashStyle(prop.line.prstDash.value, out lineStyle))
						{
							obj.LineStyle = lineStyle;
							overrideLineStyle = true;
						}
					}
				}
#endregion // Line

				if (prop.solidFill != null)
				{
					obj.FillColor = doc.ConvertFromCompColor(prop.solidFill, prop.solidFill);
					overrideFill = true;
				}
				else if (prop.noFill != null)
				{
					obj.FillColor = SolidColor.Transparent;
					overrideFill = true;
				}
			}

#region Style
			if (style != null)
			{
				var lnRef = style.lnRef;

				if (lnRef != null && !string.IsNullOrEmpty(lnRef.idx))
				{
					int index = -1;
					int.TryParse(lnRef.idx, out index);

					if (theme.elements.fmtScheme != null
						&& theme.elements.fmtScheme.lineStyles != null
						&& index > 0 && index <= theme.elements.fmtScheme.lineStyles.Count)
					{
						var refLineStyle = theme.elements.fmtScheme.lineStyles[index - 1];

						if (!overrideLineColor && refLineStyle.solidFill != null && refLineStyle.solidFill.schemeColor != null)
						{
							obj.LineColor = doc.ConvertFromCompColor(refLineStyle.solidFill, style.lnRef);
						}

						if (!overrideLineWeight)
						{
							obj.LineWidth = MeasureToolkit.EMUToPixel(refLineStyle.weight, PlatformUtility.GetDPI());
						}

						if (!overrideLineStyle && refLineStyle.prstDash != null)
						{
							Graphics.LineStyles lineStyle;
							ConvertFromDashStyle(refLineStyle.prstDash.value, out lineStyle);
							obj.LineStyle = lineStyle;
						}
					}
				}

				if (!overrideFill && style.fillRef != null && !string.IsNullOrEmpty(style.fillRef.idx))
				{
					int index = -1;
					int.TryParse(style.fillRef.idx, out index);

					if (theme.elements.fmtScheme != null
						&& theme.elements.fmtScheme.fillStyles != null
						&& index > 0 && index <= theme.elements.fmtScheme.fillStyles.Count)
					{
						var fillStyle = theme.elements.fmtScheme.fillStyles[index - 1];

						if (fillStyle is CompColor)
						{
							obj.FillColor = doc.ConvertFromCompColor((CompColor)fillStyle, style.fillRef);
						}
						else if (fillStyle is GradientFill)
						{
							var gf = (GradientFill)fillStyle;

							if (gf.gsLst.Count > 0)
							{
								var gs = gf.gsLst[gf.gsLst.Count / 2];
								obj.FillColor = doc.ConvertFromCompColor(gs, style.fillRef);
							}
						}
					}
				}

				if (style.fontRef != null)
				{
				}
			}
#endregion Style

			if (prop != null && prop.transform != null)
			{
				if (OpenXMLUtility.IsTrue(prop.transform.flipV))
				{
					if (obj is Drawing.Shapes.ShapeObject)
					{
						obj.ScaleY = -1;
					}
				}
			}
		}

		private static bool ConvertFromDashStyle(string val, out Graphics.LineStyles lineStyle)
		{
			switch (val)
			{
				case "dash":
					lineStyle = Graphics.LineStyles.Dash;
					return true;

				default:
					lineStyle = Graphics.LineStyles.Solid;
					return false;
			}
		}

#region Image
		private static ImageObject LoadImage(Document doc, RGWorksheet rgSheet, Pic pic, Schema.Drawing drawingFile)
		{
			var blipFill = pic.blipFill;

			if (blipFill != null)
			{
				var blip = pic.blipFill.blip;
				if (blip != null && !string.IsNullOrEmpty(blip.embedId) && drawingFile._relationFile != null)
				{
					var relation = drawingFile._relationFile.relations.FirstOrDefault(r => r.id == blip.embedId);
					var finalPath = RelativePathUtility.GetRelativePath(drawingFile._path, relation.target);
					var stream = doc.GetResourceStream(finalPath);

					if (stream != null)
					{

#if WINFORM
						System.Drawing.Image image = null;

						try
						{
							image = System.Drawing.Image.FromStream(stream, false);

							// note: cannot print when use 'using'
							//using (var fs = new MemoryStream(40960))
							var fs = new MemoryStream(40960);
							{
								image.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
								image.Dispose();

								fs.Position = 0;
								image = System.Drawing.Image.FromStream(fs);
							}
						}
						catch { }

						stream.Dispose();

						if (image != null)
						{
							return new ImageObject(image);
						}

#elif WPF
						try
						{
							System.Windows.Media.Imaging.BitmapImage biImg = new System.Windows.Media.Imaging.BitmapImage();

							biImg.BeginInit();
							biImg.StreamSource = stream;
							biImg.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
							biImg.EndInit();

							//stream.Dispose();

							System.Windows.Media.ImageSource image = biImg as System.Windows.Media.ImageSource;

							return image == null ? null : new ImageObject(image);
						}
						catch { }

#endif // WINFORM

					}
				}
			}

			return null;
		}
#endregion // Image

#region LoadShape
		private static Drawing.DrawingObject LoadShape(Document doc, RGWorksheet rgSheet, Schema.Shape shape)
		{
			DrawingObject obj = null;

			var prop = shape.prop;

			// create shape
			if (prop != null && prop.prstGeom != null)
			{
				switch (prop.prstGeom.presetType)
				{
					case "rect": obj = new Drawing.Shapes.RectangleShape(); break;

					case "roundRect":
						Drawing.Shapes.RoundedRectangleShape roundRect = new Drawing.Shapes.RoundedRectangleShape();

						if (shape.prop.prstGeom.avList != null)
						{
							var adj = shape.prop.prstGeom.avList.FirstOrDefault(gd => gd.name == "adj");

							if (adj != null)
							{
								if (adj.formula.StartsWith("val "))
								{
									int rate = -1;
									int.TryParse(adj.formula.Substring(4), out rate);
									if (rate >= 0)
									{
										roundRect.RoundRate = (float)rate / 50000f;
									}
								}
							}
						}

						obj = roundRect;

						break;

					case "diamond": obj = new Drawing.Shapes.DiamondShape(); break;

					case "ellipse": obj = new Drawing.Shapes.EllipseShape(); break;

					case "line":
					case "straightConnector1":
						obj = LoadLine(prop);
						break;
				}
			}

			// set style
			if (obj != null)
			{
				SetDrawingObjectStyle(doc, obj, shape);
			}

			if (obj is Drawing.Shapes.ShapeObject)
			{
				// text
				if (shape.textBody != null)
				{
					if (shape.textBody.paragraphs != null)
					{
						var rgShape = (Drawing.Shapes.ShapeObject)obj;

						rgShape.RichText = CreateRichTextFromRuns(doc, shape.textBody.paragraphs);
					var sb = new StringBuilder();
					RGFloat fontSize = 11.0f;

					foreach (var p in shape.textBody.paragraphs)
					{
						if (p.runs != null)
						{
							foreach (var r in p.runs)
							{
								if (r.text != null
									&& !string.IsNullOrEmpty(r.text.innerText))
								{
									var runPr = r.property;

									if (runPr != null)
									{
										int size = 0;
										if (int.TryParse(runPr.sizeAttr, out size))
										{
											fontSize = (float)size / 133f;
										}
									}

									sb.Append(r.text.innerText);
								}
							}

							sb.Append(Environment.NewLine);
						}
					}

					rgShape.Text = sb.ToString();
					rgShape.FontSize = fontSize;

						if (shape.textBody.bodyProperty != null)
						{
							if (shape.textBody.bodyProperty.anchor != null)
							{
								switch (shape.textBody.bodyProperty.anchor)
								{
									case "t":
										if (rgShape.RichText != null)
										{
											rgShape.RichText.VerticalAlignment = ReoGridVerAlign.Top;
										}
										break;

									default:
										if (rgShape.RichText != null)
										{
											rgShape.RichText.VerticalAlignment = ReoGridVerAlign.Middle;
										}
										break;

									case "b":
										if (rgShape.RichText != null)
										{
											rgShape.RichText.VerticalAlignment = ReoGridVerAlign.Bottom;
										}
										break;
								}
							}
						}
					}
				}
			}

			return obj;
		}

		private static Drawing.Shapes.Line LoadLine(ShapeProperty prop)
		{
			Drawing.Shapes.Line line = new Drawing.Shapes.Line();

			var xfrm = prop.transform;

			if (xfrm != null)
			{
				RGFloat dpi = PlatformUtility.GetDPI();

				Rectangle bounds = new Rectangle(MeasureToolkit.EMUToPixel(xfrm.offset.x, dpi), MeasureToolkit.EMUToPixel(xfrm.offset.y, dpi),
					MeasureToolkit.EMUToPixel(xfrm.extents.cx, dpi), MeasureToolkit.EMUToPixel(xfrm.extents.cy, dpi));

				RGFloat startX, startY, endX, endY;

				if (OpenXMLUtility.IsTrue(xfrm.flipV))
				{
					startY = bounds.Bottom;
					endY = bounds.Top;
				}
				else
				{
					startY = bounds.Top;
					endY = bounds.Bottom;
				}

				if (OpenXMLUtility.IsTrue(xfrm.flipH))
				{
					startX = bounds.Right;
					endX = bounds.Left;
				}
				else
				{
					startX = bounds.Left;
					endX = bounds.Right;
				}

				line.StartPoint = new Point(startX, startY);
				line.EndPoint = new Point(endX, endY);
			}

			if (prop.line != null)
			{
				if (prop.line.headEnd != null
					&& !string.IsNullOrEmpty(prop.line.headEnd.type))
				{
					switch (prop.line.headEnd.type)
					{
						case "triangle":
							line.StartCap = LineCapStyles.Arrow;
							break;
					}
				}

				if (prop.line.tailEnd != null
					&& !string.IsNullOrEmpty(prop.line.tailEnd.type))
				{
					switch (prop.line.tailEnd.type)
					{
						case "triangle":
							line.EndCap = LineCapStyles.Arrow;
							break;
					}
				}
			}

			return line;
		}

#endregion // LoadShape

#region GetDrawingBounds
		private static Rectangle GetDrawingBounds(RGWorksheet rgSheet, TwoCellAnchor archor)
		{
			if (archor.to.row >= rgSheet.RowCount)
			{
				rgSheet.AppendRows(archor.to.row - rgSheet.RowCount + 1);
			}

			if (archor.to.col >= rgSheet.ColumnCount)
			{
				rgSheet.AppendColumns(archor.to.col - rgSheet.ColumnCount + 1);
			}

			RGFloat dpi = PlatformUtility.GetDPI();

			RGFloat sxoff = MeasureToolkit.EMUToPixel(archor.from.colOff, dpi);
			RGFloat syoff = MeasureToolkit.EMUToPixel(archor.from.rowOff, dpi);

			Debug.Assert(sxoff >= 0 && syoff >= 0);

			Point start = rgSheet.GetCellPhysicsPosition(archor.from.row, archor.from.col);
			start.X += sxoff;
			start.Y += syoff;

			RGFloat exoff = MeasureToolkit.EMUToPixel(archor.to.colOff, dpi);
			RGFloat eyoff = MeasureToolkit.EMUToPixel(archor.to.rowOff, dpi);

			Debug.Assert(exoff >= 0 && eyoff >= 0);

			Point end = rgSheet.GetCellPhysicsPosition(archor.to.row, archor.to.col);
			end.X += exoff;
			end.Y += eyoff;

			return new Rectangle(start, end);
		}
#endregion // GetDrawingBounds

#region LoadGraphic
		private static Drawing.DrawingObject LoadGraphic(Document doc, RGWorksheet rgSheet, Schema.Drawing drawingFile, Schema.GraphicFrame graphicFrame)
		{
			var graphic = graphicFrame.graphic;

			if (graphic != null && graphic.data != null)
			{
				switch (graphic.data.uri)
				{
					case OpenXMLNamespaces.Chart________:
						if (graphic.data.chart != null
							&& !string.IsNullOrEmpty(graphic.data.chart.id))
						{
							var xmlChart = doc.LoadRelationResourceById<Schema.ChartSpace>(drawingFile, graphic.data.chart.id);
							if (xmlChart != null)
							{
								return LoadChart(rgSheet, xmlChart);
							}
						}
						break;
				}
			}

			return null;
		}
#endregion // LoadGraphic

#region Chart
		private static Chart.Chart LoadChart(RGWorksheet rgSheet, Schema.ChartSpace chartSpace)
		{
			if (chartSpace.chart == null) return null;

			var chart = chartSpace.chart;

			var plot = chart.plotArea;
			if (plot == null) return null;

			Chart.Chart rgChart = null;
			Chart.WorksheetChartDataSource dataSource = new Chart.WorksheetChartDataSource(rgSheet);

			if (plot.lineChart != null)
			{
#region Line Chart Plot Area

				if (plot.lineChart.serials != null)
				{
					foreach (var ser in plot.lineChart.serials)
					{
						ReadDataSerial(dataSource, rgSheet, ser);
					}
				}

				rgChart = new Chart.LineChart()
				{
					DataSource = dataSource,
				};
#endregion // Line Chart Plot Area
			}
			else if (plot.barChart != null)
			{
#region Column/Bar Chart Plot Area
				if (plot.barChart.serials != null)
				{
					foreach (var ser in plot.barChart.serials)
					{
						ReadDataSerial(dataSource, rgSheet, ser);
					}
				}

				if (plot.barChart.barDir != null
					&& plot.barChart.barDir.value == "col")
				{
					rgChart = new Chart.ColumnChart();
				}
				else
				{
					rgChart = new Chart.BarChart();
				}

				rgChart.DataSource = dataSource;
#endregion // Column Chart Plot Area
			}
			else if (plot.pieChart != null)
			{
#region Pie Chart Plot Area
				if (plot.pieChart.serials != null)
				{
					foreach (var ser in plot.pieChart.serials)
					{
						ReadDataSerial(dataSource, rgSheet, ser);
					}
				}

				rgChart = new Chart.PieChart()
				{
					DataSource = dataSource,
				};
#endregion // Pie Chart Plot Area
			}
			else if (plot.doughnutChart != null)
			{
#region Doughnut Chart Plot Area
				if (plot.doughnutChart.serials != null)
				{
					foreach (var ser in plot.doughnutChart.serials)
					{
						ReadDataSerial(dataSource, rgSheet, ser);
					}
				}

				rgChart = new Chart.DoughnutChart()
				{
					DataSource = dataSource,
				};
#endregion // Pie Chart Plot Area	
			}
			else if (plot.areaChart != null)
			{
#region Area Chart Plot Area
				if (plot.areaChart.serials != null)
				{
					foreach (var ser in plot.areaChart.serials)
					{
						ReadDataSerial(dataSource, rgSheet, ser);
					}
				}

				rgChart = new Chart.AreaChart()
				{
					DataSource = dataSource,
				};
#endregion // Area Chart Plot Area
			}

			bool showLegend = false;

			if (chart.legend != null)
			{
				if (chart.legend.legendPos != null)
				{
					showLegend = true;
				}
			}

			rgChart.ShowLegend = showLegend;

			return rgChart;
		}

		private static Chart.WorksheetChartDataSerial ReadDataSerial(Chart.WorksheetChartDataSource dataSource,
			RGWorksheet rgSheet, IChartSerial serial)
		{
			if (serial == null) return null;

#if FORMULA
			CellPosition labelAddress = CellPosition.Empty;

			var label = serial.ChartLabel;

			if (label != null
				&& label.strRef != null)
			{
				if (label.strRef.formula != null
					&& !string.IsNullOrEmpty(label.strRef.formula))
				{
					var serialNameVal = Formula.Evaluator.Evaluate(rgSheet.workbook, label.strRef.formula);

					if (serialNameVal.type == Formula.FormulaValueType.Cell)
					{
						labelAddress = (CellPosition)serialNameVal.value;
					}
				}

				//if (label.strRef.strCache != null
				//	&& label.strRef.strCache.ptList != null
				//	&& label.strRef.strCache.ptList.Count > 0)
				//{
				//	var pt = label.strRef.strCache.ptList[0];

				//	if (pt.value != null)
				//	{
				//		serialName = pt.value.val;
				//	}
				//}
			}

			var values = serial.Values;

			if (values.numRef != null
				&& values.numRef.formula != null
				&& !string.IsNullOrEmpty(values.numRef.formula))
			{
				var dataRangeVal = Formula.Evaluator.Evaluate(rgSheet.workbook, values.numRef.formula);

				if (dataRangeVal.type == Formula.FormulaValueType.Range)
				{
					var range = (RangePosition)dataRangeVal.value;

					if (serial is PieChartSerial)
					{
						// transfer to multiple serials
						for (int r = range.Row; r <= range.EndRow; r++)
						{
							dataSource.AddSerial(rgSheet, labelAddress, new RangePosition(r, range.Col, 1, 1));
						}
					}
					else
					{
						dataSource.AddSerial(rgSheet, labelAddress, range);
					}
				}
			}

#endif // FORMULA

			return null;
		}
#endregion // Chart
#endif // DRAWING
#endregion // Drawing

#region RichText
#if DRAWING
		private static RichText CreateRichTextFromRuns(Document doc, IEnumerable<Paragraph> paragraphs)
		{
			var rt = new RichText();

			foreach (var p in paragraphs)
			{
				foreach (var run in p.runs)
				{
					AddRunIntoRichText(doc, rt, run);
				}

				if (p.property != null)
				{
					if (p.property.align != null)
					{
						switch (p.property.align)
						{
							case "ctr":
								rt.SetStyles(halign: ReoGridHorAlign.Center);
								break;
						}
					}
				}

				rt.NewLine();
			}

			return rt;
		}

		private static void AddRunIntoRichText(Document doc, RichText rt, Run r)
		{
			if (string.IsNullOrEmpty(r.text.innerText))
			{
				// FIXME: need support to read single white space XML text
				//        https://github.com/unvell/ReoGrid/issues/29
				return;
			}

			string fontName = null;
			RGFloat fontSize = 8.5f;
			SolidColor foreColor = SolidColor.Black;
			SolidColor backColor = SolidColor.Transparent;
			Drawing.Text.FontStyles fontStyles = Drawing.Text.FontStyles.Regular;

#region Run Property
			var rpr = r.property;

			if (rpr != null)
			{
				fontName = rpr.font;

				if (rpr.size != null)
				{
					RGFloat.TryParse(rpr.size, out fontSize);
				}
				else if (rpr.sizeAttr != null)
				{
					int intFontSize = 11;
					int.TryParse(rpr.sizeAttr, out intFontSize);
					fontSize = intFontSize / 100f;
				}
#if DEBUG
				else
				{
					Debug.Assert(false); // not found font size
				}
#endif // DEBUG

				if (rpr.color != null)
				{
					ConvertFromIndexedColor(doc, rpr.color, ref foreColor);
				}
				else if (rpr.solidFill != null)
				{
					foreColor = doc.ConvertFromCompColor(rpr.solidFill);
				}

				if (rpr.strike != null)
				{
					fontStyles |= Drawing.Text.FontStyles.Strikethrough;
				}

				if (rpr.b != null)
				{
					fontStyles |= Drawing.Text.FontStyles.Bold;
				}

				if (rpr.i != null)
				{
					fontStyles |= Drawing.Text.FontStyles.Italic;
				}

				if (rpr.u != null)
				{
					fontStyles |= Drawing.Text.FontStyles.Underline;
				}

				if (rpr.vertAlign != null)
				{
					if (rpr.vertAlign.value == "superscript")
					{
						fontStyles |= Drawing.Text.FontStyles.Superscrit;
					}
					else if (rpr.vertAlign.value == "subscript")
					{
						fontStyles |= Drawing.Text.FontStyles.Subscript;
					}
				}
			}
#endregion // Run Property

			int nlIndex = r.text.innerText.IndexOf('\n');

			if (nlIndex > -1)
			{
				rt.AddText(r.text.innerText.Substring(0, nlIndex), fontName, fontSize, fontStyles, foreColor, backColor);
				rt.NewLine();
				rt.AddText(r.text.innerText.Substring(nlIndex + 1), fontName, fontSize, fontStyles, foreColor, backColor);
			}
			else
			{
				rt.AddText(r.text.innerText, fontName, fontSize, fontStyles, foreColor, backColor);
			}

		}
#endif // DRAWING
#endregion // RichText

	}

	class SharedFormulaInfo
	{
		internal CellPosition pos;
		internal string formula;
	}
#endregion // Reader

#region IndexedColors
	sealed class IndexedColorTable
	{
#region Colors
		public static readonly int[] colors = new int[] {
													0x000000, // 0
													0xFFFFFF, // 1
													0xFF0000, // 2
													0x00FF00, // 3
													0x0000FF, // 4
													0xFFFF00, // 5
													0xFF00FF, // 6
													0x00FFFF, // 7

													0x000000, // 8
													0xFFFFFF,
													0xFF0000, // 10
													0x00FF00,
													0x0000FF, // 12
													0xFFFF00,
													0xFF00FF, // 14
													0x00FFFF,
													0x800000, // 16
													0x008000,
													0x000080, // 18
													0x808000,
													0x800080, // 20
													0x008080,
													0xC0C0C0, // 22
													0x808080,
													0x9999FF, // 24
													0x993366,
													0xFFFFCC, // 26
													0xCCFFFF,
													0x660066, // 28
													0xFF8080,
													0x0066CC, // 30
													0xCCCCFF,

													0x000080, // 32
													0xFF00FF,
													0xFFFF00,
													0x00FFFF,
													0x800080, // 36
													0x800000,
													0x008080,
													0x0000FF,
													0x00CCFF, // 40
													0xCCFFFF,
													0xCCFFCC,
													0xFFFF99,
													0x99CCFF, // 44
													0xFF99CC,
													0xCC99FF,
													0xFFCC99, // 47
													0x3366FF,
													0x33CCCC, // 49

													0x99CC00, // 50
													0xFFCC00,
													0xFF9900,
													0xFF6600,
													0x666699, // 54
													0x969696,
													0x003366,
													0x339966, // 57
													0x003300,
													0x333300,
													0x993300, // 60
													0x993366,
													0x333399,
													0x333333,
													//0x0,				// 64: System Foreground
													//0x0,				// 65: System Background
													};
#endregion // Colors
	}
#endregion // IndexedColors

#region Builtin Number Formats
	sealed class BuiltinNumberFormats
	{
		public static bool SetFromExcelBuiltinFormat(RGWorksheet rgSheet, Cell cell, int formatId, out CellDataFormatFlag dataFormatFlag)
		{
			CellDataFormatFlag? format = null;
			object arg = null;

			switch (formatId)
			{
				case 0: // General
					format = CellDataFormatFlag.General;
					break;

				case 1: // 0
					format = CellDataFormatFlag.Number;
					arg = new NumberDataFormatter.NumberFormatArgs { DecimalPlaces = 0, UseSeparator = false };
					break;

				case 2:  // 0.00
				case 11: // 0.00E+00
					format = CellDataFormatFlag.Number;
					arg = new NumberDataFormatter.NumberFormatArgs { DecimalPlaces = 2, UseSeparator = false };
					break;

				case 3: // #,##0
					format = CellDataFormatFlag.Number;
					arg = new NumberDataFormatter.NumberFormatArgs { DecimalPlaces = 0, UseSeparator = true };
					break;

				case 4: // #,##0.00
					format = CellDataFormatFlag.Number;
					arg = new NumberDataFormatter.NumberFormatArgs { DecimalPlaces = 2, UseSeparator = true };
					break;

				case 9: // 0%
					format = CellDataFormatFlag.Percent;
					arg = new NumberDataFormatter.NumberFormatArgs { DecimalPlaces = 0, UseSeparator = false };
					break;

				case 10: // 0.00%
					format = CellDataFormatFlag.Percent;
					arg = new NumberDataFormatter.NumberFormatArgs { DecimalPlaces = 2, UseSeparator = false };
					break;

				case 14:
					// openxml spec: mm-dd-yy 
					// Excel implementation: m/d/yyyy 
					format = CellDataFormatFlag.DateTime;
					arg = new DateTimeDataFormatter.DateTimeFormatArgs
					{
						CultureName = "en-US",
						Format = "M/d/yyyy",
					};
					break;

				case 15: // d-mmm-yy
					format = CellDataFormatFlag.DateTime;
					arg = new DateTimeDataFormatter.DateTimeFormatArgs
					{
						CultureName = "en-US",
						Format = "d-MMM-yy",
					};
					break;

				case 16: // d-mmm
					format = CellDataFormatFlag.DateTime;
					arg = new DateTimeDataFormatter.DateTimeFormatArgs
					{
						CultureName = "en-US",
						Format = "d-MMM",
					};
					break;

				case 17: // mmm-yy
					format = CellDataFormatFlag.DateTime;
					arg = new DateTimeDataFormatter.DateTimeFormatArgs
					{
						CultureName = "en-US",
						Format = "MMM-yy",
					};
					break;

				case 18: // h:mm AM/PM
					format = CellDataFormatFlag.DateTime;
					arg = new DateTimeDataFormatter.DateTimeFormatArgs
					{
						CultureName = "en-US",
						Format = "h:mm tt",
					};
					break;

				case 19: // h:mm:ss AM/PM
					format = CellDataFormatFlag.DateTime;
					arg = new DateTimeDataFormatter.DateTimeFormatArgs
					{
						CultureName = "en-US",
						Format = "h:mm:ss tt",
					};
					break;

				case 20: // h:mm
					format = CellDataFormatFlag.DateTime;
					arg = new DateTimeDataFormatter.DateTimeFormatArgs
					{
						CultureName = "en-US",
						Format = "h:mm",
					};
					break;

				case 21: // h:mm:ss
					format = CellDataFormatFlag.DateTime;
					arg = new DateTimeDataFormatter.DateTimeFormatArgs
					{
						CultureName = "en-US",
						Format = "H:mm:ss",
					};
					break;

				case 22: // m/d/yy h:mm
					format = CellDataFormatFlag.DateTime;
					arg = new DateTimeDataFormatter.DateTimeFormatArgs
					{
						CultureName = "en-US",
						Format = "M/d/yy h:mm",
					};
					break;

				case 37: // #,##0 ;(#,##0)
					format = CellDataFormatFlag.Number;
					arg = new NumberDataFormatter.NumberFormatArgs
					{
						DecimalPlaces = 0,
						UseSeparator = true,
						NegativeStyle = NumberDataFormatter.NumberNegativeStyle.Brackets,
					};
					break;

				case 38: // #,##0 ;[Red](#,##0)
					format = CellDataFormatFlag.Number;
					arg = new NumberDataFormatter.NumberFormatArgs
					{
						DecimalPlaces = 0,
						UseSeparator = true,
						NegativeStyle = NumberDataFormatter.NumberNegativeStyle.Red | NumberDataFormatter.NumberNegativeStyle.Brackets,
					};
					break;

				case 39: // #,##0.00;(#,##0.00)
					format = CellDataFormatFlag.Number;
					arg = new NumberDataFormatter.NumberFormatArgs
					{
						DecimalPlaces = 2,
						UseSeparator = true,
						NegativeStyle = NumberDataFormatter.NumberNegativeStyle.Brackets,
					};
					break;

				case 40: // #,##0.00;[Red](#,##0.00)
					format = CellDataFormatFlag.Number;
					arg = new NumberDataFormatter.NumberFormatArgs
					{
						DecimalPlaces = 2,
						UseSeparator = true,
						NegativeStyle = NumberDataFormatter.NumberNegativeStyle.Red | NumberDataFormatter.NumberNegativeStyle.Brackets,
					};
					break;

				case 45: // mm:ss
					format = CellDataFormatFlag.DateTime;
					arg = new DateTimeDataFormatter.DateTimeFormatArgs
					{
						CultureName = "en-US",
						Format = "mm:ss",
					};
					break;

				case 46: // [h]:mm:ss
					format = CellDataFormatFlag.DateTime;
					arg = new DateTimeDataFormatter.DateTimeFormatArgs
					{
						CultureName = "en-US",
						Format = "h:mm:ss",
					};
					break;

				case 47: // mmss.0
					format = CellDataFormatFlag.DateTime;
					arg = new DateTimeDataFormatter.DateTimeFormatArgs
					{
						CultureName = "en-US",
						Format = "mmss.f",
					};
					break;

				case 49: // @
					format = CellDataFormatFlag.Text;
					break;

				case 12: // # ?/?
				case 13: // # ??/??

				case 48: // ##0.0E+0
								 //throw new NotSupportedException();
					break;
			}

			if (format == null)
			{
				dataFormatFlag = CellDataFormatFlag.General;
				return false;
			}
			else
			{
				cell.DataFormat = format.Value;
				cell.DataFormatArgs = arg;
				dataFormatFlag = format.Value;
				//rgSheet.SetCellDataFormat(cell, format.Value, ref arg);
				return true;
			}
		}

	}
#endregion // Builtin Number Formats

#region Excel Document
	internal partial class Document
	{
		private IZipArchive zipArchive;

		public Schema.Workbook Workbook { get; private set; }

		public bool IsReadonly { get; private set; }

		private Document()
		{
			IsReadonly = false;
		}

		public static Document ReadFromStream(Stream stream)
		{
			IZipArchive zip = MZipArchiveFactory.OpenOnStream(stream);

			if (zip == null) return null;

			var doc = new Document { zipArchive = zip, _path = string.Empty };

			doc.LoadRelationShipsFile(doc, string.Empty);

			doc.Workbook = doc.LoadEntryFile<Schema.Workbook>("xl/", "workbook.xml");

			return doc;
		}

#region Load Resources

		internal T LoadEntryFile<T>(string path, string name) where T : class
		{
			T obj = this.LoadObjectFromPath<T>(path, name);
			if (obj == null) throw new ExcelFormatException("Failed to load specified entry resource: " + name);

			return obj;
		}

		private void LoadRelationShipsFile(OpenXMLFile entryFile, string name)
		{
			string finalPath = entryFile._path + "_rels/" + name + ".rels";

			if (this.zipArchive.IsFileExist(finalPath))
			{
				entryFile._relationFile = this.LoadObjectFromPath<Relationships>(finalPath, null);
			}
		}

		internal T LoadRelationResourceById<T>(OpenXMLFile entryFile, string id) where T : class
		{
			var relation = entryFile._relationFile.relations.FirstOrDefault(_r => _r.id == id);

			if (relation == null)
			{
				throw new ExcelFormatException("Relation resource cannot be found: " + entryFile._path);
			}

			return this.LoadObjectFromPath<T>(entryFile._path, relation.target);
		}

		internal T LoadRelationResourceByType<T>(OpenXMLFile entryFile, string typeNamespace) where T : class
		{
			var relation = this.Workbook._relationFile.relations.FirstOrDefault(r => r.type == typeNamespace);
			return relation == null ? null : this.LoadObjectFromPath<T>(entryFile._path, relation.target);
		}

		internal T LoadObjectFromPath<T>(string path, string name) where T : class
		{
			var finalPath = RelativePathUtility.GetRelativePath(path, name);

			var entry = this.zipArchive.GetFile(finalPath);

			if (entry == null)
			{
				throw new ExcelFormatException("Resource entry cannot be found: " + path);
			}

			using (var stream = entry.GetStream())
			{
				if (stream == null)
				{
					throw new ExcelFormatException("Resource stream cannot be found: " + path);
				}

				T obj = XMLHelper.LoadXML<T>(stream) as T;

				if (obj is OpenXMLFile)
				{
					var objectPath = RelativePathUtility.GetPathWithoutFilename(finalPath);
					var objectName = RelativePathUtility.GetFileNameFromPath(finalPath);

					var entryFile = obj as OpenXMLFile;
					entryFile._path = objectPath;
					this.LoadRelationShipsFile(entryFile, objectName);
				}

				return obj;
			}
		}

		internal Stream GetResourceStream(string path)
		{
			var entry = this.zipArchive.GetFile(path);
			return entry == null ? null : entry.GetStream();
		}

#endregion // Load Resources

#region SharedStrings

		public SharedStrings SharedStrings { get; set; }

		private const string sharedStrings_xml_filename = "sharedStrings.xml";

		public SharedStrings ReadSharedStringTable()
		{
			if (this.zipArchive.IsFileExist(this.Workbook._path + sharedStrings_xml_filename))
			{
				return this.LoadEntryFile<SharedStrings>(this.Workbook._path, sharedStrings_xml_filename);
			}
			else
			{
				return null;
			}
		}

#endregion // SharedStrings

		public Stylesheet Stylesheet { get; set; }

#region Themesheet
		private Theme themesheet;

		public Theme Themesheet
		{
			get
			{
				if (this.themesheet == null && this.zipArchive != null)
				{
					this.themesheet = this.LoadRelationResourceByType<Theme>(this.Workbook, OpenXMLRelationTypes.theme____________);
				}

				return this.themesheet;
			}
			set
			{
				this.themesheet = value;
			}
		}

#region Convert CompColor
		internal SolidColor ConvertFromCompColor(CompColor compColor, CompColor overrideColor = null)
		{
			if (compColor._solidColor.A > 0)
			{
				return compColor._solidColor;
			}

			if (compColor.srgbColor != null
				&& !string.IsNullOrEmpty(compColor.srgbColor.val))
			{
				int hex = 0;
				int.TryParse(compColor.srgbColor.val, System.Globalization.NumberStyles.AllowHexSpecifier, null, out hex);
				compColor._solidColor = SolidColor.FromRGB(hex);
				return compColor._solidColor;
			}

			if (compColor.sysColor != null
				&& !string.IsNullOrEmpty(compColor.sysColor.val))
			{
				switch (compColor.sysColor.val)
				{
					case "windowText":
						compColor._solidColor = StaticResources.SystemColor_WindowText;
						return compColor._solidColor;

					case "window":
						compColor._solidColor = StaticResources.SystemColor_Window;
						return compColor._solidColor;
				}
			}

			if (compColor.schemeColor != null)
			{
				SolidColor color = SolidColor.Black;

				var theme = this.Themesheet;

				switch (compColor.schemeColor.val)
				{
					case "dk1": color = ConvertFromCompColor(theme.elements.clrScheme.dk1); break;
					case "lt1": color = ConvertFromCompColor(theme.elements.clrScheme.lt1); break;
					case "dk2": color = ConvertFromCompColor(theme.elements.clrScheme.dk2); break;
					case "lt2": color = ConvertFromCompColor(theme.elements.clrScheme.lt2); break;
					case "accent1": color = ConvertFromCompColor(theme.elements.clrScheme.accent1); break;
					case "accent2": color = ConvertFromCompColor(theme.elements.clrScheme.accent2); break;
					case "accent3": color = ConvertFromCompColor(theme.elements.clrScheme.accent3); break;
					case "accent4": color = ConvertFromCompColor(theme.elements.clrScheme.accent4); break;
					case "accent5": color = ConvertFromCompColor(theme.elements.clrScheme.accent5); break;
					case "accent6": color = ConvertFromCompColor(theme.elements.clrScheme.accent6); break;
					case "hlink": color = ConvertFromCompColor(theme.elements.clrScheme.hlink); break;
					case "folHlink": color = ConvertFromCompColor(theme.elements.clrScheme.folHlink); break;
					case "phClr": color = ConvertFromCompColor(overrideColor); break;
				}

				if (overrideColor != null)
				{
					HSLColor hlsColor = ColorUtility.RGBToHSL(color);

					var compColorVal = overrideColor.schemeColor;

					if (compColorVal.shade != null)
					{
						hlsColor.L = ColorUtility.CalculateFinalLumValue(-(float)compColorVal.shade.value / 100000f, (float)hlsColor.L * 255f) / 255f;
					}

					if (compColorVal.tint != null)
					{
						hlsColor.L = ColorUtility.CalculateFinalLumValue((float)compColorVal.tint.value / 100000f, (float)hlsColor.L * 255f) / 255f;
					}

					if (compColorVal.lumMod != null)
					{
						hlsColor.L *= (float)compColorVal.lumMod.value / 100000f;
					}

					if (compColorVal.lumOff != null)
					{
						hlsColor.L += (float)compColorVal.lumOff.value / 100000f;
					}

					if (compColorVal.satMod != null)
					{
						hlsColor.S *= (float)compColorVal.satMod.value / 100000f;
					}

					color = ColorUtility.HSLToRgb(hlsColor);
				}
				else
				{
					compColor._solidColor = color;
				}

				return color;
			}

			return SolidColor.Transparent;
		}
#endregion // Convert CompColor

#endregion // Themesheet
	}
#endregion // Excel Document

#region Exceptions

	class ExcelFormatException : ReoGridException
	{
		public ExcelFormatException(string msg) : base(msg) { }
		public ExcelFormatException(string msg, Exception inner) : base(msg, inner) { }
	}

#endregion // Exceptions
}
