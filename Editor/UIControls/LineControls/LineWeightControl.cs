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
	public partial class LineWeightControl : ComboBox
	{
		public LineWeightControl()
		{
			DrawMode = DrawMode.OwnerDrawFixed;

			base.Items.AddRange(new object[] { 0.2f, 0.5f, 1f, 1.5f, 2f, 2.5f, 3f, 4f, 5f, 7.5f, 10f });
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ObjectCollection Items { get { return base.Items; } set { } }

		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			Graphics g = e.Graphics;

			e.DrawBackground();

			if (e.Index >= 0 && e.Index < base.Items.Count)
			{
				float weight = (float)base.Items[e.Index];

				using (Brush b = new SolidBrush(
					(e.State & DrawItemState.Selected) == DrawItemState.Selected
					? SystemColors.HighlightText : SystemColors.WindowText))
				{
					using (Font font = new Font(SystemFonts.DefaultFont.FontFamily, 8f, FontStyle.Regular))
					{
						Rectangle rt = new Rectangle(e.Bounds.X, e.Bounds.Y, 22, e.Bounds.Height);

						using (StringFormat sf = new StringFormat()
						{
							Alignment = StringAlignment.Center
						})
						{
							g.DrawString(weight.ToString("0.#"), font, b, rt, sf);
						}
					}
				}

				int x = e.Bounds.X + 24;
				int y = e.Bounds.Y + e.Bounds.Height / 2 - 1;
				int x2 = e.Bounds.Right - 3;

				using (Pen p = new Pen(ForeColor, weight))
				{
					if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
					{
						p.Color = SystemColors.HighlightText;
					}

					g.DrawLine(p, x, y, x2, y);
				}
			}
		}
	}
}
