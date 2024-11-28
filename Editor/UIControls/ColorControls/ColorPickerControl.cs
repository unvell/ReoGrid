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

using unvell.Common;

namespace unvell.UIControls
{
	public class ColorPickerControl : Control
	{
		private AbstractColor currentColor;

		public AbstractColor CurrentColor
		{
			get { return currentColor; }
			set
			{
				if (currentColor != value)
				{
					currentColor = value;
					dropPanel.CurrentColor = value;
					Invalidate();
				}
			}
		}

		public Color SolidColor
		{
			get { return currentColor is SolidColor ? ((SolidColor)currentColor).Color : Color.Empty; }
			set { CurrentColor = new SolidColor(value); }
		}

		private ColorPickerWindow dropPanel = new ColorPickerWindow();

		private static readonly StringFormat sf = new StringFormat();

		public ColorPickerControl()
		{
			DoubleBuffered = true;

			dropPanel.ColorPicked += new EventHandler(dropPanel_ColorPicked);
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;

			dropPanel.VisibleChanged += dropPanel_VisibleChanged;
		}

		private bool showShadow;

		public bool ShowShadow
		{
			get { return showShadow; }
			set { showShadow = value; }
		}

		#region Draw
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
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
			//g.FillRectangle(Brushes.Gainsboro, bodyRect);

			// shadow
			if (showShadow)
				g.DrawLines(isPressed ? SystemPens.ControlLightLight
					: SystemPens.ControlDarkDark, new Point[] {
						new Point(rect.Left+1,rect.Bottom-1),
						new Point(rect.Right-1,rect.Bottom-1),
						new Point(rect.Right-1,rect.Top+1),
				});

			// draw allow
			if (isPressed) rect.Offset(1, 1);

			try
			{
				g.FillPolygon(Brushes.Black, new Point[] { 
					new Point(rect.Right-13, rect.Top+8),
					new Point(rect.Right-4, rect.Top+8),
					new Point(rect.Right-9, rect.Top+17),
				});
			}
			catch { }

			g.DrawLine(SystemPens.ControlDarkDark, rect.Right - 16, rect.Top + 3,
				rect.Right - 16, rect.Bottom - 4);
			g.DrawLine(SystemPens.ControlLightLight, rect.Right - 15, rect.Top + 4,
				rect.Right - 15, rect.Bottom - 4);


			// draw color
			Rectangle colorRect = new Rectangle(4, 4, 14, 14);
			if (isPressed) colorRect.Offset(1, 1);
			
			if (currentColor == null || currentColor.IsEmpty)
			{
				g.DrawRectangle(Pens.Black, colorRect);
				g.DrawLine(Pens.Black, colorRect.Left,
					colorRect.Bottom, colorRect.Right, colorRect.Top);
			}
			else if(currentColor is SolidColor)
			{
				//g.FillRectangle(ResourcePoolManager.Instance.GetBrush(((SolidColor)currentColor).Color), colorRect);
				Color c = currentColor.CompliantSolidColor;
				if (c.A < 255)
				{
					GraphicsToolkit.DrawTransparentBlock(g, colorRect);
				}

				using (SolidBrush b = new SolidBrush(currentColor.CompliantSolidColor))
				{
					g.FillRectangle(b, colorRect);
				}
			}
			else if (currentColor is LinearGradientColor)
			{
				LinearGradientColor lgc = (LinearGradientColor)currentColor;
				using (LinearGradientBrush b = lgc.CreateGradientBrush(colorRect))
				{
					g.PixelOffsetMode = PixelOffsetMode.Half;
					g.FillRectangle(b, colorRect);
					g.PixelOffsetMode = PixelOffsetMode.Default;
				}
			}
			else if (currentColor is HatchPatternColor)
			{
				using (HatchBrush b = ((HatchPatternColor)currentColor).CreateHatchBrush())
				{
					g.FillRectangle(b, colorRect);
				}
			}
		}
		#endregion

		void dropPanel_ColorPicked(object sender, EventArgs e)
		{
			currentColor = dropPanel.CurrentColor;

			//if (currentColor is SolidColor)
			//{
			//	dropPanel.Visible = false;
			//}
			
			Invalidate();

			if (ColorPicked != null)
				ColorPicked.Invoke(this, new EventArgs());
		}

		public event EventHandler ColorPicked;

		void dropPanel_VisibleChanged(object sender, EventArgs e)
		{
			if (!dropPanel.Visible)
			{
				isPressed = false;
			}
			Invalidate();
		}

		private bool isPressed = false;
		protected override void OnMouseDown(MouseEventArgs e)
		{
			isPressed = true;

			Rectangle panelRect = Parent.RectangleToScreen(Bounds);
			dropPanel.CurrentColor = CurrentColor;
			dropPanel.Show(this, 0, Height);
		//	dropPanel.Capture = true;

			base.OnMouseDown(e);
		}
	}
}
