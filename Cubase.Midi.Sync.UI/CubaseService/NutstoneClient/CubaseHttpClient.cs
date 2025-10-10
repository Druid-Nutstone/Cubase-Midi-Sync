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
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;


namespace Cubase.Midi.Sync.UI.CubaseService.NutstoneClient
{
    public class CubaseHttpClient : HttpClient, ICubaseHttpClient
    {
        private readonly AppSettings appSettings;
        
        public CubaseHttpClient(AppSettings appSettings)
        {
            this.appSettings = appSettings;
            this.BaseAddress = new Uri(appSettings.CubaseConnection.BaseUrl);
        }

        public string GetBaseConnection()
        {
            return this.BaseAddress.ToString();
        }
        
        public bool CanConnectToServer()
        {
            try
            {
                var respone = this.GetAsync("api/cubase/test").Result;
                return respone.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<CubaseActionResponse> ExecuteCubaseAction(CubaseActionRequest cubaseActionRequest, Action<Exception> exceptionHandler)
        {
            var response = await this.PostAsJsonAsync("api/cubase/execute", cubaseActionRequest);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CubaseActionResponse>() ?? new CubaseActionResponse { Success = false, Message = "No response from server." };
            }
            else
            {
                exceptionHandler?.Invoke(new Exception($"Error executing action: {response.ReasonPhrase}"));
                return new CubaseActionResponse { Success = false, Message = $"Error executing action: {response.ReasonPhrase}" };
            }
        }

        public async Task<CubaseCommandsCollection> GetCommands(Action<string> msgHandler, Action<string> exceptionHandler)
        {
            msgHandler.Invoke($"Fetching commands from Cubase...{this.BaseAddress.ToString()}");

            var response = await GetAsync("api/cubase/commands");

            if (!response.IsSuccessStatusCode)
            {
                // log status code + error body
                var errorBody = await response.Content.ReadAsStringAsync();
                exceptionHandler.Invoke($"Code: {response.StatusCode.ToString()} URL: {response.RequestMessage.RequestUri.ToString()} error:{errorBody}");
            }

            // success → deserialize
            var result = await response.Content.ReadFromJsonAsync<CubaseCommandsCollection>();
            return result ?? new CubaseCommandsCollection();
        }

        public async Task<MidiChannelCollection> GetTracks()
        {
            var response = await GetAsync("api/cubase/tracks");

            if (!response.IsSuccessStatusCode)
            {
                // log status code + error body
                var errorBody = await response.Content.ReadAsStringAsync();
            }

            // success → deserialize
            var result = await response.Content.ReadFromJsonAsync<MidiChannelCollection>();
            return result ?? new MidiChannelCollection();
        }
         
        public async Task<MidiChannelCollection> SetSelectedTrack(MidiChannel midiChannel)
        {
            var response = await this.PostAsJsonAsync("api/cubase/tracks/select", midiChannel);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MidiChannelCollection>() ?? new MidiChannelCollection();
            }
            else
            {
                return new MidiChannelCollection();
                //exceptionHandler?.Invoke(new Exception($"Error executing action: {response.ReasonPhrase}"));
                //return new CubaseActionResponse { Success = false, Message = $"Error executing action: {response.ReasonPhrase}" };
            }
        }

        public async Task<CubaseMixerCollection> SetMixer(CubaseMixer mixer, Page page)
        {
            var response = await this.PostAsJsonAsync("api/mixer", mixer);
            if (response.IsSuccessStatusCode) 
            {
                return await response.Content.ReadFromJsonAsync<CubaseMixerCollection>();
            }
            else
            {
                await page.DisplayAlert("Error - SetMixer", await response.Content.ReadAsStringAsync(), "OK");
                return null;
            }
        }

        public async Task<CubaseMixerCollection> GetMixer(Page page)
        {
            var response = await GetAsync("api/mixer");

            if (!response.IsSuccessStatusCode)
            {
                // log status code + error body
                var errorBody = await response.Content.ReadAsStringAsync();
                await page.DisplayAlert("Error CubaseHttpClient GetMixer ", $"Error getting mixer details. {Environment.NewLine} {errorBody}", "OK");  
            }

            // success → deserialize
            return await response.Content.ReadFromJsonAsync<CubaseMixerCollection>();
        }


    }
}
