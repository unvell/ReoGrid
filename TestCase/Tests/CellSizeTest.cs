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
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace unvell.ReoGrid.Tests
{
	[TestSet]
	class CellSizeTest : ReoGridTestSet
	{
		ushort zero = 0;

		[TestCase]
		void HideRowsTest()
		{
			SetUp(20, 20);

			AssertEquals(worksheet.GetRowHeight(2), Worksheet.InitDefaultRowHeight);
			AssertEquals(worksheet.GetRowHeight(3), Worksheet.InitDefaultRowHeight);

			// normal hide
			worksheet.HideRows(2, 2);
			AssertEquals(worksheet.GetRowHeight(2), zero);
			AssertEquals(worksheet.GetRowHeight(3), zero);
			AssertEquals(worksheet.GetRowHeight(4), Worksheet.InitDefaultRowHeight);

			// restore height of rows
			worksheet.ShowRows(2, 2);
			AssertEquals(worksheet.GetRowHeight(2), Worksheet.InitDefaultRowHeight);
			AssertEquals(worksheet.GetRowHeight(3), Worksheet.InitDefaultRowHeight);
			AssertEquals(worksheet.GetRowHeight(4), Worksheet.InitDefaultRowHeight);

			// repeat to show rows
			worksheet.ShowRows(2, 2);
			AssertEquals(worksheet.GetRowHeight(2), Worksheet.InitDefaultRowHeight);
			AssertEquals(worksheet.GetRowHeight(3), Worksheet.InitDefaultRowHeight);
			AssertEquals(worksheet.GetRowHeight(4), Worksheet.InitDefaultRowHeight);

			// show rows after adjust height
			worksheet.SetRowsHeight(2, 2, 30);
			worksheet.ShowRows(2, 2);
			AssertEquals(worksheet.GetRowHeight(2), (ushort)30);
			AssertEquals(worksheet.GetRowHeight(3), (ushort)30);
			AssertEquals(worksheet.GetRowHeight(4), Worksheet.InitDefaultRowHeight);

			// hide adjusted height
			worksheet.HideRows(2, 2);
			AssertEquals(worksheet.GetRowHeight(2), zero);
			AssertEquals(worksheet.GetRowHeight(3), zero);
			AssertEquals(worksheet.GetRowHeight(4), Worksheet.InitDefaultRowHeight);

			// restore adjusted height
			worksheet.ShowRows(2, 2);
			AssertEquals(worksheet.GetRowHeight(2), (ushort)30);
			AssertEquals(worksheet.GetRowHeight(3), (ushort)30);
			AssertEquals(worksheet.GetRowHeight(4), Worksheet.InitDefaultRowHeight);
		}

		[TestCase]
		void HideColumnsTest()
		{
			SetUp(20, 20);

			AssertEquals(worksheet.GetColumnWidth(2), Worksheet.InitDefaultColumnWidth);
			AssertEquals(worksheet.GetColumnWidth(3), Worksheet.InitDefaultColumnWidth);

			// normal hide
			worksheet.HideColumns(2, 2);
			AssertEquals(worksheet.GetColumnWidth(2), zero);
			AssertEquals(worksheet.GetColumnWidth(3), zero);
			AssertEquals(worksheet.GetColumnWidth(4), Worksheet.InitDefaultColumnWidth);

			// restore height of columns
			worksheet.ShowColumns(2, 2);
			AssertEquals(worksheet.GetColumnWidth(2), Worksheet.InitDefaultColumnWidth);
			AssertEquals(worksheet.GetColumnWidth(3), Worksheet.InitDefaultColumnWidth);
			AssertEquals(worksheet.GetColumnWidth(4), Worksheet.InitDefaultColumnWidth);

			// repeat to show columns
			worksheet.ShowColumns(2, 2);
			AssertEquals(worksheet.GetColumnWidth(2), Worksheet.InitDefaultColumnWidth);
			AssertEquals(worksheet.GetColumnWidth(3), Worksheet.InitDefaultColumnWidth);
			AssertEquals(worksheet.GetColumnWidth(4), Worksheet.InitDefaultColumnWidth);

			// show rows after adjust width
			worksheet.SetColumnsWidth(2, 2, 30);
			worksheet.ShowColumns(2, 2);
			AssertEquals(worksheet.GetColumnWidth(2), (ushort)30);
			AssertEquals(worksheet.GetColumnWidth(3), (ushort)30);
			AssertEquals(worksheet.GetColumnWidth(4), Worksheet.InitDefaultColumnWidth);

			// hide adjusted width
			worksheet.HideColumns(2, 2);
			AssertEquals(worksheet.GetColumnWidth(2), zero);
			AssertEquals(worksheet.GetColumnWidth(3), zero);
			AssertEquals(worksheet.GetColumnWidth(4), Worksheet.InitDefaultColumnWidth);

			// restore adjusted width
			worksheet.ShowColumns(2, 2);
			AssertEquals(worksheet.GetColumnWidth(2), (ushort)30);
			AssertEquals(worksheet.GetColumnWidth(3), (ushort)30);
			AssertEquals(worksheet.GetColumnWidth(4), Worksheet.InitDefaultColumnWidth);
		}

		[TestCase]
		void CellIsHiddenPropertyTest()
		{
			worksheet.HideRows(10, 2);

			AssertTrue(worksheet.IsCellVisible(9, 0));
			AssertTrue(!worksheet.IsCellVisible(10, 0));
			AssertTrue(!worksheet.IsCellVisible(11, 0));
			AssertTrue(worksheet.IsCellVisible(12, 0));

			AssertTrue(worksheet.Cells[9, 0].IsVisible);
			AssertTrue(!worksheet.Cells[10, 0].IsVisible);
			AssertTrue(!worksheet.Cells[11, 0].IsVisible);
			AssertTrue(worksheet.Cells[12, 0].IsVisible);
		}
	}

}
