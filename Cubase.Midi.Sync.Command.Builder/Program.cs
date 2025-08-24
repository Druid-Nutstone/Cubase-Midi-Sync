// See https://aka.ms/new-console-template for more information


using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Keys;

var collection = new CubaseCommandsCollection();
collection.WithNewCubaseCommand("Transport", CubaseAreaName.Transport)
    .WithNewCubaseCommand("Play", CubaseActionName.TransportPlay, CubaseButtonType.Toggle)
    .WithNewCubaseCommand("Go To Start", CubaseActionName.TransportGoToStartStart)
    .WithNewCubaseCommand("Record", CubaseActionName.TransportRecord, CubaseButtonType.Toggle)
    .WithNewCubaseCommand("Left Locator", CubaseActionName.TransportLeftLocator)
    .WithNewCubaseCommand("Right Locator", CubaseActionName.TransportRightLocator)
    .WithNewCubaseCommand("Loop", CubaseActionName.Loop, CubaseButtonType.Toggle);

var targetFile = Path.Combine("C:\\Dev\\Cubase-Midi-Sync\\Cubase.Midi.Sync.Server\\", "CubaseCommands.json");
collection.SaveToFile(targetFile);

var keys = CubaseMappingCollection.Create().AddMapping(VirtualKey.Space, CubaseActionName.TransportPlay);
keys.SaveToFile(Path.Combine("C:\\Dev\\Cubase-Midi-Sync\\Cubase.Midi.Sync.Server\\", "CubaseKeyMap.json"));
             