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

using unvell.ReoGrid.Events;

namespace unvell.ReoGrid.Demo.WorksheetDemo
{
	/// <summary>
	/// イベントを利用して数字のみ入力のデモ
	/// </summary>
	public partial class OnlyNumberInputDemo : UserControl
	{
		private Worksheet worksheet;

		public OnlyNumberInputDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;

			worksheet.AfterCellEdit += grid_AfterCellEdit;

			chkErrorPrompt.CheckedChanged += (s, e) =>
			{
				if (!chkOnlyNumeric.Checked) chkOnlyNumeric.Checked = true;
			};
		}

		void grid_AfterCellEdit(object sender, CellAfterEditEventArgs e)
		{
			if (chkOnlyNumeric.Checked)
			{
				if (e.NewData == null || !float.TryParse(e.NewData.ToString(), out var val))
				{
					if (chkErrorPrompt.Checked)
					{
						MessageBox.Show("数字以外の文字は入力できません。");
					}
				
					e.EndReason = EndEditReason.Cancel;
				}
				else
				{
					e.NewData = val;
				}
			}
		}
	}
}
