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
	public partial class MergeCellsDemo : UserControl
	{
		private Worksheet worksheet;

		public MergeCellsDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;

			worksheet.SelectionRangeChanged += (s, e) =>
			{
				label1.Text = "選択範囲：" + worksheet.SelectionRange.ToString();
			};
		}

		private void btnMerge_Click(object sender, EventArgs e)
		{
			if (worksheet.SelectionRange.Rows == 1 && worksheet.SelectionRange.Cols == 1)
			{
				MessageBox.Show("二つ以上のセルを選択してください。");
			}
			else
			{
				try
				{
					worksheet.MergeRange(worksheet.SelectionRange);
				}
				catch (RangeIntersectionException)
				{
					MessageBox.Show("結合したセルの一部を他のセルと二重結合できません。");
				}
			}
		}

		private void btnUnmerge_Click(object sender, EventArgs e)
		{
			worksheet.UnmergeRange(worksheet.SelectionRange);
		}

		private void btnMergeByScript_Click(object sender, EventArgs e)
		{
			if (worksheet.SelectionRange.Rows == 1 && worksheet.SelectionRange.Cols == 1)
			{
				MessageBox.Show("二つ以上のセルを選択してください。");
			}
			else
			{
				try
				{
					this.grid.RunScript(txtMergeScript.Text);
				}
				catch (Exception ex)
				{
					MessageBox.Show("スクリプトエラー：" + ex.Message);
				}
			}
		}

		private void btnUnmergeByScript_Click(object sender, EventArgs e)
		{
			try
			{
				this.grid.RunScript(txtUnmergeScript.Text);
			}
			catch (Exception ex)
			{
				MessageBox.Show("スクリプトエラー：" + ex.Message);
			}
		}

		private void btnSimulateException_Click(object sender, EventArgs e)
		{
			worksheet.Reset();

			try
			{
				// try to merge an intersected range will get an exception
				worksheet.MergeRange(new RangePosition(2, 2, 5, 5));
				worksheet.MergeRange(new RangePosition(3, 3, 5, 5));
			}
			catch(RangeIntersectionException)
			{
				MessageBox.Show("RangeIntersectionExceptionをキャッチしました。\n\n結合したセルの一部を他のセルと二重結合できません。");
			}
		}

		private void lnkScript_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			lnkScript.Visible = false;
			label2.Visible = label3.Visible = true;
		}
	}
}
