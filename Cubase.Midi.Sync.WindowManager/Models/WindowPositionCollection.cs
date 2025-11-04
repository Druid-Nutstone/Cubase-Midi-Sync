using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.WindowManager.Models
{
    public class WindowPositionCollection : List<WindowPosition>
    {
        public string Name { get; set; }

        
        //private void GetWindowPosition(string windowName, WindowTargetLocation windowTargetLocation)
        //{
        //    WindowPosition windowPosition = null;

        //    windowPosition = this.FirstOrDefault(x => x.Name.Equals(windowName, StringComparison.OrdinalIgnoreCase));

        //    if (windowPosition == null)
        //    {
        //        windowPosition = WindowPosition.Create(windowName);
        //        this.Add(windowPosition);
        //    }

        //    if (windowTargetLocation == WindowTargetLocation.New)
        //    {
        //        windowPosition.WithPosition(windowPosition.GetCurrentPosition());
        //    }
        //    else
        //    {
        //        windowPosition.WithOriginalPosition(windowPosition.GetCurrentPosition());
        //    }
        //}

        public WindowPositionCollection WithWindowPosition(WindowPosition windowPosition)
        {
            this.Add(windowPosition);
            return this;
        }

        public WindowPositionCollection ResetWindowState()
        {
            this.ForEach(x => x.State = WindowState.Unknown);
            return this;
        }

        public WindowPosition GetPrimaryWindow()
        {
            return this.First(x => x.Type == WindowType.Primary);   
        }

        public List<WindowPosition> GetActiveWindows()
        {
            return this.Where(x => x.State != WindowState.Unknown)
                       .Where(x => x.Type != WindowType.Primary)
                       .ToList();
        }

        public void ClearPositionsThatHaveClosed()
        {
            this.Where(x => x.State == WindowState.Unknown)
                .ToList()
                .ForEach(x => x.Position = null);
        }

        public void SetCurrentPosition(nint hwnd, string name)
        {
            this.First(x => name.StartsWith(x.Name, StringComparison.OrdinalIgnoreCase))
                                .WithOriginalPosition(hwnd);
        }

        public IEnumerable<string> GetWindowNames()
        {
            return this.Select(x => x.Name);    
        }

        //public WindowPositionCollection GetCurrentPosition(string windowName, WindowTargetLocation windowTargetLocation)
        //{
        //    this.GetWindowPosition(windowName, windowTargetLocation);
        //    return this;
        //} 
        
        //public WindowPositionCollection GetCurrentPositions(List<string> windowNames, WindowTargetLocation windowTargetLocation)
        //{
        //    foreach (var windowName in windowNames)
        //    {
        //        this.GetWindowPosition(windowName, windowTargetLocation);
        //    }
        //    return this;
        //}

        public bool SetToCurrentPositions()
        {
            foreach (var win in this)
            {
                win.SetOriginalPosition(); 
            }
            return true;
        }

        public bool Save(string fileName)
        {
            File.WriteAllText(fileName, JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true }));
            return true;
        }

        public static WindowPositionCollection Create(string name)
        {
            return new WindowPositionCollection()
            {
                Name = name,
            };
        }

        public static WindowPositionCollection Load(string fileName)
        {
            if (File.Exists(fileName))
            {
                return JsonSerializer.Deserialize<WindowPositionCollection>( File.ReadAllText(fileName));
            }
            return new WindowPositionCollection();
        }

        
    
    }

    public enum WindowTargetLocation
    {
        New = 0,
        Original = 1
    }
}
