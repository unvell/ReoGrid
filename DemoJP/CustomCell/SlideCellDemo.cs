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

using unvell.ReoGrid.CellTypes;
using unvell.ReoGrid.Events;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.Demo.CellTypeDemo
{
	/// <summary>
	/// カスタマイズしたスライドセルのデモ
	/// </summary>
	public partial class SlideCellDemo : UserControl
	{
		private Worksheet worksheet;

		public SlideCellDemo()
		{
			InitializeComponent();

			worksheet = grid.CurrentWorksheet;

			// グリッド線を表示／非表示の切り替え
			chkShowGridLines.CheckedChanged += (s, e) =>
				worksheet.SetSettings(WorksheetSettings.View_ShowGridLine, chkShowGridLines.Checked);

			// 選択モードの切り替え
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

			worksheet[6, 2] = "計算式でE8セルの値を読み込むように設定：=E8";
			worksheet[2, 3] = "スライドを移動してみてください。";

			// link
			worksheet.MergeRange(12, 0, 1, 7);
			worksheet[11, 0] = "カスタマイズしたセルについて詳しくは：";
			worksheet[12, 0] = new unvell.ReoGrid.CellTypes.HyperlinkCell(
				"https://reogrid.net/jp/document/Custom%20Cell", true);
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
			// セルから値を取得
			double value = dc.Cell.GetData<double>();

			var g = dc.Graphics;

			int halfHeight = (int)Math.Round(Bounds.Height / 2f);
			int sliderHeight = (int)Math.Min(Bounds.Height - 4, 20);

			// スライドバーを描画
			g.FillRectangle(4, halfHeight - 3, Bounds.Width - 8, 6, SolidColor.Gainsboro);

			int x = 2 + (int)Math.Round(value * (Bounds.Width - 12));

			// スライドつまみの位置とサイズを設定
			Rectangle rect = new Rectangle(x, halfHeight - sliderHeight / 2, 8, sliderHeight);

			// スライドのつまみを描画
			g.FillRectangle(rect, IsHover ? SolidColor.LimeGreen : SolidColor.LightGreen);
		}

		public override bool OnMouseDown(CellMouseEventArgs e)
		{
			UpdateValueByCursorX(e.CellPosition, e.RelativePosition.X);

			// イベントを処理した場合 true を返す
			return true;
		}

		public override bool OnMouseMove(CellMouseEventArgs e)
		{
			// マウスの左ボタンでスライドを移動
			if (e.Buttons == unvell.ReoGrid.Interaction.MouseButtons.Left)
			{
				UpdateValueByCursorX(e.CellPosition, e.RelativePosition.X);
			}

			return false;
		}

		private void UpdateValueByCursorX(CellPosition cellPos, float x)
		{
			// マウスの位置からパーセントの値を計算
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
			// 手入力での編集を禁止
			return false;
		}
	}

}
