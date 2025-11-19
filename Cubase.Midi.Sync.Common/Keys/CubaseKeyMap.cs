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
    // Letters (uppercase, lowercase, Cubase-style)
    {"A", 0x41}, {"a", 0x41}, {"ALPHA_A", 0x41},
    {"B", 0x42}, {"b", 0x42}, {"ALPHA_B", 0x42},
    {"C", 0x43}, {"c", 0x43}, {"ALPHA_C", 0x43},
    {"D", 0x44}, {"d", 0x44}, {"ALPHA_D", 0x44},
    {"E", 0x45}, {"e", 0x45}, {"ALPHA_E", 0x45},
    {"F", 0x46}, {"f", 0x46}, {"ALPHA_F", 0x46},
    {"G", 0x47}, {"g", 0x47}, {"ALPHA_G", 0x47},
    {"H", 0x48}, {"h", 0x48}, {"ALPHA_H", 0x48},
    {"I", 0x49}, {"i", 0x49}, {"ALPHA_I", 0x49},
    {"J", 0x4A}, {"j", 0x4A}, {"ALPHA_J", 0x4A},
    {"K", 0x4B}, {"k", 0x4B}, {"ALPHA_K", 0x4B},
    {"L", 0x4C}, {"l", 0x4C}, {"ALPHA_L", 0x4C},
    {"M", 0x4D}, {"m", 0x4D}, {"ALPHA_M", 0x4D},
    {"N", 0x4E}, {"n", 0x4E}, {"ALPHA_N", 0x4E},
    {"O", 0x4F}, {"o", 0x4F}, {"ALPHA_O", 0x4F},
    {"P", 0x50}, {"p", 0x50}, {"ALPHA_P", 0x50},
    {"Q", 0x51}, {"q", 0x51}, {"ALPHA_Q", 0x51},
    {"R", 0x52}, {"r", 0x52}, {"ALPHA_R", 0x52},
    {"S", 0x53}, {"s", 0x53}, {"ALPHA_S", 0x53},
    {"T", 0x54}, {"t", 0x54}, {"ALPHA_T", 0x54},
    {"U", 0x55}, {"u", 0x55}, {"ALPHA_U", 0x55},
    {"V", 0x56}, {"v", 0x56}, {"ALPHA_V", 0x56},
    {"W", 0x57}, {"w", 0x57}, {"ALPHA_W", 0x57},
    {"X", 0x58}, {"x", 0x58}, {"ALPHA_X", 0x58},
    {"Y", 0x59}, {"y", 0x59}, {"ALPHA_Y", 0x59},
    {"Z", 0x5A}, {"z", 0x5A}, {"ALPHA_Z", 0x5A},

    // Numbers (top row, Cubase-style NUMx)
    {"0", 0x30}, {"NUM0", 0x30},
    {"1", 0x31}, {"NUM1", 0x31},
    {"2", 0x32}, {"NUM2", 0x32},
    {"3", 0x33}, {"NUM3", 0x33},
    {"4", 0x34}, {"NUM4", 0x34},
    {"5", 0x35}, {"NUM5", 0x35},
    {"6", 0x36}, {"NUM6", 0x36},
    {"7", 0x37}, {"NUM7", 0x37},
    {"8", 0x38}, {"NUM8", 0x38},
    {"9", 0x39}, {"NUM9", 0x39},

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
    {"INSERT", 0x2D}, {"DELETE", 0x2E}, {"DEL", 0x2E},
    {"BACKSPACE", 0x08}, {"BKSP", 0x08},
    {"TAB", 0x09}, {"ENTER", 0x0D}, {"RETURN", 0x0D},
    {"ESC", 0x1B}, {"SPACE", 0x20}, {" ", 0x20},

    // Punctuation & symbols (with Cubase monikers)
    {"PERIOD", 0xBE}, {".", 0xBE}, {">", 0xBE},
    {"COMMA", 0xBC}, {",", 0xBC}, {"<", 0xBC},
    {"SEMICOLON", 0xBA}, {";", 0xBA}, {":", 0xBA},
    {"QUOTE", 0xDE}, {"'", 0xDE}, {"\"", 0xDE},
    {"SLASH", 0xBF}, {"/", 0xBF}, {"?", 0xBF},
    {"BACKSLASH", 0xDC}, {"\\", 0xDC}, {"|", 0xDC},
    {"OPENBRACKET", 0xDB}, {"[", 0xDB}, {"{", 0xDB},
    {"CLOSEBRACKET", 0xDD}, {"]", 0xDD}, {"}", 0xDD},
    {"MINUS", 0xBD}, {"-", 0xBD}, {"_", 0xBD},
    {"EQUALS", 0xBB}, {"=", 0xBB}, {"+", 0xBB},
    {"BACKTICK", 0xC0}, {"`", 0xC0}, {"~", 0xC0},

    // Other shifted symbols
    {"!", 0x31}, {"@", 0x32}, {"#", 0x33}, {"$", 0x34}, {"%", 0x35},
    {"^", 0x36}, {"&", 0x37}, {"*", 0x38}, {"(", 0x39}, {")", 0x30},

    // Numpad (Cubase PAD monikers)
    {"PAD0", 0x60}, {"PAD1", 0x61}, {"PAD2", 0x62}, {"PAD3", 0x63}, {"PAD4", 0x64},
    {"PAD5", 0x65}, {"PAD6", 0x66}, {"PAD7", 0x67}, {"PAD8", 0x68}, {"PAD9", 0x69},
    {"PADDECIMAL", 0x6E}, {"PAD.", 0x6E},
    {"PADENTER", 0x0D}, {"PAD+", 0x6B}, {"PAD-", 0x6D}, {"PAD*", 0x6A}, {"PAD/", 0x6F},

    // Misc
    {"CAPSLOCK", 0x14}, {"PRINTSCREEN", 0x2C}, {"SCROLLLOCK", 0x91}, {"PAUSE", 0x13},
    // windows key 
    {"META", 0x5B}, {"Meta", 0x5B}, {"meta", 0x5B},
};



        public static readonly Dictionary<string, byte> MapOld = new()
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
        {"CAPSLOCK", 0x14}, {"PRINTSCREEN", 0x2C}, {"SCROLLLOCK", 0x91}, {"PAUSE", 0x13},
        // windows key 
            {"META", 0x5B}, {"Meta", 0x5B}, {"meta", 0x5B},
        };
    }


}
