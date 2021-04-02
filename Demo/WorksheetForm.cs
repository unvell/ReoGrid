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

namespace unvell.ReoGrid.Demo
{
	public partial class WorksheetForm : Form
	{
		public WorksheetForm()
		{
			InitializeComponent();
		}

		public void Open(string file)
		{
			this.Open(file, null);
		}

		public void Open(string file, Action<Worksheet> postHandler)
		{
			grid.Load(file);
			this.PostHandler = postHandler;
		}

		public void Init(Action<Worksheet> postHandler)
		{
			this.PostHandler = postHandler;
		}

		public Action<Worksheet> PostHandler { get; set; }

		protected override void OnCreateControl()
		{
			base.OnCreateControl();

			if (this.PostHandler != null) PostHandler(grid.CurrentWorksheet);
		}
	}
}
