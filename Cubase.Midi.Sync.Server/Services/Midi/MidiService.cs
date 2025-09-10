using Cubase.Midi.Sync.Common.Midi;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cubase.Midi.Sync.Server.Services.Midi
{
    public class MidiService : IMidiService
    {
        private NutstoneDriver midiDriver;

        private ILogger<MidiService> logger;

        private CubaseMidiCommandCollection cubaseMidiCommands;

        private MidiChannelCollection midiChannels = new MidiChannelCollection();

        private CubaseTransport transport;

        private Dictionary<string, Action<string>> commandProcessors;

        public MidiService(ILogger<MidiService> logger) 
        { 
            this.logger = logger;
            this.commandProcessors = new Dictionary<string, Action<string>>()
            {
                {MidiCommand.ClearChannels.ToString(), this.ClearChannels},
                {MidiCommand.ChannelChange.ToString(), this.ChannelChange },
                {MidiCommand.Message.ToString(),this.MessageReceived },
                {MidiCommand.Ready.ToString(), this.Ready }
              
            };
        }

        public void Initialise()
        {
            this.logger.LogInformation("Initialising Nutstone Midi ..");
            this.midiDriver = new NutstoneDriver("Nutstone");
            this.midiDriver.MidiMessageReceived += MidiDriver_MidiMessageReceived;
            this.cubaseMidiCommands = new CubaseMidiCommandCollection();    
        }

        public bool SendMidiMessage(CubaseMidiCommand cubaseMidiCommand)
        {
            try
            {
                this.midiDriver.SendNoteOn(cubaseMidiCommand.Channel, cubaseMidiCommand.Note, cubaseMidiCommand.Velocity);
                return true;
            }
            catch (Exception ex) 
            {
                return false;
            }
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
            this.midiChannels = new MidiChannelCollection();
            this.logger.LogWarning($"Clearing current midi channels..");
        }

        private void ChannelChange(string channelInfo)
        {
            var channelData = JsonSerializer.Deserialize<MidiChannel>(channelInfo);
            var newChannel = this.midiChannels.AddOrUpdateChannel(channelData);
            //if (!string.IsNullOrEmpty(newChannel?.Name))
            //{

                this.logger.LogInformation($"Add or updating: Channel:{newChannel.Name} Index:{newChannel.Index} Volume: {newChannel.Volume} RecordEnable:{newChannel.RecordEnable} Mute:{newChannel.Mute} Solo: {newChannel.Solo} Selected: {newChannel.Selected}");
            //}
        }

        private void MessageReceived(string message)
        {
            this.logger.LogInformation($"Message received: {message}");
        }

        private void Ready(string emptyString)
        {
            this.logger.LogInformation($"Cubase Midi Sync is ready..");
            // this.midiDriver.SendMessage("Nutstone Midi Javascript Service is ready");
        }

    }
}
