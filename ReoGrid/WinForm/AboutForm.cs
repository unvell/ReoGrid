﻿/*****************************************************************************
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
 * Copyright (c) 2012-2023 Jingwood <jingwood at unvell.com>
 * Copyright (c) 2012-2023 unvell inc. All rights reserved.
 * 
 ****************************************************************************/

#if WINFORM

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace unvell.ReoGrid.WinForm
{
	/// <summary>
	/// About dialog form of ReoGrid
	/// </summary>
	public partial class AboutForm : Form
	{
		/// <summary>
		/// Create about dialog
		/// </summary>
		public AboutForm()
		{
			InitializeComponent();

			lnkHP.Click += (s, e) => RGUtility.OpenFileOrLink(lnkHP.Text);
			labVersion.Text = "version " + ProductVersion.ToString();

			textBox2.Text = unvell.ReoGrid.Properties.Resources.EULA_EN;
		}
	}
}

#endif