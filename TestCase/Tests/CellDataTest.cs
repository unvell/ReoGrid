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
	class CellDataTest : ReoGridTestSet
	{
		private class MyData 
		{
			public override string ToString()
			{
				return "mydata";
			}
		}

		[TestCase]
		void SetData()
		{
			SetUp();

			var dt = new DateTime(2013, 12, 12, 1, 2, 3);
			var sb = new StringBuilder("hello world");
			var mydata = new MyData();

			var objs = new object[,]{
				{1, 2, 3},
				{"a", "b", "c"},
				{'a', 'b', 'c'},
				{dt, sb, mydata},
			};

			worksheet[0, 0] = objs;

			AssertEquals(worksheet[0, 0], 1);
			AssertEquals(worksheet[0, 1], 2);
			AssertEquals(worksheet[0, 2], 3);
			AssertEquals(worksheet[1, 0], "a");
			AssertEquals(worksheet[1, 1], "b");
			AssertEquals(worksheet[1, 2], "c");
			AssertEquals(worksheet[2, 0], 'a');
			AssertEquals(worksheet[2, 1], 'b');
			AssertEquals(worksheet[2, 2], 'c');
			AssertEquals(worksheet[3, 0], dt);
			AssertEquals(worksheet[3, 1], sb);
			AssertEquals(worksheet[3, 2], mydata);
			AssertEquals(worksheet.GetCellText(3, 2), "mydata");

			worksheet[1, 1] = 12345;
			AssertEquals(worksheet[1, 1], 12345);
		}

		[TestCase]
		void DisplayText()
		{
			worksheet[5, 0] = new object[,] { { 10, "a", 'b' } };

			AssertEquals(worksheet.GetCellText(5, 0), "10");
			AssertEquals(worksheet.GetCellText(5, 1), "a");
			AssertEquals(worksheet.GetCellText(5, 2), "b");
		}

		[TestCase]
		void SetRange()
		{
			worksheet[10, 0] = new object[,] 
			{ 
				{ 10, null, null, null, 'b' } 
			};

			AssertEquals(worksheet[10, 0], 10);
			AssertEquals(worksheet[10, 2], null);
			AssertEquals(worksheet[10, 4], 'b');

			worksheet[10, 10] = new object[,] {
				{ 'a', 10, null, 15, "bbb", System.Drawing.Color.Black }
			};

			AssertEquals(worksheet[10, 10], 'a');
			AssertEquals(worksheet[10, 11], 10);
			AssertEquals(worksheet[10, 12], null);
			AssertEquals(worksheet[10, 13], 15);
			AssertEquals(worksheet[10, 14], "bbb");
			AssertEquals(worksheet[10, 15], System.Drawing.Color.Black);
		
		}

		[TestCase]
		public void ParseTabbedString()
		{
			worksheet[10, 5] = RGUtility.ParseTabbedString("A\tB\tC\t");

			AssertEquals(worksheet[10, 5], "A");
			AssertEquals(worksheet[10, 6], "B");
			AssertEquals(worksheet[10, 7], "C");

			worksheet[10, 10] = RGUtility.ParseTabbedString("A\t\tC\t");

			AssertEquals(worksheet[10, 10], "A");
			AssertEquals(worksheet[10, 11], "");
			AssertEquals(worksheet[10, 12], "C");
		
			worksheet[10, 15] = RGUtility.ParseTabbedString("A\nB\nC\n");

			AssertEquals(worksheet[10, 15], "A");
			AssertEquals(worksheet[11, 15], "B");
			AssertEquals(worksheet[12, 15], "C");
		
			worksheet[10, 20] = RGUtility.ParseTabbedString("1\t\t3\n\t\nA\t\tC");

			AssertEquals(worksheet.GetCellText(10, 20), "1");
			AssertEquals(worksheet.GetCellText(10, 22), "3");
			AssertEquals(worksheet.GetCellText(11, 20), "");
			AssertEquals(worksheet.GetCellText(11, 22), "");
			AssertEquals(worksheet.GetCellText(12, 20), "A");
			AssertEquals(worksheet.GetCellText(12, 22), "C");
		}

		[TestCase]
		public void TestMaxBounds()
		{
			SetUp(20, 20);

			worksheet[10, 10] = "A";
			AssertEquals(worksheet.MaxContentRow, 10);
			AssertEquals(worksheet.MaxContentCol, 10);

			worksheet.DeleteColumns(5, 2);
			AssertEquals(worksheet.MaxContentRow, 10, "Row, After Delete");
			AssertEquals(worksheet.MaxContentCol, 08, "Col, After Delete");

			worksheet.InsertColumns(5, 2);
			AssertEquals(worksheet.MaxContentRow, 10, "Row, After Insert");
			AssertEquals(worksheet.MaxContentCol, 10, "Col, After Insert");

			worksheet[15, 15] = "B";
			AssertEquals(worksheet.MaxContentRow, 15);
			AssertEquals(worksheet.MaxContentCol, 15);
		
			worksheet[18, 18] = null;
			AssertEquals(worksheet.MaxContentRow, 15);
			AssertEquals(worksheet.MaxContentCol, 15);
			
			worksheet[15, 15] = null;
			AssertEquals(worksheet.MaxContentRow, 15);
			AssertEquals(worksheet.MaxContentCol, 15);
		}

		[TestCase]
		void UpdateWithoutFormatAndFormula()
		{
			SetUp(20, 20);

			worksheet.SetSettings(WorksheetSettings.Edit_AutoFormatCell, false);

			worksheet[0, 0] = "10";
			AssertEquals(worksheet.GetCellText(0, 0), "10");

			worksheet[0, 1] = "'20";
			AssertEquals(worksheet.GetCellText(0, 1), "20");
		
			worksheet[0, 2] = "'=30";
			AssertEquals(worksheet.GetCellText(0, 2), "=30");
		}

		[TestCase]
		void SetToInvalidCell()
		{
			SetUp(20, 20);

			worksheet.MergeRange(0, 0, 5, 5);
			worksheet[0, 0] = "valid";
			worksheet[1, 0] = "invalid"; // nothing will be set for an invalid cell

			AssertEquals(worksheet[0, 0], "valid");
			AssertEquals(worksheet[1, 0], null); // will get null since nothing has been set into an invalid cell
		}

		[TestCase]
		void SetAnythingToNull()
		{
			worksheet[10, 10] = "hello";
			worksheet[10, 10] = null;

			AssertEquals(worksheet[10, 10], null);
		}

		[TestCase]
		void GetCellValueRaw()
		{
			SetUp(20, 20);

			worksheet[1, 1] = (int)10;
			int v1 = worksheet.GetCellData<int>(1, 1);
			AssertEquals(v1, (int)10);

			worksheet[1, 2] = (char)20;
			char v2 = worksheet.GetCellData<char>(1, 2);
			AssertEquals(v2, (char)20);

			worksheet[1, 3] = (byte)30;
			byte v3 = worksheet.GetCellData<byte>(1, 3);
			AssertEquals(v3, (byte)30);

			worksheet[1, 4] = (float)40;
			float v4 = worksheet.GetCellData<float>(1, 4);
			AssertEquals(v4, (float)40);

			worksheet[1, 5] = (short)50;
			short v5 = worksheet.GetCellData<short>(1, 5);
			AssertEquals(v5, (short)50);

			worksheet[1, 6] = (double)60;
			double v6 = worksheet.GetCellData<double>(1, 6);
			AssertEquals(v6, (double)60);

			worksheet[1, 7] = (long)70;
			long v7 = worksheet.GetCellData<long>(1, 7);
			AssertEquals(v7, (long)70);
		}

		[TestCase]
		void GetCellValueChangeType()
		{
			worksheet[2, 1] = (int)10;
			int v1 = worksheet.GetCellData<int>(2, 1);
			AssertEquals(v1, (int)10);

			worksheet[2, 2] = (char)20;
			int v2 = worksheet.GetCellData<int>(2, 2);
			AssertEquals(v2, (int)20);

			worksheet[2, 3] = (byte)30;
			int v3 = worksheet.GetCellData<int>(2, 3);
			AssertEquals(v3, (int)30);

			worksheet[2, 4] = (float)40;
			int v4 = worksheet.GetCellData<int>(2, 4);
			AssertEquals(v4, (int)40);

			worksheet[2, 5] = (short)50;
			int v5 = worksheet.GetCellData<int>(2, 5);
			AssertEquals(v5, (int)50);

			worksheet[2, 6] = (double)60;
			int v6 = worksheet.GetCellData<int>(2, 6);
			AssertEquals(v6, (int)60);

			worksheet[2, 7] = (long)70;
			int v7 = worksheet.GetCellData<int>(2, 7);
			AssertEquals(v7, (int)70);

			////////////////////////////////////////////////////////////

			worksheet[3, 1] = 10;
			double v21 = worksheet.GetCellData<double>(3, 1);
			AssertEquals(v21, (double)10);

			worksheet[3, 2] = (char)20;
			double v22 = worksheet.GetCellData<double>(3, 2);
			AssertEquals(v22, (double)20);

			worksheet[3, 3] = (byte)30;
			double v23 = worksheet.GetCellData<double>(3, 3);
			AssertEquals(v23, (double)30);

			worksheet[3, 4] = 40f;
			double v24 = worksheet.GetCellData<double>(3, 4);
			AssertEquals(v24, (double)40);

			worksheet[3, 5] = (short)50;
			double v25 = worksheet.GetCellData<double>(3, 5);
			AssertEquals(v25, (double)50);

			worksheet[3, 6] = 60D;
			double v26 = worksheet.GetCellData<double>(3, 6);
			AssertEquals(v26, (double)60);

			worksheet[3, 7] = 70L;
			double v27 = worksheet.GetCellData<double>(3, 7);
			AssertEquals(v27, (double)70);

			worksheet[3, 8] = 70M;
			double v28 = worksheet.GetCellData<double>(3, 7);
			AssertEquals(v28, (double)70);
		}

		[TestCase]
		void GetCellValueIntFromString()
		{
			// generic-cell-data-format will convert the value into number automatically
			worksheet[4, 1] = "10"; // "10" -> 10 auto convert
			AssertEquals(worksheet.GetCellData<int>(4, 1), 10);

			// set 3,2 to text
			worksheet.SetRangeDataFormat(3, 2, 1, 1, DataFormat.CellDataFormatFlag.Text, null);
			worksheet[4, 2] = "10"; // "10" without auto convert
			AssertEquals(worksheet.GetCellData<int>(4, 2), 10);
			AssertEquals(worksheet.GetCellData<double>(4, 2), 10d);
		}

		[TestCase]
		void CellDataConversion()
		{
			worksheet[5, 1] = "=TRUE";
			AssertEquals(worksheet.GetCellData<bool>(4, 1), true);
		}

		[TestCase]
		void SetData131072Rows()
		{
			SetUp(131100, 10);

			for (int r = 0; r < 100; r++)
			{
				worksheet[r, 0] = r;
			}

			for (int r = 131000; r < 131099; r++)
			{
				worksheet[r, 0] = r;
			}

			for (int r = 0; r < 100; r++)
			{
				AssertEquals(worksheet[r, 0], r);
			}

			for (int r = 131000; r < 131099; r++)
			{
				AssertEquals(worksheet[r, 0], r);
			}
		}
	}

}
