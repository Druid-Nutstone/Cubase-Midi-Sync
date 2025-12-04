using Cubase.Midi.Sync.Common.Midi;

namespace Cubase.Midi.Sync.Server.Services.Midi
{
    public interface IMidiService
    {
        Action<MidiChannelCollection> RegisterOnChannelChanged(Action<MidiChannelCollection> action);  
        
        void UnRegisterOnChannelChanged(Action<MidiChannelCollection> action);

        Action<MidiChannel> RegisterOnTrackSelected(Action<MidiChannel> action);

        void UnRegisterOnTrackSelected(Action<MidiChannel> action);

        MidiChannelCollection MidiChannels { get; set; }

        Task<MidiChannelCollection?> GetTracksAsync(Action<string> errorHandler, int timeoutMs = 5000);

        public void Initialise();

        public bool SendMidiMessage(CubaseMidiCommand cubaseMidiCommand);

        public Task<bool> SendMidiMessageAsync(CubaseMidiCommand midiCommand);

        public void SendSysExMessage<T>(MidiCommand command, T request);

        public Task GetChannels();

        public void VerifyDriver();

        void Dispose();

        public Action? OnReadyReceived { get; set; }

        public Action<CommandValue> onCommandDataHandler { get; set; }
        bool ReadyReceived { get; set; }

        public void SelectTracks(List<MidiChannel> tracks);
    }
}
