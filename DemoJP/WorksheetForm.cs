/*****************************************************************************
 * 
 * ReoGrid - .NET 表計算スプレッドシートコンポーネント
 * https://reogrid.net/jp
 *
 * ReoGrid 日本語版デモプロジェクトは MIT ライセンスでリリースされています。
 * 
 * このソフトウェアは無保証であり、このソフトウェアの使用により生じた直接・間接の損害に対し、
 * 著作権者は補償を含むあらゆる責任を負いません。 
 * 
 * Copyright (c) 2012-2016 unvell.com, All Rights Reserved.
 * https://www.unvell.com/jp
 * 
 ****************************************************************************/

using System;
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
