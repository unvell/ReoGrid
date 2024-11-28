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

namespace unvell.ReoGrid.Demo.WorkbookDemo
{
	/// <summary>
	/// ワークシートスクロール同期化のデモ
	/// </summary>
	public partial class SynchronizeScrollDemo : UserControl
	{
		public SynchronizeScrollDemo()
		{
			InitializeComponent();

			// RGF ファイルからワークシートを読み込む
			//
			// RGF はワークシートのテンプレートファイルフォーマットで、ReoGrid/ReoGridEditor で作成、編集できます。
			// 詳しくは、以下の URL をご覧ください。
			// https://reogrid.net/jp/document/rgf-format
			// 
			reoGridControl1.CurrentWorksheet.LoadRGF("_Templates\\RGF\\order_sample.rgf");
			reoGridControl2.CurrentWorksheet.LoadRGF("_Templates\\RGF\\order_sample.rgf");

			// スクロールの同期化（１から２へ）
			reoGridControl1.WorksheetScrolled += (s, e) =>
			{
				if (!this.inScrolling)
				{
					this.inScrolling = true;
					reoGridControl2.ScrollCurrentWorksheet(e.X, e.Y);
					this.inScrolling = false;
				}
			};

			// スクロールの同期化（２から１へ）
			reoGridControl2.WorksheetScrolled += (s, e) =>
			{
				if (!this.inScrolling)
				{
					this.inScrolling = true;
					reoGridControl1.ScrollCurrentWorksheet(e.X, e.Y);
					this.inScrolling = false;
				}
			};
		}

		/// <summary>
		/// 自動スクロール中のためのフラグ
		/// </summary>
		private bool inScrolling = false;

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			reoGridControl1.Width = (this.ClientRectangle.Width - panel1.Width - splitter1.Width) / 2;
		}

		private void btnScrollToTop_Click(object sender, EventArgs e)
		{
			// 下から上まで自動スクロール
			timer2.Enabled = false;
			timer1.Enabled = true;
		}

		private void btnScrollToBottom_Click(object sender, EventArgs e)
		{
			// 上から下まで自動スクロール
			timer1.Enabled = false;
			timer2.Enabled = true;
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			// 少しずつワークシートをスクロール
			reoGridControl1.ScrollCurrentWorksheet(0, -2);
		}

		private void timer2_Tick(object sender, EventArgs e)
		{
			// 少しずつワークシートをスクロール
			reoGridControl1.ScrollCurrentWorksheet(0, 2);
		}

	}
}
