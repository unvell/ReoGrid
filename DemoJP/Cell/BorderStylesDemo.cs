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

 using System.Drawing;
using System.Windows.Forms;

namespace unvell.ReoGrid.Demo.CellDemo
{
	/// <summary>
	/// 罫線スタイルデモ
	/// </summary>
	public partial class BorderStylesDemo : UserControl
	{
		public BorderStylesDemo()
		{
			InitializeComponent();

			// 現在表示中のワークシートを取得
			var sheet = this.grid.CurrentWorksheet;

			// グリッド線を非表示
			sheet.DisableSettings(WorksheetSettings.View_ShowGridLine);

			// 行列サイズを調整
			sheet.SetColumnsWidth(0, 15, 60);
			sheet.SetRowsHeight(0, 20, 30);
			
			sheet["B2"] = "罫線スタイル：";

			// 範囲を定義
			var range = new RangePosition("B4:D4");

			// 罫線スタイルを設定

			sheet[range] = "Solid";
			sheet.SetRangeBorders(range, BorderPositions.Bottom, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.Solid });

			range.Offset(1, 0);
			sheet[range] = "Dotted";
			sheet.SetRangeBorders(range, BorderPositions.Bottom, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.Dotted });

			range.Offset(1, 0);
			sheet[range] = "Dashed";
			sheet.SetRangeBorders(range, BorderPositions.Bottom, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.Dashed });

			range.Offset(1, 0);
			sheet[range] = "Dashed2";
			sheet.SetRangeBorders(range, BorderPositions.Bottom, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.Dashed2 });

			range.Offset(1, 0);
			sheet[range] = "DashDot";
			sheet.SetRangeBorders(range, BorderPositions.Bottom, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.DashDot });

			range.Offset(1, 0);
			sheet[range] = "DashDotDot";
			sheet.SetRangeBorders(range, BorderPositions.Bottom, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.DashDotDot });

			range.Offset(1, 0);
			sheet[range] = "Bold Solid";
			sheet.SetRangeBorders(range, BorderPositions.Bottom, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.BoldSolid });

			range.Offset(1, 0);
			sheet[range] = "Bold Dotted";
			sheet.SetRangeBorders(range, BorderPositions.Bottom, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.BoldDotted });

			range.Offset(1, 0);
			sheet[range] = "Bold Dashed";
			sheet.SetRangeBorders(range, BorderPositions.Bottom, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.BoldDashed });

			range.Offset(1, 0);
			sheet[range] = "Bold DashDot";
			sheet.SetRangeBorders(range, BorderPositions.Bottom, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.BoldDashDot });

			range.Offset(1, 0);
			sheet[range] = "Bold DashDotDot";
			sheet.SetRangeBorders(range, BorderPositions.Bottom, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.BoldDashDotDot });

			range.Offset(1, 0);
			sheet[range] = "Strong Solid";
			sheet.SetRangeBorders(range, BorderPositions.Bottom, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.BoldSolidStrong });

			range.Offset(1, 0);
			sheet[range] = "DoubleLine";
			sheet.SetRangeBorders(range, BorderPositions.Bottom, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.DoubleLine });

			sheet["G3"] = "外側 Outside";
			sheet.Cells["G3"].Style.HAlign = ReoGridHorAlign.Center;
			sheet.Cells["G3"].Style.VAlign = ReoGridVerAlign.Middle;
			sheet.SetRangeBorders("F2:H4", BorderPositions.Outside, RangeBorderStyle.BlackSolid);

			sheet["K3"] = "内側 Inside";
			sheet.Cells["K3"].Style.HAlign = ReoGridHorAlign.Center;
			sheet.Cells["K3"].Style.VAlign = ReoGridVerAlign.Middle;
			sheet.SetRangeBorders("J2:L4", BorderPositions.InsideAll, RangeBorderStyle.BlackDotted);

			sheet["G7"] = "左＆右 Left & Right";
			sheet.Cells["G7"].Style.HAlign = ReoGridHorAlign.Center;
			sheet.Cells["G7"].Style.VAlign = ReoGridVerAlign.Middle;
			sheet.SetRangeBorders("F6:H8", BorderPositions.Left | BorderPositions.Right, RangeBorderStyle.GraySolid);

			sheet["K7"] = "上＆下 Top & Bottom";
			sheet.Cells["K7"].Style.HAlign = ReoGridHorAlign.Center;
			sheet.Cells["K7"].Style.VAlign = ReoGridVerAlign.Middle;
			sheet.SetRangeBorders("J6:L8", BorderPositions.Top | BorderPositions.Bottom, RangeBorderStyle.GrayDotted);

			sheet["G11"] = "SlateBlue";
			sheet.Cells["G11"].Style.HAlign = ReoGridHorAlign.Center;
			sheet.Cells["G11"].Style.VAlign = ReoGridVerAlign.Middle;
			sheet.SetRangeBorders("F10:H12", BorderPositions.InsideHorizontal | BorderPositions.Top | BorderPositions.Bottom,
				new RangeBorderStyle { Color = Color.SlateBlue, Style = BorderLineStyle.Dashed });

			sheet["K11"] = "DarkGoldenrod";
			sheet.Cells["K11"].Style.HAlign = ReoGridHorAlign.Center;
			sheet.Cells["K11"].Style.VAlign = ReoGridVerAlign.Middle;
			sheet.SetRangeBorders("J10:L12", BorderPositions.InsideVertical | BorderPositions.Left | BorderPositions.Right,
				new RangeBorderStyle { Color = Color.DarkGoldenrod, Style = BorderLineStyle.Dashed });

		}
	}
}
