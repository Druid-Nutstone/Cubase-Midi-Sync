using Cubase.Midi.Sync.WindowManager.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Cubase.Midi.Sync.WindowManager.Services.Win.WindowManagerService;

namespace Cubase.Midi.Sync.WindowManager.Services.Win
{
    public class WindowPositionManager
    {
        private static WindowPositionManager _windowPositionManager= null;

        public int NumberOfScreens { get; set; }

        public Rect PrimaryScreen {  get; set; } 

        public WindowPositionManager()
        {
            var screens = WindowManagerService.GetAllMonitors();
            this.NumberOfScreens = screens.Count();
            this.PrimaryScreen = screens.Select(x => x.WorkRect)
                .OrderByDescending(r => (long)r.Width * r.Height)
                .FirstOrDefault();
        }
        
        public static WindowPositionManager Instance 
        { 
            get 
            { 
                if (_windowPositionManager == null)
                {
                    _windowPositionManager = new WindowPositionManager();
                }
                return _windowPositionManager; 
            }
        }


        public void ArrangeWindows(WindowPosition primaryHwnd, IEnumerable<WindowPosition> secondaryHwnds, int primaryWidth)
        {
            var work = this.PrimaryScreen;

            int totalWidth = work.Width;
            int secondaryWidth = totalWidth - primaryWidth;

            // Position the primary window on the right
            primaryHwnd.WithPosition(new Rect()
            {
                Left = work.Left + secondaryWidth,
                Top = work.Top,
                Right = work.Right,
                Bottom = work.Bottom

            }).SetPosition();

            // Arrange secondary windows vertically in the left third
            var others = secondaryHwnds?.Where(h => h.Hwnd != IntPtr.Zero).ToList() ?? new List<WindowPosition>();
            if (others.Count == 0)
                return;

            int heightPerWindow = work.Height / others.Count;
            int currentTop = work.Top;

            foreach (var win in others)
            {
                var targetRect = new Rect()
                {
                    Left = work.Left,
                    Top = currentTop,
                    Right = secondaryWidth,
                    Bottom = currentTop + heightPerWindow
                };
                if (!win.Position.HasValue)
                {
                    win.WithPosition(targetRect)
                       .SetPosition();
                }
                currentTop += heightPerWindow;
            }
        }
    }
}
