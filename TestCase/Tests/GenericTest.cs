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

using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System;
using System.IO;

using unvell.ReoGrid;
using unvell.ReoGrid.Data;
using System.Runtime.InteropServices;
using unvell.ReoGrid.Tests;

namespace unvell.ReoGrid.Tests
{
	[TestSet]
	class PerformanceTest : ReoGridTestSet
	{
		[TestCase(DebugEnabled = true)]
		void SetValue_100x100_Random_AutoFormatOn()
		{
			SetUp(100, 100);

			worksheet.SetSettings(WorksheetSettings.Edit_AutoFormatCell, true);
			for (int r = 0; r < 100; r++)
			{
				for (int c = 0; c < 100; c++)
				{
					worksheet.SetCellData(rand.Next(100), rand.Next(100), rand.Next(100000));
				}
			}
		}

		[TestCase(DebugEnabled = true)]
		void SetValue_100x100_Random_AutoFormatOff()
		{
			SetUp(100, 100);
		
			worksheet.SetSettings(WorksheetSettings.Edit_AutoFormatCell, false);
			for (int r = 0; r < 100; r++)
			{
				for (int c = 0; c < 100; c++)
				{
					worksheet.SetCellData(rand.Next(100), rand.Next(100), rand.Next(100000));
				}
			}
		}

		[TestCase(DebugEnabled = true)]
		void SetRowsToMinimum()
		{
			worksheet.SetRows(1);
			AssertEquals(worksheet.RowCount, 1);

			worksheet.SetRows(0);
			// should be 1
			AssertEquals(worksheet.RowCount, 1);
		}

		[TestCase(DebugEnabled = true)]
		void SetRows1to100000()
		{
			worksheet.SetRows(100000);
			AssertEquals(worksheet.RowCount, 100000);
		}

		[TestCase]
		void FindColumnIndexMiddle()
		{
			worksheet.Resize(20, 20);

			System.Reflection.MethodInfo mi = worksheet.GetType().GetMethod("FindColIndexMiddle", 
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		
			AssertEquals(mi.Invoke(worksheet, new object[] { 80 }), 1);

			AssertEquals(mi.Invoke(worksheet, new object[] { 10 }), 0);

			AssertEquals(mi.Invoke(worksheet, new object[] { 150 }), 2);
		}
	}

	#region Data Struct Test

	class DataStructTest : TestSet
	{
		protected static readonly Random rand = new Random();
	}

	[TestSet(false)]
	class PagedRectArrayTest : DataStructTest
	{
		[TestCase]
		void Fill_1000x1000_Integer()
		{
			PagedRectArray<int> arr = new PagedRectArray<int>();

			for (int r = 0; r < 1000; r++)
			{
				for (int c = 0; c < 1000; c++)
				{
					arr[r, c] = r;
				}
			}
		}

		[TestCase]
		void Fill_FullPage_Integer()
		{
			PagedRectArray<int> arr = new PagedRectArray<int>();

			int rows = arr.RowCapacity;
			int cols = arr.ColCapacity;

			for (int r = 0; r < rows; r++)
			{
				for (int c = 0; c < cols; c += PagedRectArray<int>.ColPageSize)
				{
					arr[r, c] = r*c;
				}
			}
		}

		[TestCase]
		void Fill_Random_100000_Integer()
		{
			PagedRectArray<int> arr = new PagedRectArray<int>();

			int rows = arr.RowCapacity;
			int cols = arr.ColCapacity;

			for (int i = 0; i < 100000; i++)
			{
				arr[rand.Next(rows), rand.Next(cols)] = i;
			}
		}
	}

	[TestSet(false)]
	class LargeMemoryUsageTest : ReoGridTestSet
	{
		Index4DArray<string> arr = new Index4DArray<string>();

		private int testCol = 100;

		[TestCase(true)]
		public void MetaArray_MemoryUseOf10000x100()
		{
			Fill_10000x100_Elements();
		}

		[TestCase(true)]
		public void MetaArray_MemoryUseOf10000x100_Second()
		{
			Fill_10000x100_Elements();
		}

		private void Fill_10000x100_Elements()
		{
			for (int r = 0; r < 100000; r++)
			{
				for (int c = 0; c < this.testCol; c++)
				{
					arr[r, c] = "Row " + r + " Col " + c;
				}
			}
		}

		/**********************************************************/

		[TestCase(false)]
		public void Grid_Resize_10000x100_First()
		{
			SetUp(10000, this.testCol);
		}

		[TestCase(false)]
		public void Grid_Resize_10000x100_Second()
		{
			SetUp(10000, this.testCol);
		}

		[TestCase(false)]
		public void Grid_Resize_10000x100_Thrid()
		{
			SetUp(10000, this.testCol);
		}

		/**********************************************************/

		[TestCase(false)]
		public void Grid_Data_10000x100_1()
		{
			SetUp(10000, 100);

			Fill10000x100Cells();
		}

		[TestCase(false)]
		public void Grid_Data_10000x100_2()
		{
			Fill10000x100Cells();
		}

		[TestCase(false)]
		public void Grid_Data_10000x100_3()
		{
			Fill10000x100Cells();
		}

		[TestCase(false)]
		public void Grid_Data_10000x100_4()
		{
			Fill10000x100Cells();
		}

		[TestCase(false)]
		public void Grid_Data_10000x100_5()
		{
			Fill10000x100Cells();
		}

		[TestCase(false)]
		public void Grid_Data_10000x100_6()
		{
			Fill10000x100Cells();
		}

		[TestCase(false)]
		public void Grid_Data_10000x100_7()
		{
			Fill10000x100Cells();
		}

		[TestCase(false)]
		public void Grid_Data_10000x100_8()
		{
			Fill10000x100Cells();
		}

		[TestCase(false)]
		public void Grid_Data_10000x100_9()
		{
			Fill10000x100Cells();
		}

		private void Fill10000x100Cells()
		{
			for (int r = 0; r < 10000; r++)
			{
				for (int c = 0; c < this.testCol; c++)
				{
					worksheet[r, c] = "Row " + r + "Col " + c;
				}
			}
		}
	}

	#region TriangleTreeArrayTest
	[TestSet(false)]
	class TriangleTreeArrayTest : DataStructTest
	{
		[TestCase(true)]
		void Fill_FullPage_Integer()
		{
			TriangleTreeArray<int> arr = new TriangleTreeArray<int>();

			int rows = arr.Rows;
			int cols = arr.Cols;
			int s = 0;

			for (int x = 0; x < rows; x++)
			{
				for (int y = 0; y < cols; y += TriangleTreeArray<int>.ColSize)
				{
					arr[x, y] = s++;
				}
			}
		}

		[TestCase()] 
		void Fill_Random_1000_Integer() { Fill_Random_Integer(1000); }
		[TestCase(true)]
		void Fill_Random_100000_Integer() { Fill_Random_Integer(100000); }
		[TestCase(true)]
		void Fill_Random_5000000_Integer() { Fill_Random_Integer(5000000); }	

		void Fill_Random_Integer(int count)
		{
			TriangleTreeArray<int> arr = new TriangleTreeArray<int>();

			int rows = arr.Rows;
			int cols = arr.Cols;

			for (int i = 0; i < count; i++)
			{
				arr[rand.Next(rows), rand.Next(cols)] = i;
			}
		}

		[TestCase]
		void RW_Value_Check_1000x1000()
		{
			TriangleTreeArray<int> arr = new TriangleTreeArray<int>();

			for (int r = 0; r < 1000; r++)
			{
				for (int c = 0; c < 1000; c++)
				{
					arr[r, c] = r*c;
				}
			}

			for (int r = 0; r < 1000; r++)
			{
				for (int c = 0; c < 1000; c++)
				{
					AssertEquals(arr[r, c], r * c);
				}
			}
		}

		[TestCase(true)]
		void ForEach_Full()
		{
			TriangleTreeArray<object> arr = new TriangleTreeArray<object>();
			int rows = arr.Rows;
			int cols = arr.Cols;
			int count =0;
			for (int r = 0; r < arr.Rows; r++)
			{
				for (int c = 0; c < arr.Cols; c++)
				{
					if (arr[r, c] != null) count++;
				}
			}
		}

		[TestCase]
		void Iterator()
		{
			TriangleTreeArray<object> arr = new TriangleTreeArray<object>();
			int rows = arr.Rows;
			int cols = arr.Cols;
			int count = 0;

			arr.Iterate(0, rows, 0, cols, true, (r, c, v) =>
			{
				if (arr[r, c] != null) count++;
				return true;
			});
		}
	}
	#endregion // TriangleTreeArrayTest

	#region DictionaryTreeArrayTest
	//[TestSet(false)]
	//class DictionaryTreeArrayTest : DataStructTest
	//{
	//	[TestCase()]
	//	void Fill_Random_1000_Integer() { Fill_Random_Integer(1000); }
		
	//	void Fill_Random_Integer(int count)
	//	{
	//		DictionaryTreeArray<int> arr = new DictionaryTreeArray<int>();

	//		int rows = 64*64*64;
	//		int cols = 16*16*16;

	//		for (int i = 0; i < count; i++)
	//		{
	//			arr[rand.Next(rows), rand.Next(cols)] = i;
	//		}
	//	}

	//	[TestCase(true)]
	//	void RW_Value_Check_1000x1000()
	//	{
	//		DictionaryTreeArray<int> arr = new DictionaryTreeArray<int>();

	//		for (int r = 0; r < 1000; r++)
	//		{
	//			for (int c = 0; c < 1000; c++)
	//			{
	//				arr[r, c] = r * c;
	//			}
	//		}

	//		for (int r = 0; r < 1000; r++)
	//		{
	//			for (int c = 0; c < 1000; c++)
	//			{
	//				AssertEquals(arr[r, c], r * c);
	//			}
	//		}
	//	}
	//}
	#endregion // DictionaryTreeArrayTest

	#region JaggedTreeArray
	[TestSet(DebugEnabled = true, ReleaseEnabled = true)]
	class JaggedTreeArrayTest : DataStructTest
	{
		[TestCase(true)]
		void BasicTest()
		{
			JaggedTreeArray<int> arr = new JaggedTreeArray<int>();
			arr[0, 0] = 1;

			arr[127, 0] = 11;
			arr[128, 0] = 12;
			arr[255, 0] = 13;
			arr[256, 0] = 14;
			arr[511, 0] = 15;
			arr[512, 0] = 16;
			arr[1023, 0] = 17;
			arr[1024, 0] = 18;
			arr[2047, 0] = 19;
			arr[2048, 0] = 20;
			arr[4095, 0] = 21;
			arr[4096, 0] = 22;

			arr[0, 127] = 101;
			arr[0, 128] = 102;
			arr[0, 255] = 103;
			arr[0, 256] = 104;
			arr[0, 511] = 105;
			arr[0, 512] = 106;
			arr[0, 1023] = 107;
			arr[0, 1024] = 108;
			arr[0, 2047] = 109;
			arr[0, 2048] = 110;
			arr[0, 4095] = 111;
			arr[0, 4096] = 112;

			arr[128, 32] = 999;
			arr[2097151, 32767] = 9999999;

			AssertEquals(arr[0, 0], 1);

			AssertEquals(arr[127, 0], 11);
			AssertEquals(arr[128, 0], 12);
			AssertEquals(arr[255, 0], 13);
			AssertEquals(arr[256, 0], 14);
			AssertEquals(arr[511, 0], 15);
			AssertEquals(arr[512, 0], 16);
			AssertEquals(arr[1023, 0], 17);
			AssertEquals(arr[1024, 0], 18);
			AssertEquals(arr[2047, 0], 19);
			AssertEquals(arr[2048, 0], 20);
			AssertEquals(arr[4095, 0], 21);
			AssertEquals(arr[4096, 0], 22);
			
			AssertEquals(arr[0, 127], 101);
			AssertEquals(arr[0, 128], 102);
			AssertEquals(arr[0, 255], 103);
			AssertEquals(arr[0, 256], 104);
			AssertEquals(arr[0, 511], 105);
			AssertEquals(arr[0, 512], 106);
			AssertEquals(arr[0, 1023], 107);
			AssertEquals(arr[0, 1024], 108);
			AssertEquals(arr[0, 2047], 109);
			AssertEquals(arr[0, 2048], 110);
			AssertEquals(arr[0, 4095], 111);
			AssertEquals(arr[0, 4096], 112);

			AssertEquals(arr[128, 32], 999);
			AssertEquals(arr[2097151, 32767], 9999999);
		}

		[TestCase(true)]
		void RWAccessValidate_1000x1000()
		{
			JaggedTreeArray<int> arr = new JaggedTreeArray<int>();

			for (int r = 0; r < 1000; r++)
			{
				for (int c = 0; c < 1000; c++)
				{
					arr[r, c] = r * c;
				}
			}

			for (int r = 0; r < 1000; r++)
			{
				for (int c = 0; c < 1000; c++)
				{
					AssertEquals(arr[r, c], r * c);
				}
			}
		}

		[TestCase(true)]
		void RWAccessValidate_100000x100()
		{
			JaggedTreeArray<int> arr = new JaggedTreeArray<int>();

			for (int r = 0; r < 100000; r++)
			{
				for (int c = 0; c < 100; c++)
				{
					arr[r, c] = r * c;
				}
			}

			for (int r = 0; r < 100000; r++)
			{
				for (int c = 0; c < 100; c++)
				{
					AssertEquals(arr[r, c], r * c);
				}
			}
		}

		[TestCase(true)]
		void RWAccess_100000000_Elements_Skip()
		{
			JaggedTreeArray<int> arr = new JaggedTreeArray<int>();

			int t = 10000;

			for (int r = 0; r < t; r += 100)
			{
				for (int c = 0; c < t; c += 100)
				{
					arr[r, c] = r * c;
				}
			}

			for (int r = 0; r < t; r += 100)
			{
				for (int c = 0; c < t; c += 100)
				{
					AssertEquals(arr[r, c], r * c);
				}
			}
		}

		[TestCase(true)]
		void FillRandom10000Items() { FillRandomItems(10000); }

		void FillRandomItems(int count)
		{
			JaggedTreeArray<int> arr = new JaggedTreeArray<int>();

			int rows = (int)arr.RowCapacity;
			int cols = (int)arr.ColCapacity;

			for (int i = 0; i < count; i++)
			{
				arr[rand.Next(rows), rand.Next(cols)] = i;
			}
		}

		JaggedTreeArray<int> sumarr = new JaggedTreeArray<int>();

		[TestCase(true)]
		void IterateOverEmpty1000000x200()
		{
			sumarr.Iterate(0, 0, 1000000, 200, true, (r, c, val) => 1);
		}

		[TestCase(true)]
		void Fill1000000Rows()
		{
			for (int i = 0; i < 1000000; i++)
			{
				sumarr[i, 0] = i;
			}
		}

		[TestCase(true)]
		void Sum1000000ByIterator()
		{
			int sum = 0;
			sumarr.Iterate(0, 0, 1000000, 1, true, (r, c, v) =>
			{
				sum += v;
				return 1;
			});
		}

		[TestCase(true)]
		void Sum1000000ByFor()
		{
			int sum = 0;
			for (int i = 0; i < 1000000; i++)
			{
				sum += sumarr[i, 0];
			}
		}

	}
	#endregion // JaggedTreeArray

	#region RegularTreeArrayTest
	//[TestSet(DebugEnabled = false, ReleaseEnabled = false)]
	//class RegularTreeArrayTest : DataStructTest
	//{
	//	[TestCase(true)]
	//	void BasicTest()
	//	{
	//		RegularTreeArray<int> arr = new RegularTreeArray<int>();
	//		arr[0, 0] = 1;
	//		arr[128, 32] = 10;
	//		arr[2097151, 32767] = 99;

	//		AssertEquals(arr[0, 0], 1);
	//		AssertEquals(arr[128, 32], 10);
	//		AssertEquals(arr[2097151, 32767], 99);
	//	}

	//	[TestCase(true)]
	//	void RWAccessValidate_1000x1000()
	//	{
	//		RegularTreeArray<int> arr = new RegularTreeArray<int>();

	//		for (int r = 0; r < 1000; r++)
	//		{
	//			for (int c = 0; c < 1000; c++)
	//			{
	//				arr[r, c] = r * c;
	//			}
	//		}

	//		for (int r = 0; r < 1000; r++)
	//		{
	//			for (int c = 0; c < 1000; c++)
	//			{
	//				AssertEquals(arr[r, c], r * c);
	//			}
	//		}
	//	}

	//	[TestCase(true)]
	//	void RWAccessValidate_100000x100()
	//	{
	//		RegularTreeArray<int> arr = new RegularTreeArray<int>();

	//		for (int r = 0; r < 100000; r++)
	//		{
	//			for (int c = 0; c < 100; c++)
	//			{
	//				arr[r, c] = r * c;
	//			}
	//		}

	//		for (int r = 0; r < 100000; r++)
	//		{
	//			for (int c = 0; c < 100; c++)
	//			{
	//				AssertEquals(arr[r, c], r * c);
	//			}
	//		}
	//	}

	//	[TestCase(true)]
	//	void RWAccess_100000000_Elements_Skip()
	//	{
	//		RegularTreeArray<int> arr = new RegularTreeArray<int>();

	//		int t = 10000;

	//		for (int r = 0; r < t; r+=100)
	//		{
	//			for (int c = 0; c < t; c+=100)
	//			{
	//				arr[r, c] = r * c;
	//			}
	//		}

	//		for (int r = 0; r < t; r+=100)
	//		{
	//			for (int c = 0; c < t; c+=100)
	//			{
	//				AssertEquals(arr[r, c], r * c);
	//			}
	//		}
	//	}

	//	[TestCase(true)]
	//	void FillRandom10000Items() { FillRandomItems(10000); }

	//	void FillRandomItems(int count)
	//	{
	//		RegularTreeArray<int> arr = new RegularTreeArray<int>();

	//		int rows = (int)arr.RowCapacity;
	//		int cols = (int)arr.ColCapacity;

	//		for (int i = 0; i < count; i++)
	//		{
	//			arr[rand.Next(rows), rand.Next(cols)] = i;
	//		}
	//	}

	//	RegularTreeArray<int> sumarr = new RegularTreeArray<int>();

	//	[TestCase(true)]
	//	void IterateOverEmpty1000000x200()
	//	{
	//		sumarr.Iterate(0, 0, 1000000, 200, true, (r, c, val) => 1);
	//	}

	//	[TestCase(true)]
	//	void Fill1000000Rows()
	//	{
	//		for (int i = 0; i < 1000000; i++)
	//		{
	//			sumarr[i, 0] = i;
	//		}
	//	}

	//	[TestCase(true)]
	//	void Sum1000000ByIterator()
	//	{
	//		int sum = 0;
	//		sumarr.Iterate(0, 0, 1000000, 1, true, (r, c, v) =>
	//		{
	//			sum += v;
	//			return 1;
	//		});
	//	}

	//	[TestCase(true)]
	//	void Sum1000000ByFor()
	//	{
	//		int sum = 0;
	//		for (int i = 0; i < 1000000; i++)
	//		{
	//			sum += sumarr[i, 0];
	//		}
	//	}

	//}
	#endregion // RegularTreeArrayTest

	#region LineTreeArrayTest
	[TestSet(false)]
	class LineTreeArrayTest : DataStructTest
	{
		[TestCase]
		void FullFill()
		{
			TreeArray<int> arr = new TreeArray<int>();

			long capacity = arr.Capacity;
			for (int i = 0; i < capacity; i++)
			{
				arr[i] = i;
			}
		}

		[TestCase]
		void GridPositionIndex_GetRow_Empty()
		{
			GridIndexTreeArray<int> arr = new GridIndexTreeArray<int>();

			Console.WriteLine(string.Format("{0} = {1}", 0, arr.GetPosition(0)));
			Console.WriteLine(string.Format("{0} = {1}", 5, arr.GetPosition(5)));
			Console.WriteLine(string.Format("{0} = {1}", 19, arr.GetPosition(19)));
			Console.WriteLine(string.Format("{0} = {1}", 20, arr.GetPosition(20)));
			Console.WriteLine(string.Format("{0} = {1}", 21, arr.GetPosition(21)));
			Console.WriteLine(string.Format("{0} = {1}", 1048575, arr.GetPosition(1048575)));
			Console.WriteLine(string.Format("{0} = {1}", 2000, arr.GetPosition(2000)));
		}
	}

	[TestSet(false)]
	public class PagingArrayTest
	{
		PagingArray pa1 = new PagingArray(1024, 1024);

		[TestCase]
		public void NullGet()
		{
			TestAssert.AssertEquals(pa1[0], 0);
			TestAssert.AssertEquals(pa1[20], 20 * 20);
			TestAssert.AssertEquals(pa1[2000], 2000 * 20);
			TestAssert.AssertEquals(pa1[2020], 2020 * 20);
			TestAssert.AssertEquals(pa1[1048575], 1048575 * 20);
		}

		[TestCase]
		public void SimpleChange()
		{
			pa1[10] = 10 * 20 + 50;
			TestAssert.AssertEquals(pa1[0], 0);
			TestAssert.AssertEquals(pa1[10], 10 * 20 + 50);
			TestAssert.AssertEquals(pa1[20], 20 * 20 + 50);
			TestAssert.AssertEquals(pa1[2010], 2010 * 20 + 50);
			TestAssert.AssertEquals(pa1[100000], 100000 * 20 + 50);
			TestAssert.AssertEquals(pa1[1048575], 1048575 * 20 + 50);

			pa1[2010] = 2010 * 20 + 100;
			pa1[2020] = 2020 * 20 + 100;
			TestAssert.AssertEquals(pa1[2010], 2010 * 20 + 100);
			TestAssert.AssertEquals(pa1[100000], 100000 * 20 + 100);
			//TestAssert.AssertEquals(pa1[1048575], 1048575 * 20 + 100);
			
			pa1[100000] = 2010 * 20 + 200;
			//TestAssert.AssertEquals(pa1[100000], 100000 * 20 + 200);
			//TestAssert.AssertEquals(pa1[1048575], 1048575 * 20 + 300);
		}
		
		[TestCase]
		public void SerialChanges()
		{
			for (int i = 3000; i < 5000; i++)
			{
				pa1[i] = i * 20;
			}

			for (int i = 3000; i < 5000; i+=10)
			{
				//TestAssert.AssertEquals(pa1[i], i * 20 + 120);
			}
		}
	}
	#endregion // LineTreeArrayTest

	#region FastFindTest
	[TestSet(true)]
	class FastFindTest : DataStructTest
	{
		private int[] arr = new int[10000000];
		private string[] wordsArr = null;

		public override void SetUp()
		{
			for (int i = 0; i < arr.Length; i++) arr[i] = i + 133;

			//using (StreamReader sr = new StreamReader(File.OpenRead(@"D:\Resources\dictionary\english_words.txt")))
			//{
			//  string line = null;
			//  while (((line = sr.ReadLine()) != null))
			//  {
			//    words.Add(line);
			//  }
			//}
			//wordsArr = words.ToArray();
		}

		[TestCase(true)]
		void Find_No1000()
		{
			for (int i = 0; i < arr.Length; i++) if (arr[i] == 1000) return;
		}
		[TestCase(true)]
		void Find_No50000()
		{
			for (int i = 0; i < arr.Length; i++) if (arr[i] == 50000) return;
		}
		[TestCase(true)]
		void Find_No1000000()
		{
			for (int i = 0; i < arr.Length; i++) if (arr[i] == 1000000) return;
		}
		[TestCase]
		void Find_No9999999()
		{
			for (int i = 0; i < arr.Length; i++) if (arr[i] == 7878997) return;
		}

		[TestCase(true)]
		void Find_No9999999_QuickFind()
		{
			int t = 7878997;
			AssertEquals(7878864, FindInt(t));
		}
		int lv = 0;

		private int FindInt(int t)
		{
			return FindInt(t, arr.Length / 2, 0, arr.Length);
		}
		private int FindInt(int t, int split, int start, int end)
		{
			if (split == 0 || split == arr.Length - 1) return -1;

			lv++;
			int v = arr[split];

			if (v == t)
			{
				return split;
			}
			else if (v > t)
			{
				int p = (split - start) >> 1;
				return FindInt(t, split - p, start, split);
			}
			else if (v < t)
			{
				int p = (end - split) >> 1;
				return FindInt(t, split + p, split, end);
			}

			return -1;
		}

		List<string> words = new List<string>(24475);


		int wordIndex = -1;

		[TestCase(true)]
		void FindWord_ForEach()
		{
			StringComparer comp = StringComparer.OrdinalIgnoreCase;

			for (int k = 0; k < 100; k++)
			{
				for (int i = 0; i < words.Count; i++)
				{
					if (comp.Compare("hello", words[i]) == 0)
					{
						wordIndex = i;
						break;
					}
				}
			}
		}

		//[TestCase(false)] // invalid case
		//void FindWord_QuickFind()
		//{
		//	StringComparer comp = StringComparer.OrdinalIgnoreCase;

		//	string t = "hellox";

		//	for (int k = 0; k < 100; k++)
		//	{

		//		int idx = ArrayHelper.QuickFind(words.Count >> 1, 0, words.Count, (index) =>
		//			{
		//				return comp.Compare(t, words[index]);
		//			});

		//		AssertEquals(wordIndex, idx);
		//	}
		//}

		[TestCase(false)]
		void FindWord_SystemIndexOf()
		{
			StringComparer comp = StringComparer.OrdinalIgnoreCase;

			string t = "hello";

			for (int k = 0; k < 100; k++)
			{
				int idx = Array.IndexOf(wordsArr, t);
				AssertEquals(wordIndex, idx);
			}
		}
	}
	#endregion // FastFindTest

	#region Indexed4DArray

	[TestSet]
	class Indexed4DArrayTest:TestSet
	{
		[TestCase]
		void DontIgnoreNullIterate()
		{
			var arr = new Index4DArray<object>();

			int count = 0;
			arr.Iterate(0, 0, 3, 3, false, (r, c, o) =>
				 {
					 count++;
					 return 1;
				 });

			AssertEquals(count, 9);
		}

		[TestCase]
		void DontIgnoreNullAllValidIterate()
		{
			var arr = new Index4DArray<int>();

			arr[0, 0] = 1; arr[0, 1] = 2; arr[0, 2] = 3;
			arr[1, 0] = 4; arr[1, 1] = 5; arr[1, 2] = 6;
			arr[2, 0] = 7; arr[2, 1] = 8; arr[2, 2] = 9;

			int count = 0, sum = 0;
			arr.Iterate(0, 0, 3, 3, false, (r, c, o) =>
			{
				count++;
				sum += o;
				return 1;
			});

			AssertEquals(count, 9);
			AssertEquals(sum, 45);
		}

		[TestCase]
		void HalfValue_DontIgnoreNullIterate()
		{
			var arr = new Index4DArray<int?>();

			arr[0, 0] = 1; arr[0, 1] = 2; arr[0, 2] = null;
			arr[1, 0] = null; arr[1, 1] = 5; arr[1, 2] = 6;
			arr[2, 0] = 7; arr[2, 1] = null; arr[2, 2] = 9;

			int count = 0, sum = 0;
			arr.Iterate(0, 0, 3, 3, false, (r, c, o) =>
			{
				count++;
				if (o.HasValue) sum += (int)o;
				return 1;
			});

			AssertEquals(count, 9);
			AssertEquals(sum, 30);
		}

		[TestCase]
		void HalfValue_IgnoreNullIterate()
		{
			var arr = new Index4DArray<int?>();

			arr[0, 0] = 1; arr[0, 1] = 2; arr[0, 2] = null;
			arr[1, 0] = null; arr[1, 1] = 5; arr[1, 2] = 6;
			arr[2, 0] = 7; arr[2, 1] = null; arr[2, 2] = 9;

			int count = 0, sum = 0;
			arr.Iterate(0, 0, 3, 3, true, (r, c, o) =>
			{
				count++;
				sum += (int)o;
				return 1;
			});

			AssertEquals(count, 6);
			AssertEquals(sum, 30);
		}
	}

	#endregion // Indexed4DArray
	#endregion // Data Struct Test

	#region Data Struct for test
	internal class PagingArray
	{
		private int itemSize = 20;

		private int pageSize;

		private Page[] pages;

		public PagingArray(int pageSize, int pageCount)
		{
			this.pageSize = pageSize;

			pages = new Page[pageCount];

			for (int i = 0; i < pageCount; i++)
			{
				pages[i] = new Page()
				{
					position = i * pageSize * itemSize,
					minIndex = itemSize,
				};
			}
		}

		internal int this[int index]
		{
			get
			{
				Page page = pages[index / pageSize];

				int indexInPage = index % pageSize;

				// empty page
				if (page.maxIndex == 0)
				{
					return page.position + indexInPage * itemSize;
				}

				Data data = page[indexInPage];

				// empty data
				if (data == null)
				{
					// find previous non-empty data
					if (indexInPage > page.maxIndex)
					{
						// right side of max data
						return page[page.maxIndex].Position + (indexInPage - page.maxIndex) * itemSize;
					}
					else
					{
						for (int i = indexInPage; i > page.minIndex; i--)
						{
							data = page[i];

							if (data != null)
							{
								// find data at left side
								return data.Position + (indexInPage - i) * itemSize;
							}
						}

						// not found, calc pos
						return page.position + indexInPage * itemSize;
					}
				}
				else
				{
					return page.position + data.Position;
				}
			}
			set
			{
				int pageIndex = index/pageSize;

				Page page = pages[pageIndex];

				if (page.items == null)
				{
					page.items = new Data[pageSize];
				}

				int indexInPage = index % pageSize;

				Data data = page[indexInPage];

				int offset = 0;
				
				if (data == null)
				{
					page.minIndex = Math.Min(page.minIndex, indexInPage);
					page.maxIndex = Math.Max(page.maxIndex, indexInPage);

					int newPosition = value - page.position;
					offset = newPosition - (indexInPage * itemSize);

					if (offset < 0)
					{
						newPosition = (indexInPage) * itemSize;
						offset = 0;
					}

					page[indexInPage] = new Data
					{
						Position = newPosition,
					};
				}
				else
				{
					int newPosition = value - page.position;
					offset = newPosition - data.Position;

					if (offset < 0)
					{
						newPosition = (indexInPage * itemSize);
						offset = 0;
					}

					data.Position = newPosition;
				}

				if (offset > 0)
				{
					for (int i = pageIndex + 1; i < this.pages.Length; i++)
					{
						this.pages[i].position += offset;
					}
				}
			}
		}

		class Page
		{
			internal int position;
			internal Data[] items;

			internal Data this[int index]
			{
				get
				{
					return items[index];
				}
				set
				{
					items[index] = value;
				}
			}

			internal int minIndex;
			internal int maxIndex;
		}

		class Data
		{
			public virtual int Position { get; set; }
			public virtual int Size { get; set; }
		}
	}

	internal class PagingLinkArray
	{
		public int DefaultSize { get; set; }

		public int TotalSize { get; set; }

		public int Count {get;set;}

		public int PageSize { get; set; }

		public PagingLinkArray(int size, int count)
			: this(size, count, 
			256 /* default page size */) { }

		public PagingLinkArray(int size, int count, int pageSize)
		{
			this.DefaultSize = size;
			this.Count = count;
			this.PageSize = pageSize;
		}

		//Item this[int index]
		//{
		//	get
		//	{
		//		return null;
		//	}
		//	set
		//	{
		//	}
		//}
		
		//class Page
		//{
		//	Page prev;
		//	Page next;
		//}

		//class Item
		//{
		//	private PagingLinkArray array;
			
		//	public Item(PagingLinkArray array)
		//	{
		//		this.array = array;
		//	}

		//	public int Index{get;set;}

		//	public int Position { get; set; }

		//	public int Size
		//	{
		//		get
		//		{
		//			if (Index < array.Count - 1)
		//			{
		//				return array[Index + 1].Position - this.Position;
		//			}
		//			else
		//			{
		//				return array.TotalSize - this.Position;
		//			}
		//		}
		//	}
		//}
	}
	
	internal class PagedRectArray<T>
	{
		private const int MaxRowPages = 256;
		private const int MaxColPages = 256;
		internal const int RowPageSize = 64;
		internal const int ColPageSize = 64;

		private T[,][,] data = new T[MaxRowPages, MaxColPages][,];

		public T this[int r, int c]
		{
			get
			{
				T[,] page = data[r >> 6, c >> 6];
				return page == null ? default(T) : page[r % RowPageSize, c % ColPageSize];
			}
			set
			{
				int ri = r >> 6;
				int ci = c >> 6;

				if (data[ri, ci] == null)
				{
					data[ri, ci] = new T[PagedRectArray<T>.RowPageSize, PagedRectArray<T>.ColPageSize];
				}

				data[ri,ci][r % RowPageSize, c % ColPageSize] = value;
			}
		}

		public int RowCapacity
		{
			get { return MaxRowPages * RowPageSize; }
		}

		public int ColCapacity
		{
			get { return MaxColPages * ColPageSize; }
		}

		public void Iterate(int r, int c, int count, Action<T, int, int> iterator)
		{
			//data[r / RowPageSize, c / RowPageSize];
		}
	}

	public sealed class TreeArray<T>
	{
		public static readonly int NodeSize = 16;
		public static readonly int MaxDepth = 5;
		private static readonly long capacity = (long)Math.Pow(NodeSize, MaxDepth);

		private Node root = new Node();

		public TreeArray()
		{
			root.nodes = new TreeArray<T>.Node[NodeSize];
		}

		public T this[int index]
		{
			get
			{
				Node node = root;

				for (int d = MaxDepth; d > 0; d--)
				{
					int i = (index >> (4 * d)) % NodeSize;

					node = node.nodes[i];

					if (node == null) return default(T);
				}

				return node.data[index % NodeSize];
			}
			set
			{
				Node node = root;

				for (int d = MaxDepth; d > 0; d--)
				{
					int i = (index >> (4 * d)) % NodeSize;

					Node child = node.nodes[i];

					if (child == null)
					{
						if (value == null)
							return;
						else
						{
							child = node.nodes[i] = new Node();
							if (d > 1) child.nodes = new TreeArray<T>.Node[NodeSize];
						}
					}

					node = child;
				}

				if (node.data == null)
				{
					if (value == null)
						return;
					else
						node.data = new T[NodeSize];
				}

				node.data[index % NodeSize] = value;
			}
		}

		public void RemoveAt(int index)
		{
			// todo

			this[index] = default(T);
		}

		public long Capacity { get { return capacity; } }

		public sealed class Node
		{
			internal Node[] nodes;
			internal T[] data = null;
		}

		public bool IsPageNull(int index)
		{
			Node node = root;

			for (int d = MaxDepth; d > 0; d--)
			{
				int i = (index >> (4 * d)) % NodeSize;

				node = node.nodes[i];

				if (node == null) return true;
			}

			return node.data == null;
		}

		public void Iterate(int index, int count, bool ignoreNull, Func<int, T, bool> iterator)
		{
			int end = index + count;
			if (end > capacity) end = (int)(capacity);

			for (int i = index; i < end; )
			{
				if (IsPageNull(i))
				{
					i += (NodeSize - (i % NodeSize));
				}
				else
				{
					if (!iterator(i, this[i])) return;
					i++;
				}
			}
		}

		public void IterateReverse(int index, int count, bool ignoreNull, Func<int, T, bool> iterator)
		{
			int end = index - count;
			if (end <= -1) end = -1;

			for (int i = index; i > end; )
			{
				if (IsPageNull(i))
				{
					i -= (i % NodeSize);
				}
				else
				{
					if (!iterator(i, this[i])) return;
					i--;
				}
			}
		}

		public void RemoveRange(int index, int count)
		{
			int end = index + count;

			for (int i = index; i < end; )
			{
				if (IsPageNull(i))
				{
					i += (NodeSize - (i % NodeSize));
				}
				else
				{
					this[i] = default(T);
					i++;
				}
			}
		}
	}

	public sealed class GridIndexTreeArray<T>
	{
		public static readonly int NodeSize = 16;
		public static readonly int MaxDepth = 5;
	
		private static readonly long capacity = (long)Math.Pow(NodeSize, MaxDepth);
		public long Capacity { get { return capacity; } }

		public static readonly int DefaultAmount = 20;

		private Node root = new Node();

		public GridIndexTreeArray()
		{
			root.nodes = new GridIndexTreeArray<T>.Node[NodeSize];
		}

		public T this[int index]
		{
			get
			{
				Node node = root;

				for (int d = MaxDepth; d > 0; d--)
				{
					int i = (index >> (4 * d)) % NodeSize;
					node = node.nodes[i];
					if (node == null) return default(T);
				}

				return node.data[index % NodeSize];
			}
			set
			{
				Node node = root;

				for (int d = MaxDepth; d > 0; d--)
				{
					int i = (index >> (4 * d)) % NodeSize;

					Node child = node.nodes[i];

					if (child == null)
					{
						if (value == null)
							return;
						else
						{
							child = node.nodes[i] = new Node();
							if (d > 1) child.nodes = new GridIndexTreeArray<T>.Node[NodeSize];
						}
					}

					node = child;
				}

				if (node.data == null)
				{
					if (value == null)
						return;
					else
						node.data = new T[NodeSize];
				}

				node.data[index % NodeSize] = value;
			}
		}

		public int GetPosition(int index)
		{
			Node node = root;

			int d = MaxDepth;

			for(; d > 0; d--)
			{
				int i = (index >> (4 * d)) % NodeSize;
				Node child = node.nodes[i];
				if (child == null) break;
			}

			return (int)(Math.Pow(index, MaxDepth - d)) + (index % NodeSize) * DefaultAmount  - 1;
		}

		public void RemoveAt(int index)
		{
			// todo

			this[index] = default(T);
		}

		public sealed class Node
		{
			internal Node[] nodes;
			internal T[] data = null;

			internal int position = 0;
		}

		public bool IsPageNull(int index)
		{
			Node node = root;

			for (int d = MaxDepth; d > 0; d--)
			{
				int i = (index >> (4 * d)) % NodeSize;
				node = node.nodes[i];
				if (node == null) return true;
			}

			return node.data == null;
		}

		public void Iterate(int index, int count, bool ignoreNull, Func<int, T, bool> iterator)
		{
			int end = index + count;
			if (end > capacity) end = (int)(capacity);

			for (int i = index; i < end; )
			{
				if (IsPageNull(i))
				{
					i += (NodeSize - (i % NodeSize));
				}
				else
				{
					if (!iterator(i, this[i])) return;
					i++;
				}
			}
		}

		public void IterateReverse(int index, int count, bool ignoreNull, Func<int, T, bool> iterator)
		{
			int end = index - count;
			if (end <= -1) end = -1;

			for (int i = index; i > end; )
			{
				if (IsPageNull(i))
				{
					i -= (i % NodeSize);
				}
				else
				{
					if (!iterator(i, this[i])) return;
					i--;
				}
			}
		}

		public void RemoveRange(int index, int count)
		{
			int end = index + count;

			for (int i = index; i < end; )
			{
				if (IsPageNull(i))
				{
					i += (NodeSize - (i % NodeSize));
				}
				else
				{
					this[i] = default(T);
					i++;
				}
			}
		}
	}

	/*
	internal class JudeTreeArray<T>
	{
		internal static readonly int RowSize = 32;
		internal static readonly int ColSize = 16;

		private T[,][,][,] data = new T[RowSize, ColSize][,][,];

		public T this[int row, int col]
		{
			get
			{
				int r = row >> 10;
				int c = col >> 8;
				T[,][,] page1 = data[r, c];
				if (page1 == null) return default(T);

				int r2 = (row >> 5) % RowSize;
				int c2 = (col >> 4) % ColSize;
				T[,] page2 = page1[r2, c2];

				return page2 == null ? default(T) : page2[row % RowSize, col % ColSize];
			}
			set
			{
				int r = row >> 10;
				int c = col >> 8;
				if (data[r, c] == null) data[r, c] = new T[RowSize, ColSize][,];

				int r2 = (row >> 5) % RowSize;
				int c2 = (col >> 4) % ColSize;
				if (data[r, c][r2, c2] == null) data[r, c][r2, c2] = new T[RowSize, ColSize];

				data[r, c][r2, c2][row % RowSize, col % ColSize] = value;
			}
		}

		public int Rows
		{
			get { return RowSize << 10; }
		}

		public int Cols
		{
			get { return ColSize << 8; }
		}
	}
	 */

	/*
	public class BinaryTree
	{
		string[] root;
	}
	 */

	#endregion // Data Struct for test
}

