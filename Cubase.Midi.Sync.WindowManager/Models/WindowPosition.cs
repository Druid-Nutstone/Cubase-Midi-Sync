using Cubase.Midi.Sync.WindowManager.Services.Win;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Cubase.Midi.Sync.WindowManager.Services.Win.WindowManagerService;

namespace Cubase.Midi.Sync.WindowManager.Models
{
    public class WindowPosition
    {
        public string Name { get; set; }

        public WindowState State { get; set; } = WindowState.Unknown;

        public nint Hwnd {  get; set; }

        public WindowType Type { get; set; } = WindowType.Transiant;

        public Rect? Position { get; set; } = null;

        public Rect? OriginalPosition { get; set; } = null;  

        public WindowPosition SetPosition()
        {
            MoveTo(Position.Value);
            return this;
        }   
        
        public bool SetOriginalPosition()
        {
            return MoveTo(OriginalPosition.Value);
        }

        public WindowPosition WithPosition(Rect? rect)
        {
            this.Position = rect;
            return this;
        }

        public WindowPosition WithOriginalPosition(nint hwnd)
        {
            Hwnd = hwnd;
            State = WindowManagerService.GetWindowState(hwnd);  
            this.OriginalPosition = WindowManagerService.GetWindowPosition(hwnd);
            return this;
        }

        public WindowPosition WithWindowState(nint hwnd)
        {
            Hwnd = hwnd;
            this.State = WindowManagerService.GetWindowState(hwnd);
            return this;
        }

        public WindowPosition WithWindowType(WindowType windowType)
        {
            this.Type = windowType;
            return this;
        }

        public Rect? GetCurrentPosition()
        {
            var handle = WindowManagerService.FindWindowByTitle(this.Name);
            if (handle != null)
            {
                return WindowManagerService.GetWindowPosition(handle.Value);
            }
            return null;
        }

        private bool MoveTo(Rect? rect = null)
        {
            if (rect != null)
            {
                return WindowManagerService.SetPosition(this.Hwnd, rect.Value);
            }
            return false;
        }

        public static WindowPosition Create(string name)
        {
            return new WindowPosition()
            {
                Name = name
            };
        }

        public static WindowPosition Create(string name, Rect position, Rect originalPosition)
        {
            return new WindowPosition()
            {
                Name = name,
                Position = position,
                OriginalPosition = originalPosition
            };
        }
    
    }
}
