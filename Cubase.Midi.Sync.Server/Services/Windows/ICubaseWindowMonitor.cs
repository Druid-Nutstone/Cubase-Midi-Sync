using Cubase.Midi.Sync.Common.Window;
using Cubase.Midi.Sync.WindowManager.Models;

namespace Cubase.Midi.Sync.Server.Services.Windows
{
    public interface ICubaseWindowMonitor
    {

        void RegisterForWindowEvents(Action<CubaseActiveWindowCollection> windowEventHandler);

        void CubaseWindowEvent(WindowPositionCollection cubaseWindows);
        
        WindowPositionCollection CubaseWindows { get; set; }

        bool HaveAtLeastOneMixer { get; }

        List<string> MixerConsoles { get; }

        bool FocusCubase(Action<string> errorHandler);
    }
}
