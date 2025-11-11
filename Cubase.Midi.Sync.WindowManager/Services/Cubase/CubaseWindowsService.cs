using Cubase.Midi.Sync.WindowManager.Models;
using Cubase.Midi.Sync.WindowManager.Services.Win;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    
        public WindowPositionCollection GetCubaseWindows(Process cubase)
        {
            var cubaseWindowCollection = WindowPositionCollection.Create("Cubase Windows");
            var allProcesses = new List<Process> { cubase };
            allProcesses.AddRange(WindowManagerService.GetChildProcesses(cubase));
            foreach (var proc in allProcesses)
            {
                var windows = WindowManagerService.GetWindowsForProcess(proc.Id);
                foreach (var hwnd in windows)
                {
                    var title = new StringBuilder(256);
                    WindowManagerService.GetWindowText(hwnd, title, title.Capacity);
                    var windowTitle = title.ToString();
                    var windowPosition = WindowPosition.Create(windowTitle, hwnd)
                                                       .WithWindowType(windowTitle.Contains("Cubase Pro Project", StringComparison.OrdinalIgnoreCase) ? WindowType.Primary : WindowType.Transiant);

                    cubaseWindowCollection.WithWindowPosition(windowPosition)
                                          .SetCurrentPosition(hwnd, windowTitle);
                }
            }
            return cubaseWindowCollection;
        }


        public Task WaitForCubaseWindows()
        {
            var runTask = Task.Run(async () => 
            {
                while (!this.Cancel)
                {
                    this.SetWindowPositions();
                    await Task.Delay(1000);
                }
            });
            return runTask;
        }

        public void SetWindowPositions()
        {
            var allWindows = WindowManagerService.EnumerateWindows();
            this.cubaseWindows.ResetWindowState();
            this.GetCubaseWindows();
            this.cubaseWindows.ClearPositionsThatHaveClosed();
            this.ArrangeWindows(this.cubaseWindows.GetPrimaryWindow(), this.cubaseWindows.GetActiveWindows());
        }
        
        public void ArrangeWindows(WindowPosition primaryWindow, List<WindowPosition> otherWindows)
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
