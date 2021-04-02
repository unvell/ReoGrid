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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.ComponentModel;

using unvell.Common;
using unvell.Common.Win32Lib;

namespace unvell.UIControls
{
	internal class ColorPickerWindow : ToolStripDropDown
	{
		private ColorPickerPanel colorPickerPanel = new ColorPickerPanel();

		private ToolStripControlHost controlHost;

		public ColorPickerWindow()
			: base()
		{
			this.TabStop = false;
			this.Margin = this.Padding = new Padding(1);
			this.AutoSize = false;
		
			colorPickerPanel.Dock = DockStyle.Fill;
			colorPickerPanel.Location = new Point(0, 0);
			colorPickerPanel.ColorPicked += (s, e) =>
			{
				if (ColorPicked != null) ColorPicked(this, null);
			};

			controlHost = new ToolStripControlHost(colorPickerPanel);
			controlHost.AutoSize = false;

			Items.Add(controlHost);

			this.Size = new Size(172, 220);
		}

		protected override void OnMouseMove(MouseEventArgs mea)
		{
			base.OnMouseMove(mea);
		}

		public AbstractColor CurrentColor
		{
			get { return colorPickerPanel.CurrentColor; }
			set { colorPickerPanel.CurrentColor = value; }
		}

		public Color SolidColor
		{
			get { return colorPickerPanel.SolidColor; }
			set { colorPickerPanel.SolidColor = value; }
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (controlHost != null) controlHost.Size = new Size(ClientRectangle.Width - 2, ClientRectangle.Height - 3);
		}

		internal event EventHandler ColorPicked;
	}
}
