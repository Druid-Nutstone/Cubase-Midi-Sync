using Cubase.Midi.Sync.Common.Window;
using Cubase.Midi.Sync.WindowManager.Models;

namespace Cubase.Midi.Sync.Server.Services.Windows
{
    public class CubaseWindowMonitor : ICubaseWindowMonitor
    {
        public WindowPositionCollection CubaseWindows { get; set; } = new WindowPositionCollection();

        public bool HaveAtLeastOneMixer => this.CubaseWindows.Any(x => x.Name.StartsWith("MixConsole", StringComparison.OrdinalIgnoreCase));

        public List<string> MixerConsoles => this.CubaseWindows.Select(x => x.Name)
                                                               .Where(x => x.StartsWith("MixConsole", StringComparison.OrdinalIgnoreCase))
                                                               .ToList();

        public bool WindowExists(string windowName)
        {
            return this.CubaseWindows.Any(x => x.Name.Equals(windowName, StringComparison.OrdinalIgnoreCase));
        }

        public bool MixerExists(string mixerName)
        {
            return this.CubaseWindows.Any(x => x.Name.StartsWith(mixerName, StringComparison.OrdinalIgnoreCase));
        }

        public List<Action<CubaseActiveWindowCollection>> registeredWindowEventHandlers = new();

        public void CubaseWindowEvent(WindowPositionCollection cubaseWindows)
        {
            if (!cubaseWindows.Compare(this.CubaseWindows))
            {
                this.CubaseWindows = cubaseWindows;
                foreach (var handler in this.registeredWindowEventHandlers)
                {
                    handler.Invoke(this.CreateFromCubaseWindows());
                }
            }
            this.CubaseWindows = cubaseWindows;
        }

        public bool FocusCubase(Action<string> errorHandler)
        {
            var cubaseWindow = this.CubaseWindows.GetPrimaryWindow();
            if (cubaseWindow != null)
            {
                cubaseWindow.Focus();
                return true;
            }
            errorHandler("Cannot find primary cubase window");
            return false;
        }

        public void RegisterForWindowEvents(Action<CubaseActiveWindowCollection> windowEventHandler)
        {
            this.registeredWindowEventHandlers.Add(windowEventHandler); 
        }

        private CubaseActiveWindowCollection CreateFromCubaseWindows()
        {
            var cubaseWindowCollection = new CubaseActiveWindowCollection();
            foreach (var cubaseWindow in this.CubaseWindows)
            {
                cubaseWindowCollection.AddCubaseWindow(cubaseWindow.Name, (CubaseWindowState)cubaseWindow.State, (CubaseWindowType)cubaseWindow.Type, (CubaseWindowZOrder)cubaseWindow.Zorder);
            }
            return cubaseWindowCollection;
        }
    }
}
