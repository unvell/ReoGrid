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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace unvell.ReoGrid.Demo.WorkbookDemo
{
	/// <summary>
	/// マルチシート管理のデモ
	/// </summary>
	public partial class MultisheetDemo : UserControl
	{
		private Worksheet worksheet;

		public MultisheetDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;
		}

		private static readonly Random rand = new Random();

		private static readonly WorksheetRangeStyle grayBackgroundStyle = new WorksheetRangeStyle
		{
			Flag = PlainStyleFlag.BackColor,
		};

		private WorksheetRangeStyle GetRandomBackColorStyle()
		{
			grayBackgroundStyle.BackColor = Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255));
			return grayBackgroundStyle;
		}

		private void btnAddWorksheet_Click(object sender, EventArgs e)
		{
			// ワークシートを作成
			var newSheet = this.grid.CreateWorksheet();

			// ワークシートの背景色を設定
			newSheet.SetRangeStyles(RangePosition.EntireRange, GetRandomBackColorStyle());

			// ワークシートをワークブックに追加
			this.grid.AddWorksheet(newSheet);

			// ワークシートを現在表示中のワークシートに設定
			grid.CurrentWorksheet = newSheet;
		}

		private void btnRemoveSheet_Click(object sender, EventArgs e)
		{
			if (grid.Worksheets.Count <= 1)
			{
				// ワークブックに必ず最後のワークシートを残す必要がある
				MessageBox.Show("ワークブックに必ず最後のワークシートを残す必要があります。");
			}
			else
			{
				// 現在表示中のワークシートをワークブックから削除
				grid.RemoveWorksheet(grid.CurrentWorksheet);
			}
		}

		private void btnSheetList_Click(object sender, EventArgs e)
		{
			foreach (ToolStripMenuItem oldMenuItem in this.sheetListContextMenuStrip.Items)
			{
				oldMenuItem.Click -= menuItem_Click;
			}

			this.sheetListContextMenuStrip.Items.Clear();

			foreach (var sheet in this.grid.Worksheets)
			{
				ToolStripMenuItem menuItem = new ToolStripMenuItem(sheet.Name)
				{
					Tag = sheet,
					Checked = (sheet == this.grid.CurrentWorksheet),
				};

				menuItem.Click += menuItem_Click;

				this.sheetListContextMenuStrip.Items.Add(menuItem);
			}

			sheetListContextMenuStrip.Show(this.btnSheetList, new Point(0, this.btnSheetList.Height));
		}

		void menuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
			this.grid.CurrentWorksheet = (Worksheet)menuItem.Tag;
		}

	}

}
