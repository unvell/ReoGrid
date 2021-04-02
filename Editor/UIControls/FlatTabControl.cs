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
using System.ComponentModel;

namespace unvell.UIControls
{
	/// <summary>
	/// Represent a simple flat style tab control
	/// </summary>
	public class FlatTabControl : Control
	{
		/// <summary>
		/// Construct the control
		/// </summary>
		public FlatTabControl()
		{
			this.SelectedBackColor = SystemColors.Window;
			this.SelectedTextColor = Color.DimGray;

			DoubleBuffered = true;
		}

		private string[] tabs = { };

		/// <summary>
		/// Get or set the array of tab items
		/// </summary>
		[Description("Get or set the array of tab items")]
		public string[] Tabs
		{
			get { return tabs; }
			set
			{
				tabs = value;

				using (var tabFont = new Font(FontFamily.GenericSansSerif, 8))
				{
					sizes = new int[tabs.Length];
					for (int i = 0; i < tabs.Length; i++)
					{
						sizes[i] = TextRenderer.MeasureText(tabs[i], tabFont).Width + 8;
					}
				}

				Invalidate();
			}
		}

		private int[] sizes;

		/// <summary>
		/// Specifies the position of tab control
		/// </summary>
		[Description("Specifies the position of tab control")]
		[DefaultValue(TabBorderStyle.RectShadow)]
		public TabControlPosition Position { get; set; }

		/// <summary>
		/// Specifies the border style of every tabs
		/// </summary>
		[Description("Specifies the border style of every tabs")]
		[DefaultValue(TabBorderStyle.RectShadow)]
		public TabBorderStyle BorderStyle { get; set; }

		/// <summary>
		/// Show shadows beside of every tabs
		/// </summary>
		[Description("Show shadows beside of every tabs")]
		[DefaultValue(false)]
		public bool Shadow { get; set; }

		/// <summary>
		/// Get or set the background color for selected tab
		/// </summary>
		[Description("Get or set the background color for selected tab")]
		public Color SelectedBackColor { get; set; }

		/// <summary>
		/// Get or set the text color for selected tab
		/// </summary>
		[Description("Get or set the text color for selected tab")]
		public Color SelectedTextColor { get; set; }

		private int selectedIndex = 0;

		[Description("Zero-based number of selected tab")]
		[DefaultValue(0)]
		public int SelectedIndex
		{
			get { return selectedIndex; }
			set
			{
				if (selectedIndex != value)
				{
					selectedIndex = value;

					Invalidate();

					if (SelectedIndexChanged != null)
						SelectedIndexChanged(this, null);
				}
			}
		}

		/// <summary>
		/// Override OnPaint method
		/// </summary>
		/// <param name="e">paint event argument</param>
		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			Rectangle rect = Rectangle.Empty;

			if (this.BorderStyle != TabBorderStyle.NoBorder)
			{
				rect = new Rectangle(2, 2, ClientRectangle.Width - 4, ClientRectangle.Height - 2);

				switch (this.Position)
				{
					default:
					case TabControlPosition.Top:
						g.DrawLine(SystemPens.ControlDarkDark, 0, rect.Bottom - 1,
								ClientRectangle.Right, rect.Bottom - 1);
						break;

					case TabControlPosition.Bottom:
						g.DrawLine(SystemPens.ControlDarkDark, 0, rect.Top, ClientRectangle.Right, rect.Top);
						break;
				}

			}
			else
			{
				rect = new Rectangle(0, 0, ClientRectangle.Width, ClientRectangle.Height);
			}

			using (var tabFont = new Font(FontFamily.GenericSansSerif, 8))
			{
				for (int i = 0; i < tabs.Length; i++)
				{
					rect.Width = sizes[i];

					if (i != selectedIndex)
					{
						string tab = tabs[i];
						g.DrawString(tab, tabFont, Brushes.DimGray, rect.Left + 4, rect.Top + 2);
					}

					rect.Offset(rect.Width, 0);
				}

				if (this.BorderStyle != TabBorderStyle.NoBorder)
				{
					rect = new Rectangle(2, 2, ClientRectangle.Width - 4, ClientRectangle.Height - 2);
				}
				else
				{
					rect = new Rectangle(0, 0, ClientRectangle.Width, ClientRectangle.Height);
				}

				for (int i = 0; i < tabs.Length; i++)
				{
					rect.Width = sizes[i];

					if (i == selectedIndex)
					{
						string tab = tabs[i];

						int x = rect.Left, x2 = rect.Right, y = 0, y2 = 0;

						if (this.BorderStyle != TabBorderStyle.NoBorder)
						{
							if (this.BorderStyle == TabBorderStyle.SplitRouned)
							{
								x++; x2--;
							}

							switch (this.Position)
							{
								default:
								case TabControlPosition.Top:
									y = rect.Top; y2 = rect.Bottom;

									if (this.BorderStyle == TabBorderStyle.SplitRouned)
									{
										y++; y2--;
									}

									break;

								case TabControlPosition.Bottom:
									y = rect.Bottom - 1; y2 = rect.Top;

									if (this.BorderStyle == TabBorderStyle.SplitRouned)
									{
										y--; y2++;
									}

									break;
							}

							// left and right
							g.DrawLine(SystemPens.ControlDarkDark, rect.Left, y, rect.Left, y2);
							g.DrawLine(SystemPens.ControlDarkDark, rect.Right, y, rect.Right, y2);
						}

						using (Brush selectedBackBrush = new SolidBrush(this.SelectedBackColor),
							selectedTextBrush = new SolidBrush(this.SelectedTextColor))
						{
							// top or bottom
							switch (this.Position)
							{
								default:
								case TabControlPosition.Top:
									g.FillRectangle(selectedBackBrush, new Rectangle(rect.Left + 1, rect.Top + 1, rect.Width - 1, rect.Height - 1));

									if (this.BorderStyle != TabBorderStyle.NoBorder)
									{
										g.DrawLine(SystemPens.ControlDarkDark, x, rect.Top, x2, rect.Top);
									}
									break;

								case TabControlPosition.Bottom:
									g.FillRectangle(selectedBackBrush, new Rectangle(rect.Left + 1, rect.Top, rect.Width - 1, rect.Height - 1));

									if (this.BorderStyle != TabBorderStyle.NoBorder)
									{
										g.DrawLine(SystemPens.ControlDarkDark, x, rect.Bottom - 1, x2, rect.Bottom - 1);
									}
									break;
							}

							if (this.BorderStyle != TabBorderStyle.NoBorder && Shadow)
							{
								g.DrawLine(SystemPens.ControlDark, rect.Right + 1, rect.Top + 2, rect.Right + 1, rect.Bottom - 1);
							}

							g.DrawString(tab, tabFont, selectedTextBrush, rect.Left + 4, rect.Top + 2);
						}

						break;
					}

					rect.Offset(rect.Width, 0);
				}

			}
			base.OnPaint(e);
		}

		/// <summary>
		/// Override system mouse down event
		/// </summary>
		/// <param name="e">mouse down event argument</param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			int left = 0;
			for (int i = 0; i < tabs.Length; i++)
			{
				left += sizes[i];

				if (e.X < left)
				{
					if (selectedIndex != i)
					{
						selectedIndex = i;
						if (SelectedIndexChanged != null)
							SelectedIndexChanged(this, null);
					}

					Invalidate();
					break;
				}
			}
		}

		/// <summary>
		/// Event raised when selected tab is changed
		/// </summary>
		public event EventHandler SelectedIndexChanged;
	}

	/// <summary>
	/// Tab item border style
	/// </summary>
	public enum TabBorderStyle
	{
		/// <summary>
		/// Sharp Rectangle
		/// </summary>
		RectShadow,

		/// <summary>
		/// Separated Rounded Rectangle
		/// </summary>
		SplitRouned,

		/// <summary>
		/// No Borders (Windows 8 Style)
		/// </summary>
		NoBorder,
	}

	/// <summary>
	/// Position of tab control will be located
	/// </summary>
	public enum TabControlPosition
	{
		/// <summary>
		/// Put at top to other controls
		/// </summary>
		Top,

		/// <summary>
		/// Put at bottom to other controls
		/// </summary>
		Bottom,
	}
}
