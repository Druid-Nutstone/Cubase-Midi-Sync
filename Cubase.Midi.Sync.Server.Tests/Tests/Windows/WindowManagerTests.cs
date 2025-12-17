using Cubase.Midi.Sync.Server.Constants;
using Cubase.Midi.Sync.WindowManager.Models;
using Cubase.Midi.Sync.WindowManager.Services.Cubase;
using Cubase.Midi.Sync.WindowManager.Services.Win;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Server.Tests.Tests.Windows
{
    [TestClass]
    public class WindowManagerTests
    {
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        public const uint GW_OWNER = 4;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool EnumChildWindows(IntPtr hWnd, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private string testFileName = "C:\\deleteme\\testwindows.json";
        
        [TestMethod]
        public void Can_Get_Cubase()
        {
            var handle = WindowManagerService.FindWindowByTitle("Cubase Version");
        }

        [TestMethod] 
        public void Can_Enum_Cubase()
        {
            var cubaseMain = WindowManagerService.FindWindowByTitle("Cubase Pro Project");

            // Get Cubase PID
            _ = GetWindowThreadProcessId(cubaseMain.Value, out uint cubasePid);

            var results = new List<(IntPtr Hwnd, string Title, string Class)>();

            // 1. Get ALL top-level windows belonging to Cubase.exe
            EnumWindows((hWnd, _) =>
            {
                _ = (nint)GetWindowThreadProcessId(hWnd, out uint pid);
                if (pid != cubasePid)
                    return true;

                // Now scan ALL children recursively
                EnumChildWindows(hWnd, (child, _) =>
                {
                    StringBuilder cls = new(256);
                    GetClassName(child, cls, cls.Capacity);
                    string className = cls.ToString();

                    int len = GetWindowTextLength(child);
                    StringBuilder sb = new(len + 1);
                    GetWindowText(child, sb, sb.Capacity);
                    string title = sb.ToString();

                    results.Add((child, title, className));
                    return true;

                }, IntPtr.Zero);

                return true;
            }, IntPtr.Zero);

            var r = results;
        }

        [TestMethod]
        public void Can_Get_Cubase_Children()
        {
            var cubase = WindowManagerService.FindWindowByTitle("Cubase Pro Project");
            var childeren = WindowManagerService.GetChildWindows(cubase.Value);
            foreach (var child in childeren)
            {
                var txt = child.Title;
                var hwnd = child.Hwnd;
                var className = child.ClassName;
                var evenMore = WindowManagerService.GetChildWindows(hwnd);
            }
        }


        [TestMethod]
        public void Test_can_set_to_original_Positions()
        {
            WindowPositionCollection.Load(testFileName).SetToCurrentPositions();
        }

        [TestMethod]
        public void Can_Get_Processes_From_Processes()
        {

            var cubaseWindowCollection = WindowPositionCollection.Create("Cubase Windows");
            
            var cubase = Process.GetProcessesByName("Cubase15").FirstOrDefault();
            if (cubase == null)
            {
                Console.WriteLine("Cubase 15 not running.");
                return;
            }

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
                    
                    Console.WriteLine($"PID {proc.Id} - {proc.ProcessName} → {title}");
                }
            }
            var cubaseWinService = new CubaseWindowsService();
            cubaseWinService.ArrangeWindows(cubaseWindowCollection.GetPrimaryWindow(), cubaseWindowCollection.GetActiveWindows());
            //cubaseWinService.Initialise(cubaseWindowCollection);
            //cubaseWinService.SetWindowPositions();
        }

        [TestMethod]
        public void Can_Set_Window_Positions()
        {
            var cubaseWindowCollection = WindowPositionCollection.Create("cubase Windows")
                                              .WithWindowPosition(WindowPosition.Create("Cubase Version").WithWindowType(WindowType.Primary))
                                              .WithWindowPosition(WindowPosition.Create("MixConsole").WithWindowType(WindowType.Secondary))
                                              .WithWindowPosition(WindowPosition.Create("Channel Settings").WithWindowType(WindowType.Transiant));
            var cubaseWinService = new CubaseWindowsService();
            // cubaseWinService.Initialise(cubaseWindowCollection);
            cubaseWinService.SetWindowPositions();
        }
        
        [TestMethod]
        public void Can_get_Monitor_sizes()
        {
            var monitorSizes = WindowManagerService.GetAllMonitors();
        }

        [TestMethod]
        public async Task Can_Manage_Cubase_windows()
        {
            var cubaseWindowCollection = WindowPositionCollection.Create("cubase Windows")
                                              .WithWindowPosition(WindowPosition.Create("Cubase Version").WithWindowType(WindowType.Primary))
                                              .WithWindowPosition(WindowPosition.Create("MixConsole").WithWindowType(WindowType.Secondary))
                                              .WithWindowPosition(WindowPosition.Create("Channel Settings").WithWindowType(WindowType.Transiant));
            var cubaseWinService = new CubaseWindowsService();
            // cubaseWinService.Initialise(cubaseWindowCollection);
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(2));
            await cubaseWinService.StartAsync(cts.Token);
        }
    }
}
