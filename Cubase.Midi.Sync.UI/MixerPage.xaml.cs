using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Colours;
using Cubase.Midi.Sync.Common.Extensions;
using Cubase.Midi.Sync.Common.Midi;
using Cubase.Midi.Sync.Common.Mixer;
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
                    await DisplayAlert("Error", ex.Message, "OK");
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
                            await DisplayAlert("Error", ex.Message, "OK");
                        }
                    });
                    Pages.Children.Add(button.Button);
                }
            }
        }

        private async Task InitialiseMixer()
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
                    this.mixerCollection = await this.cubaseHttpClient.SetMixer(CubaseMixer.Create(Enum.Parse<KnownCubaseMidiCommands>(command.Action), string.Empty, string.Empty));
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
            return await this.cubaseHttpClient.SetMixer(CubaseMixer.Create(KnownCubaseMidiCommands.Mixer, string.Empty, string.Empty)); 
        }

        public async Task Initialise(List<CubaseCommandCollection>? mainCommands = null)
        {
            this.basePage = this.serviceProvider.GetService<BasePage>();
            this.basePage.AddToolbars(this);

            this.mixerCollection = await this.cubaseHttpClient.GetMixer();
            await this.OpenCloseMixer();
            await this.InitialiseMixer();
            if (mainCommands != null)
            {
                await this.InitialisePages(mainCommands);
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
