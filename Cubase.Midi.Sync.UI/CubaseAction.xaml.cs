using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Colours;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.Responses;
using Cubase.Midi.Sync.UI.Extensions;
using Cubase.Midi.Sync.UI.NutstoneServices.NutstoneClient;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Cubase.Midi.Sync.UI;

public partial class CubaseAction : ContentPage
{
    private readonly CubaseCommandCollection commands;
    private readonly ICubaseHttpClient client;
    private readonly List<CubaseCommandCollection> commandsCollection;
    public CubaseAction(CubaseCommandCollection commands, List<CubaseCommandCollection> commandsCollection, ICubaseHttpClient client)
    {
        InitializeComponent();
        this.commands = commands;
        this.client = client;
        this.commandsCollection = commandsCollection;   
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
                    await DisplayAlert("Error", ex.Message, "OK");
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
            await DisplayAlert("Error", ex.Message, "OK");
        });
        if (!response.Success)
        {
            await DisplayAlert("Error", errMsg ?? "Is cubase up?", "OK");
            command.IsToggled = !command.IsToggled;
        }
        VisualStateManager.GoToState(button, "Normal");
        SetButtonState(button, command);
    }

    private async Task SetMacroButton(Button button, CubaseCommand command)
    {
        var actionStrings = new List<string>();
        var tmpCommands = new List<CubaseCommand>();
        // locate all the actions for this macro 
        var actionGroup = command.ActionGroup;


        if (command.ButtonType == CubaseButtonType.MacroToggle)
        {
            actionGroup = command.IsToggled ? command.ActionGroup : command.ActionGroupToggleOff;
        }

        foreach (var cmd in actionGroup)
        {
            // find cubase command 
            var cubaseCommands = this.commandsCollection.SelectMany(x => x.Commands)
                                                       .Where(x => x.Name == cmd);
            tmpCommands.AddRange(cubaseCommands);
            tmpCommands.ForEach(x => x.IsToggled = command.IsToggled);
            var firstCommand = cubaseCommands.First();
            // if it's a macro - then we have to add all the commands 
            if (firstCommand.IsMacro)
            {
                if (firstCommand.ButtonType == CubaseButtonType.Macro)
                {
                    actionStrings.AddRange(this.GetKeyCommandFromKeyName(firstCommand.ActionGroup)); ;
                }
                else
                {
                    if (command.IsToggled)
                    {
                        actionStrings.AddRange(this.GetKeyCommandFromKeyName(firstCommand.ActionGroup));
                    }
                    else
                    {
                        actionStrings.AddRange(this.GetKeyCommandFromKeyName(firstCommand.ActionGroupToggleOff));
                    }
                }
            }
            else
            {
                actionStrings.Add(firstCommand.Action);
            }
        }
        VisualStateManager.GoToState(button, "Pressed");
        this.SetButtonStateForMacroChildren(tmpCommands, command.IsToggled);
        var response = await this.client.ExecuteCubaseAction(CubaseActionRequest.CreateFromCommand(command, actionStrings), async (ex) =>
        {
            await DisplayAlert("Error", ex.Message, "OK");
        });
        if (response.Success)
        {

        }
    }

    private List<string> GetKeyCommandFromKeyName(List<string> keyNames)
    {
        return this.commandsCollection.SelectMany(x => x.Commands)
                                      .Where(x => keyNames.Contains(x.Name))
                                      .Select(x => x.Action)
                                      .Distinct()
                                      .ToList();
    }

    private void SetButtonStateForMacroChildren(IEnumerable<CubaseCommand> cubaseCommands, bool toggled)
    {
        foreach (var cubaseCmd in cubaseCommands)
        {
            var testButton = CommandsLayout.Children.FirstOrDefault(x => ((Button)x).Text.Equals(cubaseCmd.IsToggled ? cubaseCmd.Name : cubaseCmd.NameToggle));
            
            if (testButton != null)
            {
                SetButtonState(((Button)testButton), cubaseCmd);
            }
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
