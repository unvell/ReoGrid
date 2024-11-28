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
using System.Linq;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using System.Reflection;

using unvell.ReoGrid.DataFormat;
using unvell.ReoGrid.Actions;
using unvell.UIControls;
using unvell.ReoGrid.Editor.LangRes;

namespace unvell.ReoGrid.PropertyPages
{
	public partial class FormatPage : UserControl, IPropertyPage
	{
		private ReoGridControl grid;

		public ReoGridControl Grid
		{
			get { return grid; }
			set { this.grid = value; }
		}

		public void SetupUILanguage()
		{
			labCategory.Text = LangResource.FormatPage_Category;
			grpSample.Text = LangResource.Sample;

			labDecimalPlacesNum.Text = LangResource.FormatPage_Decimal_Places;
			labDecimalPlacesCurrency.Text = LangResource.FormatPage_Decimal_Places;
			labDecimalPlacesPercent.Text = LangResource.FormatPage_Decimal_Places;

			labNegativeNumbersNum.Text = LangResource.FormatPage_Negative_Numbers;
			labNegativeNumberCurrency.Text = LangResource.FormatPage_Negative_Numbers;

			chkNumberUseSeparator.Text = string.Format(LangResource.FormatPage_Use_1000_Separator,
				Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator);

			labSymbol.Text = LangResource.FormatPage_Symbol;
			labType.Text = LangResource.FormatPage_Type;
			labDateTimePattern.Text = LangResource.FormatPage_Pattern;
			labLocale.Text = LangResource.FormatPage_Locale;
		}

		private Cell sampleCell;

		private object originalData = null;

		private CellDataFormatFlag? currentFormat = null;
		private object currentFormatArgs = null;

		private Panel currentSettingPanel = null;

		public FormatPage()
		{
			InitializeComponent();

			#region Initialize Setting Panels

			numberNegativeStyleList.Items.Add(new NegativeStyleListItem(
				NumberDataFormatter.NumberNegativeStyle.Default, (-1234.10f).ToString("###,###.00"), SystemColors.WindowText));
			numberNegativeStyleList.Items.Add(new NegativeStyleListItem(
				NumberDataFormatter.NumberNegativeStyle.Red, (1234.10f).ToString("###,###.00"), Color.Red));
			numberNegativeStyleList.Items.Add(new NegativeStyleListItem(
				NumberDataFormatter.NumberNegativeStyle.Brackets, (1234.10f).ToString("(###,###.00)"), SystemColors.WindowText));
			numberNegativeStyleList.Items.Add(new NegativeStyleListItem(
				NumberDataFormatter.NumberNegativeStyle.RedBrackets, (1234.10f).ToString("(###,###.00)"), Color.Red));

			if (numberNegativeStyleList.Items.Count > 0) numberNegativeStyleList.SelectedIndex = 0;

			datetimeFormatList.SelectedIndexChanged += (s, e) =>
			{
				if (datetimeFormatList.SelectedItem != null)
				{
					DatetimeFormatListItem item = (DatetimeFormatListItem)datetimeFormatList.SelectedItem;
					txtDatetimeFormat.Text = item.Pattern;
				}
			};

			var currentCulture = Thread.CurrentThread.CurrentCulture;
		
			datetimeFormatList.Items.AddRange(new object[] {
				// culture patterns
				new DatetimeFormatListItem(true, currentCulture.DateTimeFormat.ShortDatePattern),
				new DatetimeFormatListItem(true, currentCulture.DateTimeFormat.LongDatePattern),
				new DatetimeFormatListItem(true, currentCulture.DateTimeFormat.ShortDatePattern 
					+ " " + currentCulture.DateTimeFormat.ShortTimePattern),
				new DatetimeFormatListItem(true, currentCulture.DateTimeFormat.LongDatePattern 
					+ " " + currentCulture.DateTimeFormat.LongTimePattern),
				new DatetimeFormatListItem(true, currentCulture.DateTimeFormat.ShortTimePattern),
				new DatetimeFormatListItem(true, currentCulture.DateTimeFormat.LongTimePattern),

				// predefine patterns
				new DatetimeFormatListItem(false, "yyyy/M/d"),
				new DatetimeFormatListItem(false, "yyyy/M/d hh:mm"),
				new DatetimeFormatListItem(false, "M/d"),
				new DatetimeFormatListItem(false, "hh:mm"),
			});

			datetimeLocationList.SelectedIndexChanged += (s, e) =>
			{
				var ci = (CultureInfo)datetimeLocationList.SelectedItem;

				((DatetimeFormatListItem)(datetimeFormatList.Items[0])).Pattern = ci.DateTimeFormat.ShortDatePattern;
				((DatetimeFormatListItem)(datetimeFormatList.Items[1])).Pattern = ci.DateTimeFormat.LongDatePattern;
				((DatetimeFormatListItem)(datetimeFormatList.Items[2])).Pattern = ci.DateTimeFormat.ShortDatePattern + " " + ci.DateTimeFormat.ShortTimePattern;
				((DatetimeFormatListItem)(datetimeFormatList.Items[3])).Pattern = ci.DateTimeFormat.LongDatePattern + " " + ci.DateTimeFormat.LongTimePattern;
				((DatetimeFormatListItem)(datetimeFormatList.Items[4])).Pattern = ci.DateTimeFormat.ShortTimePattern;
				((DatetimeFormatListItem)(datetimeFormatList.Items[5])).Pattern = ci.DateTimeFormat.LongTimePattern;

				DateTime dt = new DateTime(1980, 7, 13);
				CultureInfo culture = (CultureInfo)datetimeLocationList.SelectedItem;
				foreach (DatetimeFormatListItem item in datetimeFormatList.Items)
				{
					item.Sample = dt.ToString(item.Pattern, culture);
				}

				typeof(ListBox).InvokeMember("RefreshItems", 
					BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod,
					null, datetimeFormatList, new object[] { });

				object backup = datetimeFormatList.SelectedItem;
				if (backup != null)
				{
					datetimeFormatList.SelectedItem = null;
					datetimeFormatList.SelectedItem = backup;
				}
				else
				{
					datetimeFormatList.SelectedIndex = 0;
				}
			};

			currencySymbolList.SelectedIndexChanged += (s, e) =>
			{
				var listItem = (CurrencySymbolListItem)currencySymbolList.SelectedItem;

				currencyNegativeStyleList.Items.Clear();

				currencyNegativeStyleList.Items.Add(new NegativeStyleListItem(
					NumberDataFormatter.NumberNegativeStyle.Default, /*"-$1,234.10 ###,###.00"*/(1234.10f).ToString("c", listItem.Culture), SystemColors.WindowText));
				currencyNegativeStyleList.Items.Add(new NegativeStyleListItem(
					NumberDataFormatter.NumberNegativeStyle.Red, /*"$1,234.10"*/(1234.10f).ToString("c", listItem.Culture), Color.Red));
				currencyNegativeStyleList.Items.Add(new NegativeStyleListItem(
					NumberDataFormatter.NumberNegativeStyle.Brackets, /*"($1,234.10)"*/"(" + (1234.10f).ToString("c", listItem.Culture) + ")", SystemColors.WindowText));
				currencyNegativeStyleList.Items.Add(new NegativeStyleListItem(
					NumberDataFormatter.NumberNegativeStyle.RedBrackets, /*"($1,234.10)"*/"(" + (1234.10f).ToString("c", listItem.Culture) + ")", Color.Red));

				if (currentFormatArgs != null)
				{
					currencyFormatArgs.PrefixSymbol = null;
					currencyFormatArgs.PostfixSymbol = null;
				}

        if (currencyNegativeStyleList.Items.Count > 0) currencyNegativeStyleList.SelectedIndex = 0;

				UpdateSample();
			};

			var cultures = CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures).Where(c => !c.IsNeutralCulture).OrderBy(c => c.EnglishName);

			foreach (CultureInfo info in cultures)
			{
				datetimeLocationList.Items.Add(info);

				if (info.Equals(currentCulture))
				{
					datetimeLocationList.SelectedItem = info;
				}

				try
				{
					var item = new CurrencySymbolListItem(info);

					currencySymbolList.Items.Add(item);

					if (info.Equals(currentCulture))
					{
						currencySymbolList.SelectedItem = item;
					}
				}
				catch { }
			}

			// add valid data formatter
			foreach (var key in Enum.GetValues(typeof(CellDataFormatFlag)))
			{
				if (DataFormatterManager.Instance.DataFormatters.TryGetValue((CellDataFormatFlag) key, out var formatter))
				{
					formatList.Items.Add(key);
				}
			}

			numberFormatArgs = new NumberDataFormatter.NumberFormatArgs
			{
				DecimalPlaces = 2,
				NegativeStyle = NumberDataFormatter.NumberNegativeStyle.Default,
				UseSeparator = true,
			};
			datetimeFormatArgs = new DateTimeDataFormatter.DateTimeFormatArgs
			{
				Format = currentCulture.DateTimeFormat.ShortDatePattern,
			};
			currencyFormatArgs = new CurrencyDataFormatter.CurrencyFormatArgs()
			{
				CultureEnglishName = currentCulture.EnglishName,
				DecimalPlaces = (short)currentCulture.NumberFormat.CurrencyDecimalDigits,
				NegativeStyle = NumberDataFormatter.NumberNegativeStyle.Default,
			};

			#endregion

			formatList.SelectedIndexChanged += (sender, e) =>
			{
				try
				{
					currentFormat = (CellDataFormatFlag)Enum.Parse(typeof(CellDataFormatFlag), formatList.Text);
				}
				catch
				{
					currentFormat = CellDataFormatFlag.General;
				}

				Panel newSettingPanel = null;

				switch (currentFormat)
				{
					case CellDataFormatFlag.Number:
						newSettingPanel = numberPanel;
						break;
					case CellDataFormatFlag.DateTime:
						newSettingPanel = datetimePanel;
						break;
					case CellDataFormatFlag.Currency:
						newSettingPanel = currencyPanel;
						break;
					case CellDataFormatFlag.Percent:
						newSettingPanel = percentPanel;
						break;
				}

				if (newSettingPanel != currentSettingPanel)
				{
					if (newSettingPanel != null)
					{
						newSettingPanel.Location = new Point(grpSample.Left - 5, grpSample.Bottom + 10);
						newSettingPanel.Show();
					}

					if (currentSettingPanel != null)
					{
						currentSettingPanel.Hide();
					}

					currentSettingPanel = newSettingPanel;
				}

				UpdateSample();
			};

			numberDecimalPlaces.ValueChanged += (s, e) => UpdateSample();
			chkNumberUseSeparator.CheckedChanged += (s, e) => UpdateSample();
			numberNegativeStyleList.SelectedIndexChanged += (s, e) => UpdateSample();
			currencyDecimalPlaces.ValueChanged += (s, e) => UpdateSample();
			currencyNegativeStyleList.SelectedIndexChanged += (s, e) => UpdateSample();
			percentDecimalPlaces.ValueChanged += (s, e) => UpdateSample();

			formatList.DoubleClick += (s, e) => RaiseDone();
			numberNegativeStyleList.DoubleClick += (s, e) => RaiseDone();
			datetimeFormatList.DoubleClick += (s, e) => RaiseDone();
			currencyNegativeStyleList.DoubleClick += (s, e) => RaiseDone();

			txtDatetimeFormat.TextChanged += (s, e) => UpdateSample();
		}

		public event EventHandler Done;
		public void RaiseDone()
		{
			if (Done != null) Done(this, null);
		}

		private NumberDataFormatter.NumberFormatArgs numberFormatArgs;
		private DateTimeDataFormatter.DateTimeFormatArgs datetimeFormatArgs;
		private CurrencyDataFormatter.CurrencyFormatArgs currencyFormatArgs;

		private void UpdateSample()
		{
			currentFormatArgs = null;

			switch (currentFormat)
			{
				case CellDataFormatFlag.Number:
					numberFormatArgs.DecimalPlaces = (short)numberDecimalPlaces.Value;
					numberFormatArgs.NegativeStyle = ((NegativeStyleListItem)numberNegativeStyleList.SelectedItem).NegativeStyle;
					numberFormatArgs.UseSeparator = chkNumberUseSeparator.Checked;
					currentFormatArgs = numberFormatArgs;
					break;

				case CellDataFormatFlag.DateTime:
					datetimeFormatArgs.Format = txtDatetimeFormat.Text;
					datetimeFormatArgs.CultureName = ((CultureInfo)(datetimeLocationList.SelectedItem)).Name;
					currentFormatArgs = datetimeFormatArgs;
					break;

				case CellDataFormatFlag.Currency:
					if (currencySymbolList.SelectedItem != null)
					{
						var symbolInfo = ((CurrencySymbolListItem)currencySymbolList.SelectedItem);
						var culture = symbolInfo.Culture;

						currencyFormatArgs.CultureEnglishName = culture.EnglishName;

						switch (culture.NumberFormat.CurrencyPositivePattern)
						{
							case 0: currencyFormatArgs.PrefixSymbol = culture.NumberFormat.CurrencySymbol; break;
							case 1: currencyFormatArgs.PostfixSymbol = culture.NumberFormat.CurrencySymbol; break;
							case 2: currencyFormatArgs.PrefixSymbol = " " + culture.NumberFormat.CurrencySymbol; break;
							case 3: currencyFormatArgs.PostfixSymbol = " " + culture.NumberFormat.CurrencySymbol; break;
						}

						currencyFormatArgs.NegativeStyle = ((NegativeStyleListItem)currencyNegativeStyleList.SelectedItem).NegativeStyle;
						currencyFormatArgs.DecimalPlaces = (short)currencyDecimalPlaces.Value;
						currentFormatArgs = currencyFormatArgs;
					}
					break;

				case CellDataFormatFlag.Percent:
					numberFormatArgs.DecimalPlaces = (short)percentDecimalPlaces.Value;
					currentFormatArgs = numberFormatArgs;
					break;
			}

			if (sampleCell != null)
			{
				sampleCell.Data = originalData;
				sampleCell.DataFormat = currentFormat ?? CellDataFormatFlag.General;
				sampleCell.DataFormatArgs = currentFormatArgs;

				// force format sample cell
				labSample.Text = DataFormatterManager.Instance.DataFormatters[sampleCell.DataFormat].FormatCell(sampleCell);

				var renderColor = sampleCell.RenderColor;
				labSample.ForeColor = renderColor.IsTransparent ? Color.Black : (Color)renderColor;
			}
		}

		private CellDataFormatFlag? backupFormat;
		private object backupFormatArgs;

		public void LoadPage()
		{
			var sheet = this.grid.CurrentWorksheet;

			sheet.IterateCells(sheet.SelectionRange, (r, c, cell) =>
			{
				if (backupFormat == null)
				{
					sampleCell = cell.Clone();
					unvell.ReoGrid.Utility.CellUtility.CopyCellContent(sampleCell, cell);

					if (cell != null) originalData = cell.Data;

					backupFormat = cell.DataFormat;
					return true;
				}
				else if (backupFormat == cell.DataFormat)
				{
					return true;
				}
				else
				{
					backupFormat = null;
					return false;
				}
			});

			currentFormat = backupFormat;

			backupFormatArgs = null;

			if (currentFormat != null)
			{
				switch (currentFormat)
				{
					case CellDataFormatFlag.Number:
						if (sampleCell.DataFormatArgs is NumberDataFormatter.NumberFormatArgs)
						{
							NumberDataFormatter.NumberFormatArgs nargs = (NumberDataFormatter.NumberFormatArgs)sampleCell.DataFormatArgs;
							numberDecimalPlaces.Value = nargs.DecimalPlaces;
							chkNumberUseSeparator.Checked = nargs.UseSeparator;
							foreach (NegativeStyleListItem item in numberNegativeStyleList.Items)
							{
								if (item.NegativeStyle == nargs.NegativeStyle)
								{
									numberNegativeStyleList.SelectedItem = item;
									break;
								}
							}
							backupFormatArgs = nargs;
						}
						break;

					case CellDataFormatFlag.DateTime:
						DateTimeDataFormatter.DateTimeFormatArgs dargs = (DateTimeDataFormatter.DateTimeFormatArgs)sampleCell.DataFormatArgs;
						txtDatetimeFormat.Text = dargs.Format;
						int dfindex =-1;
						for (int i = 0; i < datetimeFormatList.Items.Count; i++)
						{
							DatetimeFormatListItem item = (DatetimeFormatListItem)datetimeFormatList.Items[i];
							if (item.Pattern.Equals(dargs.Format, StringComparison.CurrentCultureIgnoreCase))
							{
								dfindex = i;
								break;
							}
						}
						datetimeFormatList.SelectedIndex = dfindex;
						backupFormatArgs = dargs;
						break;

					case CellDataFormatFlag.Currency:
						var cargs = (CurrencyDataFormatter.CurrencyFormatArgs)sampleCell.DataFormatArgs;

						var cultureName = cargs.CultureEnglishName;

						foreach (var currencyCultureItem in currencySymbolList.Items)
						{
							if (string.Compare(((CurrencySymbolListItem)currencyCultureItem).Culture.EnglishName, cultureName, true) == 0)
							{
								currencySymbolList.SelectedItem = currencyCultureItem;
								break;
							}
						}

						currencyDecimalPlaces.Value = cargs.DecimalPlaces;

						int cnindex = (int)cargs.NegativeStyle;
						if (cnindex >= 0 && cnindex < currencyNegativeStyleList.Items.Count) currencyNegativeStyleList.SelectedIndex = cnindex;

						foreach (NegativeStyleListItem item in currencyNegativeStyleList.Items)
						{
							if (item.NegativeStyle == cargs.NegativeStyle)
							{
								currencyNegativeStyleList.SelectedItem = item;
								break;
							}
						}

						backupFormatArgs = cargs;
						break;

					case CellDataFormatFlag.Percent:
						var pargs = (NumberDataFormatter.NumberFormatArgs)sampleCell.DataFormatArgs;
						percentDecimalPlaces.Value = pargs.DecimalPlaces;
						backupFormatArgs = pargs;
						break;
				}

				for (int i = 0; i < formatList.Items.Count; i++)
				{
					var item = formatList.Items[i].ToString();

					if (string.Equals(item, currentFormat.ToString(), StringComparison.CurrentCultureIgnoreCase))
					{
						formatList.SelectedIndex = i;
						break;
					}
				}
			}
			else
			{
				formatList.SelectedIndex = 0;
			}

			backupFormat = currentFormat;
		}

		public WorksheetReusableAction CreateUpdateAction()
		{
			if (currentFormat != backupFormat || currentFormatArgs != backupFormatArgs
				&& currentFormat != null)
			{
				return new SetRangeDataFormatAction(grid.CurrentWorksheet.SelectionRange,
					(CellDataFormatFlag)currentFormat, currentFormatArgs);
			}
			else
				return null;
		}

		#region CurrencySymbolListItem
		private struct CurrencySymbolListItem
		{
			public CultureInfo Culture { get; set; }

			public CurrencySymbolListItem(CultureInfo culture)
				: this()
			{
				this.Culture = culture;
			}

			public override string ToString()
			{
				// some cultures doesn't have currency symbol
				try
				{
					try
					{
						return string.Format("{0} ({1})", this.Culture.EnglishName, this.Culture.NumberFormat.CurrencySymbol);
					}
					catch
					{
						return string.Format("{0}", this.Culture.EnglishName);
					}
				}
				catch
				{
					return Culture.EnglishName;
				}
      }
		}
		#endregion // CurrencySymbolListItem

		#region NegativeStyleListItem
		private struct NegativeStyleListItem : IColoredListBoxItem
		{
			public NumberDataFormatter.NumberNegativeStyle NegativeStyle { get; set; }

			public string Sample { get; set; }

			public Color TextColor { get; set; }
			public Color BackColor { get; set; }

			public NegativeStyleListItem(NumberDataFormatter.NumberNegativeStyle negativeStyle,
				string sample, Color textColor)
				: this(negativeStyle, sample, textColor, Color.Empty)
			{
			}

      public NegativeStyleListItem(NumberDataFormatter.NumberNegativeStyle negativeStyle,
				string sample, Color textColor, Color backColor)
				: this()
			{
				this.NegativeStyle = negativeStyle;
				this.Sample = sample;
				this.TextColor = textColor;
				this.BackColor = backColor;
			}

			public override string ToString()
			{
				return this.Sample;
			}
		}
		#endregion // NegativeStyleListItem

		#region DatetimeFormatListItem
		private class DatetimeFormatListItem
		{
			public bool InCulture { get; set; }

			public string Pattern { get; set; }

			public string Sample { get; set; }

			public DatetimeFormatListItem() { }

			public DatetimeFormatListItem(bool inCulture, string pattern)
			{
				this.InCulture = inCulture;
				this.Pattern = pattern;
			}

			public override string ToString()
			{
				return (this.InCulture ? "*": string.Empty) + this.Sample;
			}
		}
		#endregion // DatetimeFormatListItem
	}
}
