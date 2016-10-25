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
 * Source code in test-case project released under BSD license.
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using unvell.ReoGrid.Actions;

namespace unvell.ReoGrid.Tests
{
	[TestSet]
	class DeleteRowsTest : ReoGridTestSet
	{
		[TestCase]
		public void SimpleDelete()
		{
			SetUp();

			// normal 
			worksheet.SetRows(10);
			AssertEquals(worksheet.RowCount, 10);
			worksheet.DeleteRows(5, 5);
			AssertEquals(worksheet.RowCount, 5);

			// row count overflow
			worksheet.SetRows(10);
			AssertEquals(worksheet.RowCount, 10);
			worksheet.DeleteRows(8, 2);
			AssertEquals(worksheet.RowCount, 8);
		}

		[TestCase]
		public void DeleteInMergedCell()
		{
			SetUp();

			worksheet.SetRows(20);

			worksheet.MergeRange(new RangePosition(1, 1, 10, 10));
			AssertTrue(worksheet._Debug_Validate_MergedCells());
			AssertEquals(worksheet.RowCount, 20);

			worksheet.DeleteRows(3, 3);
			AssertTrue(worksheet._Debug_Validate_All());
			AssertEquals(worksheet.RowCount, 17);

			worksheet.DeleteRows(4, 10);
			AssertTrue(worksheet._Debug_Validate_All());
			AssertEquals(worksheet.RowCount, 7);
		}

		[TestCase]
		public void DeleteCrossMergedCell()
		{
			SetUp(20, 20);

			worksheet.MergeRange(new RangePosition(1, 1, 10, 10));
			AssertTrue(worksheet._Debug_Validate_MergedCells());
			AssertEquals(worksheet.RowCount, 20);

			worksheet.DeleteRows(5, 10);
			AssertTrue(worksheet._Debug_Validate_All());
			AssertEquals(worksheet.RowCount, 10);

			AssertTrue(!worksheet.IsMergedCell(6, 1));
		}

		[TestCase]
		public void DeleteAboveMergedCell()
		{
			SetUp();

			worksheet.SetRows(20);

			worksheet.MergeRange(new RangePosition(5, 1, 10, 10));
			AssertTrue(worksheet._Debug_Validate_MergedCells());
			AssertEquals(worksheet.RowCount, 20);

			worksheet.DeleteRows(2, 3);
			AssertTrue(worksheet._Debug_Validate_All());
			AssertEquals(worksheet.RowCount, 17);
		}

		[TestCase]
		public void DeleteAboveMergedCell_Action()
		{
			SetUp();

			worksheet.SetRows(20);

			worksheet.MergeRange(new RangePosition(5, 1, 10, 10));
			AssertTrue(worksheet._Debug_Validate_MergedCells());
			AssertEquals(worksheet.RowCount, 20);

			Grid.DoAction(new RemoveRowsAction(2,3));
			AssertTrue(worksheet._Debug_Validate_All());
			AssertEquals(worksheet.RowCount, 17);

			Grid.Undo();
			AssertTrue(worksheet._Debug_Validate_All());
			AssertEquals(worksheet.RowCount, 20);
		}

		[TestCase]
		public void DeleteRangeBorder()
		{
			SetUp();

			worksheet.SetRangeBorders(new RangePosition(2, 2, 3, 5), BorderPositions.All, RangeBorderStyle.BlackSolid);
			worksheet.DeleteRows(3, 2);
			AssertTrue(worksheet._Debug_Validate_All());
		}

		[TestCase]
		public void UnionBorderSpanAfterDelete()
		{
			SetUp();

			worksheet.SetRangeBorders(new RangePosition(2, 2, 3, 3), BorderPositions.All, RangeBorderStyle.BlackSolid);
			worksheet.SetRangeBorders(new RangePosition(7, 2, 3, 3), BorderPositions.All, RangeBorderStyle.BlackSolid);
			worksheet.DeleteRows(5, 2);
			AssertTrue(worksheet._Debug_Validate_All());
		}

		[TestCase]
		public void CorssMultiMergedCells()
		{
			SetUp(20, 20);

			for (int i = 1; i < 10; i += 2)
			{
				worksheet.MergeRange(i, 1, 2, 10);
			}

			for (int i = 1; i < 18; i += 2)
			{
				worksheet.DeleteRows(1, 2);
			}

			AssertEquals(worksheet.RowCount, 2);
			AssertTrue(worksheet._Debug_Validate_All());
		}

		[TestCase]
		public void CrossMultiMergedCellUndo()
		{
			SetUp(20, 20);

			for (int i = 1; i < 10; i += 2)
			{
				worksheet.MergeRange(1, i, 10, 2);
			}

			for (int i = 1; i < 18; i += 2)
			{
				Grid.DoAction(new RemoveRowsAction(1, 2));
			}

			AssertEquals(worksheet.RowCount, 2);
			AssertTrue(worksheet._Debug_Validate_All());

			while (Grid.CanUndo())
			{
				Grid.Undo();
			}

			AssertEquals(worksheet.RowCount, 20);
			AssertTrue(worksheet._Debug_Validate_All());
		}

		[TestCase]
		public void NoAffectOtherRanges()
		{
			SetUp(20, 10);

			worksheet.MergeRange(1, 2, 8, 8);
			worksheet.MergeRange(10, 10, 8, 5);

			Grid.DoAction(new RemoveRowsAction(3, 5));

			AssertTrue(worksheet._Debug_Validate_All());
		}

		[TestCase]
		public void KeepStyleAfterDeleteMergedCell()
		{
			SetUp(20, 20);

			worksheet.MergeRange(3, 3, 3, 6);
			AssertEquals(worksheet.GetCellStyles(3, 3).BackColor, Color.Empty);

			// set style to range
			Grid.DoAction(new SetRangeStyleAction(3, 3, 3, 6, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.BackColor,
				BackColor = Color.Beige,
			}));

			AssertEquals(worksheet.GetCellStyles(3, 3).BackColor, Color.Beige);

			// remove 1 column
			Grid.DoAction(new RemoveColumnsAction(3, 1));
			AssertEquals(worksheet.GetCellStyles(3, 3).BackColor, Color.Beige);

			// undo remove 1 column
			Grid.Undo();
			AssertEquals(worksheet.GetCellStyles(3, 3).BackColor, Color.Beige);

			// remove 2 columns
			Grid.DoAction(new RemoveColumnsAction(3, 2));
			AssertEquals(worksheet.GetCellStyles(3, 3).BackColor, Color.Beige);

			// undo remove 2 columns
			Grid.Undo();
			AssertEquals(worksheet.GetCellStyles(3, 3).BackColor, Color.Beige);

			// undo to default
			Grid.Undo();
			AssertEquals(worksheet.GetCellStyles(3, 3).BackColor, Color.Empty);
		}

		[TestCase]
		void RestoreDeletedOutlines()
		{
			SetUp(20, 20);

			worksheet.GroupRows(4, 4);
			worksheet.GroupRows(4, 2);

			Grid.DoAction(new RemoveRowsAction(4, 2));
			Grid.Undo();

			var l1 = worksheet.GetOutline(RowOrColumn.Row, 4, 2);
			var l2 = worksheet.GetOutline(RowOrColumn.Row, 4, 4);
		}

		[TestCase(false)]
		void MaxContentRowAfterDelete()
		{
			SetUp(20, 20);

			worksheet[19, 0] = 1;
			worksheet.Resize(5, 5);

			AssertEquals(worksheet.MaxContentRow, 4);
		}

		/// <summary>
		/// https://reogrid.net/forum/viewtopic.php?id=316
		/// </summary>
		[TestCase]
		void AppendAndUseDeleteRows()
		{
			SetUp(20, 20);

			worksheet.Rows = 3;
			worksheet.SetRangeBorders(RangePosition.EntireRange, BorderPositions.All, RangeBorderStyle.BlackSolid);

			worksheet.Rows = 1;
			worksheet.AppendRows(1);
			worksheet.AppendRows(1);
			worksheet.AppendRows(1);
			worksheet.AppendRows(1);
		}
	}
}
