// See https://aka.ms/new-console-template for more information


//using Cubase.Midi.Sync.Common;
//using Cubase.Midi.Sync.Common.Colours;
//using Cubase.Midi.Sync.Common.Keys;


//var collection = new CubaseCommandsCollection();
//collection.WithNewCubaseCommand("Transport", CubaseServiceConstants.KeyService)
//    .WithNewCubaseCommand(CubaseCommand.CreateToggleButton("Play", "Space"));
//    /*
//    .WithNewCubaseCommand(CubaseCommand.CreateStandardButton("Go To Start", CubaseActionName.TransportGoToStartStart))
//    .WithNewCubaseCommand(CubaseCommand.CreateToggleButton("⮝ Punch In", CubaseActionName.TransportPunchIn))
//    .WithNewCubaseCommand(CubaseCommand.CreateToggleButton("Punch Out", CubaseActionName.TransportPunchOut))
//    .WithNewCubaseCommand(CubaseCommand.CreateStandardButton("Left Locator", CubaseActionName.TransportLeftLocator))
//    .WithNewCubaseCommand(CubaseCommand.CreateStandardButton("Right Locator", CubaseActionName.TransportRightLocator))
//    .WithNewCubaseCommand(CubaseCommand.CreateToggleButton("Record", CubaseActionName.TransportRecord).WithToggleBackGroundColour(ColourConstants.ButtonRecordBackground).WithToggleForeColour(ColourConstants.ButtonRecordText));
//*/
//var targetFile = Path.Combine("C:\\Dev\\Cubase-Midi-Sync\\Cubase.Midi.Sync.Server\\", "CubaseCommands.json");
//collection.SaveToFile(targetFile);

/*
var keys = CubaseMappingCollection.Create().AddMapping("Space", CubaseActionName.TransportPlay);
keys.SaveToFile(Path.Combine("C:\\Dev\\Cubase-Midi-Sync\\Cubase.Midi.Sync.Server\\", "CubaseKeyMap.json"));
*/

//var commands = CubaseKeyCommandParser.Create().Parse();

//var allocated = commands.GetAllocated();  

//var categories = commands.GetCategories();

//var allKeys = commands.GetKeys();

//var locator = commands.GetByName("locator");

//File.WriteAllLines("C:\\Deleteme\\AllKeys.txt", allKeys);

//var size = allocated.Count; 