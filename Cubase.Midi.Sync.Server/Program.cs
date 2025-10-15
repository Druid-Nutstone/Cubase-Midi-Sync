using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Server.Constants;
using Cubase.Midi.Sync.Server.Services.Cache;
using Cubase.Midi.Sync.Server.Services.CommandCategproes;
using Cubase.Midi.Sync.Server.Services.CommandCategproes.Keys;
using Cubase.Midi.Sync.Server.Services.CommandCategproes.Midi;
using Cubase.Midi.Sync.Server.Services.Commands;
using Cubase.Midi.Sync.Server.Services.Cubase;
using Cubase.Midi.Sync.Server.Services.Keyboard;
using Cubase.Midi.Sync.Server.Services.Midi;
using Cubase.Midi.Sync.Server.Services.Mixer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.EventLog;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);


builder.Logging.AddConsole();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // reduce noise from framework logs
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} (Thread {ThreadId}){NewLine}{Exception}")
    .WriteTo.File(
        path: $"{CubaseServerConstants.LogFileLocation}/CubaseMidiSyncServerLog.txt",
        rollingInterval: RollingInterval.Infinite,
        retainedFileCountLimit: 1,             
        fileSizeLimitBytes: 10_000_000,         // 10 MB max per file
        rollOnFileSizeLimit: false,
        restrictedToMinimumLevel: LogEventLevel.Information,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj} (Thread {ThreadId}){NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

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
      .AddSingleton<ICacheService, CacheService>()  
      .AddTransient<IMixerService, MixerService>()  
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
var cache = app.Services.GetRequiredService<ICacheService>();
cache.Initialise();

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(() =>
{
    var midi = app.Services.GetRequiredService<IMidiService>();
    midi.Dispose();
});


app.Run();

public partial class Program { } 
