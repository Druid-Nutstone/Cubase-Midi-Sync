using Cubase.Midi.Sync.Common.Colours;
using Cubase.Midi.Sync.UI.CubaseService.NutstoneClient;
using Cubase.Midi.Sync.UI.Extensions;
using Cubase.Midi.Sync.UI.NutstoneServices.NutstoneClient;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Cubase.Midi.Sync.UI.Settings;
using System.Text.Json;

namespace Cubase.Midi.Sync.UI;

public partial class CubaseOptions : ContentPage
{
    private readonly AppSettings appSettings;
    
    private readonly ICubaseHttpClient client;

    private readonly IServiceProvider services;

    public CubaseOptions(AppSettings appSettings, ICubaseHttpClient cubaseHttpClient, IServiceProvider services)
	{
		InitializeComponent();
        this.appSettings = appSettings; 
        this.services = services;    
        this.client = cubaseHttpClient; 
        BackgroundColor = ColourConstants.WindowBackground.ToMauiColor();
        this.TargetIPAddress.TextChanged += TargetIPAddress_TextChanged;
        this.SaveButton.Clicked += SaveButton_Clicked;
        Title = "Options";
        this.Initialise();
    }

    private async void SaveButton_Clicked(object? sender, EventArgs e)
    {
#if DEBUG
        var environment = "development";
#else
             var environment = "production";
#endif

        string fileName = $"appsettings.{environment}.json";
        string writablePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
        var asString = JsonSerializer.Serialize(this.appSettings, new JsonSerializerOptions() { WriteIndented = true });
        File.WriteAllText(writablePath, asString);
        var mainPage = this.services.GetRequiredService<CubaseMainPage>();
        await DisplayAlert("Warning", "You need to restart this application for the changes to take effect", "OK");
        await this.Navigation.PushAsync(mainPage);
        await mainPage.Reload();
    }

    private void TargetIPAddress_TextChanged(object? sender, TextChangedEventArgs e)
    {
        this.appSettings.CubaseConnection.Host = this.TargetIPAddress.Text;
        this.FullHost.Text = this.appSettings.CubaseConnection.BaseUrl;
    }

    private void Initialise()
    {

        this.TargetIPAddress.Text = this.appSettings.CubaseConnection.Host;
    }
}