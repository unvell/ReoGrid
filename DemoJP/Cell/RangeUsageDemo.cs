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

namespace unvell.ReoGrid.Demo.CellDemo
{
	public partial class RangeUsageDemo : UserControl
	{
		private Worksheet sheet;

		public RangeUsageDemo()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		
			sheet = this.grid.CurrentWorksheet;

			this.Set2DArrayDataIntoRange(grid.CurrentWorksheet);
		}

		void Set2DArrayDataIntoRange(Worksheet sheet)
		{
			sheet["A1"] = new object[,] { 
				{ 1, 2, 3, 4, 5},
				{ "A", "B", "C", "D", "E"},
				{ DateTime.Now, new DateTime(2015, 3, 26), "=A1*2", "=NOW()", null },
			};
		}


	}
}
