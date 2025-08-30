using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Keys
{
    public static class CubaseKeyMap
    {
        public static readonly Dictionary<string, byte> Map = new()
    {
        // Letters
        {"A", 0x41}, {"B", 0x42}, {"C", 0x43}, {"D", 0x44}, {"E", 0x45}, {"F", 0x46},
        {"G", 0x47}, {"H", 0x48}, {"I", 0x49}, {"J", 0x4A}, {"K", 0x4B}, {"L", 0x4C},
        {"M", 0x4D}, {"N", 0x4E}, {"O", 0x4F}, {"P", 0x50}, {"Q", 0x51}, {"R", 0x52},
        {"S", 0x53}, {"T", 0x54}, {"U", 0x55}, {"V", 0x56}, {"W", 0x57}, {"X", 0x58},
        {"Y", 0x59}, {"Z", 0x5A},

        // Numbers (top row)
        {"0", 0x30}, {"1", 0x31}, {"2", 0x32}, {"3", 0x33}, {"4", 0x34},
        {"5", 0x35}, {"6", 0x36}, {"7", 0x37}, {"8", 0x38}, {"9", 0x39},

        // Function keys
        {"F1", 0x70}, {"F2", 0x71}, {"F3", 0x72}, {"F4", 0x73}, {"F5", 0x74},
        {"F6", 0x75}, {"F7", 0x76}, {"F8", 0x77}, {"F9", 0x78}, {"F10", 0x79},
        {"F11", 0x7A}, {"F12", 0x7B}, {"F13", 0x7C}, {"F14", 0x7D}, {"F15", 0x7E},
        {"F16", 0x7F}, {"F17", 0x80}, {"F18", 0x81}, {"F19", 0x82}, {"F20", 0x83},
        {"F21", 0x84}, {"F22", 0x85}, {"F23", 0x86}, {"F24", 0x87},

        // Modifiers
        {"CTRL", 0x11}, {"CONTROL", 0x11}, {"SHIFT", 0x10}, {"ALT", 0x12}, {"WIN", 0x5B},

        // Arrows
        {"UP", 0x26}, {"DOWN", 0x28}, {"LEFT", 0x25}, {"RIGHT", 0x27},

        // Navigation
        {"HOME", 0x24}, {"END", 0x23}, {"PAGEUP", 0x21}, {"PAGEDOWN", 0x22},

        // Editing
        {"INSERT", 0x2D}, {"DELETE", 0x2E}, {"DEL", 0x2E}, // DEL synonym
        {"BACKSPACE", 0x08}, {"BKSP", 0x08}, // BKSP synonym
        {"TAB", 0x09}, {"ENTER", 0x0D}, {"RETURN", 0x0D}, // RETURN synonym
        {"ESC", 0x1B}, {"SPACE", 0x20},

        // Symbols (main keyboard)
        {"PERIOD", 0xBE}, {"COMMA", 0xBC}, {"SEMICOLON", 0xBA}, {"QUOTE", 0xDE},
        {"SLASH", 0xBF}, {"BACKSLASH", 0xDC}, {"OPENBRACKET", 0xDB}, {"CLOSEBRACKET", 0xDD},

        // Numpad / PAD (canonical names, no spaces)
        {"PAD0", 0x60}, {"PAD1", 0x61}, {"PAD2", 0x62}, {"PAD3", 0x63}, {"PAD4", 0x64},
        {"PAD5", 0x65}, {"PAD6", 0x66}, {"PAD7", 0x67}, {"PAD8", 0x68}, {"PAD9", 0x69},

        {"PADDECIMAL", 0x6E}, {"PAD.", 0x6E},
        {"PADENTER", 0x0D}, {"PAD+", 0x6B}, {"PAD-", 0x6D}, {"PAD*", 0x6A}, {"PAD/", 0x6F},

        // Misc
        {"CAPSLOCK", 0x14}, {"PRINTSCREEN", 0x2C}, {"SCROLLLOCK", 0x91}, {"PAUSE", 0x13}
    };
    }


}
