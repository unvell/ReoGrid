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
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

#if iOS

using CoreGraphics;

using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Interaction;

namespace unvell.ReoGrid
{
	partial class ReoGridCell
	{
		internal CoreGraphics.CGFont renderFont;
	}
}

namespace unvell.ReoGrid.Rendering
{
	partial class PlatformUtility
	{
		internal static float GetDPI()
		{
			return 72.0f;
		}

		internal static bool IsKeyDown(KeyCode key)
		{
			// Not Supported
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