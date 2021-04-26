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

namespace unvell.ReoGrid.Demo.Welcome
{
	public partial class WhatsNewDemo : UserControl
	{
		public WhatsNewDemo()
		{
			InitializeComponent();

			// load Excel template file
			this.reoGridControl.Load("_Templates\\Excel\\Welcome.xlsx");

			var sheet1 = this.reoGridControl.Worksheets[0];

			// iterate to set hyperlink cells type
			sheet1.IterateCells("B8:N17", (row, col, cell) =>
			{
				if (cell.DisplayText.StartsWith("http:", System.StringComparison.CurrentCultureIgnoreCase)
					|| cell.DisplayText.StartsWith("https:", System.StringComparison.CurrentCultureIgnoreCase)
					|| cell.DisplayText.StartsWith("mailto:", System.StringComparison.CurrentCultureIgnoreCase))
				{
					cell.Body = new CellTypes.HyperlinkCell();
				}

				return true;
			});
		}
	}
}
