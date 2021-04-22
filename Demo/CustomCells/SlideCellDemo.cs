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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using unvell.ReoGrid.CellTypes;
using unvell.ReoGrid.Events;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.Demo.CustomCells
{
	public partial class SlideCellDemo : UserControl
	{
		private Worksheet worksheet;

		public SlideCellDemo()
		{
			InitializeComponent();

			worksheet = grid.CurrentWorksheet;

			chkShowGridLines.CheckedChanged += (s, e) =>
				worksheet.SetSettings(WorksheetSettings.View_ShowGridLine, chkShowGridLines.Checked);

			chkDisableSelection.CheckedChanged += (s, e) =>
				worksheet.SelectionMode = chkDisableSelection.Checked ?
				WorksheetSelectionMode.None : WorksheetSelectionMode.Range;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			worksheet.ColumnHeaders[2].Width = 100;
			worksheet.ColumnHeaders[4].Width = 120;

			worksheet.SetRangeDataFormat(4, 3, 1, 1, DataFormat.CellDataFormatFlag.Percent,
				DataFormat.NumberDataFormatter.NoDecimalDigitsArgument);
			worksheet.SetRangeDataFormat(7, 3, 1, 1, DataFormat.CellDataFormatFlag.Percent,
				DataFormat.NumberDataFormatter.NoDecimalDigitsArgument);

			worksheet[4, 4] = new SlideCell();
			worksheet[4, 4] = .5d;
			worksheet[4, 3] = "=E5";

			worksheet[7, 4] = new SlideCell();
			worksheet[7, 4] = .5d;
			worksheet[7, 2] = new NumericProgressCell();
			worksheet[7, 2] = "=E8";
			worksheet[7, 3] = "=E8";

			worksheet[6, 2] = "bind by '=E8'";
			worksheet[2, 3] = "Try slide the green thumb below...";

			// link
			worksheet.MergeRange(12, 0, 1, 7);
			worksheet[11, 0] = "More info about Custom Cell:";
			worksheet[12, 0] = new unvell.ReoGrid.CellTypes.HyperlinkCell(
				"https://reogrid.net/document/Custom%20Cell", true);
		}
	}

	public class SlideCell : CellBody
	{
		// hold the instance of grid control
		public Worksheet Worksheet { get; set; }

		public override void OnSetup(Cell cell)
		{
			this.Worksheet = cell.Worksheet;
		}

		public bool IsHover { get; set; }

		public override void OnPaint(CellDrawingContext dc)
		{
			// try getting the cell value
			float.TryParse(dc.Cell.DisplayText, out float value);

			// retrieve graphics object
			var g = dc.Graphics;

			int halfHeight = (int)Math.Round(Bounds.Height / 2f);
			int sliderHeight = (int)Math.Min(Bounds.Height - 4, 20);

			// draw slide bar
			g.FillRectangle(4, halfHeight - 3, Bounds.Width - 8, 6, SolidColor.Gainsboro);

			int x = 2 + (int)Math.Round(value * (Bounds.Width - 12));

			// thumb rectangle
			Rectangle rect = new Rectangle(x, halfHeight - sliderHeight / 2, 8, sliderHeight);

			// draw slide thumb
			g.FillRectangle(rect, IsHover ? SolidColor.LimeGreen : SolidColor.LightGreen);
		}

		public override bool OnMouseDown(CellMouseEventArgs e)
		{
			UpdateValueByCursorX(e.CellPosition, e.RelativePosition.X);

			// return true to notify control that the mouse-down operation has been hanlded.
			// all operations after this will be aborted.
			return true;
		}

		public override bool OnMouseMove(CellMouseEventArgs e)
		{
			// requires the left button
			if (e.Buttons == unvell.ReoGrid.Interaction.MouseButtons.Left)
			{
				UpdateValueByCursorX(e.CellPosition, e.RelativePosition.X);
			}

			return false;
		}

		private void UpdateValueByCursorX(CellPosition cellPos, float x)
		{
			// calcutate value by cursor position
			float value = x / (Bounds.Width - 2f);

			if (value < 0) value = 0;
			if (value > 1) value = 1;

			Worksheet.SetCellData(cellPos, value);
		}

		public override bool OnMouseEnter(CellMouseEventArgs e)
		{
			IsHover = true;
			return true;
		}

		public override bool OnMouseLeave(CellMouseEventArgs e)
		{
			IsHover = false;
			return true;		
		}

		public override bool OnStartEdit()
		{
			// disable editing on this cell
			return false;
		}
	}

}
