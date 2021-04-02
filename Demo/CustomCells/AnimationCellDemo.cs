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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using unvell.ReoGrid.CellTypes;
using unvell.ReoGrid.Demo.Properties;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.Demo.CustomCells
{
	public partial class AnimationCellDemo : UserControl
	{
		// timer to update animation frames intervally
		Timer timer = new Timer();
		
		private LoadingCell loadingCell;

		private GifImageCell gifCell;

		private Worksheet worksheet;

		public AnimationCellDemo()
		{
			InitializeComponent();

			// prepare timer to udpate sheet
			timer.Interval = 10;
			timer.Tick += timer_Tick;

			// get default worksheet
			worksheet = grid.CurrentWorksheet;

			// change cells size
			worksheet.SetRowsHeight(5, 1, 100);
			worksheet.SetColumnsWidth(1, 5, 100);

			worksheet.SetRangeStyles(3, 1, 5, 5, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.HorizontalAlign | PlainStyleFlag.VerticalAlign,
				HAlign = ReoGridHorAlign.Center,
				VAlign = ReoGridVerAlign.Middle,
			});

			// waiting cell
			loadingCell = new LoadingCell();
			worksheet[3, 1] = loadingCell;

			// gif image cell
			gifCell = new GifImageCell(Resources.loading);
			worksheet[5, 2] = gifCell;

			// gif image cell
			worksheet[5, 4] = new BlinkCell();
			worksheet[5, 4] = "Blink Cell";

			// note text
			worksheet[7, 1] = "NOTE: Too many updates for animation might affect the rendering performance.";
			worksheet.SetRangeStyles(7, 1, 1, 1, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.TextColor | PlainStyleFlag.BackColor,
				BackColor = SolidColor.Orange,
				TextColor = SolidColor.White,
			});
			worksheet.MergeRange(7, 1, 1, 6);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			timer.Start();
		}

		void timer_Tick(object sender, EventArgs e)
		{
			// update frame
			loadingCell.NextFrame();

			// sample: retrieve body from cell
			var cell = worksheet.Cells[5, 4];
			if (cell.Body is BlinkCell)
			{
				((BlinkCell)cell.Body).NextFrame();
			}

			// repaint control
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
	/// Swing background custom cell
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
			dc.Graphics.FillRectangle(new Rectangle(offset, 0, ThumbSize, Bounds.Height), SolidColor.SkyBlue);

			// call core text draw
			dc.DrawCellText();
		}
	}

	/// <summary>
	/// Gif Image Custom Cell
	/// </summary>
	class GifImageCell : CellBody
	{
		public System.Drawing.Image Gif { get; set; }

		public GifImageCell(System.Drawing.Image gif)
		{
			this.Gif = gif;
		}

		public override void OnSetup(Cell cell)
		{
			System.Drawing.ImageAnimator.Animate(Gif, OnFrameChanged);
		}

		private void OnFrameChanged(object o, EventArgs e)
		{
			lock (this.Gif) System.Drawing.ImageAnimator.UpdateFrames(Gif);
		}

		public override void OnPaint(CellDrawingContext dc)
		{
			lock (this.Gif) dc.Graphics.DrawImage(Gif, Bounds);

			// call core text draw
			dc.DrawCellText();
		}
	}

	/// <summary>
	/// Blink background custom cell
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

			// call core text draw
			dc.DrawCellText();
		}
	}

}
