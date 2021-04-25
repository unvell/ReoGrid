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
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using unvell.ReoGrid.CellTypes;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.Demo.CellTypeDemo
{
	/// <summary>
	/// 数値プログレスのデモ
	/// </summary>
	public partial class NumericProgressDemo : UserControl
	{
		private Worksheet worksheet;

		public NumericProgressDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;

			var rand = new Random();

			worksheet[1, 2] = "パーセントの数値を変更してみてください。";

			// 数値プログレスセルを初期化
			for (int r = 3; r < 8; r++)
			{
				// セルボディを作成してセルに格納
				worksheet[r, 2] = new NumericProgressCell();

				// 計算式を利用して右側のセルから数値を読み込み
				worksheet[r, 2] = "=" + new CellPosition(r, 3).ToAddress(); // e.g. D3

				// 数値セルの値をランダムで初期化
				worksheet[r, 3] = Math.Round(rand.NextDouble(), 2);
			}

			// 全ての数値セルの書式をパーセントに設定
			worksheet.SetRangeDataFormat(3, 3, 5, 2, DataFormat.CellDataFormatFlag.Percent,
				new DataFormat.NumberDataFormatter.NumberFormatArgs
				{
					DecimalPlaces = 0,
				});

			// フォーカスセルの移動方向を「上から下」に設定
			worksheet.SelectionForwardDirection = SelectionForwardDirection.Down;

			// フォーカスセルの位置を設定
			worksheet.FocusPos = new CellPosition(3, 3);

			// リンクを作成
			worksheet.MergeRange(12, 0, 1, 7);
			worksheet[11, 0] = "カスタマイズしたセル型について詳しくは：";
			worksheet[12, 0] = new HyperlinkCell(
				"https://reogrid.net/jp/document/Custom%20Cell", true);
		}
	}

	/// <summary>
	/// 数値プログレスセル型
	/// </summary>
	internal class NumericProgressCell : CellBody
	{
		public override void OnPaint(CellDrawingContext dc)
		{
			double value = Cell.Worksheet.GetCellData<double>(this.Cell.Position);

			int width = (int)(Math.Round(value * Bounds.Width));

			if (width > 0)
			{
				System.Drawing.Graphics g = dc.Graphics.PlatformGraphics;

				Rectangle rect = new Rectangle(Bounds.Left, Bounds.Top + 1, width, Bounds.Height - 1);

				using (LinearGradientBrush lgb = new LinearGradientBrush(rect, SolidColor.Coral, SolidColor.IndianRed, 90f))
				{
					g.PixelOffsetMode = PixelOffsetMode.Half;
					g.FillRectangle(lgb, rect);
					g.PixelOffsetMode = PixelOffsetMode.Default;
				}
			}
		}
	}
}
