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
	/// 編集可能な範囲のデモ
	/// </summary>
	public partial class SetEditableRangeDemo : UserControl
	{
		public SetEditableRangeDemo()
		{
			InitializeComponent();

			grid.CurrentWorksheet["B2"] = "右のボタンをクリックすると編集可能な範囲を設定できます。";
		}

		private void btnSetEditableRange_Click(object sender, EventArgs ee)
		{
			var editableRange = new RangePosition("B5:E6");

			var worksheet = grid.CurrentWorksheet;

			worksheet.SetRangeBorders(editableRange, BorderPositions.Outside, RangeBorderStyle.BlackSolid);

			worksheet["B4"] = "編集可能な範囲：";
			worksheet.BeforeCellEdit += (s, e) => e.IsCancelled = !editableRange.Contains(e.Cell.Position);
		}

	}
}
