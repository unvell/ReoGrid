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
 * ReoGridEditor is released under MIT license.
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

using unvell.Common;
using unvell.ReoGrid.Events;

namespace unvell.ReoGrid.Editor
{
	public partial class AddressFieldControl : Control
	{
		private TextBox addressBox;
		private DropdownListBoxWindow dropdown;
		private PushdownArrowControl arrowControl;

		public TextBox AddressBox
		{
			get { return addressBox; }
			set { addressBox = value; }
		}

		private ReoGridControl workbook;
		private Worksheet worksheet;

		public ReoGridControl GridControl
		{
			get
			{
				return workbook;
			}
			set
			{
				if (this.workbook != null)
				{
					this.workbook.CurrentWorksheetChanged -= workbook_CurrentWorksheetChanged;

					workbook.Disposed -= grid_Disposed;
				}

				this.worksheet = null;
				this.workbook = value;

				if (workbook != null)
				{
					this.workbook.CurrentWorksheetChanged += workbook_CurrentWorksheetChanged;
			
					workbook.Disposed += grid_Disposed;

					RefreshCurrentAddress();
				}
			}
		}

		void workbook_CurrentWorksheetChanged(object sender, EventArgs e)
		{
			if (this.worksheet != null)
			{
				this.worksheet.SelectionRangeChanging -= grid_SelectionRangeChanging;
				this.worksheet.SelectionRangeChanged -= grid_SelectionRangeChanged;
			}

			this.worksheet = this.workbook.CurrentWorksheet;

			if (this.worksheet != null)
			{
				RefreshCurrentAddress();

				this.worksheet.SelectionRangeChanging += grid_SelectionRangeChanging;
				this.worksheet.SelectionRangeChanged += grid_SelectionRangeChanged;
			}
		}

		public AddressFieldControl()
		{
			InitializeComponent();

			SuspendLayout();

			BackColor = SystemColors.Window;

			addressBox = new TextBox()
			{
				BorderStyle = System.Windows.Forms.BorderStyle.None,
				//Font = new Font(Font.FontFamily, 9),
				TextAlign = HorizontalAlignment.Center,
				Location = new Point(0, 0),
			};

			arrowControl = new PushdownArrowControl() { Dock = DockStyle.Right, Width = 20 };

			Controls.Add(addressBox);
			Controls.Add(arrowControl);

			addressBox.GotFocus += addressBox_GotFocus;
			addressBox.LostFocus += addressBox_LostFocus;
			addressBox.KeyDown += txtAddress_KeyDown;
			
			arrowControl.MouseDown += arrowControl_MouseDown;

			ResumeLayout();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			addressBox.Width = ClientRectangle.Width - arrowControl.Width - ClientRectangle.Left;
			addressBox.Top = ClientRectangle.Top + (ClientRectangle.Height - addressBox.Height) / 2;
		}

		void grid_SelectionRangeChanging(object sender, RangeEventArgs e)
		{
			var range = this.worksheet.SelectionRange;

			if (this.worksheet.IsMergedCell(range))
			{
				addressBox.Text = range.StartPos.ToAddress();
			}
			else
			{
				addressBox.Text = range.ToStringSpans();
			}
		}

		void grid_SelectionRangeChanged(object sender, RangeEventArgs e)
		{
			RefreshCurrentAddress();
		}

		public void RefreshCurrentAddress()
		{
			if (worksheet == null) return;

			var range = this.worksheet.SelectionRange;

			var name = this.worksheet.GetNameByRange(range);

			if (!string.IsNullOrEmpty(name))
			{
				addressBox.Text = name;
			}
			else
			{
				if (!range.IsEmpty)
				{
					if (this.worksheet.IsMergedCell(range))
					{
						addressBox.Text = range.StartPos.ToAddress();
					}
					else
					{
						addressBox.Text = range.ToAddress();
					}
				}
				else
				{
					addressBox.Text = string.Empty;
				}
			}
		}

		private bool focused = false;

		void addressBox_GotFocus(object sender, EventArgs e)
		{
			focused = true;
			addressBox.TextAlign = HorizontalAlignment.Left;
			addressBox.SelectionStart = 0;
			addressBox.SelectionLength = addressBox.Text.Length;
			focused = false;
		}

		void addressBox_LostFocus(object sender, EventArgs e)
		{
			if (dropdown != null)
			{
				// find next focus control 
				// (is there any better method to do this than WindowFromPoint?)
				try
				{
					IntPtr hwnd = unvell.Common.Win32Lib.Win32.WindowFromPoint(Cursor.Position);

					if (hwnd != IntPtr.Zero)
					{
						Control ctrl = Control.FromHandle(hwnd);
						if (ctrl == dropdown.ListBox || ctrl == dropdown)
						{
							return;
						}
					}
				}
				catch { }
			}

			if (!focused)
			{
				EndEditAddress();
			}
		}

		void txtAddress_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				string id = addressBox.Text;

				// avoid to directly use trim, it will create new string even nothing to be trimmed
				if (id.StartsWith(" ") || id.EndsWith(" "))
				{
					id = id.Trim();
				}

				if (RangePosition.IsValidAddress(id))
				{
					this.worksheet.SelectionRange = new RangePosition(id);
					workbook.Focus();
				}
				else if (RGUtility.IsValidName(id))
				{
					var refRange = this.worksheet.GetNamedRange(id);

					if (refRange != null)
					{
						this.worksheet.SelectionRange = refRange;
						workbook.Focus();
					}
					else
					{
						try
						{
							this.worksheet.DefineNamedRange(id, this.worksheet.SelectionRange);
							workbook.Focus();
						}
						catch (NamedRangeAlreadyDefinedException)
						{
							// should be not reached
							MessageBox.Show("Another range with same name does already exist.");
						}
					}
				}
			}
			else if (e.KeyCode == Keys.Down)
			{
				PushDown();
			}
			else if (e.KeyCode == Keys.Escape)
			{
				workbook.Focus();
			}
		}

		void grid_Disposed(object sender, EventArgs e)
		{
			this.GridControl = null;
		}

		void arrowControl_MouseDown(object sender, MouseEventArgs e)
		{
			PushDown();
		}

		public void StartEditAddress()
		{
			if (dropdown == null || !dropdown.Visible)
			{
				PushDown();
			}

			addressBox.Focus();
		}

		public void EndEditAddress()
		{
			if (!focused)
			{
				addressBox.TextAlign = HorizontalAlignment.Center;
				PullUp();
			}
		}

		private void PushDown()
		{
			if (dropdown == null)
			{
				dropdown = new DropdownListBoxWindow(null);
				dropdown.ItemSelected += ListBox_ItemSelected;
			}

			dropdown.ListBox.Items.Clear();
			foreach (var name in this.worksheet.GetAllNamedRanges())
			{
				dropdown.ListBox.Items.Add(name);
			}

			dropdown.Width = this.Width;
			dropdown.Height = 200;
			dropdown.AutoClose = false;
			dropdown.Show(this, 0, Height - 1);
			dropdown.Capture = true;

			StartEditAddress();
		}

		void ListBox_ItemSelected(object sender, EventArgs e)
		{
			GotoNamedRange(Convert.ToString(dropdown.ListBox.SelectedItem));
		}

		public void GotoNamedRange(string name)
		{
			if (workbook != null)
			{
				var refRange = this.worksheet.GetNamedRange(name);

				if (refRange != null)
				{
					this.worksheet.SelectionRange = refRange;
					EndEditAddress();
					workbook.Focus();
				}
			}
		}

		private void PullUp()
		{
			if (dropdown != null)
			{
				dropdown.AutoClose = true;
				dropdown.Visible = false;
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				EndEditAddress();
			}

			base.OnKeyDown(e);
		}
	}

	internal class PushdownArrowControl : Control
	{
		protected override void OnPaint(PaintEventArgs e)
		{
			GraphicsToolkit.FillTriangle(e.Graphics, 7, new Point(ClientRectangle.Right - 10,
				ClientRectangle.Top + ClientRectangle.Height / 2 - 1));
		}
	}

	internal class DropdownListBoxWindow : ToolStripDropDown
	{
		private ListBox listbox;
		private ToolStripControlHost controlHost;

		internal DropdownListBoxWindow(ToolStripItem ownerItem)
			: base()
		{
			listbox = new ListBox()
			{
				Dock = DockStyle.Fill,
				BorderStyle = System.Windows.Forms.BorderStyle.None,
			};
			
			BackColor = listbox.BackColor;
			AutoSize = false;
			TabStop = true;
			Items.Add(controlHost = new ToolStripControlHost(listbox));
			controlHost.Margin = controlHost.Padding = new Padding(0);
			controlHost.AutoSize = false;
			AutoClose = false;

			base.OwnerItem = ownerItem;

			listbox.MouseMove += (sender, e) =>
			{
				int index = listbox.IndexFromPoint(e.Location);
				if (index != -1) listbox.SelectedIndex = index;
			};

			listbox.MouseDown += (s, e) => this.ItemSelected?.Invoke(this, null);
		}

		public event EventHandler ItemSelected;

		internal ListBox ListBox { get { return listbox; } }

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (controlHost != null) controlHost.Size = new Size(ClientRectangle.Width - 2, ClientRectangle.Height - 2);
		}

	}

}
