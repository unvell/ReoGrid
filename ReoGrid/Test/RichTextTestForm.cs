#if DEBUG && WINFORM

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace unvell.ReoGrid.Test
{
	public partial class RichTextTestForm : Form
	{
		private Drawing.RichText rt;

		public RichTextTestForm()
		{
			InitializeComponent();

			this.DoubleBuffered = true;

			//rt = new Drawing.RichText()
			//	.Bold("Mary Celeste").Regular(" was an American merchant brigantine.")
			//	.NewLine()
			//	.Regular("The ").Span("final entry", textColor: Color.Blue)
			//	.Span(" in the log, ")
			//	.Span("dated ten days", fontSize: 40f)
			//	.Span(" a previously, was a ", fontSize: 10.25f, textColor: Color.Black)
			//	.Span("日本語テスト", textColor: Color.LightBlue)
			//	.Span("汉字测试，这是一本《难得的过》", textColor: Color.DarkGreen)
			//	.Span(" routine statement", textColor: Color.DarkRed)
			//	.Span(" of the ship's position.", textColor: Color.Black)
			//	.Span(" admin", textColor: Color.Red)
			//	.Span("istrator", textColor: Color.HotPink)
			//	;
			rt = new Drawing.RichText()
				.Span("aaaaa bbbbb ccccc ddddd", fontSize: 20f)
				.Span(" bbbbb eeeee fffff ggggg").NewLine()
				.Span(" ccccc ddddd hhhhh iiiii")
				.Span(" ddddd")
			//	;
			//rt = new Drawing.RichText()
				.Span("ABC", fontSize: 40)
				;

			rt.TextWrap = TextWrapMode.WordBreak;

			rt.RotationAngle = 0;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			rt.Size = this.ClientRectangle.Size;
		}

		private WinForm.GDIGraphics ig;

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (ig == null)
			{
				ig = new WinForm.GDIGraphics(e.Graphics);
			}
			else
			{
				ig.PlatformGraphics = e.Graphics;
			}

      rt.Draw(ig, ClientRectangle);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			if (rt != null)
			{
				rt.Size = this.ClientRectangle.Size;

				Invalidate();
			}
		}
	}
}

#endif // DEBUG && WINFORM