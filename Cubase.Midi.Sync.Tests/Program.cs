// See https://aka.ms/new-console-template for more information
using Cubase.Midi.Sync.Common.Midi;
using Cubase.Sync.Midi.Driver;
using System.Runtime.CompilerServices;

var driverIn = new NutstoneDriver("Nutstone");

Console.WriteLine($"is generic assinged ?");
Console.ReadKey();

var midiCommands = GenericMidiModel.LoadFromFile();

driverIn.MidiMessageReceived += DriverIn_MidiMessageReceived;

void DriverIn_MidiMessageReceived(byte[] message)
{
    byte status = message[0];
    byte note = message[1];
    byte value = message[2];

    byte status1 = message[3];
    byte note1 = message[4];
    byte value1 = message[5];

    Console.WriteLine($"Status:{status}  Note:{note}  Value:{value}  Status1:{status1} Note1: {note1} Value1:{value1}");
}


//var const_Solo = 0x08;
//Console.WriteLine("Press to send");
//Console.ReadKey();
//driverIn.SendNoteOn(1, 2, 127);

var cmdReceived = string.Empty;
ShowCommands();
while (cmdReceived != "q")
{
    Thread.Sleep(500);
    if (Console.KeyAvailable)
    {
        var key = Console.ReadKey();
        cmdReceived = key.KeyChar.ToString().ToLower();
        if (cmdReceived != "q")
        {
            var index = int.Parse(cmdReceived); 
            Console.WriteLine($"  Sending {midiCommands[index].Name}");
            driverIn.SendNoteOn(midiCommands[index].Channel, midiCommands[index].Addr, 127);
        }
    }
}

void ShowCommands()
{
    Console.Clear();
    for (int i = 0; i < midiCommands.Count; i++)
    {
        Console.WriteLine($"({i})  {midiCommands[i].Name}");
    }
}


