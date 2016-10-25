using System;
using System.Collections.Generic;
using System.Linq;

using unvell.ReoGrid.Main;

namespace unvell.ReoGrid
{
	/// <summary>
	/// Collection of Worksheet 
	/// </summary>
	public class WorksheetCollection : IList<Worksheet>
	{
		private IControlAdapter adapter;
		private Workbook workbook;

		internal WorksheetCollection(Workbook workbook)
		{
			this.adapter = workbook.controlAdapter;
			this.workbook = workbook;
		}

		/// <summary>
		/// Create worksheet by specified name
		/// </summary>
		/// <param name="name">Unique name used to identify the worksheet</param>
		/// <returns>Instance of worksheet created by specified name</returns>
		public Worksheet Create(string name = null)
		{
			return this.workbook.CreateWorksheet(name);
		}

		/// <summary>
		/// Add worksheet
		/// </summary>
		/// <param name="sheet">Worksheet to be added</param>
		public void Add(Worksheet sheet)
		{
			this.workbook.InsertWorksheet(this.workbook.worksheets.Count, sheet);
		}

		/// <summary>
		/// Insert worksheet at specified position
		/// </summary>
		/// <param name="index">Zero-based number of worksheet to insert the worksheet</param>
		/// <param name="sheet">Worksheet to be inserted</param>
		public void Insert(int index, Worksheet sheet)
		{
			this.workbook.InsertWorksheet(index, sheet);
		}

		/// <summary>
		/// Clear all worksheet from this workbook
		/// </summary>
		public void Clear()
		{
			this.workbook.ClearWorksheets();
		}

		/// <summary>
		/// Check whether or not specified worksheet is contained in this workbook
		/// </summary>
		/// <param name="sheet"></param>
		/// <returns></returns>
		public bool Contains(Worksheet sheet)
		{
			return this.workbook.worksheets.Contains(sheet);
		}

		/// <summary>
		/// Get number of worksheets in this workbook
		/// </summary>
		public int Count
		{
			get { return this.workbook.worksheets.Count; }
		}

		/// <summary>
		/// Check whether or not current workbook is read-only
		/// </summary>
		public bool IsReadOnly
		{
			get { return this.workbook.Readonly; }
		}

		/// <summary>
		/// Remove worksheet instance
		/// </summary>
		/// <param name="sheet">Instace of worksheet to be removed</param>
		/// <returns></returns>
		public bool Remove(Worksheet sheet)
		{
			return this.workbook.RemoveWorksheet(sheet);
		}

		/// <summary>
		/// Get enumerator of worksheet list
		/// </summary>
		/// <returns>Enumerator of worksheet list</returns>
		public IEnumerator<Worksheet> GetEnumerator()
		{
			return this.workbook.worksheets.GetEnumerator();
		}

		/// <summary>
		/// Get enumerator of worksheet list
		/// </summary>
		/// <returns>Enumerator of worksheet list</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.workbook.worksheets.GetEnumerator();
		}

		/// <summary>
		/// Get or set worksheet by specified position
		/// </summary>
		/// <param name="index">Zero-based number of worksheet</param>
		/// <returns>Instance of worksheet found at specified position</returns>
		public Worksheet this[int index]
		{
			get
			{
				if (index < 0 || index >= this.workbook.worksheets.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}

				return this.workbook.worksheets[index];
			}
			set
			{
				if (index < 0 || index >= this.workbook.worksheets.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}

				this.workbook.worksheets[index] = value;
			}
		}

		/// <summary>
		/// Get worksheet by specified name.
		/// </summary>
		/// <param name="name">Name to find worksheet</param>
		/// <returns>Instacne of worksheet found by specified name</returns>
		public Worksheet this[string name]
		{
			get
			{
				return this.workbook.worksheets.FirstOrDefault(s => string.Compare(s.Name, name, true) == 0);
			}
		}

		/// <summary>
		/// Get the index position of specified worksheet
		/// </summary>
		/// <param name="sheet">Instace of worksheet</param>
		/// <returns>Zero-based number of worksheet</returns>
		public int IndexOf(Worksheet sheet)
		{
			return this.workbook.GetWorksheetIndex(sheet);
		}

		/// <summary>
		/// Remove worksheet from specified position
		/// </summary>
		/// <param name="index">Zero-based number of worksheet to locate the worksheet to be removed</param>
		public void RemoveAt(int index)
		{
			this.workbook.RemoveWorksheet(index);
		}

		/// <summary>
		/// Copy all worksheet instances into specified array
		/// </summary>
		/// <param name="array">Array used to store worksheets</param>
		/// <param name="arrayIndex">Start index to copy the worksheets</param>
		public void CopyTo(Worksheet[] array, int arrayIndex)
		{
			this.workbook.worksheets.CopyTo(array, arrayIndex);
		}
	}

}
