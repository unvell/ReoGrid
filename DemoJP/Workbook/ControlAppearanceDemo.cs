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
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace unvell.ReoGrid.Demo.WorkbookDemo
{
	/// <summary>
	/// コンポーネント外観スタイルのデモ
	/// </summary>
	public partial class ControlAppearanceDemo : UserControl
	{
		public ControlAppearanceDemo()
		{
			InitializeComponent();
		}

		private void rdoGoldenSilver_CheckedChanged(object sender, EventArgs e)
		{
			// 外観スタイルオブジェクトを作成
			ControlAppearanceStyle rgcs = new ControlAppearanceStyle(Color.DarkOrange, Color.Gray, false);

			// 色を設定
			rgcs.SetColor(ControlAppearanceColors.GridBackground, Color.LightGoldenrodYellow);
			rgcs.SetColor(ControlAppearanceColors.GridLine, Color.Silver);

			// 外観スタイルオブジェクトを適用
			reoGridControl.ControlStyle = rgcs;
		}

		private void rdoLightGreen_CheckedChanged(object sender, EventArgs e)
		{
			// 外観スタイルオブジェクトを作成
			ControlAppearanceStyle rgcs = new ControlAppearanceStyle(Color.LightGreen, Color.White, false);

			// 色を設定
			rgcs.SetColor(ControlAppearanceColors.SelectionBorder, Color.Green);
			rgcs.SetColor(ControlAppearanceColors.SelectionFill, Color.FromArgb(30, Color.Green));
			rgcs.SetColor(ControlAppearanceColors.GridLine, Color.FromArgb(200, 255, 200));
			rgcs.SetColor(ControlAppearanceColors.GridBackground, Color.White);

			// 外観スタイルオブジェクトを適用
			reoGridControl.ControlStyle = rgcs;
		}

		private void rdoSnowWhite_CheckedChanged(object sender, EventArgs e)
		{
			// 外観スタイルオブジェクトを作成
			ControlAppearanceStyle rgcs = new ControlAppearanceStyle(Color.LightSkyBlue, Color.White, false);

			// 色を設定
			rgcs.SetColor(ControlAppearanceColors.SelectionBorder, Color.SkyBlue);
			rgcs.SetColor(ControlAppearanceColors.SelectionFill, Color.FromArgb(30, Color.Blue));
			rgcs.SetColor(ControlAppearanceColors.GridLine, Color.FromArgb(220, 220, 255));
			rgcs.SetColor(ControlAppearanceColors.GridBackground, Color.White);

			// 外観スタイルオブジェクトを適用
			reoGridControl.ControlStyle = rgcs;
		}

		private void mainColorPickerControl_ColorPicked(object sender, EventArgs e)
		{
			CreateFromTheme();
		}

		private void highlightColorPickerControl_ColorPicked(object sender, EventArgs e)
		{
			CreateFromTheme();
		}

		private void CreateFromTheme()
		{
			// テーマ色と強調色から外観スタイルオブジェクトを作成
			reoGridControl.ControlStyle = new ControlAppearanceStyle(
				mainColorPickerControl.SolidColor.IsEmpty ? Color.White : mainColorPickerControl.SolidColor,
				highlightColorPickerControl.SolidColor.IsEmpty ? Color.White : highlightColorPickerControl.SolidColor, false);
		}

		/// <summary>
		/// C#コードをエクスポート
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnExportCSharp_Click(object sender, EventArgs e)
		{
			StringBuilder sb = new StringBuilder("var rgcs = new ControlAppearanceStyle();");
			sb.AppendLine();

			var style = reoGridControl.ControlStyle;

			foreach (var name in Enum.GetNames(typeof(ControlAppearanceColors)))
			{
				ControlAppearanceColors key = (ControlAppearanceColors)Enum.Parse(typeof(ControlAppearanceColors), name);

				Color c = style[key];

				sb.AppendLine(string.Format("rgcs[ControlAppearanceColors.{0}] = Color.FromArgb({1},{2},{3},{4});",
					name, c.A, c.R, c.G, c.B));
			}

			sb.AppendLine("rgcs.SelectionBorderWidth = " + style.SelectionBorderWidth + ";");

			sb.AppendLine();

			Clipboard.SetText(sb.ToString());

			MessageBox.Show("C#コードをクリップボードにエクスポートしました。");
		}

		/// <summary>
		/// VB.NETコードをエクスポート
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnExportVBNET_Click(object sender, EventArgs e)
		{
			StringBuilder sb = new StringBuilder("Dim rgcs as new ControlAppearanceStyle()");
			sb.AppendLine();

			var style = reoGridControl.ControlStyle;

			foreach (var name in Enum.GetNames(typeof(ControlAppearanceColors)))
			{
				ControlAppearanceColors key = (ControlAppearanceColors)Enum.Parse(typeof(ControlAppearanceColors), name);

				Color c = style[key];

				sb.AppendLine(string.Format("rgcs(ControlAppearanceColors.{0}) = Color.FromArgb({1},{2},{3},{4})",
					name, c.A, c.R, c.G, c.B));
			}

			sb.AppendLine("rgcs.SelectionBorderWidth = " + style.SelectionBorderWidth);

			sb.AppendLine();

			Clipboard.SetText(sb.ToString());

			MessageBox.Show("VB.NETコードをクリップボードにエクスポートしました。");
		}
	}
}
