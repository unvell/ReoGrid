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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using unvell.Common;
using unvell.ReoGrid.Data;
using unvell.ReoGrid.DataFormat;

#if OUTLINE
using unvell.ReoGrid.Outline;
#endif // OUTLINE

namespace unvell.ReoGrid.Actions
{
	#region Workbook Actions

	/// <summary>
	/// Represents an action of workbook.
	/// </summary>
	public abstract class WorkbookAction : IUndoableAction
	{
		/// <summary>
		/// Get the workbook instance.
		/// </summary>
		public IWorkbook Workbook { get; internal set; }

		/// <summary>
		/// Create workbook action with specified workbook instance.
		/// </summary>
		/// <param name="workbook"></param>
		public WorkbookAction(IWorkbook workbook = null)
		{
			this.Workbook = workbook;
		}

		/// <summary>
		/// Do this action.
		/// </summary>
		public abstract void Do();

		/// <summary>
		/// Undo this action.
		/// </summary>
		public abstract void Undo();

		/// <summary>
		/// Redo this action.
		/// </summary>
		public virtual void Redo()
		{
			this.Do();
		}

		/// <summary>
		/// Get the friendly name of this action.
		/// </summary>
		/// <returns></returns>
		public abstract string GetName();
	}

	/// <summary>
	/// Action for inserting worksheet
	/// </summary>
	public class InsertWorksheetAction : WorkbookAction
	{
		/// <summary>
		/// Number of worksheet
		/// </summary>
		public int Index { get; private set; }

		/// <summary>
		/// Worksheet instance
		/// </summary>
		public Worksheet Worksheet { get; private set; }

		/// <summary>
		/// Create this action to insert worksheet
		/// </summary>
		/// <param name="index">Number of worksheet</param>
		/// <param name="worksheet">Worksheet instance</param>
		public InsertWorksheetAction(int index, Worksheet worksheet)
		{
			this.Index = index;
			this.Worksheet = worksheet;
		}

		/// <summary>
		/// Do this action to insert worksheet
		/// </summary>
		public override void Do()
		{
			this.Workbook.InsertWorksheet(this.Index, this.Worksheet);
		}

		/// <summary>
		/// Undo this action to remove the inserted worksheet
		/// </summary>
		public override void Undo()
		{
			this.Workbook.RemoveWorksheet(this.Index);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Insert Worksheet: " + this.Worksheet.Name;
		}
	}

	/// <summary>
	/// Action for removing worksheet
	/// </summary>
	public class RemoveWorksheetAction : WorkbookAction
	{
		/// <summary>
		/// Number of worksheet
		/// </summary>
		public int Index { get; private set; }

		/// <summary>
		/// Worksheet instance
		/// </summary>
		public Worksheet Worksheet { get; private set; }

		/// <summary>
		/// Create this action to insert worksheet
		/// </summary>
		/// <param name="index">Number of worksheet</param>
		/// <param name="worksheet">Worksheet instance</param>
		public RemoveWorksheetAction(int index, Worksheet worksheet)
		{
			this.Index = index;
			this.Worksheet = worksheet;
		}

		/// <summary>
		/// Do this action to remove worksheet
		/// </summary>
		public override void Do()
		{
			this.Workbook.RemoveWorksheet(this.Index);
		}

		/// <summary>
		/// Undo this action to restore the removed worksheet
		/// </summary>
		public override void Undo()
		{
			this.Workbook.InsertWorksheet(this.Index, this.Worksheet);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Remove Worksheet: " + this.Worksheet.Name;
		}
	}
	#endregion // Workbook Actions

	// Common Actions & ActionGroups
	#region Worksheet Actions

	/// <summary>
	/// Base action for all actions that are used for worksheet operations.
	/// </summary>
	public abstract class BaseWorksheetAction : IUndoableAction
	{
		/// <summary>
		/// Instance for the grid control will be setted before action performed.
		/// </summary>
		public Worksheet Worksheet { get; internal set; }

	/// <summary>
		/// Do this action.
		/// </summary>
		public abstract void Do();

		/// <summary>
		/// Undo this action.
		/// </summary>
		public abstract void Undo();

		/// <summary>
		/// Redo this action.
		/// </summary>
		public virtual void Redo()
		{
			this.Do();
		}
	
		/// <summary>
		/// Get friendly name of this action.
		/// </summary>
		/// <returns>Get friendly name of this action.</returns>
		public abstract string GetName();
	}

	/// <summary>
	/// The action group is one type of RGAction to support Do/Undo/Redo a series of actions.
	/// </summary>
	public class WorksheetActionGroup : BaseWorksheetAction
	{
		/// <summary>
		/// Actions stored in this list will be Do/Undo/Redo together
		/// </summary>
		public List<BaseWorksheetAction> Actions { get; set; }

		/// <summary>
		/// Create instance for RGActionGroup
		/// </summary>
		public WorksheetActionGroup()
		{
			this.Actions = new List<BaseWorksheetAction>();
		}

		/// <summary>
		/// Do all actions stored in this action group
		/// </summary>
		public override void Do()
		{
			foreach (var action in Actions)
			{
				action.Worksheet = this.Worksheet;
				action.Do();
			}
		}

		/// <summary>
		/// Undo all actions stored in this action group
		/// </summary>
		public override void Undo()
		{
			for (int i = Actions.Count - 1; i >= 0; i--)
			{
				var action = Actions[i];

				action.Worksheet = this.Worksheet;
				action.Undo();
			}
		}

		/// <summary>
		/// Get friendly name of this action group
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "ReoGrid Action Group";
		}
	}

	/// <summary>
	/// Reusable action is one type of RGAction to support repeat operation
	/// to a specified range. It is good practice to make all actions with 
	/// a range target to inherit from this class.
	/// </summary>
	public abstract class WorksheetReusableAction : BaseWorksheetAction
	{
		/// <summary>
		/// Range to be appiled this action
		/// </summary>
		public RangePosition Range { get; set; }

		protected WorksheetReusableAction() { }

		/// <summary>
		/// Constructor of RGReusableAction 
		/// </summary>
		/// <param name="range">Range to be applied this action</param>
		public WorksheetReusableAction(RangePosition range)
		{
			this.Range = range;
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public abstract WorksheetReusableAction Clone(RangePosition range);
	}

	/// <summary>
	/// Reusable action group is one type of RGActionGroup to support repeat 
	/// operation to a specified range. It is good practice to make all reusable 
	/// action groups to inherit from this class.
	/// </summary>
	public class WorksheetReusableActionGroup : WorksheetReusableAction
	{
		private List<WorksheetReusableAction> actions;

		/// <summary>
		/// All reusable actions stored in this list will be performed together.
		/// </summary>
		public List<WorksheetReusableAction> Actions
		{
			get { return actions; }
			set { actions = value; }
		}

		/// <summary>
		/// Constructor of ReusableActionGroup
		/// </summary>
		/// <param name="range">Range to be appiled this action group</param>
		public WorksheetReusableActionGroup(RangePosition range)
			: base(range)
		{
			this.actions = new List<WorksheetReusableAction>();
		}

		/// <summary>
		/// Constructor of ReusableActionGroup
		/// </summary>
		/// <param name="range">Range to be appiled this action group</param>
		/// <param name="actions">Action list to be performed together</param>
		public WorksheetReusableActionGroup(RangePosition range, List<WorksheetReusableAction> actions)
			: base(range)
		{
			this.actions = actions;
		}

		private bool first = true;

		/// <summary>
		/// Do all actions stored in this action group
		/// </summary>
		public override void Do()
		{
			if (first)
			{
				for (int i = 0; i < actions.Count; i++)
					actions[i].Worksheet = this.Worksheet;
				first = false;
			}

			for (int i = 0; i < actions.Count; i++)
			{
				actions[i].Do();
			}
		}

		/// <summary>
		/// Undo all actions stored in this action group
		/// </summary>
		public override void Undo()
		{
			for (int i = actions.Count - 1; i >= 0; i--)
			{
				actions[i].Undo();
			}
		}

		/// <summary>
		/// Get friendly name of this action group
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Multi-Aciton[" + actions.Count + "]";
		}

		/// <summary>
		/// Create cloned reusable action group from this action group
		/// </summary>
		/// <param name="range">Specified new range to apply this action group</param>
		/// <returns>New reusable action group cloned from this action group</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			List<WorksheetReusableAction> clonedActions = new List<WorksheetReusableAction>();

			foreach (WorksheetReusableAction action in actions)
			{
				clonedActions.Add(action.Clone(range));
			}

			return new WorksheetReusableActionGroup(range, clonedActions);
		}
	}

	#endregion // Worksheet Actions

	// Style Editing Actions
	#region Actions - Style

	#region SetRangeStyleAction
	/// <summary>
	/// Action for set styles to specified range
	/// </summary>
	public class SetRangeStyleAction : WorksheetReusableAction
	{
		private WorksheetRangeStyle style;

		/// <summary>
		/// Styles to be set
		/// </summary>
		public WorksheetRangeStyle Style { get { return style; } set { style = value; } }

		private WorksheetRangeStyle backupRootStyle = null;
		private WorksheetRangeStyle[] backupRowStyles = null;
		private WorksheetRangeStyle[] backupColStyles = null;
		private PartialGrid backupData = null;
		private bool isFullColSelected = false;
		private bool isFullRowSelected = false;
		private bool isFullGridSelected = false;

		/// <summary>
		/// Create an action to set styles into specified range
		/// </summary>
		/// <param name="row">Zero-based number of start row</param>
		/// <param name="col">Zero-based number of start column</param>
		/// <param name="rows">Number of rows in the range</param>
		/// <param name="cols">Number of columns in the range</param>
		/// <param name="style">Styles to be set</param>
		public SetRangeStyleAction(int row, int col, int rows, int cols, WorksheetRangeStyle style)
			: this(new RangePosition(row, col, rows, cols), style)
		{
		}

		/// <summary>
		/// Create an action to set styles into specified range
		/// </summary>
		/// <param name="address">Address to locate the cell or range on spreadsheet (Cannot specify named range for this method)</param>
		/// <param name="style">Styles to be set</param>
		/// <exception cref="InvalidAddressException">Throw if specified address or name is invalid</exception>
		public SetRangeStyleAction(string address, WorksheetRangeStyle style)
		{
			if (RangePosition.IsValidAddress(address))
			{
				this.Range = new RangePosition(address);
			}
			else
				throw new InvalidAddressException(address);

			this.style = style;
		}


		/// <summary>
		/// Create an action that perform set styles to specified range
		/// </summary>
		/// <param name="range">Range to be appiled this action</param>
		/// <param name="style">Style to be set to specified range</param>
		public SetRangeStyleAction(RangePosition range, WorksheetRangeStyle style)
			: base(range)
		{
			this.style = new WorksheetRangeStyle(style);
		}

		private RangePosition affectedRange;

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			backupData = Worksheet.GetPartialGrid(Range);

			affectedRange = this.Worksheet.FixRange(this.Range);

			int r1 = Range.Row;
			int c1 = Range.Col;
			int r2 = Range.EndRow;
			int c2 = Range.EndCol;

			int rowCount = Worksheet.RowCount;
			int colCount = Worksheet.ColumnCount;

			isFullColSelected = affectedRange.Rows == rowCount;
			isFullRowSelected = affectedRange.Cols == colCount;
			isFullGridSelected = isFullRowSelected && isFullColSelected;

			// update default styles
			if (isFullGridSelected)
			{
				backupRootStyle = WorksheetRangeStyle.Clone(Worksheet.RootStyle);

				this.backupRowStyles = new WorksheetRangeStyle[rowCount];
				this.backupColStyles = new WorksheetRangeStyle[colCount];

				// remote styles if it is already setted in full-row
				for (int r = 0; r < rowCount; r++)
				{
					RowHeader rowHead = Worksheet.RetrieveRowHeader(r);
					if (rowHead != null && rowHead.InnerStyle != null)
					{
						this.backupRowStyles[r] = WorksheetRangeStyle.Clone(rowHead.InnerStyle);
					}
				}

				// remote styles if it is already setted in full-col
				for (int c = 0; c < colCount; c++)
				{
					ColumnHeader colHead = Worksheet.RetrieveColumnHeader(c);
					if (colHead != null && colHead.InnerStyle != null)
					{
						this.backupColStyles[c] = WorksheetRangeStyle.Clone(colHead.InnerStyle);
					}
				}
			}
			else if (isFullRowSelected)
			{
				backupRowStyles = new WorksheetRangeStyle[r2 - r1 + 1];
				for (int r = r1; r <= r2; r++)
				{
					backupRowStyles[r - r1] = WorksheetRangeStyle.Clone(Worksheet.RetrieveRowHeader(r).InnerStyle);
				}
			}
			else if (isFullColSelected)
			{
				backupColStyles = new WorksheetRangeStyle[c2 - c1 + 1];
				for (int c = c1; c <= c2; c++)
				{
					backupColStyles[c - c1] = WorksheetRangeStyle.Clone(Worksheet.RetrieveColumnHeader(c).InnerStyle);
				}
			}

			Worksheet.SetRangeStyles(affectedRange, style);
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			if (this.isFullGridSelected)
			{
				Worksheet.RootStyle = WorksheetRangeStyle.Clone(backupRootStyle);

				// remote styles if it is already setted in full-row
				for (int r = 0; r < backupRowStyles.Length; r++)
				{
					if (backupRowStyles[r] != null)
					{
						RowHeader rowHead = Worksheet.RetrieveRowHeader(r);
						if (rowHead != null)
						{
							rowHead.InnerStyle = WorksheetRangeStyle.Clone(backupRowStyles[r]);
							//rowHead.InnerStyle.Flag = PlainStyleFlag.None;
							//rowHead.Style.BackColor = System.Drawing.Color.Empty;
						}
					}
				}

				// remote styles if it is already setted in full-col
				for (int c = 0; c < backupColStyles.Length; c++)
				{
					if (backupColStyles[c] != null)
					{
						ColumnHeader colHead = Worksheet.RetrieveColumnHeader(c);
						if (colHead != null)
						{
							colHead.InnerStyle = WorksheetRangeStyle.Clone(backupColStyles[c]);
							//colHead.InnerStyle.Flag = PlainStyleFlag.None;
							//colHead.Style.BackColor = System.Drawing.Color.Empty;
						}
					}
				}
			}
			else if (isFullRowSelected)
			{
				for (int r = 0; r < backupRowStyles.Length; r++)
				{
					RowHeader rowHead = Worksheet.RetrieveRowHeader(r + affectedRange.Row);
					if (rowHead != null) rowHead.InnerStyle = backupRowStyles[r];
				}
			}
			else if (isFullColSelected)
			{
				for (int c = 0; c < backupColStyles.Length; c++)
				{
					ColumnHeader colHead = Worksheet.RetrieveColumnHeader(c + affectedRange.Col);
					if (colHead != null) colHead.InnerStyle = backupColStyles[c];
				}
			}

			Worksheet.SetPartialGrid(affectedRange, backupData, PartialGridCopyFlag.CellStyle);
		}

		/// <summary>
		/// Returns friendly name for this action.
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Set Style";
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new SetRangeStyleAction(range, style);
		}
	}
	#endregion // SetRangeStyleAction

	#region RemoveRangeStyleAction
	/// <summary>
	/// Remove style from specified range action
	/// </summary>
	public class RemoveRangeStyleAction : WorksheetReusableAction
	{
		private PlainStyleFlag flag;

		/// <summary>
		/// Style flag indicates what type of style to be handled.
		/// </summary>
		public PlainStyleFlag Flag { get { return flag; } set { flag = value; } }

		private PartialGrid backupData;

		/// <summary>
		/// Create instance for action to remove style from specified range.
		/// </summary>
		/// <param name="range">Styles from this specified range to be removed</param>
		/// <param name="flag">Style flag indicates what type of style should be removed</param>
		public RemoveRangeStyleAction(RangePosition range, PlainStyleFlag flag)
			: base(range)
		{
			this.flag = flag;
		}

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			backupData = Worksheet.GetPartialGrid(Range);
			Worksheet.RemoveRangeStyles(Range, flag);
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			Worksheet.SetPartialGrid(Range, backupData, PartialGridCopyFlag.CellStyle);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Delete Style";
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new RemoveRangeStyleAction(Range, flag);
		}
	}
	#endregion // RemoveRangeStyleAction

	#region StepRangeFontSizeAction
	/// <summary>
	/// Make font size larger or smaller action.
	/// </summary>
	public class StepRangeFontSizeAction : WorksheetReusableAction
	{
		/// <summary>
		/// True if this action making font size larger.
		/// </summary>
		public bool Enlarge { get; set; }

		/// <summary>
		/// Create instance for this action with specified range and enlarge flag.
		/// </summary>
		/// <param name="range">Specified range to apply this action</param>
		/// <param name="enlarge">True to set text larger, false to set smaller</param>
		public StepRangeFontSizeAction(RangePosition range, bool enlarge)
			: base(range)
		{
			this.Enlarge = enlarge;
		}

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			Worksheet.StepRangeFont(Range, size =>
			{
				return Enlarge ?
						(size >= Toolkit.FontSizeList.Max()) ? size : Toolkit.FontSizeList.Where(f => f > size).Min()
						: (size <= Toolkit.FontSizeList.Min()) ? size : Toolkit.FontSizeList.Where(f => f < size).Max();
			});
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			Worksheet.StepRangeFont(Range, size =>
			{
				return !Enlarge ?
						(size >= Toolkit.FontSizeList.Max()) ? size : Toolkit.FontSizeList.Where(f => f > size).Min()
						: (size <= Toolkit.FontSizeList.Min()) ? size : Toolkit.FontSizeList.Where(f => f < size).Max();
			});
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return Enlarge ? "Make Text Bigger" : "Make Text Smaller";
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new StepRangeFontSizeAction(range, Enlarge);
		}
	}
	#endregion // RGStepRangeFontSizeAction

	#endregion // Actions - Style

	// Headers of Row and Column Editing Actions
	#region Actions - Row & Column Header

	#region InsertRowsAction
	/// <summary>
	/// Insert rows action
	/// </summary>
	public class InsertRowsAction : WorksheetReusableAction
	{
		/// <summary>
		/// Index of row to insert empty rows. Set to Control.RowCount to 
		/// append columns at end of rows.
		/// </summary>
		public int Row { get; set; }

		/// <summary>
		/// Number of empty rows to be inserted
		/// </summary>
		public int Count { get; set; }

		/// <summary>
		/// Create instance for InsertRowsAction
		/// </summary>
		/// <param name="row">Index of row to insert</param>
		/// <param name="count">Number of rows to be inserted</param>
		public InsertRowsAction(int row, int count)
			: base(RangePosition.Empty)
		{
			this.Row = row;
			this.Count = count;
		}

		int insertedRow = -1;

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			insertedRow = Row;
			Worksheet.InsertRows(Row, Count);
			Range = new RangePosition(Row, 0, Count, Worksheet.ColumnCount);
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			Worksheet.DeleteRows(insertedRow, Count);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Insert Rows";
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new InsertRowsAction(range.Row, range.Rows);
		}
	}
	#endregion // InsertRowsAction

	#region InsertColumnsAction
	/// <summary>
	/// Insert columns action
	/// </summary>
	public class InsertColumnsAction : WorksheetReusableAction
	{
		/// <summary>
		/// Index of column to insert new columns. Set to Control.ColCount to
		/// append columns at end of columns.
		/// </summary>
		public int Column { get; set; }

		/// <summary>
		/// Number of columns to be inserted
		/// </summary>
		public int Count { get; set; }

		/// <summary>
		/// Create instance for InsertColumnsAction
		/// </summary>
		/// <param name="column">Index of column to insert</param>
		/// <param name="count">Number of columns to be insertted</param>
		public InsertColumnsAction(int column, int count)
			: base(RangePosition.Empty)
		{
			this.Column = column;
			this.Count = count;
		}

		int insertedCol = -1;

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			insertedCol = Column;
			Worksheet.InsertColumns(Column, Count);
			Range = new RangePosition(0, Column, Worksheet.RowCount, Count);
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			Worksheet.DeleteColumns(insertedCol, Count);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Insert Columns";
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new InsertColumnsAction(range.Col, range.Cols);
		}
	}
	#endregion // InsertColumnsAction

	#region RemoveRowsAction
	internal struct BackupRangeInfo
	{
		internal int start;
		internal int count;

		public BackupRangeInfo(int start, int count)
		{
			this.start = start;
			this.count = count;
		}
	}

	/// <summary>
	/// Remove rows actions
	/// </summary>
	public class RemoveRowsAction : WorksheetReusableAction
	{
		// main backups 
		private PartialGrid backupData = null;
		private int[] backupHeights;
		
		// additional backups
		internal List<NamedRange> deletedNamedRanges;
		internal List<HighlightRange> deletedHighlightRanges;
		internal Dictionary<NamedRange, BackupRangeInfo> changedNamedRange;
		internal Dictionary<HighlightRange, BackupRangeInfo> changedHighlightRanges;

#if OUTLINE
		internal List<IReoGridOutline> deletedOutlines;
		internal Dictionary<IReoGridOutline, BackupRangeInfo> changedOutlines;
#endif // OUTLINE

		/// <summary>
		/// Create instance for RemoveRowsAction
		/// </summary>
		/// <param name="row">Index of row start to remove</param>
		/// <param name="rows">Number of rows to be removed</param>
		public RemoveRowsAction(int row, int rows)
			: base(new RangePosition(row, 0, rows, -1))
		{ }

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			if (Range.Rows == -1)
			{
				Logger.Log("remove rows", "attempt to remove all columns but grid must have one column, operation aborted.");
				return;
			}

			backupHeights = new int[Range.Rows];

			for (int i = Range.Row; i <= Range.EndRow; i++)
			{
				backupHeights[i - Range.Row] = Worksheet.RetrieveRowHeader(i).InnerHeight;
			}

			Range = Worksheet.FixRange(Range);
			backupData = Worksheet.GetPartialGrid(Range);
			Debug.Assert(backupData != null);
			Worksheet.DeleteRows(Range.Row, Range.Rows, this);
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			if (Range.Rows == -1)
			{
				Logger.Log("remove rows", "attempt to undo removing all rows from the worksheet must it must be have at least one row, operation aborted.");
				return;
			}
			
			Worksheet.InsertRows(Range.Row, Range.Rows);

			if (backupData == null)
			{
#if DEBUG
				Logger.Log("remove rows", "no backup data");
				Debug.Assert(false, "why no backup data here?");
#else
				return;
#endif // DEBUG
			}

			// restore deleted data
			Worksheet.SetPartialGrid(Range, backupData);

			#region Restore Outlines
#if OUTLINE
			// restore outlines
			if (this.changedOutlines != null)
			{
				foreach (var changedOutlineInfo in this.changedOutlines)
				{
					changedOutlineInfo.Key.Start = changedOutlineInfo.Value.start;
					changedOutlineInfo.Key.Count = changedOutlineInfo.Value.count;
				}
			}

			if (this.deletedOutlines != null)
			{
				foreach (var outline in this.deletedOutlines)
				{
					Worksheet.GroupRows(outline.Start, outline.Count);
				}
			}
#endif // OUTLINE
			#endregion // Restore Outlines

			#region Restore Named Range
			// restore named ranges
			//if (this.changedNamedRange != null)
			//{
			//	foreach (var changedRangeInfo in this.changedNamedRange)
			//	{
			//		changedRangeInfo.Key.Row = changedRangeInfo.Value.start;
			//		changedRangeInfo.Key.Rows = changedRangeInfo.Value.count;
			//	}
			//}
			
			if (this.deletedNamedRanges != null)
			{
				foreach (var namedRange in this.deletedNamedRanges)
				{
					Worksheet.AddNamedRange(namedRange);
				}
			}
			#endregion // Restore Named Range

			#region Restore Highlight Ranges
			// restore highlight ranges
			//if (this.changedHighlightRanges != null)
			//{
			//	foreach (var changedRangeInfo in this.changedHighlightRanges)
			//	{
			//		changedRangeInfo.Key.Row = changedRangeInfo.Value.start;
			//		changedRangeInfo.Key.Rows = changedRangeInfo.Value.count;
			//	}
			//}

			if (this.deletedHighlightRanges != null)
			{
				foreach (var range in this.deletedHighlightRanges)
				{
					Worksheet.AddHighlightRange(range);
				}
			}
			#endregion // Restore Highlight Ranges

#if OUTLINE
			if (this.changedOutlines != null && this.changedOutlines.Count > 0)
			{
				this.Worksheet.UpdateViewportController();
			}
#endif // OUTLINE
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Remove Rows";
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new RemoveRowsAction(range.Row, range.Rows);
		}
	}
	#endregion // RemoveRowsAction

	#region RemoveColumnsAction
	/// <summary>
	/// Remove columns action.
	/// </summary>
	public class RemoveColumnsAction : WorksheetReusableAction
	{
		// main backups
		private PartialGrid backupData = null;
		private int[] backupWidths;

		// additional backups
		internal List<NamedRange> deletedNamedRanges;
		internal List<HighlightRange> deletedHighlightRanges;
		internal Dictionary<NamedRange, BackupRangeInfo> changedNamedRange;
		internal Dictionary<HighlightRange, BackupRangeInfo> changedHighlightRanges;
#if OUTLINE
		internal List<IReoGridOutline> deletedOutlines;
		internal Dictionary<IReoGridOutline, BackupRangeInfo> changedOutlines;
#endif // OUTLINE

		/// <summary>
		/// Create instance for RemoveColumnsAction
		/// </summary>
		/// <param name="column">Index of column start to remove</param>
		/// <param name="count">Number of columns to be removed</param>
		public RemoveColumnsAction(int column, int count)
			: base(new RangePosition(0, column, -1, count))
		{ }


		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			if (Range.Cols == -1)
			{
				Logger.Log("remove columns", "attempt to remove all columns but worksheet must be have at least one column, operation aborted.");
				return;
			}

			backupWidths = new int[Range.Cols];

			for (int i = Range.Col; i <= Range.EndCol; i++)
			{
				backupWidths[i - Range.Col] = Worksheet.RetrieveColumnHeader(i).InnerWidth;
			}

			backupData = Worksheet.GetPartialGrid(Range);
			Debug.Assert(backupData != null);
			Worksheet.DeleteColumns(Range.Col, Range.Cols, this);
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			if (Range.Cols == -1)
			{
				Logger.Log("remove columns", "attempt to undo removing all columns but grid must have one column, operation aborted.");
				return;
			}

			Worksheet.InsertColumns(Range.Col, Range.Cols);
			Worksheet.SetColumnsWidth(Range.Col, Range.Cols, col => backupWidths[col - Range.Col], true);

			if (backupData == null)
			{
#if DEBUG
				Logger.Log("remove rows", "no backup data");
				Debug.Assert(false, "why no backup data here?");
#else
				return;
#endif
			}

			// restore deleted data
			Worksheet.SetPartialGrid(Range, backupData);

			#region Restore Outlines
#if OUTLINE
			// restore outlines
			if (this.changedOutlines != null)
			{
				foreach (var changedOutlineInfo in this.changedOutlines)
				{
					changedOutlineInfo.Key.Start = changedOutlineInfo.Value.start;
					changedOutlineInfo.Key.Count = changedOutlineInfo.Value.count;
				}
			} 
			if (this.deletedOutlines != null)
			{
				foreach (var outline in this.deletedOutlines)
				{
					Worksheet.GroupColumns(outline.Start, outline.Count);
				}
			}
#endif // OUTLINE
			#endregion // Restore Outlines

			#region Restore Named Range
			// restore named ranges
			//if (this.changedNamedRange != null)
			//{
			//	foreach (var changedRangeInfo in this.changedNamedRange)
			//	{
			//		changedRangeInfo.Key.Col = changedRangeInfo.Value.start;
			//		changedRangeInfo.Key.Cols = changedRangeInfo.Value.count;
			//	}
			//} 
			if (this.deletedNamedRanges != null)
			{
				foreach (var namedRange in this.deletedNamedRanges)
				{
					Worksheet.AddNamedRange(namedRange);
				}
			}
			#endregion // Restore Named Range

			#region Restore Highlight Ranges
			// restore highlight ranges
			//if (this.changedHighlightRanges != null)
			//{
			//	foreach (var changedRangeInfo in this.changedHighlightRanges)
			//	{
			//		changedRangeInfo.Key.Col = changedRangeInfo.Value.start;
			//		changedRangeInfo.Key.Cols = changedRangeInfo.Value.count;
			//	}
			//} 
			if (this.deletedHighlightRanges != null)
			{
				foreach (var range in this.deletedHighlightRanges)
				{
					Worksheet.AddHighlightRange(range);
				}
			}
			#endregion // Restore Highlight Ranges

#if OUTLINE
			if (this.changedOutlines != null && this.changedOutlines.Count > 0)
			{
				this.Worksheet.UpdateViewportController();
			}
#endif // OUTLINE
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Remove Columns";
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new RemoveColumnsAction(range.Col, range.Cols);
		}
	}
	#endregion // RemoveColumnsAction

	#region SetRowsHeightAction
	/// <summary>
	/// Set height of row action
	/// </summary>
	public class SetRowsHeightAction : WorksheetReusableAction
	{
		private ushort height;

		/// <summary>
		/// Height to be set
		/// </summary>
		public ushort Height { get { return height; } set { height = value; } }

		/// <summary>
		/// Create instance for SetRowsHeightAction
		/// </summary>
		/// <param name="row">Index of row start to set</param>
		/// <param name="count">Number of rows to be set</param>
		/// <param name="height">New height to set to specified rows</param>
		public SetRowsHeightAction(int row, int count, ushort height)
			: base(new RangePosition(row, 0, count, -1))
		{
			this.height = height;
		}

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			int row = Range.Row;
			int count = Range.Rows;

			backupRows.Clear();

			int r2 = row + count;
			for (int r = row; r < r2; r++)
			{
				RowHeader rowHead = Worksheet.RetrieveRowHeader(r);

				backupRows.Add(r, new RowHeadData
				{
					autoHeight = rowHead.IsAutoHeight,
					row = rowHead.Row,
					height = rowHead.InnerHeight,
				});

				// disable auto-height-adjusting if user has changed height of this row
				rowHead.IsAutoHeight = false;
			}

			Worksheet.SetRowsHeight(row, count, height);
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			Worksheet.SetRowsHeight(Range.Row, Range.Rows, r => backupRows[r].height, true);

			foreach (int r in backupRows.Keys)
			{
				RowHeader rowHead = Worksheet.RetrieveRowHeader(r);
				rowHead.IsAutoHeight = backupRows[r].autoHeight;
			}
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Set Rows Height: " + height;
		}

		Dictionary<int, RowHeadData> backupRows = new Dictionary<int, RowHeadData>();

		internal struct RowHeadData
		{
			internal int row;
			internal ushort height;
			internal bool autoHeight;
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new SetRowsHeightAction(range.Row, range.Rows, height);
		}
	}
	#endregion // SetRowsHeightAction

	#region SetColsWidthAction

	/// <summary>
	/// Action for adjusting columns width
	/// </summary>
	[Obsolete("use SetColumnsWidthAction instead")]
	public class SetColsWidthAction : SetColumnsWidthAction
	{
	/// <summary>
		/// Create instance for SetColsWidthAction
		/// </summary>
		/// <param name="col">Index of column start to set</param>
		/// <param name="count">Number of columns to be set</param>
		/// <param name="width">Width of column to be set</param>
		public SetColsWidthAction(int col, int count, ushort width)
			: base(col, count, width)
		{
		}
}

	/// <summary>
	/// Action for adjusting columns width.
	/// </summary>
	public class SetColumnsWidthAction : WorksheetReusableAction
	{
		private ushort width;

		/// <summary>
		/// Width to be set
		/// </summary>
		public ushort Width { get { return width; } set { width = value; } }

		/// <summary>
		/// Create instance for SetColsWidthAction
		/// </summary>
		/// <param name="col">Index of column start to set</param>
		/// <param name="count">Number of columns to be set</param>
		/// <param name="width">Width of column to be set</param>
		public SetColumnsWidthAction(int col, int count, ushort width)
			: base(new RangePosition(0, col, -1, count))
		{
			this.width = width;
		}

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			int col = base.Range.Col;
			int count = base.Range.Cols;

			backupCols.Clear();

			int c2 = col + count;
			for (int c = col; c < c2; c++)
			{
				ColumnHeader colHead = Worksheet.RetrieveColumnHeader(c);
				backupCols.Add(c, colHead.InnerWidth);
			}

			Worksheet.SetColumnsWidth(col, count, width);
		}

		private Dictionary<int, ushort> backupCols = new Dictionary<int, ushort>();

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			int col = base.Range.Col;
			int count = base.Range.Cols;

			Worksheet.SetColumnsWidth(col, count, c => backupCols[c]);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Set Cols Width: " + width;
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new SetColumnsWidthAction(range.Col, range.Cols, width);
		}
	}
	#endregion // SetColsWidthAction

	#region HideRowsAction
	/// <summary>
	/// Hide specified rows action
	/// </summary>
	public class HideRowsAction : WorksheetReusableAction
	{
		/// <summary>
		/// Create action to hide specified rows.
		/// </summary>
		/// <param name="row">Zero-based row index to start hiding.</param>
		/// <param name="count">Number of rows to be hidden.</param>
		public HideRowsAction(int row, int count)
			: base(new RangePosition(row, 0, count, -1)) { }

		/// <summary>
		/// Do action to hide specified rows.
		/// </summary>
		public override void Do()
		{
			this.Worksheet.HideRows(base.Range.Row, base.Range.Rows);
		}

		/// <summary>
		/// Undo action to show hidden rows.
		/// </summary>
		public override void Undo()
		{
			this.Worksheet.ShowRows(base.Range.Row, base.Range.Rows);
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new HideRowsAction(range.Row, range.Rows);
		}

		/// <summary>
		/// Get friendly name of this action.
		/// </summary>
		/// <returns>friendly name of this action.</returns>
		public override string GetName() { return "Hide Rows"; }
	}
	#endregion // RGHideRowsAction

	#region UnhideRowsAction
	/// <summary>
	/// Action to unhide specified rows.
	/// </summary>
	public class UnhideRowsAction : WorksheetReusableAction
	{
		/// <summary>
		/// Create action to show specified rows.
		/// </summary>
		/// <param name="row">number of row to start unhidden.</param>
		/// <param name="count">number of rows to be unhidden.</param>
		public UnhideRowsAction(int row, int count)
			: base(new RangePosition(row, 0, count, -1)) { }

		/// <summary>
		/// Do action to show specified hidden rows.
		/// </summary>
		public override void Do()
		{
			this.Worksheet.ShowRows(base.Range.Row, base.Range.Rows);
		}

		/// <summary>
		/// Undo action to hide visible rows.
		/// </summary>
		public override void Undo()
		{
			this.Worksheet.HideRows(base.Range.Row, base.Range.Rows);
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new UnhideRowsAction(range.Row, range.Rows);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns>firendly name of this action</returns>
		public override string GetName() { return "Unhide Rows"; }
	}
	#endregion // UnhideRowsAction

	#region HideColumnsAction
	/// <summary>
	/// Action to hide specified columns
	/// </summary>
	public class HideColumnsAction : WorksheetReusableAction
	{
		/// <summary>
		/// Create action to hide specified columns.
		/// </summary>
		/// <param name="col">zero-based number of column to start hide columns.</param>
		/// <param name="count">number of columns to be hidden.</param>
		public HideColumnsAction(int col, int count)
			: base(new RangePosition(0, col, -1, count)) { }

		/// <summary>
		/// Perform action to hide specified columns.
		/// </summary>
		public override void Do()
		{
			this.Worksheet.HideColumns(base.Range.Col, base.Range.Cols);
		}

		/// <summary>
		/// Undo action to show hidden columns.
		/// </summary>
		public override void Undo()
		{
			this.Worksheet.ShowColumns(base.Range.Col, base.Range.Cols);
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new HideColumnsAction(range.Col, range.Cols);
		}

		/// <summary>
		/// Get friendly name of this action.
		/// </summary>
		/// <returns></returns>
		public override string GetName() { return "Hide Columns"; }
	}
	#endregion // HideColumnsAction

	#region UnhideColumnsAction
	/// <summary>
	/// Unhide specified columns action
	/// </summary>
	public class UnhideColumnsAction : WorksheetReusableAction
	{
		/// <summary>
		/// Create action to show hidden columns
		/// </summary>
		/// <param name="col">Number of column start to unhide</param>
		/// <param name="count">Number of columns to be unhidden</param>
		public UnhideColumnsAction(int col, int count)
			: base(new RangePosition(0, col, -1, count)) { }

		/// <summary>
		/// Do action to show hidden columns
		/// </summary>
		public override void Do()
		{
			this.Worksheet.ShowColumns(base.Range.Col, base.Range.Cols);
		}

		/// <summary>
		/// Do action to hide specified visible columns
		/// </summary>
		public override void Undo()
		{
			this.Worksheet.HideColumns(base.Range.Col, base.Range.Cols);
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new UnhideColumnsAction(range.Col, range.Cols);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns>friendly name of this action</returns>
		public override string GetName() { return "Unhide Columns"; }
	}
	#endregion // UnhideColumnsAction

	#endregion // Actions - Row & Column Header

	// Border Editing Actions
	#region Actions - Borders

	/// <summary>
	/// Action to set borders to specified range
	/// </summary>
	public class SetRangeBorderAction : WorksheetReusableAction
	{
		private RangeBorderInfo[] borders;

		/// <summary>
		/// Borders to be set
		/// </summary>
		public RangeBorderInfo[] Borders
		{
			get { return borders; }
			set { borders = value; }
		}

		private PartialGrid backupData;

		/// <summary>
		/// Create action that perform setting border to a range
		/// </summary>
		/// <param name="range">Range to be appiled this action</param>
		/// <param name="pos">Position of range to set border</param>
		/// <param name="styles">Style of border</param>
		public SetRangeBorderAction(RangePosition range, BorderPositions pos, RangeBorderStyle styles)
			: this(range, new RangeBorderInfo[] { new RangeBorderInfo(pos, styles) }) { }

		/// <summary>
		/// Create action that perform setting border to a range
		/// </summary>
		/// <param name="range">Range to be appiled this action</param>
		/// <param name="styles">Style of border</param>
		public SetRangeBorderAction(RangePosition range, RangeBorderInfo[] styles)
			: base(range)
		{
			this.borders = styles;
		}

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			backupData = Worksheet.GetPartialGrid(Range, PartialGridCopyFlag.BorderAll,
				ExPartialGridCopyFlag.BorderOutsideOwner);

			for (int i = 0; i < borders.Length; i++)
			{
				Worksheet.SetRangeBorders(Range, borders[i].Pos, borders[i].Style);
			}
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			Worksheet.SetPartialGrid(Range, backupData, PartialGridCopyFlag.BorderAll,
				ExPartialGridCopyFlag.BorderOutsideOwner);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Set Range Border";
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new SetRangeBorderAction(range, borders);
		}
	}
	
	/// <summary>
	/// Action of Removing borders from specified range
	/// </summary>
	public class RemoveRangeBorderAction : WorksheetReusableAction
	{
		/// <summary>
		/// Get or set the position of borders to be removed
		/// </summary>
		public BorderPositions BorderPos { get; set; }

		private PartialGrid backupData;

		/// <summary>
		/// Create instance for SetRangeBorderAction with specified range and border styles.
		/// </summary>
		/// <param name="range">Range to be appiled this action</param>
		/// <param name="pos">Position of range to set border</param>
		public RemoveRangeBorderAction(RangePosition range, BorderPositions pos)
			: base(range)
		{
			this.BorderPos = pos;
		}

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			backupData = Worksheet.GetPartialGrid(Range, PartialGridCopyFlag.BorderAll,
				ExPartialGridCopyFlag.BorderOutsideOwner);

			Worksheet.RemoveRangeBorders(Range, BorderPos);
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			Worksheet.SetPartialGrid(Range, backupData, PartialGridCopyFlag.BorderAll,
				ExPartialGridCopyFlag.BorderOutsideOwner);
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Set Range Border";
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new RemoveRangeBorderAction(range, BorderPos);
		}
	}
	#endregion // Actions - Borders

	// Range/Cell Editing Actions
	#region Actions - Range & Cell Edit
	/// <summary>
	/// Merge range action
	/// </summary>
	public class MergeRangeAction : WorksheetReusableAction
	{
		/// <summary>
		/// Create instance for MergeRangeAction with specified range
		/// </summary>
		/// <param name="range">The range to be merged</param>
		public MergeRangeAction(RangePosition range) : base(range) { }

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new MergeRangeAction(range);
		}

		private PartialGrid backupData;

		/// <summary>
		/// Do this operation.
		/// </summary>
		public override void Do()
		{
			// todo
			backupData = Worksheet.GetPartialGrid(Range, PartialGridCopyFlag.All, ExPartialGridCopyFlag.None);
			Worksheet.MergeRange(Range);
		}

		/// <summary>
		/// Undo this operation.
		/// </summary>
		public override void Undo()
		{
			Worksheet.UnmergeRange(base.Range);
			Worksheet.SetPartialGrid(Range, backupData, PartialGridCopyFlag.All);
		}

		/// <summary>
		/// Get friendly name of this action.
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Merge Range";
		}
	}

	/// <summary>
	/// Unmerge range action.
	/// </summary>
	public class UnmergeRangeAction : WorksheetReusableAction
	{
		/// <summary>
		/// Create instance for UnmergeRangeAction with specified range.
		/// </summary>
		/// <param name="range">The range to be unmerged</param>
		public UnmergeRangeAction(RangePosition range) : base(range) { }

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new UnmergeRangeAction(range);
		}

		private PartialGrid backupData;

		/// <summary>
		/// Do this action.
		/// </summary>
		public override void Do()
		{
			// todo
			backupData = Worksheet.GetPartialGrid(Range, PartialGridCopyFlag.All, ExPartialGridCopyFlag.None);
			Worksheet.UnmergeRange(Range);
		}

		/// <summary>
		/// Undo this action.
		/// </summary>
		public override void Undo()
		{
			Worksheet.SetPartialGrid(Range, backupData, PartialGridCopyFlag.All);
		}

		/// <summary>
		/// Get friendly name of this action.
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return "Merge Range";
		}
	}

	/// <summary>
	/// Set data of cell action.
	/// </summary>
	public class SetCellDataAction : BaseWorksheetAction
	{
		private int row;

		/// <summary>
		/// Index of row to set data.
		/// </summary>
		public int Row { get { return row; } set { row = value; } }

		private int col;

		/// <summary>
		/// Index of column to set data.
		/// </summary>
		public int Col { get { return col; } set { col = value; } }

		//private bool isCellNull;

		private object data;

		/// <summary>
		/// Data of cell.
		/// </summary>
		public object Data
		{
			get { return data; }
			set { data = value; }
		}

		private object backupData;
		//private string backupFormula;
		//private string displayBackup;
		private CellDataFormatFlag backupDataFormat;
		private object backupDataFormatArgs;
		//private Core.ReoGridRenderHorAlign backupRenderAlign;
		//private bool autoUpdateReferenceCells = false;
		private ushort? backupRowHeight = 0;

		/// <summary>
		/// Create SetCellValueAction with specified index of row and column.
		/// </summary>
		/// <param name="row">index of row to set data.</param>
		/// <param name="col">index of column to set data.</param>
		/// <param name="data">data to be set.</param>
		public SetCellDataAction(int row, int col, object data)
		{
			this.row = row;
			this.col = col;
			this.data = data;
		}

		/// <summary>
		/// Create SetCellValueAction with specified index of row and column.
		/// </summary>
		/// <param name="pos">position to locate the cell to be set.</param>
		/// <param name="data">data to be set.</param>
		public SetCellDataAction(CellPosition pos, object data)
			: this(pos.Row, pos.Col, data)
		{
		}

		/// <summary>
		/// Create action to set cell data.
		/// </summary>
		/// <param name="address">address to locate specified cell.</param>
		/// <param name="data">data to be set.</param>
		public SetCellDataAction(string address, object data)
		{
			CellPosition pos = new CellPosition(address);
			this.row = pos.Row;
			this.col = pos.Col;
			this.data = data;
		}

		/// <summary>
		/// Do this operation.
		/// </summary>
		public override void Do()
		{
			Cell cell = Worksheet.CreateAndGetCell(row, col);

			this.backupData = cell.HasFormula ? ("=" + cell.InnerFormula) : cell.InnerData;

			this.backupDataFormat = cell.DataFormat;
			this.backupDataFormatArgs = cell.DataFormatArgs;

			try
			{
				Worksheet.SetSingleCellData(cell, data);

				var rowHeightSettings = WorksheetSettings.Edit_AutoExpandRowHeight
					| WorksheetSettings.Edit_AllowAdjustRowHeight;

				RowHeader rowHeader = this.Worksheet.GetRowHeader(cell.InternalRow);

				this.backupRowHeight = rowHeader.InnerHeight;

				if ((this.Worksheet.settings & rowHeightSettings) == rowHeightSettings
					&& rowHeader.IsAutoHeight)
				{
					cell.ExpandRowHeight();
				}
			}
			catch(Exception ex)
			{
				this.Worksheet.NotifyExceptionHappen(ex);
			}

		}

		public override void Redo()
		{
			this.Do();

			Cell cell = Worksheet.GetCell(row, col);

			if (cell != null)
			{
				Worksheet.SelectRange(new RangePosition(cell.InternalRow, cell.InternalCol, cell.Rowspan, cell.Colspan));
			}
		}

		/// <summary>
		/// Undo this operation.
		/// </summary>
		public override void Undo()
		{
			if (this.backupRowHeight != null)
			{
				var rowHeader = this.Worksheet.GetRowHeader(this.row);

				if (rowHeader.InnerHeight != this.backupRowHeight)
				{
					this.Worksheet.SetRowsHeight(this.row, 1, (ushort)this.backupRowHeight);
				}
			}

			Cell cell = Worksheet.GetCell(row, col);
			if (cell != null)
			{
				cell.DataFormat = this.backupDataFormat;
				cell.DataFormatArgs = this.backupDataFormatArgs;

				this.Worksheet.SetSingleCellData(cell, this.backupData);

				Worksheet.SelectRange(new RangePosition(cell.InternalRow, cell.InternalCol, cell.Rowspan, cell.Colspan));
			}
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			string str = data == null ? "null" : data.ToString();
			return "Set Cell Value: " + (str.Length > 10 ? (str.Substring(0, 7) + "...") : str);
		}
	}

	/// <summary>
	/// Set range data format action.
	/// </summary>
	public class SetRangeDataFormatAction : WorksheetReusableAction
	{
		private CellDataFormatFlag format;
		private object formatArgs;
		private PartialGrid backupData = null;

		/// <summary>
		/// Create instance for SetRangeDataFormatAction.
		/// </summary>
		/// <param name="range">Range to be appiled this action.</param>
		/// <param name="format">Format type of cell to be set.</param>
		/// <param name="dataFormatArgs">Argument belongs to format type to be set.</param>
		public SetRangeDataFormatAction(RangePosition range, CellDataFormatFlag format,
			object dataFormatArgs)
			: base(range)
		{
			this.format = format;
			this.formatArgs = dataFormatArgs;
		}

		/// <summary>
		/// Do this operation.
		/// </summary>
		public override void Do()
		{
			backupData = Worksheet.GetPartialGrid(Range, PartialGridCopyFlag.CellData, ExPartialGridCopyFlag.None);
			Worksheet.SetRangeDataFormat(Range, format, formatArgs);
		}

		/// <summary>
		/// Undo this operation.
		/// </summary>
		public override void Undo()
		{
			Worksheet.SetPartialGrid(Range, backupData);
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new SetRangeDataFormatAction(range, this.format, formatArgs);
		}

		/// <summary>
		/// Get friendly name of this action.
		/// </summary>
		/// <returns>friendly name of this action.</returns>
		public override string GetName()
		{
			return "Set Cells Format: " + format.ToString();
		}
	}

	/// <summary>
	/// Create action to set data into specified range of spreadsheet.
	/// </summary>
	public class SetRangeDataAction : WorksheetReusableAction
	{
		private object[,] data;
		private object[,] backupData;

		/// <summary>
		/// Create action to set data into specified range of spreadsheet.
		/// </summary>
		/// <param name="range">range to set specified data.</param>
		/// <param name="data">data to be set.</param>
		public SetRangeDataAction(RangePosition range, object[,] data)
			: base(range)
		{
			this.data = data;
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new SetRangeDataAction(range, data);
		}

		/// <summary>
		/// Do action to set data into specified range of spreadsheet
		/// </summary>
		public override void Do()
		{
			backupData = Worksheet.GetRangeData(base.Range);
			Debug.Assert(backupData != null);
			base.Worksheet.SetRangeData(base.Range, data, true);
			Worksheet.SelectRange(base.Range);
		}

		/// <summary>
		/// Undo action to remove data which has been set into specified range of spreadsheet
		/// </summary>
		public override void Undo()
		{
			Debug.Assert(backupData != null);
			base.Worksheet.SetRangeData(base.Range, backupData);
		}

		/// <summary>
		/// Get friendly name of this action.
		/// </summary>
		/// <returns>friendly name of this action.</returns>
		public override string GetName()
		{
			return "Set Cells Data";
		}
	}

	/// <summary>
	/// Action to remove data from specified range.
	/// </summary>
	public class RemoveRangeDataAction : WorksheetReusableAction
	{
		private object[,] backupData;
		
		/// <summary>
		/// Create action to remove data from specified range.
		/// </summary>
		/// <param name="range">data from cells in this range will be removed.</param>
		public RemoveRangeDataAction(RangePosition range)
			: base(range)
		{
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new RemoveRangeDataAction(range);
		}

		/// <summary>
		/// Do action to remove data from specified range.
		/// </summary>
		public override void Do()
		{
			this.backupData = Worksheet.GetRangeData(base.Range);
			this.Worksheet.DeleteRangeData(this.Range, true);
		}

		/// <summary>
		/// Undo action to restore removed data.
		/// </summary>
		public override void Undo()
		{
			this.Worksheet.SetRangeData(this.Range, this.backupData);
		}

		/// <summary>
		/// Get friendly name of this action.
		/// </summary>
		/// <returns>friendly name of this action.</returns>
		public override string GetName()
		{
			return "Remove Cells Data";
		}
	}

	/// <summary>
	/// Action to move specified range from a position to another position.
	/// </summary>
	public class MoveRangeAction : BaseWorksheetAction
	{
		/// <summary>
		/// Specifies the content to be moved: data, borders and styles.
		/// </summary>
		public PartialGridCopyFlag ContentFlags { get; set; }

		/// <summary>
		/// Range to be moved.
		/// </summary>
		public RangePosition FromRange { get; set; }

		/// <summary>
		/// Position that range will be moved to.
		/// </summary>
		public CellPosition ToPosition { get; set; }

		/// <summary>
		/// Construct this action to move specified range from a position to another position.
		/// </summary>
		/// <param name="fromRange">range to be moved.</param>
		/// <param name="toPosition">position to be moved to.</param>
		public MoveRangeAction(RangePosition fromRange, CellPosition toPosition)
		{
			this.ContentFlags = PartialGridCopyFlag.All;
			this.FromRange = fromRange;
			this.ToPosition = toPosition;
		}

		private PartialGrid backupGrid;

		/// <summary>
		/// Do this action.
		/// </summary>
		public override void Do()
		{
			var targetRange = new RangePosition(
				this.ToPosition.Row, this.ToPosition.Col,
				this.FromRange.Rows, this.FromRange.Cols);

			backupGrid = this.Worksheet.GetPartialGrid(targetRange);

			this.Worksheet.MoveRange(this.FromRange, targetRange);

			this.Worksheet.SelectionRange = targetRange;
		}

		/// <summary>
		/// Undo this action.
		/// </summary>
		public override void Undo()
		{
			var targetRange = new RangePosition(
				this.ToPosition.Row, this.ToPosition.Col,
				this.FromRange.Rows, this.FromRange.Cols);

			this.Worksheet.MoveRange(targetRange, this.FromRange);

			this.Worksheet.SetPartialGrid(targetRange, this.backupGrid);

			this.Worksheet.SelectionRange = this.FromRange;
		}

		/// <summary>
		/// Get friendly name of this action.
		/// </summary>
		/// <returns>friendly name of this action.</returns>
		public override string GetName()
		{
			return "Move Range";
		}
	}

	/// <summary>
	/// Action to copy the specified range from a position to another position.
	/// </summary>
	public class CopyRangeAction : BaseWorksheetAction
	{
		/// <summary>
		/// Specifies the content to be moved: data, borders and styles.
		/// </summary>
		public PartialGridCopyFlag ContentFlags { get; set; }

		/// <summary>
		/// Range to be moved
		/// </summary>
		public RangePosition FromRange { get; set; }

		/// <summary>
		/// Position that range will be moved to
		/// </summary>
		public CellPosition ToPosition { get; set; }

		/// <summary>
		/// Construct this action to move specified range from a position to another position
		/// </summary>
		/// <param name="fromRange">range to be moved</param>
		/// <param name="toPosition">position to be moved to</param>
		public CopyRangeAction(RangePosition fromRange, CellPosition toPosition)
		{
			this.ContentFlags = PartialGridCopyFlag.All;
			this.FromRange = fromRange;
			this.ToPosition = toPosition;
		}

		private PartialGrid backupGrid;

		/// <summary>
		/// Do this action.
		/// </summary>
		public override void Do()
		{
			var targetRange = new RangePosition(
				this.ToPosition.Row, this.ToPosition.Col,
				this.FromRange.Rows, this.FromRange.Cols);

			this.backupGrid = this.Worksheet.GetPartialGrid(targetRange);

			this.Worksheet.CopyRange(this.FromRange, targetRange);

			this.Worksheet.SelectionRange = targetRange;
		}

		/// <summary>
		/// Undo this action.
		/// </summary>
		public override void Undo()
		{
			var targetRange = new RangePosition(
				this.ToPosition.Row, this.ToPosition.Col,
				this.FromRange.Rows, this.FromRange.Cols);

			this.Worksheet.SetPartialGrid(targetRange, this.backupGrid);
		
			this.Worksheet.SelectionRange = this.FromRange;
		}

		/// <summary>
		/// Get friendly name of this action.
		/// </summary>
		/// <returns>friendly name of this action.</returns>
		public override string GetName()
		{
			return "Copy Range";
		}

	}
	#endregion // Actions - Range & Cell Edit

	// Partial Grid
	#region Actions - PartialGrid

	[Obsolete("use SetPartialGridAction instead")]
	public class RGSetPartialGridAction : SetPartialGridAction
	{
		/// <summary>
		/// Create action to set partial grid.
		/// </summary>
		/// <param name="range">Range to set grid.</param>
		/// <param name="data">Data of partial grid to be set.</param>
		public RGSetPartialGridAction(RangePosition range, PartialGrid data) : base(range, data) { }
	}

	/// <summary>
	/// Action to set partial grid.
	/// </summary>
	public class SetPartialGridAction : WorksheetReusableAction
	{
		private PartialGrid data;
		private PartialGrid backupData;

		/// <summary>
		/// Create action to set partial grid.
		/// </summary>
		/// <param name="range">target range to set partial grid.</param>
		/// <param name="data">partial grid to be set.</param>
		public SetPartialGridAction(RangePosition range, PartialGrid data)
			: base(range)
		{
			this.data = data;
		}

		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new SetPartialGridAction(range, data);
		}

		/// <summary>
		/// Do action to set partial grid.
		/// </summary>
		public override void Do()
		{
			backupData = Worksheet.GetPartialGrid(base.Range, PartialGridCopyFlag.All, ExPartialGridCopyFlag.BorderOutsideOwner);
			Debug.Assert(backupData != null);
			base.Range = base.Worksheet.SetPartialGridRepeatly(base.Range, data);
			Worksheet.SelectRange(base.Range);
		}

		/// <summary>
		/// Undo action to restore setting partial grid.
		/// </summary>
		public override void Undo()
		{
			Debug.Assert(backupData != null);
			base.Worksheet.SetPartialGrid(base.Range, backupData, PartialGridCopyFlag.All, ExPartialGridCopyFlag.BorderOutsideOwner);
		}

		/// <summary>
		/// Get friendly name of this action.
		/// </summary>
		/// <returns>Friendly name of this action.</returns>
		public override string GetName()
		{
			return "Set Partial Grid";
		}
	}
	#endregion

	// Filter & Sort
	#region Filter & Sort
	/// <summary>
	/// Action to create column filter
	/// </summary>
	public class CreateAutoFilterAction : BaseWorksheetAction
	{
		///// <summary>
		///// Zero-based number of column to start create filter
		///// </summary>
		//public int StartColumn { get; private set; }

		///// <summary>
		///// Zero-based number of column to end create filter
		///// </summary>
		//public int EndColumn { get; private set; }

		///// <summary>
		///// Indicates how many title rows exist on the spreadsheet
		///// </summary>
		//public int TitleRows { get; private set; }
		
		/// <summary>
		/// Get filter apply range.
		/// </summary>
		public RangePosition Range { get; private set; }

		private AutoColumnFilter autoColumnFilter;

		/// <summary>
		/// Get auto column filter instance created by this action. (Will be null before doing action)
		/// </summary>
		public AutoColumnFilter AutoColumnFilter { get { return this.autoColumnFilter; } }

		/// <summary>
		/// Create action to create column filter
		/// </summary>
		/// <param name="range">filter range</param>
		public CreateAutoFilterAction(RangePosition range)
		{
			this.Range = range;
		}

		///// <summary>
		///// Create action to create column filter
		///// </summary>
		///// <param name="startColumn">zero-based number of column begin to create filter</param>
		///// <param name="endColumn">zero-based number of column end to create filter</param>
		///// <param name="titleRows">number of rows as title rows will not be included in filter and sort range</param>
		//public CreateAutoFilterAction(int startColumn, int endColumn, int titleRows = 1)
		//{
		//	this.StartColumn = startColumn;
		//	this.EndColumn = endColumn;
		//	this.TitleRows = titleRows;
		//}

		/// <summary>
		/// Undo action to remove column filter that is created by this action
		/// </summary>
		public override void Undo()
		{
			if (autoColumnFilter != null)
			{
				autoColumnFilter.Detach();
			}
		}

		/// <summary>
		/// Do action to create column filter
		/// </summary>
		public override void Do()
		{
			if (this.autoColumnFilter == null)
			{
				this.autoColumnFilter = base.Worksheet.CreateColumnFilter(this.Range,
					AutoColumnFilterUI.DropdownButtonAndPanel);
			}
			else
			{
				this.autoColumnFilter.Attach(base.Worksheet);
			}
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns>friendly name of this action</returns>
		public override string GetName()
		{
			return "Create Column Filter";
		}
	}
	#endregion

	// Outline
	#region Outline
#if OUTLINE
	/// <summary>
	/// Base class for all classes of outline actions
	/// </summary>
	public abstract class BaseOutlineAction: BaseWorksheetAction
	{
		/// <summary>
		/// Outline direction.
		/// </summary>
		internal protected RowOrColumn rowOrColumn;

		/// <summary>
		/// Specify row or column outline
		/// </summary>
		public RowOrColumn RowOrColumn { get { return this.rowOrColumn; } }

		/// <summary>
		/// Create base outline action instance
		/// </summary>
		/// <param name="rowOrColumn">Flag to specify row or column</param>
		public BaseOutlineAction(RowOrColumn rowOrColumn)
		{
			this.rowOrColumn = rowOrColumn;
		}

		/// <summary>
		/// Get description text of outline direction.
		/// </summary>
		/// <returns>Text of outline direction.</returns>
		internal protected string GetRowOrColumnDesc()
		{
			switch (this.rowOrColumn)
			{
				default:
				case ReoGrid.RowOrColumn.Both:
					return "Row and Column";
				case ReoGrid.RowOrColumn.Row:
					return "Row";
				case ReoGrid.RowOrColumn.Column:
					return "Column";
			}
		}
	}

	/// <summary>
	/// Base class for all classes of single outline action
	/// </summary>
	public abstract class OutlineAction : BaseOutlineAction
	{
		internal int start;

		/// <summary>
		/// Number of line of start position to outilne
		/// </summary>
		public int Start { get { return this.start; } }

		internal int count;

		/// <summary>
		/// Number of lines does outline include
		/// </summary>
		public int Count { get { return this.count; } }
	
		/// <summary>
		/// Create base outline action instance
		/// </summary>
		/// <param name="rowOrColumn">Flag to specify row or column</param>
		/// <param name="start">Number of line to start add outline</param>
		/// <param name="count">Number of lines to be added into this outline</param>
		public OutlineAction(RowOrColumn rowOrColumn, int start, int count)
			: base(rowOrColumn)
		{
			this.start = start;
			this.count = count;
		}
	}

	/// <summary>
	/// Add outline action
	/// </summary>
	public class AddOutlineAction : OutlineAction
	{
		/// <summary>
		/// Create action to add outline
		/// </summary>
		/// <param name="rowOrColumn">Row or column to be added</param>
		/// <param name="start">Number of line to start add outline</param>
		/// <param name="count">Number of lines to be added into this outline</param>
		public AddOutlineAction(RowOrColumn rowOrColumn, int start, int count)
			: base(rowOrColumn, start, count)
		{
		}

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			if (this.Worksheet != null)
			{
				this.Worksheet.AddOutline(this.rowOrColumn, start, count);
			}
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			if (this.Worksheet != null)
			{
				this.Worksheet.RemoveOutline(this.rowOrColumn, start, count);
			}
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns>Name of action</returns>
		public override string GetName()
		{
			return string.Format("Add {0} Outline, Start at {1}, Count: {2}",
				base.GetRowOrColumnDesc(), this.start, this.count);
		}
	}

	/// <summary>
	/// Remove outline action
	/// </summary>
	public class RemoveOutlineAction : OutlineAction
	{
		private IReoGridOutline removedOutline;

		/// <summary>
		/// Instance of removed outline if operation was successfully
		/// </summary>
		public IReoGridOutline RemovedOutline { get { return this.removedOutline; } }

		/// <summary>
		/// Create action instance to remove outline
		/// </summary>
		/// <param name="rowOrColumn">Row or column to find specified outline</param>
		/// <param name="start">Number of line of specified outline</param>
		/// <param name="count">Number of lines of specified outline</param>
		public RemoveOutlineAction(RowOrColumn rowOrColumn, int start, int count)
			: base(rowOrColumn, start, count)
		{
		}

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			if (this.Worksheet != null)
			{
				this.removedOutline = this.Worksheet.RemoveOutline(this.rowOrColumn, start, count);
			}
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			if (this.Worksheet != null)
			{
				this.Worksheet.AddOutline(this.rowOrColumn, start, count);
				this.removedOutline = null;
			}
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns>Name of action</returns>
		public override string GetName()
		{
			return string.Format("Remove {0} Outline, Start at {1}, Count: {2}", 
				base.GetRowOrColumnDesc(), this.start, this.count);
		}	
	}

	/// <summary>
	/// Action to collapse outline
	/// </summary>
	public class CollapseOutlineAction : OutlineAction
	{
		/// <summary>
		/// Create action to collapse outline
		/// </summary>
		/// <param name="rowOrColumn">Row or column to find specified outline</param>
		/// <param name="start">Number of line of specified outline</param>
		/// <param name="count">Number of lines of specified outline</param>
		public CollapseOutlineAction(RowOrColumn rowOrColumn, int start, int count)
			: base(rowOrColumn, start, count)
		{
		}

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			if (this.Worksheet != null)
			{
				this.Worksheet.CollapseOutline(this.rowOrColumn, this.start, this.count);
			}
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			if (this.Worksheet != null)
			{
				this.Worksheet.ExpandOutline(this.rowOrColumn, this.start, this.count);
			}
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns>Name of action</returns>
		public override string GetName()
		{
			return string.Format("Collapse {0} Outline, Start at {1}, Count: {2}",
				base.GetRowOrColumnDesc(), this.start, this.count);
		}
	}

	/// <summary>
	/// Action to collapse outline
	/// </summary>
	public class ExpandOutlineAction : OutlineAction
	{
		/// <summary>
		/// Create action instance to expand outline
		/// </summary>
		/// <param name="rowOrColumn">Row or column to find specified outline</param>
		/// <param name="start">Number of line of specified outline</param>
		/// <param name="count">Number of lines of specified outline</param>
		public ExpandOutlineAction(RowOrColumn rowOrColumn, int start, int count)
			: base(rowOrColumn, start, count)
		{
		}

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			if (this.Worksheet != null)
			{
				this.Worksheet.ExpandOutline(this.rowOrColumn, this.start, this.count);
			}
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			if (this.Worksheet != null)
			{
				this.Worksheet.CollapseOutline(this.rowOrColumn, this.start, this.count);
			}
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns>Name of action</returns>
		public override string GetName()
		{
			return string.Format("Expand {0} Outline, Start at {1}, Count: {2}",
				base.GetRowOrColumnDesc(), this.start, this.count);
		}
	}

	/// <summary>
	/// Action to clear all outlines
	/// </summary>
	public class ClearOutlineAction : BaseOutlineAction
	{
		/// <summary>
		/// Create action to clear all outlines on specified range
		/// </summary>
		/// <param name="rowOrColumn">The range to clear outlines (row, column or both row and column)</param>
		public ClearOutlineAction(RowOrColumn rowOrColumn)
			: base(rowOrColumn)
		{
		}

		private List<IReoGridOutline> rowBackupOutlines;
		private List<IReoGridOutline> colBackupOutlines;

		/// <summary>
		/// Do this action
		/// </summary>
		public override void Do()
		{
			if (this.Worksheet != null)
			{
				if ((this.rowOrColumn & ReoGrid.RowOrColumn.Row) == ReoGrid.RowOrColumn.Row)
				{
					this.rowBackupOutlines = new List<IReoGridOutline>();
					this.Worksheet.IterateOutlines(ReoGrid.RowOrColumn.Row, (g, o) => { this.rowBackupOutlines.Add(o); return true; });
				}

				if ((this.rowOrColumn & ReoGrid.RowOrColumn.Column) == ReoGrid.RowOrColumn.Column)
				{
					this.colBackupOutlines = new List<IReoGridOutline>();
					this.Worksheet.IterateOutlines(ReoGrid.RowOrColumn.Column, (g, o) => { this.colBackupOutlines.Add(o); return true; });
				}

				this.Worksheet.ClearOutlines(this.rowOrColumn);
			}
		}

		/// <summary>
		/// Undo this action
		/// </summary>
		public override void Undo()
		{
			if (this.Worksheet != null)
			{
				if ((this.rowOrColumn & ReoGrid.RowOrColumn.Row) == ReoGrid.RowOrColumn.Row
					&& this.rowBackupOutlines != null)
				{
					foreach (var o in this.rowBackupOutlines)
					{
						this.Worksheet.AddOutline(ReoGrid.RowOrColumn.Row, o.Start, o.Count);
					}
				}

				if ((this.rowOrColumn & ReoGrid.RowOrColumn.Column) == ReoGrid.RowOrColumn.Column
					&& this.colBackupOutlines != null)
				{
					foreach (var o in this.colBackupOutlines)
					{
						this.Worksheet.AddOutline(ReoGrid.RowOrColumn.Column, o.Start, o.Count);
					}
				}
			}
		}

		/// <summary>
		/// Get friendly name of this action
		/// </summary>
		/// <returns>Name of action</returns>
		public override string GetName()
		{
			return string.Format("Clear {0} Outlines", base.GetRowOrColumnDesc());
		}
	}
#endif // OUTLINE
	#endregion Outline
}
