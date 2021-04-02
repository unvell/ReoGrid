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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

#if DEBUG
using System.Diagnostics;
#endif // DEBUG

#if FORMULA
using unvell.ReoGrid.Formula;
#endif // FORMULA

#if EX_SCRIPT
using unvell.ReoScript;
using unvell.ReoGrid.Script;
#endif // EX_SCRIPT

using unvell.ReoGrid.Properties;

namespace unvell.ReoGrid
{
	partial class Workbook
	{
		private string script;

		/// <summary>
		/// Script content for this control
		/// </summary>
		public string Script { get { return script; } set { script = value; } }

#if EX_SCRIPT
		/// <summary>
		/// ReoScript Runtime Machine 
		/// </summary>
		private ScriptRunningMachine srm;

		/// <summary>
		/// ReoScript Runtime Machine
		/// </summary>
		public ScriptRunningMachine Srm
		{
			get
			{
				if (srm == null)
				{
					InitSRM();
				}

				return srm;
			}
		}

		/// <summary>
		/// Run script belongs to this control
		/// </summary>
		/// <returns>Last value returned from script execution</returns>
		public object RunScript()
		{
			return RunScript(this.script);
		}

		/// <summary>
		/// Run specified script.
		/// </summary>
		/// <param name="script">Script to be executed.</param>
		/// <returns>Last value returned from script execution.</returns>
		public object RunScript(string script)
		{
			if (srm == null)
			{
				InitSRM();
			}

			try
			{
				return srm == null ? null : srm.Run(script == null ? this.script : script);
			}
			catch (Exception ex)
			{
				this.NotifyExceptionHappen(null, ex);
				return null;
			}
		}

		internal RSWorkbook workbookObj;

		/// <summary>
		/// Initial or reset Script Running Machine
		/// </summary>
		internal void InitSRM()
		{
#if DEBUG
			Stopwatch sw = Stopwatch.StartNew();
#endif

			if (srm == null)
			{
				// create ReoScript instance
				srm = new ScriptRunningMachine();

				// reinit instance when SRM is reset
				srm.Resetted += (s, e) => InitSRM();
			}

			// set control instance into script's context
			if (workbookObj == null)
			{
				workbookObj = new RSWorkbook(this);
			}

			srm["workbook"] = workbookObj;

#if V088_RESERVED
			// setup built-in functions
			RSFunctions.SetupBuiltinFunctions(this);
#endif // V088_RESERVED

			// load core library
			using (MemoryStream ms = new MemoryStream(Resources.base_lib))
			{
				srm.Load(ms);
			}

			if (this.SRMInitialized != null)
			{
				this.SRMInitialized(this, null);
			}

#if DEBUG
			sw.Stop();
			long ems = sw.ElapsedMilliseconds;
			if (ems > 10)
			{
				Debug.WriteLine("init srm takes " + ems + " ms.");
			}
#endif
		}

		internal event EventHandler SRMInitialized;
#endif // EX_SCRIPT
	}

#if EX_SCRIPT
	partial class Worksheet
	{
		internal RSWorksheet worksheetObj;

		internal ScriptRunningMachine Srm
		{
			get
			{
				return this.workbook == null ? null : this.workbook.Srm;
			}
		}

		/// <summary>
		/// Call script's function. The function must be method of workbook object.
		/// </summary>
		/// <param name="eventName">Function name of workbook object in script.</param>
		public object RaiseScriptEvent(string eventName)
		{
			return RaiseScriptEvent(eventName, null);
		}

		/// <summary>
		/// Call script's function. The function must be method of workbook object.
		/// </summary>
		/// <param name="eventName">Function name of workbook object in script.</param>
		/// <param name="eventArg">Function argument.</param>
		public object RaiseScriptEvent(string eventName, ObjectValue eventArg)
		{
			if (Srm != null)
			{
				return (worksheetObj != null) ? Srm.InvokeFunctionIfExisted(worksheetObj, eventName, eventArg) : null;
			}
			else
				return null;
		}
	}
#endif // EX_SCRIPT
}
