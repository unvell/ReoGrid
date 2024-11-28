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
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Diagnostics;

namespace unvell.UIControls
{
	public class FillPatternComboBox : Control
	{
		private FillPatternPickWindow pickerWindow = new FillPatternPickWindow();

		public FillPatternComboBox()
		{
			TabStop = true;
			DoubleBuffered = true;
			BackColor = Color.White;

			pickerWindow.VisibleChanged += (sender, e) =>
			{
				if (!pickerWindow.Visible)
				{
					Pullup();
				}
			};

			pickerWindow.PatternPicked += (sender, e) =>
			{
				if (CloseOnClick) Pullup();
				SelectPattern();
			};
		}

		public bool CloseOnClick { get; set; }

		public void SelectPattern()
		{
			if (PatternSelected != null) PatternSelected(this, null);
			Invalidate();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			buttonRect = new Rectangle(ClientRectangle.Right - 22, ClientRectangle.Top + 1, 21, ClientRectangle.Bottom - 2);
			patternRect = new Rectangle(ClientRectangle.Left + 6, ClientRectangle.Top + 6,
				ClientRectangle.Width - 32, ClientRectangle.Bottom - 12);
		}

		public HatchStyle PatternStyle
		{
			get { return pickerWindow.PatternStyle; }
			set { pickerWindow.PatternStyle = value; Invalidate(); }
		}

		[DefaultValue(false)]
		public bool HasPatternStyle
		{
			get { return pickerWindow.HasPatternStyle; }
			set { pickerWindow.HasPatternStyle = value; Invalidate(); }
		}

		private Color patternColor = Color.Black;

		public Color PatternColor
		{
			get { return patternColor; }
			set { patternColor = value; }
		}

		private bool pressed = false;

		public bool dropdown
		{
			get { return pressed; }
			set { pressed = value; }
		}

		public ToolStripItem OwnerItem
		{
			set { pickerWindow.OwnerItem = value; }
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			Focus();

			if (dropdown)
			{
				Pullup();
			}
			else
			{
				Dropdown();
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Space:
				case Keys.Enter:
					if (dropdown) Pullup(); else Dropdown();
					break;

				default:
					base.OnKeyDown(e);
					break;
			}
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			Invalidate();
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			Invalidate();
		}

		Rectangle patternRect;
		Rectangle buttonRect;

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			if (!HasPatternStyle)
			{
				g.DrawRectangle(Pens.Black, patternRect);
			}
			else
			{
				using (HatchBrush hb = new HatchBrush(pickerWindow.PatternStyle, patternColor, Color.White))
				{
					g.FillRectangle(hb, patternRect);
				}
			}

			if (Focused)
			{
				Rectangle focusRect = patternRect;
				focusRect.Inflate(3, 3);
				ControlPaint.DrawFocusRectangle(g, focusRect);
			}

			ControlPaint.DrawComboButton(g, buttonRect, pressed ? ButtonState.Pushed : ButtonState.Normal);
			ControlPaint.DrawBorder3D(g, ClientRectangle, Border3DStyle.Sunken);
		}

		public void Dropdown()
		{
			pickerWindow.Show(this, 0, Height);

			dropdown = true;
			Invalidate();
		}

		public void Pullup()
		{
			dropdown = false;
			pickerWindow.Close(ToolStripDropDownCloseReason.ItemClicked);
			Invalidate();
		}

		public event EventHandler PatternSelected;
	}

	internal class FillPatternPickWindow : ToolStripDropDown
	{
		private FillPatternPanel panel = new FillPatternPanel();
		
		private ToolStripControlHost controlHost;

		public HatchStyle PatternStyle
		{
			get { return panel.PatternStyle; }
			set { panel.PatternStyle = value; }
		}
	
		public bool HasPatternStyle
		{
			get { return panel.HasPatternStyle; }
			set { panel.HasPatternStyle = value; }
		}

		public FillPatternPickWindow()
			: base()
		{
			this.TabStop = true;
			this.Margin = this.Padding = new Padding(1);
			this.AutoSize = false;

			panel.Dock = DockStyle.Fill;
			panel.Location = new Point(0, 0);

			panel.PatternPicked += (s, e) =>
			{
				if (PatternPicked != null) PatternPicked(this, null);
			};

			controlHost = new ToolStripControlHost(panel);
			controlHost.AutoSize = false;

			Items.Add(controlHost);

			this.Size = new Size(265, 220);
		}
	
		public event EventHandler PatternPicked;
	
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (controlHost != null) controlHost.Size = new Size(ClientRectangle.Width - 2, ClientRectangle.Height - 3);
		}
	}

	internal class FillPatternPanel : Control
	{
		private HatchStyle patternStyle;

		public HatchStyle PatternStyle
		{
			get { return patternStyle; }
			set { patternStyle = value; }
		}

		private bool hasPatternStyle = false;

		public bool HasPatternStyle
		{
			get { return hasPatternStyle; }
			set { hasPatternStyle = value; }
		}

		private Item[] items;

		public FillPatternPanel()
		{
			BackColor = SystemColors.Window;
			DoubleBuffered = true;
			TabStop = true;
			Padding = new Padding(10);

			HatchStyle[] styles = (HatchStyle[])Enum.GetValues(typeof(HatchStyle));
			items = new Item[styles.Length];
		}

		private Size gridSize = new Size(30, 20);

		public Size GridSize
		{
			get { return gridSize; }
			set { gridSize = value; }
		}

		private short spacing = 5;

		public short Spacing
		{
			get { return spacing; }
			set { spacing = value; }
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			HatchStyle[] styles = (HatchStyle[])Enum.GetValues(typeof(HatchStyle));

			int cols = (ClientRectangle.Width - Padding.Left+Padding.Right) / (gridSize.Width + spacing);

			int x = Padding.Left;
			int y = Padding.Top;

			Rectangle rect = new Rectangle(x, y, gridSize.Width, gridSize.Height);

			int i = 0;
			foreach (HatchStyle style in styles)
			{
				items[i].rect = rect;
				items[i].style = style;

				rect.Offset(gridSize.Width + spacing, 0);

				i++;

				if (i % cols == 0)
				{
					rect.X = Padding.Top;
					rect.Y += gridSize.Height + spacing;
				}
			}

			Invalidate();
		}

		private short currentHover = -1;

		protected override void OnMouseMove(MouseEventArgs e)
		{
			for (short i = 0; i < items.Length; i++)
			{
				if (items[i].rect.Contains(e.Location))
				{
					if (currentHover != i)
					{
						currentHover = i;
						Invalidate();
					}
					break;
				}
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			for (short i = 0; i < items.Length; i++)
			{
				if (items[i].rect.Contains(e.Location))
				{
					patternStyle = items[i].style;
					hasPatternStyle = i > 0;
					if (PatternPicked != null) PatternPicked(this, null);
					break;
				}
			}
		}

		public event EventHandler PatternPicked;

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			for (int i = 0; i < items.Length; i++)
			{
				Rectangle rect = items[i].rect;

				if (currentHover == i)
				{
					Rectangle highlightRect = new Rectangle(rect.X - 1, rect.Y - 1, rect.Width + 3, rect.Height + 3);

					using (Pen p = new Pen(SystemColors.Highlight, 2))
					{
						g.DrawRectangle(p, highlightRect);
					}
				}

				if (i == 0)
				{
					g.DrawRectangle(Pens.Black, rect);
				}
				else
				{
					using (HatchBrush hb = new HatchBrush(items[i].style, Color.Black, BackColor))
					{
						g.FillRectangle(hb, rect);
						g.DrawRectangle(Pens.Black, rect);
					}
				}
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
		}

		struct Item
		{
			internal HatchStyle style;
			internal Rectangle rect;
		}
	}
}

