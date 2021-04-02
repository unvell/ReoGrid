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

#if DRAWING

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Interaction;

namespace unvell.ReoGrid.Drawing
{
	/// <summary>
	/// Represents mouse event arguments.
	/// </summary>
	public class MouseEventArgs : EventArgs
	{
		/// <summary>
		/// Get the location of mouse event happened.
		/// </summary>
		public Point Location { get; protected set; }

		/// <summary>
		/// Get mouse button press-down status of mouse event.
		/// </summary>
		public MouseButtons Buttons { get; protected set; }

		/// <summary>
		/// Create an instance of mouse event arguments.
		/// </summary>
		/// <param name="location">The location of mouse event happened.</param>
		/// <param name="buttons">The mouse button press-down status of mous event happened.</param>
		public MouseEventArgs(Point location, MouseButtons buttons)
		{
			this.Location = location;
			this.Buttons = buttons;
		}
	}

	/// <summary>
	/// Keyboard event arguments
	/// </summary>
	public class KeyboardEventArgs : EventArgs
	{
		/// <summary>
		/// Get the merged key press-down status code
		/// </summary>
		public KeyCode Keys { get; protected set; }

		/// <summary>
		/// Create an instance of keyboard event arguments
		/// </summary>
		/// <param name="keys">The key of user pressed down when event happening</param>
		public KeyboardEventArgs(KeyCode keys)
		{
			this.Keys = keys;
		}
	}	
}

#endif // DRAWING