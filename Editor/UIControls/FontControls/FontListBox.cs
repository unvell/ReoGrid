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

using unvell.Common;

namespace unvell.UIControls
{
	public class FontListBox : ListBox
	{
		public FontListBox()
		{
			DrawMode = DrawMode.OwnerDrawFixed;
			ItemHeight = 20;
			DrawItem += new DrawItemEventHandler(FontListBox_DrawItem);

			foreach (FontFamily family in FontFamily.Families)
			{
				base.Items.Add(family);
			}
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			//			return base.ProcessCmdKey(ref msg, keyData);
			return false;
		}

		protected override bool ProcessDialogChar(char charCode)
		{
			int start = ((FontFamilyInfo)base.SelectedItem).Names.Any(n => n[0] == charCode) ?
				base.SelectedIndex + 1 : 0;

			for (int i = start; i < base.Items.Count; i++)
			{
				if (((FontFamilyInfo)base.Items[i]).Names.Any(n => n[0] == charCode))
				{
					base.SelectedIndex = i;
					break;
				}
			}

			return base.ProcessDialogChar(charCode);
		}

		[System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public new ObjectCollection Items { get { return base.Items; } }

		void FontListBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			Graphics g = e.Graphics;
			e.DrawBackground();

			FontUIToolkit.DrawFontItem(g, (FontFamilyInfo)base.Items[e.Index], e.Bounds,
				(e.State & DrawItemState.Selected) == DrawItemState.Selected);
		}
	}
}
