/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * ReoGrid and ReoGrid Demo project is released under MIT license.
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace unvell.ReoGrid.Demo.Features
{
	public partial class ControlAppearanceDemo : UserControl
	{
		public ControlAppearanceDemo()
		{
			InitializeComponent();
		}

		private void rdoGoldenSilver_CheckedChanged(object sender, EventArgs e)
		{
			// create an appearance style set by two key colors
			ControlAppearanceStyle rgcs = new ControlAppearanceStyle(Color.Gray, Color.DarkOrange, false);

			// change colors
			rgcs.SetColor(ControlAppearanceColors.GridBackground, Color.LightGoldenrodYellow);
			rgcs.SetColor(ControlAppearanceColors.GridLine, Color.Silver);

			// apply to control
			reoGridControl.ControlStyle = rgcs;
		}

		private void rdoLightGreen_CheckedChanged(object sender, EventArgs e)
		{
			// create an appearance style set by two key colors
			ControlAppearanceStyle rgcs = new ControlAppearanceStyle(Color.LightGreen, Color.White, false);

			// change colors
			rgcs.SetColor(ControlAppearanceColors.SelectionBorder, Color.Green);
			rgcs.SetColor(ControlAppearanceColors.SelectionFill, Color.FromArgb(30, Color.Green));
			rgcs.SetColor(ControlAppearanceColors.GridLine, Color.FromArgb(200, 255, 200));
			rgcs.SetColor(ControlAppearanceColors.GridBackground, Color.White);

			// apply to control
			reoGridControl.ControlStyle = rgcs;
		}

		private void rdoSnowWhite_CheckedChanged(object sender, EventArgs e)
		{
			// create an appearance style set by two key colors
			ControlAppearanceStyle rgcs = new ControlAppearanceStyle(Color.LightSkyBlue, Color.White, false);

			// change colors
			rgcs.SetColor(ControlAppearanceColors.SelectionBorder, Color.SkyBlue);
			rgcs.SetColor(ControlAppearanceColors.SelectionFill, Color.FromArgb(30, Color.Blue));
			rgcs.SetColor(ControlAppearanceColors.GridLine, Color.FromArgb(220, 220, 255));
			rgcs.SetColor(ControlAppearanceColors.GridBackground, Color.White);

			// apply to control
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
			reoGridControl.ControlStyle = new ControlAppearanceStyle(
				mainColorPickerControl.SolidColor.IsEmpty ? Color.White : mainColorPickerControl.SolidColor,
				highlightColorPickerControl.SolidColor.IsEmpty ? Color.White : highlightColorPickerControl.SolidColor, false);
		}

		private void btnExport_Click(object sender, EventArgs e)
		{
			StringBuilder sb = new StringBuilder("ControlAppearanceStyle rgcs = new ControlAppearanceStyle();");
			sb.AppendLine();

			var styleKeys = Enum.GetNames(typeof(ControlAppearanceColors));
		
			foreach (var name in styleKeys)
			{
				ControlAppearanceColors key = (ControlAppearanceColors)Enum.Parse(typeof(ControlAppearanceColors), name);

				var color = reoGridControl.ControlStyle[key];

				sb.AppendLine(string.Format("rgcs[ControlAppearanceColors.{0}] = Color.FromArgb({1},{2},{3},{4});",
					name, color.A, color.R, color.G, color.B));
			}

			sb.AppendLine();

			Clipboard.SetText(sb.ToString());

			MessageBox.Show("Code exported into Clipboard.");
		}
	}
}
