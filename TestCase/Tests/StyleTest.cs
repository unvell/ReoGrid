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
	class StyleTest : ReoGridTestSet
	{
		[TestCase]
		public void SetStyleTest()
		{
			SetUp();

			worksheet.SetRangeStyles(new RangePosition(1, 1, 5, 5), new WorksheetRangeStyle()
			{
				Flag = PlainStyleFlag.BackColor,
				BackColor = Color.DarkBlue,
			});

			worksheet.SetRangeStyles(new RangePosition(2, 1, 1, 1), new WorksheetRangeStyle()
			{
				Flag = PlainStyleFlag.TextColor,
				TextColor = Color.Cyan,
			});

			worksheet.SetRangeStyles(new RangePosition(2, 2, 1, 1), new WorksheetRangeStyle()
			{
				Flag = PlainStyleFlag.HorizontalAlign,
				HAlign = ReoGridHorAlign.Right,
			});

			worksheet.SetRangeStyles(new RangePosition(1, 2, 1, 1), new WorksheetRangeStyle()
			{
				Flag = PlainStyleFlag.FontStyleBold,
				Bold = true,
			});

			worksheet.SetRangeStyles(new RangePosition(3, 1, 1, 1), new WorksheetRangeStyle()
			{
				Flag = PlainStyleFlag.TextWrap,
				TextWrapMode = TextWrapMode.WordBreak,
			});

			AssertHasStyle(worksheet.GetCellStyles(1, 1).Flag, PlainStyleFlag.BackColor);
			AssertHasStyle(worksheet.GetCellStyles(2, 1).Flag, PlainStyleFlag.TextColor | PlainStyleFlag.BackColor);
			AssertHasStyle(worksheet.GetCellStyles(2, 2).Flag, PlainStyleFlag.HorizontalAlign | PlainStyleFlag.BackColor);
			AssertHasStyle(worksheet.GetCellStyles(1, 2).Flag, PlainStyleFlag.FontStyleBold | PlainStyleFlag.BackColor);
			AssertHasStyle(worksheet.GetCellStyles(3, 1).Flag, PlainStyleFlag.TextWrap | PlainStyleFlag.BackColor);

			AssertTrue(worksheet.GetCellStyles(1, 1).BackColor == Color.DarkBlue);
			AssertTrue(worksheet.GetCellStyles(2, 1).TextColor == Color.Cyan);
			AssertTrue(worksheet.GetCellStyles(2, 2).HAlign == ReoGridHorAlign.Right);
			AssertTrue(worksheet.GetCellStyles(1, 2).Bold == true);
			AssertTrue(worksheet.GetCellStyles(3, 1).TextWrapMode == TextWrapMode.WordBreak);
		}

		private void AssertHasStyle(PlainStyleFlag flag, PlainStyleFlag target)
		{
			if ((flag & target) != target)
			{
				TestAssert.Failure(string.Format("expect has " + target + " but not"));
			}
		}

		[TestCase]
		public void SetStyleByActionTest()
		{
			SetUp();

			Grid.DoAction(new SetRangeStyleAction(new RangePosition(1, 1, 5, 5), new WorksheetRangeStyle()
			{
				Flag = PlainStyleFlag.BackColor,
				BackColor = Color.DarkBlue,
			}));

			Grid.DoAction(new SetRangeStyleAction(new RangePosition(2, 1, 1, 1), new WorksheetRangeStyle()
			{
				Flag = PlainStyleFlag.TextColor,
				TextColor = Color.Cyan,
			}));

			Grid.DoAction(new SetRangeStyleAction(new RangePosition(2, 2, 1, 1), new WorksheetRangeStyle()
			{
				Flag = PlainStyleFlag.HorizontalAlign,
				HAlign = ReoGridHorAlign.Right,
			}));

			Grid.DoAction(new SetRangeStyleAction(new RangePosition(1, 2, 1, 1), new WorksheetRangeStyle()
			{
				Flag = PlainStyleFlag.FontStyleBold,
				Bold = true,
			}));

			Grid.DoAction(new SetRangeStyleAction(new RangePosition(3, 1, 1, 1), new WorksheetRangeStyle()
			{
				Flag = PlainStyleFlag.TextWrap,
				TextWrapMode = TextWrapMode.WordBreak,
			}));

			AssertHasStyle(worksheet.GetCellStyles(1, 1).Flag, PlainStyleFlag.BackColor);
			AssertHasStyle(worksheet.GetCellStyles(2, 1).Flag, PlainStyleFlag.TextColor | PlainStyleFlag.BackColor);
			AssertHasStyle(worksheet.GetCellStyles(2, 2).Flag, PlainStyleFlag.HorizontalAlign | PlainStyleFlag.BackColor);
			AssertHasStyle(worksheet.GetCellStyles(1, 2).Flag, PlainStyleFlag.FontStyleBold | PlainStyleFlag.BackColor);
			AssertHasStyle(worksheet.GetCellStyles(3, 1).Flag, PlainStyleFlag.TextWrap | PlainStyleFlag.BackColor);

			AssertTrue(worksheet.GetCellStyles(1, 1).BackColor == Color.DarkBlue);
			AssertTrue(worksheet.GetCellStyles(2, 1).TextColor == Color.Cyan);
			AssertTrue(worksheet.GetCellStyles(2, 2).HAlign == ReoGridHorAlign.Right);
			AssertTrue(worksheet.GetCellStyles(1, 2).Bold == true);
			AssertTrue(worksheet.GetCellStyles(3, 1).TextWrapMode == TextWrapMode.WordBreak);

			// undo all
			while (Grid.CanUndo())
			{
				Grid.Undo();
			}

			// get root style
			WorksheetRangeStyle rootStyle = worksheet.GetRangeStyles(new RangePosition(0, 0, 1, 1));

			// compare to root style
			AssertTrue(worksheet.GetCellStyles(1, 1).Flag == rootStyle.Flag);
			AssertTrue(worksheet.GetCellStyles(2, 1).Flag == rootStyle.Flag);
			AssertTrue(worksheet.GetCellStyles(2, 2).Flag == rootStyle.Flag);
			AssertTrue(worksheet.GetCellStyles(1, 2).Flag == rootStyle.Flag);
			AssertTrue(worksheet.GetCellStyles(3, 1).Flag == rootStyle.Flag);

			AssertEquals(worksheet.GetCellStyles(1, 1).BackColor, rootStyle.BackColor);
			AssertEquals(worksheet.GetCellStyles(2, 1).TextColor, rootStyle.TextColor);
			AssertEquals(worksheet.GetCellStyles(2, 2).HAlign, rootStyle.HAlign);
			AssertEquals(worksheet.GetCellStyles(1, 2).Bold, rootStyle.Bold);
			AssertEquals(worksheet.GetCellStyles(3, 1).TextWrapMode, rootStyle.TextWrapMode);


		}

		[TestCase]
		public void FullRowColumnAction()
		{
			SetUp(20, 20);

			worksheet.SetRangeStyles(new RangePosition(4, 4, 2, 2), new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.BackColor,
				BackColor = Color.Red
			});

			AssertEquals(worksheet.GetCellStyles(4, 4).BackColor, Color.Red);

			// full row
			Grid.DoAction(new SetRangeStyleAction(new RangePosition(4, 0, 1, 20), new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.BackColor,
				BackColor = Color.Blue,
			}));

			for (int c = 0; c < 20; c++)
			{
				AssertEquals(worksheet.GetCellStyles(4, c).BackColor, Color.Blue);
			}

			Grid.Undo();

			for (int c = 0; c < 20; c++)
			{
				AssertEquals(worksheet.GetCellStyles(4, c).BackColor, (c == 4 || c == 5) ? Color.Red : Color.Empty);
			}
			
			// ---------------------------------------------------

			// full col
			Grid.DoAction(new SetRangeStyleAction(new RangePosition(0, 4, 20, 2), new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.BackColor,
				BackColor = Color.Blue,
			}));

			for (int r = 0; r < 20; r++)
			{
				AssertEquals(worksheet.GetCellStyles(r, 4).BackColor, Color.Blue);
				AssertEquals(worksheet.GetCellStyles(r, 5).BackColor, Color.Blue);
			}

			Grid.Undo();

			for (int r = 0; r < 20; r++)
			{
				AssertEquals(worksheet.GetCellStyles(r, 4).BackColor, (r == 4 || r == 5) ? Color.Red : Color.Empty);
			}

			Grid.Undo();
		}

		[TestCase]
		public void FullGridUndo()
		{
			SetUp(20, 20);

			// single cell
			Grid.DoAction(new SetRangeStyleAction(new RangePosition(4, 4, 1, 1), new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.BackColor,
				BackColor = Color.Blue,
			}));

			// full column
			Grid.DoAction(new SetRangeStyleAction(new RangePosition(0, 4, 20, 1), new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.BackColor,
				BackColor = Color.Red,
			}));

			// full grid
			Grid.DoAction(new SetRangeStyleAction(new RangePosition(0, 0, 20, 20), new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.BackColor,
				BackColor = Color.Yellow,
			}));

			// test full grid
			AssertEquals(worksheet.GetCellStyles(4, 4).BackColor, Color.Yellow);
			AssertEquals(worksheet.GetCellStyles(5, 4).BackColor, Color.Yellow);
			AssertEquals(worksheet.GetCellStyles(0, 0).BackColor, Color.Yellow);
			AssertEquals(worksheet.GetCellStyles(0, 19).BackColor, Color.Yellow);
			AssertEquals(worksheet.GetCellStyles(19, 0).BackColor, Color.Yellow);
			AssertEquals(worksheet.GetCellStyles(19, 19).BackColor, Color.Yellow);

			Grid.Undo();

			// test full column
			AssertEquals(worksheet.GetCellStyles(4, 4).BackColor, Color.Red);
			AssertEquals(worksheet.GetCellStyles(5, 4).BackColor, Color.Red);
			AssertEquals(worksheet.GetCellStyles(0, 4).BackColor, Color.Red);
			AssertEquals(worksheet.GetCellStyles(19, 4).BackColor, Color.Red);

			AssertEquals(worksheet.GetCellStyles(0, 0).BackColor, Color.Empty);
			AssertEquals(worksheet.GetCellStyles(0, 19).BackColor, Color.Empty);
			AssertEquals(worksheet.GetCellStyles(19, 0).BackColor, Color.Empty);
			AssertEquals(worksheet.GetCellStyles(19, 19).BackColor, Color.Empty);

			Grid.Undo();

			// test full grid
			AssertEquals(worksheet.GetCellStyles(4, 4).BackColor, Color.Blue);
			AssertEquals(worksheet.GetCellStyles(5, 4).BackColor, Color.Empty);

			Grid.Undo();

			AssertEquals(worksheet.GetCellStyles(4, 4).BackColor, Color.Empty);
			AssertEquals(worksheet.GetCellStyles(5, 4).BackColor, Color.Empty);
			AssertEquals(worksheet.GetCellStyles(0, 4).BackColor, Color.Empty);
			AssertEquals(worksheet.GetCellStyles(19, 4).BackColor, Color.Empty);

			AssertEquals(worksheet.GetCellStyles(0, 0).BackColor, Color.Empty);
			AssertEquals(worksheet.GetCellStyles(0, 19).BackColor, Color.Empty);
			AssertEquals(worksheet.GetCellStyles(19, 0).BackColor, Color.Empty);
			AssertEquals(worksheet.GetCellStyles(19, 19).BackColor, Color.Empty);

		}

		[TestCase]
		void FullColumnOverCellOwn()
		{
			SetUp(20, 20);

			worksheet.SetRangeStyles(RangePosition.EntireRange, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.BackColor | PlainStyleFlag.TextColor,
				BackColor = Color.Gray,
				TextColor = Color.Silver,
			});

			AssertEquals(worksheet.Cells[0, 0].Style.BackColor, Color.Gray);

			worksheet[1, 0] = 111;

			AssertEquals(worksheet.Cells[0, 0].Style.BackColor, Color.Gray);

			worksheet.ColumnHeaders[0].Style.BackColor = Color.LightYellow;

			AssertEquals(worksheet.Cells[0, 0].Style.BackColor, Color.LightYellow);
			AssertEquals(worksheet.Cells[1, 0].Style.BackColor, Color.LightYellow);
			AssertEquals(worksheet.Cells[2, 0].Style.BackColor, Color.LightYellow);
			
			AssertEquals(worksheet.Cells[0, 1].Style.BackColor, Color.Gray);
		}

		[TestCase]
		void FullRowOverFullColumn()
		{
			worksheet.RowHeaders[1].Style.Bold = true;

			AssertEquals(worksheet.Cells[0, 0].Style.BackColor, Color.LightYellow);
	
			AssertEquals(worksheet.Cells[1, 0].Style.BackColor, Color.LightYellow);
			AssertEquals(worksheet.Cells[1, 0].Style.Bold, true);

			AssertEquals(worksheet.Cells[1, 2].Style.BackColor, Color.Gray);
			AssertEquals(worksheet.Cells[1, 2].Style.Bold, true);

			AssertEquals(worksheet.Cells[2, 0].Style.BackColor, Color.LightYellow);
			AssertEquals(worksheet.Cells[0, 1].Style.BackColor, Color.Gray);
		}

		[TestCase]
		void AttemptRemoveDefaultStyle()
		{
			SetUp(10, 10);

			worksheet["A1"] = "hello";
			worksheet.RemoveRangeStyles("A1", PlainStyleFlag.All);

			// cell doesn't have any styles
			// there should no styles was removed.

			var styles = worksheet.GetCellStyles("A1");

			// all styles should be same with Default Style of worksheet
			AssertTrue(styles.HasStyle(PlainStyleFlag.FontName));
			AssertTrue(styles.HasStyle(PlainStyleFlag.FontSize));
			AssertSame(styles.Flag, Worksheet.DefaultStyle.Flag);
		}

		[TestCase]
		void RemoveCellTextColorStyle()
		{
			SetUp(10, 10);

			worksheet["A1"] = "hello";
			worksheet.SetRangeStyles("A1", new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.TextColor,
				TextColor = Color.Red,
			});

			worksheet.RemoveRangeStyles("A1", PlainStyleFlag.All);

			var styles = worksheet.GetCellStyles("A1");

			// cell text color style must be removed
			AssertTrue(!styles.HasStyle(PlainStyleFlag.TextColor));

			// all styles should be same with Default Style of worksheet
			AssertTrue(styles.HasStyle(PlainStyleFlag.FontName));
			AssertTrue(styles.HasStyle(PlainStyleFlag.FontSize));
			AssertSame(styles.Flag, Worksheet.DefaultStyle.Flag);
		}

		[TestCase]
		void FullRowOverCellOwn()
		{
			SetUp(20, 20);

			worksheet.SetRangeStyles(RangePosition.EntireRange, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.BackColor | PlainStyleFlag.TextColor,
				BackColor = Color.Gray,
				TextColor = Color.Silver,
			});

			AssertEquals(worksheet.Cells[0, 0].Style.BackColor, Color.Gray);

			worksheet[0, 1] = 222;

			AssertEquals(worksheet.Cells[0, 0].Style.BackColor, Color.Gray);

			worksheet.RowHeaders[0].Style.BackColor = Color.LightYellow;

			AssertEquals(worksheet.Cells[0, 0].Style.BackColor, Color.LightYellow);
			AssertEquals(worksheet.Cells[0, 1].Style.BackColor, Color.LightYellow);
			AssertEquals(worksheet.Cells[0, 2].Style.BackColor, Color.LightYellow);

			AssertEquals(worksheet.Cells[1, 0].Style.BackColor, Color.Gray);
		}

		[TestCase]
		void RemoveStyle_RestoreToOriginal()
		{
			SetUp(20, 20);

			var pos = new CellPosition("B2");

			worksheet.SetRangeStyles(new RangePosition(pos, pos), new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.BackColor,
				BackColor = Color.Silver,
			});

			worksheet.RemoveRangeStyles(new RangePosition(pos, pos), PlainStyleFlag.BackColor);

			var style = worksheet.GetCellStyles(pos);
			AssertEquals(style.BackColor, Color.Empty, "BackColor not removed");
		}

		[TestCase]
		void SkipSetRangeStyleWhenNotFullyContain()
		{
			SetUp(10, 10);
			
			worksheet.MergeRange("C3:E5");

			worksheet.SetRangeStyles("C1:C10", new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.BackColor,
				BackColor = Color.Blue,
			});

			AssertEquals(worksheet.GetCellStyles("C3").BackColor, Color.Empty);

			worksheet.SetRangeStyles("A3:J3", new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.BackColor,
				BackColor = Color.Green,
			});

			AssertEquals(worksheet.GetCellStyles("C3").BackColor, Color.Empty);

			worksheet.SetRangeStyles("C3", new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.BackColor,
				BackColor = Color.Red,
			});

			AssertEquals(worksheet.GetCellStyles("C3").BackColor, Color.Red);

		}

		[TestCase]
		void CellInstanceStyleCopy()
		{
			SetUp(10, 10);

			worksheet.Cells["C3"].Style.BackColor = Color.Red;

			worksheet.Cells["C4"].Style = worksheet.Cells["C3"].Style;

			worksheet.Cells["C3"].Style.TextColor = Color.Yellow;
			worksheet.Cells["C4"].Style.TextColor = Color.White;

			AssertEquals(worksheet.Cells["C3"].Style.TextColor, Color.Yellow);			
			AssertEquals(worksheet.Cells["C4"].Style.TextColor, Color.White);
		}
	}
}
