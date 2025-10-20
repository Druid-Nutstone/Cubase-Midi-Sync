using Cubase.Midi.Sync.UI.CubaseService.WebSocket;

namespace Cubase.Midi.Sync.UI
{
    public partial class App : Application
    {
        private readonly IServiceProvider serviceProvider;
        
        private readonly IMidiWebSocketClient midiWebSocketClient;  

        public App(IServiceProvider serviceProvider, IMidiWebSocketClient midiWebSocketClient)
        {
            this.serviceProvider = serviceProvider;
            this.midiWebSocketClient = midiWebSocketClient;
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var mainPage = serviceProvider.GetRequiredService<CubaseMainPage>();

            // Wrap the page in a NavigationPage
            var navPage = new NavigationPage(mainPage);

            // Return a Window that uses the NavigationPage
            var win =  new Window(navPage);
            win.Destroying += (s, e) =>
            {
                this.midiWebSocketClient.Close();
            };  
            return win;
        }

    }
}