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

#if DRAWING

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using unvell.ReoGrid.Data;
using unvell.ReoGrid.Graphics;

namespace unvell.ReoGrid.Chart
{
	/// <summary>
	/// Represents the interface of data source used for chart.
	/// </summary>
	/// <typeparam name="T">Standard data serial classes.</typeparam>
	public interface IChartDataSource<T> : IDataSource<T> where T : IChartDataSerial
	{
		/// <summary>
		/// Get number of categories.
		/// </summary>
		int CategoryCount { get; }

		/// <summary>
		/// Get category name by specified index position.
		/// </summary>
		/// <param name="index">Zero-based number of category to get its name.</param>
		/// <returns>Specified category's name by index position.</returns>
		string GetCategoryName(int index);
	}

	/// <summary>
	/// Represents the interface of data serial used for chart.
	/// </summary>
	public interface IChartDataSerial : IDataSerial
	{
		/// <summary>
		/// Get the serial name.
		/// </summary>
		string Label { get; }
	}

	/// <summary>
	/// Data source from given worksheet
	/// </summary>
	public class WorksheetChartDataSource : IChartDataSource<WorksheetChartDataSerial>
	{
		private Worksheet worksheet;

		/// <summary>
		/// Get instance of worksheet
		/// </summary>
		public Worksheet Worksheet { get; protected set; }

		#region Ranges

		private int categoryCount;

		#endregion // Ranges

		#region Constructor

		/// <summary>
		/// Create data source instance with specified worksheet instance
		/// </summary>
		/// <param name="worksheet">Instance of worksheet to read titles and data of plot serial.</param>
		public WorksheetChartDataSource(Worksheet worksheet)
		{
			this.worksheet = worksheet;
		}

		/// <summary>
		/// Create data source instance with specified worksheet instance
		/// </summary>
		/// <param name="worksheet">Instance of worksheet to read titles and data of plot serial.</param>
		/// <param name="serialNamesRange">Names for serial data from this range.</param>
		/// <param name="serialsRange">Serial data from this range.</param>
		/// <param name="serialPerRowOrColumn">Add serials by this specified direction. Default is Row.</param>
		public WorksheetChartDataSource(Worksheet worksheet, string serialNamesRange, string serialsRange,
			RowOrColumn serialPerRowOrColumn = RowOrColumn.Row)
			: this(worksheet)
		{
			if (worksheet == null)
			{
				throw new ArgumentNullException("worksheet");
			}

			if (!worksheet.TryGetRangeByAddressOrName(serialNamesRange, out var snRange))
			{
				throw new InvalidAddressException("cannot determine the serial names range by specified range address or name.");
			}

			if (!worksheet.TryGetRangeByAddressOrName(serialsRange, out var sRange))
			{
				throw new InvalidAddressException("cannot determine the serials range by specified range address or name.");
			}

			this.AddSerialsFromRange(snRange, sRange, serialPerRowOrColumn);
		}

		/// <summary>
		/// Create data source instance with specified worksheet instance and serial data range.
		/// </summary>
		/// <param name="worksheet">Instance of worksheet to read titles and data of plot serial.</param>
		/// <param name="serialNamesRange">Range to read labels of data serial.</param>
		/// <param name="serialsRange">Range to read serial data.</param>
		/// <param name="serialPerRowOrColumn">Add serials by this specified direction. Default is Row.</param>
		public WorksheetChartDataSource(Worksheet worksheet, RangePosition serialNamesRange, RangePosition serialsRange,
			RowOrColumn serialPerRowOrColumn = RowOrColumn.Row)
			: this(worksheet)
		{
			if (worksheet == null)
			{
				throw new ArgumentNullException("worksheet");
			}

			this.AddSerialsFromRange(serialNamesRange, serialsRange, serialPerRowOrColumn);
		}

		private void AddSerialsFromRange(RangePosition serialNamesRange, RangePosition serialsRange, 
			RowOrColumn serialPerRowOrColumn = RowOrColumn.Row)
		{
			if (serialPerRowOrColumn == RowOrColumn.Row)
			{
				for (int r = serialsRange.Row; r <= serialsRange.EndRow; r++)
				{
					var label = new CellPosition(r, serialNamesRange.Col);
					this.AddSerial(worksheet, label, new RangePosition(r, serialsRange.Col, 1, serialsRange.Cols));
				}
			}
			else
			{
				for (int c = serialsRange.Col; c <= serialsRange.EndCol; c++)
				{
					var label = new CellPosition(serialNamesRange.Row, c);
					this.AddSerial(worksheet, label, new RangePosition(serialsRange.Row, c, serialsRange.Rows, 1));
				}
			}
		}

		#endregion // Constructor

		#region Changes
		/// <summary>
		/// This method will be invoked when any data from the serial data range changed.
		/// </summary>
		public virtual void OnDataChanged()
		{
			if (this.DataChanged != null)
			{
				this.DataChanged(this, null);
			}
		}

		///// <summary>
		///// This method will be invoked when the serial data range changed.
		///// </summary>
		//public virtual void OnDataRangeChanged()
		//{
		//	if (this.DataRangeChanged != null)
		//	{
		//		this.DataRangeChanged(this, null);
		//	}
		//}

		/// <summary>
		/// This event will be raised when data from the serial data range changed.
		/// </summary>
		public event EventHandler DataChanged;

		///// <summary>
		///// This event will be raised when the serial data range changed.
		///// </summary>
		//public event EventHandler DataRangeChanged;

		#endregion // Changes

		#region Category

		/// <summary>
		/// Get or set the range that contains the category names.
		/// </summary>
		public RangePosition CategoryNameRange { get; set; }

		/// <summary>
		/// Return the title of specified column.
		/// </summary>
		/// <param name="index">Zero-based number of column.</param>
		/// <returns>Return the title that will be displayed on chart.</returns>
		public string GetCategoryName(int index)
		{
			if (this.CategoryNameRange.IsEmpty)
			{
				return null;
			}
			else
			{
				return this.worksheet.GetCellData<string>(this.CategoryNameRange.Row, this.CategoryNameRange.Col + index);
			}
		}

		#endregion // Category

		#region Serials
		/// <summary>
		/// Get number of data serials.
		/// </summary>
		public virtual int SerialCount { get { return this.serials.Count; } }

		/// <summary>
		/// Get number of categories.
		/// </summary>
		public virtual int CategoryCount { get { return this.categoryCount; } }

		internal List<WorksheetChartDataSerial> serials = new List<WorksheetChartDataSerial>();

		public WorksheetChartDataSerial this[int index]
		{
			get
			{
				if (index < 0 || index >= this.serials.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}

				return this.serials[index];
			}
		}

		/// <summary>
		/// Add serial data into data source.
		/// </summary>
		/// <param name="serial">Serial data source.</param>
		public void Add(WorksheetChartDataSerial serial)
		{
			this.serials.Add(serial);

			this.UpdateCategoryCount(serial);
		}

		internal void UpdateCategoryCount(WorksheetChartDataSerial serial)
		{
			this.categoryCount = Math.Max(this.categoryCount, Math.Max(serial.DataRange.Cols, serial.DataRange.Rows));
		}

		/// <summary>
		/// Add serial data into data source from a range, set the name as the label of serial.
		/// </summary>
		/// <param name="worksheet">Worksheet instance to read serial data.</param>
		/// <param name="name">Name for serial to be added.</param>
		/// <param name="serialRange">Range to read serial data from worksheet.</param>
		/// <returns>Instance of chart serial has been added.</returns>
		public WorksheetChartDataSerial AddSerial(Worksheet worksheet, CellPosition labelAddress, RangePosition serialRange)
		{
			var serial = new WorksheetChartDataSerial(this, worksheet, labelAddress, serialRange);
			this.Add(serial);
			return serial;
		}

		public WorksheetChartDataSerial GetSerial(int index)
		{
			if (index < 0 || index >= this.serials.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}

			return this.serials[index];
		}

		private WorksheetChartDataSerialCollection collection;

		/// <summary>
		/// Get collection of data serials.
		/// </summary>
		public WorksheetChartDataSerialCollection Serials
		{
			get
			{
				if (this.collection == null)
				{
					this.collection = new WorksheetChartDataSerialCollection(this);
				}
				return this.collection;
			}
		}
		#endregion // Serials
	}

	#region WorksheetChartDataSerialCollection
	/// <summary>
	/// Represents collection of data serial.
	/// </summary>
	public class WorksheetChartDataSerialCollection : IList<WorksheetChartDataSerial>
	{
		public WorksheetChartDataSource DataSource { get; private set; }

		private List<WorksheetChartDataSerial> serials;

		internal WorksheetChartDataSerialCollection(WorksheetChartDataSource dataSource)
		{
			this.DataSource = dataSource;
			this.serials = dataSource.serials;
		}

		public WorksheetChartDataSerial this[int index]
		{
			get { return this.serials[index]; }
			set
			{
				this.serials[index] = value;
				this.DataSource.UpdateCategoryCount(value);
			}
		}

		public int Count
		{
			get { return this.serials.Count;}
		}

		public bool IsReadOnly { get { return false; } }

		public void Add(WorksheetChartDataSerial serial)
		{
			this.DataSource.Add(serial);
		}

		public void Clear()
		{
			this.serials.Clear();
			// TODO: update category count
		}

		public bool Contains(WorksheetChartDataSerial item)
		{
			return this.serials.Contains(item);
		}

		public void CopyTo(WorksheetChartDataSerial[] array, int arrayIndex)
		{
			this.serials.CopyTo(array, arrayIndex);
		}

		public IEnumerator<WorksheetChartDataSerial> GetEnumerator()
		{
			return this.serials.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.serials.GetEnumerator();
		}

		public int IndexOf(WorksheetChartDataSerial item)
		{
			return this.serials.IndexOf(item);
		}

		public void Insert(int index, WorksheetChartDataSerial serial)
		{
			this.serials.Insert(index, serial);

			this.DataSource.UpdateCategoryCount(serial);
		}

		public bool Remove(WorksheetChartDataSerial serial)
		{
			return this.serials.Remove(serial);
			// TODO: update category count
		}

		public void RemoveAt(int index)
		{
			this.serials.RemoveAt(index);
			// TODO: update category count
		}
	}
	#endregion // WorksheetChartDataSerialCollection

	#region WorksheetChartDataSerial
	/// <summary>
	/// Represents implementation of chart data serial.
	/// </summary>
	public class WorksheetChartDataSerial : IChartDataSerial
	{
		private WorksheetChartDataSource dataSource;

		private Worksheet worksheet;

		/// <summary>
		/// Get instance of worksheet
		/// </summary>
		public Worksheet Worksheet { get; protected set; }

		private RangePosition dataRange;

		/// <summary>
		/// Determine the range to read data from worksheet
		/// </summary>
		public virtual RangePosition DataRange
		{
			get { return dataRange; }
			set
			{
				if (this.dataRange != value)
				{
					this.dataRange = value;

					//this.dataSource.OnDataRangeChanged();
					this.dataSource.OnDataChanged();
				}
			}
		}

		//private string name;
		public CellPosition LabelAddress { get; set; }

		protected WorksheetChartDataSerial(WorksheetChartDataSource dataSource, Worksheet worksheet, CellPosition labelAddress)
		{
			if (dataSource == null)
			{
				throw new ArgumentNullException("dataSource");
			}

			if (worksheet == null)
			{
				throw new ArgumentNullException("worksheet");
			}

			this.dataSource = dataSource;
			this.worksheet = worksheet;
			this.LabelAddress = labelAddress;

			if (this.worksheet != null)
			{
				this.worksheet.CellDataChanged += worksheet_CellDataChanged;
				this.worksheet.RangeDataChanged += worksheet_RangeDataChanged;
			}
		}

		/// <summary>
		/// Destroy the worksheet data serial and release all event handlers to data source.
		/// </summary>
		~WorksheetChartDataSerial()
		{
			if (this.worksheet != null)
			{
				this.worksheet.CellDataChanged -= worksheet_CellDataChanged;
				this.worksheet.RangeDataChanged -= worksheet_RangeDataChanged;
			}
		}

		/// <param name="dataSource">Data source to read chart data from worksheet.</param>
		/// <param name="worksheet">Instance of worksheet that contains the data to be read.</param>
		/// <param name="labelAddress">The address to locate label of serial on worksheet.</param>
		/// <param name="dataRange">Serial data range to read serial data for chart from worksheet.</param>
		public WorksheetChartDataSerial(WorksheetChartDataSource dataSource, Worksheet worksheet, CellPosition labelAddress, RangePosition dataRange)
			: this(dataSource, worksheet, labelAddress)
		{
			this.dataRange = dataRange;
		}

		/// <summary>
		/// Create data serial by specified worksheet instance and data range.
		/// </summary>
		/// <param name="dataSource">Data source to read chart data from worksheet.</param>
		/// <param name="worksheet">Instance of worksheet that contains the data to be read.</param>
		/// <param name="labelAddress">The address to locate label of serial on worksheet.</param>
		/// <param name="addressOrName">Serial data specified by address position or range's name.</param>
		public WorksheetChartDataSerial(WorksheetChartDataSource dataSource, Worksheet worksheet, string labelAddress, string addressOrName)
			: this(dataSource, worksheet, new CellPosition(labelAddress))
		{
			if (RangePosition.IsValidAddress(addressOrName))
			{
				this.dataRange = new RangePosition(addressOrName);
			}
			else if (NamedRange.IsValidName(addressOrName))
			{
				if (this.worksheet != null)
				{
					if (worksheet.TryGetNamedRange(addressOrName, out var range))
					{
						this.dataRange = range;
					}
					else
					{
						throw new InvalidAddressException(addressOrName);
					}
				}
				else
				{
					throw new ReferenceObjectNotAssociatedException("Data source must associate to valid worksheet instance.");
				}
			}
			else
			{
				throw new InvalidAddressException(addressOrName);
			}
		}

		#region Events
		void worksheet_CellDataChanged(object sender, Events.CellEventArgs e)
		{
			var pos = e.Cell.Position;

			if (dataRange.Contains(pos) || this.LabelAddress == pos)
			{
				this.dataSource.OnDataChanged();
			}
		}

		void worksheet_RangeDataChanged(object sender, Events.RangeEventArgs e)
		{
			var range = e.Range;

			if (dataRange.IntersectWith(range) || range.Contains(this.LabelAddress))
			{
				this.dataSource.OnDataChanged();
			}
		}
		#endregion // Events

		/// <summary>
		/// Get label text of serial.
		/// </summary>
		public string Label
		{
			get { return this.worksheet == null || this.LabelAddress.IsEmpty ? string.Empty
					: this.worksheet.GetCellText(this.LabelAddress); }
		}

		/// <summary>
		/// Get number of data items of current serial.
		/// </summary>
		public int Count
		{
			get { return dataRange.Cols; }
		}

		/// <summary>
		/// Get data from serial by specified index position.
		/// </summary>
		/// <param name="index">Zero-based index position in serial to get data.</param>
		/// <returns>Data in double type to be get from specified index of serial.
		/// If index is out of range, or data in worksheet is null, then return null.
		/// </returns>
		public double? this[int index]
		{
			get
			{
				object data;

				if (this.dataRange.Rows > this.dataRange.Cols)
				{
					data = worksheet.GetCellData(this.dataRange.Row + index, this.dataRange.Col);
				}
				else
				{
					data = worksheet.GetCellData(this.dataRange.Row, this.dataRange.Col + index);
				}

				if (unvell.ReoGrid.Utility.CellUtility.TryGetNumberData(data, out var val))
				{
					return val;
				}
				else
				{
					return null;
				}
			}
		}
	}
	#endregion // WorksheetChartDataSerial

}

#endif // DRAWING