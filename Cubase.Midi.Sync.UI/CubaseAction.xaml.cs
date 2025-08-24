using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.UI.Extensions;
using Cubase.Midi.Sync.UI.NutstoneServices.NutstoneClient;
using Microsoft.Maui.Graphics;

namespace Cubase.Midi.Sync.UI;

public partial class CubaseAction : ContentPage
{
    private readonly CubaseCommandCollection _commands;
    private readonly ICubaseHttpClient _client;

    public CubaseAction(CubaseCommandCollection commands, ICubaseHttpClient client)
    {
        InitializeComponent();
        _commands = commands;
        _client = client;

        Title = commands.Name;
        LoadCommand();
    }

    private void LoadCommand()
    {
        CommandsLayout.Children.Clear();

        foreach (var _command in _commands.Commands)
        {
            var button = new Button
            {
                Text = _command.Name,
                BackgroundColor = _command.ButtonColor.ToMauiColor(),
                TextColor = _command.ForeColor.ToMauiColor(),   
                HorizontalOptions = LayoutOptions.Fill,
                FontAttributes = FontAttributes.Bold,
                CornerRadius = 12,
                HeightRequest = 60
            };

            button.Clicked += async (s, e) =>
            {
                try
                {
                    _command.IsToggled = !_command.IsToggled;
                    button.BackgroundColor = _command.ButtonColor.ToMauiColor();
                    var response = await _client.ExecuteCubaseAction(CubaseActionRequest.CreateFromCommand(_command), async (ex) => 
                    { 
                        await DisplayAlert("Error", ex.Message, "OK");  
                    }); 
                    if (!response.Success)
                    {
                        await DisplayAlert("Error", response.Message ?? "Unknown error", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.Message, "OK");
                }
            };

            button.SizeChanged += (s, e) =>
            {
                var b = (Button)s;
                b.FontSize = Math.Max(12, Math.Min(b.Height * 0.5, 18));
            };

            CommandsLayout.Children.Add(button);
        }
    }
}
