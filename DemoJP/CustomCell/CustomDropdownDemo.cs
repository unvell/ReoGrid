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

using unvell.ReoGrid.CellTypes;

namespace unvell.ReoGrid.Demo.CellTypeDemo
{
	/// <summary>
	/// カスタマイズしたドロップダウンリストのデモ
	/// </summary>
	public partial class CustomDropdownDemo : UserControl
	{
		private Worksheet worksheet;

		public CustomDropdownDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;

			worksheet.ColumnHeaders["C"].Width = 120;

			worksheet["B3"] = "項目 1:";
			worksheet["C3"] = "選択...";
			worksheet["C3"] = new ListViewDropdownCell();
			worksheet.Ranges["C3"].BorderOutside = RangeBorderStyle.GraySolid;

			worksheet["B5"] = "項目 2:";
			worksheet["C5"] = "選択...";
			worksheet["C5"] = new ListViewDropdownCell();
			worksheet.Ranges["C5"].BorderOutside = RangeBorderStyle.GraySolid;
		}

		private void chkGridlines_CheckedChanged(object sender, EventArgs e)
		{
			worksheet.SetSettings(WorksheetSettings.View_ShowGridLine, chkGridlines.Checked);
		}
	}

	/// <summary>
	/// カスタマイズしたドロップダウンリスト
	/// </summary>
	class ListViewDropdownCell : DropdownCell
	{
		private ListView listView;

		public ListViewDropdownCell()
		{
			// ListViewコントロールを作成
			this.listView = new ListView()
			{
				BorderStyle = System.Windows.Forms.BorderStyle.None,
				View = View.Details,
				FullRowSelect = true,
			};

			// ListViewをドロップダウンパネルに格納
			this.DropdownControl = listView;

			// ListViewの列を作成
			this.listView.Columns.Add("列 1", 120);
			this.listView.Columns.Add("列 2", 120);

			// グループとDummyデータを格納
			var group1 = listView.Groups.Add("grp1", "グループ 1");
			listView.Items.Add(new ListViewItem(new string[] { "項目 1.1", "サブ項目 1.1" }) { Group = group1 });
			listView.Items.Add(new ListViewItem(new string[] { "項目 1.2", "サブ項目 1.2" }) { Group = group1 });
			listView.Items.Add(new ListViewItem(new string[] { "項目 1.3", "サブ項目 1.3" }) { Group = group1 });

			// グループとDummyデータを格納
			var group2 = listView.Groups.Add("grp2", "グループ 2");
			listView.Items.Add(new ListViewItem(new string[] { "項目 2.1", "サブ項目 2.1" }) { Group = group2 });
			listView.Items.Add(new ListViewItem(new string[] { "項目 2.2", "サブ項目 2.2" }) { Group = group2 });
			listView.Items.Add(new ListViewItem(new string[] { "項目 2.3", "サブ項目 2.3" }) { Group = group2 });

			// ドロップダウンパネルの幅を調整
			this.MinimumDropdownWidth = 300;

			// 選択イベントを処理
			this.listView.Click += listView_Click;

		}

		void listView_Click(object sender, EventArgs e)
		{
			if (this.listView.SelectedItems.Count > 0)
			{
				// セルのデータを更新
				this.Cell.Data = this.listView.SelectedItems[0].Text;
				
				// ドロップダウンパネルを閉じる
				PullUp();
			}
		}
	}
}
