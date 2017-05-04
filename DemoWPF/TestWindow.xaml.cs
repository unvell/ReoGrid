using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace unvell.ReoGrid.WPFDemo
{
	/// <summary>
	/// Interaction logic for TestWindow.xaml
	/// </summary>
	public partial class TestWindow : Window
	{
		public TestWindow()
		{
			InitializeComponent();

			TestIssue_11();
		}

		private void TestIssue_11()
		{
			// I. A1 'Word Break' Wrap Mode
			Worksheet worksheet = grid.CurrentWorksheet;
			Cell cell = worksheet.Cells["A1"];
			cell.Data = "VeryLongText";// "A Long text with \nlinebreaks";

			WorksheetRangeStyle style = new WorksheetRangeStyle();
			style.Flag = PlainStyleFlag.LayoutAll;
			style.TextWrapMode = TextWrapMode.WordBreak;

			worksheet.SetRangeStyles(cell.PositionAsRange, style);

			// II. Enable Edit_AutoExpandRowHeight
			//worksheet.EnableSettings(WorksheetSettings.Edit_AutoExpandRowHeight);

			// Attempt I
			worksheet.AutoFitRowHeight(0);

			// Attempt II
			//grid.CurrentWorksheet.GetRowHeader(0).FitHeightToCells();
		}
	}
}
