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
using System.Windows.Forms;

using unvell.ReoGrid.Events;

namespace unvell.ReoGrid.Demo.CellDemo
{
	/// <summary>
	/// セルイベント処理のデモ
	/// </summary>
	public partial class CellsEventDemo : UserControl
	{
		private Worksheet worksheet;

		public CellsEventDemo()
		{
			InitializeComponent();

			worksheet = grid.CurrentWorksheet;

			chkEnter.CheckedChanged += UpdateEventBind;
			chkLeave.CheckedChanged += UpdateEventBind;
			chkDown.CheckedChanged += UpdateEventBind;
			chkUp.CheckedChanged += UpdateEventBind;
			chkMove.CheckedChanged += UpdateEventBind;
			chkHoverHighlight.CheckedChanged += UpdateEventBind;
			chkBackground.CheckedChanged += UpdateEventBind;

			chkGridLines.CheckedChanged += (s, e) => worksheet.SetSettings(
				WorksheetSettings.View_ShowGridLine, chkGridLines.Checked);

			chkSelectionRect.CheckedChanged += (s, e) => worksheet.SelectionMode =
				chkSelectionRect.Checked ? WorksheetSelectionMode.Range : WorksheetSelectionMode.None;
		}

		private void UpdateEventBind(object sender, EventArgs e)
		{
			worksheet.CellMouseEnter -= Grid_CellMouseEnter;
			worksheet.CellMouseLeave -= Grid_CellMouseLeave;
			worksheet.CellMouseDown -= Grid_CellMouseDown;
			worksheet.CellMouseUp -= Grid_CellMouseUp;
			worksheet.CellMouseMove -= Grid_CellMouseMove;

			if (chkEnter.Checked || chkHoverHighlight.Checked || chkBackground.Checked) worksheet.CellMouseEnter += Grid_CellMouseEnter;
			if (chkLeave.Checked || chkHoverHighlight.Checked || chkBackground.Checked) worksheet.CellMouseLeave += Grid_CellMouseLeave;
			if (chkDown.Checked) worksheet.CellMouseDown += Grid_CellMouseDown;
			if (chkUp.Checked) worksheet.CellMouseUp += Grid_CellMouseUp;
			if (chkMove.Checked) worksheet.CellMouseMove += Grid_CellMouseMove;
		}

		private void Grid_CellMouseEnter(object sender, CellMouseEventArgs e)
		{
			if(chkEnter.Checked) Log("CellMouseEnter: " + e.CellPosition);

			if (chkHoverHighlight.Checked)
			{
				worksheet.SetRangeBorders(new RangePosition(e.CellPosition, e.CellPosition), BorderPositions.Outside, RangeBorderStyle.GraySolid);
			}

			if (chkBackground.Checked)
			{
				worksheet.SetRangeStyles(new RangePosition(e.CellPosition, e.CellPosition), new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.BackColor,
					BackColor = Color.Silver,
				});
			}
		}

		private void Grid_CellMouseLeave(object sender, CellMouseEventArgs e)
		{
			if (chkLeave.Checked) Log("CellMouseLeave: " + e.CellPosition);

			if (chkHoverHighlight.Checked)
			{
				worksheet.RemoveRangeBorders(new RangePosition(e.CellPosition, e.CellPosition), BorderPositions.Outside);
			}

			if (chkBackground.Checked)
			{
				worksheet.RemoveRangeStyles(new RangePosition(e.CellPosition, e.CellPosition), PlainStyleFlag.BackColor);
			}
		}

		private void Grid_CellMouseDown(object sender, CellMouseEventArgs e)
		{
			Log("CellMouseDown: " + e.CellPosition + ", " + e.RelativePosition);
		}
		private void Grid_CellMouseUp(object sender, CellMouseEventArgs e)
		{
			Log("CellMouseUp: " + e.CellPosition + ", " + e.RelativePosition);
		}
		private void Grid_CellMouseMove(object sender, CellMouseEventArgs e)
		{
			Log("CellMouseMove: " + e.CellPosition + ", " + e.RelativePosition);
		}

		private void Log(string msg)
		{
			listbox1.Items.Add(msg);
			listbox1.SelectedIndex = listbox1.Items.Count - 1;
		}
	}
}
