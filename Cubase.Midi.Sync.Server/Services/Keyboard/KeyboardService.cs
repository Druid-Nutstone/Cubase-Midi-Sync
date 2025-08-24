using Cubase.Midi.Sync.Common.Keys;
using Cubase.Midi.Sync.Server.Extensions;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Cubase.Midi.Sync.Server.Services.Keyboard
{
    public class KeyboardService : IKeyboardService
    {

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        private const int KEYEVENTF_KEYUP = 0x0002;

        public KeyboardService()
        {

        }

        public bool SendKey(VirtualKey key)
        {
            var cubase = CubaseExtensions.GetCubaseService();
            if (cubase?.MainWindowHandle == IntPtr.Zero)
                return false;

            var maxCount = 10000;
            var currentCount = 0;
            while (GetForegroundWindow() != cubase.MainWindowHandle)
            {
                if (currentCount < maxCount)
                {
                    SetForegroundWindow(cubase.MainWindowHandle);
                    Thread.Sleep(50); // wait a bit for the window to come to the foreground
                }
                else
                {
                    return false; // failed to bring Cubase to foreground
                }
            }

            keybd_event((byte)key, 0, 0, UIntPtr.Zero);
            Thread.Sleep(50); // simulate key press duration    
            keybd_event((byte)key, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            return true;
        }
    }
}
