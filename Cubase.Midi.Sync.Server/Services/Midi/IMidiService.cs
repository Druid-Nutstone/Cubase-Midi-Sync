using Cubase.Midi.Sync.Common.Midi;

namespace Cubase.Midi.Sync.Server.Services.Midi
{
    public interface IMidiService
    {
        Action<MidiChannelCollection>? OnChannelChanged { get; set; }

        MidiChannelCollection MidiChannels { get; set; }    

        public void Initialise();

        public bool SendMidiMessage(CubaseMidiCommand cubaseMidiCommand);

        public Task<bool> SendMidiMessageAsync(CubaseMidiCommand midiCommand);

        public void SendSysExMessage<T>(MidiCommand command, T request);

        public MidiChannelCollection GetChannels();

        public void VerifyDriver();

        void Dispose();

        bool ReadyReceived { get; set; }
    }
}
