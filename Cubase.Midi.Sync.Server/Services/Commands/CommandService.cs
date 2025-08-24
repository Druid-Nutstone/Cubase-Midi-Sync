using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Server.Constants;

namespace Cubase.Midi.Sync.Server.Services.Commands
{
    public class CommandService : ICommandService
    {
        public Task<CubaseCommandsCollection> GetCommands()
        {
            var collection = new CubaseCommandsCollection();
            if (File.Exists(CubaseServerConstants.CommandsFileLocation))
            {
                collection = CubaseCommandsCollection.LoadFromFile(CubaseServerConstants.CommandsFileLocation);
            }
            else
            {
                collection = CubaseCommandsCollection.CreateWithError($"Cannot find file {CubaseServerConstants.CommandsFileLocation}");
            }
                // todo 
            return Task.FromResult(collection);
            
   
        }
    }
}
