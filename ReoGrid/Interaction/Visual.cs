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

using System.Collections.Generic;

using unvell.ReoGrid.Graphics;

#if WINFORM || ANDROID
using RGFloat = System.Single;
#else
using RGFloat = System.Double;
#endif // WINFORM

namespace unvell.ReoGrid.Interaction
{
	/// <summary>
	/// Represents an user-interactive object in ReoGrid cross-platform views system.
	/// </summary>
	public interface IUserVisual
	{
		/// <summary>
		/// Handle mouse down event
		/// </summary>
		/// <param name="location">Transformed relative location to this object</param>
		/// <param name="buttons">Current mouse button pressing status</param>
		/// <returns>True if event handled; otherwise return false</returns>
		bool OnMouseDown(Point location, MouseButtons buttons);

		/// <summary>
		/// Handle mouse move event
		/// </summary>
		/// <param name="location">Transformed relative location to this object</param>
		/// <param name="buttons">Current mouse button pressing status</param>
		/// <returns>True if event handled; otherwise return false</returns>
		bool OnMouseMove(Point location, MouseButtons buttons);

		/// <summary>
		/// Handle mouse up event
		/// </summary>
		/// <param name="location">Transformed relative location to this object</param>
		/// <param name="buttons">Current mouse button pressing status</param>
		/// <returns>True if event handled; otherwise return false</returns>
		bool OnMouseUp(Point location, MouseButtons buttons);

		/// <summary>
		/// Handle mouse double click event
		/// </summary>
		/// <param name="location">Transformed relative location to this object</param>
		/// <param name="buttons">Current mouse button pressing status</param>
		/// <returns>True if event handled; otherwise return false</returns>
		bool OnMouseDoubleClick(Point location, MouseButtons buttons);

		/// <summary>
		/// Handle key down event
		/// </summary>
		/// <param name="keys">ReoGrid virtual keys (equal to System.Windows.Forms.Keys)</param>
		/// <returns>True if event handled; otherwise return false</returns>
		bool OnKeyDown(KeyCode keys);

		/// <summary>
		/// Set this object to get user interface focus. Object after get focus can always 
		/// receive user's mouse and keyboard input.
		/// </summary>
		void SetFocus();

		/// <summary>
		/// Release user interface focus from this object. This object will no longer be able to 
		/// receive user's mouse and keyboard input.
		/// </summary>
		void FreeFocus();

		/// <summary>
		/// Redraw this object.
		/// </summary>
		void Invalidate();
	}
	
	public interface ISelectableVisual
	{
		bool IsSelected { get; set; }

		void OnSelect();

		void OnDeselect();
	}

	internal interface IThumbVisualObject
	{
		IEnumerable<ResizeThumb> ThumbPoints { get; }
	}

	public struct ResizeThumb
	{
		public ResizeThumbPosition Position;

		public Point Point;

		public ResizeThumb(ResizeThumbPosition position, Point point) 
		{
			this.Position = position;
			this.Point = point;
		}

		public ResizeThumb(ResizeThumbPosition position, RGFloat width, RGFloat height)
		{
			this.Position = position;
			this.Point = new Point(width, height);
		}
	}

	public enum ResizeThumbPosition
	{
		TopLeft,
		Top,
		TopRight,

		Left,
		Right,

		BottomLeft,
		Bottom,
		BottomRight,
	}

	public interface IVisualController
	{
		IUserVisual FocusVisual { get; }
	}

}
