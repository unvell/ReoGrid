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
 * Author: Jingwood <jingwood at unvell.com>
 *
 * Copyright (c) 2012-2021 Jingwood, unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace unvell.ReoGrid.Views
{
	internal enum ViewTypes
	{
		None = 0x0,
		Cells = 0x1,

		ColumnHeader = 0x2,
		RowHeader = 0x4,
		LeadHeader = ColumnHeader | RowHeader,

		ColOutline = 0x10,
		RowOutline = 0x20,
		Outlines = ColOutline | RowOutline,
	}
}