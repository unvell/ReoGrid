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

using unvell.ReoGrid.Drawing.Shapes;

namespace unvell.ReoGrid.Tests
{
	[TestSet]
	class FloatingObjectsTest : ReoGridTestSet
	{
		[TestCase]
		void InsertAndDeleteRows()
		{
			SetUp(20, 20);

			ushort defHeight = Worksheet.InitDefaultRowHeight;

			var fo1 = new RectangleShape
				{
					Bounds = new Graphics.Rectangle(30, 30, 100, 100),
				};

			worksheet.FloatingObjects.Add(fo1);

			int changedHeight = defHeight * 2;

			// Adding

			AssertSame(fo1.Top, 30);
			worksheet.InsertRows(0, 2);
			AssertSame(fo1.Top, 30 + changedHeight);

			AssertSame(fo1.Height, 100);
			worksheet.InsertRows(4, 2);
			AssertSame(fo1.Height, 100 + changedHeight);

			worksheet.InsertRows(12, 2);
			AssertSame(fo1.Bottom, 30 + 100 + changedHeight * 2);
			AssertSame(fo1.Height, 100 + changedHeight);

			// Deleting

			worksheet.DeleteRows(12, 2);
			AssertSame(fo1.Bottom, 30 + 100 + changedHeight * 2);
			AssertSame(fo1.Height, 100 + changedHeight);

			worksheet.DeleteRows(0, 2);
			AssertSame(fo1.Top, 30);
			AssertSame(fo1.Height, 100 + changedHeight);

			worksheet.DeleteRows(4, 2);
			AssertSame(fo1.Top, 30);
			AssertSame(fo1.Height, 100);
		}

		[TestCase]
		void InsertAndDeleteColumns()
		{
			int changedWidth = Worksheet.InitDefaultColumnWidth * 2;

			var fo1 = worksheet.FloatingObjects[0];
			fo1.X = 100;
			fo1.Width = 200;
			
			// Adding

			AssertSame(fo1.Left, 100);
			worksheet.InsertColumns(0, 2);
			AssertSame(fo1.Left, 100 + changedWidth);

			AssertSame(fo1.Width, 200);
			worksheet.InsertColumns(4, 2);
			AssertSame(fo1.Width, 200 + changedWidth);

			worksheet.InsertColumns(12, 2);
			AssertSame(fo1.Right, 100 + 200 + changedWidth * 2);
			AssertSame(fo1.Width, 200 + changedWidth);

			// Deleting

			worksheet.DeleteColumns(12, 2);
			AssertSame(fo1.Right, 100 + 200 + changedWidth * 2);
			AssertSame(fo1.Width, 200 + changedWidth);

			worksheet.DeleteColumns(0, 2);
			AssertSame(fo1.Left, 100);
			AssertSame(fo1.Width, 200 + changedWidth);

			worksheet.DeleteColumns(4, 2);
			AssertSame(fo1.Left, 100);
			AssertSame(fo1.Width, 200);
		}
	}
}
