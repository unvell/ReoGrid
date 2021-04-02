/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net/
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * Author: Jing Lu <jingwood at unvell.com>
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

#if WPF

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using unvell.ReoGrid.Main;
using unvell.ReoGrid.Views;

namespace unvell.ReoGrid.WPF
{
	internal class SheetTabControl : Grid, ISheetTabControl
	{
		internal Grid canvas = new Grid()
		{
			Width = 0,
			VerticalAlignment = VerticalAlignment.Top,
			HorizontalAlignment = HorizontalAlignment.Left
		};

		private Image newSheetImage;

		public SheetTabControl()
		{
			this.Background = SystemColors.ControlBrush;
			this.BorderColor = Colors.DeepSkyBlue;

			this.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20) });
			this.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20) });
			this.ColumnDefinitions.Add(new ColumnDefinition { });
			this.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
			this.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5) });

			Border pleft = new ArrowBorder(this)
			{
				Child = new Polygon()
				{
					Points = new PointCollection(
						new Point[]{ 
							new Point(6, 0),
							new Point(0, 5),
							new Point(6, 10),
						}),
					Fill = SystemColors.ControlDarkDarkBrush,
					HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
					VerticalAlignment = System.Windows.VerticalAlignment.Center,
					Margin = new Thickness(4, 0, 0, 0),
				},
				Background = SystemColors.ControlBrush,
			};

			Border pright = new ArrowBorder(this)
			{
				Child = new Polygon()
				{
					Points = new PointCollection(
						new Point[] {
							new Point(0, 0),
							new Point(6, 5),
							new Point(0, 10),
						}),
					Fill = SystemColors.ControlDarkDarkBrush,
					HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
					VerticalAlignment = System.Windows.VerticalAlignment.Center,
					Margin = new Thickness(0, 0, 4, 0),
				},
				Background = SystemColors.ControlBrush,
			};

			this.canvas.RenderTransform = new TranslateTransform(0, 0);

			this.Children.Add(this.canvas);
			Grid.SetColumn(this.canvas, 2);

			this.Children.Add(pleft);
			Grid.SetColumn(pleft, 0);

			this.Children.Add(pright);
			Grid.SetColumn(pright, 1);

			BitmapImage imageSource = new BitmapImage();
			BitmapImage imageHoverSource = new BitmapImage();

			imageSource.BeginInit();
			imageSource.StreamSource = new System.IO.MemoryStream(unvell.ReoGrid.Properties.Resources.NewBuildDefinition_8952_inactive_png);
			imageSource.EndInit();

			imageHoverSource.BeginInit();
			imageHoverSource.StreamSource = new System.IO.MemoryStream(unvell.ReoGrid.Properties.Resources.NewBuildDefinition_8952_png);
			imageHoverSource.EndInit();

			newSheetImage = new Image()
			{
				Source = imageSource,
				HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
				VerticalAlignment = System.Windows.VerticalAlignment.Center,
				Margin = new Thickness(2),
				Cursor = System.Windows.Input.Cursors.Hand,
			};

			newSheetImage.MouseEnter += (s, e) => newSheetImage.Source = imageHoverSource;
			newSheetImage.MouseLeave += (s, e) => newSheetImage.Source = imageSource;
			newSheetImage.MouseDown += (s, e) =>
			{
				if (this.NewSheetClick != null)
				{
					this.NewSheetClick(this, null);
				}
			};

			this.Children.Add(newSheetImage);
			Grid.SetColumn(newSheetImage, 3);

			Border rightThumb = new Border
			{
				Child = new RightThumb(this),
				Cursor = System.Windows.Input.Cursors.SizeWE,
				Background = SystemColors.ControlBrush,
				Margin = new Thickness(0, 1, 0, 0),
				HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
			};
			this.Children.Add(rightThumb);
			Grid.SetColumn(rightThumb, 4);

			this.scrollTimer = new System.Windows.Threading.DispatcherTimer()
			{
				Interval = new TimeSpan(0, 0, 0, 0, 10),
			};

			scrollTimer.Tick += (s, e) =>
			{
				var tt = this.canvas.Margin.Left;

				if (this.scrollLeftDown)
				{
					if (tt < 0)
					{
						tt += 5;
						if (tt > 0) tt = 0;
					}
				}
				else if (this.scrollRightDown)
				{
					double max = this.ColumnDefinitions[2].ActualWidth - this.canvas.Width;

					if (tt > max)
					{
						tt -= 5;
						if (tt < max) tt = max;
					}
				}

				if (this.canvas.Margin.Left != tt)
				{
					this.canvas.Margin = new Thickness(tt, 0, 0, 0);
				}
			};

			pleft.MouseDown += (s, e) =>
			{
				this.scrollRightDown = false;
				if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
				{
					this.scrollLeftDown = true;
					this.scrollTimer.IsEnabled = true;
				}
				else if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
				{
					if (this.SheetListClick != null)
					{
						this.SheetListClick(this, null);
					}
				}
			};
			pleft.MouseUp += (s, e) =>
			{
				this.scrollTimer.IsEnabled = false;
				this.scrollLeftDown = false;
			};

			pright.MouseDown += (s, e) =>
			{
				this.scrollLeftDown = false;
				if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
				{
					this.scrollRightDown = true;
					this.scrollTimer.IsEnabled = true;
				}
				else if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
				{
					if (this.SheetListClick != null)
					{
						this.SheetListClick(this, null);
					}
				}
			};
			pright.MouseUp += (s, e) =>
			{
				this.scrollTimer.IsEnabled = false;
				this.scrollRightDown = false;
			};

			rightThumb.MouseDown += (s, e) =>
			{
				this.splitterMoving = true;
				rightThumb.CaptureMouse();
			};
			rightThumb.MouseMove += (s, e) =>
			{
				if (this.splitterMoving)
				{
					if (this.SplitterMoving != null)
					{
						this.SplitterMoving(this, null);
					}
				}
			};
			rightThumb.MouseUp += (s, e) =>
			{
				this.splitterMoving = false;
				rightThumb.ReleaseMouseCapture();
			};
		}

		private bool splitterMoving = false;
		private bool scrollLeftDown = false, scrollRightDown = false;

		private System.Windows.Threading.DispatcherTimer scrollTimer;

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);

			this.Clip = new RectangleGeometry(new Rect(0, 0, this.RenderSize.Width, this.RenderSize.Height));

			this.canvas.Height = this.Height - 2;
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
		}

		#region Dependency Properties

		public static readonly DependencyProperty SelectedBackColorProperty =
			DependencyProperty.Register("SelectedBackColor", typeof(Color), typeof(SheetTabControl));

		public Color SelectedBackColor
		{
			get { return (Color)GetValue(SelectedBackColorProperty); }
			set { SetValue(SelectedBackColorProperty, value); }
		}

		public static readonly DependencyProperty SelectedTextColorProperty =
			DependencyProperty.Register("SelectedTextColor", typeof(Color), typeof(SheetTabControl));

		public Color SelectedTextColor
		{
			get { return (Color)GetValue(SelectedTextColorProperty); }
			set { SetValue(SelectedTextColorProperty, value); }
		}

		public static readonly DependencyProperty BorderColorProperty =
			DependencyProperty.Register("BorderColor", typeof(Color), typeof(SheetTabControl));

		public Color BorderColor
		{
			get { return (Color)GetValue(BorderColorProperty); }
			set { SetValue(BorderColorProperty, value); }
		}

		public static readonly DependencyProperty SelectedIndexProperty =
			DependencyProperty.Register("SelectedIndex", typeof(int), typeof(SheetTabControl));

		public int SelectedIndex
		{
			get { return (int)GetValue(SelectedIndexProperty); }

			set
			{
				var tabContainer = this.canvas;

				var currentIndex = this.SelectedIndex;

				if (currentIndex >= 0 && currentIndex < tabContainer.Children.Count)
				{
					var tab = tabContainer.Children[currentIndex] as SheetTabItem;
					if (tab != null)
					{
						tab.IsSelected = false;
					}
				}

				SetValue(SelectedIndexProperty, value);
				currentIndex = value;

				if (currentIndex >= 0 && currentIndex < tabContainer.Children.Count)
				{
					var tab = tabContainer.Children[currentIndex] as SheetTabItem;
					if (tab != null)
					{
						tab.IsSelected = true;
					}
				}

				if (this.SelectedIndexChanged != null)
				{
					this.SelectedIndexChanged(this, null);
				}
			}
		}

		public bool NewButtonVisible
		{
			get { return this.newSheetImage.Visibility == Visibility.Visible; }
			set { this.newSheetImage.Visibility = value ? Visibility.Visible : Visibility.Hidden; }
		}

		#endregion // Dependency Properties

		/// <summary>
		/// Determine whether or not allow to move tab by dragging mouse
		/// </summary>
		public bool AllowDragToMove { get; set; }

		#region Tab Management
		public void AddTab(string title)
		{
			int index = this.canvas.Children.Count;
			InsertTab(index, title);
		}

		public void InsertTab(int index, string title)
		{
			var tab = new SheetTabItem(this, title)
			{
				Height = this.canvas.Height,
			};

			this.canvas.Width += tab.Width + 1;
			this.canvas.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(tab.Width + 1) });

			this.canvas.Children.Add(tab);

			Grid.SetColumn(tab, index);

			tab.MouseDown += Tab_MouseDown;

			if (this.canvas.Children.Count == 1)
			{
				tab.IsSelected = true;
			}
		}

		private void Tab_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var index = this.canvas.Children.IndexOf((UIElement)sender);

			var arg = new SheetTabMouseEventArgs()
			{
				Handled = false,
				Location = e.GetPosition(this),
				Index = index,
				MouseButtons = WPFUtility.ConvertToUIMouseButtons(e),
			};

			if (this.TabMouseDown != null)
			{
				this.TabMouseDown(this, arg);
			}

			if (!arg.Handled)
			{
				this.SelectedIndex = index;
			}
		}

		public void RemoveTab(int index)
		{
			var tab = (SheetTabItem)this.canvas.Children[index];

			this.canvas.Children.RemoveAt(index);
			this.canvas.ColumnDefinitions.RemoveAt(index);

			for (int i = index; i < this.canvas.Children.Count; i++)
			{
				Grid.SetColumn(this.canvas.Children[i], i);
			}

			this.canvas.Width -= tab.Width;
		}

		public void UpdateTab(int index, string title, Color backColor, Color textColor)
		{
			SheetTabItem item = this.canvas.Children[index] as SheetTabItem;
			if (item != null)
			{
				item.ChangeTitle(title);
				this.canvas.ColumnDefinitions[index].Width = new GridLength(item.Width+1);

				item.BackColor = backColor;
				item.TextColor = textColor;
			}
		}

		public void ClearTabs()
		{
			this.canvas.Children.Clear();
			this.canvas.ColumnDefinitions.Clear();
			this.canvas.Width = 0;
		}

		public int TabCount { get { return this.canvas.Children.Count; } }

		#endregion // Tab Management

		#region Paint

		private GuidelineSet gls = new GuidelineSet();

		protected override void OnRender(DrawingContext dc)
		{
			base.OnRender(dc);

			var g = dc;

			gls.GuidelinesX.Clear();
			gls.GuidelinesY.Clear();

			gls.GuidelinesX.Add(0.5);
			gls.GuidelinesX.Add(RenderSize.Width + 0.5);
			gls.GuidelinesY.Add(0.5);
			gls.GuidelinesY.Add(RenderSize.Height + 0.5);

			g.PushGuidelineSet(gls);

			var p = new Pen(new SolidColorBrush(this.BorderColor), 1);

			g.DrawLine(p, new Point(0, 0), new Point(this.RenderSize.Width, 0));

			g.Pop();

		}
		#endregion // Paint

		public double TranslateScrollPoint(int p)
		{
			return this.canvas.RenderTransform.Transform(new Point(p, 0)).X;
		}

		public Rect GetItemBounds(int index)
		{
			if (index < 0 || index > this.canvas.Children.Count - 1)
			{
				throw new ArgumentOutOfRangeException("index");
			}

			var tab = this.canvas.Children[index];

			return new Rect(tab.PointToScreen(new Point(0, 0)), this.RenderSize);
		}

		public void MoveItem(int index, int targetIndex)
		{
			if (index < 0 || index > this.canvas.Children.Count - 1)
			{
				throw new ArgumentOutOfRangeException("index");
			}

			var tab = this.canvas.Children[index];

			this.canvas.Children.RemoveAt(index);

			if (targetIndex > index) targetIndex--;

			this.canvas.Children.Insert(targetIndex, tab);
		}

		public void ScrollToItem(int index)
		{
			// TODO!

			//double width = this.ColumnDefinitions[2].ActualWidth;
			//int visibleWidth = this.ClientRectangle.Width - leftPadding - rightPadding;

			//if (rect.Width > visibleWidth || rect.Left < this.viewScroll + leftPadding)
			//{
			//	this.viewScroll = rect.Left - leftPadding;
			//}
			//else if (rect.Right - this.viewScroll > this.ClientRectangle.Right - rightPadding)
			//{
			//	this.viewScroll = rect.Right - this.ClientRectangle.Width + leftPadding;
			//}
		}

		public double ControlWidth { get; set; }

		public event EventHandler<SheetTabMovedEventArgs> TabMoved;

		public event EventHandler SelectedIndexChanged;

		public event EventHandler SplitterMoving;

		public event EventHandler SheetListClick;

		public event EventHandler NewSheetClick;

		public event EventHandler<SheetTabMouseEventArgs> TabMouseDown;
	}

	class SheetTabItem : Decorator
	{
		private SheetTabControl owner;

		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(Boolean), typeof(SheetTabItem));

		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set
			{
				bool currentValue = (bool)GetValue(IsSelectedProperty);

				if (currentValue != value)
				{
					SetValue(IsSelectedProperty, value);
					this.InvalidateVisual();
				}
			}
		}

		public SheetTabItem(SheetTabControl owner, string title)
		{
			this.owner = owner;

			this.SnapsToDevicePixels = true;

			this.ChangeTitle(title);
		}

		public void ChangeTitle(string title)
		{
			var label = new TextBlock
			{
				Text = title,
				VerticalAlignment = System.Windows.VerticalAlignment.Center,
				HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
				Background = Brushes.Transparent,
			};

			label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

			this.Child = label;
			this.Width = label.DesiredSize.Width + 9;
		}

		private GuidelineSet gls = new GuidelineSet();

		public Color BackColor { get; set; }
		public Color TextColor { get; set; }

		protected override void OnRender(DrawingContext drawingContext)
		{
			var g = drawingContext;

			double right = RenderSize.Width;
			double bottom = RenderSize.Height;

			gls.GuidelinesX.Clear();
			gls.GuidelinesY.Clear();

			gls.GuidelinesX.Add(0.5);
			gls.GuidelinesX.Add(right + 0.5);
			gls.GuidelinesY.Add(0.5);
			gls.GuidelinesY.Add(bottom + 0.5);

			g.PushGuidelineSet(gls);

			Brush b = new SolidColorBrush(owner.BorderColor);
			var p = new Pen(b, 1);

			if (IsSelected)
			{
				g.DrawRectangle(
					this.BackColor.A > 0 ? new SolidColorBrush(this.BackColor) : Brushes.White,
					null, new Rect(0, 0, right, bottom));

				g.DrawLine(p, new Point(0, 0), new Point(0, bottom));
				g.DrawLine(p, new Point(right, 0), new Point(right, bottom));

				g.DrawLine(p, new Point(0, bottom), new Point(right, bottom));

				g.DrawLine(new Pen(Brushes.White, 1), new Point(1, 0), new Point(right, 0));
			}
			else
			{
				g.DrawRectangle(
					this.BackColor.A > 0 ? new SolidColorBrush(this.BackColor) : SystemColors.ControlBrush,
					null, new Rect(0, 0, right, bottom));

				int index = this.owner.canvas.Children.IndexOf(this);

				if (index > 0)
				{
					g.DrawLine(new Pen(SystemColors.ControlDarkDarkBrush, 1), new Point(0, 2), new Point(0, bottom - 2));
				}

				// top border
				g.DrawLine(p, new Point(0, 0), new Point(right, 0));
			}

			g.Pop();
		}
	}

	class ArrowBorder : Border
	{
		private SheetTabControl owner;
		
		public ArrowBorder(SheetTabControl owner)
		{
			this.owner = owner;
			
			SnapsToDevicePixels = true;
		}

		private GuidelineSet gls = new GuidelineSet() { GuidelinesY = new DoubleCollection(new double[] { 0.5 }) };

		protected override void OnRender(DrawingContext dc)
		{
			base.OnRender(dc);

			var g = dc;

			g.PushGuidelineSet(this.gls);

			g.DrawLine(new Pen(new SolidColorBrush(this.owner.BorderColor), 1),
				new Point(0, 0), new Point(this.RenderSize.Width, 0));

			g.Pop();
		}
	}

	class RightThumb : FrameworkElement
	{
		private SheetTabControl owner;

		public RightThumb(SheetTabControl owner)
		{
			this.owner = owner;
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			return new Size(5, 0);
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			var g = drawingContext;

			var b = new SolidColorBrush(owner.BorderColor);
			var p = new Pen(b, 1);

			for (double y = 3; y < this.RenderSize.Height - 3; y += 4)
			{
				g.DrawRectangle(SystemColors.ControlDarkBrush, null, new Rect(0, y, 2, 2));
			}

			double right = this.RenderSize.Width;

			GuidelineSet gls = new GuidelineSet();
			gls.GuidelinesX.Add(right + 0.5);
			g.PushGuidelineSet(gls);

			g.DrawLine(p, new Point(right, 0), new Point(right, this.RenderSize.Height));

			g.Pop();
		}
	}
}

#endif // WPF