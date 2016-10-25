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

using unvell.ReoGrid.Graphics;

namespace unvell.ReoGrid.Tests.TestCases
{
	[TestSet]
	class RichTextTest : ReoGridTestSet
	{
		[TestCase]
		void GenericTest()
		{
			SetUp(20, 20);

			var rt = new Drawing.RichText();
			rt.Span("Hello World")
				.Bold("bold")
				.NewLine()
				.Italic("italic")
				.Underline("underline")
				.NewLine()
				.Regular("new line")
				.Superscript("superscript")
				.Subscript("subscript")
				.SetStyles(paragraphSpacing: 2.0f)
				.NewLine()
				.SetStyles(halign: ReoGridHorAlign.Right)
				.Span("right aligned")
				.NewLine()
				.Span("back color", backColor: SolidColor.LightCoral)
				.Span("fore color", textColor: SolidColor.DarkOrange)
				.Span("big font", fontSize: 28)
				.Span("small font", fontName: "Arial", fontSize: 6f)
				.NewLine()
				.Span("end")
				;

			worksheet["A1"] = rt;
		}

		[TestCase]
		void CopyRichText()
		{
			var rt = new Drawing.RichText();
			rt.Span("Hello ").Bold("World").Regular("!");

			worksheet["B1"] = rt;
			var pg = worksheet.GetPartialGrid("B1");
			worksheet.SetPartialGrid("C1", pg);

			AssertTrue(worksheet.GetCellData("C1").GetType() == typeof(Drawing.RichText));
			AssertSame(worksheet.GetCellText("C1"), "Hello World!");
		}

	}

}
