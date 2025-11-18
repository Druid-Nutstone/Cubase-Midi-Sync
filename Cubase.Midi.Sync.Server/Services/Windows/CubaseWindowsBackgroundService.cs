
using Cubase.Midi.Sync.Common.WebSocket;
using Cubase.Midi.Sync.Server.Constants;
using Cubase.Midi.Sync.Server.Services.WebSockets;
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

        private readonly IWebSocketServer webSocketServer;

        public CubaseWindowsBackgroundService(ILogger<CubaseWindowsBackgroundService> logger, 
                                              ICubaseWindowMonitor cubaseWindowMonitor,
                                              IWebSocketServer webSocketServer)
        {
            this.logger = logger;
            this.webSocketServer = webSocketServer;
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
            var statusChange = false;
            while (!stoppingToken.IsCancellationRequested)
            {
                var cubaseIsRunning = true;
                var cubaseWindowCollection = WindowPositionCollection.Create("Cubase Windows");

                var cubase = Process.GetProcessesByName(CubaseServerConstants.CubaseExeName).FirstOrDefault();
                if (cubase == null)
                {

                    if (cubaseIsRunning)
                    {
                        this.logger.LogError("Cubase is not running");
                        this.webSocketServer.BroadcastMessage(WebSocketMessage.Create(WebSocketCommand.CubaseNotReady));
                        cubaseIsRunning = false;
                    }
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
                                                               .WithWindowType(this.GetwindowTypeFromTitle(windowTitle));
                            cubaseWindowCollection.WithWindowPosition(windowPosition)
                                                  .SetCurrentPosition(hwnd, windowTitle);

                        }
                    }
                }
                if (cubaseWindowCollection.GetPrimaryWindow() == null)
                {
                    this.logger.LogWarning("No primary Cubase window found.");
                    this.webSocketServer.BroadcastMessage(WebSocketMessage.Create(WebSocketCommand.CubaseNotReady));
                    statusChange = false;
                }
                else
                {
                    if (!statusChange)
                    {
                        this.logger.LogInformation("Cubase is running and primary window found.");
                        this.webSocketServer.BroadcastMessage(WebSocketMessage.Create(WebSocketCommand.CubaseReady));
                        statusChange = true;
                    }
                }
                this.cubaseWindowMonitor?.CubaseWindowEvent(cubaseWindowCollection);
                await Task.Delay(200);

            };
        }

        private WindowType GetwindowTypeFromTitle(string title)
        {
            if (title.StartsWith("Cubase Pro Project", StringComparison.OrdinalIgnoreCase) ||
                title.StartsWith("Cubase Artist", StringComparison.OrdinalIgnoreCase) ||
                title.StartsWith("Cubase LE", StringComparison.OrdinalIgnoreCase) ||
                title.StartsWith("Cubase Version", StringComparison.OrdinalIgnoreCase) ||
                title.StartsWith("Cubase Elements", StringComparison.OrdinalIgnoreCase))  
            {
                return WindowType.Primary;
            }
            return WindowType.Transiant;
        }
    }
}
