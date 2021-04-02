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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace unvell.UIControls
{
	/// <summary>
	/// Line with text label control
	/// </summary>
	public class FormLine : Control
	{
		/// <summary>
		/// Create control
		/// </summary>
		public FormLine()
		{
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			BackColor = Color.Transparent;
		}

		private bool show3DLine = true;

		/// <summary>
		/// Indicates whether or not to show line in 3D style.
		/// </summary>
		[DefaultValue(true)]
		public virtual bool Show3DLine
		{
			get { return show3DLine; }
			set { show3DLine = value; Invalidate(); }
		}

		private Color lineColor;

		/// <summary>
		/// Get or set line color
		/// </summary>
		public virtual Color LineColor
		{
			get { return lineColor; }
			set { lineColor = value; Invalidate(); }
		}

		/// <summary>
		/// Get or set label text
		/// </summary>
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
				Invalidate();
			}
		}

		private int textPadding = 14;

		/// <summary>
		/// Get or set text padding.
		/// </summary>
		[DefaultValue(14)]
		public virtual int TextPadding
		{
			get { return textPadding; }
			set { textPadding = value; Invalidate(); }
		}

		/// <summary>
		/// Repaint control.
		/// </summary>
		/// <param name="e">Paint event argument.</param>
		protected override void OnPaint(PaintEventArgs e)
		{
			int x = ClientRectangle.Left;
			int x2 = ClientRectangle.Right - 1;

			int c = ClientRectangle.Height / 2;

			Graphics g = e.Graphics;

			Rectangle textRect = Rectangle.Empty;

			if (!string.IsNullOrEmpty(Text))
			{
				using (StringFormat sf = new StringFormat(StringFormat.GenericTypographic))
				{
					sf.Alignment = StringAlignment.Center;
					sf.LineAlignment = StringAlignment.Center;

					SizeF textSize = g.MeasureString(Text, Font, ClientRectangle.Width, sf);
					textRect = new Rectangle(x, 0, (int)textSize.Width + TextPadding, ClientRectangle.Height - 1);

					using (Brush b = new SolidBrush(ForeColor))
					{
						g.DrawString(Text, Font, b, textRect, sf);
					}
				}
			}

			using (Pen p = new Pen(LineColor))
			{
				g.DrawLine(p, x + textRect.Width, c, x2, c);
			}

			if (show3DLine)
			{
				using (Pen p = new Pen(ControlPaint.LightLight(LineColor)))
				{
					g.DrawLine(p, x + textRect.Width, c + 1, x2, c + 1);
				}
			}
		}
	}
}
