using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Colours;
using Cubase.Midi.Sync.Common.Extensions;
using Cubase.Midi.Sync.Common.WebSocket;
using Cubase.Midi.Sync.UI.CubaseService.WebSocket;
using Cubase.Midi.Sync.UI.Extensions;
using Cubase.Midi.Sync.UI.NutstoneServices.NutstoneClient;
using Cubase.Midi.Sync.UI.Settings;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace Cubase.Midi.Sync.UI;

public partial class CubaseMainPage : ContentPage
{
    private readonly ICubaseHttpClient client;
    private readonly IMidiWebSocketClient webSocketClient;
    private readonly IMidiWebSocketResponse midiWebSocketResponse;
    private readonly AppSettings appSettings;
    private CubaseCommandsCollection collections; // store once

    private bool loaded = false;

    private BasePage basePage;

    private MixerPage mixerPage;

    public CubaseMainPage(AppSettings appSettings, 
                          ICubaseHttpClient client, 
                          IMidiWebSocketClient webSocketClient, 
                          BasePage basePage,
                          IMidiWebSocketResponse midiWebSocketResponse,
                          MixerPage mixerPage)
    {
        InitializeComponent();
        this.mixerPage = mixerPage;
        this.basePage = basePage;
        this.appSettings = appSettings;
        this.midiWebSocketResponse = midiWebSocketResponse;
        this.webSocketClient = webSocketClient;
        basePage.AddToolbars(this);
        this.client = client;
        CollectionsLayout.Clear();

        //this.serverAvailable = this.client.CanConnectToServer();
        //if (!this.serverAvailable)
        //{
        //    DisplayAlert("Error CubaseMainPage CTOR", $"Cannot connect to server {this.client.GetBaseConnection()}", "OK");
        //}
        BackgroundColor = ColourConstants.WindowBackground.ToMauiColor();
        var label = new Label
        {
            Text = "Loading ..",
            TextColor = Colors.Black,
            FontSize = 20,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        CollectionsLayout.Children.Add(label);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (loaded) return;
        SetSpinner(true);
        var webSocketState = await this.webSocketClient.ConnectAsync();
        if (webSocketState.Command == Common.WebSocket.WebSocketCommand.Connected)
        {
            await LoadCommands();
            loaded = true;
            SetSpinner(false);
        }
        else
        {
            await DisplayAlert($"Cannot connect to {this.appSettings.CubaseConnection.Host}:{this.appSettings.CubaseConnection.Port}", webSocketState.Message, "OK");
            SetSpinner(false);
        }
    }

    public async Task Reload()
    {
        SetSpinner(true);
        await LoadCommands();
        loaded = true;
        SetSpinner(false);
    }

    private async Task LoadCommands()
    {
        try
        {
            await this.webSocketClient.SendMidiCommand(WebSocketMessage.Create(WebSocketCommand.Commands));
            collections = await this.midiWebSocketResponse.GetCommands();
            /*
            // CollectionsLayout.Children.Clear();
            collections = await this.client.GetCommands(async (msg) =>
            {
                // todo show acrtionsin a non-blocking ui section
            }, async (exception) =>
            {
                await DisplayAlert("Error CubaseMainPage LoadCommands", exception, "OK");
            });
            */

            var mixerButton = RaisedButtonFactory.Create("Mixer", System.Drawing.Color.DarkGoldenrod.ToSerializableColour(), System.Drawing.Color.Black.ToSerializableColour(), async (s, e) =>
            {
                try
                {
                    await Navigation.PushAsync(this.mixerPage);
                    await this.mixerPage.Initialise(collections);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error CubaseMainPage LoadCommands - Initialise Mixer", ex.Message, "OK");
                }
            });
            CollectionsLayout.Children.Add(mixerButton.Button);

            if (collections == null || collections.Count == 0) return;



            foreach (var collection in collections)
            {

                if (collection.Visible)
                {
                    var button = RaisedButtonFactory.Create(collection.Name, collection.BackgroundColour, collection.TextColour, async (s, e) =>
                    {
                        try
                        {
                            await Navigation.PushAsync(new CubaseAction(collection, collections, this.client, this.basePage));
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Error CubaseMainPage LoadCommands", ex.Message, "OK");
                        }
                    });
                    CollectionsLayout.Children.Add(button.Button);
                }
            }
            CollectionsLayout.Children.RemoveAt(0); // remove loading button
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error CubaseMainPage LoadCommands", ex.Message, "OK");
        }
    }

    private void SetSpinner(bool state)
    {
        LoadingSpinner.IsVisible = state;
        LoadingSpinner.IsRunning = state;
    }
}
