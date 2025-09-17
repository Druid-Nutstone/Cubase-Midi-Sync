using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Midi;
using Cubase.Midi.Sync.Common.Mixer;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.Responses;
using Cubase.Midi.Sync.UI.NutstoneServices.NutstoneClient;
using Cubase.Midi.Sync.UI.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.UI.CubaseService.NutstoneClient
{
    public class TestCubaseHttpClient : ICubaseHttpClient
    {
        private readonly AppSettings appSettings;

        public TestCubaseHttpClient(AppSettings appSettings)
        {
            this.appSettings = appSettings; 
        }

        public bool CanConnectToServer()
        {
            return true;
        }

        public Task<CubaseActionResponse> ExecuteCubaseAction(CubaseActionRequest cubaseActionRequest, Action<Exception> exceptionHandler)
        {
            return Task.FromResult(new CubaseActionResponse
            {
                Success = true,
                Message = $"{cubaseActionRequest.Action.ToString()} executed successfully (Test Client)"
            });
        }

        public string GetBaseConnection()
        {
            throw new NotImplementedException();
        }

        public async Task<CubaseCommandsCollection> GetCommands(Action<string> msgHandler, Action<string> exceptionHandler)
        {
            return null;
            /*
            try
            {
                // Simulate slow operation
                await Task.Delay(3000);

                var collection = new CubaseCommandsCollection()
            {
              new CubaseCommandCollection
              {
                 Name = "Transport",
                 Commands = new List<CubaseCommand>
                 { 
                     new CubaseCommand { Name = "Play Track", Action = CubaseActionName.TransportPlay, ButtonType = CubaseButtonType.Toggle  },
                     new CubaseCommand { Name = "Record Start", Action = CubaseActionName.TransportRecord, ButtonType = CubaseButtonType.Toggle }
                 }
              },
              new CubaseCommandCollection
              {
                 Name = "Some other thing",
                 Commands = new List<CubaseCommand>
              {
                  new CubaseCommand { Name = "Command C", Action = CubaseActionName.TransportRightLocator }
              }
            }};

                return collection; // ✅ returning normally is fine
            }
            catch (Exception ex)
            {
                exceptionHandler?.Invoke(ex.Message);
                return null;
            }
        }
            */
        }

        public Task<CubaseMixerCollection> GetMixer()
        {
            throw new NotImplementedException();
        }

        public Task<MidiChannelCollection> GetTracks()
        {
            throw new NotImplementedException();
        }

        public Task<CubaseMixerCollection> SetMixer(CubaseMixer mixer)
        {
            throw new NotImplementedException();
        }

        public Task<MidiChannelCollection> SetSelectedTrack(MidiChannel midiChannel)
        {
            throw new NotImplementedException();
        }

        public Task<MidiChannelCollection> SetSelectedTracks(List<MidiChannel> midiChannels)
        {
            throw new NotImplementedException();
        }
    }
}
