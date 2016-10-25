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
using unvell.ReoGrid.DataFormat;

namespace unvell.ReoGrid.Tests
{
	[TestSet]
	class DoActionTest : ReoGridTestSet
	{
		[TestCase]
		public void DoAllActionsWithoutException()
		{
			SetUp();

			Grid.DoAction(new SetCellDataAction(1, 1, "abc"));
			Grid.Undo();
			Grid.Redo();
			AssertTrue(worksheet._Debug_Validate_All());

			Grid.DoAction(new SetRangeBorderAction(new RangePosition(1, 1, 2, 2),
				BorderPositions.All, RangeBorderStyle.BlackSolid));
			Grid.Undo();
			Grid.Redo();
			AssertTrue(worksheet._Debug_Validate_All());

			Grid.DoAction(new SetRangeStyleAction(new RangePosition(1, 1, 2, 2),
				new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.BackAll | PlainStyleFlag.TextColor,
					BackColor = Color.DarkBlue,
					TextColor = Color.Silver,
				}));
			Grid.Undo();
			Grid.Redo();
			AssertTrue(worksheet._Debug_Validate_All());

			Grid.DoAction(new MergeRangeAction(new RangePosition(1, 1, 2, 2)));
			Grid.Undo();
			Grid.Redo();
			AssertTrue(worksheet._Debug_Validate_All());

			Grid.DoAction(new SetRangeDataFormatAction(new RangePosition(1, 1, 2, 2),
				DataFormat.CellDataFormatFlag.Number,
				new NumberDataFormatter.NumberFormatArgs
				{
					DecimalPlaces = 4,
					UseSeparator = true,
				}));
			Grid.Undo();
			Grid.Redo();
			AssertTrue(worksheet._Debug_Validate_All());

			Grid.RepeatLastAction(new RangePosition(3, 3, 2, 2));
			Grid.Undo();
			Grid.Redo();
			AssertTrue(worksheet._Debug_Validate_All());

			
		}

	}
}
