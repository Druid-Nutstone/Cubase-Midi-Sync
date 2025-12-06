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

    private RecordingPage recordingPage;

    public CubaseMainPage(AppSettings appSettings, 
                          ICubaseHttpClient client, 
                          IMidiWebSocketClient webSocketClient, 
                          BasePage basePage,
                          IMidiWebSocketResponse midiWebSocketResponse,
                          MixerPage mixerPage,
                          RecordingPage recordingPage)
    {
        InitializeComponent();
        this.mixerPage = mixerPage;
        this.basePage = basePage;
        this.recordingPage = recordingPage;
        this.appSettings = appSettings;
        this.midiWebSocketResponse = midiWebSocketResponse;
        this.webSocketClient = webSocketClient;
        basePage.AddToolbars(this);
        this.client = client;
        CollectionsLayout.Clear();
        midiWebSocketResponse.RegisterForSystemMessages(this.ProcessSystemError);
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
        if (this.appSettings.Connect)
        {
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
                var activeconnection = this.appSettings.CubaseConnection.First(x => x.Name.Equals(this.appSettings.ActiveConnection, StringComparison.OrdinalIgnoreCase));
                await DisplayAlert($"Cannot connect to {activeconnection.Host}:{activeconnection.Port}", webSocketState.Message, "OK");
                SetSpinner(false);
            }
        }
        else
        {
            await DisplayAlert("Oops","Appsettings connect is set to false. so not connecting", "OK");
        }
    }

    public async Task Reload()
    {
        SetSpinner(true);
        var checkSocket = await this.webSocketClient.ConnectIfNotConnectedAsync();
        if (string.IsNullOrEmpty(checkSocket.Message))
        {
            await LoadCommands();
            loaded = true;
            SetSpinner(false);
        }
        else
        {
            await DisplayAlert("Oops - cannot connect to server", checkSocket.Message, "OK");
        }
    }

    private async Task ProcessSystemError(WebSocketCommand webSocketCommand)
    {
        switch (webSocketCommand)
        {
            case WebSocketCommand.ServerClosed:
                await DisplayAlert("Cubase Server Closed", "The Cubase Midi Sync server has closed the connection.", "OK");
                CollectionsLayout.Children.Clear();
                break;
            case WebSocketCommand.CubaseNotReady:
                CollectionsLayout.Children.Clear();
                CollectionsLayout.Children.Add(new Label
                {
                    Text = "Cubase Not Ready",
                    TextColor = Colors.Red,
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                });
                break;
            case WebSocketCommand.CubaseReady:
                await Reload();
                break;
        }
    }

    private async Task LoadCommands()
    {
        try
        {
            collections = null;
            await this.webSocketClient.SendMidiCommand(WebSocketMessage.Create(WebSocketCommand.Commands));
            collections = await this.midiWebSocketResponse.GetCommands();
            
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
            }, this.appSettings);
            CollectionsLayout.Children.Add(mixerButton.Button);

            var recordingButton = RaisedButtonFactory.Create("Record", System.Drawing.Color.Red.ToSerializableColour(), System.Drawing.Color.White.ToSerializableColour(), async (s, e) =>
            {
                try
                {
                    await Navigation.PushAsync(this.recordingPage);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error CubaseMainPage LoadCommands - Initialise Recording", ex.Message, "OK");
                }
            }, this.appSettings);
            CollectionsLayout.Children.Add(recordingButton.Button);

            if (collections == null || collections.Count == 0) return;



            foreach (var collection in collections)
            {

                if (collection.Visible)
                {
                    var button = RaisedButtonFactory.Create(collection.Name, collection.BackgroundColour, collection.TextColour, async (s, e) =>
                    {
                        try
                        {
                            await Navigation.PushAsync(new CubaseAction(collection, collections, this.client, this.webSocketClient, this.midiWebSocketResponse, this.appSettings, this.basePage));
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Error CubaseMainPage LoadCommands", ex.Message, "OK");
                        }
                    }, this.appSettings);
                    CollectionsLayout.Children.Add(button.Button);
                }
            }
            CollectionsLayout.Children.RemoveAt(0); // remove loading button
            ScrollView.InvalidateMeasure();
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
