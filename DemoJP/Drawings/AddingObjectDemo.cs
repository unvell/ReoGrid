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

namespace unvell.ReoGrid.Demo.Drawings
{
	public partial class AddingObjectDemo : UserControl, IDemoHelp
	{
		public AddingObjectDemo()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			var worksheet = this.grid.CurrentWorksheet;

			worksheet["A2"] = "ワークシートに描画図形を追加するサンプルです。";

			#region 長方形 1
			// 長方形を作成
			var rect1 = new Drawing.Shapes.RectangleShape()
				{
					Location = new Graphics.Point(60, 90),   // 長方形の位置
					Size = new Graphics.Size(160, 80),       // 長方形のサイズ

					Text = "描画図形を作成",
				};

			// 長方形をワークシートに追加
			worksheet.FloatingObjects.Add(rect1);
			#endregion // 長方形 1

			#region 線 1
			var line1 = new Drawing.Shapes.Line
			{
				StartPoint = new Graphics.Point(220, 130),  // 線の開始位置
				EndPoint = new Graphics.Point(280, 130),    // 線の終了位置
			};

			line1.Style.LineWidth = 1.5f;
			line1.Style.EndCap = Graphics.LineCapStyles.Arrow;

			worksheet.FloatingObjects.Add(line1);
			#endregion // 線 1

			#region Rect 2
			// 長方形を作成
			Drawing.Shapes.RectangleShape rect2 = new Drawing.Shapes.RectangleShape()
			{
				Location = new Graphics.Point(280, 90),
				Size = new Graphics.Size(160, 80),

				Text = "描画図形のテキスト、\nスタイルを設定",
			};

			// 長方形をワークシートに追加
			worksheet.FloatingObjects.Add(rect2);
			#endregion // 長方形 2

			#region 線 2
			var line2 = new Drawing.Shapes.Line
			{
				StartPoint = new Graphics.Point(440, 130),
				EndPoint = new Graphics.Point(500, 130),
			};

			line2.Style.LineWidth = 1.5f;
			line2.Style.EndCap = Graphics.LineCapStyles.Arrow;

			worksheet.FloatingObjects.Add(line2);
			#endregion // 線 2

			#region 長方形 3
			// 長方形を作成
			Drawing.Shapes.RectangleShape rect3 = new Drawing.Shapes.RectangleShape()
			{
				Location = new Graphics.Point(500, 90),
				Size = new Graphics.Size(160, 80),

				Text = "ワークシートに追加",
			};

			// 長方形をワークシートに追加
			worksheet.FloatingObjects.Add(rect3);
			#endregion // 長方形 3
		}

		public string GetHTMLHelp()
		{
			return unvell.ReoGrid.DemoJP.Properties.Resources.AddingObjectDemo_src;
		}
	}
}
