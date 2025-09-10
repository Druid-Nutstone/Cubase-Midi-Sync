using Cubase.Midi.Sync.Common.Midi;

namespace Cubase.Midi.Sync.Server.Services.Midi
{
    public interface IMidiService
    {
        public void Initialise();

        public bool SendMidiMessage(CubaseMidiCommand cubaseMidiCommand);
    }
}
