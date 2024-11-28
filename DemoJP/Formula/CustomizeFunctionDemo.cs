/*****************************************************************************
 * 
 * ReoGrid - .NET 表計算スプレッドシートコンポーネント
 * https://reogrid.net/jp
 *
 * ReoGrid 日本語版デモプロジェクトは MIT ライセンスでリリースされています。
 * 
 * このソフトウェアは無保証であり、このソフトウェアの使用により生じた直接・間接の損害に対し、
 * 著作権者は補償を含むあらゆる責任を負いません。 
 * 
 * Copyright (c) 2012-2016 unvell.com, All Rights Reserved.
 * https://www.unvell.com/jp
 * 
 ****************************************************************************/

using System;
using System.Windows.Forms;

using unvell.ReoScript;

namespace unvell.ReoGrid.Demo.FormulaDemo
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
			// FormulaExtension インターフェイスを利用してカスタマイズした関数を作成

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
