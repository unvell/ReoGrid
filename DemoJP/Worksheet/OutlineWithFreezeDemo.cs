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

using System.Windows.Forms;

namespace unvell.ReoGrid.Demo.WorksheetDemo
{
	/// <summary>
	/// アウトラインと行列固定のデモ
	/// </summary>
	public partial class OutlineWithFreezeDemo : UserControl
	{
		public OutlineWithFreezeDemo()
		{
			InitializeComponent();

			var worksheet = grid.CurrentWorksheet;

			for (int i = 1; i < 9; i++)
			{
				worksheet.GroupRows(4, i);
			}
			for (int i = 1; i < 9; i++)
			{
				worksheet.GroupColumns(2, i);
			}

			worksheet.FreezeToCell(5, 5);

			worksheet[2, 2] = "アウトラインと行列固定のデモ";
		}
	}
}
