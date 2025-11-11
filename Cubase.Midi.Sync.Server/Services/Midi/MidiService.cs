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

        private readonly ICategoryService keyService;
        
        private readonly ILogger<MidiService> logger;

        //private readonly Thread midiThread;

        //private readonly BlockingCollection<CubaseMidiCommand> midiQueue = new();

        private CubaseMidiCommandCollection cubaseMidiCommands;

        public bool ReadyReceived { get; set; } = false;

        private bool disposed;

        public MidiChannelCollection MidiChannels { get; set; } = new MidiChannelCollection();

        private Dictionary<string, Action<string>> commandProcessors;

        private bool tracksReceived = false;

        public Action<MidiChannelCollection>? OnChannelChanged { get; set; } = null;

        public MidiService(ILogger<MidiService> logger, IServiceProvider serviceProvider) 
        { 
            this.logger = logger;
            this.keyService = serviceProvider.GetKeyedService<ICategoryService>(CubaseServiceConstants.KeyService);
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

        public bool SendMidiMessage(CubaseMidiCommand cubaseMidiCommand)
        {
            try
            {
                if (cubaseMidiCommand.Channel > -1)
                {
                    //midiQueue.Add(cubaseMidiCommand);
                    //return true;
                    this.logger.LogInformation($"Sending Midi Name:{cubaseMidiCommand.Name} Command:{cubaseMidiCommand.Command} Channel:{cubaseMidiCommand.Channel} Note:{cubaseMidiCommand.Note} ");
                    this.midiDriver.SendNoteOn(cubaseMidiCommand.Channel, cubaseMidiCommand.Note, cubaseMidiCommand.Velocity);
                    return true;
                }
                else
                {
                    var keyResult = this.keyService.ProcessAction(ActionEvent.Create(CubaseAreaTypes.Keys, cubaseMidiCommand.Command));
                    return keyResult.Success;
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


        public MidiChannelCollection GetChannels()
        {
            this.tracksReceived = false;
            this.MidiChannels = new MidiChannelCollection();    
            this.midiDriver.SendMessage(MidiCommand.Tracks, "");
            var maxCount = 1000;
            var count = 0;
            while (!this.tracksReceived && count < maxCount)
            {
                Task.Delay(50).Wait();
                count++; 
            }
            return this.MidiChannels;
        }

        public void SendSysExMessage<T>(MidiCommand command, T request)
        {
            this.midiDriver.SendMessage(command, request);  
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
            if (OnChannelChanged != null)
            {
                OnChannelChanged(channelCollection);
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
            this.tracksReceived = true;
            this.logger.LogInformation($"Tracks received complete. Total channels: {this.MidiChannels.Count}");
        }

        private void Ready(string emptyString)
        {
            this.ReadyReceived = true;
            this.logger.LogInformation($"Cubase Midi Sync is ready..");
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
