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
 * Author: Jingwood <jingwood at unvell.com>
 *
 * Copyright (c) 2012-2025 Jingwood <jingwood at unvell.com>
 * Copyright (c) 2012-2025 UNVELL Inc. All rights reserved.
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
using Size = unvell.ReoGrid.Graphics.Size;
using System.Windows;


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

    #endregion // Dropdown Window
  }
#elif WPF
  /// <summary>
  /// Abstract base class for WPF dropdown cells
  /// </summary>
  public abstract class DropdownCell : CellBody
  {
    private DropdownWindow dropdownPanel;

    /// <summary>
    /// Get dropdown panel
    /// </summary>
    protected DropdownWindow DropdownPanel => dropdownPanel;

    private bool pullDownOnClick = true;
    public virtual bool PullDownOnClick
    {
      get => pullDownOnClick;
      set => pullDownOnClick = value;
    }

    private Size dropdownButtonSize = new Size(20, 20);
    public virtual Size DropdownButtonSize
    {
      get => dropdownButtonSize;
      set => dropdownButtonSize = value;
    }

    private bool dropdownButtonAutoHeight = true;
    public virtual bool DropdownButtonAutoHeight
    {
      get => dropdownButtonAutoHeight;
      set { dropdownButtonAutoHeight = value; OnBoundsChanged(); }
    }

    private Rectangle dropdownButtonRect = new Rectangle(0, 0, 20, 20);
    protected Rectangle DropdownButtonRect => dropdownButtonRect;

    private FrameworkElement dropdownControl;
    public virtual FrameworkElement DropdownControl
    {
      get => dropdownControl;
      set => dropdownControl = value;
    }

    protected virtual void OnDropdownControlLostFocus()
    {
      PullUp();
    }

    private bool isDropdown;
    public bool IsDropdown
    {
      get => isDropdown;
      set
      {
        if (isDropdown != value)
        {
          if (value) PushDown();
          else PullUp();
        }
      }
    }

    public DropdownCell() { }

    public override void OnBoundsChanged()
    {
      dropdownButtonRect.Width = dropdownButtonSize.Width;
      dropdownButtonRect.Width = Math.Max(3, Math.Min(dropdownButtonRect.Width, Bounds.Width));

      dropdownButtonRect.Height = dropdownButtonAutoHeight
          ? Bounds.Height - 1
          : Math.Min(dropdownButtonSize.Height, Bounds.Height - 1);

      dropdownButtonRect.X = Bounds.Right - dropdownButtonRect.Width;

      ReoGridVerAlign valign = ReoGridVerAlign.General;
      if (Cell?.InnerStyle?.HasStyle(PlainStyleFlag.VerticalAlign) == true)
        valign = Cell.InnerStyle.VAlign;

      switch (valign)
      {
        case ReoGridVerAlign.Top:
          dropdownButtonRect.Y = 1;
          break;
        case ReoGridVerAlign.General:
        case ReoGridVerAlign.Bottom:
          dropdownButtonRect.Y = Bounds.Bottom - dropdownButtonRect.Height;
          break;
        case ReoGridVerAlign.Middle:
          dropdownButtonRect.Y = Bounds.Top + (Bounds.Height - dropdownButtonRect.Height) / 2 + 1;
          break;
      }
    }

    public override void OnPaint(CellDrawingContext dc)
    {
      base.OnPaint(dc);
      OnPaintDropdownButton(dc, dropdownButtonRect);
    }

    protected virtual void OnPaintDropdownButton(CellDrawingContext dc, Rectangle buttonRect)
    {
      if (Cell == null) return;

      var backgroundBrush = new System.Windows.Media.SolidColorBrush(
          Cell.IsReadOnly ? System.Windows.SystemColors.ControlBrush.Color :
          (isDropdown ? System.Windows.SystemColors.ControlDarkBrush.Color : System.Windows.SystemColors.ControlBrush.Color));

      var borderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.SystemColors.ControlDarkBrush.Color);
      var arrowBrush = new System.Windows.Media.SolidColorBrush(
          Cell.IsReadOnly ? System.Windows.SystemColors.GrayTextBrush.Color : System.Windows.SystemColors.ControlTextBrush.Color);

      var dv = new System.Windows.Media.DrawingVisual();
      using (var dw = dv.RenderOpen())
      {
        dw.DrawRectangle(backgroundBrush, new System.Windows.Media.Pen(borderBrush, 1), (System.Windows.Rect)buttonRect);

        var arrowRect = new System.Windows.Rect(
            buttonRect.X + buttonRect.Width / 2 - 4,
            buttonRect.Y + buttonRect.Height / 2 - 2,
            8, 4);

        var points = new[]
        {
                new System.Windows.Point(arrowRect.Left, arrowRect.Top),
                new System.Windows.Point(arrowRect.Right, arrowRect.Top),
                new System.Windows.Point(arrowRect.Left + arrowRect.Width / 2, arrowRect.Bottom)
            };

        dw.DrawGeometry(arrowBrush, null,
            new System.Windows.Media.PathGeometry(new[]
            {
                    new System.Windows.Media.PathFigure(points[0],
                        new[]
                        {
                            new System.Windows.Media.LineSegment(points[1], true),
                            new System.Windows.Media.LineSegment(points[2], true)
                        }, true)
            }));
      }
      dc.Graphics.PlatformGraphics.DrawDrawing(dv.Drawing);
    }

    public override bool OnMouseDown(CellMouseEventArgs e)
    {
      if (PullDownOnClick || dropdownButtonRect.Contains(e.RelativePosition))
      {
        if (isDropdown) PullUp();
        else PushDown();
        return true;
      }
      return false;
    }

    public override bool OnMouseMove(CellMouseEventArgs e)
    {
      if (dropdownButtonRect.Contains(e.RelativePosition))
      {
        e.CursorStyle = CursorStyle.Hand;
        return true;
      }
      return false;
    }

    public override void OnLostFocus() => PullUp();

    public event EventHandler DropdownOpened;
    public event EventHandler DropdownClosed;

    public override bool OnStartEdit()
    {
      PushDown();
      return false;
    }

    private Worksheet sheet;

    public virtual void PushDown()
    {
      if (Cell == null || Cell.Worksheet == null) return;
      if (Cell.IsReadOnly && DisableWhenCellReadonly) return;

      sheet = Cell.Worksheet;

      if (sheet != null && DropdownControl != null &&
          Views.CellsViewport.TryGetCellPositionToControl(sheet.ViewportController.FocusView, Cell.InternalPos, out var p))
      {
        if (dropdownPanel == null)
        {
          dropdownPanel = new DropdownWindow(this);
          dropdownPanel.Closed += DropdownPanel_Closed;
        }

        dropdownPanel.Width = Math.Max(Bounds.Width, MinimumDropdownWidth);
        dropdownPanel.Height = DropdownPanelHeight;

        dropdownPanel.Show(sheet.workbook.ControlInstance, new System.Windows.Point(p.X, p.Y + Bounds.Height));
        DropdownControl.Focus();
        isDropdown = true;
      }
      DropdownOpened?.Invoke(this, null);
    }

    private void DropdownPanel_Closed(object sender, EventArgs e) => OnDropdownControlLostFocus();

    private int dropdownHeight = 200;
    public virtual int DropdownPanelHeight
    {
      get => dropdownHeight;
      set => dropdownHeight = value;
    }

    private int minimumDropdownWidth = 120;
    public virtual int MinimumDropdownWidth
    {
      get => minimumDropdownWidth;
      set => minimumDropdownWidth = value;
    }

    public virtual void PullUp()
    {
      if (dropdownPanel != null)
      {
        dropdownPanel.Hide();
        isDropdown = false;
        sheet?.RequestInvalidate();
      }
      DropdownClosed?.Invoke(this, null);
    }

    #region Dropdown Window
    /// <summary>
    /// WPF dropdown popup window
    /// </summary>
    protected class DropdownWindow : System.Windows.Controls.Primitives.Popup
    {
      private DropdownCell owner;
      private System.Windows.Controls.Border border;
      private System.Windows.UIElement controlHost;

      public DropdownWindow(DropdownCell owner)
      {
        this.owner = owner;
        border = new System.Windows.Controls.Border
        {
          BorderBrush = System.Windows.Media.Brushes.Gray,
          BorderThickness = new System.Windows.Thickness(1),
          Background = System.Windows.Media.Brushes.White
        };

        controlHost = owner.DropdownControl as System.Windows.UIElement;
        if (controlHost != null)
          border.Child = controlHost;

        Child = border;
        Opened += DropdownWindow_Opened;
      }

      private void DropdownWindow_Opened(object sender, EventArgs e)
      {
        if (owner.DropdownControl != null)
          System.Windows.Input.Keyboard.Focus(controlHost);
      }

      public new void Show(System.Windows.UIElement parent, System.Windows.Point point)
      {
        Placement = System.Windows.Controls.Primitives.PlacementMode.Absolute;
        HorizontalOffset = point.X;
        VerticalOffset = point.Y;
        IsOpen = true;
      }

      public void Hide()
      {
        IsOpen = false;
        owner.sheet?.EndEdit(EndEditReason.Cancel);
      }

      protected override void OnPreviewMouseDown(System.Windows.Input.MouseButtonEventArgs e)
      {
        base.OnPreviewMouseDown(e);
        e.Handled = true;
      }

      public bool DisableWhenCellReadonly { get; set; } = true;
    }
    #endregion
  }
#endif // WPF
}