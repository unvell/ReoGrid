/*****************************************************************************
 * 
 * ReoGrid - Opensource .NET Spreadsheet Control
 * 
 * https://reogrid.net/
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * Thank you to all contributors!
 * 
 * (c) 2012-2020 Jingwood, unvell.com <jingwood at unvell.com>
 * 
 ****************************************************************************/

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using unvell.Common;

#if OUTLINE
using unvell.ReoGrid.Outline;
#endif // OUTLINE

namespace unvell.ReoGrid.Actions
{
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
}
