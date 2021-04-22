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
using System.Reflection;
using System.Linq;

#if WINFORM
using System.Windows.Forms;
using RGFloat = System.Single;
using RGImage = System.Drawing.Image;
#else
using RGFloat = System.Double;
using RGImage = System.Windows.Media.ImageSource;
#endif // WINFORM

using unvell.ReoGrid.Events;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Interaction;

namespace unvell.ReoGrid.CellTypes
{
#if WINFORM

	/// <summary>
	/// Represents an abstract base class for custom drop-down cell.
	/// </summary>
	public abstract class DropdownCell : CellBody
	{
		private DropdownWindow dropdownPanel;

		/// <summary>
		/// Get dropdown panel.
		/// </summary>
		protected DropdownWindow DropdownPanel { get { return this.dropdownPanel; } }

		private bool pullDownOnClick = true;

		/// <summary>
		/// Determines whether or not to open the drop-down panel when user clicked inside cell.
		/// </summary>
		public virtual bool PullDownOnClick
		{
			get { return pullDownOnClick; }
			set { this.pullDownOnClick = value; }
		}

		private Size dropdownButtonSize = new Size(20, 20);

		/// <summary>
		/// Get or set the drop-down button size.
		/// </summary>
		public virtual Size DropdownButtonSize
		{
			get { return this.dropdownButtonSize; }
			set { this.dropdownButtonSize = value; }
		}

		private bool dropdownButtonAutoHeight = true;

		/// <summary>
		/// Determines whether or not to adjust the height of drop-down button to fit entire cell.
		/// </summary>
		public virtual bool DropdownButtonAutoHeight
		{
			get { return this.dropdownButtonAutoHeight; }
			set { this.dropdownButtonAutoHeight = value; OnBoundsChanged(); }
		}

		private Rectangle dropdownButtonRect = new Rectangle(0, 0, 20, 20);

		/// <summary>
		/// Get the drop-down button bounds.
		/// </summary>
		protected Rectangle DropdownButtonRect { get { return this.dropdownButtonRect; } }

		private System.Windows.Forms.Control dropdownControl;

		/// <summary>
		/// Get or set the control in drop-down panel.
		/// </summary>
		public virtual System.Windows.Forms.Control DropdownControl
		{
			get
			{
				return this.dropdownControl;
			}
			set
			{
				this.dropdownControl = value;
			}
		}

		/// <summary>
		/// Override method to handle the event when drop-down control lost focus.
		/// </summary>
		protected virtual void OnDropdownControlLostFocus()
		{
			this.PullUp();
		}

		private bool isDropdown;

		/// <summary>
		/// Get or set whether the drop-down button is pressed. When this value is set to true, the drop-down panel will popped up.
		/// </summary>
		public bool IsDropdown
		{
			get
			{
				return this.isDropdown;
			}
			set
			{
				if (this.isDropdown != value)
				{
					if (value)
					{
						PushDown();
					}
					else
					{
						PullUp();
					}
				}
			}
		}

		/// <summary>
		/// Create custom drop-down cell instance.
		/// </summary>
		public DropdownCell()
		{
		}

		/// <summary>
		/// Process boundary changes.
		/// </summary>
		public override void OnBoundsChanged()
		{
			this.dropdownButtonRect.Width = this.dropdownButtonSize.Width;

			if (this.dropdownButtonRect.Width > Bounds.Width)
			{
				this.dropdownButtonRect.Width = Bounds.Width;
			}
			else if (this.dropdownButtonRect.Width < 3)
			{
				this.dropdownButtonRect.Width = 3;
			}

			if (this.dropdownButtonAutoHeight)
			{
				this.dropdownButtonRect.Height = Bounds.Height - 1;
			}
			else
			{
				this.dropdownButtonRect.Height = Math.Min(DropdownButtonSize.Height, Bounds.Height - 1);
			}

			this.dropdownButtonRect.X = Bounds.Right - this.dropdownButtonRect.Width;

			ReoGridVerAlign valign = ReoGridVerAlign.General;

			if (this.Cell != null && this.Cell.InnerStyle != null
				&& this.Cell.InnerStyle.HasStyle(PlainStyleFlag.VerticalAlign))
			{
				valign = this.Cell.InnerStyle.VAlign;
			}

			switch (valign)
			{
				case ReoGridVerAlign.Top:
					this.dropdownButtonRect.Y = 1;
					break;

				case ReoGridVerAlign.General:
				case ReoGridVerAlign.Bottom:
					this.dropdownButtonRect.Y = Bounds.Bottom - this.dropdownButtonRect.Height;
					break;

				case ReoGridVerAlign.Middle:
					this.dropdownButtonRect.Y = Bounds.Top + (Bounds.Height - this.dropdownButtonRect.Height) / 2 + 1;
					break;
			}
		}

		/// <summary>
		/// Paint the dropdown button inside cell.
		/// </summary>
		/// <param name="dc">Platform no-associated drawing context instance.</param>
		public override void OnPaint(CellDrawingContext dc)
		{
			// call base to draw cell background and text
			base.OnPaint(dc);

			// draw button surface
			this.OnPaintDropdownButton(dc, this.dropdownButtonRect);
		}

		/// <summary>
		/// Draw the drop-down button surface.
		/// </summary>
		/// <param name="dc">ReoGrid cross-platform drawing context.</param>
		/// <param name="buttonRect">Rectangle of drop-down button.</param>
		protected virtual void OnPaintDropdownButton(CellDrawingContext dc, Rectangle buttonRect)
		{
			if (this.Cell != null)
			{
				if (this.Cell.IsReadOnly)
				{
					ControlPaint.DrawComboButton(dc.Graphics.PlatformGraphics, (System.Drawing.Rectangle)(buttonRect),
						System.Windows.Forms.ButtonState.Inactive);
				}
				else
				{
					ControlPaint.DrawComboButton(dc.Graphics.PlatformGraphics, (System.Drawing.Rectangle)(buttonRect),
						this.isDropdown ? System.Windows.Forms.ButtonState.Pushed : System.Windows.Forms.ButtonState.Normal);
				}
			}
		}

		/// <summary>
		/// Process when mouse button pressed inside cell.
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public override bool OnMouseDown(CellMouseEventArgs e)
		{
			if (PullDownOnClick || dropdownButtonRect.Contains(e.RelativePosition))
			{
				if (this.isDropdown)
				{
					PullUp();
				}
				else
				{
					PushDown();
				}

				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Handle event when mouse moving inside this cell body.
		/// </summary>
		/// <param name="e">Argument of mouse moving event.</param>
		/// <returns>True if event has been handled; Otherwise return false.</returns>
		public override bool OnMouseMove(CellMouseEventArgs e)
		{
			if (dropdownButtonRect.Contains(e.RelativePosition))
			{
				e.CursorStyle = CursorStyle.Hand;
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Handle event if cell has lost focus.
		/// </summary>
		public override void OnLostFocus()
		{
			PullUp();
		}

		/// <summary>
		/// Event rasied when dropdown-panel is opened.
		/// </summary>
		public event EventHandler DropdownOpened;

		/// <summary>
		/// Event raised when dropdown-panel is closed.
		/// </summary>
		public event EventHandler DropdownClosed;

		/// <summary>
		/// Open dropdown panel when cell enter edit mode.
		/// </summary>
		/// <returns>True if edit operation is allowed; otherwise return false to abort edit.</returns>
		public override bool OnStartEdit()
		{
			PushDown();
			return false;
		}

		private Worksheet sheet;

		/// <summary>
		/// Push down to open the dropdown panel.
		/// </summary>
		public virtual void PushDown()
		{
			if (this.Cell == null && this.Cell.Worksheet == null) return;

			if (this.Cell.IsReadOnly && this.DisableWhenCellReadonly)
			{
				return;
			}

			sheet = base.Cell == null ? null : (base.Cell.Worksheet);

			if (sheet != null && this.DropdownControl != null
				&& Views.CellsViewport.TryGetCellPositionToControl(sheet.ViewportController.FocusView, this.Cell.InternalPos, out var p))
			{
				if (this.dropdownPanel == null)
				{
					this.dropdownPanel = new DropdownWindow(this);
					//dropdown.VisibleChanged += dropdown_VisibleChanged;

					//this.dropdownPanel.LostFocus -= DropdownControl_LostFocus;
					//this.dropdownPanel.OwnerItem = this.dropdownControl;
					this.dropdownPanel.VisibleChanged += DropdownPanel_VisibleChanged;
				}

				this.dropdownPanel.Width = Math.Max((int)Math.Round(Bounds.Width * sheet.renderScaleFactor), MinimumDropdownWidth);
				this.dropdownPanel.Height = DropdownPanelHeight;

				this.dropdownPanel.Show(sheet.workbook.ControlInstance,
					new System.Drawing.Point((int)Math.Round(p.X), (int)Math.Round(p.Y + Bounds.Height * sheet.renderScaleFactor)));

				this.DropdownControl.Focus();

				this.isDropdown = true;
			}

			DropdownOpened?.Invoke(this, null);
		}

		private void DropdownPanel_VisibleChanged(object sender, EventArgs e)
		{
			OnDropdownControlLostFocus();
		}

		private int dropdownHeight = 200;

		/// <summary>
		/// Get or set height of dropdown-panel
		/// </summary>
		public virtual int DropdownPanelHeight
		{
			get { return this.dropdownHeight; }
			set { this.dropdownHeight = value; }
		}

		private int minimumDropdownWidth = 120;

		/// <summary>
		/// Minimum width of dropdown panel
		/// </summary>
		public virtual int MinimumDropdownWidth
		{
			get { return minimumDropdownWidth; }
			set { this.minimumDropdownWidth = value; }
		}

		/// <summary>
		/// Close condidate list
		/// </summary>
		public virtual void PullUp()
		{
			if (this.dropdownPanel != null)
			{
				this.dropdownPanel.Hide();

				this.isDropdown = false;

				if (this.sheet != null)
				{
					this.sheet.RequestInvalidate();
				}
			}

			if (DropdownClosed != null)
			{
				DropdownClosed(this, null);
			}
		}

		#region Dropdown Window

		/// <summary>
		/// Prepresents dropdown window for dropdown cells.
		/// </summary>
#if WINFORM
		protected class DropdownWindow : ToolStripDropDown
		{
			private DropdownCell owner;
			private ToolStripControlHost controlHost;

			/// <summary>
			/// Create dropdown window instance.
			/// </summary>
			/// <param name="owner">The owner cell to this dropdown window.</param>
			public DropdownWindow(DropdownCell owner)
				: base()
			{
				this.owner = owner;
				AutoSize = false;
				TabStop = true;

				Items.Add(controlHost = new ToolStripControlHost(this.owner.DropdownControl));

				controlHost.Margin = controlHost.Padding = new Padding(0);
				controlHost.AutoSize = false;
			}

			/// <summary>
			/// Handle event when visible property changed.
			/// </summary>
			/// <param name="e">Arguments of visible changed event.</param>
			protected override void OnVisibleChanged(EventArgs e)
			{
				base.OnVisibleChanged(e);

				if (!Visible)
				{
					this.owner.sheet.EndEdit(EndEditReason.Cancel);
				}
				else
				{
					if (owner.DropdownControl != null)
					{
						BackColor = owner.DropdownControl.BackColor;
					}
				}
			}

			/// <summary>
			/// Handle event when size property changed.
			/// </summary>
			/// <param name="e">Arguments of size changed event.</param>
			protected override void OnResize(EventArgs e)
			{
				base.OnResize(e);

				if (controlHost != null)
				{
					controlHost.Size = new System.Drawing.Size(ClientRectangle.Width - 2, ClientRectangle.Height - 2);
				}
			}
		}
#elif WPF
		protected class DropdownWindow : System.Windows.Controls.Primitives.Popup
		{
			private DropdownCell owner;

			public DropdownWindow(DropdownCell owner)
			{
				this.owner = owner;
			}

			public void Hide()
			{
				this.IsOpen = false;
			}
		}
#endif // WPF

		#endregion // Dropdown Window

	}

#endif // WINFORM
}
