/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net/
 * 
 * Win32 API Entry Library
 * 
 * - Provides the interfaces to invoke Windows Platform API 
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


// Disable XML comment document
#pragma warning disable 1591

#if WINFORM || WPF

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

#if WINFORM
using System.Windows.Forms;
#endif // WINFORM

namespace unvell.Common.Win32Lib
{
	public static class Win32
	{
#region Windows Message

		/// <summary>
		/// Virtual Messages
		/// </summary>
		public enum WMessages : uint
		{
			WM_NULL = 0x0000,
			WM_CREATE = 0x0001,
			WM_DESTROY = 0x0002,
			WM_MOVE = 0x0003,
			WM_SIZE = 0x0005,
			WM_ACTIVATE = 0x0006,
			WM_SETFOCUS = 0x0007,
			WM_KILLFOCUS = 0x0008,
			WM_ENABLE = 0x000A,
			WM_SETTEXT = 0x000C,
			WM_GETTEXT = 0x000D,
			WM_GETTEXTLENGTH = 0x000E,
			WM_PAINT = 0x000F,
			WM_CLOSE = 0x0010,
			WM_QUERYENDSESSION = 0x0011,
			WM_QUERYOPEN = 0x0013,
			WM_ERASEBKGND = 0x0014,
			WM_ENDSESSION = 0x0016,
			WM_COPYDATA = 0x004A,

			WM_HSCROLL = 0x0114,
			WM_VSCROLL = 0x0115,

			WM_CONTEXTMENU = 0x007B,

			WM_KEYDOWN = 0x100,  //Key down
			WM_KEYUP = 0x101,   //Key up
			WM_MOUSEMOVE = 0x0200,   //Mouse move
			WM_CHAR = 0x0102,

			WM_LBUTTONDOWN = 0x201, //Left mousebutton down
			WM_LBUTTONUP = 0x202,  //Left mousebutton up
			WM_LBUTTONDBLCLK = 0x203, //Left mousebutton doubleclick
			WM_RBUTTONDOWN = 0x204, //Right mousebutton down
			WM_RBUTTONUP = 0x205,   //Right mousebutton up
			WM_RBUTTONDBLCLK = 0x206, //Right mousebutton doubleclick

			WM_MOUSEHWHEEL = 0x020E,
#region IME
			WM_IME_SETCONTEXT = 0x0281,
			WM_IME_NOTIFY = 0x0282,
			WM_IME_CONTROL = 0x0283,
			WM_IME_COMPOSITIONFULL = 0x0284,
			WM_IME_SELECT = 0x0285,
			WM_IME_CHAR = 0x0286,
			WM_IME_REQUEST = 0x0288,
			WM_IME_KEYDOWN = 0x0290,
			WM_IME_KEYUP = 0x0291,
			WM_IME_STARTCOMPOSITION = 0x010D,
			WM_IME_ENDCOMPOSITION = 0x010E,
			WM_IME_COMPOSITION = 0x010F,
			WM_IME_KEYLAST = 0x010F,
#endregion

			WM_HOTKEY = 0x0312,

#region Clipboard
			WM_DRAWCLIPBOARD = 0x308,
			WM_CHANGECBCHAIN = 0x030D,
#endregion

			WM_USER = 0x0400,
		}

		public enum EndSessionParam : long
		{
			ENDSESSION_CLOSEAPP = 0x00000001,
			ENDSESSION_CRITICAL = 0x40000000,
			ENDSESSION_LOGOFF = 0x80000000,
		}

		public struct COPYDATASTRUCT
		{
			public IntPtr dwData;
			public int cbData;

			[MarshalAs(UnmanagedType.LPStr)]
			public string lpData;
		}

		[DllImport("USER32.DLL")]
		public static extern int SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("USER32.DLL")]
		public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("USER32.DLL")]
		public static extern long MAKELPARAM(int wLow, int wHigh);

#endregion

#region Window Utility
		// Get a handle to an application window.
		[DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll")]
		public static extern bool EnumWindows(IntPtr lpEnumFunc, long lParam);

		[DllImport("user32.dll")]
		public static extern bool EnumDesktopWindows(IntPtr hWnd, EnumWindowsProc proc, IntPtr lParam);
		public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr param);

		[DllImport("user32.dll")]
		public static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder str, int count);

		[DllImport("user32.dll")]
		public static extern bool BringWindowToTop(IntPtr hwnd);

		[DllImport("user32.dll")]
		public static extern bool IsWindowVisible(IntPtr hwnd);

		[DllImport("user32.dll")]
		public extern static long SetWindowLong(IntPtr hwnd, int index, long value);

		[DllImport("user32.dll")]
		public extern static long GetWindowLong(IntPtr hwnd, int nIndex);

		[DllImport("user32.dll")]
		public static extern int SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern int IsIconic(IntPtr hWnd);

		[DllImport("USER32.DLL")]
		public static extern IntPtr GetDesktopWindow();

		[DllImport("user32.dll")]
		public extern static bool InvalidateRect(
			IntPtr hWnd,           // handle to window
			Rectangle lpRect,  // rectangle coordinates
			bool bErase          // erase state
		);

		[DllImport("USER32.DLL")]
		public static extern IntPtr WindowFromPoint(System.Drawing.Point point);

		/// <summary>
		/// Retrieves a handle to the specified window's parent or owner.
		/// To retrieve a handle to a specified ancestor, use the GetAncestor function.
		/// </summary>
		/// <param name="hwnd">A handle to the window whose parent window handle is to be retrieved.</param>
		/// <returns>If the window is a child window, the return value is a handle to the parent window. 
		/// If the window is a top-level window with the WS_POPUP style, the return value is a handle to the owner window.
		/// If the function fails, the return value is NULL. To get extended error information, call GetLastError.
		/// This function typically fails for one of the following reasons:
		/// <ul><li>The window is a top-level window that is unowned or does not have the WS_POPUP style.</li>
		///	<li>The owner window has WS_POPUP style.</li></ul></returns>
		/// <seealso cref="GetAncestor"/>
		[DllImport("user32.dll")]
		public static extern IntPtr GetParent(IntPtr hwnd);

		/// <summary>
		/// Retrieves the handle to the ancestor of the specified window.
		/// </summary>
		/// <param name="hwnd">A handle to the window whose ancestor is to be retrieved. 
		/// If this parameter is the desktop window, the function returns NULL.</param>
		/// <param name="gaFlags">The ancestor to be retrieved. This parameter can be one of the GAFlag enum.</param>
		/// <returns></returns>
		[DllImport("user32.dll")]
		public static extern IntPtr GetAncestor(IntPtr hwnd, uint gaFlags);

		/// <summary>
		/// Retrieves a handle to a window that has the specified relationship (Z-Order or owner) to the specified window.
		/// </summary>
		/// <param name="hwnd">A handle to a window.
		/// The window handle retrieved is relative to this window, based on the value of the uCmd parameter.</param>
		/// <param name="wCMD">The relationship between the specified window and the window whose handle is to be retrieved.
		/// This parameter can be one of the following values.</param>
		/// <returns></returns>
		[DllImport("user32.dll")]
		public static extern IntPtr GetWindow(IntPtr hwnd, uint wCMD);

		public enum WindowStyle : long
		{
			GWL_EXSTYLE = -20,
			WS_EX_TRANSPARENT = 0x20,
		}

		public enum ExtendedWindowStyles : long
		{
			WS_EX_NOACTIVATE = 0x08000000L,
			WS_EX_TOOLWINDOW = 0x00000080L,
			WS_EX_TRANSPARENT = 0x20,
		}

		public enum ShowWindowCmd : int
		{
			/// <summary>
			/// Hides the window and activates another window.
			/// </summary>
			SW_HIDE = 0,

			/// <summary>
			/// Maximizes the specified window.
			/// </summary>
			SW_MAXIMIZE = 3,

			/// <summary>
			/// Minimizes the specified window and activates the next top-level window in the z-order.
			/// </summary>
			SW_MINIMIZE = 6,

			/// <summary>
			/// Activates and displays the window. If the window is minimized or maximized, the system restores it to its original size and position.
			/// An application should specify this flag when restoring a minimized window.
			/// </summary>
			SW_RESTORE = 9,

			/// <summary>
			/// Activates the window and displays it in its current size and position.
			/// </summary>
			SW_SHOW = 5,

			/// <summary>
			/// Activates the window and displays it as a maximized window.
			/// </summary>
			SW_SHOWMAXIMIZED = 3,

			/// <summary>
			/// Activates the window and displays it as a minimized window.
			/// </summary>
			SW_SHOWMINIMIZED = 2,

			/// <summary>
			/// Displays the window as a minimized window.
			/// This value is similar to SW_SHOWMINIMIZED, except the window is not activated.
			/// </summary>
			SW_SHOWMINNOACTIVE = 7,

			/// <summary>
			/// Displays the window in its current size and position.
			/// This value is similar to SW_SHOW, except the window is not activated.
			/// </summary>
			SW_SHOWNA = 8,

			/// <summary>
			/// Displays a window in its most recent size and position.
			/// This value is similar to SW_SHOWNORMAL, except the window is not activated.
			/// </summary>
			SW_SHOWNOACTIVATE = 4,

			/// <summary>
			/// Activates and displays a window. If the window is minimized or maximized, 
			/// the system restores it to its original size and position. 
			/// An application should specify this flag when displaying the window for the first time.
			/// </summary>
			SW_SHOWNORMAL = 1,
		}

		/*
		 * Scroll Bar Commands
		 */
		public enum ScrollBar : int
		{
			SB_LINEUP = 0,
			SB_LINELEFT = 0,
			SB_LINEDOWN = 1,
			SB_LINERIGHT = 1,
			SB_PAGEUP = 2,
			SB_PAGELEFT = 2,
			SB_PAGEDOWN = 3,
			SB_PAGERIGHT = 3,
			SB_THUMBPOSITION = 4,
			SB_THUMBTRACK = 5,
			SB_TOP = 6,
			SB_LEFT = 6,
			SB_BOTTOM = 7,
			SB_RIGHT = 7,
			SB_ENDSCROLL = 8,
		}

		[DllImport("user32.dll")]
		public static extern int GetScrollPos(IntPtr hWnd, int nBar);

		[DllImport("user32.dll")]
		public static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool Redraw);

		[DllImport("user32.dll", SetLastError = false)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInstertAfter, int x, int y, int cx, int cy, uint flags);

		public enum SWP : uint
		{
			SWP_NOSIZE = 0x0001,
			SWP_NOZORDER = 0x0004,
		}

		[DllImport("user32.dll")]
		public static extern IntPtr SetFocus(IntPtr hwnd);

		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(IntPtr hWnd, ref Rectangle lpRect);

		/// <summary>
		/// Retrieves the show state and the restored, minimized, and maximized positions of the specified window.
		/// </summary>
		/// <param name="hWnd">A handle to the window.</param>
		/// <param name="lpwndpl">A pointer to the WINDOWPLACEMENT structure that receives the show state and position information. 
		/// Before calling GetWindowPlacement, set the length member to sizeof(WINDOWPLACEMENT).
		/// GetWindowPlacement fails if lpwndpl-> length is not set correctly.</param>
		/// <returns>If the function succeeds, the return value is true.
		/// If the function fails, the return value is false. To get extended error information, call GetLastError.</returns>
		[DllImport("user32.dll")]
		public static extern bool GetWindowPlacement(IntPtr hWnd, ref WindowPlacement lpwndpl);

		/// <summary>
		/// Contains information about the placement of a window on the screen.
		/// </summary>
		public struct WindowPlacement
		{
			/// <summary>
			/// The length of the structure, in bytes. Before calling the GetWindowPlacement or SetWindowPlacement functions, set this member to sizeof(WINDOWPLACEMENT).
			/// GetWindowPlacement and SetWindowPlacement fail if this member is not set correctly.
			/// </summary>
			public uint length;

			/// <summary>
			/// The flags that control the position of the minimized window and the method by which the window is restored. This member can be one or more of the following values.
			/// Value	Meaning 
			/// WPF_ASYNCWINDOWPLACEMENT	0x0004	If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request.
			/// WPF_RESTORETOMAXIMIZED		0x0002	The restored window will be maximized, regardless of whether it was maximized before it was minimized. This setting is only valid the next time the window is restored. It does not change the default restoration behavior.
			///																		This flag is only valid when the SW_SHOWMINIMIZED value is specified for the showCmd member.
			/// WPF_SETMINPOSITION				0x0001	The coordinates of the minimized window may be specified.
			///																		This flag must be specified if the coordinates are set in the ptMinPosition member.
			/// </summary>
			public uint flags;

			/// <summary>
			/// The current show state of the window. This member can be one of the following values.
			/// </summary>
			public uint showCmd;

			/// <summary>
			/// The coordinates of the window's upper-left corner when the window is minimized.
			/// </summary>
			public Point ptMinPosition;

			/// <summary>
			/// The coordinates of the window's upper-left corner when the window is maximized.
			/// </summary>
			public Point ptMaxPosition;

			/// <summary>
			/// The window's coordinates when the window is in the restored position.
			/// </summary>
			public Rectangle rcNormalPosition;
		};

		/// <summary>
		/// The GetWindowRgn function obtains a copy of the window region of a window.
		/// The window region of a window is set by calling the SetWindowRgn function. 
		/// The window region determines the area within the window where the system
		/// permits drawing. The system does not display any portion of a window that
		/// lies outside of the window region
		/// </summary>
		/// <param name="hWnd">Handle to the window whose window region is to be obtained.</param>
		/// <param name="hRgn">Handle to the region which will be modified to represent the window region.</param>
		/// <returns>The return value specifies the type of the region that the function obtains. It can be one of the following values.
		/// NULLREGION - The region is empty.
		/// SIMPLEREGION - The region is a single rectangle.
		/// COMPLEXREGION - The region is more than one rectangle.
		/// ERROR - The specified window does not have a region, 
		///					or an error occurred while attempting to return the region.</returns>
		[DllImport("user32.dll")]
		public static extern int GetWindowRgn(IntPtr hWnd, ref Region hRgn);

		public enum GWCmd : uint
		{
			GW_HWNDFIRST = 0,
			GW_HWNDLAST = 1,
			GW_HWNDNEXT = 2,
			GW_HWNDPREV = 3,
			GW_OWNER = 4,
			GW_CHILD = 5,
			GW_ENABLEDPOPUP = 6,
		}

		public enum GAFlag : uint
		{
			/// <summary>
			/// Retrieves the parent window. This does not include the owner, as it does with the GetParent function.
			/// </summary>
			GA_PARENT = 1,

			/// <summary>
			/// Retrieves the root window by walking the chain of parent windows.
			/// </summary>
			GA_ROOT = 2,

			/// <summary>
			/// Retrieves the owned root window by walking the chain of parent and owner windows returned by GetParent.
			/// </summary>
			GA_ROOTOWNER = 3,
		}

#endregion

#region DMW
		/// <summary>
		/// Retrieves the current value of a specified attribute applied to a window.
		/// </summary>
		/// <param name="hwnd"></param>
		/// <param name="dwAttribute"></param>
		/// <param name="pvAttribute"></param>
		/// <param name="cbAttribute"></param>
		/// <returns></returns>
		[DllImport("dwmapi.dll")]
		public static extern int DwmGetWindowAttribute(IntPtr hwnd,
			int dwAttribute, ref Rectangle pvAttribute, int cbAttribute);

		/// <summary>
		/// Flags used by the DwmGetWindowAttribute and DwmSetWindowAttribute
		/// functions to specify window attributes for non-client rendering.
		/// </summary>
		public enum DwmWindowAttribute : int
		{
			/// <summary>
			/// Use with DwmGetWindowAttribute. Discovers whether non-client rendering is enabled. 
			/// The retrieved value is of type BOOL. TRUE if non-client rendering is enabled; 
			/// otherwise, FALSE.
			/// </summary>
			DWMWA_NCRENDERING_ENABLED = 1,

			/// <summary>
			/// Use with DwmSetWindowAttribute. Sets the non-client rendering policy. 
			/// The pvAttribute parameter points to a value from the DWMNCRENDERINGPOLICY enumeration.
			/// </summary>
			DWMWA_NCRENDERING_POLICY,

			/// <summary>
			/// Use with DwmSetWindowAttribute. Enables or forcibly disables DWM transitions. 
			/// The pvAttribute parameter points to a value of TRUE to disable transitions or FALSE to enable transitions.
			/// </summary>
			DWMWA_TRANSITIONS_FORCEDISABLED,

			/// <summary>
			/// Use with DwmSetWindowAttribute. Enables content rendered in the non-client area to be visible on the frame drawn by DWM. 
			/// The pvAttribute parameter points to a value of TRUE to enable content rendered in the non-client area to be visible on the frame; otherwise, it points to FALSE.
			/// </summary>
			DWMWA_ALLOW_NCPAINT,

			/// <summary>
			/// Use with DwmGetWindowAttribute. Retrieves the bounds of the caption button area in the window-relative space. 
			/// The retrieved value is of type RECT.
			/// </summary>
			DWMWA_CAPTION_BUTTON_BOUNDS,

			/// <summary>
			/// Use with DwmSetWindowAttribute. Specifies whether non-client content is right-to-left (RTL) mirrored. 
			/// The pvAttribute parameter points to a value of TRUE if the non-client content is right-to-left (RTL) mirrored; otherwise, it points to FALSE.
			/// </summary>
			DWMWA_NONCLIENT_RTL_LAYOUT,

			/// <summary>
			/// Use with DwmSetWindowAttribute. Forces the window to display an iconic thumbnail or 
			/// peek representation (a static bitmap), even if a live or snapshot representation of the window is available. 
			/// This value normally is set during a window's creation and not changed throughout the window's lifetime. 
			/// Some scenarios, however, might require the value to change over time. 
			/// The pvAttribute parameter points to a value of TRUE to require a iconic thumbnail or peek representation; 
			/// otherwise, it points to FALSE.
			/// </summary>
			DWMWA_FORCE_ICONIC_REPRESENTATION,

			/// <summary>
			/// Use with DwmSetWindowAttribute. Sets how Flip3D treats the window. 
			/// The pvAttribute parameter points to a value from the DWMFLIP3DWINDOWPOLICY enumeration.
			/// </summary>
			DWMWA_FLIP3D_POLICY,

			/// <summary>
			/// Use with DwmGetWindowAttribute. Retrieves the extended frame bounds rectangle in screen space. 
			/// The retrieved value is of type RECT.
			/// </summary>
			DWMWA_EXTENDED_FRAME_BOUNDS,

			/// <summary>
			/// Use with DwmSetWindowAttribute. The window will provide a bitmap for use by DWM as an iconic thumbnail or 
			/// peek representation (a static bitmap) for the window. DWMWA_HAS_ICONIC_BITMAP can be specified 
			/// with DWMWA_FORCE_ICONIC_REPRESENTATION. DWMWA_HAS_ICONIC_BITMAP normally is set during a window's 
			/// creation and not changed throughout the window's lifetime. Some scenarios, however, might require 
			/// the value to change over time. The pvAttribute parameter points to a value of TRUE to inform DWM that 
			/// the window will provide an iconic thumbnail or peek representation; otherwise, it points to FALSE.
			/// Windows Vista and earlier:  This value is not supported.
			/// </summary>
			DWMWA_HAS_ICONIC_BITMAP,

			/// <summary>
			/// Use with DwmSetWindowAttribute. Do not show peek preview for the window. The peek view shows a 
			/// full-sized preview of the window when the mouse hovers over the window's thumbnail in the taskbar. 
			/// If this attribute is set, hovering the mouse pointer over the window's thumbnail 
			/// dismisses peek (in case another window in the group has a peek preview showing). 
			/// The pvAttribute parameter points to a value of TRUE to prevent peek functionality or FALSE to allow it.
			/// Windows Vista and earlier:  This value is not supported.
			/// </summary>
			DWMWA_DISALLOW_PEEK,

			/// <summary>
			/// Use with DwmSetWindowAttribute. Prevents a window from fading to a glass sheet when peek is invoked. 
			/// The pvAttribute parameter points to a value of TRUE to prevent the window from fading during another
			/// window's peek or FALSE for normal behavior.
			/// Windows Vista and earlier:  This value is not supported.
			/// </summary>
			DWMWA_EXCLUDED_FROM_PEEK,

			/// <summary>
			/// Do not use.
			/// </summary>
			DWMWA_CLOAK,

			/// <summary>
			/// Use with DwmGetWindowAttribute. If the window is cloaked, provides one of the following values 
			/// explaining why:
			/// Name									Value			Meaning
			/// DWM_CLOAKED_APP				0x0000001	The window was cloaked by its owner application.
			/// DWM_CLOAKED_SHELL			0x0000002	The window was cloaked by the Shell.
			/// DWM_CLOAKED_INHERITED	0x0000004	The cloak value was inherited from its owner window.
			/// Windows 7 and earlier:  This value is not supported.
			/// </summary>
			DWMWA_CLOAKED,

			/// <summary>
			/// Use with DwmSetWindowAttribute. Freeze the window's thumbnail image with its current visuals. 
			/// Do no further live updates on the thumbnail image to match the window's contents.
			/// Windows 7 and earlier:  This value is not supported.
			/// </summary>
			DWMWA_FREEZE_REPRESENTATION,

			/// <summary>
			/// The maximum recognized DWMWINDOWATTRIBUTE value, used for validation purposes.
			/// </summary>
			DWMWA_LAST,
		};



		/// <summary>
		/// Retrieves information about the current operating system.
		/// </summary>
		/// <param name="lpVersionInfo">version info</param>
		/// <returns>true if execution is successful</returns>
		[DllImport("kernel32.dll")]
		public static extern bool GetVersionEx(ref OSVersionInfo lpVersionInfo);

		/// <summary>
		/// Contains operating system version information.
		/// The information includes major and minor version numbers, 
		/// a build number, a platform identifier, and descriptive text about the 
		/// operating system. This structure is used with the GetVersionEx function.
		/// To obtain additional version information, use the OSVERSIONINFOEX 
		/// structure with GetVersionEx instead.
		/// </summary>
		public struct OSVersionInfo
		{
			/// <summary>
			/// The size of this data structure, in bytes. 
			/// Set this member to sizeof(OSVERSIONINFO).
			/// </summary>
			public Int32 dwOSVersionInfoSize;

			/// <summary>
			/// The major version number of the operating system. 
			/// For more information, see Remarks.
			/// </summary>
			public Int32 dwMajorVersion;

			/// <summary>
			/// The minor version number of the operating system. 
			/// For more information, see Remarks.
			/// </summary>
			public Int32 dwMinorVersion;

			/// <summary>
			/// The build number of the operating system.
			/// </summary>
			public Int32 dwBuildNumber;

			/// <summary>
			/// The operating system platform. This member can be the following value.
			/// VER_PLATFORM_WIN32_NT = 2 : The operating system is Windows 7,
			/// Windows Server 2008, Windows Vista, Windows Server 2003, Windows XP,
			/// or Windows 2000.
			/// </summary>
			public Int32 dwPlatformId;

			/// <summary>
			/// A null-terminated string, such as "Service Pack 3", that indicates
			/// the latest Service Pack installed on the system. 
			/// If no Service Pack has been installed, the string is empty.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string szCSDVersion;
		}
#endregion

#region Keyboard Events
		public enum WKey : uint
		{
			MK_LBUTTON = 0x0001,
			MK_RBUTTON = 0x0002,
			MK_SHIFT = 0x0004,
			MK_CONTROL = 0x0008,
			MK_MBUTTON = 0x0010,
		}

#region VKey
		public enum VKey : int
		{
			/*
			 * Virtual Keys, Standard Set
			 */
			VK_LBUTTON = 0x01,
			VK_RBUTTON = 0x02,
			VK_CANCEL = 0x03,
			VK_MBUTTON = 0x04,    /* NOT contiguous with L & RBUTTON */

			//#if(_WIN32_WINNT >= 0x0500)
			VK_XBUTTON1 = 0x05,    /* NOT contiguous with L & RBUTTON */
			VK_XBUTTON2 = 0x06,    /* NOT contiguous with L & RBUTTON */
			//#endif /* _WIN32_WINNT >= 0x0500 */

			/*
			 * 0x07 : unassigned
			 */

			VK_BACK = 0x08,
			VK_TAB = 0x09,

			/*
			 * 0x0A - 0x0B : reserved
			 */

			VK_CLEAR = 0x0C,
			VK_RETURN = 0x0D,

			VK_SHIFT = 0x10,
			VK_CONTROL = 0x11,
			VK_MENU = 0x12,
			VK_PAUSE = 0x13,
			VK_CAPITAL = 0x14,

			VK_KANA = 0x15,
			VK_HANGEUL = 0x15,  /* old name - should be here for compatibility */
			VK_HANGUL = 0x15,
			VK_JUNJA = 0x17,
			VK_FINAL = 0x18,
			VK_HANJA = 0x19,
			VK_KANJI = 0x19,

			VK_ESCAPE = 0x1B,

			VK_CONVERT = 0x1C,
			VK_NONCONVERT = 0x1D,
			VK_ACCEPT = 0x1E,
			VK_MODECHANGE = 0x1F,

			VK_SPACE = 0x20,
			VK_PRIOR = 0x21,
			VK_NEXT = 0x22,
			VK_END = 0x23,
			VK_HOME = 0x24,
			VK_LEFT = 0x25,
			VK_UP = 0x26,
			VK_RIGHT = 0x27,
			VK_DOWN = 0x28,
			VK_SELECT = 0x29,
			VK_PRINT = 0x2A,
			VK_EXECUTE = 0x2B,
			VK_SNAPSHOT = 0x2C,
			VK_INSERT = 0x2D,
			VK_DELETE = 0x2E,
			VK_HELP = 0x2F,

			/*
			 * VK_0 - VK_9 are the same as ASCII '0' - '9' (0x30 - 0x39)
			 * 0x40 : unassigned
			 * VK_A - VK_Z are the same as ASCII 'A' - 'Z' (0x41 - 0x5A)
			 */

			VK_LWIN = 0x5B,
			VK_RWIN = 0x5C,
			VK_APPS = 0x5D,

			/*
			 * 0x5E : reserved
			 */

			VK_SLEEP = 0x5F,

			VK_NUMPAD0 = 0x60,
			VK_NUMPAD1 = 0x61,
			VK_NUMPAD2 = 0x62,
			VK_NUMPAD3 = 0x63,
			VK_NUMPAD4 = 0x64,
			VK_NUMPAD5 = 0x65,
			VK_NUMPAD6 = 0x66,
			VK_NUMPAD7 = 0x67,
			VK_NUMPAD8 = 0x68,
			VK_NUMPAD9 = 0x69,
			VK_MULTIPLY = 0x6A,
			VK_ADD = 0x6B,
			VK_SEPARATOR = 0x6C,
			VK_SUBTRACT = 0x6D,
			VK_DECIMAL = 0x6E,
			VK_DIVIDE = 0x6F,
			VK_F1 = 0x70,
			VK_F2 = 0x71,
			VK_F3 = 0x72,
			VK_F4 = 0x73,
			VK_F5 = 0x74,
			VK_F6 = 0x75,
			VK_F7 = 0x76,
			VK_F8 = 0x77,
			VK_F9 = 0x78,
			VK_F10 = 0x79,
			VK_F11 = 0x7A,
			VK_F12 = 0x7B,
			VK_F13 = 0x7C,
			VK_F14 = 0x7D,
			VK_F15 = 0x7E,
			VK_F16 = 0x7F,
			VK_F17 = 0x80,
			VK_F18 = 0x81,
			VK_F19 = 0x82,
			VK_F20 = 0x83,
			VK_F21 = 0x84,
			VK_F22 = 0x85,
			VK_F23 = 0x86,
			VK_F24 = 0x87,

			/*
			 * 0x88 - 0x8F : unassigned
			 */

			VK_NUMLOCK = 0x90,
			VK_SCROLL = 0x91,

			/*
			 * NEC PC-9800 kbd definitions
			 */
			VK_OEM_NEC_EQUAL = 0x92,   // '=' key on numpad

			/*
			 * Fujitsu/OASYS kbd definitions
			 */
			VK_OEM_FJ_JISHO = 0x92,   // 'Dictionary' key
			VK_OEM_FJ_MASSHOU = 0x93,   // 'Unregister word' key
			VK_OEM_FJ_TOUROKU = 0x94,   // 'Register word' key
			VK_OEM_FJ_LOYA = 0x95,   // 'Left OYAYUBI' key
			VK_OEM_FJ_ROYA = 0x96,   // 'Right OYAYUBI' key

			/*
			 * 0x97 - 0x9F : unassigned
			 */

			/*
			 * VK_L* & VK_R* - left and right Alt, Ctrl and Shift virtual keys.
			 * Used only as parameters to GetAsyncKeyState() and GetKeyState().
			 * No other API or message will distinguish left and right keys in this way.
			 */
			VK_LSHIFT = 0xA0,
			VK_RSHIFT = 0xA1,
			VK_LCONTROL = 0xA2,
			VK_RCONTROL = 0xA3,
			VK_LMENU = 0xA4,
			VK_RMENU = 0xA5,

			//#if(_WIN32_WINNT >= 0x0500)
			VK_BROWSER_BACK = 0xA6,
			VK_BROWSER_FORWARD = 0xA7,
			VK_BROWSER_REFRESH = 0xA8,
			VK_BROWSER_STOP = 0xA9,
			VK_BROWSER_SEARCH = 0xAA,
			VK_BROWSER_FAVORITES = 0xAB,
			VK_BROWSER_HOME = 0xAC,

			VK_VOLUME_MUTE = 0xAD,
			VK_VOLUME_DOWN = 0xAE,
			VK_VOLUME_UP = 0xAF,
			VK_MEDIA_NEXT_TRACK = 0xB0,
			VK_MEDIA_PREV_TRACK = 0xB1,
			VK_MEDIA_STOP = 0xB2,
			VK_MEDIA_PLAY_PAUSE = 0xB3,
			VK_LAUNCH_MAIL = 0xB4,
			VK_LAUNCH_MEDIA_SELECT = 0xB5,
			VK_LAUNCH_APP1 = 0xB6,
			VK_LAUNCH_APP2 = 0xB7,

			//#endif /* _WIN32_WINNT >= 0x0500 */

			/*
			 * 0xB8 - 0xB9 : reserved
			 */

			VK_OEM_1 = 0xBA,   // ';:' for US
			VK_OEM_PLUS = 0xBB,   // '+' any country
			VK_OEM_COMMA = 0xBC,   // ',' any country
			VK_OEM_MINUS = 0xBD,   // '-' any country
			VK_OEM_PERIOD = 0xBE,   // '.' any country
			VK_OEM_2 = 0xBF,   // '/?' for US
			VK_OEM_3 = 0xC0,   // '`~' for US

			/*
			 * 0xC1 - 0xD7 : reserved
			 */

			/*
			 * 0xD8 - 0xDA : unassigned
			 */

			VK_OEM_4 = 0xDB,  //  '[{' for US
			VK_OEM_5 = 0xDC,  //  '\|' for US
			VK_OEM_6 = 0xDD,  //  ']}' for US
			VK_OEM_7 = 0xDE,  //  ''"' for US
			VK_OEM_8 = 0xDF,

			/*
			 * 0xE0 : reserved
			 */

			/*
			 * Various extended or enhanced keyboards
			 */
			VK_OEM_AX = 0xE1,  //  'AX' key on Japanese AX kbd
			VK_OEM_102 = 0xE2,  //  "<>" or "\|" on RT 102-key kbd.
			VK_ICO_HELP = 0xE3,  //  Help key on ICO
			VK_ICO_00 = 0xE4,  //  00 key on ICO

			//#if(WINVER >= 0x0400)
			VK_PROCESSKEY = 0xE5,
			//#endif /* WINVER >= 0x0400 */

			VK_ICO_CLEAR = 0xE6,


			//#if(_WIN32_WINNT >= 0x0500)
			VK_PACKET = 0xE7,
			//#endif /* _WIN32_WINNT >= 0x0500 */

			/*
			 * 0xE8 : unassigned
			 */

			/*
			 * Nokia/Ericsson definitions
			 */
			VK_OEM_RESET = 0xE9,
			VK_OEM_JUMP = 0xEA,
			VK_OEM_PA1 = 0xEB,
			VK_OEM_PA2 = 0xEC,
			VK_OEM_PA3 = 0xED,
			VK_OEM_WSCTRL = 0xEE,
			VK_OEM_CUSEL = 0xEF,
			VK_OEM_ATTN = 0xF0,
			VK_OEM_FINISH = 0xF1,
			VK_OEM_COPY = 0xF2,
			VK_OEM_AUTO = 0xF3,
			VK_OEM_ENLW = 0xF4,
			VK_OEM_BACKTAB = 0xF5,

			VK_ATTN = 0xF6,
			VK_CRSEL = 0xF7,
			VK_EXSEL = 0xF8,
			VK_EREOF = 0xF9,
			VK_PLAY = 0xFA,
			VK_ZOOM = 0xFB,
			VK_NONAME = 0xFC,
			VK_PA1 = 0xFD,
			VK_OEM_CLEAR = 0xFE,
		}
#endregion

		public enum Modifiers : uint
		{
			/// <summary>
			/// Either ALT key must be held down.
			/// </summary>
			MOD_ALT = 0x1,

			/// <summary>
			/// Either CTRL key must be held down.
			/// </summary>
			MOD_CONTROL = 0x2,

			/// <summary>
			/// Either SHIFT key must be held down.
			/// </summary>
			MOD_SHIFT = 0x4,

			/// <summary>
			/// Either WINDOWS key was held down. These keys are labeled with the Windows logo. 
			/// Keyboard shortcuts that involve the WINDOWS key are reserved for use by the operating system.
			/// </summary>
			MOD_WIN = 0x8,

			/// <summary>
			/// Changes the hotkey behavior so that the keyboard auto-repeat does not yield multiple hotkey notifications.
			/// </summary>
			/// 
			/// <remarks>Windows Vista and Windows XP/2000:  This flag is not supported.</remarks>
			MOD_NOREPEAT = 0x4000,
		}

		[DllImport("user32.dll")]
		internal static extern bool GetKeyboardState(byte[] lpKeyState);

		[DllImport("user32.dll")]
		public static extern short GetKeyState(VKey nVirtKey);

#endregion

#region Graphics

		[DllImport("USER32.DLL")]
		public static extern bool ScreenToClient(
			IntPtr hWnd,        // handle to window
			ref Point lpPoint   // screen coordinates
		);

		[DllImport("GDI32.DLL")]
		public static extern bool BitBlt(
			IntPtr hdcDest,
			int nXDest,
			int nYDest,
			int nWidth,
			int nHeight,
			IntPtr hdcSrc,
			int nXSrc,
			int nYSrc,
			System.Int32 dwRop
		);

		[DllImport("GDI32.DLL")]
		public static extern bool StretchBlt(
			IntPtr hdcDest,      // handle to destination DC
			int nXOriginDest, // x-coord of destination upper-left corner
			int nYOriginDest, // y-coord of destination upper-left corner
			int nWidthDest,   // width of destination rectangle
			int nHeightDest,  // height of destination rectangle
			IntPtr hdcSrc,       // handle to source DC
			int nXOriginSrc,  // x-coord of source upper-left corner
			int nYOriginSrc,  // y-coord of source upper-left corner
			int nWidthSrc,    // width of source rectangle
			int nHeightSrc,   // height of source rectangle
			UInt32 dwRop       // raster operation code
		);

		public enum RopCode : int
		{
			BLACKNESS = 0x42,
			DSTINVERT = 0x550009,
			MERGECOPY = 0xC000CA,
			MERGEPAINT = 0xBB0226,
			NOTSRCCOPY = 0x330008,
			NOTSRCERASE = 0x1100A6,
			PATCOPY = 0xF00021,
			PATINVERT = 0x5A0049,
			PATPAINT = 0xFB0A09,
			SRCAND = 0x8800C6,
			SRCCOPY = 0xCC0020,
			SRCERASE = 0x440328,
			SRCINVERT = 0x660046,
			SRCPAINT = 0xEE0086,
			WHITENESS = 0xFF0062,
		};

		[DllImport("GDI32.DLL")]
		public static extern long GetBitmapBits(
			IntPtr hbmp,      // handle to bitmap
			long cbBuffer,     // number of bytes to copy
			ref byte[] lpvBits     // buffer to receive bits
		);

		[DllImport("user32.dll")]
		public static extern bool DrawFocusRect(
			IntPtr hDC,  // handle to device context
			ref Rectangle lprc  // logical coordinates
		);

		[DllImport("user32.dll")]
		public static extern IntPtr GetDC(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern int ReleaseDC(IntPtr pDC);

		[DllImport("user32.dll")]
		public static extern bool ScrollDC(IntPtr hDC, int dx, int dy, ref Rectangle rect, ref Rectangle lprcClip,
			IntPtr hrgnUpdate, ref Rectangle lprcUpdate);

		[DllImport("gdi32.dll")]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hobject);

		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);

		[DllImport("Kernel32.dll")]
		public static extern int MulDiv(int nNumber, int nNumerator, int nDenominator);

#endregion

#region Font & Text

		[DllImport("gdi32.dll")]
		public static extern bool GetTextMetrics(IntPtr hdc, ref TEXTMETRIC lptm);

		public struct TEXTMETRIC
		{
			public int tmHeight;
			public int tmAscent;
			public int tmDescent;
			public int tmInternalLeading;
			public int tmExternalLeading;
			public int tmAveCharWidth;
			public int tmMaxCharWidth;
			public int tmWeight;
			public int tmOverhang;
			public int tmDigitizedAspectX;
			public int tmDigitizedAspectY;
			public char tmFirstChar;
			public char tmLastChar;
			public char tmDefaultChar;
			public char tmBreakChar;
			public byte tmItalic;
			public byte tmUnderlined;
			public byte tmStruckOut;
			public byte tmPitchAndFamily;
			public byte tmCharSet;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public class LOGFONT
		{
			public int lfHeight = 0;
			public int lfWidth = 0;
			public int lfEscapement = 0;
			public int lfOrientation = 0;
			public int lfWeight = 0;
			public byte lfItalic = 0;
			public byte lfUnderline = 0;
			public byte lfStrikeOut = 0;
			public byte lfCharSet = 0;
			public byte lfOutPrecision = 0;
			public byte lfClipPrecision = 0;
			public byte lfQuality = 0;
			public byte lfPitchAndFamily = 0;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
			public string lfFaceName;
		}

		[DllImport("gdi32.dll")]
		public static extern int GetFontData(
			IntPtr hdc,           // デバイスコンテキストのハンドル
			int dwTable,     // 照会するメトリックテーブル
			int dwOffset,    // 照会するテーブル内のオフセット
			byte[] lpvBuffer,  // 返されるデータを受け取るバッファへのポインタ
			int cbData       // 照会するデータの長さ
		);

		[DllImport("gdi32.dll")]
		public static extern bool GetCharWidthFloat(IntPtr hdc, uint iFirstChar, uint iLastChar, out float width);

		[DllImport("gdi32.dll")]
		public static extern bool GetTextExtentPoint32(IntPtr hdc, string lpString, int c, ref Size lpSize);

		[DllImport("gdi32.dll")]
		public static extern uint GetFontUnicodeRanges(IntPtr hdc, IntPtr lpgs);

#endregion

#region Charset & IME

		public struct COMPOSITIONFORM
		{
			public int dwStyle;
			public System.Drawing.Point ptCurrentPos;
			public System.Drawing.Rectangle rcArea;
		}

		public struct CANDIDATEFORM
		{
			public int dwIndex;
			public int dwStyle;
			public Point ptCurrentPos;
			public Rectangle rcArea;
		}

		[DllImport("imm32.dll")]
		public static extern IntPtr ImmCreateContext();

		[DllImport("imm32.dll")]
		public static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC);

		[DllImport("imm32.dll")]
		public static extern IntPtr ImmGetContext(IntPtr hWnd);

		[DllImport("imm32.dll")]
		public static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);

		[DllImport("imm32.dll")]
		public static extern bool ImmSetCompositionWindow(IntPtr hIMC, ref COMPOSITIONFORM lpCompForm);

		[DllImport("imm32.dll")]
		public static extern bool ImmSetCompositionFont(IntPtr hIMC, LOGFONT lplf);

		[DllImport("imm32.dll")]
		public static extern int ImmGetCompositionString(IntPtr hIMC, int dwIndex, StringBuilder sb, int dwBufLen);
		//public static extern int ImmGetCompositionString(IntPtr hIMC, int dwIndex, IntPtr lpBuf, int dwBufLen);

		[DllImport("imm32.dll")]
		public static extern bool ImmGetCompositionWindow(IntPtr hIMC, ref COMPOSITIONFORM lpCompForm);

		[DllImport("imm32.dll")]
		public static extern bool ImmSetCandidateWindow(IntPtr hIMC, ref CANDIDATEFORM lpCandidate);

		// wParam of report message WM_IME_REQUEST
		public enum IMR : int
		{
			IMR_COMPOSITIONWINDOW = 0x0001,
			IMR_CANDIDATEWINDOW = 0x0002,
			IMR_COMPOSITIONFONT = 0x0003,
			IMR_RECONVERTSTRING = 0x0004,
			IMR_CONFIRMRECONVERTSTRING = 0x0005,
			IMR_QUERYCHARPOSITION = 0x0006,
			IMR_DOCUMENTFEED = 0x0007,
		}

		// parameter of ImmGetCompositionString
		public enum GCS : int
		{
			GCS_COMPREADSTR = 0x0001,
			GCS_COMPREADATTR = 0x0002,
			GCS_COMPREADCLAUSE = 0x0004,
			GCS_COMPSTR = 0x0008,
			GCS_COMPATTR = 0x0010,
			GCS_COMPCLAUSE = 0x0020,
			GCS_CURSORPOS = 0x0080,
			GCS_DELTASTART = 0x0100,
			GCS_RESULTREADSTR = 0x0200,
			GCS_RESULTREADCLAUSE = 0x0400,
			GCS_RESULTSTR = 0x0800,
			GCS_RESULTCLAUSE = 0x1000,
		}

		// style bit flags for WM_IME_COMPOSITION
		public enum IME_CS : int
		{
			CS_INSERTCHAR = 0x2000,
			CS_NOMOVECARET = 0x4000,
		}

		// bit field for IMC_SETCOMPOSITIONWINDOW, IMC_SETCANDIDATEWINDOW
		public enum CFS : int
		{
			CFS_DEFAULT = 0x0000,
			CFS_RECT = 0x0001,
			CFS_POINT = 0x0002,
			CFS_FORCE_POSITION = 0x0020,
			CFS_CANDIDATEPOS = 0x0040,
			CFS_EXCLUDE = 0x0080,
		}

		public enum Charset : int
		{
			ANSI_CHARSET = 0,
			DEFAULT_CHARSET = 1,
			SYMBOL_CHARSET = 2,
			SHIFTJIS_CHARSET = 128,
			HANGEUL_CHARSET = 129,
			HANGUL_CHARSET = 129,
			GB2312_CHARSET = 134,
			CHINESEBIG5_CHARSET = 136,
			OEM_CHARSET = 255,
			JOHAB_CHARSET = 130,
			HEBREW_CHARSET = 177,
			ARABIC_CHARSET = 178,
			GREEK_CHARSET = 161,
			TURKISH_CHARSET = 162,
			VIETNAMESE_CHARSET = 163,
			THAI_CHARSET = 222,
			EASTEUROPE_CHARSET = 238,
			RUSSIAN_CHARSET = 204,
		}

#endregion

#region Threading

		[DllImport("kernel32.dll")]
		public static extern int GetCurrentThreadId();

		[DllImport("user32.dll")]
		public static extern IntPtr GetThreadDesktop(int dwThread);

#endregion

#region Cursor

		/*
		 * Standard Cursor IDs
		 */
		public enum Cursors : int
		{
			IDC_ARROW = 32512,
			IDC_IBEAM = 32513,
			IDC_WAIT = 32514,
			IDC_CROSS = 32515,
			IDC_UPARROW = 32516,
			IDC_SIZE = 32640,  /* OBSOLETE: use IDC_SIZEALL */
			IDC_ICON = 32641,  /* OBSOLETE: use IDC_ARROW */
			IDC_SIZENWSE = 32642,
			IDC_SIZENESW = 32643,
			IDC_SIZEWE = 32644,
			IDC_SIZENS = 32645,
			IDC_SIZEALL = 32646,
			IDC_NO = 32648, /*not in win3.1 */
			IDC_HAND = 32649,
			IDC_APPSTARTING = 32650, /*not in win3.1 */
			IDC_HELP = 32651,
		}

		[DllImport("user32.dll")]
		public static extern IntPtr SetCursor(int hCursor);

		[DllImport("USER32.DLL")]
		public static extern bool SetCursorPos(int X, int Y);

		[DllImport("user32.dll")]
		public static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);

		[DllImport("user32.dll")]
		public static extern bool HideCaret(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool DestroyCaret();

		[DllImport("user32.dll")]
		public static extern bool ShowCaret(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool SetCaretPos(int X, int Y);

		[DllImport("user32.dll")]
		public static extern bool GetCursorPos(ref Point point);

		public static long CreateLParamPoint(int x, int y)
		{
			return (y << 16) | (x & 0xffff);
		}

#endregion

#region Clipboard
		[DllImport("User32.dll")]
		public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);
#endregion

#region Console

		[DllImport("kernel32.dll")]
		public static extern bool AllocConsole();

		[DllImport("kernel32.dll")]
		public static extern bool FreeConsole();

		[DllImport("kernel32.dll")]
		public static extern IntPtr GetConsoleWindow();

		[DllImport("kernel32.dll")]
		public static extern bool SetConsoleCP(uint wCodePageID);

		[DllImport("kernel32.dll")]
		public static extern uint GetConsoleCP();

		[DllImport("kernel32.dll")]
		public static extern bool AttachConsole(long dwProcessId);

#endregion
	}

}

#endif // WINFORM || WPF