using System.Windows;

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
