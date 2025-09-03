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

        public bool SendKey(string keyText, Action<string> errHandler)
        {
            var cubase = CubaseExtensions.GetCubaseService();
            if (cubase?.MainWindowHandle == IntPtr.Zero)
                return false;

            // Bring Cubase to foreground
            const int maxCount = 10000;
            int currentCount = 0;
            while (GetForegroundWindow() != cubase.MainWindowHandle)
            {
                if (currentCount++ < maxCount)
                {
                    SetForegroundWindow(cubase.MainWindowHandle);
                    Thread.Sleep(50);
                }
                else
                {
                    return false; // failed to bring Cubase to foreground
                }
            }

            // Split keyText into parts (modifiers + main key)
            var parts = keyText.ToUpper().Split('+');
            var modifiers = new List<byte>();
            byte key = 0;

            foreach (var part in parts)
            {
                // Normalize: remove spaces and handle Cubase naming quirks
                var lookup = part.Replace(" ", "");

                // Lookup in CubaseKeyMap
                if (CubaseKeyMap.Map.TryGetValue(lookup, out byte vk))
                {
                    if (part == parts[^1])
                        key = vk; // last part = main key
                    else
                        modifiers.Add(vk); // treat as modifier
                }
                else
                {
                    errHandler($"Could not find a keyboard mapping for {part}");
                    return false;
                }
            }

            // Press modifiers
            foreach (var mod in modifiers)
                keybd_event(mod, 0, 0, UIntPtr.Zero);

            // Press main key
            keybd_event(key, 0, 0, UIntPtr.Zero);
            Thread.Sleep(50); // simulate key press
            keybd_event(key, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

            // Release modifiers
            foreach (var mod in modifiers)
                keybd_event(mod, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

            return true;
        }

    }
}
