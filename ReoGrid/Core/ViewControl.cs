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
using System.Linq;
using System.Text;

#if DEBUG
using System.Diagnostics;
#endif // DEBUG

using unvell.ReoGrid.Views;

namespace unvell.ReoGrid
{
	partial class Worksheet
	{
		private bool suspendingUIUpdates = false;

		/// <summary>
		/// Suspend worksheet UI updates.
		/// </summary>
		public void SuspendUIUpdates()
		{
			this.suspendingUIUpdates = true;
		}

		/// <summary>
		/// Resume worksheet UI updates.
		/// </summary>
		public void ResumeUIUpdates()
		{
			if (this.suspendingUIUpdates)
			{
				this.suspendingUIUpdates = false;
				this.RequestInvalidate();
			}
		}

		/// <summary>
		/// Check whether UI updates is suspending.
		/// </summary>
		public bool IsUIUpdatesSuspending { get { return this.suspendingUIUpdates; } }

		private IViewportController viewportController;

		/// <summary>
		/// Get or set viewport controller for worksheet.
		/// </summary>
		internal IViewportController ViewportController
		{
			get { return this.viewportController; }
			set { this.viewportController = value; }
		}

		internal void UpdateViewportController()
		{
			if (this.suspendingUIUpdates) return;

#if DEBUG
			Stopwatch sw = Stopwatch.StartNew();
#endif // DEBUG

			AutoAdjustRowHeaderPanelWidth();

			if (viewportController != null)
			{
				this.viewportController.UpdateController();

				if (this.IsFrozen)
				{
					this.FreezeToCell(this.FreezePos, this.FreezeArea);
				}
			}

#if PRINT // TODO: why do this here
			// update page breaks 
			if (this.HasSettings(WorksheetSettings.View_ShowPageBreaks)
				&& this.rows.Count > 0 && this.cols.Count > 0)
			{
				AutoSplitPage();
			}
#endif // PRINT

#if DEBUG
			sw.Stop();
			long ms = sw.ElapsedMilliseconds;
			if (ms > 15)
			{
				Debug.WriteLine("updating viewport controller takes " + sw.ElapsedMilliseconds + " ms.");
			}
#endif // DEBUG
		}

		internal void UpdateViewportControllBounds()
		{
			// update boundary of viewportcontroller 
			if (this.viewportController != null && this.controlAdapter != null)
			{
				// don't compare Bounds before set
				this.viewportController.Bounds = this.controlAdapter.GetContainerBounds();
				
				// need to update controller anytime when this method is called
				this.viewportController.UpdateController();
			}
		}

		// reserved
		//private bool isLeadHeadHover = false;
		internal bool isLeadHeadSelected = false;

		#region Invalidations

		/// <summary>
		/// Request to repaint entire worksheet.
		/// </summary>
		public void RequestInvalidate()
		{
			if (!this.viewDirty && !this.suspendingUIUpdates)
			{
				this.viewDirty = true;

				if (this.controlAdapter != null) this.controlAdapter.Invalidate();
			}
		}

		#endregion // Invalidations

		/// <summary>
		/// Get or set view mode of current worksheet (Reserved)
		/// </summary>
		public ReoGridViewMode ViewMode { get; set; }

	}
}
