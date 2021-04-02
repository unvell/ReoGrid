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
using System.Windows.Forms;
using System.Drawing.Drawing2D;

using unvell.ReoGrid.Actions;
using unvell.ReoGrid.Editor.LangRes;

namespace unvell.ReoGrid.PropertyPages
{
	public partial class FillPage : UserControl, IPropertyPage
	{
		public FillPage()
		{
			InitializeComponent();

			patternColorComboBox.SolidColor = Color.Black;
			patternStyleComboBox.CloseOnClick = true;

			labSample.Paint += new PaintEventHandler(labSample_Paint);

			patternColorComboBox.ColorSelected += (s, e) => labSample.Invalidate();
			patternStyleComboBox.PatternSelected += (s, e) => labSample.Invalidate();
			colorPanel.ColorPicked += (s, e) => labSample.Invalidate();
		}

		void labSample_Paint(object sender, PaintEventArgs e)
		{
			if (!patternStyleComboBox.HasPatternStyle)
			{
				using (Brush b = new SolidBrush(colorPanel.SolidColor))
				{
					e.Graphics.FillRectangle(b, labSample.ClientRectangle);
				}
			}
			else
			{
				using (HatchBrush hb = new HatchBrush(patternStyleComboBox.PatternStyle,
					patternColorComboBox.SolidColor, colorPanel.SolidColor))
				{
					e.Graphics.FillRectangle(hb, labSample.ClientRectangle);
				}
			}
		}

		private ReoGridControl grid;

		public ReoGridControl Grid
		{
			get { return grid; }
			set { this.grid = value; }
		}

		public void SetupUILanguage()
		{
			labBackgroundColor.Text = LangResource.FillPage_Background_Color;
			grpSample.Text = LangResource.Sample;
			grpPattern.Text = LangResource.FillPage_Fill_Pattern;
			labPatternColor.Text = LangResource.FillPage_Fill_Pattern_Color;
			labPatternStyle.Text = LangResource.FillPage_Fill_Pattern_Style;
		}

		private bool backuphasPatternStyle = false;
		private Color backupBackColor = Color.Empty;
		private Color backupPatternColor = Color.Empty;
		private HatchStyle backupPatternStyle = HatchStyle.Min;

		public void LoadPage()
		{
			var sheet = this.grid.CurrentWorksheet;

			WorksheetRangeStyle style = sheet.GetRangeStyles(sheet.SelectionRange);
			colorPanel.SolidColor = backupBackColor = style.BackColor;

			if (unvell.ReoGrid.Utility.StyleUtility.HasStyle(style, PlainStyleFlag.FillPattern))
			{
				patternColorComboBox.SolidColor = style.FillPatternColor;
				patternStyleComboBox.PatternStyle = (HatchStyle)style.FillPatternStyle;
				patternStyleComboBox.HasPatternStyle = true;
			}

			backupPatternColor = patternColorComboBox.SolidColor;
			backupPatternStyle = patternStyleComboBox.PatternStyle;
			backuphasPatternStyle = patternStyleComboBox.HasPatternStyle;
		}

		public WorksheetReusableAction CreateUpdateAction()
		{
			if (backupBackColor != colorPanel.SolidColor
				|| backuphasPatternStyle != patternStyleComboBox.HasPatternStyle
				|| backupPatternColor != patternColorComboBox.SolidColor
				|| backupPatternStyle != patternStyleComboBox.PatternStyle)
			{
				WorksheetRangeStyle style = new WorksheetRangeStyle();

				style.Flag |= PlainStyleFlag.BackColor;
				style.BackColor = colorPanel.SolidColor;

				style.Flag |= PlainStyleFlag.FillPattern;
				style.FillPatternColor = patternStyleComboBox.HasPatternStyle ? patternColorComboBox.SolidColor : Color.Empty;
				style.FillPatternStyle = (unvell.ReoGrid.Graphics.HatchStyles)patternStyleComboBox.PatternStyle;

				// pattern style need a back color
				// when pattern style setted but back color is not setted, set the backcolor to white
				if (patternStyleComboBox.HasPatternStyle)
				{
					style.BackColor = Color.White;
					style.Flag |= PlainStyleFlag.BackColor;
				}

				return new SetRangeStyleAction(grid.CurrentWorksheet.SelectionRange, style);
			}
			else
				return null;
		}

#pragma warning disable 67 // variable is never used
		/// <summary>
		/// Setting dialog will be closed when this event rasied
		/// </summary>
		public event EventHandler Done;
#pragma warning restore 67 // variable is never used
	}
}
