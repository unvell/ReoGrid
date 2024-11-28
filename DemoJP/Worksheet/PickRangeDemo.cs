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
	/// ワークシートの範囲ポジションを選択するデモ
	/// </summary>
	public partial class PickRangeDemo : UserControl
	{
		private int currentRow = 0;

		public PickRangeDemo()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			StartPickRange();
		}

		private void StartPickRange()
		{
			grid.PickRange((inst, range) =>
			{
				// 範囲ポジションが選択された場合の処理

				MessageBox.Show("選択した範囲ポジション： " + range.ToAddress());

				if (currentRow < grid.CurrentWorksheet.RowCount - 1)
				{
					grid.CurrentWorksheet[currentRow++, 0] = range.ToAddress();
				}

				// trueを返すと選択を続ける；falseを返すと選択を終了
				return false;

			}, 

			// 選択時のカーソルスタイルを設定
			Cursors.Hand);
		}
	}
}
