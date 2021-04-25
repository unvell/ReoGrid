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

namespace unvell.ReoGrid.Demo.DocumentDemo
{
	public partial class MonthCalendarDocDemo : UserControl
	{
		public MonthCalendarDocDemo()
		{
			InitializeComponent();

			// get first worksheet instance
			var sheet = reoGridControl.Worksheets[0];

			// load tepmlate from RGF file.
			// RGF file is a file format that contains worksheet information, 
			// such as data, styles, borders, formula and etc, RGF file can 
			// be saved and loaded by ReoGrid and ReoGridEditor.
			//
			// https://reogrid.net/document/rgf-format
			// 
			sheet.LoadRGF("_Templates\\RGF\\calendar_2008_1.rgf");

			// hide sheet tab control
			reoGridControl.SetSettings(WorkbookSettings.View_ShowSheetTabControl, false);

			// hide row header and column header
			sheet.SetSettings(WorksheetSettings.View_ShowHeaders, false);

			// set entire worksheet read-only
			sheet.SetSettings(WorksheetSettings.Edit_Readonly, true);
		}
	}
}
