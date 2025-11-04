using Cubase.Midi.Sync.WindowManager.Models;
using Cubase.Midi.Sync.WindowManager.Services.Cubase;
using Cubase.Midi.Sync.WindowManager.Services.Win;
using System;
using System.Collections.Generic;
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
        public void Can_Manage_Cubase_windows()
        {
            var cubaseWindowCollection = WindowPositionCollection.Create("cubase Windows")
                                              .WithWindowPosition(WindowPosition.Create("Cubase Version").WithWindowType(WindowType.Primary))
                                              .WithWindowPosition(WindowPosition.Create("MixConsole").WithWindowType(WindowType.Secondary))
                                              .WithWindowPosition(WindowPosition.Create("Channel Settings").WithWindowType(WindowType.Transiant));
            var cubaseWinService = new CubaseWindowsService();
            cubaseWinService.Initialise(cubaseWindowCollection);
            var tsk = cubaseWinService.WaitForCubaseWindows();
            tsk.Wait();
        }
    }
}
