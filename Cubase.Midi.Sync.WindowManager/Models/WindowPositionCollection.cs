using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.WindowManager.Models
{
    public class WindowPositionCollection : List<WindowPosition>
    {
        public string Name { get; set; }


        public bool Compare(WindowPositionCollection other)
        {
            if (other == null) return false;
            if (this.Count != other.Count) return false;

            var a = this.OrderBy(i => i.Name).ToArray();
            var b = other.OrderBy(i => i.Name).ToArray();

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i].Name != b[i].Name) return false;
                if (a[i].State != b[i].State) return false;
                if (a[i].Type != b[i].Type) return false;
                if (a[i].Zorder != b[i].Zorder) return false;
                // don't think i want to check the position because i may want to 
                // move the window myself 
            }

            return true;
        }

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
            return this.FirstOrDefault(x => x.Type == WindowType.Primary);   
        }

        public WindowPosition GetWindowByName(string name)
        {
            return this.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public WindowPosition GetWindowThatStartsWith(string name)
        {
            return this.FirstOrDefault(x => x.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase));
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

        public bool SetToCurrentPositions()
        {
            foreach (var win in this)
            {
                win.SetOriginalPosition(); 
            }
            return true;
        }

        public bool HaveFocusedwindow()
        {
            return this.Any(x => x.Zorder == WindowZorder.Focused);
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
