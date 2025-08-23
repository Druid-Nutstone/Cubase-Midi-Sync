using System.Reflection;
using System.Text.Json;

public class MidiPage
{
    public string Name { get; set; } = "";
    public List<ButtonMapping> Buttons { get; set; } = new();
    public List<FaderMapping> Faders { get; set; } = new();
    public List<KnobMapping> Knobs { get; set; } = new();
}

public class MidiMapper
{
    public List<MidiPage> Pages { get; set; } = new();

    public static MidiMapper LoadFromJson()
    {
        var location = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            "controls.json");

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return System.Text.Json.JsonSerializer.Deserialize<MidiMapper>(
            File.ReadAllText(location), options
        ) ?? new MidiMapper();
    }

    public string AsJson()
    {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return System.Text.Json.JsonSerializer.Serialize(this, options);
    } 
}

public class ButtonMapping
{
    public string Name { get; set; } = "";       // e.g., "Play"
    public int CC { get; set; }                  // e.g., 64
    public string[] Command { get; set; } = new string[0]; // e.g., ["Transport","Start"]
}

public class FaderMapping
{
    public string Name { get; set; } = "";       // e.g., "Fader 1"
    public int CC { get; set; }                  // e.g., 10
    public string Parameter { get; set; } = "";  // e.g., "mMixerChannelBank.channel[0].mVolume"
}

public class KnobMapping
{
    public string Name { get; set; } = "";       // e.g., "Pan 1"
    public int CC { get; set; }                  // e.g., 20
    public string Parameter { get; set; } = "";  // e.g., "mMixerChannelBank.channel[0].mPan"
}