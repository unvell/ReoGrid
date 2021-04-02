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

namespace unvell.ReoGrid
{
	#region ReoGridException

	/// <summary>
	/// Common exception of ReoGrid component
	/// </summary>
	[Serializable]
	public class ReoGridException : Exception
	{
		/// <summary>
		/// Create common exception with specified exception message
		/// </summary>
		/// <param name="msg">message used to describe the exception</param>
		public ReoGridException(string msg) : base(msg) { }

		/// <summary>
		/// Create common exception with specified message and inner exception
		/// </summary>
		/// <param name="msg">message used to describe the exception</param>
		/// <param name="innerException">inner exception</param>
		public ReoGridException(string msg, Exception innerException) : base(msg, innerException) { }
	}

	/// <summary>
	/// Exception is thrown when any errors happened during loading spreadsheet from a stream.
	/// </summary>
	[Serializable]
	public class WorkbookLoadException : ReoGridException
	{
		/// <summary>
		/// Create exception instance
		/// </summary>
		/// <param name="msg">Additional message to describe the exception</param>
		public WorkbookLoadException(string msg) : base(msg) { }

		/// <summary>
		/// Create exception instance
		/// </summary>
		/// <param name="msg">Addtional message to describe the exception</param>
		/// <param name="ex">Addtional original exception</param>
		public WorkbookLoadException(string msg, Exception ex) : base(msg, ex) { }
	}

	#endregion

	#region Workbook Exception

	/// <summary>
	/// Common workbook exception
	/// </summary>
	[Serializable]
	public class WorkbookException : ReoGridException
	{
		/// <summary>
		/// Create common workbook exception with specified message
		/// </summary>
		/// <param name="msg">message to describe this exception</param>
		public WorkbookException(string msg) : base(msg) { }

		/// <summary>
		/// Create common workbook exception with specified message
		/// </summary>
		/// <param name="msg">message to describe this exception</param>
		/// <param name="ex">inner exception</param>
		public WorkbookException(string msg, Exception ex) : base(msg, ex) { }
	}

	#endregion

	#region Worksheet Exceptions

	/// <summary>
	/// Common worksheet exception
	/// </summary>
	[Serializable]
	public class WorksheetException : ReoGridException
	{
		/// <summary>
		/// worksheet of exception
		/// </summary>
		public Worksheet Worksheet { get; set; }

		/// <summary>
		/// Create this exception with specified message
		/// </summary>
		/// <param name="msg">additional message to describe this exception</param>
		public WorksheetException(string msg) : base(msg) { }

		/// <summary>
		/// Create this exception with specified worksheet and message
		/// </summary>
		/// <param name="Worksheet">worksheet where exception happened</param>
		/// <param name="msg">additional message to describe this exception</param>
		public WorksheetException(Worksheet Worksheet, string msg)
			: base(msg)
		{
			this.Worksheet = Worksheet;
		}
	}

	/// <summary>
	/// Exception is thrown when current viewport controller does not support the freeze functionality.
	/// </summary>
	[Serializable]
	public class FreezeUnsupportedException : ReoGridException
	{
		public FreezeUnsupportedException() : base("Current view model is not compatible with freeze panel.") { }
	}

	/// <summary>
	/// Excepiton is thrown when an address is passed into a method which is described in the incorrect format, 
	/// or the address is out of valid range of spreadsheet.
	/// </summary>
	[Serializable]
	public class InvalidAddressException : ReoGridException
	{
		public InvalidAddressException(string addr) : base("unrecognized identifier to locate cell or range: " + addr) { }
	}

	/// <summary>
	/// Exception is thrown when an reference as string neither is an valid address nor a registered name to locate a range.
	/// </summary>
	[Serializable]
	public class InvalidReferenceException : ReoGridException
	{
		public InvalidReferenceException(string arg) : base("Reference from an address or name is invalid: " + arg) { }
	}

	/// <summary>
	/// Exception will be thrown when a reference object lost its owner object.
	/// </summary>
	[Serializable]
	public class ReferenceObjectNotAssociatedException : ReoGridException
	{
		/// <summary>
		/// Construct this exception with specified message
		/// </summary>
		/// <param name="msg">additional message to describe this exception</param>
		public ReferenceObjectNotAssociatedException(string msg) : base(msg) { }
	}

	/// <summary>
	/// Exception thrown when an invalid name specified for worksheet
	/// </summary>
	[Serializable]
	public class InvalidWorksheetNameException : WorksheetException
	{
		/// <summary>
		/// Create this exception with specified worksheet and message
		/// </summary>
		/// <param name="worksheet">worksheet where exception happened</param>
		/// <param name="msg">additional message to describe this exception</param>
		public InvalidWorksheetNameException(Worksheet worksheet, string msg) : base(worksheet, msg) { }
	}

	/// <summary>
	/// Exception caused when specified worksheet cannot be found
	/// </summary>
	[Serializable]
	public class WorksheetNotFoundException : ReoGridException
	{
		/// <summary>
		/// Create exception with additional message.
		/// </summary>
		/// <param name="msg">Additional message to describe this exception.</param>
		public WorksheetNotFoundException(string msg) : base(msg) { }
	}

	#endregion // Worksheet Exceptions

	#region Cell Exceptions

	/// <summary>
	/// Cell operations exception 
	/// </summary>
	[Serializable]
	public abstract class ReoGridCellException : Exception
	{
		/// <summary>
		/// Position of the cell where the exception happens
		/// </summary>
		public CellPosition Pos { get; set; }

		/// <summary>
		/// Create cell operations exception with specified cell position information
		/// </summary>
		/// <param name="pos">cell position information</param>
		public ReoGridCellException(CellPosition pos) { this.Pos = pos; }
	}

	#endregion // Cell Exceptions

	#region Range Exceptions
	/// <summary>
	/// Common range exception
	/// </summary>
	[Serializable]
	public class RangeException : ReoGridException
	{
		/// <summary>
		/// Get or set the range that is caused this exception
		/// </summary>
		public RangePosition Range { get; set; }

		/// <summary>
		/// Create this exception by given range that is caused this exception happen
		/// </summary>
		/// <param name="range">Target range</param>
		public RangeException(RangePosition range) : this(range, null) { }

		/// <summary>
		/// Create this exception by given range and additional message
		/// </summary>
		/// <param name="msg">Additional message used to describe this exception</param>
		/// <param name="range">The range is caused this exception happen</param>
		public RangeException(RangePosition range, string msg)
			: base(msg)
		{
			this.Range = range;
		}
	}

	/// <summary>
	/// The range to be processed intersected with another merged cell will
	/// cause this exception happen. Often caused when attempt to merged/move/change
	/// a range that contains any cells belonging to some merged cell.
	/// </summary>
	[Serializable]
	public class RangeIntersectionException : RangeException
	{
		/// <summary>
		/// Create instance of this exception with given range.
		/// </summary>
		/// <param name="range">Intersected another range</param>
		public RangeIntersectionException(RangePosition range) : base(range) { }

		/// <summary>
		/// Create instance of this exception with given range and additional message.
		/// </summary>
		/// <param name="range">Intersected another range.</param>
		public RangeIntersectionException(RangePosition range, string msg) : base(range, msg) { }
	}

	/// <summary>
	/// Event thrown when range is invalid to do specified operations
	/// </summary>
	[Serializable]
	public class InvalidRangeException : RangeException
	{
		/// <summary>
		/// Create instance of this exception with given range
		/// </summary>
		/// <param name="range">Target range to do specified operations</param>
		public InvalidRangeException(RangePosition range) : base(range) { }
	}

	/// <summary>
	/// Event thrown when range is too small to do specified operations
	/// </summary>
	[Serializable]
	public class RangeTooSmallException : RangeException
	{
		/// <summary>
		/// Create instance of this exception with given range
		/// </summary>
		/// <param name="range">Target range to do specified operations</param>
		public RangeTooSmallException(RangePosition range) : base(range) { }
	}

	/// <summary>
	/// This exception will be thrown when a reference range lost its reference 
	/// to the instance of grid control.
	/// 
	/// ReferenceRange should be always created from grid control.
	/// If the grid control it associated has been disposed, the ReferenceRanges 
	/// should be also disposed and created again from the grid.
	/// </summary>
	[Serializable]
	public class ReferenceRangeNotAssociatedException : RangeException
	{
		/// <summary>
		/// Create exception instance with specified worksheet range.
		/// </summary>
		/// <param name="range">Range information.</param>
		public ReferenceRangeNotAssociatedException(RangePosition range) : base(range) { }
	}

	/// <summary>
	/// Event thrown when attempt to reference a non-existed named range
	/// </summary>
	[Serializable]
	public class NamedRangeNotFoundException : ReoGridException
	{
		/// <summary>
		/// Get or set the name of range
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Create instance of this exception with additional message
		/// </summary>
		/// <param name="msg">Additional message used to describe this exception</param>
		public NamedRangeNotFoundException(string msg) : base(msg) { }
	}

	/// <summary>
	/// Event thrown when attempt to define a named range with existed name 
	/// </summary>
	[Serializable]
	public class NamedRangeAlreadyDefinedException : ReoGridException
	{
		/// <summary>
		/// Create instance of this exception
		/// </summary>
		public NamedRangeAlreadyDefinedException() :
			this("Another range defined with same name does already exist.") { }

		/// <summary>
		/// Create instance of this exception with additional message
		/// </summary>
		/// <param name="name">Name for range</param>
		public NamedRangeAlreadyDefinedException(string name) : base("Identifier for named range is used: " + name) { }
	}

	/// <summary>
	/// Event thrown when attempt to define named range with an invalid name
	/// </summary>
	[Serializable]
	public class InvalidNamedRangeException : ReoGridException
	{
		/// <summary>
		/// Get or set the name of range
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Create this exception with given invalid name
		/// </summary>
		/// <param name="name">The name used to define named range</param>
		public InvalidNamedRangeException(string name)
			: base("Name is invalid for define named range: " + name)
		{
			this.Name = name;
		}
	}

	#endregion // RangeException

	#region Script Exceptions
#if FORMULA
	/// <summary>
	/// This exception caused when a formula has a reference that causes a circular reference problem
	/// </summary>
	[Serializable]
	public class CircularReferenceException : ReoGridException
	{
		/// <summary>
		/// Create the instance of this exception
		/// </summary>
		public CircularReferenceException()
			: base("The referenced cell of a refereced range is contained in the range itself.")
		{
		}
	}
#endif // FORMULA
	#endregion // ScriptException

	#region Outline Exceptions
	/// <summary>
	/// Exception raised when any exception happen during outline operations
	/// </summary>
	[Serializable]
	public class OutlineException : ReoGridException
	{
		/// <summary>
		/// Zero-based start position of row or column in the outline
		/// </summary>
		public int Start { get; set; }

		/// <summary>
		/// Number of rows or columns in the outline
		/// </summary>
		public int Count { get; set; }

		/// <summary>
		/// Zero-based end position of row or column in the outline
		/// </summary>
		public int End { get { return Start + Count; } }

		/// <summary>
		/// Create the instance of exception by additional message
		/// </summary>
		/// <param name="msg">Additional message to describe this exception</param>
		public OutlineException(string msg) : base(msg) { }
	}

	/// <summary>
	/// Exception thrown when attempt to create an outline that is intersected with other existed outlines. 
	/// The outlines cannot be intersected each other, they can only be either contained by each other, 
	/// or be added at different positions.
	/// </summary>
	[Serializable]
	public class OutlineIntersectedException : OutlineException
	{
		/// <summary>
		/// Create the instance of exception
		/// </summary>
		public OutlineIntersectedException()
			: base("Specified outline intersects with another one. To group rows make sure that range of outline containing other outlines completely.")
		{
		}
	}

	/// <summary>
	/// Exception thrown when adding an outline at specified position, but another outline has already exist at same position.
	/// </summary>
	[Serializable]
	public class OutlineAlreadyDefinedException : ReoGridException
	{
		/// <summary>
		/// Create the instance of exception
		/// </summary>
		public OutlineAlreadyDefinedException()
			: base("Specified outline already exists.") { }
	}

	/// <summary>
	/// Exception thrown when performing an action to an outline but the outline cannot be found at specified position.
	/// </summary>
	[Serializable]
	public class OutlineNotFoundException : OutlineException
	{
		/// <summary>
		/// Create the instance of exception
		/// </summary>
		/// <param name="start"></param>
		/// <param name="msg"></param>
		public OutlineNotFoundException(int start, string msg) : base(msg) { }
	}

	/// <summary>
	/// Exception thrown when adding an outline at specified position, but the position is out of available range of worksheet.
	/// </summary>
	[Serializable]
	public class OutlineOutOfRangeException : OutlineException
	{
		/// <summary>
		/// Create instance of exception.
		/// </summary>
		/// <param name="start">Start position of outline to be added.</param>
		/// <param name="count">Number of rows or columns of outline to be added.</param>
		/// <param name="msg">Additional message to describe this exception.</param>
		public OutlineOutOfRangeException(int start, int count, string msg)
			: base("Specified outline cannot be added because it is out of the maximum available range.") { }
	}

	/// <summary>
	/// This exception is thrown when there is already maximum available outlines added into
	/// grid control. ReoGrid supports maximum 9 outlines in either row or column direction.
	/// </summary>
	[Serializable]
	public class OutlineTooMuchException : OutlineException
	{
		/// <summary>
		/// Create the instance of exception
		/// </summary>
		public OutlineTooMuchException()
			: base("Number of outline levels reached the maximum levels 10.") { }
	}
	#endregion // OutlineIntersectedException

	#region Cell Exceptions

	/// <summary>
	/// Exception happens when operation applied on a range that contains read-only cells.
	/// </summary>
	[Serializable]
	public class OperationOnReadonlyCellException : ReoGridException
	{
		/// <summary>
		/// Create exception with additional message.
		/// </summary>
		/// <param name="msg">The additional message to describe this exception.</param>
		public OperationOnReadonlyCellException(string msg)
			: base(msg)
		{
		}
	}

	/// <summary>
	/// Exception will be thrown when attempt modify the data of cell which is set to readonly.
	/// </summary>
	[Serializable]
	public class CellDataReadonlyException : OperationOnReadonlyCellException
	{
		/// <summary>
		/// Get cell position. (zero-based number of row and column)
		/// </summary>
		public CellPosition CellPos { get; private set; }

		/// <summary>
		/// Create exception with specified cell position.
		/// </summary>
		/// <param name="pos">Cell position on worksheet.</param>
		public CellDataReadonlyException(CellPosition pos)
			: base("Attempt to modify the data of read-only cell.")
		{
			this.CellPos = pos;
		}
	}

	/// <summary>
	/// Exception happens when operation applied on a range that contains read-only cells.
	/// </summary>
	public class RangeContainsReadonlyCellsException : OperationOnReadonlyCellException
	{
		/// <summary>
		/// Range that operation applied on.
		/// </summary>
		public RangePosition Range { get; set; }

        /// <summary>
        /// Create exception with the default message.
        /// </summary>
        /// <param name="range">Range that operation applied on.</param>
        public RangeContainsReadonlyCellsException(RangePosition range)
            : this(range, "Operation cannot be performed since target range contains read-only cells.")
        {
        }

        /// <summary>
        /// Create exception with additional message.
        /// </summary>
        /// <param name="range">Range that operation applied on.</param>
        /// <param name="msg">The additional message to describe this exception.</param>
        public RangeContainsReadonlyCellsException(RangePosition range, string msg)
			: base(msg)
		{
		    Range = range;
		}
    }

	/// <summary>
	/// Exception will be thrown when the cell body cannot be created for a cell automatically.
	/// </summary>
	[Serializable]
	public class CannotCreateCellBodyException : ReoGridException
	{
		/// <summary>
		/// Create exception with additional message.
		/// </summary>
		/// <param name="msg">Additional message to describe this exception.</param>
		public CannotCreateCellBodyException(string msg)
			: base(msg)
		{
		}

		/// <summary>
		/// Create exception with additional message and inner exception.
		/// </summary>
		/// <param name="msg">Additional message to describe this exception.</param>
		/// <param name="inner">Inner exception that happens in dependency method calls.</param>
		public CannotCreateCellBodyException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}
	#endregion // Cell Exceptions

	#region Print Exceptions
	/// <summary>
	/// Common print exception
	/// </summary>
	[Serializable]
	public class ReoGridPrintException : ReoGridException
	{
		/// <summary>
		/// Create common print exception with specified message
		/// </summary>
		/// <param name="msg">message of exception</param>
		public ReoGridPrintException(string msg) : this(msg, null)
		{
		}

		/// <summary>
		/// Create common print exception with specified message and previous exception
		/// </summary>
		/// <param name="msg">message of exception</param>
		/// <param name="innerEx">previous or inner exception</param>
		public ReoGridPrintException(string msg, Exception innerEx) :
			base(msg, innerEx)
		{
		}
	}

	/// <summary>
	/// Exception cause when print command received, but there is no valid content
	/// to be printed out from the spreadsheet. Check whether the PrintRange set to
	/// a region where not contain any valid cells if this exception happened.
	/// </summary>
	[Serializable]
	public class NoPrintableContentException : ReoGridPrintException
	{
		/// <summary>
		/// Create no printable content exception
		/// </summary>
		public NoPrintableContentException() :
			this("No content was contained in the spreadsheet could be printed.")
		{
		}

		/// <summary>
		/// Create no printable cotent exception with a message
		/// </summary>
		/// <param name="msg">message of exception</param>
		public NoPrintableContentException(string msg) : base(msg)
		{
		}
	}

	/// <summary>
	/// Exception cause when specified page break cannot be found
	/// </summary>
	[Serializable]
	public class PageBreakNotFoundException : ReoGridPrintException
	{
		/// <summary>
		/// Zero-based number of index to find specified page break
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// Create excpetion with specified number of page break
		/// </summary>
		/// <param name="index">zero-based number of page break that cannot be found</param>
		public PageBreakNotFoundException(int index)
			: this("Page break at index " + index + " was not found", index)
		{
		}
		
		/// <summary>
		/// Create exception with specified message and target index
		/// </summary>
		/// <param name="msg">message of exception</param>
		/// <param name="index">zero-based number of page break that cannot be found</param>
		public PageBreakNotFoundException(string msg, int index)
			: base(msg)
		{
			this.Index = index;
		}
	}

	/// <summary>
	/// Exception for page-break index cannot be removed
	/// </summary>
	[Serializable]
	public class PageBreakCannotRemoveException : ReoGridPrintException
	{
		/// <summary>
		/// Get or set page-break index
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// Create exception instance
		/// </summary>
		/// <param name="index">Number of page-break index</param>
		public PageBreakCannotRemoveException(int index)
			: this("Page break at index " + index 
				+ " cannot be removed, spreadsheet must be have at least two page breaks in order to decide the printable range.", index)
		{
		}

		/// <summary>
		/// Create exception instance
		/// </summary>
		/// <param name="msg">Addtional message to describe the message</param>
		/// <param name="index">Page-break index to be removed</param>
		public PageBreakCannotRemoveException(string msg, int index)
			: base(msg)
		{
			this.Index = index;
		}
	}
	
	#endregion // Print Exceptions

	#region IO Exceptions
	/// <summary>
	/// ReoGrid common I/O exception
	/// </summary>
	[Serializable]
	public class ReoGridIOException : ReoGridException
	{
		/// <summary>
		/// Create exception instance by specified message
		/// </summary>
		/// <param name="msg">Additional message to describe this exception</param>
		public ReoGridIOException(string msg) : this(msg, null)
		{
		}

		/// <summary>
		/// Create exception instance by specified message and original exception object
		/// </summary>
		/// <param name="msg">Additional message to describe this exception</param>
		/// <param name="innerEx">Original exception happened inside loading process</param>
		public ReoGridIOException(string msg, Exception innerEx) :
			base(msg, innerEx)
		{
		}
	}

	/// <summary>
	/// Loading exception
	/// </summary>
	[Serializable]
	public class ReoGridLoadException : ReoGridIOException
	{
		/// <summary>
		/// Create this exception with specified message
		/// </summary>
		/// <param name="msg">Addtional message to describe the exception</param>
		public ReoGridLoadException(string msg) : this(msg, null)
		{
		}

		/// <summary>
		/// Create this exception with specified message
		/// </summary>
		/// <param name="msg">Addtional message to describe the exception</param>
		/// <param name="innerEx">Inner exception where original exception happened</param>
		public ReoGridLoadException(string msg, Exception innerEx) :
			base(msg, innerEx)
		{
		}
	}

	/// <summary>
	/// File format not supported exception
	/// </summary>
	[Serializable]
	public class FileFormatNotSupportException : ReoGridException
	{
		/// <summary>
		/// Create exception instance
		/// </summary>
		public FileFormatNotSupportException()
			: this(null)
		{
		}
		
		/// <summary>
		/// Create exception instance by specified message
		/// </summary>
		/// <param name="msg">Additional message to describe this exception</param>
		public FileFormatNotSupportException(string msg) : this(msg, null)
		{
		}

		/// <summary>
		/// Create exception instance by specified message
		/// </summary>
		/// <param name="msg">Additional message to describe this exception</param>
		/// <param name="innerEx">Inner exception where original exception happened</param>
		public FileFormatNotSupportException(string msg, Exception innerEx) :
			base(msg, innerEx)
		{
		}
	}
	#endregion // IO Exceptions
}
