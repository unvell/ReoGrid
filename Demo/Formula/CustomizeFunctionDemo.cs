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

using unvell.ReoScript;

namespace unvell.ReoGrid.Demo.Formula
{
	public partial class CustomizeFunctionDemo : UserControl
	{
		private Worksheet worksheet;

		public CustomizeFunctionDemo()
		{
			InitializeComponent();

			worksheet = grid.CurrentWorksheet;

			worksheet["A1"] = "func1:";
			worksheet["A2"] = "func2:";
			worksheet["A3"] = "func3:";
		}

		private void btnAddCustomFunction_Click(object sender, EventArgs e)
		{
			// Core customize function supported from 0.8.8,
			// This interface does not require ReoScript module.

			unvell.ReoGrid.Formula.FormulaExtension.CustomFunctions["func1"] = 
				(cell, args) =>
				{
					return (args.Length < 1) ? null : ("[" + args[0] + "]");
				};

			worksheet["B1"] = "abc";
			worksheet["C1"] = "=func1(B1)";

		}

		private void btnAddReoScriptFunction_Click(object sender, EventArgs e)
		{
			// ReoScript language function
			
			this.grid.Srm["func2"] = new NativeFunctionObject("func2", (ctx, owner, args) =>
			{
				return (args.Length < 1) ? null : ("[" + args[0] + "]");
			});

			worksheet["B2"] = "xyz";
			worksheet["C2"] = "=func2(B2)";
		}

		private void btnAddReoScriptFunctionInScript_Click(object sender, EventArgs e)
		{
			// Run script to add ReoScript function

			grid.RunScript(@"

script.func3 = function(data) {
  return '[' + data + ']';
};

@");

			worksheet["B3"] = "qwe";
			worksheet["C3"] = "=func3(B3)";
		}

	}
}
