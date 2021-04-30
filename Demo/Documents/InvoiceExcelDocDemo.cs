/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * ReoGrid and ReoGrid Demo project is released under MIT license.
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

namespace unvell.ReoGrid.Demo.Documents
{
	public partial class InvoiceExcelDocDemo : UserControl
	{
		public InvoiceExcelDocDemo()
		{
			InitializeComponent();

			var filename = "_Templates\\Excel\\simple-invoice.xlsx";

			// load tepmlate from Excel file.
			// https://reogrid.net/document/excel-file-format
			// 
			reoGridControl.Load(filename);

			var worksheet = reoGridControl.Worksheets[0];

			worksheet.Ranges["K16:L16"].Merge();

			var button = new CellTypes.ButtonCell("Open in Excel");
			button.Click += (s, e) => RGUtility.OpenFileOrLink(filename);

			worksheet["K16"] = button;
		}
	}
}
