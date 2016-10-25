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

namespace unvell.ReoGrid.Tests.TestCases
{
	[TestSet]
	class ColumnFilterTest : ReoGridTestSet
	{
		[TestCase]
		void NormalFilter()
		{
			SetUp(20, 20);

			worksheet["C1:C5"] = new object[] { "A", "B", "C", "D", "E" };

			var filter = worksheet.CreateColumnFilter("C", "C", 0, Data.AutoColumnFilterUI.NoGUI);
			
			// select items
			filter.Columns["C"].SelectedTextItems.AddRange(new string[] { "A", "C", "D" });

			// do filter
			filter.Apply();

			AssertNotSame(worksheet.GetRowHeight(0), 0);
			AssertSame(worksheet.GetRowHeight(1), 0);
			AssertNotSame(worksheet.GetRowHeight(2), 0);
			AssertNotSame(worksheet.GetRowHeight(3), 0);
			AssertSame(worksheet.GetRowHeight(4), 0);

			// detech filter, resotre all rows
			filter.Detach();

			AssertNotSame(worksheet.GetRowHeight(0), 0);
			AssertNotSame(worksheet.GetRowHeight(1), 0);
			AssertNotSame(worksheet.GetRowHeight(2), 0);
			AssertNotSame(worksheet.GetRowHeight(3), 0);
			AssertNotSame(worksheet.GetRowHeight(4), 0);
		}

		private unvell.ReoGrid.Data.AutoColumnFilter combineFilter;

		[TestCase]
		void CombineFilter1()
		{
			SetUp(20, 20);

			worksheet["A1"] = new object[,] {
				{ "1", "A", "D" },
				{ "2", "B", "E" },
				{ "3", "C", "F" },
				{ "4", "D", "G"	},
				{ "5", "E", "H"	},
			};

			combineFilter = worksheet.CreateColumnFilter("A", "C", 0, Data.AutoColumnFilterUI.NoGUI);
			AssertTrue(combineFilter != null);
			
			// test1
			combineFilter.Columns["A"].SelectedTextItems.AddRange(new string[] { "1", "2" });
			combineFilter.Columns["B"].SelectedTextItems.AddRange(new string[] { "A", "B" });
			combineFilter.Columns["C"].SelectedTextItems.AddRange(new string[] { "D", "E" });

			// apply filter
			combineFilter.Apply();

			AssertTrue(worksheet.GetRowHeight(0) != 0);
			AssertTrue(worksheet.GetRowHeight(1) != 0);
			AssertTrue(worksheet.GetRowHeight(2) == 0);
			AssertTrue(worksheet.GetRowHeight(3) == 0);
			AssertTrue(worksheet.GetRowHeight(4) == 0);

			combineFilter.Columns["A"].SelectedTextItems.Clear();
			combineFilter.Columns["B"].SelectedTextItems.Clear();
			combineFilter.Columns["C"].SelectedTextItems.Clear();
		}

		[TestCase]
		void CombineFilter2()
		{
			combineFilter.Columns["A"].SelectedTextItems.AddRange(new string[] { "1", "3", "4", "5" });
			combineFilter.Columns["B"].SelectedTextItems.AddRange(new string[] { "A", "B", "C", "D" });
			combineFilter.Columns["C"].SelectedTextItems.AddRange(new string[] { "D", "E", "G", "H" });

			// apply filter
			combineFilter.Apply();

			// should be all rows visible
			AssertTrue(worksheet.GetRowHeight(0) != 0);
			AssertTrue(worksheet.GetRowHeight(1) == 0);
			AssertTrue(worksheet.GetRowHeight(2) == 0);
			AssertTrue(worksheet.GetRowHeight(3) != 0);
			AssertTrue(worksheet.GetRowHeight(4) == 0);

			combineFilter.Columns["A"].SelectedTextItems.Clear();
			combineFilter.Columns["B"].SelectedTextItems.Clear();
			combineFilter.Columns["C"].SelectedTextItems.Clear();
		}

		private void AssertData(object[,] data)
		{
			for (int r = 0; r < data.GetLength(0); r++)
			{
				for (int c = 0; c < data.GetLength(1); c++)
				{
					AssertEquals(Convert.ToString(worksheet[r, c]), Convert.ToString(data[r, c]));
				}
			}
		}

		[TestCase]
		void NormalSort()
		{
			SetUp(20, 20);

			var data = new object[,] {
				{ "1", "A", "D" },
				{ "2", "B", "E" },
				{ "3", "C", "F" },
				{ "4", "D", "G"	},
				{ "5", "E", "H"	},
			};

			worksheet["A1"] = data;

			worksheet.SortColumn(0, SortOrder.Ascending);
			AssertData(data);

			worksheet.SortColumn(0, SortOrder.Descending);
			AssertData(new object[,] {
				{ "5", "E", "H"	},
				{ "4", "D", "G"	},
				{ "3", "C", "F" },
				{ "2", "B", "E" },
				{ "1", "A", "D" },
			});

			worksheet.SortColumn(1, SortOrder.Ascending);
			AssertData(new object[,] {
				{ "1", "A", "D" },
				{ "2", "B", "E" },
				{ "3", "C", "F" },
				{ "4", "D", "G"	},
				{ "5", "E", "H"	},
			});

			worksheet.SortColumn(1, SortOrder.Descending);
			AssertData(new object[,] {
				{ "5", "E", "H"	},
				{ "4", "D", "G"	},
				{ "3", "C", "F" },
				{ "2", "B", "E" },
				{ "1", "A", "D" },
			});
		}

		[TestCase]
		void RangeSort()
		{
			SetUp(20, 20);

			var data = new object[,] {
				{ "1", "A", "D" },
				{ "2", "B", "E" },
				{ "3", "C", "F" },
				{ "4", "D", "G"	},
				{ "5", "E", "H"	},
			};

			worksheet["A1"] = data;

			worksheet.SortColumn(0, new RangePosition("A1:B5"), SortOrder.Descending);
			AssertData(new object[,] {
				{ "5", "E", "D" },
				{ "4", "D", "E" },
				{ "3", "C", "F" },
				{ "2", "B", "G"	},
				{ "1", "A", "H"	},
			});

			worksheet.SortColumn(0, new RangePosition("A1:B5"), SortOrder.Ascending);
			AssertData(new object[,] {
				{ "1", "A", "D" },
				{ "2", "B", "E" },
				{ "3", "C", "F" },
				{ "4", "D", "G"	},
				{ "5", "E", "H"	},
			});
		}

		[TestCase]
		void TitledFilter()
		{
			SetUp(20, 20);

			worksheet["A1"] = new object[,] {
				{ "Col1", "Col2", "Col3" },
				{ "1", "A", "D" },
				{ "2", "B", "E" },
				{ "3", "C", "F" },
				{ "4", "D", "G"	},
				{ "5", "E", "H"	},
			};

			var filter = worksheet.CreateColumnFilter("A", "C", 1, Data.AutoColumnFilterUI.NoGUI);

			filter.Columns["B"].SelectedTextItems.AddRange(new string[] { "A", "C", "E" });
			filter.Apply();

			AssertTrue(worksheet.GetRowHeight(0) != 0); // header
			AssertTrue(worksheet.GetRowHeight(1) != 0);
			AssertTrue(worksheet.GetRowHeight(2) == 0);
			AssertTrue(worksheet.GetRowHeight(3) != 0);
			AssertTrue(worksheet.GetRowHeight(4) == 0);
			AssertTrue(worksheet.GetRowHeight(5) != 0);

			worksheet.SortColumn(0, new RangePosition("A2:B6"), SortOrder.Descending);
			AssertData(new object[,] {
				{ "Col1", "Col2", "Col3" },
				{ "5", "E", "D" },
				{ "4", "D", "E" },
				{ "3", "C", "F" },
				{ "2", "B", "G"	},
				{ "1", "A", "H"	},
			});
		}
	}
}
