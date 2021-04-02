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

#if EX_SCRIPT

using System;

using unvell.Common;
using unvell.ReoScript;
using unvell.ReoGrid.XML;

using unvell.ReoGrid.Utility;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Main;

namespace unvell.ReoGrid.Script
{
	#region Base Object
	internal class RSWorkbook : ObjectValue
	{
		public IWorkbook workbook { get; set; }

		public IVisualWorkbook ControlInstance { get; set; }

		private RSWorksheetCollection worksheetCollection;

		public RSWorkbook(IWorkbook workbook)
		{
			this.workbook = workbook;

			this["worksheets"] = new ExternalProperty(() =>
			{
				if (worksheetCollection == null)
				{
					worksheetCollection = new RSWorksheetCollection(workbook);
				}

				return worksheetCollection;
			});

			this["currentWorksheet"] = new ExternalProperty(() =>
			{
				if (ControlInstance != null)
				{
					var rsWorksheet = ControlInstance.CurrentWorksheet.worksheetObj;

					if (rsWorksheet == null)
					{
						rsWorksheet = new RSWorksheet(ControlInstance.CurrentWorksheet);
						ControlInstance.CurrentWorksheet.worksheetObj = rsWorksheet;
					}

					return rsWorksheet;
				}
				else
				{
					return null;
				}
			});

			this["createWorksheet"] = new NativeFunctionObject("createWorksheet", (ctx, owner, args) =>
			{
				return new RSWorksheet(workbook.CreateWorksheet(args.Length > 0 ? ScriptRunningMachine.ConvertToString(args[0]) : null));
			});

			this["addWorksheet"] = new NativeFunctionObject("createWorksheet", (ctx, owner, args) =>
			{
				if (args.Length < 1) return null;

				workbook.AddWorksheet(RSWorksheet.Unbox(args[0]));

				return null;
			});

			this["insertWorksheet"] = new NativeFunctionObject("createWorksheet", (ctx, owner, args) =>
			{
				if (args.Length < 2) return null;

				workbook.InsertWorksheet(ScriptRunningMachine.GetIntValue(args[0]), RSWorksheet.Unbox(args[1]));

				return null;
			});

			this["removeWorksheet"] = new NativeFunctionObject("createWorksheet", (ctx, owner, args) =>
			{
				if (args.Length < 1) return null;

				Worksheet sheet = RSWorksheet.Unbox(args[0]);

				if (sheet != null)
				{
					return workbook.RemoveWorksheet(sheet);
				}
				else
				{
					return workbook.RemoveWorksheet(ScriptRunningMachine.GetIntValue(args[0]));
				}
			});

		}
	}

	internal class RSWorksheetCollection : System.Collections.Generic.IEnumerable<RSWorksheet>
	{
		public IWorkbook workbook { get; set; }

		public RSWorksheetCollection(IWorkbook workbook)
		{
			this.workbook = workbook;
		}

		private System.Collections.Generic.IEnumerator<RSWorksheet> GetEnum()
		{
			foreach (var sheet in workbook.Worksheets)
			{
				yield return RSWorksheet.Box(sheet);
			}
		}

		public System.Collections.Generic.IEnumerator<RSWorksheet> GetEnumerator()
		{
			return GetEnum();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnum();
		}
	}

	internal class RSWorksheet : ObjectValue
	{
		private Worksheet sheet;

		private RSSelectionObject selection;

		public RSWorksheet(Worksheet sheet)
		{
			this.sheet = sheet;
			sheet.worksheetObj = this;

			#region Attributes
			this["readonly"] = new ExternalProperty(
				() => { return sheet.HasSettings(WorksheetSettings.Edit_Readonly); },
				(v) => { sheet.SetSettings(WorksheetSettings.Edit_Readonly, (v as bool?) ?? false); });
			#endregion

			#region Selection
			this["selection"] = new ExternalProperty(() =>
			{
				if (this.selection == null)
				{
					this.selection = new RSSelectionObject(sheet);
				}
				return this.selection;
			}, (obj) =>
			{
				sheet.SelectionRange = RSUtility.GetRangeFromValue(sheet, obj);
			});

			this["selectRange"] = new NativeFunctionObject("selectRange", (srm, owner, args) =>
			{
				RangePosition range = RSUtility.GetRangeFromArgs(sheet, args);
				if (range.IsEmpty) return false;

				try
				{
					sheet.SelectRange(range);
					return true;
				}
				catch (Exception)
				{
					return false;
				}
			});

			this["selectionMode"] = new ExternalProperty(() => null);
			#endregion

			#region Rows & Cols
			this["rows"] = new ExternalProperty(() => { return sheet.RowCount; }, (v) => { sheet.SetRows(ScriptRunningMachine.GetIntValue(v)); });
			this["cols"] = new ExternalProperty(() => { return sheet.ColumnCount; }, (v) => { sheet.SetCols(ScriptRunningMachine.GetIntValue(v)); });
			this["getRow"] = new NativeFunctionObject("getRow", (srm, owner, args) =>
			{
				return args.Length == 0 ? null : new RSRowObject(sheet, sheet.GetRowHeader(ScriptRunningMachine.GetIntValue(args[0])));
			});
			this["getCol"] = new NativeFunctionObject("getCol", (srm, owner, args) =>
			{
				return args.Length == 0 ? null : new RSColumnObject(sheet, sheet.GetColumnHeader(ScriptRunningMachine.GetIntValue(args[0])));
			});
			#endregion

			#region Cell & Style
			this["setRangeStyle"] = new NativeFunctionObject("setRangeStyle", (ctx, owner, args) =>
			{
				if (args.Length < 1) return false;

				RangePosition range = RSUtility.GetRangeFromValue(sheet, args[0]);
				WorksheetRangeStyle styleObj = RSUtility.GetRangeStyleObject(args[0]);

				sheet.SetRangeStyles(range, styleObj);

				return styleObj;
			});

			this["getCell"] = new NativeFunctionObject("getCell", (srm, owner, args) =>
			{
				if (args.Length < 1) return null;

				CellPosition pos = RSUtility.GetPosFromValue(sheet, args);

				return new RSCellObject(sheet, pos, sheet.GetCell(pos));
			});

			#endregion

			#region Range
			this["mergeRange"] = new NativeFunctionObject("mergeRange", (srm, owner, args) =>
			{
				RangePosition range = RSUtility.GetRangeFromArgs(sheet, args);
				if (range.IsEmpty) return false;

				try
				{
					sheet.MergeRange(range);
					return true;
				}
				catch (Exception)
				{
					return false;
				}
			});

			this["unmergeRange"] = new NativeFunctionObject("unmergeRange", (srm, owner, args) =>
			{
				RangePosition range = RSUtility.GetRangeFromArgs(sheet, args);
				if (range.IsEmpty) return false;

				try
				{
					sheet.UnmergeRange(range);
					return true;
				}
				catch (Exception)
				{
					return false;
				}
			});
			#endregion

			this["reset"] = new NativeFunctionObject("reset", (ctx, owner, args) =>
			{
				if (args.Length == 2)
				{
					sheet.Reset(ScriptRunningMachine.GetIntParam(args, 0, 1),
						ScriptRunningMachine.GetIntParam(args, 1, 1));
				}
				else
				{
					sheet.Reset();
				}
				return null;
			});

			this["focuspos"] = new ExternalProperty(() => sheet.FocusPos,
				(focuspos) => sheet.FocusPos = RSUtility.GetPosFromValue(sheet, focuspos));

			this["fixPos"] = new NativeFunctionObject("fixPos", (ctx, own, args) =>
				{
					var pos = RSUtility.GetPosFromArgs(this.sheet, args);
					return RSUtility.CreatePosObject(this.sheet.FixPos(pos));
				});

			this["fixRange"] = new NativeFunctionObject("fixRange", (ctx, own, args) =>
				{
					var range = RSUtility.GetRangeFromArgs(this.sheet, args);
					return new RSRangeObject(this.sheet, this.sheet.FixRange(range));
				});

		}

		internal static RSWorksheet Box(object obj)
		{
			if (obj is RSWorksheet)
				return ((RSWorksheet)obj);
			else if (obj is Worksheet)
			{
				var sheet = (Worksheet)obj;

				if (sheet.worksheetObj != null)
					return sheet.worksheetObj;
				else 
					return new RSWorksheet((Worksheet)obj);
			}
			else
				return null;
		}

		internal static Worksheet Unbox(object obj)
		{
			if (obj is RSWorksheet)
				return ((RSWorksheet)obj).sheet;
			else if (obj is Worksheet)
				return (Worksheet)obj;
			else
				return null;
		}
	}
	#endregion // Base Object

	#region Range
	internal class RSRangeObject : RSWorksheet
	{
		public RangePosition Range { get; set; }

		public WorksheetRangeStyle Style { get; set; }

		public RSRangeObject(Worksheet sheet)
			: this(sheet, RangePosition.Empty)
		{
		}

		public RSRangeObject(Worksheet sheet, RangePosition range)
			: base(sheet)
		{
			this.Range = range;

			this["style"] = new ExternalProperty(
				() =>
				{
					if (Style == null)
					{
						Style = sheet.GetRangeStyles(this.Range);
					}
					return Style;
				},
				(v) =>
				{
					sheet.SetRangeStyles(this.Range, RSUtility.GetRangeStyleObject(v));
				}
			);

			this["pos"] = new ExternalProperty(
				() => sheet.SelectionRange.StartPos,
				(v) =>
				{
					range.StartPos = RSUtility.GetPosFromValue(sheet, v);
					sheet.SelectRange(range);
				});

			this["range"] = new ExternalProperty(
				() => sheet.SelectionRange,
				(v) =>
				{
					sheet.SelectRange(RSUtility.GetRangeFromValue(sheet, v));
				});

			this["row"] = new ExternalProperty(() => sheet.SelectionRange.Row,
				(v) => this.Range = new RangePosition(CellUtility.ConvertData<int>(v), this.Range.Col,
						this.Range.Rows, this.Range.Cols));

			this["col"] = new ExternalProperty(() => sheet.SelectionRange.Col,
				(v) => this.Range = new RangePosition(this.Range.Row, CellUtility.ConvertData<int>(v),
						this.Range.Rows, this.Range.Cols));

			this["rows"] = new ExternalProperty(() => sheet.SelectionRange.Row,
				(v) => this.Range = new RangePosition(this.Range.Row, this.Range.Col,
				CellUtility.ConvertData<int>(v), this.Range.Cols));

			this["cols"] = new ExternalProperty(() => sheet.SelectionRange.Col,
				(v) => this.Range = new RangePosition(this.Range.Row, this.Range.Col,
						this.Range.Rows, CellUtility.ConvertData<int>(v)));

			this["toString"] = new NativeFunctionObject("toString", (ctx, owner, args) =>	this.Range.ToString());

			this["toAddress"] = new NativeFunctionObject("toAddress", (ctx, owner, args) => this.Range.ToAddress());

			this["toSpans"] = new NativeFunctionObject("toSpans", (ctx, owner, args) => this.Range.ToStringSpans());

			this["merge"] = new NativeFunctionObject("merge", (ctx, owner, args) => {
				sheet.MergeRange(this.Range); return null;
			});

			this["unmerge"] = new NativeFunctionObject("unmerge", (ctx, owner, args) => {
				sheet.UnmergeRange(this.Range); return null;
			});
		}
	}
	#endregion // Range

	#region Selection
	internal class RSSelectionObject : RSWorksheet
	{
		public RSSelectionObject(Worksheet sheet)
			: base(sheet)
		{
			this["moveUp"] = new NativeFunctionObject("moveUp", (srm, owner, args) =>
			{
				sheet.MoveSelectionUp(); return null;
			});
			this["moveDown"] = new NativeFunctionObject("moveDown", (srm, owner, args) =>
			{
				sheet.MoveSelectionDown(); return null;
			});
			this["moveLeft"] = new NativeFunctionObject("moveLeft", (srm, owner, args) =>
			{
				sheet.MoveSelectionLeft(); return null;
			});
			this["moveRight"] = new NativeFunctionObject("moveRight", (srm, owner, args) =>
			{
				sheet.MoveSelectionRight(); return null;
			});

			this["pos"] = new ExternalProperty(
				() => sheet.SelectionRange.StartPos,
				(v) =>
				{
					RangePosition range = sheet.SelectionRange;
					range.StartPos = RSUtility.GetPosFromValue(sheet, v);
					sheet.SelectRange(range);
				});

			this["range"] = new ExternalProperty(
				() => sheet.SelectionRange,
				(v) =>
				{
					sheet.SelectRange(RSUtility.GetRangeFromValue(sheet, v));
				});

			this["row"] = new ExternalProperty(() => sheet.SelectionRange.Row,
				(v) =>
				{
					RangePosition range = sheet.SelectionRange;
					range.Row = CellUtility.ConvertData<int>(v);
					sheet.SelectRange(range);
				});

			this["col"] = new ExternalProperty(() => sheet.SelectionRange.Col,
				(v) =>
				{
					RangePosition range = sheet.SelectionRange;
					range.Col = CellUtility.ConvertData<int>(v);
					sheet.SelectRange(range);
				});

			this["toString"] = new NativeFunctionObject("toString", (ctx, owner, args) => sheet.SelectionRange.ToString());
			this["toAddress"] = new NativeFunctionObject("toAddress", (ctx, owner, args) => sheet.SelectionRange.ToAddress());
			this["toSpans"] = new NativeFunctionObject("toSpans", (ctx, owner, args) => sheet.SelectionRange.ToStringSpans());

			this["merge"] = new NativeFunctionObject("merge", (ctx, owner, args) =>
			{
				sheet.MergeRange(sheet.SelectionRange); return null;
			});

			this["unmerge"] = new NativeFunctionObject("unmerge", (ctx, owner, args) =>
			{
				sheet.UnmergeRange(sheet.SelectionRange); return null;
			});
		}
	}
	#endregion // Selection

	#region Row & Column & Cell
	internal class RSRowObject : ObjectValue
	{
		private Worksheet sheet;

		public RSCellStyleObject Style { get; set; }

		private RowHeader rowHeader;

		public RSRowObject(Worksheet sheet, RowHeader rowHeader)
		{
			this.sheet = sheet;
			this.rowHeader = rowHeader;

			this["height"] = new ExternalProperty(
				() => rowHeader.Height,
				(v) => rowHeader.Height = (ushort)ScriptRunningMachine.GetIntValue(v));

			this["visible"] = new ExternalProperty(
				() => rowHeader.IsVisible,
				(v) => rowHeader.IsVisible = ScriptRunningMachine.GetBoolValue(v));
		}
	}

	internal class RSColumnObject : ObjectValue
	{
		private Worksheet sheet;

		private ColumnHeader colHeader;

		public RSColumnObject(Worksheet sheet, ColumnHeader colHeader)
		{
			this.sheet = sheet;
			this.colHeader = colHeader;

			this["width"] = new ExternalProperty(
				() => colHeader.Width,
				(v) => colHeader.Width = (ushort)ScriptRunningMachine.GetIntValue(v));

			this["visible"] = new ExternalProperty(
				() => colHeader.IsVisible,
				(v) => colHeader.IsVisible = ScriptRunningMachine.GetBoolValue(v));
		}
	}

	internal class RSCellObject : ObjectValue
	{
		private Worksheet sheet;
		public CellPosition Pos { get; set; }
		public Cell Cell { get; set; }
		public RSCellStyleObject Style { get; set; }

		public RSCellObject(Worksheet sheet, CellPosition pos, Cell cell)
		{
			this.sheet = sheet;
			this.Pos = pos;
			this.Cell = cell;

			this["style"] = new ExternalProperty(() =>
			{
				if (cell == null)
				{
					cell = sheet.CreateAndGetCell(pos);
				}
				if (this.Style == null)
				{
					this.Style = new RSCellStyleObject(sheet, cell);
				}
				return this.Style;
			}, null);

			this["data"] = new ExternalProperty(
				() => cell == null ? null : cell.InnerData,
				(v) =>
				{
					if (cell == null)
						sheet[pos] = v;
					else if (!cell.IsReadOnly)
						sheet[cell] = v;
				});

#if FORMULA
			this["formula"] = new ExternalProperty(
				() => cell == null ? string.Empty : cell.InnerFormula,
				(v) =>
				{
					if (cell == null)
						sheet.SetCellFormula(pos, ScriptRunningMachine.ConvertToString(v));
					else if (!cell.IsReadOnly)
						cell.Formula = ScriptRunningMachine.ConvertToString(v);
				});
#endif // FORMULA

			this["text"] = new ExternalProperty(() => cell == null ? string.Empty : cell.DisplayText);

			this["pos"] = new ExternalProperty(() => cell == null ? Pos : cell.InternalPos);
			this["row"] = new ExternalProperty(() => cell == null ? Pos.Row : cell.InternalRow);
			this["col"] = new ExternalProperty(() => cell == null ? Pos.Col : cell.InternalCol);

			this["address"] = new ExternalProperty(() => cell.Position.ToAddress());
		}

		public override string Name
		{
			get
			{
				return string.Format("cell[{0},{1}]",
					Cell == null ? Pos.Row : Cell.InternalPos.Row,
					Cell == null ? Pos.Col : Cell.InternalPos.Col);
			}
		}
	}
	#endregion // Row & Column & Cell

	#region Cell Style
	internal class RSCellStyleObject : ObjectValue
	{
		private Worksheet sheet;

		public Cell Cell { get; set; }

		public RSCellStyleObject(Worksheet sheet, Cell cell)
		{
			this.sheet = sheet;
			this.Cell = cell;

			this["backgroundColor"] = new ExternalProperty(
				() => TextFormatHelper.EncodeColor(cell.InnerStyle.BackColor),
				(v) =>
				{
					SolidColor color;

					if (TextFormatHelper.DecodeColor(ScriptRunningMachine.ConvertToString(v), out color))
					{
						this.sheet.SetCellStyleOwn(cell.InternalPos, new WorksheetRangeStyle
						{
							Flag = PlainStyleFlag.BackColor,
							BackColor = color,
						});

						this.sheet.RequestInvalidate();
					}
				});

			this["color"] = new ExternalProperty(
				() => TextFormatHelper.EncodeColor(cell.InnerStyle.TextColor),
				(v) =>
				{
					SolidColor color;

					if (TextFormatHelper.DecodeColor(ScriptRunningMachine.ConvertToString(v), out color))
					{
						this.sheet.SetCellStyleOwn(cell.InternalPos, new WorksheetRangeStyle
						{
							Flag = PlainStyleFlag.TextColor,
							TextColor = color,
						});

						this.sheet.RequestInvalidate();
					}
				});

			this["fontName"] = new ExternalProperty(() => cell.Style.FontName,
				(v) => cell.Style.FontName = ScriptRunningMachine.ConvertToString(v));

			this["fontSize"] = new ExternalProperty(
				() => cell.Style.FontSize,
				(v) => cell.Style.FontSize = TextFormatHelper.GetFloatPixelValue(ScriptRunningMachine.ConvertToString(v),
					System.Drawing.SystemFonts.DefaultFont.Size));

			this["align"] = new ExternalProperty(
				() => cell.Style.HAlign,
				(v) =>
				{
					this.sheet.SetCellStyleOwn(cell.InternalPos, new WorksheetRangeStyle
					{
						Flag = PlainStyleFlag.HorizontalAlign,
						HAlign = XmlFileFormatHelper.DecodeHorizontalAlign(ScriptRunningMachine.ConvertToString(v)),
					});

					this.sheet.RequestInvalidate();
				});

			this["valign"] = new ExternalProperty(
				() => cell.Style.VAlign,
				(v) =>
				{
					this.sheet.SetCellStyleOwn(cell.InternalPos, new WorksheetRangeStyle
					{
						Flag = PlainStyleFlag.VerticalAlign,
						VAlign = XmlFileFormatHelper.DecodeVerticalAlign(ScriptRunningMachine.ConvertToString(v)),
					});

					this.sheet.RequestInvalidate();
				});

			this["rotationAngle"] = new ExternalProperty(
				() => cell.Style.VAlign,
				(v) =>
				{
					this.sheet.SetCellStyleOwn(cell.InternalPos, new WorksheetRangeStyle
					{
						Flag = PlainStyleFlag.RotationAngle,
						RotationAngle = ScriptRunningMachine.GetFloatValue(v),
					});

					this.sheet.RequestInvalidate();
				});
		}

		public override string Name
		{
			get { return "CellStyle"; }
		}
	}
	#endregion // Cell Style

	#region Event Args
	internal class RSKeyEvent : ObjectValue
	{
		public int KeyCode { get; set; }

		public RSKeyEvent(int keyCode)
		{
			this.KeyCode = keyCode;

			this["keyCode"] = new ExternalProperty(() => keyCode);
		}
	}
	#endregion // Event Args

	#region Script Utility
	/// <summary>
	/// Utility for ReoGrid script extension
	/// </summary>
	public class RSUtility
	{
		#region Range
		/// <summary>
		/// Get range information from script value
		/// </summary>
		/// <param name="sheet">worksheet instance</param>
		/// <param name="arg">script object to be converted</param>
		/// <returns></returns>
		public static RangePosition GetRangeFromValue(Worksheet sheet, object arg)
		{
			if (arg is RangePosition)
			{
				return (RangePosition)arg;
			}
			else if (arg is string)
			{
				var addr = (string)arg;
				NamedRange namedRange;
				if (RangePosition.IsValidAddress(addr))
				{
					return new RangePosition(addr);
				}
				else if (NamedRange.IsValidName(addr)
					&& sheet.TryGetNamedRange(addr, out namedRange))
				{
					return (RangePosition)namedRange;
				}
				else
					throw new InvalidAddressException(addr);
			}
			else if (arg is ReferenceRange)
			{
				return ((ReferenceRange)arg).Position;
			}
			else if (arg is RSSelectionObject)
			{
				return sheet.SelectionRange;
			}
			else if (arg is RSRangeObject)
			{
				return ((RSRangeObject)arg).Range;
			}

			ObjectValue obj = arg as ObjectValue;
			if (obj == null) return RangePosition.Empty;

			RangePosition range = RangePosition.Empty;

			range.Row = ScriptRunningMachine.GetIntValue(obj["row"]);
			range.Col = ScriptRunningMachine.GetIntValue(obj["col"]);
			range.Rows = ScriptRunningMachine.GetIntValue(obj["rows"]);
			range.Cols = ScriptRunningMachine.GetIntValue(obj["cols"]);

			return range;
		}

		/// <summary>
		/// Get range information from script value
		/// </summary>
		/// <param name="sheet">worksheet instance</param>
		/// <param name="args">script value to be converted</param>
		/// <returns></returns>
		public static RangePosition GetRangeFromArgs(Worksheet sheet, object[] args)
		{
			if (args.Length == 0) return RangePosition.Empty;

			if (args.Length == 1)
			{
				return GetRangeFromValue(sheet, args[0]);
			}
			else
			{
				RangePosition range = RangePosition.Empty;

				range.Row = Convert.ToInt32(args[0]);
				if (args.Length > 1) range.Col = ScriptRunningMachine.GetIntValue(args[1]);
				if (args.Length > 2) range.Rows = ScriptRunningMachine.GetIntValue(args[2]);
				if (args.Length > 3) range.Cols = ScriptRunningMachine.GetIntValue(args[3]);

				return range;
			}
		}
		#endregion

		#region Pos
		public static ObjectValue CreatePosObject(CellPosition pos)
		{
			return CreatePosObject(pos.Row, pos.Col);
		}
		public static ObjectValue CreatePosObject(int row, int col)
		{
			ObjectValue obj = new ObjectValue();
			
			obj["row"] = row;
			obj["col"] = col;

			return obj;
		}

		public static CellPosition GetPosFromValue(Worksheet sheet, object arg)
		{
			CellPosition pos;
			TryGetPosFromValue(sheet, arg, out pos);
			return pos;
		}

		public static bool TryGetPosFromValue(Worksheet sheet, object arg, out CellPosition pos)
		{
			if (arg is object[])
			{
				object[] args = (object[])arg;

				return TryGetPosFromArgs(sheet, args, out pos);
			}
			else if (arg is ObjectValue)
			{
				var obj = (ObjectValue)arg;

				pos = new CellPosition(ScriptRunningMachine.GetIntValue(obj["row"]),
					ScriptRunningMachine.GetIntValue(obj["col"]));
				return true;
			}

			pos = CellPosition.Empty;
			return false;
		}

		public static CellPosition GetPosFromArgs(Worksheet sheet, object[] args)
		{
			CellPosition pos;
			TryGetPosFromArgs(sheet, args, out pos);
			return pos;
		}

		public static bool TryGetPosFromArgs(Worksheet sheet, object[] args, out CellPosition pos)
		{
			if (args.Length == 0)
			{
				pos = CellPosition.Empty;
				return false;
			}

			else if (args.Length == 1)
				{
					if (args[0] is CellPosition)
					{
						pos = (CellPosition)args[0];
						return true;
					}
					else if (args[0] is string || args[0] is System.Text.StringBuilder)
					{
						var addressOrName = Convert.ToString(args[0]);

						if (CellPosition.IsValidAddress(addressOrName))
						{
							pos = new CellPosition(addressOrName);
							return true;
						}

						NamedRange namedRange;
						if (sheet.TryGetNamedRange(addressOrName, out namedRange))
						{
							pos = namedRange.StartPos;
							return true;
						}

						pos = CellPosition.Empty;
						return false;
					}
					else if (args[0] is ObjectValue)
					{
						var obj = (ObjectValue)args[0];

						pos = new CellPosition(ScriptRunningMachine.GetIntValue(obj["row"]),
							ScriptRunningMachine.GetIntValue(obj["col"]));

						return true;
					}
				}
				else //if (arr.Length == 2)
				{
					pos = new CellPosition(ScriptRunningMachine.GetIntValue(args[0], 0),
						ScriptRunningMachine.GetIntValue(args[1], 0));

					return true;
				}
			//ReoGridPos pos = ReoGridPos.Empty;

			//if (args.Length == 1)
			//{
			//	ObjectValue obj = args[0] as ObjectValue;
			//	if (obj == null) return ReoGridPos.Empty;

			//	pos.Row = ScriptRunningMachine.GetIntValue(obj["row"]);
			//	pos.Col = ScriptRunningMachine.GetIntValue(obj["col"]);
			//}
			//else
			//{
			//	pos.Row = Convert.ToInt32(args[0]);
			//	if (args.Length > 1) pos.Col = Convert.ToInt32(args[1]);
			//}

			//return pos;
			pos = CellPosition.Empty;
			return false;
		}
		#endregion

		public static T DecodeSelectionMode<T>(object obj)
		{
			return (T)Enum.Parse(typeof(T), Convert.ToString(obj));
		}

		public static string EncodeSelectionMode(object enm)
		{
			string str = Convert.ToString(enm);
			if (!string.IsNullOrEmpty(str))
			{
				str = char.ToLower(str[0]).ToString() + str.Substring(1);
			}
			return str;
		}

		internal static WorksheetRangeStyle GetRangeStyleObject(object p)
		{
			if (p is WorksheetRangeStyle)
			{
				return (WorksheetRangeStyle)p;
			}
			else if (p is ObjectValue)
			{
				var obj = (ObjectValue)p;

				WorksheetRangeStyle style = new WorksheetRangeStyle();

				SolidColor color;

				object backColor = obj["backgroundColor"];
				if (TextFormatHelper.DecodeColor(ScriptRunningMachine.ConvertToString(backColor), out color))
				{
					style.Flag |= PlainStyleFlag.BackColor;
					style.BackColor = color;
				}
				
				return style;
			}
			else
				return null;
		}
	}
	#endregion // Script Utility
}

#endif // EX_SCRIPT