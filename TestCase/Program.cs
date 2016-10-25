/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * http://reogrid.net/
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * Source code in test-case project released under BSD license.
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Linq;
using System.Windows.Forms;

namespace unvell.ReoGrid.Tests
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			TestCaseManager.Instance.Filters = args.Where(a => !a.StartsWith("-")).ToArray();

			if (args.Any(a => a.Equals("-gui", StringComparison.CurrentCultureIgnoreCase)))
			{
				Application.Run(new RunForm());
				return 0;
			}
			else
			{
				return new ConsoleRunner().Run() ? 0 : 1;
			}
		}


	}
}
