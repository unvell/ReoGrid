using System;

using unvell.ReoGrid.CellTypes;
using unvell.ReoGrid.Events;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.WPFDemo
{
	public class SlideCellBody : CellBody
	{
		// hold the instance of grid control
		public Worksheet Worksheet { get; set; }

		public override void OnSetup(Cell cell)
		{
			this.Worksheet = cell.Worksheet;
		}

		public bool IsHover { get; set; }

		public bool IsPressed { get; set; }

		public override void OnPaint(CellDrawingContext dc)
		{
			// try getting the cell value
			float value = 0;
			float.TryParse(dc.Cell.DisplayText, out value);

			// retrieve graphics object
			var g = dc.Graphics;

			int halfHeight = (int)Math.Round(Bounds.Height / 2f);
			int sliderHeight = (int)Math.Min(Bounds.Height - 4, 20);

			// draw slide bar
			g.FillRectangle(4, halfHeight - 3, Bounds.Width - 8, 6, SolidColor.Gainsboro);

			int x = 2 + (int)Math.Round(value * (Bounds.Width - 12));

			// thumb rectangle
			Rectangle rect = new Rectangle(x, halfHeight - sliderHeight / 2, 8, sliderHeight);

			// draw slide thumb
			g.FillRectangle(rect, IsHover ? SolidColor.LimeGreen : SolidColor.LightGreen);
		}

		public override bool OnMouseDown(CellMouseEventArgs e)
		{
			IsPressed = true;

			UpdateValueByCursorX(e.CellPosition, e.RelativePosition.X);

			// return true to notify control that the mouse-down operation has been hanlded.
			// all operations after this will be aborted.
			return true;
		}

		public override bool OnMouseMove(CellMouseEventArgs e)
		{
			if (IsPressed)
			{
				// requires the left button
				if (e.Buttons == unvell.ReoGrid.Interaction.MouseButtons.Left)
				{
					UpdateValueByCursorX(e.CellPosition, e.RelativePosition.X);
				}
			}

			return false;
		}

		public override bool OnMouseUp(CellMouseEventArgs e)
		{
			IsPressed = false;

			return base.OnMouseUp(e);
		}

		private void UpdateValueByCursorX(CellPosition cellPos, double x)
		{
			// calcutate value by cursor position
			double value = x / (Bounds.Width - 2d);

			if (value < 0) value = 0;
			if (value > 1) value = 1;

			Worksheet.SetCellData(cellPos, value);

			if (this.ValueChanged != null)
			{
				this.ValueChanged(this, null);
			}
		}

		public event EventHandler ValueChanged;

		public override bool OnMouseEnter(CellMouseEventArgs e)
		{
			IsHover = true;
			return true;
		}

		public override bool OnMouseLeave(CellMouseEventArgs e)
		{
			IsHover = false;
			return true;
		}

		public override bool OnStartEdit()
		{
			// disable editing on this cell
			return false;
		}
	}

}
