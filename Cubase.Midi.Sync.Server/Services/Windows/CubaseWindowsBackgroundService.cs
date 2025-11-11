
using Cubase.Midi.Sync.Server.Constants;
using Cubase.Midi.Sync.WindowManager.Models;
using Cubase.Midi.Sync.WindowManager.Services.Win;
using System.Diagnostics;
using System.Text;

namespace Cubase.Midi.Sync.Server.Services.Windows
{
    public class CubaseWindowsBackgroundService : BackgroundService
    {
        private readonly ILogger<CubaseWindowsBackgroundService> logger;

        private readonly ICubaseWindowMonitor cubaseWindowMonitor;

        public CubaseWindowsBackgroundService(ILogger<CubaseWindowsBackgroundService> logger, ICubaseWindowMonitor cubaseWindowMonitor)
        {
            this.logger = logger;
            this.cubaseWindowMonitor = cubaseWindowMonitor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("CubaseWindowsBackgroundService started.");
            try
            {
                await this.Monitorcubasewindows(stoppingToken);
                // Keep the hosted service alive; do not try to modify the web pipeline here.
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException) { /* expected on shutdown */ }
            logger.LogInformation("CubaseWindowsBackgroundService stopping.");
        }

        private async Task Monitorcubasewindows(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {

                var cubaseWindowCollection = WindowPositionCollection.Create("Cubase Windows");

                var cubase = Process.GetProcessesByName(CubaseServerConstants.CubaseExeName).FirstOrDefault();
                if (cubase == null)
                {
                    this.logger.LogError("Cubase is not running");
                }
                else
                {
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
                }
                this.cubaseWindowMonitor?.CubaseWindowEvent(cubaseWindowCollection);
                await Task.Delay(500);

            };
        }

    }
}
