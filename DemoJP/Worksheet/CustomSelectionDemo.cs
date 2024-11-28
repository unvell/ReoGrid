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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using unvell.ReoGrid.Interaction;

namespace unvell.ReoGrid.Demo.WorksheetDemo
{
	/// <summary>
	/// カスタマイズした選択モードのデモ
	/// https://reogrid.net/jp/document/worksheet/selection/
	/// </summary>
	public partial class CustomSelectionDemo : UserControl
	{
		private Worksheet worksheet;

		public CustomSelectionDemo()
		{
			InitializeComponent();

			string path = "_Templates\\RGF\\order_sample.rgf";

			// シート切り替えタブを非表示（ワークブックの設定）
			this.grid.SetSettings(WorkbookSettings.View_ShowSheetTabControl, false);
	
			// 表示中のワークシートを取得
			this.worksheet = this.grid.CurrentWorksheet;

			// テンプレートファイルからスプレッドシートを読み込む
			this.worksheet.Load(path);
			
			// マウスドラッグでのセルの移動を禁止（ワークシートの設定）
			this.worksheet.SetSettings(WorksheetSettings.Edit_DragSelectionToMoveCells, false);

			// 選択範囲変更前イベントを処理
			this.worksheet.BeforeSelectionRangeChange += worksheet_BeforeSelectionRangeChange;

			// セル中キーボードの入力を処理
			this.worksheet.BeforeCellKeyDown += worksheet_BeforeCellKeyDown;

			// 選択可能な範囲にスタイルを設定
			foreach (var addr in this.validRanges)
			{
				this.worksheet.SetRangeStyles(addr, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.BackColor,
					BackColor = Color.LightYellow,
				});
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			this.grid.Focus();
		}

		/// <summary>
		/// 選択可能な範囲
		/// </summary>
		private List<string> validRanges = new List<string>() 
		{
			"A1:A2", "A5:A8", "G2:G3", "A11:A15", "D11:D15", "A18:G18",
			"A21:G35", "A38:D42", "G36:G41", "F46:G46", "A49:G49",
		};

		void worksheet_BeforeSelectionRangeChange(object sender, Events.BeforeSelectionChangeEventArgs e)
		{
			// 選択範囲を制御する場合
			if (chkLimitSelection.Checked)
			{
				// 選択範囲が変更された場合、選択可能な範囲に囲まれるかどうかをチェックする
				// 選択可能な範囲からはみ出る場合、IsCancelledプロパティをtrueに設定して選択操作を禁止
				e.IsCancelled = !validRanges.Any(vr =>
				{
					var range = new RangePosition(vr);
					return range.Contains(e.SelectionStart) && range.Contains(e.SelectionEnd);
				});
			}
		}

		void worksheet_BeforeCellKeyDown(object sender, Events.BeforeCellKeyDownEventArgs e)
		{
			// 選択範囲を制御する場合
			if (chkLimitSelection.Checked)
			{
				// Tabキーが押された場合の処理
				if (chkTabToNextBlock.Checked
					&& (e.KeyCode | KeyCode.Tab) == KeyCode.Tab)
				{
					int index = this.validRanges.FindIndex(vr => new RangePosition(vr).Contains(e.CellPosition));

					index++;
					if (index >= this.validRanges.Count) index = 0;

					this.worksheet.SelectionRange = new RangePosition(validRanges[index]);
				}
				// Tab以外のキーが押された場合の処理（例えば選択範囲の移動）
				else
				{
					// 選択範囲が変更された場合、選択可能な範囲に囲まれるかどうかをチェックする
					// 選択可能な範囲からはみ出る場合、IsCancelledプロパティをtrueに設定して選択操作を禁止
					e.IsCancelled = !validRanges.Any(vr => new RangePosition(vr).Contains(e.CellPosition));
				}
			}
		}

		private void chkLimitSelection_CheckedChanged(object sender, EventArgs e)
		{
			if (this.chkLimitSelection.Checked)
			{
				var firstRange = new RangePosition(validRanges[0]);

				// 選択範囲を選択可能な範囲に格納
				this.worksheet.SelectionRange = new RangePosition(firstRange.Row, firstRange.Col, 1, 1);
			}
			else
			{
				// Tabキーの制御は選択範囲の制御が有効化された場合のみ有効
				this.chkTabToNextBlock.Enabled = false;
			}
		}
	}
}
