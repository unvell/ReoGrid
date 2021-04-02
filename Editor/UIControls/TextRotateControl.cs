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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace unvell.UIControls
{
	internal class TextRotateControl : Control
	{
		/// <summary>
		/// Construct this control.
		/// </summary>
		public TextRotateControl()
		{
			this.DoubleBuffered = true;

			//this.Font = new Font(this.Font.FontFamily, this.Font.Size + 4f); 
			this.SmallChange = 5;
		}

		private int angle;

		/// <summary>
		/// Get or set the angle value.
		/// </summary>
		[DefaultValue(0)]
		[Description("Get or set the angle value")]
		public int Angle
		{
			get { return this.angle; }
			set
			{
				if (this.angle != value)
				{
					this.angle = value;

					if (this.angle < -90) this.angle = -90;
					if (this.angle > 90) this.angle = 90;

					this.Invalidate();
				}
			}
		}

		private string sampleText = "Text";

		/// <summary>
		/// Get or set the sample text.
		/// </summary>
		[Description("Get or set the sample text. Default is 'Text'.")]
		public string SampleText
		{
			get { return this.sampleText; }
			set
			{
				if (this.sampleText != value)
				{
					this.sampleText = value;
					Invalidate();
				}
			}
		}

		public int SmallChange { get; set; }

		/// <summary>
		/// Repaint control.
		/// </summary>
		/// <param name="e">Event argument.</param>
		protected override void OnPaint(PaintEventArgs e)
		{
			var g = e.Graphics;

			float cx = this.ClientRectangle.Left + 10;
			float cy = this.ClientRectangle.Height / 2;

			float len = cy - 10;
			g.TranslateTransform(cx, cy);

			var pb = SystemBrushes.WindowText;

			for (float a = 0; a <= 180f; a += 15f)
			{
				var d = a * Math.PI / 180f;

				int x = (int)Math.Round(Math.Sin(d) * len);
				int y = (int)Math.Round(Math.Cos(d) * len);

				if ((a % 45) == 0)
				{
					g.FillPolygon(pb, new Point[] {
						new Point(x - 3, y), new Point(x, y - 4),
						new Point(x + 4, y), new Point(x, y + 4),
					});
				}
				else
				{
					g.FillRectangle(pb, new Rectangle(x-1, y-1, 2, 2));
				}
			}

			g.RotateTransform(-this.angle);

			SizeF textSize = new SizeF(0, 0);

			using (var sf = new StringFormat(StringFormat.GenericTypographic))
			{
				sf.FormatFlags |= StringFormatFlags.NoWrap;

				textSize = g.MeasureString("Text", this.Font, 999999, sf);
				g.DrawString("Text", this.Font, SystemBrushes.WindowText, 0, -textSize.Height / 2, sf);
			}

			g.DrawLine(SystemPens.WindowText, textSize.Width + 5, 0, len - 7, 0);

			g.RotateTransform(this.angle);

			g.TranslateTransform(-cx, -cy);

			// border
			ControlPaint.DrawBorder3D(g, this.ClientRectangle);
		}

		private bool showBorder = false;

		/// <summary>
		/// Get or set the border of control
		/// </summary>
		[DefaultValue(false)]
		[Description("Determines whether or not to show border")]
		public bool ShowBorder
		{
			get { return this.showBorder; }
			set
			{
				this.showBorder = value;
				Invalidate();
			}
		}

		protected override void OnResize(EventArgs e)
		{
			Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			UpdateAngleByPoint(e.Location);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left
				|| e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				UpdateAngleByPoint(e.Location);
			}
		}

		void UpdateAngleByPoint(Point p)
		{
			float cx = this.ClientRectangle.Left+10;
			float cy = this.ClientRectangle.Height / 2;

			var angle = (float)(Math.Atan2(p.Y - cy, p.X - cx) * 180f / Math.PI);

			if (this.SmallChange > 0)
			{
				var halfSmallChange = this.SmallChange / 2;
				var m = angle % this.SmallChange;

				if (m > halfSmallChange) angle += this.SmallChange - m;
				else if (m < halfSmallChange) angle -= m;
			}

			angle = (float)Math.Round(-angle);

			if (angle < -90) angle = -90;
			if (angle > 90) angle = 90;

			if (this.Angle != angle)
			{
				this.Angle = (int)angle;

				if (this.AngleChanged != null)
				{
					this.AngleChanged(this, null);
				}
			}
		}

		public event EventHandler AngleChanged;
	}
}
