using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Colours;
using Cubase.Midi.Sync.Common.InternalCommands;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.Responses;
using Cubase.Midi.Sync.UI.Extensions;
using Cubase.Midi.Sync.UI.NutstoneServices.NutstoneClient;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Cubase.Midi.Sync.UI;

// To fix CS0263, ensure all partial declarations of 'CubaseAction' specify the same base class.
// If you have another file (e.g., CubaseAction.xaml) with a partial class declaration like:
// public partial class CubaseAction : ContentPage
// Change it to:
// public partial class CubaseAction : BasePage
//
// No code changes are needed in this file if the base class is correct here.
// Please update the other partial class declaration(s) to match this base class: BasePage.
public partial class CubaseAction : ContentPage
{
    private readonly CubaseCommandCollection commands;
    private readonly ICubaseHttpClient client;
    private readonly List<CubaseCommandCollection> commandsCollection;

    private Dictionary<InternalCommandType, Action<InternalCommand>> internalCommands;
    
    private BasePage basePage;  

    public CubaseAction(CubaseCommandCollection commands, List<CubaseCommandCollection> commandsCollection, ICubaseHttpClient client, BasePage basePage)
    {
        this.basePage = basePage;
        basePage.AddToolbars(this);  
        InitializeComponent();
        this.commands = commands;
        this.client = client;
        this.commandsCollection = commandsCollection;   
        this.internalCommands = new Dictionary<InternalCommandType, Action<InternalCommand>>()
        {
            { InternalCommandType.Navigate, this.ProcessInternalNavigate }
        };
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
                    var button = (Button)s;
                    
                    command.IsToggled = !command.IsToggled;
                    this.SetButtonState(button, command);
                    CubaseActionResponse response = null;

                    if (command.IsMacro)
                    {
                        await this.SetMacroButton(button, command);
                    }
                    else
                    {
                        await this.SetMomentaryOrToggleButton(button, command);
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error CubaseAction.cs LoadCommand", ex.Message, "OK");
                }
            }, toggleMode: true);
            this.SetButtonState(button.Button, command);
            CommandsLayout.Children.Add(button.Button);
        }
    }

    private async Task SetMomentaryOrToggleButton(Button button, CubaseCommand command)
    {
        string errMsg = null;
        VisualStateManager.GoToState(button, "Pressed");
        var response = await this.client.ExecuteCubaseAction(CubaseActionRequest.CreateFromCommand(command), async (ex) =>
        {
            errMsg = ex.Message;
            await DisplayAlert("Error CubaseAction SetMomentaryOrToggleButton", ex.Message, "OK");
        });
        if (!response.Success)
        {
            await DisplayAlert("Error SetMomentaryOrToggleButton", errMsg ?? "Is cubase up?", "OK");
            command.IsToggled = !command.IsToggled;
        }
        VisualStateManager.GoToState(button, "Normal");
        SetButtonState(button, command);
    }

    private async Task SetMacroButton(Button button, CubaseCommand command)
    {
        var actionGroup = command.ActionGroup;
        if (command.ButtonType == CubaseButtonType.MacroToggle)
        {
            actionGroup = command.IsToggled ? command.ActionGroup : command.ActionGroupToggleOff;
        }
        VisualStateManager.GoToState(button, "Pressed");
        var response = await this.client.ExecuteCubaseAction(CubaseActionRequest.CreateFromCommand(command, actionGroup), async (ex) =>
        {
            await DisplayAlert("Error CubaseAction SetMacroButton", ex.Message, "OK");
        });
        if (response.Success)
        {
            var duplicateCommands = this.commandsCollection
                            .SelectMany(x => x.Commands)
                            .Where(x => x.Name.Equals(command.Name))
                            .Where(x => x != command);
            foreach (var dupItem in duplicateCommands)
            {
                dupItem.IsToggled = command.IsToggled;
            }
        }
    }

    private void ProcessInternalNavigate(InternalCommand command)
    {
        var target = this.commandsCollection.FirstOrDefault(x => x.Name.Equals(command.Parameter, StringComparison.OrdinalIgnoreCase));
        if (target != null)
        {
            Navigation.PushAsync(new CubaseAction(target, this.commandsCollection, this.client, this.basePage));
        }
    }

    private void SetButtonState(Button button, CubaseCommand command)
    {
        button.BackgroundColor = command.ButtonColour.ToMauiColour();
        button.TextColor = command.TextColor.ToMauiColour();
        if (command.IsToggleButton && command.IsToggled)
        {
            button.Text = command.NameToggle;
        }
        else
        {
            button.Text = command.Name;
        }
    }

}
