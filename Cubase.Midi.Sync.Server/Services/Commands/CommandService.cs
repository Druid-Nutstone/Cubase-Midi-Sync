using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Server.Constants;
using Cubase.Midi.Sync.Server.Services.Cubase;

namespace Cubase.Midi.Sync.Server.Services.Commands
{
    public class CommandService : ICommandService
    {
        private readonly ILogger<CubaseService> logger;

        public CommandService(ILogger<CubaseService> logger) 
        { 
             this.logger = logger;  
        }

        public Task<CubaseCommandsCollection> GetCommands()
        {
            var cubaseServerSettings = new CubaseServerSettings();
            var commands = cubaseServerSettings.GetCubaseCommands();

            this.logger.LogInformation($"Loaded commands from {cubaseServerSettings.FilePath} Count: {commands.Count()}");
            return Task.FromResult(commands);
        }
    }
}
