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

#if FORMULA

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using unvell.ReoGrid.Utility;

namespace unvell.ReoGrid.Formula
{
	class Evaluator
	{
		internal static IFunctionNameProvider functionNameProvider = new StandardFunctionNameProvider();

		#region Evaluate
		public static FormulaValue Evaluate(Cell cell)
		{
			if (cell == null) return null;
			return Evaluate(cell.Worksheet == null ? null : cell.Worksheet.workbook, cell, cell.FormulaTree);
		}

		public static FormulaValue Evaluate(IWorkbook workbook, string input)
		{
			return Evaluate(workbook, null, Parser.Parse(workbook, null, input));
		}

		public static FormulaValue Evaluate(Cell cell, STNode node)
		{
			return Evaluate(cell.Worksheet == null ? null : cell.Worksheet.workbook, cell, node);
		}

		private static FormulaValue Evaluate(IWorkbook workbook, Cell cell, STNode node)
		{
			if (node == null)
			{
				return FormulaValue.Nil;
			}

			switch (node.Type)
			{
				#region Parse Nodes
				case STNodeType.CELL:
					#region CELL
					{
						var cellNode = (STCellNode)node;

						var sheet = cellNode.Worksheet;
						if (sheet == null && cell != null) sheet = cell.Worksheet;

						var pos = cellNode.Position;
						var targetCell = sheet.GetCell(pos);

						if (targetCell == null)
						{
							return FormulaValue.Nil;
						}

						if (targetCell.formulaStatus != FormulaStatus.Normal)
						{
							switch (targetCell.formulaStatus)
							{
								case FormulaStatus.InvalidValue:
									throw new FormulaTypeMismatchException(targetCell);
								case FormulaStatus.NameNotFound:
									throw new FormulaNoNameException(targetCell);
								case FormulaStatus.MismatchedParameter:
									throw new FormulaParameterMismatchException(targetCell);
								default:
									throw new FormulaEvalutionException(targetCell, "formula error happens in " + targetCell.Address);
							}
						}

						return CreateFormulaValue(targetCell);
					}
				#endregion // CELL

				case STNodeType.NUMBER:
					return ((STNumberNode)node).Value;

				case STNodeType.RANGE:
					return ((STRangeNode)node).Range;

				case STNodeType.STRING:
					return ((STStringNode)node).Text;

				case STNodeType.TRUE:
					return true;

				case STNodeType.FALSE:
					return false;

				case STNodeType.IDENTIFIER:
					#region Identifier
					{
						string name = ((STIdentifierNode)node).Identifier;

						if (cell.Worksheet.TryGetNamedRange(name, out var range))
						{
							if (range.Position.IsSingleCell)
							{
								return CreateFormulaValue(cell.Worksheet.GetCellData(range.StartPos));
							}
							else
							{
								return range.Position;
							}
						}
						else if (FormulaExtension.NameReferenceProvider != null)
						{
							return CreateFormulaValue(FormulaExtension.NameReferenceProvider(cell, name));
						}
						else
						{
							throw new FormulaNoNameException(cell);
							//return FormulaValue._NoName;
						}
					}
				#endregion Identifier

				#region ADD, SUB, MUL, DIV
				case STNodeType.ADD:
					#region Add
					{
						if (node.Children.Count < 2) return FormulaValue.Nil;

						FormulaValue v1 = CheckAndGetDefaultValue(cell, Evaluate(workbook, cell, node[0]));
						FormulaValue v2 = CheckAndGetDefaultValue(cell, Evaluate(workbook, cell, node[1]));

						if (v1.type != FormulaValueType.Number || v2.type != FormulaValueType.Number)
						{
							throw new FormulaTypeMismatchException(cell);
							//return FormulaValue.Nil;
						}

						return (double)v1.value + (double)v2.value;
					}
				#endregion // Add

				case STNodeType.SUB:
					#region Sub
					{
						if (node.Children.Count < 2) return FormulaValue.Nil;

						FormulaValue v1 = CheckAndGetDefaultValue(cell, Evaluate(cell, node[0]));
						FormulaValue v2 = CheckAndGetDefaultValue(cell, Evaluate(cell, node[1]));

						if (v1.type != FormulaValueType.Number || v2.type != FormulaValueType.Number)
						{
							throw new FormulaTypeMismatchException(cell);
							//return FormulaValue.Nil;
						}

						return (double)v1.value - (double)v2.value;
					}
				#endregion // Sub

				case STNodeType.MUL:
					#region Mul
					{
						if (node.Children.Count < 2) return FormulaValue.Nil;

						FormulaValue v1 = CheckAndGetDefaultValue(cell, Evaluate(workbook, cell, node[0]));
						FormulaValue v2 = CheckAndGetDefaultValue(cell, Evaluate(workbook, cell, node[1]));

						if (v1.type != FormulaValueType.Number || v2.type != FormulaValueType.Number)
						{
							throw new FormulaTypeMismatchException(cell);
							//return FormulaValue.Nil;
						}

						return (double)v1.value * (double)v2.value;
					}
				#endregion // Mul

				case STNodeType.DIV:
					#region Div
					{
						if (node.Children.Count < 2) return FormulaValue.Nil;

						FormulaValue v1 = CheckAndGetDefaultValue(cell, Evaluate(workbook, cell, node[0]));
						FormulaValue v2 = CheckAndGetDefaultValue(cell, Evaluate(workbook, cell, node[1]));

						if (v1.type != FormulaValueType.Number || v2.type != FormulaValueType.Number)
						{
							throw new FormulaTypeMismatchException(cell);
							//return FormulaValue.Nil;
						}

						return (double)v1.value / (double)v2.value;
					}
				#endregion // Div
				#endregion // ADD, SUB, MUL, DIV

				case STNodeType.CONNECT:
					#region CONNECT
					{
						if (node.Children.Count < 2) return FormulaValue.Nil;

						FormulaValue v1 = Evaluate(workbook, cell, node[0]);
						FormulaValue v2 = Evaluate(workbook, cell, node[1]);

						return Convert.ToString(v1.value) + Convert.ToString(v2.value);
					}
				#endregion // CONNECT

				case STNodeType.POW:
					#region POW
					{
						if (node.Children.Count < 2) return FormulaValue.Nil;

						FormulaValue v1 = CheckAndGetDefaultValue(cell, Evaluate(workbook, cell, node[0]));
						FormulaValue v2 = CheckAndGetDefaultValue(cell, Evaluate(workbook, cell, node[1]));

						if (v1.type != FormulaValueType.Number || v2.type != FormulaValueType.Number)
						{
							throw new FormulaTypeMismatchException(cell);
							//return FormulaValue.Nil;
						}

						return Math.Pow((double)v1.value, (double)v2.value);
					}
				#endregion // POW

				case STNodeType.UNARY_MINUS:
					#region UNARY_MINUS
					{
						FormulaValue val = Evaluate(workbook, cell, node[0]);

						if (val.type != FormulaValueType.Number)
						{
							throw new FormulaTypeMismatchException(cell);
						}

						return (FormulaValue)(-(double)val.value);
						//return val.type == FormulaValueType.Number ? (FormulaValue)(-(double)val.value) : FormulaValue.Nil;
					}
				#endregion // UNARY_MINUS

				case STNodeType.UNARY_PERCENT:
					#region UNARY_PERCENT
					{
						FormulaValue val = Evaluate(workbook, cell, node[0]);

						if (val.type != FormulaValueType.Number)
						{
							throw new FormulaTypeMismatchException(cell);
						}

						return (FormulaValue)((double)val.value / 100d);
					}
				#endregion // UNARY_PERCENT

				case STNodeType.FUNCTION_CALL:
					return CallFunction(cell, node);

				#region Equals
				case STNodeType.EQUALS:
					{
						FormulaValue v1 = Evaluate(workbook, cell, node[0]);
						FormulaValue v2 = Evaluate(workbook, cell, node[1]);

						if (v1.type != v2.type) return false;

						switch (v1.type)
						{
							case FormulaValueType.Number:
								return (double)v1.value == (double)v2.value;

							case FormulaValueType.String:
								return (string)v1.value == (string)v2.value;

							case FormulaValueType.Boolean:
								return (bool)v1.value == (bool)v2.value;

							default:
								return false;
						}
					}
				#endregion // Equals

				#region Not Equals
				case STNodeType.NOT_EQUALS:
					{
						FormulaValue v1 = Evaluate(workbook, cell, node[0]);
						FormulaValue v2 = Evaluate(workbook, cell, node[1]);

						if (v1.type != v2.type) return true;

						switch (v1.type)
						{
							case FormulaValueType.Number:
								return (double)v1.value != (double)v2.value;

							case FormulaValueType.String:
								return (string)v1.value != (string)v2.value;

							case FormulaValueType.Boolean:
								return (bool)v1.value != (bool)v2.value;

							default:
								return true;
						}
					}
				#endregion // Not Equals

				#region Greater than
				case STNodeType.GREAT_EQUALS:
				case STNodeType.GREAT_THAN:
					{
						FormulaValue v1 = Evaluate(workbook, cell, node[0]);
						FormulaValue v2 = Evaluate(workbook, cell, node[1]);

						if (v1.type == FormulaValueType.Number && v2.type == FormulaValueType.Number)
						{
							return (node.Type == STNodeType.GREAT_EQUALS)
								? (double)v1.value >= (double)v2.value
								: (double)v1.value > (double)v2.value;
						}

						if (v1.type == FormulaValueType.String || v2.type == FormulaValueType.String)
						{
							string str1 = (v1.type == FormulaValueType.String) ? (string)v1.value : Convert.ToString(v1.value);
							string str2 = (v2.type == FormulaValueType.String) ? (string)v2.value : Convert.ToString(v2.value);

							return node.Type == STNodeType.GREAT_EQUALS
								? string.Compare(str1, str2) >= 0
								: string.Compare(str1, str2) > 0;
						}

						if (v1.type == FormulaValueType.Boolean && v2.type == FormulaValueType.Boolean)
						{
							return (node.Type == STNodeType.GREAT_EQUALS)
								? ((bool)v1.value ? 1 : 0) >= ((bool)v2.value ? 1 : 0)
								: ((bool)v1.value ? 1 : 0) > ((bool)v2.value ? 1 : 0);
						}

						return false;
					}
				#endregion // Greater than

				#region Less than
				case STNodeType.LESS_EQUALS:
				case STNodeType.LESS_THAN:
					{
						FormulaValue v1 = Evaluate(workbook, cell, node[0]);
						FormulaValue v2 = Evaluate(workbook, cell, node[1]);

						if (v1.type == FormulaValueType.Number && v2.type == FormulaValueType.Number)
						{
							return (node.Type == STNodeType.LESS_EQUALS)
								? (double)v1.value <= (double)v2.value
								: (double)v1.value < (double)v2.value;
						}

						if (v1.type == FormulaValueType.String || v2.type == FormulaValueType.String)
						{
							string str1 = (v1.type == FormulaValueType.String) ? (string)v1.value : Convert.ToString(v1.value);
							string str2 = (v2.type == FormulaValueType.String) ? (string)v2.value : Convert.ToString(v2.value);

							return node.Type == STNodeType.LESS_EQUALS
								? string.Compare(str1, str2) <= 0
								: string.Compare(str1, str2) < 0;
						}

						if (v1.type == FormulaValueType.Boolean && v2.type == FormulaValueType.Boolean)
						{
							return (node.Type == STNodeType.LESS_EQUALS)
								? ((bool)v1.value ? 1 : 0) <= ((bool)v2.value ? 1 : 0)
								: ((bool)v1.value ? 1 : 0) < ((bool)v2.value ? 1 : 0);
						}

						return false;
					}
				#endregion // Less than

				case STNodeType.SUB_EXPR:
					return (node.Children.Count < 1) ? FormulaValue.Nil : Evaluate(workbook, cell, node[0]);

				case STNodeType._FORMULA_VALUE:
					return ((STValueNode)node).Value;

				default:
					return FormulaValue.Nil;
					#endregion // Parse Nodes
			}
		}
		#endregion // Evaluate

		internal static FormulaValue CheckAndGetDefaultValue(Cell cell, FormulaValue val)
		{
			if (val.type == FormulaValueType.Nil)
			{
				if (FormulaExtension.EmptyCellReferenceProvider != null)
				{
					return CreateFormulaValue(FormulaExtension.EmptyCellReferenceProvider(cell.Worksheet, cell.InternalPos, null));
				}
				else
					return 0;
			}
			else
				return val;
		}

		internal static FormulaValue CreateFormulaValue(Cell cell)
		{
			object obj = null;

			if (cell != null)
			{
				obj = cell.Data;
			}

			if (obj == null)
			{
				return FormulaValue.Nil;
			}

			return CreateFormulaValue(obj);
		}

		internal static FormulaValue CreateFormulaValue(object obj)
		{
			if (obj == null) return FormulaValue.Nil;

			if (CellUtility.TryGetNumberData(obj, out var val))
			{
				return val;
			}
			else if (obj is string)
				return (string)obj;
			else if (obj is bool)
				return (bool)obj;
			else if (obj is DateTime)
				return (DateTime)obj;
			else
				return Convert.ToString(obj);
		}

		internal static FormulaValue CallFunction(Cell cell, STNode node)
		{
			var funNode = (STFunctionNode)node;
			if (funNode == null || string.IsNullOrEmpty(funNode.Name)) return FormulaValue.Nil;

			FormulaValue[] args;
			double val;
			DateTime dt;
			string str, str2;
			int i, i2;

			switch (funNode.Name)
			{
				#region Stat

				case BuiltinFunctionNames.SUM_EN:
				case BuiltinFunctionNames.SUM_RU:
					return ExcelFunctions.Sum(cell, GetFunctionArgs(cell, funNode.Children, 1));

				case BuiltinFunctionNames.AVERAGE_EN:
				case BuiltinFunctionNames.AVERAGE_RU:
					return ExcelFunctions.Average(cell, GetFunctionArgs(cell, funNode.Children, 1));

				case BuiltinFunctionNames.COUNT_EN:
				case BuiltinFunctionNames.COUNT_RU:
					return ExcelFunctions.Count(cell, GetFunctionArgs(cell, funNode.Children, 1), includeEmpty: false);

				case BuiltinFunctionNames.COUNTA_EN:
				case BuiltinFunctionNames.COUNTA_RU:
					return ExcelFunctions.Count(cell, GetFunctionArgs(cell, funNode.Children, 1), includeEmpty: true);

				case BuiltinFunctionNames.MIN_EN:
				case BuiltinFunctionNames.MIN_RU:
					return ExcelFunctions.Min(cell, GetFunctionArgs(cell, funNode.Children, 1));

				case BuiltinFunctionNames.MAX_EN:
				case BuiltinFunctionNames.MAX_RU:
					return ExcelFunctions.Max(cell, GetFunctionArgs(cell, funNode.Children, 1));

				case BuiltinFunctionNames.SUMIF_EN:
				case BuiltinFunctionNames.SUMIF_RU:
					return ExcelFunctions.SumIf(cell, GetFunctionArgs(cell, funNode.Children, 2));

				case BuiltinFunctionNames.AVERAGEIF_EN:
				case BuiltinFunctionNames.AVERAGEIF_RU:
					return ExcelFunctions.AverageIf(cell, GetFunctionArgs(cell, funNode.Children, 2));

				case BuiltinFunctionNames.COUNTIF_EN:
				case BuiltinFunctionNames.COUNTIF_RU:
					return ExcelFunctions.CountIf(cell, GetFunctionArgs(cell, funNode.Children, 2));

				#endregion // Stat

				#region Data
				case BuiltinFunctionNames.VLOOKUP_EN:
				case BuiltinFunctionNames.VLOOKUP_RU:
					return ExcelFunctions.VLookup(cell, GetFunctionArgs(cell, funNode.Children, 3, 4));

				#endregion // Data

				#region Conditions

				case BuiltinFunctionNames.IF_EN:
				case BuiltinFunctionNames.IF_RU:
					return ExcelFunctions.If(cell, funNode);

				case BuiltinFunctionNames.AND_EN:
				case BuiltinFunctionNames.AND_RU:
					return ExcelFunctions.And(cell, funNode.Children);

				case BuiltinFunctionNames.OR_EN:
				case BuiltinFunctionNames.OR_RU:
					return ExcelFunctions.Or(cell, funNode.Children);

				case BuiltinFunctionNames.NOT_EN:
				case BuiltinFunctionNames.NOT_RU:
					return ExcelFunctions.Not(cell, funNode.Children[0]);

				#endregion // Conditions

				#region Address

				case BuiltinFunctionNames.ROW_EN:
				case BuiltinFunctionNames.ROW_RU:
					return ExcelFunctions.Row(cell, funNode.Children);

				case BuiltinFunctionNames.COLUMN_EN:
				case BuiltinFunctionNames.COLUMN_RU:
					return ExcelFunctions.Column(cell, funNode.Children);

				case BuiltinFunctionNames.ADDRESS_EN:
				case BuiltinFunctionNames.ADDRESS_RU:
					return ExcelFunctions.Address(cell, GetFunctionArgs(cell, funNode.Children, 2));

				case BuiltinFunctionNames.INDIRECT_EN:
				case BuiltinFunctionNames.INDIRECT_RU:
					return ExcelFunctions.Indirect(cell, GetFunctionArgs(cell, funNode.Children, 1));

				#endregion // Address

				#region Math

				case BuiltinFunctionNames.ABS_EN:
				//case BuiltinFunctionNames.ABS_RU:
					return Math.Abs(GetFunctionNumberArg(cell, funNode.Children));

				case BuiltinFunctionNames.ROUND_EN:
				case BuiltinFunctionNames.ROUND_RU:
					return ExcelFunctions.Round(cell, GetFunctionArgs(cell, funNode.Children, 1, 2));

				case BuiltinFunctionNames.CEILING_EN:
				case BuiltinFunctionNames.CEILING_RU:
					return ExcelFunctions.Ceiling(cell, GetFunctionArgs(cell, funNode.Children, 1, 2));

				case BuiltinFunctionNames.FLOOR_EN:
				case BuiltinFunctionNames.FLOOR_RU:
					return ExcelFunctions.Floor(cell, GetFunctionArgs(cell, funNode.Children, 1, 2));

				case BuiltinFunctionNames.SIN_EN:
				//case BuiltinFunctionNames.SIN_RU:
					return Math.Sin(GetFunctionNumberArg(cell, funNode.Children));

				case BuiltinFunctionNames.COS_EN:
				//case BuiltinFunctionNames.COS_RU:
					return Math.Cos(GetFunctionNumberArg(cell, funNode.Children));

				case BuiltinFunctionNames.TAN_EN:
				//case BuiltinFunctionNames.TAN_RU:
					return Math.Tan(GetFunctionNumberArg(cell, funNode.Children));

				case BuiltinFunctionNames.ASIN_EN:
				//case BuiltinFunctionNames.ASIN_RU:
					return Math.Asin(GetFunctionNumberArg(cell, funNode.Children));

				case BuiltinFunctionNames.ACOS_EN:
				//case BuiltinFunctionNames.ACOS_RU:
					return Math.Acos(GetFunctionNumberArg(cell, funNode.Children));

				case BuiltinFunctionNames.ATAN_EN:
				//case BuiltinFunctionNames.ATAN_RU:
					return Math.Atan(GetFunctionNumberArg(cell, funNode.Children));

				case BuiltinFunctionNames.ATAN2_EN:
				//case BuiltinFunctionNames.ATAN2_RU:
					#region ATAN2
					args = GetFunctionArgs(cell, funNode.Children, 2);
					return (args[0].type != FormulaValueType.Number || args[1].type != FormulaValueType.Number)
						? (FormulaValue)null : (FormulaValue)Math.Atan2((double)args[0].value, (double)args[1].value);
				#endregion // ATAN2

				case BuiltinFunctionNames.POWER_EN:
				case BuiltinFunctionNames.POWER_RU:
					#region POWER
					args = GetFunctionArgs(cell, funNode.Children, 2);

					if (args[0].type != FormulaValueType.Number || args[1].type != FormulaValueType.Number)
					{
						throw new FormulaParameterMismatchException(cell);
					}

					return (FormulaValue)Math.Pow((double)args[0].value, (double)args[1].value);
					#endregion // POWER

				case BuiltinFunctionNames.EXP_EN:
				//case BuiltinFunctionNames.EXP_RU:
					val = (double)GetFunctionArg(cell, funNode.Children, FormulaValueType.Number);
					return Math.Exp(val);

				case BuiltinFunctionNames.LOG_EN:
				//case BuiltinFunctionNames.LOG_RU:
					#region LOG
					args = GetFunctionArgs(cell, funNode.Children, 1, 2);
					if (args.Length < 2)
					{
						if (args[0].type != FormulaValueType.Number)
						{
							throw new FormulaParameterMismatchException(cell);
						}
						return Math.Log((double)args[0].value);
					}
					else
					{
						if (args[0].type != FormulaValueType.Number || args[1].type != FormulaValueType.Number)
						{
							throw new FormulaParameterMismatchException(cell);
						}
						return Math.Log((double)args[0].value, (double)args[1].value);
					}
					#endregion // LOG

				case BuiltinFunctionNames.LOG10_EN:
				//case BuiltinFunctionNames.LOG10_RU:
					#region LOG10
					val = (double)GetFunctionArg(cell, funNode.Children, FormulaValueType.Number);
					return Math.Log10(val);
				#endregion // LOG10

				case BuiltinFunctionNames.MOD_EN:
				case BuiltinFunctionNames.MOD_RU:
					#region MOD
					args = GetFunctionArgs(cell, funNode.Children, 2);

					if (args[0].type != FormulaValueType.Number || args[1].type != FormulaValueType.Number)
					{
						throw new FormulaParameterMismatchException(cell);
					}

					return (FormulaValue)((double)args[0].value % (double)args[1].value);
					#endregion // MOD

				#endregion // Math

				#region Datetime

				case BuiltinFunctionNames.NOW_EN:
				case BuiltinFunctionNames.NOW_RU:
					#region NOW
					if (cell.DataFormat == DataFormat.CellDataFormatFlag.General)
					{
						cell.DataFormat = DataFormat.CellDataFormatFlag.DateTime;
						cell.DataFormatArgs = new DataFormat.DateTimeDataFormatter.DateTimeFormatArgs
						{
							Format = "yyyy/MM/dd HH:mm",
							CultureName = "en-US",
						};
					}
					return DateTime.Now;
					#endregion // NOW

				case BuiltinFunctionNames.TODAY_EN:
				case BuiltinFunctionNames.TODAY_RU:
					#region TODAY
					if (cell.DataFormat == DataFormat.CellDataFormatFlag.General)
					{
						cell.DataFormat = DataFormat.CellDataFormatFlag.DateTime;
						cell.DataFormatArgs = new DataFormat.DateTimeDataFormatter.DateTimeFormatArgs
						{
							Format = "yyyy/MM/dd",
							CultureName = "en-US",
						};
					}
					return DateTime.Now;
					#endregion // TODAY

				case BuiltinFunctionNames.TIME_EN:
				case BuiltinFunctionNames.TIME_RU:
					#region TIME
					args = GetFunctionArgs(cell, funNode.Children, 3);

					if (args[0].type != FormulaValueType.Number
						|| args[1].type != FormulaValueType.Number
						|| args[2].type != FormulaValueType.Number)
					{
						throw new FormulaParameterMismatchException(cell);
					}

					dt = new DateTime(1900, 1, 1, (int)(double)args[0].value, (int)(double)args[1].value, (int)(double)args[2].value);

					if (cell.DataFormat == DataFormat.CellDataFormatFlag.General)
					{
						cell.DataFormat = DataFormat.CellDataFormatFlag.DateTime;
						cell.DataFormatArgs = new DataFormat.DateTimeDataFormatter.DateTimeFormatArgs
						{
							Format = "HH:mm:ss",
							CultureName = "en-US",
						};
					}

					return dt;
				#endregion // TIME

				case BuiltinFunctionNames.YEAR_EN:
				case BuiltinFunctionNames.YEAR_RU:
					dt = (DateTime)GetFunctionArg(cell, funNode.Children, FormulaValueType.DateTime);
					return dt.Year;

				case BuiltinFunctionNames.MONTH_EN:
				case BuiltinFunctionNames.MONTH_RU:
					dt = (DateTime)GetFunctionArg(cell, funNode.Children, FormulaValueType.DateTime);
					return dt.Month;

				case BuiltinFunctionNames.DAY_EN:
				case BuiltinFunctionNames.DAY_RU:
					dt = (DateTime)GetFunctionArg(cell, funNode.Children, FormulaValueType.DateTime);
					return dt.Day;

				case BuiltinFunctionNames.HOUR_EN:
				case BuiltinFunctionNames.HOUR_RU:
					dt = (DateTime)GetFunctionArg(cell, funNode.Children, FormulaValueType.DateTime);
					return dt.Hour;

				case BuiltinFunctionNames.MINUTE_EN:
				case BuiltinFunctionNames.MINUTE_RU:
					dt = (DateTime)GetFunctionArg(cell, funNode.Children, FormulaValueType.DateTime);
					return dt.Minute;

				case BuiltinFunctionNames.SECOND_EN:
				case BuiltinFunctionNames.SECOND_RU:
					dt = (DateTime)GetFunctionArg(cell, funNode.Children, FormulaValueType.DateTime);
					return dt.Second;

				case BuiltinFunctionNames.MILLISECOND_EN:
				case BuiltinFunctionNames.MILLISECOND_RU:
					dt = (DateTime)GetFunctionArg(cell, funNode.Children, FormulaValueType.DateTime);
					return dt.Millisecond;

				case BuiltinFunctionNames.DAYS_EN:
				case BuiltinFunctionNames.DAYS_RU:
					args = GetFunctionArgs(cell, funNode.Children, 2);
					if (args[0].type != FormulaValueType.DateTime || args[1].type != FormulaValueType.DateTime)
					{
						throw new FormulaParameterMismatchException(cell);
					}
					return ((DateTime)args[0].value - (DateTime)args[1].value).TotalDays;

				#endregion // Datetime

				#region String
				case BuiltinFunctionNames.LEFT_EN:
				case BuiltinFunctionNames.LEFT_RU:
					args = GetFunctionArgs(cell, funNode.Children, 2);

					if (args[0].type != FormulaValueType.String || args[1].type != FormulaValueType.Number)
					{
						throw new FormulaParameterMismatchException(cell);
					}

					str = (string)args[0].value;
					i = (int)(double)args[1].value;

					if (str == null) return null;

					if (i >= str.Length)
						return str;
					else
						return str.Substring(0, i);

				case BuiltinFunctionNames.RIGHT_EN:
				case BuiltinFunctionNames.RIGHT_RU:
					args = GetFunctionArgs(cell, funNode.Children, 2);

					if (args[0].type != FormulaValueType.String || args[1].type != FormulaValueType.Number)
					{
						throw new FormulaParameterMismatchException(cell);
					}

					str = (string)args[0].value;
					i = (int)(double)args[1].value;

					if (str == null) return null;

					if (i >= str.Length)
						return str;
					else
						return str.Substring(str.Length - i);

				case BuiltinFunctionNames.MID_EN:
				case BuiltinFunctionNames.MID_RU:
					args = GetFunctionArgs(cell, funNode.Children, 3);

					if (args[0].type != FormulaValueType.String || args[1].type != FormulaValueType.Number
						|| args[2].type != FormulaValueType.Number)
					{
						throw new FormulaParameterMismatchException(cell);
					}

					str = (string)args[0].value;
					i = (int)(double)args[1].value;

					if (i > str.Length)
					{
						return null;
					}

					if (i < 1) i = 1;

					i2 = (int)(double)args[2].value;

					if (i + i2 > str.Length)
					{
						i2 = str.Length - i + 1;
					}

					return str.Substring(i - 1, i2);

				case BuiltinFunctionNames.UPPER_EN:
				case BuiltinFunctionNames.UPPER_RU:
					str = (string)GetFunctionArg(cell, funNode.Children, FormulaValueType.String);
					return str.ToUpper();

				case BuiltinFunctionNames.LOWER_EN:
				case BuiltinFunctionNames.LOWER_RU:
					str = (string)GetFunctionArg(cell, funNode.Children, FormulaValueType.String);
					return str.ToLower();

				case BuiltinFunctionNames.LEN_EN:
				case BuiltinFunctionNames.LEN_RU:
					str = (string)GetFunctionArg(cell, funNode.Children, FormulaValueType.String);
					return str.Length;

				case BuiltinFunctionNames.FIND_EN:
				case BuiltinFunctionNames.FIND_RU:
					args = GetFunctionArgs(cell, funNode.Children, 2);
					if (args[0].type != FormulaValueType.String || args[1].type != FormulaValueType.String) return null;

					if (args.Length > 2)
					{
						if (args[2].type != FormulaValueType.Number)
						{
							return null;
						}

						i = (int)(double)args[2];

						if (i < 1) i = 1;
					}
					else
					{
						i = 1;
					}

					str = (string)args[0].value;
					str2 = (string)args[1].value;

					if (str == null || str.Length <= 0
						|| str2 == null || str2.Length <= 0 || i > str2.Length)
					{
						return 0;
					}

					return str2.IndexOf(str, i - 1) + 1;

				case BuiltinFunctionNames.TRIM_EN:
				case BuiltinFunctionNames.TRIM_RU:
					return ((string)GetFunctionArg(cell, funNode.Children, FormulaValueType.String)).Trim();

				#endregion // String

				#region System
				#region ISERROR
				case BuiltinFunctionNames.ISERROR_EN:
				case BuiltinFunctionNames.ISERROR_RU:
					if (funNode.Children == null || funNode.Children.Count < 1) return null;

					//if (cell.formulaParseError)
					//{
					//	return true;
					//}

					try
					{
						Evaluator.Evaluate(cell, funNode.Children[0]);

						return cell.formulaStatus != FormulaStatus.Normal;

						//return value.type == FormulaValueType._NoValue
						//	|| value.type == FormulaValueType._NoName;
					}
					catch (Exception)
					{
						return true;
					}
				#endregion // ISERROR
				#region ISNUMBER
				case BuiltinFunctionNames.ISNUMBER_EN:
				case BuiltinFunctionNames.ISNUMBER_RU:
					if (funNode.Children == null || funNode.Children.Count < 1)
					{
						return false;
					}
					else if (funNode.Children[0].Type == STNodeType.NUMBER)
					{
						return true;
					}
					else
					{
						var obj = Evaluator.Evaluate(cell, funNode.Children[0]);

						if (obj.type == FormulaValueType.Number)
						{
							return true;
						}
						else
							return false;

						//return CellUtility.IsNumberData(funNode.Children[0]);
					}
				#endregion // ISNUMBER
				#endregion System

				default:

					object[] objArgs = null;

					#region Customize Functions
					if (FormulaExtension.customFunctions != null)
					{
						if (FormulaExtension.customFunctions.TryGetValue(funNode.Name, out var customFunction))
						{
							objArgs = new object[funNode.Children == null ? 0 : funNode.Children.Count];

							for (i = 0; i < objArgs.Length; i++)
							{
								objArgs[i] = Evaluate(cell, funNode.Children[i]).value;
							}

							return CreateFormulaValue(customFunction(cell, objArgs));
						}
					}
					#endregion // Customize Functions

					#region ReoScript Extension Function
#if EX_SCRIPT
					if (cell.Worksheet != null && cell.Worksheet.Srm.IsFunction(funNode.Name))
					{
						if (objArgs == null)
						{
							objArgs = new object[funNode.Children.Count];

							for (i = 0; i < objArgs.Length; i++)
							{
								objArgs[i] = Evaluate(cell, funNode.Children[i]).value;
							}
						}

						return CreateFormulaValue(cell.Worksheet.Srm.InvokeFunctionIfExisted(funNode.Name, objArgs));
					}
#endif
					#endregion // ReoScript Extension Function

					//throw new FormulaEvalutionException("function not supported: " + funNode.FunName);
					return FormulaValue.Nil;
			}
		}

		#region Get Function Arguments

		static FormulaValue[] GetFunctionArgs(Cell cell, List<STNode> argNodes, int min = -1, int max = -1)
		{
			int argCount = argNodes == null ? 0 : argNodes.Count;

			if (min > argCount)
			{
				throw new FormulaParameterMismatchException(cell, string.Format(
					"need at least {0} arguments but only {1} was given", min, argCount));
			}

			if (max == -1 || max > argCount)
			{
				max = argCount;
			}

			FormulaValue[] values = new FormulaValue[max];

			for (int i = 0, k = 0; i < max; i++, k++)
			{
				values[k] = Evaluator.Evaluate(cell, argNodes[i]);
			}

			return values;
		}

		static double GetFunctionNumberArg(Cell cell, List<STNode> argNodes)
		{
			return (double)GetFunctionArg(cell, argNodes, FormulaValueType.Number);
		}

		static string GetFunctionStringArg(Cell cell, List<STNode> argNodes)
		{
			return (string)GetFunctionArg(cell, argNodes, FormulaValueType.String, true);
		}

		static object GetFunctionArg(Cell cell, List<STNode> argNodes, FormulaValueType type, bool allowConvert = false)
		{
			if (argNodes.Count < 1)
			{
				throw new FormulaParameterMismatchException(cell, "Function needs at least one argument");
			}

			var val = Evaluator.Evaluate(cell, argNodes[0]);

			// given value type does not meet function's requirement
			if (val.type != type)
			{
				if (allowConvert)
				{
					switch (type)
					{
						case FormulaValueType.String:
							return Convert.ToString(val.value);
					}
				}

				throw new FormulaTypeMismatchException(cell);
			}

			return val.value;
		}

		#endregion // Get Function Arguments

		static internal Worksheet GetWorksheetFromValue(Cell ownerCell, FormulaValue val)
		{
			Worksheet sheet = null;

			if (val.type == FormulaValueType.Cell)
			{
				sheet = ((SheetCellPosition)val.value).Worksheet;
			}
			else if (val.type == FormulaValueType.Range)
			{
				sheet = ((SheetRangePosition)val.value).Worksheet;
			}

			return sheet == null ? ownerCell.Worksheet : sheet;
		}
	}

	#region FormulaValue

	//#if DEBUG
	//	public
	//#endif
	struct FormulaValue
	{
		public FormulaValueType type;
		public object value;

		public static FormulaValue Nil = new FormulaValue { type = FormulaValueType.Nil };
		public static FormulaValue True = new FormulaValue { type = FormulaValueType.Boolean, value = true };
		public static FormulaValue False = new FormulaValue { type = FormulaValueType.Boolean, value = false };

		//public static FormulaValue _Error = new FormulaValue { type = FormulaValueType._Error };
		//public static FormulaValue _NoValue = new FormulaValue { type = FormulaValueType._NoValue };
		//public static FormulaValue _NoName = new FormulaValue { type = FormulaValueType._NoName };

		public FormulaValue(FormulaValueType type, object value)
		{
			this.type = type;
			this.value = value;
		}

		/// <summary>
		/// Convert this value into string
		/// </summary>
		/// <returns>Return the value in string type</returns>
		public override string ToString()
		{
			return string.Format("Value[{0}: {1}]", this.type, this.value);
		}

		#region String
		public static implicit operator string(FormulaValue value)
		{
			return Convert.ToString(value.value);
		}

		public static implicit operator FormulaValue(string text)
		{
			return new FormulaValue(FormulaValueType.String, text);
		}
		#endregion // String

		#region Number
		public static implicit operator double(FormulaValue value)
		{
			return value.type != FormulaValueType.Number ? 0 : (double)value.value;
		}

		public static implicit operator FormulaValue(double num)
		{
			return new FormulaValue(FormulaValueType.Number, num);
		}
		#endregion // Number

		#region Boolean
		public static implicit operator bool(FormulaValue value)
		{
			return value.type != FormulaValueType.Boolean ? false : (bool)value.value;
		}

		public static implicit operator FormulaValue(bool b)
		{
			return new FormulaValue(FormulaValueType.Boolean, b);
		}
		#endregion // Boolean

		#region DateTime
		public static implicit operator DateTime(FormulaValue value)
		{
			return value.type != FormulaValueType.DateTime ? new DateTime(1900, 1, 1) : (DateTime)value.value;
		}

		public static implicit operator FormulaValue(DateTime b)
		{
			return new FormulaValue(FormulaValueType.DateTime, b);
		}
		#endregion // DateTime

		#region Cell
		public static implicit operator CellPosition(FormulaValue value)
		{
			return value.type != FormulaValueType.Cell ? CellPosition.Empty : (CellPosition)value.value;
		}

		public static implicit operator FormulaValue(CellPosition pos)
		{
			return new FormulaValue(FormulaValueType.Cell, pos);
		}
		#endregion // Cell

		#region Range
		public static implicit operator RangePosition(FormulaValue value)
		{
			return value.type != FormulaValueType.Range ? RangePosition.Empty : (RangePosition)value.value;
		}

		public static implicit operator FormulaValue(RangePosition range)
		{
			return new FormulaValue(FormulaValueType.Range, range);
		}
		#endregion // Range

		#region object/null
		//public static implicit operator object(FormulaValue range)
		//{
		//	return new FormulaValue(FormulaValueType.Range, range);
		//}

		//public static implicit operator FormulaValue(object value)
		//{
		//	if (value == null)
		//		return FormulaValue.Nil;
		//	else if (value is double)
		//		return (double)value;
		//	else if (value is string)
		//		return (string)value;
		//	else if (value is bool)
		//		return (bool)value;
		//	else if (value is DateTime)
		//		return (DateTime)value;
		//	else if (value is ReoGridPos)
		//		return (ReoGridPos)value;
		//	else if (value is ReoGridRange)
		//		return (ReoGridRange)value;
		//	else
		//		throw new NotSupportedException();
		//}
		#endregion // Range
	}

	#endregion // FormulaValue

	class SheetCellPosition
	{
		public Worksheet Worksheet { get; set; }
		public CellPosition Position { get; set; }

		public static implicit operator CellPosition(SheetCellPosition pos)
		{
			return pos.Position;
		}
	}

	class SheetRangePosition
	{
		public Worksheet Worksheet { get; set; }
		public RangePosition Position { get; set; } 

		public static implicit operator RangePosition(SheetRangePosition pos)
		{
			return pos.Position;
		}
	}

	//#if DEBUG
	//	public
	//#endif
	enum FormulaValueType : byte
	{
		Nil,
		Boolean,
		Number,
		String,
		DateTime,
		Range,
		Cell,
	}

	/// <summary>
	/// Flag to identify the status of formula parsing and calculation
	/// </summary>
	public enum FormulaStatus
	{
		/// <summary>
		/// Normal status (No errors happen)
		/// </summary>
		Normal,

		/// <summary>
		/// Formula has syntax errors and cannot be parsed correctly
		/// </summary>
		SyntaxError,

		/// <summary>
		/// Represents that circular reference problem was detected in formula.
		/// </summary>
		CircularReference,

		/// <summary>
		/// A specified type was required during formula calculation, but no valid type found. (Similar to "#VALUE!" in Excel)
		/// </summary>
		InvalidValue,

		/// <summary>
		/// A reference to a cell or range is invalid. (Similar to "#REF!" in Excel)
		/// </summary>
		InvalidReference,

		/// <summary>
		/// A name was referenced in formula, but no valid range was found by this name. (Similar to "#NAME!" in Excel)
		/// </summary>
		NameNotFound,

		/// <summary>
		/// Attempt to call a function with mismatched parameter type or amount.
		/// </summary>
		MismatchedParameter,

		/// <summary>
		/// Some unspecified errors happened during formula parsing and calculation.
		/// </summary>
		UnspecifiedError,
	}

	interface IFunctionNameProvider
	{
		FormulaValue CallFunction(Evaluator evaluator, Cell cell, STNode node);
	}

	class StandardFunctionNameProvider : IFunctionNameProvider
	{
		public FormulaValue CallFunction(Evaluator evaluator, Cell cell, STNode node)
		{
			return null;
		}
	}

	class RussianFunctionNameProvider : IFunctionNameProvider
	{
		public FormulaValue CallFunction(Evaluator evaluator, Cell cell, STNode node)
		{
			return null;
		}
	}

	#region Formula Exceptions

	/// <summary>
	/// Exception will be thrown when errors happened during formula evalution
	/// </summary>
	[Serializable]
	public class FormulaEvalutionException : ReoGridException
	{
		/// <summary>
		/// Cell of exception happened.
		/// </summary>
		public Cell Cell { get; set; }

		/// <summary>
		/// Create exception instance
		/// </summary>
		/// <param name="cell">Cell instance that may include the formula.</param>
		/// <param name="innerException">Original exception instance</param>
		/// <param name="message">Addtional message to describe this exception</param>
		public FormulaEvalutionException(Cell cell, Exception innerException, string message)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Create exception instance.
		/// </summary>
		/// <param name="cell">Cell instance that may include the formula.</param>
		/// <param name="message">Addtional message to describe this exception</param>
		public FormulaEvalutionException(Cell cell, string message)
			: base(message)
		{
		}
	}

	/// <summary>
	/// This exception will be throw if formula attempts to use a number value during calculation, 
	/// but the cell data cannot be converted into number value.
	/// </summary>
	[Serializable]
	public class FormulaTypeMismatchException : FormulaEvalutionException
	{
		/// <summary>
		/// Create the exception instance
		/// </summary>
		public FormulaTypeMismatchException(Cell cell) : base(cell, "Value type does not match in cell " + cell.Address) { }
	}

	/// <summary>
	/// This exception will be thrown if formula attempts to reference a named range, 
	/// but the name cannot be found or referenced correctly. 
	/// </summary>
	[Serializable]
	public class FormulaNoNameException : FormulaEvalutionException
	{
		/// <summary>
		/// Create the exception instance
		/// </summary>
		public FormulaNoNameException(Cell cell) : base(cell, "Invalid name reference in cell " + cell.Address) { }
	}

	/// <summary>
	/// This exception will be thrown if formula attempts to 
	/// call a function with mismatched parameters type or amount.
	/// </summary>
	[Serializable]
	public class FormulaParameterMismatchException : FormulaEvalutionException
	{
		/// <summary>
		/// Create the exception instance
		/// </summary>
		public FormulaParameterMismatchException(Cell cell) : base(cell, "Call function with mismatched parameters in cell " + cell.Address) { }

		/// <summary>
		/// Create the exception instance with additional message
		/// </summary>
		/// <param name="cell">Cell instance where the exception happened</param>
		/// <param name="message">Additional message to describe this exception</param>
		public FormulaParameterMismatchException(Cell cell, string message) : base(cell, message) { }
	}

	#endregion // Formula Exceptions
}

#endif // FORMULA
