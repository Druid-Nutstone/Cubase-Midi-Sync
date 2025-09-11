using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Server.Services.CommandCategproes;
using Cubase.Midi.Sync.Server.Services.CommandCategproes.Keys;
using Cubase.Midi.Sync.Server.Services.CommandCategproes.Midi;
using Cubase.Midi.Sync.Server.Services.Commands;
using Cubase.Midi.Sync.Server.Services.Cubase;
using Cubase.Midi.Sync.Server.Services.Keyboard;
using Cubase.Midi.Sync.Server.Services.Midi;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.EventLog;

var builder = WebApplication.CreateBuilder(args);

////if (builder.Environment.IsProduction())
////{
//    builder.Host.UseWindowsService();
////}

//builder.Logging.AddEventLog(new EventLogSettings
//{
//    SourceName = "CubaseMidiSync",    // will appear in Event Viewer
//    LogName = "Application",          // default log
//    MachineName = "."                 // local machine
//});


builder.Logging.AddConsole();

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.CombineLogs = true;
});

builder.Services
      .AddTransient<ICommandService, CommandService>()
      .AddTransient<ICubaseService, CubaseService>()
      .AddTransient<IKeyboardService, KeyboardService>()
      .AddSingleton<IMidiService, MidiService>()   
      .AddKeyedTransient<ICategoryService, CubaseMidiService>(CubaseServiceConstants.MidiService)
      .AddKeyedTransient<ICategoryService, CubaseKeyService>(CubaseServiceConstants.KeyService);


builder.Services.AddControllers();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8014); // Listen on all network interfaces
});

var app = builder.Build();

// Configure the HTTP request pipeline.

// app.UseHttpsRedirection();
app.UseHttpLogging();

app.UseAuthorization();

app.MapControllers();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("CubaseMidiSync service started at {time}", DateTimeOffset.Now);

var midi = app.Services.GetRequiredService<IMidiService>();
midi.Initialise();
app.Run();

public partial class Program { } 
