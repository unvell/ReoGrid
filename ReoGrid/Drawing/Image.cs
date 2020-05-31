
#if DRAWING

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Interaction;
using unvell.ReoGrid.Utility;

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

        #region //---------custom-----------
        public override bool OnMouseDown(Point location, MouseButtons buttons)
        {
            if (IsLocked || !Visible) return false;
            SetFocus();
            movestart = location;
            movestartW = location;
            var resizept = this.GetResizeCursorByPoint(location);
            if (resizept.Cusror != CursorStyle.PlatformDefault)
            {
                var wdc = (WorksheetDrawingCanvas)this.Container;
                wdc.Worksheet.controlAdapter.ChangeCursor(resizept.Cusror);
                IsResize = true;
                resizedir = resizept.Dir;
            }
            return base.OnMouseDown(location, buttons);
        }

        public override bool OnMouseMove(Point location, MouseButtons buttons)
        {
            if (IsLocked || !Visible) return false;
            bool isProcessing = false;
            if (this.IsSelected)
            {
                if (IsResize)
                {
                    if (buttons == MouseButtons.Left)
                    {
                        this.StartResize(location, resizedir);
                    }
                }
                else
                {
                    var wdc = (WorksheetDrawingCanvas)this.Container;
                    var resizept = this.GetResizeCursorByPoint(location);
                    if (resizept.Cusror != CursorStyle.PlatformDefault)
                        wdc.Worksheet.controlAdapter.ChangeCursor(resizept.Cusror);
                    else
                        wdc.Worksheet.controlAdapter.ChangeCursor(CursorStyle.Move);
                    if (buttons == MouseButtons.Left)
                    {
                        this.StartDrag(new Point(location.X, location.Y));
                    }
                }
            }
            return isProcessing;
        }

        public override bool OnMouseUp(Point location, MouseButtons buttons)
        {
            if (IsLocked || !Visible) return false;
            IsResize = false;
            return base.OnMouseUp(location, buttons);
        }

        //reccord last mouse click position in case of mouse out of bounds
        private Point movestart, movestartW;
        public bool IsResize = false;
        public ResizeThumbPosition? resizedir = null;
        private const float MaxPicSize = 1024;
        private const float MinPicSize = 32;
        public void StartResize(Point pos, ResizeThumbPosition? dir)
        {
            if (dir == null) return;

            var offset = new Point(pos.X - movestart.X, pos.Y - movestart.Y);
            var offsetW = new Point(pos.X - movestartW.X, pos.Y - movestartW.Y);
            switch (dir)
            {
                case ResizeThumbPosition.Top:
                    this.Y += offset.Y;
                    this.Height = (this.Height - offset.Y).Clamp(MinPicSize, MaxPicSize);
                    break;
                case ResizeThumbPosition.Bottom:
                    this.Height = (this.Height + offsetW.Y).Clamp(MinPicSize, MaxPicSize);
                    break;
                case ResizeThumbPosition.Left:
                    this.X += offset.X;
                    this.Width = (this.Width - offset.X).Clamp(MinPicSize, MaxPicSize);
                    break;
                case ResizeThumbPosition.Right:
                    this.Width = (this.Width + offsetW.X).Clamp(MinPicSize, MaxPicSize);
                    break;
                case ResizeThumbPosition.TopLeft:
                    this.Y += offset.Y;
                    this.Height = (this.Height - offset.Y).Clamp(MinPicSize, MaxPicSize);
                    this.X += offset.X;
                    this.Width = (this.Width - offset.X).Clamp(MinPicSize, MaxPicSize);
                    break;
                case ResizeThumbPosition.TopRight:
                    this.Y += offset.Y;
                    this.Height = (this.Height - offset.Y).Clamp(MinPicSize, MaxPicSize);
                    this.Width = (this.Width + offsetW.X).Clamp(MinPicSize, MaxPicSize);
                    break;
                case ResizeThumbPosition.BottomLeft:
                    this.X += offset.X;
                    this.Width = (this.Width - offset.X).Clamp(MinPicSize, MaxPicSize);
                    this.Height = (this.Height + offsetW.Y).Clamp(MinPicSize, MaxPicSize);
                    break;
                case ResizeThumbPosition.BottomRight:
                    this.Width = (this.Width + offsetW.X).Clamp(MinPicSize, MaxPicSize);
                    this.Height = (this.Height + offsetW.Y).Clamp(MinPicSize, MaxPicSize);
                    break;
            }
            movestartW = pos;
            movestart.X = 0;
            movestart.Y = 0;
        }

        public void StartDrag(Point pos)
        {
            var offset = new Point(pos.X - movestart.X, pos.Y - movestart.Y);
            this.Location = new Point(this.Location.X + offset.X, this.Location.Y + offset.Y);
        }

        public override void FreeFocus()
        {
            this.IsSelected = false;
        }
        public override void SetFocus()
        {
            this.IsSelected = true;
        }

        public bool IsLocked
        {
            get { return _islocked; }
            set
            {
                if (_islocked != value)
                {
                    _islocked = value;
                    if (_islocked)
                    {
                        this.FreeFocus();
                    }
                }
            }
        }
        private bool _islocked;

        #endregion //---------custom-----------

        public string GetFriendlyTypeName()
		{
			return "Picture";
		}

		//public INvProperty NvProperty { get; set; }
	}
}

#endif // DRAWING