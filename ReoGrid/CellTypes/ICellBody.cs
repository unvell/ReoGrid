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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

#if WINFORM
using System.Windows.Forms;
using RGFloat = System.Single;
using RGImage = System.Drawing.Image;
#else
using RGFloat = System.Double;
using RGImage = System.Windows.Media.ImageSource;
#endif // WINFORM

using unvell.ReoGrid.Events;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Interaction;

namespace unvell.ReoGrid.CellTypes
{
	/// <summary>
	/// Represents cell body interface.
	/// </summary>
	public interface ICellBody
	{
		/// <summary>
		/// This method invoked when cell body set into a cell.
		/// </summary>
		/// <param name="cell">The cell instance to load this body.</param>
		void OnSetup(Cell cell);

		/// <summary>
		/// Get the cell body bounds. (Relative position to owner cell)
		/// </summary>
		Rectangle Bounds { get; set; }

		/// <summary>
		/// This method invoked when body bounds is changed.
		/// </summary>
		void OnBoundsChanged();

		/// <summary>
		/// Determine whether or not to allow capture the mouse moving after mouse button pressed inside the body bounds.
		/// </summary>
		/// <returns>Return true to capture mouse after mouse down; Otherwise return false to do nothing.</returns>
		bool AutoCaptureMouse();

		/// <summary>
		/// This method will be invoked when mouse button pressed inside the body bounds.
		/// </summary>
		/// <param name="e">Mouse event argument</param>
		/// <returns>Return true if event has been handled; Otherwise return false to recall default operations.</returns>
		bool OnMouseDown(CellMouseEventArgs e);

		/// <summary>
		/// This method will be invoked when mouse has been moved inside the body bounds.
		/// </summary>
		/// <param name="e">Mouse event argument</param>
		/// <returns>Return true if event has been handled; Otherwise false to recall default operations.</returns>
		bool OnMouseMove(CellMouseEventArgs e);

		/// <summary>
		/// This method will be invoked when mouse button released inside the body bounds.
		/// </summary>
		/// <param name="e">Mouse event argument</param>
		/// <returns>Return true if event has been handled; Otherwise false to recall default operations.</returns>
		bool OnMouseUp(CellMouseEventArgs e);

		/// <summary>
		/// This method will be invoked when mouse moved enter the body bounds.
		/// </summary>
		/// <param name="e">Mouse event argument</param>
		/// <returns>Return true if event has been handled; Otherwise return false to recall default operations.</returns>
		bool OnMouseEnter(CellMouseEventArgs e);

		/// <summary>
		/// This method will be invoked when mouse moved out from the body bounds.
		/// </summary>
		/// <param name="e">Mouse event argument</param>
		/// <returns>Return true if event has been handled; Otherwise return false to recall default operations.</returns>
		bool OnMouseLeave(CellMouseEventArgs e);

		/// <summary>
		/// This method will be invoked when mouse scrolled inside the body bounds.
		/// </summary>
		/// <param name="e">Mouse event argument</param>
		/// <returns>Return true if event has been handled; Otherwise return false to recall default operations</returns>
		void OnMouseWheel(CellMouseEventArgs e);

		/// <summary>
		/// This method will be invoked when any key pressed when body being focused.
		/// </summary>
		/// <param name="e">Mouse event argument.</param>
		/// <returns>Return true if event has been handled inside the body bounds; Otherwise return false to recall default operations.</returns>
		bool OnKeyDown(KeyCode e);

		/// <summary>
		/// This method will be invoked when any key released on this body.
		/// </summary>
		/// <param name="e">Mouse event argument.</param>
		/// <returns>Return true if event has been handled; Otherwise return false to recall default operations.</returns>
		bool OnKeyUp(KeyCode e);

		/// <summary>
		/// This method will be invoked when cell body is required to repaint on worksheet.
		/// </summary>
		/// <param name="dc">Drawing context used to paint the cell body.</param>
		void OnPaint(CellDrawingContext dc);

		/// <summary>
		/// This method will be invoked when the owner cell of this body begin to edit. (Changing to editing mode)
		/// </summary>
		/// <returns>Return true to allow editing; Otherwise return false to abort editing operation.</returns>
		bool OnStartEdit();

		/// <summary>
		/// Determines whether or not become disable when owner cell is set as read-only. 
		/// </summary>
		bool DisableWhenCellReadonly { get; }

		/// <summary>
		/// This method will be invoked when the owner cell of this body finished edit.
		/// </summary>
		/// <param name="data">The data of user inputted.</param>
		/// <returns>Data used to be set into the cell. If don't want to change user data, return the data from method parameter.</returns>
		object OnEndEdit(object data);

		/// <summary>
		/// This method invoked when cell getting focus.
		/// </summary>
		void OnGotFocus();

		/// <summary>
		/// This method invoked when cell losing focus.
		/// </summary>
		void OnLostFocus();

		/// <summary>
		/// This method invoked when cell data was updated.
		/// </summary>
		/// <param name="data">The data will be set into the cell.</param>
		/// <returns>Return the new data used to set into the cell.</returns>
		object OnSetData(object data);

		/// <summary>
		/// Clone a cell body from this object.
		/// </summary>
		/// <returns>New instance of cell body.</returns>
		ICellBody Clone();
	}
}
