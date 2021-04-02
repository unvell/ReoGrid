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

#if ANDROID

using Android.Graphics;
using Android.Util;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Interaction;

namespace unvell.ReoGrid
{
	partial class ReoGridCell
	{
		internal Typeface renderFont;
	}
}

namespace unvell.ReoGrid.Rendering
{
	partial class PlatformUtility
	{
		internal static float GetDPI()
		{
			DisplayMetrics metrics = new DisplayMetrics();
			//getResources().getDisplayMetrics();
			return metrics.Xdpi;
		}

		internal static bool IsKeyDown(KeyCode key)
		{
			// TODO
			return false;
		}
	}

	partial class StaticResources
	{
		internal static readonly SolidColor SystemColor_Window = SolidColor.White;
		internal static readonly SolidColor SystemColor_WindowText = SolidColor.Black;
		internal static readonly SolidColor SystemColor_Highlight = SolidColor.SkyBlue;
		internal static readonly SolidColor SystemColor_Control = SolidColor.Silver;
		internal static readonly SolidColor SystemColor_ControlDark = SolidColor.Gray;
	}
}

#endif // ANDROID