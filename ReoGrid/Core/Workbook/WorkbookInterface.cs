using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using unvell.ReoGrid.Events;
using unvell.ReoGrid.IO;

#if WINFORM || WPF
using ControlType = unvell.ReoGrid.ReoGridControl;
#elif ANDROID
using ControlType = unvell.ReoGrid.ReoGridView;
#endif // ANDROID

namespace unvell.ReoGrid
{
	/// <summary>
	/// Represents an interface of workbook instance
	/// </summary>
	public interface IWorkbook
#if WINFORM || WPF
		: IDisposable
#endif // WINFORM || WPF
	{
		#region Save & Load

		/// <summary>
		/// Save workbook into file.
		/// </summary>
		/// <param name="path">File path to create file and save workbook data.</param>
		void Save(string path);

		/// <summary>
		/// Save workbook into file.
		/// </summary>
		/// <param name="path">File path to create file and save workbook data.</param>
		/// <param name="fileFormat">Specified file format used to write data of workbook.</param>
		void Save(string path, IO.FileFormat fileFormat);

		/// <summary>
		/// Save workbook into file.
		/// </summary>
		/// <param name="path">File path to create file and save workbook data.</param>
		/// <param name="fileFormat">Specified file format used to write data of workbook.</param>
		/// <param name="encoding">Encoding used to write plain-text format file data. (Optional)</param>
		void Save(string path, IO.FileFormat fileFormat, Encoding encoding);

		/// <summary>
		/// Save workbook into stream.
		/// </summary>
		/// <param name="stream">Output stream to write data of workbook.</param>
		/// <param name="fileFormat">Specifies the file format used to write data of workbook.</param>
		void Save(System.IO.Stream stream, unvell.ReoGrid.IO.FileFormat fileFormat);

		/// <summary>
		/// Save workbook into stream.
		/// </summary>
		/// <param name="stream">Output stream to write data of workbook.</param>
		/// <param name="fileFormat">Specifies the file format used to write data of workbook.</param>
		/// <param name="encoding">Encoding used to write plain-text from resource. (Optional)</param>
		void Save(System.IO.Stream stream, unvell.ReoGrid.IO.FileFormat fileFormat, Encoding encoding);

		/// <summary>
		/// Load workbook from file by specified path.
		/// </summary>
		/// <param name="path">Path to locate the file and read workbook data from the file.</param>
		void Load(string path);

		/// <summary>
		/// Load workbook from file by specified path.
		/// </summary>
		/// <param name="path">Path to locate the file and read workbook data from the file.</param>
		/// <param name="fileFormat">Specified file format used to read data of workbook.</param>
		void Load(string path, IO.FileFormat fileFormat);

		/// <summary>
		/// Load workbook from file by specified path.
		/// </summary>
		/// <param name="path">Path to locate the file and read workbook data from the file.</param>
		/// <param name="fileFormat">Specified file format used to read data of workbook.</param>
		/// <param name="encoding">Encoding used to read plain-text format workbook data from stream. (Optional)</param>
		void Load(string path, IO.FileFormat fileFormat, Encoding encoding);

		/// <summary>
		/// Load workbook from specified stream.
		/// </summary>
		/// <param name="stream">Input stream to read data of workbook.</param>
		/// <param name="fileFormat">Specified file format used to read workbook data from stream.</param>
		void Load(System.IO.Stream stream, unvell.ReoGrid.IO.FileFormat fileFormat);

		/// <summary>
		/// Load workbook from specified stream.
		/// </summary>
		/// <param name="stream">Input stream to read data of workbook.</param>
		/// <param name="fileFormat">Specified file format used to read workbook data from stream.</param>
		/// <param name="encoding">Encoding used to read plain-text format workbook data from stream. (Optional)</param>
		void Load(System.IO.Stream stream, unvell.ReoGrid.IO.FileFormat fileFormat, Encoding encoding);

		/// <summary>
		/// Event raised when workbook loaded from stream or file.
		/// </summary>
		event EventHandler WorkbookLoaded;

		/// <summary>
		/// Event raised when workbook saved into stream or file.
		/// </summary>
		event EventHandler WorkbookSaved;

		#endregion // Save & Load

		#region Worksheet Management

		/// <summary>
		/// Create a new worksheet
		/// </summary>
		/// <param name="name">Name for the worksheet (null or empty name to use default sheet name)</param>
		/// <returns>Instance of worksheet has been created</returns>
		Worksheet CreateWorksheet(string name = null);

		/// <summary>
		/// Insert worksheet into workbook before specified index
		/// </summary>
		/// <param name="index">Zero-based number of workbook to insert specified worksheet</param>
		/// <param name="sheet">Worksheet to be inserted</param>
		void InsertWorksheet(int index, Worksheet sheet);

		/// <summary>
		/// Add worksheet into workbook
		/// </summary>
		/// <param name="sheet">Worksheet to be added</param>
		void AddWorksheet(Worksheet sheet);

		/// <summary>
		/// Remove specified worksheet from workbook by specified index
		/// </summary>
		/// <param name="index">Zero-based number of worksheet to be removed</param>
		/// <returns>True if specified worksheet can be found, and removed successfully</returns>
		bool RemoveWorksheet(int index);

		/// <summary>
		/// Remove specified worksheet from workbook
		/// </summary>
		/// <param name="sheet">Worksheet to be removed</param>
		/// <returns>True if worksheet is removed from this workbook successfully</returns>
		bool RemoveWorksheet(Worksheet sheet);

		/// <summary>
		/// Get the index of specified worksheet from workbook.
		/// </summary>
		/// <param name="sheet">Worksheet to get.</param>
		/// <returns>Zero-based number of worksheet in worksheet collection of workbook.</returns>
		int GetWorksheetIndex(Worksheet sheet);

		/// <summary>
		/// Get the index of specified worksheet by name from workbook.
		/// </summary>
		/// <param name="sheet">Worksheet to get.</param>
		/// <returns>Zero-based number of worksheet in worksheet collection of workbook.</returns>
		int GetWorksheetIndex(string name);

		/// <summary>
		/// Find worksheet by specified name
		/// </summary>
		/// <param name="name">Name to find worksheet.</param>
		/// <returns>Instance of worksheet that is found by specified name; otherwise return null</returns>
		Worksheet GetWorksheetByName(string name);

		/// <summary>
		/// Create a cloned worksheet and put into specified position.
		/// </summary>
		/// <param name="index">Index of source worksheet to be copied</param>
		/// <param name="newIndex">Target index used to insert the copied worksheet</param>
		/// <param name="newName">Name for new worksheet, set as null to use a default worksheet name e.g. Sheet1, Sheet2...</param>
		/// <returns>New instance of copid worksheet</returns>
		Worksheet CopyWorksheet(int index, int newIndex, string newName = null);

		/// <summary>
		/// Create a cloned worksheet and put into specified position.
		/// </summary>
		/// <param name="sheet">Source worksheet to be copied, the worksheet must be already added into this workbook</param>
		/// <param name="newIndex">Target index used to insert the copied worksheet</param>
		/// <param name="newName">Name for new worksheet, set as null to use a default worksheet name e.g. Sheet1, Sheet2...</param>
		/// <returns>New instance of copid worksheet</returns>
		Worksheet CopyWorksheet(Worksheet sheet, int newIndex, string newName = null);

		/// <summary>
		/// Move worksheet from a position to another position
		/// </summary>
		/// <param name="index">Worksheet in this position to be moved</param>
		/// <param name="newIndex">Target position moved to</param>
		/// <returns>Instance of moved worksheet</returns>
		Worksheet MoveWorksheet(int index, int newIndex);

		/// <summary>
		/// Create a cloned worksheet and put into specified position
		/// </summary>
		/// <param name="sheet">Instance of worksheet to be moved, the worksheet must be already added into this workbook</param>
		/// <param name="newIndex">Target position moved to</param>
		/// <returns>Instance of moved worksheet</returns>
		Worksheet MoveWorksheet(Worksheet sheet, int newIndex);

		/// <summary>
		/// Get the collection of worksheets
		/// </summary>
		WorksheetCollection Worksheets { get; }

		/// <summary>
		/// Determine whether or not this workbook is read-only
		/// </summary>
		bool Readonly { get; set; }

		/// <summary>
		/// Reset this workbook (Clear all worksheets and put one new)
		/// </summary>
		void Reset();

		/// <summary>
		/// Event raised when worksheet is created.
		/// </summary>
		event EventHandler<WorksheetCreatedEventArgs> WorksheetCreated;

		/// <summary>
		/// Event raised when worksheet is inserted into this workbook.
		/// </summary>
		event EventHandler<WorksheetInsertedEventArgs> WorksheetInserted;

		/// <summary>
		/// Event raised when worksheet is removed from this workbook.
		/// </summary>
		event EventHandler<WorksheetRemovedEventArgs> WorksheetRemoved;

		/// <summary>
		/// Event raised when name of worksheet is changed.
		/// </summary>
		event EventHandler<WorksheetNameChangingEventArgs> WorksheetNameChanged;

		/// <summary>
		/// Event raised when background color of worksheet name is changed.
		/// </summary>
		event EventHandler<WorksheetEventArgs> WorksheetNameBackColorChanged;

		/// <summary>
		/// Event raised when text color of worksheet name is changed.
		/// </summary>
		event EventHandler<WorksheetEventArgs> WorksheetNameTextColorChanged;

		#endregion // Worksheet Management

		#region Internal Exceptions
		/// <summary>
		/// Event raised when exception has been happened during internal processed.
		/// Usually these internal processes are raised by hot-keys pressed by user.
		/// </summary>
		event EventHandler<ExceptionHappenEventArgs> ExceptionHappened;

		/// <summary>
		/// Notify that there are exceptions happen on any worksheet. 
		/// The event ExceptionHappened of workbook will be invoked.
		/// </summary>
		/// <param name="sheet">Worksheet where the exception happened</param>
		/// <param name="ex">Exception to describe the details of error information</param>
		void NotifyExceptionHappen(Worksheet sheet, Exception ex);
		#endregion // Internal Exceptions
	}

	//internal interface IScreenWorkbook : IWorkbook
	//{
	//	#region Control Relative
	//	/// <summary>
	//	/// Retrieve the control instance from this workbook. (Memory workbook doesn't have instance)
	//	/// </summary>
	//	ControlType ControlInstance { get; }

	//	/// <summary>
	//	/// Get or set the control appearance style for entire workbook.
	//	/// </summary>
	//	ControlAppearanceStyle ControlStyle { get; set; }
	//	#endregion // Control Relative
	//}
}
