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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;

using unvell.Common;
using unvell.Common.Win32Lib;
using unvell.ReoGrid;
using unvell.ReoGrid.WinForm;

namespace unvell.ReoGrid.Tests
{
	public partial class RunForm : Form
	{
		private static readonly ReoGridControl grid = new ReoGridControl();
		private static readonly DebugForm cellDebug = new DebugForm();
		private static readonly DebugForm borderDebug = new DebugForm() { InitTabType = DebugForm.InitTab.Border, };
		
		public RunForm()
		{
			InitializeComponent();

			cellDebug.Grid = grid.CurrentWorksheet;
			borderDebug.Grid = grid.CurrentWorksheet;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			TestCaseManager.Instance.Init();

			grid.Dock = DockStyle.Fill;
			panel1.Controls.Add(grid);

			foreach (TestSetInfo testCaseInfo in TestCaseManager.Instance.TestSets)
			{
				caseList.Items.Add(new ListViewItem(
					new string[] { testCaseInfo.Name, string.Empty, string.Empty, string.Empty, string.Empty })
				{
					Tag = testCaseInfo,
					ImageIndex = 0,
					Checked = true,
				});
			}

			TestCaseManager.Instance.BeforePerformTestCase += new EventHandler<TestCaseEventArgs>(Instance_BeforePerformTestCase);
			TestCaseManager.Instance.AfterPerformTestCase += new EventHandler<TestCaseEventArgs>(Instance_AfterPerformTestCase);

			Show();

			Logger.Off();

			runToolStripButton.PerformClick();
		}
	
		void Instance_BeforePerformTestCase(object sender, TestCaseEventArgs e)
		{
			bool showGrid = false;
		 	
			if(e.TestSetInfo.Instance is ReoGridTestSet)
			{
				((ReoGridTestSet)e.TestSetInfo.Instance).Grid = grid;

				if (((ReoGridTestSet)e.TestSetInfo.Instance).ShowControl)
				{
					showGrid = true;
				}
			}

			grid.Visible = showGrid;

			var listNode = GetItemByTestSetInfo(e.TestSetInfo);
			e.Cancel = listNode == null || !listNode.Checked;
		}

		private ListViewItem GetItemByTestSetInfo(TestSetInfo testSetInfo)
		{
			foreach (ListViewItem item in caseList.Items)
			{
				if (item.Tag == testSetInfo) return item;
			}
			return null;
		}

		void Instance_AfterPerformTestCase(object sender, TestCaseEventArgs e)
		{
			ListViewItem item = GetItemByTestSetInfo(e.TestSetInfo);

			item.SubItems[3].Text = string.Format("{0} ms.", e.TestCaseInfo.ElapsedMilliseconds);
			item.SubItems[4].Text = string.Format("{0} KB", (e.TestCaseInfo.MemoryUsage / 1024).ToString("###,###,##0"));

			Exception ex = e.TestCaseInfo.Exception;
			if (ex != null)
			{
				item.SubItems[2].Text = "Failed: " + ex.Message;
				item.ImageIndex = 2;

				exceptionBuffer.AppendLine(ex.ToString());
				exceptionBuffer.AppendLine();

				grid.Show();

				cellDebug.Show();
				cellDebug.Location = new Point(this.Right + 1, this.Top + 1);
				borderDebug.Show();
				borderDebug.Location = new Point(this.Right + 1, cellDebug.Bottom + 1);
			}
			else if (item.ImageIndex != 2)
			{
				item.SubItems[2].Text = "Success";
				item.ImageIndex = 1;
			}

			Application.DoEvents();
		}

		private StringBuilder exceptionBuffer = new StringBuilder();

		private void runToolStripButton_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem item in caseList.Items)
			{
				for (int i = 1; i < item.SubItems.Count; i++) item.SubItems[i].Text = string.Empty;
				item.ImageIndex = 0;
			}

			Application.DoEvents();

			exceptionBuffer.Length = 0;

			TestCaseManager.Instance.Run();

			txtException.Text = exceptionBuffer.ToString();
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.F5)
			{
				runToolStripButton.PerformClick();
				return true;
			}
			else
				return base.ProcessCmdKey(ref msg, keyData);
		}

		protected override void OnMove(EventArgs e)
		{
			base.OnMove(e);

			cellDebug.Location = new Point(this.Right + 1, this.Top + 1);
			borderDebug.Location = new Point(this.Right + 1, cellDebug.Bottom + 1);
		}

		ConsoleRunner consoleRunner = null;

		private void runInConsoleToolStripButton_Click(object sender, EventArgs e)
		{
			if (consoleRunner == null && Win32.AllocConsole())
			{
				consoleRunner = new ConsoleRunner();
			}

			if (consoleRunner != null)
			{
				IntPtr hwnd = Win32.GetConsoleWindow();
				if (hwnd != IntPtr.Zero)
				{
					Win32.BringWindowToTop(hwnd);
				}

				consoleRunner.Run();
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			if (consoleRunner != null)
			{
				Win32.FreeConsole();
			}
		}

		private void checkAllToolStripButton_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem item in this.caseList.Items)
			{
				item.Checked = true;
			}
		}

		private void uncheckAllToolStripButton_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem item in this.caseList.Items)
			{
				item.Checked = false;
			}
		}
	}

	internal abstract class ReoGridTestSet : TestSet
	{
		protected static readonly Random rand = new Random();

		private ReoGridControl grid;

		public ReoGridControl Grid
		{
			get { return this.grid; }
			set
			{
				this.grid = value;
				this.worksheet = grid.CurrentWorksheet;
			}
		}

		protected Worksheet worksheet;

		public Worksheet Worksheet
		{
			get { return worksheet; }
			set { worksheet = value; }
		}

		public Worksheet sheet
		{
			get { return worksheet; }
			set { worksheet = value; }
		}

		public void SetUp(int rows ,int cols)
		{
			if (worksheet != null) worksheet.Reset(rows, cols);
		}

		public override void SetUp()
		{
			if (worksheet != null) worksheet.Reset();
		}

		// show control ui and update immdelity
		private bool showControl;

		public bool ShowControl
		{
			get { return showControl; }
			set { showControl = value; }
		}

		#region Proxy

		protected object HBorder(int row, int col)
		{
			return RuntimeClassHelper.InvokeMethod(worksheet, "RetrieveHBorder", row, col);
		}
		protected RangeBorderStyle HBorderStyle(int row, int col)
		{
			object obj = HBorder(row, col);
			return (RangeBorderStyle)RuntimeClassHelper.GetField(worksheet, "Border");
		}
		protected int HBorderCols(int row, int col)
		{
			object obj = HBorder(row, col);
			return obj == null ? 0 : (int)RuntimeClassHelper.GetField(worksheet, "Cols");
		}
		protected int HBorderPos(int row, int col)
		{
			object obj = HBorder(row, col);
			return obj == null ? 0 : (int)RuntimeClassHelper.GetField(worksheet, "Pos");
		}

		protected object VBorder(int row, int col)
		{
			return RuntimeClassHelper.InvokeMethod(worksheet, "RetrieveVBorder", row, col);
		}
		protected RangeBorderStyle VBorderStyle(int row, int col)
		{
			object obj = VBorder(row, col);
			return (RangeBorderStyle)RuntimeClassHelper.GetField(worksheet, "Border");
		}
		protected int HBorderRows(int row, int col)
		{
			object obj = VBorder(row, col);
			return obj == null ? 0 : (int)RuntimeClassHelper.GetField(worksheet, "Rows");
		}
		protected int VBorderPos(int row, int col)
		{
			object obj = VBorder(row, col);
			return obj == null ? 0 : (int)RuntimeClassHelper.GetField(worksheet, "Pos");
		}

		protected bool IsBorderSame(params object[] borders)
		{
			return (bool)RuntimeClassHelper.InvokeMethod(worksheet, "IsBorderSame", borders);
		}
		#endregion
	}

	public sealed class RuntimeClassHelper
	{
		public static object InvokeMethod(object obj, string method, params object[] args)
		{
			MethodInfo mi = obj.GetType().GetMethod(method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			return (mi == null) ? null : mi.Invoke(obj, args);
		}
		public static object GetProperty(object obj, string name)
		{
			PropertyInfo pi = obj.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			return (pi == null) ? null : pi.GetValue(obj, null);
		}
		public static object GetField(object obj, string field)
		{
			FieldInfo fi = obj.GetType().GetField(field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			return fi == null ? null : fi.GetValue(obj);
		}
	}
}
