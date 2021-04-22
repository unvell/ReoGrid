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
using System.Linq;
using unvell.ReoGrid.Events;

namespace unvell.ReoGrid
{
	partial class Worksheet
	{
		internal Dictionary<string, NamedRange> registeredNamedRanges = new Dictionary<string, NamedRange>();

		/// <summary>
		/// Add a named range into current spreadsheet.
		/// </summary>
		/// <param name="namedRange">Named range to be added.</param>
		public void AddNamedRange(NamedRange namedRange)
		{
			if (namedRange == null)
			{
				throw new ArgumentNullException("namedRange", "Specified range object cannot be null");
			}

			if (string.IsNullOrEmpty(namedRange.Name))
			{
				throw new ArgumentNullException("namedRange.Name", "Name of range object cannot be empty");
			}

			if (namedRange.Worksheet != null && namedRange.Worksheet != this)
			{
				throw new ArgumentException("Specified range is belong to another worksheet, remove from other worksheet firstly, or create a new range at same position.");
			}

			if (namedRange.Scope == NamedRangeScope.Workbook
				&& this.workbook.worksheets.Any(s => s.registeredNamedRanges.Any(p =>
					p.Value.Scope == NamedRangeScope.Workbook
					&& p.Key.Equals(namedRange.Name, StringComparison.CurrentCultureIgnoreCase))))
			{
				throw new NamedRangeAlreadyDefinedException();
			}

			this.registeredNamedRanges[namedRange.Name] = namedRange;
			namedRange.Worksheet = this;

			if (NamedRangeDefined != null)
			{
				NamedRangeDefined(this, new NamedRangeAddedEventArgs(namedRange));
			}
		}

		/// <summary>
		/// Define named range from an address.
		/// </summary>
		/// <param name="name">Name to identify a range in spreadsheet.</param>
		/// <param name="address">Address reference to a range.</param>
		/// <returns>Instance of named range added into spreadsheet</returns>
		/// <exception cref="InvalidAddressException">throw if specified address or name is illegal</exception>
		public NamedRange DefineNamedRange(string name, string address)
		{
			return this.DefineNamedRange(name, address, NamedRangeScope.Workbook);
		}

		/// <summary>
		/// Define named range from an address.
		/// </summary>
		/// <param name="name">Name to identify a range in spreadsheet</param>
		/// <param name="address">Address reference to a range</param>
		/// <param name="scope">The valid scope of this named range</param>
		/// <returns>Instance of named range added into spreadsheet</returns>
		/// <exception cref="InvalidAddressException">throw if specified address or name is illegal</exception>
		public NamedRange DefineNamedRange(string name, string address, NamedRangeScope scope)
		{
			if (!RangePosition.IsValidAddress(address))
			{
				throw new InvalidAddressException(address);
			}

			return DefineNamedRange(name, new RangePosition(address), scope);
		}

		/// <summary>
		/// Define named range to specify region
		/// </summary>
		/// <param name="name">name to be defined to the range</param>
		/// <param name="row">number of row start to the range</param>
		/// <param name="col">number of column start to the range</param>
		/// <param name="rows">number of rows contained in the range</param>
		/// <param name="cols">number of columns contained in the range</param>
		/// <returns>Instance of named range</returns>
		public NamedRange DefineNamedRange(string name, int row, int col, int rows, int cols)
		{
			return DefineNamedRange(name, new RangePosition(row, col, rows, cols), NamedRangeScope.Workbook);
		}

		/// <summary>
		/// Define named range to specify region
		/// </summary>
		/// <param name="name">name to be defined to the range</param>
		/// <param name="row">number of row start to the range</param>
		/// <param name="col">number of column start to the range</param>
		/// <param name="rows">number of rows contained in the range</param>
		/// <param name="cols">number of columns contained in the range</param>
		/// <param name="scope">The valid scope of this named range</param>
		/// <returns>Instance of named range</returns>
		public NamedRange DefineNamedRange(string name, int row, int col, int rows, int cols, NamedRangeScope scope)
		{
			return DefineNamedRange(name, new RangePosition(row, col, rows, cols), scope);
		}

		/// <summary>
		/// Define named range registered into spreadsheet
		/// </summary>
		/// <param name="name">Name to identify a range in spreadsheet</param>
		/// <param name="range">Range to be added into spreadsheet</param>
		/// <param name="scope">The valid scope of this named range</param>
		/// <returns>Instance of named range added into spreadsheet</returns>
		public NamedRange DefineNamedRange(string name, RangePosition range, NamedRangeScope scope = NamedRangeScope.Workbook)
		{
			var namedRange = new NamedRange(this, name, FixRange(range))
			{
				Scope = scope,
			};

			AddNamedRange(namedRange);

			return namedRange;
		}

		/// <summary>
		/// Get named range which registered in current sheet
		/// </summary>
		/// <param name="name">string to name a range</param>
		/// <returns>referenced range of specified name</returns>
		public NamedRange GetNamedRange(string name)
		{
			if (registeredNamedRanges == null) return null;

			registeredNamedRanges.TryGetValue(name, out var range);

			return range;
		}

		/// <summary>
		/// Try get a named range by specified name
		/// </summary>
		/// <param name="name">name for range to be get</param>
		/// <param name="namedRange">output argument, range assoicated with specified name will be returned</param>
		/// <returns>true if specified name exists and the instance of range has been returned from spreadsheet, otherwise false</returns>
		public bool TryGetNamedRange(string name, out NamedRange namedRange)
		{
			if (!RGUtility.IsValidName(name))
			{
				namedRange = null;
				return false;
			}
			else
				return registeredNamedRanges.TryGetValue(name, out namedRange);
		}

		internal bool TryGetNamedRangePosition(string name, out RangePosition range)
		{
			if (!RGUtility.IsValidName(name))
			{
				range = RangePosition.Empty;
				return false;
			}
			else
			{
				NamedRange namedRange;
				registeredNamedRanges.TryGetValue(name, out namedRange);
				range = namedRange.Position;
				return true;
			}
		}

		/// <summary>
		/// Try get range by specified address or range name.
		/// </summary>
		/// <param name="addressOrName">Address or range name used to find range on worksheet.</param>
		/// <param name="range">Range that was found by specified address or name on worksheet.</param>
		/// <returns>True if range was found; Otherwise return false.</returns>
		public bool TryGetRangeByAddressOrName(string addressOrName, out RangePosition range)
		{
			if (RangePosition.IsValidAddress(addressOrName))
			{
				range = new RangePosition(addressOrName);
				return true;
			}
			else if (NamedRange.IsValidName(addressOrName))
			{
				NamedRange namedRange;

				if (this.registeredNamedRanges.TryGetValue(addressOrName, out namedRange))
				{
					range = (RangePosition)namedRange;
					return true;
				}
			}

			range = RangePosition.Empty;
			return false;
		}

		/// <summary>
		/// Get all named range that has been registered in current spreadsheet
		/// </summary>
		/// <returns>list of name for all named ranges registered in this</returns>
		public IEnumerable<string> GetAllNamedRanges()
		{
			return (IEnumerable<string>)(this.registeredNamedRanges.Keys);
		}

		/// <summary>
		/// Find name by specified address of range
		/// </summary>
		/// <param name="address">an address used to locate the range</param>
		/// <returns>name to target range, and null if not found</returns>
		public string GetNameByRange(string address)
		{
			return RangePosition.IsValidAddress(address) ? GetNameByRange(new RangePosition(address)) : null;
		}

		/// <summary>
		/// Find name for a named range by its location
		/// </summary>
		/// <param name="range">a range location used to search the name</param>
		/// <returns>name as string for the range, return null if no range was found</returns>
		public string GetNameByRange(RangePosition range)
		{
			var rr = this.registeredNamedRanges.FirstOrDefault(nr => nr.Value.StartPos == range.StartPos
				&& nr.Value.EndPos == range.EndPos);

			return rr.Key == null ? null : rr.Key;
		}

		/// <summary>
		/// Undefine named range from this worksheet
		/// </summary>
		/// <param name="name">Name of range</param>
		/// <returns>true if the range was found and removed by specified name</returns>
		public bool UndefineNamedRange(string name)
		{
			if (registeredNamedRanges.TryGetValue(name, out var namedRange))
			{
				namedRange.Worksheet = null;
				registeredNamedRanges.Remove(name);

				if (this.NamedRangeUndefined != null)
				{
					this.NamedRangeUndefined(this, new NamedRangeUndefinedEventArgs(namedRange, namedRange.Name));
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Rename a named range to another name
		/// </summary>
		/// <param name="oldName">Old name used to find range to be renamed</param>
		/// <param name="newName">New name to set to the range</param>
		/// <returns>true if range could be found by old name, and renamed to new name successfully</returns>
		public bool RenameNamedRange(string oldName, string newName)
		{
			if (this.registeredNamedRanges.TryGetValue(oldName, out var namedRange))
			{
				namedRange.internalName = newName;
				this.registeredNamedRanges.Remove(oldName);
				this.registeredNamedRanges[newName] = namedRange;

				return true;
			}
			else
			{
				return false;
			}
		}

		private string GetAvailableRangeName()
		{
			string name = null;
			int index = 0;
			while (registeredNamedRanges.ContainsKey((name = "__unnamed_range" +
				(index == 0 ? string.Empty : ("_" + index)))))
				index++;

			return name;
		}

		private NamedRangeCollection namedRangeCollection = null;

		/// <summary>
		/// Get collection of named range.
		/// </summary>
		public NamedRangeCollection NamedRanges
		{
			get
			{
				if (this.namedRangeCollection == null)
				{
					this.namedRangeCollection = new NamedRangeCollection(this);
				}
				return this.namedRangeCollection;
			}
		}

		/// <summary>
		/// Event raised when named range is defined
		/// </summary>
		public event EventHandler<NamedRangeAddedEventArgs> NamedRangeDefined;

		/// <summary>
		/// Event raised when named range is undefined
		/// </summary>
		public event EventHandler<NamedRangeUndefinedEventArgs> NamedRangeUndefined;
	}

	/// <summary>
	/// Named range reference to spreadsheet
	/// </summary>
	public class NamedRange : ReferenceRange
	{
		internal string internalName;

		/// <summary>
		/// Name to the range
		/// </summary>
		public string Name
		{
			get { return internalName; }
			set
			{
				if (this.internalName != value)
				{
					NamedRange oldRange = null;

					if (this.Worksheet != null)
					{
						oldRange = this.Worksheet.GetNamedRange(this.internalName);
					}

					if (oldRange != null)
					{
						this.Worksheet.RenameNamedRange(this.internalName, value);
					}
					else
					{
						this.internalName = value;
					}

					if (this.NameChanged != null)
					{
						this.NameChanged(this, null);
					}
				}
			}
		}

		/// <summary>
		/// Comment for describing this range
		/// </summary>
		public string Comment { get; set; }

		/// <summary>
		/// Create named range from specified worksheet and address
		/// </summary>
		/// <param name="worksheet">worksheet to hold this named range</param>
		/// <param name="name">name to identify this range on spreadsheet</param>
		/// <param name="address">address identifier to locate a range on spreadsheet</param>
		public NamedRange(Worksheet worksheet, string name, string address)
			: this(worksheet, name, new RangePosition(address))
		{
		}

		/// <summary>
		/// Create named range from specified worksheet and address
		/// </summary>
		/// <param name="worksheet">worksheet to hold this named range</param>
		/// <param name="name">name to identify this range on spreadsheet</param>
		/// <param name="range">address identifier to locate a range on spreadsheet</param>
		public NamedRange(Worksheet worksheet, string name, RangePosition range)
			: this(worksheet, name, range.StartPos, range.EndPos)
		{
		}

		/// <summary>
		/// Create named range from specified worksheet and address
		/// </summary>
		/// <param name="worksheet">worksheet to hold this named range</param>
		/// <param name="name">name to identify this range on spreadsheet</param>
		/// <param name="startPos">start position to locate range start on spreadsheet</param>
		/// <param name="endPos">start position to locate range end on spreadsheet</param>
		public NamedRange(Worksheet worksheet, string name, CellPosition startPos, CellPosition endPos)
			: base(worksheet, startPos, endPos)
		{
			if (RangePosition.IsValidAddress(name))
			{
				throw new InvalidNamedRangeException(name);
			}

			this.internalName = name;
		}

		/// <summary>
		/// Event raised when name is changed
		/// </summary>
		public event EventHandler NameChanged;

		/// <summary>
		/// Check whether or not the specified name is valid name
		/// </summary>
		/// <param name="name">Name to be checked</param>
		/// <returns>True if specified name is valid; otherwise return false</returns>
		public static bool IsValidName(string name)
		{
			return RGUtility.IsValidName(name);
		}

		/// <summary>
		/// Get or set the scope of named range
		/// </summary>
		public NamedRangeScope Scope { get; set; }

		/// <summary>
		/// Convert named range to string.
		/// </summary>
		/// <returns>String to describe the named range.</returns>
		public override string ToString()
		{
			return string.Format("{0}!{1}", this.Worksheet.Name, this.ToAddress());
		}
	}

	/// <summary>
	/// Scope flags for named range
	/// </summary>
	public enum NamedRangeScope
	{
		/// <summary>
		/// Named range is valid inside whole workbook (available for all worksheets)
		/// </summary>
		Workbook,

		/// <summary>
		/// Named range is valid inside specified worksheet
		/// </summary>
		Worksheet,
	}

	/// <summary>
	/// Collection of named range on workbook.
	/// </summary>
	public class NamedRangeCollection : ICollection<NamedRange>
	{
		private Worksheet sheet;

		internal NamedRangeCollection(Worksheet sheet)
		{
			this.sheet = sheet;
		}

		/// <summary>
		/// Add named range instance
		/// </summary>
		/// <param name="item">range to be added</param>
		public void Add(NamedRange item)
		{
			this.sheet.AddNamedRange(item);
		}

		/// <summary>
		/// Clear all named range
		/// </summary>
		public void Clear()
		{
			this.sheet.registeredNamedRanges.Clear();
		}

		/// <summary>
		/// Check whether or not specified range exists already
		/// </summary>
		/// <param name="range">Range to be checked</param>
		/// <returns>Return true if specified range exists; Otherwise return false</returns>
		public bool Contains(NamedRange range)
		{
			return this.sheet.registeredNamedRanges.Values.Any(nr => nr.Name == range.Name && nr.Position == range.Position);
		}

		/// <summary>
		/// Copy named ranges into specified array
		/// </summary>
		/// <param name="array">Array to store named range</param>
		/// <param name="arrayIndex">Index to start copy ranges</param>
		public void CopyTo(NamedRange[] array, int arrayIndex)
		{
			this.sheet.registeredNamedRanges.Values.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Get number of named ranges
		/// </summary>
		public int Count
		{
			get { return this.sheet.registeredNamedRanges.Values.Count; }
		}

		/// <summary>
		/// Check whether or not to allow modify named range collection in this workbook
		/// </summary>
		public bool IsReadOnly
		{
			get { return false; }
		}

		/// <summary>
		/// Remove specified range from named range list
		/// </summary>
		/// <param name="range">Range to be removed</param>
		/// <returns>Return true if specified range can be found, and removed successfully</returns>
		public bool Remove(NamedRange range)
		{
			//todo: remove global named range 
			return this.sheet.UndefineNamedRange(range.Name);
		}

		/// <summary>
		/// Get enumerator of this collection
		/// </summary>
		/// <returns>Enumerator used to iterate over this collection</returns>
		public IEnumerator<NamedRange> GetEnumerator()
		{
			return this.sheet.registeredNamedRanges.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.sheet.registeredNamedRanges.Values.GetEnumerator();
		}

		/// <summary>
		/// Get or set the named range by specified name.
		/// </summary>
		/// <param name="name">Name of range.</param>
		/// <returns>Instance of named range.</returns>
		public NamedRange this[string name]
		{
			get
			{
				return this.sheet.TryGetNamedRange(name, out var range) ? range : null;
			}
			set
			{
				if (value.Name != name)
				{
					throw new ArgumentException("Specified name is different from the name that is set in the range instance.", "name");
				}

				this.sheet.AddNamedRange(value);
			}
		}
	}
}
