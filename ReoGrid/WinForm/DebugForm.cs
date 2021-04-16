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
 * Author: Jing Lu <jingwood at unvell.com>
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

#if WINFORM
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using unvell.Common;
using unvell.ReoGrid.Core;
using unvell.ReoGrid.DataFormat;

namespace unvell.ReoGrid.WinForm
{
	public partial class DebugForm : Form
	{
		public DebugForm()
		{
			InitializeComponent();
		}

		private Worksheet grid;

		public Worksheet Grid
		{
			get { return grid; }
			set
			{
				if (this.grid != null)
				{
					this.grid.SelectionRangeChanged -= grid_SelectionRangeChanged;
				}

				this.grid = value;

				if (this.grid != null)
				{
					this.grid.SelectionRangeChanged += grid_SelectionRangeChanged;
				}
			}
		}

		private InitTab initTab;

		void grid_SelectionRangeChanged(object sender, Events.RangeEventArgs e)
		{
			UpdateDebugInfo();
		}

		public InitTab InitTabType
		{
			get { return initTab; }
			set { initTab = value; }
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			switch (initTab)
			{
				default:
				case InitTab.Grid:
					tabControl1.SelectedIndex = 0;
					break;

				case InitTab.Border:
					tabControl1.SelectedIndex = 1;
					break;

				case InitTab.Format:
					tabControl1.SelectedIndex = 2;
					break;
			}

		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			
			UpdateDebugInfo();

			if (Visible)
			{
				txtLog.Text = string.Empty;

				foreach (string line in RGDebugLogWritter.Instance.Lines)
				{
					txtLog.Text += line + "\r\n";
					txtLog.SelectionStart = txtLog.Text.Length;
				}

				RGDebugLogWritter.Instance.LogAppended += (Instance_LogAppended);
			}
			else
			{
				RGDebugLogWritter.Instance.LogAppended -= (Instance_LogAppended);
			}
		}

		void Instance_LogAppended(object sender, DebugLogEventArgs e)
		{
			txtLog.Text += e.Text + "\r\n";
			txtLog.SelectionStart = txtLog.Text.Length;
		}

		private void UpdateDebugInfo()
		{
			if (Visible)
			{
				RangePosition range = grid.SelectionRange;

				statusToolStripStatusLabel.Text = string.Format(
					"Range[{0,3},{1,3} -{2,3},{3,3}] Span[{4,3},{5,3}]",
					range.Row, range.Col, range.EndRow, range.EndCol, range.Rows, range.Cols);

				int r = range.Row;
				int c = range.Col;
				SetCellLabelInfo(labCenter, r, c);
				SetCellLabelInfo(labCenter2, r, c);

				labLeft.Text = labRight.Text = labTop.Text = labDown.Text = string.Empty;
				labBorderLeft.Text = labBorderRight.Text = labBorderTop.Text = labBorderDown.Text = string.Empty;

				if (c > 0) SetCellLabelInfo(labLeft, r, c - 1);
				if (c < grid.ColumnCount - 1) SetCellLabelInfo(labRight, r, c + 1);
				if (r > 0) SetCellLabelInfo(labTop, r - 1, c);
				if (r < grid.RowCount - 1) SetCellLabelInfo(labDown, r + 1, c);

				if (c >= 0) labBorderLeft.Text = GetBorderInfo(r, c, BorderPositions.Left);
				if (c < grid.ColumnCount) labBorderRight.Text = GetBorderInfo(r, range.EndCol, BorderPositions.Right);
				if (r >= 0) labBorderTop.Text = GetBorderInfo(r, c, BorderPositions.Top);
				if (r < grid.RowCount) labBorderDown.Text = GetBorderInfo(range.EndRow, c, BorderPositions.Bottom);

				Cell firstCell = grid.GetCell(range.StartPos);
				labFormat.Text = firstCell == null ? string.Empty : firstCell.DataFormat.ToString();
				labFormatArgs.Text = firstCell == null ? string.Empty : DumpFormatArgs(firstCell.DataFormatArgs);
			}
		}

		private string DumpFormatArgs(object args)
		{
			StringBuilder sb = new StringBuilder();
			if (args is NumberDataFormatter.NumberFormatArgs)
			{
				NumberDataFormatter.NumberFormatArgs nargs = (NumberDataFormatter.NumberFormatArgs)args;
				sb.AppendLine(string.Format("{0,-20} = {1}", "Decimal Places", nargs.DecimalPlaces));
				sb.AppendLine(string.Format("{0,-20} = {1}", "Negative Style", nargs.NegativeStyle.ToString()));
				sb.AppendLine(string.Format("{0,-20} = {1}", "Use Separator", nargs.UseSeparator));
			}
			else if (args is DateTimeDataFormatter.DateTimeFormatArgs)
			{
				DateTimeDataFormatter.DateTimeFormatArgs dargs = (DateTimeDataFormatter.DateTimeFormatArgs)args;
				sb.AppendLine(string.Format("{0,-20} = {1}", "Format", dargs.Format));
			}
			else if (args is CurrencyDataFormatter.CurrencyFormatArgs)
			{
				CurrencyDataFormatter.CurrencyFormatArgs cmargs = (CurrencyDataFormatter.CurrencyFormatArgs)args;
				sb.AppendLine(string.Format("{0,-20} = {1}", "Culture EnglishName", cmargs.CultureEnglishName));
				sb.AppendLine(string.Format("{0,-20} = {1}", "Decimal Places", cmargs.DecimalPlaces));
				sb.AppendLine(string.Format("{0,-20} = {1}", "Negative Style", cmargs.NegativeStyle));
				sb.AppendLine(string.Format("{0,-20} = {1}", "Prefix Symbol", cmargs.PrefixSymbol));
				sb.AppendLine(string.Format("{0,-20} = {1}", "Postfix Symbol", cmargs.PostfixSymbol));
			}
			return sb.ToString();
		}

		private void SetCellLabelInfo(Label lab, int r, int c)
		{
			Cell cell = grid.GetCell(r, c);

			bool health = true;

			if (cell != null)
			{
				if (!cell.MergeStartPos.IsEmpty || !cell.MergeEndPos.IsEmpty)
				{
					Cell mergedStartCell = grid.GetCell(cell.MergeStartPos);
					Cell mergedEndCell = grid.GetCell(cell.MergeEndPos);

					health = cell.InternalRow == r && cell.InternalCol == c
						&& !((mergedStartCell == null || mergedStartCell.InternalPos != cell.MergeStartPos)
						|| (mergedEndCell == null || mergedEndCell.InternalPos != cell.MergeEndPos));
				}
			}

			lab.BackColor = health ? SystemColors.Window : Color.LightCoral;
			lab.Text = GetCellInfo(r, c);
		}

		private string GetCellInfo(int r, int c)
		{
			Cell cell = grid.GetCell(r, c);

			StringBuilder sb = new StringBuilder();
			sb.AppendLine(string.Format("Cell  [{0,3},{1,3}]", cell == null ? r : cell.InternalRow, cell== null ? c: cell.InternalCol));

			if (cell != null)
			{
				sb.AppendLine(string.Format("Span  [{0,3},{1,3}]", cell.Rowspan, cell.Colspan));

				if (cell.MergeStartPos != CellPosition.Empty)
				{
					sb.AppendLine(string.Format("MStart[{0,3},{1,3}]", cell.MergeStartPos.Row, cell.MergeStartPos.Col));
				}

				if (cell.MergeEndPos != CellPosition.Empty)
				{
					sb.AppendLine(string.Format("MEnd  [{0,3},{1,3}]", cell.MergeEndPos.Row, cell.MergeEndPos.Col));
				}
			}
			else
			{
				sb.AppendLine("Empty");
			}

			return sb.ToString();
		}

		private string GetBorderInfo(int r, int c, BorderPositions borderPos)
		{
			StringBuilder sb = new StringBuilder();

			if ((borderPos & BorderPositions.Top) > 0)
			{
				ReoGridHBorder top = grid.RetrieveHBorder(r, c);
				sb.AppendLine(string.Format("Top   [{0,3},{1,3}]", r, c));

				if (top != null)
				{
					sb.AppendLine(string.Format("Cols  [{0,7}]", top.Span));
					sb.AppendLine(string.Format("Pos   [{0,7}]", top.Pos));
					sb.AppendLine(string.Format("Style [{0,7}]", top.Style == null ? "" : " YES "));
				}
				else sb.AppendLine("Empty");
			}
			if ((borderPos & BorderPositions.Bottom) > 0)
			{
				ReoGridHBorder bottom = grid.RetrieveHBorder(r + 1, c);
				sb.AppendLine(string.Format("Bottom[{0,3},{1,3}]", r + 1, c));

				if (bottom != null)
				{
					sb.AppendLine(string.Format("Cols  [{0,7}]", bottom.Span));
					sb.AppendLine(string.Format("Pos   [{0,7}]", bottom.Pos));
					sb.AppendLine(string.Format("Style [{0,7}]", bottom.Style == null ? "" : " YES "));
				}
				else sb.AppendLine("Empty");
			}
			if ((borderPos & BorderPositions.Left) > 0)
			{
				ReoGridVBorder left = grid.RetrieveVBorder(r, c);
				sb.AppendLine(string.Format("Left  [{0,3},{1,3}]", r, c));

				if (left != null)
				{
					sb.AppendLine(string.Format("Rows  [{0,7}]", left.Span));
					sb.AppendLine(string.Format("Pos   [{0,7}]", left.Pos));
					sb.AppendLine(string.Format("Style [{0,7}]", left.Style == null ? "" : " YES "));
				}
				else sb.AppendLine("Empty");
			}
			if ((borderPos & BorderPositions.Right) > 0)
			{
				ReoGridVBorder right = grid.RetrieveVBorder(r, c + 1);
				sb.AppendLine(string.Format("Right [{0,3},{1,3}]", r, c + 1));

				if (right != null)
				{
					sb.AppendLine(string.Format("Rows  [{0,7}]", right.Span));
					sb.AppendLine(string.Format("Pos   [{0,7}]", right.Pos));
					sb.AppendLine(string.Format("Style [{0,7}]", right.Style == null ? "" : " YES "));
				}
				else sb.AppendLine("Empty");
			}
			return sb.ToString();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			e.Cancel = true;
			Visible = false;
		}

		public enum InitTab
		{
			Grid,
			Border,
			Format,
		}
	}

	internal class RGCellLayoutGraphControl : Control
	{
		private Worksheet grid;
		
		public Worksheet Grid
		{
			get { return this.grid; }
			set
			{
				if (grid != value)
				{
					this.grid = value;
					Invalidate();
				}
			}
		}

		private RangePosition range;
		public RangePosition Range
		{
			get { return range; }
			set
			{
				if (range != value)
				{
					this.range = value;
					Invalidate();
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			System.Drawing.Graphics g = e.Graphics;

			if (grid == null)
			{
				g.DrawString("No ReoGrid Control specified.", Font, Brushes.Red, 10, 10);
			}
			else if (range.IsEmpty)
			{
				g.DrawString("Range is empty.", Font, Brushes.Red, 10, 10);
			}
			else
			{
				Rectangle bounds = new Rectangle(Left + Padding.Left, Top + Padding.Top,
					Right - Padding.Left + Padding.Right - 1, Bottom - Padding.Bottom + Padding.Top - 1);

				Rectangle rect = bounds;
				rect.Inflate(-5, -5);

				int hh = rect.Top + rect.Height / 2;
				//if (grid.RangeIsCell(range))
				//{
				//  ReoGridCell cell = grid.GetCell(range.Row, range.Col);

				//}
				//else
				//{
				//}
				RectangleF cellBounds;

				Cell cell = grid.GetCell(range.Row,range.Col);
				if (cell == null)
				{
					cellBounds = grid.GetCellBounds(range.Row, range.Col);
				}
				else
				{
					cell.Bounds = cell.Bounds;

					using (StringFormat sf = new StringFormat(StringFormat.GenericTypographic))
					{
						//float maxSpace = 1;

						//string leftPx = cellBounds.Left + "px";
						//SizeF leftSize = g.MeasureString(leftPx, Font, rect.Width, sf);
						//if (maxSpace < leftSize.Width) maxSpace = leftSize.Width;

						//g.DrawString(leftPx, Font, Brushes.Black, rect.Left, (rect.Top + hh - leftSize.Height / 2));

						g.DrawRectangle(Pens.DimGray, rect);
					}
				}
			}
		}
	}

	public class RGDebugLogWritter : ILogWritter
	{
		private static readonly RGDebugLogWritter instance = new RGDebugLogWritter();
		public static RGDebugLogWritter Instance { get { return instance; } }

		private RGDebugLogWritter()
		{
		}

		private List<string> lines = new List<string>();

		public List<string> Lines
		{
			get { return lines; }
			set { lines = value; }
		}

		private int capacity = 100;

		public event EventHandler<DebugLogEventArgs> LogAppended;

		#region ILogWritter Members

		public void Log(string cat, string msg)
		{
			if (string.Compare(cat, "rgdebug") == 0)
			{
				if (lines.Count >= capacity)
				{
					lines.RemoveRange(0, lines.Count - capacity + 1);
				}

				lines.Add(msg);

				if (LogAppended != null) LogAppended(this, new DebugLogEventArgs(msg));
			}
		}

		#endregion
	}

	public class DebugLogEventArgs: EventArgs
	{
		public string Text{get;set;}
		public DebugLogEventArgs(string text)
		{
			this.Text = text;
		}
	}
}

#pragma warning restore 1591
#endif // WINFORM
