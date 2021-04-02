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

#if OUTLINE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using unvell.ReoGrid.Core;
using unvell.ReoGrid.Views;
using unvell.ReoGrid.Events;
using unvell.ReoGrid.Graphics;

namespace unvell.ReoGrid
{
	using unvell.ReoGrid.Outline;

	partial class Worksheet
	{
		private RowOutlineCollection rowOutlineCollection;

		/// <summary>
		/// Get row outline collection.
		/// </summary>
		public RowOutlineCollection RowOutlines
		{
			get
			{
				if (this.rowOutlineCollection == null)
				{
					this.rowOutlineCollection = new RowOutlineCollection(this);
				}

				return this.rowOutlineCollection;
			}
		}

		private ColumnOutlineCollection columnOutlineCollection;
	
		/// <summary>
		/// Get column outline collection.
		/// </summary>
		public ColumnOutlineCollection ColumnOutlines
		{
			get
			{
				if (this.columnOutlineCollection == null)
				{
					this.columnOutlineCollection = new ColumnOutlineCollection(this);
				}

				return this.columnOutlineCollection;
			}
		}

		#region Group & Outline
		internal Dictionary<RowOrColumn, OutlineCollection<ReoGridOutline>> outlines;

		/// <summary>
		/// Retrieve the attached outlines from spreadsheet.
		/// </summary>
		/// <param name="flag">Row or column to be retrieved.</param>
		/// <returns>Retrieved collection of outline.</returns>
		public OutlineCollection<ReoGridOutline> GetOutlines(RowOrColumn flag)
		{
			return outlines == null ? null : outlines[flag];
		}

		/// <summary>
		/// Iterate over all attached outlines.
		/// </summary>
		/// <param name="flag">Spcifiy that row or column to be iterated.</param>
		/// <param name="iterator">Iterator to handle all outlines.</param>
		public void IterateOutlines(RowOrColumn flag, Func<OutlineGroup<ReoGridOutline>, ReoGridOutline, bool> iterator)
		{
			if (iterator == null)
			{
				throw new ArgumentNullException("iterator", "iterator cannot be null");
			}

			OutlineCollection<ReoGridOutline> outlines = null;

			if (this.outlines != null
				&& this.outlines.TryGetValue(flag, out outlines))
			{
				foreach (var group in outlines)
				{
					foreach (var outline in group)
					{
						if (!iterator(group, outline)) return;
					}
				}
			}
		}

		private static void InsertOutline(OutlineCollection<ReoGridOutline> outlineGroups, int groupIndex, ReoGridOutline outline)
		{
			int k = groupIndex;

			if (k < 0)
			{
				outlineGroups.Insert(0, new OutlineGroup<ReoGridOutline>() { outline });
				return;
			}

			var group = outlineGroups[k];

			for (int i = 0; i < group.Count; i++)
			{
				var o = group[i];

				if (o.Contains(outline))
				{
					group.RemoveAt(i);

					InsertOutline(outlineGroups, k - 1, o);
					break;
				}
			}

			group.Add(outline);
		}

		/// <summary>
		/// Group columns from specified number of columns.
		/// </summary>
		/// <param name="col">number of columns to start group.</param>
		/// <param name="count">numbers of column to be grouped.</param>
		/// <returns>an outline instance including the information of grouped columns.</returns>
		/// <exception cref="OutlineOutOfRangeException">if specified number of rows out of maximum row in this worksheet.</exception>
		/// <exception cref="OutlineTooMuchException">If there is more than 9 outlines existed this exception will be thrown.</exception>
		/// <exception cref="OutlineAlreadyDefinedException">If there is a same outline with specified number and count has been already added.</exception>
		/// <exception cref="OutlineIntersectedException">If specified outline intersects with another one which has been already added.</exception>
		public ReoGridOutline GroupColumns(int col, int count)
		{
			return AddOutline(RowOrColumn.Column, col, count);
		}

		/// <summary>
		/// Group rows from specified number of rows.
		/// </summary>
		/// <param name="row">number of rows to start group.</param>
		/// <param name="count">numbers of row to be grouped.</param>
		/// <returns>an outline instance including the information of grouped rows.</returns>
		/// <exception cref="OutlineOutOfRangeException">if specified number of rows out of maximum row in this worksheet</exception>
		/// <exception cref="OutlineTooMuchException">If there is more than 9 outlines existed this exception will be thrown.</exception>
		/// <exception cref="OutlineAlreadyDefinedException">If there is a same outline with specified number and count has been already added.</exception>
		/// <exception cref="OutlineIntersectedException">If specified outline intersects with another one which has been already added.</exception>
		public ReoGridOutline GroupRows(int row, int count)
		{
			return AddOutline(RowOrColumn.Row, row, count);
		}

		/// <summary>
		/// Add outline (Group rows) from specified start position.
		/// </summary>
		/// <param name="flag">what direction used to add outline. (specifying Row or Column)</param>
		/// <param name="start">Start position of outline.</param>
		/// <param name="count">Total count of outline.</param>
		/// <returns>An outline instance including the information of grouped rows or columns.</returns>
		/// <exception cref="OutlineOutOfRangeException">If specified number of rows out of maximum row on this worksheet.</exception>
		/// <exception cref="OutlineTooMuchException">If there is more than 9 outlines existed this exception will be thrown.</exception>
		/// <exception cref="OutlineAlreadyDefinedException">If there is a same outline with specified number and count has been already added.</exception>
		/// <exception cref="OutlineIntersectedException">If specified outline intersects with another one which has been already added.</exception>
		public ReoGridOutline AddOutline(RowOrColumn flag, int start, int count)
		{
			if (flag == RowOrColumn.Both)
			{
				AddOutline(RowOrColumn.Column, start, count);
				return AddOutline(RowOrColumn.Row, start, count);
			}

			int limit = ((flag == RowOrColumn.Row) ? this.rows.Count : this.cols.Count) - 1;

			if (start < 0 || start >= limit)
			{
				throw new OutlineOutOfRangeException(start, count, "row");
			}

			if (count < 0 || (start + count > limit))
			{
				throw new OutlineOutOfRangeException(start, count, "count");
			}

			if (count == 0) return null;

			if (this.outlines == null)
			{
				this.outlines = new Dictionary<RowOrColumn, OutlineCollection<ReoGridOutline>>() 
				{
					{ RowOrColumn.Row, new OutlineCollection<ReoGridOutline>() },
					{ RowOrColumn.Column, new OutlineCollection<ReoGridOutline>() },
				};
			}

			var outlineGroups = this.outlines[flag];

			if (outlineGroups.Count >= 9)
			{
				throw new OutlineTooMuchException();
			}

			int endrow = start + count;

			int targetGroupIndex = outlineGroups.Count - 2; // last group in master

			// create new outline
			ReoGridOutline newOutline = null;
			bool found = false;

			if (flag == RowOrColumn.Row)
			{
				newOutline = new RowOutline(this, start, count);
			}
			else
			{
				newOutline = new ColumnOutline(this, start, count);
			}

			for (int k = 0; k < outlineGroups.Count - 1; k++)
			{
				var group = outlineGroups[k];

				foreach (var o in group)
				{
					if (start == o.Start && endrow == o.End)
					{
						throw new OutlineAlreadyDefinedException();
					}
					else if (newOutline.IntersectWith(o))
					{
						throw new OutlineIntersectedException
						{
							Start = o.Start,
							Count = o.Count,
						};
					}
					// find surrounded outline 
					else if (newOutline.Contains(o))
					{
						targetGroupIndex = k - 1;
						found = true;
					}
				}

				if (found) break;
			}

			// insert outline
			InsertOutline(outlineGroups, targetGroupIndex, newOutline);

			// update viewport controller
			if (this.viewportController != null)
			{
				if (flag == RowOrColumn.Row)
				{
					if (this.settings.Has(WorksheetSettings.View_AllowShowRowOutlines))
					{
						this.viewportController.SetViewVisible(ViewTypes.RowOutline, true);
					}
				}
				else
				{
					if (this.settings.Has(WorksheetSettings.View_AllowShowColumnOutlines))
					{
						this.viewportController.SetViewVisible(ViewTypes.ColOutline, true);
					}
				}

				this.viewportController.UpdateController();
				this.RequestInvalidate();
			}

			if (this.OutlineAdded != null)
			{
				this.OutlineAdded(this, new OutlineAddedEventArgs(newOutline));
			}

			return newOutline;
		}

		/// <summary>
		/// Get an instance of outline by specified position.
		/// </summary>
		/// <param name="flag">Determine that outline in row or column direction to be get.</param>
		/// <param name="start">Zero-based start position of the outline.</param>
		/// <param name="count">Number of rows or columns contained in the outline.</param>
		/// <returns>Instance of outline if found with specified position.</returns>
		public IReoGridOutline GetOutline(RowOrColumn flag, int start, int count)
		{
			ReoGridOutline outline = null;

			var outlines = this.outlines[flag];

			foreach (var g in outlines)
			{
				outline = g.FirstOrDefault(o => o.Start == start && o.Count == count);
				if (outline != null) break;
			}

			//if (outline == null)
			//{
			//	throw new OutlineNotFoundException(start,
			//		string.Format("Outline not found at specified position {0}-{1}", start, start + count));
			//}

			return outline;
		}

		/// <summary>
		/// Collapse specified outline.
		/// </summary>
		/// <param name="flag">Determine that outline in row or column direction to be get.</param>
		/// <param name="start">Zero-based start position of the outline.</param>
		/// <param name="count">Number of rows or columns contained in the outline.</param>
		/// <returns>Instance if specified outline was found and collapsed.</returns>
		public IReoGridOutline CollapseOutline(RowOrColumn flag, int start, int count)
		{
			var outline = GetOutline(flag, start, count);
			outline.Collapse();
			return outline;
		}

		/// <summary>
		/// Expand specified outline.
		/// </summary>
		/// <param name="flag">Determine that outline in row or column direction to be get.</param>
		/// <param name="start">Zero-based start position of the outline.</param>
		/// <param name="count">Number of rows or columns contained in the outline.</param>
		/// <returns>Instance if specified outline was found and expanded.</returns>
		public IReoGridOutline ExpandOutline(RowOrColumn flag, int start, int count)
		{
			var outline = GetOutline(flag, start, count);
			outline.Expand();
			return outline;
		}

		private static void RearrangementOutline(OutlineCollection<ReoGridOutline> outlineGroups, int startGroup)
		{
			if (startGroup >= outlineGroups.Count - 1 || startGroup < 1) return;

			// search for contained childrens, bring it into this level
			int k = startGroup;

			var rightGroup = outlineGroups[k];

		reloop:
			for (int i = 0; i < rightGroup.Count; i++)
			{
				var or = rightGroup[i];

				var leftGroup = outlineGroups[k - 1];

				// check for intersected
				var leftOverlap = leftGroup.FirstOrDefault(_o => _o.Start <= or.Start && _o.Count >= or.Start
					|| _o.Start <= or.End && _o.End >= or.End);

				// no intersects
				if (leftOverlap == null)
				{
					rightGroup.RemoveAt(i);
					leftGroup.Add(or);

					// all children outlines has been removed
					if (rightGroup.Count == 0)
					{
						// remove this group
						outlineGroups.RemoveAt(k);

						RearrangementOutline(outlineGroups, k);
					}
					else
					{
						RearrangementOutline(outlineGroups, k + 1);
					}

					goto reloop;
				}
			}
		}

		/// <summary>
		/// Remove outline from specified position by number of rows or columns.
		/// </summary>
		/// <param name="flag">Which row and column to be removed.</param>
		/// <param name="start">Zero-based start position of the outline.</param>
		/// <param name="count">Number of rows or columns contained in the outline.</param>
		/// <returns>Instance of outline has been removed; Return null if no outlines found 
		/// from specified position and count information.</returns>
		public IReoGridOutline RemoveOutline(RowOrColumn flag, int start, int count)
		{
			var outline = GetOutline(flag, start, count);
			
			if (outline != null)
			{
				RemoveOutline(outline);
			}

			return outline;
		}

		/// <summary>
		/// Remove specfieid outline from collection of outlines of control.
		/// </summary>
		/// <param name="outline">The instance of outline will be removed.</param>
		/// <returns>True if outline has been found by specified position and count,
		/// and has been removed successfully; otherwise return false.</returns>
		public bool RemoveOutline(IReoGridOutline outline)
		{
			RowOrColumn flag = outline is RowOutline ? RowOrColumn.Row : RowOrColumn.Column;

			var outlineGroups = this.GetOutlines(flag);
			if (outlineGroups == null) return false;

			bool found = false;

			for (int i = 0; i < outlineGroups.Count - 1; i++)
			{
				var group = outlineGroups[i];

				for (int j = 0; j < group.Count; j++)
				{
					var o = group[j];

					if (o == outline)
					//if (o.Start == start && o.Count == count)
					{
						// remove this outline
						group.Remove(o);

						// rearrangement behind the outlines
						RearrangementOutline(outlineGroups, i + 1);

						if (group.Count == 0)
						{
							outlineGroups.RemoveAt(i);
						}

						found = true;
						break;
					}
				}

				if (found) break;
			}

			if (found)
			{
				if (this.viewportController != null)
				{
					if (outlineGroups.Count <= 1)
					{
						this.viewportController.SetViewVisible(flag == RowOrColumn.Row ?
							ViewTypes.RowOutline : ViewTypes.ColOutline, false);
					}

					UpdateViewportController();
				}

				this.RequestInvalidate();

				if (this.OutlineRemoved != null)
				{
					this.OutlineRemoved(this, new OutlineRemovedEventArgs(outline));
				}
			}

			return found;
		}

		/// <summary>
		/// Ungroup specified rows. (Remove outline from specified rows)
		/// </summary>
		/// <param name="row">number of row to ungroup.</param>
		/// <param name="count">number of rows to ungroup.</param>
		public void UngroupRows(int row, int count)
		{
			this.RemoveOutline(RowOrColumn.Row, row, count);
		}

		/// <summary>
		/// Ungroup specified columns. (Remove outline from specified columns)
		/// </summary>
		/// <param name="col">number of column to ungroup.</param>
		/// <param name="count">number of columns to ungroup.</param>
		public void UngroupColumns(int col, int count)
		{
			this.RemoveOutline(RowOrColumn.Column, col, count);
		}

		/// <summary>
		/// Ungroup all rows. (Remove all row outlines from spreadsheet)
		/// </summary>
		public void UngroupAllRows()
		{
			if (this.outlines == null) return;

			var rowOutlines = this.outlines[RowOrColumn.Row];

			if (rowOutlines != null)
			{
				// clear all column outlines
				rowOutlines.Reset();

				if (this.viewportController != null)
				{
					// hide column outline panel
					viewportController.SetViewVisible(ViewTypes.RowOutline, false);

					// update viewport controller
					UpdateViewportController();

					// repaint
					this.RequestInvalidate();
				}
			}
		}

		/// <summary>
		/// Ungroup all columns. (Remove all column outlines from spreadsheet)
		/// </summary>
		public void UngroupAllColumns()
		{
			if (this.outlines == null) return;

			var columnOutlines = this.outlines[RowOrColumn.Column];

			if (columnOutlines != null)
			{
				// clear all column outlines
				columnOutlines.Reset();

				if (this.viewportController != null)
				{
					// hide column outline panel
					viewportController.SetViewVisible(ViewTypes.ColOutline, false);

					// update viewport controller
					UpdateViewportController();

					// repaint
					this.RequestInvalidate();
				}
			}
		}

		/// <summary>
		/// Clear all outlines and close the outline panel.
		/// </summary>
		/// <param name="flag">Specifies to process row or column outline of row and column to be clear.</param>
		internal void ClearOutlines(RowOrColumn flag)
		{
			if (this.outlines == null) return;

			ViewTypes hideViews = ViewTypes.None;

			if ((flag & RowOrColumn.Row) == RowOrColumn.Row)
			{
				this.outlines[RowOrColumn.Row].Reset();
				hideViews |= ViewTypes.RowOutline;
			}

			if ((flag & RowOrColumn.Column) == RowOrColumn.Column)
			{
				this.outlines[RowOrColumn.Column].Reset();
				hideViews |= ViewTypes.ColOutline;
			}

			if (this.viewportController != null)
			{
				viewportController.SetViewVisible(hideViews, false);

				viewportController.UpdateController();
			}
		}

		#endregion 

		#region Events

		/// <summary>
		/// Event raised when an outline has been added onto spreadsheet
		/// </summary>
		public event EventHandler<OutlineAddedEventArgs> OutlineAdded;

		/// <summary>
		/// Event raised when an outline has been removed from spreadsheet
		/// </summary>
		public event EventHandler<OutlineRemovedEventArgs> OutlineRemoved;

		/// <summary>
		/// Event is raised before an outline collapse
		/// </summary>
		public event EventHandler<BeforeOutlineCollapseEventArgs> BeforeOutlineCollapse;

		/// <summary>
		/// Event is raised after an outline collapse
		/// </summary>
		public event EventHandler<AfterOutlineCollapseEventArgs> AfterOutlineCollapse;

		/// <summary>
		/// Event is raised before an outline expanding
		/// </summary>
		public event EventHandler<BeforeOutlineExpandingEventArgs> BeforeOutlineExpand;

		/// <summary>
		/// Event is raised after an outline expanding
		/// </summary>
		public event EventHandler<AfterOutlineExpandingEventArgs> AfterOutlineExpand;

		internal void RaiseBeforeOutlineCollapseEvent(BeforeOutlineCollapseEventArgs arg)
		{
			if (this.BeforeOutlineCollapse != null)
			{
				this.BeforeOutlineCollapse(this, arg);
			}
		}

		internal void RaiseAfterOutlineCollapseEvent(AfterOutlineCollapseEventArgs arg)
		{
			if (this.AfterOutlineCollapse != null)
			{
				this.AfterOutlineCollapse(this, arg);
			}
		}

		internal void RaiseBeforeOutlineExpandEvent(BeforeOutlineExpandingEventArgs arg)
		{
			if (this.BeforeOutlineExpand != null)
			{
				this.BeforeOutlineExpand(this, arg);
			}
		}

		internal void RaiseAfterOutlineExpandEvent(AfterOutlineExpandingEventArgs arg)
		{
			if (this.AfterOutlineExpand != null)
			{
				this.AfterOutlineExpand(this, arg);
			}
		}
		
		#endregion // Events
	}
}

namespace unvell.ReoGrid.Outline
{
	/// <summary>
	/// Outline Collection for both Row and Column Outline.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class OutlineCollection<T> : List<OutlineGroup<T>> where T : IReoGridOutline
	{
		/// <summary>
		/// Only allowed to create instance by ReoGridControl.
		/// </summary>
		internal OutlineCollection()
		{
			Reset();
		}

		/// <summary>
		/// Clear all outlines, reset to default status.
		/// </summary>
		public void Reset()
		{
			Clear();
			Add(new OutlineGroup<T>());
		}

		/// <summary>
		/// Iterate over the all outlines
		/// </summary>
		/// <param name="iterator">iterator callback function</param>
		public void IterateOutlines(Func<T, bool> iterator)
		{
			for (int i = 0; i < this.Count - 1; i++)
			{
				var group = this[i];

				foreach (var outline in group)
				{
					if (!iterator(outline)) return;
				}
			}
		}

		/// <summary>
		/// Reverse iterate over the all outlines
		/// </summary>
		/// <param name="iterator"></param>
		public void IterateReverseOutlines(Func<T, bool> iterator)
		{
			for (int i = this.Count - 1; i >= 0; i--)
			{
				var group = this[i];

				foreach (var outline in group)
				{
					if (!iterator(outline)) return;
				}
			}
		}

		/// <summary>
		/// Check whether there is same outline exist
		/// </summary>
		/// <param name="target">Outline used to find</param>
		/// <param name="exclusions">Outline in this list will not be compare</param>
		/// <returns>true if there is another same as target</returns>
		public bool HasSame(IReoGridOutline target, IList<IReoGridOutline> exclusions)
		{
			for (int i = 0; i < this.Count - 1; i++)
			{
				var group = this[i];

				foreach (IReoGridOutline outline in group)
				{
					if (outline != target
						&& (exclusions == null || !exclusions.Contains(outline))
						&& outline.Start == target.Start
						&& outline.Count == target.Count)
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Determine whether any outlines existed in this collection.
		/// </summary>
		public bool HasOutlines
		{
			get
			{
				for (int i = this.Count - 1; i >= 0; i--)
				{
					var group = this[i];

					if (group.Count > 0) return true;
				}

				return false;
			}
		}

		/// <summary>
		/// Get number of outlines.
		/// </summary>
		public int OutlineCount
		{
			get
			{
				int count = 0;

				for (int i = this.Count - 1; i >= 0; i--)
				{
					var group = this[i];
					count += group.Count;
				}

				return count;
			}
		}
	}

	/// <summary>
	/// Represents the interface of row or column outline.
	/// </summary>
	public interface IReoGridOutline
	{
		/// <summary>
		/// Start position of outline.
		/// </summary>
		int Start { get; set; }

		/// <summary>
		/// Number of rows or columns in this outline.
		/// </summary>
		int Count { get; set; }

		/// <summary>
		/// End position of outline
		/// </summary>
		int End { get; }

		/// <summary>
		/// Collapse this outline. (Hide all the cells which are contained by this outline)
		/// </summary>
		void Collapse();

		/// <summary>
		/// Expand this outline. (Show all the cells which are contained by this outline)
		/// </summary>
		void Expand();

		/// <summary>
		/// Determine whether current outline is collapsed.
		/// </summary>
		bool Collapsed { get; set; }

		/// <summary>
		/// Event is raised before this outline collapse.
		/// </summary>
		event EventHandler<BeforeOutlineCollapseEventArgs> BeforeCollapse;

		/// <summary>
		/// Event is raised after this outline collapse.
		/// </summary>
		event EventHandler<AfterOutlineCollapseEventArgs> AfterCollapse;

		/// <summary>
		/// Event is raised before this outline expanding.
		/// </summary>
		event EventHandler<BeforeOutlineExpandingEventArgs> BeforeExpand;

		/// <summary>
		/// Event is raised after this outline expanding.
		/// </summary>
		event EventHandler<AfterOutlineExpandingEventArgs> AfterExpand;
	}

	/// <summary>
	/// Outline group for both Row and Column Outline.
	/// </summary>
	/// <typeparam name="T">Outline define type, must be IReoGridOutline</typeparam>
	public class OutlineGroup<T> : List<T> where T : IReoGridOutline
	{
		/// <summary>
		/// Only allowed to create instance by ReoGridControl.
		/// </summary>
		internal OutlineGroup() { }

		/// <summary>
		/// Number Button Rectangle.
		/// </summary>
		internal Rectangle NumberButtonBounds { get; set; }

		/// <summary>
		/// Collapse all outlines inside this group.
		/// </summary>
		public void CollapseAll()
		{
			foreach (var outline in this)
			{
				outline.Collapse();
			}
		}

		/// <summary>
		/// Expand all outlines inside this group.
		/// </summary>
		public void ExpandAll()
		{
			foreach (var outline in this)
			{
				outline.Expand();
			}
		}
	}

	/// <summary>
	/// Outline instance for both Row and Column Outline.
	/// </summary>
	public abstract class ReoGridOutline : IReoGridOutline
	{
		/// <summary>
		/// instance of ReoGridControl.
		/// </summary>
		public Worksheet sheet { get; set; }

		internal ReoGridOutline(Worksheet sheet, int start, int count)
		{
			this.sheet = sheet;
			this.Start = start;
			this.Count = count;
		}

		/// <summary>
		/// Outline start index. (either number of row or number of column)
		/// </summary>
		public int Start { get; set; }

		/// <summary>
		/// Outline number of count. (either number of rows or number of columns)
		/// </summary>
		public int Count { get; set; }

		/// <summary>
		/// Outline end index. (either number of row or number of column)
		/// </summary>
		public int End { get { return Start + Count; } }

		/// <summary>
		/// Internal flag to determine whether this outline is collapsed.
		/// </summary>
		internal bool InternalCollapsed { get; set; }

		/// <summary>
		/// Check whether current outline is collapsed.
		/// </summary>
		public bool Collapsed
		{
			get { return this.InternalCollapsed; }
			set { if (!this.InternalCollapsed) { this.Collapse(); } }
		}

		internal Rectangle ToggleButtonBounds { get; set; }

		/// <summary>
		/// Determine whether specified outline is contained entirely by this outline.
		/// </summary>
		/// <param name="outline">The outline to be tested.</param>
		/// <returns>True if another outline is contained by this outline; otherwise return false.</returns>
		public bool Contains(ReoGridOutline outline)
		{
			return this.Start <= outline.Start && this.End >= outline.End;
		}

		/// <summary>
		/// Determine whether specified index is contained by this outline.
		/// </summary>
		/// <param name="index">index to be tested.</param>
		/// <returns></returns>
		public bool Contains(int index)
		{
			return index >= Start && index < End;
		}

		/// <summary>
		/// Determine whether specified outline is intersected with this outline.
		/// </summary>
		/// <param name="outline">Outline to be tested.</param>
		/// <returns>True if the specified outline is intersected with this outline.</returns>
		public bool IntersectWith(ReoGridOutline outline)
		{
			return IntersectWith(outline.Start, outline.Count);
		}

		/// <summary>
		/// Determine whether specified range is intersected with this outline.
		/// </summary>
		/// <param name="start">Start index. (either number of row or number of column)</param>
		/// <param name="count">Number of count. (either number of rows or number of columns)</param>
		/// <returns>True if the specified outline is intersected with this outline.</returns>
		public bool IntersectWith(int start, int count)
		{
			int targetEnd = start + count;

			return (this.Start < start && this.End >= start && this.End < targetEnd)
				|| (this.Start > start && this.Start <= targetEnd && this.End > targetEnd);
		}

		/// <summary>
		/// Collapse specified outline.
		/// </summary>
		public abstract void Collapse();

		/// <summary>
		/// Expand specified outline.
		/// </summary>
		public abstract void Expand();

		protected void CollapseInnerOutlines(OutlineCollection<ReoGridOutline> outlines, int groupIndex)
		{
			// find right-side inner outlines and collapse them
			for (int i = groupIndex + 1; i < outlines.Count - 1; i++)
			{
				var g = outlines[i];

				// find collapsed inner outlines
				foreach (var outline in g.Where(o => !o.InternalCollapsed && o.Start > this.Start && o.End == this.End))
				{
					outline.InternalCollapsed = true;
					outline.RaiseAfterCollapseEvent();
				}
			}
		}

		protected void ExpandOuterOutlines(OutlineCollection<ReoGridOutline> outlines, int groupIndex)
		{
			ReoGridOutline outerOutline = null;

			// find right-side inner outlines and collapse them
			for (int i = groupIndex - 1; i >= 0; i--)
			{
				var g = outlines[i];

				// find collapsed inner outlines
				outerOutline = g.FirstOrDefault(o => o.InternalCollapsed && o.Start < this.Start && o.End == this.End);

				if (outerOutline != null)
				{
					break;
				}
			}

			if (outerOutline != null)
			{
				outerOutline.Expand();
			}
		}

		internal abstract bool RaiseBeforeCollapseEvent();
		internal abstract void RaiseAfterCollapseEvent();

		internal abstract bool RaiseBeforeExpandingEvent();
		internal abstract void RaiseAfterExpandingEvent();

		/// <summary>
		/// Event is raised before this outline collapse.
		/// </summary>
		public abstract event EventHandler<BeforeOutlineCollapseEventArgs> BeforeCollapse;

		/// <summary>
		/// Event is raised after this outline collapse.
		/// </summary>
		public abstract event EventHandler<AfterOutlineCollapseEventArgs> AfterCollapse;

		/// <summary>
		/// Event is raised before this outline expanding.
		/// </summary>
		public abstract event EventHandler<BeforeOutlineExpandingEventArgs> BeforeExpand;

		/// <summary>
		/// Event is raised after this outline expanding.
		/// </summary>
		public abstract event EventHandler<AfterOutlineExpandingEventArgs> AfterExpand;
	}

	/// <summary>
	/// Outline collection property
	/// </summary>
	/// <typeparam name="T">Row or column outline instance type</typeparam>
	public class OutlineCollectionProperty<T> : IEnumerable<T> where T : ReoGridOutline
	{
		private Worksheet worksheet;

		private RowOrColumn rowOrColumn;

		internal OutlineCollectionProperty(Worksheet worksheet, RowOrColumn rowOrColumn)
		{
			this.worksheet = worksheet;
			this.rowOrColumn = rowOrColumn;
		}

		/// <summary>
		/// Add outline on worksheet.
		/// </summary>
		/// <param name="start">Zero-based start position to add outline.</param>
		/// <param name="count">Number of rows or columns to be added into outline.</param>
		/// <returns>Instance of new outline.</returns>
		public T AddOutline(int start, int count)
		{
			return (T)this.worksheet.AddOutline(this.rowOrColumn, start, count);
		}

		/// <summary>
		/// Remove outline from worksheet.
		/// </summary>
		/// <param name="start">Zero-based start position to add outline.</param>
		/// <param name="count">Number of rows or columns exist in the outline.</param>
		/// <returns>Instance of removed outline.</returns>
		public T RemoveOutline(int start, int count)
		{
			return (T)this.worksheet.RemoveOutline(this.rowOrColumn, start, count);
		}

		/// <summary>
		/// Get outline by specified position. (start position and number of rows or columns)
		/// </summary>
		/// <param name="start">Zero-based start position to find outline.</param>
		/// <param name="count">Number of rows or columns that is contained in target outline.</param>
		/// <returns>Instance of outline</returns>
		public T this[int start, int count]
		{
			get
			{
				return (T)this.worksheet.GetOutline(this.rowOrColumn, start, count);
			}
		}

		private IEnumerator<T> GetEnum()
		{
			foreach (var og in this.worksheet.GetOutlines(this.rowOrColumn))
			{
				foreach (var o in og)
				{
					yield return (T)o;
				}
			}
		}

		/// <summary>
		/// Get enumerator of outline collection.
		/// </summary>
		/// <returns>Enumerator of outline collection.</returns>
		public IEnumerator<T> GetEnumerator()
		{
			return GetEnum();
		}

		/// <summary>
		/// Get enumerator of outline collection.
		/// </summary>
		/// <returns>Enumerator of outline collection.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnum();
		}
	}

	/// <summary>
	/// Row outline collection.
	/// </summary>
	public class RowOutlineCollection : OutlineCollectionProperty<RowOutline>
	{
		internal RowOutlineCollection(Worksheet worksheet) : base(worksheet, RowOrColumn.Row) { }
	}

	/// <summary>
	/// Column outline collection.
	/// </summary>
	public class ColumnOutlineCollection : OutlineCollectionProperty<ColumnOutline>
	{
		internal ColumnOutlineCollection(Worksheet worksheet) : base(worksheet, RowOrColumn.Column) { }
	}
	
	/// <summary>
	/// Represents instance of row outline.
	/// </summary>
	public class RowOutline : ReoGridOutline, IRowRange
	{
		internal RowOutline(Worksheet sheet, int start, int count)
			: base(sheet, start, count) { }

		/// <summary>
		/// Collapse outline
		/// </summary>
		public override void Collapse()
		{
			var outlines = sheet.GetOutlines(RowOrColumn.Row);
			if (outlines == null) return;

			if (!RaiseBeforeCollapseEvent()) return;

			// find self group index
			int groupIndex = 0;
			for (; groupIndex < outlines.Count - 1; groupIndex++)
			{
				var o = outlines[groupIndex];
				if (o.Contains(this))
				{
					break;
				}
			}

			// find whether or not any rows contained by other inner outlines
			sheet.SetRowsHeight(this.Start, this.Count, r =>
			{
				for (int i = groupIndex + 1; i < outlines.Count - 1; i++)
				{
					var g = outlines[i];

					// find collapsed inner outlines
					var outline = g.FirstOrDefault(o => o.InternalCollapsed && o != this && o.Start <= r && o.End > r);

					// if there is any inner outlines attached on this row, skip adjust this row's height
					if (outline != null)
					{
						return -1;
					}
				}

				return 0;
			}, false);

			base.CollapseInnerOutlines(outlines, groupIndex);

			this.InternalCollapsed = true;
			RaiseAfterCollapseEvent();
		}

		internal override bool RaiseBeforeCollapseEvent()
		{
			BeforeOutlineCollapseEventArgs arg = new BeforeOutlineCollapseEventArgs(this);

			if (this.BeforeCollapse != null)
			{
				this.BeforeCollapse(this, arg);
			}

			sheet.RaiseBeforeOutlineCollapseEvent(arg);

			return !arg.IsCancelled;
		}

		internal override void RaiseAfterCollapseEvent()
		{
			AfterOutlineCollapseEventArgs arg = new AfterOutlineCollapseEventArgs(this);

			if (this.AfterCollapse != null)
			{
				this.AfterCollapse(this, arg);
			}

			sheet.RaiseAfterOutlineCollapseEvent(arg);
		}

		/// <summary>
		/// Expand outline
		/// </summary>
		public override void Expand()
		{
			var outlines = sheet.GetOutlines(RowOrColumn.Row);
			if (outlines == null) return;

			if (!RaiseBeforeExpandingEvent()) return;

			// find self group index
			int groupIndex = 0;
			for (; groupIndex < outlines.Count - 1; groupIndex++)
			{
				var o = outlines[groupIndex];
				if (o.Contains(this))
				{
					break;
				}
			}

			base.ExpandOuterOutlines(outlines, groupIndex);

			// find whether or not any rows are contained by any inner outlines
			sheet.SetRowsHeight(this.Start, this.Count, r =>
			{
				for (int i = groupIndex + 1; i < outlines.Count - 1; i++)
				{
					var g = outlines[i];
					var outline = g.FirstOrDefault(o => o.Start <= r && o.End > r);

					if (outline != null && outline.InternalCollapsed)
					{
						return 0;
					}
				}

				var rowhead = sheet.RetrieveRowHeader(r);
				return rowhead.IsVisible ? rowhead.InnerHeight : rowhead.LastHeight;
			}, false);

			this.InternalCollapsed = false;
			RaiseAfterExpandingEvent();
		}

		/// <summary>
		/// Get or set the number of row (Same as Start property)
		/// </summary>
		public int Row
		{
			get { return this.Start; }
			set { this.Start = value; }
		}

		/// <summary>
		/// Get or set the number of rows (Same as Count property)
		/// </summary>
		public int Rows
		{
			get { return this.Count; }
			set { this.Count = value; }
		}

		/// <summary>
		/// Get the number of end row (Same as End property - 1)
		/// </summary>
		public int EndRow
		{
			get { return this.End - 1; }
		}

		internal override bool RaiseBeforeExpandingEvent()
		{
			BeforeOutlineExpandingEventArgs arg = new BeforeOutlineExpandingEventArgs(this);

			if (this.BeforeExpand != null)
			{
				this.BeforeExpand(this, arg);
			}

			sheet.RaiseBeforeOutlineExpandEvent(arg);

			return !arg.IsCancelled;
		}

		internal override void RaiseAfterExpandingEvent()
		{
			AfterOutlineExpandingEventArgs arg = new AfterOutlineExpandingEventArgs(this);

			if (this.AfterExpand != null)
			{
				this.AfterExpand(this, arg);
			}

			sheet.RaiseAfterOutlineExpandEvent(arg);
		}

		/// <summary>
		/// Event is raised before this outline collapse
		/// </summary>
		public override event EventHandler<BeforeOutlineCollapseEventArgs> BeforeCollapse;

		/// <summary>
		/// Event is raised after this outline collapse
		/// </summary>
		public override event EventHandler<AfterOutlineCollapseEventArgs> AfterCollapse;

		/// <summary>
		/// Event is raised before this outline expanding
		/// </summary>
		public override event EventHandler<BeforeOutlineExpandingEventArgs> BeforeExpand;

		/// <summary>
		/// Event is raised after this outline expanding
		/// </summary>
		public override event EventHandler<AfterOutlineExpandingEventArgs> AfterExpand;

	}

	/// <summary>
	/// Represents instance of column outline.
	/// </summary>
	public class ColumnOutline : ReoGridOutline, IColumnRange
	{
		internal ColumnOutline(Worksheet grid, int start, int count)
			: base(grid, start, count) { }

		/// <summary>
		/// Collapse outline
		/// </summary>
		public override void Collapse()
		{
			var outlines = sheet.GetOutlines(RowOrColumn.Column);
			if (outlines == null) return;

			if (!RaiseBeforeCollapseEvent()) return;

			// find self group index
			int groupIndex = 0;
			for (; groupIndex < outlines.Count - 1; groupIndex++)
			{
				var o = outlines[groupIndex];
				if (o.Contains(this))
				{
					break;
				}
			}

			// find whether the rows contained by any inner outlines
			sheet.SetColumnsWidth(this.Start, this.Count, c =>
			{
				for (int i = groupIndex + 1; i < outlines.Count - 1; i++)
				{
					var g = outlines[i];

					// find collapsed inner outlines
					var outline = g.FirstOrDefault(o => o.InternalCollapsed && o != this && o.Start <= c && o.End > c);

					// if there is any inner outlines attached on this column, skip adjust this row's height
					if (outline != null)
					{
						return -1;
					}
				}

				return 0;
			}, false);

			base.CollapseInnerOutlines(outlines, groupIndex);

			this.InternalCollapsed = true;
			RaiseAfterCollapseEvent();
		}

		internal override bool RaiseBeforeCollapseEvent()
		{
			BeforeOutlineCollapseEventArgs arg = new BeforeOutlineCollapseEventArgs(this);

			if (this.BeforeCollapse != null)
			{
				this.BeforeCollapse(this, arg);
			}

			sheet.RaiseBeforeOutlineCollapseEvent(arg);

			return !arg.IsCancelled;
		}

		internal override void RaiseAfterCollapseEvent()
		{
			AfterOutlineCollapseEventArgs arg = new AfterOutlineCollapseEventArgs(this);

			if (this.AfterCollapse != null)
			{
				this.AfterCollapse(this, arg);
			}

			sheet.RaiseAfterOutlineCollapseEvent(arg);
		}

		/// <summary>
		/// Expand outline
		/// </summary>
		public override void Expand()
		{
			var outlines = sheet.GetOutlines(RowOrColumn.Column);
			if (outlines == null) return;

			if (!RaiseBeforeExpandingEvent()) return;

			// find self group index
			int groupIndex = 0;
			for (; groupIndex < outlines.Count - 1; groupIndex++)
			{
				var o = outlines[groupIndex];
				if (o.Contains(this))
				{
					break;
				}
			}

			base.ExpandOuterOutlines(outlines, groupIndex);

			// find whether the columns contained by other inner outlines
			sheet.SetColumnsWidth(this.Start, this.Count, r =>
			{
				for (int i = groupIndex + 1; i < outlines.Count - 1; i++)
				{
					var g = outlines[i];
					var outline = g.FirstOrDefault(o => o.Start <= r && o.End > r);

					if (outline != null && outline.InternalCollapsed)
					{
						return 0;
					}
				}

				var colhead = sheet.RetrieveColumnHeader(r);
				return colhead.IsVisible ? colhead.InnerWidth : colhead.LastWidth;
			}, false);
	
			this.InternalCollapsed = false;
			RaiseAfterExpandingEvent();
		}

		/// <summary>
		/// Get or set the number of column (Same as Start property)
		/// </summary>
		public int Col
		{
			get { return this.Start; }
			set { this.Start = value; }
		}

		/// <summary>
		/// Get or set the number of columns (Same as Count property)
		/// </summary>
		public int Cols
		{
			get { return this.Count; }
			set { this.Count = value; }
		}

		/// <summary>
		/// Get the number of end column (Same as End property)
		/// </summary>
		public int EndCol
		{
			get { return this.End; }
		}

		internal override bool RaiseBeforeExpandingEvent()
		{
			BeforeOutlineExpandingEventArgs arg = new BeforeOutlineExpandingEventArgs(this);

			if (this.BeforeExpand != null)
			{
				this.BeforeExpand(this, arg);
			}

			sheet.RaiseBeforeOutlineExpandEvent(arg);

			return !arg.IsCancelled;
		}

		internal override void RaiseAfterExpandingEvent()
		{
			AfterOutlineExpandingEventArgs arg = new AfterOutlineExpandingEventArgs(this);

			if (this.AfterExpand != null)
			{
				this.AfterExpand(this, arg);
			}

			sheet.RaiseAfterOutlineExpandEvent(arg);
		}

		/// <summary>
		/// Event is raised before this outline collapse
		/// </summary>
		public override event EventHandler<BeforeOutlineCollapseEventArgs> BeforeCollapse;

		/// <summary>
		/// Event is raised after this outline collapse
		/// </summary>
		public override event EventHandler<AfterOutlineCollapseEventArgs> AfterCollapse;

		/// <summary>
		/// Event is raised before this outline expanding
		/// </summary>
		public override event EventHandler<BeforeOutlineExpandingEventArgs> BeforeExpand;

		/// <summary>
		/// Event is raised after this outline expanding
		/// </summary>
		public override event EventHandler<AfterOutlineExpandingEventArgs> AfterExpand;
	}

	/// <summary>
	/// Common outline event argument.
	/// </summary>
	public class OutlineEventArgs : EventArgs
	{
		/// <summary>
		/// Outline instance for both row and column outline.
		/// </summary>
		public IReoGridOutline Outline { get; set; }

		/// <summary>
		/// Create outline event argument with specified outline instance.
		/// </summary>
		/// <param name="outline">instance of either row or column outline</param>
		public OutlineEventArgs(IReoGridOutline outline)
		{
			this.Outline = outline;
		}
	}

	/// <summary>
	/// Event raised when outline has been added into spreadsheet.
	/// </summary>
	public class OutlineAddedEventArgs : OutlineEventArgs
	{
		/// <summary>
		/// Create outline event argument with specified outline instance.
		/// </summary>
		/// <param name="outline">instance of either row or column outline</param>
		public OutlineAddedEventArgs(ReoGridOutline outline) : base(outline) { }
	}

	/// <summary>
	/// Event raised when outline has been removed into spreadsheet.
	/// </summary>
	public class OutlineRemovedEventArgs : OutlineEventArgs
	{
		/// <summary>
		/// Create outline event argument with specified outline instance.
		/// </summary>
		/// <param name="outline">instance of either row or column outline</param>
		public OutlineRemovedEventArgs(IReoGridOutline outline) : base(outline) { }
	}

	/// <summary>
	/// Event raised before outline collapsing.
	/// </summary>
	public class BeforeOutlineCollapseEventArgs : OutlineEventArgs
	{
		/// <summary>
		/// Create outline event argument with specified outline instance.
		/// </summary>
		/// <param name="outline">instance of either row or column outline</param>
		public BeforeOutlineCollapseEventArgs(ReoGridOutline outline) : base(outline) { }

		/// <summary>
		/// Get or set whether or not to abort current operation.
		/// </summary>
		public bool IsCancelled { get; set; }
	}

	/// <summary>
	/// Event raised before outline expanding.
	/// </summary>
	public class BeforeOutlineExpandingEventArgs : OutlineEventArgs
	{
		/// <summary>
		/// Create event arguments instance.
		/// </summary>
		/// <param name="outline"></param>
		public BeforeOutlineExpandingEventArgs(ReoGridOutline outline) : base(outline) { }

		/// <summary>
		/// Get or set whether or not to abort current operation.
		/// </summary>
		public bool IsCancelled { get; set; }
	}

	/// <summary>
	/// Event raised after outline collapsing.
	/// </summary>
	public class AfterOutlineCollapseEventArgs : OutlineEventArgs
	{
		/// <summary>
		/// Create event arguments instance.
		/// </summary>
		/// <param name="outline">The outline has been collapsed.</param>
		public AfterOutlineCollapseEventArgs(ReoGridOutline outline) : base(outline) { }
	}

	/// <summary>
	/// Event raised after outline expanding.
	/// </summary>
	public class AfterOutlineExpandingEventArgs : OutlineEventArgs
	{
		/// <summary>
		/// Create event arguments instance.
		/// </summary>
		/// <param name="outline">The outline has been expanded.</param>
		public AfterOutlineExpandingEventArgs(ReoGridOutline outline) : base(outline) { }
	}

}

#endif // OUTLINE