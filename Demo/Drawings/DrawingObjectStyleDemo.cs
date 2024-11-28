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
	public partial class DrawingObjectStyleDemo : UserControl, IDemoHelp
	{
		public DrawingObjectStyleDemo()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs _unused)
		{
			base.OnLoad(_unused);

			var worksheet = this.grid.CurrentWorksheet;

			worksheet["A2"] = "Easy to set styles such as back color or line style to drawing objects.";

			#region Add drawing object

			// create drawing object: rectangle shape
			var rect = new Drawing.Shapes.RectangleShape()
				{
					Location = new Graphics.Point(120, 90),
					Size = new Graphics.Size(160, 100),

					Text = "Object Style Sample",
				};

			// set fill color
			rect.Style.FillColor = Graphics.SolidColor.LightGoldenrodYellow;

			// set line width and color
			rect.Style.LineWidth = 3;
			rect.Style.LineColor = Graphics.SolidColor.LimeGreen;

			// rotate object
			rect.RotateAngle = 45;

			// add shape into worksheet
			worksheet.FloatingObjects.Add(rect);

			#endregion // Add drawing object


			#region Add angle slider

			worksheet["B13"] = "Slide to rotate this object.";

			worksheet.Ranges["B15:D15"].Merge();
			worksheet["B15"] = new Demo.CustomCells.SlideCell();
			worksheet["B15"] = 45f / 350f;

			// create a customize function to format degree number
			ReoGrid.Formula.FormulaExtension.CustomFunctions["formatToDegree"] = (cell, args) =>
			{
				unvell.ReoGrid.Utility.CellUtility.TryGetNumberData(args[0], out var data);
				return Math.Round(data) + "\u00b0";
			};

			worksheet["E15"] = "=formatToDegree(B15*360)";
			worksheet.Cells["E15"].Style.HAlign = ReoGridHorAlign.Right;

			// handle the slide changes and rotate object
			worksheet.CellDataChanged += (s, e) =>
			{
				if (e.Cell.Position.ToAddress() == "B15")
				{
					rect.RotateAngle = (float)(e.Cell.GetData<double>() * 360f);
				}
			};

			#endregion Add angle slider
		}

		public string GetHTMLHelp()
		{
			var resMan = new System.Resources.ResourceManager(this.GetType());
			return (string)resMan.GetObject("HTMLHelp");
		}

	}

}
