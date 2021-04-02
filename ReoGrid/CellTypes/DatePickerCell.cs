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
 * Author: Jing Lu <jingwood at unvell.com>
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

#if WINFORM
using System.Windows.Forms;
using RGFloat = System.Single;
using RGImage = System.Drawing.Image;
#else
using RGFloat = System.Double;
using RGImage = System.Windows.Media.ImageSource;
#endif // WINFORM

namespace unvell.ReoGrid.CellTypes
{
#if WINFORM
	/// <summary>
	/// Represetns a date picker cell on worksheet.
	/// </summary>
	[Serializable]
	public class DatePickerCell : DropdownCell
	{
		private MonthCalendar calendar = null;

		/// <summary>
		/// Create instance of date picker cell.
		/// </summary>
		public DatePickerCell()
		{
			calendar = new MonthCalendar()
			{
				CalendarDimensions = new System.Drawing.Size(1, 1),
				MaxSelectionCount = 1,
			};

			calendar.DateSelected += Calendar_DateSelected;

			base.DropdownControl = calendar;
			base.MinimumDropdownWidth = calendar.Width + 20;
			base.DropdownPanelHeight = calendar.Height + 10;
		}

		private void Calendar_DateSelected(object sender, DateRangeEventArgs e)
		{
			if (this.Cell != null) this.Cell.Data = calendar.SelectionStart;
		}

		/// <summary>
		/// Override method invoked when this cell body is set into any cells.
		/// </summary>
		/// <param name="cell">The cell this body will be set to.</param>
		public override void OnSetup(Cell cell)
		{
			base.OnSetup(cell);

			cell.Style.Indent = 3;
			cell.DataFormat = DataFormat.CellDataFormatFlag.DateTime;
		}

		/// <summary>
		/// Clone new date picker from this object.
		/// </summary>
		/// <returns>New instance of date picker.</returns>
		public override ICellBody Clone()
		{
			return new DatePickerCell();
		}
	}
#endif // WINFORM
}
