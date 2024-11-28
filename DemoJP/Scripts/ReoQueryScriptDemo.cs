/*****************************************************************************
 * 
 * ReoGrid - .NET 表計算スプレッドシートコンポーネント
 * https://reogrid.net/jp
 *
 * ReoGrid 日本語版デモプロジェクトは MIT ライセンスでリリースされています。
 * 
 * このソフトウェアは無保証であり、このソフトウェアの使用により生じた直接・間接の損害に対し、
 * 著作権者は補償を含むあらゆる責任を負いません。 
 * 
 * Copyright (c) 2012-2016 unvell.com, All Rights Reserved.
 * https://www.unvell.com/jp
 * 
 ****************************************************************************/

using System;
using System.Windows.Forms;

using unvell.ReoScript.Editor;

namespace unvell.ReoGrid.Demo.Scripts
{
	/// <summary>
	/// スクリプト言語の応用例
	/// </summary>
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
