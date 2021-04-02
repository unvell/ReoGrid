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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

#if WINFORM
using System.Windows.Forms;
using RGFloat = System.Single;
using RGImage = System.Drawing.Image;
#else
using RGFloat = System.Double;
using RGImage = System.Windows.Media.ImageSource;
#endif // WINFORM

namespace unvell.ReoGrid.CellTypes
{
#if WINFORM
	/// <summary>
	/// Representation of a typecial dropdown control on spreadsheet
	/// </summary>
	public class DropdownListCell : DropdownCell
	{
		/// <summary>
		/// Construct dropdown control with an empty candidates list
		/// </summary>
		public DropdownListCell()
			: base()
		{
			this.candidates = new List<object>(0);
		}

		/// <summary>
		/// Construct dropdown control with specified candidates array
		/// </summary>
		/// <param name="candidates">candidate object array to be displayed in the listbox</param>
		public DropdownListCell(params object[] candidates)
			: this()
		{
			this.candidates.AddRange(candidates);
		}

		/// <summary>
		/// Construct dropdown control with specified candidates array
		/// </summary>
		/// <param name="candidates">candidate object array to be displayed in the listbox</param>
		public DropdownListCell(IEnumerable<object> candidates)
			: this()
		{
			this.candidates.AddRange(candidates);
		}

		/// <summary>
		/// Get or set the selected item in candidates list
		/// </summary>
		public object SelectedItem
		{
			get
			{
				return this.listBox.SelectedItem;
			}
			set
			{
				this.listBox.SelectedItem = value;
			}
		}

		/// <summary>
		/// Get or set the selected index in candidates list
		/// </summary>
		public int SelectedIndex
		{
			get
			{
				return this.listBox.SelectedIndex;
			}
			set
			{
				this.listBox.SelectedIndex = value;
			}
		}

		/// <summary>
		/// Set selected item
		/// </summary>
		/// <param name="obj">Selected item to be handled</param>
		protected virtual void SetSelectedItem(object obj)
		{
			Cell.Data = obj;

			this.SelectedItemChanged?.Invoke(this, null);
		}

		/// <summary>
		/// Event for selected item changed
		/// </summary>
		public event EventHandler SelectedItemChanged;

		private List<object> candidates;

		/// <summary>
		/// Push down the dropdown panel.
		/// </summary>
		public override void PushDown()
		{
			if (this.listBox == null)
			{
				this.listBox = new ListBox()
				{
					Dock = DockStyle.Fill,
					BorderStyle = System.Windows.Forms.BorderStyle.None,
				};

				listBox.Click += ListBox_Click;
				listBox.KeyDown += ListBox_KeyDown;
				listBox.MouseMove += (sender, e) =>
				{
					int index = listBox.IndexFromPoint(e.Location);
					if (index != -1) listBox.SelectedIndex = index;
				};

				if (this.candidates != null)
				{
					listBox.Items.AddRange(this.candidates.ToArray());
				}

				base.DropdownControl = listBox;
			}

			listBox.SelectedItem = this.Cell.InnerData;

			base.PushDown();
		}

#if WINFORM
		private ListBox listBox;

		void ListBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (base.Cell != null && base.Cell.Worksheet != null)
			{
				if ((e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space))
				{
					SetSelectedItem(this.listBox.SelectedItem);
					PullUp();
				}
				else if (e.KeyCode == Keys.Escape)
				{
					PullUp();
				}
			}
		}

		void ListBox_Click(object sender, EventArgs e)
		{
			if (this.Cell != null && this.Cell.Worksheet != null)
			{
				SetSelectedItem(this.listBox.SelectedItem);
			}

			PullUp();
		}
#elif WPF
		private System.Windows.Controls.ListBox listBox;

#endif // WPF

		#region Items Property
		private DropdownItemsCollection itemsCollection;

		/// <summary>
		/// Collection of condidate items
		/// </summary>
		public DropdownItemsCollection Items
		{
			get
			{
				if (this.itemsCollection == null)
				{
					this.itemsCollection = new DropdownItemsCollection(this);
				}

				return this.itemsCollection;
			}
		}
		#endregion // Items Property

		#region DropdownItemsCollection
		/// <summary>
		/// Represents drop-down items collection.
		/// </summary>
		public class DropdownItemsCollection : ICollection<object>
		{
			private DropdownListCell owner;

			internal DropdownItemsCollection(DropdownListCell owner)
			{
				this.owner = owner;
			}

			/// <summary>
			/// Add candidate item.
			/// </summary>
			/// <param name="item">Item to be added.</param>
			public void Add(object item)
			{
				if (this.owner.listBox != null)
				{
					this.owner.listBox.Items.Add(item);
				}
				else
				{
					this.owner.candidates.Add(item);
				}
			}

			/// <summary>
			/// Add multiple candidate items.
			/// </summary>
			/// <param name="items">Items to be added.</param>
			public void AddRange(params object[] items)
			{
				if (this.owner.listBox != null)
				{
					this.owner.listBox.Items.AddRange(items);
				}
				else
				{
					this.owner.candidates.AddRange(items);
				}
			}

			/// <summary>
			/// Clear all candidate items.
			/// </summary>
			public void Clear()
			{
				if (this.owner.listBox != null)
				{
					this.owner.listBox.Items.Clear();
				}
				else
				{
					this.owner.candidates.Clear();
				}
			}

			/// <summary>
			/// Check whether or not the candidate list contains specified item.
			/// </summary>
			/// <param name="item">item to be checked.</param>
			/// <returns>true if contained, otherwise return false.</returns>
			public bool Contains(object item)
			{
				if (this.owner.listBox != null)
				{
					return this.owner.listBox.Items.Contains(item);
				}
				else
				{
					return this.owner.candidates.Contains(item);
				}
			}

			/// <summary>
			/// Copy the candidate list into specified array.
			/// </summary>
			/// <param name="array">array to be copied into.</param>
			/// <param name="arrayIndex">number of item to start copy.</param>
			public void CopyTo(object[] array, int arrayIndex)
			{
				if (this.owner.listBox != null)
				{
					this.owner.listBox.Items.CopyTo(array, arrayIndex);
				}
				else
				{
					this.owner.candidates.CopyTo(array, arrayIndex);
				}
			}

			/// <summary>
			/// Return the number of items in candidate list.
			/// </summary>
			public int Count
			{
				get
				{
					if (this.owner.listBox != null)
					{
						return this.owner.listBox.Items.Count;
					}
					else
					{
						return this.owner.candidates.Count;
					}
				}
			}

			/// <summary>
			/// Check whether or not the candidate list is read-only.
			/// </summary>
			public bool IsReadOnly
			{
				get
				{
					if (this.owner.listBox != null)
					{
						return this.owner.listBox.Items.IsReadOnly;
					}
					else
					{
						return false;
					}
				}
			}

			/// <summary>
			/// Remove specified item from candidate list.
			/// </summary>
			/// <param name="item">item to be removed.</param>
			/// <returns>true if item has been removed successfully.</returns>
			public bool Remove(object item)
			{
				if (this.owner.listBox != null)
				{
					this.owner.listBox.Items.Remove(item);
					return true;
				}
				else
				{
					return this.owner.candidates.Remove(item);
				}
			}

			/// <summary>
			/// Get enumerator of candidate list.
			/// </summary>
			/// <returns>enumerator of candidate list.</returns>
			public IEnumerator<object> GetEnumerator()
			{
				if (this.owner.listBox != null)
				{
					var items = this.owner.listBox.Items;
					foreach (var item in items)
					{
						yield return item;
					}
				}
				else
				{
					foreach (var item in this.owner.candidates)
						yield return item;
				}
			}

			/// <summary>
			/// Get enumerator of candidate list.
			/// </summary>
			/// <returns>enumerator of candidate list.</returns>
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				if (this.owner.listBox != null)
				{
					var items = this.owner.listBox.Items;

					foreach (var item in items)
					{
						yield return item;
					}
				}
				else
				{
					foreach (var item in this.owner.candidates)
					{
						yield return item;
					}
				}
			}
		}
		#endregion // DropdownItemsCollection

		/// <summary>
		/// Clone a drop-down list from this object.
		/// </summary>
		/// <returns>New instance of dropdown list.</returns>
		public override ICellBody Clone()
		{
			return new DropdownListCell(this.candidates);
		}
	}
#endif // WINFORM
}
