using Cubase.Midi.Sync.Common.Midi;

namespace Cubase.Midi.Sync.Server.Services.Midi
{
    public interface IMidiService
    {
        void RegisterOnChannelChanged(Action<MidiChannelCollection> action);  
        
        MidiChannelCollection MidiChannels { get; set; }    

        public void Initialise();

        public bool SendMidiMessage(CubaseMidiCommand cubaseMidiCommand);

        public Task<bool> SendMidiMessageAsync(CubaseMidiCommand midiCommand);

        public void SendSysExMessage<T>(MidiCommand command, T request);

        public Task GetChannels();

        public void VerifyDriver();

        void Dispose();

        public Action? OnReadyReceived { get; set; } 

        bool ReadyReceived { get; set; }

        public void SelectTracks(List<MidiChannel> tracks);
    }
}
