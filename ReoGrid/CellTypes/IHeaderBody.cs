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

namespace unvell.ReoGrid.CellTypes
{
	/// <summary>
	/// Represent the interface of row and column header body
	/// </summary>
	public interface IHeaderBody
	{
		/// <summary>
		/// Onwer drawing
		/// </summary>
		/// <param name="dc">Drawing context</param>
		/// <param name="headerSize">Header size</param>
		void OnPaint(CellDrawingContext dc, Size headerSize);

		/// <summary>
		/// Mouse move event
		/// </summary>
		/// <param name="headerSize">Header size</param>
		/// <param name="e">Event argument</param>
		/// <returns>true if this event is handled</returns>
		bool OnMouseMove(Size headerSize, WorksheetMouseEventArgs e);

		/// <summary>
		/// Mouse down event
		/// </summary>
		/// <param name="headerSize">Header size</param>
		/// <param name="e">Event argument</param>
		/// <returns>true if this event is handled</returns>
		bool OnMouseDown(Size headerSize, WorksheetMouseEventArgs e);

		/// <summary>
		/// Event when data in any cells on this header is changed
		/// </summary>
		/// <param name="startRow">Zero-based number of row of changed cells</param>
		/// <param name="endRow">Zero-based number of column of changed cells</param>
		void OnDataChange(int startRow, int endRow);
	}

	/// <summary>
	/// Represent the interface of row and column header body
	/// </summary>
	public class HeaderBody : IHeaderBody
	{
		/// <summary>
		/// Paint this header body.
		/// </summary>
		/// <param name="dc">Drawing context</param>
		/// <param name="headerSize">Header size</param>
		public virtual void OnPaint(CellDrawingContext dc, Size headerSize) { }

		/// <summary>
		/// Method raised when mouse moving inside this body.
		/// </summary>
		/// <param name="headerSize">Header size</param>
		/// <param name="e">Event argument</param>
		/// <returns>true if this event is handled</returns>
		public virtual bool OnMouseMove(Size headerSize, WorksheetMouseEventArgs e) { return false; }

		/// <summary>
		/// Method raised when mouse pressed inside this body.
		/// </summary>
		/// <param name="headerSize">Header size</param>
		/// <param name="e">Event argument</param>
		/// <returns>true if this event is handled</returns>
		public virtual bool OnMouseDown(Size headerSize, WorksheetMouseEventArgs e) { return false; }

		/// <summary>
		/// Method raised when data changed from cells on this header.
		/// </summary>
		/// <param name="startRow">Zero-based number of row of changed cells.</param>
		/// <param name="endRow">Zero-based number of column of changed cells.</param>
		public virtual void OnDataChange(int startRow, int endRow) { }
	}
}
