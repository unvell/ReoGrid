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

namespace unvell.ReoGrid.Demo.CellDemo
{
	/// <summary>
	/// セル中リッチテキストのデモ
	/// </summary>
	public partial class RichTextDemo : UserControl
	{
		private Worksheet sheet;

		public RichTextDemo()
		{
			InitializeComponent();

			sheet = grid.CurrentWorksheet;
		}

		private void RichTextDemo_Load(object sender, EventArgs e)
		{
			sheet.MergeRange("B2:F6");

			sheet.Cells["B2"].Style.TextWrap = TextWrapMode.WordBreak;

			sheet["B2"] = new Drawing.RichText()
				.Regular("The ")
				.Bold("Rich Text Format")
				.Regular(" (often abbreviated ")
				.Bold("RTF")
				.Regular(") is a proprietary")
				.Superscript("[6][7][8]")
				.Regular(" document file format with published specification developed by Microsoft Corporation from ")
				.Span("1987", textColor: Color.OrangeRed)
				.Span(" until ", textColor: Color.Black)
				.Span("2008", textColor: Color.OrangeRed)
				.Span(" for cross-platform document interchange with Microsoft products.", textColor: Color.Black);
		}
	}
}
