/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net/
 * 
 * Sheet Tab Control - Represents a lightweight and fast sheet tab control
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

using System;
using System.ComponentModel;

#if WINFORM
using RGFloat = System.Single;
using RGPoint = System.Drawing.Point;
using RGColor = System.Drawing.Color;
using RGRect = System.Drawing.Rectangle;
using RGIntDouble = System.Int32;
#elif WPF
using RGFloat = System.Double;
using RGPoint = System.Windows.Point;
using RGColor = System.Windows.Media.Color;
using RGRect = System.Windows.Rect;
using RGIntDouble = System.Double;
#elif ANDROID
using RGFloat = System.Single;
using RGPoint = Android.Graphics.Point;
using RGColor = Android.Graphics.Color;
using RGRect = Android.Graphics.Rect;
using RGIntDouble = System.Int32;
#elif iOS
using RGPoint = CoreGraphics.CGPoint;
using RGIntDouble = System.Double;
#endif

using unvell.ReoGrid.Interaction;

namespace unvell.ReoGrid.Main
{
	/// <summary>
	/// Mouse event arguments for sheet tab control.
	/// </summary>
	public class SheetTabMouseEventArgs : EventArgs
	{
		/// <summary>
		/// Mouse button flags. (Left, Right or Middle)
		/// </summary>
		public MouseButtons MouseButtons { get; set; }

		/// <summary>
		/// Mouse location related to sheet tab control.
		/// </summary>
		public RGPoint Location { get; set; }

		/// <summary>
		/// Number of tab specified by this index to be moved.
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// Get or set whether the user-code handled this event. 
		/// Built-in operations will be cancelled if this property is set to true.
		/// </summary>
		public bool Handled { get; set; }
	}

	/// <summary>
	/// Sheet moved event arguments.
	/// </summary>
	public class SheetTabMovedEventArgs : EventArgs
	{
		/// <summary>
		/// Number of tab specified by this index to be moved.
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// Number of tab as position moved to.
		/// </summary>
		public int TargetIndex { get; set; }
	}

	/// <summary>
	/// Represents the border style of tab item.
	/// </summary>
	public enum SheetTabBorderStyle
	{
		/// <summary>
		/// Sharp Rectangle
		/// </summary>
		RectShadow,

		/// <summary>
		/// Separated Rounded Rectangle
		/// </summary>
		SplitRouned,

		/// <summary>
		/// No Borders (Windows 8 Style)
		/// </summary>
		NoBorder,
	}

	/// <summary>
	/// Position of tab control will be located.
	/// </summary>
	public enum SheetTabControlPosition
	{
		/// <summary>
		/// Put at top to other controls.
		/// </summary>
		Top,

		/// <summary>
		/// Put at bottom to other controls.
		/// </summary>
		Bottom,
	}

	/// <summary>
	/// Representes the sheet tab control interface.
	/// </summary>
	internal interface ISheetTabControl
	{
		///// <summary>
		///// Get or set the border color.
		///// </summary>
		//[Description("Get or set the border color")]
		//RGColor BorderColor { get; set; }

		///// <summary>
		///// Get or set the background color for selected tab.
		///// </summary>
		//[Description("Get or set the background color for selected tab")]
		//RGColor SelectedBackColor { get; set; }

		///// <summary>
		///// Get or set the text color for selected tab.
		///// </summary>
		//[Description("Get or set the text color for selected tab")]
		//RGColor SelectedTextColor { get; set; }

		/// <summary>
		/// Get or set the current tab index.
		/// </summary>
		int SelectedIndex { get; set; }

		/// <summary>
		/// Event raised when tab item is moved.
		/// </summary>
		event EventHandler<SheetTabMovedEventArgs> TabMoved;

		///// <summary>
		///// Convert the absolute point on this sheet tab control to scrolled view point.
		///// </summary>
		///// <param name="p">point to be converted.</param>
		///// <returns>converted view point.</returns>
		//RGFloat TranslateScrollPoint(int p);

		///// <summary>
		///// Get rectangle of specified tab item.
		///// </summary>
		///// <param name="index">Number of tab to get bounds.</param>
		///// <returns>Rectangle bounds of specified tab.</returns>
		//RGRect GetItemBounds(int index);

		/// <summary>
		/// Event raised when selected tab is changed.
		/// </summary>
		event EventHandler SelectedIndexChanged;

		/// <summary>
		/// Event raised when splitter is moved.
		/// </summary>
		event EventHandler SplitterMoving;

		/// <summary>
		/// Event raised when sheet list button is clicked.
		/// </summary>
		event EventHandler SheetListClick;

		/// <summary>
		/// Event raised when new sheet butotn is clicked.
		/// </summary>
		event EventHandler NewSheetClick;

		/// <summary>
		/// Event raised when mouse is pressed down on tab items.
		/// </summary>
		event EventHandler<SheetTabMouseEventArgs> TabMouseDown;

		///// <summary>
		///// Move item to specified position.
		///// </summary>
		///// <param name="index">number of tab to be moved.</param>
		///// <param name="targetIndex">position of moved to.</param>
		//void MoveItem(int index, int targetIndex);

		/// <summary>
		/// Scroll view to show tab item by specified index.
		/// </summary>
		/// <param name="index">Number of item to scrolled.</param>
		void ScrollToItem(int index);

		/// <summary>
		/// Get or set the width of sheet tab control
		/// </summary>
		RGIntDouble ControlWidth { get; set; }

		/// <summary>
		/// Add tab.
		/// </summary>
		/// <param name="title">Title of tab.</param>
		void AddTab(string title);

		/// <summary>
		/// Insert tab
		/// </summary>
		/// <param name="index">Zero-based number of tab.</param>
		/// <param name="title">Title of tab.</param>
		void InsertTab(int index, string title);

		/// <summary>
		/// Update tab title.
		/// </summary>
		/// <param name="index">Zero-based number of tab.</param>
		/// <param name="title">Title of tab.</param>
		void UpdateTab(int index, string title, RGColor backgroundColor, RGColor foregroundColor);

		/// <summary>
		/// Remove specified tab.
		/// </summary>
		/// <param name="index">Zero-based number of tab.</param>
		void RemoveTab(int index);

		/// <summary>
		/// Clear all tabs.
		/// </summary>
		void ClearTabs();

		/// <summary>
		/// Determine whether or not allow to move tab by dragging mouse.
		/// </summary>
		bool AllowDragToMove { get; set; }

		/// <summary>
		/// Determine whether or not to show new sheet button.
		/// </summary>
		bool NewButtonVisible { get; set; }
	}
}
