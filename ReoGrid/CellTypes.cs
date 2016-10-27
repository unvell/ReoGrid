/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * http://reogrid.net/
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * Author: Jing <lujing at unvell.com>
 *
 * Copyright (c) 2012-2016 Jing <lujing at unvell.com>
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

using unvell.Common;
using unvell.ReoGrid.Events;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Interaction;
using unvell.ReoGrid.Main;

namespace unvell.ReoGrid.CellTypes
{
	#region Cell Types Manager

	/// <summary>
	/// Manage the collection of available cell types 
	/// </summary>
	public static class CellTypesManager
	{
		private static Dictionary<string, Type> cellTypes;

		/// <summary>
		/// Get the available collection of cell types
		/// </summary>
		public static Dictionary<string, Type> CellTypes
		{
			get
			{
				if (cellTypes == null)
				{
					cellTypes = new Dictionary<string, Type>();

					try
					{
						var types = Assembly.GetAssembly(typeof(Worksheet)).GetTypes();

						foreach (var type in types.OrderBy(t => t.Name))
						{
							if (type != typeof(ICellBody) && type != typeof(CellBody)
								&& (type.IsSubclassOf(typeof(ICellBody))
								|| type.IsSubclassOf(typeof(CellBody)))
								&& type.IsPublic
								&& !type.IsAbstract)
							{
								cellTypes[type.Name] = type;
							}
						}
					}
					catch { }
				}

				return cellTypes;
			}
		}
	}

	#endregion // Cell Types Manager

	#region Interface
	/// <summary>
	/// Represents cell body interface.
	/// </summary>
	public interface ICellBody
	{
		/// <summary>
		/// This method invoked when cell body set into a cell.
		/// </summary>
		/// <param name="cell">The cell instance to load this body.</param>
		void OnSetup(Cell cell);

		/// <summary>
		/// Get the cell body bounds. (Relative position to owner cell)
		/// </summary>
		Rectangle Bounds { get; set; }

		/// <summary>
		/// This method invoked when body bounds is changed.
		/// </summary>
		void OnBoundsChanged();

		/// <summary>
		/// Determine whether or not to allow capture the mouse moving after mouse button pressed inside the body bounds.
		/// </summary>
		/// <returns>Return true to capture mouse after mouse down; Otherwise return false to do nothing.</returns>
		bool AutoCaptureMouse();

		/// <summary>
		/// This method will be invoked when mouse button pressed inside the body bounds.
		/// </summary>
		/// <param name="e">Mouse event argument</param>
		/// <returns>Return true if event has been handled; Otherwise return false to recall default operations.</returns>
		bool OnMouseDown(CellMouseEventArgs e);

		/// <summary>
		/// This method will be invoked when mouse has been moved inside the body bounds.
		/// </summary>
		/// <param name="e">Mouse event argument</param>
		/// <returns>Return true if event has been handled; Otherwise false to recall default operations.</returns>
		bool OnMouseMove(CellMouseEventArgs e);

		/// <summary>
		/// This method will be invoked when mouse button released inside the body bounds.
		/// </summary>
		/// <param name="e">Mouse event argument</param>
		/// <returns>Return true if event has been handled; Otherwise false to recall default operations.</returns>
		bool OnMouseUp(CellMouseEventArgs e);

		/// <summary>
		/// This method will be invoked when mouse moved enter the body bounds.
		/// </summary>
		/// <param name="e">Mouse event argument</param>
		/// <returns>Return true if event has been handled; Otherwise return false to recall default operations.</returns>
		bool OnMouseEnter(CellMouseEventArgs e);

		/// <summary>
		/// This method will be invoked when mouse moved out from the body bounds.
		/// </summary>
		/// <param name="e">Mouse event argument</param>
		/// <returns>Return true if event has been handled; Otherwise return false to recall default operations.</returns>
		bool OnMouseLeave(CellMouseEventArgs e);

		/// <summary>
		/// This method will be invoked when mouse scrolled inside the body bounds.
		/// </summary>
		/// <param name="e">Mouse event argument</param>
		/// <returns>Return true if event has been handled; Otherwise return false to recall default operations</returns>
		void OnMouseWheel(CellMouseEventArgs e);

		/// <summary>
		/// This method will be invoked when any key pressed when body being focused.
		/// </summary>
		/// <param name="e">Mouse event argument.</param>
		/// <returns>Return true if event has been handled inside the body bounds; Otherwise return false to recall default operations.</returns>
		bool OnKeyDown(KeyCode e);

		/// <summary>
		/// This method will be invoked when any key released on this body.
		/// </summary>
		/// <param name="e">Mouse event argument.</param>
		/// <returns>Return true if event has been handled; Otherwise return false to recall default operations.</returns>
		bool OnKeyUp(KeyCode e);

		/// <summary>
		/// This method will be invoked when cell body is required to repaint on worksheet.
		/// </summary>
		/// <param name="dc">Drawing context used to paint the cell body.</param>
		void OnPaint(CellDrawingContext dc);

		/// <summary>
		/// This method will be invoked when the owner cell of this body begin to edit. (Changing to editing mode)
		/// </summary>
		/// <returns>Return true to allow editing; Otherwise return false to abort editing operation.</returns>
		bool OnStartEdit();

		/// <summary>
		/// Determines whether or not become disable when owner cell is set as read-only. 
		/// </summary>
		bool DisableWhenCellReadonly { get; }

		/// <summary>
		/// This method will be invoked when the owner cell of this body finished edit.
		/// </summary>
		/// <param name="data">The data of user inputted.</param>
		/// <returns>Data used to be set into the cell. If don't want to change user data, return the data from method parameter.</returns>
		object OnEndEdit(object data);

		/// <summary>
		/// This method invoked when cell getting focus.
		/// </summary>
		void OnGotFocus();

		/// <summary>
		/// This method invoked when cell losing focus.
		/// </summary>
		void OnLostFocus();

		/// <summary>
		/// This method invoked when cell data was updated.
		/// </summary>
		/// <param name="data">The data will be set into the cell.</param>
		/// <returns>Return the new data used to set into the cell.</returns>
		object OnSetData(object data);

		/// <summary>
		/// Clone a cell body from this object.
		/// </summary>
		/// <returns>New instance of cell body.</returns>
		ICellBody Clone();
	}
	#endregion // Interface

	#region Base Class
	/// <summary>
	/// Cell body
	/// </summary>
	[Serializable]
	public class CellBody : ICellBody
	{
		private Cell cell;

		internal Cell InnerCell { set { this.cell = value; } }

		/// <summary>
		/// Owner cell contains this body.
		/// </summary>
		public Cell Cell { get { return this.cell; } }

		/// <summary>
		/// When the body set into a cell.
		/// </summary>
		/// <param name="cell">Current owner cell</param>
		public virtual void OnSetup(Cell cell) { this.cell = cell; }

		/// <summary>
		/// Get cell body bounds rectangle.
		/// </summary>
		public virtual Rectangle Bounds { get; set; }

		/// <summary>
		/// Determines whether or not become disable when owner cell is set as read-only. (Default is True)
		/// </summary>
		public virtual bool DisableWhenCellReadonly
		{
			get { return true; }
		}

		/// <summary>
		/// Invoked when body boundary has been changed.
		/// </summary>
		public virtual void OnBoundsChanged() { }

		/// <summary>
		/// Determines whether or not to allow capture the mouse when mouse down inside this body.
		/// </summary>
		/// <returns>True to allow caption; False to abort capture.</returns>
		public virtual bool AutoCaptureMouse() { return true; }

		/// <summary>
		/// This method will be invoked when mouse button pressed inside the body bounds.
		/// </summary>
		/// <param name="e">Mouse event argument.</param>
		/// <returns>Return true if event has been handled; Otherwise return false to recall default operations.</returns>
		public virtual bool OnMouseDown(CellMouseEventArgs e) { return false; }

		/// <summary>
		/// This method will be invoked when mouse has been moved inside the body bounds.
		/// </summary>
		/// <param name="e">Mouse event argument</param>
		/// <returns>Return true if event has been handled; Otherwise false to recall default operations.</returns>
		public virtual bool OnMouseMove(CellMouseEventArgs e) { return false; }

		/// <summary>
		/// This method will be invoked when any key released on this body.
		/// </summary>
		/// <param name="e">Mouse event argument.</param>
		/// <returns>Return true if event has been handled; Otherwise return false to recall default operations.</returns>
		public virtual bool OnMouseUp(CellMouseEventArgs e) { return false; }

		/// <summary>
		/// Invoked when mouse moved enter this body.
		/// </summary>
		/// <param name="e">Mouse event argument.</param>
		/// <returns>True if event has been handled inside this body, otherwise false to recall built-in operations.</returns>
		public virtual bool OnMouseEnter(CellMouseEventArgs e) { return false; }

		/// <summary>
		/// Invoked when mouse moved out from this body.
		/// </summary>
		/// <param name="e">Mouse event argument.</param>
		/// <returns>True if event has been handled inside this body, otherwise false to recall built-in operations.</returns>
		public virtual bool OnMouseLeave(CellMouseEventArgs e) { return false; }

		/// <summary>
		/// Invoked when mouse scrolled inside this body.
		/// </summary>
		/// <param name="e">Mouse event argument.</param>
		/// <returns>True if event has been handled inside this body, otherwise false to recall built-in operations.</returns>
		public virtual void OnMouseWheel(CellMouseEventArgs e) { }

		/// <summary>
		/// Invoked when any key pressed on this body.
		/// </summary>
		/// <param name="e">Mouse event argument.</param>
		/// <returns>True if event has been handled inside this body, otherwise false to recall built-in operations.</returns>
		public virtual bool OnKeyDown(KeyCode e) { return false; }

		/// <summary>
		/// Invoked when any key released on this body.
		/// </summary>
		/// <param name="e">Mouse event argument.</param>
		/// <returns>True if event has been handled inside this body, otherwise false to recall built-in operations.</returns>
		public virtual bool OnKeyUp(KeyCode e) { return false; }

		/// <summary>
		/// Paint the content of body.
		/// </summary>
		/// <param name="dc">Platform independency graphics context.</param>
		public virtual void OnPaint(CellDrawingContext dc)
		{
			dc.DrawCellBackground();
			dc.DrawCellText();
		}

		/// <summary>
		/// Invoked when cell of this body begin to edit. (Enter edit mode)
		/// </summary>
		/// <returns>True to allow edit; Otherwise false to cancel edit.</returns>
		public virtual bool OnStartEdit() { return true; }

		/// <summary>
		/// Invoked when cell of this body finish edit. 
		/// Return data to be set into spreadsheet instead of user inputted.
		/// </summary>
		/// <param name="data">user inputted data.</param>
		/// <returns>new data to be into spreadsheet.</returns>
		public virtual object OnEndEdit(object data) { return data; }

		/// <summary>
		/// Invoked when cell get focus.
		/// </summary>
		public virtual void OnGotFocus() { }

		/// <summary>
		/// Invoked when cell lost focus.
		/// </summary>
		public virtual void OnLostFocus() { }

		/// <summary>
		/// Invoked when cell data updating.
		/// </summary>
		/// <param name="data">Data to be updated.</param>
		/// <returns>New data that is used to replace the data inputted.</returns>
		public virtual object OnSetData(object data) { return data; }

		/// <summary>
		/// Clone a cell body from this object.
		/// </summary>
		/// <returns>New instance of cell body.</returns>
		public virtual ICellBody Clone()
		{
			return new CellBody();
		}
	}

	/// <summary>
	/// Represents a cell body that maintains a visual content region for child objects, such as Check-box or Radio-button.
	/// </summary>
	public abstract class ContentCellBody : CellBody
	{
		/// <summary>
		/// Get or set child content bounds rectangle.
		/// </summary>
		public virtual Rectangle ContentBounds { get; set; }

		/// <summary>
		/// Determines the preferred body size.
		/// </summary>
		protected virtual Graphics.Size GetContentSize()
		{
			return new Size(17, 17);
		}

		/// <summary>
		/// Handles when bounds changed.
		/// </summary>
		public override void OnBoundsChanged()
		{
			base.OnBoundsChanged();

			var contentRect = new Rectangle(new Point(0, 0), this.GetContentSize());

			if (this.Cell != null)
			{
				RGFloat x = 0, y = 0;

				switch (Cell.InnerStyle.HAlign)
				{
					case ReoGridHorAlign.Left:
						x = Bounds.X + 1;
						break;

					case ReoGridHorAlign.Center:
						x = Bounds.X + (Bounds.Width - contentRect.Width) / 2;
						break;

					case ReoGridHorAlign.Right:
						x = Bounds.Right - contentRect.Width - 1;
						break;
				}

				switch (Cell.InnerStyle.VAlign)
				{
					case ReoGridVerAlign.Top:
						y = Bounds.Y + 1;
						break;

					case ReoGridVerAlign.Middle:
						y = Bounds.Y + (Bounds.Height - contentRect.Height) / 2;
						break;

					case ReoGridVerAlign.Bottom:
						y = Bounds.Bottom - contentRect.Height - 1;
						break;
				}

				contentRect = new Rectangle(x, y, contentRect.Width, contentRect.Height);
			}
			else
			{
				contentRect = new Rectangle(Bounds.X + (Bounds.Width - contentRect.Width) / 2,
						Bounds.Y + (Bounds.Height - contentRect.Height) / 2, contentRect.Width, contentRect.Height);
			}

			this.ContentBounds = contentRect;
		}

		/// <summary>
		/// Paint cell body.
		/// </summary>
		/// <param name="dc">Platform independency graphics context.</param>
		public override void OnPaint(CellDrawingContext dc)
		{
			dc.DrawCellBackground();

			if (this.ContentBounds.Width > 0 || this.ContentBounds.Height > 0)
			{
				this.OnContentPaint(dc);
			}
		}

		/// <summary>
		/// Paint content of cell body.
		/// </summary>
		/// <param name="dc">Platform independency graphics context.</param>
		protected virtual void OnContentPaint(CellDrawingContext dc)
		{
		}
	}
	#endregion // Base Class

	#region Header Body
	/// <summary>
	/// Represent the interface of row and column header body
	/// </summary>
	public interface IHeaderBody
	{
		/// <summary>
		/// Onwer drawing
		/// </summary>
		/// <param name="dc">Drawing context</param>
		/// <param name="headerSize">Header size</param>
		void OnPaint(CellDrawingContext dc, Size headerSize);

		/// <summary>
		/// Mouse move event
		/// </summary>
		/// <param name="headerSize">Header size</param>
		/// <param name="e">Event argument</param>
		/// <returns>true if this event is handled</returns>
		bool OnMouseMove(Size headerSize, WorksheetMouseEventArgs e);

		/// <summary>
		/// Mouse down event
		/// </summary>
		/// <param name="headerSize">Header size</param>
		/// <param name="e">Event argument</param>
		/// <returns>true if this event is handled</returns>
		bool OnMouseDown(Size headerSize, WorksheetMouseEventArgs e);

		/// <summary>
		/// Event when data in any cells on this header is changed
		/// </summary>
		/// <param name="startRow">Zero-based number of row of changed cells</param>
		/// <param name="endRow">Zero-based number of column of changed cells</param>
		void OnDataChange(int startRow, int endRow);
	}

	/// <summary>
	/// Represent the interface of row and column header body
	/// </summary>
	public class HeaderBody : IHeaderBody
	{
		/// <summary>
		/// Paint this header body.
		/// </summary>
		/// <param name="dc">Drawing context</param>
		/// <param name="headerSize">Header size</param>
		public virtual void OnPaint(CellDrawingContext dc, Size headerSize) { }

		/// <summary>
		/// Method raised when mouse moving inside this body.
		/// </summary>
		/// <param name="headerSize">Header size</param>
		/// <param name="e">Event argument</param>
		/// <returns>true if this event is handled</returns>
		public virtual bool OnMouseMove(Size headerSize, WorksheetMouseEventArgs e) { return false; }

		/// <summary>
		/// Method raised when mouse pressed inside this body.
		/// </summary>
		/// <param name="headerSize">Header size</param>
		/// <param name="e">Event argument</param>
		/// <returns>true if this event is handled</returns>
		public virtual bool OnMouseDown(Size headerSize, WorksheetMouseEventArgs e) { return false; }

		/// <summary>
		/// Method raised when data changed from cells on this header.
		/// </summary>
		/// <param name="startRow">Zero-based number of row of changed cells.</param>
		/// <param name="endRow">Zero-based number of column of changed cells.</param>
		public virtual void OnDataChange(int startRow, int endRow) { }
	}
	#endregion

	#region Built-in Cell Types

	#region Button

	/// <summary>
	/// Representation for button of cell body.
	/// </summary>
	[Serializable]
	public class ButtonCell : CellBody
	{
		private string defaultText;

		/// <summary>
		/// Create button cell
		/// </summary>
		public ButtonCell()
		{
		}

		/// <summary>
		/// Create button cell with specified text.
		/// </summary>
		/// <param name="defaultText"></param>
		public ButtonCell(string defaultText)
		{
			this.defaultText = defaultText;
		}

		/// <summary>
		/// On body is setup to cell.
		/// </summary>
		/// <param name="cell">cell instance</param>
		public override void OnSetup(Cell cell)
		{
			// set text alignment to center
			if (cell != null)
			{
				if (cell.Worksheet != null)
				{
					cell.Worksheet.SetRangeStyles(cell.PositionAsRange,
						new WorksheetRangeStyle
						{
							Flag = PlainStyleFlag.HorizontalAlign | PlainStyleFlag.VerticalAlign,
							HAlign = ReoGridHorAlign.Center,
							VAlign = ReoGridVerAlign.Middle,
						});
				}

				// set default cell value
				if (!string.IsNullOrEmpty(this.defaultText))
				{
					cell.Data = this.defaultText;
				}
			}
		}

		#region Draw

		/// <summary>
		/// Paint this cell body.
		/// </summary>
		/// <param name="dc">ReoGrid common drawing context</param>
		public override void OnPaint(CellDrawingContext dc)
		{
			if (this.Cell != null)
			{
				DrawButton(dc);

				// get style
				//var style = this.Cell.InnerStyle;
				//var textColor = style.TextColor;
			}

			// call core text drawing method
			dc.DrawCellText();
		}

		/// <summary>
		/// Draw button surface.
		/// </summary>
		/// <param name="dc">Platform independence drawing context.</param>
		/// <param name="state">Button state.</param>
		protected virtual void DrawButton(CellDrawingContext dc)
		{
#if WINFORM
			ControlPaint.DrawButton(dc.Graphics.PlatformGraphics, (System.Drawing.Rectangle)Bounds,
				this.IsPressed ? ButtonState.Pushed :
				(this.Cell.IsReadOnly ? ButtonState.Inactive : ButtonState.Normal));
#elif WPF

			var g = dc.Graphics;

			//g.TranslateTransform(20f, 00f);

			var r = this.Bounds;
			g.DrawRectangle(r, SolidColor.Dark(StaticResources.SystemColor_ControlDark));

			//var r2 = new Rectangle(r.X, r.Y, r.Width - 1, r.Height - 1);
			var r3 = new Rectangle(r.X + 1, r.Y + 1, r.Width - 2, r.Height - 2);
			g.FillRectangle(r3, StaticResources.SystemColor_Control);

			if (this.IsPressed)
			{
				//	g.DrawRectangle(r, StaticResources.SystemColor_ControlDark);

				//	r.X++; r.Y++; r.Width--; r.Height--;
				//	g.DrawRectangle(r, SolidColor.Dark(StaticResources.SystemColor_ControlDark));
				var r2 = new Rectangle(r.X + 1, r.Y + 1, r.Width - 2, r.Height - 2);
				g.DrawRectangle(r2, SolidColor.Dark(StaticResources.SystemColor_ControlDark));
			}
			else
			{
				//var r2 = new Rectangle(r.X + 1, r.Y + 1, r.Width - 1, r.Height - 1);
				//g.DrawRectangle(r2, StaticResources.SystemColor_ControlDark);
			}


#endif // WPF
		}

		#endregion Draw

		#region Mouse
		/// <summary>
		/// Invoked when mouse down inside this body
		/// </summary>
		/// <param name="e">mouse event argument</param>
		/// <returns>true if event has been handled</returns>
		public override bool OnMouseDown(CellMouseEventArgs e)
		{
			if (this.Bounds.Contains(e.RelativePosition))
			{
				IsPressed = true;
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Determine whether the button is pressed or released
		/// </summary>
		/// <param name="e">mouse event argument</param>
		/// <returns>true to notify spreadsheet that event has been handled</returns>
		public override bool OnMouseUp(CellMouseEventArgs e)
		{
			if (IsPressed)
			{
				if (Bounds.Contains(e.RelativePosition))
				{
					this.PerformClick();
				}

				IsPressed = false;
				return true;
			}
			else
				return false;
		}
		#endregion // Mouse

		#region Keyboard
		/// <summary>
		/// Check when user pressed Space key to press button.
		/// </summary>
		/// <param name="keyCode">Virtual keys code that is converted from system platform.</param>
		/// <returns>True to notify spreadsheet that event has been handled</returns>
		public override bool OnKeyDown(KeyCode keyCode)
		{
			if (keyCode == KeyCode.Space)
			{
				this.IsPressed = true;
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Check when user released Space key to restore button.
		/// </summary>
		/// <param name="keyCode">Virtual keys code that is converted from system platform.</param>
		/// <returns>True to notify spreadsheet that event has been handled.</returns>
		public override bool OnKeyUp(KeyCode keyCode)
		{
			if (keyCode == KeyCode.Space && this.IsPressed)
			{
				this.IsPressed = false;
				this.PerformClick();
				return true;
			}
			else
			{
				return false;
			}
		}
		#endregion // Keyboard

		/// <summary>
		/// Return false to disable edit operation for this cell.
		/// </summary>
		/// <returns>False to disable edit operation for this cell.</returns>
		public override bool OnStartEdit()
		{
			return false;
		}

		/// <summary>
		/// Check whether or not button is pressed.
		/// </summary>
		public bool IsPressed { get; set; }

		/// <summary>
		/// Perform click operation.
		/// </summary>
		public virtual void PerformClick()
		{
			Click?.Invoke(this, null);
		}

		/// <summary>
		/// Click event raised when user clicked on the button.
		/// </summary>
		public event EventHandler Click;

		/// <summary>
		/// Clone a button cell from this object.
		/// </summary>
		/// <returns>New instance of button cell.</returns>
		public override ICellBody Clone()
		{
			return new ButtonCell(this.Cell.DisplayText);
		}
	}
	#endregion Button

	#region Progress
	/// <summary>
	/// Representation for a button of cell body
	/// </summary>
	[Serializable]
	public class ProgressCell : CellBody
	{
		/// <summary>
		/// Get or set the top color.
		/// </summary>
		public SolidColor TopColor { get; set; }

		/// <summary>
		/// Get or set the bottom color.
		/// </summary>
		public SolidColor BottomColor { get; set; }

		/// <summary>
		/// Create progress cell body.
		/// </summary>
		public ProgressCell()
		{
			TopColor = SolidColor.LightSkyBlue;
			BottomColor = SolidColor.SkyBlue;
		}

		/// <summary>
		/// Render the progress cell body.
		/// </summary>
		/// <param name="dc"></param>
		public override void OnPaint(CellDrawingContext dc)
		{
			double value = this.Cell.GetData<double>();

			if (value > 0)
			{
				var g = dc.Graphics.PlatformGraphics;

				Rectangle rect = new Rectangle(Bounds.Left, Bounds.Top + 1, (RGFloat)(Bounds.Width * value), Bounds.Height - 1);

				if (rect.Width > 0 && rect.Height > 0)
				{
					dc.Graphics.FillRectangleLinear(TopColor, BottomColor, 90f, rect);
				}
			}
		}

		/// <summary>
		/// Clone a progress bar from this object.
		/// </summary>
		/// <returns>New instance of progress bar.</returns>
		public override ICellBody Clone()
		{
			return new ProgressCell();
		}
	}
	#endregion // Progress

	#region Hyperlink
	/// <summary>
	/// Representation for hyperlink of cell body.
	/// </summary>
	[Serializable]
	public class HyperlinkCell : CellBody
	{
		/// <summary>
		/// Get or set color of mouse-hover status.
		/// </summary>
		public SolidColor ActivateColor { get; set; }

		/// <summary>
		/// Get or set color of normal hyperlink.
		/// </summary>
		public SolidColor LinkColor { get; set; }

		/// <summary>
		/// Get or set color of visited status.
		/// </summary>
		public SolidColor VisitedColor { get; set; }

		/// <summary>
		/// Get or set the navigation url. (Redirected when AutoNavigation property is true)
		/// </summary>
		public string LinkURL { get; set; }

		/// <summary>
		/// Create hyperlink cell body instance.
		/// </summary>
		public HyperlinkCell()
			: this(null, true)
		{
		}

		/// <summary>
		/// Create instane of hyperlink cell body with specified navigation url and AutoNavigate property.
		/// </summary>
		/// <param name="navigationURL">Navigation url redirected to when hyperlink clicked. (Default is emtpy)</param>
		public HyperlinkCell(string navigationURL)
			: this(navigationURL, true)
		{
		}

		/// <summary>
		/// Create instane of hyperlink cell body with specified navigation url and AutoNavigate property.
		/// </summary>
		/// <param name="navigationURL">Navigation url redirected to when hyperlink clicked. (Default is emtpy)</param>
		/// <param name="autoNavigate">Determine whether or not redirect to specified url 
		/// when hyperlink clicked automatically. (Default is true)</param>
		public HyperlinkCell(string navigationURL, bool autoNavigate)
		{
			this.ActivateColor = SolidColor.Red;
			this.LinkColor = SolidColor.Blue;
			this.VisitedColor = SolidColor.Purple;

			this.LinkURL = navigationURL;
			this.AutoNavigate = autoNavigate;
		}

		/// <summary>
		/// Determine whether or not the hyperlink is in pressed status.
		/// </summary>
		public bool IsPressed { get; set; }

		/// <summary>
		/// Handle event when the cell of this body entered edit mode.
		/// </summary>
		/// <returns>True to allow edit; False to disallow edit.</returns>
		public override bool OnStartEdit()
		{
			return !this.IsPressed;
		}

		/// <summary>
		/// Initialize cell body when set up into a cell.
		/// </summary>
		/// <param name="cell">Instance of cell to be set up.</param>
		public override void OnSetup(Cell cell)
		{
			// set cell text style
			cell.Style.TextColor = LinkColor;
			cell.Style.Underline = true;

			// set cell default value
			if (this.LinkURL != null)
			{
				cell.Data = this.LinkURL;
			}
			else if (cell.InnerData != null)
			{
				this.LinkURL = Convert.ToString(cell.InnerData);
			}
		}

		/// <summary>
		/// Change color of hyperlink to activate-status when mouse button pressed inside cell.
		/// </summary>
		/// <param name="e">Event argument of cell body mouse-down.</param>
		/// <returns>True if event has been handled.</returns>
		public override bool OnMouseDown(CellMouseEventArgs e)
		{
			this.IsPressed = true;

			e.Cell.Style.TextColor = ActivateColor;

			return true;
		}

		/// <summary>
		/// Restore color of hyperlink to normal-status or hover-status when mouse button was released from cell.
		/// </summary>
		/// <param name="e">Event argument of cell body mouse-up.</param>
		/// <returns>True if event has been handled.</returns>
		public override bool OnMouseUp(CellMouseEventArgs e)
		{
			if (this.IsPressed)
			{
				if (this.Bounds.Contains(e.RelativePosition))
				{
					this.PerformClick();
				}
			}

			this.IsPressed = false;

			e.Cell.Style.TextColor = VisitedColor;

			return true;
		}

		/// <summary>
		/// Change color of hyperlink to hover-status when mouse moved into the cell.
		/// </summary>
		/// <param name="e">Event argument of cell body mouse-enter.</param>
		/// <returns>True if event has been handled.</returns>
		public override bool OnMouseEnter(CellMouseEventArgs e)
		{
			e.Worksheet.controlAdapter.ChangeSelectionCursor(CursorStyle.Hand);
			return false;
		}

		/// <summary>
		/// Restore color of hyperlink from hover-status when mouse leaved from cell.
		/// </summary>
		/// <param name="e">Argument of mouse leaving event.</param>
		/// <returns>True if this event has been handled; Otherwise return false.</returns>
		public override bool OnMouseLeave(CellMouseEventArgs e)
		{
			// change current cursor to hand
			e.Worksheet.ControlAdapter.ChangeSelectionCursor(CursorStyle.PlatformDefault);
			return false;
		}

		/// <summary>
		/// Handle keyboard down event.
		/// </summary>
		/// <param name="keyCode">Virtual keys code that is converted from system platform.</param>
		/// <returns>True if event has been handled; Otherwise return false.</returns>
		public override bool OnKeyDown(KeyCode keyCode)
		{
			if (keyCode == KeyCode.Space)
			{
				this.IsPressed = true;
				this.Cell.Style.TextColor = ActivateColor;

				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Handle keyboard up event.
		/// </summary>
		/// <param name="keyCode">Virtual keys code that is converted from system platform.</param>
		/// <returns>True if event has been handled; Otherwise return false;</returns>
		public override bool OnKeyUp(KeyCode keyCode)
		{
			if (IsPressed)
			{
				this.IsPressed = false;

				this.PerformClick();

				this.Cell.Style.TextColor = VisitedColor;

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
			if (this.IsPressed)
			{
				this.IsPressed = false;
			}
		}

		/// <summary>
		/// Manually fire the hyperlink click event.
		/// </summary>
		public void PerformClick()
		{
			if (AutoNavigate && LinkURL != null)
			{
				try
				{
					System.Diagnostics.Process.Start(LinkURL);
				}
				catch { }
			}

			Click?.Invoke(this, null);
		}

		/// <summary>
		/// When data of cell set, read navigation url from cell data
		/// </summary>
		/// <param name="data">New data to be set</param>
		/// <returns>Replacement data if needed</returns>
		public override object OnSetData(object data)
		{
			LinkURL = Convert.ToString(data);
			return data;
		}

		/// <summary>
		/// Event raised when hyperlink was preseed
		/// </summary>
		public event EventHandler Click;

		/// <summary>
		/// Determine whether or not redirect to navigation url when hyperlink was pressed
		/// </summary>
		public bool AutoNavigate { get; set; }
	}
	#endregion // Hyperlink

	#region Check box
	/// <summary>
	/// Representation for check box of cell body
	/// </summary>
	[Serializable]
	public class CheckBoxCell : ContentCellBody
	{
		#region Constructor
		private bool initChecked = false;

		/// <summary>
		/// Create check box cell body.
		/// </summary>
		public CheckBoxCell()
			: this(false)
		{
		}

		/// <summary>
		/// Create check box cell body.
		/// </summary>
		/// <param name="initChecked">Set the initial status. If this value is true, checkbox keep checked status when added into a cell.</param>
		public CheckBoxCell(bool initChecked)
		{
			this.initChecked = initChecked;
		}

		/// <summary>
		/// Override OnSetup method to set initial checked status.
		/// </summary>
		/// <param name="cell">The cell this body will set into.</param>
		public override void OnSetup(Cell cell)
		{
			if (cell != null)
			{
				if (initChecked) cell.Data = true;

				cell.Style.HAlign = ReoGridHorAlign.Center;
				cell.Style.VAlign = ReoGridVerAlign.Middle;
			}
		}
		#endregion // Constructor

		#region Mouse
		/// <summary>
		/// Determines whether or not mouse or key pressed inside check box.
		/// </summary>
		protected virtual bool IsPressed { get; set; }

		/// <summary>
		/// Handle the mouse down event.
		/// </summary>
		/// <param name="e">Arguments of mouse down event.</param>
		/// <returns>True if event has been handled; Otherwise return false.</returns>
		public override bool OnMouseDown(CellMouseEventArgs e)
		{
			if (ContentBounds.Contains(e.RelativePosition))
			{
				IsPressed = true;
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Handle the mouse up event.
		/// </summary>
		/// <param name="e">Arguments of mouse up event.</param>
		/// <returns>True if event has been handled; Otherwise return false.</returns>
		public override bool OnMouseUp(CellMouseEventArgs e)
		{
			if (this.IsPressed)
			{
				this.IsPressed = false;

				if (ContentBounds.Contains(e.RelativePosition))
				{
					this.ToggleCheckStatus();

					this.RaiseClickEvent();
				}

				return true;
			}
			else
				return false;
		}
		#endregion // Mouse

		#region Event

		/// <summary>
		/// Event raied when user clicked inside check box.
		/// </summary>
		public event EventHandler Click;

		/// <summary>
		/// Event raised when check status changed.
		/// </summary>
		public event EventHandler CheckChanged;

		/// <summary>
		/// Raise the click event.
		/// </summary>
		protected virtual void RaiseClickEvent()
		{
			if (this.Click != null)
			{
				this.Click(this, null);
			}
		}

		/// <summary>
		/// Raise the click event.
		/// </summary>
		protected virtual void RaiseCheckChangedEvent()
		{
			this.CheckChanged?.Invoke(this, null);
		}

		#endregion // Event

		#region Paint

		/// <summary>
		/// Paint content of cell body.
		/// </summary>
		/// <param name="dc">Platform independency graphics context.</param>
		protected override void OnContentPaint(CellDrawingContext dc)
		{
#if WINFORM
			System.Windows.Forms.ButtonState bs = ButtonState.Normal;
			if (IsPressed) bs |= ButtonState.Pushed;
			if (IsChecked) bs |= ButtonState.Checked;

			ControlPaint.DrawCheckBox(dc.Graphics.PlatformGraphics, (System.Drawing.Rectangle)ContentBounds, bs);
#elif WPF
			var g = dc.Graphics;

			if (this.IsPressed)
			{
				g.FillRectangle(this.ContentBounds, StaticResources.SystemColor_Control);
			}

			g.DrawRectangle(this.ContentBounds, StaticResources.SystemColor_ControlDark);

			if (this.isChecked)
			{
				var x = this.ContentBounds.X;
				var y = this.ContentBounds.Y;
				var w = this.ContentBounds.Width;
				var h = this.ContentBounds.Height;

				var path = new System.Windows.Media.PathGeometry();
				var figure = new System.Windows.Media.PathFigure(new System.Windows.Point(x + w * 0.167, y + h * 0.546),
					new System.Windows.Media.LineSegment[] {
					new System.Windows.Media.LineSegment(new System.Windows.Point(x + w * 0.444, y + h * 0.712), false),
					new System.Windows.Media.LineSegment(new System.Windows.Point(x + w * 0.833, y + h * 0.157), false),
					new System.Windows.Media.LineSegment(new System.Windows.Point(x + w * 0.944, y + h * 0.323), false),
					new System.Windows.Media.LineSegment(new System.Windows.Point(x + w * 0.500, y + h * 0.934), false),
					new System.Windows.Media.LineSegment(new System.Windows.Point(x + w * 0.080, y + h * 0.712), false),
					}, true);

				path.Figures.Add(figure);

				g.FillPath(StaticResources.SystemColor_WindowText, path);
			}
#endif // WPF
		}

		#endregion // Paint

		public override bool OnStartEdit()
		{
			return false;
		}

		/// <summary>
		/// Handle event when data set into the cell of this body.
		/// </summary>
		/// <param name="data">Data inputted by user.</param>
		/// <returns>Data to be set into the cell.</returns>
		public override object OnSetData(object data)
		{
			this.IsChecked = data is bool && (bool)data;

			return base.OnSetData(data);
		}

		#region Check Status

		/// <summary>
		/// Toggle the check status of check box.
		/// </summary>
		public virtual void ToggleCheckStatus()
		{
			if (this.Cell != null
				&& this.DisableWhenCellReadonly
				&& this.Cell.IsReadOnly)
			{
				return;
			}

			this.IsChecked = !IsChecked;
		}

		/// <summary>
		/// Determines whether or not check-box is pressed or checked.
		/// </summary>
		[Obsolete("use IsChecked instead")]
		public bool Checked
		{
			get { return this.IsChecked; }
			set { this.IsChecked = value; }
		}

		/// <summary>
		/// Check status.
		/// </summary>
		protected bool isChecked;

		/// <summary>
		/// Get or set the check-status of check box.
		/// </summary>
		public virtual bool IsChecked
		{
			get
			{
				return this.isChecked;
			}
			set
			{
				if (this.isChecked != value)
				{
					this.isChecked = value;

					if (this.Cell != null && ((this.Cell.InnerData as bool?) ?? false) != value)
					{
						this.Cell.Data = value;
					}

					CheckChanged?.Invoke(this, null);
				}
			}
		}
		#endregion // Check Status

		#region Keyboard
		/// <summary>
		/// Handle keyboard down event.
		/// </summary>
		/// <param name="keyCode">Virtual keys that are pressed.</param>
		/// <returns>True if event has been marked as handled.</returns>
		public override bool OnKeyDown(KeyCode keyCode)
		{
			if (keyCode == KeyCode.Space)
			{
				IsPressed = true;
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Handle keyboard up event.
		/// </summary>
		/// <param name="keyCode">Virtual keys code that is converted from system platform.</param>
		/// <returns>True if event has been handled; Otherwise return false.</returns>
		public override bool OnKeyUp(KeyCode keyCode)
		{
			if (IsPressed)
			{
				IsPressed = false;

				if (keyCode == KeyCode.Space)
				{
					ToggleCheckStatus();
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		#endregion // Keyboard

		public override ICellBody Clone()
		{
			return new CheckBoxCell();
		}
	}
	#endregion // Check box

	#region Radio button

	#region Radio Group
	/// <summary>
	/// Radio button group for toggling radios inside one group.
	/// </summary>
	[Serializable]
	public class RadioButtonGroup
	{
		private List<RadioButtonCell> radios = new List<RadioButtonCell>();

		/// <summary>
		/// Add radio button into this group.
		/// </summary>
		/// <param name="cell"></param>
		public virtual void AddRadioButton(RadioButtonCell cell)
		{
			if (cell == null) return;

			if (!radios.Contains(cell))
			{
				radios.Add(cell);
			}

			if (cell.RadioGroup != this)
			{
				cell.RadioGroup = this;
			}
		}

		public List<RadioButtonCell> RadioButtons { get { return this.radios; } }

		/// <summary>
		/// Check whether specified radio is contained by this group.
		/// </summary>
		/// <param name="cell">radio cell body to be checked.</param>
		/// <returns>true if the radio cell body is contained by this group.</returns>
		public virtual bool Contains(RadioButtonCell cell)
		{
			return radios.Contains(cell);
		}
	}
	#endregion // Radio Group

	/// <summary>
	/// Representation for a radio button of cell body.
	/// </summary>
	[Serializable]
	public class RadioButtonCell : CheckBoxCell
	{
		/// <summary>
		/// Create instance of radio button cell.
		/// </summary>
		public RadioButtonCell()
		{
		}

		#region Group
		private RadioButtonGroup radioGroup;

		/// <summary>
		/// Radio groups for toggling other radios inside same group.
		/// </summary>
		public virtual RadioButtonGroup RadioGroup
		{
			get { return radioGroup; }
			set
			{
				if (value == null)
				{
					this.RadioGroup = null;
				}
				else
				{
					if (!value.Contains(this))
					{
						value.AddRadioButton(this);
					}

					this.radioGroup = value;
				}
			}
		}
		#endregion // Group

		/// <summary>
		/// Get or set check status for radio button
		/// </summary>
		public override bool IsChecked
		{
			get
			{
				return this.isChecked;
			}
			set
			{
				if (this.isChecked != value)
				{
					this.isChecked = value;

					// uncheck other radios in same group
					if (this.isChecked && this.radioGroup != null)
					{
						foreach (var other in this.radioGroup.RadioButtons)
						{
							if (other != this)
							{
								other.IsChecked = false;
							}
						}
					}

					if (this.Cell != null && ((this.Cell.InnerData as bool?) ?? false) != value)
					{
						this.Cell.Data = value;
					}

					this.RaiseCheckChangedEvent();
				}
			}
		}

		/// <summary>
		/// Toggle check status of radio-button. (Only work when radio button not be added into any groups)
		/// </summary>
		public override void ToggleCheckStatus()
		{
			if (!this.isChecked || this.radioGroup == null)
			{
				base.ToggleCheckStatus();
			}
		}

		/// <summary>
		/// Paint content of cell body.
		/// </summary>
		/// <param name="dc">Platform independency graphics context.</param>
		protected override void OnContentPaint(CellDrawingContext dc)
		{
#if WINFORM
			System.Windows.Forms.ButtonState state = ButtonState.Normal;

			if (this.IsPressed) state |= ButtonState.Pushed;
			if (this.IsChecked) state |= ButtonState.Checked;

			ControlPaint.DrawRadioButton(dc.Graphics.PlatformGraphics,
				(System.Drawing.Rectangle)this.ContentBounds, state);

#elif WPF
			var g = dc.Graphics;

			var ox = this.ContentBounds.OriginX;
			var oy = this.ContentBounds.OriginY;

			var hw = this.ContentBounds.Width / 2;
			var hh = this.ContentBounds.Height / 2;
			var r = new Rectangle(ox - hw / 2, oy - hh / 2, hw, hh);
			g.DrawEllipse(StaticResources.SystemColor_ControlDark, r);

			if (this.IsPressed)
			{
				g.FillEllipse(StaticResources.SystemColor_Control, r);
			}

			if (this.isChecked)
			{
				var hhw = this.ContentBounds.Width / 4;
				var hhh = this.ContentBounds.Height / 4;
				r = new Rectangle(ox - hhw / 2, oy - hhh / 2, hhw, hhh);
				g.FillEllipse(StaticResources.SystemColor_WindowText, r);
			}
#endif // WINFORM
		}

		/// <summary>
		/// Clone radio button from this object.
		/// </summary>
		/// <returns>New instance of radio button.</returns>
		public override ICellBody Clone()
		{
			return new RadioButtonCell();
		}
	}
	#endregion // Radio button

	#region Image
	/// <summary>
	/// Representation for an image of cell body
	/// </summary>
	public class ImageCell : CellBody
	{
		/// <summary>
		/// Get or set the image to be displayed in cell
		/// </summary>
		public RGImage Image { get; set; }

		#region Constructor
		/// <summary>
		/// Create image cell object.
		/// </summary>
		public ImageCell() { }

		/// <summary>
		/// Construct image cell-body to show a specified image
		/// </summary>
		/// <param name="image">Image to be displayed</param>
		public ImageCell(RGImage image)
			: this(image, default(ImageCellViewMode))
		{
		}

		/// <summary>
		/// Construct image cell-body to show a image by specified display-method
		/// </summary>
		/// <param name="image">Image to be displayed</param>
		/// <param name="viewMode">View mode decides how to display a image inside a cell</param>
		public ImageCell(RGImage image, ImageCellViewMode viewMode)
		{
			this.Image = image;
			this.viewMode = viewMode;
		}
		#endregion // Constructor

		#region ViewMode
		protected ImageCellViewMode viewMode;

		/// <summary>
		/// Set or get the view mode of this image cell
		/// </summary>
		public ImageCellViewMode ViewMode
		{
			get
			{
				return this.viewMode;
			}
			set
			{
				if (this.viewMode != value)
				{
					this.viewMode = value;

					if (base.Cell != null && base.Cell.Worksheet != null)
					{
						base.Cell.Worksheet.RequestInvalidate();
					}
				}
			}
		}
		#endregion // ViewMode

		#region OnPaint
		/// <summary>
		/// Render the image cell body.
		/// </summary>
		/// <param name="dc">Platform no-associated drawing context instance.</param>
		public override void OnPaint(CellDrawingContext dc)
		{
			if (Image != null)
			{
				RGFloat x = Bounds.X, y = Bounds.Y, width = 0, height = 0;
				bool needClip = false;

				switch (this.viewMode)
				{
					default:
					case ImageCellViewMode.Stretch:
						width = Bounds.Width;
						height = Bounds.Height;
						break;

					case ImageCellViewMode.Zoom:
						RGFloat widthRatio = (RGFloat)Bounds.Width / Image.Width;
						RGFloat heightRatio = (RGFloat)Bounds.Height / Image.Height;
						RGFloat minRatio = Math.Min(widthRatio, heightRatio);
						width = minRatio * Image.Width;
						height = minRatio * Image.Height;
						break;

					case ImageCellViewMode.Clip:
						width = Image.Width;
						height = Image.Height;

						if (width > Bounds.Width || height > Bounds.Height) needClip = true;
						break;
				}

				switch (Cell.Style.HAlign)
				{
					default:
					case ReoGridHorAlign.Left:
						x = Bounds.X;
						break;

					case ReoGridHorAlign.Center:
						x = (Bounds.Width - width) / 2;
						break;

					case ReoGridHorAlign.Right:
						x = Bounds.Width - width;
						break;
				}

				switch (Cell.Style.VAlign)
				{
					default:
					case ReoGridVerAlign.Top:
						y = Bounds.Y;
						break;

					case ReoGridVerAlign.Middle:
						y = (Bounds.Height - height) / 2;
						break;

					case ReoGridVerAlign.Bottom:
						y = Bounds.Height - height;
						break;
				}

				var g = dc.Graphics;

				if (needClip)
				{
					g.PushClip(Bounds);
				}

				g.DrawImage(Image, x, y, width, height);

				if (needClip)
				{
					g.PopClip();
				}
			}

			dc.DrawCellText();
		}
		#endregion // OnPaint

		public override ICellBody Clone()
		{
			return new ImageCell(this.Image);
		}
	}

	#region ImageCellViewMode
	/// <summary>
	/// Image dispaly method in ImageCell-body
	/// </summary>
	public enum ImageCellViewMode
	{
		/// <summary>
		/// Fill to cell boundary. (default)
		/// </summary>
		Stretch,

		/// <summary>
		/// Lock aspect ratio to fit cell boundary.
		/// </summary>
		Zoom,

		/// <summary>
		/// Keep original image size and clip to fill the cell.
		/// </summary>
		Clip,
	}
	#endregion // ImageCellViewMode
	#endregion // Image

#if WINFORM

	#region Dropdown List

	/// <summary>
	/// Representation of a typecial dropdown control on spreadsheet
	/// </summary>
	public class DropdownListCell : DropdownCell
	{
		/// <summary>
		/// Construct dropdown control with an empty candidates list
		/// </summary>
		public DropdownListCell()
			: base()
		{
			this.candidates = new List<object>(0);
		}

		/// <summary>
		/// Construct dropdown control with specified candidates array
		/// </summary>
		/// <param name="candidates">candidate object array to be displayed in the listbox</param>
		public DropdownListCell(params object[] candidates)
			: this()
		{
			this.candidates.AddRange(candidates);
		}

		/// <summary>
		/// Construct dropdown control with specified candidates array
		/// </summary>
		/// <param name="candidates">candidate object array to be displayed in the listbox</param>
		public DropdownListCell(IEnumerable<object> candidates)
			: this()
		{
			this.candidates.AddRange(candidates);
		}

		/// <summary>
		/// Get or set the selected item in candidates list
		/// </summary>
		public object SelectedItem
		{
			get
			{
				return this.listBox.SelectedItem;
			}
			set
			{
				this.listBox.SelectedItem = value;
			}
		}

		/// <summary>
		/// Get or set the selected index in candidates list
		/// </summary>
		public int SelectedIndex
		{
			get
			{
				return this.listBox.SelectedIndex;
			}
			set
			{
				this.listBox.SelectedIndex = value;
			}
		}

		/// <summary>
		/// Set selected item
		/// </summary>
		/// <param name="obj">Selected item to be handled</param>
		protected virtual void SetSelectedItem(object obj)
		{
			Cell.Data = obj;

			this.SelectedItemChanged?.Invoke(this, null);
		}

		/// <summary>
		/// Event for selected item changed
		/// </summary>
		public event EventHandler SelectedItemChanged;

		private List<object> candidates;

		/// <summary>
		/// Push down the dropdown panel.
		/// </summary>
		public override void PushDown()
		{
			if (this.listBox == null)
			{
				this.listBox = new ListBox()
				{
					Dock = DockStyle.Fill,
					BorderStyle = System.Windows.Forms.BorderStyle.None,
				};

				listBox.Click += ListBox_Click;
				listBox.KeyDown += ListBox_KeyDown;
				listBox.MouseMove += (sender, e) =>
				{
					int index = listBox.IndexFromPoint(e.Location);
					if (index != -1) listBox.SelectedIndex = index;
				};

				if (this.candidates != null)
				{
					listBox.Items.AddRange(this.candidates.ToArray());
				}

				base.DropdownControl = listBox;
			}

			listBox.SelectedItem = this.Cell.InnerData;

			base.PushDown();
		}

#if WINFORM
		private ListBox listBox;

		void ListBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (base.Cell != null && base.Cell.Worksheet != null)
			{
				if ((e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space))
				{
					SetSelectedItem(this.listBox.SelectedItem);
					PullUp();
				}
				else if (e.KeyCode == Keys.Escape)
				{
					PullUp();
				}
			}
		}

		void ListBox_Click(object sender, EventArgs e)
		{
			if (this.Cell != null && this.Cell.Worksheet != null)
			{
				SetSelectedItem(this.listBox.SelectedItem);
			}

			PullUp();
		}
#elif WPF
		private System.Windows.Controls.ListBox listBox;

#endif // WPF

		#region Items Property
		private DropdownItemsCollection itemsCollection;

		/// <summary>
		/// Collection of condidate items
		/// </summary>
		public DropdownItemsCollection Items
		{
			get
			{
				if (this.itemsCollection == null)
				{
					this.itemsCollection = new DropdownItemsCollection(this);
				}

				return this.itemsCollection;
			}
		}
		#endregion // Items Property

		#region DropdownItemsCollection
		/// <summary>
		/// Represents drop-down items collection.
		/// </summary>
		public class DropdownItemsCollection : ICollection<object>
		{
			private DropdownListCell owner;

			internal DropdownItemsCollection(DropdownListCell owner)
			{
				this.owner = owner;
			}

			/// <summary>
			/// Add candidate item.
			/// </summary>
			/// <param name="item">Item to be added.</param>
			public void Add(object item)
			{
				if (this.owner.listBox != null)
				{
					this.owner.listBox.Items.Add(item);
				}
				else
				{
					this.owner.candidates.Add(item);
				}
			}

			/// <summary>
			/// Add multiple candidate items.
			/// </summary>
			/// <param name="items">Items to be added.</param>
			public void AddRange(params object[] items)
			{
				if (this.owner.listBox != null)
				{
					this.owner.listBox.Items.AddRange(items);
				}
				else
				{
					this.owner.candidates.AddRange(items);
				}
			}

			/// <summary>
			/// Clear all candidate items.
			/// </summary>
			public void Clear()
			{
				if (this.owner.listBox != null)
				{
					this.owner.listBox.Items.Clear();
				}
				else
				{
					this.owner.candidates.Clear();
				}
			}

			/// <summary>
			/// Check whether or not the candidate list contains specified item.
			/// </summary>
			/// <param name="item">item to be checked.</param>
			/// <returns>true if contained, otherwise return false.</returns>
			public bool Contains(object item)
			{
				if (this.owner.listBox != null)
				{
					return this.owner.listBox.Items.Contains(item);
				}
				else
				{
					return this.owner.candidates.Contains(item);
				}
			}

			/// <summary>
			/// Copy the candidate list into specified array.
			/// </summary>
			/// <param name="array">array to be copied into.</param>
			/// <param name="arrayIndex">number of item to start copy.</param>
			public void CopyTo(object[] array, int arrayIndex)
			{
				if (this.owner.listBox != null)
				{
					this.owner.listBox.Items.CopyTo(array, arrayIndex);
				}
				else
				{
					this.owner.candidates.CopyTo(array, arrayIndex);
				}
			}

			/// <summary>
			/// Return the number of items in candidate list.
			/// </summary>
			public int Count
			{
				get
				{
					if (this.owner.listBox != null)
					{
						return this.owner.listBox.Items.Count;
					}
					else
					{
						return this.owner.candidates.Count;
					}
				}
			}

			/// <summary>
			/// Check whether or not the candidate list is read-only.
			/// </summary>
			public bool IsReadOnly
			{
				get
				{
					if (this.owner.listBox != null)
					{
						return this.owner.listBox.Items.IsReadOnly;
					}
					else
					{
						return false;
					}
				}
			}

			/// <summary>
			/// Remove specified item from candidate list.
			/// </summary>
			/// <param name="item">item to be removed.</param>
			/// <returns>true if item has been removed successfully.</returns>
			public bool Remove(object item)
			{
				if (this.owner.listBox != null)
				{
					this.owner.listBox.Items.Remove(item);
					return true;
				}
				else
				{
					return this.owner.candidates.Remove(item);
				}
			}

			/// <summary>
			/// Get enumerator of candidate list.
			/// </summary>
			/// <returns>enumerator of candidate list.</returns>
			public IEnumerator<object> GetEnumerator()
			{
				if (this.owner.listBox != null)
				{
					var items = this.owner.listBox.Items;
					foreach (var item in items)
					{
						yield return item;
					}
				}
				else
				{
					foreach (var item in this.owner.candidates)
						yield return item;
				}
			}

			/// <summary>
			/// Get enumerator of candidate list.
			/// </summary>
			/// <returns>enumerator of candidate list.</returns>
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				if (this.owner.listBox != null)
				{
					var items = this.owner.listBox.Items;

					foreach (var item in items)
					{
						yield return item;
					}
				}
				else
				{
					foreach (var item in this.owner.candidates)
					{
						yield return item;
					}
				}
			}
		}
		#endregion // DropdownItemsCollection

		/// <summary>
		/// Clone a drop-down list from this object.
		/// </summary>
		/// <returns>New instance of dropdown list.</returns>
		public override ICellBody Clone()
		{
			return new DropdownListCell(this.candidates);
		}
	}

	#endregion // Dropdown

	#region Dropdown Button
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

			Point p;

			if (sheet != null && this.DropdownControl != null
				&& Views.CellsViewport.TryGetCellPositionToControl(sheet.ViewportController.FocusView, this.Cell.InternalPos, out p))
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
	#endregion // Custom Dropdown

	#region Column Dropdown List
	/// <summary>
	/// Represents dropdown list cell for entire column.
	/// </summary>
	public class ColumnDropdownListCell : DropdownCell
	{
		/// <summary>
		/// Listbox component instance.
		/// </summary>
		protected static ListBox listBox;

		/// <summary>
		/// Push down the dropdown panel.
		/// </summary>
		public override void PushDown()
		{
			if (ColumnDropdownListCell.listBox == null)
			{
				ColumnDropdownListCell.listBox = new ListBox
				{
					BorderStyle = System.Windows.Forms.BorderStyle.None,
				};

				base.DropdownControl = ColumnDropdownListCell.listBox;
			}

			ColumnDropdownListCell.listBox.Click += listBox_Click;

			base.PushDown();
		}

		/// <summary>
		/// Push up the dropdown panel.
		/// </summary>
		public override void PullUp()
		{
			if (ColumnDropdownListCell.listBox != null)
			{
				ColumnDropdownListCell.listBox.Click -= listBox_Click;
			}

			base.PullUp();
		}

		void listBox_Click(object sender, EventArgs e)
		{
			base.Cell.Data = ColumnDropdownListCell.listBox.SelectedItem;

			this.PullUp();
		}
	}
	#endregion

	#region NumberInputCell
	internal class NumberInputCell : CellBody
	{
		private NumericTextbox textbox = new NumericTextbox
		{
			BorderStyle = System.Windows.Forms.BorderStyle.None,
			TextAlign = HorizontalAlignment.Right,
			Visible = false,
		};

		private class NumericTextbox : TextBox
		{
			private static readonly string validChars = "0123456789-.";
			protected override bool IsInputChar(char charCode)
			{
				return charCode == '\b' || false;
			}
			protected override bool ProcessDialogChar(char charCode)
			{
				return validChars.IndexOf(charCode) < 0;
			}
		}

		private Worksheet owner;
		private System.Windows.Forms.Timer timer;
		public NumberInputCell(Worksheet owner)
		{
			this.owner = owner;
			timer = new System.Windows.Forms.Timer();
			timer.Tick += new EventHandler(timer_Tick);
			timer.Enabled = false;

			//textbox.KeyDown += new KeyEventHandler(textbox_KeyDown);
			//textbox.MouseUp += new MouseEventHandler(textbox_MouseUp);
		}

		//void textbox_MouseUp(object sender, MouseEventArgs e)
		//{
		//	OnMouseUp(e);
		//}

		//void textbox_KeyDown(object sender, KeyEventArgs e)
		//{
		//	if (Visible && e.KeyCode == Keys.Tab || e.KeyCode == Keys.Enter)
		//	{
		//		e.SuppressKeyPress = true;
		//		owner.EndEdit(GetValue());
		//		owner.MoveSelectionForward();
		//	}
		//	else if (e.KeyCode == Keys.Escape)
		//	{
		//		e.SuppressKeyPress = true;
		//		owner.EndEdit(backupData);
		//	}
		//	else if (e.KeyCode == Keys.Up)
		//	{
		//		ValueAdd(Toolkit.IsKeyDown(unvell.Common.Win32Lib.Win32.VKey.VK_SHIFT) ? 10 :
		//			(Toolkit.IsKeyDown(unvell.Common.Win32Lib.Win32.VKey.VK_CONTROL) ? 100 : 1));
		//		e.SuppressKeyPress = true;
		//	}
		//	else if (e.KeyCode == Keys.Down)
		//	{
		//		ValueSub(Toolkit.IsKeyDown(unvell.Common.Win32Lib.Win32.VKey.VK_SHIFT) ? 10 :
		//			(Toolkit.IsKeyDown(unvell.Common.Win32Lib.Win32.VKey.VK_CONTROL) ? 100 : 1));
		//		e.SuppressKeyPress = true;
		//	}
		//	else if (e.KeyCode == Keys.PageUp)
		//	{
		//		ValueAdd(10);
		//		e.SuppressKeyPress = true;
		//	}
		//	else if (e.KeyCode == Keys.PageDown)
		//	{
		//		ValueSub(10);
		//		e.SuppressKeyPress = true;
		//	}
		//	else if (e.KeyCode == Keys.V
		//		&& Toolkit.IsKeyDown(unvell.Common.Win32Lib.Win32.VKey.VK_CONTROL))
		//	{
		//		textbox.Paste();
		//	}
		//	else if (e.KeyCode == Keys.C
		//		&& Toolkit.IsKeyDown(unvell.Common.Win32Lib.Win32.VKey.VK_CONTROL))
		//	{
		//		textbox.Copy();
		//	}
		//	else if (e.KeyCode == Keys.X
		//		&& Toolkit.IsKeyDown(unvell.Common.Win32Lib.Win32.VKey.VK_CONTROL))
		//	{
		//		textbox.Cut();
		//	}
		//	//else if ((e.KeyValue & (int)Keys.LButton) > 0
		//	//  || (e.KeyValue & (int)Keys.RButton) > 0)
		//	//{
		//	//}
		//	//else if ((e.KeyValue < '0' || e.KeyValue > '9') && e.KeyCode != Keys.Back)
		//	//{
		//	//  e.SuppressKeyPress = true;
		//	//}
		//}

		void timer_Tick(object sender, EventArgs e)
		{
			if (isUpPressed)
				ValueAdd(1);
			else if (isDownPressed)
				ValueSub(1);
			timer.Interval = 50;
		}
		object backupData;
		internal void SetValue(object data)
		{
			backupData = data;
			int value = 0;
			if (data is int)
				value = (int)data;
			else if (data is string)
				int.TryParse(data as string, out value);
			textbox.Text = value.ToString();
		}
		internal object GetValue()
		{
			int value = 0;
			int.TryParse(textbox.Text as string, out value);
			return value;
		}
		private const int ButtonSize = 17;
		private const int ArrowSize = 9;
		private bool isUpPressed = false;
		private bool isDownPressed = false;

		//		public override bool OnStartEdit(ReoGridCell cell)
		//{

		//				textbox.Visible = true;
		//				textbox.Focus();
		//			}
		//			else
		//			{
		//				owner.EndEdit(GetValue());
		//			}
		//		}

		public override void OnBoundsChanged()
		{
			base.OnBoundsChanged();

			int hh = textbox.Height / 2;
			textbox.Bounds = new System.Drawing.Rectangle(
				(int)Math.Round(Bounds.Left),
				(int)Math.Round(Bounds.Top + Bounds.Height / 2 - hh - 1),
				(int)Math.Round(Bounds.Width - ButtonSize - 1),
				textbox.Height);
		}

		public override void OnPaint(CellDrawingContext dc)
		{
			var g = dc.Graphics;

			RGFloat hh = Bounds.Height / 2 - 1;

			Rectangle rect = Bounds;

			Rectangle upRect = new Rectangle(rect.Right - ButtonSize, rect.Top, ButtonSize, hh);
			GraphicsToolkit.Draw3DButton(g.PlatformGraphics, (System.Drawing.Rectangle)upRect, isUpPressed);
			GraphicsToolkit.FillTriangle(g.PlatformGraphics, ArrowSize,
				new Point(upRect.Left + ButtonSize / 2 - ArrowSize / 2,
					upRect.Top + hh / 2 + (isUpPressed ? 2 : 1)),
				GraphicsToolkit.TriangleDirection.Up);

			Rectangle downRect = new Rectangle(rect.Right - ButtonSize, rect.Top + hh + 1, ButtonSize, hh);
			GraphicsToolkit.Draw3DButton(g.PlatformGraphics, (System.Drawing.Rectangle)downRect, isDownPressed);
			GraphicsToolkit.FillTriangle(g.PlatformGraphics, ArrowSize,
				new Point(downRect.Left + ButtonSize / 2 - ArrowSize / 2,
					downRect.Top + hh / 2 - (isDownPressed ? 1 : 2)),
				GraphicsToolkit.TriangleDirection.Down);
		}

		internal void ValueAdd(int d)
		{
			int value = 0;
			int.TryParse(textbox.Text, out value);
			value += d;
			textbox.Text = value.ToString();
			textbox.SelectAll();
		}
		internal void ValueSub(int d)
		{
			int value = 0;
			int.TryParse(textbox.Text, out value);
			value -= d;
			textbox.Text = value.ToString();
			textbox.SelectAll();
		}

		public override bool OnMouseDown(CellMouseEventArgs e)
		{
			RGFloat hh = Bounds.Height / 2 - 1;

			Rectangle upRect = new Rectangle(Bounds.Right - ButtonSize, Bounds.Top, ButtonSize, hh);
			Rectangle downRect = new Rectangle(Bounds.Right - ButtonSize, Bounds.Top + hh + 1, ButtonSize, hh);

			if (upRect.Contains(e.RelativePosition))
			{
				textbox.Capture = true;
				isUpPressed = true;
				ValueAdd(1);
				timer.Interval = 600;
				timer.Start();
				return true;
			}
			else if (downRect.Contains(e.RelativePosition))
			{
				textbox.Capture = true;
				isDownPressed = true;
				ValueSub(1);
				timer.Interval = 600;
				timer.Start();
				return true;
			}

			return false;
		}
		public override bool OnMouseUp(CellMouseEventArgs e)
		{

			isUpPressed = false;
			isDownPressed = false;
			timer.Stop();
			return true;
		}
		public override bool OnMouseMove(CellMouseEventArgs e)
		{

			//Cursor = Cursors.Default;
			base.OnMouseMove(e);
			return true;
		}

		internal int GetNumericValue()
		{
			int num = 0;
			int.TryParse(textbox.Text, out num);
			return num;
		}
		internal void SelectAll()
		{
			textbox.SelectAll();
		}


		//protected override void OnGotFocus(EventArgs e)
		//{
		//	//base.OnGotFocus(e);
		//	textbox.Focus();
		//}
		internal IntPtr GetTextboxHandle()
		{
			return textbox.Handle;
		}
	}
	#endregion // NumberInputCell

#endif // WINFORM
	#endregion // Built-in Cell Types

#if WINFORM

	/// <summary>
	/// Represetns a date picker cell on worksheet.
	/// </summary>
	[Serializable]
	public class DatePickerCell : DropdownCell
	{
		private MonthCalendar calendar = null;

		/// <summary>
		/// Create instance of date picker cell.
		/// </summary>
		public DatePickerCell()
		{
			calendar = new MonthCalendar()
			{
				CalendarDimensions = new System.Drawing.Size(1, 1),
				MaxSelectionCount = 1,
			};

			calendar.DateSelected += Calendar_DateSelected;

			base.DropdownControl = calendar;
			base.MinimumDropdownWidth = calendar.Width + 20;
			base.DropdownPanelHeight = calendar.Height + 10;
		}

		private void Calendar_DateSelected(object sender, DateRangeEventArgs e)
		{
			if (this.Cell != null) this.Cell.Data = calendar.SelectionStart;
		}

		/// <summary>
		/// Override method invoked when this cell body is set into any cells.
		/// </summary>
		/// <param name="cell">The cell this body will be set to.</param>
		public override void OnSetup(Cell cell)
		{
			base.OnSetup(cell);

			cell.Style.Indent = 3;
			cell.DataFormat = DataFormat.CellDataFormatFlag.DateTime;
		}

		/// <summary>
		/// Clone new date picker from this object.
		/// </summary>
		/// <returns>New instance of date picker.</returns>
		public override ICellBody Clone()
		{
			return new DatePickerCell();
		}
	}
#endif // WINFORM

	#region ImageButtonCell

	/// <summary>
	/// Represents an image button cell on worksheet.
	/// </summary>
	[Serializable]
	public class ImageButtonCell : ButtonCell
	{
		/// <summary>
		/// Image that displayed on button.
		/// </summary>
		public RGImage Image { get; set; }

		/// <summary>
		/// Create image button cell without image specified.
		/// </summary>
		public ImageButtonCell()
			: this(null)
		{
		}

		/// <summary>
		/// Create image button cell with specified image.
		/// </summary>
		/// <param name="image"></param>
		public ImageButtonCell(RGImage image)
		{
			this.Image = image;
		}

		/// <summary>
		/// Paint image button cell.
		/// </summary>
		/// <param name="dc">Platform non-associated drawing context.</param>
		public override void OnPaint(CellDrawingContext dc)
		{
			base.OnPaint(dc);

			if (this.Image != null)
			{
				RGFloat widthScale = Math.Min((Bounds.Width - 4) / this.Image.Width, 1);
				RGFloat heightScale = Math.Min((Bounds.Height - 4) / this.Image.Height, 1);

				RGFloat minScale = Math.Min(widthScale, heightScale);
				RGFloat imageScale = (RGFloat)Image.Height / Image.Width;
				RGFloat width = Image.Width * minScale;

				Rectangle r = new Rectangle(0, 0, width, imageScale * width);

				r.X = (Bounds.Width - r.Width) / 2;
				r.Y = (Bounds.Height - r.Height) / 2;

				if (this.IsPressed)
				{
					r.X++;
					r.Y++;
				}

				dc.Graphics.DrawImage(this.Image, r);
			}
		}

		/// <summary>
		/// Clone image button from this object.
		/// </summary>
		/// <returns>New instance of image button.</returns>
		public override ICellBody Clone()
		{
			return new ImageButtonCell(this.Image);
		}
	}

	#endregion // ImageButtonCell

	#region NegativeProgressCell

	/// <summary>
	/// Progress bar for display both positive and negative percent.
	/// </summary>
	[Serializable]
	public class NegativeProgressCell : CellBody
	{
		#region Attributes
		/// <summary>
		/// Get or set color for positive display.
		/// </summary>
		public SolidColor PositiveColor { get; set; }

		/// <summary>
		/// Get or set color for negative display.
		/// </summary>
		public SolidColor NegativeColor { get; set; }

		/// <summary>
		/// Determines whether or not display a linear gradient color on progress bar.
		/// </summary>
		public bool LinearGradient { get; set; }

		/// <summary>
		/// Determines whether or not display the cell text or value.
		/// </summary>
		public bool DisplayCellText { get; set; }

		/// <summary>
		/// Determines whether or not force to display the progress inside cell.
		/// </summary>
		public bool LimitedInsideCell { get; set; }
		#endregion // Attributes

		#region Constructor
		/// <summary>
		/// Create negative progress cell.
		/// </summary>
		public NegativeProgressCell()
		{
			this.PositiveColor = SolidColor.LightGreen;
			this.NegativeColor = SolidColor.LightCoral;
			this.LinearGradient = true;
			this.DisplayCellText = true;
			this.LimitedInsideCell = true;
		}
		#endregion // Constructor

		#region OnPaint
		/// <summary>
		/// Render the negative progress cell body.
		/// </summary>
		/// <param name="dc"></param>
		public override void OnPaint(CellDrawingContext dc)
		{
			double value = this.Cell.GetData<double>();

			if (LimitedInsideCell)
			{
				if (value > 1) value = 1;
				else if (value < -1) value = -1;
			}

			var g = dc.Graphics;

			Rectangle rect;

			if (value >= 0)
			{
				rect = new Rectangle(Bounds.Left + Bounds.Width / 2, Bounds.Top + 1,
					(RGFloat)(Bounds.Width * (value / 2.0d)), Bounds.Height - 1);

				if (rect.Width > 0 && rect.Height > 0)
				{
					if (this.LinearGradient)
					{
						g.FillRectangleLinear(this.PositiveColor,
							new SolidColor(0, this.PositiveColor), 0, rect);
					}
					else
					{
						g.FillRectangle(rect, this.PositiveColor);
					}
				}
			}
			else
			{
				RGFloat center = Bounds.Left + Bounds.Width / 2.0f;
				RGFloat left = (RGFloat)(Bounds.Width * value * 0.5d);
				rect = new Rectangle(center + left, Bounds.Top + 1, -left, Bounds.Height - 1);

				if (rect.Width > 0 && rect.Height > 0)
				{
					if (this.LinearGradient)
					{
						g.FillRectangleLinear(this.NegativeColor,
							new SolidColor(0, this.NegativeColor), 180, rect);
					}
					else
					{
						g.FillRectangle(rect, this.NegativeColor);
					}
				}
			}

			if (DisplayCellText)
			{
				dc.DrawCellText();
			}
		}
		#endregion // OnPaint

		#endregion // NegativeProgressCell

	}

}
