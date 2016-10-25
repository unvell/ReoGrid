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
using System.Diagnostics;
using System.IO;

using unvell.ReoGrid.Actions;

namespace unvell.ReoGrid.Tests
{
	[TestSet]
	class DeleteColsTest : ReoGridTestSet
	{
		[TestCase]
		public void ValidColCountAfterDelete()
		{
			SetUp();

			// normal 
			worksheet.SetCols(7);
			AssertEquals(worksheet.ColumnCount, 7);
			worksheet.DeleteColumns(2, 5);
			AssertEquals(worksheet.ColumnCount, 2);

			// row count overflow
			worksheet.SetCols(10);
			AssertEquals(worksheet.ColumnCount, 10);
			worksheet.DeleteColumns(8, 2);
			AssertEquals(worksheet.ColumnCount, 8);
		}

		[TestCase]
		public void DeleteRangeBorder()
		{
			SetUp();

			worksheet.SetRangeBorders(new RangePosition(2, 2, 3, 5), BorderPositions.All, RangeBorderStyle.BlackSolid);
			worksheet.SetRangeBorders(new RangePosition(2, 7, 3, 5), BorderPositions.All, RangeBorderStyle.BlackSolid);

			worksheet.DeleteColumns(3, 5);

			AssertTrue(worksheet._Debug_Validate_All());
		}

		[TestCase]
		public void DeleteOnBoundary()
		{
			SetUp();

			int colcount = worksheet.ColumnCount;

			worksheet.DeleteColumns(0, 1);
			worksheet.DeleteColumns(worksheet.ColumnCount - 1, 1);
			worksheet.DeleteColumns(0, 2);
			worksheet.DeleteColumns(worksheet.ColumnCount - 2, 2);

			AssertEquals(colcount - 6, worksheet.ColumnCount);
		}

		[TestCase]
		public void DeleteInMergedCell()
		{
			SetUp();

			worksheet.SetCols(20);

			worksheet.MergeRange(new RangePosition(1, 1, 10, 10));
			AssertTrue(worksheet._Debug_Validate_MergedCells());
			AssertEquals(worksheet.ColumnCount, 20);

			worksheet.DeleteColumns(3, 3);
			AssertTrue(worksheet._Debug_Validate_All());
			AssertEquals(worksheet.ColumnCount, 17);

			worksheet.DeleteColumns(4, 10);
			AssertTrue(worksheet._Debug_Validate_All());
			AssertEquals(worksheet.ColumnCount, 7);
		}

		[TestCase]
		public void DeleteCorssOneMergedCell()
		{
			SetUp();

			worksheet.SetCols(20);
			AssertEquals(worksheet.ColumnCount, 20);

			worksheet.MergeRange(new RangePosition(1, 5, 10, 10));
			AssertTrue(worksheet._Debug_Validate_MergedCells());
			worksheet.DeleteColumns(3, 5);
			AssertTrue(worksheet._Debug_Validate_All());
			AssertEquals(worksheet.ColumnCount, 15);

			worksheet.DeleteColumns(10, 5);
			AssertTrue(worksheet._Debug_Validate_All());
			AssertEquals(worksheet.ColumnCount, 10);
		}

		[TestCase]
		public void DeleteLeftAndRightMergedCell()
		{
			SetUp();

			worksheet.SetCols(20);

			worksheet.MergeRange(new RangePosition(1, 5, 10, 10));
			worksheet.DeleteColumns(2, 3);
			AssertTrue(worksheet._Debug_Validate_All());
			AssertEquals(worksheet.ColumnCount, 17);

			worksheet.DeleteColumns(12, 3);
			AssertTrue(worksheet._Debug_Validate_All());
			AssertEquals(worksheet.ColumnCount, 14);
		}

		[TestCase]
		public void DeleteCorssTwoMergedCells()
		{
			SetUp();

			worksheet.SetCols(20);
			worksheet.MergeRange(new RangePosition(1, 2, 5, 5));
			worksheet.MergeRange(new RangePosition(1, 8, 5, 5));

			worksheet.DeleteColumns(5, 5);
			AssertTrue(worksheet._Debug_Validate_All());
			AssertEquals(worksheet.ColumnCount, 15);

			worksheet.DeleteColumns(2, 11);
			AssertTrue(worksheet._Debug_Validate_All());
			AssertEquals(worksheet.ColumnCount, 4);
		}

		[TestCase]
		public void DeleteCrossBorder()
		{
			SetUp();

			worksheet.SetRangeBorders(new RangePosition(2, 2, 3, 5), BorderPositions.All, RangeBorderStyle.BlackSolid);
			worksheet.DeleteColumns(3, 2);
			AssertTrue(worksheet._Debug_Validate_All());
		}

		[TestCase]
		public void MergeBorderAfterDelete()
		{
			SetUp();

			worksheet.SetRangeBorders(new RangePosition(2, 2, 3, 3), BorderPositions.All, RangeBorderStyle.BlackSolid);
			worksheet.SetRangeBorders(new RangePosition(2, 7, 3, 3), BorderPositions.All, RangeBorderStyle.BlackSolid);
			worksheet.DeleteColumns(5, 2);
			AssertTrue(worksheet._Debug_Validate_All());
		}

		[TestCase]
		public void DeleteHalfSideBorder()
		{
			SetUp();

			worksheet.SetRangeBorders(new RangePosition(2, 2, 3, 5), BorderPositions.All, RangeBorderStyle.BlackSolid);
			worksheet.DeleteColumns(1, 3);
			AssertTrue(worksheet._Debug_Validate_All());

			worksheet.SetRangeBorders(new RangePosition(2, 2, 3, 5), BorderPositions.All, RangeBorderStyle.BlackSolid);
			worksheet.DeleteColumns(2, 3);
			AssertTrue(worksheet._Debug_Validate_All());
		}

		// this case cannot be passed 100% (fail 10%)
		//
		// Unknown reason
		//
		[TestCase(false)]
		public void DeleteRandomBorder()
		{
			SetUp();
			var rand = new Random();
			for (int i = 0; i < 20; i++)
			{
				int r = rand.Next(30);
				int c = rand.Next(30);
				int rs = rand.Next(30);
				int cs = rand.Next(30);

				worksheet.SetRangeBorders(new RangePosition(r, c, rs, cs), BorderPositions.Outside, RangeBorderStyle.BlackSolid);
			}
			AssertTrue(worksheet._Debug_Validate_All());

			MemoryStream ms = new MemoryStream(40960);

			for (int i = 0; i < 20; i++)
			{
				int c = rand.Next(20);
				int cc = rand.Next(5);

				ms.Seek(0, SeekOrigin.Begin);
				ms.SetLength(0);

				TestHelper.DumpGrid(Worksheet, ms);

				worksheet.DeleteColumns(c, cc);
				var rs = worksheet._Debug_Validate_All();

				if(!rs)
				{
					using (var fs = new FileStream("randomly-delcols-hborder-before.txt", FileMode.Create, FileAccess.Write))
					{
						byte[] buf = ms.ToArray();
						fs.Write(buf, 0, buf.Length);
					}

					Process.Start("randomly-delcols-hborder-before.txt");

					TestHelper.DumpGrid(Worksheet, "randomly-delcols-hborder-after.txt");
					Process.Start("randomly-delcols-hborder-after.txt");

					AssertTrue(false);
				}
			}
		}

		[TestCase]
		public void BoundsTest()
		{
			SetUp(20, 20);

			worksheet.SetRangeBorders(RangePosition.EntireRange, BorderPositions.All, RangeBorderStyle.BlackSolid);
			AssertTrue(worksheet._Debug_Validate_All());

			worksheet.DeleteColumns(19, 1);
			AssertEquals(worksheet.ColumnCount, 19);
			AssertTrue(worksheet._Debug_Validate_All());
		}

		[TestCase]
		public void CorssMultiMergedCells()
		{
			SetUp(20, 20);

			for (int i = 1; i < 10; i += 2)
			{
				worksheet.MergeRange(i, i, 2, 10);
			}

			for (int i = 1; i < 18; i += 2)
			{
				worksheet.DeleteColumns(1, 2);
			}

			AssertEquals(worksheet.ColumnCount, 2);
			AssertTrue(worksheet._Debug_Validate_All());
		}

		[TestCase]
		public void CrossMultiMergedCellUndo()
		{
			SetUp(20, 20);

			for (int i = 1; i < 10; i += 2)
			{
				worksheet.MergeRange(i, 1, 2, 10);
			}

			for (int i = 1; i < 18; i += 2)
			{
				Grid.DoAction(new RemoveColumnsAction(1, 2));
			}

			AssertEquals(worksheet.ColumnCount, 2);
			AssertTrue(worksheet._Debug_Validate_All());

			while (Grid.CanUndo())
			{
				Grid.Undo();
			}

			AssertEquals(worksheet.ColumnCount, 20);
			AssertTrue(worksheet._Debug_Validate_All());
		}

		[TestCase]
		public void NoAffectOtherRanges()
		{
			SetUp(20, 20);

			worksheet.MergeRange(2, 1, 8, 8);
			worksheet.MergeRange(10, 10, 5, 8);

			Grid.DoAction(new RemoveColumnsAction(5, 3));

			AssertTrue(worksheet._Debug_Validate_All());
		}

		[TestCase]
		public void RemoveUndoRemove()
		{
			worksheet.Resize(11, 21);

			AssertEquals(worksheet.RowCount, 11);
			AssertEquals(worksheet.ColumnCount, 21);

			// last cell
			worksheet[10, 20] = "A";

			// remove
			Grid.DoAction(new RemoveColumnsAction(2, 1));
			AssertTrue(worksheet._Debug_Validate_All());
			AssertEquals(worksheet.ColumnCount, 20);

			// undo
			Grid.Undo();
			AssertTrue(worksheet._Debug_Validate_All());
			AssertEquals(worksheet.ColumnCount, 21);

			// remove
			Grid.DoAction(new RemoveColumnsAction(2, 1));
			AssertTrue(worksheet._Debug_Validate_All());
			AssertEquals(worksheet.ColumnCount, 20);

			// undo
			Grid.Undo();
			AssertTrue(worksheet._Debug_Validate_All());
			AssertEquals(worksheet.ColumnCount, 21);

			AssertEquals(worksheet.GetCellData(10, 20), "A");
		}

		[TestCase]
		void RestoreDeletedOutlines()
		{
			SetUp(20, 20);

			worksheet.GroupColumns(4, 4);
			worksheet.GroupColumns(4, 2);

			Grid.DoAction(new RemoveColumnsAction(4, 2));
			Grid.Undo();

			var l1 = worksheet.GetOutline(RowOrColumn.Column, 4, 2);
			var l2 = worksheet.GetOutline(RowOrColumn.Column, 4, 4);
		}

		[TestCase(false)]
		void MaxContentColAfterDelete()
		{
			SetUp(20, 20);

			worksheet[0, 19] = 1;
			worksheet.Resize(5, 5);

			AssertEquals(worksheet.MaxContentCol, 4);
		}
	}
}
