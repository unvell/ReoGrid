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
 * Author: Jingwood <jingwood at unvell.com>
 *
 * Copyright (c) 2012-2021 Jingwood, unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;

#if DEBUG
using System.Diagnostics;
#endif

#if EX_SCRIPT
using unvell.ReoScript;
using unvell.ReoGrid.Script;
#endif // EX_SCRIPT

#if WINFORM || ANDROID
using RGFloat = System.Single;
#else
using RGFloat = System.Double;
#endif // WINFORM

using unvell.Common;

#if WINFORM || WPF
using unvell.Common.Win32Lib;
#endif // WINFORM || WPF

using unvell.ReoGrid.Core;
using unvell.ReoGrid.Utility;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Events;
using unvell.ReoGrid.Actions;
using unvell.ReoGrid.Data;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Interaction;

namespace unvell.ReoGrid.Views
{
	interface IRangeSelectableView : IViewport
	{
	}
}
