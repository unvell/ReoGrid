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

namespace unvell.ReoGrid.Demo.DocumentDemo
{
	public partial class KamokuPanel : UserControl
	{
		public KamokuPanel()
		{
			InitializeComponent();

			listView1.Items.Add(new ListViewItem(new string[] { "101", "現　　　　　金" }));
			listView1.Items.Add(new ListViewItem(new string[] { "102", "当　座　預　金" }));
			listView1.Items.Add(new ListViewItem(new string[] { "103", "受　取　手　形" }));
			listView1.Items.Add(new ListViewItem(new string[] { "201", "売　　掛　　金" }));
			listView1.Items.Add(new ListViewItem(new string[] { "202", "繰　越　商　品" }));
			listView1.Items.Add(new ListViewItem(new string[] { "203", "前　　払　　金" }));
			listView1.Items.Add(new ListViewItem(new string[] { "204", "前　払　家　賃" }));
			listView1.Items.Add(new ListViewItem(new string[] { "205", "未　収　地　代" }));
			listView1.Items.Add(new ListViewItem(new string[] { "301", "備　　　　　品" }));
			listView1.Items.Add(new ListViewItem(new string[] { "302", "土　　　　　地" }));

			listView1.Cursor = Cursors.Hand;
			listView1.MouseMove += ListView1_MouseMove;
			listView1.MultiSelect = false;

			listView1.Click += ListView1_Click;
		}

		private void ListView1_MouseMove(object sender, MouseEventArgs e)
		{
			var item = listView1.GetItemAt(e.X, e.Y);
			if (item != null) item.Selected = true;
		}
		
		private void ListView1_Click(object sender, EventArgs e)
		{
			if (listView1.SelectedItems.Count > 0)
			{
				this.SelectedKamokuCode = listView1.SelectedItems[0].SubItems[0].Text;
			}
			else
			{
				this.SelectedKamokuCode = null;
			}

			this.KamokuCodeSelected?.Invoke(this, null);
		}

		public string SelectedKamokuCode { get; set; }

		public event EventHandler KamokuCodeSelected;
	}
}
