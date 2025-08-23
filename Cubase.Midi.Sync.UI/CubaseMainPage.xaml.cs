using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.UI.NutstoneServices.NutstoneClient;
using System.Net.Http;

namespace Cubase.Midi.Sync.UI;

public partial class CubaseMainPage : ContentPage
{
	private readonly ICubaseHttpClient client;

    public CubaseMainPage(ICubaseHttpClient cubaseHttpClient)
	{
		InitializeComponent();
        this.client = cubaseHttpClient;
        this.LoadCommands();
    }


    private async void LoadCommands()
    {

        try
        {
            var collections = await client.GetCommands();
            if (collections != null)
            {
                foreach (var collection in collections)
                {
                    var button = new Button
                    {
                        Text = collection.Name,
                        HorizontalOptions = LayoutOptions.Fill,
                        // VerticalOptions = LayoutOptions.Center, // important
                        VerticalOptions = LayoutOptions.Start,  // align each button to top
                        BackgroundColor = Colors.SkyBlue,
                        TextColor = Colors.White,
                        CornerRadius = 12
                    };

                    button.Clicked += async (s, e) =>
                    {
                        // Navigate to details page
                        await Navigation.PushAsync(new CubaseAction(collection, this.client));
                    };

                    button.SizeChanged += (s, e) =>
                    {
                        var btn = s as Button;
                        btn.FontSize = btn.Height * 0.5; // adjust factor as needed
                    };

                    CollectionsLayout.Children.Add(button);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}