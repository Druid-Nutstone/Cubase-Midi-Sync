using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.UI.NutstoneServices.NutstoneClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.UI.CubaseService.NutstoneClient
{
    public class TestCubaseHttpClient : ICubaseHttpClient
    {
        public Task<CubaseCommandsCollection> GetCommands()
        {
            var collection = new CubaseCommandsCollection()
            {
              new CubaseCommandCollection
              {
                 Name = "Collection 1",
                 Commands = new List<CubaseCommand>
                 {
                     new CubaseCommand { Name = "Command A", Action = "ActionA" },
                     new CubaseCommand { Name = "Command B", Action = "ActionB" }
                 }
              },
              new CubaseCommandCollection
              {
                 Name = "Collection 2",
                 Commands = new List<CubaseCommand>
              {
                  new CubaseCommand { Name = "Command C", Action = "ActionC" }
              }
            }};
            return Task.FromResult(collection);
        }
    }
}
