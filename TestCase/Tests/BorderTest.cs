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

namespace unvell.ReoGrid.Tests
{
	[TestSet]
	class BorderTest : ReoGridTestSet
	{
		[TestCase]
		public void NormalSet()
		{
			SetUp();

			worksheet.SetRangeBorders(new RangePosition(1, 1, 2, 10), BorderPositions.Outside, RangeBorderStyle.BlackSolid);
			worksheet.SetRangeBorders(new RangePosition(1, 5, 10, 2), BorderPositions.Outside, RangeBorderStyle.BlackSolid);

			AssertTrue(worksheet._Debug_Validate_All());
		}
		
		[TestCase]
		void RandomSpanTest()
		{
			// set
			for (int i = 0; i < 100; i++)
			{
				int r = rand.Next(25);
				int c = rand.Next(20);

				BorderPositions borderPos = BorderPositions.None;
				switch (rand.Next(5))
				{
					case 0: borderPos = BorderPositions.Left; break;
					case 1: borderPos = BorderPositions.Right; break;
					case 2: borderPos = BorderPositions.Top; break;
					case 3: borderPos = BorderPositions.Bottom; break;
					case 4: borderPos = BorderPositions.Outside; break;
				}

				worksheet.SetRangeBorders(new RangePosition(r, c, 1, 1), borderPos, RangeBorderStyle.BlackSolid);

				//Application.DoEvents();
			}

			// check
			AssertTrue(Worksheet._Debug_Validate_BorderSpan());
		}

		[TestCase]
		void BorderMaxContentPosition()
		{
			SetUp(10, 10);

			worksheet.SetRangeBorders(9, 9, 1, 1, BorderPositions.Bottom, RangeBorderStyle.BlackSolid);
			AssertEquals(worksheet.MaxContentRow, 9);
			AssertEquals(worksheet.MaxContentCol, 9);

			worksheet.SetRangeBorders(9, 9, 1, 1, BorderPositions.Right, RangeBorderStyle.BlackSolid);
			AssertEquals(worksheet.MaxContentRow, 9);
			AssertEquals(worksheet.MaxContentCol, 9);
		}
	}

}
