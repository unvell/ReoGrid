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

namespace unvell.ReoGrid.Demo.Scripts
{
	public partial class LoadScriptDocDemo : UserControl
	{
		public LoadScriptDocDemo()
		{
			InitializeComponent();

			// get first worksheet instance
			var sheet = reoGridControl.Worksheets[0];

			// load tepmlate from RGF file.
			// RGF file is a file format that contains worksheet information, 
			// such as data, styles, borders, formula and etc, RGF file can 
			// be saved and loaded by ReoGrid and ReoGridEditor.
			//
			// https://reogrid.net/document/rgf-format
			// 
			sheet.LoadRGF("_Templates\\RGF\\change_colors.rgf");

			// hide sheet tab control
			reoGridControl.SetSettings(WorkbookSettings.View_ShowSheetTabControl, false);

			// hide row header and column header
			sheet.SetSettings(WorksheetSettings.View_ShowHeaders, false);

			// set entire worksheet read-only
			sheet.SetSettings(WorksheetSettings.Edit_Readonly, true);

			reoScriptEditorControl1.Text = reoGridControl.Script;
		}

		private void btnRun_Click(object sender, EventArgs e)
		{
			reoGridControl.RunScript();
		}

		private void btnStop_Click(object sender, EventArgs e)
		{
			reoGridControl.Srm.ForceStop();
		}
	}
}
