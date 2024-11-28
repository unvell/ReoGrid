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

using unvell.ReoGrid.PropertyPages;
using unvell.ReoGrid.Actions;
using unvell.ReoGrid.Editor.LangRes;

namespace unvell.ReoGrid.Editor.PropertyPages
{
	public partial class ProtectionPage : UserControl, IPropertyPage
	{
		public ReoGridControl Grid { get; set; }

		public ProtectionPage()
		{
			InitializeComponent();
		}

		public void SetupUILanguage()
		{
			chkLocked.Text = LangResource.ProtectionPage_Locked;
		}

#pragma warning disable 67 // variable is never used
		/// <summary>
		/// Setting dialog will be closed when this event rasied
		/// </summary>
		public event EventHandler Done;
#pragma warning restore 67 // variable is never used

		public WorksheetReusableAction CreateUpdateAction()
		{
			return new SetRangeReadonlyAction(Grid.CurrentWorksheet.SelectionRange, this.chkLocked.Checked);
		}

		public void LoadPage()
		{
			var sheet = Grid.CurrentWorksheet;

			sheet.IterateCells(sheet.SelectionRange, (r, c, cell) =>
			{
				if (cell.IsReadOnly)
				{
					if (!chkLocked.Checked)
					{
						chkLocked.Checked = true;
					}
					else
					{
						chkLocked.CheckState = CheckState.Indeterminate;
						return false;
					}
				}

				return true;
			});
		}
	}

	/// <summary>
	/// Action to set range read-only.
	/// </summary>
	class SetRangeReadonlyAction : WorksheetReusableAction
	{
		/// <summary>
		/// The read-only value to be set.
		/// </summary>
		public bool SetToReadonly { get; private set; }

		/// <summary>
		/// Create instance of action to set range read-only value.
		/// </summary>
		/// <param name="range">The range to be applied.</param>
		/// <param name="setToReadonly">The read-only value to be set.</param>
		public SetRangeReadonlyAction(RangePosition range, bool setToReadonly)
			: base(range)
		{
			this.SetToReadonly = setToReadonly;
		}

		/// <summary>
		/// Backup all values of read-only cell from worksheet in order to undo this operation.
		/// </summary>
		private bool[,] readonlyBackup;

		/// <summary>
		/// Do this operation.
		/// </summary>
		public override void Do()
		{
			readonlyBackup = new bool[this.Range.Rows, this.Range.Cols];

			this.Worksheet.IterateCells(this.Range, false, (r, c, cell) =>
			{
				if (cell != null)
				{
					// read the read-only value from cell and backup it
					readonlyBackup[r - this.Range.Row, c - this.Range.Col] = cell.IsReadOnly;
				}
				else
				{
					cell = this.Worksheet.CreateAndGetCell(r, c);
				}

				// apply the new read-only value
				cell.IsReadOnly = this.SetToReadonly;

				return true;
			});
		}

		/// <summary>
		/// Undo this operation.
		/// </summary>
		public override void Undo()
		{
			this.Worksheet.IterateCells(this.Range, (r, c, cell) =>
			{
				// read backup read-only value to restore the original value for every cells
				cell.IsReadOnly = this.readonlyBackup[r - this.Range.Row, c - this.Range.Col];

				return true;
			});
		}

		/// <summary>
		/// Returns a friendly name for this action.
		/// </summary>
		/// <returns>The string name.</returns>
		public override string GetName()
		{
			return "Set Range Read-only: " + this.SetToReadonly;
		}

		/// <summary>
		/// Create a copy from this action in order to apply the operation to another range.
		/// </summary>
		/// <param name="range">New range where this operation will be appiled to.</param>
		/// <returns>New action instance copied from this action.</returns>
		public override WorksheetReusableAction Clone(RangePosition range)
		{
			return new SetRangeReadonlyAction(range, this.SetToReadonly);
		}
	}
}
