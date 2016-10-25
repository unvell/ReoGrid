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

using unvell.ReoGrid.CellTypes;

namespace unvell.ReoGrid.Tests
{
	class TestCellBody : CellBody
	{
		public bool initialized = false;

		public override void OnSetup(Cell cell)
		{
			// set flag to specify that OnSetup method has been called actually
			this.initialized = true;
		}

		public override object OnSetData(object data)
		{
			// test for return an edited data
			return "(" + data + ")";
		}

		public bool editRequired = false;

		public override bool OnStartEdit()
		{
			// set flag to specify that editing operation has been called actually
			editRequired = true;

			// return false to stop editing operation
			return false;
		}
	}

	[TestSet]
	class CustomCellTest : ReoGridTestSet
	{
		[TestCase]
		void SetAndRemove()
		{
			SetUp();

			// create customize cell body for test
			var testCell = new TestCellBody();

			// apply this body to cell 1,1
			worksheet[1, 1] = testCell;

			// check the cell body whether it is inside the cell
			AssertEquals(worksheet.GetCell(1, 1).Body, testCell);

			// check flag to test whether OnSetup method has been called actually
			AssertEquals(testCell.initialized, true);

			// remove the cell body
			worksheet.RemoveCellBody(1, 1);

			// check body that must be not there
			AssertEquals(worksheet.GetCell(1, 1).Body, null);
		}

		[TestCase]
		void DataEdit()
		{
			var testCell = new TestCellBody();
			worksheet[1, 2] = testCell;

			// set data, this should triggers the OnSetData method of body
			worksheet[1, 2] = "abc";

			// check data that has been edited by cell body atucally
			AssertEquals(worksheet[1, 2], "(abc)");
		}

		[TestCase]
		void StopEdit()
		{
			var testCell = new TestCellBody();

			worksheet[1, 3] = testCell;
			worksheet.StartEdit(1, 3);

			// grid must be not in editing mode
			AssertEquals(worksheet.IsEditing, false);

			// edit-required flag must be set to true
			AssertEquals(testCell.editRequired, true);

		}
	}

}
