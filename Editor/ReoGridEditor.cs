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
 * ReoGrid and ReoGridEditor is released under MIT license.
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

using unvell.Common;

using unvell.ReoGrid.Editor.Properties;
using unvell.ReoGrid.PropertyPages;
using unvell.ReoGrid.Actions;
using unvell.ReoGrid.CellTypes;
using unvell.ReoGrid.Events;
using unvell.ReoGrid.Data;
using unvell.ReoGrid.WinForm;
using unvell.ReoGrid.IO;
using unvell.ReoGrid.DataFormat;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Editor.LangRes;

using unvell.ReoGrid.Print;
using unvell.ReoGrid.Drawing;
using unvell.ReoGrid.Drawing.Text;

#if EX_SCRIPT
using unvell.ReoScript.Editor;
#endif // EX_SCRIPT

using Point = System.Drawing.Point;

namespace unvell.ReoGrid.Editor
{
	/// <summary>
	/// Represents Editor of ReoGrid component.
	/// </summary>
	public partial class ReoGridEditor : Form
	{
		#region Constructor

		private NamedRangeManageForm nameManagerForm = null;

		/// <summary>
		/// Create instance of ReoGrid Editor.
		/// </summary>
		public ReoGridEditor()
		{
			InitializeComponent();

			NewDocumentOnLoad = true;

			SuspendLayout();
			isUIUpdating = true;

			SetupUILanguage();

			fontToolStripComboBox.Text = Worksheet.DefaultStyle.FontName;

			fontSizeToolStripComboBox.Text = Worksheet.DefaultStyle.FontSize.ToString();
			fontSizeToolStripComboBox.Items.AddRange(FontUIToolkit.FontSizeList.Select(f => (object)f).ToArray());

			backColorPickerToolStripButton.CloseOnClick = true;
			borderColorPickToolStripItem.CloseOnClick = true;
			textColorPickToolStripItem.CloseOnClick = true;

			this.undoToolStripButton.Enabled =
				this.undoToolStripMenuItem.Enabled =
				this.redoToolStripButton.Enabled =
				this.redoToolStripMenuItem.Enabled =
				this.repeatLastActionToolStripMenuItem.Enabled =
				false;

			zoomToolStripDropDownButton.Text = "100%";

			isUIUpdating = false;

			toolbarToolStripMenuItem.Click += (s, e) => fontToolStrip.Visible = toolStrip1.Visible = toolbarToolStripMenuItem.Checked;
			formulaBarToolStripMenuItem.CheckedChanged += (s, e) => formulaBar.Visible = formulaBarToolStripMenuItem.Checked;
			statusBarToolStripMenuItem.CheckedChanged += (s, e) => statusStrip1.Visible = statusBarToolStripMenuItem.Checked;
			sheetSwitcherToolStripMenuItem.CheckedChanged += (s, e) =>
				this.grid.SetSettings(WorkbookSettings.View_ShowSheetTabControl, sheetSwitcherToolStripMenuItem.Checked);

			showHorizontaScrolllToolStripMenuItem.CheckedChanged += (s, e) =>
				this.grid.SetSettings(WorkbookSettings.View_ShowHorScroll, showHorizontaScrolllToolStripMenuItem.Checked);
			showVerticalScrollbarToolStripMenuItem.CheckedChanged += (s, e) =>
				this.grid.SetSettings(WorkbookSettings.View_ShowVerScroll, showVerticalScrollbarToolStripMenuItem.Checked);

			showGridLinesToolStripMenuItem.CheckedChanged += (s, e) =>
				this.CurrentWorksheet.SetSettings(WorksheetSettings.View_ShowGridLine, showGridLinesToolStripMenuItem.Checked);
			showPageBreakToolStripMenuItem.CheckedChanged += (s, e) =>
				this.CurrentWorksheet.SetSettings(WorksheetSettings.View_ShowPageBreaks, showPageBreakToolStripMenuItem.Checked);
			showFrozenLineToolStripMenuItem.CheckedChanged += (s, e) =>
				this.CurrentWorksheet.SetSettings(WorksheetSettings.View_ShowFrozenLine, showFrozenLineToolStripMenuItem.Checked);
			showRowHeaderToolStripMenuItem.CheckedChanged += (s, e) =>
				this.CurrentWorksheet.SetSettings(WorksheetSettings.View_ShowRowHeader, showRowHeaderToolStripMenuItem.Checked);
			showColumnHeaderToolStripMenuItem.CheckedChanged += (s, e) =>
				this.CurrentWorksheet.SetSettings(WorksheetSettings.View_ShowColumnHeader, showColumnHeaderToolStripMenuItem.Checked);
			showRowOutlineToolStripMenuItem.CheckedChanged += (s, e) =>
				this.CurrentWorksheet.SetSettings(WorksheetSettings.View_AllowShowRowOutlines, showRowOutlineToolStripMenuItem.Checked);
			showColumnOutlineToolStripMenuItem.CheckedChanged += (s, e) =>
				this.CurrentWorksheet.SetSettings(WorksheetSettings.View_AllowShowColumnOutlines, showColumnOutlineToolStripMenuItem.Checked);

			sheetReadonlyToolStripMenuItem.CheckedChanged += (s, e) =>
				this.CurrentWorksheet.SetSettings(WorksheetSettings.Edit_Readonly, sheetReadonlyToolStripMenuItem.Checked);

			resetAllPageBreaksToolStripMenuItem.Click += (s, e) => this.CurrentWorksheet.ResetAllPageBreaks();
			resetAllPageBreaksToolStripMenuItem1.Click += (s, e) => this.CurrentWorksheet.ResetAllPageBreaks();

			this.grid.WorksheetInserted += (ss, ee) =>
			{
				var worksheet = ee.Worksheet;

				worksheet.SelectionRangeChanged += grid_SelectionRangeChanged;
				worksheet.SelectionModeChanged += worksheet_SelectionModeChanged;
				worksheet.SelectionStyleChanged += worksheet_SelectionModeChanged;
				worksheet.SelectionForwardDirectionChanged += worksheet_SelectionForwardDirectionChanged;
				worksheet.FocusPosStyleChanged += worksheet_SelectionModeChanged;
				worksheet.CellsFrozen += UpdateMenuAndToolStripsWhenAction;
				worksheet.Resetted += worksheet_Resetted;
				worksheet.SettingsChanged += worksheet_SettingsChanged;
				worksheet.Scaled += worksheet_GridScaled;
			};

			this.grid.WorksheetRemoved += (ss, ee) =>
			{
				var worksheet = ee.Worksheet;

				worksheet.SelectionRangeChanged -= grid_SelectionRangeChanged;
				worksheet.SelectionModeChanged -= worksheet_SelectionModeChanged;
				worksheet.SelectionStyleChanged -= worksheet_SelectionModeChanged;
				worksheet.SelectionForwardDirectionChanged -= worksheet_SelectionForwardDirectionChanged;
				worksheet.FocusPosStyleChanged -= worksheet_SelectionModeChanged;
				worksheet.CellsFrozen -= UpdateMenuAndToolStripsWhenAction;
				worksheet.Resetted -= worksheet_Resetted;
				worksheet.SettingsChanged -= worksheet_SettingsChanged;
				worksheet.Scaled -= worksheet_GridScaled;
			};

			selModeNoneToolStripMenuItem.Click += (s, e) => this.grid.CurrentWorksheet.SelectionMode = WorksheetSelectionMode.None;
			selModeCellToolStripMenuItem.Click += (s, e) => this.grid.CurrentWorksheet.SelectionMode = WorksheetSelectionMode.Cell;
			selModeRangeToolStripMenuItem.Click += (s, e) => this.grid.CurrentWorksheet.SelectionMode = WorksheetSelectionMode.Range;
			selModeRowToolStripMenuItem.Click += (s, e) => this.grid.CurrentWorksheet.SelectionMode = WorksheetSelectionMode.Row;
			selModeColumnToolStripMenuItem.Click += (s, e) => this.grid.CurrentWorksheet.SelectionMode = WorksheetSelectionMode.Column;

			selStyleNoneToolStripMenuItem.Click += (s, e) => this.grid.CurrentWorksheet.SelectionStyle = WorksheetSelectionStyle.None;
			selStyleDefaultToolStripMenuItem.Click += (s, e) => this.grid.CurrentWorksheet.SelectionStyle = WorksheetSelectionStyle.Default;
			selStyleFocusRectToolStripMenuItem.Click += (s, e) => this.grid.CurrentWorksheet.SelectionStyle = WorksheetSelectionStyle.FocusRect;

			selDirRightToolStripMenuItem.Click += (s, e) => this.grid.CurrentWorksheet.SelectionForwardDirection = SelectionForwardDirection.Right;
			selDirDownToolStripMenuItem.Click += (s, e) => this.grid.CurrentWorksheet.SelectionForwardDirection = SelectionForwardDirection.Down;

			zoomToolStripDropDownButton.TextChanged += zoomToolStripDropDownButton_TextChanged;

			undoToolStripButton.Click += Undo;
			redoToolStripButton.Click += Redo;
			undoToolStripMenuItem.Click += Undo;
			redoToolStripMenuItem.Click += Redo;

			mergeRangeToolStripMenuItem.Click += MergeSelectionRange;
			cellMergeToolStripButton.Click += MergeSelectionRange;
			unmergeRangeToolStripMenuItem.Click += UnmergeSelectionRange;
			unmergeRangeToolStripButton.Click += UnmergeSelectionRange;
			mergeCellsToolStripMenuItem.Click += MergeSelectionRange;
			unmergeCellsToolStripMenuItem.Click += UnmergeSelectionRange;
			formatCellsToolStripMenuItem.Click += formatCellToolStripMenuItem_Click;
			resizeToolStripMenuItem.Click += resizeToolStripMenuItem_Click;
			textWrapToolStripButton.Click += textWrapToolStripButton_Click;

			// todo
			this.grid.ActionPerformed += (s, e) => UpdateMenuAndToolStripsWhenAction(s, e);
			this.grid.Undid += (s, e) => UpdateMenuAndToolStripsWhenAction(s, e);
			this.grid.Redid += (s, e) => UpdateMenuAndToolStripsWhenAction(s, e);

			rowHeightToolStripMenuItem.Click += (s, e) =>
			{
				var worksheet = this.CurrentWorksheet;

				using (SetWidthOrHeightDialog rowHeightForm = new SetWidthOrHeightDialog(RowOrColumn.Row))
				{
					rowHeightForm.Value = worksheet.GetRowHeight(worksheet.SelectionRange.Row);

					if (rowHeightForm.ShowDialog() == DialogResult.OK)
					{
						this.grid.DoAction(new SetRowsHeightAction(worksheet.SelectionRange.Row,
							worksheet.SelectionRange.Rows, (ushort)rowHeightForm.Value));
					}
				}
			};

			columnWidthToolStripMenuItem.Click += (s, e) =>
			{
				var worksheet = this.CurrentWorksheet;

				using (SetWidthOrHeightDialog colWidthForm = new SetWidthOrHeightDialog(RowOrColumn.Column))
				{
					colWidthForm.Value = worksheet.GetColumnWidth(worksheet.SelectionRange.Col);

					if (colWidthForm.ShowDialog() == DialogResult.OK)
					{
						this.grid.DoAction(new SetColumnsWidthAction(worksheet.SelectionRange.Col,
							worksheet.SelectionRange.Cols, (ushort)colWidthForm.Value));
					}
				}
			};

			exportAsHtmlToolStripMenuItem.Click += (s, e) =>
			{
				using (SaveFileDialog sfd = new SaveFileDialog())
				{
					sfd.Filter = "HTML File(*.html;*.htm)|*.html;*.htm";
					sfd.FileName = "Exported ReoGrid Worksheet";

					if (sfd.ShowDialog() == DialogResult.OK)
					{
						using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create))
						{
							this.CurrentWorksheet.ExportAsHTML(fs);
						}

						RGUtility.OpenFileOrLink(sfd.FileName);
					}
				}
			};

			editXMLToolStripMenuItem.Click += (s, e) =>
			{
				string filepath = null;

				if (string.IsNullOrEmpty(this.CurrentFilePath))
				{
					if (string.IsNullOrEmpty(currentTempFilePath))
					{
						currentTempFilePath = Path.Combine(Path.GetTempPath(),
							Path.GetFileNameWithoutExtension(Path.GetTempFileName()) + ".txt");
					}
					filepath = currentTempFilePath;
				}
				else if (!this.CurrentFilePath.EndsWith(".rgf")
					&& !this.CurrentFilePath.EndsWith(".xml"))
				{
					MessageBox.Show(LangResource.Msg_Only_RGF_Edit_XML);
					return;
				}
				else
				{
					if (MessageBox.Show(LangResource.Msg_Save_File_Immediately,
						"Edit XML", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
						== System.Windows.Forms.DialogResult.Cancel)
					{
						return;
					}

					filepath = this.CurrentFilePath;
				}

				using (var fs = new FileStream(filepath, FileMode.Create, FileAccess.Write))
				{
					this.CurrentWorksheet.Save(fs);
				}

				Process p = RGUtility.OpenFileOrLink("notepad.exe", filepath);
				p.WaitForExit();

				if (p.ExitCode == 0)
				{
					this.CurrentWorksheet.Load(filepath);
				}
			};

			saveToolStripButton.Click += (s, e) => SaveDocument();
			saveToolStripMenuItem.Click += (s, e) => SaveDocument();
			saveAsToolStripMenuItem.Click += (s, e) => SaveAsDocument();

			groupRowsToolStripMenuItem.Click += groupRowsToolStripMenuItem_Click;
			groupRowsToolStripMenuItem1.Click += groupRowsToolStripMenuItem_Click;
			ungroupRowsToolStripMenuItem.Click += ungroupRowsToolStripMenuItem_Click;
			ungroupRowsToolStripMenuItem1.Click += ungroupRowsToolStripMenuItem_Click;
			ungroupAllRowsToolStripMenuItem.Click += ungroupAllRowsToolStripMenuItem_Click;
			ungroupAllRowsToolStripMenuItem1.Click += ungroupAllRowsToolStripMenuItem_Click;

			groupColumnsToolStripMenuItem.Click += groupColumnsToolStripMenuItem_Click;
			groupColumnsToolStripMenuItem1.Click += groupColumnsToolStripMenuItem_Click;
			ungroupColumnsToolStripMenuItem.Click += ungroupColumnsToolStripMenuItem_Click;
			ungroupColumnsToolStripMenuItem1.Click += ungroupColumnsToolStripMenuItem_Click;
			ungroupAllColumnsToolStripMenuItem.Click += ungroupAllColumnsToolStripMenuItem_Click;
			ungroupAllColumnsToolStripMenuItem1.Click += ungroupAllColumnsToolStripMenuItem_Click;

			hideRowsToolStripMenuItem.Click += (s, e) => this.grid.DoAction(new HideRowsAction(
				this.CurrentWorksheet.SelectionRange.Row, this.CurrentWorksheet.SelectionRange.Rows));
			unhideRowsToolStripMenuItem.Click += (s, e) => this.grid.DoAction(new UnhideRowsAction(
				this.CurrentWorksheet.SelectionRange.Row, this.CurrentWorksheet.SelectionRange.Rows));

			hideColumnsToolStripMenuItem.Click += (s, e) => this.grid.DoAction(new HideColumnsAction(
				this.CurrentWorksheet.SelectionRange.Col, this.CurrentWorksheet.SelectionRange.Cols));
			unhideColumnsToolStripMenuItem.Click += (s, e) => this.grid.DoAction(new UnhideColumnsAction(
				this.CurrentWorksheet.SelectionRange.Col, this.CurrentWorksheet.SelectionRange.Cols));

			// freeze to cell / edges
			freezeToCellToolStripMenuItem.Click += (s, e) => FreezeToEdge(FreezeArea.LeftTop);
			freezeToLeftToolStripMenuItem.Click += (s, e) => FreezeToEdge(FreezeArea.Left);
			freezeToTopToolStripMenuItem.Click += (s, e) => FreezeToEdge(FreezeArea.Top);
			freezeToRightToolStripMenuItem.Click += (s, e) => FreezeToEdge(FreezeArea.Right);
			freezeToBottomToolStripMenuItem.Click += (s, e) => FreezeToEdge(FreezeArea.Bottom);
			freezeToLeftTopToolStripMenuItem.Click += (s, e) => FreezeToEdge(FreezeArea.LeftTop);
			freezeToLeftBottomToolStripMenuItem.Click += (s, e) => FreezeToEdge(FreezeArea.LeftBottom);
			freezeToRightTopToolStripMenuItem.Click += (s, e) => FreezeToEdge(FreezeArea.RightTop);
			freezeToRightBottomToolStripMenuItem.Click += (s, e) => FreezeToEdge(FreezeArea.RightBottom);

			grid.GotFocus += (s, e) =>
				{
					cutToolStripButton.Enabled =
					cutToolStripMenuItem.Enabled =
					pasteToolStripButton.Enabled =
					pasteToolStripMenuItem.Enabled =
					copyToolStripButton.Enabled =
					copyToolStripMenuItem.Enabled =
					undoToolStripButton.Enabled =
					undoToolStripMenuItem.Enabled =
					redoToolStripButton.Enabled =
					redoToolStripMenuItem.Enabled =
					repeatLastActionToolStripMenuItem.Enabled =
					rowCutToolStripMenuItem.Enabled =
					rowCopyToolStripMenuItem.Enabled =
					rowPasteToolStripMenuItem.Enabled =
					colCutToolStripMenuItem.Enabled =
					colCopyToolStripMenuItem.Enabled =
					colPasteToolStripMenuItem.Enabled =
						true;
				};

			grid.LostFocus += (s, e) =>
			{
				cutToolStripButton.Enabled =
				cutToolStripMenuItem.Enabled =
				pasteToolStripButton.Enabled =
				pasteToolStripMenuItem.Enabled =
				copyToolStripButton.Enabled =
				copyToolStripMenuItem.Enabled =
				undoToolStripButton.Enabled =
				undoToolStripMenuItem.Enabled =
				redoToolStripButton.Enabled =
				redoToolStripMenuItem.Enabled =
				repeatLastActionToolStripMenuItem.Enabled =
				rowCutToolStripMenuItem.Enabled =
				rowCopyToolStripMenuItem.Enabled =
				rowPasteToolStripMenuItem.Enabled =
				colCutToolStripMenuItem.Enabled =
				colCopyToolStripMenuItem.Enabled =
				colPasteToolStripMenuItem.Enabled =
					false;
			};

			defineNamedRangeToolStripMenuItem.Click += (s, e) =>
			{
				var sheet = this.CurrentWorksheet;

				var name = sheet.GetNameByRange(sheet.SelectionRange);
				NamedRange namedRange = null;

				if (!string.IsNullOrEmpty(name))
				{
					namedRange = sheet.GetNamedRange(name);
				}

				using (DefineNamedRangeDialog dnrf = new DefineNamedRangeDialog())
				{
					dnrf.Range = sheet.SelectionRange;
					if (namedRange != null)
					{
						dnrf.RangeName = name;
						dnrf.Comment = namedRange.Comment;
					}

					if (dnrf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						var newName = dnrf.RangeName;

						var existedRange = sheet.GetNamedRange(newName);
						if (existedRange != null)
						{
							if (MessageBox.Show(this, LangRes.LangResource.Msg_Named_Range_Overwrite,
								Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
								== System.Windows.Forms.DialogResult.Cancel)
							{
								return;
							}

							sheet.UndefineNamedRange(newName);
						}

						var range = NamedRangeManageForm.DefineNamedRange(this, sheet, newName, dnrf.Comment, dnrf.Range);

						if (this.formulaBar != null && this.formulaBar.Visible)
						{
							this.formulaBar.RefreshCurrentAddress();
						}
					}
				}
			};

			this.nameManagerToolStripMenuItem.Click += (s, e) =>
			{
				if (this.nameManagerForm == null || this.nameManagerForm.IsDisposed)
				{
					this.nameManagerForm = new NamedRangeManageForm(this.grid);
				}

				this.nameManagerForm.Show(this);
			};

			tracePrecedentsToolStripMenuItem.Click += (s, e) => this.CurrentWorksheet.TraceCellPrecedents(this.CurrentWorksheet.FocusPos);
			traceDependentsToolStripMenuItem.Click += (s, e) => this.CurrentWorksheet.TraceCellDependents(this.CurrentWorksheet.FocusPos);

			removeAllArrowsToolStripMenuItem.Click += (s, e) => this.CurrentWorksheet.RemoveRangeAllTraceArrows(this.CurrentWorksheet.SelectionRange);
			removePrecedentArrowsToolStripMenuItem.Click += (s, e) =>
				this.CurrentWorksheet.IterateCells(this.CurrentWorksheet.SelectionRange, (r, c, cell) =>
					this.CurrentWorksheet.RemoveCellTracePrecedents(cell));
			removeDependentArrowsToolStripMenuItem.Click += (s, e) =>
				this.CurrentWorksheet.IterateCells(this.CurrentWorksheet.SelectionRange, (r, c, cell) =>
					this.CurrentWorksheet.RemoveCellTraceDependents(cell));

			columnPropertiesToolStripMenuItem.Click += (s, e) =>
			{
				var worksheet = this.CurrentWorksheet;

				int index = worksheet.SelectionRange.Col;
				int count = worksheet.SelectionRange.Cols;

				using (var hf = new HeaderPropertyDialog(RowOrColumn.Column))
				{
					var sampleHeader = worksheet.ColumnHeaders[index];

					hf.HeaderText = sampleHeader.Text;
					hf.HeaderTextColor = sampleHeader.TextColor ?? Color.Empty;
					hf.DefaultCellBody = sampleHeader.DefaultCellBody;
					hf.AutoFitToCell = sampleHeader.IsAutoWidth;

					if (hf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						var newText = string.IsNullOrEmpty(hf.HeaderText) ? null : hf.HeaderText;

						for (int i = index; i < index + count; i++)
						{
							var header = worksheet.ColumnHeaders[i];

							if (string.IsNullOrEmpty(header.Text) || newText == null)
							{
								header.Text = newText;
							}

							header.TextColor = hf.HeaderTextColor;
							header.DefaultCellBody = hf.DefaultCellBody;
							header.IsAutoWidth = hf.AutoFitToCell;
						}
					}
				}
			};

			rowPropertiesToolStripMenuItem.Click += (s, e) =>
			{
				var sheet = this.grid.CurrentWorksheet;

				int index = sheet.SelectionRange.Row;
				int count = sheet.SelectionRange.Rows;

				using (var hpf = new HeaderPropertyDialog(RowOrColumn.Row))
				{
					var sampleHeader = sheet.RowHeaders[index];

					hpf.HeaderText = sampleHeader.Text;
					hpf.HeaderTextColor = sampleHeader.TextColor ?? Color.Empty;
					hpf.DefaultCellBody = sampleHeader.DefaultCellBody;
					hpf.RowHeaderWidth = sheet.RowHeaderWidth;
					hpf.AutoFitToCell = sampleHeader.IsAutoHeight;

					if (hpf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						var newText = string.IsNullOrEmpty(hpf.HeaderText) ? null : hpf.HeaderText;

						for (int i = index; i < index + count; i++)
						{
							var header = sheet.RowHeaders[i];

							if (string.IsNullOrEmpty(header.Text) || newText == null)
							{
								header.Text = newText;
							}

							header.TextColor = hpf.HeaderTextColor;
							header.DefaultCellBody = hpf.DefaultCellBody;
							header.IsAutoHeight = hpf.AutoFitToCell;
						}

						if (hpf.RowHeaderWidth != sheet.RowHeaderWidth)
						{
							sheet.RowHeaderWidth = hpf.RowHeaderWidth;
						}
					}
				}
			};

			rowCutToolStripMenuItem.Click += this.cutRangeToolStripMenuItem_Click;
			rowCopyToolStripMenuItem.Click += this.copyRangeToolStripMenuItem_Click;
			rowPasteToolStripMenuItem.Click += this.pasteRangeToolStripMenuItem_Click;

			colCutToolStripMenuItem.Click += this.cutRangeToolStripMenuItem_Click;
			colCopyToolStripMenuItem.Click += this.copyRangeToolStripMenuItem_Click;
			colPasteToolStripMenuItem.Click += this.pasteRangeToolStripMenuItem_Click;

			rowFormatCellsToolStripMenuItem.Click += this.formatCellToolStripMenuItem_Click;
			colFormatCellsToolStripMenuItem.Click += this.formatCellToolStripMenuItem_Click;

			printSettingsToolStripMenuItem.Click += this.printSettingsToolStripMenuItem_Click;

			printToolStripMenuItem.Click += PrintToolStripMenuItem_Click;

			var noneTypeMenuItem = new ToolStripMenuItem(LangResource.None);
			noneTypeMenuItem.Click += cellTypeNoneMenuItem_Click;
			changeCellsTypeToolStripMenuItem.DropDownItems.Add(noneTypeMenuItem);
			changeCellsTypeToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());

			var noneTypeMenuItem2 = new ToolStripMenuItem(LangResource.None);
			noneTypeMenuItem2.Click += cellTypeNoneMenuItem_Click;
			changeCellsTypeToolStripMenuItem2.DropDownItems.Add(noneTypeMenuItem2);
			changeCellsTypeToolStripMenuItem2.DropDownItems.Add(new ToolStripSeparator());

			foreach (var cellType in CellTypesManager.CellTypes)
			{
				var name = cellType.Key;
				if (name.EndsWith("Cell")) name = name.Substring(0, name.Length - 4);

				var menuItem = new ToolStripMenuItem(name)
				{
					Tag = cellType.Value,
				};

				menuItem.Click += cellTypeMenuItem_Click;
				changeCellsTypeToolStripMenuItem.DropDownItems.Add(menuItem);

				var menuItem2 = new ToolStripMenuItem(name)
				{
					Tag = cellType.Value,
				};

				menuItem2.Click += cellTypeMenuItem_Click;
				changeCellsTypeToolStripMenuItem2.DropDownItems.Add(menuItem2);
			}

			rowContextMenuStrip.Opening += (s, e) =>
			{
				insertRowPageBreakToolStripMenuItem.Enabled = !this.grid.CurrentWorksheet.PrintableRange.IsEmpty;
				removeRowPageBreakToolStripMenuItem.Enabled = this.grid.CurrentWorksheet.RowPageBreaks.Contains(this.grid.CurrentWorksheet.FocusPos.Row);
			};

			columnContextMenuStrip.Opening += (s, e) =>
			{
				insertColPageBreakToolStripMenuItem.Enabled = !this.grid.CurrentWorksheet.PrintableRange.IsEmpty;
				removeColPageBreakToolStripMenuItem.Enabled = this.grid.CurrentWorksheet.ColumnPageBreaks.Contains(this.grid.CurrentWorksheet.FocusPos.Col);
			};

			this.AutoFunctionSumToolStripMenuItem.Click += (s, e) => ApplyFunctionToSelectedRange("SUM");
			this.AutoFunctionAverageToolStripMenuItem.Click += (s, e) => ApplyFunctionToSelectedRange("AVERAGE");
			this.AutoFunctionCountToolStripMenuItem.Click += (s, e) => ApplyFunctionToSelectedRange("COUNT");
			this.AutoFunctionMaxToolStripMenuItem.Click += (s, e) => ApplyFunctionToSelectedRange("MAX");
			this.AutoFunctionMinToolStripMenuItem.Click += (s, e) => ApplyFunctionToSelectedRange("MIN");

			this.focusStyleDefaultToolStripMenuItem.CheckedChanged += (s, e) =>
				{
					if (this.focusStyleDefaultToolStripMenuItem.Checked) this.CurrentWorksheet.FocusPosStyle = FocusPosStyle.Default;
				};
			this.focusStyleNoneToolStripMenuItem.CheckedChanged += (s, e) =>
				{
					if (focusStyleNoneToolStripMenuItem.Checked) this.CurrentWorksheet.FocusPosStyle = FocusPosStyle.None;
				};

#if EX_SCRIPT
			scriptEditorToolStripMenuItem.Click += (s, e) =>
			{
				if (scriptEditor == null || scriptEditor.IsDisposed)
				{
					scriptEditor = new ReoScriptEditor();
					scriptEditor.Srm = this.grid.Srm;

					// synchronize script from the editor to control once the script is compiled 
					scriptEditor.ScriptCompiled += (ss, ee) =>
					{
						this.grid.Script = scriptEditor.Script;
					};
				}

				scriptEditor.Show();

				if (this.grid.Script == null)
				{
					this.grid.Script = Resources._default;
				}

				scriptEditor.Script = this.grid.Script;

				scriptEditor.Disposed += (ss, ee) => this.grid.Script = scriptEditor.Script;
			};

			runFunctionToolStripMenuItem.Click += (s, e) =>
			{
				using (var runFuncForm = new RunFunctionForm())
				{
					Cursor = Cursors.WaitCursor;

					if (this.grid.Srm != null && this.grid.Script != null)
					{
						var compiledScript = this.grid.Srm.Compile(
							scriptEditor.Visible ? scriptEditor.Script : this.grid.Script);
						runFuncForm.Srm = this.grid.Srm;
						runFuncForm.Script = compiledScript;
					}

					Cursor = Cursors.Default;

					runFuncForm.ShowDialog(this);
				}
			};

#else // !EX_SCRIPT

			//scriptToolStripMenuItem.Visible = false;
			scriptEditorToolStripMenuItem.Click += (s, e) =>
			{
				MessageBox.Show("Script execution is not supported by this edition.", Application.ProductName);
			};
#endif // EX_SCRIPT

			homepageToolStripMenuItem.Click += (s, e) =>
			{
				try
				{
					RGUtility.OpenFileOrLink(LangResource.HP_Homepage);
				}
				catch { }
			};

			documentationToolStripMenuItem.Click += (s, e) =>
			{
				try
				{
					RGUtility.OpenFileOrLink(LangResource.HP_Homepage_Document);
				}
				catch { }
			};

			insertColPageBreakToolStripMenuItem.Click += insertColPageBreakToolStripMenuItem_Click;
			insertRowPageBreakToolStripMenuItem.Click += insertRowPageBreakToolStripMenuItem_Click;
			removeColPageBreakToolStripMenuItem.Click += removeColPageBreakToolStripMenuItem_Click;
			removeRowPageBreakToolStripMenuItem.Click += removeRowPageBreakToolStripMenuItem_Click;

			filterToolStripMenuItem.Click += filterToolStripMenuItem_Click;
			clearFilterToolStripMenuItem.Click += clearFilterToolStripMenuItem_Click;
			columnFilterToolStripMenuItem.Click += filterToolStripMenuItem_Click;
			clearColumnFilterToolStripMenuItem.Click += clearFilterToolStripMenuItem_Click;

			this.grid.ExceptionHappened += (s, e) =>
			{
				if (e.Exception is RangeIntersectionException)
				{
					MessageBox.Show(this, LangResource.Msg_Range_Intersection_Exception,
						"ReoGrid Editor", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				}
				else if (e.Exception is OperationOnReadonlyCellException)
				{
					MessageBox.Show(this, LangResource.Msg_Operation_Aborted,
						"ReoGrid Editor", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				}
			};

			this.grid.CurrentWorksheetChanged += (s, e) =>
				{
					UpdateMenuAndToolStrips();
					worksheet_GridScaled(this.CurrentWorksheet, e);

					UpdateWorksheetSettings(this.grid.CurrentWorksheet);
					UpdateSelectionModeAndStyle();
					UpdateSelectionForwardDirection();
				};

			this.grid.SettingsChanged += (s, e) =>
				{
					sheetSwitcherToolStripMenuItem.Checked = this.grid.HasSettings(WorkbookSettings.View_ShowSheetTabControl);
					showHorizontaScrolllToolStripMenuItem.Checked = this.grid.HasSettings(WorkbookSettings.View_ShowHorScroll);
					showVerticalScrollbarToolStripMenuItem.Checked = this.grid.HasSettings(WorkbookSettings.View_ShowVerScroll);
				};

			this.clearAllToolStripMenuItem.Click += (s, e) =>
				this.CurrentWorksheet.ClearRangeContent(this.CurrentSelectionRange, CellElementFlag.All);
			this.clearDataToolStripMenuItem.Click += (s, e) =>
				this.CurrentWorksheet.ClearRangeContent(this.CurrentSelectionRange, CellElementFlag.Data);
			this.clearDataFormatToolStripMenuItem.Click += (s, e) =>
				this.CurrentWorksheet.ClearRangeContent(this.CurrentSelectionRange, CellElementFlag.DataFormat);
			this.clearFormulaToolStripMenuItem.Click += (s, e) =>
				this.CurrentWorksheet.ClearRangeContent(this.CurrentSelectionRange, CellElementFlag.Formula);
			this.clearCellBodyToolStripMenuItem.Click += (s, e) =>
				this.CurrentWorksheet.ClearRangeContent(this.CurrentSelectionRange, CellElementFlag.Body);
			this.clearStylesToolStripMenuItem.Click += (s, e) =>
				this.CurrentWorksheet.ClearRangeContent(this.CurrentSelectionRange, CellElementFlag.Style);
			this.clearBordersToolStripButton.Click += (s, e) =>
				this.CurrentWorksheet.ClearRangeContent(this.CurrentSelectionRange, CellElementFlag.Border);

			this.exportCurrentWorksheetToolStripMenuItem.Click += (s, e) => ExportAsCsv(RangePosition.EntireRange);
			this.exportSelectedRangeToolStripMenuItem.Click += (s, e) => ExportAsCsv(CurrentSelectionRange);

			this.dragToMoveRangeToolStripMenuItem.CheckedChanged += (s, e) => CurrentWorksheet.SetSettings(
				WorksheetSettings.Edit_DragSelectionToMoveCells, this.dragToMoveRangeToolStripMenuItem.Checked);
			this.dragToFillSerialToolStripMenuItem.CheckedChanged += (s, e) => CurrentWorksheet.SetSettings(
				WorksheetSettings.Edit_DragSelectionToFillSerial, this.dragToFillSerialToolStripMenuItem.Checked);

			this.suspendReferenceUpdatingToolStripMenuItem.CheckedChanged += (s, e) =>
				this.CurrentWorksheet.SetSettings(WorksheetSettings.Formula_AutoUpdateReferenceCell,
				!this.suspendReferenceUpdatingToolStripMenuItem.Checked);

			this.recalculateWorksheetToolStripMenuItem.Click += (s, e) => this.CurrentWorksheet.Recalculate();

#if RG_DEBUG

			this.showDebugFormToolStripButton.Click += new System.EventHandler(this.showDebugFormToolStripButton_Click);

			#region Debug Validation Events
			this.grid.WorksheetInserted += (ss, ee) =>
			{
				var worksheet = ee.Worksheet;

				worksheet.RowsInserted += (s, e) => _Debug_Auto_Validate_All((Worksheet)s);
				worksheet.ColumnsInserted += (s, e) => _Debug_Auto_Validate_All((Worksheet)s);
				worksheet.RowsDeleted += (s, e) => _Debug_Auto_Validate_All((Worksheet)s);
				worksheet.ColumnsDeleted += (s, e) => _Debug_Auto_Validate_All((Worksheet)s);
				worksheet.RangeMerged += (s, e) => _Debug_Auto_Validate_All((Worksheet)s);
				worksheet.RangeUnmerged += (s, e) => _Debug_Auto_Validate_All((Worksheet)s, e.Range);
				worksheet.AfterPaste += (s, e) => _Debug_Auto_Validate_All((Worksheet)s);
			};

			this.grid.Undid += (s, e) => _Debug_Auto_Validate_All(((BaseWorksheetAction)e.Action).Worksheet);
			this.grid.Redid += (s, e) => _Debug_Auto_Validate_All(((BaseWorksheetAction)e.Action).Worksheet);

			showDebugInfoToolStripMenuItem.Click += (s, e) =>
			{
				showDebugFormToolStripButton.PerformClick();
				showDebugInfoToolStripMenuItem.Checked = showDebugFormToolStripButton.Checked;
			};

			validateBorderSpanToolStripMenuItem.Click += (s, e) => _Debug_Validate_BorderSpan(this.CurrentWorksheet, true);
			validateMergedRangeToolStripMenuItem.Click += (s, e) => _Debug_Validate_Merged_Cell(this.CurrentWorksheet, true);
			validateAllToolStripMenuItem.Click += (s, e) => _Debug_Validate_All(this.CurrentWorksheet, true);

			#endregion // Debug Validation Events
#endif // RG_DEBUG

			ResumeLayout();
		}

		private void ExportAsCsv(RangePosition range)
		{
			using (SaveFileDialog dlg = new SaveFileDialog())
			{
				dlg.Filter = LangResource.Filter_Export_As_CSV;
				dlg.FileName = Path.GetFileNameWithoutExtension(this.CurrentFilePath);

				if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					using (FileStream fs = new FileStream(dlg.FileName, FileMode.Create, FileAccess.Write, FileShare.Read))
					{
						CurrentWorksheet.ExportAsCSV(fs, range);
					}

#if DEBUG
					RGUtility.OpenFileOrLink(dlg.FileName);
#endif
				}
			};
		}

		void worksheet_SelectionModeChanged(object sender, EventArgs e)
		{
			UpdateSelectionModeAndStyle();
		}

		void worksheet_SelectionForwardDirectionChanged(object sender, EventArgs e)
		{
			UpdateSelectionForwardDirection();
		}

		void worksheet_Resetted(object sender, EventArgs e)
		{
			statusToolStripStatusLabel.Text = string.Empty;
		}

		void worksheet_SettingsChanged(object sender, SettingsChangedEventArgs e)
		{
			var worksheet = sender as Worksheet;
			if (worksheet != null) UpdateWorksheetSettings(worksheet);
		}

		void UpdateWorksheetSettings(Worksheet sheet)
		{
			bool visible = false;

			visible = sheet.HasSettings(WorksheetSettings.View_ShowGridLine);
			if (showGridLinesToolStripMenuItem.Checked != visible) showGridLinesToolStripMenuItem.Checked = visible;

			visible = sheet.HasSettings(WorksheetSettings.View_ShowPageBreaks);
			if (showPageBreakToolStripMenuItem.Checked != visible) showPageBreakToolStripMenuItem.Checked = visible;

			visible = sheet.HasSettings(WorksheetSettings.View_ShowFrozenLine);
			if (showFrozenLineToolStripMenuItem.Checked != visible) showFrozenLineToolStripMenuItem.Checked = visible;

			visible = sheet.HasSettings(WorksheetSettings.View_ShowRowHeader);
			if (showRowHeaderToolStripMenuItem.Checked != visible) showRowHeaderToolStripMenuItem.Checked = visible;

			visible = sheet.HasSettings(WorksheetSettings.View_ShowColumnHeader);
			if (showColumnHeaderToolStripMenuItem.Checked != visible) showColumnHeaderToolStripMenuItem.Checked = visible;

			visible = sheet.HasSettings(WorksheetSettings.View_AllowShowRowOutlines);
			if (showRowOutlineToolStripMenuItem.Checked != visible) showRowOutlineToolStripMenuItem.Checked = visible;

			visible = sheet.HasSettings(WorksheetSettings.View_AllowShowColumnOutlines);
			if (showColumnOutlineToolStripMenuItem.Checked != visible) showColumnOutlineToolStripMenuItem.Checked = visible;

			var check = sheet.HasSettings(WorksheetSettings.Edit_DragSelectionToMoveCells);
			if (this.dragToMoveRangeToolStripMenuItem.Checked != check) this.dragToMoveRangeToolStripMenuItem.Checked = check;

			check = sheet.HasSettings(WorksheetSettings.Edit_DragSelectionToFillSerial);
			if (this.dragToFillSerialToolStripMenuItem.Checked != check) this.dragToFillSerialToolStripMenuItem.Checked = check;

			check = !sheet.HasSettings(WorksheetSettings.Formula_AutoUpdateReferenceCell);
			if (this.suspendReferenceUpdatingToolStripMenuItem.Checked != check) this.suspendReferenceUpdatingToolStripMenuItem.Checked = check;

			sheetReadonlyToolStripMenuItem.Checked = sheet.HasSettings(WorksheetSettings.Edit_Readonly);

		}

		void cellTypeNoneMenuItem_Click(object sender, EventArgs e)
		{
			var worksheet = this.CurrentWorksheet;

			if (worksheet != null)
			{
				worksheet.IterateCells(worksheet.SelectionRange, false, (r, c, cell) =>
				{
					cell.Body = null;
					return true;
				});
			}
		}

		void cellTypeMenuItem_Click(object sender, EventArgs e)
		{
			var worksheet = this.CurrentWorksheet;

			var menuItem = sender as ToolStripMenuItem;

			if (menuItem != null && menuItem.Tag is Type && worksheet != null && !worksheet.SelectionRange.IsEmpty)
			{
				foreach (var cell in worksheet.Ranges[worksheet.SelectionRange].Cells)
				{
					cell.Body = System.Activator.CreateInstance((Type)menuItem.Tag) as ICellBody;
				}
			}
		}

		void textWrapToolStripButton_Click(object sender, EventArgs e)
		{
			this.grid.DoAction(new SetRangeStyleAction(this.CurrentSelectionRange, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.TextWrap,
				TextWrapMode = textWrapToolStripButton.Checked ? TextWrapMode.WordBreak : TextWrapMode.NoWrap,
			}));
		}

		void worksheet_GridScaled(object sender, EventArgs e)
		{
			var worksheet = (Worksheet)sender;
			zoomToolStripDropDownButton.Text = worksheet.ScaleFactor * 100 + "%";
		}

		#endregion // Constructor

		#region Utility

#if RG_DEBUG
		#region Debug Validations
		/// <summary>
		/// Use for Debug mode. Check for border span is valid.
		/// </summary>
		/// <param name="showSuccessMsg"></param>
		/// <returns></returns>
		bool _Debug_Validate_BorderSpan(Worksheet sheet, bool showSuccessMsg)
		{
			bool rs = sheet._Debug_Validate_BorderSpan();

			if (rs)
			{
				if (showSuccessMsg) ShowStatus("Border span validation ok.");
			}
			else
			{
				ShowError("Border span test failed.");

				if (!showDebugInfoToolStripMenuItem.Checked)
				{
					showDebugInfoToolStripMenuItem.PerformClick();
				}
			}

			return rs;
		}
		bool _Debug_Validate_Merged_Cell(Worksheet sheet, bool showSuccessMsg)
		{
			bool rs = sheet._Debug_Validate_MergedCells();

			if (rs)
			{
				if (showSuccessMsg) ShowStatus("Merged range validation ok.");
			}
			else
			{
				ShowError("Merged range validation failed.");

				if (!showDebugInfoToolStripMenuItem.Checked)
				{
					showDebugInfoToolStripMenuItem.PerformClick();
				}
			}

			return rs;
		}
		bool _Debug_Validate_Unmerged_Range(Worksheet sheet, bool showSuccessMsg, RangePosition range)
		{
			bool rs = sheet._Debug_Validate_Unmerged_Range(range);

			if (rs)
			{
				if (showSuccessMsg) ShowStatus("Unmerged range validation ok.");
			}
			else
			{
				ShowError("Unmerged range validation failed.");

				if (!showDebugInfoToolStripMenuItem.Checked)
				{
					showDebugInfoToolStripMenuItem.PerformClick();
				}
			}

			return rs;
		}
		bool _Debug_Validate_All(Worksheet sheet, bool showSuccessMsg)
		{
			return _Debug_Validate_All(sheet, showSuccessMsg, RangePosition.EntireRange);
		}
		bool _Debug_Validate_All(Worksheet sheet, RangePosition range)
		{
			return _Debug_Validate_All(sheet, false, range);
		}
		bool _Debug_Validate_All(Worksheet sheet, bool showSuccessMsg, RangePosition range)
		{
			bool rs = _Debug_Validate_BorderSpan(sheet, showSuccessMsg);
			if (rs) rs = _Debug_Validate_Merged_Cell(sheet, showSuccessMsg);
			if (rs) rs = _Debug_Validate_Unmerged_Range(sheet, showSuccessMsg, range);

			return rs;
		}
		bool _Debug_Auto_Validate_All(Worksheet sheet) { return _Debug_Validate_All(sheet, false); }
		bool _Debug_Auto_Validate_All(Worksheet sheet, RangePosition range) { return _Debug_Validate_All(sheet, range); }
		#endregion // Debug Validations
#endif // RG_DEBUG

		public ReoGridControl GridControl { get { return this.grid; } }

		public Worksheet CurrentWorksheet { get { return this.grid.CurrentWorksheet; } }

		public RangePosition CurrentSelectionRange
		{
			get { return this.grid.CurrentWorksheet.SelectionRange; }
			set { this.grid.CurrentWorksheet.SelectionRange = value; }
		}

#if EX_SCRIPT
		private ReoScriptEditor scriptEditor;
		public ReoScriptEditor ScriptEditor { get { return scriptEditor; } }
#endif // EX_SCRIPT

		internal void ShowStatus(string msg)
		{
			ShowStatus(msg, false);
		}
		internal void ShowStatus(string msg, bool error)
		{
			statusToolStripStatusLabel.Text = msg;
			statusToolStripStatusLabel.ForeColor = error ? Color.Red : SystemColors.WindowText;
		}
		public void ShowError(string msg)
		{
			ShowStatus(msg, true);
		}

		private void UpdateMenuAndToolStripsWhenAction(object sender, EventArgs e)
		{
			UpdateMenuAndToolStrips();
		}

		private void Undo(object sender, EventArgs e)
		{
			this.grid.Undo();
		}

		private void Redo(object sender, EventArgs e)
		{
			this.grid.Redo();
		}

		void zoomToolStripDropDownButton_TextChanged(object sender, EventArgs e)
		{
			if (isUIUpdating) return;

			if (zoomToolStripDropDownButton.Text.Length > 0)
			{
				int value = 100;
				if (int.TryParse(zoomToolStripDropDownButton.Text.Substring(0, zoomToolStripDropDownButton.Text.Length - 1), out value))
				{
					float scale = (float)value / 100f;
					scale = (float)Math.Round(scale, 1);

					this.CurrentWorksheet.SetScale(scale);
				}
			}
		}

		void grid_SelectionRangeChanged(object sender, RangeEventArgs e)
		{
			// get event source worksheet
			var worksheet = sender as Worksheet;

			// if source worksheet is current worksheet, update menus and tool strips
			if (worksheet == this.CurrentWorksheet)
			{
				if (worksheet.SelectionRange == RangePosition.Empty)
				{
					rangeInfoToolStripStatusLabel.Text = "Selection None";
				}
				else
				{
					rangeInfoToolStripStatusLabel.Text =
						string.Format("{0} {1} x {2}", worksheet.SelectionRange.ToString(),
						worksheet.SelectionRange.Rows, worksheet.SelectionRange.Cols);
				}

				UpdateMenuAndToolStrips();
			}
		}

		#endregion // Utility

		#region Language

		void SetupUILanguage()
		{
			#region Menu
			// File
			this.fileToolStripMenuItem.Text = LangResource.Menu_File;
			this.newToolStripMenuItem.Text = LangResource.Menu_File_New;
			this.newWindowToolStripMenuItem.Text = LangResource.Menu_File_New_Window;
			this.openToolStripMenuItem.Text = LangResource.Menu_File_Open;
			this.saveToolStripMenuItem.Text = LangResource.Menu_File_Save;
			this.saveAsToolStripMenuItem.Text = LangResource.Menu_File_Save_As;
			this.editXMLToolStripMenuItem.Text = LangResource.Menu_File_Edit_RGF_XML;
			this.exportAsHtmlToolStripMenuItem.Text = LangResource.Menu_File_Export_As_HTML;
			this.exportAsCSVToolStripMenuItem.Text = LangResource.Menu_File_Export_As_CSV;
			this.exportSelectedRangeToolStripMenuItem.Text = LangResource.Menu_File_Export_As_CSV_Selected_Range;
			this.exportCurrentWorksheetToolStripMenuItem.Text = LangResource.Menu_File_Export_As_CSV_Current_Worksheet;
			this.printPreviewToolStripMenuItem.Text = LangResource.Menu_File_Print_Preview;
			this.printSettingsToolStripMenuItem.Text = LangResource.Menu_File_Print_Settings;
			this.printToolStripMenuItem.Text = LangResource.Menu_File_Print;
			this.exitToolStripMenuItem.Text = LangResource.Menu_File_Exit;

			// Edit
			this.editToolStripMenuItem.Text = LangResource.Menu_Edit;
			this.undoToolStripMenuItem.Text = LangResource.Menu_Undo;
			this.redoToolStripMenuItem.Text = LangResource.Menu_Redo;
			this.repeatLastActionToolStripMenuItem.Text = LangResource.Menu_Edit_Repeat_Last_Action;
			this.cutToolStripMenuItem.Text = LangResource.Menu_Cut;
			this.copyToolStripMenuItem.Text = LangResource.Menu_Copy;
			this.pasteToolStripMenuItem.Text = LangResource.Menu_Paste;
			this.clearToolStripMenuItem.Text = LangResource.Menu_Edit_Clear;
			this.clearAllToolStripMenuItem.Text = LangResource.All;
			this.clearDataToolStripMenuItem.Text = LangResource.Data;
			this.clearDataFormatToolStripMenuItem.Text = LangResource.Data_Format;
			this.clearFormulaToolStripMenuItem.Text = LangResource.Formula;
			this.clearCellBodyToolStripMenuItem.Text = LangResource.CellBody;
			this.clearStylesToolStripMenuItem.Text = LangResource.Style;
			this.clearBordersToolStripMenuItem.Text = LangResource.Border;
			this.focusCellStyleToolStripMenuItem.Text = LangResource.Menu_Edit_Focus_Cell_Style;
			this.focusStyleDefaultToolStripMenuItem.Text = LangResource.Default;
			this.focusStyleNoneToolStripMenuItem.Text = LangResource.None;
			this.selectionToolStripMenuItem.Text = LangResource.Menu_Edit_Selection;
			this.dragToMoveRangeToolStripMenuItem.Text = LangResource.Menu_Edit_Selection_Drag_To_Move_Content;
			this.dragToFillSerialToolStripMenuItem.Text = LangResource.Menu_Edit_Selection_Drag_To_Fill_Serial;
			this.selectionStyleToolStripMenuItem.Text = LangResource.Menu_Edit_Selection_Style;
			this.selStyleDefaultToolStripMenuItem.Text = LangResource.Menu_Edit_Selection_Style_Default;
			this.selStyleFocusRectToolStripMenuItem.Text = LangResource.Menu_Edit_Selection_Style_Focus_Rect;
			this.selStyleNoneToolStripMenuItem.Text = LangResource.Menu_Edit_Selection_Style_None;
			this.selectionModeToolStripMenuItem.Text = LangResource.Menu_Edit_Selection_Mode;
			this.selModeNoneToolStripMenuItem.Text = LangResource.Menu_Edit_Selection_Mode_None;
			this.selModeCellToolStripMenuItem.Text = LangResource.Menu_Edit_Selection_Mode_Cell;
			this.selModeRangeToolStripMenuItem.Text = LangResource.Menu_Edit_Selection_Mode_Range;
			this.selModeRowToolStripMenuItem.Text = LangResource.Menu_Edit_Selection_Mode_Row;
			this.selModeColumnToolStripMenuItem.Text = LangResource.Menu_Edit_Selection_Mode_Column;
			this.selectionMoveDirectionToolStripMenuItem.Text = LangResource.Menu_Edit_Selection_Move_Direction;
			this.selDirRightToolStripMenuItem.Text = LangResource.Menu_Edit_Selection_Move_Direction_Right;
			this.selDirDownToolStripMenuItem.Text = LangResource.Menu_Edit_Selection_Move_Direction_Down;
			this.selectAllToolStripMenuItem.Text = LangResource.Menu_Edit_Select_All;

			// View
			this.viewToolStripMenuItem.Text = LangResource.Menu_View;
			this.componentsToolStripMenuItem.Text = LangResource.Menu_View_Components;
			this.toolbarToolStripMenuItem.Text = LangResource.Menu_View_Components_Toolbar;
			this.formulaBarToolStripMenuItem.Text = LangResource.Menu_View_Components_FormulaBar;
			this.statusBarToolStripMenuItem.Text = LangResource.Menu_View_Components_StatusBar;
			this.visibleToolStripMenuItem.Text = LangResource.Menu_View_Visible;
			this.showGridLinesToolStripMenuItem.Text = LangResource.Menu_View_Visible_Grid_Lines;
			this.showPageBreakToolStripMenuItem.Text = LangResource.Menu_View_Visible_Page_Breaks;
			this.showFrozenLineToolStripMenuItem.Text = LangResource.Menu_View_Visible_Forzen_Line;
			this.sheetSwitcherToolStripMenuItem.Text = LangResource.Menu_View_Visible_Sheet_Tab;
			this.showHorizontaScrolllToolStripMenuItem.Text = LangResource.Menu_View_Visible_Horizontal_ScrollBar;
			this.showVerticalScrollbarToolStripMenuItem.Text = LangResource.Menu_View_Visible_Vertical_ScrollBar;
			this.showRowHeaderToolStripMenuItem.Text = LangResource.Menu_View_Visible_Row_Header;
			this.showColumnHeaderToolStripMenuItem.Text = LangResource.Menu_View_Visible_Column_Header;
			this.showRowOutlineToolStripMenuItem.Text = LangResource.Menu_View_Visible_Row_Outline_Panel;
			this.showColumnOutlineToolStripMenuItem.Text = LangResource.Menu_View_Visible_Column_Outline_Panel;
			this.resetAllPageBreaksToolStripMenuItem.Text = LangResource.Menu_Reset_All_Page_Breaks;
			this.freezeToCellToolStripMenuItem.Text = LangResource.Menu_View_Freeze_To_Cell;
			this.freezeToSpecifiedEdgeToolStripMenuItem.Text = LangResource.Menu_View_Freeze_To_Edges;
			this.freezeToLeftToolStripMenuItem.Text = LangResource.Menu_View_Freeze_To_Edges_Left;
			this.freezeToRightToolStripMenuItem.Text = LangResource.Menu_View_Freeze_To_Edges_Right;
			this.freezeToTopToolStripMenuItem.Text = LangResource.Menu_View_Freeze_To_Edges_Top;
			this.freezeToBottomToolStripMenuItem.Text = LangResource.Menu_View_Freeze_To_Edges_Bottom;
			this.freezeToLeftTopToolStripMenuItem.Text = LangResource.Menu_View_Freeze_To_Edges_Top_Left;
			this.freezeToLeftBottomToolStripMenuItem.Text = LangResource.Menu_View_Freeze_To_Edges_Bottom_Left;
			this.freezeToRightTopToolStripMenuItem.Text = LangResource.Menu_View_Freeze_To_Edges_Top_Right;
			this.freezeToRightBottomToolStripMenuItem.Text = LangResource.Menu_View_Freeze_To_Edges_Bottom_Right;
			this.unfreezeToolStripMenuItem.Text = LangResource.Menu_View_Unfreeze;

			// Cells
			this.cellsToolStripMenuItem.Text = LangResource.Menu_Cells;
			this.mergeCellsToolStripMenuItem.Text = LangResource.Menu_Cells_Merge_Cells;
			this.unmergeCellsToolStripMenuItem.Text = LangResource.Menu_Cells_Unmerge_Cells;
			this.changeCellsTypeToolStripMenuItem.Text = LangResource.Menu_Change_Cells_Type;
			this.formatCellsToolStripMenuItem.Text = LangResource.Menu_Format_Cells;

			// Sheet
			this.sheetToolStripMenuItem.Text = LangResource.Menu_Sheet;
			this.filterToolStripMenuItem.Text = LangResource.Menu_Sheet_Filter;
			this.clearFilterToolStripMenuItem.Text = LangResource.Menu_Sheet_Clear_Filter;
			this.groupToolStripMenuItem.Text = LangResource.Menu_Sheet_Group;
			this.groupRowsToolStripMenuItem.Text = LangResource.Menu_Sheet_Group_Rows;
			this.groupColumnsToolStripMenuItem.Text = LangResource.Menu_Sheet_Group_Columns;
			this.ungroupToolStripMenuItem.Text = LangResource.Menu_Sheet_Ungroup;
			this.ungroupRowsToolStripMenuItem.Text = LangResource.Menu_Sheet_Ungroup_Selection_Rows;
			this.ungroupAllRowsToolStripMenuItem.Text = LangResource.Menu_Sheet_Ungroup_All_Rows;
			this.ungroupColumnsToolStripMenuItem.Text = LangResource.Menu_Sheet_Ungroup_Selection_Columns;
			this.ungroupAllColumnsToolStripMenuItem.Text = LangResource.Menu_Sheet_Ungroup_All_Columns;
			this.insertToolStripMenuItem.Text = LangResource.Menu_Sheet_Insert;
			this.resizeToolStripMenuItem.Text = LangResource.Menu_Sheet_Resize;
			this.sheetReadonlyToolStripMenuItem.Text = LangResource.Menu_Edit_Readonly;

			// Formula
			this.formulaToolStripMenuItem.Text = LangResource.Menu_Formula;
			this.autoFunctionToolStripMenuItem.Text = LangResource.Menu_Formula_Auto_Function;
			this.defineNamedRangeToolStripMenuItem.Text = LangResource.Menu_Formula_Define_Name;
			this.nameManagerToolStripMenuItem.Text = LangResource.Menu_Formula_Name_Manager;
			this.tracePrecedentsToolStripMenuItem.Text = LangResource.Menu_Formula_Trace_Precedents;
			this.traceDependentsToolStripMenuItem.Text = LangResource.Menu_Formula_Trace_Dependents;
			this.removeArrowsToolStripMenuItem.Text = LangResource.Menu_Formula_Remove_Trace_Arrows;
			this.removeAllArrowsToolStripMenuItem.Text = LangResource.Menu_Formula_Remove_Trace_Arrows_Remove_All_Arrows;
			this.removePrecedentArrowsToolStripMenuItem.Text = LangResource.Menu_Formula_Remove_Trace_Arrows_Remove_Precedent_Arrows;
			this.removeDependentArrowsToolStripMenuItem.Text = LangResource.Menu_Formula_Remove_Trace_Arrows_Remove_Dependent_Arrows;
			this.suspendReferenceUpdatingToolStripMenuItem.Text = LangResource.Menu_Formula_Suspend_Reference_Updates;
			this.recalculateWorksheetToolStripMenuItem.Text = LangResource.Menu_Formula_Recalculate_Worksheet;

			// Script
			this.scriptToolStripMenuItem.Text = LangResource.Menu_Script;
			this.scriptEditorToolStripMenuItem.Text = LangResource.Menu_Script_Script_Editor;
			this.runFunctionToolStripMenuItem.Text = LangResource.Menu_Script_Run_Function;

			// Tools
			this.toolsToolStripMenuItem.Text = LangResource.Menu_Tools;
			this.controlStyleToolStripMenuItem.Text = LangResource.Menu_Tools_Control_Appearance;
			this.helpToolStripMenuItem.Text = LangResource.Menu_Help;
			this.homepageToolStripMenuItem.Text = LangResource.Menu_Help_Homepage;
			this.documentationToolStripMenuItem.Text = LangResource.Menu_Help_Documents;
			this.aboutToolStripMenuItem.Text = LangResource.Menu_Help_About;

			// Column Context Menu
			this.colCutToolStripMenuItem.Text = LangResource.Menu_Cut;
			this.colCopyToolStripMenuItem.Text = LangResource.Menu_Copy;
			this.colPasteToolStripMenuItem.Text = LangResource.Menu_Paste;
			this.insertColToolStripMenuItem.Text = LangResource.CtxMenu_Col_Insert_Columns;
			this.deleteColumnToolStripMenuItem.Text = LangResource.CtxMenu_Col_Delete_Columns;
			this.resetToDefaultWidthToolStripMenuItem.Text = LangResource.CtxMenu_Col_Reset_To_Default_Width;
			this.columnWidthToolStripMenuItem.Text = LangResource.CtxMenu_Col_Column_Width;
			this.hideColumnsToolStripMenuItem.Text = LangResource.Menu_Hide;
			this.unhideColumnsToolStripMenuItem.Text = LangResource.Menu_Unhide;
			this.columnFilterToolStripMenuItem.Text = LangResource.CtxMenu_Col_Filter;
			this.clearColumnFilterToolStripMenuItem.Text = LangResource.CtxMenu_Col_Clear_Filter;
			this.groupColumnsToolStripMenuItem1.Text = LangResource.Menu_Group;
			this.ungroupColumnsToolStripMenuItem1.Text = LangResource.Menu_Ungroup;
			this.ungroupAllColumnsToolStripMenuItem1.Text = LangResource.Menu_Ungroup_All;
			this.insertColPageBreakToolStripMenuItem.Text = LangResource.Menu_Insert_Page_Break;
			this.removeColPageBreakToolStripMenuItem.Text = LangResource.Menu_Remove_Page_Break;
			this.columnPropertiesToolStripMenuItem.Text = LangResource.Menu_Property;
			this.colFormatCellsToolStripMenuItem.Text = LangResource.Menu_Format_Cells;

			// Row Context Menu
			this.rowCutToolStripMenuItem.Text = LangResource.Menu_Cut;
			this.rowCopyToolStripMenuItem.Text = LangResource.Menu_Copy;
			this.rowPasteToolStripMenuItem.Text = LangResource.Menu_Paste;
			this.insertRowToolStripMenuItem.Text = LangResource.CtxMenu_Row_Insert_Rows;
			this.deleteRowsToolStripMenuItem.Text = LangResource.CtxMenu_Row_Delete_Rows;
			this.resetToDefaultHeightToolStripMenuItem.Text = LangResource.CtxMenu_Row_Reset_to_Default_Height;
			this.rowHeightToolStripMenuItem.Text = LangResource.CtxMenu_Row_Row_Height;
			this.hideRowsToolStripMenuItem.Text = LangResource.Menu_Hide;
			this.unhideRowsToolStripMenuItem.Text = LangResource.Menu_Unhide;
			this.groupRowsToolStripMenuItem1.Text = LangResource.Menu_Group;
			this.ungroupRowsToolStripMenuItem1.Text = LangResource.Menu_Ungroup;
			this.ungroupAllRowsToolStripMenuItem1.Text = LangResource.Menu_Ungroup_All;
			this.insertRowPageBreakToolStripMenuItem.Text = LangResource.Menu_Insert_Page_Break;
			this.removeRowPageBreakToolStripMenuItem.Text = LangResource.Menu_Remove_Page_Break;
			this.rowPropertiesToolStripMenuItem.Text = LangResource.Menu_Property;
			this.rowFormatCellsToolStripMenuItem.Text = LangResource.Menu_Format_Cells;

			// Cell Context Menu
			this.cutRangeToolStripMenuItem.Text = LangResource.Menu_Cut;
			this.copyRangeToolStripMenuItem.Text = LangResource.Menu_Copy;
			this.pasteRangeToolStripMenuItem.Text = LangResource.Menu_Paste;
			this.mergeRangeToolStripMenuItem.Text = LangResource.CtxMenu_Cell_Merge;
			this.unmergeRangeToolStripMenuItem.Text = LangResource.CtxMenu_Cell_Unmerge;
			this.changeCellsTypeToolStripMenuItem2.Text = LangResource.Menu_Change_Cells_Type;
			this.formatCellToolStripMenuItem.Text = LangResource.Menu_Format_Cells;

			// Lead Header Context Menu
			this.resetAllPageBreaksToolStripMenuItem1.Text = LangResource.Menu_Reset_All_Page_Breaks;

			#endregion // Menu
		}

		System.Globalization.CultureInfo cultureEN_US;
		System.Globalization.CultureInfo cultureJP_JP;
		System.Globalization.CultureInfo cultureZH_CN;

		public void ChangeLanguageToEnglish()
		{
			if (cultureEN_US == null) cultureEN_US = new System.Globalization.CultureInfo("en-US");
			System.Threading.Thread.CurrentThread.CurrentUICulture = cultureEN_US;

			SetupUILanguage();
		}

		public void ChangeLanguageToJapanese()
		{
			if (cultureJP_JP == null) cultureJP_JP = new System.Globalization.CultureInfo("ja-JP");
			System.Threading.Thread.CurrentThread.CurrentUICulture = cultureJP_JP;

			SetupUILanguage();
		}

		public void ChangeLanguageToChinese()
		{
			if (cultureZH_CN == null) cultureZH_CN = new System.Globalization.CultureInfo("zh-CN");
			System.Threading.Thread.CurrentThread.CurrentUICulture = cultureZH_CN;

			SetupUILanguage();
		}

		private void englishenUSToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ChangeLanguageToEnglish();
		}

		private void japanesejpJPToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ChangeLanguageToJapanese();
		}

		private void simplifiedChinesezhCNToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ChangeLanguageToChinese();
		}

		#endregion // Langauge

		#region Update Menus & Toolbars
		private bool isUIUpdating = false;
		private void UpdateMenuAndToolStrips()
		{
			if (isUIUpdating)
				return;

			isUIUpdating = true;

			var worksheet = this.CurrentWorksheet;

			WorksheetRangeStyle style = worksheet.GetCellStyles(worksheet.SelectionRange.StartPos);
			if (style != null)
			{
				// cross-thread exception
				Action set = () =>
				{
					fontToolStripComboBox.Text = style.FontName;
					fontSizeToolStripComboBox.Text = style.FontSize.ToString();
					boldToolStripButton.Checked = style.Bold;
					italicToolStripButton.Checked = style.Italic;
					strikethroughToolStripButton.Checked = style.Strikethrough;
					underlineToolStripButton.Checked = style.Underline;
					textColorPickToolStripItem.SolidColor = style.TextColor;
					backColorPickerToolStripButton.SolidColor = style.BackColor;
					textAlignLeftToolStripButton.Checked = style.HAlign == ReoGridHorAlign.Left;
					textAlignCenterToolStripButton.Checked = style.HAlign == ReoGridHorAlign.Center;
					textAlignRightToolStripButton.Checked = style.HAlign == ReoGridHorAlign.Right;
					distributedIndentToolStripButton.Checked = style.HAlign == ReoGridHorAlign.DistributedIndent;
					textAlignTopToolStripButton.Checked = style.VAlign == ReoGridVerAlign.Top;
					textAlignMiddleToolStripButton.Checked = style.VAlign == ReoGridVerAlign.Middle;
					textAlignBottomToolStripButton.Checked = style.VAlign == ReoGridVerAlign.Bottom;
					textWrapToolStripButton.Checked = style.TextWrapMode != TextWrapMode.NoWrap;

					RangeBorderInfoSet borderInfo = worksheet.GetRangeBorders(worksheet.SelectionRange);
					if (borderInfo.Left != null)
					{
						borderColorPickToolStripItem.SolidColor = borderInfo.Left.Color;
					}
					else if (borderInfo.Right != null)
					{
						borderColorPickToolStripItem.SolidColor = borderInfo.Right.Color;
					}
					else if (borderInfo.Top != null)
					{
						borderColorPickToolStripItem.SolidColor = borderInfo.Top.Color;
					}
					else if (borderInfo.Bottom != null)
					{
						borderColorPickToolStripItem.SolidColor = borderInfo.Bottom.Color;
					}
					else if (borderInfo.InsideHorizontal != null)
					{
						borderColorPickToolStripItem.SolidColor = borderInfo.InsideHorizontal.Color;
					}
					else if (borderInfo.InsideVertical != null)
					{
						borderColorPickToolStripItem.SolidColor = borderInfo.InsideVertical.Color;
					}
					else
					{
						borderColorPickToolStripItem.SolidColor = Color.Black;
					}

					undoToolStripButton.Enabled =
						undoToolStripMenuItem.Enabled =
						this.grid.CanUndo();

					redoToolStripButton.Enabled =
						redoToolStripMenuItem.Enabled =
						this.grid.CanRedo();

					repeatLastActionToolStripMenuItem.Enabled =
						this.grid.CanUndo() || this.grid.CanRedo();

					cutToolStripButton.Enabled =
						cutToolStripMenuItem.Enabled =
						rowCutToolStripMenuItem.Enabled =
						colCutToolStripMenuItem.Enabled =
						worksheet.CanCut();

					copyToolStripButton.Enabled =
						copyToolStripMenuItem.Enabled =
						rowCopyToolStripMenuItem.Enabled =
						colCopyToolStripMenuItem.Enabled =
						worksheet.CanCopy();

					pasteToolStripButton.Enabled =
						pasteToolStripMenuItem.Enabled =
						rowPasteToolStripMenuItem.Enabled =
						colPasteToolStripMenuItem.Enabled =
						worksheet.CanPaste();

					unfreezeToolStripMenuItem.Enabled = worksheet.IsFrozen;

					isUIUpdating = false;
				};

				if (this.InvokeRequired)
					this.Invoke(set);
				else
					set();
			}

#if !DEBUG
			debugToolStripMenuItem.Enabled = false;
#endif // DEBUG

		}

		private bool settingSelectionMode = false;

		private void UpdateSelectionModeAndStyle()
		{
			if (settingSelectionMode) return;

			settingSelectionMode = true;

			selModeNoneToolStripMenuItem.Checked = false;
			selModeCellToolStripMenuItem.Checked = false;
			selModeRangeToolStripMenuItem.Checked = false;
			selModeRowToolStripMenuItem.Checked = false;
			selModeColumnToolStripMenuItem.Checked = false;

			switch (this.CurrentWorksheet.SelectionMode)
			{
				case WorksheetSelectionMode.None:
					selModeNoneToolStripMenuItem.Checked = true;
					break;
				case WorksheetSelectionMode.Cell:
					selModeCellToolStripMenuItem.Checked = true;
					break;
				default:
				case WorksheetSelectionMode.Range:
					selModeRangeToolStripMenuItem.Checked = true;
					break;
				case WorksheetSelectionMode.Row:
					selModeRowToolStripMenuItem.Checked = true;
					break;
				case WorksheetSelectionMode.Column:
					selModeColumnToolStripMenuItem.Checked = true;
					break;
			}

			selStyleNoneToolStripMenuItem.Checked = false;
			selStyleDefaultToolStripMenuItem.Checked = false;
			selStyleFocusRectToolStripMenuItem.Checked = false;

			switch (this.CurrentWorksheet.SelectionStyle)
			{
				case WorksheetSelectionStyle.None:
					selStyleNoneToolStripMenuItem.Checked = true;
					break;
				default:
				case WorksheetSelectionStyle.Default:
					selStyleDefaultToolStripMenuItem.Checked = true;
					break;
				case WorksheetSelectionStyle.FocusRect:
					selStyleFocusRectToolStripMenuItem.Checked = true;
					break;
			}

			focusStyleDefaultToolStripMenuItem.Checked = false;
			focusStyleNoneToolStripMenuItem.Checked = false;

			switch (this.CurrentWorksheet.FocusPosStyle)
			{
				default:
				case FocusPosStyle.Default:
					focusStyleDefaultToolStripMenuItem.Checked = true;
					break;
				case FocusPosStyle.None:
					focusStyleNoneToolStripMenuItem.Checked = true;
					break;
			}

			settingSelectionMode = false;
		}

		private void UpdateSelectionForwardDirection()
		{
			switch (this.CurrentWorksheet.SelectionForwardDirection)
			{
				default:
				case SelectionForwardDirection.Right:
					selDirRightToolStripMenuItem.Checked = true;
					selDirDownToolStripMenuItem.Checked = false;
					break;
				case SelectionForwardDirection.Down:
					selDirRightToolStripMenuItem.Checked = false;
					selDirDownToolStripMenuItem.Checked = true;
					break;
			}
		}
		#endregion

		#region Document
		public string CurrentFilePath { get; set; }
		private string currentTempFilePath;

		/// <summary>
		/// Load spreadsheet form specified file
		/// </summary>
		/// <param name="path">path to load file</param>
		public void LoadFile(string path)
		{
			LoadFile(path, Encoding.Default);
		}

		/// <summary>
		/// Load spreadsheet from specified file
		/// </summary>
		/// <param name="path">path to load file</param>
		/// <param name="encoding">encoding to read input stream</param>
		public void LoadFile(string path, Encoding encoding)
		{
			this.CurrentFilePath = null;

			var worksheet = this.CurrentWorksheet;

			bool success = false;

			grid.CurrentWorksheet.Reset();

			try
			{
				grid.Load(path, IO.FileFormat._Auto, encoding);
				success = true;
			}
			catch (FileNotFoundException ex)
			{
				success = false;
				MessageBox.Show(LangResource.Msg_File_Not_Found + ex.FileName, "ReoGrid Editor", MessageBoxButtons.OK, MessageBoxIcon.Stop);
			}
			catch (Exception ex)
			{
				success = false;
				MessageBox.Show(LangResource.Msg_Load_File_Failed + ex.Message, "ReoGrid Editor", MessageBoxButtons.OK, MessageBoxIcon.Stop);
			}

			if (success)
			{
				this.Text = System.IO.Path.GetFileName(path) + " - ReoGrid Editor " + this.ProductVersion;
				//showGridLinesToolStripMenuItem.Checked = worksheet.HasSettings(WorksheetSettings.View_ShowGuideLine);
				ShowStatus(string.Empty);
				this.CurrentFilePath = path;
				this.currentTempFilePath = null;

#if EX_SCRIPT
				// check whether grid contains any scripts
				if (!string.IsNullOrEmpty(this.grid.Script))
				{
					if (MessageBox.Show(LangResource.Msg_Load_Script_Prompt,
						LangResource.Msg_Load_Script_Prompt_Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						if (scriptEditor == null || scriptEditor.IsDisposed)
						{
							scriptEditor = new ReoScriptEditor()
							{
								Script = this.grid.Script,
								Srm = this.grid.Srm,
							};
						}

						// run init script
						this.grid.RunScript();

						// show script editor window
						if (!scriptEditor.Visible)
						{
							scriptEditor.Show();
						}
					}
				}
#endif
			}
		}

		private void NewFile()
		{
			if (!CloseDocument())
			{
				return;
			}

			this.grid.Reset();

			this.Text = LangResource.Untitled + " - ReoGrid Editor " + this.ProductVersion;

			//showGridLinesToolStripMenuItem.Checked = workbook.HasSettings(ReoGridSettings.View_ShowGridLine);
			this.CurrentFilePath = null;
			this.currentTempFilePath = null;

#if DEBUG // for test case
			//showDebugFormToolStripButton.PerformClick();
			ForTest();
#endif
		}

		private void SaveFile(string path)
		{
#if EX_SCRIPT
			if (scriptEditor != null && scriptEditor.Visible)
			{
				this.grid.Script = scriptEditor.Script;
			}
#endif // EX_SCRIPT

			//, "ReoGridEditor " + this.ProductVersion.ToString())
			FileFormat fm = FileFormat._Auto;

			if (path.EndsWith(".xlsx", StringComparison.CurrentCultureIgnoreCase))
			{
				fm = FileFormat.Excel2007;
			}
			else if (path.EndsWith(".rgf", StringComparison.CurrentCultureIgnoreCase))
			{
				fm = FileFormat.ReoGridFormat;
			}
			else if (path.EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase))
			{
				fm = FileFormat.CSV;
			}

			try
			{
				this.grid.Save(path, fm);

				this.SetCurrentDocumentFile(path);

#if DEBUG
				RGUtility.OpenFileOrLink(path);
#endif
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, "Save error: " + ex.Message, "Save Workbook", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		private void SetCurrentDocumentFile(string filepath)
		{
			this.Text = System.IO.Path.GetFileName(filepath) + " - ReoGrid Editor " + this.ProductVersion;
			this.CurrentFilePath = filepath;
			this.currentTempFilePath = null;
		}

		private void newToolStripButton_Click(object sender, EventArgs e)
		{
			NewFile();
		}

		private void loadToolStripButton_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog ofd = new OpenFileDialog())
			{
				ofd.Filter = LangResource.Filter_Load_File;

				if (ofd.ShowDialog(this) == DialogResult.OK)
				{
					LoadFile(ofd.FileName);
					this.SetCurrentDocumentFile(ofd.FileName);
				}
			}
		}

		/// <summary>
		/// Save current document
		/// </summary>
		public void SaveDocument()
		{
			if (string.IsNullOrEmpty(CurrentFilePath))
			{
				SaveAsDocument();
			}
			else
			{
				SaveFile(this.CurrentFilePath);
			}
		}

		/// <summary>
		/// Save current document by specifying new file path
		/// </summary>
		/// <returns>true if operation is successful, otherwise false</returns>
		public bool SaveAsDocument()
		{
			using (SaveFileDialog sfd = new SaveFileDialog())
			{
				sfd.Filter = LangResource.Filter_Save_File;

				if (!string.IsNullOrEmpty(this.CurrentFilePath))
				{
					sfd.FileName = Path.GetFileNameWithoutExtension(this.CurrentFilePath);

					var format = GetFormatByExtension(this.CurrentFilePath);

					switch (format)
					{
						case FileFormat.Excel2007:
							sfd.FilterIndex = 1;
							break;

						case FileFormat.ReoGridFormat:
							sfd.FilterIndex = 2;
							break;

						case FileFormat.CSV:
							sfd.FilterIndex = 3;
							break;
					}
				}

				if (sfd.ShowDialog(this) == DialogResult.OK)
				{
					SaveFile(sfd.FileName);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Event raised when document has been reset to initial
		/// </summary>
		public bool NewDocumentOnLoad { get; set; }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

#if DEBUG
			// hold control key when quit to save current worksheet automatically
			if (Toolkit.IsKeyDown(unvell.Common.Win32Lib.Win32.VKey.VK_CONTROL))
			{
				FileInfo file = new FileInfo("..\\..\\autosave.sgf");
				if (file.Exists) LoadFile(file.FullName);
			}
#endif

			// load file if specified 
			if (!string.IsNullOrEmpty(CurrentFilePath))
			{
				this.grid.Reset();
				LoadFile(CurrentFilePath);
			}
			else if (NewDocumentOnLoad)
			{
				NewFile();
			}

#if EX_SCRIPT
			if (!string.IsNullOrEmpty(this.grid.Script) && (scriptEditor == null || !scriptEditor.Visible || scriptEditor.IsDisposed))
			{
				scriptEditorToolStripMenuItem.PerformClick();
			}
#endif // EX_SCRIPT

			UpdateSelectionModeAndStyle();
			UpdateSelectionForwardDirection();

			grid.Focus();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

#if DEBUG
			// Uncomment out the code below to allow autosave.
			// Only RGF(ReoGrid Format, *.rgf) is supported.

			// this.CurrentWorksheet.Save("..\\..\\autosave.rgf");
#endif // DEBUG
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			newToolStripButton.PerformClick();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			loadToolStripButton.PerformClick();
		}

		public bool CloseDocument()
		{
			if (this.grid.IsWorkbookEmpty)
			{
				return true;
			}

			var dr = MessageBox.Show(LangResource.Msg_Save_Changes, "ReoGrid Editor", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

			if (dr == System.Windows.Forms.DialogResult.No)
				return true;
			else if (dr == System.Windows.Forms.DialogResult.Cancel)
				return false;

			FileFormat format = FileFormat._Auto;

			if (!string.IsNullOrEmpty(this.CurrentFilePath))
			{
				format = GetFormatByExtension(this.CurrentFilePath);
			}

			if (format == FileFormat._Auto || string.IsNullOrEmpty(this.CurrentFilePath))
			{
				return SaveAsDocument();
			}
			else
			{
				SaveDocument();
			}

			return true;
		}

		private FileFormat GetFormatByExtension(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return FileFormat._Auto;
			}

			string ext = Path.GetExtension(this.CurrentFilePath);

			if (ext.Equals(".rgf", StringComparison.CurrentCultureIgnoreCase)
				|| ext.Equals(".xml", StringComparison.CurrentCultureIgnoreCase))
			{
				return FileFormat.ReoGridFormat;
			}
			else if (ext.Equals(".xlsx", StringComparison.CurrentCultureIgnoreCase))
			{
				return FileFormat.Excel2007;
			}
			else if (ext.Equals(".csv", StringComparison.CurrentCultureIgnoreCase))
			{
				return FileFormat.CSV;
			}
			else
			{
				return FileFormat._Auto;
			}
		}

		#endregion

		#region Alignment
		private void textLeftToolStripButton_Click(object sender, EventArgs e)
		{
			this.grid.DoAction(new SetRangeStyleAction(this.CurrentWorksheet.SelectionRange, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.HorizontalAlign,
				HAlign = ReoGridHorAlign.Left,
			}));
		}
		private void textCenterToolStripButton_Click(object sender, EventArgs e)
		{
			this.grid.DoAction(new SetRangeStyleAction(this.CurrentWorksheet.SelectionRange, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.HorizontalAlign,
				HAlign = ReoGridHorAlign.Center,
			}));
		}
		private void textRightToolStripButton_Click(object sender, EventArgs e)
		{
			this.grid.DoAction(new SetRangeStyleAction(this.CurrentWorksheet.SelectionRange, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.HorizontalAlign,
				HAlign = ReoGridHorAlign.Right,
			}));
		}
		private void distributedIndentToolStripButton_Click(object sender, EventArgs e)
		{
			this.grid.DoAction(new SetRangeStyleAction(this.CurrentWorksheet.SelectionRange, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.HorizontalAlign,
				HAlign = ReoGridHorAlign.DistributedIndent,
			}));
		}

		private void textAlignTopToolStripButton_Click(object sender, EventArgs e)
		{
			this.grid.DoAction(new SetRangeStyleAction(this.CurrentWorksheet.SelectionRange, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.VerticalAlign,
				VAlign = ReoGridVerAlign.Top,
			}));
		}
		private void textAlignMiddleToolStripButton_Click(object sender, EventArgs e)
		{
			this.grid.DoAction(new SetRangeStyleAction(this.CurrentWorksheet.SelectionRange, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.VerticalAlign,
				VAlign = ReoGridVerAlign.Middle,
			}));
		}
		private void textAlignBottomToolStripButton_Click(object sender, EventArgs e)
		{
			this.grid.DoAction(new SetRangeStyleAction(this.CurrentWorksheet.SelectionRange, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.VerticalAlign,
				VAlign = ReoGridVerAlign.Bottom,
			}));
		}
		#endregion

		#region Border Settings

		#region Outside Borders
		private void SetSelectionBorder(BorderPositions borderPos, BorderLineStyle style)
		{
			this.grid.DoAction(new SetRangeBorderAction(this.CurrentWorksheet.SelectionRange, borderPos,
				new RangeBorderStyle { Color = borderColorPickToolStripItem.SolidColor, Style = style }));
		}

		private void boldOutlineToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Outside, BorderLineStyle.BoldSolid);
		}
		private void dottedOutlineToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Outside, BorderLineStyle.Dotted);
		}
		private void boundsSolidToolStripButton_ButtonClick(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Outside, BorderLineStyle.Solid);
		}
		private void solidOutlineToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Outside, BorderLineStyle.Solid);
		}
		private void dashedOutlineToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Outside, BorderLineStyle.Dashed);
		}
		#endregion // Outside Borders

		#region Inside Borders
		private void insideSolidToolStripSplitButton_ButtonClick(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.InsideAll, BorderLineStyle.Solid);
		}
		private void insideSolidToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.InsideAll, BorderLineStyle.Solid);
		}
		private void insideBoldToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.InsideAll, BorderLineStyle.BoldSolid);
		}
		private void insideDottedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.InsideAll, BorderLineStyle.Dotted);
		}
		private void insideDashedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.InsideAll, BorderLineStyle.Dashed);
		}
		#endregion // Inside Borders

		#region Left & Right Borders
		private void leftRightSolidToolStripSplitButton_ButtonClick(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.LeftRight, BorderLineStyle.Solid);
		}
		private void leftRightSolidToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.LeftRight, BorderLineStyle.Solid);
		}
		private void leftRightBoldToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.LeftRight, BorderLineStyle.BoldSolid);
		}
		private void leftRightDotToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.LeftRight, BorderLineStyle.Dotted);
		}
		private void leftRightDashToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.LeftRight, BorderLineStyle.Dashed);
		}
		#endregion // Left & Right Borders

		#region Top & Bottom Borders
		private void topBottomSolidToolStripSplitButton_ButtonClick(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.TopBottom, BorderLineStyle.Solid);
		}
		private void topBottomSolidToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.TopBottom, BorderLineStyle.Solid);
		}
		private void topBottomBoldToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.TopBottom, BorderLineStyle.BoldSolid);
		}
		private void topBottomDotToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.TopBottom, BorderLineStyle.Dotted);
		}
		private void topBottomDashToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.TopBottom, BorderLineStyle.Dashed);
		}
		#endregion // Top & Bottom Borders

		#region All Borders
		private void allSolidToolStripSplitButton_ButtonClick(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.All, BorderLineStyle.Solid);
		}
		private void allSolidToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.All, BorderLineStyle.Solid);
		}
		private void allBoldToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.All, BorderLineStyle.BoldSolid);
		}
		private void allDottedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.All, BorderLineStyle.Dotted);
		}
		private void allDashedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.All, BorderLineStyle.Dashed);
		}
		#endregion // All Borders

		#region Left Border
		private void leftSolidToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Left, BorderLineStyle.Solid);
		}

		private void leftSolidToolStripButton_ButtonClick(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Left, BorderLineStyle.Solid);
		}

		private void leftBoldToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Left, BorderLineStyle.BoldSolid);
		}

		private void leftDotToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Left, BorderLineStyle.Dotted);
		}

		private void leftDashToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Left, BorderLineStyle.Dashed);
		}
		#endregion // Left Border

		#region Top Border
		private void topSolidToolStripButton_ButtonClick(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Top, BorderLineStyle.Solid);
		}

		private void topSolidToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Top, BorderLineStyle.Solid);
		}

		private void topBlodToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Top, BorderLineStyle.BoldSolid);
		}

		private void topDotToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Top, BorderLineStyle.Dotted);
		}

		private void topDashToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Top, BorderLineStyle.Dashed);
		}
		#endregion // Top Border

		#region Bottom Border
		private void bottomToolStripButton_ButtonClick(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Bottom, BorderLineStyle.Solid);
		}

		private void bottomSolidToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Bottom, BorderLineStyle.Solid);
		}

		private void bottomBoldToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Bottom, BorderLineStyle.BoldSolid);
		}

		private void bottomDotToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Bottom, BorderLineStyle.Dotted);
		}

		private void bottomDashToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Bottom, BorderLineStyle.Dashed);
		}
		#endregion // Bottom Border

		#region Right Border
		private void rightSolidToolStripButton_ButtonClick(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Right, BorderLineStyle.Solid);
		}

		private void rightSolidToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Right, BorderLineStyle.Solid);
		}

		private void rightBoldToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Right, BorderLineStyle.BoldSolid);
		}

		private void rightDotToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Right, BorderLineStyle.Dotted);
		}

		private void rightDashToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Right, BorderLineStyle.Dashed);
		}
		#endregion // Right Border

		#region Slash
		private void slashRightSolidToolStripButton_ButtonClick(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Slash, BorderLineStyle.Solid);
		}

		private void slashRightSolidToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Slash, BorderLineStyle.Solid);
		}

		private void slashRightBoldToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Slash, BorderLineStyle.BoldSolid);
		}

		private void slashRightDotToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Slash, BorderLineStyle.Dotted);
		}

		private void slashRightDashToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Slash, BorderLineStyle.Dashed);
		}
		#endregion // Slash

		#region Backslash
		private void slashLeftSolidToolStripButton_ButtonClick(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Backslash, BorderLineStyle.Solid);
		}

		private void slashLeftSolidToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Backslash, BorderLineStyle.Solid);
		}

		private void slashLeftBoldToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Backslash, BorderLineStyle.BoldSolid);
		}

		private void slashLeftDotToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Backslash, BorderLineStyle.Dotted);
		}

		private void slashLeftDashToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetSelectionBorder(BorderPositions.Backslash, BorderLineStyle.Dashed);
		}
		#endregion // Backslash

		#region Clear Borders
		private void clearBordersToolStripButton_Click(object sender, EventArgs e)
		{
			this.grid.DoAction(new SetRangeBorderAction(this.CurrentWorksheet.SelectionRange, BorderPositions.All,
				new RangeBorderStyle { Color = Color.Empty, Style = BorderLineStyle.None }));
		}
		#endregion // Clear Borders

		#endregion // Border Settings

		#region Style
		private void backColorPickerToolStripButton_ColorPicked(object sender, EventArgs e)
		{
			//Color c = backColorPickerToolStripButton.SolidColor;
			//if (c.IsEmpty)
			//{
			//  workbook.DoAction(new SGRemoveRangeStyleAction(workbook.SelectionRange, PlainStyleFlag.FillColor));
			//}
			//else
			//{
			this.grid.DoAction(new SetRangeStyleAction(this.CurrentWorksheet.SelectionRange, new WorksheetRangeStyle()
			{
				Flag = PlainStyleFlag.BackColor,
				BackColor = backColorPickerToolStripButton.SolidColor,
			}));
			//}
		}

		private void textColorPickToolStripItem_ColorPicked(object sender, EventArgs e)
		{
			var color = textColorPickToolStripItem.SolidColor;

			if (color.IsEmpty)
			{
				this.grid.DoAction(new RemoveRangeStyleAction(this.CurrentWorksheet.SelectionRange, PlainStyleFlag.TextColor));
			}
			else
			{
				this.grid.DoAction(new SetRangeStyleAction(this.CurrentWorksheet.SelectionRange, new WorksheetRangeStyle
				{
					Flag = PlainStyleFlag.TextColor,
					TextColor = color,
				}));
			}
		}

		private void boldToolStripButton_Click(object sender, EventArgs e)
		{
			this.grid.DoAction(new SetRangeStyleAction(this.CurrentWorksheet.SelectionRange, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.FontStyleBold,
				Bold = boldToolStripButton.Checked,
			}));
		}

		private void italicToolStripButton_Click(object sender, EventArgs e)
		{
			this.grid.DoAction(new SetRangeStyleAction(this.CurrentWorksheet.SelectionRange, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.FontStyleItalic,
				Italic = italicToolStripButton.Checked,
			}));
		}

		private void underlineToolStripButton_Click(object sender, EventArgs e)
		{
			this.grid.DoAction(new SetRangeStyleAction(this.CurrentWorksheet.SelectionRange, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.FontStyleUnderline,
				Underline = underlineToolStripButton.Checked,
			}));
		}

		private void strikethroughToolStripButton_Click(object sender, EventArgs e)
		{
			this.grid.DoAction(new SetRangeStyleAction(this.CurrentWorksheet.SelectionRange, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.FontStyleStrikethrough,
				Strikethrough = strikethroughToolStripButton.Checked,
			}));
		}

		private void styleBrushToolStripButton_Click(object sender, EventArgs e)
		{
			this.CurrentWorksheet.StartPickRangeAndCopyStyle();
		}

		private void enlargeFontToolStripButton_Click(object sender, EventArgs e)
		{
			this.grid.DoAction(new StepRangeFontSizeAction(this.CurrentWorksheet.SelectionRange, true));
			UpdateMenuAndToolStrips();
		}

		private void fontSmallerToolStripButton_Click(object sender, EventArgs e)
		{
			this.grid.DoAction(new StepRangeFontSizeAction(this.CurrentWorksheet.SelectionRange, false));
			UpdateMenuAndToolStrips();
		}

		private void fontToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (isUIUpdating) return;

			this.grid.DoAction(new SetRangeStyleAction(this.CurrentWorksheet.SelectionRange, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.FontName,
				FontName = fontToolStripComboBox.Text,
			}));
		}

		private void fontSizeToolStripComboBox_TextChanged(object sender, EventArgs e)
		{
			SetGridFontSize();
		}

		private void SetGridFontSize()
		{
			if (isUIUpdating) return;

			float size = 9;
			float.TryParse(fontSizeToolStripComboBox.Text, out size);

			if (size <= 0) size = 1f;

			this.grid.DoAction(new SetRangeStyleAction(this.CurrentWorksheet.SelectionRange, new WorksheetRangeStyle
			{
				Flag = PlainStyleFlag.FontSize,
				FontSize = size,
			}));
		}

		#endregion

		#region Cell & Range
		private void MergeSelectionRange(object sender, EventArgs e)
		{
			try
			{
				this.grid.DoAction(new MergeRangeAction(this.CurrentWorksheet.SelectionRange));
			}
			catch (RangeTooSmallException) { }
			catch (RangeIntersectionException)
			{
				MessageBox.Show(LangResource.Msg_Range_Intersection_Exception,
					Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void UnmergeSelectionRange(object sender, EventArgs e)
		{
			this.grid.DoAction(new UnmergeRangeAction(this.CurrentWorksheet.SelectionRange));
		}

		void resizeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var worksheet = this.CurrentWorksheet;

			using (var rgf = new ResizeGridDialog())
			{
				rgf.Rows = worksheet.RowCount;
				rgf.Cols = worksheet.ColumnCount;

				if (rgf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					WorksheetActionGroup ag = new WorksheetActionGroup();

					if (rgf.Rows < worksheet.RowCount)
					{
						ag.Actions.Add(new RemoveRowsAction(rgf.Rows, worksheet.RowCount - rgf.Rows));
					}
					else if (rgf.Rows > worksheet.RowCount)
					{
						ag.Actions.Add(new InsertRowsAction(worksheet.RowCount, rgf.Rows - worksheet.RowCount));
					}

					if (rgf.Cols < worksheet.ColumnCount)
					{
						ag.Actions.Add(new RemoveColumnsAction(rgf.Cols, worksheet.ColumnCount - rgf.Cols));
					}
					else if (rgf.Cols > worksheet.ColumnCount)
					{
						ag.Actions.Add(new InsertColumnsAction(worksheet.ColumnCount, rgf.Cols - worksheet.ColumnCount));
					}

					if (ag.Actions.Count > 0)
					{
						Cursor = Cursors.WaitCursor;

						try
						{
							this.grid.DoAction(ag);
						}
						finally
						{
							Cursor = Cursors.Default;
						}
					}
				}
			}
		}

		void ApplyFunctionToSelectedRange(string funName)
		{
			var sheet = this.CurrentWorksheet;
			var range = this.CurrentSelectionRange;

			// fill bottom rows
			if (range.Rows > 1)
			{
				for (int c = range.Col; c <= range.EndCol; c++)
				{
					var cell = sheet.Cells[range.EndRow, c];

					if (string.IsNullOrEmpty(cell.DisplayText))
					{
						cell.Formula = string.Format("{0}({1})", funName,
							RangePosition.FromCellPosition(range.Row, range.Col, range.EndRow - 1, c).ToAddress());
						break;
					}
				}
			}

			// fill right columns
			if (range.Cols > 1)
			{
				for (int r = range.Row; r <= range.EndRow; r++)
				{
					var cell = sheet.Cells[r, range.EndCol];

					if (string.IsNullOrEmpty(cell.DisplayText))
					{
						cell.Formula = string.Format("{0}({1})", funName,
							RangePosition.FromCellPosition(range.Row, range.Col, r, range.EndCol - 1).ToAddress());
						break;
					}
				}
			}
		}
		#endregion

		#region Context Menu
		private void insertColToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.CurrentSelectionRange.Cols >= 1)
			{
				this.grid.DoAction(new InsertColumnsAction(CurrentSelectionRange.Col, CurrentSelectionRange.Cols));
			}
		}
		private void insertRowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.CurrentSelectionRange.Rows >= 1)
			{
				this.grid.DoAction(new InsertRowsAction(CurrentSelectionRange.Row, CurrentSelectionRange.Rows));
			}
		}
		private void deleteColumnToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.CurrentSelectionRange.Cols >= 1)
			{
				this.grid.DoAction(new RemoveColumnsAction(CurrentSelectionRange.Col, CurrentSelectionRange.Cols));
			}
		}
		private void deleteRowsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.CurrentSelectionRange.Rows >= 1)
			{
				this.grid.DoAction(new RemoveRowsAction(CurrentSelectionRange.Row, CurrentSelectionRange.Rows));
			}
		}
		private void resetToDefaultWidthToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.grid.DoAction(new SetColumnsWidthAction(CurrentSelectionRange.Col, CurrentSelectionRange.Cols, Worksheet.InitDefaultColumnWidth));
		}
		private void resetToDefaultHeightToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.grid.DoAction(new SetRowsHeightAction(CurrentSelectionRange.Row, CurrentSelectionRange.Rows, Worksheet.InitDefaultRowHeight));
		}
		#endregion

		#region Debug Form
#if DEBUG
		private DebugForm cellDebugForm = null;
		private DebugForm borderDebugForm = null;

		private void showDebugFormToolStripButton_Click(object sender, EventArgs e)
		{
			if (cellDebugForm == null)
			{
				cellDebugForm = new DebugForm();
				cellDebugForm.VisibleChanged += (ss, se) => showDebugFormToolStripButton.Checked = cellDebugForm.Visible;
			}
			else if (cellDebugForm.Visible)
			{
				cellDebugForm.Visible = false;
				borderDebugForm.Visible = false;
				return;
			}

			cellDebugForm.Grid = this.CurrentWorksheet;

			if (!cellDebugForm.Visible)
			{
				cellDebugForm.InitTabType = DebugForm.InitTab.Grid;
				cellDebugForm.Show(this);
			}

			if (borderDebugForm == null)
			{
				borderDebugForm = new DebugForm();
				borderDebugForm.Grid = this.CurrentWorksheet;
			}

			if (!borderDebugForm.Visible)
			{
				borderDebugForm.InitTabType = DebugForm.InitTab.Border;
				borderDebugForm.Show(this);
			}

			if (cellDebugForm.Visible || borderDebugForm.Visible) ResetDebugFormLocation();
		}
#endif // DEBUG

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
		}

		protected override void OnMove(EventArgs e)
		{
			base.OnMove(e);

#if DEBUG
			ResetDebugFormLocation();
#endif // DEBUG
		}

#if DEBUG
		private void ResetDebugFormLocation()
		{
			if (cellDebugForm != null && cellDebugForm.Visible)
			{
				cellDebugForm.Location = new Point(this.Right, this.Top);
			}
			if (borderDebugForm != null && borderDebugForm.Visible)
			{
				borderDebugForm.Location = new Point(cellDebugForm.Left, cellDebugForm.Bottom);
			}
		}
#endif // DEBUG
		#endregion // Debug Form

		#region Editing
		private void cutRangeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Cut();
		}
		private void copyRangeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Copy();
		}
		private void pasteRangeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Paste();
		}

		private void Cut()
		{
			// Cut method will always perform action to do cut
			try
			{
				this.CurrentWorksheet.Cut();
			}
			catch (RangeIntersectionException)
			{
				MessageBox.Show(LangResource.Msg_Range_Intersection_Exception);
			}
			catch
			{
				MessageBox.Show(LangResource.Msg_Operation_Aborted);
			}
		}

		private void Copy()
		{
			try
			{
				this.CurrentWorksheet.Copy();
			}
			catch (RangeIntersectionException)
			{
				MessageBox.Show(LangResource.Msg_Range_Intersection_Exception);
			}
			catch
			{
				MessageBox.Show(LangResource.Msg_Operation_Aborted);
			}
		}

		private void Paste()
		{
			try
			{
				this.CurrentWorksheet.Paste();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void cutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			cutToolStripButton.PerformClick();
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			copyToolStripButton.PerformClick();
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			pasteToolStripButton.PerformClick();
		}

		private void repeatLastActionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.grid.RepeatLastAction(this.CurrentWorksheet.SelectionRange);
		}

		private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.CurrentWorksheet.SelectAll();
		}

		#endregion // Editing

		#region Window
		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void newWindowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new ReoGridEditor().Show();
		}

		private void styleEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ControlAppearanceEditorForm styleEditor = new ControlAppearanceEditorForm();
			styleEditor.Grid = this.grid;
			styleEditor.Show(this);
		}
		#endregion // Window

		#region View & Print

		private void formatCellToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (PropertyForm form = new PropertyForm(this.grid))
			{
				form.ShowDialog(this);
			}
		}

		private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
		{
			printPreviewToolStripButton.PerformClick();
		}

		private void printPreviewToolStripButton_Click(object sender, EventArgs e)
		{
			try
			{
				this.grid.CurrentWorksheet.AutoSplitPage();

				this.grid.CurrentWorksheet.EnableSettings(WorksheetSettings.View_ShowPageBreaks);
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, Application.ProductName + " " + Application.ProductVersion,
					MessageBoxButtons.OK, MessageBoxIcon.Information);

				return;
			}

			using (var session = this.grid.CurrentWorksheet.CreatePrintSession())
			{
				using (PrintPreviewDialog ppd = new PrintPreviewDialog())
				{
					ppd.Document = session.PrintDocument;
					ppd.SetBounds(200, 200, 1024, 768);
					ppd.PrintPreviewControl.Zoom = 1d;
					ppd.ShowDialog(this);
				}
			}
		}

		private void PrintToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PrintSession session = null;

			try
			{
				session = grid.CurrentWorksheet.CreatePrintSession();
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, this.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			using (var pd = new System.Windows.Forms.PrintDialog())
			{
				pd.Document = session.PrintDocument;
				pd.UseEXDialog = true;

				if (pd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					session.Print();
				}
			}

			if (session != null) session.Dispose();
		}

		void removeRowPageBreakToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.grid.CurrentWorksheet.RemoveRowPageBreak(this.grid.CurrentWorksheet.FocusPos.Row);
		}

		void printSettingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (PrintSettingsDialog psf = new PrintSettingsDialog())
			{
				var sheet = this.grid.CurrentWorksheet;

				if (sheet.PrintSettings == null)
				{
					sheet.PrintSettings = new PrintSettings();
				}

				psf.PrintSettings = (PrintSettings)sheet.PrintSettings.Clone();

				if (psf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					sheet.PrintSettings = psf.PrintSettings;
					sheet.AutoSplitPage();
					sheet.EnableSettings(WorksheetSettings.View_ShowPageBreaks);
				}
			}
		}

		void removeColPageBreakToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				this.grid.CurrentWorksheet.RemoveColumnPageBreak(this.grid.CurrentWorksheet.FocusPos.Col);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		void insertRowPageBreakToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				this.CurrentWorksheet.RowPageBreaks.Add(this.CurrentWorksheet.FocusPos.Row);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		void insertColPageBreakToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.CurrentWorksheet.ColumnPageBreaks.Add(this.CurrentWorksheet.FocusPos.Col);
		}

		#endregion // View & Print

		#region Freeze

		private void FreezeToEdge(FreezeArea freezePos)
		{
			var worksheet = this.CurrentWorksheet;

			if (!worksheet.SelectionRange.IsEmpty)
			{
				worksheet.FreezeToCell(worksheet.FocusPos, freezePos);
				UpdateMenuAndToolStrips();
			}
		}

		private void unfreezeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.CurrentWorksheet.Unfreeze();
			UpdateMenuAndToolStrips();
		}

		#endregion // Freeze

		#region Help
		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new AboutForm().ShowDialog(this);
		}
		#endregion

		#region Outline

		void groupRowsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				this.grid.DoAction(new AddOutlineAction(RowOrColumn.Row, this.CurrentSelectionRange.Row, this.CurrentSelectionRange.Rows));
			}
			catch (OutlineOutOfRangeException)
			{
				MessageBox.Show(LangResource.Msg_Outline_Out_Of_Range);
			}
			catch (OutlineAlreadyDefinedException)
			{
				MessageBox.Show(LangResource.Msg_Outline_Already_Exist);
			}
			catch (OutlineIntersectedException)
			{
				MessageBox.Show(LangResource.Msg_Outline_Intersected);
			}
			catch (OutlineTooMuchException)
			{
				MessageBox.Show(LangResource.Msg_Outline_Too_Much);
			}
		}

		void ungroupRowsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var removeOutlineAction = new RemoveOutlineAction(RowOrColumn.Row, this.CurrentSelectionRange.Row, this.CurrentSelectionRange.Rows);

			try
			{
				this.grid.DoAction(removeOutlineAction);
			}
			catch { }

			if (removeOutlineAction.RemovedOutline == null)
			{
				MessageBox.Show(LangResource.Msg_Outline_Not_Found);
			}
		}

		void groupColumnsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var worksheet = this.CurrentWorksheet;

			try
			{
				this.grid.DoAction(new AddOutlineAction(RowOrColumn.Column, this.CurrentSelectionRange.Col, this.CurrentSelectionRange.Cols));
			}
			catch (OutlineOutOfRangeException)
			{
				MessageBox.Show(LangResource.Msg_Outline_Out_Of_Range);
			}
			catch (OutlineAlreadyDefinedException)
			{
				MessageBox.Show(LangResource.Msg_Outline_Already_Exist);
			}
			catch (OutlineIntersectedException)
			{
				MessageBox.Show(LangResource.Msg_Outline_Intersected);
			}
			catch (OutlineTooMuchException)
			{
				MessageBox.Show(LangResource.Msg_Outline_Too_Much);
			}
		}

		void ungroupColumnsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var removeOutlineAction = new RemoveOutlineAction(RowOrColumn.Column, this.CurrentSelectionRange.Col, this.CurrentSelectionRange.Cols);

			try
			{
				this.grid.DoAction(removeOutlineAction);
			}
			catch { }

			if (removeOutlineAction.RemovedOutline == null)
			{
				MessageBox.Show(LangResource.Msg_Outline_Not_Found);
			}
		}

		void ungroupAllRowsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.grid.DoAction(new ClearOutlineAction(RowOrColumn.Row));
		}

		void ungroupAllColumnsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.grid.DoAction(new ClearOutlineAction(RowOrColumn.Column));
		}

		#endregion // Outline

		#region Filter
		private AutoColumnFilter columnFilter;

		void filterToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.columnFilter != null)
			{
				this.columnFilter.Detach();
			}

			CreateAutoFilterAction action = new CreateAutoFilterAction(this.CurrentWorksheet.SelectionRange);
			this.grid.DoAction(action);

			this.columnFilter = action.AutoColumnFilter;
		}

		void clearFilterToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.columnFilter != null)
			{
				columnFilter.Detach();
			}
		}
		#endregion // Filter

		#if DEBUG
		private void ForTest()
		{
			var sheet = this.grid.CurrentWorksheet;

		}

#endif // DEBUG
	}
}
