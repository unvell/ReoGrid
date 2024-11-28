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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using unvell.ReoScript;

namespace unvell.ReoGrid.Demo.Scripts
{
	public partial class HandleEventsDemo : UserControl
	{
		private Worksheet worksheet;

		public HandleEventsDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;

			worksheet.Resize(14, 6);

			for (int r = 0; r < 10; r++)
			{
				for (int c = 0; c < 5; c++)
				{
					worksheet[r, c] = (r + 1) * (c + 1);
				}
			}

			chkCellBeforeEdit.CheckedChanged += UpdateEventHandlers;
			chkCellMouseDown.CheckedChanged += UpdateEventHandlers;
			chkCellMouseUp.CheckedChanged += UpdateEventHandlers;
			chkCellDataChanged.CheckedChanged += UpdateEventHandlers;
			chkSelectionChange.CheckedChanged += UpdateEventHandlers;
			chkSelectionMoveNext.CheckedChanged += UpdateEventHandlers;
			chkOnload.CheckedChanged += UpdateEventHandlers;
			chkUnload.CheckedChanged += UpdateEventHandlers;
			chkOnCopy.CheckedChanged += UpdateEventHandlers;
			chkOnPaste.CheckedChanged += UpdateEventHandlers;
			chkOnCut.CheckedChanged += UpdateEventHandlers;

			this.grid.Srm.AddStdOutputListener(new ListBoxStandardOutputListener { List = listbox1 });
		}

		#region For output message from script into listbox
		/// <summary>
		/// 
		///  ReoScript Standard Output 'console.log'
		///                   |
		///                   |
		///         Standard I/O Listeners
		///                   |
		///                   |
		///  ListBoxStandardOutputListener (this class)
		///                   |
		///                   |
		///                listbox1
		///  
		/// </summary>
		private class ListBoxStandardOutputListener : IStandardOutputListener
		{
			internal ListBox List { get; set; }

			private void AppendLine(string msg)
			{
				List.Items.Add(msg);
				List.SelectedIndex = List.Items.Count - 1;
			}
			
			public void Write(object obj)
			{
				AppendLine(obj.ToString());
			}

			public void Write(byte[] buf, int index, int count)
			{
				AppendLine(Encoding.Unicode.GetString(buf, index,count));
			}

			public void WriteLine(string line)
			{
				AppendLine(line);
			}
		}
		#endregion

		private void btnReset_Click(object sender, EventArgs e)
		{
			worksheet.Reset(14, 6);
		}

		private void UpdateEventHandlers(object sender, EventArgs e)
		{
			#region onload
			if (chkOnload.Checked)
			{
				grid.RunScript(" workbook.currentWorksheet.onload = function() { console.log('onload raised'); }; ");
			}
			else
			{
				grid.RunScript(" grid.onload = null; ");
			}
			#endregion // onload

			#region onmousedown
			if (chkCellMouseDown.Checked)
			{
				grid.RunScript(" workbook.currentWorksheet.onmousedown = function(pos) { console.log('onmousedown: ' + pos.row + ':' + pos.col); }; ");
			}
			else
			{
				grid.RunScript(" workbook.currentWorksheet.onmousedown = null; ");
			}
			#endregion // onmousedown

			#region onmouseup
			if (chkCellMouseUp.Checked)
			{
				grid.RunScript(" workbook.currentWorksheet.onmouseup = function(pos) { console.log('onmouseup: ' + pos.row + ':' + pos.col); }; ");
			}
			else
			{
				grid.RunScript(" workbook.currentWorksheet.onmouseup = null; ");
			}
			#endregion // onmouseup

			#region oncelledit
			if (chkCellBeforeEdit.Checked)
			{
				grid.RunScript(" workbook.currentWorksheet.oncelledit = function(cell) { console.log('oncelledit: ' + cell.row + ':' + cell.col); }; ");
			}
			else
			{
				grid.RunScript(" workbook.currentWorksheet.oncelledit = null; ");
			}
			#endregion // oncelledit

			#region onselectionchange
			if (chkSelectionChange.Checked)
			{
				grid.RunScript(" workbook.currentWorksheet.onselectionchange = function() { console.log('onselectionchange'); }; ");
			}
			else
			{
				grid.RunScript(" workbook.currentWorksheet.onselectionchange = null; ");
			}
			#endregion // onselectionchange

			#region onnextfocus
			if (chkSelectionMoveNext.Checked)
			{
				grid.RunScript(" workbook.currentWorksheet.onnextfocus = function() { console.log('onnextfocus'); }; ");
			}
			else
			{
				grid.RunScript(" workbook.currentWorksheet.onnextfocus = null; ");
			}
			#endregion // onnextfocus

			#region ondatachange
			if (chkCellDataChanged.Checked)
			{
				grid.RunScript(" workbook.currentWorksheet.ondatachange = function(cell) { console.log('ondatachange: ' + cell.pos + ':' + cell.data); }; ");
			}
			else
			{
				grid.RunScript(" workbook.currentWorksheet.ondatachange = null; ");
			}
			#endregion // ondatachange

			#region oncopy
			if (chkOnCopy.Checked)
			{
				grid.RunScript(" workbook.currentWorksheet.oncopy = function() { console.log('oncopy'); }; ");
			}
			else
			{
				grid.RunScript(" workbook.currentWorksheet.oncopy = null; ");
			}
			#endregion // oncopy

			#region onpaste
			if (chkOnPaste.Checked)
			{
				grid.RunScript(" workbook.currentWorksheet.onpaste = function() { console.log('onpaste'); }; ");
			}
			else
			{
				grid.RunScript(" workbook.currentWorksheet.onpaste = null; ");
			}
			#endregion // onpaste

			#region oncut
			if (chkOnCut.Checked)
			{
				grid.RunScript(" workbook.currentWorksheet.oncut = function() { console.log('oncut'); }; ");
			}
			else
			{
				grid.RunScript(" workbook.currentWorksheet.oncut = null; ");
			}
			#endregion // oncut
		}

	}
}
