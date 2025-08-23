// See https://aka.ms/new-console-template for more information
using Cubase.Sync.Midi.Midi;
using System.Runtime.CompilerServices;

Console.WriteLine("Loading Midi");

using var midi = new MidiManager("Nutstone IN", "Nutstone OUT");
var mapper = MidiMapper.LoadFromJson();
var controller = new MidiController(midi, mapper);
var jsonAsString = mapper.AsJson();
Console.WriteLine("Load Cubase");
Console.ReadKey();


// controller.SendJsonMapping(jsonAsString);  
// Switch to Transport page
// controller.SwitchPage("Transport");
// controller.SendCC(0, 1, 127); 
// play start ?
// add audio mono 
// controller.SendNote(0, 60, 127); // Send Note On for note 4 (C#) on channel 0 with velocity 127

// Send Play button
// controller.SendButton("Play", 127);
Thread.Sleep(200);
//controller.SendButton("Play", 0);

// Listen for Cubase feedback
var cts = new CancellationTokenSource();

var task = Task.Run(() =>
{
    var state = true;
    try
    {
        while (state)
        {
            // this is a blocking call  
            var msg = midi.WaitForMessage(cts.Token);

            Console.WriteLine($"CC{msg.CC} = {msg.Value}");
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.Escape)
            {
                state = false;
            } 
        }
    }
    catch (OperationCanceledException) { }
});
task.Wait();

midi.Dispose();
cts.Cancel();
