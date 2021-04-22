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

using unvell.ReoGrid.Formula;

namespace unvell.ReoGrid
{
	partial class Worksheet
	{
		#region SetCellFormula

		/// <summary>
		/// Set formula into cell, calculate the value of formula and update referenced cells.
		/// </summary>
		/// <param name="addressOrName">Address or name to locate range on worksheet</param>
		/// <param name="formula">Formula to be set. Equal sign is not required.</param>
		public void SetCellFormula(string addressOrName, string formula)
		{
			if (CellPosition.IsValidAddress(addressOrName))
			{
				this.SetCellFormula(new CellPosition(addressOrName), formula);
				return;
			}

			if (this.registeredNamedRanges.TryGetValue(addressOrName, out var range))
			{
				this.SetCellFormula(range.StartPos, formula);
			}
			else
			{
				throw new InvalidAddressException(addressOrName);
			}
		}

		/// <summary>
		/// Set formula into cell, calculate the value of formula and update referenced cells.
		/// </summary>
		/// <param name="row">Number of row of cell</param>
		/// <param name="col">Number of column of cell</param>
		/// <param name="formula">Formula to be set. Equal sign is not required.</param>
		public void SetCellFormula(int row, int col, string formula)
		{
			this.SetCellFormula(CreateAndGetCell(row, col), formula);
		}

		/// <summary>
		/// Set formula into cell, calculate the value of formula and update referenced cells.
		/// </summary>
		/// <param name="pos">position of cell</param>
		/// <param name="formula">Formula to be set. Equal sign is not required.</param>
		public void SetCellFormula(CellPosition pos, string formula)
		{
			this.SetCellFormula(pos.Row, pos.Col, formula);
		}

		#endregion // SetCellFormula

		internal void SetCellFormula(Cell cell, string formula)
		{
			cell.InnerFormula = formula;

			this.ClearCellReferenceList(cell);

			// clear formula status
			cell.formulaStatus = FormulaStatus.Normal;

			STNode rootNode = null;
			
			try
			{
				rootNode = Parser.Parse(this.workbook, cell, formula);
			}
			catch(Exception ex)
			{
				cell.formulaStatus = FormulaStatus.SyntaxError;

				this.NotifyExceptionHappen(ex);
			}

			if (rootNode != null)
			{
				this.SetCellFormula(cell, rootNode);
			}
		}

		internal void SetCellFormula(Cell cell, STNode node)
		{
			// clear formula status
			cell.formulaStatus = FormulaStatus.Normal;

			// clear range references from this cell
			List<ReferenceRange> referencedRanges;

			if (this.formulaRanges.TryGetValue(cell, out referencedRanges))
			{
				referencedRanges.Clear();
			}
			else
			{
				referencedRanges = this.formulaRanges[cell] = new List<ReferenceRange>();
			}

			if (node != null)
			{
				// clear dependents arrows
				RemoveCellTraceDependents(cell);

				// clear precedents arrow
				RemoveCellTracePrecedents(cell);

				try
				{
					IterateToAddReference(cell, node, referencedRanges, true);

					cell.FormulaTree = node;
				}
				catch (CircularReferenceException crex)
				{
					cell.FormulaTree = null;
					cell.InnerFormula = null;

					cell.formulaStatus = FormulaStatus.CircularReference;
					this.NotifyExceptionHappen(crex);
				}
			}
		}

		#region Build Formula References

		private static void AddCellReferenceIntoList(List<ReferenceRange> referencedRanges, ReferenceRange range)
		{
			if (referencedRanges.All(rr => rr.Worksheet == range.Worksheet && !rr.Contains(range)))
			{
				referencedRanges.Add(range);
			}
		}

		internal static void IterateToAddReference(Cell cell, STNode node, List<ReferenceRange> referencedRanges, bool checkSelfRefer)
		{
			if (node == null) return;

			Worksheet worksheet;
			NamedRange range;
			RangePosition rangePos;
			STFunctionNode funNode;

			switch (node.Type)
			{
				case STNodeType.FUNCTION_CALL:
					funNode = (STFunctionNode)node;

					switch (funNode.Name)
					{
						default:
							if (funNode.Children != null)
							{
								foreach (var child in funNode.Children)
								{
									IterateToAddReference(cell, child, referencedRanges, checkSelfRefer);
								}
							}
							break;

						// self reference is allowed in following functions call
						case "ROW":
						case "COLUMN":
						case "ADDRESS":
						case "INDIRECT":
							if (funNode.Children != null)
							{
								foreach (var child in funNode.Children)
								{
									IterateToAddReference(cell, child, referencedRanges, false);
								}
							}
							break;
					}
					break;

				case STNodeType.CELL:
					var cellNode = (STCellNode)node;
					var pos = cellNode.Position;

					worksheet = cellNode.Worksheet;
					if (worksheet == null) worksheet = cell.Worksheet;

					if (checkSelfRefer && pos.Equals(cell.Position) && worksheet == cell.Worksheet)
					{
						throw new CircularReferenceException();
					}

					if (pos.Row >= worksheet.RowCount || pos.Col >= worksheet.ColumnCount)
					{
						throw new ArgumentOutOfRangeException();
					}

					var refCell = worksheet.CreateAndGetCell(pos);
					AddCellReferenceIntoList(referencedRanges, new ReferenceRange(worksheet, refCell, refCell));
					break;

				case STNodeType.RANGE:
					var rangeNode = (STRangeNode)node;

					worksheet = rangeNode.Worksheet;
					if (worksheet == null) worksheet = cell.Worksheet;

					rangePos = worksheet.FixRange(rangeNode.Range);

					if (checkSelfRefer && rangePos.Contains(cell.Position) && worksheet == cell.Worksheet)
					{
						throw new CircularReferenceException();
					}

					AddCellReferenceIntoList(referencedRanges, new ReferenceRange(worksheet, rangePos));
					break;

				case STNodeType.IDENTIFIER:
					var idNode = (STIdentifierNode)node;

					worksheet = idNode.Worksheet;
					if (worksheet == null) worksheet = cell.Worksheet;

					if (worksheet.TryGetNamedRange(idNode.Identifier, out range))
					{
						if (checkSelfRefer && range.Contains(cell.Position) && worksheet == cell.Worksheet)
						{
							throw new CircularReferenceException();
						}

						AddCellReferenceIntoList(referencedRanges, range);
					}
					break;

				default:
					if (node.Children != null)
					{
						foreach (var child in node.Children)
						{
							IterateToAddReference(cell, child, referencedRanges, checkSelfRefer);
						}
					}
					break;
			}
		}

		#endregion // Build Formula References

		#region DeleteCellFormula
		/// <summary>
		/// Delete formula from specified cell.
		/// </summary>
		/// <param name="addressOrName">Address or name used to locate cell on worksheet.</param>
		public void DeleteCellFormula(string addressOrName)
		{
			if (CellPosition.IsValidAddress(addressOrName))
			{
				var pos = new CellPosition(addressOrName);
				this.DeleteCellFormula(pos.Row, pos.Col);
			}
			else if (RangePosition.IsValidAddress(addressOrName))
			{
				this.ClearRangeContent(new RangePosition(addressOrName), CellElementFlag.Formula);
			}
			else
			{
				if (this.TryGetNamedRange(addressOrName, out var namedRange))
				{
					if (namedRange.IsMergedCell)
					{
						DeleteCellFormula(namedRange.StartPos);
					}
					else
					{
						ClearRangeContent(namedRange, CellElementFlag.Formula);
					}
				}
			}
		}

		/// <summary>
		/// Delete formula from specified cell.
		/// </summary>
		/// <param name="pos">Position to locate the cell on worksheet.</param>
		public void DeleteCellFormula(CellPosition pos)
		{
			DeleteCellFormula(pos.Row, pos.Col);
		}

		/// <summary>
		/// Delete formula from specified cell.
		/// </summary>
		/// <param name="row">Number of row of the cell to be deleted.</param>
		/// <param name="col">Number of column of the cell to be deleted.</param>
		public void DeleteCellFormula(int row, int col)
		{
			var cell = this.cells[row, col];
			if (cell != null)
			{
				DeleteCellFormula(cell);
			}
		}

		/// <summary>
		/// Delete formula from specified.
		/// </summary>
		/// <param name="cell">Instance of cell to be deleted.</param>
		public void DeleteCellFormula(Cell cell)
		{
			cell.InnerFormula = null;
			cell.InnerData = null;
			cell.InnerDisplay = null;

			cell.FontDirty = false;

#if WINFORM
			cell.RenderFont = null;
#endif // WINFORM
			RequestInvalidate();
		}
		#endregion // DeleteCellFormula

		#region RecalcCell

		/// <summary>
		/// Recalculate specified cell formula.
		/// </summary>
		/// <param name="pos">Address on worksheet to locate the cell.</param>
		public void RecalcCell(string address)
		{
			if (CellPosition.IsValidAddress(address))
			{
				this.RecalcCell(new CellPosition(address));
			}
			else if (this.TryGetNamedRangePosition(address, out var range))
			{
				this.RecalcCell(range.StartPos);
			}
			else
				throw new InvalidAddressException(address);
		}

		/// <summary>
		/// Recalculate specified cell formula.
		/// </summary>
		/// <param name="pos">Position of cell to be recalculated.</param>
		public void RecalcCell(CellPosition pos)
		{
			RecalcCell(pos.Row, pos.Col);
		}

		/// <summary>
		/// Recalculate specified cell formula.
		/// </summary>
		/// <param name="row">Index of row of cell.</param>
		/// <param name="col">Index of column of cell.</param>
		public void RecalcCell(int row, int col)
		{
			Cell cell = GetCell(row, col);
			if (cell == null) return;
			RecalcCell(cell);
		}

		/// <summary>
		/// Recalculate and get the value of formula stored in the specified cell.
		/// </summary>
		/// <param name="cell">Instance of cell to be recalculated.</param>
		/// <param name="dirtyCellStack">Dirty cell stack.</param>
		internal void RecalcCell(Cell cell, Stack<List<Cell>> dirtyCellStack = null)
		{
			if (!cell.HasFormula)
			{
				return;
			}

			object value = null;

			if (cell.formulaStatus != FormulaStatus.CircularReference
				&& cell.formulaStatus != FormulaStatus.SyntaxError)
			{
				try
				{
					value = Evaluator.CheckAndGetDefaultValue(cell, Evaluator.Evaluate(cell, cell.FormulaTree)).value;

					cell.formulaStatus = FormulaStatus.Normal;
				}
				catch (FormulaTypeMismatchException)
				{
					cell.formulaStatus = FormulaStatus.InvalidValue;
				}
				catch (FormulaNoNameException)
				{
					cell.formulaStatus = FormulaStatus.NameNotFound;
				}
				catch (FormulaParameterMismatchException)
				{
					cell.formulaStatus = FormulaStatus.MismatchedParameter;
				}
				catch (Exception ex)
				{
					cell.formulaStatus = FormulaStatus.UnspecifiedError;

					this.NotifyExceptionHappen(ex);
				}
			}

			UpdateCellData(cell, value, dirtyCellStack);
		}

#endregion // RecalcCell

#region Formula Reference List

		/// <summary>
		/// Suspend to update formula referenced cells.
		/// </summary>
		public void SuspendFormulaReferenceUpdates()
		{
			this.settings &= ~WorksheetSettings.Formula_AutoUpdateReferenceCell;
		}

		/// <summary>
		/// Resume to update formula referenced cells.
		/// </summary>
		public void ResumeFormulaReferenceUpdates()
		{
			this.settings |= WorksheetSettings.Formula_AutoUpdateReferenceCell;
		}

		private Dictionary<Cell, List<ReferenceRange>> formulaRanges = new Dictionary<Cell, List<ReferenceRange>>();

		private static void UpdateWorksheetReferencedFormulaCells(Worksheet worksheet, Cell fromCell,
			Stack<List<Cell>> dirtyCellStack = null)
		{
			List<Cell> dirtyCells = null;

			foreach (var range in worksheet.formulaRanges)
			{
				if (range.Value.Any(r => r.Contains(fromCell.InternalPos)))
				{
					if ((dirtyCells == null || !dirtyCells.Contains(range.Key))
						&& (dirtyCellStack == null || dirtyCellStack.All(s => !s.Contains(range.Key))))
					{
						if (dirtyCells == null)
						{
							dirtyCells = new List<Cell>();
						}

						dirtyCells.Add(range.Key);
					}
				}
			}

			if (dirtyCells != null && dirtyCells.Count > 0)
			{
				try
				{
					if (dirtyCellStack == null) dirtyCellStack = new Stack<List<Cell>>();

					dirtyCellStack.Push(dirtyCells);

					foreach (var dirtyCell in dirtyCells)
					{
						try
						{
							worksheet.RecalcCell(dirtyCell, dirtyCellStack);
						}
						catch (Exception ex)
						{
							worksheet.NotifyExceptionHappen(ex);
						}
					}
				}
				finally
				{
					dirtyCellStack.Pop();
				}
			}
		}

		internal void UpdateReferencedFormulaCells(Cell cell, Stack<List<Cell>> dirtyCellStack = null)
		{
			if (this.workbook == null)
			{
				UpdateWorksheetReferencedFormulaCells(this, cell, dirtyCellStack);
			}
			else
			{
				foreach (var worksheet in this.workbook.worksheets)
				{
					UpdateWorksheetReferencedFormulaCells(worksheet, cell, dirtyCellStack);
				}
			}
		}

		internal void ClearCellReferenceList(Cell cell)
		{
			// clear range references from this cell
			List<ReferenceRange> referencedRanges;

			if (this.formulaRanges.TryGetValue(cell, out referencedRanges))
			{
				referencedRanges.Clear();
			}
		}

		/// <summary>
		/// Get a list of referenced ranges from formula of specified cell
		/// </summary>
		/// <param name="address">address of cell to find its reference list</param>
		/// <returns>a list of referenced cell</returns>
		public List<ReferenceRange> GetCellFormulaReferenceRanges(string address)
		{
			if (CellPosition.IsValidAddress(address))
			{
				return this.GetCellFormulaReferenceRanges(new CellPosition(address));
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Get a list of referenced ranges from formula of specified cell
		/// </summary>
		/// <param name="pos">position of cell to find its reference list</param>
		/// <returns>a list of referenced cell</returns>
		public List<ReferenceRange> GetCellFormulaReferenceRanges(CellPosition pos)
		{
			Cell cell = this.cells[pos.Row, pos.Col];
			return cell == null ? null : GetCellFormulaReferenceRanges(cell);
		}

		/// <summary>
		/// Get a list of referenced ranges from formula of specified cell
		/// </summary>
		/// <param name="cell">cell to find its reference list</param>
		/// <returns>a list of referenced cell</returns>
		public List<ReferenceRange> GetCellFormulaReferenceRanges(Cell cell)
		{
			if (this.formulaRanges.TryGetValue(cell, out var referencedRangeList))
			{
				return referencedRangeList;
			}
			else
			{
				return null;
			}
		}

#endregion // Formula Reference List

#region Get Formula

		/// <summary>
		/// Get formula from cell specified by an address or registered name
		/// </summary>
		/// <param name="addressOrName">address or name used to locate a cell</param>
		/// <returns>formula as string returned from cell</returns>
		public string GetCellFormula(string addressOrName)
		{
			if (CellPosition.IsValidAddress(addressOrName))
			{
				return GetCellFormula(new CellPosition(addressOrName));
			}

			if (this.registeredNamedRanges.TryGetValue(addressOrName, out var namedRange))
			{
				return GetCellFormula(namedRange.StartPos);
			}

			throw new InvalidReferenceException(
				"Specified reference neither is an valid address nor name registered in spreadsheet.");
		}

		/// <summary>
		/// Get formula from cell specified by position
		/// </summary>
		/// <param name="pos">position to locate the cell to be get</param>
		/// <returns>formula as string returned from specified cell</returns>
		public string GetCellFormula(CellPosition pos)
		{
			var cell = GetCell(pos);
			return cell == null ? string.Empty : cell.InnerFormula;
		}

#endregion // Get Formula

#region Trace Arrow
		internal Dictionary<Cell, List<Cell>> traceDependentArrows = null;

		/// <summary>
		/// Show cell formula precedent trace lines on worksheet
		/// </summary>
		/// <param name="address">address to trace the cell</param>
		/// <returns>true if trace is successful</returns>
		public bool TraceCellPrecedents(string address)
		{
			return CellPosition.IsValidAddress(address) ? TraceCellPrecedents(new CellPosition(address)) : false;
		}

		/// <summary>
		/// Show cell formula precendent trace lines on worksheet
		/// </summary>
		/// <param name="pos">position to trace the cell</param>
		/// <returns>true if trace is successful</returns>
		public bool TraceCellPrecedents(CellPosition pos)
		{
			Cell cell = GetCell(pos);
			return cell == null ? false : TraceCellPrecedents(cell);
		}

		internal bool TraceCellPrecedents(Cell cell)
		{
			if (!cell.HasFormula) return false;

			var rfs = GetCellFormulaReferenceRanges(cell);

			if (rfs == null)
			{
				return false;
			}

			foreach (var rf in rfs)
			{
				AddTraceLine(GetCell(rf.StartPos), cell);
			}

			this.RequestInvalidate();

			return true;
		}

		/// <summary>
		/// Remove cell trace precedents from specified address
		/// </summary>
		/// <param name="address">address to remove trace precedents</param>
		/// <returns></returns>
		public bool RemoveCellTracePrecedents(string address)
		{
			return CellPosition.IsValidAddress(address) ? RemoveCellTracePrecedents(new CellPosition(address)) : false;
		}

		/// <summary>
		/// Remove cell formula precedent trace lines from worksheet
		/// </summary>
		/// <param name="pos">position to locate the cell remove trace lines</param>
		/// <returns>true if trace lines has been removed successfully</returns>
		public bool RemoveCellTracePrecedents(CellPosition pos)
		{
			Cell cell = GetCell(pos);
			return cell == null ? false : RemoveCellTracePrecedents(cell);
		}

		/// <summary>
		/// Remove cell formula precedent trace lines from worksheet
		/// </summary>
		/// <param name="cell">cell to be checked and removed</param>
		/// <returns>true if trace lines has been removed successfully</returns>
		public bool RemoveCellTracePrecedents(Cell cell)
		{
			bool found = false;
			List<Cell> emptyPrecedentCells = null;

			if (this.traceDependentArrows != null)
			{
				foreach (var dependentCell in this.traceDependentArrows.Keys)
				{
					var lines = this.traceDependentArrows[dependentCell];

					var traceLine = lines.FirstOrDefault(l => l == cell);

					if (traceLine != null)
					{
						lines.Remove(traceLine);

						found = true;

						if (lines.Count == 0)
						{
							if (emptyPrecedentCells == null) emptyPrecedentCells = new List<Cell>();
							emptyPrecedentCells.Add(dependentCell);
						}
					}
				}

				if (emptyPrecedentCells != null)
				{
					// remove empty precedent cells
					foreach (var removeCell in emptyPrecedentCells)
					{
						this.traceDependentArrows.Remove(removeCell);
					}
				}
			}

			if (found)
			{
				this.RequestInvalidate();
			}

			return found;
		}

		/// <summary>
		/// Start trace dependents to specified address
		/// </summary>
		/// <param name="address">target address to trace dependents</param>
		/// <returns>true if trace arrow added</returns>
		public bool TraceCellDependents(string address)
		{
			return CellPosition.IsValidAddress(address) ? TraceCellDependents(new CellPosition(address)) : false;
		}

		/// <summary>
		/// Start trace dependents to specified position
		/// </summary>
		/// <param name="pos">target position to trace dependents</param>
		/// <returns>true if trace arrow added</returns>
		public bool TraceCellDependents(CellPosition pos)
		{
			Cell cell = GetCell(pos);
			return cell == null ? false : TraceCellPrecedents(cell);
		}

		/// <summary>
		/// Start trace dependents to specified cell
		/// </summary>
		/// <param name="cell">target cell to trace dependents</param>
		/// <returns>true if trace arrow added</returns>
		public bool TraceCellDependents(Cell cell)
		{
			if (!cell.HasFormula) return false;

			if (traceDependentArrows.Keys.Contains(cell))
			{
				// already on trace
				return false;
			}

			bool found = false;

			foreach (var dependentCell in this.formulaRanges.Keys)
			{
				var refRanges = formulaRanges[dependentCell];

				if (refRanges.Any(rr => rr.Contains(cell.InternalPos)))
				{
					AddTraceLine(cell, dependentCell);
					found = true;
				}
			}

			if (found)
			{
				this.RequestInvalidate();
			}

			return true;
		}

		/// <summary>
		/// Remove all trace dependent arrows from specified address
		/// </summary>
		/// <param name="address">address to remove cell dependents</param>
		/// <returns></returns>
		public bool RemoveCellTraceDependents(string address)
		{
			return CellPosition.IsValidAddress(address) ? RemoveCellTraceDependents(new CellPosition(address)) : false;
		}

		/// <summary>
		/// Remove all trace dependent arrows from specified position
		/// </summary>
		/// <param name="pos">position to remove cell dependents</param>
		/// <returns></returns>
		public bool RemoveCellTraceDependents(CellPosition pos)
		{
			Cell cell = GetCell(pos);
			return cell == null ? false : RemoveCellTraceDependents(cell);
		}

		/// <summary>
		/// Remove all trace dependent arrows from specified cell
		/// </summary>
		/// <param name="cell">cell to remove trace dependents</param>
		/// <returns></returns>
		public bool RemoveCellTraceDependents(Cell cell)
		{
			if (this.traceDependentArrows != null
				&& this.traceDependentArrows.Keys.Contains(cell))
			{
				this.traceDependentArrows.Remove(cell);
				this.RequestInvalidate();
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Add trace arrow between specified two cells
		/// </summary>
		/// <param name="fromCell">Trace arrow begin from this cell</param>
		/// <param name="toCell">Trace arrow end to this cell</param>
		/// <returns>true if </returns>
		internal bool AddTraceLine(Cell fromCell, Cell toCell)
		{
			if (traceDependentArrows == null)
			{
				traceDependentArrows = new Dictionary<Cell, List<Cell>>();
			}

			if (traceDependentArrows.Keys.Contains(fromCell))
			{
				return false;
			}

			List<Cell> lines;

			if (traceDependentArrows.TryGetValue(fromCell, out lines))
			{
				if (lines.Any(l => l.InternalPos == toCell.InternalPos))
				{
					return false;
				}
			}
			else
			{
				lines = traceDependentArrows[fromCell] = new List<Cell>();
			}

			lines.Add(toCell);

			RequestInvalidate();

			return true;
		}

		/// <summary>
		/// Remove trace arrow between specified two cells
		/// </summary>
		/// <param name="fromCell">Trace arrow begin from this cell</param>
		/// <param name="toCell">Trace arrow end to this cell</param>
		/// <returns></returns>
		internal bool RemoveTraceLine(Cell fromCell, Cell toCell)
		{
			if (!this.traceDependentArrows.Keys.Contains(fromCell))
			{
				return false;
			}

			if (traceDependentArrows.TryGetValue(fromCell, out var lines))
			{
				var traceLine = lines.FirstOrDefault(l => l.InternalPos == toCell.InternalPos);
				if (traceLine != null)
				{
					lines.Remove(traceLine);
				}

				if (lines.Count == 0)
				{
					this.traceDependentArrows.Remove(fromCell);
				}

				RequestInvalidate();

				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Remove all trace arrows from specified address
		/// </summary>
		/// <param name="address">Address of position to locate a cell</param>
		public void RemoveCellAllTraceArrows(string address)
		{
			if (CellPosition.IsValidAddress(address))
			{
				RemoveCellAllTraceArrows(new CellPosition(address));
			}
		}

		/// <summary>
		/// Remove all trace arrows from specified position
		/// </summary>
		/// <param name="pos">Position to locate a cell</param>
		public void RemoveCellAllTraceArrows(CellPosition pos)
		{
			RemoveCellTraceDependents(pos);
			RemoveCellTracePrecedents(pos);
		}

		/// <summary>
		/// Remove all trace arrows from specified cell
		/// </summary>
		/// <param name="cell">cell instance to remove trace arrows</param>
		internal bool RemoveCellAllTraceArrows(Cell cell)
		{
			RemoveCellTraceDependents(cell);
			RemoveCellTracePrecedents(cell);
			return true;
		}

		/// <summary>
		/// Remove all trace arrows from specified range of address
		/// </summary>
		/// <param name="address">address to locate a range</param>
		/// <returns>true if address is valid</returns>
		public bool RemoveRangeAllTraceArrows(string address)
		{
			if (RangePosition.IsValidAddress(address))
			{
				RemoveRangeAllTraceArrows(new RangePosition(address));
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Remove all trace arrows from specified range
		/// </summary>
		/// <param name="range"></param>
		public void RemoveRangeAllTraceArrows(RangePosition range)
		{
			this.IterateCells(range, (r, c, cell) => RemoveCellAllTraceArrows(cell));
		}

		/// <summary>
		/// Check whether specified cell currently in trace precedents
		/// </summary>
		/// <param name="pos">a position to locate cell</param>
		/// <returns>true if cell in trace precedents</returns>
		public bool IsCellInTracePrecedents(CellPosition pos)
		{
			if (this.traceDependentArrows == null) return false;

			return this.traceDependentArrows.Values.Any(ls => ls.Any(l => l.InternalPos == pos));
		}

		/// <summary>
		/// Check whether specified cell currently in trace dependents
		/// </summary>
		/// <param name="pos">a position to locate cell</param>
		/// <returns>true if cell in trace dependents</returns>
		public bool IsCellInTraceDependents(CellPosition pos)
		{
			if (this.traceDependentArrows == null) return false;

			return this.traceDependentArrows.Keys.Any(c => c.InternalPos == pos);
		}

		/// <summary>
		/// Retrieve all cells which in trace precedents or dependents
		/// </summary>
		/// <returns>list of cells</returns>
		public IEnumerable<Cell> GetAllTraceDependentCells()
		{
			return this.traceDependentArrows.Keys;
		}

#endregion // Trace Lines

#region Recalculate
		/// <summary>
		/// Recalculate entire worksheet.
		/// </summary>
		public void Recalculate()
		{
			var dirtyCells = new Stack<List<Cell>>();

			var entireRange = new RangePosition(0, 0, this.MaxContentRow + 1, this.MaxContentCol + 1);

			this.IterateCells(entireRange, (row, col, cell) =>
				{
					if (cell.HasFormula)
					{
						RecalcCell(cell, dirtyCells);
					}
					return true;
				});
		}

		///// <summary>
		///// This event will be invoked when worksheet is recalculated after any cell's value change.
		///// </summary>
		//public event EventHandler Recalculated;
#endregion // Recalculate
	}

	partial class Cell
	{
		/// <summary>
		/// Show or hide trace formula percendents on spreadsheet.
		/// </summary>
		public bool TraceFormulaPrecedents
		{
			get
			{
				return Worksheet.IsCellInTracePrecedents(this.InternalPos);
			}
			set
			{
				if (this.Worksheet != null)
				{
					if (value)
					{
						Worksheet.TraceCellPrecedents(this);
					}
					else
					{
						Worksheet.RemoveCellTracePrecedents(this);
					}
				}
			}
		}

		/// <summary>
		/// Show or hide trace formula dependents on spreadsheet.
		/// </summary>
		public bool TraceFormulaDependents
		{
			get
			{
				if (Worksheet == null)
					return false;
				else
					return Worksheet.IsCellInTracePrecedents(this.InternalPos);
			}
			set
			{
				if (this.Worksheet != null)
				{
					if (value)
					{
						Worksheet.TraceCellDependents(this);
					}
					else
					{
						Worksheet.RemoveCellTraceDependents(this);
					}
				}
			}
		}
	}
}

#endif // FORMULA
