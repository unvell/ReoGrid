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

namespace unvell.ReoGrid.Demo.WorksheetDemo
{
	/// <summary>
	/// 拡大縮小のデモ
	/// </summary>
	public partial class ZoomDemo : UserControl
	{
		private Worksheet worksheet;

		public ZoomDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;

			for (int r = 0; r < 50; r++)
			{
				for (int c = 0; c < 20; c++)
				{
					worksheet[r, c] = (r + 1) * (c + 1);
				}
			}
		}

		private void trackBar1_Scroll(object sender, EventArgs e)
		{
			worksheet.ScaleFactor = trackBar1.Value / 10f;

			label2.Text = "比率: " + (worksheet.ScaleFactor * 100) + "%";
		}
	}
}
