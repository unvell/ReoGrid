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
 * Copyright (c) 2012-2023 Jingwood <jingwood at unvell.com>
 * Copyright (c) 2012-2023 unvell inc. All rights reserved.
 * 
 ****************************************************************************/

#if AVALONIA


using Avalonia.Input;
using System;
using unvell.ReoGrid.Interaction;

namespace unvell.ReoGrid.AvaloniaPlatform
{
    internal static class AvaloniaUtility
    {

        public static MouseButtons ConvertToUIMouseButtons(MouseButton e)
        {
            MouseButtons btn = MouseButtons.None;
            switch (e)
            {
                case MouseButton.None:
                    break;
                case MouseButton.Left:
                    btn |= MouseButtons.Left;
                    break;
                case MouseButton.Right:
                    btn |= MouseButtons.Right;
                    break;
                case MouseButton.Middle:
                    btn |= MouseButtons.Middle;
                    break;
                case MouseButton.XButton1:
                    break;
                case MouseButton.XButton2:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return btn;
        }
        public static MouseButtons ConvertToUIMouseButtons(PointerPointProperties e)
        {
            MouseButtons btn = MouseButtons.None;
            if (e.IsLeftButtonPressed)
                btn |= MouseButtons.Left;
            else if (e.IsRightButtonPressed)
                btn |= MouseButtons.Right;
            else if (e.IsMiddleButtonPressed)
                btn |= MouseButtons.Middle;
            return btn;
        }
        public static MouseButtons ConvertToUIMouseButtons(PointerPressedEventArgs e)
        {
            var btn = MouseButtons.None;
            if (e.Pointer.Type == PointerType.Mouse)
            {
                var properties = e.GetCurrentPoint(null).Properties;
                if (properties.IsLeftButtonPressed)
                {
                    btn |= MouseButtons.Left;
                }
                else if (properties.IsRightButtonPressed)
                {
                    btn |= MouseButtons.Right;
                }
                else if (properties.IsMiddleButtonPressed)
                {
                    btn |= MouseButtons.Middle;
                }
                else if (properties.IsXButton1Pressed)
                {
                }
                else if (properties.IsMiddleButtonPressed)
                {
                }
            }

            return btn;
        }

    }
}

#endif