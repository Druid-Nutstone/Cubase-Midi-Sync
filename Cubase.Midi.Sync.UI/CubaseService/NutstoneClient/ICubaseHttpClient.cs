using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.UI.NutstoneServices.NutstoneClient
{
    public interface ICubaseHttpClient
    {
        Task<CubaseCommandsCollection> GetCommands(Action<string> msgHandler, Action<string> exceptionHandler);

        Task<CubaseActionResponse> ExecuteCubaseAction(CubaseActionRequest cubaseActionRequest, Action<Exception> exceptionHandler);

        bool CanConnectToServer();

        string GetBaseConnection();


    }
}
