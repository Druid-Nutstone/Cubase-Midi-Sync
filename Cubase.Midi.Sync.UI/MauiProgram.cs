using Cubase.Midi.Sync.UI.CubaseService.NutstoneClient;
using Cubase.Midi.Sync.UI.NutstoneServices.NutstoneClient;
using Microsoft.Extensions.Logging;

namespace Cubase.Midi.Sync.UI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            // builder.Services.AddSingleton<ICubaseHttpClient, CubaseHttpClient>();
            builder.Services.AddSingleton<ICubaseHttpClient, TestCubaseHttpClient>();
            builder.Services.AddTransient<CubaseMainPage>();    
            return builder.Build();
        }
    }
}
