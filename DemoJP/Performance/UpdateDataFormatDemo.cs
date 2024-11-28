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

using unvell.ReoGrid.DataFormat;

namespace unvell.ReoGrid.Demo.PerformanceDemo
{
	/// <summary>
	/// セル更新のレスポンス
	/// </summary>
	public partial class UpdateDataFormatDemo : UserControl
	{
		private Worksheet worksheet;

		public UpdateDataFormatDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;

			worksheet.SetRows(100);

			for (int r = 0; r < 100; r++)
			{
				for (int c = 0; c < 20; c++)
				{
					worksheet[r, c] = (r + 1) * (c + 1);
				}
			}
		}

		private void btnFormatAsNumber_Click(object sender, EventArgs e)
		{
			worksheet.SetRangeDataFormat(RangePosition.EntireRange, CellDataFormatFlag.Number,
				new NumberDataFormatter.NumberFormatArgs()
				{
					// decimal digit places 0.1234
					DecimalPlaces = 4,
					
					// negative number style: ( 123) 
					NegativeStyle = NumberDataFormatter.NumberNegativeStyle.RedBrackets,

					// use separator: 123,456
					UseSeparator = true,
				});
		}

		private void button2_Click(object sender, EventArgs e)
		{
			worksheet.SetRangeDataFormat(RangePosition.EntireRange, CellDataFormatFlag.DateTime,
				new DateTimeDataFormatter.DateTimeFormatArgs
				{
					// culture
					CultureName = "en-US",

					// pattern
					Format = "yyyy/MM/dd",
				});
		}

		private void button1_Click(object sender, EventArgs e)
		{
			worksheet.SetRangeDataFormat(RangePosition.EntireRange, CellDataFormatFlag.Percent,
				new NumberDataFormatter.NumberFormatArgs
				{
					// decimal digit places
					DecimalPlaces = 2,
				});
		}

		private void button4_Click(object sender, EventArgs e)
		{
			worksheet.SetRangeDataFormat(RangePosition.EntireRange, CellDataFormatFlag.Currency,
				new CurrencyDataFormatter.CurrencyFormatArgs
				{
					// culture name
					CultureEnglishName = "ja-JP",

					// decimal digit places
					DecimalPlaces = 0,

					// symbol
					PrefixSymbol = "￥",
				});
		}

		private void button3_Click(object sender, EventArgs e)
		{
			worksheet.SetRangeDataFormat(RangePosition.EntireRange, CellDataFormatFlag.Text, null);
		}
	}
}
