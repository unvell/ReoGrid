using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if WINFORM || ANDROID
using RGFloat = System.Single;
#elif WPF
using RGFloat = System.Double;
#elif iOS
using RGFloat = System.Double;
#endif // WPF

namespace unvell.ReoGrid.Utility
{
	internal static class MeasureToolkit
	{
		// 1 inch = 2.54 cm
		private const RGFloat _cm_pre_inch = 2.54f;
		private const RGFloat _windows_standard_dpi = 96f;

		public static RGFloat InchToPixel(RGFloat inch, RGFloat dpi)
		{
			return inch * dpi;
		}
		public static RGFloat PixelToInch(RGFloat px, RGFloat dpi)
		{
			return px / dpi;
		}

		public static RGFloat InchToPixel(RGFloat inch)
		{
			return InchToPixel(inch, _windows_standard_dpi);
		}
		public static RGFloat PixelToInch(RGFloat px)
		{
			return PixelToInch(px, _windows_standard_dpi);
		}

		public static RGFloat InchToCM(RGFloat inch)
		{
			return inch * _cm_pre_inch;
		}
		public static RGFloat CMToInch(RGFloat cm)
		{
			return cm / _cm_pre_inch;
		}

		public static RGFloat CMToPixel(RGFloat cm, RGFloat dpi)
		{
			return cm * dpi / _cm_pre_inch;
		}
		public static RGFloat PixelToCM(RGFloat px, RGFloat dpi)
		{
			return px * _cm_pre_inch / dpi;
		}

		public static RGFloat CMTOPixel(RGFloat cm)
		{
			return CMToPixel(cm, _windows_standard_dpi);
		}
		public static RGFloat PixelToCM(RGFloat px)
		{
			return PixelToCM(px, _windows_standard_dpi);
		}

		public const int _emi_in_inch = 914400;

		public static RGFloat EMUToPixel(int emu, RGFloat dpi)
		{
			return (RGFloat)emu / _emi_in_inch * dpi;
		}

		public static int PixelToEMU(RGFloat pixel, RGFloat dpi)
		{
			return (int)(pixel * _emi_in_inch / dpi);
		}
	}

}
