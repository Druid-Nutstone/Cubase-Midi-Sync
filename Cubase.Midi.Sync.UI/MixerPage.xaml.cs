using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Colours;
using Cubase.Midi.Sync.Common.Extensions;
using Cubase.Midi.Sync.Common.InternalCommands;
using Cubase.Midi.Sync.Common.Midi;
using Cubase.Midi.Sync.Common.Mixer;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.Responses;
using Cubase.Midi.Sync.UI.Extensions;
using Cubase.Midi.Sync.UI.NutstoneServices.NutstoneClient;
using Microsoft.Maui.Controls;

namespace Cubase.Midi.Sync.UI
{

    public partial class MixerPage : ContentPage
    {
        private ICubaseHttpClient cubaseHttpClient;

        private BasePage basePage;

        private IServiceProvider serviceProvider;

        private CubaseMixerCollection mixerCollection;

        private List<CubaseCommand> staticCommands;

        public MixerPage(ICubaseHttpClient cubaseHttpClient, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            this.cubaseHttpClient = cubaseHttpClient;
            this.serviceProvider = serviceProvider;
            BackgroundColor = ColourConstants.WindowBackground.ToMauiColor();

        }

        protected override async void OnDisappearing()
        {
            await this.OpenCloseMixer();
            base.OnDisappearing();
        }

        private async Task InitialiseCustomCommands(List<CubaseCommandCollection> collections)
        {
            CustomCommands.Children.Clear();

            var customCommands = collections.Where(x => x.Visible)
                                            .SelectMany(x => x.Commands)
                                            .Where(x => x.IsAvailableToTheMixer);

            foreach (var command in customCommands)
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
                            await this.SetMacroButton(button, command, collections);
                        }
                        else
                        {
                            await this.SetMomentaryOrToggleButton(button, command);
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error MixerPage InitialiseCustomCommands", ex.Message, "OK");
                    }
                });
                CustomCommands.Children.Add(button.Button);
            }
        }

        private async Task InitialisePages(List<CubaseCommandCollection> collections)
        {
            Pages.Children.Clear();

            var homeButton = RaisedButtonFactory.Create("Home", ColourConstants.ButtonBackground.ToSerializableColour(), ColourConstants.ButtonText.ToSerializableColour(), async (s, e) =>
            {
                try
                {
                    await Navigation.PopToRootAsync();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error InitialisePages", ex.Message, "OK");
                }
            });
            Pages.Children.Add(homeButton.Button);

            foreach (var collection in collections)
            {

                if (collection.Visible)
                {
                    var button = RaisedButtonFactory.Create(collection.Name, collection.BackgroundColour, collection.TextColour, async (s, e) =>
                    {
                        try
                        {
                            await Navigation.PushAsync(new CubaseAction(collection, collections, this.cubaseHttpClient, this.basePage));
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Error InitialisePages", ex.Message, "OK");
                        }
                    });
                    Pages.Children.Add(button.Button);
                }
            }
        }

        private void InitialiseMixer()
        {
            this.StaticButtons.Children.Clear();
            this.staticCommands = new List<CubaseCommand>();
            foreach (var midiToggleCommand in this.mixerCollection.Where(x => x.Visible))
            {
                this.staticCommands.Add(CubaseCommand.CreateToggleButton(midiToggleCommand.ButtonText, midiToggleCommand.Command.ToString(), midiToggleCommand.ButtonTextToggled)
                                                     .WithCategory(CubaseServiceConstants.MidiService)
                                                     .WithIsInitiallyVisible(midiToggleCommand.IsInitiallyVisible));

            }

            foreach (var command in this.staticCommands)
            {
                var btn = RaisedButtonFactory.Create(command.Name, ColourConstants.ButtonBackground.ToSerializableColour(), ColourConstants.ButtonText.ToSerializableColour(), async (s, e) =>
                {
                    this.mixerCollection = await this.cubaseHttpClient.SetMixer(CubaseMixer.Create(Enum.Parse<KnownCubaseMidiCommands>(command.Action), string.Empty, string.Empty), this);
                    foreach (var mixer in mixerCollection)
                    {
                        // command.IsToggled = mixer.Toggled;
                        var showHideBtn = this.StaticButtons.Children.OfType<Button>().FirstOrDefault(x => x.AutomationId == mixer.Command.ToString());
                        // if null it's probably open/close mixer 
                        if (showHideBtn != null)
                        {
                            showHideBtn.IsVisible = mixer.Visible;
                            var mixerCommand = this.staticCommands.First(x => x.Name.Equals(mixer.ButtonText, StringComparison.OrdinalIgnoreCase));
                            mixerCommand.IsToggled = mixer.Toggled;
                            this.SetButtonState(showHideBtn, mixerCommand);
                        }
                    }
                    // this.SetButtonState((Button)s, command);
                }, command.IsToggleButton, command.Action);
                StaticButtons.Children.Add(btn.Button);
                if (!command.IsInitiallyVisible)
                {
                    btn.Button.IsVisible = false;
                }
            }
        }

        private async Task<CubaseMixerCollection> OpenCloseMixer()
        {
            return await this.cubaseHttpClient.SetMixer(CubaseMixer.Create(KnownCubaseMidiCommands.Mixer, string.Empty, string.Empty), this);
        }

        public async Task Initialise(List<CubaseCommandCollection>? mainCommands = null)
        {
            this.basePage = this.serviceProvider.GetService<BasePage>();
            this.basePage.AddToolbars(this);

            try
            {
                this.mixerCollection = await this.OpenCloseMixer();
                if (this.mixerCollection.Count > 0)
                {
                    this.InitialiseMixer();
                    if (mainCommands != null)
                    {
                        await this.InitialisePages(mainCommands);
                        await this.InitialiseCustomCommands(mainCommands);
                    }
                    else
                    {
                        await DisplayAlert("ERROR from GetMixer", "The mixer collection contains zero records", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("ERROR from getmixer", $"The mixer collection has {this.mixerCollection.Count.ToString()} records in it", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error MixerPage GetMixer", $"Error getting mixer {ex.Message}", "OK");
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

        private async Task SetMomentaryOrToggleButton(Button button, CubaseCommand command)
        {
            string errMsg = null;
            VisualStateManager.GoToState(button, "Pressed");
            var response = await this.cubaseHttpClient.ExecuteCubaseAction(CubaseActionRequest.CreateFromCommand(command), async (ex) =>
            {
                errMsg = ex.Message;
                await DisplayAlert("Error SetMomentaryOrToggleButton", ex.Message, "OK");
            });
            if (!response.Success)
            {
                await DisplayAlert("Error SetMomentaryOrToggleButton", errMsg ?? "Is cubase up?", "OK");
                command.IsToggled = !command.IsToggled;
            }
            VisualStateManager.GoToState(button, "Normal");
            SetButtonState(button, command);
        }

        private async Task SetMacroButton(Button button, CubaseCommand command, List<CubaseCommandCollection> commandsCollection)
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
                if (!InternalCommandsCollection.IsInternalCommand(cmd))
                {
                    // find cubase command 
                    var cubaseCommands = commandsCollection.SelectMany(x => x.Commands)
                                                               .Where(x => x.Name == cmd);
                    tmpCommands.AddRange(cubaseCommands);
                    tmpCommands.ForEach(x => x.IsToggled = command.IsToggled);
                    var firstCommand = cubaseCommands.First();
                    // if it's a macro - then we have to add all the commands 
                    if (firstCommand.IsMacro)
                    {
                        if (firstCommand.ButtonType == CubaseButtonType.Macro)
                        {
                            actionStrings.AddRange(this.GetKeyCommandFromKeyName(firstCommand.ActionGroup, commandsCollection)); ;
                        }
                        else
                        {
                            if (command.IsToggled)
                            {
                                actionStrings.AddRange(this.GetKeyCommandFromKeyName(firstCommand.ActionGroup, commandsCollection));
                            }
                            else
                            {
                                actionStrings.AddRange(this.GetKeyCommandFromKeyName(firstCommand.ActionGroupToggleOff, commandsCollection));
                            }
                        }
                    }
                    else
                    {
                        actionStrings.Add(firstCommand.Action);
                    }
                }
            }
            VisualStateManager.GoToState(button, "Pressed");
            this.SetButtonStateForMacroChildren(tmpCommands, command.IsToggled);
            var response = await this.cubaseHttpClient.ExecuteCubaseAction(CubaseActionRequest.CreateFromCommand(command, actionStrings), async (ex) =>
            {
                await DisplayAlert("Error SetMacroButton", ex.Message, "OK");
            });
            if (response.Success)
            {

            }
        }

        private void SetButtonStateForMacroChildren(IEnumerable<CubaseCommand> cubaseCommands, bool toggled)
        {
            foreach (var cubaseCmd in cubaseCommands)
            {
                var testButton = CustomCommands.Children.FirstOrDefault(x => ((Button)x).Text.Equals(cubaseCmd.IsToggled ? cubaseCmd.Name : cubaseCmd.NameToggle));

                if (testButton != null)
                {
                    SetButtonState(((Button)testButton), cubaseCmd);
                }
            }
        }

        private List<string> GetKeyCommandFromKeyName(List<string> keyNames, List<CubaseCommandCollection> commandsCollection)
        {
            return commandsCollection.SelectMany(x => x.Commands)
                                          .Where(x => keyNames.Contains(x.Name))
                                          .Select(x => x.Action)
                                          .Distinct()
                                          .ToList();
        }
    }
}
