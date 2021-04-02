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
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

using unvell.ReoGrid.Actions;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Editor.LangRes;

namespace unvell.ReoGrid.PropertyPages
{
	/// <summary>
	/// Border setting dialog panel
	/// </summary>
	public partial class BorderPropertyPage : UserControl, IPropertyPage
	{
		private ReoGridControl grid;

		public ReoGridControl Grid
		{
			get { return grid; }
			set { this.grid = value; }
		}

		/// <summary>
		/// Create dialog panel for border settings
		/// </summary>
		public BorderPropertyPage()
		{
			InitializeComponent();

			borderColorSelector.ColorSelected += (sender, e) =>
			{
				borderStyleList.BorderColor = borderColorSelector.SolidColor;
				borderSetter.CurrentColor = borderColorSelector.SolidColor;
			};

			borderStyleList.BorderStyleSelectionChanged += (sender, e) =>
			{
				borderSetter.CurrentBorderStlye = borderStyleList.SelectedBorderStyle;
			};

			borderColorSelector.SolidColor = Color.Black;
			borderStyleList.SelectedBorderStyle = BorderLineStyle.Solid;
			borderSetter.CurrentColor = borderColorSelector.SolidColor;
		}

		public void SetupUILanguage()
		{
			grpLine.Text = LangResource.BorderPage_Line;

			labStyle.Text = LangResource.BorderPage_Style;
			labColor.Text = LangResource.Label_Color;

			formLinePresets.Text = LangResource.BorderPage_Presets;
			formLineBorder.Text = LangResource.Border;
		}

#pragma warning disable 67 // variable is never used
		/// <summary>
		/// Setting dialog will be closed when this event rasied
		/// </summary>
		public event EventHandler Done;
#pragma warning restore 67 // variable is never used

		#region Border Button
		private void btnNone_Click(object sender, EventArgs e)
		{
			borderSetter.RemoveBorderStyle(BorderPositions.All);
		}

		private void btnOutline_Click(object sender, EventArgs e)
		{
			borderSetter.CheckBorderStyle(BorderPositions.Outside);
		}

		private void btnInside_Click(object sender, EventArgs e)
		{
			borderSetter.CheckBorderStyle(BorderPositions.InsideAll);
		}

		private void btnTop_Click(object sender, EventArgs e)
		{
			borderSetter.InvertBorderStats(BorderPositions.Top);
		}

		private void btnMiddle_Click(object sender, EventArgs e)
		{
			borderSetter.InvertBorderStats(BorderPositions.InsideHorizontal);
		}

		private void btnBottom_Click(object sender, EventArgs e)
		{
			borderSetter.InvertBorderStats(BorderPositions.Bottom);
		}

		private void btnSlash_Click(object sender, EventArgs e)
		{
			borderSetter.InvertBorderStats(BorderPositions.Slash);
		}

		private void btnLeft_Click(object sender, EventArgs e)
		{
			borderSetter.InvertBorderStats(BorderPositions.Left);
		}

		private void btnCenter_Click(object sender, EventArgs e)
		{
			borderSetter.InvertBorderStats(BorderPositions.InsideVertical);
		}

		private void btnRight_Click(object sender, EventArgs e)
		{
			borderSetter.InvertBorderStats(BorderPositions.Right);
		}

		private void btnBackslash_Click(object sender, EventArgs e)
		{
			borderSetter.InvertBorderStats(BorderPositions.Backslash);
		}
		#endregion // Border Button

		/// <summary>
		/// Initialize dialog panel
		/// </summary>
		public void LoadPage()
		{
			borderSetter.ReadFromGrid(grid);

			btnCenter.Enabled = borderSetter.Rows > 1;
			btnMiddle.Enabled = borderSetter.Cols > 1;

			RangeBorderStyle style = borderSetter.Borders.Values.FirstOrDefault();

			if (!style.IsEmpty)
			{
				borderStyleList.SelectedBorderStyle = style.Style;
				borderColorSelector.SolidColor = style.Color;
				borderColorSelector.RaiseColorPicked();
			}
		}

		/// <summary>
		/// Create action to update spreadsheet
		/// </summary>
		/// <returns></returns>
		public WorksheetReusableAction CreateUpdateAction()
		{
			return borderSetter.CreateUpdateAction(grid.CurrentWorksheet);
		}
	}

	#region Border Style List
	internal class BorderStyleListControl : Control
	{
		public List<BorderStyleListItem> items = new List<BorderStyleListItem>();

		public BorderStyleListControl()
		{
			DoubleBuffered = true;

			BorderLineStyle[] styles = (BorderLineStyle[])Enum.GetValues(typeof(BorderLineStyle));
			for (int i = 0; i < styles.Length; i++)
			{
				items.Add(new BorderStyleListItem
				{
					Bounds = Rectangle.Empty,
					Style = styles[i],
					IsSelected = styles[i] == borderLineStyle,
				});
			}
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();

			int w = (ClientRectangle.Width >> 1) - 10;

			Rectangle rect = new Rectangle(5, 7, w, 15);

			for (int i = 0; i < items.Count; i++)
			{
				items[i].Bounds = rect;

				rect.Offset(0, 15);

				if (i == 6)
				{
					rect.Location = new Point(w + 15, 7);
				}
			}
		}

		private Color borderColor = Color.Black;

		public Color BorderColor
		{
			get { return borderColor; }
			set { borderColor = value; Invalidate(); }
		}

		private BorderLineStyle borderLineStyle = BorderLineStyle.Solid;

		[DefaultValue(BorderLineStyle.Solid)]
		public BorderLineStyle BorderLineStyle
		{
			get { return borderLineStyle; }
			set { borderLineStyle = value; }
		}

		public BorderLineStyle SelectedBorderStyle
		{
			get
			{
				BorderStyleListItem item = items.FirstOrDefault(i => i.IsSelected);
				return (item == null) ? BorderLineStyle.None : item.Style;
			}
			set
			{
				SelectBorderStyle(value);
			}
		}

		public void SelectBorderStyle(BorderLineStyle style)
		{
			foreach (var i in items)
			{
				i.IsSelected = (i.Style == style);
			}

			Invalidate();
			if (BorderStyleSelectionChanged != null) BorderStyleSelectionChanged(this, null);
		}

		public event EventHandler BorderStyleSelectionChanged;

		protected override void OnMouseDown(MouseEventArgs e)
		{
			BorderStyleListItem item = items.FirstOrDefault(i=>i.Bounds.Contains(e.Location));
			if (item != null) SelectBorderStyle(item.Style);
		}

		private static readonly StringFormat sf = new StringFormat();
		static BorderStyleListControl()
		{
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			System.Drawing.Graphics g = e.Graphics;
		
			foreach(var i in items)
			{
				if (i.Style == BorderLineStyle.None)
				{
					g.DrawString(LangResource.None, SystemFonts.DefaultFont, Brushes.Black, i.Bounds, sf);
				}
				else
				{
					BorderPainter.Instance.DrawLine(g, 
						i.Bounds.Left, i.Bounds.Top + i.Bounds.Height / 2,
						i.Bounds.Right, i.Bounds.Top + i.Bounds.Height / 2, i.Style, borderColor);
				}

				if (i.IsSelected)
				{
					using (Pen p = new Pen(Color.Black))
					{
						p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
						g.DrawRectangle(p, i.Bounds);
					}
				}

				if (i.IsFocus)
				{
					ControlPaint.DrawFocusRectangle(g, i.Bounds);
				}
			}

			Rectangle rect = new Rectangle(ClientRectangle.Left, ClientRectangle.Left,
				ClientRectangle.Width - 1, ClientRectangle.Height - 1);
			g.DrawRectangle(SystemPens.ButtonShadow, rect);
		}
	}

	internal class BorderStyleListItem
	{
		private Rectangle bounds;

		public Rectangle Bounds
		{
			get { return bounds; }
			set { bounds = value; }
		}

		private BorderLineStyle style;

		public BorderLineStyle Style
		{
			get { return style; }
			set { style = value; }
		}

		private bool isFocus = false;

		public bool IsFocus
		{
			get { return isFocus; }
			set { isFocus = value; }
		}

		private bool isSelected = false;

		public bool IsSelected
		{
			get { return isSelected; }
			set { isSelected = value; }
		}
	}
	#endregion // Border Style List

	#region Border Setter
	internal class BorderSetterControl : Control
	{
		BorderPositions[] allPos;

		public BorderSetterControl()
		{
			DoubleBuffered = true;
			allPos = (BorderPositions[])Enum.GetValues(typeof(BorderPositions));
		}

		private int rows = 2;

		public int Rows
		{
			get { return rows; }
			set { rows = value; if (rows > 2) rows = 2; UpdateRects(); }
		}

		private int cols = 2;

		public int Cols
		{
			get { return cols; }
			set { cols = value; if (cols > 2) cols = 2; UpdateRects(); }
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			UpdateRects();
		}

		private BorderLineItem[] items = new BorderLineItem[8];

		private void UpdateRects()
		{
			int width = ClientRectangle.Width - 16 - Padding.Left - Padding.Right;
			int height = ClientRectangle.Height - 16 - Padding.Top - Padding.Bottom;

			int cw = width / cols;
			int rw = height / rows;

			int y = ClientRectangle.Top + 8 + Padding.Top;
			int x = ClientRectangle.Left + 8 + Padding.Left;

			Rectangle rect = new Rectangle(x, y - 6, width, 13);
			
			int i = 0;

			for (int r = 0; r <= rows; r++)
			{
				items[i].rect = rect;
				if (r == 0) items[i].pos = BorderPositions.Top;
				else if (r == rows) items[i].pos = BorderPositions.Bottom;
				else items[i].pos = BorderPositions.InsideHorizontal;
				rect.Y += rw;
				i++;
			}

			rect.X = x - 6;
			rect.Y = y;
			rect.Size = new Size(13, height);

			for (int c = 0; c <= cols; c++)
			{
				items[i].rect = rect;
				if (c == 0) items[i].pos = BorderPositions.Left;
				else if (c == cols) items[i].pos = BorderPositions.Right;
				else items[i].pos = BorderPositions.InsideVertical;
				rect.X += cw;
				i++;
			}
			
			Invalidate();
		}

		private bool allowHover = false;

		[DefaultValue(false)]
		public bool AllowHover
		{
			get { return allowHover; }
			set { allowHover = value; }
		}

		#region Draw
		protected override void OnPaint(PaintEventArgs e)
		{
			var g = e.Graphics;

			int width = ClientRectangle.Width - 16 - Padding.Right - Padding.Left;
			int height = ClientRectangle.Height - 16 - Padding.Top - Padding.Bottom;

			int cw = width / cols;
			int rw = height / rows;

			if (allowHover)
			{
				using (Brush hoverB = new SolidBrush(Color.FromArgb(224, 224, 224)))
				{
					for (int i = 0; i < items.Length; i++)
					{
						if (items[i].hover) g.FillRectangle(hoverB, items[i].rect);
					}
				}
			}

			Pen markPen = SystemPens.ButtonShadow;

			int y = ClientRectangle.Top + 8 + Padding.Top;
			int x = 0;

			for (int r = 0; r <= rows; r++)
			{
				x = ClientRectangle.Left + 8 + Padding.Left;

				for (int c = 0; c <= cols; c++)
				{
					if (r == 0)
					{
						if (c == 0)
						{
							g.DrawLine(markPen, x - 1, y, x - 5, y);
							g.DrawLine(markPen, x, y - 1, x, y - 5);
						}
						else if (c == cols)
						{
							g.DrawLine(markPen, x + 1, y, x + 5, y);
							g.DrawLine(markPen, x, y - 1, x, y - 5);
						}
						else
						{
							g.DrawLine(markPen, x - 3, y - 1, x + 3, y - 1);
							g.DrawLine(markPen, x, y - 1, x, y - 5);
						}
					}
					else if (r == rows)
					{
						if (c == 0)
						{
							g.DrawLine(markPen, x - 1, y, x - 5, y);
							g.DrawLine(markPen, x, y + 1, x, y + 5);
						}
						else if (c == cols)
						{
							g.DrawLine(markPen, x + 1, y, x + 5, y);
							g.DrawLine(markPen, x, y + 1, x, y + 5);
						}
						else
						{
							g.DrawLine(markPen, x - 3, y + 1, x + 3, y + 1);
							g.DrawLine(markPen, x, y + 2, x, y + 6);
						}
					}
					else
					{
						if (c == 0)
						{
							g.DrawLine(markPen, x - 1, y - 3, x - 1, y + 3);
							g.DrawLine(markPen, x - 2, y, x - 6, y);
						}
						else if (c == cols)
						{
							g.DrawLine(markPen, x + 1, y - 3, x + 1, y + 3);
							g.DrawLine(markPen, x + 2, y, x + 6, y);
						}
					}

					if (r < rows && c < cols)
					{
						Rectangle rect = new Rectangle(x, y, cw, rw);

						using (StringFormat sf = new StringFormat()
						{
							Alignment = StringAlignment.Center,
							LineAlignment = StringAlignment.Center
						})
						{
							g.DrawString(LangResource.Text, Font, SystemBrushes.ControlDarkDark, rect, sf);
						}
					}

					x += cw;
				}

				y += rw;
			}

			y = ClientRectangle.Top + 8 + Padding.Top;
			x = ClientRectangle.Left + 8 + Padding.Left;

			using (Brush mixB = new HatchBrush(HatchStyle.Percent50, Color.White, Color.Silver))
			{
				for (int r = 0; r <= rows; r++)
				{
					if (r == 0)
					{
						if ((mixBorders & BorderPositions.Top) == BorderPositions.Top)
							g.FillRectangle(mixB, x, y - 1, width, 3);
						else if (borders.ContainsKey(BorderPositions.Top))
							BorderPainter.Instance.DrawLine(g, x, y, x + width, y, borders[BorderPositions.Top]);
					}
					else if (r == rows)
					{
						if ((mixBorders & BorderPositions.Bottom) == BorderPositions.Bottom)
							g.FillRectangle(mixB, x, y - 1, width, 3);
						else if (borders.ContainsKey(BorderPositions.Bottom))
							BorderPainter.Instance.DrawLine(g, x, y, x + width, y, borders[BorderPositions.Bottom]);
					}
					else
					{
						if ((mixBorders & BorderPositions.InsideHorizontal) == BorderPositions.InsideHorizontal)
							g.FillRectangle(mixB, x, y - 1, width, 3);
						else if (borders.ContainsKey(BorderPositions.InsideHorizontal))
							BorderPainter.Instance.DrawLine(g, x, y, x + width, y, borders[BorderPositions.InsideHorizontal]);
					}

					y += rw;
				}

				y = ClientRectangle.Top + 8 + Padding.Top;
				for (int c = 0; c <= cols; c++)
				{
					if (c == 0)
					{
						if ((mixBorders & BorderPositions.Left) == BorderPositions.Left)
							g.FillRectangle(mixB, x - 1, y, 3, height);
						else if (c == 0 && borders.ContainsKey(BorderPositions.Left))
							BorderPainter.Instance.DrawLine(g, x, y, x, y + height, borders[BorderPositions.Left]);
					}
					else if (c == cols)
					{
						if ((mixBorders & BorderPositions.Right) == BorderPositions.Right)
							g.FillRectangle(mixB, x - 1, y, 3, height);
						else if (borders.ContainsKey(BorderPositions.Right))
							BorderPainter.Instance.DrawLine(g, x, y, x, y + height, borders[BorderPositions.Right]);
					}
					else
					{
						if ((mixBorders & BorderPositions.InsideVertical) == BorderPositions.InsideVertical)
							g.FillRectangle(mixB, x - 1, y, 3, height);
						else if (borders.ContainsKey(BorderPositions.InsideVertical))
							BorderPainter.Instance.DrawLine(g, x, y, x, y + height, borders[BorderPositions.InsideVertical]);
					}

					x += cw;
				}
			}

			if (controlBorder)
			{
				Rectangle controlBoundsRect = new Rectangle(ClientRectangle.Left, ClientRectangle.Top,
					ClientRectangle.Width - 1, ClientRectangle.Height - 1);
				g.DrawRectangle(SystemPens.ButtonShadow, controlBoundsRect);
			}
		}
		#endregion

		private bool controlBorder = true;

		[DefaultValue(true)]
		public bool ControlBorder
		{
			get { return controlBorder; }
			set { controlBorder = value; Invalidate(); }
		}

		private Color currentColor = Color.Black;

		public Color CurrentColor
		{
			get { return currentColor; }
			set { currentColor = value; }
		}

		private BorderLineStyle currentBorderStlye;

		public BorderLineStyle CurrentBorderStlye
		{
			get { return currentBorderStlye; }
			set { currentBorderStlye = value; }
		}

		private Dictionary<BorderPositions, RangeBorderStyle> borders =
			new Dictionary<BorderPositions, RangeBorderStyle>();

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<BorderPositions, RangeBorderStyle> Borders
		{
			get { return borders; }
			set { borders = value; }
		}

		private void ProcessBorderStyles(BorderPositions tpos, Action<BorderPositions> handler)
		{
			for (int i = 0; i < allPos.Length; i++)
			{
				BorderPositions pos = allPos[i];
				if (allPos[i] != BorderPositions.None && (tpos & pos) == pos)
				{
					handler(pos);
				}
			}
		}

		public void CheckBorderStyle(BorderPositions pos)
		{
			ProcessBorderStyles(pos, p =>
			{
				borders[p] = new RangeBorderStyle
				{
					Style = currentBorderStlye,
					Color = currentColor,
				};
				mixBorders &= ~p;
			});

			borderAdded |= pos;
			borderRemoved &= ~pos;
			Invalidate();
		}

		public void RemoveBorderStyle(BorderPositions pos)
		{
			ProcessBorderStyles(pos, p =>
			{
				borders.Remove(p);
				mixBorders &= ~p;
				borderRemoved |= pos;
				borderAdded &= ~p;
			});
			Invalidate();
		}

		public void InvertBorderStats(BorderPositions pos)
		{
			ProcessBorderStyles(pos, p =>
			{
				if ((mixBorders & p) == p)
				{
					mixBorders &= ~p;
					borders.Remove(p);
					borderRemoved |= p;
					borderAdded &= ~p;
				}

				if (borders.ContainsKey(p) && borders[p].Style == currentBorderStlye && borders[p].Color == currentColor)
				{
					borders.Remove(p);
					borderRemoved |= p;
					borderAdded &= ~p;
				}
				else
					CheckBorderStyle(p);
			});

			Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			BorderPositions pos = GetBorderPos(e.Location);
			if (pos != BorderPositions.None)
			{
				InvertBorderStats(pos);
				Invalidate();
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (allowHover)
			{
				for (int i = 0; i < items.Length; i++)
				{
					bool beforeHover = items[i].hover;
					items[i].hover = items[i].rect.Contains(e.Location);
					if (beforeHover != items[i].hover) Invalidate();
				}
			}
		}

		private BorderPositions GetBorderPos(Point p)
		{
			for (int i = 0; i < items.Length; i++)
			{
				if (items[i].rect.Contains(p)) return items[i].pos;
			}
			return BorderPositions.None;
		}

		private BorderPositions borderAdded = BorderPositions.None;
		private BorderPositions borderRemoved = BorderPositions.None;

		public SetRangeBorderAction CreateUpdateAction(Worksheet grid)
		{
			// just create actions for changed borders
			if (borderAdded != BorderPositions.None || borderRemoved != BorderPositions.None)
			{
				List<RangeBorderInfo> styles = new List<RangeBorderInfo>();

				AddBorderPosStyle(styles, borderAdded);

				// fix bug: border cannot be removed
				//AddBorderPosStyle(styles, borderRemoved);
				styles.Add(new RangeBorderInfo(borderRemoved, RangeBorderStyle.Empty));

				SetRangeBorderAction action = new SetRangeBorderAction(grid.SelectionRange, styles.ToArray());
				return action;
			}
			else
				return null;
		}

		private void AddBorderPosStyle(List<RangeBorderInfo> posStyles, BorderPositions scope)
		{
			// remove borders that not changed
			var q = borders.Where(b => (scope & b.Key) == b.Key).GroupBy(b => b.Value);

			BorderPositions allPos = BorderPositions.None;

			// create fewer actions for multiple border settings
			foreach (var bs in q)
			{
				BorderPositions pos = BorderPositions.None;

				// find same styles
				foreach (var p in borders.Where(b => (scope & b.Key) == b.Key && b.Value == bs.Key))
				{
					// merge positions
					pos |= p.Key;
				}

				allPos |= pos;

				if (pos != BorderPositions.None)
				{
					posStyles.Add(new RangeBorderInfo(pos, bs.Key));
				}
			}
		}

		/// <summary>
		/// Load border info from spreadsheet
		/// </summary>
		/// <param name="sheet">current worksheet instance</param>
		public void ReadFromGrid(ReoGridControl grid)
		{
			var sheet = grid.CurrentWorksheet;

			if (sheet.SelectionRange.IsEmpty)
			{
				this.Enabled = false;
			}
			else
			{
				RangeBorderInfoSet info = sheet.GetRangeBorders(sheet.SelectionRange, BorderPositions.All, false);

				if (!info.Left.IsEmpty) borders[BorderPositions.Left] = info.Left;
				if (!info.Right.IsEmpty) borders[BorderPositions.Right] = info.Right;
				if (!info.Top.IsEmpty) borders[BorderPositions.Top] = info.Top;
				if (!info.Bottom.IsEmpty) borders[BorderPositions.Bottom] = info.Bottom;
				if (!info.InsideHorizontal.IsEmpty) borders[BorderPositions.InsideHorizontal] = info.InsideHorizontal;
				if (!info.InsideVertical.IsEmpty) borders[BorderPositions.InsideVertical] = info.InsideVertical;

				rows = sheet.SelectionRange.Rows > 2 ? 2 : sheet.SelectionRange.Rows;
				cols = sheet.SelectionRange.Cols > 2 ? 2 : sheet.SelectionRange.Cols;

				mixBorders |= info.NonUniformPos;
			}
		}

		private struct BorderLineItem
		{
			internal Rectangle rect;
			internal BorderPositions pos;
			internal bool hover;
			//internal BorderLineStyle style;
		}

		private BorderPositions mixBorders = BorderPositions.None;

		public BorderPositions MixBorders
		{
			get { return mixBorders; }
			set { mixBorders = value; }
		}
	}
	#endregion // Border Setter
}
