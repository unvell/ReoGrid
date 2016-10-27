/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * http://reogrid.net/
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * Author: Jing <lujing at unvell.com>
 *
 * Copyright (c) 2012-2016 Jing <lujing at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

#if NO_LONGER_SUPPORT_SINCE_V088

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using unvell.Common;
using unvell.ReoGrid.Formula;

#if EX_SCRIPT
using unvell.ReoScript;
using unvell.ReoGrid.Script;
#endif

namespace unvell.ReoGrid.Formula
{
	static class RSFunctions
	{
		private static readonly Regex AutoCellReferenceRegex = new Regex(
			"(?:\"[^\"]*\\s*\")|(?:(?<from_col>[A-Z]+)(?<from_row>[0-9]+):(?<to_col>[A-Z]+)(?<to_row>[0-9]+))|(?:(?<from_col>[A-Z]+)(?<from_row>[0-9]+))");

		/// <summary>
		/// Adds its arguments
		/// </summary>
		/// <param name="ctrl"></param>
		/// <param name="range"></param>
		/// <returns></returns>
		public static double SUM(Worksheet ctrl, ReoGridRange range)
		{
			double val = 0;

			ctrl.IterateCells(range, (row, col, cell) =>
			{
				var data = cell.InnerData;

				if(data != null)
				{
					double cellValue;
					if (RGUtility.ConvertCellData<double>(data, out cellValue))
					{
						val += cellValue;
					}
				}

				return true;
			});

			return val;
		}

		/// <summary>
		/// Returns the average of its arguments
		/// </summary>
		/// <param name="ctrl">instance of ReoGrid control</param>
		/// <param name="range">range to count</param>
		/// <returns></returns>
		public static double AVERAGE(Worksheet ctrl, ReoGridRange range)
		{
			double val = 0;
			int count = 0;

			ctrl.IterateCells(range, (row, col, cell) =>
			{
				var data = cell.InnerData;

				if (data != null)
				{
					double cellValue;
					if (!RGUtility.ConvertCellData<double>(data, out cellValue))
					{
						throw new FormulaEvalutionException("Input value is not in number: " + cell.InternalPos.ToAddress());
					}

					count++;
				}

				return true;
			});

			return val / count;
		}

		/// <summary>
		/// Counts how many numbers are in the list of arguments
		/// </summary>
		/// <param name="ctrl">instance of ReoGrid control</param>
		/// <param name="range">range to count</param>
		/// <returns>number of items in specified range</returns>
		public static double COUNT(Worksheet ctrl, ReoGridRange range)
		{
			int count = 0;

			ctrl.IterateCells(range, (row, col, cell) =>
			{
				count++;
				return true;
			});

			return count;
		}

		/// <summary>
		/// Rounds a number to the nearest integer or to the nearest multiple of significance
		/// </summary>
		/// <param name="ctrl">instance of ReoGrid control</param>
		/// <param name="args">function arguments</param>
		/// <returns>nearest integer or to the nearest multiple of significance</returns>
		public static double CEILING(Worksheet ctrl, object[] args)
		{
			if (args.Length < 1) return double.NaN;

			double input = RGUtility.ConvertCellData<double>(args[0]);

			if (args.Length < 2)
			{
				return Math.Ceiling(input);
			}

			double significance = RGUtility.ConvertCellData<double>(args[1]);

			if ((input % significance) != 0)
			{
				return ((int)(input / significance) * significance) + significance;
			}
			else
			{
				return Math.Round(input);
			}
		}

		internal static void SetupBuiltinFunctions(Worksheet grid)
		{
#if EX_SCRIPT
			var srm = grid.Srm;

			srm["SUM"] = new NativeFunctionObject("SUM", (ctx, owner, args) => SUM(grid, RSUtility.GetRangeFromArgs(grid, args)));
			srm["AVERAGE"] = new NativeFunctionObject("AVERAGE", (ctx, owner, args) => AVERAGE(grid, RSUtility.GetRangeFromArgs(grid, args)));
			srm["COUNT"] = new NativeFunctionObject("COUNT", (ctx, owner, args) => COUNT(grid, RSUtility.GetRangeFromArgs(grid, args)));
			srm["CEILING"] = new NativeFunctionObject("CEILING", (ctx, owner, args) => CEILING(grid, args));

			#region ROW & COLUMN
			srm["ROW"] = new NativeFunctionObject("ROW", (ctx, owner, args) =>
			{
				var cell = ctx["__cell__"] as ReoGridCell;
				if (cell == null)
				{
					throw new FormulaEvalutionException("ROW function must be used in an evaluation of formula belonging to a cell.");
				}
				return cell.InternalRow;
			});

			srm["COLUMN"] = new NativeFunctionObject("COLUMN", (ctx, owner, args) =>
			{
				var cell = ctx["__cell__"] as ReoGridCell;
				if (cell == null)
				{
					throw new FormulaEvalutionException("COLUMN function must be used in an evaluation of formula belonging to a cell.");
				}
				return cell.InternalCol;
			});
			#endregion // ROW & COLUMN

			#region ADDRESS
			srm["ADDRESS"] = new NativeFunctionObject("ADDRESS", (ctx, owner, args) =>
			{
				if (args.Length < 2)
				{
					throw new FormulaEvalutionException("ADDRESS function needs at least 2 arguments.");
				}

				int row = RGUtility.ConvertCellData<int>(args[0]);
				int col = RGUtility.ConvertCellData<int>(args[1]);

				int absNum = 1;

				if (args.Length > 2)
				{
					absNum = RGUtility.ConvertCellData<int>(args[2]);
				}

				// Excel standard: false
				bool a1style = true;

				if (args.Length > 3)
				{
					a1style = RGUtility.ConvertCellData<bool>(args[3]);
				}

				return RGUtility.ToAddress(row, col, absNum, a1style);
			});
			#endregion // ADDRESS

			srm["ROUND"] = new NativeFunctionObject("ROUND", (ctx, owner, args) =>
			{
				if (args.Length < 1)
				{
					throw new FormulaEvalutionException("ROUND function needs at least 2 arguments.");
				}

				int digit = 0;
				if (args.Length > 1)
				{
					digit = RGUtility.ConvertCellData<int>(args[1]);
				}

				double value = RGUtility.ConvertCellData<double>(args[0]);

				return Math.Round(value, digit);
			});

			srm["RANGE"] = new NativeFunctionObject("RANGE", (ctx, owner, args) =>
			{
				if (args.Length < 2)
				{
					throw new FormulaEvalutionException("RANGE function needs at least 2 arguments.");
				}

				int row = RGUtility.ConvertCellData<int>(args[0]);
				int col = RGUtility.ConvertCellData<int>(args[1]);

				int rows = 1, cols = 1;

				if (args.Length > 2)
				{
					rows = RGUtility.ConvertCellData<int>(args[2]);
				}

				if (args.Length > 3)
				{
					cols = RGUtility.ConvertCellData<int>(args[3]);
				}

				return grid.FixRange(new ReoGridRange(row, col, rows, cols));
			});

			srm["INDIRECT"] = new NativeFunctionObject("INDIRECT", (ctx, owner, args) =>
			{
				if (args.Length == 0)
				{
					throw new FormulaEvalutionException("INDIRECT function needs at least 1 arguments.");
				}

				var arg1 = args[0];

				if (arg1 is string || arg1 is StringBuilder)
				{
					return grid.GetCellData(Convert.ToString(arg1));
				}
				else if (arg1 is ReoGridPos || arg1 is RSCellObject || arg1 is ReoGridCell)
				{
					return grid.GetCellData(RSUtility.GetPosFromValue(grid, arg1));
				}
				else
					return 0;
			});
#endif // EX_SCRIPT
		}
	}
}

namespace unvell.ReoGrid
{
	using Formula;
	using unvell.Common;
	using System.ComponentModel;

	#region ReoGrid Formula Provider Adapter Interface

	public partial class Worksheet
	{
		private IFormulaEvaluationProvider<ICompiledFormula> formulaEvaluationProvider;

		[System.ComponentModel.DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal IFormulaEvaluationProvider<ICompiledFormula> FormulaEvaluationProvider
		{
			get
			{
				return this.formulaEvaluationProvider;
			}
			set
			{
				if (value != null)
				{
					value.Workbook = this.workbook;
				}

				this.formulaEvaluationProvider = value;
			}
		}

		/// <summary>
		/// Evaluate a formula calculation
		/// </summary>
		/// <param name="formula">the formula to be evaluated</param>
		public object EvaluateFormula(string formula)
		{
			if (this.FormulaEvaluationProvider == null)
			{
				throw new NotSupportedException("No formula evaluation provider available. Set property 'FormulaEvalutionProvider' of control to specify a provider.");
			}

			// compile formula
			var compiledFormula = this.FormulaEvaluationProvider.Compile(formula);

			// evaluate formula
			return compiledFormula == null ? null : this.FormulaEvaluationProvider.Evaluate(compiledFormula);
		}

	}

	#endregion
}

#region Formula Provider Define

namespace unvell.ReoGrid.Formula
{
	internal interface IFormulaEvaluationProvider<T> where T : ICompiledFormula
	{
		Workbook Workbook { get; set; }

		T Compile(string input);

		object Evaluate(T cformula);
	}

	internal interface ICompiledFormula
	{
		IEnumerable<ReferenceRange> ReferencedCellOrRanges { get; }
	}

	internal interface IReferenceNode
	{
		int Start { get; }

		int Length { get; }

		string Value { get; }
	}
}

#endregion

#region ReoScript Adapter
#if EX_SCRIPT
namespace unvell.ReoGrid.Adapter
{
	internal sealed class ReoScriptFormulaProvider : IFormulaEvaluationProvider<ICompiledFormula>
	{
		public Workbook Workbook { get; set; }

		public ICompiledFormula Compile(string input)
		{
			return null;
			//var srm = this.Workbook.Srm;

			//// TODO: Compile formula in advance is not supported by current ReoScriptAdapter
			//// return (srm != null) ? new ReoScriptCompiledFormula(input, srm.Compile(input)) : null;
			//return (srm != null) ? new ReoScriptCompiledFormula(input, null) : null;
		}

		public object Evaluate(ReoGridCell cell, ICompiledFormula cformula)
		{
			var grid = this.Workbook;
			if (grid == null) return null;

			var formulaContext = (ReoScriptCompiledFormula)cformula;

			//var cell = formulaContext.Cell;
			var formula = formulaContext.Formula;

			List<ReferenceRange> referencedRanges = formulaContext.ReferencedCellOrRanges as List<ReferenceRange>;

			if (referencedRanges == null)
			{
				formulaContext.ReferencedCellOrRanges = referencedRanges = new List<ReferenceRange>();
			}
			else
			{
				referencedRanges.Clear();
			}
			
			// todo: improve: only create script context once
			//                when set data to a range
			var ctx = grid.Srm.CreateContext();

			// create an global variable getter
			ctx.ExternalVariableGetter = (id) =>
			{
#if FORMULA_CELL_INSTANCE_REF
				if (id.StartsWith("$"))
				{
					var address = id.Substring(1);
					if (ReoGridPos.IsValidAddress(address))
					{
						var pos = new ReoGridPos(address);
						return new RSCellObject(this, pos, cells[pos.Row, pos.Col]);
					}
					else
						return null;
				}
				else
#endif // FORMULA_CELL_INSTANCE_REF
				if (ReoGridPos.IsValidAddress(id))
				{
					var pos = new ReoGridPos(id);
					referencedRanges.Add(new ReferenceRange(grid, pos));

					var cell = grid.GetCell(pos);
					return cell == null ? 0 : cell.InnerData;
				}
				else
				{
					NamedRange range = grid.GetNamedRange(id);

					if (range != null)
					{
						referencedRanges.Add(range);

						var referencedCell = grid.GetCell(range.StartPos);
						return (referencedCell == null || referencedCell.InnerData == null) ? 0 : referencedCell.InnerData;
					}
					else
					{
						return null;
					}
				}
			};

			try
			{
				// preprocess range syntax
				formula = RGUtility.RangeReferenceRegex.Replace(formula, (m) =>
				{
					if (m.Groups["to_col"].Length > 0 && m.Groups["to_row"].Length > 0
						&& m.Groups["from_col"].Length > 0 && m.Groups["from_row"].Length > 0)
					{
						// range
						int fromRow = -1;
						if (!int.TryParse(m.Groups["from_row"].Value, out fromRow)) return "null";
						fromRow--;

						int toRow = -1;
						if (!int.TryParse(m.Groups["to_row"].Value, out toRow)) return "null";
						toRow--;

						int fromCol = RGUtility.GetNumberOfChar(m.Groups["from_col"].Value);
						int toCol = RGUtility.GetNumberOfChar(m.Groups["to_col"].Value);

						if (fromRow < 0) fromRow = 0;
						if (fromCol < 0) fromCol = 0;
						if (toRow > grid.RowCount - 1) toRow = grid.RowCount - 1;
						if (toCol > grid.RowCount - 1) toCol = grid.ColumnCount - 1;

						ReoGridRange range = new ReoGridRange(fromRow, fromCol, toRow - fromRow + 1, toCol - fromCol + 1);
						referencedRanges.Add(new ReferenceRange(grid, range));

						return string.Format("new Range({0},{1},{2},{3})", range.Row, range.Col, range.Rows, range.Cols);
					}
					else
						return m.Value;
				});

				return grid.Srm.CalcExpression(formula, ctx);
			}
			catch (ReoScriptException ex)
			{
				Logger.Log("formula", string.Format("error to evaluate formula: ", ex.Message));
				throw new FormulaEvalutionException(ex, "#ERR: " + ex.Message);
			}
		}
	}

	internal class ReoScriptCompiledFormula : ICompiledFormula
	{
		public string Formula { get; set; }

		public CompiledScript CompiledScript { get; set; }

		public ReoGridCell SourceCell { get; set; }

		public IEnumerable<ReferenceRange> ReferencedCellOrRanges { get; set; }

		public ReoScriptCompiledFormula(string formula, CompiledScript compiledScript)
		{
			this.Formula = formula;
			this.CompiledScript = compiledScript;
		}
	}
}
#endif // EX_SCRIPT
#endregion

#region ExcelCompatible Adapter

namespace unvell.ReoGrid.Adapter
{
	#region Excel Formula Provider

	// TODO

	#endregion
}

namespace unvell.ReoGrid.Formula.ExcelProvider
{
	public sealed class FormulaEvaluator
	{
		// TODO
	}
}

#endregion
#endif // NO_LONGER_SUPPORT_SINCE_V088
