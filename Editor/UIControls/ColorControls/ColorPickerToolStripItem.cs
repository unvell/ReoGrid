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
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace unvell.UIControls
{
	public class ColorPickerToolStripItem : ToolStripItem
	{
		private ColorPickMode mode = ColorPickMode.Background;

		[DefaultValue(ColorPickMode.Background)]
		public ColorPickMode Mode
		{
			get { return mode; }
			set
			{
				if (mode != value)
				{
					mode = value;
					Invalidate();
				}
			}
		}

		private ColorPickerWindow dropPanel = new ColorPickerWindow();

		private static readonly StringFormat sf = new StringFormat();

		public ColorPickerToolStripItem()
			: base()
		{
			Width = 39;

			dropPanel.ColorPicked += new EventHandler(dropPanel_ColorPicked);
			dropPanel.OwnerItem = this;

			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;
			dropPanel.VisibleChanged += new EventHandler(dropPanel_VisibleChanged);
		}

		public override Size GetPreferredSize(Size constrainingSize)
		{
			return new Size(39, 23);
		}

		public bool CloseOnClick { get; set; }

		void dropPanel_ColorPicked(object sender, EventArgs e)
		{
			currentColor = dropPanel.CurrentColor;

			if(CloseOnClick && currentColor is SolidColor)
			{
				dropPanel.Close(ToolStripDropDownCloseReason.ItemClicked);
			}
			
			Invalidate();
			
			if (currentColor is SolidColor)
			{
				SolidColorPickerPanel.AddRecentColor(((SolidColor)currentColor).Color);
			}

			if (ColorPicked != null)
			{
				ColorPicked.Invoke(this, new EventArgs());
			}
		}

		public event EventHandler ColorPicked;

		void dropPanel_VisibleChanged(object sender, EventArgs e)
		{
			if (!dropPanel.Visible)
			{
				isPressed = false;
				Invalidate();
			}
		}

		private bool isPressed = false;
		
		protected override void OnMouseDown(MouseEventArgs e)
		{
			isPressed = true;

			Rectangle panelRect = Parent.RectangleToScreen(Bounds);
			dropPanel.CurrentColor = CurrentColor;
			dropPanel.Show(panelRect.X, panelRect.Y + panelRect.Height);

			base.OnMouseDown(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			Rectangle rect = new Rectangle(0, 0, Size.Width - 1, Size.Height - 1);

			// background
			Rectangle bgRect = rect;
			bgRect.Offset(1, 1);
			//g.FillRectangle(isPressed ? Brushes.Black : Brushes.White, bgRect);

			// outter frame
			g.DrawLine(SystemPens.ButtonShadow, rect.X + 1, rect.Y, rect.Right - 1, rect.Y);
			g.DrawLine(SystemPens.ButtonShadow, rect.X + 1, rect.Bottom, rect.Right - 1, rect.Bottom);
			g.DrawLine(SystemPens.ButtonShadow, rect.X, rect.Y + 1, rect.X, rect.Bottom - 1);
			g.DrawLine(SystemPens.ButtonShadow, rect.Right, rect.Y + 1, rect.Right, rect.Bottom - 1);

			// content
			Rectangle bodyRect = rect;
			bodyRect.Inflate(-1, -1);
			bodyRect.Offset(1, 1);

			// draw allow
			if (isPressed) rect.Offset(1, 1);

			//GraphicsToolkit.FillTriangle(g, 9, new Point(rect.Right - 9, rect.Top + 8));
			try
			{
				g.FillPolygon(SystemBrushes.WindowText, new Point[] { 
					new Point(rect.Right-12, rect.Top+8),
					new Point(rect.Right-5, rect.Top+8),
					new Point(rect.Right-9, rect.Top+15),
				});

				g.DrawLine(SystemPens.ControlDarkDark, rect.Right - 16, rect.Top + 3, rect.Right - 16, rect.Bottom - 4);
				g.DrawLine(SystemPens.ControlLightLight, rect.Right - 15, rect.Top + 4, rect.Right - 15, rect.Bottom - 4);
			}
			catch { }

			// draw color
			Rectangle rectColor = new Rectangle(4, 4, 14, 14);
			if (isPressed) rectColor.Offset(1, 1);
			if (currentColor == null || currentColor.IsEmpty)
			{
				g.DrawRectangle(Pens.Black, rectColor);
				g.DrawLine(Pens.Black, rectColor.Left,
					rectColor.Bottom, rectColor.Right, rectColor.Top);
			}
			else
			{
				Brush b = null;

				if (currentColor is SolidColor)
				{
					b = new SolidBrush(((SolidColor)currentColor).Color);
				}
				else if (currentColor is LinearGradientColor)
				{
					b = ((LinearGradientColor)currentColor).CreateGradientBrush(rectColor);
				}
				else if (currentColor is HatchPatternColor)
				{
					b = ((HatchPatternColor)currentColor).CreateHatchBrush();
				}

				switch (mode)
				{
					default:
					case ColorPickMode.Background:
						g.PixelOffsetMode = PixelOffsetMode.Half;
						g.FillRectangle(b, rectColor);
						g.PixelOffsetMode = PixelOffsetMode.Default;
						break;
					case ColorPickMode.Outline:
						using (Pen p = new Pen(currentColor.CompliantSolidColor, 2))
							g.DrawRectangle(p, rectColor);
						break;
					case ColorPickMode.Font:
						g.DrawString("A", Font, b, new Rectangle(rectColor.Left, rectColor.Top - 3,
							rectColor.Width, rectColor.Height));
						g.FillRectangle(b, new Rectangle(rectColor.Left, rectColor.Bottom - 3, rectColor.Width, 3));
						break;
					case ColorPickMode.FontBackground:
						g.FillRectangle(b, rectColor);
						g.DrawString("A", Font, b, rectColor);
						break;
				}

				if (b != null)
				{
					b.Dispose();
					b = null;
				}
			}
		}

		private AbstractColor currentColor;

		public AbstractColor CurrentColor
		{
			get { return currentColor; }
			set
			{
				currentColor = value;
				Invalidate();
			}
		}

		public Color SolidColor
		{
			get { return currentColor is SolidColor ? ((SolidColor)currentColor).Color : Color.Empty; }
			set { currentColor = new SolidColor(value); Invalidate(); }
		}
	}

	public enum ColorPickMode
	{
		Background,
		Outline,
		Font,
		FontBackground,
		None,
	}

}
