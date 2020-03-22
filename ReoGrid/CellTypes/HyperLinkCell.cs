/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * http://reogrid.net/
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * Author: Jing <lujing at unvell.com>
 *
 * Copyright (c) 2012-2016 Jing <lujing at unvell.com>
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
using unvell.ReoGrid.Interaction;

namespace unvell.ReoGrid.CellTypes
{
	/// <summary>
	/// Representation for hyperlink of cell body.
	/// </summary>
	[Serializable]
	public class HyperlinkCell : CellBody
	{
		/// <summary>
		/// Get or set color of mouse-hover status.
		/// </summary>
		public SolidColor ActivateColor { get; set; }

		/// <summary>
		/// Get or set color of normal hyperlink.
		/// </summary>
		public SolidColor LinkColor { get; set; }

		/// <summary>
		/// Get or set color of visited status.
		/// </summary>
		public SolidColor VisitedColor { get; set; }

		/// <summary>
		/// Get or set the navigation url. (Redirected when AutoNavigation property is true)
		/// </summary>
		public string LinkURL { get; set; }

		/// <summary>
		/// Create instane of hyperlink cell body with specified navigation url and AutoNavigate property.
		/// </summary>
		/// <param name="navigationURL">Navigation url redirected to when hyperlink clicked. (Default is emtpy)</param>
		/// <param name="autoNavigate">Determine whether or not redirect to specified url 
		/// when hyperlink clicked automatically. (Default is true)</param>
		public HyperlinkCell(string navigationURL = null, bool autoNavigate = true)
		{
			this.ActivateColor = SolidColor.Red;
			this.LinkColor = SolidColor.Blue;
			this.VisitedColor = SolidColor.Purple;

			this.LinkURL = navigationURL;
			this.AutoNavigate = autoNavigate;
		}

		/// <summary>
		/// Determine whether or not the mouse is over the hyperlink.
		/// </summary>
		public bool IsOverLink { get; set; }

		/// <summary>
		/// Initialize cell body when set up into a cell.
		/// </summary>
		/// <param name="cell">Instance of cell to be set up.</param>
		public override void OnSetup(Cell cell)
		{
			// set cell text style
			cell.Style.TextColor = LinkColor;
			cell.Style.Underline = true;

			// set cell default value
			if (this.LinkURL != null)
			{
				cell.Data = this.LinkURL;
			}
			else if (cell.InnerData != null)
			{
				this.LinkURL = Convert.ToString(cell.InnerData);
			}
		}

		/// <summary>
		/// Change color of hyperlink to activate-status when mouse button pressed inside cell.
		/// </summary>
		/// <param name="e">Event argument of cell body mouse-down.</param>
		/// <returns>True if event has been handled.</returns>
		public override bool OnMouseDown(CellMouseEventArgs e)
		{
			if (this.IsOverLink)
			{
				e.Cell.Style.TextColor = ActivateColor;
			}

			return base.OnMouseDown(e);
		}

		/// <summary>
		/// Restore color of hyperlink to normal-status or hover-status when mouse button was released from cell.
		/// </summary>
		/// <param name="e">Event argument of cell body mouse-up.</param>
		/// <returns>True if event has been handled.</returns>
		public override bool OnMouseUp(CellMouseEventArgs e)
		{
			if (this.IsOverLink)
			{
				this.IsOverLink = false;
				e.Cell.Style.TextColor = VisitedColor;
				this.PerformClick();
				return true;
			}

			return base.OnMouseUp(e);
		}
		
		/// <summary>
		/// Determine whether the mouse is over the link and change the cursor accordingly.
		/// </summary>
		/// <param name="e">Event argument of cell body mouse-enter.</param>
		/// <returns>True if event has been handled.</returns>
		public override bool OnMouseMove(CellMouseEventArgs e)
		{
			if (e.Cell.TextBounds.Contains(e.AbsolutePosition))
			{
				if (!this.IsOverLink)
				{
					this.IsOverLink = true;
					e.Worksheet.controlAdapter.ChangeSelectionCursor(CursorStyle.Hand);
				}
			}
			else
			{
				if (this.IsOverLink)
				{
					this.IsOverLink = false;
					e.Worksheet.controlAdapter.ChangeSelectionCursor(CursorStyle.PlatformDefault);
				}
			}

			return base.OnMouseMove(e);
		}		
		
		/// <summary>
		/// Restore color of hyperlink from hover-status when mouse leaved from cell.
		/// </summary>
		/// <param name="e">Argument of mouse leaving event.</param>
		/// <returns>True if this event has been handled; Otherwise return false.</returns>
		public override bool OnMouseLeave(CellMouseEventArgs e)
		{
			// change current cursor to hand
			if (this.IsOverLink)
			{
				this.IsOverLink = false;
				e.Worksheet.controlAdapter.ChangeSelectionCursor(CursorStyle.PlatformDefault);
			}
			return base.OnMouseLeave(e);
		}

		/// <summary>
		/// Handle event if cell has lost focus.
		/// </summary>
		public override void OnLostFocus()
		{
			if (this.IsOverLink)
			{
				this.IsOverLink = false;
			}
		}

		/// <summary>
		/// Manually fire the hyperlink click event.
		/// </summary>
		public void PerformClick()
		{
			if (AutoNavigate && !string.IsNullOrEmpty(LinkURL))
			{
				try
				{
					System.Diagnostics.Process.Start(LinkURL);
				}
				catch { }
			}

			Click?.Invoke(this, null);
		}

		/// <summary>
		/// When data of cell set, read navigation url from cell data
		/// </summary>
		/// <param name="data">New data to be set</param>
		/// <returns>Replacement data if needed</returns>
		public override object OnSetData(object data)
		{
			LinkURL = Convert.ToString(data);
			return data;
		}

		/// <summary>
		/// Event raised when hyperlink was preseed
		/// </summary>
		public event EventHandler Click;

		/// <summary>
		/// Determine whether or not redirect to navigation url when hyperlink was pressed
		/// </summary>
		public bool AutoNavigate { get; set; }
	}
}
