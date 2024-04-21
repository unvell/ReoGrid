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
using System.Collections.Generic;
using unvell.ReoGrid.Interaction;

namespace unvell.ReoGrid.AvaloniaPlatform
{
    internal static class AvaloniaUtility
    {
        private static readonly Dictionary<Key, KeyCode> keyMap = new(169)
        {
            { Key.Cancel, KeyCode.Cancel },
            { Key.Back, KeyCode.Back },
            { Key.Tab, KeyCode.Tab },
            { Key.Clear, KeyCode.Clear },
            { Key.Return, KeyCode.Return },
            { Key.Pause, KeyCode.Pause },
            { Key.Capital, KeyCode.Capital },
            { Key.KanaMode, KeyCode.KanaMode },
            { Key.JunjaMode, KeyCode.JunjaMode },
            { Key.FinalMode, KeyCode.FinalMode },
            { Key.HanjaMode, KeyCode.HanjaMode },
            { Key.Escape, KeyCode.Escape },
            { Key.ImeConvert, KeyCode.IMEConvert },
            { Key.ImeNonConvert, KeyCode.IMENonconvert },
            { Key.ImeAccept, KeyCode.IMEAccept },
            { Key.ImeModeChange, KeyCode.IMEModeChange },
            { Key.Space, KeyCode.Space },
            { Key.PageUp, KeyCode.PageUp },
            { Key.PageDown, KeyCode.PageDown },
            { Key.End, KeyCode.End },
            { Key.Home, KeyCode.Home },
            { Key.Left, KeyCode.Left },
            { Key.Up, KeyCode.Up },
            { Key.Right, KeyCode.Right },
            { Key.Down, KeyCode.Down },
            { Key.Select, KeyCode.Select },
            { Key.Print, KeyCode.Print },
            { Key.Execute, KeyCode.Execute },
            { Key.Snapshot, KeyCode.Snapshot },
            { Key.Insert, KeyCode.Insert },
            { Key.Delete, KeyCode.Delete },
            { Key.Help, KeyCode.Help },
            { Key.D0, KeyCode.D0 },
            { Key.D1, KeyCode.D1 },
            { Key.D2, KeyCode.D2 },
            { Key.D3, KeyCode.D3 },
            { Key.D4, KeyCode.D4 },
            { Key.D5, KeyCode.D5 },
            { Key.D6, KeyCode.D6 },
            { Key.D7, KeyCode.D7 },
            { Key.D8, KeyCode.D8 },
            { Key.D9, KeyCode.D9 },
            { Key.A, KeyCode.A },
            { Key.B, KeyCode.B },
            { Key.C, KeyCode.C },
            { Key.D, KeyCode.D },
            { Key.E, KeyCode.E },
            { Key.F, KeyCode.F },
            { Key.G, KeyCode.G },
            { Key.H, KeyCode.H },
            { Key.I, KeyCode.I },
            { Key.J, KeyCode.J },
            { Key.K, KeyCode.K },
            { Key.L, KeyCode.L },
            { Key.M, KeyCode.M },
            { Key.N, KeyCode.N },
            { Key.O, KeyCode.O },
            { Key.P, KeyCode.P },
            { Key.Q, KeyCode.Q },
            { Key.R, KeyCode.R },
            { Key.S, KeyCode.S },
            { Key.T, KeyCode.T },
            { Key.U, KeyCode.U },
            { Key.V, KeyCode.V },
            { Key.W, KeyCode.W },
            { Key.X, KeyCode.X },
            { Key.Y, KeyCode.Y },
            { Key.Z, KeyCode.Z  },
            { Key.LWin, KeyCode.LWin },
            { Key.RWin, KeyCode.RWin },
            { Key.Apps, KeyCode.Apps },
            { Key.Sleep, KeyCode.Sleep },
            { Key.NumPad0, KeyCode.NumPad0 },
            { Key.NumPad1, KeyCode.NumPad1 },
            { Key.NumPad2, KeyCode.NumPad2 },
            { Key.NumPad3, KeyCode.NumPad3 },
            { Key.NumPad4, KeyCode.NumPad4 },
            { Key.NumPad5, KeyCode.NumPad5 },
            { Key.NumPad6, KeyCode.NumPad6 },
            { Key.NumPad7, KeyCode.NumPad7 },
            { Key.NumPad8, KeyCode.NumPad8 },
            { Key.NumPad9, KeyCode.NumPad9 },
            { Key.Multiply, KeyCode.Multiply },
            { Key.Add, KeyCode.Add },
            { Key.Separator, KeyCode.Separator },
            { Key.Subtract, KeyCode.Subtract },
            { Key.Decimal, KeyCode.Decimal },
            { Key.Divide, KeyCode.Divide },
            { Key.F1, KeyCode.F1 },
            { Key.F2, KeyCode.F2 },
            { Key.F3, KeyCode.F3 },
            { Key.F4, KeyCode.F4 },
            { Key.F5, KeyCode.F5 },
            { Key.F6, KeyCode.F6 },
            { Key.F7, KeyCode.F7 },
            { Key.F8, KeyCode.F8 },
            { Key.F9, KeyCode.F9 },
            { Key.F10, KeyCode.F10 },
            { Key.F11, KeyCode.F11 },
            { Key.F12, KeyCode.F12 },
            { Key.F13, KeyCode.F13 },
            { Key.F14, KeyCode.F14 },
            { Key.F15, KeyCode.F15 },
            { Key.F16, KeyCode.F16 },
            { Key.F17, KeyCode.F17 },
            { Key.F18, KeyCode.F18 },
            { Key.F19, KeyCode.F19 },
            { Key.F20, KeyCode.F20 },
            { Key.F21, KeyCode.F21 },
            { Key.F22, KeyCode.F22 },
            { Key.F23, KeyCode.F23 },
            { Key.F24, KeyCode.F24 },
            { Key.NumLock, KeyCode.NumLock },
            { Key.Scroll, KeyCode.Scroll },
            { Key.LeftShift, KeyCode.LShiftKey },
            { Key.RightShift, KeyCode.RShiftKey },
            { Key.LeftCtrl, KeyCode.LControlKey },
            { Key.RightCtrl, KeyCode.RControlKey },
            { Key.LeftAlt, KeyCode.Alt },
            { Key.RightAlt, KeyCode.Alt },
            { Key.BrowserBack, KeyCode.BrowserBack },
            { Key.BrowserForward, KeyCode.BrowserForward },
            { Key.BrowserRefresh, KeyCode.BrowserRefresh },
            { Key.BrowserStop, KeyCode.BrowserStop },
            { Key.BrowserSearch, KeyCode.BrowserSearch },
            { Key.BrowserFavorites, KeyCode.BrowserFavorites },
            { Key.BrowserHome, KeyCode.BrowserHome },
            { Key.VolumeMute, KeyCode.VolumeMute },
            { Key.VolumeDown, KeyCode.VolumeDown },
            { Key.VolumeUp, KeyCode.VolumeUp },
            { Key.MediaNextTrack, KeyCode.MediaNextTrack },
            { Key.MediaPreviousTrack, KeyCode.MediaPreviousTrack },
            { Key.MediaStop, KeyCode.MediaStop },
            { Key.MediaPlayPause, KeyCode.MediaPlayPause },
            { Key.LaunchMail, KeyCode.LaunchMail },
            { Key.SelectMedia, KeyCode.SelectMedia },
            { Key.LaunchApplication1, KeyCode.LaunchApplication1 },
            { Key.LaunchApplication2, KeyCode.LaunchApplication2 },
            { Key.Oem1, KeyCode.Oem1 },
            { Key.OemPlus, KeyCode.Oemplus },
            { Key.OemComma, KeyCode.Oemcomma },
            { Key.OemMinus, KeyCode.OemMinus },
            { Key.OemPeriod, KeyCode.OemPeriod },
            { Key.OemQuestion, KeyCode.OemQuestion },
            { Key.Oem3, KeyCode.Oem3 },
            { Key.OemOpenBrackets, KeyCode.OemOpenBrackets },
            { Key.Oem5, KeyCode.Oem5 },
            { Key.Oem6, KeyCode.Oem6 },
            { Key.OemQuotes, KeyCode.OemQuotes },
            { Key.Oem8, KeyCode.Oem8 },
            { Key.OemBackslash, KeyCode.OemBackslash },
            { Key.EraseEof, KeyCode.EraseEof },
            { Key.Play, KeyCode.Play },
            { Key.NoName, KeyCode.NoName },
            { Key.Pa1, KeyCode.Pa1 },
            { Key.OemClear, KeyCode.OemClear }
        };

        public static KeyCode GetKeyCode(Key key)
        {
            keyMap.TryGetValue(key, out var result);
            return result;
        }

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