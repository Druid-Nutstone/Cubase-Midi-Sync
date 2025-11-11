using Cubase.Midi.Sync.Common.Keys;
using Cubase.Midi.Sync.Server.Extensions;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Cubase.Midi.Sync.Server.Services.Keyboard
{
    public class KeyboardService : IKeyboardService
    {

        private ILogger<KeyboardService> logger;
        
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        private const int KEYEVENTF_KEYUP = 0x0002;

        public KeyboardService(ILogger<KeyboardService> logger)
        {
            this.logger = logger;
        }

        public bool SendKey(string keyText, Action<string> errHandler)
        {
           
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
                    this.logger.LogWarning($"Could not find Could not find a keyboard mapping for {part}");
                    errHandler($"Could not find a keyboard mapping for {part}");
                    return false;
                }
            }

            // Press modifiers
            foreach (var mod in modifiers)
            {
                this.logger.LogInformation($"Sending {mod}");
                keybd_event(mod, 0, 0, UIntPtr.Zero);
            }

            // Press main key
            this.logger.LogInformation($"Sending main key {key}");
            keybd_event(key, 0, 0, UIntPtr.Zero);
            Thread.Sleep(50); // simulate key press
            keybd_event(key, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

            // Release modifiers
            foreach (var mod in modifiers)
                keybd_event(mod, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

            return true;
        }

          private Process GetCubase()
        {
            var cubase = CubaseExtensions.GetCubaseService();
            if (cubase == null) return null;
            if (cubase.MainWindowHandle == IntPtr.Zero)
                return null;
            return cubase;
        }
    }
}
