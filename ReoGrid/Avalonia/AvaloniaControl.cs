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
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Interaction;
using unvell.ReoGrid.Main;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Views;
using unvell.ReoGrid.AvaloniaPlatform;
using HorizontalAlignment = Avalonia.Layout.HorizontalAlignment;
using Point = unvell.ReoGrid.Graphics.Point;

namespace unvell.ReoGrid
{
    /// <summary>
    /// ReoGrid Spreadsheet Control
    /// </summary>
    public partial class ReoGridControl : Decorator, IVisualWorkbook,
    IRangePickableControl, IContextMenuControl, IPersistenceWorkbook, IActionControl, IWorkbook
    {
        internal const int ScrollBarSize = 18;

        private ReoGridAvaloniaControlAdapter adapter;
        //private Grid bottomGrid;
        private SheetTabControl sheetTab;
        private InputTextBox editTextbox;

        private ScrollBar horScrollbar;
        private ScrollBar verScrollbar;

        //private Canvas canvas;

        //private DockPanel layout;

        /// <summary>
        /// Create ReoGrid spreadsheet control
        /// </summary>
        public ReoGridControl()
        {
            //this.SnapsToDevicePixels = true;
            this.Focusable = true;
            //this.FocusVisualStyle = null;

            this.BeginInit();
            //this.canvas = new Canvas();

            //layout = new DockPanel();

            //this.bottomGrid = new DockPanel() { Height = ScrollBarSize };

            //this.bottomGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(ScrollBarWidth) });

            this.sheetTab = new SheetTabControl()
            {
                ControlWidth = 400,
            };

            this.horScrollbar = new ScrollBar()
            {
                Orientation = Orientation.Horizontal,
                Height = ScrollBarSize,
                SmallChange = Worksheet.InitDefaultColumnWidth,
            };

            this.verScrollbar = new ScrollBar()
            {
                Orientation = Orientation.Vertical,
                Width = ScrollBarSize,
                SmallChange = Worksheet.InitDefaultRowHeight,
            };
            this.Child = new Canvas();
            var canvas = Child as Canvas;

            canvas.Children.Add(this.sheetTab);
            canvas.Children.Add(this.horScrollbar);

            Grid.SetColumn(this.horScrollbar, 1);

            //this.Children.Add(this.bottomGrid);
            canvas.Children.Add(this.verScrollbar);

            this.horScrollbar.Scroll += (s, e) =>
            {
                if (this.currentWorksheet.ViewportController is IScrollableViewportController)
                {
                    ((IScrollableViewportController)this.currentWorksheet.ViewportController).HorizontalScroll(e.NewValue);
                }
            };

            this.verScrollbar.Scroll += (s, e) =>
            {
                if (this.currentWorksheet.ViewportController is IScrollableViewportController)
                {
                    ((IScrollableViewportController)this.currentWorksheet.ViewportController).VerticalScroll(e.NewValue);
                }
            };

            this.sheetTab.SplitterMoving += (s, e) =>
            {
                var arg = e as PointerEventArgs;
   
                double width = arg.GetPosition(this).X + 3;
                if (width < 75) width = 75;
                if (width > this.Bounds.Size.Width - ScrollBarSize) width = this.Bounds.Size.Width - ScrollBarSize;

                this.SheetTabWidth = width;

                this.UpdateSheetTabAndScrollBarsLayout();
                //this.bottomGrid.ColumnDefinitions[0].Width = new GridLength(width);

                //double newScrollWidth = this.Bounds.Size.Width
                //	- this.bottomGrid.ColumnDefinitions[0].Width - this.bottomGrid.ColumnDefinitions[2].Width;

                //if (newScrollWidth < 0) newScrollWidth = 0;

                //this.bottomGrid.ColumnDefinitions[1].Width = new GridLength(newScrollWidth);
                //this.horScrollbar.Width = this.bottomGrid.ColumnDefinitions[1].Width;
            };

            this.InitControl();

            this.editTextbox = new InputTextBox()
            {
                Owner = this,
                Padding = new Thickness(0),
                Margin = new Thickness(0),
                [ScrollViewer.HorizontalScrollBarVisibilityProperty] = ScrollBarVisibility.Hidden,
                [ScrollViewer.VerticalScrollBarVisibilityProperty] = ScrollBarVisibility.Hidden,
            };

            canvas.Children.Add(editTextbox);

            this.adapter = new ReoGridAvaloniaControlAdapter(this);
            this.adapter.editTextbox = this.editTextbox;

            InitWorkbook(this.adapter);

            //TextCompositionManager.AddPreviewTextInputHandler(this, OnTextInputStart);

            this.EndInit();

            this.renderer = new AvaloniaRenderer();

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (!string.IsNullOrEmpty(this.LoadFromFile))
                {
                    var file = new System.IO.FileInfo(this.LoadFromFile);
                    this.currentWorksheet.Load(file.FullName);
                }
            }, Avalonia.Threading.DispatcherPriority.Input);

            this.SizeChanged += ReoGridControl_SizeChanged;
            this.AddHandler(PointerPressedEvent, MouseDownHandler, handledEventsToo: true);
            this.AddHandler(PointerReleasedEvent, MouseUpHandler, handledEventsToo: true);
            PointerMoved += OnMouseMove;
            PointerWheelChanged += OnMouseWheel;

        }


        private void MouseUpHandler(object sender, PointerReleasedEventArgs e)
        {
            this.OnWorksheetMouseUp(e.GetPosition(this), AvaloniaUtility.ConvertToUIMouseButtons(e.InitialPressMouseButton));

            //if (mouseCaptured) ReleaseMouseCapture();
        }

        private void MouseDownHandler(object sender, PointerPressedEventArgs e)
        {
            Focus();

            var pos = e.GetPosition(this);

            double right = this.Bounds.Size.Width;
            double bottom = this.Bounds.Size.Height;

            if (this.verScrollbar.IsVisible)
            {
                right = Canvas.GetLeft(this.verScrollbar);
            }

            if (this.sheetTab.IsVisible)
            {
                bottom = Canvas.GetTop(this.sheetTab);
            }
            else if (this.horScrollbar.IsVisible)
            {
                bottom = Canvas.GetTop(this.horScrollbar);
            }

            if (pos.X < right && pos.Y < bottom)
            {
                if (e.ClickCount == 2)
                {
                    this.currentWorksheet.OnMouseDoubleClick(e.GetPosition(this), AvaloniaUtility.ConvertToUIMouseButtons(e));
                }
                else
                {
                    this.OnWorksheetMouseDown(e.GetPosition(this), AvaloniaUtility.ConvertToUIMouseButtons(e));
                    //if (CaptureMouse()) mouseCaptured = true;
                }
            }
        }

        private void ReoGridControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            if (this.IsVisible)
            {
                if (e.PreviousSize.Width > 0)
                {
                    this.SheetTabWidth += e.NewSize.Width - e.PreviousSize.Width;
                    if (this.SheetTabWidth < 0) this.SheetTabWidth = 0;
                }
            }

            this.UpdateSheetTabAndScrollBarsLayout();

            this.InvalidateVisual();
        }

        #region SheetTab & Scroll Bars Visibility


        private void SetHorizontalScrollBarSize()
        {
            double hsbWidth = this.Bounds.Width;

            if (this.sheetTab.IsVisible)
            {
                hsbWidth -= this.SheetTabWidth;
            }

            if (this.verScrollbar.IsVisible)
            {
                hsbWidth -= ScrollBarSize;
            }

            if (hsbWidth < 0) hsbWidth = 0;
            this.horScrollbar.Width = hsbWidth;
        }

        private void SetSheetTabSize()
        {
            double stWidth = 0;

            if (this.horScrollbar.IsVisible)
            {
                stWidth = this.SheetTabWidth;
            }
            else
            {
                stWidth = this.Width;
            }

            if (this.verScrollbar.IsVisible)
            {
                stWidth -= ScrollBarSize;
            }

            if (stWidth < 0) stWidth = 0;
            this.horScrollbar.Width = stWidth;
        }

        private void UpdateSheetTabAndScrollBarsLayout()
        {
            Canvas.SetTop(this.sheetTab, this.Bounds.Size.Height - ScrollBarSize);
            Canvas.SetTop(this.horScrollbar, this.Bounds.Size.Height - ScrollBarSize);

            this.sheetTab.Height = ScrollBarSize;
            this.horScrollbar.Height = ScrollBarSize;

            Canvas.SetLeft(verScrollbar, this.Bounds.Size.Width - ScrollBarSize);

            var vsbHeight = this.Bounds.Size.Height - ScrollBarSize;
            if (vsbHeight < 0) vsbHeight = 0;
            verScrollbar.Height = vsbHeight;

            if (this.sheetTab.IsVisible
                && this.horScrollbar.IsVisible)
            {
                this.sheetTab.Width = this.SheetTabWidth;

                Canvas.SetLeft(this.horScrollbar, this.SheetTabWidth);
                SetHorizontalScrollBarSize();
            }
            else if (this.sheetTab.IsVisible)
            {
                this.sheetTab.Width = this.Width;
            }
            else if (this.horScrollbar.IsVisible)
            {
                Canvas.SetLeft(this.horScrollbar, 0);
                SetHorizontalScrollBarSize();
            }
            else
            {
                this.verScrollbar.Height = this.Bounds.Size.Height;
            }

            this.currentWorksheet.UpdateViewportControllBounds();
        }

        private void ShowSheetTabControl()
        {
            if (!this.sheetTab.IsVisible)
            {
                this.sheetTab.IsVisible = true;
                this.UpdateSheetTabAndScrollBarsLayout();
            }
        }

        private void HideSheetTabControl()
        {
            if (this.sheetTab.IsVisible)
            {
                this.sheetTab.IsVisible = false;
                this.UpdateSheetTabAndScrollBarsLayout();
            }
        }

        private void ShowHorScrollBar()
        {
            if (!this.horScrollbar.IsVisible)
            {
                this.horScrollbar.IsVisible = true;
                this.UpdateSheetTabAndScrollBarsLayout();
            }
        }

        private void HideHorScrollBar()
        {
            if (this.horScrollbar.IsVisible)
            {
                this.horScrollbar.IsVisible = false;
                this.UpdateSheetTabAndScrollBarsLayout();
            }
        }

        private void ShowVerScrollBar()
        {
            if (!this.verScrollbar.IsVisible)
            {
                this.verScrollbar.IsVisible = true;
                this.UpdateSheetTabAndScrollBarsLayout();
            }
        }

        private void HideVerScrollBar()
        {
            if (this.verScrollbar.IsVisible)
            {
                this.verScrollbar.IsVisible = false;
                this.UpdateSheetTabAndScrollBarsLayout();
            }
        }

        #endregion // SheetTab & Scroll Bars Visibility

        #region Render

        /// <summary>
        /// Handle repaint event to draw component.
        /// </summary>
        /// <param name="dc">Platform independence drawing context.</param>
        public override void Render(Avalonia.Media.DrawingContext dc)
        {
#if DEBUG
            Stopwatch watch = Stopwatch.StartNew();
#endif

            using var ds = dc.PushTransform(Matrix.CreateTranslation(0.5, 0.5));
            if (this.currentWorksheet != null
                && this.currentWorksheet.workbook != null
                && this.currentWorksheet.controlAdapter != null)
            {

                SolidColorBrush bgBrush;
                if (this.controlStyle.TryGetColor(ControlAppearanceColors.GridBackground, out SolidColor bgColor))
                {
                    bgBrush = new SolidColorBrush(bgColor);
                }
                else
                {
                    bgBrush = new SolidColorBrush(Colors.White);
                }

                dc.DrawRectangle(bgBrush, null, new Rect(0, 0, this.Bounds.Size.Width, this.Bounds.Size.Height));

                this.renderer.Reset();

                ((AvaloniaRenderer)this.renderer).SetPlatformGraphics(dc);

                var rgdc = new CellDrawingContext(this.currentWorksheet, DrawMode.View, this.renderer);
                this.currentWorksheet.ViewportController.Draw(rgdc);

            }

#if DEBUG

            watch.Stop();
            long ms = watch.ElapsedMilliseconds;
            if (ms > 30)
            {
                Debug.WriteLine(string.Format("end draw: {0} ms.", watch.ElapsedMilliseconds));
            }
#endif
            base.Render(dc);
        }

        #endregion // Render

        #region Mouse

        bool mouseCaptured = false;

        protected void OnMouseMove(object sender, PointerEventArgs e)
        {
            var point = e.GetCurrentPoint(this);
            this.OnWorksheetMouseMove(e.GetPosition(this), AvaloniaUtility.ConvertToUIMouseButtons(point.Properties));
        }


        protected void OnMouseWheel(object sender, PointerWheelEventArgs e)
        {
            this.currentWorksheet.OnMouseWheel(e.GetPosition(this), (int)e.Delta.Length, AvaloniaUtility.ConvertToUIMouseButtons(MouseButton.Middle));
        }

        #endregion // Mouse

        #region Keyboard

        /// <summary>
        /// Handle event when key down.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!this.currentWorksheet.IsEditing)
            {
                var wfkeys = (KeyCode)Avalonia.Win32.Input.KeyInterop.VirtualKeyFromKey(e.Key);

                if ((e.KeyModifiers & KeyModifiers.Control) == KeyModifiers.Control)
                {
                    wfkeys |= KeyCode.Control;
                }
                else if ((e.KeyModifiers & KeyModifiers.Shift) == KeyModifiers.Shift)
                {
                    wfkeys |= KeyCode.Shift;
                }
                else if ((e.KeyModifiers & KeyModifiers.Alt) == KeyModifiers.Alt)
                {
                    wfkeys |= KeyCode.Alt;
                }

                if (wfkeys != KeyCode.Control
                    && wfkeys != KeyCode.Shift
                    && wfkeys != KeyCode.Alt)
                {
                    if (this.currentWorksheet.OnKeyDown(wfkeys))
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (!this.currentWorksheet.IsEditing)
            {
                var wfkeys = (KeyCode)Avalonia.Win32.Input.KeyInterop.VirtualKeyFromKey(e.Key);

                if ((e.KeyModifiers & KeyModifiers.Control) == KeyModifiers.Control)
                {
                    wfkeys |= KeyCode.Control;
                }
                else if ((e.KeyModifiers & KeyModifiers.Shift) == KeyModifiers.Shift)
                {
                    wfkeys |= KeyCode.Shift;
                }
                else if ((e.KeyModifiers & KeyModifiers.Alt) == KeyModifiers.Alt)
                {
                    wfkeys |= KeyCode.Alt;
                }

                if (wfkeys != KeyCode.Control
                    && wfkeys != KeyCode.Shift
                    && wfkeys != KeyCode.Alt)
                {
                    if (this.currentWorksheet.OnKeyUp(wfkeys))
                    {
                        e.Handled = true;
                    }
                }

                //base.OnKeyUp(e);
            }
        }

        /// <summary>
        /// Handle event when text inputted
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);
        }

        private void OnTextInputStart(object sender, TextInputEventArgs args)
        {
            if (!this.currentWorksheet.IsEditing)
            {
                this.currentWorksheet.StartEdit();
                this.currentWorksheet.CellEditText = string.Empty;
            }
        }

        #endregion // Keyboard

        #region Adapter
        internal class ReoGridAvaloniaControlAdapter : IControlAdapter
        {
            #region Constructor
            private readonly ReoGridControl canvas;
            internal InputTextBox editTextbox;

            internal ReoGridAvaloniaControlAdapter(ReoGridControl canvas)
            {
                this.canvas = canvas;
            }
            #endregion // Constructor

            #region IControlAdapter Members

            public IVisualWorkbook ControlInstance
            {
                get { return this.canvas; }
            }

            public ControlAppearanceStyle ControlStyle { get { return this.canvas.controlStyle; } }

            public IRenderer Renderer { get { return this.canvas.renderer; } }

            public void ShowContextMenuStrip(ViewTypes viewType, Graphics.Point containerLocation)
            {
                switch (viewType)
                {
                    default:
                    case ViewTypes.Cells:
                        this.canvas.BaseContextMenu = this.canvas.CellsContextMenu;
                        break;

                    case ViewTypes.ColumnHeader:
                        this.canvas.BaseContextMenu = this.canvas.ColumnHeaderContextMenu;
                        break;

                    case ViewTypes.RowHeader:
                        this.canvas.BaseContextMenu = this.canvas.RowHeaderContextMenu;
                        break;

                    case ViewTypes.LeadHeader:
                        this.canvas.BaseContextMenu = this.canvas.LeadHeaderContextMenu;
                        break;
                }
            }

            private Cursor oldCursor = null;

            public void ChangeCursor(CursorStyle cursor)
            {
                oldCursor = this.canvas.Cursor;

                this.canvas.Cursor = cursor switch
                {
                    CursorStyle.PlatformDefault => new Cursor(StandardCursorType.Arrow),
                    CursorStyle.Hand => new Cursor(StandardCursorType.Hand),
                    CursorStyle.Selection => this.canvas.internalCurrentCursor,
                    CursorStyle.FullRowSelect => this.canvas.builtInFullRowSelectCursor,
                    CursorStyle.FullColumnSelect => this.canvas.builtInFullColSelectCursor,
                    CursorStyle.EntireSheet => this.canvas.builtInEntireSheetSelectCursor,
                    CursorStyle.Move => new Cursor(StandardCursorType.SizeAll),
                    //CursorStyle.Copy => throw new NotImplementedException(),
                    CursorStyle.ChangeColumnWidth => new Cursor(StandardCursorType.SizeWestEast),
                    CursorStyle.ChangeRowHeight => new Cursor(StandardCursorType.SizeNorthSouth),
                    CursorStyle.ResizeHorizontal => new Cursor(StandardCursorType.SizeWestEast),
                    CursorStyle.ResizeVertical => new Cursor(StandardCursorType.SizeNorthSouth),
                    CursorStyle.Busy => new Cursor(StandardCursorType.AppStarting),
                    CursorStyle.Cross => this.canvas.builtInCrossCursor,
                    _ => new Cursor(StandardCursorType.Arrow)
                };
            }

            public void RestoreCursor()
            {
                this.canvas.Cursor = oldCursor;
            }

            public void ChangeSelectionCursor(CursorStyle cursor)
            {
                switch (cursor)
                {
                    default:
                    case CursorStyle.PlatformDefault:
                        this.canvas.internalCurrentCursor = new Cursor(StandardCursorType.Arrow);
                        break;

                    case CursorStyle.Hand:
                        this.canvas.internalCurrentCursor = new Cursor(StandardCursorType.Hand);
                        break;
                }
            }

            public Rectangle GetContainerBounds()
            {
                double w = this.canvas.Bounds.Width;
                double h = this.canvas.Bounds.Height + 1;

                if (this.canvas.verScrollbar.IsVisible)
                {
                    w -= ScrollBarSize;
                }

                if (this.canvas.sheetTab.IsVisible
                    || this.canvas.horScrollbar.IsVisible)
                {
                    h -= ScrollBarSize;
                }

                if (w < 0) w = 0;
                if (h < 0) h = 0;

                return new Rectangle(0, 0, w, h);
            }

            public void Focus()
            {
                this.canvas.Focus();
            }

            public void Invalidate()
            {
                this.canvas.InvalidateVisual();
            }

            //public void ChangeBackColor(Color color)
            //{
            //    ((Canvas)this.canvas.Child).Background = new SolidColorBrush(color);
            //}

            public bool IsVisible
            {
                get { return this.canvas.IsVisible; }
            }

            public Graphics.Point PointToScreen(Graphics.Point p)
            {
                var pixelPoint = this.canvas.PointToScreen(p);
                return new Point(pixelPoint.X, pixelPoint.Y);
            }

            public IGraphics PlatformGraphics { get { return null; } }

            public void ChangeBackgroundColor(SolidColor color)
            {
            }

            public void ShowTooltip(Graphics.Point point, string content)
            {
                // not implemented
            }

            public ISheetTabControl SheetTabControl
            {
                get { return this.canvas.sheetTab; }
            }

            public double BaseScale { get { return 0f; } }
            public double MinScale { get { return 0.1f; } }
            public double MaxScale { get { return 4f; } }

            #endregion // IControlAdapter Members

            #region IEditableControlInterface Members

            public void ShowEditControl(Graphics.Rectangle bounds, Cell cell)
            {
                var sheet = this.canvas.CurrentWorksheet;

                Color textColor;

                if (!cell.RenderColor.IsTransparent)
                {
                    textColor = cell.RenderColor;
                }
                else if (cell.InnerStyle.HasStyle(PlainStyleFlag.TextColor))
                {
                    // cell text color, specified by SetRangeStyle
                    textColor = cell.InnerStyle.TextColor;
                }
                else
                {
                    // default cell text color
                    textColor = this.canvas.controlStyle[ControlAppearanceColors.GridText];
                }

                Canvas.SetLeft(this.editTextbox, bounds.X);
                Canvas.SetTop(this.editTextbox, bounds.Y);

                this.editTextbox.Width = bounds.Width - 1;
                this.editTextbox.Height = bounds.Height - 1;

                this.editTextbox.CellSize = cell.Bounds.Size;
                this.editTextbox.VAlign = cell.InnerStyle.VAlign;
                this.editTextbox.FontFamily = new FontFamily(cell.InnerStyle.FontName);
                this.editTextbox.FontSize = cell.InnerStyle.FontSize * sheet.ScaleFactor * 96f / 72f;
                this.editTextbox.FontStyle = PlatformUtility.ToAvaloniaFontStyle(cell.InnerStyle.fontStyles);
                this.editTextbox.Foreground = this.Renderer.GetBrush(textColor);
                this.editTextbox.Background = this.Renderer.GetBrush(cell.InnerStyle.HasStyle(PlainStyleFlag.BackColor)
                    ? cell.InnerStyle.BackColor : this.canvas.controlStyle[ControlAppearanceColors.GridBackground]);
                this.editTextbox.SelectionStart = this.editTextbox.Text.Length;
                this.editTextbox.TextWrap = cell.InnerStyle.TextWrapMode != TextWrapMode.NoWrap;
                this.editTextbox.TextWrapping = (cell.InnerStyle.TextWrapMode == TextWrapMode.NoWrap)
                    ? TextWrapping.NoWrap : TextWrapping.Wrap;

                this.editTextbox.IsVisible = true;
                this.editTextbox.Focus();
            }

            public void HideEditControl()
            {
                this.editTextbox.IsVisible = false;
            }

            public void SetEditControlText(string text)
            {
                this.editTextbox.Text = text;
            }

            public string GetEditControlText()
            {
                return this.editTextbox.Text;
            }

            public void EditControlSelectAll()
            {
                this.editTextbox.SelectAll();
            }

            public void SetEditControlCaretPos(int pos)
            {
                this.editTextbox.SelectionStart = pos;
            }

            public int GetEditControlCaretPos()
            {
                return this.editTextbox.SelectionStart;
            }

            public int GetEditControlCaretLine()
            {
                return this.editTextbox.CaretIndex;
                //this.editTextbox.TextLayout.GetLineIndexFromCharacterIndex(this.editTextbox.SelectionStart,false);
            }

            public void SetEditControlAlignment(ReoGridHorAlign align)
            {
                switch (align)
                {
                    default:
                    case ReoGridHorAlign.Left:
                        this.editTextbox.HorizontalAlignment = HorizontalAlignment.Left;
                        break;

                    case ReoGridHorAlign.Center:
                    case ReoGridHorAlign.DistributedIndent:
                        this.editTextbox.HorizontalAlignment = HorizontalAlignment.Center;
                        break;

                    case ReoGridHorAlign.Right:
                        this.editTextbox.HorizontalAlignment = HorizontalAlignment.Right;
                        break;
                }
            }

            public void EditControlApplySystemMouseDown()
            {
                //Point p = System.Windows.Input.Mouse.GetPosition(this.editTextbox);

                //p.X += 2; // fix 2 pixels (borders of left and right)
                //p.Y -= 1; // fix 1 pixels (top)

                int caret = this.editTextbox.CaretIndex; //this.editTextbox.TextLayout.GetCharacterIndexFromPoint(p, true);

                if (caret >= 0 && caret <= this.editTextbox.Text.Length)
                {
                    this.editTextbox.SelectionStart = caret;
                }

                this.editTextbox.Focus();
            }

            public void EditControlCopy()
            {
                this.editTextbox.Copy();
            }

            public void EditControlPaste()
            {
                this.editTextbox.Paste();
            }

            public void EditControlCut()
            {
                this.editTextbox.Cut();
            }

            public void EditControlUndo()
            {
                this.editTextbox.Undo();
            }
            #endregion

            #region IScrollableControlInterface Members

            public bool ScrollBarHorizontalVisible
            {
                get { return this.canvas.horScrollbar.IsVisible; }
                set { this.canvas.horScrollbar.IsVisible = value; }
            }

            public bool ScrollBarVerticalVisible
            {
                get { return this.canvas.verScrollbar.IsVisible; }
                set { this.canvas.verScrollbar.IsVisible = value; }
            }

            public double ScrollBarHorizontalMaximum
            {
                get { return this.canvas.horScrollbar.Maximum; }
                set { this.canvas.horScrollbar.Maximum = value; }
            }

            public double ScrollBarHorizontalMinimum
            {
                get { return this.canvas.horScrollbar.Minimum; }
                set { this.canvas.horScrollbar.Minimum = value; }
            }

            public double ScrollBarHorizontalValue
            {
                get { return this.canvas.horScrollbar.Value; }
                set { this.canvas.horScrollbar.Value = value; }
            }

            public double ScrollBarHorizontalLargeChange
            {
                get { return this.canvas.horScrollbar.LargeChange; }
                set
                {
                    this.canvas.horScrollbar.LargeChange = value;
                    this.canvas.horScrollbar.ViewportSize = value;
                }
            }

            public double ScrollBarVerticalMaximum
            {
                get { return this.canvas.verScrollbar.Maximum; }
                set { this.canvas.verScrollbar.Maximum = value; }
            }

            public double ScrollBarVerticalMinimum
            {
                get { return this.canvas.verScrollbar.Minimum; }
                set { this.canvas.verScrollbar.Minimum = value; }
            }

            public double ScrollBarVerticalValue
            {
                get { return this.canvas.verScrollbar.Value; }
                set { this.canvas.verScrollbar.Value = value; }
            }

            public double ScrollBarVerticalLargeChange
            {
                get { return this.canvas.verScrollbar.LargeChange; }
                set
                {
                    this.canvas.verScrollbar.LargeChange = value;
                    this.canvas.verScrollbar.ViewportSize = value;
                }
            }

            #endregion

            #region ITimerSupportedControlInterface Members

            public void StartTimer()
            {
                throw new NotImplementedException();
            }

            public void StopTimer()
            {
                throw new NotImplementedException();
            }

            #endregion
        }
        #endregion // Adapter

        #region Editor - TextBox
        internal class InputTextBox : TextBox
        {
            internal ReoGridControl Owner { get; set; }
            internal bool TextWrap { get; set; }
            internal Avalonia.Size CellSize { get; set; }
            internal ReoGridVerAlign VAlign { get; set; }

            static InputTextBox()
            {
                TextProperty.Changed.Subscribe(OnTextChanged);
                IsKeyboardFocusWithinProperty.Changed.Subscribe(OnLostKeyboardFocus);
            }
            internal InputTextBox() : base()
            {
                AddHandler(KeyDownEvent, OnPreviewKeyDown, RoutingStrategies.Tunnel);
                AddHandler(TextInputEvent, OnPreviewTextInput, RoutingStrategies.Tunnel);
                
                this.AcceptsReturn = true;
                this.Template = new FuncControlTemplate<InputTextBox>((t, ns) =>
                {
                    var t1 = new TextPresenter()
                    {
                        Name = "PART_TextPresenter",
                        Margin = new Thickness(2),
                        [!TextPresenter.BackgroundProperty] = t[!TextBox.BackgroundProperty],
                        [!TextPresenter.WidthProperty] = t[!TextBox.WidthProperty],
                        [!TextPresenter.TextProperty] = t[!TextBox.TextProperty],
                        [!TextPresenter.CaretIndexProperty] = t[!TextBox.CaretIndexProperty],
                        [!TextPresenter.SelectionStartProperty] = t[!TextBox.SelectionStartProperty],
                        [!TextPresenter.SelectionEndProperty] = t[!TextBox.SelectionEndProperty],
                        [!TextPresenter.TextAlignmentProperty] = t[!TextBox.TextAlignmentProperty],
                        [!TextPresenter.TextWrappingProperty] = t[!TextBox.TextWrappingProperty],
                        [!TextPresenter.LineHeightProperty] = t[!TextBox.LineHeightProperty],
                        [!TextPresenter.LetterSpacingProperty] = t[!TextBox.LetterSpacingProperty],
                        [!TextPresenter.PasswordCharProperty] = t[!TextBox.PasswordCharProperty],
                        [!TextPresenter.RevealPasswordProperty] = t[!TextBox.RevealPasswordProperty],
                        [!TextPresenter.SelectionBrushProperty] = t[!TextBox.SelectionBrushProperty],
                        [!TextPresenter.SelectionForegroundBrushProperty] = t[!TextBox.SelectionForegroundBrushProperty],
                        [!TextPresenter.CaretBrushProperty] = t[!TextBox.CaretBrushProperty],
                        [!TextPresenter.HorizontalAlignmentProperty] = t[!TextBox.HorizontalAlignmentProperty],
                        [!TextPresenter.VerticalAlignmentProperty] = t[!TextBox.HorizontalContentAlignmentProperty],
                    };
                    var s = new ScrollViewer()
                    {
                        Name = "PART_ScrollViewer",
                        [!ScrollViewer.HorizontalScrollBarVisibilityProperty] = t[!ScrollViewer.HorizontalScrollBarVisibilityProperty],
                        [!ScrollViewer.VerticalScrollBarVisibilityProperty] = t[!ScrollViewer.VerticalScrollBarVisibilityProperty],
                        [!ScrollViewer.IsScrollChainingEnabledProperty] = t[!ScrollViewer.IsScrollChainingEnabledProperty],
                        [!ScrollViewer.AllowAutoHideProperty] = t[!ScrollViewer.AllowAutoHideProperty],
                        [!ScrollViewer.BringIntoViewOnFocusChangeProperty] = t[!ScrollViewer.BringIntoViewOnFocusChangeProperty],
                        Content = t1
                    };
                    ns.Register(s.Name, s);
                    ns.Register(t1.Name, t1);
                    return s;
                });

            }

            protected override void OnLostFocus(RoutedEventArgs e)
            {
                var sheet = this.Owner.CurrentWorksheet;

                if (sheet.currentEditingCell != null && IsVisible)
                {
                    sheet.EndEdit(Text);
                    IsVisible = false;
                }
                base.OnLostFocus(e);
            }

            private void OnPreviewKeyDown(object sender, KeyEventArgs e)
            {
                var sheet = this.Owner.CurrentWorksheet;

                // in single line text
                if (!TextWrap && Text.IndexOf('\n') == -1)
                {
                    Action moveAction = null;

                    if (e.Key == Key.Up)
                    {
                        moveAction = () => sheet.MoveSelectionUp();
                    }
                    else if (e.Key == Key.Down)
                    {
                        moveAction = () => sheet.MoveSelectionDown();
                    }
                    else if (e.Key == Key.Left && SelectionStart == 0)
                    {
                        moveAction = () => sheet.MoveSelectionLeft();
                    }
                    else if (e.Key == Key.Right && SelectionStart == Text.Length)
                    {
                        moveAction = () => sheet.MoveSelectionRight();
                    }
                    if (moveAction != null)
                    {
                        sheet.EndEdit(Text);
                        moveAction();
                        e.Handled = true;
                    }
                }
            }

            protected override void OnKeyDown(KeyEventArgs e)
            {
                var sheet = this.Owner.CurrentWorksheet;

                if (sheet.currentEditingCell != null && IsVisible)
                {
                    if (e.KeyModifiers == KeyModifiers.Control
                        && e.Key == Key.Enter)
                    {
                        var str = this.Text;
                        var selstart = this.SelectionStart;
                        str = str.Insert(this.SelectionStart, Environment.NewLine);
                        this.Text = str;
                        this.SelectionStart = selstart + Environment.NewLine.Length;
                    }
                    else if (e.KeyModifiers == KeyModifiers.None && e.Key == Key.Enter)
                    {
                        sheet.EndEdit(this.Text);
                        sheet.MoveSelectionForward();
                        e.Handled = true;
                    }
                    else if (e.Key == Key.Enter)
                    {
                        // TODO: auto adjust row height
                    }
                    // shift + tab
                    else if (e.KeyModifiers == KeyModifiers.Meta && e.Key == Key.Tab)
                    {
                        sheet.EndEdit(this.Text);
                        sheet.MoveSelectionBackward();
                        e.Handled = true;
                    }
                    // tab
                    else if (e.Key == Key.Tab)
                    {
                        sheet.EndEdit(this.Text);
                        sheet.MoveSelectionForward();
                        e.Handled = true;
                    }
                    else if (e.Key == Key.Escape)
                    {
                        sheet.EndEdit(EndEditReason.Cancel);
                        e.Handled = true;
                    }
                }

                base.OnKeyDown(e);
            }


            private static void OnLostKeyboardFocus(AvaloniaPropertyChangedEventArgs<bool> args)
            {
                var @this = args.Sender as InputTextBox;
                if(@this is not null && args.NewValue == false)
                {
                    @this.Owner.CurrentWorksheet.EndEdit(@this.Text, EndEditReason.NormalFinish);
                }
            }
            private static void OnTextChanged(AvaloniaPropertyChangedEventArgs e)
            {
                var @this = e.Sender as InputTextBox;
                if (@this != null)
                {
                    @this.Text = @this.Owner.currentWorksheet.RaiseCellEditTextChanging(@this.Text);
                }
            }

            private void OnPreviewTextInput(object sender, TextInputEventArgs e)
            {
                if (e.Text.Length > 0)
                {
                    int inputChar = e.Text[0];
                    if (inputChar != this.Owner.currentWorksheet.RaiseCellEditCharInputed(inputChar))
                    {

                        e.Handled = true;
                    }

                }
            }
        }

        #endregion // Editor - TextBox

        #region Context Menu Strips
        internal ContextMenu BaseContextMenu { get { return base.ContextMenu; } set { base.ContextMenu = value; } }

        /// <summary>
        /// Get or set the cells context menu
        /// </summary>
        public ContextMenu CellsContextMenu { get; set; }

        /// <summary>
        /// Get or set the row header context menu
        /// </summary>
        public ContextMenu RowHeaderContextMenu { get; set; }

        /// <summary>
        /// Get or set the column header context menu
        /// </summary>
        public ContextMenu ColumnHeaderContextMenu { get; set; }

        /// <summary>
        /// Get or set the lead header context menu
        /// </summary>
        public ContextMenu LeadHeaderContextMenu { get; set; }
        #endregion // Context Menu Strips

        /// <summary>
        /// Get or set filepath of startup template file
        /// </summary>
        public string LoadFromFile { get; set; }

        public void Dispose() { }
    }
}

#endif