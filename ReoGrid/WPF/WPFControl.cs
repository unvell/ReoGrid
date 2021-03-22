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
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using unvell.Common;
using unvell.Common.Win32Lib;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Interaction;
using unvell.ReoGrid.Main;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Views;
using unvell.ReoGrid.WPF;

using Point = unvell.ReoGrid.Graphics.Point;
using WPFPoint = System.Windows.Point;

namespace unvell.ReoGrid
{
	/// <summary>
	/// ReoGrid Spreadsheet Control
	/// </summary>
	public partial class ReoGridControl : Canvas, IVisualWorkbook,
		IRangePickableControl, IContextMenuControl, IPersistenceWorkbook, IActionControl, IWorkbook
	{
		internal const int ScrollBarSize = 18;

		private ReoGridWPFControlAdapter adapter;
		//private Grid bottomGrid;
		private SheetTabControl sheetTab;
		private InputTextBox editTextbox;

		private ScrollBar horScrollbar;
		private ScrollBar verScrollbar;

		//private DockPanel layout;

		/// <summary>
		/// Create ReoGrid spreadsheet control
		/// </summary>
		public ReoGridControl()
		{
			this.SnapsToDevicePixels = true;
			this.Focusable = true;
			this.FocusVisualStyle = null;

			this.BeginInit();

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
				SmallChange=	Worksheet.InitDefaultColumnWidth,
			};

			this.verScrollbar = new System.Windows.Controls.Primitives.ScrollBar()
			{
				Orientation = Orientation.Vertical,
				Width = ScrollBarSize,
				SmallChange = Worksheet.InitDefaultRowHeight,
			};

			this.Children.Add(this.sheetTab);
			this.Children.Add(this.horScrollbar);

			Grid.SetColumn(this.horScrollbar, 1);

			//this.Children.Add(this.bottomGrid);
			this.Children.Add(this.verScrollbar);

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
				double width = System.Windows.Input.Mouse.GetPosition(this).X + 3;
				if (width < 75) width = 75;
				if (width > this.RenderSize.Width - ScrollBarSize) width = this.RenderSize.Width - ScrollBarSize;

				this.SheetTabWidth = width;

				this.UpdateSheetTabAndScrollBarsLayout();
				//this.bottomGrid.ColumnDefinitions[0].Width = new GridLength(width);

				//double newScrollWidth = this.RenderSize.Width
				//	- this.bottomGrid.ColumnDefinitions[0].ActualWidth - this.bottomGrid.ColumnDefinitions[2].ActualWidth;

				//if (newScrollWidth < 0) newScrollWidth = 0;

				//this.bottomGrid.ColumnDefinitions[1].Width = new GridLength(newScrollWidth);
				//this.horScrollbar.Width = this.bottomGrid.ColumnDefinitions[1].ActualWidth;
			};

			this.InitControl();

			this.editTextbox = new InputTextBox()
			{
				Owner = this,
				BorderThickness = new Thickness(0),
				Visibility = System.Windows.Visibility.Hidden,
				HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
				VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
				Padding = new Thickness(0),
				Margin = new Thickness(0),
			};

			this.Children.Add(editTextbox);

			this.adapter = new ReoGridWPFControlAdapter(this);
			this.adapter.editTextbox = this.editTextbox;

			InitWorkbook(this.adapter);

			TextCompositionManager.AddPreviewTextInputHandler(this, OnTextInputStart);

			this.EndInit();

			this.renderer = new Rendering.WPFRenderer();

			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input,
				new Action(delegate ()
			{
				if (!string.IsNullOrEmpty(this.LoadFromFile))
				{
					var file = new System.IO.FileInfo(this.LoadFromFile);
					this.currentWorksheet.Load(file.FullName);
				}
			}));
		}

		#region SheetTab & Scroll Bars Visibility

		/// <summary>
		/// Handle event on render size changed
		/// </summary>
		/// <param name="sizeInfo">size information</param>
		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);

			//this.bottomGrid.ColumnDefinitions[1].Width = new GridLength(this.RenderSize.Width
			//	- this.bottomGrid.ColumnDefinitions[0].ActualWidth - ScrollBarWidth);

			//Canvas.SetTop(this.bottomGrid, this.RenderSize.Height - ScrollBarHeight);
			//this.bottomGrid.Width = this.RenderSize.Width;
			//this.bottomGrid.Height = ScrollBarHeight;

			if (this.Visibility == Visibility.Visible)
			{
				if (sizeInfo.PreviousSize.Width > 0)
				{
					this.SheetTabWidth += sizeInfo.NewSize.Width - sizeInfo.PreviousSize.Width;
					if (this.SheetTabWidth < 0) this.SheetTabWidth = 0;
				}
			}

			this.UpdateSheetTabAndScrollBarsLayout();

			this.InvalidateVisual();
		}

		private void SetHorizontalScrollBarSize()
		{
			double hsbWidth = this.ActualWidth;

			if (this.sheetTab.Visibility == Visibility.Visible)
			{
				hsbWidth -= this.SheetTabWidth;
			}

			if (this.verScrollbar.Visibility == Visibility.Visible)
			{
				hsbWidth -= ScrollBarSize;
			}

			if (hsbWidth < 0) hsbWidth = 0;
			this.horScrollbar.Width = hsbWidth;
		}

		private void SetSheetTabSize()
		{
			double stWidth = 0;

			if (this.horScrollbar.Visibility == Visibility.Visible)
			{
				stWidth = this.SheetTabWidth;
			}
			else
			{
				stWidth = this.ActualWidth;
			}

			if (this.verScrollbar.Visibility == Visibility.Visible)
			{
				stWidth -= ScrollBarSize;
			}

			if (stWidth < 0) stWidth = 0;
			this.horScrollbar.Width = stWidth;
		}

		private void UpdateSheetTabAndScrollBarsLayout()
		{
			Canvas.SetTop(this.sheetTab, this.ActualHeight - ScrollBarSize);
			Canvas.SetTop(this.horScrollbar, this.ActualHeight - ScrollBarSize);

			this.sheetTab.Height = ScrollBarSize;
			this.horScrollbar.Height = ScrollBarSize;

			Canvas.SetLeft(verScrollbar, this.RenderSize.Width - ScrollBarSize);

			var vsbHeight = this.RenderSize.Height - ScrollBarSize;
			if (vsbHeight < 0) vsbHeight = 0;
			verScrollbar.Height = vsbHeight;

			if (this.sheetTab.Visibility == Visibility.Visible
				&& this.horScrollbar.Visibility == Visibility.Visible)
			{
				this.sheetTab.Width = this.SheetTabWidth;

				Canvas.SetLeft(this.horScrollbar, this.SheetTabWidth);
				SetHorizontalScrollBarSize();
			}
			else if (this.sheetTab.Visibility == Visibility.Visible)
			{
				this.sheetTab.Width = this.ActualWidth;
			}
			else if (this.horScrollbar.Visibility == Visibility.Visible)
			{
				Canvas.SetLeft(this.horScrollbar, 0);
				SetHorizontalScrollBarSize();
			}
			else
			{
				this.verScrollbar.Height = this.RenderSize.Height;
			}

			this.currentWorksheet.UpdateViewportControllBounds();
		}

		private void ShowSheetTabControl()
		{
			if (this.sheetTab.Visibility != Visibility.Visible)
			{
				this.sheetTab.Visibility = System.Windows.Visibility.Visible;
				this.UpdateSheetTabAndScrollBarsLayout();
			}
		}

		private void HideSheetTabControl()
		{
			if (this.sheetTab.Visibility != Visibility.Hidden)
			{
				this.sheetTab.Visibility = System.Windows.Visibility.Hidden;
				this.UpdateSheetTabAndScrollBarsLayout();
			}
		}

		private void ShowHorScrollBar()
		{
			if (this.horScrollbar.Visibility != System.Windows.Visibility.Visible)
			{
				this.horScrollbar.Visibility = System.Windows.Visibility.Visible;
				this.UpdateSheetTabAndScrollBarsLayout();
			}
		}

		private void HideHorScrollBar()
		{
			if (this.horScrollbar.Visibility != System.Windows.Visibility.Hidden)
			{
				this.horScrollbar.Visibility = System.Windows.Visibility.Hidden;
				this.UpdateSheetTabAndScrollBarsLayout();
			}
		}

		private void ShowVerScrollBar()
		{
			if (this.verScrollbar.Visibility != System.Windows.Visibility.Visible)
			{
				this.verScrollbar.Visibility = System.Windows.Visibility.Visible;
				this.UpdateSheetTabAndScrollBarsLayout();
			}
		}

		private void HideVerScrollBar()
		{
			if (this.verScrollbar.Visibility != System.Windows.Visibility.Hidden)
			{
				this.verScrollbar.Visibility = System.Windows.Visibility.Hidden;
				this.UpdateSheetTabAndScrollBarsLayout();
			}
		}

		#endregion // SheetTab & Scroll Bars Visibility

		#region Render

		/// <summary>
		/// Handle repaint event to draw component.
		/// </summary>
		/// <param name="dc">Platform independence drawing context.</param>
		protected override void OnRender(System.Windows.Media.DrawingContext dc)
		{
#if DEBUG
			Stopwatch watch = Stopwatch.StartNew();
#endif

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
					bgBrush = Brushes.White;
				}

				dc.DrawRectangle(bgBrush, null, new Rect(0, 0, this.RenderSize.Width, this.RenderSize.Height));

				this.renderer.Reset();

				((WPFRenderer)this.renderer).SetPlatformGraphics(dc);

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
		}

		#endregion // Render

		#region Mouse

		bool mouseCaptured = false;

		protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);

			Focus();

			var pos = e.GetPosition(this);

			double right = this.RenderSize.Width;
			double bottom = this.RenderSize.Height;

			if (this.verScrollbar.Visibility == Visibility.Visible)
			{
				right = Canvas.GetLeft(this.verScrollbar);
			}

			if (this.sheetTab.Visibility == Visibility.Visible)
			{
				bottom = Canvas.GetTop(this.sheetTab);
			}
			else if (this.horScrollbar.Visibility == Visibility.Visible)
			{
				bottom = Canvas.GetTop(this.horScrollbar);
			}

			if (pos.X < right && pos.Y < bottom)
			{
				if (e.ClickCount == 2)
				{
					this.currentWorksheet.OnMouseDoubleClick(e.GetPosition(this), WPFUtility.ConvertToUIMouseButtons(e));
				}
				else
				{
					this.OnWorksheetMouseDown(e.GetPosition(this), WPFUtility.ConvertToUIMouseButtons(e));
					if (CaptureMouse()) mouseCaptured = true;
				}
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			this.OnWorksheetMouseMove(e.GetPosition(this), WPFUtility.ConvertToUIMouseButtons(e));
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);

			this.OnWorksheetMouseUp(e.GetPosition(this), WPFUtility.ConvertToUIMouseButtons(e));

			if (mouseCaptured) ReleaseMouseCapture();
		}

		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			base.OnMouseWheel(e);

			this.currentWorksheet.OnMouseWheel(e.GetPosition(this), e.Delta, WPFUtility.ConvertToUIMouseButtons(e));
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
				var wfkeys = (KeyCode)KeyInterop.VirtualKeyFromKey(e.Key);

				if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
				{
					wfkeys |= KeyCode.Control;
				}
				else if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
				{
					wfkeys |= KeyCode.Shift;
				}
				else if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
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
				var wfkeys = (KeyCode)KeyInterop.VirtualKeyFromKey(e.Key);

				if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
				{
					wfkeys |= KeyCode.Control;
				}
				else if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
				{
					wfkeys |= KeyCode.Shift;
				}
				else if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
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
		protected override void OnTextInput(TextCompositionEventArgs e)
		{
			base.OnTextInput(e);
		}

		private void OnTextInputStart(object sender, TextCompositionEventArgs args)
		{
			if (!this.currentWorksheet.IsEditing)
			{
				this.currentWorksheet.StartEdit();
				this.currentWorksheet.CellEditText = string.Empty;
			}
		}

		#endregion // Keyboard

		#region Adapter
		private class ReoGridWPFControlAdapter : IControlAdapter
		{
			#region Constructor
			private readonly ReoGridControl canvas;
			internal InputTextBox editTextbox;

			internal ReoGridWPFControlAdapter(ReoGridControl canvas)
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

				switch (cursor)
				{
					default:
					case CursorStyle.PlatformDefault: this.canvas.Cursor = Cursors.Arrow; break;
					case CursorStyle.Selection: this.canvas.Cursor = this.canvas.internalCurrentCursor; break;
					case CursorStyle.Busy: this.canvas.Cursor = Cursors.AppStarting; break;
					case CursorStyle.Hand: this.canvas.Cursor = Cursors.Hand; break;
					case CursorStyle.FullColumnSelect: this.canvas.Cursor = this.canvas.builtInFullColSelectCursor; break;
					case CursorStyle.FullRowSelect: this.canvas.Cursor = this.canvas.builtInFullRowSelectCursor; break;
					case CursorStyle.ChangeRowHeight: this.canvas.Cursor = Cursors.SizeNS; break;
					case CursorStyle.ChangeColumnWidth: this.canvas.Cursor = Cursors.SizeWE; break;
					case CursorStyle.ResizeHorizontal: this.canvas.Cursor = Cursors.SizeWE; break;
					case CursorStyle.ResizeVertical: this.canvas.Cursor = Cursors.SizeNS; break;
					case CursorStyle.Move: this.canvas.Cursor = Cursors.SizeAll; break;
					case CursorStyle.Cross: this.canvas.Cursor = this.canvas.builtInCrossCursor; break;
				}
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
						this.canvas.internalCurrentCursor = Cursors.Arrow;
						break;

					case CursorStyle.Hand:
						this.canvas.internalCurrentCursor = Cursors.Hand;
						break;
				}
			}

			public Rectangle GetContainerBounds()
			{
				double w = this.canvas.ActualWidth;
				double h = this.canvas.ActualHeight+1;

				if (this.canvas.verScrollbar.Visibility == Visibility.Visible)
				{
					w -= ScrollBarSize;
				}

				if (this.canvas.sheetTab.Visibility == Visibility.Visible
					|| this.canvas.horScrollbar.Visibility == Visibility.Visible)
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

			public void ChangeBackColor(Color color)
			{
				this.canvas.Background = new SolidColorBrush(color);
			}

			public bool IsVisible
			{
				get { return this.canvas.Visibility == Visibility.Visible; }
			}

			public Graphics.Point PointToScreen(Graphics.Point p)
			{
				return this.canvas.PointToScreen(p);
			}

			public IGraphics PlatformGraphics { get { return null; } }

			public void ChangeBackgroundColor(SolidColor color)
			{
				this.canvas.Background = new SolidColorBrush(color);
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

				this.editTextbox.Width = bounds.Width;
				this.editTextbox.Height = bounds.Height;
				this.editTextbox.RenderSize = bounds.Size;

				this.editTextbox.CellSize = cell.Bounds.Size;
				this.editTextbox.VAlign = cell.InnerStyle.VAlign;
				this.editTextbox.FontFamily = new FontFamily(cell.InnerStyle.FontName);
				this.editTextbox.FontSize = cell.InnerStyle.FontSize * sheet.ScaleFactor * 96f / 72f;
				this.editTextbox.FontStyle = PlatformUtility.ToWPFFontStyle(cell.InnerStyle.fontStyles);
				this.editTextbox.Foreground = this.Renderer.GetBrush(textColor);
				this.editTextbox.Background = this.Renderer.GetBrush(cell.InnerStyle.HasStyle(PlainStyleFlag.BackColor)
					? cell.InnerStyle.BackColor : this.canvas.controlStyle[ControlAppearanceColors.GridBackground]);
				this.editTextbox.SelectionStart = this.editTextbox.Text.Length;
				this.editTextbox.TextWrap = cell.InnerStyle.TextWrapMode != TextWrapMode.NoWrap;
				this.editTextbox.TextWrapping = (cell.InnerStyle.TextWrapMode == TextWrapMode.NoWrap)
					? TextWrapping.NoWrap : TextWrapping.Wrap;

				this.editTextbox.Visibility = Visibility.Visible;
				this.editTextbox.Focus();
			}

			public void HideEditControl()
			{
				this.editTextbox.Visibility = Visibility.Hidden;
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
				return this.editTextbox.GetLineIndexFromCharacterIndex(this.editTextbox.SelectionStart);
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
				Point p = System.Windows.Input.Mouse.GetPosition(this.editTextbox);

				p.X += 2; // fix 2 pixels (borders of left and right)
				p.Y -= 1; // fix 1 pixels (top)

				int caret = this.editTextbox.GetCharacterIndexFromPoint(p, true);

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
				get { return this.canvas.horScrollbar.Visibility == Visibility.Visible; }
				set { this.canvas.horScrollbar.Visibility = value ? Visibility.Visible : Visibility.Hidden; }
			}

			public bool ScrollBarVerticalVisible
			{
				get { return this.canvas.verScrollbar.Visibility == Visibility.Visible; }
				set { this.canvas.verScrollbar.Visibility = value ? Visibility.Visible : Visibility.Hidden; }
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
		private class InputTextBox : TextBox
		{
			internal ReoGridControl Owner { get; set; }
			internal bool TextWrap { get; set; }
			internal System.Windows.Size CellSize { get; set; }
			internal ReoGridVerAlign VAlign { get; set; }

			internal InputTextBox()
				: base()
			{
				//SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			}

			protected override void OnLostFocus(RoutedEventArgs e)
			{
				var sheet = this.Owner.CurrentWorksheet;

				if (sheet.currentEditingCell != null && Visibility == System.Windows.Visibility.Visible)
				{
					sheet.EndEdit(Text);
					Visibility = System.Windows.Visibility.Hidden;
				}
				base.OnLostFocus(e);
			}

			protected override void OnPreviewKeyDown(KeyEventArgs e)
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

				if (sheet.currentEditingCell != null && Visibility == System.Windows.Visibility.Visible)
				{
					if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
						&& e.Key == Key.Enter)
					{
						var str = this.Text;
						var selstart = this.SelectionStart;
						str = str.Insert(this.SelectionStart, Environment.NewLine);
						this.Text = str;
						this.SelectionStart = selstart + Environment.NewLine.Length;
					}
					else if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl) && e.Key == Key.Enter)
					{
						sheet.EndEdit(this.Text);
						sheet.MoveSelectionForward();
						e.Handled = true;
					}
					else if (e.Key == Key.Enter)
					{
						// TODO: auto adjust row height
					}
					else if (e.Key == Key.Escape)
					{
						sheet.EndEdit(EndEditReason.Cancel);
						e.Handled = true;
					}
				}
			}
			protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
			{
				base.OnLostKeyboardFocus(e);
				this.Owner.CurrentWorksheet.EndEdit(Text, EndEditReason.NormalFinish);
			}
			protected override void OnTextChanged(TextChangedEventArgs e)
			{
				base.OnTextChanged(e);
				this.Text = this.Owner.currentWorksheet.RaiseCellEditTextChanging(this.Text);
			}
			protected override void OnPreviewTextInput(TextCompositionEventArgs e)
			{
				if (e.Text.Length > 0)
				{
					int inputChar = e.Text[0];
					if (inputChar != this.Owner.currentWorksheet.RaiseCellEditCharInputed(inputChar))
					{
						
						e.Handled = true;
					}

				}
				base.OnPreviewTextInput(e);
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

	#region WPFUtility
	internal class WPFUtility
	{
		public static MouseButtons ConvertToUIMouseButtons(MouseEventArgs e)
		{
			MouseButtons btn = MouseButtons.None;
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				btn |= MouseButtons.Left;
			}
			if (e.MiddleButton == MouseButtonState.Pressed)
			{
				btn |= MouseButtons.Middle;
			}
			if (e.RightButton == MouseButtonState.Pressed)
			{
				btn |= MouseButtons.Right;
			}
			return btn;
		}
	}
	#endregion

}

#endif