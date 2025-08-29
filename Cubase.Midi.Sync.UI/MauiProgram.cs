using Cubase.Midi.Sync.UI.CubaseService.NutstoneClient;
using Cubase.Midi.Sync.UI.NutstoneServices.NutstoneClient;
using Cubase.Midi.Sync.UI.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using System.Reflection;



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
            AppSettings appSettings;
            using (var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json").Result)
            {
                var config = new ConfigurationBuilder()
                    .AddJsonStream(stream)
                    .Build();

                appSettings = new AppSettings();
                config.Bind(appSettings); // This now works because of the using above
            }

            builder.Services.AddSingleton<ICubaseHttpClient, CubaseHttpClient>();
            // builder.Services.AddSingleton<ICubaseHttpClient, TestCubaseHttpClient>();
            builder.Services.AddSingleton(appSettings)
                            .AddTransient<CubaseMainPage>();



            return builder.Build();
        }
    }
}
