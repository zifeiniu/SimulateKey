﻿//
// Authors:
//   Lucas Ontivero lucasontivero@gmail.com
//
// Copyright (C) 2014 Lucas Ontivero
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Open.WinKeyboardHook
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct KBDLLHOOKSTRUCT
    {
        public uint KeyCode;
        public uint ScanCode;
        public uint Flags;
        public uint Time;
        public uint ExtraInfo;
    }

    internal delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);


    [ComVisible(false), SuppressUnmanagedCodeSecurity]
    internal static class NativeMethods
    {
        internal const int WH_KEYBOARD_LL = 0x0D;

        internal const byte VK_CAPITAL = 0x14;
        internal const byte VK_CONTROL = 0x11;
        internal const byte VK_MENU = 0x12;
        internal const byte VK_SHIFT = 0x10;
        internal const byte VK_LSHIFT = 0xA0;
        internal const byte VK_RSHIFT = 0xA1;
        internal const byte VK_LALT = 0xA4;
        internal const byte VK_LCONTROL = 0xA2;
        internal const byte VK_NUMLOCK = 0x90;
        internal const byte VK_RALT = 0xA5;
        internal const byte VK_RCONTROL = 0xA3;
        internal const byte VK_RMENU = 0xA5;

        internal const int WM_KEYDOWN = 0x100;
        internal const int WM_SYSKEYDOWN = 0x104;
        internal const int WM_KEYUP = 0x101;
        internal const int WM_SYSKEYUP = 0x105;
        internal const int WM_DEADCHAR = 0x0103;
        internal const int WM_SYSDEADCHAR = 0x0107;

        [DllImport("user32.dll")]
        internal static extern IntPtr CallNextHookEx(SafeWinHandle hhk, int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern SafeWinHandle SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod,
                                                       uint dwThreadId);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        internal static extern short GetKeyState(int vKey);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        internal static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        internal static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[]
                                                                                  lpKeyState,
                                               [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff,
                                               int cchBuff, uint wFlags, IntPtr dwhkl);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetKeyboardLayout(uint idThread);
    }
}