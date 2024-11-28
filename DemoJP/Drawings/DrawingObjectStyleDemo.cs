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
	/// <summary>
	/// 描画オブジェクトのスタイル設定のデモ
	/// </summary>
	public partial class DrawingObjectStyleDemo : UserControl, IDemoHelp
	{
		public DrawingObjectStyleDemo()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs _unused)
		{
			base.OnLoad(_unused);

			var worksheet = this.grid.CurrentWorksheet;

			worksheet["A2"] = "描画オブジェクトのスタイルを簡単に設定することができます。";

			#region 描画オブジェクトを作成

			// 長方形の描画オブジェクトを作成
			var rect = new Drawing.Shapes.RectangleShape()
				{
					// 位置
					Location = new Graphics.Point(120, 90),
					// サイズ
					Size = new Graphics.Size(160, 100),

					// テキスト
					Text = "描画オブジェクト",
				};

			// 塗りつぶし色を設定
			rect.Style.FillColor = Graphics.SolidColor.LightGoldenrodYellow;

			// 枠線の幅と色を設定
			rect.Style.LineWidth = 3;
			rect.Style.LineColor = Graphics.SolidColor.LimeGreen;

			// 描画オブジェクトの初期角度を設定
			rect.RotateAngle = 45;

			// 描画オブジェクトをワークシートに追加
			worksheet.FloatingObjects.Add(rect);

			#endregion // 描画オブジェクトを作成

			#region 角度調整用スライドを作成

			worksheet["B13"] = "スライドを移動するとオブジェクトが回転します。";

			worksheet.Ranges["B15:D15"].Merge();
			worksheet["B15"] = new Demo.CellTypeDemo.SlideCell();
			worksheet["B15"] = 45f / 350f;

			// カスタマイズ関数 formatToDegree を作成。小数を角度の書式で表示
			ReoGrid.Formula.FormulaExtension.CustomFunctions["formatToDegree"] = (cell, args) =>
			{
				unvell.ReoGrid.Utility.CellUtility.TryGetNumberData(args[0], out var data);
				return Math.Round(data) + "\u00b0";
			};

			worksheet["E15"] = "=formatToDegree(B15*360)";
			worksheet.Cells["E15"].Style.HAlign = ReoGridHorAlign.Right;

			// スライドの値が変更された際のイベントを処理
			worksheet.CellDataChanged += (s, e) =>
			{
				if (e.Cell.Position.ToAddress() == "B15")
				{
					// 描画オブジェクトの角度を変更
					rect.RotateAngle = (float)(e.Cell.GetData<double>() * 360f);
				}
			};

			#endregion 角度調整用スライドを作成
		}

		public string GetHTMLHelp()
		{
			var resMan = new System.Resources.ResourceManager(this.GetType());
			return (string)resMan.GetObject("HTMLHelp");
		}

	}

}
