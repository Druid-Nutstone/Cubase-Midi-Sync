using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Colours;
using Cubase.Midi.Sync.Common.Extensions;
using Cubase.Midi.Sync.Common.InternalCommands;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.Responses;
using Cubase.Midi.Sync.Common.WebSocket;
using Cubase.Midi.Sync.UI.CubaseService.WebSocket;
using Cubase.Midi.Sync.UI.Extensions;
using Cubase.Midi.Sync.UI.NutstoneServices.NutstoneClient;
using Cubase.Midi.Sync.UI.Settings;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.UI;

public partial class CubaseAction : ContentPage
{
    private readonly CubaseCommandCollection commands;
    private readonly ICubaseHttpClient client;
    private readonly IMidiWebSocketClient webSocketClient;
    private readonly List<CubaseCommandCollection> commandsCollection;
    private readonly IMidiWebSocketResponse webSocketResponse;
    private readonly AppSettings appSettings;

    private Dictionary<InternalCommandType, Action<InternalCommand>> internalCommands;
    
    private BasePage basePage;

    private FlexLayout maximizedLayout = null;
    private VisualElement maximizedBanner = null;

    public CubaseAction(CubaseCommandCollection commands, 
                        List<CubaseCommandCollection> commandsCollection, 
                        ICubaseHttpClient client, 
                        IMidiWebSocketClient webSocketClient,
                        IMidiWebSocketResponse webSocketResponse,
                        AppSettings appSettings,
                        BasePage basePage)
    {
        this.basePage = basePage;
        basePage.AddToolbars(this);  
        InitializeComponent();
        this.commands = commands;
        this.client = client;
        this.webSocketResponse = webSocketResponse;
        this.webSocketResponse.RegisterForSystemMessages(this.OnSystemError);
        this.commandsCollection = commandsCollection;   
        this.webSocketClient = webSocketClient;
        this.appSettings = appSettings;
        this.internalCommands = new Dictionary<InternalCommandType, Action<InternalCommand>>()
        {
            { InternalCommandType.Navigate, this.ProcessInternalNavigate }
        };
        BackgroundColor = ColourConstants.WindowBackground.ToMauiColor();
        Title = commands.Name;
        this.webSocketResponse.RegisterForErrors(this.OnError);

        // start async initialization without blocking ctor
        _ = InitializeAsync();

        LoadCommand();
    }

    private async Task InitializeAsync()
    {
        try
        {
            await LoadPreCommandsAsync().ConfigureAwait(false);
            var initTask = trackSelector.Initialise(this.webSocketResponse, this.webSocketClient, this.appSettings, this.OnTracksExpanded);
            if (initTask != null)
                await initTask.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(() => DisplayAlert("Initialization error", ex.Message, "OK")).ConfigureAwait(false);
        }
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();

        try
        {
            foreach (var pcmd in this.commands.PostCommands)
            {
                var cubaseSocketRequest = CubaseActionRequest.Create(pcmd.Action);
                var socketMessage = WebSocketMessage.Create(WebSocketCommand.ExecuteCubaseAction, cubaseSocketRequest);
                await this.webSocketClient.SendMidiCommand(socketMessage).ConfigureAwait(false);
            }
        }
        catch
        {
            // swallow to avoid exceptions on disappear
        }
    }
    
    private void OnTracksExpanded(bool expanded)
    {
        foreach (var child in CommandsContainer.Children)
        {
            if (child is VisualElement ve)
            {
                ve.IsVisible = !expanded;
            }
        }
        _ = this.Scroller.ScrollToAsync(0, 0, animated: true);
    }

    private async Task LoadPreCommandsAsync()
    {
        foreach (var pcmd in this.commands.PreCommands)
        {
            try
            {
                var cubaseSocketRequest = CubaseActionRequest.Create(pcmd.Action);
                var socketMessage = WebSocketMessage.Create(WebSocketCommand.ExecuteCubaseAction, cubaseSocketRequest);
                await this.webSocketClient.SendMidiCommand(socketMessage).ConfigureAwait(false);
            }
            catch
            {
                // ignore individual failures
            }
        }
    }

    private async Task OnError(string message)
    {
        await DisplayAlert("Error CubaseAction", message, "OK");
    }

    private async Task OnSystemError(WebSocketCommand command)
    {
        if (command == WebSocketCommand.CubaseNotReady || command == WebSocketCommand.ServerClosed)
        {
            await Navigation.PopToRootAsync();
        }
    }

    private void LoadCommand()
    {
        CommandsContainer.Children.Clear();

        // 🟡 Layout for uncategorized commands (no banner)
        var noCategoryLayout = CreateFlexLayout();
        CommandsContainer.Children.Add(noCategoryLayout);

        string currentCategory = string.Empty;
        FlexLayout currentCategoryLayout = null;

        foreach (var command in commands.GetCommandsByOrderedCategory())
        {
            // CASE 1: Command has no category
            if (string.IsNullOrEmpty(command.ButtonCategory))
            {
                var button = CreateCommandButton(command);
                noCategoryLayout.Children.Add(button);
                continue;
            }

            // CASE 2: New category detected
            if (!command.ButtonCategory.Equals(currentCategory, StringComparison.OrdinalIgnoreCase))
            {
                currentCategory = command.ButtonCategory;
                currentCategoryLayout = CreateFlexLayout();

                // 🏷️ Create the banner + toggle button
                var banner = CreateBanner(currentCategory, currentCategoryLayout);

                CommandsContainer.Children.Add(banner);
                CommandsContainer.Children.Add(currentCategoryLayout);
            }

            // ➕ Add button to the current category layout
            var categoryButton = CreateCommandButton(command);
            currentCategoryLayout?.Children.Add(categoryButton);
        }
    }

    private FlexLayout CreateFlexLayout()
    {
        return new FlexLayout
        {
            Direction = FlexDirection.Row,
            Wrap = FlexWrap.Wrap,
            JustifyContent = FlexJustify.Start,
            AlignItems = FlexAlignItems.Start,
            AlignContent = FlexAlignContent.Start,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0)
        };
    }

    private View CreateBanner(string categoryName, FlexLayout categoryLayout)
    {
        var label = new Label
        {
            Text = categoryName,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.StartAndExpand,
            VerticalTextAlignment = TextAlignment.Center,
            TextColor = Colors.Black,
            FontSize = 16
        };

        var toggleButton = new Button
        {
            Text = "Max",
            FontSize = 14,
            Padding = new Thickness(0),
            Margin = new Thickness(0),
            FontAttributes = FontAttributes.Bold,
            WidthRequest = 50,
            HeightRequest = 40,
            BackgroundColor = Colors.Transparent,
            TextColor = Colors.Black,
            BorderWidth = 0,
            CornerRadius = 0,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center
        };
        
        toggleButton.Clicked += (s, e) =>
        {
            var clickedBanner = ((Button)s).Parent?.Parent as VisualElement;

            // 🟡 Case 1 — No layout currently maximized
            if (maximizedLayout == null)
            {
                Maximize(clickedBanner, categoryLayout, toggleButton);
                return;
            }

            // 🟡 Case 2 — Same banner clicked again → restore
            if (maximizedLayout == categoryLayout)
            {
                RestoreAll();
                return;
            }

            // 🟡 Case 3 — Another banner clicked → switch
            RestoreAll();
            Maximize(clickedBanner, categoryLayout, toggleButton);
        };

        var bannerGrid = new Grid
        {
            ColumnDefinitions =
        {
            new ColumnDefinition { Width = GridLength.Star },
            new ColumnDefinition { Width = GridLength.Auto }
        },
            Padding = new Thickness(10, 5)
        };
        bannerGrid.Add(label, 0, 0);
        bannerGrid.Add(toggleButton, 1, 0);

        return new Border
        {
            BackgroundColor = ColourConstants.CategoryColour.ToMauiColor(),
            StrokeThickness = 0,
            Margin = new Thickness(0, 10, 0, 5),
            HorizontalOptions = LayoutOptions.Fill,
            HeightRequest = 40,
            Content = bannerGrid
        };
    }

    private void Maximize(VisualElement banner, FlexLayout layout, Button toggleButton)
    {
        maximizedLayout = layout;
        maximizedBanner = banner;

        foreach (var child in CommandsContainer.Children)
        {
            if (child is VisualElement ve)
            {
                if (child != maximizedLayout && child != maximizedBanner)
                    ve.IsVisible = false;
            }
        }

        layout.VerticalOptions = LayoutOptions.FillAndExpand;
        toggleButton.Text = "Min"; // Minimize icon
    }

    private void RestoreAll()
    {
        maximizedLayout = null;
        maximizedBanner = null;

        foreach (var child in CommandsContainer.Children)
        {
            if (child is VisualElement ve)
                ve.IsVisible = true;
        }

        // Reset all toggle button icons
        foreach (var child in CommandsContainer.Children)
        {
            if (child is Border border && border.Content is Grid grid)
            {
                foreach (var element in grid.Children)
                {
                    if (element is Button btn)
                        btn.Text = "Max";
                }
            }
        }
    }


    private Button CreateCommandButton(CubaseCommand command)
    {
        var buttonWrapper = RaisedButtonFactory.Create(
            command.Name,
            command.ButtonBackgroundColour,
            command.ButtonTextColour,
            async (s, e) =>
            {
                try
                {
                    var btn = (Button)s;
                    command.IsToggled = !command.IsToggled;
                    SetButtonState(btn, command);

                    // call async work directly
                    if (command.IsMacro)
                        await SetMacroButton(btn, command).ConfigureAwait(false);
                    else
                        await SetMomentaryOrToggleButton(btn, command).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await MainThread.InvokeOnMainThreadAsync(() => DisplayAlert("Error CubaseAction.cs LoadCommand", ex.Message, "OK")).ConfigureAwait(false);
                }
            }, this.appSettings,
            toggleMode: true
        );

        SetButtonState(buttonWrapper.Button, command);
        return buttonWrapper.Button;
    }


    private async Task SetMomentaryOrToggleButton(Button button, CubaseCommand command)
    {
        string errMsg = null;
        await MainThread.InvokeOnMainThreadAsync(() => VisualStateManager.GoToState(button, "Pressed")).ConfigureAwait(false);
        var cubaseSocketRequest = CubaseActionRequest.CreateFromCommand(command);
        if (command.ButtonType == CubaseButtonType.SysEx)
        {
            cubaseSocketRequest.Action
                               .Clone()
                               .WithSubCommand(command.Action.Action)
                               .WithAction(trackSelector.GetSelectedTracksAsString());
        }
        var socketMessage = WebSocketMessage.Create(WebSocketCommand.ExecuteCubaseAction, cubaseSocketRequest);
        var response = await this.webSocketClient.SendMidiCommand(socketMessage).ConfigureAwait(false);
        if (response.Command != WebSocketCommand.Success)
        {
            await MainThread.InvokeOnMainThreadAsync(() => DisplayAlert("Error SetMomentaryOrToggleButton", errMsg ?? "Is cubase up?", "OK")).ConfigureAwait(false);
            command.IsToggled = !command.IsToggled;
        }
        await MainThread.InvokeOnMainThreadAsync(() => VisualStateManager.GoToState(button, "Normal")).ConfigureAwait(false);
        SetButtonState(button, command);
    }

    private async Task SetMacroButton(Button button, CubaseCommand command)
    {
        try
        {
            var actionGroup = command.ActionGroup?.Clone();
            if (command.ButtonType == CubaseButtonType.MacroToggle)
            {
                actionGroup = command.IsToggled ? command.ActionGroup?.Clone() : command.ActionGroupToggleOff?.Clone();
                foreach (var act in actionGroup)
                {
                    if (act.CommandType == CubaseAreaTypes.SysEx)
                    {
                            act.WithSubCommand(act.Action)
                               .WithAction(trackSelector.GetSelectedTracksAsString());
                    }
                }
            }
            await MainThread.InvokeOnMainThreadAsync(() => VisualStateManager.GoToState(button, "Pressed")).ConfigureAwait(false);
            var cubaseSocketRequest = CubaseActionRequest.CreateFromCommand(command, actionGroup);
            var socketMessage = WebSocketMessage.Create(WebSocketCommand.ExecuteCubaseAction, cubaseSocketRequest);
            var response = await this.webSocketClient.SendMidiCommand(socketMessage).ConfigureAwait(false);
            if (response.Command == WebSocketCommand.Success)
            {
                var duplicateCommands = this.commandsCollection
                                .SelectMany(x => x.Commands)
                                .Where(x => x.Name.Equals(command.Name))
                                .Where(x => x != command);
                foreach (var dupItem in duplicateCommands)
                {
                    dupItem.IsToggled = command.IsToggled;
                }
            }
            else
            {
                await MainThread.InvokeOnMainThreadAsync(() => DisplayAlert("Error Executing Command", response.Message, "OK")).ConfigureAwait(false);
                command.IsToggled = !command.IsToggled;
                SetButtonState(button, command);
            }
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(() => DisplayAlert("Error CubaseAction SetMacroButton", ex.Message, "OK")).ConfigureAwait(false);
        }
        finally
        {
            await MainThread.InvokeOnMainThreadAsync(() => VisualStateManager.GoToState(button, "Normal")).ConfigureAwait(false);
            SetButtonState(button, command);
        }
    }

    private void ProcessInternalNavigate(InternalCommand command)
    {
        var target = this.commandsCollection.FirstOrDefault(x => x.Name.Equals(command.Parameter, StringComparison.OrdinalIgnoreCase));
        if (target != null)
        {
            Navigation.PushAsync(new CubaseAction(target, this.commandsCollection, this.client, this.webSocketClient, this.webSocketResponse, this.appSettings, this.basePage));
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
