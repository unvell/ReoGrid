/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * ReoGrid and ReoGrid Demo project is released under MIT license.
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Windows.Forms;

using unvell.ReoGrid.Graphics;

namespace unvell.ReoGrid.Demo.Features
{
	public partial class RichTextDemo : UserControl
	{
		private Worksheet sheet;

		public RichTextDemo()
		{
			InitializeComponent();

			var sheet = grid.CurrentWorksheet;

			sheet.MergeRange("B2:F6");

			sheet["B2"] = new Drawing.RichText()
				.Regular("The ")
				.Bold("Rich Text Format")
				.Regular(" (often abbreviated ")
				.Bold("RTF")
				.Regular(") is a proprietary")
				.Superscript("[6][7][8]")
				.Regular(" document file format with published specification developed by Microsoft Corporation from ")
				.Span("1987", textColor: SolidColor.OrangeRed)
				.Span(" until ", textColor: SolidColor.Black)
				.Span("2008", textColor: SolidColor.OrangeRed)
				.Span(" for cross-platform document interchange with Microsoft products.", textColor: SolidColor.Black);

			sheet.Cells["B2"].Style.TextWrap = TextWrapMode.WordBreak;
		}
		
	}
}
