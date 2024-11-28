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

using unvell.ReoScript;

namespace unvell.ReoGrid.Tests
{
	[TestSet]
	class ScriptTest : ReoGridTestSet
	{

#if NO_LONGER_SUPPORTED_V088
		[TestCase]
		public void ScriptMathFunctions()
		{
			SetUp();

			// ReoScript built-in objects and functions

			worksheet[0, 0] = "=Math.floor(Math.sin(0.625)*100000)";
			AssertEquals(worksheet.GetCellText(0, 0), "58509");

			// cell reference sin
			worksheet[0, 1] = 0.625;
			worksheet[0, 2] = "=Math.floor(Math.sin(B1)*100000)";
			worksheet[0, 1] = 1.25;
			AssertEquals(worksheet.GetCellText(0, 2), "94898");
		}
#endif // NO_LONGER_SUPPORTED_V088

		[TestCase]
		public void ReoScriptFunction()
		{
			SetUp(20, 20);

			// custom function in script
			Grid.RunScript("script.myfun = data => '[' + data + ']'; ");
			worksheet[10, 0] = "=myfun(\"abc\")";
			AssertEquals(worksheet.GetCellText(10, 0), "[abc]");

			// custom function in .NET
			Grid.Srm["add"] = new NativeFunctionObject("add", (ctx, owner, args) =>
			{
				int v1 = ScriptRunningMachine.GetIntParam(args, 0);
				int v2 = ScriptRunningMachine.GetIntParam(args, 1);
				return v1 + v2;
			});

			worksheet[0, 10] = "=add(2,3)";
			AssertEquals(worksheet.GetCellText(0, 10), "5");
		}

		[TestCase]
		public void RangeReference()
		{
			worksheet.Reset(20, 20);
			worksheet[0, 0] = 10;
			worksheet[1, 0] = 20;

			worksheet[10, 10] = "=SUM(1,1,10,10)";
		}

#if NO_LONGER_SUPPORTED_V088
		
		[TestCase]
		public void FreeFormulaEvaluation()
		{
			// free formula does not need '=' prefix
			string formula = "1+2*3";
			var result = worksheet.EvaluateFormula(formula);
			AssertEquals(result, (double)7);

			worksheet["K1"] = "3";
			worksheet["K2"] = "5";
			formula = "K1+K2*2";
			AssertEquals(worksheet.EvaluateFormula(formula), (double)13);

			worksheet["E10"] = "hello";
			var expression = "E10+'world'+'!'.repeat(3)";
			AssertEquals(worksheet.EvaluateFormula(expression), "helloworld!!!");
		}

#endif // NO_LONGER_SUPPORTED_V088

		[TestCase]
		public void RangeCalc()
		{
			var range = new RangePosition(10, 0, 3, 3);

			worksheet[range] = new object[,] {
				{1,2,3},
				{4,5,6},
				{7,8,9},
			};

			worksheet[10, 5] = "=SUM(" + range.ToAddress() + ")";
			AssertEquals(worksheet.GetCellData<int>(10, 5), 45);

#if NO_LONGER_SUPPORTED_V088
			worksheet[10, 6] = "=SUM(new Range(10, 0, 3, 3))";
			AssertEquals(worksheet.GetCellText(10, 6), "45");
			AssertEquals(worksheet.GetCellText(10, 5), worksheet.GetCellText(10, 6));
#endif // NO_LONGER_SUPPORTED_V088

		}

	}
}
