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

        public async Task LoadTracks()
        {
            TrackButtons.Children.Clear();
            var trackCommands = new List<MidiMixerCommand>();
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

            var hideAudio = this.commandCollection.GetCommandByName(KnownCubaseMidiCommands.Hide_Audio);
            var hideGroups = this.commandCollection.GetCommandByName(KnownCubaseMidiCommands.Hide_Groups);
            var hideInputs = this.commandCollection.GetCommandByName(KnownCubaseMidiCommands.Hide_Inputs);
            var hideInstruments = this.commandCollection.GetCommandByName(KnownCubaseMidiCommands.Hide_Instruments);
            var hideMidi = this.commandCollection.GetCommandByName(KnownCubaseMidiCommands.Hide_Midi);
            var hideOutputs = this.commandCollection.GetCommandByName(KnownCubaseMidiCommands.Hide_Outputs);
            var mixerShowAll = this.commandCollection.GetCommandByName(KnownCubaseMidiCommands.Mixer_Show_All);

            var staticCommands = new List<MidiMixerCommand>
            {
                MidiMixerCommand.Create("Show Groups", CubaseActionRequest.CreateMidiActionGroup([mixerShowAll,hideAudio, hideInputs, hideInstruments, hideMidi, hideOutputs])),
                MidiMixerCommand.Create("Show Audio", CubaseActionRequest.CreateMidiActionGroup([mixerShowAll,hideGroups, hideInputs, hideInstruments, hideMidi, hideOutputs])),
                MidiMixerCommand.Create("Show Inputs", CubaseActionRequest.CreateMidiActionGroup([mixerShowAll,hideAudio, hideGroups, hideInstruments, hideMidi, hideOutputs])),
                MidiMixerCommand.Create("Show Instruments", CubaseActionRequest.CreateMidiActionGroup([mixerShowAll,hideAudio, hideGroups, hideInputs, hideMidi, hideOutputs])),
                MidiMixerCommand.Create("Show Midi", CubaseActionRequest.CreateMidiActionGroup([mixerShowAll,hideAudio, hideGroups, hideInputs, hideInstruments, hideOutputs])),
                MidiMixerCommand.Create("Show Outputs", CubaseActionRequest.CreateMidiActionGroup([mixerShowAll,hideAudio, hideGroups, hideInputs, hideInstruments, hideMidi])),
                MidiMixerCommand.Create("Show All", CubaseActionRequest.CreateSingleMidiCommand(mixerShowAll))
            };

            foreach (var command in staticCommands)
            {
                var btn = RaisedButtonFactory.Create(command.Name, ColourConstants.ButtonBackground.ToSerializableColour(), ColourConstants.ButtonText.ToSerializableColour(), async (s, e) => 
                {
                    var result = await this.cubaseHttpClient.ExecuteCubaseAction(command.Request, async (err) =>
                    {
                        await DisplayAlert("Error", err.Message, "OK");
                    });
                });
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
    }

    public class MidiMixerCommand
    {
        public CubaseActionRequest Request { get; set; }    

        public string Name { get; set; }

        public static MidiMixerCommand Create(string name, CubaseActionRequest cubaseActionRequest)
        {
            return new MidiMixerCommand() { Name = name, Request = cubaseActionRequest  };
        }
    }
}
