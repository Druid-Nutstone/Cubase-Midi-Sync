namespace Cubase.Midi.Sync.UI
{
    public partial class App : Application
    {
        private readonly IServiceProvider serviceProvider;
        public App(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var mainPage = serviceProvider.GetRequiredService<CubaseMainPage>();

            // Wrap the page in a NavigationPage
            var navPage = new NavigationPage(mainPage);

            // Return a Window that uses the NavigationPage
            return new Window(navPage);
        }
    }
}