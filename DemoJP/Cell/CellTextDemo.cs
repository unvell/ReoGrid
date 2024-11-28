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

using System.Windows.Forms;

namespace unvell.ReoGrid.Demo.CellDemo
{
	/// <summary>
	/// セル中テキスト設定のデモ
	/// </summary>
	public partial class CellTextDemo : UserControl
	{
		private Worksheet sheet;

		public CellTextDemo()
		{
			InitializeComponent();

			sheet = grid.CurrentWorksheet;

			sheet.SetColumnsWidth(1, 1, 200);

			// セル中表示テキストの編集

			// demo 1
			this.SetCellNormalText();

			// demo 2
			this.AlignText();

			// demo 3
			this.SetNumberFormat();
		}

		/// <summary>
		/// Demo 1: Set normal text
		/// </summary>
		void SetCellNormalText()
		{
			sheet["B2"] = "通常のテキスト";
			sheet.Ranges["B2:C2"].BorderOutside = RangeBorderStyle.BlackSolid;
		}

		/// <summary>
		/// Demo 3: Align text to right
		/// </summary>
		void AlignText()
		{
			sheet["B7"] = "右寄せのテキスト";
			sheet.Cells["B7"].Style.HAlign = ReoGridHorAlign.Right;
		}

		/// <summary>
		/// Demo 4: Set number format
		/// </summary>
		void SetNumberFormat()
		{
			sheet["B9"] = 12345.23456;
			sheet.Cells["B9"].DataFormat = DataFormat.CellDataFormatFlag.Number;
			sheet.Cells["B9"].DataFormatArgs = new DataFormat.NumberDataFormatter.NumberFormatArgs
			{
				DecimalPlaces = 2,
				UseSeparator = true
			};

			sheet["C9"] = "(数字は自動的に右寄せに設定されます)";
		}
	}
}
