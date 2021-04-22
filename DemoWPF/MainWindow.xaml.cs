using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using unvell.ReoGrid.CellTypes;
using unvell.ReoGrid.Chart;
using unvell.ReoGrid.Drawing.Shapes;

namespace unvell.ReoGrid.WPFDemo
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			// don't use Clear method in actual application,
			// instead, load template into the first worksheet directly.
			grid.Worksheets.Clear();

			// handles event to update menu check status.
			grid.SettingsChanged += (s, e) => UpdateMenuChecks();
			grid.CurrentWorksheetChanged += (s, e) => UpdateMenuChecks();

			// add demo sheet 1: document template
			AddDemoSheet1();

			// add demo sheet 2: chart and drawing
			AddDemoSheet2();

			// add demo sheet 3: cell types
			AddDemoSheet3();
		}

		private void UpdateMenuChecks()
		{
			this.viewHorizontalScrollbarVisible.IsChecked = grid.HasSettings(unvell.ReoGrid.WorkbookSettings.View_ShowHorScroll);
			this.viewVerticalScrollbarVisible.IsChecked = grid.HasSettings(unvell.ReoGrid.WorkbookSettings.View_ShowVerScroll);
			this.viewSheetTabVisible.IsChecked = grid.SheetTabVisible;
			this.viewSheetTabNewButtonVisible.IsChecked = grid.SheetTabNewButtonVisible;

			var sheet = grid.CurrentWorksheet;
			this.viewGuideLineVisible.IsChecked = sheet.HasSettings(WorksheetSettings.View_ShowGridLine);
			this.viewPageBreaksVisible.IsChecked = sheet.HasSettings(WorksheetSettings.View_ShowPageBreaks);
		}

		#region Demo Sheet 1 : Document Template
		private void AddDemoSheet1()
		{
			/****************** Sheet1 : Document Template ********************/
			var worksheet = grid.NewWorksheet("Document");

			// load template
			using (MemoryStream ms = new MemoryStream(Properties.Resources.order_sample))
			{
				worksheet.LoadRGF(ms);
			}

			// fill data into worksheet
			var dataRange = worksheet.Ranges["A21:F35"];

			dataRange.Data = new object[,]
			{
				{"[23423423]", "Product ABC", 15, 150},
				{"[45645645]", "Product DEF", 1, 75},
				{"[78978978]", "Product GHI", 2, 30},
			};

			// set subtotal formula
			worksheet.Cells["G21"].Formula = "E21*F21";

			// auto fill other subtotals
			worksheet.AutoFillSerial("G21", "G22:G35");
		}
		#endregion // Demo Sheet 1 : Document Template

		#region Demo Sheet 2 : Chart & Drawing
		private void AddDemoSheet2()
		{
			/****************** Sheet2 : Chart & Drawing ********************/
			var worksheet = grid.NewWorksheet("Chart & Drawing");

			worksheet["A2"] = new object[,] {
					{null, 2008,  2009, 2010, 2011, 2012},
					{"City 1",  5,  10, 12, 11, 14},
					{"City 2",  7,  8,  7,  6,  4},
					{"City 3",  13, 10, 9,  10, 9},
					{"Total", 25, 28, 28, 27, 27},
			};

			worksheet.AddOutline(RowOrColumn.Row, 3, 4);

			var range = worksheet.Ranges["B3:F6"];
			worksheet.AddHighlightRange(range);

			var chart = new LineChart
			{
				Location = new Point(360, 140),

				Title = "Line Chart Sample",

				DataSource = new WorksheetChartDataSource(worksheet, "A2:A6", "B3:F6")
				{
					CategoryNameRange = new RangePosition("B2:F2"),
				},
			};

			worksheet.FloatingObjects.Add(chart);

			// flow chart
			Line line1, line2;

			worksheet.FloatingObjects.Add(new RectangleShape
			{
				Location = new Graphics.Point(100, 200),
				Size = new Graphics.Size(160, 40),

				Text = "1. Add Data Source",
			});

			worksheet.FloatingObjects.Add(line1 = new Line
			{
				StartPoint = new Graphics.Point(180, 240),
				EndPoint = new Graphics.Point(180, 270),
			});

			worksheet.FloatingObjects.Add(new RectangleShape
			{
				Location = new Graphics.Point(100, 270),
				Size = new Graphics.Size(160, 40),

				Text = "2. Create Data Source",
			});

			worksheet.FloatingObjects.Add(line2 = new Line
			{
				StartPoint = new Graphics.Point(180, 310),
				EndPoint = new Graphics.Point(180, 340),
			});

			worksheet.FloatingObjects.Add(new RectangleShape
			{
				Location = new Graphics.Point(100, 340),
				Size = new Graphics.Size(160, 40),

				Text = "3. Create and Put Chart",
			});

			// not available yet
			//line1.Style.EndCap = Graphics.LineCapStyles.Arrow;
			//line2.Style.EndCap = Graphics.LineCapStyles.Arrow;
		}
		#endregion // Demo Sheet 2 : Chart & Drawing

		#region Demo Sheet 3 : Built-in Cell Types
		private void AddDemoSheet3()
		{
			/****************** Sheet3 : Built-in Cell Types ********************/
			var worksheet = grid.NewWorksheet("Cell Types");

			// set default sheet style
			worksheet.SetRangeStyles(RangePosition.EntireRange, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.FontName | PlainStyleFlag.VerticalAlign,
				FontName = "Arial",
				VAlign = ReoGridVerAlign.Middle,
			});

			worksheet.SetSettings(WorksheetSettings.View_ShowGridLine |
				 WorksheetSettings.Edit_DragSelectionToMoveCells, false);
			worksheet.SelectionMode = WorksheetSelectionMode.Cell;
			worksheet.SelectionStyle = WorksheetSelectionStyle.FocusRect;

			var middleStyle = new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.Padding | PlainStyleFlag.HorizontalAlign,
				Padding = new PaddingValue(2),
				HAlign = ReoGridHorAlign.Center,
			};

			var grayTextStyle = new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.TextColor,
				TextColor = Colors.DimGray
			};

			worksheet.MergeRange(1, 1, 1, 6);

			worksheet.SetRangeStyles(1, 1, 1, 6, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.TextColor | PlainStyleFlag.FontSize,
				TextColor = Colors.DarkGreen,
				FontSize = 18,
			});

			worksheet[1, 1] = "Built-in Cell Bodies";

			worksheet.SetColumnsWidth(1, 1, 100);
			worksheet.SetColumnsWidth(2, 1, 30);
			worksheet.SetColumnsWidth(3, 1, 100);
			worksheet.SetColumnsWidth(6, 2, 65);

			// button
			worksheet.MergeRange(3, 2, 1, 2);
			var btn = new ButtonCell("Hello");
			worksheet[3, 1] = new object[] { "Button: ", btn };
			btn.Click += (s, e) => ShowText(worksheet, "Button clicked.");

			// link
			worksheet.MergeRange(5, 2, 1, 3);
			var link = new HyperlinkCell("http://www.google.com");
			worksheet[5, 1] = new object[] { "Hyperlink", link };

			// checkbox
			var checkbox = new CheckBoxCell();
			worksheet.SetRangeStyles(7, 2, 1, 1, middleStyle);
			worksheet.SetRangeStyles(8, 2, 1, 1, grayTextStyle);
			worksheet[7, 1] = new object[] { "Check box", checkbox, "Auto destroy after 5 minutes." };
			worksheet[8, 2] = "(Keyboard is also supported to change the status of control)";
			checkbox.CheckChanged += (s, e) => ShowText(worksheet, "Check box switch to " + checkbox.IsChecked.ToString());

			// radio & radio group
			worksheet[10, 1] = "Radio Button";
			worksheet.SetRangeStyles(10, 2, 3, 1, middleStyle);
			var radioGroup = new RadioButtonGroup();
			worksheet[10, 2] = new object[,] {
				{new RadioButtonCell() { RadioGroup = radioGroup }, "Apple"},
				{new RadioButtonCell() { RadioGroup = radioGroup }, "Orange"},
				{new RadioButtonCell() { RadioGroup = radioGroup }, "Banana"}
			};
			radioGroup.RadioButtons.ForEach(rb => rb.CheckChanged += (s, e) =>
				ShowText(worksheet, "Radio button selected: " + worksheet[rb.Cell.Row, rb.Cell.Column + 1]));
			worksheet[10, 2] = true;
			worksheet[13, 2] = "(By adding radio buttons into same RadioGroup to make them toggle each other automatically)";
			worksheet.SetRangeStyles(13, 2, 1, 1, grayTextStyle);

			// dropdown - Not available yet - Planned from next version
			//worksheet.MergeRange(15, 2, 1, 3);
			//var dropdown = new DropdownListCell("Apple", "Orange", "Banana", "Pear", "Pumpkin", "Cherry", "Coconut");
			//worksheet[15, 1] = new object[] { "Dropdown", dropdown };
			//worksheet.SetRangeBorders(15, 2, 1, 3, BorderPositions.Outside, RangeBorderStyle.GraySolid);

			// custom cell type - slide cell body
			worksheet.MergeRange(15, 2, 1, 2);
			worksheet[15, 1] = new object[] { "Brightness", new SlideCellBody() };
			worksheet[15, 2] = 1;
			worksheet.CellDataChanged += (s, e) =>
				{
					if (e.Cell.Position == new CellPosition(15, 2))
					{
						byte val = (byte)(worksheet.GetCellData<double>(e.Cell.Position) * 255);
						worksheet.SetRangeStyles(RangePosition.EntireRange, new WorksheetRangeStyle
						{
							Flag = PlainStyleFlag.BackColor,
							BackColor = new Graphics.SolidColor(val, val, val),
						});
					}
				};

			// image
			worksheet.MergeRange(2, 6, 5, 2);

			var image = new System.Windows.Media.Imaging.BitmapImage();
			image.BeginInit();

			using (MemoryStream memory = new MemoryStream(Properties.Resources.computer_laptop_png))
			{
				image.StreamSource = memory;
				image.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
				image.EndInit();
			}

			worksheet[2, 6] = new ImageCell(image);

			// information cell
			worksheet.SetRangeBorders(19, 0, 1, 10, BorderPositions.Top, RangeBorderStyle.GraySolid);
		}

		private void ShowText(Worksheet sheet, string text)
		{
			sheet[19, 0] = text;
		}
		#endregion // Demo Sheet 3 : Built-in Cell Types

		#region Menu - File
		private void File_New_Click(object sender, RoutedEventArgs e)
		{
			grid.Reset();
		}

		private void File_Open_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

			dlg.DefaultExt = ".xlsx";
			dlg.Filter = "Supported file format(*.xlsx;*.rgf;*.xml)|*.xlsx;*.rgf;*.xml|Excel 2007 Document(*.xlsx)|*.xlsx|ReoGrid Format(*.rgf;*.xml)|*.rgf;*.xml";

			// Process open file dialog box results 
			if (dlg.ShowDialog() == true)
			{
				// Open document 
				try
				{
					grid.Load(dlg.FileName);
				}
				catch (Exception ex)
				{
					MessageBox.Show(this, "Loading error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
		}

		private void File_Save_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

			dlg.DefaultExt = ".xlsx";
			dlg.Filter = "Excel 2007 Document|*.xlsx|ReoGrid Format|*.rgf";

			// Process open file dialog box results 
			if (dlg.ShowDialog() == true)
			{
				// Open document 
				grid.Save(dlg.FileName);

				System.Diagnostics.Process.Start(dlg.FileName);
			}
		}

		private void File_Exit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		#endregion // Menu - File

		#region Menu - View
		private void View_SheetTab_Click(object sender, RoutedEventArgs e)
		{
			grid.SetSettings(unvell.ReoGrid.WorkbookSettings.View_ShowSheetTabControl, viewSheetTabVisible.IsChecked);
		}

		private void View_SheetTabNewButton_Click(object sender, RoutedEventArgs e)
		{
			grid.SheetTabNewButtonVisible = viewSheetTabNewButtonVisible.IsChecked;
		}

		private void View_HorizontalScrollbar_Click(object sender, RoutedEventArgs e)
		{
			grid.SetSettings(unvell.ReoGrid.WorkbookSettings.View_ShowHorScroll, viewHorizontalScrollbarVisible.IsChecked);
		}

		private void View_VerticalScrollbar_Click(object sender, RoutedEventArgs e)
		{
			grid.SetSettings(unvell.ReoGrid.WorkbookSettings.View_ShowVerScroll, viewVerticalScrollbarVisible.IsChecked);
		}

		private void View_GuideLine_Click(object sender, RoutedEventArgs e)
		{
			grid.CurrentWorksheet.SetSettings(WorksheetSettings.View_ShowGridLine, viewGuideLineVisible.IsChecked);
		}

		private void View_PageBreaks_Click(object sender, RoutedEventArgs e)
		{
			grid.CurrentWorksheet.SetSettings(WorksheetSettings.View_ShowPageBreaks, viewPageBreaksVisible.IsChecked);
		}
		#endregion // Menu - View

		#region Menu - Sheet

		private void freezeToCell_Click(object sender, RoutedEventArgs e)
		{
			grid.CurrentWorksheet.FreezeToCell(grid.CurrentWorksheet.FocusPos);
		}

		private void Sheet_Append_100_Rows_Click(object sender, RoutedEventArgs e)
		{
			grid.DoAction(new Actions.InsertRowsAction(grid.CurrentWorksheet.Rows, 100));
		}

		#endregion Menu - Sheet

	}
}
