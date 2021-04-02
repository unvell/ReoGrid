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

using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.CellTypes
{
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
