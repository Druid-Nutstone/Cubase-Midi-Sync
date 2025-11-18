using Cubase.Midi.Sync.Common.Colours;
using Cubase.Midi.Sync.UI.CubaseService.NutstoneClient;
using Cubase.Midi.Sync.UI.Extensions;
using Cubase.Midi.Sync.UI.NutstoneServices.NutstoneClient;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Cubase.Midi.Sync.UI.Settings;
using System.Text.Json;
using Cubase.Midi.Sync.Common.Extensions;

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
        this.ClearSettings.Clicked += ClearSettings_Clicked;
        this.IncreaseButtonHeight.Clicked += IncreaseButtonHeight_Clicked;
        this.IncreaseButtonWidth.Clicked += IncreaseButtonWidth_Clicked;
        this.DescreaseButtonHeight.Clicked += DescreaseButtonHeight_Clicked;
        this.DescreaseButtonWidth.Clicked += DescreaseButtonWidth_Clicked;
        this.SetSizeText();
        this.BuildExampleButton();
        Title = "Options";
        this.Initialise();
    }

    private void DescreaseButtonWidth_Clicked(object? sender, EventArgs e)
    {
        this.appSettings.ButtonSizes.Width += 0.1;
        this.SetSizeText();
    }

    private void DescreaseButtonHeight_Clicked(object? sender, EventArgs e)
    {
        this.appSettings.ButtonSizes.Height += 0.1;
        this.SetSizeText();
    }

    private void IncreaseButtonWidth_Clicked(object? sender, EventArgs e)
    {
        this.appSettings.ButtonSizes.Width -= 0.1;
        this.SetSizeText();
    }

    private void IncreaseButtonHeight_Clicked(object? sender, EventArgs e)
    {
        this.appSettings.ButtonSizes.Height -= 0.1;
        this.SetSizeText();
    }

    private void BuildExampleButton()
    {
        if (this.OptionCollection.Children.Last().GetType() == typeof(Button))
        {
            this.OptionCollection.Children.Remove(this.OptionCollection.Children.Last());
        }
        var button = RaisedButtonFactory.Create("Example Button", System.Drawing.Color.Green.ToSerializableColour(), System.Drawing.Color.Black.ToSerializableColour(), async (s, e) => { }, this.appSettings);
        this.OptionCollection.Children.Add(button.Button);
    }

    private void SetSizeText()
    {
        this.ButtonSize.Text = $"Button Size - Height: {this.appSettings.ButtonSizes.Height} Width: {this.appSettings.ButtonSizes.Width}";
        this.BuildExampleButton();
    }

    private async void ClearSettings_Clicked(object? sender, EventArgs e)
    {
        File.Delete(GetSettingsPath());
        var mainPage = this.services.GetRequiredService<CubaseMainPage>();
        await this.Navigation.PushAsync(mainPage);
        await mainPage.Reload();
    }

    private async void SaveButton_Clicked(object? sender, EventArgs e)
    {
         var asString = JsonSerializer.Serialize(this.appSettings, new JsonSerializerOptions() { WriteIndented = true });
        File.WriteAllText(this.GetSettingsPath(), asString);
        var mainPage = this.services.GetRequiredService<CubaseMainPage>();
        await DisplayAlert("Warning", "You need to restart this application for the changes to take effect", "OK");
        await this.Navigation.PushAsync(mainPage);
        await mainPage.Reload();
    }

    private string GetSettingsPath()
    {
#if DEBUG
        var environment = "development";
#else
             var environment = "production";
#endif
        string fileName = $"appsettings.{environment}.json";
        return Path.Combine(FileSystem.AppDataDirectory, fileName);
    
    }

    private void TargetIPAddress_TextChanged(object? sender, TextChangedEventArgs e)
    {
        this.GetActiveConnection().Host = this.TargetIPAddress.Text;
        this.FullHost.Text = this.GetActiveConnection().BaseUrl;
    }

    private void Initialise()
    {
        this.TargetIPAddress.Text = this.GetActiveConnection().Host;
        this.InitialiseComponents();
    }

    private CubaseConnection GetActiveConnection()
    {
        return this.appSettings.CubaseConnection.First(x => x.Name.Equals(this.appSettings.ActiveConnection, StringComparison.OrdinalIgnoreCase));
    }

    private void InitialiseComponents()
    {
        ActiveConnections.ItemsSource = this.appSettings.CubaseConnection.Select(x => x.Name).ToList();
        ActiveConnections.SelectedIndexChanged += ActiveConnections_SelectedIndexChanged; ;       
    }

    private void ActiveConnections_SelectedIndexChanged(object? sender, EventArgs e)
    {
        this.appSettings.ActiveConnection = ActiveConnections.SelectedItem.ToString() ?? string.Empty;
        this.TargetIPAddress.Text = this.GetActiveConnection().Host;
    }
}