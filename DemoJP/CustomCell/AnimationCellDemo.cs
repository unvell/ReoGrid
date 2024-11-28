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

using unvell.ReoGrid.CellTypes;
using unvell.ReoGrid.DemoJP.Properties;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.Demo.CellTypeDemo
{
	/// <summary>
	/// カスタマイズしたアニメーションセル型のデモ
	/// </summary>
	public partial class AnimationCellDemo : UserControl
	{
		// アニメーションのためのタイマー
		Timer timer = new Timer();
		
		private LoadingCell loadingCell;

		private GifImageCell gifCell;

		private Worksheet worksheet;

		public AnimationCellDemo()
		{
			InitializeComponent();

			// タイマーを設定
			timer.Interval = 10;
			timer.Tick += timer_Tick;

			// ワークシートインスタンスを取得
			worksheet = grid.CurrentWorksheet;

			// 行の高さと列幅の調整
			worksheet.SetRowsHeight(5, 1, 100);
			worksheet.SetColumnsWidth(1, 5, 100);

			worksheet.SetRangeStyles(3, 1, 5, 5, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.HorizontalAlign | PlainStyleFlag.VerticalAlign,
				HAlign = ReoGridHorAlign.Center,
				VAlign = ReoGridVerAlign.Middle,
			});

			// ローディングデモ
			loadingCell = new LoadingCell();
			worksheet[3, 1] = loadingCell;

			// GIFイメージデモ
			gifCell = new GifImageCell(Resources.loading);
			worksheet[5, 2] = gifCell;

			// 点滅セルデモ
			worksheet[5, 4] = new BlinkCell();
			worksheet[5, 4] = "点滅セル";

			// 備考テキスト
			worksheet[7, 1] = "ワークシートのメソッド RequestInvalidate を利用するとワークシートを再描画させることができます。";

			// 備考テキストスタイル
			worksheet.SetRangeStyles(7, 1, 1, 1, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.TextColor | PlainStyleFlag.BackColor,
				BackColor = SolidColor.Orange,
				TextColor = SolidColor.White,
			});

			// 備考テキストセルを結合
			worksheet.MergeRange(7, 1, 1, 6);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			timer.Start();
		}

		void timer_Tick(object sender, EventArgs e)
		{
			// フレームを更新
			loadingCell.NextFrame();

			// セルボディを取得
			var cell = worksheet.Cells[5, 4];
			if (cell.Body is BlinkCell)
			{
				((BlinkCell)cell.Body).NextFrame();
			}

			// ワークシートの再描画を請求
			worksheet.RequestInvalidate();
		}

		protected override void DestroyHandle()
		{
			base.DestroyHandle();

			timer.Stop();
			timer.Dispose();
		}
	}

	/// <summary>
	/// ローディングセル
	/// </summary>
	class LoadingCell : CellBody
	{
		public LoadingCell()
		{
			ThumbSize = 30;
			StepSize = 1;
		}

		public void NextFrame()
		{
			if (dir > 0)
			{
				offset += StepSize;
				if (offset >= Bounds.Width - ThumbSize - StepSize) dir = -1;
			}
			else
			{
				offset -= StepSize;
				if (offset <= 0) dir = 1;
			}
		}

		private int offset = 0;
		private int dir = 1;

		public int ThumbSize { get; set; }
		public int StepSize { get; set; }

		public override void OnPaint(CellDrawingContext dc)
		{
			// 長方形を描画
			dc.Graphics.FillRectangle(new Rectangle(offset, 0, ThumbSize, Bounds.Height), SolidColor.SkyBlue);

			// コアの描画メソッドを呼び出してセルテキストを出力
			dc.DrawCellText();
		}
	}

	/// <summary>
	/// GIFイメージセル
	/// </summary>
	class GifImageCell : CellBody
	{
		public System.Drawing.Image Gif { get; set; }

		public GifImageCell(System.Drawing.Image gif)
		{
			this.Gif = gif;

			System.Drawing.ImageAnimator.Animate(Gif, OnFrameChanged);
		}

		private void OnFrameChanged(object o, EventArgs e)
		{
			lock (this.Gif) System.Drawing.ImageAnimator.UpdateFrames(Gif);
		}

		public override void OnPaint(CellDrawingContext dc)
		{
			lock (this.Gif) dc.Graphics.DrawImage(Gif, Bounds);

			// コアの描画メソッドを呼び出してセルテキストを出力
			dc.DrawCellText();
		}
	}

	/// <summary>
	/// 点滅セル
	/// </summary>
	class BlinkCell : CellBody
	{
		public BlinkCell()
		{
			StepSize = 2;
		}

		public void NextFrame()
		{
			if (dir > 0)
			{
				alpha += StepSize;
				if (alpha >= 100) dir = -1;
			}
			else
			{
				alpha -= StepSize;
				if (alpha <= 0) dir = 1;
			}
		}

		private int alpha = 0;
		private int dir = 1;

		public int StepSize { get; set; }

		public override void OnPaint(CellDrawingContext dc)
		{
			dc.Graphics.FillRectangle(Bounds, new SolidColor(alpha, SolidColor.Orange));

			// コアの描画メソッドを呼び出してセルテキストを出力
			dc.DrawCellText();
		}
	}

}
