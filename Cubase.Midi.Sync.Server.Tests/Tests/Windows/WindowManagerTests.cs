using Cubase.Midi.Sync.WindowManager.Models;
using Cubase.Midi.Sync.WindowManager.Services.Cubase;
using Cubase.Midi.Sync.WindowManager.Services.Win;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Server.Tests.Tests.Windows
{
    [TestClass]
    public class WindowManagerTests
    {
        private string testFileName = "C:\\deleteme\\testwindows.json";
        
        [TestMethod]
        public void Can_Get_Cubase()
        {
            var handle = WindowManagerService.FindWindowByTitle("Cubase Version");
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
        public void Can_Manage_Cubase_windows()
        {
            var cubaseWindowCollection = WindowPositionCollection.Create("cubase Windows")
                                              .WithWindowPosition(WindowPosition.Create("Cubase Version").WithWindowType(WindowType.Primary))
                                              .WithWindowPosition(WindowPosition.Create("MixConsole").WithWindowType(WindowType.Secondary))
                                              .WithWindowPosition(WindowPosition.Create("Channel Settings").WithWindowType(WindowType.Transiant));
            var cubaseWinService = new CubaseWindowsService();
            // cubaseWinService.Initialise(cubaseWindowCollection);
            var tsk = cubaseWinService.WaitForCubaseWindows();
            tsk.Wait();
        }
    }
}
