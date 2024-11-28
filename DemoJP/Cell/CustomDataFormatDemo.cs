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
using unvell.ReoGrid.DataFormat;

namespace unvell.ReoGrid.Demo.CellDemo
{
	/// <summary>
	/// カスタマイズしたデータフォーマットのデモ
	/// </summary>
	public partial class CustomDataFormatDemo : UserControl
	{
		private Worksheet sheet;

		public CustomDataFormatDemo()
		{
			InitializeComponent();

			this.sheet = grid.CurrentWorksheet;
			sheet.Name = "カスタマイズしたデータ書式";

			DataFormatterManager.Instance.DataFormatters[CellDataFormatFlag.Custom] = new MyDataFormatter();

			var cell = this.sheet.Cells["B2"];
			cell.DataFormat = CellDataFormatFlag.Custom;
			cell.Data = 12345.6789;
		}
	}

	class MyDataFormatter : IDataFormatter
	{
		public string FormatCell(Cell cell)
		{
			// Custom formatter only valid for this demo
			if (cell.Worksheet.Name != "カスタマイズしたデータ書式") return null;

			double val = cell.GetData<double>();

			return val < 0 ? string.Format("[{0}]", (-val).ToString("###,###,##0.00")) : val.ToString("###,###,###.00");
		}

		public bool PerformTestFormat()
		{
			return true;
		}
	}
}
