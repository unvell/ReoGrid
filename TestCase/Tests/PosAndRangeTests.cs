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

namespace unvell.ReoGrid.Tests
{
	[TestSet]
	class PosAndRangeTests : ReoGridTestSet
	{
		[TestCase]
		public void BasePos()
		{
			var pos1 = new CellPosition(0, 0);
			AssertEquals(pos1.Row, 0);
			AssertEquals(pos1.Col, 0);

			var pos2 = new CellPosition(10, 20);
			AssertEquals(pos2.Row, 10);
			AssertEquals(pos2.Col, 20);

			AssertTrue(pos1 != pos2);
			AssertEquals(pos2, new CellPosition(10, 20));
		}

		[TestCase]
		public void BaseRange()
		{
			var r1 = new RangePosition(0, 0, 1, 1);
			AssertEquals(r1.Row, 0);
			AssertEquals(r1.Col, 0);
			AssertEquals(r1.EndRow, 0);
			AssertEquals(r1.EndCol, 0);
			AssertEquals(r1.Rows, 1);
			AssertEquals(r1.Cols, 1);

			var r2 = new RangePosition(10, 20, 5, 5);
			AssertEquals(r2.Row, 10);
			AssertEquals(r2.Col, 20);
			AssertEquals(r2.EndRow, 14);
			AssertEquals(r2.EndCol, 24);
			AssertEquals(r2.Rows, 5);
			AssertEquals(r2.Cols, 5);

			AssertTrue(r1 != r2);
			AssertEquals(r2, new RangePosition(10, 20, 5, 5));
		}

		[TestCase]
		public void ValidRelativeAddress()
		{
			// cell
			AssertEquals(CellPosition.IsValidAddress("x1"), false, "x1");
			AssertEquals(CellPosition.IsValidAddress("-x1"), false);
			AssertEquals(CellPosition.IsValidAddress("A"), false);
			AssertEquals(CellPosition.IsValidAddress("1"), false);

			AssertEquals(CellPosition.IsValidAddress("B1:"), false);
			AssertEquals(CellPosition.IsValidAddress(":A1"), false);

			AssertEquals(CellPosition.IsValidAddress("A1"), true);
			AssertEquals(CellPosition.IsValidAddress(" ZZZ777 "), true);
			AssertEquals(CellPosition.IsValidAddress("AZHL1048576"), true);

			// range address
			AssertEquals(RangePosition.IsValidAddress("x1:C1"), false, "x1:C1");
			AssertEquals(RangePosition.IsValidAddress("-x1:C2"), false);
			AssertEquals(RangePosition.IsValidAddress("A:B"), true);
			AssertEquals(RangePosition.IsValidAddress("1"), false);
			AssertEquals(RangePosition.IsValidAddress("1:2"), true, "1:2");
			AssertEquals(RangePosition.IsValidAddress("1:X2"), false, "1:X2");
			AssertEquals(RangePosition.IsValidAddress("-A1:X2"), false);
			AssertEquals(RangePosition.IsValidAddress("$A1:X2"), true);

			AssertEquals(RangePosition.IsValidAddress("B1:"), false);
			AssertEquals(RangePosition.IsValidAddress(":A1"), false);

			AssertEquals(RangePosition.IsValidAddress("A1:C3"), true);
			AssertEquals(RangePosition.IsValidAddress("A1:A1"), true);
			AssertEquals(RangePosition.IsValidAddress("  A1:D5  "), true);
			AssertEquals(RangePosition.IsValidAddress("A1:AZHL1048576"), true);

			AssertEquals(RangePosition.IsValidAddress("A1"), true);
			AssertEquals(RangePosition.IsValidAddress(" A1 "), true);
			AssertEquals(RangePosition.IsValidAddress("AZHL1048576 "), true);
		}

		[TestCase]
		public void Generic()
		{
			RangePosition range;
			
			// 1,1
			range = new RangePosition("A1");
			AssertSame(range.Row, 0);
			AssertSame(range.Col, 0);
			AssertSame(range.Rows, 1);
			AssertSame(range.Cols, 1);
			AssertEquals(range.ToAddress(), "A1:A1");
			AssertEquals(range.ToRelativeAddress(), "A1:A1");
			AssertEquals(range.ToAbsoluteAddress(), "$A$1:$A$1");
			AssertEquals(range.StartPos.ToAddress(), "A1");
			AssertEquals(range.StartPos.ToRelativeAddress(), "A1");
			AssertEquals(range.StartPos.ToAbsoluteAddress(), "$A$1");
			AssertEquals(range.EndPos.ToAddress(), "A1");
			AssertEquals(range.EndPos.ToRelativeAddress(), "A1");
			AssertEquals(range.EndPos.ToAbsoluteAddress(), "$A$1");

			range = new RangePosition("$A$1");
			AssertSame(range.Row, 0);
			AssertSame(range.Col, 0);
			AssertSame(range.Rows, 1);
			AssertSame(range.Cols, 1);
			AssertEquals(range.ToAddress(), "$A$1:$A$1");
			AssertEquals(range.ToRelativeAddress(), "A1:A1");
			AssertEquals(range.ToAbsoluteAddress(), "$A$1:$A$1");
			AssertEquals(range.StartPos.ToAddress(), "$A$1");
			AssertEquals(range.StartPos.ToRelativeAddress(), "A1");
			AssertEquals(range.StartPos.ToAbsoluteAddress(), "$A$1");
			AssertEquals(range.EndPos.ToAddress(), "$A$1");
			AssertEquals(range.EndPos.ToRelativeAddress(), "A1");
			AssertEquals(range.EndPos.ToAbsoluteAddress(), "$A$1");

			range = new RangePosition("C53:C60");
			AssertSame(range.Row, 52);
			AssertSame(range.Col, 2);
			AssertSame(range.Rows, 8);
			AssertSame(range.Cols, 1);
			AssertEquals(range.ToAddress(), "C53:C60");
			AssertEquals(range.ToRelativeAddress(), "C53:C60");
			AssertEquals(range.ToAbsoluteAddress(), "$C$53:$C$60");
			AssertEquals(range.StartPos.ToAddress(), "C53");
			AssertEquals(range.StartPos.ToRelativeAddress(), "C53");
			AssertEquals(range.StartPos.ToAbsoluteAddress(), "$C$53");
			AssertEquals(range.EndPos.ToAddress(), "C60");
			AssertEquals(range.EndPos.ToRelativeAddress(), "C60");
			AssertEquals(range.EndPos.ToAbsoluteAddress(), "$C$60");

			range = new RangePosition("$C$53:$C$60");
			AssertSame(range.Row, 52);
			AssertSame(range.Col, 2);
			AssertSame(range.Rows, 8);
			AssertSame(range.Cols, 1);
			AssertEquals(range.ToAddress(), "$C$53:$C$60");
			AssertEquals(range.ToRelativeAddress(), "C53:C60");
			AssertEquals(range.ToAbsoluteAddress(), "$C$53:$C$60");
			AssertEquals(range.StartPos.ToAddress(), "$C$53");
			AssertEquals(range.StartPos.ToRelativeAddress(), "C53");
			AssertEquals(range.StartPos.ToAbsoluteAddress(), "$C$53");
			AssertEquals(range.EndPos.ToAddress(), "$C$60");
			AssertEquals(range.EndPos.ToRelativeAddress(), "C60");
			AssertEquals(range.EndPos.ToAbsoluteAddress(), "$C$60");

			range = new RangePosition("$C$53:C60");
			AssertSame(range.Row, 52);
			AssertSame(range.Col, 2);
			AssertSame(range.Rows, 8);
			AssertSame(range.Cols, 1);
			AssertEquals(range.ToAddress(), "$C$53:C60");
			AssertEquals(range.ToRelativeAddress(), "C53:C60");
			AssertEquals(range.ToAbsoluteAddress(), "$C$53:$C$60");
			AssertEquals(range.StartPos.ToAddress(), "$C$53");
			AssertEquals(range.StartPos.ToRelativeAddress(), "C53");
			AssertEquals(range.StartPos.ToAbsoluteAddress(), "$C$53");
			AssertEquals(range.EndPos.ToAddress(), "C60");
			AssertEquals(range.EndPos.ToRelativeAddress(), "C60");
			AssertEquals(range.EndPos.ToAbsoluteAddress(), "$C$60");

			range = new RangePosition("C53:$C$60");
			AssertSame(range.Row, 52);
			AssertSame(range.Col, 2);
			AssertSame(range.Rows, 8);
			AssertSame(range.Cols, 1);
			AssertEquals(range.ToAddress(), "C53:$C$60");
			AssertEquals(range.ToRelativeAddress(), "C53:C60");
			AssertEquals(range.ToAbsoluteAddress(), "$C$53:$C$60");
			AssertEquals(range.StartPos.ToAddress(), "C53");
			AssertEquals(range.StartPos.ToRelativeAddress(), "C53");
			AssertEquals(range.StartPos.ToAbsoluteAddress(), "$C$53");
			AssertEquals(range.EndPos.ToAddress(), "$C$60");
			AssertEquals(range.EndPos.ToRelativeAddress(), "C60");
			AssertEquals(range.EndPos.ToAbsoluteAddress(), "$C$60");

			// 32768,1048576
			range = new RangePosition("$XFD$1048576");
			AssertSame(range.Row, 1048575);
			AssertSame(range.Col, 16383);
			AssertSame(range.Rows, 1);
			AssertSame(range.Cols, 1);
			AssertEquals(range.ToAddress(), "$XFD$1048576:$XFD$1048576");
			AssertEquals(range.ToRelativeAddress(), "XFD1048576:XFD1048576");
			AssertEquals(range.ToAbsoluteAddress(), "$XFD$1048576:$XFD$1048576");
			AssertEquals(range.StartPos.ToAddress(), "$XFD$1048576");
			AssertEquals(range.StartPos.ToRelativeAddress(), "XFD1048576");
			AssertEquals(range.StartPos.ToAbsoluteAddress(), "$XFD$1048576");
			AssertEquals(range.EndPos.ToAddress(), "$XFD$1048576");
			AssertEquals(range.EndPos.ToRelativeAddress(), "XFD1048576");
			AssertEquals(range.EndPos.ToAbsoluteAddress(), "$XFD$1048576");

			// full rows
			range = new RangePosition("1:12");
			AssertSame(range.Row, 0);
			AssertSame(range.Rows, 12);
			AssertSame(range.Col, 0);
			AssertSame(range.Cols, -1);
			AssertEquals(range.ToAddress(), "1:12");
			AssertEquals(range.ToRelativeAddress(), "1:12");
			AssertEquals(range.ToAbsoluteAddress(), "$1:$12");
			AssertEquals(range.StartPos.ToAddress(), "A1");
			AssertEquals(range.StartPos.ToRelativeAddress(), "A1");
			AssertEquals(range.StartPos.ToAbsoluteAddress(), "$A$1");
			AssertEquals(range.EndPos.ToAddress(), "AVLI12");
			AssertEquals(range.EndPos.ToRelativeAddress(), "AVLI12");
			AssertEquals(range.EndPos.ToAbsoluteAddress(), "$AVLI$12");

			// full rows
			range = new RangePosition("$1:$12");
			AssertSame(range.Row, 0);
			AssertSame(range.Rows, 12);
			AssertSame(range.Col, 0);
			AssertSame(range.Cols, -1);
			AssertEquals(range.ToAddress(), "$1:$12");
			AssertEquals(range.ToRelativeAddress(), "1:12");
			AssertEquals(range.ToAbsoluteAddress(), "$1:$12");
			AssertEquals(range.StartPos.ToAddress(), "A$1");
			AssertEquals(range.StartPos.ToRelativeAddress(), "A1");
			AssertEquals(range.StartPos.ToAbsoluteAddress(), "$A$1");
			AssertEquals(range.EndPos.ToAddress(), "AVLI$12");
			AssertEquals(range.EndPos.ToRelativeAddress(), "AVLI12");
			AssertEquals(range.EndPos.ToAbsoluteAddress(), "$AVLI$12");

			// full rows
			range = new RangePosition("$1:12");
			AssertSame(range.Row, 0);
			AssertSame(range.Rows, 12);
			AssertSame(range.Col, 0);
			AssertSame(range.Cols, -1);
			AssertEquals(range.ToAddress(), "$1:12");
			AssertEquals(range.ToRelativeAddress(), "1:12");
			AssertEquals(range.ToAbsoluteAddress(), "$1:$12");
			AssertEquals(range.StartPos.ToAddress(), "A$1");
			AssertEquals(range.StartPos.ToRelativeAddress(), "A1");
			AssertEquals(range.StartPos.ToAbsoluteAddress(), "$A$1");
			AssertEquals(range.EndPos.ToAddress(), "AVLI12");
			AssertEquals(range.EndPos.ToRelativeAddress(), "AVLI12");
			AssertEquals(range.EndPos.ToAbsoluteAddress(), "$AVLI$12");
			
			// full rows
			range = new RangePosition("1:$12");
			AssertSame(range.Row, 0);
			AssertSame(range.Rows, 12);
			AssertSame(range.Col, 0);
			AssertSame(range.Cols, -1);
			AssertEquals(range.ToAddress(), "1:$12");
			AssertEquals(range.ToRelativeAddress(), "1:12");
			AssertEquals(range.ToAbsoluteAddress(), "$1:$12");

			// full cols
			range = new RangePosition("B:C");
			AssertSame(range.Row, 0);
			AssertSame(range.Rows, -1);
			AssertSame(range.Col, 1);
			AssertSame(range.Cols, 2);
			AssertEquals(range.ToAddress(), "B:C");
			AssertEquals(range.ToRelativeAddress(), "B:C");
			AssertEquals(range.ToAbsoluteAddress(), "$B:$C");

			// full cols
			range = new RangePosition("$B:C");
			AssertSame(range.Row, 0);
			AssertSame(range.Rows, -1);
			AssertSame(range.Col, 1);
			AssertSame(range.Cols, 2);
			AssertEquals(range.ToAddress(), "$B:C");
			AssertEquals(range.ToRelativeAddress(), "B:C");
			AssertEquals(range.ToAbsoluteAddress(), "$B:$C");

			// full cols
			range = new RangePosition("B:$C");
			AssertSame(range.Row, 0);
			AssertSame(range.Rows, -1);
			AssertSame(range.Col, 1);
			AssertSame(range.Cols, 2);
			AssertEquals(range.ToAddress(), "B:$C");
			AssertEquals(range.ToRelativeAddress(), "B:C");
			AssertEquals(range.ToAbsoluteAddress(), "$B:$C");
		
			// full cols
			range = new RangePosition("$A:$G");
			AssertSame(range.Row, 0);
			AssertSame(range.Rows, -1);
			AssertSame(range.Col, 0);
			AssertSame(range.Cols, 7);
			AssertEquals(range.ToAddress(), "$A:$G");
			AssertEquals(range.ToRelativeAddress(), "A:G");
			AssertEquals(range.ToAbsoluteAddress(), "$A:$G");


		}

		[TestCase]
		void MixedRelativeAndAbsolute()
		{
			CellPosition pos;
			RangePosition range;

			pos = new CellPosition("$A1");
			AssertEquals(pos.Col, 0);
			AssertEquals(pos.Row, 0);
			AssertEquals("$A1", pos.ToAddress());
			AssertEquals("A1", pos.ToRelativeAddress());
			AssertEquals("$A$1", pos.ToAbsoluteAddress());

			pos = new CellPosition("Z$10");
			AssertEquals(pos.Col, 25);
			AssertEquals(pos.Row, 9);
			AssertEquals("Z$10", pos.ToAddress());
			AssertEquals("Z10", pos.ToRelativeAddress());
			AssertEquals("$Z$10", pos.ToAbsoluteAddress());

			// full cols
			range = new RangePosition("$A1:B2");
			AssertSame(range.Row, 0);
			AssertSame(range.Rows, 2);
			AssertSame(range.Col, 0);
			AssertSame(range.Cols, 2);
			AssertEquals("$A1:B2", range.ToAddress());
			AssertEquals("A1:B2", range.ToRelativeAddress());
			AssertEquals("$A$1:$B$2", range.ToAbsoluteAddress());
		}

		[TestCase]
		public void AddressConvert()
		{
			var pos1 = new CellPosition(0, 0);
			var pos2 = new CellPosition(10, 20);

			AssertEquals(pos1.ToAddress(), "A1");
			AssertEquals(pos2.ToAddress(), "U11");

			AssertEquals(pos1, new CellPosition("A1"));
			AssertEquals(pos2, new CellPosition("U11"));

			var r1 = new RangePosition(0, 0, 1, 1);
			var r2 = new RangePosition(0, 0, 3, 3);
			var r3 = new RangePosition(10, 20, 5, 5);

			AssertEquals(r1.ToAddress(), "A1:A1");
			AssertEquals(r2.ToAddress(), "A1:C3");
			AssertEquals(r3.ToAddress(), "U11:Y15");

			AssertEquals(r1, new RangePosition("A1:A1"));
			AssertEquals(r2, new RangePosition("A1:C3"));
			AssertEquals(r3, new RangePosition("U11:Y15"));

			AssertEquals(new RangePosition("A1:A1"), new RangePosition(0, 0, 1, 1));
		}

		[TestCase]
		public void AddressConvert2()
		{
			AssertEquals(new CellPosition("Z1").ToAddress(), "Z1");
			AssertEquals(new CellPosition("AA1").ToAddress(), "AA1");
			AssertEquals(new CellPosition("ZZ1").ToAddress(), "ZZ1");
		}

		[TestCase]
		public void IntersectTest()
		{
			RangePosition r1 = new RangePosition(5, 5, 5, 5);
			RangePosition r2 = new RangePosition(10, 5, 5, 5); // bottom
			RangePosition r3 = new RangePosition(5, 10, 5, 5);
			RangePosition r4 = new RangePosition(0, 5, 5, 5);
			RangePosition r5 = new RangePosition(5, 0, 5, 5);

			AssertTrue(!r1.IntersectWith(r2));
			AssertTrue(!r1.IntersectWith(r3));
			AssertTrue(!r1.IntersectWith(r4));
			AssertTrue(!r1.IntersectWith(r5));

			AssertTrue(!r2.IntersectWith(r1));
			AssertTrue(!r2.IntersectWith(r3));
			AssertTrue(!r2.IntersectWith(r4));
			AssertTrue(!r2.IntersectWith(r5));

			RangePosition r6 = new RangePosition(50, 50, 5, 5);
			RangePosition r7 = new RangePosition(46, 50, 5, 5);
			RangePosition r8 = new RangePosition(50, 46, 5, 5);
			RangePosition r9 = new RangePosition(54, 50, 5, 5);
			RangePosition r10 = new RangePosition(50, 54, 5, 5);

			AssertTrue(r6.IntersectWith(r7));
			AssertTrue(r6.IntersectWith(r8));
			AssertTrue(r6.IntersectWith(r9));
			AssertTrue(r6.IntersectWith(r10));

		}

		[TestCase]
		void NamedRangeTest()
		{
			SetUp(30, 30);

			var range = worksheet.DefineNamedRange("r1", "B3:C5");

			AssertSame(range.Row, 2);
			AssertSame(range.Col, 1);
			AssertSame(range.Rows, 3);
			AssertSame(range.Cols, 2);

			range.Data = "hello";
			var data = range.Data as object[,];

			AssertSame(data[0, 0], "hello");
			AssertSame(data[0, 0], worksheet["B3"]);
		}

		[TestCase]
		void InsertRowBeforeNamedRange()
		{
			var r1 = worksheet.GetNamedRange("r1");
			AssertSame(r1.Row, 2);
			AssertSame(r1.Col, 1);
			AssertSame(r1.Rows, 3);
			AssertSame(r1.Cols, 2);
			AssertEquals("B3:C5", r1.ToAddress());
			AssertEquals("$B$3:$C$5", r1.ToAbsoluteAddress());

			var r2 = worksheet.DefineNamedRange("r2", "C8:E11");
			AssertSame(r2.Row, 7);
			AssertSame(r2.Col, 2);
			AssertSame(r2.Rows, 4);
			AssertSame(r2.Cols, 3);
			AssertEquals("C8:E11", r2.ToAddress());
			AssertEquals("$C$8:$E$11", r2.ToAbsoluteAddress());

			// insert before r2, after r1
			worksheet.InsertRows(6, 2);

			// r1 must be not changed
			AssertSame(r1.Row, 2);
			AssertSame(r1.Col, 1);
			AssertSame(r1.Rows, 3);
			AssertSame(r1.Cols, 2);
			AssertEquals("B3:C5", r1.ToAddress());
			AssertEquals("$B$3:$C$5", r1.ToAbsoluteAddress());

			// r2 must be changed
			AssertSame(r2.Row, 9);
			AssertSame(r2.Col, 2);
			AssertSame(r2.Rows, 4);
			AssertSame(r2.Cols, 3);
			AssertEquals("C10:E13", r2.ToAddress());
			AssertEquals("$C$10:$E$13", r2.ToAbsoluteAddress());
		}

		[TestCase]
		void InsertRowInsideNamedRange()
		{
			// insert before r2, after r1
			worksheet.InsertRows(3, 1);

			// r1 must be expended
			var r1 = worksheet.GetNamedRange("r1");
			AssertSame(r1.Row, 2);
			AssertSame(r1.Col, 1);
			AssertSame(r1.Rows, 4);
			AssertSame(r1.Cols, 2);
			AssertEquals("B3:C6", r1.ToAddress());
			AssertEquals("$B$3:$C$6", r1.ToAbsoluteAddress());

			// r2 must be moved down
			var r2 = worksheet.GetNamedRange("r2");
			AssertSame(r2.Row, 10);
			AssertSame(r2.Col, 2);
			AssertSame(r2.Rows, 4);
			AssertSame(r2.Cols, 3);
			AssertEquals("C11:E14", r2.ToAddress());
			AssertEquals("$C$11:$E$14", r2.ToAbsoluteAddress());
		}
	}
}
