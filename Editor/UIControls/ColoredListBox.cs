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
 * ReoGrid and ReoGridEditor is released under MIT license.
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace unvell.UIControls
{
	/// <summary>
	/// A list box which can have colored list items, extended from a standard .NET list box.
	/// </summary>
	public class ColoredListBox : ListBox
	{
		public ColoredListBox()
			: base()
		{
			base.DrawMode = DrawMode.OwnerDrawFixed;
		}

		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			e.DrawBackground();

			if (Items.Count <= 0)
			{
				base.OnDrawItem(e);
				return;
			}

			var item = this.Items[e.Index];

			Color textColor = SystemColors.WindowText;
			Color backColor = Color.Empty;

			if (item is IColoredListBoxItem)
			{
				var coloredItem = ((IColoredListBoxItem)item);
        textColor = coloredItem.TextColor;
				backColor = coloredItem.BackColor;
			}

			if (textColor == SystemColors.WindowText 
				&& ((e.State & DrawItemState.Selected) == DrawItemState.Selected))
				textColor = SystemColors.HighlightText;

			var g = e.Graphics;

			// draw background if has back color 
			if (backColor != Color.Empty)
			{
				using (var b = new SolidBrush(backColor))
				{
					g.FillRectangle(b, e.Bounds);
				}
			}

			using (Brush b = new SolidBrush(textColor))
			{
				e.Graphics.DrawString(Convert.ToString(Items[e.Index]), e.Font, b, e.Bounds);
			}

			// draw default focus
			if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
			{
				ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds);
			}
		}
	}

	/// <summary>
	/// Interface that represents colored list box items.
	/// </summary>
	public interface IColoredListBoxItem
	{
		/// <summary>
		/// Get the text color for this list item.
		/// </summary>
		Color TextColor { get; }

		/// <summary>
		/// Get the background color for this list item.
		/// </summary>
		Color BackColor { get; }
	}
}
