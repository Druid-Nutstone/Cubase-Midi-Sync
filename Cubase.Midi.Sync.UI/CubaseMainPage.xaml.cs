using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Colours;
using Cubase.Midi.Sync.Common.Extensions;
using Cubase.Midi.Sync.UI.Extensions;
using Cubase.Midi.Sync.UI.NutstoneServices.NutstoneClient;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace Cubase.Midi.Sync.UI;

public partial class CubaseMainPage : ContentPage
{
    private readonly ICubaseHttpClient client;
    private List<CubaseCommandCollection> collections; // store once

    private bool loaded = false;

    private bool serverAvailable = false;

    private BasePage basePage;

    private MixerPage mixerPage;    

    public CubaseMainPage(ICubaseHttpClient client, BasePage basePage, MixerPage mixerPage)
    {
        InitializeComponent();
        this.mixerPage = mixerPage; 
        this.basePage = basePage;   
        basePage.AddToolbars(this);
        this.client = client;
        CollectionsLayout.Clear();
        this.serverAvailable = this.client.CanConnectToServer();
        if (!this.serverAvailable)
        {
            DisplayAlert("Error", $"Cannot connect to server {this.client.GetBaseConnection()}", "OK");
        }
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
        if (loaded || !serverAvailable) return;
        SetSpinner(true);
        await LoadCommands();
        loaded = true;
        SetSpinner(false);
    }

 

    private async Task LoadCommands()
    {
        try
        {
            // CollectionsLayout.Children.Clear();
            collections = await this.client.GetCommands(async (msg) =>
            {
                // todo show acrtionsin a non-blocking ui section
            }, async (exception) =>
            {
                await DisplayAlert("Error", exception, "OK");
            });

            var mixerButton = RaisedButtonFactory.Create("Mixer", System.Drawing.Color.DarkGoldenrod.ToSerializableColour(), System.Drawing.Color.Black.ToSerializableColour(), async (s, e) =>
            {
                try
                {
                    await this.mixerPage.Initialise(collections);
                    await Navigation.PushAsync(this.mixerPage);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.Message, "OK");
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
                            await DisplayAlert("Error", ex.Message, "OK");
                        }
                    });
                    CollectionsLayout.Children.Add(button.Button);
                }
            }
            CollectionsLayout.Children.RemoveAt(0); // remove loading button    
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void SetSpinner(bool state)
    {
        LoadingSpinner.IsVisible = state;
        LoadingSpinner.IsRunning = state;
    }
}
