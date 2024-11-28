/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * ReoGrid and ReoGrid Demo project is released under MIT license.
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using unvell.ReoGrid;

namespace unvell.ReoGrid.Demo.Drawings
{
	public partial class AddingObjectDemo : UserControl, IDemoHelp
	{
		public AddingObjectDemo()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			var worksheet = this.grid.CurrentWorksheet;

			worksheet["A2"] = "Easy to add drawing shapes onto worksheet.";

			#region Rect 1
			// create drawing object: rectangle shape
			var rect1 = new Drawing.Shapes.RectangleShape()
				{
					Location = new Graphics.Point(60, 90),
					Size = new Graphics.Size(160, 80),

					Text = "Create drawing object",
				};

			// add shape into worksheet
			worksheet.FloatingObjects.Add(rect1);
			#endregion // Rect 1

			#region Line 1
			var line1 = new Drawing.Shapes.Line
			{
				StartPoint = new Graphics.Point(220, 130),
				EndPoint = new Graphics.Point(280, 130),
			};

			line1.Style.LineWidth = 1.5f;
			line1.Style.EndCap = Graphics.LineCapStyles.Arrow;

			worksheet.FloatingObjects.Add(line1);
			#endregion // Line 1

			#region Rect 2
			// create drawing object: rectangle shape
			Drawing.Shapes.RectangleShape rect2 = new Drawing.Shapes.RectangleShape()
			{
				Location = new Graphics.Point(280, 90),
				Size = new Graphics.Size(160, 80),

				Text = "Set text and styles",
			};

			// add shape into worksheet
			worksheet.FloatingObjects.Add(rect2);
			#endregion // Rect 2

			#region Line 2
			var line2 = new Drawing.Shapes.Line
			{
				StartPoint = new Graphics.Point(440, 130),
				EndPoint = new Graphics.Point(500, 130),
			};

			line2.Style.LineWidth = 1.5f;
			line2.Style.EndCap = Graphics.LineCapStyles.Arrow;

			worksheet.FloatingObjects.Add(line2);
			#endregion // Line 2

			#region Rect 3
			// create drawing object: rectangle shape
			Drawing.Shapes.RectangleShape rect3 = new Drawing.Shapes.RectangleShape()
			{
				Location = new Graphics.Point(500, 90),
				Size = new Graphics.Size(160, 80),

				Text = "Add onto worksheet",
			};

			// add shape into worksheet
			worksheet.FloatingObjects.Add(rect3);
			#endregion // Rect 3
		}

		public string GetHTMLHelp()
		{
			return unvell.ReoGrid.Demo.Properties.Resources.AddingObjectDemo_src;
		}
	}
}
