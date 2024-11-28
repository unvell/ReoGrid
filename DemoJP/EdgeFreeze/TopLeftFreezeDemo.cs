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

namespace unvell.ReoGrid.Demo.WorksheetDemo.EdgeFreeze
{
	public partial class TopLeftFreezeDemo : UserControl
	{
		public TopLeftFreezeDemo()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			var worksheet = reoGridControl.CurrentWorksheet;

			worksheet.Resize(100, 30);

			// freeze to top left
			worksheet.FreezeToCell(10, 5, FreezeArea.LeftTop);

			worksheet[5, 1] = "frozen region";
			worksheet[15, 7] = "active region";
		}
	}
}
