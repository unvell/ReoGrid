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

namespace unvell.ReoGrid
{
	/// <summary>
	/// Workbook Control Settings
	/// </summary>
	public enum WorkbookSettings : ulong
	{
		/// <summary>
		/// None
		/// </summary>
		None = 0x000000000L,

		/// <summary>
		/// Default Settings
		/// </summary>
		Default = View_Default | Behavior_Default | View_ShowScrolls 
#if EX_SCRIPT
			| Script_Default
#endif // EX_SCRIPT
			,

		#region Behavior

		/// <summary>
		/// Default behavior settings
		/// </summary>
		Behavior_Default = 0L,

		#endregion // Behavior

		#region View

		/// <summary>
		/// Default View Settings 
		/// </summary>
		View_Default = View_ShowSheetTabControl,

		/// <summary>
		/// Determine whether or not to show sheet tab control
		/// </summary>
		View_ShowSheetTabControl = 0x00010000L,

		/// <summary>
		/// Determine whether or not to show horizontal and vertical scroll bars
		/// </summary>
		View_ShowScrolls = View_ShowHorScroll | View_ShowVerScroll,

		/// <summary>
		/// Determine whether or not to show horizontal scroll bar
		/// </summary>
		View_ShowHorScroll = 0x00020000L,

		/// <summary>
		/// Determine whether or not to show vertical scroll bar
		/// </summary>
		View_ShowVerScroll = 0x00040000L,

		#endregion // View

		#region Script
#if EX_SCRIPT
		/// <summary>
		/// Default settings of script
		/// </summary>
		Script_Default = Script_AutoRunOnload | Script_PromptBeforeAutoRun,

		/// <summary>
		/// Whether to run script if grid loaded from file which contains script
		/// </summary>
		Script_AutoRunOnload = 0x10000000L,

		/// <summary>
		/// Confirm to user that whether allowed to run script if the script is loaded from a file
		/// </summary>
		Script_PromptBeforeAutoRun = 0x20000000L,
#endif // EX_SCRIPT
		#endregion // Script
	}
}
