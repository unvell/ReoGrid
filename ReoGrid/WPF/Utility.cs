#if WPF

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace unvell.ReoGrid.WPF
{
	class Utility
	{
		internal static BitmapSource LoadGDIBitmap(System.Drawing.Bitmap source)
		{
			IntPtr ip = source.GetHbitmap();
			BitmapSource bs = null;

			try
			{
				bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
					 IntPtr.Zero, Int32Rect.Empty,
					 System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
			}
			finally
			{
				unvell.Common.Win32Lib.Win32.DeleteObject(ip);
			}

			return bs;
		}

	}
}

#endif // WPF