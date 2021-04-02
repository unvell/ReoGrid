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
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

#if iOS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CoreGraphics;
using UIKit;
using unvell.ReoGrid.Main;

namespace unvell.ReoGrid
{
	using Graphics;
	using unvell.ReoGrid.iOS;
	using Rendering;

	partial class ReoGridView : UIView, IReoGridControl
	{
		CGPath path;

		private ReoGridViewAdapter adapter;

		private SheetTabView sheetTab;

#region Sheet Tab
		private void ShowSheetTabControl()
		{
			if (this.sheetTab.Hidden)
			{
				this.sheetTab.Hidden = false;
			}
		}

		private void HideSheetTabControl()
		{
			this.sheetTab.Hidden = true;
		}
#endregion // Sheet Tab

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

#region Adapter
		private class ReoGridViewAdapter : ICompViewAdapter
		{
			private ReoGridView view;

			public ReoGridViewAdapter(ReoGridView view)
			{
				this.view = view;
			}


			public IReoGridControl ControlInstance { get { return this.view; } }

			public ControlAppearanceStyle ControlStyle { get { return this.view.controlStyle; } }

			public bool IsVisible { get { return !this.view.Hidden; } }

			public double BaseScale { get { return 0f; } }
			public double MaxScale { get { return 4f; } }
			public double MinScale { get { return 0.1f; } }

			public IRenderer Renderer { get { return this.view.renderer; } }

			public ISheetTabControl SheetTabControl { get { return this.view.sheetTab; } }

			public void ChangeBackgroundColor(SolidColor color)
			{
				// TODO
			}

#region Not supported
			public void ChangeCursor(RGCursor cursor)
			{
				// Not supported
			}

			public void ChangeSelectionCursor(RGCursor cursor)
			{
				// Not supported
			}

			public void RestoreCursor()
			{
				// Not supported
			}

			public void ShowTooltip(Point point, string content)
			{
				// Not supported
			}

			public void Focus()
			{
				// TODO 
			}
#endregion // Not supported

			public Rectangle GetContainerBounds()
			{
				return this.view.Bounds;
			}

			public void Invalidate()
			{
				this.view.DrawRect(GetContainerBounds(), new UIViewPrintFormatter());
			}

			public Point PointToScreen(Point point)
			{
				return this.view.ConvertPointToCoordinateSpace(point, this.view);
			}
		}
#endregion // Adapter

		public override void Draw(CGRect rect)
		{
			using (var g = UIGraphics.GetCurrentContext())
			{
				g.SetLineWidth(20);
				g.SetBlendMode(CGBlendMode.Clear);
				UIColor.Clear.SetColor();

				path.AddLines(new CGPoint[] {
							new CGPoint(10, 10), new CGPoint(100, 100)
						});

				g.SetLineCap(CGLineCap.Round);
				g.AddPath(path);
				g.DrawPath(CGPathDrawingMode.Stroke);
			}
		}
	}
}

namespace unvell.ReoGrid.iOS
{
	class SheetTabView : UIView, ISheetTabControl
	{
		public bool AllowDragToMove
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public double ControlWidth
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public bool NewButtonVisible
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public int SelectedIndex
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
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
			throw new NotImplementedException();
		}

		public void ClearTabs()
		{
			throw new NotImplementedException();
		}

		public void InsertTab(int index, string title)
		{
			throw new NotImplementedException();
		}

		public void RemoveTab(int index)
		{
			throw new NotImplementedException();
		}

		public void ScrollToItem(int index)
		{
			throw new NotImplementedException();
		}

		public void UpdateTab(int index, string title)
		{
			throw new NotImplementedException();
		}
	}

}

#endif // iOS