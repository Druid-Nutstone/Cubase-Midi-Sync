using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Server.Services.Area;
using Cubase.Midi.Sync.Server.Services.Area.Transport;
using Cubase.Midi.Sync.Server.Services.Commands;
using Cubase.Midi.Sync.Server.Services.Cubase;
using Cubase.Midi.Sync.Server.Services.Keyboard;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
      .AddTransient<ICommandService, CommandService>()
      .AddTransient<ICubaseService, CubaseService>()
      .AddTransient<IKeyboardService, KeyboardService>()
      .AddKeyedTransient<IAreaService, TransportAreaService>(CubaseAreaName.Transport);


builder.Services.AddControllers();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8014); // Listen on all network interfaces
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
