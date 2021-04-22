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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using unvell.Common;
using unvell.ReoGrid.Events;
using unvell.ReoGrid.IO;
using unvell.ReoGrid.Main;
using unvell.ReoGrid.Interaction;

#if PRINT
using unvell.ReoGrid.Print;
#endif // PRINT

#if WINFORM || ANDROID
using RGFloat = System.Single;
#elif WPF
using RGFloat = System.Double;
#elif iOS
using RGFloat = System.Double;

#endif // WPF

#if WINFORM || WPF
using ReoGridControl = unvell.ReoGrid.ReoGridControl;
#elif ANDROID
using ReoGridControl = unvell.ReoGrid.ReoGridView;

#elif iOS
using ReoGridControl = unvell.ReoGrid.ReoGridView;

#endif // ANDROID

namespace unvell.ReoGrid
{
	internal partial class Workbook : IWorkbook
#if (WINFORM || WPF) && PRINT
		, IPrintableContainer
#endif // (WINFORM || WPF) && PRINT
	{
		internal List<Worksheet> worksheets = new List<Worksheet>();

		internal IControlAdapter controlAdapter;

		public ReoGridControl ControlInstance { get { return (ReoGridControl)this.controlAdapter.ControlInstance; } }

		#region Readonly
		private bool isReadonly = false;

		public bool Readonly
		{
			get
			{
				return isReadonly;
			}
			set
			{
				isReadonly = value;

				foreach (var sheet in this.worksheets)
				{
					sheet.SetSettings(WorksheetSettings.Edit_Readonly, value);
				}
			}
		}
		#endregion // Readonly

		/// <summary>
		/// Create workbook instance
		/// </summary>
		/// <param name="adapter">Control instance adapter</param>
		public Workbook(IControlAdapter adapter)
		{

#if DEBUG
			Stopwatch sw = Stopwatch.StartNew();
			Debug.WriteLine("start creating workbook...");
#endif // DEBUG

			if (adapter != null)
			{
				this.controlAdapter = adapter;

				this.AttachSheetTabControl(this.controlAdapter.SheetTabControl);
			}

			// default control styles
			//SetControlStyle(ControlAppearanceStyle.DefaultControlStyle);

#if DEBUG
			sw.Stop();
			long ms = sw.ElapsedMilliseconds;
			Debug.WriteLine("creating workbook done: " + ms + " ms.");
#endif
		}

		static Workbook()
		{
			FileFormatProviders[FileFormat.ReoGridFormat] = (IFileFormatProvider)new ReoGridFileFormatProvider();
			FileFormatProviders[FileFormat.Excel2007] = (IFileFormatProvider)new ExcelFileFormatProvider();
			FileFormatProviders[FileFormat.CSV] = (IFileFormatProvider)new CSVFileFormatProvider();
		}

		/// <summary>
		/// Clear all worksheets.
		/// </summary>
		public void Clear()
		{
			this.ClearWorksheets();
		}

		#region Save & Load

		public static readonly Dictionary<FileFormat, IFileFormatProvider> FileFormatProviders =
			new Dictionary<FileFormat, IFileFormatProvider>();

		public void Save(string path)
		{
			this.Save(path, FileFormat._Auto);
		}

		public void Save(string path, IO.FileFormat fileFormat)
		{
			this.Save(path, fileFormat, Encoding.Default);
		}

		public void Save(string path, IO.FileFormat fileFormat, Encoding encoding)
		{
			if (fileFormat == IO.FileFormat._Auto)
			{
				foreach (var p in FileFormatProviders)
				{
					if (p.Value.IsValidFormat(path))
					{
						fileFormat = p.Key;
						break;
					}
				}

				if (fileFormat == FileFormat._Auto)
				{
					throw new NotSupportedException("Cannot determine a file format to load workbook from specified path, try specify the file format.");
				}
			}

			using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				Save(fs, fileFormat, encoding);
			}
		}

		public void Save(System.IO.Stream stream, IO.FileFormat fileFormat)
		{
			this.Save(stream, fileFormat, Encoding.Default);
		}

		public void Save(System.IO.Stream stream, IO.FileFormat fileFormat, Encoding encoding)
		{
			if (!FileFormatProviders.TryGetValue(fileFormat, out var provider))
			{
				throw new FileFormatNotSupportException("Specified file format is not supported");
			}

			if (this.controlAdapter != null)
			{
				this.controlAdapter.ChangeCursor(CursorStyle.Busy);
			}

			try
			{
				provider.Save(this, stream, encoding, null);
			}
			finally
			{
				if (this.controlAdapter != null)
				{
					this.controlAdapter.ChangeCursor(CursorStyle.PlatformDefault);
				}

				if (this.WorkbookSaved != null)
				{
					this.WorkbookSaved(this, null);
				}
			}
		}

		public void Load(string path)
		{
			this.Load(path, IO.FileFormat._Auto);
		}

		public void Load(string path, IO.FileFormat fileFormat)
		{
			this.Load(path, fileFormat, Encoding.Default);
		}

		public void Load(string path, IO.FileFormat fileFormat, Encoding encoding)
		{
			if (fileFormat == IO.FileFormat._Auto)
			{
				foreach (var p in FileFormatProviders)
				{
					if (p.Value.IsValidFormat(path))
					{
						fileFormat = p.Key;
						break;
					}
				}

				if (fileFormat == FileFormat._Auto)
				{
					throw new NotSupportedException("Cannot determine the file format to load workbook from specified path, try specify explicitly the file format by argument.");
				}
			}

			using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				this.Load(fs, fileFormat, encoding == null ? Encoding.Default : encoding);
			}

			// for csv only
			if (fileFormat == FileFormat.CSV)
			{
				if (this.worksheets.Count > 0)
				{
					this.worksheets[0].Name = Path.GetFileNameWithoutExtension(path);
				}
			}
		}

		public void Load(System.IO.Stream stream, IO.FileFormat fileFormat)
		{
			this.Load(stream, fileFormat, Encoding.Default);
		}

		public void Load(System.IO.Stream stream, IO.FileFormat fileFormat, Encoding encoding)
		{
			if (fileFormat == FileFormat._Auto)
			{
				throw new System.ArgumentException("File format 'Auto' is invalid for loading workbook from stream, try specify a file format.");
			}

			if (!FileFormatProviders.TryGetValue(fileFormat, out var provider))
			{
				throw new FileFormatNotSupportException("Specified file format is not supported.");
			}

			if (this.controlAdapter != null)
			{
				this.controlAdapter.ChangeCursor(CursorStyle.Busy);
			}

			if (encoding == null) encoding = Encoding.Default;

			try
			{
				provider.Load(this, stream, encoding, null);
			}
			finally
			{
				if (this.controlAdapter != null)
				{
					this.controlAdapter.ChangeCursor(CursorStyle.PlatformDefault);
				}

				this.WorkbookLoaded?.Invoke(this, null);
			}
		}

		/// <summary>
		/// Event raised when workbook loaded from stream or file
		/// </summary>
		public event EventHandler WorkbookLoaded;

		/// <summary>
		/// Event raised when workbook saved into stream or file
		/// </summary>
		public event EventHandler WorkbookSaved;

		#endregion // Save & Load

		#region Worksheet Management

		internal string GetAvailableWorksheetName()
		{
			string name;
			int index = 1;
			while (!this.CheckWorksheetName(name = (LanguageResource.Sheet + index))) index++;
			return name;
		}

		public Worksheet CreateWorksheet(string name = null)
		{
			if (string.IsNullOrEmpty(name))
			{
				name = GetAvailableWorksheetName();
			}
			else
			{
				this.ValidateWorksheetName(name);
			}

			var sheet = new Worksheet(this, name);

			this.WorksheetCreated?.Invoke(this, new WorksheetCreatedEventArgs(sheet));

			return sheet;
		}

		public void AddWorksheet(Worksheet sheet)
		{
			this.InsertWorksheet(this.worksheets.Count, sheet);
		}

		public void NewWorksheet(string name = null)
		{
			this.AddWorksheet(this.CreateWorksheet(name));
		}

		public void InsertWorksheet(int index, Worksheet sheet)
		{
			if (index < 0 || index > this.worksheets.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}

			if (sheet.Workbook != null && sheet.Workbook != this)
			{
				throw new WorkbookException("Specified worksheet belongs to another workbook, remove from another workbook firstly.");
			}

			this.ValidateWorksheetName(sheet.Name);

			this.worksheets.Insert(index, sheet);

			sheet.workbook = this;
			sheet.ControlAdapter = this.controlAdapter;

			// sheet management
			if (this.sheetTab != null)
			{
				this.sheetTab.InsertTab(index, sheet.Name);

				if (this.worksheets.Count > 0 
					&& this.sheetTab.SelectedIndex == -1 || this.sheetTab.SelectedIndex >= this.worksheets.Count)
				{
					this.sheetTab.SelectedIndex = 0;
				}
			}

			// update current worksheet
			if (this.controlAdapter != null && this.controlAdapter.ControlInstance.CurrentWorksheet == null)
			{
				this.controlAdapter.ControlInstance.CurrentWorksheet = sheet;
			}

			// event
			this.WorksheetInserted?.Invoke(this, new WorksheetInsertedEventArgs(sheet)
			{
				Index = index,
			});
		}

		public bool RemoveWorksheet(int index)
		{
			if (index < 0 || index >= this.worksheets.Count)
				throw new ArgumentOutOfRangeException("index");

			if (this.sheetTab != null)
			{
				this.sheetTab.RemoveTab(index);
			}

			var sheet = this.worksheets[index];
			sheet.workbook = null;

			this.worksheets.RemoveAt(index);

			this.WorksheetRemoved?.Invoke(this, new WorksheetRemovedEventArgs(sheet));

			return true;
		}

		public bool RemoveWorksheet(Worksheet sheet)
		{
			int index = this.worksheets.IndexOf(sheet);

			if (index < 0 || index >= this.worksheets.Count)
			{
				throw new WorksheetNotFoundException("Specified worksheet cannot be found.");
			}

			return RemoveWorksheet(index);
		}

		/// <summary>
		/// Duplicate worksheet and insert the new instance into specified position
		/// </summary>
		/// <param name="index">zero-based number of worksheet to be duplicated</param>
		/// <param name="newIndex">position used to insert duplicated new instance</param>
		/// <param name="newName">New name to be apply to copied worksheet</param>
		/// <returns>instance of duplicated worksheet from specified worksheet</returns>
		public Worksheet CopyWorksheet(int index, int newIndex, string newName = null)
		{
			if (newIndex < 0 || newIndex > this.worksheets.Count)
			{
				throw new ArgumentOutOfRangeException("newIndex");
			}

			return CopyWorksheet(this.worksheets[index], newIndex, newName);
		}

		/// <summary>
		/// Duplicate worksheet and insert the new instance into specified position
		/// </summary>
		/// <param name="sheet">worksheet to be duplicated. The worksheet passed here should be 
		/// already added into current workbook.</param>
		/// <param name="newIndex">position used to insert duplicated new instance</param>
		/// <param name="newName">New name to be apply</param>
		/// <returns>instance of duplicated worksheet from specified worksheet</returns>
		/// <exception cref="WorksheetNotFoundException">when specified worksheet does not belong to
		/// this workbook.</exception>
		/// <exception cref="ArgumentOutOfRangeException">when the position used to insert
		/// duplicated instace of worksheet is out of valid range of this workbook.</exception>
		public Worksheet CopyWorksheet(Worksheet sheet, int newIndex, string newName = null)
		{
			if (sheet.workbook != this)
			{
				throw new WorksheetNotFoundException("Specified worksheet does not belong to this workbook.");
			}

			if (newIndex < 0 || newIndex > this.worksheets.Count)
			{
				throw new ArgumentOutOfRangeException("newIndex");
			}

			var newSheet = sheet.Clone(newName);

			if (this.WorksheetCreated != null)
			{
				this.WorksheetCreated(this, new WorksheetCreatedEventArgs(newSheet));
			}

			InsertWorksheet(newIndex, newSheet);

			return newSheet;
		}


		/// <summary>
		/// Move worksheet from a position to another position
		/// </summary>
		/// <param name="index">Worksheet in this position to be moved</param>
		/// <param name="newIndex">Target position moved to</param>
		/// <returns>Instance of moved worksheet</returns>
		public Worksheet MoveWorksheet(int index, int newIndex)
		{
			if (index < 0 || index > this.worksheets.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}

			if (newIndex < 0 || newIndex > this.worksheets.Count)
			{
				throw new ArgumentOutOfRangeException("newIndex");
			}

			var sheet = this.worksheets[index];

			if (index == newIndex) return sheet;

			this.sheetTab.RemoveTab(index);
			this.worksheets.RemoveAt(index);

			//if (newIndex > index) newIndex--;

			this.worksheets.Insert(newIndex, sheet);

			// sheet management
			this.sheetTab.InsertTab(newIndex, sheet.Name);

			return sheet;
		}

		/// <summary>
		/// Create a cloned worksheet and put into specified position
		/// </summary>
		/// <param name="sheet">Instance of worksheet to be moved, the worksheet must be already added into this workbook</param>
		/// <param name="newIndex">Target position moved to</param>
		/// <returns>New instance of copid worksheet</returns>
		public Worksheet MoveWorksheet(Worksheet sheet, int newIndex)
		{
			if (sheet.workbook != this)
			{
				throw new WorksheetNotFoundException("Specified worksheet does not belong to this workbook.");
			}

			int index = GetWorksheetIndex(sheet);

			return MoveWorksheet(index, newIndex);
		}

		/// <summary>
		/// Get index of specified worksheet from the collection in this workbook
		/// </summary>
		/// <param name="sheet">worksheet to be get</param>
		/// <returns>zero-based number of worksheet in this workbook's collection</returns>
		public int GetWorksheetIndex(Worksheet sheet)
		{
			return this.worksheets.IndexOf(sheet);
		}

		/// <summary>
		/// Get the index of specified worksheet by name from workbook.
		/// </summary>
		/// <param name="sheet">Worksheet to get.</param>
		/// <returns>Zero-based number of worksheet in worksheet collection of workbook.</returns>
		public int GetWorksheetIndex(string name)
		{
			var sheet = this.GetWorksheetByName(name);
			return sheet == null ? -1 : this.GetWorksheetIndex(sheet);
		}

		/// <summary>
		/// Find worksheet by specified name
		/// </summary>
		/// <param name="name">Name to find worksheet</param>
		/// <returns>Instance of worksheet that is found by specified name; otherwise return null</returns>
		public Worksheet GetWorksheetByName(string name)
		{
			return this.worksheets.FirstOrDefault(w => string.Compare(w.Name, name, true) == 0);
		}

		#region Collection of worksheet
		private WorksheetCollection worksheetCollection;

		/// <summary>
		/// Collection of worksheets
		/// </summary>
		public WorksheetCollection Worksheets
		{
			get
			{
				if (this.worksheetCollection == null)
				{
					this.worksheetCollection = new WorksheetCollection(this);
				}

				return this.worksheetCollection;
			}
		}

		#endregion Collection of worksheet

		/// <summary>
		/// Event raised when new worksheet is created
		/// </summary>
		public event EventHandler<WorksheetCreatedEventArgs> WorksheetCreated;

		/// <summary>
		/// Event raised when new worksheet is inserted
		/// </summary>
		public event EventHandler<WorksheetInsertedEventArgs> WorksheetInserted;

		/// <summary>
		/// Event raised when new worksheet is removed
		/// </summary>
		public event EventHandler<WorksheetRemovedEventArgs> WorksheetRemoved;

		/// <summary>
		/// Event raised before name of worksheet changing
		/// </summary>
		public event EventHandler<WorksheetNameChangingEventArgs> BeforeWorksheetNameChange;

		/// <summary>
		/// Event raised when name of worksheet is changed
		/// </summary>
		public event EventHandler<WorksheetNameChangingEventArgs> WorksheetNameChanged;

		/// <summary>
		/// Event raised when background color of worksheet name is changed.
		/// </summary>
		public event EventHandler<WorksheetEventArgs> WorksheetNameBackColorChanged;

		/// <summary>
		/// Event raised when text color of worksheet name is changed.
		/// </summary>
		public event EventHandler<WorksheetEventArgs> WorksheetNameTextColorChanged;

		internal bool CheckWorksheetName(string name)
		{
			return this.worksheets.All(s => string.Compare(s.Name, name, true) != 0);
		}

		internal void ValidateWorksheetName(string name)
		{
			if (!CheckWorksheetName(name))
			{
				throw new Exception("Specified name is already used by another worksheet.");
			}
		}

		internal string NotifyWorksheetNameChange(Worksheet sheet, string name)
		{
			if (this.BeforeWorksheetNameChange != null)
			{
				var arg = new WorksheetNameChangingEventArgs(sheet, name);
				this.BeforeWorksheetNameChange(this, arg);
				return arg.NewName;
			}
			else
			{
				return name;
			}
		}

		internal void RaiseWorksheetNameChangedEvent(Worksheet worksheet)
		{
			int index = GetWorksheetIndex(worksheet);

			if (index >= 0 && index < this.worksheets.Count)
			{
				if (this.sheetTab != null)
				{
					this.sheetTab.UpdateTab(index, worksheet.Name, worksheet.NameBackColor, worksheet.NameTextColor);
				}

				if (this.WorksheetNameChanged != null)
				{
					this.WorksheetNameChanged(this, new WorksheetNameChangingEventArgs(worksheet, worksheet.Name));
				}
			}
		}

		internal void RaiseWorksheetNameBackColorChangedEvent(Worksheet worksheet)
		{
			int index = GetWorksheetIndex(worksheet);

			if (index >= 0 && index < this.worksheets.Count)
			{
				if (this.sheetTab != null)
				{
					this.sheetTab.UpdateTab(index, worksheet.Name, worksheet.NameBackColor, worksheet.NameTextColor);
				}

				if (this.WorksheetNameBackColorChanged != null)
				{
					this.WorksheetNameBackColorChanged(this, new WorksheetEventArgs(worksheet));
				}
			}
		}

		internal void RaiseWorksheetNameTextColorChangedEvent(Worksheet worksheet)
		{
			int index = GetWorksheetIndex(worksheet);

			if (index >= 0 && index < this.worksheets.Count)
			{
				if (this.sheetTab != null)
				{
					this.sheetTab.UpdateTab(index, worksheet.Name, worksheet.NameBackColor, worksheet.NameTextColor);
				}

				if (this.WorksheetNameTextColorChanged != null)
				{
					this.WorksheetNameTextColorChanged(this, new WorksheetEventArgs(worksheet));
				}
			}
		}

		internal void RaiseWorksheetScrolledEvent(Worksheet worksheet, RGFloat x, RGFloat y)
		{
			if (this.controlAdapter != null
				&& this.controlAdapter.ControlInstance != null)
			{
				this.controlAdapter.ControlInstance.RaiseWorksheetScrolledEvent(worksheet, x, y);
			}
		}

		internal void ClearWorksheets()
		{
			while (this.worksheets.Count > 0)
			{
				var sheet = this.worksheets[this.worksheets.Count - 1];

				this.worksheets.Remove(sheet);
				sheet.workbook = null;

				if (this.WorksheetRemoved != null)
				{
					this.WorksheetRemoved(this, new WorksheetRemovedEventArgs(sheet));
				}
			}

			if (this.sheetTab != null)
			{
				this.sheetTab.ClearTabs();
			}

#if DEBUG
			Debug.Assert(this.worksheets.Count == 0);
#endif // DEBUG
		}

		public int WorksheetCount { get { return this.worksheets.Count; } }

		/// <summary>
		/// Reset a workbook to initial status (clear all worksheets and create one default)
		/// </summary>
		public void Reset()
		{
			ClearWorksheets();

			AddWorksheet(CreateWorksheet());
		}

		public bool IsEmpty
		{
			get
			{
				foreach (var sheet in worksheets)
				{
					if (sheet.MaxContentRow > 0 || sheet.MaxContentCol > 0)
					{
						return false;
					}
				}

				return true;
			}
		}

		#endregion // Worksheet Management

		#region Sheet Tab Control Interaction

		private ISheetTabControl sheetTab;

		internal void AttachSheetTabControl(ISheetTabControl sheetTab)
		{
			this.sheetTab = sheetTab;

			this.sheetTab.NewSheetClick += sheetTab_NewSheetClick;
			this.sheetTab.TabMoved += sheetTab_TabMoved;
		}

		internal void DetechSheetTabControl()
		{
			if (this.sheetTab != null)
			{
				this.sheetTab.NewSheetClick -= sheetTab_NewSheetClick;
				this.sheetTab.TabMoved -= sheetTab_TabMoved;

				this.sheetTab = null;
			}
		}

		#region Sheet Tab Events
		void sheetTab_NewSheetClick(object sender, EventArgs e)
		{
			var sheet = this.CreateWorksheet();

			if (sheet != null)
			{
				this.AddWorksheet(sheet);
				sheetTab.SelectedIndex = this.worksheets.Count - 1;
			}
		}

		void sheetTab_TabMoved(object sender, SheetTabMovedEventArgs e)
		{
			var sheet = this.worksheets[e.Index];
			this.worksheets.RemoveAt(e.Index);
			var targetIndex = e.TargetIndex;
			if (targetIndex > e.Index) targetIndex--;
			this.worksheets.Insert(targetIndex, sheet);
		}

		#endregion // Sheet Tab Events

		#endregion // Sheet Tab Control Interaction

		#region Settings

		private WorkbookSettings settings = WorkbookSettings.Default;

		/// <summary>
		/// Set settings for this workbook
		/// </summary>
		/// <param name="settings">settings to be set</param>
		/// <param name="value">set true to enable specified settings, false to disable</param>
		public void SetSettings(WorkbookSettings settings, bool value)
		{
			if (value)
			{
				if ((this.settings & settings) != settings)
				{
					this.settings |= settings;

					if (this.SettingsChanged != null)
					{
						this.SettingsChanged(this, null);
					}
				}
			}
			else
			{
				if ((this.settings & settings) > 0)
				{
					this.settings &= ~settings;

					if (this.SettingsChanged != null)
					{
						this.SettingsChanged(this, null);
					}
				}
			}
		}

		/// <summary>
		/// Get current settings of workbook
		/// </summary>
		/// <returns>Workbook settings set</returns>
		public WorkbookSettings GetSettings()
		{
			return this.settings;
		}

		/// <summary>
		/// Determine whether or not specified settings are set
		/// </summary>
		/// <param name="settings">settings to be checked</param>
		/// <returns>true if specified settings are set in current workbook</returns>
		public bool HasSettings(WorkbookSettings settings)
		{
			return this.settings.Has(settings);
		}

		/// <summary>
		/// Event raised when workbook settings is changed
		/// </summary>
		public event EventHandler SettingsChanged;
		#endregion // Settings

		#region Internal Exceptions

		/// <summary>
		/// Event is used to notify if there are any internal exceptions happen on worksheets
		/// </summary>
		public event EventHandler<ExceptionHappenEventArgs> ExceptionHappened;

		/// <summary>
		/// Notify that there are exceptions happen on any worksheet. 
		/// The event ExceptionHappened of workbook will be invoked.
		/// </summary>
		/// <param name="sheet">Worksheet where the exception happened</param>
		/// <param name="ex">Exception to describe the details of error information</param>
		public void NotifyExceptionHappen(Worksheet sheet, Exception ex)
		{
			Logger.Log("workbook", "internal exception: " + ex.Message);

			if (this.ExceptionHappened != null)
			{
				this.ExceptionHappened(this, new ExceptionHappenEventArgs(sheet, ex));
			}
		}
		#endregion // Internal Exceptions

		#region Appearance
		//private ControlAppearanceStyle controlStyle;

		///// <summary>
		///// Control Style Settings
		///// </summary>
		//internal ControlAppearanceStyle ControlStyle
		//{
		//	get;
		//	set;
		//}

		///// <summary>
		///// Set the style of grid control.
		///// </summary>
		///// <param name="controlStyle"></param>
		//public void SetControlStyle(ControlAppearanceStyle controlStyle)
		//{
		//	this.controlStyle = controlStyle;

		//	//foreach (var sheet in this.worksheets)
		//	//{
		//	//	sheet.controlStyle = this.controlStyle;
		//	//}

		//	if (this.controlAdapter != null && this.controlAdapter.IsVisible)
		//	{
		//		this.controlAdapter.Invalidate();
		//	}
		//}
		#endregion

#if PRINT
		public PrintSession CreatePrintSession()
		{
			var ps = new PrintSession();

			foreach (var sheet in this.worksheets)
			{
				ps.worksheets.Add(sheet);
			}

			ps.Init();

			return ps;
		}
#endif // PRINT

		public void Dispose()
		{
			if (this.sheetTab != null)
			{
				this.DetechSheetTabControl();
				this.sheetTab = null;
			}

			this.Clear();
		}
	}
}
