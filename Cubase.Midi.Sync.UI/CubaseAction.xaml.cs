using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Colours;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.UI.Extensions;
using Cubase.Midi.Sync.UI.NutstoneServices.NutstoneClient;
using Microsoft.Maui.Graphics;

namespace Cubase.Midi.Sync.UI;

public partial class CubaseAction : ContentPage
{
    private readonly CubaseCommandCollection commands;
    private readonly ICubaseHttpClient client;
    public CubaseAction(CubaseCommandCollection commands, ICubaseHttpClient client)
    {
        InitializeComponent();
        this.commands = commands;
        this.client = client;
        BackgroundColor = ColourConstants.WindowBackground.ToMauiColor();
        Title = commands.Name;
        LoadCommand();
    }

    private void LoadCommand()
    {
        CommandsLayout.Children.Clear();

        foreach (var command in commands.Commands)
        {
            var button = RaisedButtonFactory.Create(command.Name, command.ButtonBackgroundColour, command.ButtonTextColour, async (s, e) =>
            {
                try
                {
                    command.IsToggled = !command.IsToggled;
                    this.SetButtonState((Button)s, command);    
                    var response = await this.client.ExecuteCubaseAction(CubaseActionRequest.CreateFromCommand(command), async (ex) =>
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
            }, toggleMode: true);
            this.SetButtonState(button.Button, command);
            CommandsLayout.Children.Add(button.Button);
        }
    }

    private void SetButtonState(Button button, CubaseCommand command)
    {
        button.BackgroundColor = command.ButtonColour.ToMauiColour();
        button.TextColor = command.TextColor.ToMauiColour();
    }
}
