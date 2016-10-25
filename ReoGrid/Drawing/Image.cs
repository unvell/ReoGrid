
#if DRAWING

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;

#if WINFORM
using RGImage = System.Drawing.Image;
#elif WPF
using RGImage = System.Windows.Media.ImageSource;
#elif ANDROID
using RGImage = Android.Graphics.Picture;
#endif // WINFORM

namespace unvell.ReoGrid.Drawing
{
	/// <summary>
	/// Represents an image object.
	/// </summary>
	public class ImageObject : DrawingObject//, IExcelDrawingML
	{
		/// <summary>
		/// Get the image instance to be displayed on worksheet.
		/// </summary>
		public RGImage Image { get; protected set; }

		/// <summary>
		/// Construct the image object with specified image instance.
		/// </summary>
		/// <param name="image">Platform image instance to be displayed on worksheet.</param>
		public ImageObject(RGImage image)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}

			this.FillColor = SolidColor.Transparent;
			this.LineColor = SolidColor.Transparent;

			this.Image = image;

			this.Size = new Graphics.Size(image.Width, image.Height);
		}

		/// <summary>
		/// Draw drawing object to graphics context.
		/// </summary>
		/// <param name="dc">Platform no-associated drawing context instance.</param>
		protected override void OnPaint(DrawingContext dc)
		{
			if (this.Image == null)
			{
				base.OnPaint(dc);
			}
			else
			{
				var g = dc.Graphics;

				var clientRect = this.ClientBounds;

				g.DrawImage(this.Image, clientRect);

				if (!this.LineColor.IsTransparent)
				{
					g.DrawRectangle(clientRect, this.LineColor);
				}
			}
		}

		public string GetFriendlyTypeName()
		{
			return "Picture";
		}

		//public INvProperty NvProperty { get; set; }
	}
}

#endif // DRAWING