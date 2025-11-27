using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Midi;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Server.Constants;
using Cubase.Midi.Sync.Server.Services.CommandCategproes;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cubase.Midi.Sync.Server.Services.Midi
{
    public class MidiService : IMidiService, IDisposable
    {
        private NutstoneDriver midiDriver;

        private readonly ILogger<MidiService> logger;

        private readonly IServiceProvider serviceProvider;

        private List<Action<MidiChannelCollection>> onChannelChangedHandlers = new List<Action<MidiChannelCollection>>();

        //private readonly Thread midiThread;

        //private readonly BlockingCollection<CubaseMidiCommand> midiQueue = new();

        private CubaseMidiCommandCollection cubaseMidiCommands;

        public Action? OnReadyReceived { get; set; } = null;

        public bool ReadyReceived { get; set; } = false;

        private bool disposed;

        public MidiChannelCollection MidiChannels { get; set; } = new MidiChannelCollection();

        private Dictionary<string, Action<string>> commandProcessors;

        private bool tracksReceived = false;

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

        public void RegisterOnChannelChanged(Action<MidiChannelCollection> action)
        {
            this.onChannelChangedHandlers.Add(action);
        }

        public bool SendMidiMessage(CubaseMidiCommand cubaseMidiCommand)
        {
            try
            {
                if (cubaseMidiCommand.Channel > -1)
                {
                    bool isReady = false;
                    var maxAttempts = 10000;
                    var attempts = 0;
                    isReady = false;
                    this.OnReadyReceived = () => { isReady = true; };
                    this.SendSysExMessage(MidiCommand.Ready, "{}");
                    while (!isReady && attempts < maxAttempts)
                    {
                        attempts++;
                        if (!isReady)
                        {
                            Task.Delay(1).Wait();
                        }
                    }
                    if (attempts == maxAttempts)
                    {
                        this.logger.LogError($"Cubase virtual.js is not responding to the ready message");
                        return false;
                    }
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

        public async Task<bool> SendMidiMessageAsync(CubaseMidiCommand midiCommand)
        {
            var result = true;
            await Task.Run(() => 
            { 
               result = this.SendMidiMessage(midiCommand);   
            });
            return result;
        }


        public async Task GetChannels()
        {
            this.tracksReceived = false;
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
            this.logger.LogInformation($"Channel Change: {channelData.Name} Index:{channelData.Index} Selected:{channelData.Selected}");
            var channelCollection = this.MidiChannels.AddOrUpdateChannel(channelData);
            this.onChannelChangedHandlers.ForEach(handler => handler.Invoke(channelCollection));
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
            this.tracksReceived = true;
            this.logger.LogInformation($"Tracks received complete. Total channels: {this.MidiChannels.Count}");
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
