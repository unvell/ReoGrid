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

#if PRINT

#if WINFORM

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows;
using unvell.ReoGrid.Print;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Views;

namespace unvell.ReoGrid.Print
{
	/// <summary>
	/// Represents a print session. Print session can be created by single worksheet or a workbook.
	/// Print session created by workbook can be used to print all target worksheets.
	/// </summary>
	partial class PrintSession
	{
		// used in GDI print
		private System.Drawing.Printing.PageSettings currentGDIPageSettings;

		internal void Init()
		{
			this.printDocument = new PrintDocument();
			
			this.printDocument.PrintPage += doc_PrintPage;
			this.printDocument.BeginPrint += doc_BeginPrint;
			this.printDocument.EndPrint += doc_EndPrint;
			this.printDocument.QueryPageSettings += printDocument_QueryPageSettings;
		}
		
		#region Print Events

		void printDocument_QueryPageSettings(object sender, QueryPageSettingsEventArgs e)
		{
			e.PageSettings = this.currentGDIPageSettings;
		}
		
		void doc_BeginPrint(object sender, PrintEventArgs e)
		{
			if (this.IsPrinting)
			{
				throw new ReoGridPrintException("Another print session is in progress, try to create a new session to print.");
			}

			this.IsPrinting = true;

			this.CurrentWorksheetIndex = -1;

			NextWorksheet();
		}

		void doc_PrintPage(object sender, PrintPageEventArgs e)
		{
			this.NextPage(e.Graphics);

			e.HasMorePages = this.hasMorePages;
		}

		void doc_EndPrint(object sender, PrintEventArgs e)
		{
			this.IsPrinting = false;
		}
		#endregion // Print Events

		internal PrintDocument printDocument;

		/// <summary>
		/// Get printable document instance
		/// </summary>
		public PrintDocument PrintDocument { get { return this.printDocument; } }

		/// <summary>
		/// Start output document to printer
		/// </summary>
		public void Print()
		{
			foreach (var sheet in this.Worksheets)
			{
				sheet.Recalculate();
			}

			this.printDocument.Print();
		}

		/// <summary>
		/// Dispose print session and document instance.
		/// </summary>
		public void Dispose()
		{
			if (this.printDocument != null)
			{
				this.printDocument.Dispose();
			}
		}

	
	}
}

#endif // WINFORM

#endif // PRINT