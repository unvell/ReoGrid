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

#if WINFORM

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using unvell.Common.Win32Lib;

namespace unvell.ReoGrid
{
	// Must inherit Control, not Component, in order to have Handle
	// http://stackoverflow.com/questions/621577/clipboard-event-c-sharp/1394225#1394225
	[DefaultEvent("ClipboardChanged")]
	internal class ClipboardMonitor : Control
	{
		private static readonly ClipboardMonitor instance = new ClipboardMonitor();
		public static ClipboardMonitor Instance { get { return instance; } }

		IntPtr nextClipboardViewer;

		private ClipboardMonitor()
		{
			this.BackColor = System.Drawing.Color.Red;
			this.Visible = false;

			nextClipboardViewer = Win32.SetClipboardViewer(this.Handle);
		}

		/// <summary>
		/// Clipboard contents changed.
		/// </summary>
		public event EventHandler<ClipboardChangedEventArgs> ClipboardChanged;

		protected override void WndProc(ref System.Windows.Forms.Message m)
		{
			// defined in winuser.h
			const int WM_DRAWCLIPBOARD = 0x308;
			const int WM_CHANGECBCHAIN = 0x030D;

			switch (m.Msg)
			{
				case WM_DRAWCLIPBOARD:
					OnClipboardChanged();
					Win32.SendMessage(nextClipboardViewer, (uint)m.Msg, m.WParam, m.LParam);
					break;

				case WM_CHANGECBCHAIN:
					if (m.WParam == nextClipboardViewer)
						nextClipboardViewer = m.LParam;
					else
						Win32.SendMessage(nextClipboardViewer, (uint)m.Msg, m.WParam, m.LParam);
					break;

				default:
					base.WndProc(ref m);
					break;

				case (int)Win32.WMessages.WM_DESTROY:
					Win32.ChangeClipboardChain(this.Handle, nextClipboardViewer);
					base.WndProc(ref m);
					break;
			}
		}

		void OnClipboardChanged()
		{
			try
			{
				IDataObject iData = Clipboard.GetDataObject();
				if (ClipboardChanged != null)
				{
					ClipboardChanged(this, new ClipboardChangedEventArgs(iData));
				}

			}
			catch (Exception e)
			{
				// Swallow or pop-up, not sure
				// Trace.Write(e.ToString());
				MessageBox.Show(e.ToString());
			}
		}
	}

	/// <summary>
	/// Argument for clipboard changing event.
	/// </summary>
	public class ClipboardChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Get data object from clipboard.
		/// </summary>
		public readonly IDataObject DataObject;

		/// <summary>
		/// Create event argument.
		/// </summary>
		/// <param name="dataObject">Data object that is transformed by Clipboard.</param>
		public ClipboardChangedEventArgs(IDataObject dataObject)
		{
			DataObject = dataObject;
		}
	}
}
#endif // WINFORM
