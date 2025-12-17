using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.WebSocket;
using Cubase.Midi.Sync.Server.Constants;
using Cubase.Midi.Sync.Server.Services.Cache;
using Cubase.Midi.Sync.Server.Services.CommandCategproes;
using Cubase.Midi.Sync.Server.Services.CommandCategproes.Keys;
using Cubase.Midi.Sync.Server.Services.CommandCategproes.Midi;
using Cubase.Midi.Sync.Server.Services.CommandCategproes.Script;
using Cubase.Midi.Sync.Server.Services.Commands;
using Cubase.Midi.Sync.Server.Services.Cubase;
using Cubase.Midi.Sync.Server.Services.Keyboard;
using Cubase.Midi.Sync.Server.Services.Midi;
using Cubase.Midi.Sync.Server.Services.Mixer;
using Cubase.Midi.Sync.Server.Services.WebSockets;
using Cubase.Midi.Sync.Server.Services.Windows;
using Cubase.Midi.Sync.WindowManager.Services.Cubase;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.EventLog;
using Serilog;
using Serilog.Events;
using System.Diagnostics;


var builder = WebApplication.CreateBuilder(args);

var app = Configure(builder);

var (midi, wsServer) = ConfigureApp(app);

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(() =>
{
    midi.Dispose();
    _ = SendClosingMessageAsync(wsServer);
});

AppDomain.CurrentDomain.ProcessExit += (_, __) =>
{
    midi.Dispose();
    _ = SendClosingMessageAsync(wsServer);
};

app.Run();

static async Task SendClosingMessageAsync(IWebSocketServer socketServer)
{
    try
    {
        await socketServer.BroadcastMessageAsync(WebSocketMessage.Create(WebSocketCommand.ServerClosed, "Server is shutting down")).ConfigureAwait(false);
    }
    catch
    {
        // best-effort on shutdown
    }
}


public partial class Program 
{
    // used for testing 
    public static IHost BuildHost(int port = 8014)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls($"http://localhost:{port}");
        var app = Configure(builder);
        ConfigureApp(app);
        return app;
    }

    public static (IMidiService Midi, IWebSocketServer WebSocketServer) ConfigureApp(WebApplication app)
    {
        app.UseWebSockets(new WebSocketOptions
        {
            KeepAliveInterval = TimeSpan.FromSeconds(30)
        });

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

        // Call Configure explicitly on the built app
        var wsServer = app.Services.GetRequiredService<IWebSocketServer>();
        wsServer.Configure(app);

        return (midi, wsServer);
    }

    public static WebApplication Configure(WebApplicationBuilder builder)
    {

#if DEBUG
        RestartTeVirtualMidi();
#endif

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
              .AddSingleton<ICommandService, CommandService>()
              .AddSingleton<ICubaseService, CubaseService>()
              .AddSingleton<IKeyboardService, KeyboardService>()
              .AddSingleton<IMidiService, MidiService>()
              .AddSingleton<ICacheService, CacheService>()
              .AddSingleton<IMixerService, MixerService>()
              .AddSingleton<ICubaseWindowMonitor, CubaseWindowMonitor>()
              .AddSingleton<IWebSocketServer, WebSocketServer>()
              .AddSingleton<ICategoryService, CubaseMidiService>()
              .AddSingleton<ICategoryService, CubaseKeyService>()
              .AddSingleton<ICategoryService, CubaseScriptService>();


        builder.Services.AddControllers();

        builder.Services.AddHostedService<MidiWebSocketService>();
        builder.Services.AddHostedService<CubaseWindowsBackgroundService>();


        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.ListenAnyIP(8014); // Listen on all network interfaces    
        });

        return builder.Build();
    }

    static void RestartTeVirtualMidi()
    {
        string devconPath = @"C:\Program Files (x86)\Windows Kits\10\Tools\10.0.26100.0\x64\devcon.exe";
        string arguments = "restart *teVirtualMIDI*";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = devconPath,
                Arguments = arguments,
                Verb = "runas",        // requires admin
                UseShellExecute = false,
                // CreateNoWindow = true,
            }
        };

        process.Start();
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            Log.Logger.Error("Could not restart virtual midi driver");
        }
    }
}
