using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.UI.Extensions;
using Cubase.Midi.Sync.UI.NutstoneServices.NutstoneClient;
using Microsoft.Maui.Graphics;

namespace Cubase.Midi.Sync.UI;

public partial class CubaseMainPage : ContentPage
{
    private readonly ICubaseHttpClient client;
    private List<CubaseCommandCollection> collections; // store once

    private bool loaded = false;

    public CubaseMainPage(ICubaseHttpClient client)
    {
        InitializeComponent();
        this.client = client;
        CollectionsLayout.Clear();
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
            },async (exception) =>
            {
                await DisplayAlert("Error", exception, "OK");
            });

            if (collections == null) return;



            foreach (var collection in collections)
            {
                var button = new Button
                {
                    Text = collection.Name,
                    HeightRequest = 60,
                    WidthRequest = 150,
                    HorizontalOptions = LayoutOptions.Fill,
                    BackgroundColor = Colors.SkyBlue,
                    TextColor = Colors.White,
                    FontAttributes = FontAttributes.Bold,
                    CornerRadius = 12
                };

                button.Clicked += async (s, e) =>
                {
                    try
                    {
                        // await _client.SendAction(cmd.Action);
                        await Navigation.PushAsync(new CubaseAction(collection, this.client));
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", ex.Message, "OK");
                    }
                };

                button.SizeChanged += (s, e) =>
                {
                    var b = (Button)s;
                    if (b.Height > 0)
                        b.FontSize = Math.Min(b.Height * 0.5, 18);
                };
                CollectionsLayout.Children.Add(button);
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
