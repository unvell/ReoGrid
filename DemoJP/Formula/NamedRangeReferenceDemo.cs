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

namespace unvell.ReoGrid.Demo.FormulaDemo
{
	/// <summary>
	/// 命名範囲の利用デモ
	/// </summary>
	public partial class NamedRangeReferenceDemo : UserControl
	{
		public NamedRangeReferenceDemo()
		{
			InitializeComponent();

			var sheet = grid.CurrentWorksheet;

			sheet.SetColumnsWidth(1, 4, 100);

			sheet[1, 1] = "赤線で囲まれている範囲は名前 myRange の命名範囲として定義されている";

			// define a named range
			var myRange = sheet.DefineNamedRange("myRange", "B4:E6");

			// set border style for the range
			myRange.BorderOutside = new RangeBorderStyle
			{
				Color = Color.Red,
				Style = BorderLineStyle.Solid,
			};

			// set data for the range
			myRange.Data = new object[,] {
				{ 1, 2, 3, 4 },
				{ .1, .2, .3, .4 },
				{ "リンゴ", "バナナ", "ミカン", "ナシ" },
			};

			sheet.Ranges["C8:E9"].Data = new object[,] {
				{ "SUM", "COUNT", "AVERAGE" },
				{ "=SUM(myRange)", "=COUNT(myRange)", "=AVERAGE(myRange)" },
			};
		}
	}
}
