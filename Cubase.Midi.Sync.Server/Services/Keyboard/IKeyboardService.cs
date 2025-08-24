using Cubase.Midi.Sync.Common.Keys;
using static Cubase.Midi.Sync.Server.Services.Keyboard.KeyboardService;

namespace Cubase.Midi.Sync.Server.Services.Keyboard
{
    public interface IKeyboardService
    {
        bool SendKey(VirtualKey key);
    }
}
