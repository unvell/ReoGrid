#if PRINT

using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace unvell.ReoGrid.Utility
{
#if DEBUG
	public 
#else
		internal
#endif // DEBUG
		class PrinterUtility
	{
			public static byte[] GetDevModeData(PrinterSettings settings)
			{
				//Contract.Requires(settings != null);

				byte[] devModeData;
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
					// cer since hDevMode is not a SafeHandle
				}
				finally
				{
					var hDevMode = settings.GetHdevmode();
					try
					{
						IntPtr pDevMode = NativeMethods.GlobalLock(hDevMode);
						try
						{
							var devMode = (NativeMethods.DEVMODE)Marshal.PtrToStructure(
									pDevMode, typeof(NativeMethods.DEVMODE));

							var devModeSize = devMode.dmSize + devMode.dmDriverExtra;
							devModeData = new byte[devModeSize];
							Marshal.Copy(pDevMode, devModeData, 0, devModeSize);
						}
						finally
						{
							NativeMethods.GlobalUnlock(hDevMode);
						}
					}
					finally
					{
						Marshal.FreeHGlobal(hDevMode);
					}
				}
				return devModeData;
			}

			public static void SetDevModeData(PrinterSettings settings, byte[] data)
			{
				//Contract.Requires(settings != null);
				//Contract.Requires(data != null);
				//Contract.Requires(data.Length >= Marshal.SizeOf(typeof(NativeMethods.DEVMODE)));

				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
					// cer since AllocHGlobal does not return SafeHandle
				}
				finally
				{
					var pDevMode = Marshal.AllocHGlobal(data.Length);

					try
					{
						// we don't have to worry about GlobalLock since AllocHGlobal only uses LMEM_FIXED
						Marshal.Copy(data, 0, pDevMode, data.Length);
						var devMode = (NativeMethods.DEVMODE)Marshal.PtrToStructure(
										pDevMode, typeof(NativeMethods.DEVMODE));

						// The printer name must match the original printer, otherwise an AV will be thrown
						settings.PrinterName = devMode.dmDeviceName;

						// SetHDevmode creates a copy of the devmode, so we don't have to keep ours around
						settings.SetHdevmode(pDevMode);
					}
					finally
					{
						Marshal.FreeHGlobal(pDevMode);
					}
				}
			}
		}

	static class NativeMethods
	{
		private const string Kernel32 = "kernel32.dll";

		[DllImport(Kernel32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
		public static extern IntPtr GlobalLock(IntPtr handle);

		[DllImport(Kernel32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
		public static extern bool GlobalUnlock(IntPtr handle);

		[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Auto)]
		public struct DEVMODE
		{
			private const int CCHDEVICENAME = 32;
			private const int CCHFORMNAME = 32;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
			public string dmDeviceName;
			public short dmSpecVersion;
			public short dmDriverVersion;
			public short dmSize;
			public short dmDriverExtra;
			public int dmFields;

			public int dmPositionX;
			public int dmPositionY;
			public int dmDisplayOrientation;
			public int dmDisplayFixedOutput;

			public short dmColor;
			public short dmDuplex;
			public short dmYResolution;
			public short dmTTOption;
			public short dmCollate;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
			public string dmFormName;
			public short dmLogPixels;
			public int dmBitsPerPel;
			public int dmPelsWidth;
			public int dmPelsHeight;
			public int dmDisplayFlags;
			public int dmDisplayFrequency;
			public int dmICMMethod;
			public int dmICMIntent;
			public int dmMediaType;
			public int dmDitherType;
			public int dmReserved1;
			public int dmReserved2;
			public int dmPanningWidth;
			public int dmPanningHeight;
		}
	}
}

#endif // PRINT