using Cubase.Midi.Sync.Common;

namespace Cubase.Midi.Sync.Server.Services.Commands
{
    public interface ICommandService
    {
        Task<CubaseCommandsCollection> GetCommands();
    }
}
