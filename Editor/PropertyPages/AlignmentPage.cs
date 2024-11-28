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
using System.Windows.Forms;

using unvell.ReoGrid.PropertyPages;
using unvell.ReoGrid.Actions;
using unvell.ReoGrid.Editor.LangRes;

namespace unvell.ReoGrid.Editor.PropertyPages
{
	public partial class AlignmentPage : UserControl, IPropertyPage
	{
		public AlignmentPage()
		{
			InitializeComponent();

			cmbHorAlign.Items.AddRange(new string[] 
			{
				//"General", "Left", "Center", "Right", "Distributed (Indent)"
				LangResource.Horizontal_Alignment_General,
				LangResource.Horizontal_Alignment_Left,
				LangResource.Horizontal_Alignment_Right,
				LangResource.Horizontal_Alignment_Distributed,
			});

			cmbVerAlign.Items.AddRange(new string[]
			{
				//"General", "Top", "Middle", "Bottom"
				LangResource.Vertical_Alignment_General,
				LangResource.Vertical_Alignment_Top,
				LangResource.Vertical_Alignment_Middle,
				LangResource.Vertical_Alignment_Bottom,
			});

			textRotateControl.AngleChanged += (s, e) =>
				{
					if (numRotationAngle.Value != textRotateControl.Angle)
					{
						numRotationAngle.Value = textRotateControl.Angle;
					}
				};

			numRotationAngle.ValueChanged += (s, e) =>
				{
					if (textRotateControl.Angle != numRotationAngle.Value)
					{
						textRotateControl.Angle = (int)numRotationAngle.Value;
					}
				};
		}

		#region IPropertyPage Members

		private ReoGridControl grid;

		public ReoGridControl Grid
		{
			get { return grid; }
			set { this.grid = value; }
		}

		public void SetupUILanguage()
		{
			this.formLineTextAlignment.Text = LangResource.AlignmentPage_Text_Alignment;
			this.labelHorizontal.Text = LangResource.AlignmentPage_Horizontal;
			this.labelVertical.Text = LangResource.AlignmentPage_Vertical;
			this.labelIndent.Text = LangResource.AlignmentPage_Indent;

			this.formLineTextControl.Text = LangResource.AlignmentPage_Text_Control;
			this.chkWrapText.Text = LangResource.AlignmentPage_Wrap_Text;

			this.grpOrientation.Text = LangResource.AlignmentPage_Orientation;
			this.labelDegrees.Text = LangResource.AlignmentPage_Degrees;
		}

		private CheckState backupTextWrapState;
		private ReoGridHorAlign backupHorAlign;
		private ReoGridVerAlign backupVerAlign;
		private ushort backupIndent = 0;
		private float backupRotateAngle = 0;

		public void LoadPage()
		{
			var sheet = grid.CurrentWorksheet;

			WorksheetRangeStyle style = sheet.GetRangeStyles(sheet.SelectionRange);

			if (style.TextWrapMode == TextWrapMode.WordBreak
				|| style.TextWrapMode == TextWrapMode.BreakAll)
			{
				backupTextWrapState = CheckState.Checked;
			}
			else
			{
				backupTextWrapState = CheckState.Unchecked;
			}

			backupHorAlign = style.HAlign;
			backupVerAlign = style.VAlign;

			switch (style.HAlign)
			{
				case ReoGridHorAlign.General:
					cmbHorAlign.SelectedIndex = 0; break;
				case ReoGridHorAlign.Left:
					cmbHorAlign.SelectedIndex = 1; break;
				case ReoGridHorAlign.Center:
					cmbHorAlign.SelectedIndex = 2; break;
				case ReoGridHorAlign.Right:
					cmbHorAlign.SelectedIndex = 3; break;
				case ReoGridHorAlign.DistributedIndent:
					cmbHorAlign.SelectedIndex = 4; break;
			}

			switch (style.VAlign)
			{
				case ReoGridVerAlign.General:
					cmbVerAlign.SelectedIndex = 0; break;
				case ReoGridVerAlign.Top:
					cmbVerAlign.SelectedIndex = 1; break;
				case ReoGridVerAlign.Middle:
					cmbVerAlign.SelectedIndex = 2; break;
				case ReoGridVerAlign.Bottom:
					cmbVerAlign.SelectedIndex = 3; break;
			}

			chkWrapText.CheckState = backupTextWrapState;

			backupIndent = style.Indent;
			numIndent.Value = backupIndent;

			// cell text rotate

			var angle = style.RotationAngle;

			if (angle < -90) angle = -90;
			if (angle > 90) angle = 90;

			backupRotateAngle = angle;
			textRotateControl.Angle = (int)angle;
			numRotationAngle.Value = (int)angle;
		}

		public WorksheetReusableAction CreateUpdateAction()
		{
			var style = new WorksheetRangeStyle();

			ReoGridHorAlign halign = ReoGridHorAlign.General;
			ReoGridVerAlign valign = ReoGridVerAlign.Middle;

			switch (cmbHorAlign.SelectedIndex)
			{
				default:
				case 0: halign = ReoGridHorAlign.General; break;
				case 1: halign = ReoGridHorAlign.Left; break;
				case 2: halign = ReoGridHorAlign.Center; break;
				case 3: halign = ReoGridHorAlign.Right; break;
				case 4: halign = ReoGridHorAlign.DistributedIndent; break;
			}

			switch (cmbVerAlign.SelectedIndex)
			{
				default: 
				case 0: valign = ReoGridVerAlign.General; break;
				case 1: valign = ReoGridVerAlign.Top; break;
				case 2: valign = ReoGridVerAlign.Middle; break;
				case 3: valign = ReoGridVerAlign.Bottom; break;
			}

			if (backupHorAlign != halign)
			{
				style.Flag |= PlainStyleFlag.HorizontalAlign;
				style.HAlign = halign;
			}

			if (backupVerAlign != valign)
			{
				style.Flag |= PlainStyleFlag.VerticalAlign;
				style.VAlign = valign;
			}

			if (backupTextWrapState != chkWrapText.CheckState)
			{
				style.Flag |= PlainStyleFlag.TextWrap;

				if (chkWrapText.Checked)
				{
					style.TextWrapMode = TextWrapMode.WordBreak;
				}
				else
				{
					style.TextWrapMode = TextWrapMode.NoWrap;
				}
			}

			if (backupIndent != numIndent.Value)
			{
				style.Flag |= PlainStyleFlag.Indent;
				style.Indent = (ushort)numIndent.Value;
			}

			if (backupRotateAngle != textRotateControl.Angle)
			{
				style.RotationAngle = textRotateControl.Angle;
				style.Flag |= PlainStyleFlag.RotationAngle;
			}

			return style.Flag == PlainStyleFlag.None ? null : new SetRangeStyleAction(grid.CurrentWorksheet.SelectionRange, style);
		}

#pragma warning disable 67 // variable is never used
		/// <summary>
		/// Setting dialog will be closed when this event rasied
		/// </summary>
		public event EventHandler Done;
#pragma warning restore 67 // variable is never used

		#endregion // IPropertyPage Members
	}
}
