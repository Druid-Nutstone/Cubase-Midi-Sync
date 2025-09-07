// See https://aka.ms/new-console-template for more information
using Cubase.Sync.Midi.Driver;
using System.Runtime.CompilerServices;

var driverIn = new NutstoneDriver("Nutstone IN");
var driverOut = new NutstoneDriver("Nutstone OUT");


Console.WriteLine("Open cubase");
Console.ReadKey();

var const_Solo = 0x08;

driverIn.SendNoteOn(0, const_Solo, 64);

Console.ReadKey();

