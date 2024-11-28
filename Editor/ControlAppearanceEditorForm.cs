/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net/
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * ReoGrid and ReoGridEditor is released under MIT license.
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using unvell.ReoGrid.Editor.LangRes;

namespace unvell.ReoGrid.Editor
{
	internal partial class ControlAppearanceEditorForm : Form
	{
		public ControlAppearanceEditorForm()
		{
			InitializeComponent();

			lstColors.Items.AddRange(Enum.GetNames(typeof(
				ControlAppearanceColors)).ToList().ConvertAll<object>(s => s).ToArray());

			lstColors.SelectedIndex = 0;

			colorPickerControl.ColorSelected += colorPickerControl_ColorPicked;
			lstColors.SelectedIndexChanged += lstColors_SelectedIndexChanged;
			numSelectionWidth.ValueChanged += NumSelectionWidth_ValueChanged;

			SetupUILanguage();
		}

		void SetupUILanguage()
		{
			this.Text = LangResource.AppearanceEditor_Caption;

			this.labMain.Text = LangResource.AppearanceEditor_Main;
			this.labHighligh.Text = LangResource.AppearanceEditor_Highlight;
			this.chkUseSystemHighlight.Text = LangResource.AppearanceEditor_Use_System_Highligh_Color;
			this.labSelectionBorderWidth.Text = LangResource.AppearanceEditor_Selection_Border_Width;
			this.labPixel.Text = LangResource.Pixel;
			this.btnReset.Text = LangResource.AppearanceEditor_Reset_To_Default;
			this.btnExportCSharp.Text = LangResource.AppearanceEditor_Export_CSharp;
			this.btnExportVBNET.Text = LangResource.AppearanceEditor_Export_VBNET;
		}

		private ReoGridControl grid;

		public ReoGridControl Grid
		{
			get { return grid; }
			set { grid = value;

				LoadStylesFromControl();
			}
		}

		void LoadStylesFromControl()
		{
			var controlStyle = grid.ControlStyle;

			mainColorPickerControl.SolidColor = controlStyle[ControlAppearanceColors.LeadHeadNormal];
			highlightColorPickerControl.SolidColor = controlStyle[ControlAppearanceColors.SelectionBorder];

			numSelectionWidth.Value = (int)controlStyle.SelectionBorderWidth;
		}

		#region Set Styles
		private void lstColors_SelectedIndexChanged(object sender, EventArgs e)
		{
			ControlAppearanceColors key = (ControlAppearanceColors)Enum.Parse(
				typeof(ControlAppearanceColors), lstColors.SelectedItem.ToString());

			colorPickerControl.SolidColor = grid.ControlStyle[key];
		}

		private void colorPickerControl_ColorPicked(object sender, EventArgs e)
		{
			ControlAppearanceColors key = (ControlAppearanceColors)Enum.Parse(
			typeof(ControlAppearanceColors), lstColors.SelectedItem.ToString());

			grid.ControlStyle[key] = colorPickerControl.SolidColor;
		}

		private void NumSelectionWidth_ValueChanged(object sender, EventArgs e)
		{
			grid.ControlStyle.SelectionBorderWidth = (float)numSelectionWidth.Value;
		}
		#endregion // Set Styles

		#region Export
		private void btnExportCSharp_Click(object sender, EventArgs e)
		{
			StringBuilder sb = new StringBuilder("var rgcs = new ControlAppearanceStyle();");
			sb.AppendLine();

			foreach (var name in Enum.GetNames(typeof(ControlAppearanceColors)))
			{
				ControlAppearanceColors key = (ControlAppearanceColors)Enum.Parse(typeof(ControlAppearanceColors), name);

				Color c = grid.ControlStyle[key];

				sb.AppendLine(string.Format("rgcs[ControlAppearanceColors.{0}] = Color.FromArgb({1},{2},{3},{4});", 
					name, c.A, c.R, c.G, c.B));
			}

			sb.AppendLine("rgcs.SelectionBorderWidth = " + numSelectionWidth.Value + ";");

			sb.AppendLine();

			Clipboard.SetText(sb.ToString());

			MessageBox.Show("C# "+ LangRes.LangResource.Msg_Source_Code_Exported_Into_Clipboard);
		}

		private void btnExportVBNET_Click(object sender, EventArgs e)
		{
			StringBuilder sb = new StringBuilder("Dim rgcs as new ControlAppearanceStyle()");
			sb.AppendLine();

			foreach (var name in Enum.GetNames(typeof(ControlAppearanceColors)))
			{
				ControlAppearanceColors key = (ControlAppearanceColors)Enum.Parse(typeof(ControlAppearanceColors), name);

				Color c = grid.ControlStyle[key];

				sb.AppendLine(string.Format("rgcs(ControlAppearanceColors.{0}) = Color.FromArgb({1},{2},{3},{4})",
					name, c.A, c.R, c.G, c.B));
			}

			sb.AppendLine("rgcs.SelectionBorderWidth = " + numSelectionWidth.Value);

			sb.AppendLine();

			Clipboard.SetText(sb.ToString());

			MessageBox.Show("VB.NET " + LangRes.LangResource.Msg_Source_Code_Exported_Into_Clipboard);
		}
		#endregion // Export

		#region Create From Theme Colors

		private void mainColorPickerControl_ColorPicked(object sender, EventArgs e)
		{
			CreateFromTheme();
		}

		private void highlightColorPickerControl_ColorPicked(object sender, EventArgs e)
		{
			CreateFromTheme();
		}

		private void chkUseSystemHighlight_CheckedChanged(object sender, EventArgs e)
		{
			CreateFromTheme();
		}

		private void CreateFromTheme()
		{
			grid.ControlStyle = new ControlAppearanceStyle(
				mainColorPickerControl.SolidColor.IsEmpty ? Color.White : mainColorPickerControl.SolidColor,
				highlightColorPickerControl.SolidColor.IsEmpty ? Color.White : highlightColorPickerControl.SolidColor, 
				chkUseSystemHighlight.Checked);
		}

		#endregion // Create From Theme Colors

		private void btnReset_Click(object sender, EventArgs e)
		{
			grid.ControlStyle = ControlAppearanceStyle.CreateDefaultControlStyle();
		}

	}
}
