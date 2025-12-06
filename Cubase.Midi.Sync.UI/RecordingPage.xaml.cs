using Cubase.Midi.Sync.Common.Colours;
using Cubase.Midi.Sync.UI.Controls;
using Cubase.Midi.Sync.UI.CubaseService.WebSocket;
using Cubase.Midi.Sync.UI.Extensions;

namespace Cubase.Midi.Sync.UI;

public partial class RecordingPage : ContentPage
{
    private readonly IMidiWebSocketResponse midiWebSocketResponse;

    public RecordingPage(IMidiWebSocketResponse midiWebSocketResponse)
	{
		this.midiWebSocketResponse = midiWebSocketResponse; 
        InitializeComponent();
        BackgroundColor = ColourConstants.WindowBackground.ToMauiColor();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await trackSelector.Initialise(this.midiWebSocketResponse);
    }
}