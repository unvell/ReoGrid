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
using System.Linq;
using System.Text;

using unvell.ReoGrid.Events;
using unvell.ReoGrid.Actions;
using unvell.ReoGrid.Main;
using unvell.ReoGrid.Interaction;

#if WINFORM
using DataObject = System.Windows.Forms.DataObject;
using Clipboard = System.Windows.Forms.Clipboard;
#elif WPF
using DataObject = System.Windows.DataObject;
using Clipboard = System.Windows.Clipboard;
#endif // WINFORM

#if EX_SCRIPT
using unvell.ReoScript;
using unvell.ReoGrid.Script;
#endif // EX_SCRIPT

namespace unvell.ReoGrid
{
	partial class Worksheet
	{
		private static readonly string ClipBoardDataFormatIdentify = "{CB3BE3D1-2BF9-4fa6-9B35-374F6A0412CE}";

		private RangePosition currentCopingRange = RangePosition.Empty;

		public string StringifyRange(string addressOrName)
		{
			if (RangePosition.IsValidAddress(addressOrName))
			{
				return this.StringifyRange(new RangePosition(addressOrName));
			}
			else if (this.registeredNamedRanges.TryGetValue(addressOrName, out var namedRange))
			{
				return this.StringifyRange(namedRange);
			}
			else
				throw new InvalidAddressException(addressOrName);
		}

		/// <summary>
		/// Convert all data from specified range to a tabbed string.
		/// </summary>
		/// <param name="range">The range to be converted.</param>
		/// <returns>Tabbed string contains all data converted from specified range.</returns>
		public string StringifyRange(RangePosition range)
		{
			int erow = range.EndRow;
			int ecol = range.EndCol;

			// copy plain text
			StringBuilder sb = new StringBuilder();

			bool isFirst = true;
			for (int r = range.Row; r <= erow; r++)
			{
				if (isFirst) isFirst = false; else sb.Append('\n');

				bool isFirst2 = true;
				for (int c = range.Col; c <= ecol; c++)
				{
					if (isFirst2) isFirst2 = false; else sb.Append('\t');

					var cell = this.cells[r, c];
					if (cell != null)
					{
						var text = cell.DisplayText;

						if (!string.IsNullOrEmpty(text))
						{
							if (text.Contains('\n'))
							{
								text = string.Format("\"{0}\"", text);
							}

							sb.Append(text);
						}
					}
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Paste data from tabbed string into worksheet.
		/// </summary>
		/// <param name="address">Start cell position to be filled.</param>
		/// <param name="content">Data to be pasted.</param>
		/// <returns>Range position that indicates the actually filled range.</returns>
		public RangePosition PasteFromString(string address, string content)
		{
			if (!CellPosition.IsValidAddress(address))
			{
				throw new InvalidAddressException(address);
			}

			return this.PasteFromString(new CellPosition(address), content);
		}

		/// <summary>
		/// Paste data from tabbed string into worksheet.
		/// </summary>
		/// <param name="startPos">Start position to fill data.</param>
		/// <param name="content">Tabbed string to be pasted.</param>
		/// <returns>Range position that indicates the actually filled range.</returns>
		public RangePosition PasteFromString(CellPosition startPos, string content)
		{
			//int rows = 0, cols = 0;

			//string[] lines = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);
			//for (int r = 0; r < lines.Length; r++)
			//{
			//	string line = lines[r];
			//	if (line.EndsWith("\n")) line = line.Substring(0, line.Length - 1);
			//	//line = line.Trim();

			//	if (line.Length > 0)
			//	{
			//		string[] tabs = line.Split('\t');
			//		cols = Math.Max(cols, tabs.Length);

			//		for (int c = 0; c < tabs.Length; c++)
			//		{
			//			int toRow = startPos.Row + r;
			//			int toCol = startPos.Col + c;

			//			if (!this.IsValidCell(toRow, toCol))
			//			{
			//				throw new RangeIntersectionException(new RangePosition(toRow, toCol, 1, 1));
			//			}

			//			string text = tabs[c];

			//			if (text.StartsWith("\"") && text.EndsWith("\""))
			//			{
			//				text = text.Substring(1, text.Length - 2);
			//			}

			//			SetCellData(toRow, toCol, text);
			//		}

			//		rows++;
			//	}
			//}

			object[,] parsedData = RGUtility.ParseTabbedString(content);

			int rows = parsedData.GetLength(0);
			int cols = parsedData.GetLength(1);

			var range = new RangePosition(startPos.Row, startPos.Col, rows, cols);

			this.SetRangeData(range, parsedData);

			return range;
		}

		#region Copy

		/// <summary>
		/// Copy data and put into Clipboard.
		/// </summary>
		public bool Copy()
		{
			if (IsEditing)
			{
				this.controlAdapter.EditControlCopy();
			}
			else
			{
				this.controlAdapter.ChangeCursor(CursorStyle.Busy);

				try
				{
					if (BeforeCopy != null)
					{
						var evtArg = new BeforeRangeOperationEventArgs(selectionRange);
						BeforeCopy(this, evtArg);
						if (evtArg.IsCancelled)
						{
							return false;
						}
					}

#if EX_SCRIPT
					var scriptReturn = RaiseScriptEvent("oncopy");
					if (scriptReturn != null && !ScriptRunningMachine.GetBoolValue(scriptReturn))
					{
						return false;
					}
#endif // EX_SCRIPT

					// highlight current copy range
					currentCopingRange = selectionRange;

#if WINFORM || WPF
					DataObject data = new DataObject();
					data.SetData(ClipBoardDataFormatIdentify,
						GetPartialGrid(currentCopingRange, PartialGridCopyFlag.All, ExPartialGridCopyFlag.None, true));

					string text = StringifyRange(currentCopingRange);
					if (!string.IsNullOrEmpty(text)) data.SetText(text);

					// set object data into clipboard
					Clipboard.SetDataObject(data);
#endif // WINFORM || WPF

					if (AfterCopy != null)
					{
						AfterCopy(this, new RangeEventArgs(this.selectionRange));
					}
				}
				catch (Exception ex)
				{
					this.NotifyExceptionHappen(ex);
					return false;
				}
				finally
				{
					this.controlAdapter.ChangeCursor(CursorStyle.PlatformDefault);
				}

			}

			return true;
		}

		#endregion // Copy

		#region Paste

		/// <summary>
		/// Copy data from Clipboard and put on grid.
		/// 
		/// Currently ReoGrid supports the following types of source from the clipboard.
		///  - Data from another ReoGrid instance
		///  - Plain/Unicode Text from any Windows Applications
		///  - Tabbed Plain/Unicode Data from Excel or similar applications
		/// 
		/// When data copied from another ReoGrid instance, and the destination range 
		/// is bigger than the source, ReoGrid will try to repeat putting data to fill 
		/// the destination range entirely.
		/// 
		/// Todo: Copy border and cell style from Excel.
		/// </summary>
		public bool Paste()
		{
			if (IsEditing)
			{
				this.controlAdapter.EditControlPaste();
			}
			else
			{
				// Paste method will always perform action to do paste

				// do nothing if in readonly mode
				if (this.HasSettings(WorksheetSettings.Edit_Readonly)
					// or selection is empty
					|| this.selectionRange.IsEmpty)
				{
					return false;
				}

				try
				{
					this.controlAdapter.ChangeCursor(CursorStyle.Busy);

					PartialGrid partialGrid = null;
					string clipboardText = null;

#if WINFORM || WPF
					DataObject data = Clipboard.GetDataObject() as DataObject;
					if (data != null)
					{
						partialGrid = data.GetData(ClipBoardDataFormatIdentify) as PartialGrid;

						if (data.ContainsText())
						{
							clipboardText = data.GetText();
						}
					}

#elif ANDROID

#endif // WINFORM || WPF

					if (partialGrid != null)
					{
						#region Partial Grid Pasting
						int startRow = selectionRange.Row;
						int startCol = selectionRange.Col;

						int rows = partialGrid.Rows;
						int cols = partialGrid.Columns;

						int rowRepeat = 1;
						int colRepeat = 1;

						if (selectionRange.Rows % partialGrid.Rows == 0)
						{
							rows = selectionRange.Rows;
							rowRepeat = selectionRange.Rows / partialGrid.Rows;
						}
						if (selectionRange.Cols % partialGrid.Columns == 0)
						{
							cols = selectionRange.Cols;
							colRepeat = selectionRange.Cols / partialGrid.Columns;
						}

						var targetRange = new RangePosition(startRow, startCol, rows, cols);

						if (!RaiseBeforePasteEvent(targetRange))
						{
							return false;
						}

						if (targetRange.EndRow >= this.rows.Count
							|| targetRange.EndCol >= this.cols.Count)
						{
							// TODO: paste range overflow
							// need to notify user-code to handle this 
							return false;
						}

						// check whether the range to be pasted contains readonly cell
						if (this.CheckRangeReadonly(targetRange))
						{
							this.NotifyExceptionHappen(new OperationOnReadonlyCellException("specified range contains readonly cell"));
							return false;
						}

						// check any intersected merge-range in partial grid 
						// 
						bool cancelPerformPaste = false;

						if (partialGrid.Cells != null)
						{
							try
							{
								#region Check repeated intersected ranges
								for (int rr = 0; rr < rowRepeat; rr++)
								{
									for (int cc = 0; cc < colRepeat; cc++)
									{
										partialGrid.Cells.Iterate((row, col, cell) =>
										{
											if (cell.IsMergedCell)
											{
												for (int r = startRow; r < cell.MergeEndPos.Row - cell.InternalRow + startRow + 1; r++)
												{
													for (int c = startCol; c < cell.MergeEndPos.Col - cell.InternalCol + startCol + 1; c++)
													{
														int tr = r + rr * partialGrid.Rows;
														int tc = c + cc * partialGrid.Columns;

														var existedCell = cells[tr, tc];

														if (existedCell != null)
														{
															if (
																// cell is a part of merged cell
																(existedCell.Rowspan == 0 && existedCell.Colspan == 0)
																// cell is merged cell
																|| existedCell.IsMergedCell)
															{
																throw new RangeIntersectionException(selectionRange);
															}
															// cell is readonly
															else if (existedCell.IsReadOnly)
															{
																throw new CellDataReadonlyException(cell.InternalPos);
															}
														}
													}
												}
											}

											return Math.Min(cell.Colspan, (short)1);
										});
									}
								}
								#endregion // Check repeated intersected ranges
							}
							catch (Exception ex)
							{
								cancelPerformPaste = true;

								// raise event to notify user-code there is error happened during paste operation
								if (OnPasteError != null)
								{
									OnPasteError(this, new RangeOperationErrorEventArgs(selectionRange, ex));
								}
							}
						}

						if (!cancelPerformPaste)
						{
							DoAction(new SetPartialGridAction(new RangePosition(
								startRow, startCol, rows, cols), partialGrid));
						}

						#endregion // Partial Grid Pasting
					}
					else if (!string.IsNullOrEmpty(clipboardText))
					{
						#region Plain Text Pasting
						var arrayData = RGUtility.ParseTabbedString(clipboardText);

						int rows = Math.Max(selectionRange.Rows, arrayData.GetLength(0));
						int cols = Math.Max(selectionRange.Cols, arrayData.GetLength(1));

						var targetRange = new RangePosition(selectionRange.Row, selectionRange.Col, rows, cols);
						if (!RaiseBeforePasteEvent(targetRange))
						{
							return false;
						}

						if (this.controlAdapter != null)
						{
							var actionSupportedControl = this.controlAdapter.ControlInstance as IActionControl;

							if (actionSupportedControl != null)
							{
								actionSupportedControl.DoAction(this, new SetRangeDataAction(targetRange, arrayData));
							}
						}
						#endregion // Plain Text Pasting
					}
				}
				catch (Exception ex)
				{
					// raise event to notify user-code there is error happened during paste operation
					//if (OnPasteError != null)
					//{
					//	OnPasteError(this, new RangeOperationErrorEventArgs(selectionRange, ex));
					//}
					this.NotifyExceptionHappen(ex);
				}
				finally
				{
					this.controlAdapter.ChangeCursor(CursorStyle.Selection);

					RequestInvalidate();
				}

				if (AfterPaste != null)
				{
					AfterPaste(this, new RangeEventArgs(this.selectionRange));
				}
			}

			return true;
		}

		private bool RaiseBeforePasteEvent(RangePosition range)
		{
			if (BeforePaste != null)
			{
				var evtArg = new BeforeRangeOperationEventArgs(range);
				BeforePaste(this, evtArg);
				if (evtArg.IsCancelled)
				{
					return false;
				}
			}

#if EX_SCRIPT
			object scriptReturn = RaiseScriptEvent("onpaste", new RSRangeObject(this, range));
			if (scriptReturn != null && !ScriptRunningMachine.GetBoolValue(scriptReturn))
			{
				return false;
			}
#endif // EX_SCRIPT

			return true;
		}

		#endregion // Paste

		#region Cut
		/// <summary>
		/// Copy any remove anything from selected range into Clipboard.
		/// </summary>
		public bool Cut()
		{
			if (IsEditing)
			{
				this.controlAdapter.EditControlCut();
			}
			else
			{
				if (!Copy()) return false;

				if (BeforeCut != null)
				{
					var evtArg = new BeforeRangeOperationEventArgs(this.selectionRange);

					BeforeCut(this, evtArg);

					if (evtArg.IsCancelled)
					{
						return false;
					}
				}

#if EX_SCRIPT
				object scriptReturn = RaiseScriptEvent("oncut");
				if (scriptReturn != null && !ScriptRunningMachine.GetBoolValue(scriptReturn))
				{
					return false;
				}
#endif

				if (!HasSettings(WorksheetSettings.Edit_Readonly))
				{
					this.DeleteRangeData(currentCopingRange);
					this.RemoveRangeStyles(currentCopingRange, PlainStyleFlag.All);
					this.RemoveRangeBorders(currentCopingRange, BorderPositions.All);
				}

				if (AfterCut != null)
				{
					AfterCut(this, new RangeEventArgs(this.selectionRange));
				}
			}

			return true;
		}
		#endregion // Cut

		//private void CheckCanPaste()
		//{
		//	// TODO
		//}

		//void ClipboardMonitor_ClipboardChanged(object sender, ClipboardChangedEventArgs e)
		//{
		//	CheckCanPaste();
		//}

		#region Checks

		/// <summary>
		/// Determine whether the selected range can be copied.
		/// </summary>
		/// <returns>True if the selected range can be copied.</returns>
		public bool CanCopy()
		{
			//TODO
			return true;
		}

		/// <summary>
		/// Determine whether the selected range can be cutted.
		/// </summary>
		/// <returns>True if the selected range can be cutted.</returns>
		public bool CanCut()
		{
			//TODO
			return true;
		}

		/// <summary>
		/// Determine whether the data contained in Clipboard can be pasted into grid control.
		/// </summary>
		/// <returns>True if the data contained in Clipboard can be pasted</returns>
		public bool CanPaste()
		{
			//TODO
			return true;
		}

		#endregion // Checks

		#region Events

		/// <summary>
		/// Before a range will be pasted from Clipboard
		/// </summary>
		public event EventHandler<BeforeRangeOperationEventArgs> BeforePaste;

		/// <summary>
		/// When a range has been pasted into grid
		/// </summary>
		public event EventHandler<RangeEventArgs> AfterPaste;

		/// <summary>
		/// When an error happened during perform paste
		/// </summary>
		[Obsolete("use ReoGridControl.ErrorHappened instead")]
		public event EventHandler<RangeOperationErrorEventArgs> OnPasteError;

		/// <summary>
		/// Before a range to be copied into Clipboard
		/// </summary>
		public event EventHandler<BeforeRangeOperationEventArgs> BeforeCopy;

		/// <summary>
		/// When a range has been copied into Clipboard
		/// </summary>
		public event EventHandler<RangeEventArgs> AfterCopy;

		/// <summary>
		/// Before a range to be moved into Clipboard
		/// </summary>
		public event EventHandler<BeforeRangeOperationEventArgs> BeforeCut;

		/// <summary>
		/// After a range to be moved into Clipboard
		/// </summary>
		public event EventHandler<RangeEventArgs> AfterCut;

		#endregion // Events
	}
}

#if WINFORM || WPF

#endif // WINFORM || WPF