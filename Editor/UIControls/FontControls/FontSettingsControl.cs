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

using unvell.Common;
using unvell.UIControls;

namespace unvell.UIControls
{
	public partial class FontSettingsControl : UserControl
	{
		private bool updateUI = false;
		private bool noRaiseEvent = false;
	
		public FontSettingsControl()
		{
			InitializeComponent();

			ShowColorPicker = true;

			sizeList.Items.AddRange(FontUIToolkit.FontSizeList.Select(f => (object)f).ToArray());

			//----------------------------------------------

			fontList.SelectedIndexChanged += (s, e) =>
			{
				if (!updateUI)
				{
					updateUI = true;
					
					FontFamilyInfo fontFamily = (FontFamilyInfo)fontList.SelectedItem;
					selectedFont.FontFamilyInfo = fontFamily;
					UpdateFontStyle();
					txtFont.Text = fontFamily.CultureName;
					labSample.Invalidate();
					
					updateUI = false;

					RaiseFontNameChanged();
				}
			};

			styleList.SelectedIndexChanged += (s, e) =>
			{
				if (!updateUI)
				{
					string style = styleList.SelectedItem as string;
					if (!string.IsNullOrEmpty(style))
					{
						updateUI = true;

						selectedFont.Style = FontUIToolkit.GetFontStyleByName(style, "italic", "bold");
						if (chkUnderline.Checked) selectedFont.Style |= FontStyle.Underline;
						if (chkStrikeout.Checked) selectedFont.Style |= FontStyle.Strikeout;

						labSample.Invalidate();

						updateUI = false;

						RaiseFontStyleChanged();
					}
				}
			};

			sizeList.SelectedIndexChanged += (s, e) =>
			{
				if (!updateUI)
				{
					updateUI = true;

					float size = sizeList.SelectedIndex == -1 ? 0 : ((float)sizeList.SelectedItem);
					txtSize.Text = size == 0 ? string.Empty : size.ToString();
					selectedFont.Size = size;
					labSample.Invalidate();

					updateUI = false;

					RaiseFontSizeChanged();
				}
			};

			//----------------------------------------------

			txtFont.TextChanged += (s, e) =>
			{
				string fontName = txtFont.Text;

				if (!string.IsNullOrEmpty(fontName))
				{
					SetFontNameByString(fontName);
				}
			};

			txtFont.KeyDown += (s, e) =>
			{
				switch (e.KeyCode)
				{
					case Keys.Up:
						if (fontList.SelectedIndex > 0) fontList.SelectedIndex--;
						e.Handled = true;
						break;
					case Keys.Down:
						if (fontList.SelectedIndex < fontList.Items.Count - 1) fontList.SelectedIndex++;
						e.Handled = true;
						break;
				}
			};

			txtStyle.TextChanged += (s, e) =>
			{
				if (!updateUI)
				{
					updateUI = true;

					string styleString = txtStyle.Text.Trim();

					if (styleString.Length > 0)
					{
						foreach (string style in styleList.Items)
						{
							if (style.StartsWith(styleString, StringComparison.InvariantCultureIgnoreCase))
							{
								styleList.SelectedItem = style;

								RaiseFontStyleChanged();
								break;
							}
						}
					}

					updateUI = false;
				}
			};

			txtStyle.KeyDown += (s, e) =>
			{
				switch (e.KeyCode)
				{
					case Keys.Up:
						if (styleList.SelectedIndex > 0) styleList.SelectedIndex--;
						e.Handled = true;
						break;

					case Keys.Down:
						if (styleList.SelectedIndex < styleList.Items.Count - 1)
							styleList.SelectedIndex++;
						e.Handled = true;
						break;
				}
			};

			txtSize.TextChanged += (s, e) =>
			{
				if (!updateUI && txtSize.Text.Trim().Length > 0)
				{
					string sizeStr = txtSize.Text;
					if (sizeStr.StartsWith(" ") || sizeStr.EndsWith(" ")) sizeStr = sizeStr.Trim();

					float inputSize = SystemFonts.DefaultFont.Size;
					float.TryParse(txtSize.Text, out inputSize);

					for (int i = 0; i < sizeList.Items.Count; i++)
					{
						if ((float)sizeList.Items[i] == inputSize)
						{
							sizeList.SelectedIndex = i;

							RaiseFontSizeChanged();
							break;
						}
					}

					selectedFont.Size = inputSize;
					labSample.Invalidate();
				}
			};

			txtSize.KeyDown += (s, e) =>
			{
				switch (e.KeyCode)
				{
					case Keys.Up:
						if (sizeList.SelectedIndex > 0) sizeList.SelectedIndex--;
						e.Handled = true;
						break;
					case Keys.Down:
						if (sizeList.SelectedIndex < sizeList.Items.Count - 1)
							sizeList.SelectedIndex++;
						e.Handled = true;
						break;
				}
			};

			labSample.Paint += (s, e) =>
			{
				Graphics g = e.Graphics;
				g.Clear(Color.White);

				if (labSample.ForeColor.A < 255)
				{
					GraphicsToolkit.DrawTransparentBlock(g, labSample.ClientRectangle);
				}

				using (StringFormat sf = new StringFormat()
				{
					Alignment = StringAlignment.Center,
					LineAlignment = StringAlignment.Center,
					FormatFlags = StringFormatFlags.NoWrap,
				})
				{
					using (Brush b = new SolidBrush(labSample.ForeColor))
					{
						if (selectedFont != null && !string.IsNullOrEmpty(selectedFont.Name)
							&& selectedFont.Size > 0)
						{
							try
							{
								using (Font font = new Font(SelectedFont.Name, SelectedFont.Size, SelectedFont.Style))
								{
									g.DrawString(labSample.Text, font, b, labSample.ClientRectangle, sf);
								}
							}
							catch { }
						}
					}
				}
			};

			chkUnderline.CheckedChanged += (s, e) =>
			{
				if (chkUnderline.Checked)
					selectedFont.Style |= FontStyle.Underline;
				else
					selectedFont.Style &= ~FontStyle.Underline;

				labSample.Invalidate();
				RaiseFontStyleChanged();
			};

			chkStrikeout.CheckedChanged += (s, e) =>
			{
				if (chkStrikeout.Checked)
					selectedFont.Style |= FontStyle.Strikeout;
				else
					selectedFont.Style &= ~FontStyle.Strikeout;

				labSample.Invalidate();
				RaiseFontStyleChanged();
			};

			fontColor.ColorSelected += (s, e) =>
			{
				labSample.ForeColor = fontColor.SolidColor;
				if (SelectedFontColorChanged != null)
				{
					SelectedFontColorChanged(this, null);
				}
			};
		}

		private void SetFontNameByString(string fontName)
		{
			if (!updateUI)
			{
				for (int i = 0; i < fontList.Items.Count; i++)
				{
					var fi = ((FontFamilyInfo)fontList.Items[i]);

					if (fi.IsFamilyName(fontName))
					{
						updateUI = true;

						fontList.SelectedIndex = i;
						selectedFont.FontFamilyInfo = fi;
						UpdateFontStyle();

						updateUI = false;

						RaiseFontNameChanged();
						break;
					}
				}
			}
		}

		private void SetFontStyleByStyle(FontStyle style)
		{
			txtStyle.Text = FontUIToolkit.GetFontStyleName(style, "Regular", "Italic", "Bold");
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			UpdateCultureLanguage();

			if (fontList.SelectedIndex == -1)
			{
				SelectedFont = new FontInfo(SystemFonts.DefaultFont);
			}
		}

		public string CultureFontText { get; set; }
		public string CultureSizeText { get; set; }
		public string CultureStyleText { get; set; }
		public string CultureColorText { get; set; }
		public string CultureStrikethoughText { get; set; }
		public string CultureUnderlineText { get; set; }
		public string CultureSampleGroupText { get; set; }
	
		public void UpdateCultureLanguage()
		{
			labFont.Text = CultureFontText;
			labSize.Text = CultureSizeText;
			labStyle.Text = CultureStyleText;
			labColor.Text = CultureColorText;
			chkStrikeout.Text = CultureStrikethoughText;
			chkUnderline.Text = CultureUnderlineText;
			sampleGroup.Text = CultureSampleGroupText;
		}

		private FontInfo selectedFont;

		public FontInfo SelectedFont
		{
			get
			{
				return selectedFont;
			}
			set
			{
				if (selectedFont != value)
				{
					selectedFont = value;

					noRaiseEvent = true;

					txtFont.Text = selectedFont.Name;
					SetFontNameByString(selectedFont.Name);
					SetFontStyleByStyle(selectedFont.Style);
					txtSize.Text = selectedFont.Size.ToString();
					for (int i = 0; i < sizeList.Items.Count; i++)
					{
						if (((float)sizeList.Items[i]) == selectedFont.Size)
						{
							sizeList.SelectedIndex = i;
							break;
						}
					}

					noRaiseEvent = false;
				}
			}
		}

		private void UpdateFontStyle()
		{
			chkStrikeout.Checked = (selectedFont.Style & FontStyle.Strikeout) == FontStyle.Strikeout;
			chkUnderline.Checked = (selectedFont.Style & FontStyle.Underline) == FontStyle.Underline;

			styleList.Items.Clear();

			if (string.IsNullOrEmpty(selectedFont.Name))
			{
				styleList.Enabled = false;
			}
			else
			{
				styleList.Enabled = true;

				using (FontFamily ff = new FontFamily(selectedFont.Name))
				{
					if (ff.IsStyleAvailable(FontStyle.Regular))
						styleList.Items.Add("Regular");

					if (ff.IsStyleAvailable(FontStyle.Italic))
						styleList.Items.Add("Italic");

					if (ff.IsStyleAvailable(FontStyle.Bold))
						styleList.Items.Add("Bold");

					if (ff.IsStyleAvailable(FontStyle.Bold | FontStyle.Italic))
						styleList.Items.Add("Bold Italic");

					chkStrikeout.Enabled = ff.IsStyleAvailable(FontStyle.Strikeout);
					if (!chkStrikeout.Enabled) chkStrikeout.Checked = false;

					chkUnderline.Enabled = ff.IsStyleAvailable(FontStyle.Underline);
					if (!chkUnderline.Enabled) chkUnderline.Checked = false;

					int foundOldStyle = -1;
					for (int i = 0; i < styleList.Items.Count; i++)
					{
						string text = Convert.ToString(styleList.Items[i]);
						FontStyle fs = FontUIToolkit.GetFontStyleByName(text,
							"italic", "bold");
						if (fs == SelectedFont.Style)
						{
							styleList.SelectedIndex = foundOldStyle = i;
							break;
						}
					}

					if (foundOldStyle == -1 && styleList.Items.Count > 0)
					{
						styleList.SelectedIndex = 0;
						selectedFont.Style = FontUIToolkit.GetFontStyleByName(Convert.ToString(styleList.Items[0]),
							"italic", "bold");

						if (chkUnderline.Checked) selectedFont.Style |= FontStyle.Underline;
						if (chkStrikeout.Checked) selectedFont.Style |= FontStyle.Strikeout;
					}
				}
			}

			txtStyle.Text = FontUIToolkit.GetFontStyleName(selectedFont.Style,
							"Regular", "Italic", "Bold");

			labSample.Invalidate();
		}

		public void RaiseFontNameChanged()
		{
			if (!noRaiseEvent)
			{
				if (SelectedFontNameChanged != null) SelectedFontNameChanged(this, null);
				RaiseSelectedFontChanged();
			}
		}
		public void RaiseFontSizeChanged()
		{
			if (!noRaiseEvent)
			{
				if (SelectedFontSizeChanged != null) SelectedFontSizeChanged(this, null);
				RaiseSelectedFontChanged();
			}
		}
		public void RaiseFontStyleChanged()
		{
			if (!noRaiseEvent)
			{
				if (SelectedFontStyleChanged != null) SelectedFontStyleChanged(this, null);
				RaiseSelectedFontChanged();
			}
		}
		public void RaiseSelectedFontChanged()
		{
			if (!noRaiseEvent)
			{
				if (SelectedFontChanged != null) SelectedFontChanged(this, null);
			}
		}

		public event EventHandler SelectedFontChanged;
		public event EventHandler SelectedFontNameChanged;
		public event EventHandler SelectedFontSizeChanged;
		public event EventHandler SelectedFontStyleChanged;
		public event EventHandler SelectedFontColorChanged;

		public bool ShowColorPicker
		{
			get
			{
				return fontColor.Visible;
			}
			set
			{
				labColor.Visible = fontColor.Visible = value;
			}
		}

		public Color FontColor
		{
			set
			{
				fontColor.SolidColor = value;
			}
			get
			{
				return fontColor.SolidColor;
			}
		}
	}
}
