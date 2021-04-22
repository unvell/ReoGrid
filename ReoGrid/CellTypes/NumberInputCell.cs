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

using unvell.Common;
using unvell.ReoGrid.Events;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.CellTypes
{
#if WINFORM

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
			int.TryParse(textbox.Text, out var value);
			value += d;
			textbox.Text = value.ToString();
			textbox.SelectAll();
		}
		internal void ValueSub(int d)
		{
			int.TryParse(textbox.Text, out var value);
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
			int.TryParse(textbox.Text, out var num);
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

}
