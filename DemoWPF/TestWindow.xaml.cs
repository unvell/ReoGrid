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
		//Data.Index4DArray<int> arr = new Data.Index4DArray<int>();

		public TestWindow()
		{
			InitializeComponent();

			var sheet = grid.CurrentWorksheet;

			sheet.AddOutline(RowOrColumn.Column, 5, 2).Collapse();
			sheet.FreezeToCell(2, 7, FreezeArea.LeftTop);
		}
	}
}
