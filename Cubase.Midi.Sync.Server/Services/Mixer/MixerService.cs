using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Midi;
using Cubase.Midi.Sync.Common.Mixer;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.Window;
using Cubase.Midi.Sync.Server.Constants;
using Cubase.Midi.Sync.Server.Services.Cache;
using Cubase.Midi.Sync.Server.Services.CommandCategproes;
using Cubase.Midi.Sync.Server.Services.Midi;
using Cubase.Midi.Sync.Server.Services.Windows;
using Cubase.Midi.Sync.WindowManager.Models;
using Cubase.Midi.Sync.WindowManager.Services.Win;
using static Cubase.Midi.Sync.WindowManager.Services.Win.WindowManagerService;

namespace Cubase.Midi.Sync.Server.Services.Mixer
{
    public class MixerService : IMixerService
    {
        private readonly ICacheService cacheService;

        private readonly IMidiService midiService;

        private readonly IServiceProvider services;

        private readonly ILogger<MixerService> logger;

        private readonly ICubaseWindowMonitor cubaseWindowMonitor;

        private ICategoryService categoryService;

        private CubaseActiveWindowCollection windowsCollection = new CubaseActiveWindowCollection();

        private CubaseMidiCommandCollection cubaseMidiCommands;

        private bool mixersEnabled = false;

        private MixerOrientation currentOrientation = MixerOrientation.Auto;

        public MixerService(ILogger<MixerService> logger,
                            ICacheService cacheService,
                            IMidiService midiService,
                            ICubaseWindowMonitor cubaseWindowMonitor,
                            IServiceProvider services)
        {
            this.cacheService = cacheService;
            this.midiService = midiService;
            this.cubaseWindowMonitor = cubaseWindowMonitor;
            this.logger = logger;
            this.cubaseMidiCommands = new CubaseMidiCommandCollection(CubaseServerConstants.KeyCommandsFileLocation);
            this.services = services;
            this.categoryService = this.services.GetServices<ICategoryService>().FirstOrDefault(x => x.SupportedKeys.Contains(CubaseServiceConstants.KeyService));
            if (this.categoryService == null)
            {
                throw new Exception("Cannot get key service");
            }
            this.cubaseWindowMonitor.RegisterForWindowEvents(this.CubaseWindowsEvent);
        }

        private void CubaseWindowsEvent(CubaseActiveWindowCollection cubaseActiveWindows)
        {
            this.windowsCollection = cubaseActiveWindows;
            if (mixersEnabled == false)
            {
                return;
            }
            if (this.windowsCollection.HaveAnyMixers())
            {
                if (this.cubaseWindowMonitor.GetMixerWindows().Count == 1)
                {
                    var firstMixer = this.cubaseWindowMonitor.GetMixerWindows().FirstOrDefault();
                    if (firstMixer != null)
                    {
                        if (firstMixer.State != WindowManager.Models.WindowState.Maximized)
                        {
                            this.logger.LogInformation("Setting focus to the first mixer window.");
                            firstMixer.Maximise()
                                     .Focus();
                        }
                    }
                }
                else
                {
                    if (this.windowsCollection.CountOfMixers() > 1)
                    {
                        BuildMixerLayout();
                    }
                    else
                    {
                        if (this.windowsCollection.CountOfMixers() == 0)
                        {
                            // No mixers
                            var mainCubaseWindow = this.windowsCollection.GetPrimaryWindow();
                            if (mainCubaseWindow != null)
                            {
                                this.cubaseWindowMonitor.CubaseWindows.GetWindowByName(mainCubaseWindow.Name)
                                       .Maximise()
                                       .Focus();
                            }
                        }
                    }
                }
            }
        }

        public async Task<CubaseMixerCollection> GetMixer()
        {
            this.logger.LogInformation($"MixerService - Loading CubaseMixer Collection. The current Success flag is {this.cacheService.CubaseMixer.Success} with an error message of {this.cacheService.CubaseMixer.ErrorMessage}");
            return this.cacheService.CubaseMixer;
        }

        public async Task<CubaseMixerCollection> MixerCommand(CubaseMixer cubaseMixer)
        {
            if (!string.IsNullOrEmpty(cubaseMixer.KeyAction))
            {
                this.logger.LogInformation($"Executing mixer Key command {cubaseMixer.KeyAction}");
                await this.categoryService.ProcessActionAsync(ActionEvent.Create(CubaseAreaTypes.Midi, cubaseMixer.KeyAction));
            }
            else
            {
                var commands = cacheService.CubaseMixer.GetCommands(cubaseMixer.Command);
                foreach (var cmd in commands)
                {
                    await this.SendMidiCommand(cmd);
                }
            }
            return cacheService.CubaseMixer.WithMixerNames(this.cubaseWindowMonitor.MixerConsoles);
        }

        public async Task<CubaseMixerResponse> MixerRequest(CubaseMixerRequest cubaseMixerRequest)
        {
            this.mixersEnabled = true;
            switch (cubaseMixerRequest.Command)
            {
                case CubaseMixerCommand.Orientation:
                    this.currentOrientation = cubaseMixerRequest.Orientation;
                    BuildMixerLayout();
                    return CubaseMixerResponse.Create(CubaseMixerCommand.Orientation);
                case CubaseMixerCommand.FocusMixer:
                    this.FocusWindow(cubaseMixerRequest.TargetMixer ?? this.cubaseWindowMonitor.MixerConsoles.First());
                    return CubaseMixerResponse.Create(CubaseMixerCommand.FocusMixer);
                case CubaseMixerCommand.MixerStaticCommand:
                    var actionEvent = cubaseMixerRequest.GetData<ActionEvent>();
                    var knownCommand = Enum.Parse<KnownCubaseMidiCommands>(actionEvent.Action);
                    var commandList = this.cacheService.CubaseMixer.GetCommands(knownCommand);
                    var targetWindow = cubaseMixerRequest.TargetMixer ?? this.cubaseWindowMonitor.MixerConsoles.FirstOrDefault();
                    if (!string.IsNullOrEmpty(cubaseMixerRequest.TargetMixer))
                    {
                        this.cubaseWindowMonitor.CubaseWindows
                                                .GetWindowByName(cubaseMixerRequest.TargetMixer)
                                                .Focus()
                                                .WithWindowZorder();
                    }
                    else
                    {
                        if (targetWindow != null)
                        {
                            this.FocusWindow(targetWindow)
                                .WithWindowZorder();
                        } 
                    }
                    foreach (var cmd in commandList)
                    {
                      await this.SendMidiCommand(cmd);
                    }
                    return CubaseMixerResponse.Create(CubaseMixerCommand.MixerStaticCommand);
                case CubaseMixerCommand.OpenMixer:
                    await this.AddOrFocusMixer(cubaseMixerRequest.TargetMixer ?? string.Empty);
                    await this.SendMidiCommand(this.cubaseMidiCommands.GetMidiCommandByName(KnownCubaseMidiCommands.Show_Tracks_With_Data));
                    return CubaseMixerResponse.Create(CubaseMixerCommand.MixerCollection, await GetMixer());
                case CubaseMixerCommand.RestoreMixers:
                    BuildMixerLayout();
                    return CubaseMixerResponse.Create(CubaseMixerCommand.RestoreMixers);
                case CubaseMixerCommand.MixerCollection:
                    return CubaseMixerResponse.Create(CubaseMixerCommand.MixerCollection, await GetMixer());
                case CubaseMixerCommand.CloseMixers:
                    this.mixersEnabled = false;
                    var mixerWindows = this.cubaseWindowMonitor.GetMixerWindows();
                    this.logger.LogInformation($"Closing {mixerWindows.Count} mixer windows. {string.Join(';', mixerWindows.Select(x => x.Name))}");
                    foreach (var mixer in mixerWindows)
                    {
                        if (!mixer.Close())
                        {
                            this.logger.LogWarning($"Failed to close mixer window {mixer.Name}");
                        }
                    }
                    var mainCubaseWindowToRestore = this.cubaseWindowMonitor.CubaseWindows.GetPrimaryWindow();
                    mainCubaseWindowToRestore?.Restore()
                                              .Maximise()
                                              .Focus();
                    await this.SendMidiCommand(this.cubaseMidiCommands.GetMidiCommandByName(KnownCubaseMidiCommands.Show_All_Tracks));
                    return CubaseMixerResponse.Create(CubaseMixerCommand.CloseMixers);
                case CubaseMixerCommand.ProjectWindow:
                    this.mixersEnabled = false;
                    var mainCubaseWindow = this.cubaseWindowMonitor.CubaseWindows.GetPrimaryWindow();

                    if (mainCubaseWindow != null)
                    {
                        this.cubaseWindowMonitor.CubaseWindows
                                       .GetActiveWindows()
                                       .Where(w => w.Name.StartsWith("MixConsole"))
                                       .ToList()
                                       .ForEach(w => w.Minimise());

                        mainCubaseWindow?.Restore()
                                        .Maximise()  
                                        .Focus();
                    }
                    return CubaseMixerResponse.Create(CubaseMixerCommand.ProjectWindow);
                case CubaseMixerCommand.SyncMixer:
                    await this.SendMidiCommand(this.cubaseMidiCommands.GetMidiCommandByName(KnownCubaseMidiCommands.Sync_Mixer));
                    return CubaseMixerResponse.Create(CubaseMixerCommand.SyncMixer);
                default:
                    return CubaseMixerResponse.CreateError("Unknown Mixer command.");
            }
        }

        private async Task AddOrFocusMixer(string mixerName)
        {
            this.cubaseWindowMonitor.CubaseWindows.GetPrimaryWindow()
                                                  .MoveOffScreen();
            if (this.cubaseWindowMonitor.MixerExists(mixerName))
            {
                this.FocusWindow(mixerName);
                return;
            }
            else
            {
                var currentMixerCount = cubaseWindowMonitor.MixerConsoles.Count == 0 ? 1 : cubaseWindowMonitor.MixerConsoles.Count;
                var midiCommand = KnownCubaseMidiCommands.Mixer;
                var windowBits = mixerName.Split(' ');
                if (windowBits.Length >= 2 && int.TryParse(windowBits[1], out int mixerNumber))
                {
                    switch (mixerNumber)
                    {
                        case 2:
                            midiCommand = KnownCubaseMidiCommands.Mixer_2;
                            break;
                        case 3:
                            midiCommand = KnownCubaseMidiCommands.Mixer_3;
                            break;
                        case 4:
                            midiCommand = KnownCubaseMidiCommands.Mixer_4;
                            break;
                    }
                    currentMixerCount++;
                }
                await this.SendMidiCommand(this.cubaseMidiCommands.GetMidiCommandByName(midiCommand));
                await WaitForMixConsoleCount(currentMixerCount);
                if (currentMixerCount > 1)
                {
                    BuildMixerLayout();
                    // this.FocusWindow(mixerName);
                }
                else
                {
                    var mixerWindow = this.cubaseWindowMonitor.CubaseWindows.GetWindowThatStartsWith(mixerName);
                    if (mixerWindow != null)
                    {
                        mixerWindow.Maximise()
                                   .Focus();
                    }
                }
            }
        }

        private void BuildMixerLayout()
        {
            // ---- GUARDS (critical) --------------------------------------------

            // No mixer windows detected yet – valid transient state
            if (this.cubaseWindowMonitor.MixerConsoles.Count == 0)
            {
                this.logger.LogDebug("BuildMixerLayout skipped: no mixer consoles detected");
                return;
            }

            // No screens detected – extremely rare but possible during display changes
            var screens = WindowManagerService.GetAllMonitors();
            if (screens == null || !screens.Any())
            {
                this.logger.LogWarning("BuildMixerLayout skipped: no monitors detected");
                return;
            }

            int border = 7;
            int minMixerHeight = 487;

            // -------------------------------------------------------------------

            // todo - if there are multiple screens , need some way of knowing which
            // screen the mixers should be on
            var primaryScreen = screens
                .Select(x => x.WorkRect)
                .OrderByDescending(r => (long)r.Width * r.Height)
                .First();

            // Screen sanity check
            if (primaryScreen.Width <= 0 || primaryScreen.Height <= 0)
            {
                this.logger.LogWarning("BuildMixerLayout skipped: invalid primary screen dimensions");
                return;
            }

            int mixerCount = this.cubaseWindowMonitor.MixerConsoles.Count;

            // Decide layout mode safely
            MixerLayoutMode mixerLayoutMode =
                (primaryScreen.Height / mixerCount) < minMixerHeight
                    ? MixerLayoutMode.ByWidth
                    : MixerLayoutMode.ByHeight;

            if (this.currentOrientation != MixerOrientation.Auto)
            {
                mixerLayoutMode =
                    this.currentOrientation == MixerOrientation.Vertical
                        ? MixerLayoutMode.ByWidth
                        : MixerLayoutMode.ByHeight;
            }

            // Expand screen slightly to allow for borders
            primaryScreen.Left -= border;
            primaryScreen.Right += border * 2;

            var currentTop = primaryScreen.Top;
            var currentLeft = primaryScreen.Left;

            int heightPerWindow = mixerLayoutMode == MixerLayoutMode.ByHeight
                ? primaryScreen.Height / mixerCount
                : primaryScreen.Height + border;

            int widthPerWindow = mixerLayoutMode == MixerLayoutMode.ByWidth
                ? primaryScreen.Width / mixerCount
                : primaryScreen.Width;

            var mixerWindows = this.cubaseWindowMonitor.GetMixerWindows();
            if (mixerWindows == null || mixerWindows.Count == 0)
            {
                this.logger.LogDebug("BuildMixerLayout skipped: no mixer windows returned");
                return;
            }

            // -------------------------------------------------------------------

            mixerWindows.ForEach(mixer =>
            {
                if (mixer.State == WindowManager.Models.WindowState.Minimized)
                {
                    mixer.Restore();
                }

                var targetRect = new Rect
                {
                    Left = primaryScreen.Left,
                    Top = currentTop,
                    Right = primaryScreen.Right,
                    Bottom = currentTop + heightPerWindow
                };

                if (mixerLayoutMode == MixerLayoutMode.ByWidth)
                {
                    targetRect.Left = currentLeft;
                    targetRect.Right = currentLeft + (widthPerWindow + border);
                    targetRect.Top = primaryScreen.Top;
                    targetRect.Bottom = primaryScreen.Bottom;
                }

                mixer.RestoreResize(targetRect).Refresh();

                currentTop += heightPerWindow;
                currentLeft += (widthPerWindow - border);
            });
        }


        private WindowPosition FocusWindow(string windowName)
        {
            var window = this.cubaseWindowMonitor.CubaseWindows.GetWindowThatStartsWith(windowName);
            if (window != null)
            {
                return window.Focus();
            }
            return null;
        }

        private async Task<bool> WaitForMixConsoleCount(int consoleCount)
        {
            var maxCount = 10000;
            var currentCount = 0;
            while (this.cubaseWindowMonitor.MixerConsoles.Count < consoleCount)
            {
                currentCount++;
                await Task.Run(() => Task.Delay(50));
                if (currentCount >= maxCount)
                {
                    return false;
                }
            }
            return true;
        }

        private async Task SendMidiCommand(CubaseMidiCommand cmd)
        {
            await Task.Run(() => this.midiService.SendMidiMessage(cmd));
            // await Task.Delay(20); // Small delay to ensure Cubase processes the command
        }
    }

    public enum MixerLayoutMode
    {
        ByHeight,
        ByWidth
    }
}
