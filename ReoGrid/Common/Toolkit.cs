/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net/
 *
 * Toolkit - Common Utility Library
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

#if WINFORM || WPF
using unvell.Common.Win32Lib;
#endif // WINFORM || WPF

using System;
using System.Security.Cryptography;
using System.Text;

namespace unvell.Common
{
	/// <summary>
	/// Common Toolkit
	/// </summary>
	public static class Toolkit
	{
#if WINFORM || WPF
		/// <summary>
		/// Check whether or not the specified key is pressed.
		/// </summary>
		/// <param name="vkey">Windows virtual key.</param>
		/// <returns>true if pressed, otherwise false if not pressed.</returns>
		public static bool IsKeyDown(Win32.VKey vkey)
		{
			return ((Win32.GetKeyState(vkey) >> 15) & 1) == 1;
		}
#endif // WINFORM || WPF

		private static MD5 md5 = null;

		internal static byte[] GetMD5Hash(string str)
		{
			if (md5 == null)
			{
				md5 = MD5.Create();
			}

			return md5.ComputeHash(Encoding.Default.GetBytes(str));
		}

		internal static string GetHexString(byte[] data)
		{
			return Convert.ToBase64String(data);
		}

		internal static string GetMD5HashedString(string str)
		{
			return GetHexString(GetMD5Hash(str));
		}

		/// <summary>
		/// Default font size list.
		/// </summary>
		public static readonly float[] FontSizeList = new float[] {
            5f, 6f, 7f, 8f, 9f, 10f, 10.5f, 11f, 11.5f, 12f, 12.5f, 14f, 16f, 18f,
            20f, 22f, 24f, 26f, 28f, 30f, 32f, 34f, 38f, 46f, 58f, 64f, 78f, 92f};

		internal static double Ceiling(double val, double scale)
		{
			double m = val % scale;
			if (m == 0) return val;

			return val - m + scale;
		}

	}
}
