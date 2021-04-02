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

using unvell.ReoGrid.CellTypes;

namespace unvell.ReoGrid.Demo.Styles
{
	public partial class BorderStylesDemo : UserControl
	{
		public BorderStylesDemo()
		{
			InitializeComponent();

			// get first worksheet
			var sheet = this.grid.CurrentWorksheet;

			// hide grid lines
			sheet.DisableSettings(WorksheetSettings.View_ShowGridLine);

			// set rows and columns size
			sheet.SetColumnsWidth(0, 15, 60);
			sheet.SetRowsHeight(0, 20, 30);
			
			sheet["B2"] = "Border Styles:";

			// create a range position
			var range = new RangePosition("B4:D4");

			// set border styles demo

			sheet[range] = "Solid";
			sheet.SetRangeBorders(range, BorderPositions.Bottom, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.Solid });

			range.Offset(1, 0);
			sheet[range] = "Dotted";
			sheet.SetRangeBorders(range, BorderPositions.Bottom, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.Dotted });

			range.Offset(1, 0);
			sheet[range] = "Dashed";
			sheet.SetRangeBorders(range, BorderPositions.Bottom, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.Dashed });

			range.Offset(1, 0);
			sheet[range] = "Dashed2";
			sheet.SetRangeBorders(range, BorderPositions.Bottom, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.Dashed2 });

			range.Offset(1, 0);
			sheet[range] = "DashDot";
			sheet.SetRangeBorders(range, BorderPositions.Bottom, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.DashDot });

			range.Offset(1, 0);
			sheet[range] = "DashDotDot";
			sheet.SetRangeBorders(range, BorderPositions.Bottom, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.DashDotDot });

			range.Offset(1, 0);
			sheet[range] = "Bold Solid";
			sheet.SetRangeBorders(range, BorderPositions.Bottom, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.BoldSolid });

			range.Offset(1, 0);
			sheet[range] = "Bold Dotted";
			sheet.SetRangeBorders(range, BorderPositions.Bottom, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.BoldDotted });

			range.Offset(1, 0);
			sheet[range] = "Bold Dashed";
			sheet.SetRangeBorders(range, BorderPositions.Bottom, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.BoldDashed });

			range.Offset(1, 0);
			sheet[range] = "Bold DashDot";
			sheet.SetRangeBorders(range, BorderPositions.Bottom, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.BoldDashDot });

			range.Offset(1, 0);
			sheet[range] = "Bold DashDotDot";
			sheet.SetRangeBorders(range, BorderPositions.Bottom, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.BoldDashDotDot });

			range.Offset(1, 0);
			sheet[range] = "Strong Solid";
			sheet.SetRangeBorders(range, BorderPositions.Bottom, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.BoldSolidStrong });

			range.Offset(1, 0);
			sheet[range] = "DoubleLine";
			sheet.SetRangeBorders(range, BorderPositions.Bottom, new RangeBorderStyle { Color = System.Drawing.Color.Black, Style = BorderLineStyle.DoubleLine });

			sheet["G3"] = "Outside";
			sheet.Cells["G3"].Style.HAlign = ReoGridHorAlign.Center;
			sheet.Cells["G3"].Style.VAlign = ReoGridVerAlign.Middle;
			sheet.SetRangeBorders("F2:H4", BorderPositions.Outside, RangeBorderStyle.BlackSolid);

			sheet["K3"] = "Inside";
			sheet.Cells["K3"].Style.HAlign = ReoGridHorAlign.Center;
			sheet.Cells["K3"].Style.VAlign = ReoGridVerAlign.Middle;
			sheet.SetRangeBorders("J2:L4", BorderPositions.InsideAll, RangeBorderStyle.BlackDotted);

			sheet["G7"] = "Left & Right";
			sheet.Cells["G7"].Style.HAlign = ReoGridHorAlign.Center;
			sheet.Cells["G7"].Style.VAlign = ReoGridVerAlign.Middle;
			sheet.SetRangeBorders("F6:H8", BorderPositions.Left | BorderPositions.Right, RangeBorderStyle.GraySolid);

			sheet["K7"] = "Top & Bottom";
			sheet.Cells["K7"].Style.HAlign = ReoGridHorAlign.Center;
			sheet.Cells["K7"].Style.VAlign = ReoGridVerAlign.Middle;
			sheet.SetRangeBorders("J6:L8", BorderPositions.Top | BorderPositions.Bottom, RangeBorderStyle.GrayDotted);

			sheet["G11"] = "SlateBlue";
			sheet.Cells["G11"].Style.HAlign = ReoGridHorAlign.Center;
			sheet.Cells["G11"].Style.VAlign = ReoGridVerAlign.Middle;
			sheet.SetRangeBorders("F10:H12", BorderPositions.InsideHorizontal | BorderPositions.Top | BorderPositions.Bottom,
				new RangeBorderStyle { Color = Color.SlateBlue, Style = BorderLineStyle.Dashed });

			sheet["K11"] = "DarkGoldenrod";
			sheet.Cells["K11"].Style.HAlign = ReoGridHorAlign.Center;
			sheet.Cells["K11"].Style.VAlign = ReoGridVerAlign.Middle;
			sheet.SetRangeBorders("J10:L12", BorderPositions.InsideVertical | BorderPositions.Left | BorderPositions.Right,
				new RangeBorderStyle { Color = Color.DarkGoldenrod, Style = BorderLineStyle.Dashed });

		}
	}
}
