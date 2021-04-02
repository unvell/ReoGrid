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
using System.Text;

namespace unvell.ReoGrid.Data
{
	/// <summary>
	/// Represents common data source.
	/// </summary>
	public interface IDataSource<T> //: IEnumerable<T> 
		where T : IDataSerial
	{
		/// <summary>
		/// Get the number of available data serials of this source.
		/// </summary>
		int SerialCount { get; }

		///// <summary>
		///// Get the number of available categories from this source.
		///// </summary>
		//int CategoryCount { get; }

		///// <summary>
		///// Get data by specified category and data serial.
		///// </summary>
		///// <param name="serial">Number of column to get data.</param>
		///// <param name="category">Number of category to get data.</param>
		///// <returns>Data fetched from specified two-dimensional position. Null might be returned from this method.</returns>
		//double? GetData(int serial, int category);
		/// <summary>
		/// Get data serial from specified position.
		/// </summary>
		/// <param name="index">Zero-based index to get data serial from this source.</param>
		/// <returns>Data serial object. Return null if given index is out of valid range.</returns>
		T this[int index] { get; }

		void Add(T serial);
		
		/// <summary>
		/// This event will be raised when any data from the serial data range changed.
		/// </summary>
		event EventHandler DataChanged;

		///// <summary>
		///// This event will be raised when serial data range changed.
		///// </summary>
		//event EventHandler DataRangeChanged;
	}

	/// <summary>
	/// Represents data serial in data source.
	/// </summary>
	public interface IDataSerial
	{
		/// <summary>
		/// Get the number of items of this data serial.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Get data from specified position.
		/// </summary>
		/// <param name="index">Zero-based index position to get data from current serial.</param>
		/// <returns>Data in double. Return null if data cannot be converted to double or data not exist.</returns>
		double? this[int index] { get; }
	}
}
