using Cubase.Midi.Sync.Common.Window;
using Cubase.Midi.Sync.WindowManager.Models;

namespace Cubase.Midi.Sync.Server.Services.Windows
{
    public class CubaseWindowMonitor : ICubaseWindowMonitor
    {
        public WindowPositionCollection CubaseWindows { get; set; } = new WindowPositionCollection();

        private readonly ILogger<CubaseWindowMonitor> logger;

        public bool HaveAtLeastOneMixer => this.CubaseWindows.Any(x => x.Name.StartsWith("MixConsole", StringComparison.OrdinalIgnoreCase));

        public List<string> MixerConsoles => this.CubaseWindows.Select(x => x.Name)
                                                               .Where(x => x.StartsWith("MixConsole", StringComparison.OrdinalIgnoreCase))
                                                               .ToList();

        public CubaseWindowMonitor(ILogger<CubaseWindowMonitor> logger)
        {
            this.logger = logger;
        }

        public List<WindowPosition> GetMixerWindows()
        {
            return this.CubaseWindows
                       .Where(x => x.Name.StartsWith("MixConsole", StringComparison.OrdinalIgnoreCase))
                       .OrderBy(x =>
                       {
                           // Remove the prefix
                           var rest = x.Name.Substring("MixConsole".Length).Trim();

                           // Try to read the number immediately following "MixConsole"
                           if (int.TryParse(rest.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(), out int num))
                               return num;

                           // No number → treat as 1 (or 0)
                           return 1;
                       })
             .ToList();

        }

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
            if (!this.CubaseWindows.Compare(cubaseWindows))
            {
                this.CubaseWindows = cubaseWindows;
                var activeWindowCollection = this.CreateFromCubaseWindows();
                foreach (var handler in this.registeredWindowEventHandlers)
                {
                    handler.Invoke(activeWindowCollection);
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

        public void UnRegisterForWindowEvents(Action<CubaseActiveWindowCollection> windowEventHandler)
        {
            this.registeredWindowEventHandlers.Remove(windowEventHandler);
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

        public bool HaveAtLeastOneCubaseWindowFocused()
        {
            var cubaseWindow = this.CubaseWindows.GetPrimaryWindow();
            if (cubaseWindow != null)
            {
                if (!this.CubaseWindows.HaveFocusedwindow())
                {
                    cubaseWindow.Focus();
                }
                return true;
            }
            return false;
        }
    }
}
