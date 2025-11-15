using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Colours;
using Cubase.Midi.Sync.Common.Extensions;
using Cubase.Midi.Sync.Common.InternalCommands;
using Cubase.Midi.Sync.Common.Midi;
using Cubase.Midi.Sync.Common.Mixer;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.Responses;
using Cubase.Midi.Sync.Common.WebSocket;
using Cubase.Midi.Sync.Common.Window;
using Cubase.Midi.Sync.UI.CubaseService.WebSocket;
using Cubase.Midi.Sync.UI.Extensions;
using Cubase.Midi.Sync.UI.Models;
using Cubase.Midi.Sync.UI.NutstoneServices.NutstoneClient;
using Microsoft.Maui.Controls;

namespace Cubase.Midi.Sync.UI
{

    public partial class MixerPage : ContentPage
    {
        private ICubaseHttpClient cubaseHttpClient;

        private IMidiWebSocketClient webSocketClient;

        private IMidiWebSocketResponse midiWebSocketResponse;

        private CubaseActiveWindowCollection cubaseActiveWindows;

        private BasePage basePage;

        private IServiceProvider serviceProvider;

        public CubaseMixerCollection mixerCollection { get; set; }

        private List<CubaseCommand> staticCommands;

        private IMidiWebSocketResponse webSocketResponse;

        private List<CubaseCommandCollection> commandCollection;

        private CubaseUIMixerCollection uiMixerCollection = new CubaseUIMixerCollection();

        private string currentMixerConsole;

        public MixerPage(ICubaseHttpClient cubaseHttpClient, 
                         IMidiWebSocketClient webSocketClient, 
                         IMidiWebSocketResponse midiWebSocketResponse,
                         IServiceProvider serviceProvider)
        {
            InitializeComponent();
            this.cubaseHttpClient = cubaseHttpClient;
            this.serviceProvider = serviceProvider;
            this.webSocketClient = webSocketClient;
            this.webSocketResponse = midiWebSocketResponse;
            this.midiWebSocketResponse = midiWebSocketResponse;
            this.midiWebSocketResponse.RegisterCubaseWindowHandler(this.OnCubaseWindowChanges);
            BackgroundColor = ColourConstants.WindowBackground.ToMauiColor();

        }

        private async Task OnCubaseWindowChanges(CubaseActiveWindowCollection cubaseActiveWindows)
        {
            this.cubaseActiveWindows = cubaseActiveWindows;
            await this.UpdateMixConsoles();
        }
        
        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private async Task UpdateMixConsoles()
        {
            uiMixerCollection.Populate(this.cubaseActiveWindows);
            if (this.cubaseActiveWindows.GetAllMixers().Count > 0)
            {
                currentMixerConsole = uiMixerCollection.FirstOrDefault(x => x.GetZOrder() == CubaseWindowZOrder.Focused)?.WindowTitle ?? string.Empty;
            }

            foreach (var mixerButton in MixerConsoles.Children.OfType<Button>())
            {
                var buttonText = mixerButton.Text;
                var uimixer = this.uiMixerCollection.FirstOrDefault(x => x.Name.Equals(mixerButton.Text, StringComparison.OrdinalIgnoreCase));
                if (uimixer != null)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        mixerButton.BackgroundColor = uimixer.GetBackgroundColour();
                        mixerButton.TextColor = uimixer.GetForeGroundColour();
                        mixerButton.InvalidateMeasure();
                        (mixerButton.Parent as VisualElement)?.InvalidateMeasure();
                    });
                }
            }
        }

        private async void InitialiseMixers()
        {
            MixerConsoles.Children.Clear(); 
            foreach (var uimixer in this.uiMixerCollection)
            {
                var button = RaisedButtonFactory.Create(uimixer.Name, System.Drawing.Color.White.ToSerializableColour(), System.Drawing.Color.SlateGray.ToSerializableColour(), async (s, e) =>
                {
                    try
                    {
                        var button = (Button)s;
                        var mixer = this.uiMixerCollection.GetMixerByName(button.Text); 
                        if (mixer != null)
                        {
                            switch (mixer.GetZOrder())
                            {
                                case CubaseWindowZOrder.Focused:
                                case CubaseWindowZOrder.Active:
                                    await this.SendMidiCommand(CubaseMixerCommand.FocusMixer, mixer.WindowTitle);
                                    break;
                                case CubaseWindowZOrder.Unknown:
                                    await SendMidiCommand(CubaseMixerCommand.OpenMixer, mixer.Indentifier);
                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error InitialiseMixers", ex.Message, "OK");
                    }
                }, true);
                MixerConsoles.Children.Add(button.Button);
            }
            var closeButton = RaisedButtonFactory.Create("Close Mixers", System.Drawing.Color.Firebrick.ToSerializableColour(), System.Drawing.Color.White.ToSerializableColour(), async (s, e) => 
            {
                await this.SendMidiCommand(CubaseMixerCommand.CloseMixers);
                await this.Navigation.PopToRootAsync();
            });
            MixerConsoles.Children.Add(closeButton.Button);
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
                        CubaseActionResponse response = null;
                        if (command.IsMacro)
                        {
                            await this.SetMacroButton(button, command, collections);
                        }
                        else
                        {
                            await this.SetMomentaryOrToggleButton(button, command);
                        }
                        this.SetButtonState(button, command);
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error MixerPage InitialiseCustomCommands", ex.Message, "OK");
                    }
                }, command.IsToggleButton);
                this.SetButtonState(button.Button, command);
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
                            await Navigation.PushAsync(new CubaseAction(collection, collections, this.cubaseHttpClient, this.webSocketClient, this.webSocketResponse, this.basePage));
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
            foreach (var midiStaticCommand in this.mixerCollection.GetStaticMixConsoleCommands())
            {
                this.staticCommands.Add(CubaseCommand.CreateStandardButton(midiStaticCommand.ButtonText, ActionEvent.Create(CubaseAreaTypes.Midi, midiStaticCommand.Command.ToString())));
            }

            foreach (var command in this.staticCommands)
            {
                var btn = RaisedButtonFactory.Create(command.Name, ColourConstants.ButtonBackground.ToSerializableColour(), ColourConstants.ButtonText.ToSerializableColour(), async (s, e) =>
                {
                    this.webSocketResponse.mixerCollection = null;
                    await this.SendMidiCommand(CubaseMixerCommand.MixerStaticCommand, currentMixerConsole, command.Action);
                }, command.IsToggleButton, command.Action.Action);
                StaticButtons.Children.Add(btn.Button);
            }
        }

        private async Task<CubaseMixerCollection> OpenCloseMixer()
        {
            var socketMessage = WebSocketMessage.Create(WebSocketCommand.Windows, CubaseWindowRequest.CreateCommand(CubaseWindowRequestCommand.ActiveWindows));
            await this.webSocketClient.SendMidiCommand(socketMessage);
            if (this.cubaseActiveWindows == null || !this.cubaseActiveWindows.HaveAnyMixers())
            {
                await this.SendMidiCommand(CubaseMixerCommand.OpenMixer, this.uiMixerCollection.GetPrimaryMixerName());
            }
            else
            {
                // focus mixers and move them based on number of mixers   
            }
            return await this.webSocketResponse.GetMixer();

        }

        private async Task SendMidiCommand(CubaseMixerCommand cubaseMixerCommand, string? targetMixer = null, object? data = null)
        {
            var mixerRequest = CubaseMixerRequest.Create(cubaseMixerCommand, data, targetMixer);
            var socketMessage = WebSocketMessage.Create(WebSocketCommand.Mixer, mixerRequest);
            var response = await this.webSocketClient.SendMidiCommand(socketMessage);
        }

        public async Task Initialise(List<CubaseCommandCollection>? mainCommands = null)
        {
            this.basePage = this.serviceProvider.GetService<BasePage>();
            this.basePage.AddToolbars(this);

            try
            {
                this.commandCollection = mainCommands;
                this.mixerCollection = await this.OpenCloseMixer();
                if (this.mixerCollection.Count > 0)
                {
                    this.InitialiseMixers();
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
            var cubaseSocketRequest = CubaseActionRequest.CreateFromCommand(command);
            var socketMessage = WebSocketMessage.Create(WebSocketCommand.ExecuteCubaseAction, cubaseSocketRequest);
            var response = await this.webSocketClient.SendMidiCommand(socketMessage);
            if (response.Command != WebSocketCommand.Success)
            {
                await DisplayAlert("Error SetMomentaryOrToggleButton", errMsg ?? "Is cubase up?", "OK");
                command.IsToggled = !command.IsToggled;
            }
            VisualStateManager.GoToState(button, "Normal");
            SetButtonState(button, command);
        }

        private async Task SetMacroButton(Button button, CubaseCommand command, List<CubaseCommandCollection> commandsCollection)
        {
            var actionGroup = command.ActionGroup;


            if (command.ButtonType == CubaseButtonType.MacroToggle)
            {
                actionGroup = command.IsToggled ? command.ActionGroup : command.ActionGroupToggleOff;
            }

            await MainThread.InvokeOnMainThreadAsync(() => VisualStateManager.GoToState(button, "Pressed"));
            var cubaseSocketRequest = CubaseActionRequest.CreateFromCommand(command, actionGroup);
            var socketMessage = WebSocketMessage.Create(WebSocketCommand.ExecuteCubaseAction, cubaseSocketRequest);
            var response = await this.webSocketClient.SendMidiCommand(socketMessage);
            if (response.Command == WebSocketCommand.Success)
            {
                if (this.commandCollection != null)
                {
                    var duplicateCommands = this.commandCollection
                                    .SelectMany(x => x.Commands)
                                    .Where(x => x.Name.Equals(command.Name))
                                    .Where(x => x != command);
                    foreach (var dupItem in duplicateCommands)
                    {
                        dupItem.IsToggled = command.IsToggled;
                    }
                }
            }
        }
    }
}
