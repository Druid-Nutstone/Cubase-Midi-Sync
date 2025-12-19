using Cubase.Midi.Sync.Common.Window;
using Cubase.Midi.Sync.WindowManager.Models;

namespace Cubase.Midi.Sync.Server.Services.Windows
{
    public interface ICubaseWindowMonitor
    {

        void RegisterForWindowEvents(Action<CubaseActiveWindowCollection> windowEventHandler);

        void UnRegisterForWindowEvents(Action<CubaseActiveWindowCollection> windowEventHandler);

        void CubaseWindowEvent(WindowPositionCollection cubaseWindows);
        
        WindowPositionCollection CubaseWindows { get; set; }

        bool HaveAtLeastOneMixer { get; }

        bool WindowExists(string windowName);

        bool MixerExists(string mixerName);

        List<string> MixerConsoles { get; }

        List<WindowPosition> GetMixerWindows();

        bool FocusCubase(Action<string> errorHandler);

        bool HaveAtLeastOneCubaseWindowFocused();
    }
}
