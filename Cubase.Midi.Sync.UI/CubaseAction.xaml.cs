using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.UI.NutstoneServices.NutstoneClient;
using System.Net.Http;

namespace Cubase.Midi.Sync.UI;

public partial class CubaseAction : ContentPage
{
	private readonly CubaseCommandCollection collection;

	private readonly ICubaseHttpClient httpClient;


    public CubaseAction(CubaseCommandCollection collection, ICubaseHttpClient httpClient)
	{
		InitializeComponent();
		this.collection = collection;
		this.httpClient = httpClient;
        this.LoadCommands();
	}

    private void LoadCommands()
    {
        foreach (var cmd in collection.Commands)
        {
            var button = new Button
            {
                Text = cmd.Name,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Colors.MediumPurple,
                TextColor = Colors.White,
                CornerRadius = 12
            };

            button.Clicked += async (s, e) =>
            {
                bool confirm = await DisplayAlert("Send Action?", $"Do you want to send '{cmd.Action}' to server?", "Yes", "No");
                if (confirm)
                {
                    await SendAction(cmd.Action);
                }
            };

            button.SizeChanged += (s, e) =>
            {
                var btn = s as Button;
                btn.FontSize = btn.Height * 0.5; // adjust factor as needed
            };

            CommandsLayout.Children.Add(button);
        }
    }

    private async Task SendAction(string action)
    {
        try
        {
            //var response = await _httpClient.PostAsJsonAsync("run", new { action });
            //if (response.IsSuccessStatusCode)
            //{
            //    await DisplayAlert("Success", $"Action '{action}' sent to server.", "OK");
           // }
            //else
            //{
            //    var error = await response.Content.ReadAsStringAsync();
            //    await DisplayAlert("Error", error, "OK");
            //}
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}