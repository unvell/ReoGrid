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

			var sheet = grid.CurrentWorksheet;

			sheet[0, 0] = new object[,] {
				{ "A", "B", "C", "D", "E" }
			};

			sheet.CreateColumnFilter("A", "E");
		}
	}
}
