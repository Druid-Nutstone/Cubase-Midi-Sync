using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Midi;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Server.Constants;
using Cubase.Midi.Sync.Server.Services.CommandCategproes;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace Cubase.Midi.Sync.Server.Services.Midi
{
    public class MidiService : IMidiService, IDisposable
    {
        private NutstoneDriver midiDriver;

        private readonly ILogger<MidiService> logger;

        private readonly IServiceProvider serviceProvider;

        private List<Action<MidiChannelCollection>> onChannelChangedHandlers = new List<Action<MidiChannelCollection>>();

        public List<Action<MidiChannel>> onTrackSelectedHandlers { get; set; } = new List<Action<MidiChannel>>();

        private CubaseMidiCommandCollection cubaseMidiCommands;

        private Action<MidiChannelCollection>? OnTrackCollection { get; set; } = null;

        public Action? OnReadyReceived { get; set; } = null;

        public bool ReadyReceived { get; set; } = false;

        public Action<CommandValue> onCommandDataHandler { get; set; }

        private bool disposed;

        public MidiChannelCollection MidiChannels { get; set; } = new MidiChannelCollection();

        private Dictionary<string, Action<string>> commandProcessors;

        public MidiService(ILogger<MidiService> logger, IServiceProvider serviceProvider) 
        { 
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.commandProcessors = new Dictionary<string, Action<string>>()
            {
                {MidiCommand.ClearChannels.ToString(), this.ClearChannels},
                {MidiCommand.ChannelChange.ToString(), this.ChannelChange },
                {MidiCommand.Message.ToString(),this.MessageReceived },
                {MidiCommand.Ready.ToString(), this.Ready },
                {MidiCommand.TrackUpdate.ToString(), this.TracksReceived },
                {MidiCommand.TrackComplete.ToString(), this.TracksComplete },
                {MidiCommand.CommandValueChanged.ToString(), this.CommandValueChanged },
             
            };
            //midiThread = new Thread(ProcessMidiQueue)
            //{
            //    IsBackground = true,
            //    Name = "MidiThread"
            //};
            //midiThread.SetApartmentState(ApartmentState.STA);
            //midiThread.Start();
        }

        //private void ProcessMidiQueue()
        //{
        //    foreach (var cmd in midiQueue.GetConsumingEnumerable())
        //    {
        //        try
        //        {
        //            if (cmd.Channel > -1)
        //                midiDriver.SendNoteOn(cmd.Channel, cmd.Note, cmd.Velocity);
        //            else
        //                midiDriver.SendMessage(MidiCommand.Message, cmd.Command);
        //        }
        //        catch (Exception ex)
        //        {
        //            this.logger.LogError($"Cannot send midi {ex.Message}");
        //            // log failures if needed
        //        }
        //    }
        //}

        public void Initialise()
        {
            this.logger.LogInformation("Initialising Nutstone Midi ..");
            this.midiDriver = new NutstoneDriver("Nutstone");
            this.midiDriver.MidiMessageReceived += MidiDriver_MidiMessageReceived;
            this.cubaseMidiCommands = new CubaseMidiCommandCollection(CubaseServerConstants.KeyCommandsFileLocation);
        }

        public Action<MidiChannelCollection> RegisterOnChannelChanged(Action<MidiChannelCollection> action)
        {
            this.onChannelChangedHandlers.Add(action);
            return action;
        }

        public void UnRegisterOnChannelChanged(Action<MidiChannelCollection> action)
        {
            this.onChannelChangedHandlers.Remove(action);   
        }

        public Action<MidiChannel> RegisterOnTrackSelected(Action<MidiChannel> action)
        {
            this.onTrackSelectedHandlers.Add(action);
            return action;
        }

        public void UnRegisterOnTrackSelected(Action<MidiChannel> action)
        {
            this.onTrackSelectedHandlers.Remove(action);
        }

        public bool SendMidiMessage(CubaseMidiCommand cubaseMidiCommand)
        {
            try
            {
                if (cubaseMidiCommand.Channel > -1)
                {
                    this.logger.LogInformation($"Sending Midi Name:{cubaseMidiCommand.Name} Command:{cubaseMidiCommand.Command} Channel:{cubaseMidiCommand.Channel} Note:{cubaseMidiCommand.Note} ");
                    this.midiDriver.SendNoteOn(cubaseMidiCommand.Channel, cubaseMidiCommand.Note, cubaseMidiCommand.Velocity);
                    return true;
                }
                else
                {
                    var keyService = serviceProvider.GetServices<ICategoryService>().FirstOrDefault(x => x.SupportedKeys.Contains(CubaseServiceConstants.KeyService));
                    if (keyService != null)
                    {
                        var keyResult = keyService.ProcessAction(ActionEvent.Create(CubaseAreaTypes.Keys, cubaseMidiCommand.Command));
                        return keyResult.Success;
                    }
                    return false;
                }
            }
            catch (Exception ex) 
            {
                return false;
            }
        }

        public Task<bool> SendMidiMessageAsync(CubaseMidiCommand midiCommand)
        {
            return Task.FromResult(this.SendMidiMessage(midiCommand));
        }


        public async Task GetChannels()
        {
            this.midiDriver.SendMessage(MidiCommand.Tracks, "");
        }

        public void SendSysExMessage<T>(MidiCommand command, T request)
        {
            this.midiDriver.SendMessage(command, request);  
        }

        public void SelectTracks(List<MidiChannel> tracks)
        {
            this.SendSysExMessage(MidiCommand.SelectTracks, tracks);
        }
        
        public async Task<MidiChannelCollection?> GetTracksAsync(Action<string> errorHandler, int timeoutMs = 5000)
        {
            var tcs = new TaskCompletionSource<MidiChannelCollection?>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            // Hook the event — fires when tracks arrive
            this.OnTrackCollection = (midiCollection) =>
            {
                tcs.TrySetResult(midiCollection);
            };

            // Send request
            this.SendSysExMessage(MidiCommand.Tracks, string.Empty);

            // Create a timeout task
            var timeoutTask = Task.Delay(timeoutMs);

            // Wait for whichever finishes first
            var completed = await Task.WhenAny(tcs.Task, timeoutTask);

            if (completed == timeoutTask)
            {
                // Optional: reset event handler to avoid leaks
                this.OnTrackCollection = null;
                errorHandler.Invoke($"Could not get tracks. Timeout occured after {timeoutMs} milliseconds");
                return null; // indicates timeout
            }
            return await tcs.Task; // success
        }




        private void MidiDriver_MidiMessageReceived(byte[] message)
        {
            if (message.Length >= 3 && message[0] == 0xF0 && message[1] == 0x7D)
            {
                // Extract the payload between 0xF0..0xF7
                int endIndex = Array.IndexOf(message, (byte)0xF7, 0);
                if (endIndex == -1) return; // incomplete

                // Convert SysEx bytes to string
                List<byte> content = new List<byte>();
                for (int i = 2; i < endIndex; i++) content.Add((byte)(message[i] & 0x7F));

                // Split command / message at separator 0x00
                int sep = content.IndexOf(0x00);
                if (sep == -1) return;

                string command = Encoding.ASCII.GetString(content.Take(sep).ToArray());
                string payload = Encoding.ASCII.GetString(content.Skip(sep + 1).ToArray());
                Debug.WriteLine($"Received Command {command}");
                if (this.commandProcessors.ContainsKey(command))
                {
                    this.commandProcessors[command](payload);
                }
                else
                {
                    this.logger.LogWarning($"The command {command} does not have an associated processor?? - David!!");
                }
            }
        }

        private void ClearChannels(string emptyString)
        {
            this.MidiChannels = new MidiChannelCollection();
            this.logger.LogWarning($"Clearing current midi channels..");
        }

        private void ChannelChange(string channelInfo)
        {
            var channelData = JsonSerializer.Deserialize<MidiChannel>(channelInfo);
            // this.logger.LogInformation($"Channel Change: {channelData.Name} Index:{channelData.Index} Selected:{channelData.Selected}");

            var channelCollection = this.MidiChannels.AddOrUpdateChannel(channelData);
            if (channelData.Selected.HasValue && channelData.Selected.Value)
            {
                foreach (var handler in this.onTrackSelectedHandlers.ToList())
                {
                    handler?.Invoke(channelData);
                }
            }

            foreach (var handler in this.onChannelChangedHandlers.ToList())
            {
                handler?.Invoke(channelCollection);
            }

        }

        private void CommandValueChanged(string commandValue)
        {
            var commandData = JsonSerializer.Deserialize<CommandValue>(commandValue);
            if (this.onCommandDataHandler != null)
            {
                this.onCommandDataHandler.Invoke(commandData);
            }
        }

        private void MessageReceived(string message)
        {
            this.logger.LogInformation($"Message received: {message}");
        }

        private void TracksReceived(string tracksJson)
        {
            var channel = JsonSerializer.Deserialize<MidiChannel>(tracksJson);
            this.MidiChannels.AddOrUpdateChannel(channel);
            this.logger.LogInformation($"Channel add/changed {channel.Name} Index:{channel.Index} Selected:{channel.Selected} volume:{channel.Volume}");
        }

        private void TracksComplete(string emptyString)
        {
            this.logger.LogInformation($"Tracks received complete. Total channels: {this.MidiChannels.Count}");
            if (this.OnTrackCollection != null)
            {
                this.OnTrackCollection.Invoke(MidiChannels);
            } 
        }

        private void Ready(string emptyString)
        {
            this.ReadyReceived = true;
            this.logger.LogInformation($"Cubase Midi Sync is ready..");
            if (OnReadyReceived != null)
            {
                OnReadyReceived();
            }
        }

       
        public void VerifyDriver()
        {
            this.ReadyReceived = false;
            this.SendSysExMessage(MidiCommand.Ready, "{}");
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            this.midiDriver.Dispose();
          
        }


    }
}
