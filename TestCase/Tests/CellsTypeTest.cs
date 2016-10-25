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
using unvell.ReoGrid.CellTypes;
using unvell.ReoGrid.Events;

namespace unvell.ReoGrid.Tests
{
	[TestSet]
	class CellsTypeTest : ReoGridTestSet
	{
		[TestCase]
		public void ButtonEvent()
		{
			SetUp();

			var btn = new ButtonCell("test");
			worksheet["A1"] = btn;

			bool eventClicked = false;
			btn.Click += (s, e) => eventClicked = true;
			btn.PerformClick();

			AssertTrue(eventClicked, "button event");
		}

		[TestCase]
		public void CheckboxEvent()
		{
			var checkbox = new CheckBoxCell();
			worksheet["B1"] = checkbox;

			bool eventCheckChanged = false;
			checkbox.CheckChanged += (s, e) => eventCheckChanged = true;

			bool cellDataChanged = false;
			EventHandler<CellEventArgs> cellDataChangeHandler = (s, e) => cellDataChanged = true;
			worksheet.CellDataChanged += cellDataChangeHandler;

			worksheet["B1"] = true;

			worksheet.CellDataChanged -= cellDataChangeHandler;
			
			AssertTrue(eventCheckChanged, "CheckChanged");
			AssertTrue(cellDataChanged, "CellDataChanged");
		} 

	}
}
