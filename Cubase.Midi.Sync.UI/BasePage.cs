namespace Cubase.Midi.Sync.UI;

public class BasePage
{
    private ContentPage contentPage;
   
    private MixerPage mixerPage;    

    public BasePage(MixerPage mixerPage)
	{
        this.mixerPage = mixerPage; 
    }

    public void AddToolbars(ContentPage contentPage)
    {
        this.contentPage = contentPage;

        contentPage.ToolbarItems.Clear();
        
        contentPage.ToolbarItems.Add(new ToolbarItem
        {
            Text = "Mixer",
            IconImageSource = "settings.png",
            Order = ToolbarItemOrder.Primary,
            Priority = 0, // leftmost
            Command = new Command(OnMixerClicked)
        });
        contentPage.ToolbarItems.Add(new ToolbarItem
        {
            Text = "Home",
            Order = ToolbarItemOrder.Primary, // must be Primary
            Priority = 1, // appears after Mixer
            Command = new Command(OnHomeClicked)
        });
    }

    protected async virtual void OnMixerClicked()
    {
        await this.contentPage.Navigation.PushAsync(this.mixerPage);
        await this.mixerPage.Initialise(); 
    }

    protected virtual void OnHomeClicked()
    {
        this.contentPage.Navigation.PopToRootAsync();
    }


}