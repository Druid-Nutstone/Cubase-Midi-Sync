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

        public async Task LoadTracks()
        {
            TrackButtons.Children.Clear();
            // To do implement this - The backend is implemented and the http client    
            foreach (var track in this.tracks)
            {
                // trackCommands.Add(MidiMixerCommand.Create(track.Name,null));
                var btn = RaisedButtonFactory.Create(track.Name, ColourConstants.ButtonBackground.ToSerializableColour(), ColourConstants.ButtonText.ToSerializableColour(), async (s, e) =>
                {
                    //var result = await this.cubaseHttpClient.ExecuteCubaseAction(command.Request, async (err) =>
                    //{
                    //    await DisplayAlert("Error", err.Message, "OK");
                    //});
                }, true);
                TrackButtons.Children.Add(btn.Button);
            }
        }

        public async Task InitialiseStaticButtons()
        {
            StaticButtons.Children.Clear();


            var hideShowAudio = this.commandCollection.GetCommandByName(KnownCubaseMidiCommands.Hide_Audio);
            var hideShowGroups = this.commandCollection.GetCommandByName(KnownCubaseMidiCommands.Hide_Groups);
            var hideShowInputs = this.commandCollection.GetCommandByName(KnownCubaseMidiCommands.Hide_Inputs);
            var hideShowInstruments = this.commandCollection.GetCommandByName(KnownCubaseMidiCommands.Hide_Instruments);
            var hideShowMidi = this.commandCollection.GetCommandByName(KnownCubaseMidiCommands.Hide_Midi);
            var hideShowOutputs = this.commandCollection.GetCommandByName(KnownCubaseMidiCommands.Hide_Outputs);
            var mixerShowAll = this.commandCollection.GetCommandByName(KnownCubaseMidiCommands.Mixer_Show_All);

            var staticCommands = new List<CubaseCommand>
            {
                CubaseCommand.CreateToggleButton("Show Groups", hideShowGroups, "Hide Groups").WithCategory(CubaseServiceConstants.MidiService),
                CubaseCommand.CreateToggleButton("Show Audio", hideShowAudio, "Hide Audio").WithCategory(CubaseServiceConstants.MidiService),
                CubaseCommand.CreateToggleButton("Show Instruments", hideShowInstruments, "Hide Instruments").WithCategory(CubaseServiceConstants.MidiService),
                CubaseCommand.CreateToggleButton("Show Midi", hideShowMidi, "Hide Midi").WithCategory(CubaseServiceConstants.MidiService),
                CubaseCommand.CreateToggleButton("Show Inputs", hideShowInputs, "Hide Inputs").WithCategory(CubaseServiceConstants.MidiService),
                CubaseCommand.CreateToggleButton("Show Outputs", hideShowOutputs, "Hide Outputs").WithCategory(CubaseServiceConstants.MidiService),
                CubaseCommand.CreateStandardButton("Show All", mixerShowAll).WithCategory(CubaseServiceConstants.MidiService),
            };

            // start by showing everything
            await cubaseHttpClient.ExecuteCubaseAction(CubaseActionRequest.CreateFromCommand(staticCommands.Last()), async (err) =>
            {
                await DisplayAlert("Error", err.Message, "OK");
            });

            // then hide everything
            await cubaseHttpClient.ExecuteCubaseAction(CubaseActionRequest.CreateMidiActionGroup([hideShowAudio, hideShowGroups, hideShowInputs, hideShowInstruments, hideShowMidi, hideShowOutputs]), async (err) =>
            {
                await DisplayAlert("Error", err.Message, "OK");
            });

            foreach (var command in staticCommands)
            {
                var btn = RaisedButtonFactory.Create(command.Name, ColourConstants.ButtonBackground.ToSerializableColour(), ColourConstants.ButtonText.ToSerializableColour(), async (s, e) =>
                {
                    command.IsToggled = !command.IsToggled;
                    if (command.Name == "Show All")
                    {
                        var toggled = staticCommands.Where(x => x.IsToggleButton && !x.IsToggled).ToList();
                        foreach (var resetToggle in toggled)
                        {
                            await cubaseHttpClient.ExecuteCubaseAction(CubaseActionRequest.CreateFromCommand(resetToggle), async (err) =>
                            {
                                await DisplayAlert("Error", err.Message, "OK");
                            });
                            resetToggle.IsToggled = !resetToggle.IsToggled;
                            var restBtn = this.StaticButtons.Children.OfType<Button>().First(x => x.Text == resetToggle.Name);
                            this.SetButtonState((Button)restBtn, resetToggle);
                        }
                    }
                    var result = await this.cubaseHttpClient.ExecuteCubaseAction(CubaseActionRequest.CreateFromCommand(command), async (err) =>
                    {
                        await DisplayAlert("Error", err.Message, "OK");
                    });
                    this.SetButtonState((Button)s, command);
                }, command.IsToggleButton);
                StaticButtons.Children.Add(btn.Button);
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
