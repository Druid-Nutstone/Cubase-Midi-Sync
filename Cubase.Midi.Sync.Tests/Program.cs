// See https://aka.ms/new-console-template for more information
using Cubase.Midi.Sync.Common.Midi;
using Cubase.Sync.Midi.Driver;
using System.Runtime.CompilerServices;

NutstoneDriver driver = null;


    driver = new NutstoneDriver("Nutstone");
    driver.MidiMessageReceived += (message) =>
    {
        int index = 0;
        while (index + 2 < message.Length) // at least 3 bytes remaining
        {
            byte status = message[index];
            byte data1 = message[index + 1];
            byte data2 = message[index + 2];

            int command = status & 0xF0;
            int channel = status & 0x0F;

            switch (command)
            {
                case 0x80:
                    Console.WriteLine($"Note Off - Ch:{channel} Note:{data1} Vel:{data2}");
                    break;
                case 0x90:
                    Console.WriteLine($"Note On  - Ch:{channel} Note:{data1} Vel:{data2}");
                    break;
                case 0xA0:
                    Console.WriteLine($"Poly Aftertouch - Ch:{channel} Note:{data1} Value:{data2}");
                    break;
                case 0xB0:
                    Console.WriteLine($"Control Change - Ch:{channel} CC:{data1} Value:{data2}");
                    break;
                default:
                    Console.WriteLine($"Other Status {status:X2} Data1:{data1} Data2:{data2}");
                    break;
            }

            index += 3; // move to the next message
        }
    };



Console.WriteLine($"is generic assinged ?");
Console.ReadKey();

var midiCommands = new CubaseMidiCommandCollection();


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
            driver.SendNoteOn(midiCommands[index].Channel, midiCommands[index].Note, midiCommands[index].Velocity);
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


