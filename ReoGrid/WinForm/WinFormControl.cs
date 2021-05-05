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

#if WINFORM

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using unvell.Common.Win32Lib;
using unvell.Common;
using unvell.ReoGrid.Views;
using unvell.ReoGrid.DataFormat;
using unvell.ReoGrid.Properties;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.WinForm.Designer;
using unvell.ReoGrid.Events;
using unvell.ReoGrid.Actions;
using unvell.ReoGrid.Core;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Interaction;
using unvell.ReoGrid.Main;

using DrawMode = unvell.ReoGrid.Rendering.DrawMode;
using Rectangle = unvell.ReoGrid.Graphics.Rectangle;

using Point = System.Drawing.Point;
using Size = System.Drawing.Size;
using WFRect = System.Drawing.Rectangle;
using unvell.ReoGrid.WinForm;

namespace unvell.ReoGrid
{
	/// <summary>
	/// ReoGrid - .NET Spreadsheet Component for Windows Form
	/// </summary>

	//[DefaultProperty("Template")]
	//#if WINFORM
	//	[Designer(typeof(ReoGridControlDesigner))]
	//	[ToolboxItem(typeof(ReoGridControlToolboxItem))]
	//#endif // WINFORM

	public partial class ReoGridControl : System.Windows.Forms.Control, IVisualWorkbook,
		IRangePickableControl, IContextMenuControl, IPersistenceWorkbook, IActionControl, IWorkbook,
		IScrollableWorksheetContainer
	{
		#region Constructor, Init & Dispose
		private WinFormControlAdapter adapter;

		//private Graphics canvasGraphics;
		/// <summary>
		/// Create component instance.
		/// </summary>
		public ReoGridControl()
		{
			SuspendLayout();

			BackColor = Color.White;
			DoubleBuffered = true;
			TabStop = true;

			#region Scroll bars & panels

			hScrollBar = new HScrollBar()
			{
				Dock = DockStyle.Fill,
				SmallChange = Worksheet.InitDefaultColumnWidth,
			};

			hScrollBar.Scroll += OnHorScroll;
			hScrollBar.MouseEnter += (sender, e) => Cursor = Cursors.Default;

			vScrollBar = new VScrollBar()
			{
				Dock = DockStyle.Right,
				SmallChange = Worksheet.InitDefaultRowHeight,
			};

			vScrollBar.Scroll += OnVerScroll;
			vScrollBar.MouseEnter += (sender, e) => Cursor = Cursors.Default;

			vScrollBar.MouseEnter += (s, e) => this.Cursor = Cursors.Default;
			hScrollBar.MouseEnter += (s, e) => this.Cursor = Cursors.Default;

			this.Controls.Add(vScrollBar);

			Controls.Add(vScrollBar);

			hScrollBar.BringToFront();

			#endregion // Scroll bars & panels

			#region Sheet tabs

			this.sheetTab = new WinForm.SheetTabControl(this)
			{
				Dock = DockStyle.Left,
				Width = 400,
				Height = 26,
				Position = SheetTabControlPosition.Bottom,
				//ForeColor = SystemColors.WindowText,
				//BorderColor = SystemColors.Highlight,
				//SelectedBackColor = Color.White,
				//SelectedTextColor = SystemColors.WindowText,
			};

			this.sheetTab.MouseMove += (s, e) => this.Cursor = Cursors.Default;

			this.sheetTab.SplitterMoving += (s, e) =>
			{
				if (this.sheetTab.Dock != DockStyle.Fill)
				{
					var p = this.bottomPanel.PointToClient(Cursor.Position);
					int newWidth = p.X - this.sheetTab.Left;

					if (newWidth < 50)
					{
						newWidth = 50;
					}

					this.sheetTab.Width = newWidth;
				}
			};

			this.sheetTab.SheetListClick += (s, e) =>
			{
				if (this.sheetListMenu == null)
				{
					this.sheetListMenu = new ContextMenuStrip();

					this.sheetListMenu.Items.Clear();

					foreach (var sheet in this.Worksheets)
					{
						var sheetMenuItem = new ToolStripMenuItem(sheet.Name) { Tag = sheet };
						sheetMenuItem.Click += sheetMenuItem_Click;
						this.sheetListMenu.Items.Add(sheetMenuItem);
					}
				}

				var p = Cursor.Position;
				p.Y -= this.sheetListMenu.Height / 2;
				this.sheetListMenu.Show(p);
			};

			this.sheetTab.TabMouseDown += (s, e) =>
			{
				if (e.MouseButtons == unvell.ReoGrid.Interaction.MouseButtons.Right)
				{
					if (this.sheetContextMenu == null)
					{
						if (this.SheetTabContextMenuStrip != null)
						{
							this.sheetContextMenu = this.SheetTabContextMenuStrip;
						}
						else
						{
							this.sheetContextMenu = new ContextMenuStrip();

							#region Add sheet context menu items
							var insertSheetMenu = new ToolStripMenuItem(LanguageResource.Menu_InsertSheet);
							insertSheetMenu.Click += (ss, ee) =>
							{
								var sheet = this.CreateWorksheet();
								if (sheet != null)
								{
									this.workbook.InsertWorksheet(this.sheetTab.SelectedIndex, sheet);
									this.CurrentWorksheet = sheet;
								}
							};

							var deleteSheetMenu = new ToolStripMenuItem(LanguageResource.Menu_DeleteSheet);
							deleteSheetMenu.Click += (ss, ee) =>
							{
								if (this.workbook.WorksheetCount > 1)
								{
									this.workbook.RemoveWorksheet(this.sheetTab.SelectedIndex);
								}
							};

							var renameSheetMenu = new ToolStripMenuItem(LanguageResource.Menu_RenameSheet);
							renameSheetMenu.Click += (ss, ee) =>
							{
								var index = this.sheetTab.SelectedIndex;
								if (index >= 0 && index < this.workbook.worksheets.Count)
								{
									var sheet = this.workbook.worksheets[this.sheetTab.SelectedIndex];
									if (sheet != null)
									{
										using (var rsd = new unvell.ReoGrid.WinForm.RenameSheetDialog())
										{
											var rect = this.sheetTab.GetItemBounds(this.sheetTab.SelectedIndex);

											var p = this.sheetTab.PointToScreen(rect.Location);
											p.X -= (rsd.Width - rect.Width) / 2;
											p.Y -= rsd.Height + 5;

											rsd.Location = p;
											rsd.SheetName = sheet.Name;

											if (rsd.ShowDialog() == DialogResult.OK)
											{
												sheet.Name = rsd.SheetName;
											}
										}
									}
								}
							};

							this.sheetContextMenu.Items.Add(insertSheetMenu);
							this.sheetContextMenu.Items.Add(deleteSheetMenu);
							this.sheetContextMenu.Items.Add(renameSheetMenu);
							#endregion // Add sheet context menu items
						}
					}

					this.sheetContextMenu.Show(this.sheetTab, e.Location);
				}
			};


			#endregion // Sheet tabs

			#region Bottom Panel

			this.bottomPanel = new Panel
			{
				Dock = DockStyle.Bottom,
				Height = this.hScrollBar.Height,
				BackColor = SystemColors.Control,
			};

			this.bottomPanel.Controls.Add(this.hScrollBar);

			this.bottomPanel.Controls.Add(this.sheetTab);

			this.bottomPanel.Controls.Add(new ScrollBarCorner()
			{
				Dock = DockStyle.Right,
				Size = new Size(SystemInformation.HorizontalScrollBarHeight, SystemInformation.HorizontalScrollBarHeight),
				BackColor = SystemColors.Control,
				TabStop = false,
			});

			Controls.Add(bottomPanel);

			#endregion // Bottom Panel

			this.InitControl();

			ResumeLayout();

			//TODO: detect clipboard changes
			// need detect and remove the hightlight range when content has been removed from System Clipboard
			//ClipboardMonitor.Instance.ClipboardChanged += new EventHandler<ClipboardChangedEventArgs>(ClipboardMonitor_ClipboardChanged);

			this.adapter = new WinFormControlAdapter(this);

			this.editTextbox = new InputTextBox(this) { Visible = false, BorderStyle = System.Windows.Forms.BorderStyle.None, Multiline = true };
			this.Controls.Add(this.editTextbox);
			this.adapter.editTextbox = this.editTextbox;

			this.InitWorkbook(this.adapter);

#if WINFORM && DEBUG
			Logger.RegisterWritter(WinForm.RGDebugLogWritter.Instance);
#endif // WINFORM && DEBUG

			//this.sheetTab.VisibleChanged += canvasElements_VisibleChanged;
			//this.hScrollBar.VisibleChanged += canvasElements_VisibleChanged;
			this.bottomPanel.VisibleChanged += canvasElements_VisibleChanged;
			this.vScrollBar.VisibleChanged += canvasElements_VisibleChanged;

			var g = System.Drawing.Graphics.FromHwnd(this.Handle);

			this.renderer = new GDIRenderer(g);
		}

		void canvasElements_VisibleChanged(object sender, EventArgs e)
		{
			if (this.currentWorksheet != null)
			{
				this.currentWorksheet.UpdateViewportControllBounds();
			}
		}

		/// <summary>
		/// Release resources used in this component.
		/// </summary>
		/// <param name="disposing">True to release both managed and unmanaged resources;
		/// False to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			var gdiRenderer = (GDIRenderer)renderer;

			if (gdiRenderer != null)
			{
				gdiRenderer.Dispose();
			}

			base.Dispose(disposing);

			this.workbook.Dispose();

			if (builtInCellsSelectionCursor != null) builtInCellsSelectionCursor.Dispose();
			if (defaultPickRangeCursor != null) defaultPickRangeCursor.Dispose();
			if (builtInFullColSelectCursor != null) builtInFullColSelectCursor.Dispose();
			if (builtInFullRowSelectCursor != null) builtInFullRowSelectCursor.Dispose();
		}

		#endregion // Constructor

		#region Adapter

		private class WinFormControlAdapter : IControlAdapter//, IPlatformDependencyInterface
		{
			private readonly ReoGridControl control;
			internal InputTextBox editTextbox;

			public WinFormControlAdapter(ReoGridControl control)
			{
				this.control = control;
			}

			#region Scroll
			public int ScrollBarHorizontalMaximum
			{
				get { return this.control.hScrollBar.Maximum; }
				set { this.control.hScrollBar.Maximum = value; }
			}

			public int ScrollBarHorizontalMinimum
			{
				get { return this.control.hScrollBar.Minimum; }
				set { this.control.hScrollBar.Minimum = value; }
			}

			public int ScrollBarHorizontalValue
			{
				get { return this.control.hScrollBar.Value; }
				set { this.control.hScrollBar.Value = value; }
			}

			public int ScrollBarHorizontalLargeChange
			{
				get { return this.control.hScrollBar.LargeChange; }
				set { this.control.hScrollBar.LargeChange = value; }
			}

			public int ScrollBarVerticalMaximum
			{
				get { return this.control.vScrollBar.Maximum; }
				set { this.control.vScrollBar.Maximum = value; }
			}

			public int ScrollBarVerticalMinimum
			{
				get { return this.control.vScrollBar.Minimum; }
				set { this.control.vScrollBar.Minimum = value; }
			}

			public int ScrollBarVerticalValue
			{
				get { return this.control.vScrollBar.Value; }
				set { this.control.vScrollBar.Value = value; }
			}

			public int ScrollBarVerticalLargeChange
			{
				get { return this.control.vScrollBar.LargeChange; }
				set { this.control.vScrollBar.LargeChange = value; }
			}

			#endregion

			#region Cursor & Context Menu
			public void ShowContextMenuStrip(ViewTypes viewType, Graphics.Point containerLocation)
			{
				Point p = Point.Round(containerLocation);

				switch (viewType)
				{
					default:
					case ViewTypes.Cells:
						if (this.control.ContextMenuStrip != null)
							this.control.ContextMenuStrip.Show(this.control, p);
						break;

					case ViewTypes.ColumnHeader:
						if (this.control.columnHeaderContextMenuStrip != null)
							this.control.columnHeaderContextMenuStrip.Show(this.control, p);
						break;

					case ViewTypes.RowHeader:
						if (this.control.rowHeaderContextMenuStrip != null)
							this.control.rowHeaderContextMenuStrip.Show(this.control, p);
						break;

					case ViewTypes.LeadHeader:
						if (this.control.leadHeaderContextMenuStrip != null)
							this.control.leadHeaderContextMenuStrip.Show(this.control, p);
						break;
				}
			}

			private Cursor oldCursor = null;
			public void ChangeCursor(CursorStyle cursor)
			{
				oldCursor = this.control.Cursor;

				switch (cursor)
				{
					default:
					case CursorStyle.PlatformDefault: this.control.Cursor = Cursors.Default; break;
					case CursorStyle.Selection: this.control.Cursor = this.control.internalCurrentCursor; break;
					case CursorStyle.Busy: this.control.Cursor = Cursors.WaitCursor; break;
					case CursorStyle.Hand: this.control.Cursor = Cursors.Hand; break;
					case CursorStyle.FullColumnSelect:
						this.control.Cursor = this.control.FullColumnSelectionCursor != null ?
							this.control.FullColumnSelectionCursor : this.control.builtInFullColSelectCursor;
						break;
					case CursorStyle.FullRowSelect:
						this.control.Cursor = this.control.FullRowSelectionCursor != null ?
							this.control.FullRowSelectionCursor : this.control.builtInFullRowSelectCursor;
						break;
					case CursorStyle.EntireSheet:
						this.control.Cursor = this.control.EntireSheetSelectionCursor != null ?
							this.control.EntireSheetSelectionCursor : this.control.builtInEntireSheetSelectCursor;
						break;
					case CursorStyle.ChangeColumnWidth: this.control.Cursor = Cursors.VSplit; break;
					case CursorStyle.ChangeRowHeight: this.control.Cursor = Cursors.HSplit; break;
					case CursorStyle.ResizeHorizontal: this.control.Cursor = Cursors.SizeWE; break;
					case CursorStyle.ResizeVertical: this.control.Cursor = Cursors.SizeNS; break;
					case CursorStyle.Move: this.control.Cursor = Cursors.SizeAll; break;
					case CursorStyle.Cross: this.control.Cursor = this.control.builtInCrossCursor; break;
				}
			}

			public void RestoreCursor()
			{
				if (this.oldCursor != null)
				{
					this.control.Cursor = this.oldCursor;
				}
			}

			public void ChangeSelectionCursor(CursorStyle cursor)
			{
				switch (cursor)
				{
					default:
					case CursorStyle.PlatformDefault:
						this.control.internalCurrentCursor = this.control.CellsSelectionCursor;
						break;

					case CursorStyle.Hand:
						this.control.internalCurrentCursor = Cursors.Hand;
						break;
				}
			}

			#endregion // Cursor & Context Menu

			#region Edit Control
			public void ShowEditControl(Graphics.Rectangle bounds, Cell cell)
			{
				var rect = new WFRect((int)Math.Round(bounds.Left), (int)Math.Round(bounds.Top),
					(int)Math.Round(bounds.Width), (int)Math.Round(bounds.Height));

				editTextbox.SuspendLayout();
				editTextbox.Bounds = rect;
				editTextbox.TextWrap = cell.IsMergedCell || cell.InnerStyle.TextWrapMode != TextWrapMode.NoWrap;
				editTextbox.InitialSize = rect.Size;
				editTextbox.VAlign = cell.InnerStyle.VAlign;
				editTextbox.Font = cell.RenderFont;
				editTextbox.ForeColor = cell.InnerStyle.HasStyle(PlainStyleFlag.TextColor)
					? cell.InnerStyle.TextColor : this.control.ControlStyle[ControlAppearanceColors.GridText];
				editTextbox.BackColor = cell.InnerStyle.HasStyle(PlainStyleFlag.BackColor)
					? cell.InnerStyle.BackColor : this.control.ControlStyle[ControlAppearanceColors.GridBackground];
				editTextbox.ResumeLayout();

				editTextbox.Visible = true;
				editTextbox.Focus();
			}

			public void HideEditControl()
			{
				editTextbox.Visible = false;
			}

			public void SetEditControlText(string text)
			{
				bool sameBeforeChange = editTextbox.Text == text;

				editTextbox.Text = text;
				editTextbox.SelectionStart = text == null ? 0 : text.Length;

				if (sameBeforeChange)
				{
					this.control.currentWorksheet.RaiseCellEditTextChanging(text);
				}
			}

			public string GetEditControlText()
			{
				return editTextbox.Text;
			}

			public void EditControlSelectAll()
			{
				this.editTextbox.SelectAll();
			}

			public void SetEditControlCaretPos(int pos)
			{
				this.editTextbox.SelectionStart = pos;
			}

			public void SetEditControlAlignment(ReoGridHorAlign align)
			{
				switch (align)
				{
					default:
					case ReoGridHorAlign.Left:
						this.editTextbox.TextAlign = HorizontalAlignment.Left;
						break;

					case ReoGridHorAlign.Center:
						this.editTextbox.TextAlign = HorizontalAlignment.Center;
						break;

					case ReoGridHorAlign.Right:
						this.editTextbox.TextAlign = HorizontalAlignment.Right;
						break;
				}
			}

			public void EditControlApplySystemMouseDown()
			{
				Point p = Cursor.Position;

				Point p2 = this.control.PointToScreen(this.editTextbox.Location);
				p.X -= p2.X;
				p.Y -= p2.Y;

				try
				{
					Win32.SendMessage(this.editTextbox.Handle, (uint)Win32.WMessages.WM_LBUTTONDOWN, new IntPtr(0), new IntPtr(Win32.CreateLParamPoint(p.X, p.Y)));
					Win32.SendMessage(this.editTextbox.Handle, (uint)Win32.WMessages.WM_LBUTTONUP, new IntPtr(0), new IntPtr(Win32.CreateLParamPoint(p.X, p.Y)));
				}
				catch { }
			}

			public void EditControlCopy()
			{
				this.editTextbox.Copy();
			}

			public void EditControlPaste()
			{
				this.editTextbox.Paste();
			}

			public void EditControlCut()
			{
				this.editTextbox.Cut();
			}

			public void EditControlUndo()
			{
				this.editTextbox.Copy();
			}
			#endregion // Edit Control

			#region Control Owner

			public IVisualWorkbook ControlInstance { get { return this.control; } }

			public ControlAppearanceStyle ControlStyle { get { return this.control.controlStyle; } }

			public float BaseScale { get { return 0f; } }
			public float MinScale { get { return 0.1f; } }
			public float MaxScale { get { return 4f; } }

			public ISheetTabControl SheetTabControl
			{
				get { return this.control.sheetTab; }
			}

			public Rectangle GetContainerBounds()
			{
				int width = this.control.ClientRectangle.Width - (this.control.vScrollBar.Visible ? this.control.vScrollBar.Width : 0);
				int height = this.control.ClientRectangle.Height - (this.control.bottomPanel.Visible ? this.control.bottomPanel.Height : 0);

				if (width < 0) width = 0;
				if (height < 0) height = 0;

				return new Rectangle(0, 0, width, height);
			}

			public IRenderer Renderer { get { return this.control.renderer; } }

			public void Invalidate()
			{
				this.control.Invalidate();
			}

			public void Focus()
			{
				if (!this.control.Focused) this.control.Focus();
			}

			public void ChangeBackgroundColor(SolidColor color)
			{
				this.control.BackColor = color;
			}

			public bool IsVisible { get { return this.control.Visible; } }
			#endregion Control Owner

			#region Timer
			private System.Threading.Timer dispatchTimer = null;

			public void StartTimer()
			{
				if (dispatchTimer == null)
				{
					dispatchTimer = new System.Threading.Timer(TimerRun);
				}

				this.dispatchTimer.Change(100, 150);
			}

			public void StopTimer()
			{
				this.dispatchTimer.Change(0, 0);
			}

			/// <summary>
			/// Threading to update frames of focus highlighted range
			/// </summary>
			/// <param name="state"></param>
			public void TimerRun(object state)
			{
				if (this.control.IsDisposed)
				{
					StopTimer();
				}
				else
				{
					this.control.currentWorksheet.TimerRun();
				}
			}

			#endregion

			#region IEditableControlInterface Members

			public int GetEditControlCaretPos()
			{
				return this.editTextbox.SelectionStart;
			}

			public int GetEditControlCaretLine()
			{
				return this.editTextbox.GetLineFromCharIndex(this.editTextbox.SelectionStart);
			}

			#endregion

			#region IControlAdapter Members

			public Graphics.Point PointToScreen(Graphics.Point p)
			{
				return this.control.PointToScreen(Point.Round(p));
			}

			public void ShowTooltip(Graphics.Point point, string content)
			{
			}

			#endregion // IControlAdapter Members
		}

		#endregion // Adapter

		#region Mouse

		/// <summary>
		/// Overrides mouse-down events
		/// </summary>
		/// <param name="e">Argument of mouse pressing event.</param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			this.Focus();

			this.OnWorksheetMouseDown(e.Location, (unvell.ReoGrid.Interaction.MouseButtons)e.Button);

			this.Capture = true;
		}

		/// <summary>
		/// Overrides mouse-move events
		/// </summary>
		/// <param name="e">Argument of mouse moving event.</param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			this.OnWorksheetMouseMove(e.Location, (unvell.ReoGrid.Interaction.MouseButtons)e.Button);
		}

		/// <summary>
		/// Overrides mouse-up events
		/// </summary>
		/// <param name="e">Argument of mouse release event.</param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			this.OnWorksheetMouseUp(e.Location, (unvell.ReoGrid.Interaction.MouseButtons)e.Button);

			this.Capture = false;
		}

		/// <summary>
		/// Overrides mouse-wheel events.
		/// </summary>
		/// <param name="e">Argument of mouse wheel event.</param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			this.currentWorksheet.OnMouseWheel(e.Location, e.Delta, (unvell.ReoGrid.Interaction.MouseButtons)e.Button);
		}

		/// <summary>
		/// Overrides mouse-double-click events.
		/// </summary>
		/// <param name="e">Argument of mouse double click event.</param>
		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			this.currentWorksheet.OnMouseDoubleClick(e.Location, (unvell.ReoGrid.Interaction.MouseButtons)e.Button);
		}

		//protected override void OnDragOver(DragEventArgs drgevent)
		//{
		//	//TODO
		//	MessageBox.Show("drag");
		//}

		#endregion // Mouse

		#region Keyboard

		/// <summary>
		/// Overrides is-input-key process
		/// </summary>
		/// <param name="keyData">Windows virtual key code</param>
		/// <returns>true to override</returns>
		protected override bool IsInputKey(Keys keyData)
		{
			// TODO: if (editTextbox.Visible) ...
			if (this.currentWorksheet.currentEditingCell != null) return false;

			return true;

			//switch (keyData)
			//{
			//	case Keys.Up:
			//	case Keys.Down:
			//	case Keys.Left:
			//	case Keys.Right:

			//	case Keys.Up | Keys.Shift:
			//	case Keys.Down | Keys.Shift:
			//	case Keys.Left | Keys.Shift:
			//	case Keys.Right | Keys.Shift:

			//	case Keys.Control | Keys.C:
			//	case Keys.Control | Keys.X:
			//	case Keys.Control | Keys.V:

			//	case Keys.Control | Keys.Z:
			//	case Keys.Control | Keys.Y:

			//	case Keys.Control | Keys.Oemplus:
			//	case Keys.Control | Keys.OemMinus:
			//	case Keys.Control | Keys.D0:

			//	case Keys.Control | Keys.A:

			//		return true;
			//}

			//return base.IsInputKey(keyData);
		}

		/// <summary>
		/// Overrides key-down event
		/// </summary>
		/// <param name="e">Argument of key-down event</param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (!this.currentWorksheet.OnKeyDown((KeyCode)e.KeyData))
			{
				base.OnKeyDown(e);
			}
		}

		/// <summary>
		/// Overrides key-up event
		/// </summary>
		/// <param name="e">Argument of key-up event</param>
		protected override void OnKeyUp(KeyEventArgs e)
		{
			this.currentWorksheet.OnKeyUp((KeyCode)e.KeyData);
		}

		#endregion // Keyboard

		#region Scroll

		//private Panel rightPanel;
		//private Panel horizontalScrollbarPanel;
		private HScrollBar hScrollBar = null;
		private VScrollBar vScrollBar = null;

		private class ScrollBarCorner : Control
		{
			public ScrollBarCorner()
			{
				Size = new Size(20, 20);
			}

			protected override void OnMouseMove(MouseEventArgs e)
			{
				base.OnMouseMove(e);

				Cursor = Cursors.Default;
			}
		}

		private void OnHorScroll(object sender, ScrollEventArgs e)
		{
			if (this.currentWorksheet.ViewportController is IScrollableViewportController svc)
			{
				svc.HorizontalScroll(e.NewValue);
			}
		}
		private void OnVerScroll(object sender, ScrollEventArgs e)
		{
			if (this.currentWorksheet.ViewportController is IScrollableViewportController svc)
			{
				svc.VerticalScroll(e.NewValue);
			}
		}

		#endregion // Scroll

		#region Sheet Tab

		private WinForm.SheetTabControl sheetTab;
		private Panel bottomPanel;
		private ContextMenuStrip sheetListMenu;
		private ContextMenuStrip sheetContextMenu;

		/// <summary>
		/// Get or set menu strip of sheet tab control.
		/// </summary>
		public ContextMenuStrip SheetTabContextMenuStrip { get; set; }

		void sheetMenuItem_Click(object sender, EventArgs e)
		{
			var sheet = ((ToolStripMenuItem)sender).Tag as Worksheet;

			if (sheet != null)
			{
				this.CurrentWorksheet = sheet;
			}
		}

		private void ShowSheetTabControl()
		{
			if (!this.bottomPanel.Visible)
			{
				this.bottomPanel.Visible = true;
			}

			this.sheetTab.Visible = true;
		}

		private void HideSheetTabControl()
		{
			this.sheetTab.Visible = false;

			if (!this.hScrollBar.Visible)
			{
				this.bottomPanel.Visible = false;
			}
		}

		private void ShowHorScrollBar()
		{
			if (!this.hScrollBar.Visible)
			{
				if (this.sheetTab.Visible)
				{
					this.sheetTab.Dock = DockStyle.Left;
				}

				if (!this.bottomPanel.Visible)
				{
					this.bottomPanel.Visible = true;
				}

				this.hScrollBar.Visible = true;
			}
		}

		private void HideHorScrollBar()
		{
			if (this.hScrollBar.Visible)
			{
				this.hScrollBar.Visible = false;

				if (this.sheetTab.Visible)
				{
					this.sheetTab.Dock = DockStyle.Fill;
				}
				else
				{
					this.bottomPanel.Visible = false;
				}
			}
		}

		private void ShowVerScrollBar()
		{
			if (!this.vScrollBar.Visible)
			{
				this.vScrollBar.Visible = true;
			}
		}

		private void HideVerScrollBar()
		{
			this.vScrollBar.Visible = false;
		}

		#endregion // Sheet Tab

		#region Context Menu

		private ContextMenuStrip columnHeaderContextMenuStrip;

		/// <summary>
		/// Context menu strip displayed when user click on header of column
		/// </summary>
		public ContextMenuStrip ColumnHeaderContextMenuStrip
		{
			get { return columnHeaderContextMenuStrip; }
			set { columnHeaderContextMenuStrip = value; }
		}

		private ContextMenuStrip rowHeaderContextMenuStrip;

		/// <summary>
		/// Context menu strip displayed when user click on header of row
		/// </summary>
		public ContextMenuStrip RowHeaderContextMenuStrip
		{
			get { return rowHeaderContextMenuStrip; }
			set { rowHeaderContextMenuStrip = value; }
		}

		private ContextMenuStrip leadHeaderContextMenuStrip;

		/// <summary>
		/// Context menu strip displayed when user click on header of row
		/// </summary>
		public ContextMenuStrip LeadHeaderContextMenuStrip
		{
			get { return leadHeaderContextMenuStrip; }
			set { leadHeaderContextMenuStrip = value; }
		}

		#endregion // Cursor & Context Menu

		#region Edit Control

		#region InputTextBox
		private class InputTextBox : TextBox
		{
			private ReoGridControl owner;
			internal bool TextWrap { get; set; }
			internal Size InitialSize { get; set; }
			internal ReoGridVerAlign VAlign { get; set; }
			private System.Drawing.Graphics graphics;
			private StringFormat sf;

			internal InputTextBox(ReoGridControl owner)
				: base()
			{
				this.owner = owner;

				SetStyle(ControlStyles.SupportsTransparentBackColor, true);
				BackColor = Color.Transparent;

				this.graphics = System.Drawing.Graphics.FromHwnd(this.Handle);
			}
			protected override void OnCreateControl()
			{
				sf = new StringFormat(StringFormat.GenericDefault);
			}
			protected override void OnKeyDown(KeyEventArgs e)
			{
				var sheet = owner.currentWorksheet;

				if (sheet.currentEditingCell != null && Visible)
				{
					bool isProcessed = false;

					// in single line text
					if (!TextWrap && Text.IndexOf('\n') == -1)
					{
						isProcessed = true;
						if (e.KeyCode == Keys.Up)
						{
							ProcessSelectionMoveKey(e, sheet, () => sheet.MoveSelectionUp());
						}
						else if (e.KeyCode == Keys.Down)
						{
							ProcessSelectionMoveKey(e, sheet, () => sheet.MoveSelectionDown());
						}
						else if (e.KeyCode == Keys.Left && SelectionStart == 0)
						{
							ProcessSelectionMoveKey(e, sheet, () => sheet.MoveSelectionLeft());
						}
						else if (e.KeyCode == Keys.Right && SelectionStart == Text.Length)
						{
							ProcessSelectionMoveKey(e, sheet, () => sheet.MoveSelectionRight());
						}
						else
						{
							isProcessed = false;
						}
					}

					if (!isProcessed)
					{
						if (!Toolkit.IsKeyDown(Win32.VKey.VK_CONTROL) && e.KeyCode == Keys.Enter)
						{
							ProcessSelectionMoveKey(e, sheet, () => sheet.MoveSelectionForward());
						}
					}
				}
			}

			private void ProcessSelectionMoveKey(KeyEventArgs e, Worksheet sheet, Action moveAction)
			{
				e.SuppressKeyPress = true;
				sheet.EndEdit(Text);
				moveAction();
			}

			protected override bool ProcessCmdKey(ref Message msg, System.Windows.Forms.Keys keyData)
			{
				var sheet = owner.currentWorksheet;

				if (keyData == System.Windows.Forms.Keys.Escape)
				{
					sheet.EndEdit(EndEditReason.Cancel);
					sheet.DropKeyUpAfterEndEdit = true;

					return true;
				}
				else if (keyData == System.Windows.Forms.Keys.Tab
					|| keyData == (System.Windows.Forms.Keys.Tab | System.Windows.Forms.Keys.Shift))
				{
					sheet.EndEdit(EndEditReason.NormalFinish);
					sheet.OnKeyDown((KeyCode)keyData);

					return true;
				}
				else
				{
					return base.ProcessCmdKey(ref msg, keyData);
				}
			}

			protected override void OnTextChanged(EventArgs e)
			{
				base.OnTextChanged(e);

				this.Text = this.owner.currentWorksheet.RaiseCellEditTextChanging(this.Text);

				CheckAndUpdateWidth();
			}

			protected override void OnVisibleChanged(EventArgs e)
			{
				base.OnVisibleChanged(e);

				if (Visible)
				{
					CheckAndUpdateWidth();
				}
			}

			private void CheckAndUpdateWidth()
			{
				if (sf == null) sf = new StringFormat(StringFormat.GenericTypographic);

				int fieldWidth = 0;

				if (TextWrap)
				{
					fieldWidth = InitialSize.Width;
				}
				else
				{
					fieldWidth = 9999999; // todo: avoid unsafe magic number
				}

				if (TextWrap)
				{
					sf.FormatFlags &= ~StringFormatFlags.NoWrap;
				}
				else
				{
					sf.FormatFlags |= StringFormatFlags.NoWrap;
				}

				Size size = Size.Round(graphics.MeasureString(Text, Font, fieldWidth, sf));

				if (TextWrap)
				{
					this.SuspendLayout();

					if (Height < size.Height)
					{
						int offset = size.Height - Height + 1;

						Height += offset;

						if (Height < Font.Height)
						{
							offset = Font.Height - Height;
						}

						Height += offset;

						switch (VAlign)
						{
							case ReoGridVerAlign.Top:
								break;
							default:
							case ReoGridVerAlign.Middle:
								Top -= offset / 2;
								break;
							case ReoGridVerAlign.Bottom:
								Top -= offset;
								break;
						}
					}

					this.ResumeLayout();
				}
				else
				{
					this.SuspendLayout();

					if (Width < size.Width + 5)
					{
						int widthOffset = size.Width + 5 - Width;

						switch (TextAlign)
						{
							default:
							case HorizontalAlignment.Left:
								break;
							case HorizontalAlignment.Right:
								Left -= widthOffset;
								break;
						}

						Width += widthOffset;
					}

					if (Height < size.Height + 1)
					{
						int offset = size.Height - 1 - Height;
						Top -= offset / 2 + 0;
						Height = size.Height + 1;
					}

					this.ResumeLayout();
				}
			}

			#region IME
			protected override void WndProc(ref Message m)
			{
				if (m.Msg == (int)Win32.WMessages.WM_CHAR)
				{
					int inputChar = m.WParam.ToInt32();

					if (inputChar != 8      // backspace
						&& inputChar != 13     // enter
						&& inputChar != 27     // escape
																	 //&& inputChar != '\t'   // tab
						)
					{
						inputChar = this.owner.currentWorksheet.RaiseCellEditCharInputed(m.WParam.ToInt32());
					}

					m.WParam = new IntPtr(inputChar);
					base.WndProc(ref m);

					return;
				}
				else if (m.Msg == (int)Win32.WMessages.WM_IME_STARTCOMPOSITION)
				{
					CheckAndUpdateWidth();
				}
				else if (m.Msg == (int)Win32.WMessages.WM_IME_COMPOSITION)
				{
					IntPtr ime = Win32.ImmGetContext(this.Handle);
					int strLen = Win32.ImmGetCompositionString(ime, (int)Win32.GCS.GCS_COMPREADATTR, null, 0);
					StringBuilder sb = new StringBuilder(strLen);
					Win32.ImmGetCompositionString(ime, (int)Win32.GCS.GCS_COMPREADATTR, sb, strLen);

					Win32.COMPOSITIONFORM com = new Win32.COMPOSITIONFORM();
					Win32.ImmGetCompositionWindow(ime, ref com);

					com.rcArea = new System.Drawing.Rectangle(0, 0, 300, Height);
					bool b = Win32.ImmSetCompositionWindow(ime, ref com);

					Win32.ImmReleaseContext(this.Handle, ime);
				}

				base.WndProc(ref m);
			}
			#endregion

			protected override void Dispose(bool disposing)
			{
				if (graphics != null) graphics.Dispose();
				if (sf != null) sf.Dispose();

				base.Dispose(disposing);
			}
		}
		#endregion // InputTextBox

		/// <summary>
		/// Handle the dialog char input event.
		/// </summary>
		/// <param name="charCode">Character code inputted by user.</param>
		/// <returns>True if event has been handled; Otherwise return false.</returns>
		protected override bool ProcessDialogChar(char charCode)
		{
			if (Toolkit.IsKeyDown(Win32.VKey.VK_MENU)
				|| Toolkit.IsKeyDown(Win32.VKey.VK_CONTROL)
				|| charCode == 8      // backspace
				|| charCode == 13     // enter
				|| charCode == 27     // escape
				|| charCode == '\t'   // tab
				)
			{
				return false;
			}

			int @char = this.currentWorksheet.RaiseCellEditCharInputed(charCode);
			if (@char <= 0) return false;

			// start edit on focus cell position
			return this.currentWorksheet.StartEdit(((char)(int)@char).ToString());
		}

		private InputTextBox editTextbox;

		#endregion // Edit Control

		#region WndProc

		/// <summary>
		/// Overrides system message process method
		/// </summary>
		/// <param name="m">Windows message</param>
		protected override void WndProc(ref Message m)
		{
			// ignore the control default context-menu
			if (m.Msg == (int)Win32.WMessages.WM_CONTEXTMENU)
			{
				return;
			}
			else
				// Chinese and Japanese IME will send this message
				// before start to accept user's input
				if (m.Msg == (int)Win32.WMessages.WM_IME_STARTCOMPOSITION)
			{
				this.currentWorksheet.StartEdit(string.Empty);
			}
			else if (m.Msg == (int)Win32.WMessages.WM_MOUSEHWHEEL)
			{
				if (this.currentWorksheet.ViewportController is IScrollableViewportController)
				{
					var svc = this.currentWorksheet.ViewportController as IScrollableViewportController;

					// get an overflow error by my logitech mouse t620
					// fixed by (int)(long) cast
					int delta = (short)((long)m.WParam >> 16) / 10; // get delta
					svc.ScrollViews(ScrollDirection.Horizontal, -delta, 0);
				}
			}

			base.WndProc(ref m);
		}

		#endregion // WndProc

		#region View

		/// <summary>
		/// Overrides on-resize process method
		/// </summary>
		/// <param name="e">Argument of on-resize event</param>
		protected override void OnResize(EventArgs e)
		{
			this.currentWorksheet?.UpdateViewportControllBounds();

			base.OnResize(e);

			if (this.sheetTab.Right > this.bottomPanel.Width - 40)
			{
				this.sheetTab.Width = this.bottomPanel.Width - 40 - this.sheetTab.Left;
			}

			if (this.sheetTab.Width < 60) this.sheetTab.Width = 60;
		}

		/// <summary>
		/// Overrides visible-changed process method
		/// </summary>
		/// <param name="e">Argument of visible-changed event</param>
		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);

			if (this.Visible)
			{
				this.currentWorksheet.UpdateViewportControllBounds();
			}
		}

		/// <summary>
		/// Overrides repaint process method
		/// </summary>
		/// <param name="e">Argument of visible-changed event</param>
		protected override void OnPaint(PaintEventArgs e)
		{
			var sheet = this.currentWorksheet;

			if (sheet != null && sheet.ViewportController != null)
			{
#if DEBUG
				Stopwatch watch = Stopwatch.StartNew();
#endif // DEBUG

				this.renderer.Reset();
				this.renderer.PlatformGraphics = e.Graphics;

				CellDrawingContext dc = new CellDrawingContext(this.currentWorksheet, DrawMode.View, renderer);

				sheet.ViewportController.Draw(dc);

#if DEBUG
				watch.Stop();
				long ms = watch.ElapsedMilliseconds;
				if (ms > 30)
				{
					Debug.WriteLine(string.Format("draw workbook: {0} ms. clip: {1}", watch.ElapsedMilliseconds, e.ClipRectangle));
				}
#endif // DEBUG
			}

		}

		#endregion // View
	}
}

#endif // WINFORM
