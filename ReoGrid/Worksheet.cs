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
 * Author:        Jing Lu <lujing at unvell.com>
 * Contributors:  Rick Meyer
 * 

 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * Copyright (c) 2014 Rick Meyer, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;

using unvell.Common;

#if WINFORM || WPF
using unvell.Common.Win32Lib;
#endif // WINFORM || WPF

using unvell.ReoGrid.Actions;
using unvell.ReoGrid.IO;
using unvell.ReoGrid.Events;
using unvell.ReoGrid.Views;
using unvell.ReoGrid.CellTypes;
using unvell.ReoGrid.Utility;

#if EX_SCRIPT
using unvell.ReoScript;
using unvell.ReoGrid.Script;
#endif // EX_SCRIPT

#if WINFORM || WPF
//using CellArray = unvell.ReoGrid.Data.JaggedTreeArray<unvell.ReoGrid.ReoGridCell>;
using CellArray = unvell.ReoGrid.Data.Index4DArray<unvell.ReoGrid.Cell>;
#elif ANDROID || iOS
using CellArray = unvell.ReoGrid.Data.ReoGridCellArray;
#endif // ANDROID

#if WINFORM || ANDROID
using RGFloat = System.Single;
#elif WPF || iOS
using RGFloat = System.Double;
#endif // WINFORM & WPF

#if WINFORM
using RGKeys = System.Windows.Forms.Keys;
#endif // WINFORM

using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Interaction;
using unvell.ReoGrid.Main;

namespace unvell.ReoGrid
{
	/// <summary>
	/// ReoGrid worksheet object, manage the single spreadsheet data and styles.
	/// </summary>
	public sealed partial class Worksheet : IDisposable
	{
		#region ControlAdapter
		internal IControlAdapter controlAdapter;

		internal IControlAdapter ControlAdapter
		{
			get
			{
				return this.controlAdapter;
			}
			set
			{
				this.controlAdapter = value;

				if (this.controlAdapter == null)
				{
					this.viewportController = null;
				}
				else if (this.viewportController == null)
				{
					InitViewportController();
				}
			}
		}
		#endregion // ControlAdapter

		#region Constants
		/// <summary>
		/// Default width of column
		/// </summary>
		public static readonly ushort InitDefaultColumnWidth = 70;

		/// <summary>
		/// Default height of row
		/// </summary>
		public static readonly ushort InitDefaultRowHeight = 20;

		/// <summary>
		/// Default number of columns
		/// </summary>
		internal const int DefaultCols = 100;

		/// <summary>
		/// Default number of rows
		/// </summary>
		internal const int DefaultRows = 200;

		/// <summary>
		/// Default button size of outlinetextColor buttons
		/// </summary>
		internal const int OutlineButtonSize = 13;

		/// <summary>
		/// Default root style of entire range of grid control
		/// </summary>
		public static readonly WorksheetRangeStyle DefaultStyle = new WorksheetRangeStyle
		{
			Flag = PlainStyleFlag.FontName | PlainStyleFlag.FontSize | PlainStyleFlag.AlignAll,
			FontName = "Calibri",
			FontSize = 10.25f,
			HAlign = ReoGridHorAlign.General,
			VAlign = ReoGridVerAlign.General,
		};
		#endregion

		#region Workbook Relation
		internal Workbook workbook;

		/// <summary>
		/// Instance of workbook of this worksheet
		/// </summary>
		public IWorkbook Workbook { get { return this.workbook; } }

		private void CheckWorkbookAssociated()
		{
			if (this.workbook == null)
			{
				throw new InvalidOperationException("Worksheet is not associated to any workbook. Add it into workbook firstly.");
			}
		}

		#region Name
		private string name;

		/// <summary>
		/// Get or set the name of worksheet
		/// </summary>
		public string Name
		{
			get { return this.name; }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException("cannot set worksheet's name to null or empty.");
				}

				if (this.name == value) return;

				if (this.workbook != null)
				{
					this.workbook.ValidateWorksheetName(value);
					value = this.workbook.NotifyWorksheetNameChange(this, value);
				}

				this.name = value;

				if (this.NameChanged != null)
				{
					this.NameChanged(this, null);
				}

				if (this.workbook != null)
				{
					this.workbook.RaiseWorksheetNameChangedEvent(this);
				}
			}
		}

		/// <summary>
		/// Event raised when name of worksheet is changed
		/// </summary>
		public event EventHandler NameChanged;
		#endregion // Name

		#region Name Text Color & Back Color

		/// <summary>
		/// Get or set the background color for worksheet name that is displayed on sheet tab control
		/// </summary>
		private SolidColor nameBackColor = SolidColor.Transparent;

		public SolidColor NameBackColor
		{
			get { return this.nameBackColor; }
			set { this.nameBackColor = value; this.OnNameBackColorChanged(); }
		}

		private SolidColor nameTextColor = SolidColor.Transparent;

		/// <summary>
		/// Get or set the text color for worksheet name.
		/// </summary>
		public SolidColor NameTextColor
		{
			get { return this.nameTextColor; }
			set { this.nameTextColor = value; this.OnNameTextColorChanged(); }
		}

		/// <summary>
		/// Method invoked when background color of worksheet name is changed.
		/// </summary>
		internal void OnNameBackColorChanged()
		{
			if (this.workbook != null)
			{
				this.workbook.RaiseWorksheetNameBackColorChangedEvent(this);
			}
		}

		/// <summary>
		/// Method invoked when text color of worksheet name is changed.
		/// </summary>
		internal void OnNameTextColorChanged()
		{
			if (this.workbook != null)
			{
				this.workbook.RaiseWorksheetNameTextColorChangedEvent(this);
			}
		}
		#endregion // Name Text Color & Back Color

		//internal ControlAppearanceStyle controlStyle;

		#endregion // Workbook Relation

		#region Constructor & Initialize

		/// <summary>
		/// Create ReoGrid worksheet instance
		/// </summary>
		/// <param name="workbook">ReoGrid workbook instance</param>
		/// <param name="name">Name for this worksheet</param>
		internal Worksheet(Workbook workbook, string name)
			: this(workbook, name, Worksheet.DefaultRows, Worksheet.DefaultCols)
		{
		}

		/// <summary>
		/// </summary>
		/// <param name="workbook">ReoGrid workbook instance</param>
		/// <param name="name">Name for this worksheet</param>
		/// <param name="rows">Initial number of rows</param>
		/// <param name="cols">Initial number of columns</param>
		internal Worksheet(Workbook workbook, string name, int rows, int cols)
		{
			this.workbook = workbook;

			this.name = name;
			this.ControlAdapter = workbook.controlAdapter;

			// initialize spreadsheet 
			InitGrid(rows, cols);
		}

		#endregion // Constructor & Initialize

		#region Draw

#if WINFORM
		/*
		public System.Drawing.Bitmap DrawToBitmap()
		{
			//int maxWidth = 0, maxHeight = 0;
			//foreach (var part in this.viewportController.Views)
			//{
			//	if (maxWidth < part.Bounds.Width)
			//	{
			//		maxWidth = (int)Math.Ceiling(part.Bounds.Width);
			//	}

			//	if (maxHeight < part.Bounds.Height)
			//	{
			//		maxHeight = (int)Math.Ceiling(part.Bounds.Height);
			//	}
			//}

			// TODO
			var vcBounds = this.viewportController.Bounds;
			return DrawToBitmap(vcBounds.Width, vcBounds.Height);
		}

		/// <summary>
		/// Draw spreadsheet into bitmap
		/// </summary>
		/// <param name="width">Width of bitmap</param>
		/// <param name="height">Height of bitmap</param>
		/// <returns>Bitmap contains the drawing result of spreadsheet</returns>
		public System.Drawing.Bitmap DrawToBitmap(int width, int height)
		{
			return DrawToBitmap(0, 0, width, height, DrawMode.Print);
		}

		/// <summary>
		/// Draw spreadsheet into bitmap
		/// </summary>
		/// <param name="x">X coordinate on spreadsheet</param>
		/// <param name="y">Y coordinate on spreadsheet</param>
		/// <param name="width">Width of bitmap</param>
		/// <param name="height">Height of bitmap</param>
		/// <param name="drawMode">Drawing mode</param>
		/// <returns>Bitmap contains the drawing result of spreadsheet</returns>
		public System.Drawing.Bitmap DrawToBitmap(int x, int y, int width, int height, DrawMode drawMode)
		{
			System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height);

			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
			{

#if WINFORM
				var ig = new GDIGraphics(g);
#elif WPF
				var ig = new unvell.ReoGrid.Rendering.GDIAdapterGraphics(g);
#endif

				var memViewportController = new NormalViewportController(this);
				memViewportController.Bounds = new RGRect(0, 0, width, height);
				memViewportController.UpdateController();
				memViewportController.ScrollViews(ScrollDirection.Both, x, y);

				RGDrawingContext dc = new RGDrawingContext(this, drawMode, ig);
				memViewportController.Draw(dc);
			}

			return bitmap;
		}
		 */
#endif // WINFORM
		#endregion Draw To Bitmap

		#region Freeze
		/// <summary>
		/// Get current frozen areas. Use method <code>FreezeToCells</code> to change this property.
		/// </summary>
		public FreezeArea FreezeArea { get; private set; }

		/// <summary>
		/// Get current frozen position. Use method <code>FreezeToCells</code> to change this property.
		/// </summary>
		public CellPosition FreezePos { get; private set; }

		/// <summary>
		/// Freezes worksheet at specified cell position.
		/// </summary>
		/// <param name="pos">Position to freeze worksheet.</param>
		public void FreezeToCell(CellPosition pos)
		{
			this.FreezeToCell(pos.Row, pos.Col);
		}

		/// <summary>
		/// Freezes worksheet at specified address position.
		/// </summary>
		/// <param name="address">Cell position described in address string to freeze worksheet.</param>
		public void FreezeToCell(string address)
		{
			RangePosition range;

			if (CellPosition.IsValidAddress(address))
			{
				this.FreezeToCell(new CellPosition(address));
			}
			else if (this.TryGetNamedRangePosition(address, out range))
			{
				this.FreezeToCell(range.StartPos);
			}
			else
			{
				throw new InvalidAddressException(address);
			}
		}

		/// <summary>
		/// Freezes worksheet at specified cell position and specifies the freeze areas.
		/// </summary>
		/// <param name="pos">Cell position to freeze worksheet.</param>
		/// <param name="area">Specifies the frozen panes.</param>
		public void FreezeToCell(CellPosition pos, FreezeArea area)
		{
			this.FreezeToCell(pos.Row, pos.Col, area);
		}

		/// <summary>
		/// Freezes worksheet at specified cell position.
		/// </summary>
		/// <param name="row">Zero-based number of row to freeze worksheet.</param>
		/// <param name="col">Zero-based number of column to freeze worksheet.</param>
		public void FreezeToCell(int row, int col)
		{
			if (row < 0 || col < 0 || row >= this.rows.Count || col >= this.cols.Count) return;

			this.FreezeToCell(row, col, FreezeArea.LeftTop);
		}

		//private CellPosition lastFrozenPosition;
		//private FreezeArea lastFrozenArea = FreezeArea.None;

		/// <summary>
		/// Freezes worksheet at specified cell position and specifies the freeze areas.
		/// </summary>
		/// <param name="row">Zero-based number of row to freeze worksheet.</param>
		/// <param name="col">Zero-based number of column to freeze worksheet.</param>
		/// <param name="area">Specifies the frozen panes.</param>
		public void FreezeToCell(int row, int col, FreezeArea area)
		{
			/////////////////////////////////////////////////////////////////
			// fix issue #151, #172, #313
			//if (lastFrozenPosition == new CellPosition(row, col) && lastFrozenArea == area)
			//{
			//	//skip to perform freeze if forzen position and area are not changed
			//	return;
			//}

			//lastFrozenPosition = new CellPosition(row, col);
			//lastFrozenArea = area;
			/////////////////////////////////////////////////////////////////

			if (this.viewportController != null)
			{
				// update viewport bounds - sometimes the viewport may cannot get the correct size for freezing,
				// so trying update it here
				this.viewportController.Bounds = this.controlAdapter.GetContainerBounds();
			}

			if (viewportController is IFreezableViewportController)
			{
				// if position is none, doing unfreeze
				if (area == FreezeArea.None)
				{
					row = 0;
					col = 0;
				}
				// if position is left or right, just set start row to zero
				else if (area == FreezeArea.Left)
				{
					row = 0;
					area = FreezeArea.LeftTop;
				}
				else if (area == FreezeArea.Right)
				{
					row = 0;
					area = FreezeArea.RightTop;
				}
				// if poisition is top or bottom, just set start column to zero
				else if (area == FreezeArea.Top)
				{
					col = 0;
					area = FreezeArea.LeftTop;
				}
				else if (area == FreezeArea.Bottom)
				{
					col = 0;
					area = FreezeArea.LeftBottom;
				}
			}

			this.FreezePos = new CellPosition(row, col);
			this.FreezeArea = area;

			if (viewportController is IFreezableViewportController freezableViewportController)
			{
				// freeze via supported viewportcontroller
				freezableViewportController.Freeze(this.FreezePos, area);
				RequestInvalidate();

				// raise events
				if (row > 0 || col > 0)
				{
					this.CellsFrozen?.Invoke(this, null);
				}
				else
				{
					this.CellsUnfrozen?.Invoke(this, null);
				}
			}
			//else
			//{
			//	// no supported viewportcontroller available, throw exception
			//	throw new FreezeUnsupportedException();
			//}
		}

		/// <summary>
		/// Unfreeze current worksheet.
		/// </summary>
		public void Unfreeze()
		{
			FreezeToCell(0, 0);
		}

		/// <summary>
		/// Check whether or not current worksheet can be frozen.
		/// </summary>
		/// <returns>True if current worksheet can be frozen; Otherwise return false.</returns>
		public bool CanFreeze()
		{
			return viewportController is IFreezableViewportController;
		}

		/// <summary>
		/// Return whether or not current worksheet has frozen rows or columns.
		/// </summary>
		public bool IsFrozen
		{
			get
			{
				return (this.FreezePos.Row > 0 || this.FreezePos.Col > 0)
					&& this.FreezeArea != FreezeArea.None;
			}
		}

		/// <summary>
		/// Event raised when worksheet is frozen.
		/// </summary>
		public event EventHandler CellsFrozen;

		/// <summary>
		/// Event raised when worksheet is unfreezed.
		/// </summary>
		public event EventHandler CellsUnfrozen;
		#endregion // Freeze

		#region Zoom
		private static readonly float minScaleFactor = 0.1f;
		private static readonly float maxScaleFactor = 4f;
		private RGFloat _scaleFactor = 1f;
		internal RGFloat renderScaleFactor = 1f;

		/// <summary>
		/// Get or set worksheet scale factor.
		/// </summary>
		public RGFloat ScaleFactor
		{
			get { return _scaleFactor; }
			set { SetScale(value); }
		}

		/// <summary>
		/// Event raised when worksheet is scaled.
		/// </summary>
		public event EventHandler<EventArgs> Scaled;

		/// <summary>
		/// Set scale factor to zoom in/out current worksheet.
		/// </summary>
		/// <param name="factor">Scale factor to be applied.</param>
		public void SetScale(RGFloat factor)
		{
			if (currentEditingCell != null)
			{
				EndEdit(EndEditReason.NormalFinish);
			}

			if (this.controlAdapter == null)
			{
				if (factor < minScaleFactor) factor = minScaleFactor;
				if (factor > maxScaleFactor) factor = maxScaleFactor;
			}
			else
			{
				if (factor < this.controlAdapter.MinScale) factor = this.controlAdapter.MinScale;
				if (factor > this.controlAdapter.MaxScale) factor = this.controlAdapter.MaxScale;
			}

			if (this._scaleFactor != factor)
			{
				this._scaleFactor = factor;

				if (this.controlAdapter == null)
				{
					this.renderScaleFactor = this._scaleFactor;
				}
				else
				{
					this.renderScaleFactor = this.controlAdapter.BaseScale + this._scaleFactor;
				}

				if (this.viewportController is IScalableViewportController scalableViewController)
				{
					scalableViewController.ScaleFactor = this.renderScaleFactor;
				}

				this.viewportController?.UpdateController();

				Scaled?.Invoke(this, null);
			}
		}

		/// <summary>
		/// Zoom in current worksheet.
		/// </summary>
		public void ZoomIn()
		{
			this.SetScale(this._scaleFactor + 0.1f);
		}

		/// <summary>
		/// Zoom out current worksheet.
		/// </summary>
		public void ZoomOut()
		{
			this.SetScale(this._scaleFactor - 0.1f);
		}

		/// <summary>
		/// Set scale factor to 1.0. (Reset worksheet to original scale)
		/// </summary>
		public void ZoomReset()
		{
			SetScale(1f);
		}
		#endregion

		#region Index Getter/Setter

		/// <summary>
		/// Access cells data from worksheet at specified position.
		/// </summary>
		/// <param name="pos">Position on worksheet to be access.</param>
		/// <returns>Cells data from worksheet at specified position.</returns>
		public object this[CellPosition pos]
		{
			get { return this[pos.Row, pos.Col]; }
			set { this[pos.Row, pos.Col] = value; }
		}

		/// <summary>
		/// Access cells data on worksheet at specified position.
		/// </summary>
		/// <param name="row">Number of row of the cell to be accessed.</param>
		/// <param name="col">Number of column of the cell to be accessed.</param>
		/// <returns>Cells Data from worksheet at specified position.</returns>
		public object this[int row, int col]
		{
			get
			{
				Cell cell = GetCell(row, col);
				return cell == null ? null : cell.InnerData;
			}
			set
			{
				SetCellData(row, col, value);
			}
		}

		/// <summary>
		/// Get or set data in specified range.
		/// </summary>
		/// <param name="row">Number of start row.</param>
		/// <param name="col">Number of start column.</param>
		/// <param name="rows">Number of rows.</param>
		/// <param name="cols">Number of columns.</param>
		/// <returns></returns>
		public object this[int row, int col, int rows, int cols]
		{
			get
			{
				return this[new RangePosition(row, col, rows, cols)];
			}
			set
			{
				this[new RangePosition(row, col, rows, cols)] = value;
			}
		}

		/// <summary>
		/// Get or set data from specified range.
		/// </summary>
		/// <param name="range">Range to be get or set.</param>
		/// <returns>Data copied from grid.</returns>
		public object this[RangePosition range]
		{
			get
			{
				return GetRangeData(range);
			}
			set
			{
				SetRangeData(range, value);
			}
		}

		/// <summary>
		/// Access cells data from worksheet at specified position or range.
		/// </summary>
		/// <example>A1 or A1:C3</example>
		/// <param name="addressOrName">Position specified in address code or name. (e.g. A1, A1:C3, $B$5, mydata)</param>
		/// <returns>Cells data returned from worksheet at specified position.</returns>
		/// <exception cref="InvalidAddressException">Throw this exception if specified address or name is invalid.</exception>
		public object this[string addressOrName]
		{
			get
			{
				if (CellPosition.IsValidAddress(addressOrName))
				{
					return this[new CellPosition(addressOrName)];
				}
				else if (RangePosition.IsValidAddress(addressOrName))
				{
					return this[new RangePosition(addressOrName)];
				}

				NamedRange referenceRange;
				if (registeredNamedRanges.TryGetValue(addressOrName, out referenceRange))
				{
					return this[referenceRange];
				}

				throw new InvalidAddressException(addressOrName);
			}
			set
			{
				if (CellPosition.IsValidAddress(addressOrName))
				{
					this[new CellPosition(addressOrName)] = value;
					return;
				}
				else if (RangePosition.IsValidAddress(addressOrName))
				{
					this[new RangePosition(addressOrName)] = value;
					return;
				}

				NamedRange referenceRange;
				if (registeredNamedRanges.TryGetValue(addressOrName, out referenceRange))
				{
					this[referenceRange] = value;
					return;
				}

				throw new InvalidAddressException(addressOrName);
			}
		}

		/// <summary>
		/// Access cells data by using a cell instance.
		/// </summary>
		/// <param name="cell">Cell instance to set data.</param>
		/// <returns>Data returned from specifed cell instance.</returns>
		internal object this[Cell cell]
		{
			get { return cell.InnerData; }
			set { SetSingleCellData(cell, value); }
		}

		#endregion // Index Getter/Setter

		#region Cell & Grid Management

		internal CellArray cells = new CellArray();

		/// <summary>
		/// Get cell from specified address. 
		/// If cell instance does not exist, create and return a new cell instance.
		/// </summary>
		/// <remarks>Use <code>GetCell</code> to get cell without creating new instance.</remarks>
		/// <param name="address">Address to create and get cell instance.</param>
		/// <returns>Cell instance at specified address.</returns>
		public Cell CreateAndGetCell(string address)
		{
			return CellPosition.IsValidAddress(address) ? CreateAndGetCell(new CellPosition(address)) : null;
		}

		/// <summary>
		/// Get cell from specified cell position.
		/// If cell instance does not exist, create and return a new cell instance.
		/// </summary>
		/// <remarks>Use <code>GetCell</code> to get cell without creating new instance.</remarks>
		/// <param name="pos">Position to create and get cell instance</param>
		/// <returns>Cell instance at specified position.</returns>
		public Cell CreateAndGetCell(CellPosition pos)
		{
			return CreateAndGetCell(pos.Row, pos.Col);
		}

		/// <summary>
		/// Get cell from specified cell position.
		/// If cell instance does not exist, create and return a new cell instance.
		/// </summary>
		/// <remarks>Use <code>GetCell</code> to get cell without creating new instance.</remarks>
		/// <param name="row">Zero-based number of row to create and return cell instance.</param>
		/// <param name="col">Zero-based number of column to create and return cell instance.</param>
		/// <returns>Cell instance at specified position.</returns>
		public Cell CreateAndGetCell(int row, int col)
		{
			Cell cell = cells[row, col];
			if (cell == null) cell = CreateCell(row, col);
			return cell;
		}

		/// <summary>
		/// Create cell instance at specified position.
		/// </summary>
		/// <param name="row">Zero-based number of row to create and return cell instance.</param>
		/// <param name="col">Zero-based number ofcolumn to create and return cell instance.</param>
		/// <param name="updateStyle">Determines whether or not to initial the cell's style</param>
		/// <returns>Created cell instance at specified position.</returns>
		internal Cell CreateCell(int row, int col, bool updateStyle = true)
		{
			// create cell instance
			var cell = new Cell(this)
			{
				InternalRow = row,
				InternalCol = col,
				Colspan = 1,
				Rowspan = 1,
				Bounds = GetCellRectFromHeader(row, col),
			};

			StyleUtility.UpdateCellParentStyle(this, cell);

			// attach to grid, this must be executed before set cell body
			cells[row, col] = cell;

			var colHeader = this.cols[col];
			var rowHeader = this.rows[row];

			// create cell body if the either column or row header has a default body type specified
			if (colHeader.DefaultCellBody != null)
			{
				try
				{
					cell.Body = System.Activator.CreateInstance(colHeader.DefaultCellBody) as ICellBody;
				}
				catch (Exception ex)
				{
					throw new CannotCreateCellBodyException(
						"Cannot create instance of default cell body specified in column header.", ex);
				}
			}
			else if (rowHeader.DefaultCellBody != null)
			{
				try
				{
					cell.Body = System.Activator.CreateInstance(rowHeader.DefaultCellBody) as ICellBody;
				}
				catch (Exception ex)
				{
					throw new CannotCreateCellBodyException(
						"Cannot create instance of default cell body specified in row header.", ex);
				}
			}

			if (cell.body != null)
			{
				cell.body.OnSetup(cell);
			}

			if (updateStyle)
			{
				// update render align
				StyleUtility.UpdateCellRenderAlign(this, cell);

				// update font of cell
				UpdateCellFont(cell);
			}

			return cell;
		}

		/// <summary>
		/// Retrieve cell instance from specified address or defined name.
		/// </summary>
		/// <param name="addressOrName">Address or name to find the cell instance.</param>
		/// <returns>Instance of cell retrieved from specified address or defined name, 
		/// return null if cell instance does not exist.</returns>
		/// <remarks>Use <code>CreateAndGetCell</code> to create and get cell instance.</remarks>
		/// <exception cref="InvalidAddressException">Throws if specified address or name is invalid.</exception>
		public Cell GetCell(string addressOrName)
		{
			if (CellPosition.IsValidAddress(addressOrName))
			{
				return GetCell(new CellPosition(addressOrName));
			}

			NamedRange range;

			if (this.registeredNamedRanges.TryGetValue(addressOrName, out range))
			{
				return this.GetCell(range.StartPos);
			}
			else
			{
				throw new InvalidAddressException(addressOrName);
			}
		}

		/// <summary>
		/// Retrieve cell from specified position.
		/// </summary>
		/// <param name="pos">Position to locate cell.</param>
		/// <returns>Null if cell instance not found at specified position.</returns>
		public Cell GetCell(CellPosition pos)
		{
			return GetCell(pos.Row, pos.Col);
		}

		/// <summary>
		/// Retrieve cell at specified number of row and number of column.
		/// </summary>
		/// <param name="row">Zero-based number of row.</param>
		/// <param name="col">Zero-based number of column.</param>
		/// <returns>Null if cell instance not found at specified position.</returns>
		public Cell GetCell(int row, int col)
		{
			return cells[row, col];
		}

		/// <summary>
		/// Return the merged first cell inside range.
		/// </summary>
		/// <param name="address">Position in range.</param>
		/// <returns>First left-top cell of the range which cell belongs to.</returns>
		public Cell GetMergedCellOfRange(string address)
		{
			return CellPosition.IsValidAddress(address) ? GetMergedCellOfRange(new CellPosition(address)) : null;
		}

		/// <summary>
		/// Return the merged first cell inside range.
		/// </summary>
		/// <param name="pos">Position in range.</param>
		/// <returns>First left-top cell of the range which cell belongs to.</returns>
		public Cell GetMergedCellOfRange(CellPosition pos)
		{
			return GetMergedCellOfRange(pos.Row, pos.Col);
		}

		/// <summary>
		/// Return the first cell inside merged range.
		/// </summary>
		/// <param name="row">Row of position in range.</param>
		/// <param name="col">Column of position in range.</param>
		/// <returns></returns>
		public Cell GetMergedCellOfRange(int row, int col)
		{
			return GetMergedCellOfRange(CreateAndGetCell(row, col));
		}

		/// <summary>
		/// Return the first cell inside merged range.
		/// </summary>
		/// <param name="cell">Cell instance in range.</param>
		/// <returns>Cell instance of merged range.</returns>
		public Cell GetMergedCellOfRange(Cell cell)
		{
			if (cell == null) return null;

			if (cell.InsideMergedRange)
			{
				if (cell.IsStartMergedCell)
					return cell;
				else
					return CreateAndGetCell(cell.MergeStartPos);
			}
			else
			{
				return cell;
			}
		}

		/// <summary>
		/// Return the range if the cell specified by position is a merged cell
		/// </summary>
		/// <param name="pos">Cell of this position to be checked</param>
		/// <returns>Range of merged cell returned from this method</returns>
		public RangePosition GetRangeIfMergedCell(CellPosition pos)
		{
			var cell = this.cells[pos.Row, pos.Col];

			if (cell == null)
				return new RangePosition(pos, pos);
			else if (cell.IsStartMergedCell)
				return new RangePosition(cell.Row, cell.Column, cell.Rowspan, cell.Colspan);
			else if (!cell.IsValidCell)
				return GetRangeIfMergedCell(cell.MergeStartPos);
			else
				return new RangePosition(pos, pos);
		}

		/// <summary>
		/// Check whether the cell specified by an address is merged cell
		/// </summary>
		/// <param name="address">address to be checked</param>
		/// <returns>true if the cell is merged cell</returns>
		public bool IsMergedCell(string address)
		{
			return CellPosition.IsValidAddress(address) ? IsMergedCell(new CellPosition(address)) : false;
		}

		/// <summary>
		/// Check whether the cell at specified position is a merged cell
		/// </summary>
		/// <param name="pos">position to be checked</param>
		/// <returns>true if the cell is merged cell</returns>
		public bool IsMergedCell(CellPosition pos)
		{
			return IsMergedCell(pos.Row, pos.Col);
		}

		/// <summary>
		/// Check whether specified range does just contains one merged cell
		/// </summary>
		/// <param name="range">specified range to be checked</param>
		/// <returns>true if range contains only one merged cell</returns>
		public bool IsMergedCell(RangePosition range)
		{
			var cell = GetCell(range.StartPos);

			if (cell == null)
			{
				return range.Rows == 1 && range.Cols == 1;
			}

			return cell.Rowspan == range.Rows && cell.Colspan == range.Cols;
		}

		/// <summary>
		/// Check whether a cell is merged cell
		/// </summary>
		/// <param name="row">number of row to be checked</param>
		/// <param name="col">number of column to be checked</param>
		/// <returns>true if the cell is merged cell</returns>
		public bool IsMergedCell(int row, int col)
		{
			var cell = cells[row, col];
			return cell != null && cell.IsMergedCell;
		}

		/// <summary>
		/// Check whether the specified cell is valid (Not merged by others cell)
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public bool IsValidCell(string address)
		{
			return CellPosition.IsValidAddress(address) ? IsValidCell(new CellPosition(address)) : false;
		}

		/// <summary>
		/// Check whether the specified cell is valid (Not merged by others cell)
		/// </summary>
		/// <param name="pos">Position to be checked</param>
		/// <returns>true if specified position is a valid cell</returns>
		public bool IsValidCell(CellPosition pos)
		{
			return IsValidCell(pos.Row, pos.Col);
		}

		/// <summary>
		/// Check whether the specified cell is valid. (Not merged by others cell)
		/// </summary>
		/// <param name="row">Position of row to be checked.</param>
		/// <param name="col">Position of column to be checked.</param>
		/// <returns>true if specified position is valid cell.</returns>
		public bool IsValidCell(int row, int col)
		{
			if (row < 0 || col < 0 || row >= this.cells.RowCapacity || col >= this.cells.ColCapacity) return false;
			var cell = cells[row, col];
			return cell == null || cell.IsValidCell;
		}

		/// <summary>
		/// Check whether or not the specified cell is visible.
		/// </summary>
		/// <param name="cell">Cell instance to be checked.</param>
		/// <returns>True if the cell is visible; otherwise return false.</returns>
		public bool IsCellVisible(Cell cell)
		{
			return IsCellVisible(cell.InternalPos);
		}

		/// <summary>
		/// Check whether or not the specified cell is visible.
		/// </summary>
		/// <param name="pos">Position to locate the cell on worksheet.</param>
		/// <returns>True if the cell is visible; otherwise return false.</returns>
		public bool IsCellVisible(CellPosition pos)
		{
			return IsCellVisible(pos.Row, pos.Col);
		}

		/// <summary>
		/// Check whether or not the specified cell is visible.
		/// </summary>
		/// <param name="row">Zero-based number of row used to locate the cell.</param>
		/// <param name="col">Zero-based number of column used to locate the cell.</param>
		/// <returns>True if the cell is visible; otherwise return false.</returns>
		public bool IsCellVisible(int row, int col)
		{
			return IsRowVisible(row) && IsColumnVisible(col);
		}

		/// <summary>
		/// Iterate over all cells in specified range. Invalid cells (merged by others cell) will be skipped.
		/// </summary>
		/// <param name="addressOrName">address or name to locate the range</param>
		/// <param name="iterator">callback iterator to check through all cells</param>
		/// <remarks>anytime return <code>false</code> to abort iteration.</remarks>
		/// <exception cref="InvalidAddressException">throw if specified address or name is invalid</exception>
		public void IterateCells(string addressOrName, Func<int, int, Cell, bool> iterator)
		{
			this.IterateCells(addressOrName, true, iterator);
		}

		/// <summary>
		/// Iterate over all valid cells inside specified range. Invalid cells (merged by others cell) will be skipped.
		/// </summary>
		/// <param name="addressOrName">Address or name to locate the range on worksheet.</param>
		/// <param name="skipEmptyCells">Determines whether or not to skip empty cells. (Default is true)</param>
		/// <param name="iterator">Callback iterator to check all cells in specified range.</param>
		/// <remarks>Anytime return <code>false</code> to abort iteration.</remarks>
		/// <exception cref="InvalidAddressException">throw if specified address or name is invalid</exception>
		public void IterateCells(string addressOrName, bool skipEmptyCells, Func<int, int, Cell, bool> iterator)
		{
			NamedRange namedRange;

			if (RangePosition.IsValidAddress(addressOrName))
			{
				this.IterateCells(new RangePosition(addressOrName), skipEmptyCells, iterator);
			}
			else if (this.registeredNamedRanges.TryGetValue(addressOrName, out namedRange))
			{
				this.IterateCells(namedRange, skipEmptyCells, iterator);
			}
			else
				throw new InvalidAddressException(addressOrName);
		}

		/// <summary>
		/// Iterate over all valid cells inside specified range. Invalid cells (merged by others cell) will be skipped.
		/// </summary>
		/// <param name="range">Specified range to iterate cells</param>
		/// <param name="iterator">callback iterator to check through all cells</param>
		/// <remarks>Anytime return <code>false</code> to abort iteration.</remarks>
		public void IterateCells(RangePosition range, Func<int, int, Cell, bool> iterator)
		{
			this.IterateCells(range, true, iterator);
		}

		/// <summary>
		/// Iterate over all valid cells inside specified range. Invalid cells (merged by others cell) will be skipped.
		/// </summary>
		/// <param name="range">Specified range to iterate cells</param>
		/// <param name="skipEmptyCells">Determines whether or not to skip empty cells. (Default is true)</param>
		/// <param name="iterator">callback iterator to check through all cells</param>
		/// <remarks>anytime return <code>false</code> to abort iteration.</remarks>
		public void IterateCells(RangePosition range, bool skipEmptyCells, Func<int, int, Cell, bool> iterator)
		{
			var fixedRange = FixRange(range);
			this.IterateCells(fixedRange.Row, fixedRange.Col, fixedRange.Rows, fixedRange.Cols, skipEmptyCells, iterator);
		}

		/// <summary>
		/// Iterate over all valid cells inside specified range. Invalid cells (merged by others cell) will be skipped.
		/// </summary>
		/// <param name="row">Number of row of the range to be iterated.</param>
		/// <param name="col">Number of column of the range to be iterated.</param>
		/// <param name="rows">Number of rows of the range to be iterated.</param>
		/// <param name="cols">Number of columns of the range to be iterated.</param>
		/// <param name="skipEmptyCells">Determines whether or not to skip empty cells.</param>
		/// <param name="iterator">Callback iterator to check over all cells in specified range.</param>
		/// <remarks>Anytime return false in iterator to abort iteration.</remarks>
		public void IterateCells(int row, int col, int rows, int cols, bool skipEmptyCells, Func<int, int, Cell, bool> iterator)
		{
			cells.Iterate(row, col, rows, cols, skipEmptyCells, (r, c, cell) =>
			{
				int cspan = (cell == null) ? 1 : cell.Colspan;

				if (cspan <= 0) return 1;

				if (!iterator(r, c, cell))
				{
					return 0;
				}

				return (cspan <= 0) ? 1 : cspan;
			});
		}

		private int maxColumnHeader = -1;
		private int maxRowHeader = -1;

		/// <summary>
		/// Get used range of this worksheet.
		/// </summary>
		public RangePosition UsedRange
		{
			get { return RangePosition.FromCellPosition(0, 0, MaxContentRow, MaxContentCol); }
		}

		/// <summary>
		/// Get maximum content number of row.
		/// </summary>
		public int MaxContentRow
		{
			get
			{
				//return Math.Min(this.rows.Count - 1, 
				//	Math.Max(Math.Max(cells.MaxRow, Math.Max(hBorders.MaxRow, vBorders.MaxRow)), 
				//	maxRowHeader + 1));

				return Math.Min(this.rows.Count - 1, 

					Math.Max(Math.Max(cells.MaxRow, Math.Max(hBorders.MaxRow - 1, vBorders.MaxRow)), 
					
					// init is -1, so return +1=0 when -1
					maxRowHeader + 1));
			}
		}

		/// <summary>
		/// Get maximum content number of column.
		/// </summary>
		public int MaxContentCol
		{
			get
			{
				//return Math.Min(this.cols.Count - 1, 
				//	Math.Max(Math.Max(cells.MaxCol, Math.Max(hBorders.MaxCol, vBorders.MaxCol)), 
				//	maxColumnHeader + 1));

				return Math.Min(this.cols.Count - 1, 
					Math.Max(Math.Max(cells.MaxCol, Math.Max(hBorders.MaxCol, vBorders.MaxCol - 1)),
					
					// init is -1, so return +1=0 when -1
					maxColumnHeader + 1));
			}
		}

		// todo: remove this method by merging cells, hborders and vborders in next version
		//internal bool IsPageNull(int row, int col)
		//{
		//	return this.cells.IsPageNull(row, col) && this.hBorders.IsPageNull(row, col) && this.vBorders.IsPageNull(row, col);
		//}

		#region Cell Collection

		private CellCollection cellCollection;

		/// <summary>
		/// Get collection of cells from spreadsheet.
		/// 
		/// (Careful: this method will create cell instance even there is no data and styles used in the cell,
		/// create many empty cell instances will spend a lot of memory. To get cell's data or style without
		/// creating instance use the <code>GetCellData</code> or <code>GetRangeStyle</code> API instead.)
		/// </summary>
		public CellCollection Cells
		{
			get
			{
				if (this.cellCollection == null)
				{
					this.cellCollection = new CellCollection(this);
				}
				return this.cellCollection;
			}
		}

		/// <summary>
		/// Collection of cell returned from range or worksheet instance.
		/// </summary>
		public class CellCollection : IEnumerable<Cell>
		{
			private Worksheet grid;
			private ReferenceRange range;

			internal CellCollection(Worksheet grid)
				: this(grid, null)
			{
			}

			internal CellCollection(Worksheet grid, ReferenceRange range)
			{
				this.grid = grid;
				this.range = range;
			}

			/// <summary>
			/// Get cell instance by speicified reference from an address or name.
			/// </summary>
			/// <param name="addressOrName">Reference from an address or name.</param>
			/// <returns>Instance for cell.</returns>
			public Cell this[string addressOrName]
			{
				get
				{
					if (CellPosition.IsValidAddress(addressOrName))
					{
						return grid.CreateAndGetCell(new CellPosition(addressOrName));
					}
					else if (RGUtility.IsValidName(addressOrName))
					{
						NamedRange refRange;
						if (grid.registeredNamedRanges.TryGetValue(addressOrName, out refRange))
						{
							return grid.CreateAndGetCell(refRange.StartPos);
						}
					}
					return null;
				}
			}

			/// <summary>
			/// Get cell instance by specified number of row and column
			/// </summary>
			/// <param name="row">number of row to get cell instance</param>
			/// <param name="col">number of column to get cell instance</param>
			/// <returns>instance for cell</returns>
			public Cell this[int row, int col]
			{
				get
				{
					return this.grid.CreateAndGetCell(row, col);
				}
			}

			/// <summary>
			/// Get cell instance by specified position
			/// </summary>
			/// <param name="pos">position to get cell instance</param>
			/// <returns>instance for cell</returns>
			public Cell this[CellPosition pos]
			{
				get
				{
					return this[pos.Row, pos.Col];
				}
			}

			private IEnumerator<Cell> GetEnum()
			{
				if (this.grid == null)
				{
					throw new ReferenceObjectNotAssociatedException("Collection of cells must be associated with an valid ReoGrid control.");
				}

				var fixedRange = this.range == null ? RangePosition.EntireRange : this.grid.FixRange(this.range);

				for (int r = fixedRange.Row; r <= fixedRange.EndRow; r++)
				{
					for (int c = fixedRange.Col; c <= fixedRange.EndCol; c++)
					{
						var cell = this.grid.cells[r, c];
						if (cell == null)
						{
							yield return this.grid.CreateCell(r, c);
							continue;
						}
						else
						{
							if (!cell.IsValidCell)
							{
								continue;
							}
							else
							{
								yield return this.grid.CreateAndGetCell(r, c);
							}
						}
					}
				}
			}

			/// <summary>
			/// Get enumerator
			/// </summary>
			/// <returns></returns>
			public IEnumerator<Cell> GetEnumerator()
			{
				return GetEnum();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnum();
			}
		}

		#endregion // Cell Collection

		#endregion // Cell & Grid Management

		#region Mouse
		internal OperationStatus operationStatus = OperationStatus.Default;
		internal int currentColWidthChanging = -1;
		internal int currentRowHeightChanging = -1;

		internal int pageBreakAdjustFocusIndex = -1;
		internal int pageBreakAdjustRow = -1;
		internal int pageBreakAdjustCol = -1;

		internal Cell mouseCapturedCell = null;

		internal RGFloat headerAdjustNewValue = 0;

		internal Point lastMouseMoving = new Point(-1, -1);
		internal RangePosition lastChangedSelectionRange = RangePosition.Empty;
		internal RangePosition draggingSelectionRange = RangePosition.Empty;
		internal CellPosition focusMovingRangeOffset = CellPosition.Empty;

		#region OnMouseWheel
		internal void OnMouseWheel(Point location, int delta, MouseButtons buttons)
		{
			this.waitingEndDirection = false;


#if WINFORM || WPF
			if (Toolkit.IsKeyDown(Win32.VKey.VK_CONTROL))
			{
				if (this.HasSettings(WorksheetSettings.Behavior_MouseWheelToZoom))
				{
					SetScale(this.ScaleFactor + 0.001f * delta);
				}
			}
			else
#endif // WINFORM || WPF
			{
				if (!this.selStart.IsEmpty)
				{
					var cell = cells[this.selStart.Row, this.selStart.Col];

					RGFloat scale = this.renderScaleFactor;

					Rectangle rc = this.GetCellBounds(this.selStart);
					rc.X *= scale;
					rc.Y *= scale;
					rc.Width *= scale;
					rc.Height *= scale;

					var rp = new Point(location.X - rc.X, location.Y - rc.Y);

					var cellWheelEvent = new CellMouseEventArgs(this, cell, this.selStart, rp, location, buttons, 0)
					{
						Delta = delta,
						CellPosition = this.selStart,
					};

					if (cell != null && cell.body != null)
					{
						cell.body.OnMouseWheel(cellWheelEvent);
					}
				}

				if (this.controlAdapter != null && this.HasSettings(WorksheetSettings.Behavior_MouseWheelToScroll))
				{
					if (this.viewportController is IScrollableViewportController svc)
					{
#if WINFORM || WPF
						if (Toolkit.IsKeyDown(Win32.VKey.VK_SHIFT))
						{
							svc.ScrollOffsetViews(ScrollDirection.Horizontal, -delta, 0);
						}
						else
						{
							svc.ScrollOffsetViews(ScrollDirection.Vertical, 0, -delta);
						}
#else
							svc.ScrollViews(ScrollDirection.Vertical, 0, -delta);
#endif // WINFORM || WPF
					}
				}
			}
		}
		#endregion // OnMouseWheel

		#region OnMouseDoubleClick
		/// <summary>
		/// When a cell body has procesed any mousedown event,
		/// this flag is used to notify the Control to ignore the double click event
		/// </summary>
		internal bool IgnoreMouseDoubleClick { get; set; }

		internal void OnMouseDoubleClick(Point location, MouseButtons buttons)
		{
			if (
				// current sheet is not a in-memory worksheet
				this.viewportController != null

				// make sure no request to cancel double click once
				&& !IgnoreMouseDoubleClick

				// do not start cell if selection mode is null
				&& this.selectionMode != WorksheetSelectionMode.None
			)
			{
				this.viewportController.OnMouseDoubleClick(location, buttons);
			}

			// only ignore once if there is a request to ignore double click
			IgnoreMouseDoubleClick = false;
		}
		#endregion // OnMouseDoubleClick

		#region Mouse Events

		/// <summary>
		/// Event raised when mouse pointer moved into any cells
		/// </summary>
		public event EventHandler<CellMouseEventArgs> CellMouseEnter;

		/// <summary>
		/// Event rasied when mouse pointer moved out from any cells
		/// </summary>
		public event EventHandler<CellMouseEventArgs> CellMouseLeave;

		/// <summary>
		/// Event raised when mouse moving over all cells
		/// </summary>
		public event EventHandler<CellMouseEventArgs> CellMouseMove;

		/// <summary>
		/// Event raised after mouse button pressed down on spreadsheet
		/// </summary>
		public event EventHandler<CellMouseEventArgs> CellMouseDown;

		/// <summary>
		/// Event raised after mouse button released up on spreadsheet
		/// </summary>
		public event EventHandler<CellMouseEventArgs> CellMouseUp;

		internal bool HasCellMouseDown { get { return this.CellMouseDown != null; } }
		internal void RaiseCellMouseDown(CellMouseEventArgs args) { if (CellMouseDown != null) { CellMouseDown(this, args); } }

		internal bool HasCellMouseUp { get { return this.CellMouseUp != null; } }
		internal void RaiseCellMouseUp(CellMouseEventArgs args) { if (CellMouseUp != null) { CellMouseUp(this, args); } }

		internal bool HasCellMouseMove { get { return this.CellMouseMove != null; } }
		internal void RaiseCellMouseMove(CellMouseEventArgs args) { if (CellMouseMove != null) { CellMouseMove(this, args); } }

		internal void RaiseSelectionRangeChanged(RangeEventArgs args) => SelectionRangeChanged?.Invoke(this, args);

		#endregion // Mouse Events

		/// <summary>
		/// Get current focused visual object.
		/// </summary>
		public IUserVisual FocusVisual { get => this.viewportController?.FocusVisual; }

		#endregion // Mouse

		#region Keyboard
		private bool waitingEndDirection = false;

		internal bool OnKeyDown(KeyCode keyData)
		{
			if (!FocusPos.IsEmpty && this.BeforeCellKeyDown != null)
			{
				var args = new BeforeCellKeyDownEventArgs
				{
					Cell = GetCell(this.selStart),
					CellPosition = this.selStart,
					KeyCode = (KeyCode)keyData
				};

				this.BeforeCellKeyDown(this, args);

				if (args.IsCancelled) return true;
			}

#if EX_SCRIPT
			object rs = RaiseScriptEvent("onkeydown", new RSKeyEvent((int)keyData));
			if (rs != null && !ScriptRunningMachine.GetBoolValue(rs))
			{
				return true;
			}
#endif

			bool isProcessed = false;

			if (
				// is not in editing
				!IsEditing

				// must has an valid focus cell
				&& !this.selStart.IsEmpty)
			{
				var cell = cells[this.selStart.Row, this.selStart.Col];
				if (cell != null && cell.body != null)
				{
					isProcessed = cell.body.OnKeyDown(keyData);
					if (isProcessed) this.RequestInvalidate();
				}
			}

			bool endDirection = this.waitingEndDirection;

			// when single shift/ctrl is pressed,
			// two-step waiting mode will continue
			if (keyData != (KeyCode.ShiftKey | KeyCode.Shift)
				&& keyData != (KeyCode.ControlKey | KeyCode.Control))
			{
				// any key unless shift/ctrl is pressed,
				// quit from two-step waiting mode
				this.waitingEndDirection = false;
			}

			if (!isProcessed)
			{
				isProcessed = true;

				switch (keyData)
				{
					#region Up/Down/Left/Right
					case KeyCode.Up:
						if (endDirection)
							MoveSelectionHome(RowOrColumn.Row, false);
						else
							MoveSelectionUp();
						break;
					case KeyCode.Down:
						if (endDirection)
							MoveSelectionEnd(RowOrColumn.Row, false);
						else
							MoveSelectionDown();
						break;
					case KeyCode.Left:
						if (endDirection)
							MoveSelectionHome(RowOrColumn.Column, false);
						else
							MoveSelectionLeft();
						break;
					case KeyCode.Right:
						if (endDirection)
							MoveSelectionEnd(RowOrColumn.Column, false);
						else
							MoveSelectionRight();
						break;
					#endregion // Up/Down/Left/Right

					#region Shift+ UP/Down/Left/Right
					case KeyCode.Up | KeyCode.Shift:
						if (this.selectionMode != WorksheetSelectionMode.None && selEnd.Row > 0)
						{
							if (endDirection)
							{
								MoveSelectionHome(RowOrColumn.Row, true);
							}
							else
							{
								MoveSelectionUp(true);
							}
						}
						break;
					case KeyCode.Down | KeyCode.Shift:
						if (this.selectionMode != WorksheetSelectionMode.None && selEnd.Row < this.rows.Count - 1)
						{
							if (endDirection)
							{
								MoveSelectionEnd(RowOrColumn.Row, true);
							}
							else
							{
								MoveSelectionDown(true);
							}
						}
						break;
					case KeyCode.Left | KeyCode.Shift:
						if (this.selectionMode != WorksheetSelectionMode.None && selEnd.Col > 0)
						{
							if (endDirection)
							{
								MoveSelectionHome(RowOrColumn.Column, true);
							}
							else
							{
								MoveSelectionLeft(true);
							}
						}
						break;
					case KeyCode.Right | KeyCode.Shift:
						if (this.selectionMode != WorksheetSelectionMode.None && selEnd.Col < this.cols.Count - 1)
						{
							if (endDirection)
							{
								MoveSelectionEnd(RowOrColumn.Column, true);
							}
							else
							{
								MoveSelectionRight(true);
							}
						}
						break;
					#endregion // Shift+ UP/Down/Left/Right

					#region Shift+Ctrl+ Up/Down/Left/Right
					case KeyCode.Control | KeyCode.Up:
						MoveSelectionHome(RowOrColumn.Row);
						break;

					case KeyCode.Control | KeyCode.Shift | KeyCode.Up:
						MoveSelectionHome(RowOrColumn.Row, true);
						break;

					case KeyCode.Control | KeyCode.Down:
						MoveSelectionEnd(RowOrColumn.Row);
						break;

					case KeyCode.Control | KeyCode.Shift | KeyCode.Down:
						MoveSelectionEnd(RowOrColumn.Row, true);
						break;

					case KeyCode.Control | KeyCode.Left:
						MoveSelectionHome(RowOrColumn.Column);
						break;

					case KeyCode.Control | KeyCode.Shift | KeyCode.Left:
						MoveSelectionHome(RowOrColumn.Column, true);
						break;

					case KeyCode.Control | KeyCode.Right:
						MoveSelectionEnd(RowOrColumn.Column);
						break;

					case KeyCode.Control | KeyCode.Shift | KeyCode.Right:
						MoveSelectionEnd(RowOrColumn.Column, true);
						break;
					#endregion // Shift+Ctrl+ Up/Down/Left/Right

					#region (Ctrl+) (Shift+) Home
					case KeyCode.Home:
						MoveSelectionHome(RowOrColumn.Column);
						break;

					case KeyCode.Control | KeyCode.Home:
						MoveSelectionHome(RowOrColumn.Both);
						break;

					case KeyCode.Shift | KeyCode.Home:
						MoveSelectionHome(RowOrColumn.Column, true);
						break;

					case KeyCode.Shift | KeyCode.Control | KeyCode.Home:
						MoveSelectionHome(RowOrColumn.Both, true);
						break;
					#endregion // (Ctrl+) (Shift+) Home

					#region (Ctrl+) (Shift+) End
					case KeyCode.End:
					case KeyCode.Shift | KeyCode.End:
						this.waitingEndDirection = true;
						break;

					case KeyCode.Control | KeyCode.End:
						MoveSelectionEnd(RowOrColumn.Both);
						break;

					case KeyCode.Shift | KeyCode.Control | KeyCode.End:
						MoveSelectionEnd(RowOrColumn.Both, true);
						break;
					#endregion // (Ctrl+) (Shift+) End

					#region (Shift+) Page Down/Up
					case KeyCode.PageDown:
						MoveSelectionPageDown(false);
						break;

					case KeyCode.PageDown | KeyCode.Shift:
						MoveSelectionPageDown(true);
						break;

					case KeyCode.PageUp:
						MoveSelectionPageUp(false);
						break;

					case KeyCode.PageUp | KeyCode.Shift:
						MoveSelectionPageUp(true);
						break;
					#endregion // (Shift+) Page Down/Up

					#region Ctrl+ C/X/V
					case KeyCode.Control | KeyCode.C:
						Copy();
						break;
					case KeyCode.Control | KeyCode.X:
						Cut();
						break;
					case KeyCode.Control | KeyCode.V:
						Paste();
						break;
					#endregion // Ctrl+ C/X/V

					#region (Shift+) Tab
					case KeyCode.Tab:
						OnTabKeyPressed(false);
						break;
					case KeyCode.Shift | KeyCode.Tab:
						OnTabKeyPressed(true);
						break;
					#endregion // (Shift+) Tab/Enter

					#region (Shift+) Enter
					case KeyCode.Enter:
						OnEnterKeyPressed(false);
						break;
					case KeyCode.Shift | KeyCode.Enter:
						OnEnterKeyPressed(true);
						break;
					#endregion // (Shift+) Tab/Enter

					#region F2 / F4 / Delete/ Back
					case KeyCode.F2:
						StartEdit();
						break;

					case KeyCode.F4:
						if (this.controlAdapter != null)
						{
							var actionSupportedControl = this.controlAdapter.ControlInstance as IActionControl;
							if (actionSupportedControl != null)
							{
								actionSupportedControl.RepeatLastAction(selectionRange);
							}
						}
						break;

					case KeyCode.Delete:
						if (this.controlAdapter != null && !HasSettings(WorksheetSettings.Edit_Readonly))
						{
							var actionSupportedControl = this.controlAdapter.ControlInstance as IActionControl;
							if (actionSupportedControl != null)
							{
								actionSupportedControl.DoAction(this, new RemoveRangeDataAction(this.selectionRange));
							}
						}
						break;

					case KeyCode.Back:
						if (!this.selStart.IsEmpty)
						{
							StartEdit(this.selStart);
							this.controlAdapter.SetEditControlText(string.Empty);
						}
						break;
					#endregion // F2 / F4 / Delete/ Back

					#region Undo/Redo
					case KeyCode.Control | KeyCode.Z:
						if (this.controlAdapter != null && !HasSettings(WorksheetSettings.Edit_Readonly))
						{
							var actionSupportedControl = this.controlAdapter.ControlInstance as IActionControl;
							if (actionSupportedControl != null)
							{
								actionSupportedControl.Undo();
							}
						}
						break;
					case KeyCode.Control | KeyCode.Y:
						if (this.controlAdapter != null && !HasSettings(WorksheetSettings.Edit_Readonly))
						{
							var actionSupportedControl = this.controlAdapter.ControlInstance as IActionControl;
							if (actionSupportedControl != null)
							{
								actionSupportedControl.Redo();
							}
						}
						break;
					#endregion // Undo/Redo

					#region Zoom
					case KeyCode.Control | KeyCode.Oemplus:
						if (this.HasSettings(WorksheetSettings.Behavior_ShortcutKeyToZoom)) ZoomIn();
						break;
					case KeyCode.Control | KeyCode.OemMinus:
						if (this.HasSettings(WorksheetSettings.Behavior_ShortcutKeyToZoom)) ZoomOut();
						break;
					case KeyCode.Control | KeyCode.D0:
						if (this.HasSettings(WorksheetSettings.Behavior_ShortcutKeyToZoom)) ZoomReset();
						break;
					#endregion // Zoom

					#region Select All
					case KeyCode.Control | KeyCode.A:
						SelectAll();
						break;
					#endregion // Select All

					default:
						isProcessed = false;
						break;
				}

				if (!this.selStart.IsEmpty && this.AfterCellKeyDown != null)
				{
					var arg = new AfterCellKeyDownEventArgs
					{
						Cell = GetCell(this.selStart),
						CellPosition = this.selStart,
						KeyCode = keyData,
					};

					this.AfterCellKeyDown(this, arg);
				}
			}

			return isProcessed;
		}

		internal bool OnKeyUp(KeyCode keyData)
		{
#if EX_SCRIPT
			object rs = RaiseScriptEvent("onkeyup", new RSKeyEvent((int)keyData));
			if (rs != null && !ScriptRunningMachine.GetBoolValue(rs))
			{
				return false; 
			}
#endif

			if (!this.selStart.IsEmpty &&
				// if there is request to cancel notify to cell body about this KeyUp event
				// do not pass key up to cell body.
				// sometimes this raised when an Escape key is received to cancel editing.
				// see KeyDown event of EditTextBox.
				!DropKeyUpAfterEndEdit)
			{
				var cell = cells[this.selStart.Row, this.selStart.Col];
				if (cell != null && cell.body != null)
				{
					bool processed = cell.body.OnKeyUp(keyData);
					if (processed) this.RequestInvalidate();
				}
			}

			// ignore to pass KeyUp to cell body only once
			DropKeyUpAfterEndEdit = false;

			this.CellKeyUp?.Invoke(this, new CellKeyDownEventArgs
			{
				Cell = GetCell(this.selStart),
				CellPosition = this.selStart,
				KeyCode = keyData,
			});

			return true;
		}

		/// <summary>
		/// Sometimes when in editing mode, the Escape key used to cancel editing,
		/// The keyUp event of Escape to cancel editing should be ignored to pass to cell body.
		/// When this flag is true, the KeyUp event notify to the cell body will be ignored once.
		/// </summary>
		internal bool DropKeyUpAfterEndEdit { get; set; }

		/// <summary>
		/// Event raised before key pressed down on spreadsheet
		/// </summary>
		public event EventHandler<BeforeCellKeyDownEventArgs> BeforeCellKeyDown;

		/// <summary>
		/// Event raised after key pressed down on spreadsheet
		/// </summary>
		public event EventHandler<AfterCellKeyDownEventArgs> AfterCellKeyDown;

		/// <summary>
		/// Event raised after key released up on spreadsheet
		/// </summary>
		public event EventHandler<CellKeyDownEventArgs> CellKeyUp;
		#endregion // Keyboard

		#region Internal Utilites
		internal void DoAction(BaseWorksheetAction action)
		{
			if (this.workbook == null)
			{
				throw new InvalidOperationException("Worksheet need to be added into workbook before doing actions.");
			}

			var actionSupportedControl = this.controlAdapter == null ? null
				: (this.controlAdapter.ControlInstance as IActionControl);

			if (actionSupportedControl != null)
			{
				actionSupportedControl.DoAction(this, action);
			}
		}

		#endregion // Internal Utilites

		#region Pick Range & Style Brush
#if WINFORM || WPF
		internal Func<Worksheet, RangePosition, bool> whenRangePicked;

		internal void PickRange(Func<Worksheet, RangePosition, bool> onPicked)
		{
			this.whenRangePicked = onPicked;
		}

		internal void EndPickRange()
		{
			if (this.whenRangePicked != null)
			{
				this.whenRangePicked = null;

				if (this.controlAdapter != null)
				{
					var pickSupportedControl = (this.controlAdapter.ControlInstance as IRangePickableControl);

					if (pickSupportedControl != null)
					{
						pickSupportedControl.EndPickRange();
					}
				}
			}
		}

		/// <summary>
		/// Start to pick a range and copy the style from selected range.
		/// </summary>
		public void StartPickRangeAndCopyStyle()
		{
			RangePosition fromRange = selectionRange;

			var pickSupportedControl = (this.controlAdapter.ControlInstance as IRangePickableControl);

			if (pickSupportedControl != null)
			{
				pickSupportedControl.PickRange((grid, range) =>
				{
					Cell fromCell = GetCell(fromRange.Row, fromRange.Col);

					if (fromCell != null && fromCell.InnerStyle != null)
					{
						Cell toCell = CreateAndGetCell(range.StartPos);

						// todo: copy and merge targer range
						WorksheetReusableActionGroup actionGroup = new WorksheetReusableActionGroup(range);
						actionGroup.Actions.Add(new SetRangeStyleAction(range, fromCell.InnerStyle));
						actionGroup.Actions.Add(new SetRangeDataFormatAction(range, fromCell.DataFormat, fromCell.DataFormatArgs));

						DoAction(actionGroup);
					}

					return !Toolkit.IsKeyDown(Win32.VKey.VK_CONTROL);
				});
			}
		}
#endif // WINFORM || WPF
		#endregion // Pick Range & Style Brush

		#region Error Notification

		/// <summary>
		/// Event raised when control resetted
		/// </summary>
		public event EventHandler Resetted;

		internal void NotifyExceptionHappen(Exception ex)
		{
			if (this.workbook != null)
			{
				this.workbook.NotifyExceptionHappen(this, ex);
			}
		}

		#endregion // Error Notification

		#region Settings
		internal WorksheetSettings settings;

		//[DefaultValue(WorksheetSettings.Default)]
		//internal WorksheetSettings Settings { get { return settings; } set { this.settings = value; } }

		/// <summary>
		/// Enable control settings
		/// </summary>
		/// <param name="settings">Setting flags to be set</param>
		public void EnableSettings(WorksheetSettings settings)
		{
			SetSettings(settings, true);
		}

		/// <summary>
		/// Disable control settings
		/// </summary>
		/// <param name="settings">Settings to be removed</param>
		public void DisableSettings(WorksheetSettings settings)
		{
			SetSettings(settings, false);
		}

		/// <summary>
		/// Set control settings
		/// </summary>
		/// <param name="settings">Setting flags to be set</param>
		/// <param name="value">value of setting to be set</param>
		public void SetSettings(WorksheetSettings settings, bool value)
		{
			if (value)
			{
				this.settings |= settings;
			}
			else
			{
				this.settings &= ~settings;
			}

			if (this.viewportController != null)
			{
				if ((settings & WorksheetSettings.View_ShowColumnHeader) == WorksheetSettings.View_ShowColumnHeader
					|| (settings & WorksheetSettings.View_ShowRowHeader) == WorksheetSettings.View_ShowRowHeader)
				{
					ViewTypes head = ViewTypes.None;

					if ((settings & WorksheetSettings.View_ShowColumnHeader) == WorksheetSettings.View_ShowColumnHeader)
						head |= ViewTypes.ColumnHeader;
					if ((settings & WorksheetSettings.View_ShowRowHeader) == WorksheetSettings.View_ShowRowHeader)
						head |= ViewTypes.RowHeader;

					this.viewportController.SetViewVisible(head, value);
				}

#if OUTLINE
				if (settings.Has(WorksheetSettings.View_AllowShowRowOutlines)
					&& this.outlines != null && this.outlines[RowOrColumn.Row] != null)
				{
					this.viewportController.SetViewVisible(ViewTypes.RowOutline, value);
				}

				if (settings.HasAny(WorksheetSettings.View_AllowShowColumnOutlines)
					&& this.outlines != null && this.outlines[RowOrColumn.Column] != null)
				{
					this.viewportController.SetViewVisible(ViewTypes.ColOutline, value);
				}
#endif // OUTLINE
			}

#if PRINT
			if (settings.Has(WorksheetSettings.View_ShowPageBreaks) && value)
			{
				AutoSplitPage();
			}
#endif // PRINT

			if (this.SettingsChanged != null)
			{
				this.SettingsChanged(this, new SettingsChangedEventArgs()
				{
					AddedSettings = value ? settings : WorksheetSettings.None,
					RemovedSettings = !value ? settings : WorksheetSettings.None,
				});
			}

			if (this.controlAdapter != null)
			{
				this.UpdateViewportControllBounds();

				this.RequestInvalidate();
			}
		}

		/// <summary>
		/// Determine whether specified settings have been set.
		/// </summary>
		/// <param name="setting">Setting flags to be checked.</param>
		/// <returns>True if all settings has setted.</returns>
		public bool HasSettings(WorksheetSettings setting)
		{
			return (this.settings & setting) == setting;
		}

		/// <summary>
		/// Event raisd when worksheet settings is changed.
		/// </summary>
		public event EventHandler<SettingsChangedEventArgs> SettingsChanged;

		private float indentSize = 8;

		/// <summary>
		/// Get or set cell's text indent size.
		/// </summary>
		public float IndentSize { get { return this.indentSize; } set { this.indentSize = value; } }

		#endregion

		#region Export

		/// <summary>
		/// Export spreadsheet as html into specified stream
		/// </summary>
		/// <param name="s">Stream is used to write html content</param>
		public void ExportAsHTML(Stream s)
		{
			ExportAsHTML(s, null);
		}

		/// <summary>
		/// Export spreadsheet as html into specified stream
		/// </summary>
		/// <param name="s">Stream is used to write html content</param>
		/// <param name="pageTitle">A string will be printed out to the html as page title</param>
		/// <param name="exportHeader">true to export the html headers, false to export content only inside table tag.</param>
		public void ExportAsHTML(Stream s, string pageTitle, bool exportHeader = true)
		{
			RGHTMLExporter.Export(s, this, string.IsNullOrEmpty(pageTitle) ? "Exported ReoGrid" : pageTitle, exportHeader);
		}

		#endregion // Export

		/// <summary>
		/// Convert to friendly string.
		/// </summary>
		/// <returns>Friendly string.</returns>
		public override string ToString()
		{
			return string.Format("Worksheet[{0}]", this.name);
		}

		/// <summary>
		/// Dispose worksheet, release all attached resources.
		/// </summary>
		public void Dispose()
		{
			if (this.workbook != null)
			{
				if (this.workbook.worksheets.Contains(this))
				{
					throw new InvalidOperationException("Cannot dispose a worksheet that is still contained in a workbook. Remove from workbook and try again.");
				}
			}

			this.workbook = null;

			this.rows.Clear();
			this.cols.Clear();

			this.rowHeaderCollection = null;
			this.colHeaderCollection = null;

			this.cells.Reset();
			this.hBorders.Reset();
			this.vBorders.Reset();

			this.cellDataComparer = null;
			this.controlAdapter = null;

#if OUTLINE
			if (this.outlines != null)
			{
				this.outlines.Clear();
			}
			this.rowOutlineCollection = null;
			this.columnOutlineCollection = null;
#endif // OUTLINE

#if PRINT
			if (this.pageBreakCols != null) this.pageBreakCols.Clear();
			if (this.pageBreakRows != null) this.pageBreakRows.Clear();
			if (this.userPageBreakCols != null) this.userPageBreakCols.Clear();
			if (this.userPageBreakRows != null) this.userPageBreakRows.Clear();
			this.pageBreakRowCollection = null;
			this.pageBreakColumnCollection = null;
#endif // PRINT

#if FORMULA
			if (this.formulaRanges != null)
			{
				this.formulaRanges.Clear();
			}
#endif // FORMULA

#if DRAWING
			if (this.drawingCanvas != null)
			{
				this.drawingCanvas.Clear();
			}
#endif // DRAWING
		}
	}

}
