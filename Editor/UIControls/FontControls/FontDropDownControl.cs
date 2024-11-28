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
 * ReoGrid and ReoGridEditor is released under MIT license\.
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using unvell.Common;

namespace unvell.UIControls
{
	public partial class FontDropDownControl : ComboBox
	{
		public FontDropDownControl()
		{
			DropDownStyle = ComboBoxStyle.DropDownList;
			DrawMode = DrawMode.OwnerDrawFixed;
			DropDownHeight = 500;
			ItemHeight = 20;
			DrawItem += new DrawItemEventHandler(ComboBox_DrawItem);

			foreach (FontFamily family in FontFamily.Families)
			{
				base.Items.Add(family);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object Items { get { return null; } set { } }

		void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			Graphics g = e.Graphics;

			e.DrawBackground();

			if (e.Index >= 0 && e.Index < base.Items.Count)
			{
				FontUIToolkit.DrawFontItem(g, (FontFamilyInfo)base.Items[e.Index], e.Bounds,
					(e.State & DrawItemState.Selected) == DrawItemState.Selected);
			}
		}

		public FontFamilyInfo SelectedFontFamily
		{
			get
			{
				return ((FontFamilyInfo)base.SelectedItem);
			}
			set
			{
				base.SelectedItem = value;
			}
		}
	}
}
