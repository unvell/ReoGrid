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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using unvell.Common;

namespace unvell.UIControls
{
	#region Panel
	public class ColorPickerPanel : Control
	{
		private FlatTabControl tab;

		private AbstractColor currentColor;

		public AbstractColor CurrentColor
		{
			get
			{
				if (currentColor == null)
				{
					currentColor = new SolidColor(solidPanel.CurrentColor);
				}
				return currentColor;
			}
			set
			{
				if (currentColor != value)
				{
					currentColor = value;

					if (currentColor is SolidColor)
					{
						tab.SelectedIndex = 0;
						solidPanel.CurrentColor = ((SolidColor)value).Color;
					}
				}
			}
		}

		public Color SolidColor
		{
			get
			{
				return currentColor == null ? solidPanel.CurrentColor : currentColor.CompliantSolidColor;
			}
			set { currentColor = new SolidColor(value); solidPanel.CurrentColor = value; }
		}

		private SolidColorPickerPanel solidPanel = new SolidColorPickerPanel();

		private List<Control> panels = new List<Control>();

		private Panel panel;

		public ColorPickerPanel()
			: base()
		{
			this.TabStop = false;
			this.Margin = this.Padding = new Padding(1);
			this.AutoSize = false;

			panel = new Panel();
			panel.TabStop = false;
			panel.AutoSize = false;
			panel.Location = new Point(0, 0);
			panel.Dock = DockStyle.Fill;

			tab = new FlatTabControl();
			tab.TabStop = false;
			tab.Tabs = new string[] { unvell.ReoGrid.Editor.LangRes.LangResource.SolidColor };
			tab.Size = new Size(ClientRectangle.Width, 20);
			tab.Dock = DockStyle.Top;
			tab.SelectedIndexChanged += (s, e) => panels[tab.SelectedIndex].BringToFront();

			Controls.Add(tab);
			Controls.Add(panel);

			solidPanel.Dock = DockStyle.Fill;
			solidPanel.ColorPicked += (s, e) =>
			{
				currentColor = null;// new SolidColor(colorPickerPanel.CurrentColor);
				if (ColorPicked != null) ColorPicked(this, e);
			};
			solidPanel.BringToFront();
			
			panel.Controls.Add(solidPanel);

			panels.Add(solidPanel);


			this.Size = new Size(172, 220);
			panel.BringToFront();
		}

		public event EventHandler ColorPicked;
	}
	#endregion Panel

	#region Solid
	internal class SolidColorPickerPanel : Control
	{
		private static readonly Color[] recentColor = new Color[8]{
			Color.White,
			Color.White,
			Color.White,
			Color.White,
			Color.White,
			Color.White,
			Color.White,
			Color.White,
		};
		
		internal event EventHandler ColorPicked;
	
		public SolidColorPickerPanel()
			: base()
		{
			this.DoubleBuffered = true;
		}

		private int hoverColorIndex = -1;

		private int selectedColorIndex;

		private Color currentColor;

		public Color CurrentColor
		{
			get { return currentColor; }
			set
			{
				currentColor = value;
				selectedColorIndex = GetIndexByColor(value);
			}
		}

		bool isAlphaPressed = false;

		private void PickColor(Color color)
		{
			selectedColorIndex = hoverColorIndex;
			currentColor = color;

			if (ColorPicked != null) ColorPicked.Invoke(this, null);

			Invalidate();
		}

		public static void AddRecentColor(Color color)
		{
			if (recentColor[0] != color)
			{
				Array.Copy(recentColor, 0, recentColor, 1, 7);
				recentColor[0] = color;
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			hoverColorIndex = GetColorIndexByPoint(e.Location);
			
			if (hoverColorIndex != -1)
			{			
				Color color;

				if (hoverColorIndex == 41)
				{
					using (ColorDialog cd = new ColorDialog())
					{
						cd.FullOpen = true;
						cd.Color = currentColor;
						if (cd.ShowDialog() == DialogResult.OK)
						{
							color = GetTranparentedColor(cd.Color);
						}
						else
							return;
					}

					PickColor(color);
				}
				else if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
				{
					
					using (ColorDialog cd = new ColorDialog())
					{
						cd.FullOpen = true;
						cd.Color = GetColorByIndex(hoverColorIndex);
						if (cd.ShowDialog() == DialogResult.OK)
						{
							color = GetTranparentedColor(cd.Color);
						}
						else
							return;
					}
				}
				else
				{
					PickColor(GetColorByIndex(hoverColorIndex));
				}
			}
			else if (isAlphaPressed)
			{
				isAlphaPressed = false;

				int a = (int)((e.X - transparentRect.Left) * 255 / transparentRect.Width);
				if (a < 0) a = 0;
				else if (a > 255) a = 255;

				PickColor(Color.FromArgb(a, currentColor.R, currentColor.G, currentColor.B));
				//currentColor = Color.FromArgb(a, currentColor.R, currentColor.G, currentColor.B);
			}
			base.OnMouseUp(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			hoverColorIndex = GetColorIndexByPoint(e.Location);
			if (hoverColorIndex == -1)
			{
				if (e.Button == MouseButtons.Left
					&& isAlphaPressed)
				{
					int a = (int)((e.X - transparentRect.Left) * 255 / transparentRect.Width);
					if (a < 0) a = 0;
					else if (a > 255) a = 255;

					currentColor = Color.FromArgb(a, currentColor.R, currentColor.G, currentColor.B);
					Invalidate();
				}
			}
			Invalidate();
			base.OnMouseMove(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			hoverColorIndex = GetColorIndexByPoint(e.Location);
			if (hoverColorIndex == -1)
			{
				if (transparentRect.Y < e.Y && transparentRect.Bottom + 9 > e.Y)
				{
					int a = (int)((e.X - transparentRect.Left) * 255 / transparentRect.Width);
					if (a < 0) a = 0;
					else if (a > 255) a = 255;

					currentColor = Color.FromArgb(a, currentColor.R, currentColor.G, currentColor.B);

					isAlphaPressed = true;
					Invalidate();
				}
			}
			
			base.OnMouseDown(e);
		}

		#region Predefined Colors
		protected static readonly Color[,] fixedColor =  {
														{ // gray
														Color.FromArgb(255,255,255),
														Color.FromArgb(238,238,238),
														Color.FromArgb(208,208,208),
														Color.FromArgb(120,120,120),
														Color.FromArgb(0, 0, 0),
														}, { // red
														Color.FromArgb(255,238,238),
														Color.FromArgb(255,178,178),
														Color.FromArgb(255, 0, 0),
														Color.FromArgb(150, 0, 0),
														Color.FromArgb(70, 0, 0),
														}, { // orange
														Color.FromArgb(255,238,218),
														Color.FromArgb(255,220,170),
														Color.FromArgb(255,153,0),
														Color.FromArgb(180,120,0),
														Color.FromArgb(120,60,0),
														}, { // yellow
														Color.FromArgb(255,255,238),
														Color.FromArgb(255,255,208),
														Color.FromArgb(255,255,0),
														Color.FromArgb(153,153,0),
														Color.FromArgb(70, 70, 0),
														}, { // green
														Color.FromArgb(238,255,238),
														Color.FromArgb(190,255,190),
														Color.FromArgb(0,  255,  0),
														Color.FromArgb(0,  153,  0),
														Color.FromArgb(0,   70,  0),
														}, { // sky
														Color.FromArgb(238,255,255),
														Color.FromArgb(210,255,255),
														Color.FromArgb(153,255,255),
														Color.FromArgb(0,  153,153),
														Color.FromArgb(0,   70, 70),
														}, { // blue
														Color.FromArgb(238,238,255),
														Color.FromArgb(210,210,255),
														Color.FromArgb(  0,  0,255),
														Color.FromArgb(  0,  0,153),
														Color.FromArgb(  0,  0, 70),
														}, { // maginate
														Color.FromArgb(255,238,255),
														Color.FromArgb(255,210,255),
														Color.FromArgb(255,  0,255),
														Color.FromArgb(153,  0,153),
														Color.FromArgb( 70,  0, 70),
														},
												 };
		#endregion

		Rectangle transparentRect = new Rectangle(7, 170, 155, 10);

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			int index = -1;
			using (SolidBrush b = new SolidBrush(Color.White))
			{
				for (int x = 8, i = 0; x < 160 && i < fixedColor.Length; i++, x += 20)
				{
					for (int y = 8, p = 0; y < 100; p++, y += 20)
					{
						index++;

						if (index == hoverColorIndex)
						{
							b.Color = SystemColors.Highlight;
							g.FillRectangle(b, x - 2, y - 2, 18, 18);
							g.DrawRectangle(SystemPens.WindowFrame, x - 2, y - 2, 18, 18);
						}

						if (index == selectedColorIndex)
						{
							g.DrawRectangle(SystemPens.Highlight, x - 2, y - 2, 18, 18);
						}

						b.Color = fixedColor[i, p];

						g.DrawRectangle(Pens.Black, x, y, 14, 14);
						g.FillRectangle(b, x + 1, y + 1, 13, 13);
					}
				}

				g.DrawLine(SystemPens.ControlDark, 4, 110, ClientRectangle.Width - 4, 110);

				// no color
				index++;

				if (index == hoverColorIndex)
				{
					b.Color = SystemColors.Highlight;
					g.FillRectangle(b, 6, 113, 80, 21);
					g.DrawRectangle(SystemPens.WindowFrame, 6, 113, 80, 21);
				}

				if (index == selectedColorIndex)
				{
					g.DrawRectangle(SystemPens.Highlight, 6, 113, 80, 21);
				}

				g.DrawRectangle(Pens.Black, 8, 116, 14, 14);
				g.DrawLine(Pens.Black, 8, 130, 22, 116);
				g.DrawString(ReoGrid.Editor.LangRes.LangResource.NoColor, Font,
					index == hoverColorIndex ?
					SystemBrushes.HighlightText : SystemBrushes.WindowText,
					new Rectangle(26, 116, 60, 20));

				// more 
				index++;

				if (index == hoverColorIndex)
				{
					b.Color = SystemColors.Highlight;
					g.FillRectangle(b, 95, 113, 68, 21);
					g.DrawRectangle(SystemPens.WindowFrame, 95, 113, 68, 21);
				}

				if (index == selectedColorIndex)
				{
					g.DrawRectangle(SystemPens.Highlight, 95, 113, 68, 21);
				}

				g.DrawLine(SystemPens.ControlDark, 90, 115, 90, 132);

				b.Color = currentColor;
				g.FillRectangle(b, 100, 116, 14, 14);

				g.DrawRectangle(Pens.Black, 100, 116, 14, 14);
				g.DrawString(ReoGrid.Editor.LangRes.LangResource.Menu_More, Font,
					index == hoverColorIndex ?
					SystemBrushes.HighlightText : SystemBrushes.WindowText,
					new Rectangle(118, 116, 60, 20));

				g.DrawLine(SystemPens.ControlDark, 4, 138, ClientRectangle.Width - 4, 138);

				// recent colors
				for (int x = 8, k = 0; x < 160; k++, x += 20)
				{
					index++;

					if (index == hoverColorIndex)
					{
						b.Color = SystemColors.Highlight;
						g.FillRectangle(b, x - 2, 142, 18, 18);
						g.DrawRectangle(SystemPens.WindowFrame, x - 2, 142, 18, 18);
					}

					if (index == selectedColorIndex)
					{
						g.DrawRectangle(SystemPens.Highlight, x - 2, 142, 18, 18);
					}

					b.Color = recentColor[k];
					using (SolidBrush sb = new SolidBrush(b.Color))
					{
						g.FillRectangle(sb, x, 144, 14, 14);
						g.DrawRectangle(Pens.Black, x, 144, 14, 14);
					}
				}

				// transparent
				GraphicsToolkit.DrawTransparentBlock(g, transparentRect);

				using (Brush lineBrush = new LinearGradientBrush(transparentRect,
					Color.FromArgb(0, Color.White), Color.FromArgb(255, Color.White),
					LinearGradientMode.Horizontal))
					g.FillRectangle(lineBrush, transparentRect);
				
				g.DrawRectangle(Pens.DimGray, transparentRect);

				GraphicsToolkit.FillTriangle(g, 10,
					new Point(transparentRect.Left  + (int)(currentColor.A * transparentRect.Width / 255),
					transparentRect.Bottom+6), GraphicsToolkit.TriangleDirection.Up);
			}
		}

		protected virtual Color GetTranparentedColor(Color c)
		{
			if (currentColor == Color.Empty || currentColor == Color.Transparent) return c;
			return Color.FromArgb(currentColor.A, c.R, c.G, c.B);
		}

		public virtual int GetColorIndexByPoint(Point p)
		{
			int i = -1;
			for (int x = 8; x < 160; x += 20)
			{
				for (int y = 8; y < 100; y += 20)
				{
					i++;
					if (GraphicsToolkit.PointInRect(new Rectangle(x - 2, y - 2, 19, 19), p))
						return i;
				}
			}

			i++;
			// no color : 40
			if (GraphicsToolkit.PointInRect(new Rectangle(8, 112, 88, 20), p))
				return i;

			i++;
			// more : 41
			if (GraphicsToolkit.PointInRect(new Rectangle(96, 112, 70, 20), p))
				return i;

			// recent : 42 ~ 50
			for (int x = 8; x < 160; x += 20)
			{
				i++;
				if (GraphicsToolkit.PointInRect(new Rectangle(x - 2, 142, 19, 19), p))
					return i;
			}

			return -1;
		}

		public virtual Color GetColorByIndex(int i, bool modify)
		{
			Color color = GetColorByIndex(i);
			if (modify && i != 41 /* more color */)
			{
				using (ColorDialog cd = new ColorDialog())
				{
					cd.FullOpen = true;
					cd.Color = color;
					if (cd.ShowDialog() == DialogResult.OK)
					{
						return GetTranparentedColor(cd.Color);
					}
				}
			}
			
			return color;
		}

		private Color GetColorByIndex(int i)
		{
			if (i < 0)
				return Color.Empty;
			else if (i < 40)
				return GetTranparentedColor(fixedColor[(i / 5), i % 5]);
			else if (i == 40)
				return Color.Empty;
			else if (i == 41)
			{
				return Color.Empty;
			}
			else if (i <= 49)
				return recentColor[i - 42];
			else
				return Color.Empty;
		}

		private int GetIndexByColor(Color color)
		{
			// no color : 40
			if (color.IsEmpty) return 40;

			int i = 0;

			int ex = fixedColor.GetLength(0);
			int ey = fixedColor.GetLength(1);

			// fixed color : 0 - 39
			for (int x = 0; x < ex; x++)
			{
				for (int y = 0; y < ey; y++)
				{
					if (fixedColor[x, y].R == color.R && fixedColor[x, y].G == color.G && fixedColor[x, y].B == color.B)
						return i;
					i++;
				}
			}

			i++;

			// recent : 42 ~ 50
			for (int k = 0; k < recentColor.Length; k++)
			{
				i++;
				if (recentColor[k].R == color.R && recentColor[k].G == color.G && recentColor[k].B == color.B)
					return i;
			}

			// can't find, return more : 41
			
			return 41;
		}

	}
	#endregion Solid

	#region Colors

	public abstract class AbstractColor
	{
		public abstract bool IsEmpty { get; }

		public abstract AbstractColor Clone();

		public abstract Color CompliantSolidColor { get; }
	}

	internal class SolidColor : AbstractColor
	{
		private Color color;

		public Color Color
		{
			get { return color; }
			set { color = value; }
		}

		public SolidColor(Color color)
		{
			this.color = color;
		}

		public override bool IsEmpty
		{
			get { return color.IsEmpty; }
		}

		public override Color CompliantSolidColor
		{
			get { return color; }
		}

		public override AbstractColor Clone()
		{
			return new SolidColor(color);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is SolidColor)) return false;
			return color.Equals(((SolidColor)obj).color);
		}

		public override int GetHashCode()
		{
			return color.GetHashCode();
		}
	}

	internal class LinearGradientColor : AbstractColor
	{
		public float Angle { get; private set; }

		private Color[] colors;
		public Color[] Colors { get { return colors; } set { colors = value; } }

		private float[] positions;
		public float[] Positions { get { return positions; } set { positions = value; } }

		public LinearGradientColor(ColorBlend colors)
			: this(colors.Colors, colors.Positions, 90f)
		{
		}

		public LinearGradientColor(Color[] colors, float[] positions, float angle)
		{
			this.colors = colors;
			this.positions = positions;
			this.Angle = angle;

#if DEBUG
			Debug.Assert(positions.Length >= 2);
			Debug.Assert(positions[0] == 0f);
			Debug.Assert(positions[positions.Length - 1] == 1f);
#endif
		}

		public override bool IsEmpty
		{
			get { return colors == null || colors.Length < 2 || positions == null || positions.Length < 2; }
		}

		public override Color CompliantSolidColor
		{
			get { return colors == null || colors.Length <= 0 ? Color.Empty : colors[0]; }
		}

		public LinearGradientBrush CreateGradientBrush(RectangleF rect)
		{
			LinearGradientBrush brush = new LinearGradientBrush(rect, Color.White, Color.Black, Angle);
			brush.InterpolationColors = new ColorBlend { Colors = colors, Positions = positions };
			return brush;
		}

		public override AbstractColor Clone()
		{
			Color[] new_colors = new Color[Colors.Length];
			Array.Copy(Colors, new_colors, colors.Length);

			float[] new_positions = new float[Positions.Length];
			Array.Copy(Positions, new_positions, positions.Length);

			return new LinearGradientColor(new_colors, new_positions, Angle);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is LinearGradientColor)) return false;

			LinearGradientColor lgc = (LinearGradientColor)obj;

			// always return false when compare to an invalid gradient color
			if (lgc.colors.Length != lgc.positions.Length) return false;

			if (colors.Length != lgc.colors.Length
				|| positions.Length != lgc.positions.Length) return false;

			for (int i = 0; i < colors.Length; i++)
			{
				if (colors[i] != lgc.colors[i]) return false;
				if (positions[i] != lgc.positions[i]) return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			int hash = 0;
			foreach (var c in colors)
			{
				hash ^= c.ToArgb();
			}
			foreach (var f in positions)
			{
				hash ^= (int)f;
			}
			return hash;
		}
	}

	internal class HatchPatternColor : AbstractColor
	{
		private Color foreColor;
		public Color ForeColor { get { return foreColor; } set { foreColor = value; } }

		private Color backColor;
		public Color BackColor { get { return backColor; } set { backColor = value; } }

		private HatchStyle hatchStyle { get; set; }
		public HatchStyle HatchStyle { get { return hatchStyle; } set { hatchStyle = value; } }

		public HatchPatternColor(Color foreColor, Color backColor, HatchStyle style)
		{
			this.foreColor = foreColor;
			this.backColor = backColor;
			this.hatchStyle = style;
		}

		public override bool IsEmpty
		{
			get { return foreColor.IsEmpty && backColor.IsEmpty; }
		}

		public override Color CompliantSolidColor
		{
			get { return backColor.IsEmpty ? foreColor : backColor; }
		}

		public override AbstractColor Clone()
		{
			return new HatchPatternColor(foreColor, backColor, hatchStyle);
		}

		public HatchBrush CreateHatchBrush()
		{
			return new HatchBrush(this.hatchStyle, this.foreColor, this.backColor);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is HatchPatternColor)) return false;

			HatchPatternColor hpc = (HatchPatternColor)obj;

			return foreColor == hpc.foreColor
				&& backColor == hpc.backColor
				&& hatchStyle == hpc.hatchStyle;
		}

		public override int GetHashCode()
		{
			return foreColor.GetHashCode() ^ backColor.GetHashCode() ^ (short)hatchStyle;
		}
	}

	#endregion
}
