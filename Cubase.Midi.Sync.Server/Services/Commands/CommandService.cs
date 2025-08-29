using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Server.Constants;

namespace Cubase.Midi.Sync.Server.Services.Commands
{
    public class CommandService : ICommandService
    {
        public Task<CubaseCommandsCollection> GetCommands()
        {
            var cubaseServerSettings = new CubaseServerSettings();
            return Task.FromResult(cubaseServerSettings.GetCubaseCommands());
        }
    }
}
