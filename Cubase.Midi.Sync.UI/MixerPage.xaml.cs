using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Colours;
using Cubase.Midi.Sync.Common.Extensions;
using Cubase.Midi.Sync.Common.Midi;
using Cubase.Midi.Sync.Common.Requests;
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

        private List<MidiChannel> tracks;

        private CubaseMidiCommandCollection commandCollection = new CubaseMidiCommandCollection();

        private string hideShowAudio;
        private string hideShowGroups;
        private string hideShowInputs;
        private string hideShowInstruments;
        private string hideShowMidi;
        private string hideShowOutputs;
        private string showSelectedTracks;

        private string mixerShowAll;

        private CubaseCommand showHideAll;

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
            if (this.showHideAll != null)
            {
                if (!this.showHideAll.IsToggled)
                {
                    var result = await this.cubaseHttpClient.ExecuteCubaseAction(CubaseActionRequest.CreateFromCommand(showHideAll), async (err) =>
                    {
                        await DisplayAlert("Error", err.Message, "OK");
                    });
                }
            }
            await this.OpenCloseMixer();

            base.OnDisappearing();
        }

        public async Task LoadTracks()
        {
            var trackCommands = new List<CubaseCommand>();

            TrackButtons.Children.Clear();
            trackCommands.AddRange(this.tracks.Select(x => CubaseCommand.CreateStandardButton(x.Name, x.Index.ToString())
                                                                        .WithCategory(CubaseServiceConstants.MidiService)));

            foreach (var trackCommand in trackCommands)
            {
                var btn = RaisedButtonFactory.Create(trackCommand.Name, ColourConstants.ButtonBackground.ToSerializableColour(), ColourConstants.ButtonText.ToSerializableColour(), async (s, e) =>
                {
                    var selectedTrack = this.tracks
                                            .First(tracks => tracks.Name == trackCommand.Name);
                    var newTracks = await this.cubaseHttpClient.SetSelectedTrack(selectedTrack);
                    this.tracks = newTracks.GetActiveChannels();
                    // this.SetButtonState((Button)s, trackCommand);

                }, true);
                TrackButtons.Children.Add(btn.Button);
            }
        }

        public async Task InitialiseStaticButtons()
        {
            StaticButtons.Children.Clear();

            this.hideShowAudio = this.commandCollection.GetCommandByName(KnownCubaseMidiCommands.Hide_Audio);
            this.hideShowGroups = this.commandCollection.GetCommandByName(KnownCubaseMidiCommands.Hide_Groups);
            this.hideShowInputs = this.commandCollection.GetCommandByName(KnownCubaseMidiCommands.Hide_Inputs);
            this.hideShowInstruments = this.commandCollection.GetCommandByName(KnownCubaseMidiCommands.Hide_Instruments);
            this.hideShowMidi = this.commandCollection.GetCommandByName(KnownCubaseMidiCommands.Hide_Midi);
            this.hideShowOutputs = this.commandCollection.GetCommandByName(KnownCubaseMidiCommands.Hide_Outputs);
            // this.showSelectedTracks = this.commandCollection.GetCommandByName(KnownCubaseMidiCommands.Show_Selected_Tracks);
            this.mixerShowAll = this.commandCollection.GetCommandByName(KnownCubaseMidiCommands.Show_All_Tracks);

            this.showHideAll = CubaseCommand.CreateMacroToggleButton("Show All", [hideShowAudio, hideShowGroups, hideShowInputs, hideShowInstruments, hideShowMidi, hideShowOutputs], [hideShowAudio, hideShowGroups, hideShowInputs, hideShowInstruments, hideShowMidi, hideShowOutputs])
                                .WithNameToggle("Hide All")
                                .WithCategory(CubaseServiceConstants.MidiService)
                                .WithToggleBackGroundColour(ColourConstants.ButtonToggledBackground)
                                .WithToggleForeColour(ColourConstants.ButtonToggledText);

            this.staticCommands = new List<CubaseCommand>
            {
                CubaseCommand.CreateToggleButton("Show Groups", hideShowGroups, "Hide Groups").WithCategory(CubaseServiceConstants.MidiService),
                CubaseCommand.CreateToggleButton("Show Audio", hideShowAudio, "Hide Audio").WithCategory(CubaseServiceConstants.MidiService),
                CubaseCommand.CreateToggleButton("Show Instruments", hideShowInstruments, "Hide Instruments").WithCategory(CubaseServiceConstants.MidiService),
                CubaseCommand.CreateToggleButton("Show Midi", hideShowMidi, "Hide Midi").WithCategory(CubaseServiceConstants.MidiService),
                CubaseCommand.CreateToggleButton("Show Inputs", hideShowInputs, "Hide Inputs").WithCategory(CubaseServiceConstants.MidiService),
                CubaseCommand.CreateToggleButton("Show Outputs", hideShowOutputs, "Hide Outputs").WithCategory(CubaseServiceConstants.MidiService),
                CubaseCommand.CreateMacroToggleButton("Show Selected", [this.commandCollection.GetCommandByName(KnownCubaseMidiCommands.Show_Selected_Tracks)],[this.commandCollection.GetCommandByName(KnownCubaseMidiCommands.Show_All_Tracks)]).WithNameToggle("Show All").WithCategory(CubaseServiceConstants.MidiService),
                showHideAll,
            };

            foreach (var command in this.staticCommands)
            {
                var btn = RaisedButtonFactory.Create(command.Name, ColourConstants.ButtonBackground.ToSerializableColour(), ColourConstants.ButtonText.ToSerializableColour(), async (s, e) =>
                {
                    if (command == this.showHideAll)
                    {
                        var mixerToggleStates = GetMixerStates(command);
                        command.IsToggled = !command.IsToggled;
                        var actionGroup = GetShowHideAllAction(mixerToggleStates);
                        await this.cubaseHttpClient.ExecuteCubaseAction(CubaseActionRequest.CreateFromCommand(command, actionGroup), async (err) =>
                        {
                            await DisplayAlert("Error", err.Message, "OK");
                        });
                        this.SetStaticButtonState(command);
                    }
                    else
                    { 
                        command.IsToggled = !command.IsToggled;


                    var result = await this.cubaseHttpClient.ExecuteCubaseAction(CubaseActionRequest.CreateFromCommand(command), async (err) =>
                    {
                        await DisplayAlert("Error", err.Message, "OK");
                    });
                    }
                    this.SetButtonState((Button)s, command);
                }, command.IsToggleButton);
                StaticButtons.Children.Add(btn.Button);
            }
            // await ShowAllChannels();
            var showHideBtn = this.StaticButtons.Children.OfType<Button>().First(x => x.Text == showHideAll.Name);
            SetButtonState(showHideBtn, showHideAll.WithFlipToggle());
            this.SetStaticButtonState(showHideAll);
        }

        private List<string> GetShowHideAllAction(List<CubaseCommand> mixerStates)
        {
            var actionGroup = showHideAll.IsToggled ? showHideAll.ActionGroupToggleOff : showHideAll.ActionGroup;
            return actionGroup.Where(ag => !mixerStates.Select(x => x.Action).Contains(ag)).ToList();
        }

        private List<CubaseCommand> GetMixerStates(CubaseCommand toggleCommand)
        {
            return staticCommands.Where(x => x.ButtonType == CubaseButtonType.Toggle)
                            .Where(x => x != showHideAll)
                            .Where(x => x.IsToggled != toggleCommand.IsToggled)
                            .ToList();
        }


        private void SetStaticButtonState(CubaseCommand toggleCommand)
        {
            var toggled = GetMixerStates(toggleCommand);    
            foreach (var resetToggle in toggled)
            {
                var targetTextName = resetToggle.IsToggled ? resetToggle.NameToggle : resetToggle.Name;
                var restBtn = this.StaticButtons.Children.OfType<Button>().FirstOrDefault(x => x.Text == targetTextName);
                if (restBtn != null)
                {
                    resetToggle.IsToggled = !resetToggle.IsToggled;
                    this.SetButtonState((Button)restBtn, resetToggle);
                }
            }
        }

        private async Task<bool> OpenCloseMixer()
        {
            var mixerCommand = this.commandCollection.GetCommandByName(KnownCubaseMidiCommands.Mixer); ;
            if (mixerCommand != null)
            {
                var result = await this.cubaseHttpClient.ExecuteCubaseAction(CubaseActionRequest.CreateSingleMidiCommand(mixerCommand), async (err) =>
                {
                    await DisplayAlert("Error", err.Message, "OK");
                });
                return result.Success;
            }
            return false;
        }

        public async Task Initialise()
        {
            this.basePage = this.serviceProvider.GetService<BasePage>();
            this.basePage.AddToolbars(this);
            var allTracks = await this.cubaseHttpClient.GetTracks();
            this.tracks = allTracks.GetActiveChannels();
            this.commandCollection = new CubaseMidiCommandCollection();

            if (await this.OpenCloseMixer())
            {
                await this.InitialiseStaticButtons();
                await this.LoadTracks();
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


}
