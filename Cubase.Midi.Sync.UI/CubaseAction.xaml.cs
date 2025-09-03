using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Colours;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.Responses;
using Cubase.Midi.Sync.UI.Extensions;
using Cubase.Midi.Sync.UI.NutstoneServices.NutstoneClient;
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
                    command.IsToggled = !command.IsToggled;
                    this.SetButtonState((Button)s, command);
                    CubaseActionResponse response = null;

                    if (command.ButtonType == CubaseButtonType.Macro || command.ButtonType == CubaseButtonType.MacroToggle)
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
                            var firstCommand = cubaseCommands.First();
                            // if it's a macro - then we have to add all the commands 
                            if (firstCommand.ButtonType == CubaseButtonType.MacroToggle || firstCommand.ButtonType == CubaseButtonType.Macro)
                            {
                                if (firstCommand.ButtonType == CubaseButtonType.Macro)
                                {
                                    actionStrings.AddRange(firstCommand.ActionGroup);
                                }
                                else
                                {
                                    if (command.IsToggled)
                                    {
                                        actionStrings.AddRange(firstCommand.ActionGroup);
                                    }
                                    else
                                    {
                                        actionStrings.AddRange(firstCommand.ActionGroupToggleOff);
                                    }
                                }
                            }
                            else
                            {
                                actionStrings.Add(firstCommand.Action);
                            }
                        }
                        response = await this.client.ExecuteCubaseAction(CubaseActionRequest.CreateFromCommand(command, actionStrings), async (ex) =>
                        {
                            await DisplayAlert("Error", ex.Message, "OK");
                        });
                        if (response.Success)
                        {
                            // toggle any buttons that have been truned on/off 
                            foreach (var cubaseCmd in tmpCommands)
                            {
                                cubaseCmd.IsToggled = !cubaseCmd.IsToggled;
                            }

                            var uniqueCommands = tmpCommands
                                .GroupBy(p => p.Name)
                                .Select(g => g.First())
                                .ToList();
                            
                            foreach (var cubaseCmd in uniqueCommands)
                            {
                                foreach (var btn in CommandsLayout.Children)
                                {
                                    var testBtn = (Button)btn;
                                    if (testBtn.Text == (cubaseCmd.IsToggled ? cubaseCmd.NameToggle : cubaseCmd.Name))
                                    {
                                        SetButtonState(testBtn, cubaseCmd); 
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        string errMsg = null;
                        response = await this.client.ExecuteCubaseAction(CubaseActionRequest.CreateFromCommand(command), async (ex) =>
                        {
                            errMsg = ex.Message;    
                            await DisplayAlert("Error", ex.Message, "OK");
                        });
                        if (!response.Success)
                        {
                            await DisplayAlert("Error", errMsg ?? "Is cubase up?", "OK");
                            command.IsToggled = !command.IsToggled;
                        }
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
