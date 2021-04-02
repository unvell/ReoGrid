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

#if ANDROID

using System;

using Android.Views;
using Android.Util;
using Android.Content;
using Android.Graphics;
using Android.Widget;
using Android.Runtime;

using View = Android.Views.View;

using unvell.ReoGrid.Actions;
using unvell.ReoGrid.Events;
using unvell.ReoGrid.IO;
using unvell.ReoGrid.Main;
using unvell.ReoGrid.Views;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.AndroidOS;
using System.Threading;

namespace unvell.ReoGrid
{
	public partial class ReoGridView : ViewGroup, IReoGridControl, IWorkbook
	{
		private ReoGridViewAdapter adapter;

		private SheetTabView sheetTab;

		private WorksheetView worksheetView;

		internal float baseScale;

		public ReoGridView(Context ctx, IAttributeSet attrs)
			: base(ctx, attrs)
		{
			this.baseScale = this.Context.Resources.DisplayMetrics.ScaledDensity - 0.5f;

			this.sheetTab = new SheetTabView(this, ctx, attrs);

			this.AddView(this.sheetTab);

			this.worksheetView = new WorksheetView(this, ctx, attrs);
			this.AddView(this.worksheetView);

			this.adapter = new ReoGridViewAdapter(this);
			this.renderer = new AndroidRenderer();

			this.InitControl();
			this.InitWorkbook(this.adapter);
		}

		protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
		{
			int sheetTabHeight = (int)Math.Floor(15 * this.baseScale);

			if (sheetTabHeight < 50)
			{
				sheetTabHeight = 50;
			}

			this.sheetTab.Layout(0, this.Height - sheetTabHeight, this.Width, this.Height);
			this.worksheetView.Layout(0, 0, this.Width, this.Height - sheetTabHeight);
		}

		#region Sheet Tab
		private void ShowSheetTabControl()
		{
			if (this.sheetTab.Visibility != ViewStates.Visible)
			{
				this.sheetTab.Visibility = ViewStates.Visible;
			}
		}

		private void HideSheetTabControl()
		{
			this.sheetTab.Visibility = ViewStates.Invisible;
		}
		#endregion // Sheet Tab

		#region Adapter
		class ReoGridViewAdapter : IControlAdapter
		{
			private ReoGridView view;

			public ReoGridViewAdapter(ReoGridView view)
			{
				this.view = view;
			}

			public IReoGridControl ControlInstance
			{
				get
				{
					return this.view;
				}
			}

			public ISheetTabControl SheetTabControl
			{
				get { return this.view.sheetTab; }
			}

			public ControlAppearanceStyle ControlStyle { get { return this.view.controlStyle; } }

			public float BaseScale { get { return this.view.baseScale; } }
			public float MinScale { get { return -this.view.baseScale + 0.5f; } }
			public float MaxScale { get { return 4f; } }

			public bool IsVisible
			{
				get { return this.view.Visibility == ViewStates.Visible; }
			}

			public IRenderer Renderer { get { return this.view.renderer; } }

			#region Scroll Bars
			public int ScrollBarHorizontalLargeChange
			{
				get
				{
					return 0; // TODO
				}
				set
				{
					// TODO
				}
			}

			public int ScrollBarHorizontalMaximum
			{
				get
				{
					return 0;
				}
				set
				{
					// TODO
				}
			}

			public int ScrollBarHorizontalMinimum
			{
				get
				{
					return 0;
				}
				set
				{
					// TODO
				}
			}

			public int ScrollBarHorizontalValue
			{
				get
				{
					return 0;
				}
				set
				{
					// TODO
				}
			}

			public int ScrollBarVerticalLargeChange
			{
				get
				{
					return 0;
				}
				set
				{
					// TODO
				}
			}

			public int ScrollBarVerticalMaximum
			{
				get
				{
					return 0;
				}
				set
				{
					// TODO
				}
			}

			public int ScrollBarVerticalMinimum
			{
				get
				{
					return 0;
				}
				set
				{
					// TODO
				}
			}

			public int ScrollBarVerticalValue
			{
				get
				{
					return 0;
				}
				set
				{
					// TODO
				}
			}
			#endregion // Scroll Bars

			#region Not Supported
			public void ChangeCursor(RGCursor cursor)
			{
				// Do Not Support
			}

			public void ChangeSelectionCursor(RGCursor cursor)
			{
				// Do Not Support
			}

			public void EditControlCopy()
			{
				// Do Not Support
			}

			public void EditControlCut()
			{
				// Do Not Support
			}

			public void EditControlPaste()
			{
				// Do Not Support
			}

			public void EditControlSelectAll()
			{
				// Do Not Support
			}

			public void EditControlUndo()
			{
				// Do Not Support
			}

			public void Focus()
			{
				// Do Not Support
			}

			public void RestoreCursor()
			{
				// Do Not Support
			}

			public void ShowContextMenuStrip(ViewTypes viewType, Graphics.Point containerLocation)
			{
				// Do Not Support
			}

			public void ShowContextMenuStrip(ViewTypes viewType, PointF containerLocation)
			{
				// Do Not Support
			}

			public void ShowTooltip(Graphics.Point point, string content)
			{
				// TODO
			}
			#endregion // Not Supported

			#region UI

			public void ChangeBackgroundColor(SolidColor color)
			{
				// TODO
			}

			public Graphics.Rectangle GetContainerBounds()
			{
				return new Rectangle(this.view.Left, 0, this.view.Width, this.view.Height);
			}

			public void Invalidate()
			{
				this.view.worksheetView.PostInvalidate();
			}

			public Graphics.Point PointToScreen(Graphics.Point point)
			{
				var pts = new int[] { (int)point.X, (int)point.Y };
				this.view.GetLocationOnScreen(pts);
				return new Graphics.Point(pts[0], pts[1]);
			}

			#endregion // UI

			#region Edit Control
			public void EditControlApplySystemMouseDown()
			{
				// TODO
			}
			public int GetEditControlCaretLine()
			{
				return 0; // TODO
			}

			public int GetEditControlCaretPos()
			{
				return 0; // TODO
			}

			public string GetEditControlText()
			{
				return null;
			}

			public void HideEditControl()
			{
				// TODO
			}

			public void SetEditControlAlignment(ReoGridHorAlign align)
			{
				// TODO
			}

			public void SetEditControlCaretPos(int pos)
			{
				// TODO
			}

			public void SetEditControlText(string text)
			{
				// TODO
			}

			public void ShowEditControl(Graphics.Rectangle bounds, ReoGridCell cell)
			{
				// TODO
			}
			#endregion Edit Control

			#region Timer
			public void StartTimer()
			{
				// TODO
			}

			public void StopTimer()
			{
				// TODO
			}
			#endregion Timer
		}
		#endregion // Adapter

		#region Scroll Bars
		private void ShowHorScrollBar()
		{
			// TODO
		}

		private void HideHorScrollBar()
		{
			// TODO
		}

		private void ShowVerScrollBar()
		{
			// TODO
		}

		private void HideVerScrollBar()
		{
			// TODO
		}
		#endregion // Scroll Bars

		#region WorksheetView
		class WorksheetView : View
		{
			private ReoGridView rgView;

			private Timer timer;

			public WorksheetView(ReoGridView rgView, Context ctx, IAttributeSet attrs)
				: base(ctx, attrs)
			{
				this.rgView = rgView;

				this.timer = new Timer(Tick, null, Timeout.Infinite, Timeout.Infinite);

				this.Focusable = true;
				this.SetBackgroundColor(Color.White);
			}

			void Tick(object state)
			{
				bool scroll = false;

				if (remainX > 0.001 || remainX < -0.001)
				{
					remainX -= remainX * 0.3f;
					scroll = true;
				}
				else
				{
					remainX = 0;
				}

				if (remainY > 0.001 || remainY < -0.001)
				{
					remainY -= remainY * 0.3f;
					scroll = true;
				}
				else
				{
					remainY = 0;
				}

				if (scroll)
				{
					this.rgView.ScrollCurrentWorksheet(remainX, remainY);
				}
				else
				{
					this.timer.Change(Timeout.Infinite, Timeout.Infinite);
				}
			}

			private float startTouchX, startTouchY;
			private float touchX, touchY;
			private float remainX, remainY;
			private float lastDistance = 0;
			private DateTime touchTime;

			public override bool OnTouchEvent(MotionEvent e)
			{
				float x1, y1, x2, y2, x, y;

				switch (e.Action)
				{

					case MotionEventActions.Down:
						this.touchX = this.startTouchX = e.GetX();
						this.touchY = this.startTouchY = e.GetY();
						this.touchTime = DateTime.Now;
						return true;

					case MotionEventActions.Move:
						switch (e.PointerCount)
						{
							case 1:
								x = e.GetX();
								y = e.GetY();
								this.rgView.ScrollCurrentWorksheet((this.touchX - x) / this.rgView.currentWorksheet.renderScaleFactor,
									(this.touchY - y) / this.rgView.currentWorksheet.renderScaleFactor);
								this.touchX = x;
								this.touchY = y;
								this.Invalidate();
								break;

							case 2:
								x1 = e.GetX(0); x2 = e.GetX(1);
								y1 = e.GetY(0); y2 = e.GetY(0);
								x = x2 - x1;
								y = y2 - y1;
								float distance = (float)Math.Sqrt(x * x + y * y);
								float scaleChange = (distance - lastDistance) / (100 * this.rgView.baseScale);
								if (scaleChange != 0)
								{
									this.rgView.currentWorksheet.ScaleFactor += scaleChange;
									lastDistance = distance;
								}
								break;
						}
						return true;

					case MotionEventActions.Up:
						float ms = ((float)(DateTime.Now - this.touchTime).TotalMilliseconds + 0.3f);
						this.remainX = (this.startTouchX - e.GetX()) / ms;
						this.remainY = (this.startTouchY - e.GetY()) / ms;

						if (this.remainX > 1 || this.remainY > 1
							|| this.remainX < -1 || this.remainY < -1)
						{
							this.remainX *= 15.0f * this.rgView.baseScale;
							this.remainY *= 15.0f * this.rgView.baseScale;

							timer.Change(20, 10);
						}
						return true;

					case MotionEventActions.Pointer2Down:
						x1 = e.GetX(0); x2 = e.GetX(1);
						y1 = e.GetY(0); y2 = e.GetY(0);
						x = x2 - x1;
						y = y2 - y1;
						lastDistance = (float)Math.Sqrt(x * x + y * y);
						return true;

					case MotionEventActions.Pointer2Up:
						return true;

					case MotionEventActions.Scroll:
						break;
				}

				//this.scaleDetector.OnTouchEvent(e);
				return base.OnTouchEvent(e);
			}

			protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
			{
				base.OnSizeChanged(w, h, oldw, oldh);

				if (this.rgView.currentWorksheet != null
					&& this.rgView.currentWorksheet.ViewportController != null)
				{
					if (this.rgView.currentWorksheet != null)
					{
						this.rgView.currentWorksheet.UpdateViewportControllBounds();
					}
				}
			}

			protected override void OnDraw(Canvas canvas)
			{
				if (this.rgView.currentWorksheet != null
					&& this.rgView.currentWorksheet.ViewportController != null)
				{
					var dc = new ReoGrid.Rendering.CellDrawingContext(this.rgView.currentWorksheet,
						DrawMode.View, this.rgView.renderer);

					this.rgView.renderer.PlatformGraphics = canvas;

					this.rgView.currentWorksheet.ViewportController.Draw(dc);
				}
			}

		}

		#endregion // WorksheetView
	}

	class SheetTabView : HorizontalScrollView, ISheetTabControl
	{
		private ReoGridView rgView;

		private InnerView innerView;

		internal SheetTabView(ReoGridView rgView, Context ctx, IAttributeSet attrs)
			: base(ctx, attrs)
		{
			this.rgView = rgView;

			innerView = new InnerView(rgView, ctx, attrs);
			this.AddView(this.innerView);

			this.SetBackgroundColor(Color.WhiteSmoke);
		}

		#region Do not support

		public bool AllowDragToMove
		{
			get { return false; }
			set { /* Do not support */ }
		}

		public int ControlWidth
		{
			get { return 0; /* Do not support */ }
			set { /* Do not support */ }
		}

		public bool NewButtonVisible
		{
			get { return false; /* Do not support */ }
			set { /* Do not support */ }
		}

		#endregion // Do not support

		private int selectedIndex = 0;

		public int SelectedIndex
		{
			get { return selectedIndex; }
			set
			{
				selectedIndex = value;

				for (int i = 0; i < this.innerView.ChildCount; i++)
				{
					SheetTab tab = this.innerView.GetChildAt(i) as SheetTab;
					if (tab != null)
					{
						tab.IsSelected = i == value;
					}
				}

				this.innerView.Invalidate();
			}
		}

		public event EventHandler NewSheetClick;
		public event EventHandler SelectedIndexChanged;
		public event EventHandler SheetListClick;
		public event EventHandler SplitterMoving;
		public event EventHandler<SheetTabMouseEventArgs> TabMouseDown;
		public event EventHandler<SheetTabMovedEventArgs> TabMoved;

		public void AddTab(string title)
		{
			this.InsertTab(this.innerView.ChildCount, title);
		}

		public void ClearTabs()
		{
			this.innerView.RemoveAllViews();
		}

		public void InsertTab(int index, string title)
		{
			var tab = new SheetTab(this.rgView, this.Context)
			{
				Text = title,
			};

			this.innerView.AddView(tab);

			UpdateTabWidthForText(tab);
		}

		public void RemoveTab(int index)
		{
			this.innerView.RemoveViewAt(index);
		}

		public void ScrollToItem(int index)
		{
			// TODO
		}

		public void UpdateTab(int index, string title, Color backgroundColor, Color foregroundColor)
		{
			SheetTab tab = this.GetChildAt(index) as SheetTab;

			if (tab != null)
			{
				tab.Text = title;
				tab.SetTextColor(foregroundColor);
				tab.SetBackgroundColor(backgroundColor);

				UpdateTabWidthForText(tab);
			}
		}

		private void UpdateTabWidthForText(SheetTab tab)
		{
			using (var p = new Paint())
			{
				p.TextSize = tab.TextSize;

				Rect rect = new Rect();
				p.GetTextBounds(tab.Text, 0, tab.Text.Length, rect);

				tab.TabWidth = (int)Math.Round(rect.Width() + 10.0f * this.rgView.baseScale);

				if (tab.TabWidth < 70 * this.rgView.baseScale)
				{
					tab.TabWidth = (int)Math.Round(70 * this.rgView.baseScale);
				}
			}

			this.UpdateContainerWidth();
		}

		private int tabsWidth = 0;

		private void UpdateContainerWidth()
		{
			tabsWidth = 0;

			for (int i = 0; i < this.innerView.ChildCount; i++)
			{
				var tab = this.innerView.GetChildAt(i) as SheetTab;
				if (tab != null)
				{
					tabsWidth += tab.TabWidth;
				}
			}

			this.RequestLayout();
		}

		protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
		{
			this.innerView.Layout(0, 0, this.tabsWidth, this.Height);
		}

		protected override void OnDraw(Canvas canvas)
		{
			using (var p = new Paint())
			{
				float lineWidth = 1.0f * this.rgView.baseScale;
				float halfLineWidth = lineWidth / 2;

				p.StrokeWidth = lineWidth;
				p.Color = Color.SkyBlue;

				canvas.DrawLine(0, halfLineWidth, this.Width, halfLineWidth, p);
			}
		}

		class InnerView : ViewGroup
		{
			private ReoGridView rgView;

			public InnerView(ReoGridView rgView, Context ctx, IAttributeSet attrs)
				: base(ctx, attrs)
			{
				this.rgView = rgView;
			}

			protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
			{
				int x = 0;

				for (int i = 0; i < this.ChildCount; i++)
				{
					SheetTab tab = this.GetChildAt(i) as SheetTab;

					if (tab != null)
					{
						int height = bottom - top;
						tab.Layout(x, 0, x + tab.TabWidth, height);
						x += tab.TabWidth;
					}
				}
			}
		}

		class SheetTab : TextView
		{
			private ReoGridView rgView;

			private bool isSelected;

			internal bool IsSelected
			{
				get
				{
					return this.isSelected;
				}
				set
				{
					this.isSelected = value;

					this.SetBackgroundColor(this.isSelected ? Color.White : Color.WhiteSmoke);
				}
			}

			internal int TabWidth { get; set; }

			internal SheetTab(ReoGridView rgView, Context ctx)
				: this(rgView, ctx, null)
			{
			}

			internal SheetTab(ReoGridView rgView, Context ctx, IAttributeSet attrs)
				: base(ctx, attrs)
			{
				this.rgView = rgView;

				TextSize = 12f;
				
				SetBackgroundColor(Color.WhiteSmoke);
				SetTextColor(Color.DimGray);
			}

			protected override void OnDraw(Canvas canvas)
			{
				using (var p = new Paint())
				{
					p.TextSize = this.TextSize;
					p.Color = new Color(this.CurrentTextColor);
					p.AntiAlias = true;

					Rect r = new Rect();
					p.GetTextBounds(this.Text, 0, this.Text.Length, r);

					canvas.DrawText(this.Text, (this.Width - r.Width()) / 2, this.Height - (this.Height - r.Height()) / 2, p);


					p.AntiAlias = false;

					float lineWidth = 1.0f * this.rgView.baseScale;
					float halfLineWidth = lineWidth / 2;

					p.StrokeWidth = lineWidth;
					p.Color = Color.SkyBlue;

					if (this.IsSelected)
					{
						canvas.DrawLine(0, this.Height - halfLineWidth, this.Width, this.Height - halfLineWidth, p);
						canvas.DrawLine(halfLineWidth, 0, halfLineWidth, this.Height, p);
						canvas.DrawLine(this.Width - halfLineWidth, 0, this.Width - halfLineWidth, this.Height, p);
					}
					else
					{
						canvas.DrawLine(0, halfLineWidth, this.Width, halfLineWidth, p);
					}
				}
			}

			public override bool OnTouchEvent(MotionEvent e)
			{
				switch (e.Action)
				{
					case MotionEventActions.Down:
						var worksheet = this.rgView.GetWorksheetByName(this.Text);
						if (worksheet != null)
						{
							this.rgView.CurrentWorksheet = worksheet;
						}
						return true;

					default:
						return base.OnTouchEvent(e);
				}
			}
		}
	}



}

#endif // ANDROID