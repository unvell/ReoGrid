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
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml.Serialization;

using unvell.ReoGrid.Editor;

namespace unvell.ReoGrid.Demo
{
	public partial class DemoItemsForm : Form
	{
		public DemoItemsForm()
		{
			InitializeComponent();

			Screen s = Screen.FromControl(this);
			int scrWidth = s.WorkingArea.Width;
			int scrHeight = s.WorkingArea.Height;
			this.Width = (int)(0.7f * scrWidth);
			this.Height = (int)(0.9f * scrHeight);
			this.Left = (scrWidth - this.Width) / 2;
			this.Top = (scrHeight - this.Height) / 2;
		}

		protected override void OnLoad(EventArgs e)
		{
			this.Text = "ReoGrid デモプログラム " + ProductVersion.ToString();

			string rootCategory = DemoJP.Properties.Resources.DemoCategory;

			using (var sr = new System.IO.StringReader(rootCategory))
			{
				demoCategoryFile = (DemoFile)xmlSerializer.Deserialize(sr) as DemoFile;
			}

			if (demoCategoryFile == null)
			{
				MessageBox.Show("カテゴリファイルの読み込みに失敗しました。もう一度ダウンロードして試してください。");
				Close();
				return;
			}

			tree.BeforeExpand += tree_BeforeExpand;
			tree.AfterSelect += tree_AfterSelect;

			LoadAllItems();

			var dummyGrid = new ReoGridControl();

			this.labTitle.Text = dummyGrid.ProductName + " " + dummyGrid.ProductVersion.ToString();
			web.Visible = false;

			// load default demo item
			if (!string.IsNullOrEmpty(demoCategoryFile.defaultItem))
			{
				var demoItem = FindDemoItemByName(demoCategoryFile, demoCategoryFile.defaultItem);
				if (demoItem != null)
				{
					this.currentItem = demoItem;
					LoadDemo(demoItem);
				}
			}
		}

		private DemoFile demoCategoryFile;
		private XmlSerializer xmlSerializer = new XmlSerializer(typeof(DemoFile));

		public void LoadAllItems()
		{
			LoadCategory(null, demoCategoryFile);
		}

		void tree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			var cat = e.Node.Tag as DemoCategory;
			if (cat != null)
			{
				if (e.Node.Nodes.Count > 0 && e.Node.Nodes[0].Text == "読み込み中...")
				{
					e.Node.Nodes.Clear();
					LoadCategory(e.Node, cat);
				}
			}
		}

		public void LoadCategory(TreeNode node, IDemoCategory cc)
		{
			var parentNode = (node == null ? tree.Nodes : node.Nodes);

			foreach (var cat in cc.Categories)
			{
				var catNode = parentNode.Add(cat.name);
				catNode.Tag = cat;
				catNode.ImageIndex = 0;
				catNode.SelectedImageIndex = 0;

				if (string.Compare(cat.expansion, "true", true) == 0)
				{
					LoadCategory(catNode, cat);
					catNode.Expand();
				}
				else
				{
					catNode.Nodes.Add("読み込み中...");
				}
			}

			foreach (var item in cc.Items)
			{
				item._category = cc;

				var itemNode = parentNode.Add(item.name);
				itemNode.Tag = item;
				itemNode.ImageIndex = 1;
				itemNode.SelectedImageIndex = 1;
			}
		}

		void tree_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node != null)
			{
				var item = e.Node.Tag as DemoItem;
				if (item != null)
				{
					this.currentItem = item;
					LoadDemo(item);
				}
			}
		}

		private DemoItem currentItem;

		public void LoadDemo(DemoItem item)
		{
			labTitle.Text = string.Format("{0} ({1})", item.name, item.itemClass);

			if (item._demoInstance == null)
			{
				Application.DoEvents();

				var typeName = string.Format("{0}.{1}", this.GetType().Namespace, item.itemClass);

				var type = Type.GetType(typeName);

				if (type != null)
				{
					item._demoInstance = System.Activator.CreateInstance(type) as UserControl;
					item._demoInstance.Dock = DockStyle.Fill;
				}
			}

			UserControl demoCtrl = item._demoInstance;
			string htmlHelp = null;

			if (item._demoInstance != null)
			{
				demoPanel.Controls.Clear();
				demoPanel.Controls.Add(item._demoInstance);

				if (demoCtrl is IDemoHelp
					&& !string.IsNullOrEmpty(htmlHelp = ((IDemoHelp)demoCtrl).GetHTMLHelp()))
				{
					web.DocumentText = string.Format(
						unvell.ReoGrid.DemoJP.Properties.Resources.HTMLHelpTemp, item.name, htmlHelp);

					web.Document.OpenNew(true);
					web.Visible = true;
				}
				else if (item.docUrl != null && !string.IsNullOrEmpty(item.docUrl.val))
				{
					web.Navigate(string.Format("{0}/{1}", demoCategoryFile.baseSite, item.docUrl.val));
					web.Visible = true;
				}
				else
				{
					web.Visible = false;
					//web.DocumentText = unvell.ReoGrid.Demo.Properties.Resources.NoHelpAvailable;
					//web.Document.OpenNew(true);
				}
			
			}
		}

		DemoItem FindDemoItemByName(IDemoCategory category, string name)
		{
			if (category.Categories != null)
			{
				foreach (var item in category.Items)
				{
					if (string.Compare(item.name, name, true) == 0)
					{
						return item;
					}
				}

				foreach (var subCat in category.Categories)
				{
					var item = FindDemoItemByName(subCat, name);
					if (item != null) return item;
				}
			}

			return null;
		}

		DemoItem FindDemoItemByType(IDemoCategory category, string typeName)
		{
			if (category.Categories != null)
			{
				foreach (var item in category.Items)
				{
					if (string.Compare(item.name, typeName, true) == 0)
					{
						return item;
					}
				}

				foreach (var subCat in category.Categories)
				{
					var item = FindDemoItemByName(subCat, typeName);
					if (item != null) return item;
				}
			}

			return null;
		}

		private void lnkReset_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (this.currentItem != null)
			{
				this.currentItem._demoInstance = null;
				LoadDemo(this.currentItem);
			}
		}

		private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.Filter = "サポートする全てのファイル(*.xlsx;*.rgf;*.csv;*.txt;*.xml)|(*.xlsx;*.rgf;*.csv;*.txt;*.xml)|Excel ファイル（Office 2007以降）(*.xlsx)|*.xlsx|RGF ファイル(*.rgf)|*.rgf|CSV ファイル(*.csv)|*.csv|テキストファイル(*.txt)|*.txt|全てのファイル(*.*)|*.*";
				if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					this.Open(ofd.FileName);
				}
			}
		}

		/// <summary>
		/// Open specified file by ReoGridEditor.
		/// </summary>
		/// <param name="filename">Path to open file.</param>
		private void Open(string filename)
		{
			try
			{
				Cursor = Cursors.WaitCursor;

				var editor = new ReoGridEditor();

				if (!string.IsNullOrEmpty(filename))
				{
					editor.CurrentFilePath = filename;
				}

				editor.Show();
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void newEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Open(null); // null to create new document
		}

		private void closeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		#region Help
		private void reportBugToolStripMenuItem_Click(object sender, EventArgs e)
		{
			RGUtility.OpenFileOrLink("https://github.com/unvell/ReoGrid/issues");
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new unvell.ReoGrid.WinForm.AboutForm().ShowDialog();
		}

		private void projectHomepageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			RGUtility.OpenFileOrLink("https://reogrid.net/jp/");
		}
		private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			RGUtility.OpenFileOrLink("https://reogrid.net/jp/download");
		}
		#endregion // Help
	}

	public interface IDemoHelp
	{
		string GetHTMLHelp();
	}

	public interface IDemoCategory
	{
		string ClassNamespace { get; set; }
		List<DemoCategory> Categories { get; }
		List<DemoItem> Items { get; }
	}

	[XmlRoot("Demo")]
	public class DemoFile : IDemoCategory
	{
		[XmlAttribute("BaseSite")]
		public string baseSite;

		[XmlAttribute("DefaultItem")]
		public string defaultItem;

		[XmlAttribute("Namespace")]
		public string ClassNamespace { get; set; }

		[XmlArray("New"), XmlArrayItem("Item")]
		public List<DemoItem> newItems;

		[XmlArray("Categories"), XmlArrayItem("Category")]
		public List<DemoCategory> Categories { get; set; }

		[XmlElement("Item")]
		public List<DemoItem> Items { get; set; }
	}

	public class DemoCategory : IDemoCategory
	{
		[XmlAttribute("Name")]
		public string name;

		[XmlAttribute("Expansion")]
		public string expansion;

		[XmlAttribute("Namespace")]
		public string ClassNamespace { get; set; }

		[XmlArray("Categories"), XmlArrayItem("Category")]
		public List<DemoCategory> Categories { get; set; }

		[XmlElement("Item")]
		public List<DemoItem> Items { get; set; }
	}

	public class DemoItem
	{
		[XmlAttribute("Name")]
		public string name;

		[XmlAttribute("Class")]
		public string itemClass;

		[XmlAttribute("Edition")]
		public string edition;

		[XmlElement("Doc-Url")]
		public ElementText docUrl;

		[XmlIgnore]
		public UserControl _demoInstance;

		[XmlIgnore]
		public IDemoCategory _category;
	}

	public class ElementText
	{
		[XmlText]
		public string val;
	}
}
