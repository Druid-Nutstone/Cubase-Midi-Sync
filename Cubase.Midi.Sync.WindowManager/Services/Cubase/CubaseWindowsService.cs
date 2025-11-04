using Cubase.Midi.Sync.WindowManager.Models;
using Cubase.Midi.Sync.WindowManager.Services.Win;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.WindowManager.Services.Cubase
{
    public class CubaseWindowsService : ICubaseWindowsService
    {
        private WindowPositionCollection cubaseWindows;

        public bool Cancel { get; set; } = false;

        public CubaseWindowsService()
        {

        } 
    
        public void Initialise(WindowPositionCollection cubaseWindows)
        {
            this.cubaseWindows = cubaseWindows;
            this.GetCubaseWindows();
            var activeCubaseWindows = WindowManagerService.EnumerateWindows()
                                                          .GetWindows(this.cubaseWindows.GetWindowNames());
            foreach (var cubaseWin in activeCubaseWindows)
            {
                this.cubaseWindows.SetCurrentPosition(cubaseWin.Item1, cubaseWin.Item2);
            }
        }

        public Task WaitForCubaseWindows()
        {
            var runTask = Task.Run(async () => 
            {
                while (!this.Cancel)
                {
                    // get all windows 
                    var allWindows = WindowManagerService.EnumerateWindows();
                    this.cubaseWindows.ResetWindowState();
                    this.GetCubaseWindows();
                    this.cubaseWindows.ClearPositionsThatHaveClosed();
                    this.ArrangeWindows(this.cubaseWindows.GetPrimaryWindow(), this.cubaseWindows.GetActiveWindows());
                    await Task.Delay(1000);
                }
            });
            return runTask;
        }

        private void ArrangeWindows(WindowPosition primaryWindow, List<WindowPosition> otherWindows)
        {
            if (!otherWindows.Any(x => x.State == WindowState.Maximized))
            {
                if (primaryWindow.Hwnd != IntPtr.Zero)
                {
                    if (otherWindows.Count == 0)
                    {
                        if (primaryWindow.State != WindowState.Maximized)
                        {
                            WindowManagerService.SetWindowToMax(primaryWindow.Hwnd);
                        }
                        return;
                    }
                    // todo - need to parameterise the primary width - by default it's two thirds 
                    WindowPositionManager.Instance.ArrangeWindows(primaryWindow, otherWindows, WindowPositionManager.Instance.PrimaryScreen.TwoThirdsWidth);
                }
            }
            return;
        }

        private void GetCubaseWindows()
        {
            var activeCubaseWindows = WindowManagerService.EnumerateWindows()
                                                          .GetWindows(this.cubaseWindows.GetWindowNames());
            foreach (var cubaseWin in activeCubaseWindows)
            {
                this.cubaseWindows.SetCurrentPosition(cubaseWin.Item1, cubaseWin.Item2);
            }
        } 
    
    }
}
