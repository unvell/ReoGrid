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
 * Author: Jingwood <jingwood at unvell.com>
 *
 * Copyright (c) 2012-2023 Jingwood <jingwood at unvell.com>
 * Copyright (c) 2012-2023 unvell inc. All rights reserved.
 * 
 ****************************************************************************/

#if AVALONIA

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using System;
using System.Linq;
using unvell.ReoGrid.Main;
using Brushes = Avalonia.Media.Brushes;
using Color = Avalonia.Media.Color;
using DrawingContext = Avalonia.Media.DrawingContext;
using Pen = Avalonia.Media.Pen;
using Point = Avalonia.Point;
using Size = Avalonia.Size;

namespace unvell.ReoGrid.AvaloniaPlatform
{

    internal class SheetTabControl : TemplatedControl, ISheetTabControl
    {
        internal DockPanel canvas = new DockPanel()
        {
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Left,
            LastChildFill = false,
        };
        Bitmap imageSource = new Bitmap(new System.IO.MemoryStream(unvell.ReoGrid.Properties.Resources.NewBuildDefinition_8952_inactive_png));
        Bitmap imageHoverSource = new Bitmap(new System.IO.MemoryStream(unvell.ReoGrid.Properties.Resources.NewBuildDefinition_8952_png));

        public SheetTabControl()
        {

            this.BorderColor = Colors.DeepSkyBlue;
            this.Background = new SolidColorBrush(SystemColors.ControlLight);

            this.Template = new FuncControlTemplate<SheetTabControl>((p, ns) => 
                new Grid()
                {
                    Name = "root",
                    ColumnDefinitions = new("20,20,*,auto,auto"),
                    [!BackgroundProperty] = new TemplateBinding(BackgroundProperty),
                    Children =
                {
                    new Border()
                    {
                        Name="pleft",
                        [Grid.ColumnProperty] = 0,
                        BorderBrush = new SolidColorBrush(BorderColor),
                        //BorderThickness = new Thickness(0, 1, 0, 0),
                        Child = new Polygon()
                        {
                            Points =
                                new Point[]{
                                    new Point(6, 0),
                                    new Point(0, 5),
                                    new Point(6, 10),
                                },
                            Fill = new SolidColorBrush(SystemColors.ControlDarkDark),
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(4, 0, 0, 0),
                        },
                        Background = new SolidColorBrush(SystemColors.Control),
                    }.RegisterName(ns),
                    new Border()
                    {
                        Name="pright",
                        [Grid.ColumnProperty] = 1,
                        BorderBrush = new SolidColorBrush(BorderColor),
                        //BorderThickness = new Thickness(0, 1, 0, 0),
                        Child = new Polygon()
                        {
                            Points =
                                new Point[] {
                                    new Point(0, 0),
                                    new Point(6, 5),
                                    new Point(0, 10),
                                },
                            Fill = new SolidColorBrush(SystemColors.ControlDarkDark),
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(0, 0, 4, 0),
                        },
                        Background = new SolidColorBrush(SystemColors.Control),
                    }.RegisterName(ns),
                    new ScrollViewer()
                    {
                        [Grid.ColumnProperty] = 2,
                        Margin = new Thickness(0,0,0,0),
                        HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Hidden,
                        VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Hidden,
                        Content = this.canvas
                    }.RegisterName(ns),
                    new Border()
                    {
                        Name="newSheetImage",
                        [Grid.ColumnProperty] = 3,
                        [!IsVisibleProperty] = new TemplateBinding(NewButtonVisibleProperty),
                        Width = 30,
                        BorderBrush = new SolidColorBrush(BorderColor),
                        //BorderThickness = new Thickness(0, 1, 0, 0),
                        Child = new Avalonia.Controls.Image()
                        {
                            Source = imageSource,
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                            Margin = new Thickness(2),
                            Cursor = new Avalonia.Input.Cursor(StandardCursorType.Hand),
                        },
                        Background = new SolidColorBrush(SystemColors.Control),
                    }.RegisterName(ns),
                    new Border
                    {
                        Name="rightThumb",
                        [Grid.ColumnProperty] = 4,
                        Width = 5,
                        BorderBrush = new SolidColorBrush(BorderColor),
                        //BorderThickness = new Thickness(0, 1, 0, 0),
                        Child = new RightThumb(this),
                        Cursor = new Cursor(StandardCursorType.SizeWestEast),
                        Background = new SolidColorBrush(SystemColors.Control),
                        HorizontalAlignment = HorizontalAlignment.Center,
                    }.RegisterName(ns)
                }
                }.RegisterName(ns)
            );


            this.SizeChanged += OnSizeChanged;

        }

        private IPointer? Pointer;

        private bool splitterMoving = false;
        private bool scrollLeftDown = false, scrollRightDown = false;

        private Avalonia.Threading.DispatcherTimer scrollTimer;

        protected void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Clip = new RectangleGeometry(new Rect(0, 0, e.NewSize.Width, e.NewSize.Height));

            this.canvas.Height = this.Bounds.Height;
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            var grid = e.NameScope.Find<Grid>("root");
            var pleft = e.NameScope.Find<Control>("pleft");
            var pright = e.NameScope.Find<Control>("pright");
            var newSheetImage = e.NameScope.Find<Border>("newSheetImage");
            var rightThumb = e.NameScope.Find<Border>("rightThumb");





            newSheetImage.PointerEntered += (s, e) => (newSheetImage.Child as Image).Source = imageHoverSource;
            newSheetImage.PointerExited += (s, e) => (newSheetImage.Child as Image).Source = imageSource;
            newSheetImage.PointerPressed += (s, e) =>
            {
                if (this.NewSheetClick != null)
                {
                    this.NewSheetClick(this, null);
                }
            };
            newSheetImage.ZIndex = 100;


            this.scrollTimer = new Avalonia.Threading.DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(10)
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
                    double max = grid.ColumnDefinitions[2].ActualWidth - this.canvas.Bounds.Width;

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

            pleft.PointerPressed += (s, e) =>
            {
                this.scrollRightDown = false;
                if (e.Pointer.Type == PointerType.Mouse)
                {
                    var properties = e.GetCurrentPoint(this).Properties;
                    if (properties.IsLeftButtonPressed)
                    {
                        this.scrollLeftDown = true;
                        this.scrollTimer.IsEnabled = true;
                    }
                    else if (properties.IsRightButtonPressed)
                    {
                        if (this.SheetListClick != null)
                        {
                            this.SheetListClick(this, null);
                        }
                    }
                }
            };
            pleft.PointerReleased += (s, e) =>
            {
                this.scrollTimer.IsEnabled = false;
                this.scrollLeftDown = false;
            };

            pright.PointerPressed += (s, e) =>
            {
                this.scrollLeftDown = false;
                if (e.Pointer.Type == PointerType.Mouse)
                {
                    var properties = e.GetCurrentPoint(this).Properties;
                    if (properties.IsLeftButtonPressed)
                    {
                        this.scrollRightDown = true;
                        this.scrollTimer.IsEnabled = true;
                    }
                    else if (properties.IsRightButtonPressed)
                    {
                        if (this.SheetListClick != null)
                        {
                            this.SheetListClick(this, null);
                        }
                    }
                }
            };
            pright.PointerReleased += (s, e) =>
            {
                this.scrollTimer.IsEnabled = false;
                this.scrollRightDown = false;
            };

            rightThumb.PointerPressed += (s, e) =>
            {
                this.splitterMoving = true;
                this.Pointer = e.Pointer;
                this.Pointer.Capture(rightThumb);
            };
            rightThumb.PointerMoved += (s, e) =>
            {
                if (this.splitterMoving)
                {
                    if (this.SplitterMoving != null)
                    {
                        this.SplitterMoving(this, e);
                    }
                }
            };
            rightThumb.PointerReleased += (s, e) =>
            {
                this.splitterMoving = false;
                this.Pointer.Capture(null);
            };



        }


        #region Dependency Properties

        public static readonly StyledProperty<Color> SelectedBackColorProperty =
            AvaloniaProperty.Register<SheetTabControl, Color>("SelectedBackColor");

        public Color SelectedBackColor
        {
            get { return (Color)GetValue(SelectedBackColorProperty); }
            set { SetValue(SelectedBackColorProperty, value); }
        }

        public static readonly StyledProperty<Color> SelectedTextColorProperty =
            AvaloniaProperty.Register<SheetTabControl, Color>("SelectedTextColor");

        public Color SelectedTextColor
        {
            get { return (Color)GetValue(SelectedTextColorProperty); }
            set { SetValue(SelectedTextColorProperty, value); }
        }

        public static readonly StyledProperty<Color> BorderColorProperty =
            AvaloniaProperty.Register<SheetTabControl, Color>("BorderColor");

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public static readonly StyledProperty<int> SelectedIndexProperty =
            AvaloniaProperty.Register<SheetTabControl, int>("SelectedIndex");

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

        /// <summary>
        /// NewButtonVisible StyledProperty definition
        /// </summary>
        public static readonly StyledProperty<bool> NewButtonVisibleProperty =
            AvaloniaProperty.Register<SheetTabControl, bool>(nameof(NewButtonVisible), true);

        public bool NewButtonVisible
        {
            get { return GetValue(NewButtonVisibleProperty); }
            set { SetValue(NewButtonVisibleProperty, value); }
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

            this.canvas.Children.Add(tab);

            tab.PointerPressed += Tab_MouseDown;

            if (this.canvas.Children.Count == 1)
            {
                tab.IsSelected = true;
            }
        }

        private void Tab_MouseDown(object sender, PointerPressedEventArgs e)
        {
            var index = this.canvas.Children.IndexOf((Control)sender);

            var arg = new SheetTabMouseEventArgs()
            {
                Handled = false,
                Location = e.GetPosition(this),
                Index = index,
                MouseButtons = AvaloniaUtility.ConvertToUIMouseButtons(e),
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
            this.canvas.Children.RemoveAt(index);
        }

        public void UpdateTab(int index, string title, Color backColor, Color textColor)
        {
            SheetTabItem item = this.canvas.Children[index] as SheetTabItem;
            if (item != null)
            {
                item.ChangeTitle(title);
                item.BackColor = backColor;
                item.TextColor = textColor;
            }
        }

        public void ClearTabs()
        {
            this.canvas.Children.Clear();
        }

        public int TabCount { get { return this.canvas.Children.Count; } }

        #endregion // Tab Management

        #region Paint

        //private GuidelineSet gls = new GuidelineSet();


        public override void Render(DrawingContext dc)
        {

            var g = dc;

            //gls.GuidelinesX.Clear();
            //gls.GuidelinesY.Clear();

            //gls.GuidelinesX.Add(0.5);
            //gls.GuidelinesX.Add(Bounds.Size.Width + 0.5);
            //gls.GuidelinesY.Add(0.5);
            //gls.GuidelinesY.Add(Bounds.Size.Height + 0.5);

            //g.PushGuidelineSet(gls);

            var p = new Pen(new SolidColorBrush(this.BorderColor), 3);

            g.DrawLine(p, new Point(0, 0), new Point(this.Bounds.Size.Width, 0));

            //g.Pop();

        }
        #endregion // Paint

        //public double TranslateScrollPoint(int p)
        //{
        //	return this.canvas.RenderTransform.Transform(new Point(p, 0)).X;
        //}

        public Rect GetItemBounds(int index)
        {
            if (index < 0 || index > this.canvas.Children.Count - 1)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            var tab = this.canvas.Children[index];

            var pos = tab.PointToScreen(new Point(0, 0));
            return new Rect(new Point(pos.X, pos.Y), this.Bounds.Size);
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

    class SheetTabItem : ContentControl
    {
        private SheetTabControl owner;
        private Border separator;
        public static readonly StyledProperty<Boolean> IsSelectedProperty =
            AvaloniaProperty.Register<SheetTabItem, Boolean>("IsSelected");

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set
            {
                SetValue(IsSelectedProperty, value);
                SetColor();
                SetSepatator();
            }
        }

        private void SetColor()
        {
            if (IsSelected)
            {
                this.Background = this.BackColor.A > 0
                    ? new SolidColorBrush(this.BackColor)
                    : Brushes.White;

                this.BorderBrush = new SolidColorBrush(owner.BorderColor);
                this.BorderThickness = new Thickness(0, 0, 0, 2);

                //this.Margin = new Thickness(0, -1, 0, 0);

            }
            else
            {
                this.Background = this.BackColor.A > 0
                    ? new SolidColorBrush(this.BackColor)
                    : new SolidColorBrush(SystemColors.Control);

                this.BorderBrush = Brushes.Transparent;
                this.BorderThickness = new Thickness(0, 0, 0, 0);

                //this.Margin = new Thickness(0, 0, 0, 0);
            }
        }

        private void SetSepatator()
        {
            int index = this.owner.canvas.Children.IndexOf(this);
            if (index == 0)
                separator.Background = Brushes.Transparent;
        }

        public SheetTabItem(SheetTabControl owner, string title)
        {
            this.owner = owner;
            this.ChangeTitle(title);
            this.SetColor();
            this.separator = new Border()
            {
                Name = "separator",
                Background = Brushes.DarkGray,
                Width = 2,
                HorizontalAlignment = HorizontalAlignment.Center,
                Height = 12,
                IsVisible = true
            };
            this.Template = new FuncControlTemplate((_, ns) => new DockPanel()
            {
                Children =
                {
                    separator,
                    new Border()
                    {
                        [!BackgroundProperty] = new TemplateBinding(BackgroundProperty),
                        [!BorderBrushProperty] = new TemplateBinding(BorderBrushProperty),
                        [!BorderThicknessProperty] = new TemplateBinding(BorderThicknessProperty),
                        Child = new ContentPresenter()
                        {
                            Margin = new Thickness(4,0),
                            [!ContentPresenter.ContentProperty] = new TemplateBinding(ContentControl.ContentProperty),
                            //Foreground = new SolidColorBrush(TextColor)
                        }

                    }
                }
            });
        }

        public void ChangeTitle(string title)
        {
            this.Content = title;
        }

        public Color BackColor { get; set; }
        public Color TextColor { get; set; }
    }

    class RightThumb : Control
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

        public override void Render(DrawingContext drawingContext)
        {
            var g = drawingContext;

            var b = new SolidColorBrush(owner.BorderColor);
            var p = new Avalonia.Media.Pen(b, 1);

            for (double y = 3; y < this.Bounds.Size.Height - 3; y += 4)
            {
                g.DrawRectangle(new SolidColorBrush(SystemColors.ControlDark), null, new Rect(0, y, 2, 2));
            }
        }
    }
}

#endif // WPF