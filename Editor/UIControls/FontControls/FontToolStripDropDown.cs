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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.ComponentModel;

using unvell.Common;

namespace unvell.UIControls
{
	public class FontToolStripDropDown : ToolStripComboBox
	{
		public FontToolStripDropDown()
		{
			ComboBox.DrawMode = DrawMode.OwnerDrawFixed;
			ComboBox.DropDownHeight = 400;
			ComboBox.DrawItem += new DrawItemEventHandler(ComboBox_DrawItem);

			foreach (var family in FontFamily.Families)
			{
				ComboBox.Items.Add(new FontFamilyInfo(family));
			}

			if (ComboBox.Items.Count > 0) ComboBox.Text = Font.FontFamily.Name;
		}

		[DefaultValue(500)]
		public new int DropDownHeight
		{
			get
			{
				return base.DropDownHeight;
			}
			set { base.DropDownHeight = value; }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object Items { get { return null; } set { } }

		void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			Graphics g = e.Graphics;

			e.DrawBackground();

			FontUIToolkit.DrawFontItem(g, (FontFamilyInfo)ComboBox.Items[e.Index], e.Bounds,
				(e.State & DrawItemState.Selected) == DrawItemState.Selected);
		}
	}


}
