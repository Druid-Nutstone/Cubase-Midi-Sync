using Cubase.Midi.Sync.WindowManager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Cubase.Midi.Sync.WindowManager.Services.Win.WindowManagerService;

namespace Cubase.Midi.Sync.WindowManager.Services.Win
{
    public static class WindowManagerService
    {
        // --- Win32 imports ---
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        // --- Monitor API ---
        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfo lpmi);

        [DllImport("user32.dll")]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("kernel32.dll")]
        private static extern uint FormatMessage(
                  uint dwFlags,
                  IntPtr lpSource,
                  uint dwMessageId,
                  uint dwLanguageId,
                  [Out] StringBuilder lpBuffer,
                  uint nSize,
                  IntPtr Arguments);

        [DllImport("dwmapi.dll")]
        private static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out Rect pvAttribute, int cbAttribute);





        private delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);

        [DllImport("user32.dll")]
        private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);

        private const int DWMWA_EXTENDED_FRAME_BOUNDS = 9;
        private const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;
        private static readonly IntPtr HWND_TOP = IntPtr.Zero;
        private const uint SWP_SHOWWINDOW = 0x0040;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const int SW_RESTORE = 9;
        private const uint MONITOR_DEFAULTTONEAREST = 2;

        private const int border = 7;


        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct MonitorInfo
        {
            public int Size;
            public Rect Monitor;
            public Rect Work;
            public uint Flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public Rect rcNormalPosition;
        }

        //private const int SW_HIDE = 0;
        //private const int SW_NORMAL = 1;
        //private const int SW_MINIMIZE = 6;
        //private const int SW_MAXIMIZE = 3;



        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }

            public int Bottom { get; set; }

            public int Width => Right - Left;
            public int Height => Bottom - Top;

            public int TwoThirdsWidth => (Width * 2) / 3;
        }

        public static IEnumerable<(IntPtr Handle, Rect MonitorRect, Rect WorkRect)> GetAllMonitors()
        {
            var result = new List<(IntPtr, Rect, Rect)>();

            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
                (IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData) =>
                {
                    var mi = new MonitorInfo { Size = Marshal.SizeOf(typeof(MonitorInfo)) };
                    if (GetMonitorInfo(hMonitor, ref mi))
                        result.Add((hMonitor, mi.Monitor, mi.Work));

                    return true; // continue enumeration
                },
            IntPtr.Zero);

            return result;
        }

        public static IEnumerable<Process> GetChildProcesses(Process parent)
        {
            var result = new List<Process>();

            var query = $"SELECT * FROM Win32_Process WHERE ParentProcessId={parent.Id}";
            using var searcher = new ManagementObjectSearcher(query);

            foreach (ManagementObject mo in searcher.Get())
            {
                int pid = Convert.ToInt32(mo["ProcessId"]);

                try
                {
                    var child = Process.GetProcessById(pid);
                    result.Add(child);
                }
                catch
                {
                    // Process may have exited or is inaccessible
                }
            }

            return result;
        }

        public static IEnumerable<IntPtr> GetWindowsForProcess(int processId)
        {
            var windows = new List<IntPtr>();
            EnumWindows((hWnd, lParam) =>
            {
                GetWindowThreadProcessId(hWnd, out uint pid);
                if (pid == processId && IsWindowVisible(hWnd))
                    windows.Add(hWnd);
                return true;
            }, IntPtr.Zero);
            return windows;
        }
        
        public static Rect GetVisibleBounds(IntPtr hwnd)
        {
            DwmGetWindowAttribute(hwnd, DWMWA_EXTENDED_FRAME_BOUNDS, out Rect rect, Marshal.SizeOf(typeof(Rect)));
            return rect;
        }

        public static IEnumerable<(IntPtr Handle, string Title)> EnumerateWindows()
        {
            var list = new List<(IntPtr, string)>();
            EnumWindows((hWnd, lParam) =>
            {
                if (!IsWindowVisible(hWnd)) return true;

                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                var builder = new StringBuilder(length + 1);
                GetWindowText(hWnd, builder, builder.Capacity);
                list.Add((hWnd, builder.ToString()));
                return true;
            }, IntPtr.Zero);

            return list;
        }

        public static IntPtr? FindWindowByTitle(string partialTitle)
        {
            foreach (var (handle, title) in EnumerateWindows())
            {
                if (title.Contains(partialTitle, StringComparison.OrdinalIgnoreCase))
                    return handle;
            }
            return null;
        }

        public static Rect GetWindowPosition(IntPtr hwnd)
        {
            GetWindowRect(hwnd, out Rect rect);
            return rect;
        }

        public static bool Move(IntPtr hWnd, int x, int y, int width, int height)
        {
            return MoveWindow(hWnd, x, y, width, height, true);
        }

        public static bool SetPosition(IntPtr hWnd, int x, int y, int width, int height)
        {
            return SetWindowPos(hWnd, HWND_TOP, x - border, y, width + (border * 2), height + border, SWP_SHOWWINDOW);
        }

        public static void SetWindowToMax(IntPtr hWnd)
        {
            ShowWindow(hWnd, (int)WindowState.Maximized);
        }

        public static bool SetPosition(IntPtr hWnd, Rect position)
        {
            bool ok = SetPosition(hWnd, position.Left, position.Top, position.Width, position.Height);
            if (!ok)
            {
                int err = Marshal.GetLastWin32Error();
                string message = new Win32Exception(err).Message;
            }
            return ok;
        }

        public static MonitorInfo GetMonitorForWindow(IntPtr hWnd)
        {
            var hMonitor = MonitorFromWindow(hWnd, MONITOR_DEFAULTTONEAREST);
            var info = new MonitorInfo { Size = Marshal.SizeOf<MonitorInfo>() };
            if (GetMonitorInfo(hMonitor, ref info))
                return info;
            return default;
        }

        public static WindowState GetWindowState(IntPtr hWnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(hWnd, ref placement);
            return Enum.GetValues<WindowState>().Contains((WindowState)placement.showCmd) ? (WindowState)placement.showCmd : WindowState.Unknown; 
        }
        
        
        private static string GetErrorMessage(int errorCode)
        {
            var message = new StringBuilder(256);
            uint result = FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero,
                (uint)errorCode, 0, message, (uint)message.Capacity, IntPtr.Zero);

            if (result == 0)
                return $"Unknown error (code {errorCode})";

            return message.ToString().Trim();
        }
    }

}
