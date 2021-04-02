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
using unvell.ReoScript.Editor;

namespace unvell.ReoGrid.Demo.Scripts
{
	public partial class ReoQueryScriptDemo : UserControl
	{
		ReoScriptEditorControl scriptEditor;

		public ReoQueryScriptDemo()
		{
			InitializeComponent();

			// create script editor control

			scriptEditor = new ReoScript.Editor.ReoScriptEditorControl()
				{
					Dock = DockStyle.Fill,
				};

			// add script editor control into panel
			panel1.Controls.Add(scriptEditor);

			// make sure editor visible entirely
			scriptEditor.BringToFront();

			// synchronize script running container
			scriptEditor.Srm = reoGridControl.Srm;

			scriptEditor.Text = @"// Joke sample: reoquery

function reo(cell) {
  this.cell = cell;
  
  this.val = function(str) { 
    if(__args__.length == 0) {
      return this.cell.data;
    } else {
      this.cell.data = str;
      return this;
    }
  };
  
  this.style = function(key, value) {
    if (__args__.length == 1) {
      return this.cell.style[key];
    } else {
      this.cell.style[key] = value;
      return this;
    }
  };
}

script.$ = function(r, c) {
  var sheet = workbook.currentWorksheet;

  return new reo(c == null ? sheet.getCell(r) : sheet.getCell(r, c));
};

// call like jQuery
$('B4').val('hello').style('backgroundColor', 'yellow');
$(3, 2).val('world!').style('backgroundColor', 'lightgreen');


";

		}

		private void btnRun_Click(object sender, EventArgs e)
		{
			// copy script to grid control
			reoGridControl.Script = scriptEditor.Text;

			// run script
			reoGridControl.RunScript();
		}

	}
}
