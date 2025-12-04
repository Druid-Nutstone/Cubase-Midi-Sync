
using Cubase.Midi.Sync.Common.WebSocket;
using Cubase.Midi.Sync.Server.Constants;
using Cubase.Midi.Sync.Server.Services.Cache;
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

        private readonly ICacheService cacheService;

        public CubaseWindowsBackgroundService(ILogger<CubaseWindowsBackgroundService> logger, 
                                              ICubaseWindowMonitor cubaseWindowMonitor,
                                              ICacheService cacheService,
                                              IWebSocketServer webSocketServer)
        {
            this.logger = logger;
            this.cacheService = cacheService;
            this.webSocketServer = webSocketServer;
            this.cubaseWindowMonitor = cubaseWindowMonitor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("CubaseWindowsBackgroundService started.");
            try
            {
                var windowMonitor = this.Monitorcubasewindows(stoppingToken);
                var fileMonitor = this.MonitorChangedFiles(stoppingToken);

                await Task.WhenAll(windowMonitor, fileMonitor);
                
                // Keep the hosted service alive; do not try to modify the web pipeline here.
                // await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException) { /* expected on shutdown */ }
            logger.LogInformation("CubaseWindowsBackgroundService stopping.");
        }

        private async Task MonitorChangedFiles(CancellationToken stoppingToken)
        {
            this.logger.LogInformation("Starting file monitor");
            while (!stoppingToken.IsCancellationRequested)
            {
                await this.cacheService.RefreshCubaseMixer();
                await this.cacheService.RefreshMidiAndKeys();
                await Task.Delay(5 * 1000); 
            }
        }

        private async Task Monitorcubasewindows(CancellationToken stoppingToken)
        {
            this.logger.LogInformation("Starting cubase window monitor");
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
