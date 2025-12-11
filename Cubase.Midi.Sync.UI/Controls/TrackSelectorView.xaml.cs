using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Extensions;
using Cubase.Midi.Sync.Common.Midi;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.SysEx;
using Cubase.Midi.Sync.Common.WebSocket;
using Cubase.Midi.Sync.UI.CubaseService.WebSocket;
using Cubase.Midi.Sync.UI.Models;
using Cubase.Midi.Sync.UI.Settings;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics.Text;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using static System.Net.Mime.MediaTypeNames;

namespace Cubase.Midi.Sync.UI.Controls;

public partial class TrackSelectorView : ContentView
{

    private bool Expanded = true;

    private IMidiWebSocketResponse midiWebSocketResponse;
    private IMidiWebSocketClient midiWebSocketClient;
    private AppSettings appSettings;

    private readonly object _mergeLock = new();
    private readonly Dictionary<int, TrackModel> _pending = new();
    private bool _updateScheduled = false;

     private readonly Dictionary<int, (Border border, Grid container, Button btn, BoxView sel, BoxView rec)> _uiMap
    = new Dictionary<int, (Border, Grid, Button, BoxView, BoxView)>();

    public Action<bool>? OnExpanded { get; set; } = null;
    
    public TrackSelectorView()
    {
        this.InitializeComponent();
        MaxMinButton.Clicked += MaxMinButton_Clicked;
    }

 
    public string GetSelectedTracksAsString() =>
       string.Join(';', this.GetSelectedTracksAsList());

    public IEnumerable<string> GetSelectedTracksAsList() =>
        Tracks?.Where(t => t.IsSelected).Select(x => x.Name) ?? Enumerable.Empty<string>();

    public IEnumerable<TrackModel> GetSelectedTracks() =>
        Tracks?.Where(t => t.IsSelected) ?? Enumerable.Empty<TrackModel>();

    public static readonly BindableProperty TracksProperty =
        BindableProperty.Create(
            nameof(Tracks),
            typeof(ObservableCollection<TrackModel>),
            typeof(TrackSelectorView),
            propertyChanged: OnTracksChanged);

    public ObservableCollection<TrackModel> Tracks
    {
        get => (ObservableCollection<TrackModel>)GetValue(TracksProperty);
        set => SetValue(TracksProperty, value);
    }

    public async Task Initialise(IMidiWebSocketResponse midiWebSocketResponse, IMidiWebSocketClient midiWebSocketClient, AppSettings appSettings, Action<bool>? onExpanded)
    {
        SetExpandedState();
        this.OnExpanded = onExpanded;
        this.midiWebSocketClient = midiWebSocketClient;
        this.midiWebSocketResponse = midiWebSocketResponse; 
        this.Tracks = new ObservableCollection<TrackModel>();
        midiWebSocketResponse.RegisterdTrackHandler(this.TracksUpdatedHandler);
        await this.UpdateTracks();
        this.appSettings = appSettings;
        this.BuildButtonUi();
    }


    private Button MakeControlButton(Color backgroundColour, Color textColour, string text)
    {
        return new Button
        {
            Text = text,
            BackgroundColor = backgroundColour,
            TextColor = textColour,
            CornerRadius = 4,
            Padding = new Thickness(6),
            Margin = new Thickness(6),
            FontSize = 13,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            LineBreakMode = LineBreakMode.WordWrap,
            ClassId = "false",
            MinimumWidthRequest = 120
        };
    } 

    private Button MakeToggleTrackButton(Color backgroundColour, 
                                         Color textColour, 
                                         string text, 
                                         Color toggleBackGroundColor,
                                         Color toggleTextColour,
                                         string toggleText,
                                         Action<object?, EventArgs> ClickHandler, 
                                         Action<object?, EventArgs> ClickHandlerToggled)
    {
        var btn = MakeControlButton(backgroundColour, textColour, text);
        btn.Clicked += async (s, e) =>
        {
            var btn = (Button)s;
            if (btn.ClassId == "false")
            {
                ClickHandler(s, e);
                btn.ClassId = "true";
                btn.BackgroundColor = toggleBackGroundColor;
                btn.TextColor = toggleTextColour;
                btn.Text = toggleText;
            }
            else
            {
                ClickHandlerToggled(s, e);
                btn.ClassId = "false";
                btn.BackgroundColor = backgroundColour;
                btn.TextColor = textColour;
                btn.Text = text;
            }
        };
        return btn;
    }

    private Button MakeTrackButton(Color backgroundColour, Color textColour, string text, Action<object?, EventArgs> ClickHandler)
    {
        var btn = MakeControlButton(backgroundColour, textColour, text);    
        btn.Clicked += async (s, e) => ClickHandler(s, e);  
        return btn;
    }

    private void BuildButtonUi()
    {
        TrackButtons.Children.Clear();

        var standardBackgroundColour = Color.FromArgb("3B3C3F");
        var standardTextColour = Color.FromArgb("D7D7D8");

        var selectRecordEnablebtn = MakeToggleTrackButton(Colors.Red,
                                                    Colors.White,
                                                    "Select And Record Enable",
                                                    standardBackgroundColour,
                                                    standardTextColour,
                                                    "Show all",
                                                    async (s, e) =>
        {
            var socketMessage = WebSocketMessage.Create(WebSocketCommand.ExecuteCubaseAction,
                                     CubaseActionRequest
                                        .Create(
                                            ActionEvent.Create()
                                                       .WithAction(this.GetSelectedTracksAsString())
                                                       .WithSubCommand(SysExCommand.SelectTracks.ToString())
                                                       .WithCommandType(CubaseAreaTypes.SysEx)));
            var response = await this.midiWebSocketClient.SendMidiCommand(socketMessage);
            if (response.Command == WebSocketCommand.Success)
            {
                socketMessage = WebSocketMessage.Create(WebSocketCommand.ExecuteCubaseAction,
                                                 CubaseActionRequest
                                                    .Create(
                                                        ActionEvent.Create()
                                                                   .WithAction(this.GetSelectedTracksAsString())
                                                                   .WithSubCommand(SysExCommand.EnableRecord.ToString())
                                                                   .WithCommandType(CubaseAreaTypes.SysEx)));
                response = await this.midiWebSocketClient.SendMidiCommand(socketMessage);
            }
        }, async(s, e) => // toggled
        {
            var socketMessage = WebSocketMessage.Create(WebSocketCommand.ExecuteCubaseAction,
                                     CubaseActionRequest
                                        .Create(
                                            ActionEvent.Create()
                                                       .WithAction(this.GetSelectedTracksAsString())
                                                       .WithSubCommand(SysExCommand.DisableRecord.ToString())
                                                       .WithCommandType(CubaseAreaTypes.SysEx)));
            var response = await this.midiWebSocketClient.SendMidiCommand(socketMessage);
            socketMessage = WebSocketMessage.Create(WebSocketCommand.ExecuteCubaseAction,
                                                 CubaseActionRequest
                                                    .Create(
                                                        ActionEvent.Create(CubaseAreaTypes.Midi, "ShowAll")));

            response = await this.midiWebSocketClient.SendMidiCommand(socketMessage);
        });


        var recordEnablebtn = MakeToggleTrackButton(Colors.Red, 
                                                   Colors.White, 
                                                   "Record Enable",
                                                    standardBackgroundColour,
                                                    standardTextColour,
                                                    "Disable Record",
                                                   async (s, e) => 
        {
            var socketMessage = WebSocketMessage.Create(WebSocketCommand.ExecuteCubaseAction,
                                                 CubaseActionRequest
                                                    .Create(
                                                        ActionEvent.Create()
                                                                   .WithAction(this.GetSelectedTracksAsString())
                                                                   .WithSubCommand(SysExCommand.DisableAndEnable.ToString())
                                                                   .WithCommandType(CubaseAreaTypes.SysEx)));
            var response = await this.midiWebSocketClient.SendMidiCommand(socketMessage);
        }, async(s, e) => // toggled
        {
            var socketMessage = WebSocketMessage.Create(WebSocketCommand.ExecuteCubaseAction,
                                                 CubaseActionRequest
                                                    .Create(
                                                        ActionEvent.Create()
                                                                   .WithAction(this.GetSelectedTracksAsString())
                                                                   .WithSubCommand(SysExCommand.DisableRecord.ToString())
                                                                   .WithCommandType(CubaseAreaTypes.SysEx)));
            var response = await this.midiWebSocketClient.SendMidiCommand(socketMessage);
        });



        var selectTracksbtn = MakeTrackButton(Color.FromArgb("3B3C3F"), Color.FromArgb("D7D7D8"), "Select Tracks", async (s, e) => 
        { 
            var socketMessage = WebSocketMessage.Create(WebSocketCommand.ExecuteCubaseAction,
                                                 CubaseActionRequest
                                                    .Create(
                                                        ActionEvent.Create()
                                                                   .WithAction(this.GetSelectedTracksAsString())
                                                                   .WithSubCommand(SysExCommand.SelectTracks.ToString())
                                                                   .WithCommandType(CubaseAreaTypes.SysEx)));
            var response = await this.midiWebSocketClient.SendMidiCommand(socketMessage);
        });

        var mutebtn = MakeToggleTrackButton(Colors.Yellow,
                                                   Colors.Black,
                                                   "Mute",
                                                    standardBackgroundColour,
                                                    standardTextColour,
                                                    "Mute Off",
                                                   async (s, e) =>
        {
            var socketMessage = WebSocketMessage.Create(WebSocketCommand.ExecuteCubaseAction,
                                                 CubaseActionRequest
                                                    .Create(
                                                        ActionEvent.Create()
                                                                   .WithAction(this.GetSelectedTracksAsString())
                                                                   .WithSubCommand(SysExCommand.EnableMute.ToString())
                                                                   .WithCommandType(CubaseAreaTypes.SysEx)));
            var response = await this.midiWebSocketClient.SendMidiCommand(socketMessage);
        }, async(s, e) => // toggled
        {
            var socketMessage = WebSocketMessage.Create(WebSocketCommand.ExecuteCubaseAction,
                                                 CubaseActionRequest
                                                    .Create(
                                                        ActionEvent.Create()
                                                                   .WithAction(this.GetSelectedTracksAsString())
                                                                   .WithSubCommand(SysExCommand.DisableMute.ToString())
                                                                   .WithCommandType(CubaseAreaTypes.SysEx)));
            var response = await this.midiWebSocketClient.SendMidiCommand(socketMessage);
        });

        var solobtn = MakeToggleTrackButton(Colors.Orange,
                                                   Colors.Black,
                                                   "Solo",
                                                    standardBackgroundColour,
                                                    standardTextColour,
                                                    "Solo Off",
                                                   async (s, e) =>
        {
            var socketMessage = WebSocketMessage.Create(WebSocketCommand.ExecuteCubaseAction,
                                                 CubaseActionRequest
                                                    .Create(
                                                        ActionEvent.Create()
                                                                   .WithAction(this.GetSelectedTracksAsString())
                                                                   .WithSubCommand(SysExCommand.EnableSolo.ToString())
                                                                   .WithCommandType(CubaseAreaTypes.SysEx)));
            var response = await this.midiWebSocketClient.SendMidiCommand(socketMessage);
        }, async(s, e) => // toggled
        {
            var socketMessage = WebSocketMessage.Create(WebSocketCommand.ExecuteCubaseAction,
                                                 CubaseActionRequest
                                                    .Create(
                                                        ActionEvent.Create()
                                                                   .WithAction(this.GetSelectedTracksAsString())
                                                                   .WithSubCommand(SysExCommand.DisableSolo.ToString())
                                                                   .WithCommandType(CubaseAreaTypes.SysEx)));
            var response = await this.midiWebSocketClient.SendMidiCommand(socketMessage);
        });

        var listenbtn = MakeToggleTrackButton(Colors.Red,
                                                   Colors.Black,
                                                   "Listen",
                                                    standardBackgroundColour,
                                                    standardTextColour,
                                                    "Listen Off",
                                                   async (s, e) =>
        {
            var socketMessage = WebSocketMessage.Create(WebSocketCommand.ExecuteCubaseAction,
                                                 CubaseActionRequest
                                                    .Create(
                                                        ActionEvent.Create()
                                                                   .WithAction(this.GetSelectedTracksAsString())
                                                                   .WithSubCommand(SysExCommand.EnableListen.ToString())
                                                                   .WithCommandType(CubaseAreaTypes.SysEx)));
            var response = await this.midiWebSocketClient.SendMidiCommand(socketMessage);
        }, async(s, e) => // toggled
        {
            var socketMessage = WebSocketMessage.Create(WebSocketCommand.ExecuteCubaseAction,
                                                 CubaseActionRequest
                                                    .Create(
                                                        ActionEvent.Create()
                                                                   .WithAction(this.GetSelectedTracksAsString())
                                                                   .WithSubCommand(SysExCommand.DisableListen.ToString())
                                                                   .WithCommandType(CubaseAreaTypes.SysEx)));
            var response = await this.midiWebSocketClient.SendMidiCommand(socketMessage);
        });

        var showallbtn = MakeTrackButton(Color.FromArgb("3B3C3F"), Color.FromArgb("D7D7D8"), "Show All", async (s, e) => 
        { 
            var socketMessage = WebSocketMessage.Create(WebSocketCommand.ExecuteCubaseAction,
                                                 CubaseActionRequest
                                                    .Create(
                                                        ActionEvent.Create(CubaseAreaTypes.Midi, "ShowAll")));

            var response = await this.midiWebSocketClient.SendMidiCommand(socketMessage);
            this.ResetTrackSelections();
            await this.GetTracks();
        });

        var resetbtn = MakeTrackButton(Color.FromArgb("3B3C3F"), Color.FromArgb("D7D7D8"), "Reset", (s, e) =>
        {
            this.ResetTrackSelections();
        });


        var closeBtn = MakeTrackButton(Colors.Red, Color.FromArgb("D7D7D8"), "Close", (s, e) =>
        {
            this.SetExpandedState();
        });

        TrackButtons.Children.Add(selectRecordEnablebtn);
        TrackButtons.Children.Add(selectTracksbtn);
        TrackButtons.Children.Add(recordEnablebtn);
        TrackButtons.Children.Add(mutebtn);
        TrackButtons.Children.Add(solobtn);
        TrackButtons.Children.Add(listenbtn);
        TrackButtons.Children.Add(showallbtn);
        TrackButtons.Children.Add(resetbtn);
        TrackButtons.Children.Add(closeBtn);
    }
    
    
    private void BuildUI()
    {
        TrackLayout.Children.Clear();
        _uiMap.Clear();

        if (Tracks == null) return;

        foreach (var track in Tracks)
        {
            var (border, container, btn, sel, rec) = CreateTrackUI(track);

            TrackLayout.Children.Add(border);

            // store by index so updates can find controls quickly
            _uiMap[track.Index] = (border, container, btn, sel, rec);
        }
    }

    

    private (Border border, Grid container, Button btn, BoxView sel, BoxView rec) CreateTrackUI(TrackModel track)
    {
        var container = new Grid
        {
            WidthRequest = 100,
            HeightRequest = 100,
            Padding = 4,
            RowSpacing = 4,
            BackgroundColor = Color.FromArgb("3B3C3F"),
            RowDefinitions =
            {
               new RowDefinition { Height = GridLength.Star },
               new RowDefinition { Height = GridLength.Auto }
            }
        };

        var border = new Border
        {
            Stroke = track.BorderColour,
            StrokeThickness = 5,
            Padding = 1,            // <— add padding to create space for the stroke
            BackgroundColor = track.BorderColour,
            StrokeShape = new RoundRectangle { CornerRadius = 2 },
            Content = container,
            Margin = new Thickness(2)
        };

        var btn = new Button
        {
            Text = track.Name,
            BackgroundColor = track.IsSelected ? Color.FromArgb("bdc3c9") : Color.FromArgb("3B3C3F"),
            TextColor = track.IsSelected ? Colors.Black : Color.FromArgb("D7D7D8"),
            CornerRadius = 6,
            Padding = 6,
            FontSize = 13,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            LineBreakMode = LineBreakMode.WordWrap   
        };

        btn.Clicked += (s, e) => TrackClicked((Button)s,track);

        container.Add(btn, 0, 0);

        var indicators = new Grid
        {
            ColumnDefinitions =
        {
            new ColumnDefinition { Width = GridLength.Auto },
            new ColumnDefinition { Width = GridLength.Auto }
        },
            ColumnSpacing = 6,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
        };

        var selectedIndicator = new BoxView
        {
            WidthRequest = 12,
            HeightRequest = 12,
            BackgroundColor = track.TrackSelectedEnabled ? Colors.LimeGreen : Colors.Transparent
        };
        indicators.Add(selectedIndicator, 0, 0);

        var recordIndicator = new BoxView
        {
            WidthRequest = 12,
            HeightRequest = 12,
            BackgroundColor = track.TrackRecordEnabled ? Colors.Red : Colors.Transparent
        };
        indicators.Add(recordIndicator, 1, 0);

        container.Add(indicators, 0, 1);

        return (border, container, btn, selectedIndicator, recordIndicator);
    }


    private void ResetTrackSelections()
    {
        foreach (var track in Tracks)
        {
            if (track.IsSelected)
            {
                var btnValue = _uiMap.FirstOrDefault(x => x.Value.btn.Text == track.Name);
                if (btnValue.Value.btn != null)
                {
                    TrackClicked(btnValue.Value.btn, track);
                }
            }
        }
    }

    private void TrackClicked(Button btn, TrackModel track)
    {
        track.IsSelected = !track.IsSelected;
        btn.TextColor = track.IsSelected ? Colors.Black : Color.FromArgb("D7D7D8");
        btn.FontAttributes = track.IsSelected ? FontAttributes.Bold : FontAttributes.None;
        btn.BackgroundColor = track.IsSelected ? Color.FromArgb("bdc3c9") : Color.FromArgb("3B3C3F");
    }

    private void ApplyUpdatesToUI(List<TrackModel> updates)
    {
        if (updates == null || updates.Count == 0) return;

        foreach (var tm in updates)
        {
            if (!_uiMap.TryGetValue(tm.Index, out var ui))
            {
                // UI not built yet for this index (e.g., name not known or BuildUI not called).
                continue;
            }

            // Update indicators quickly
            ui.sel.BackgroundColor = tm.TrackSelectedEnabled ? Colors.LimeGreen : Colors.Transparent;
            ui.rec.BackgroundColor = tm.TrackRecordEnabled ? Colors.Red : Colors.Transparent;

            // Update button appearance if needed
            ui.btn.FontAttributes =
                (tm.TrackSelectedEnabled || tm.TrackRecordEnabled) ? FontAttributes.Bold : FontAttributes.None;

            // Update text if name changed
            if (!string.Equals(ui.btn.Text, tm.Name, StringComparison.Ordinal))
                ui.btn.Text = tm.Name;
        }
    }


    private void ProcessMergedUpdates()
    {
        List<TrackModel> batch;

        lock (_mergeLock)
        {
            batch = _pending.Values.ToList(); // this now WORKS
            _pending.Clear();
            _updateScheduled = false;
        }

        ApplyUpdatesToUI(batch);
    }


    private async Task UpdateTracks()
    {
        if (midiWebSocketResponse.Tracks == null)
            return;

        // 1. Build a new list on BACKGROUND THREAD (safe)
        var newTrackModels = await Task.Run(() =>
        {
            return midiWebSocketResponse.Tracks
                .OrderBy(c => c.Index)
                .Where(c => !string.IsNullOrEmpty(c.Name))
                .Select(TrackModel.FromMidiChannel)
                .ToList();
        });

        this.Tracks.Clear();

        // 2. Apply to UI on UI thread
        MainThread.BeginInvokeOnMainThread(() =>
        {

            foreach (var t in newTrackModels)
                this.Tracks.Add(t);

            BuildUI();
        });
    }
    
    private async Task GetTracks()
    {
        var socketMessage = WebSocketMessage.Create(WebSocketCommand.Tracks);
        var response = await this.midiWebSocketClient.SendMidiCommand(socketMessage);
    }

    private async Task UpdateUI(List<TrackModel> updates)
    {
        var latestUpdates = updates
              .GroupBy(tm => tm.Name, StringComparer.OrdinalIgnoreCase)
              .Select(g => g.Last()) // last update wins
              .ToList();

        var newTracks = latestUpdates
            .Where(tm => !Tracks.Any(t => t.Name.Equals(tm.Name, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        if (newTracks.Any())
        {
            await UpdateTracks(); // safely on main thread
        }
        else
        {
            foreach (var tm in updates)
            {
                foreach (var ctrl in TrackLayout.Children)
                {
                    if (ctrl is not Grid grid) continue;
                    if (grid.Children.FirstOrDefault() is not Button btn) continue;
                    if (!btn.Text.Equals(tm.Name, StringComparison.OrdinalIgnoreCase)) continue;

                    if (grid.Children.LastOrDefault() is not Grid indicators) continue;
                    if (indicators.Children.FirstOrDefault() is not BoxView selectedIndicator) continue;
                    if (indicators.Children.LastOrDefault() is not BoxView recordEnableIndicator) continue;

                    selectedIndicator.BackgroundColor = tm.TrackSelectedEnabled ? Colors.LimeGreen : Colors.Transparent;
                    recordEnableIndicator.BackgroundColor = tm.TrackRecordEnabled ? Colors.Red : Colors.Transparent;
                }
            }
        }

    }

    private Task ApplyTrackDelta(MidiChannel midiChannel)
    {
        var update = TrackModel.FromMidiChannel(midiChannel);

        lock (_mergeLock)
        {
            // merge the latest update for this track
            _pending[update.Index] = update;

            if (!_updateScheduled)
            {
                _updateScheduled = true;
                MainThread.BeginInvokeOnMainThread(ProcessMergedUpdates);
            }
        }

        return Task.CompletedTask;
    }

    private async Task TracksUpdatedHandler(MidiChannel midiChannel)
    {
        await ApplyTrackDelta(midiChannel);
    }

    private static void OnTracksChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (TrackSelectorView)bindable;
        control.BuildUI();
    }

    private void MaxMinButton_Clicked(object? sender, EventArgs e)
    {
        SetExpandedState();
    }

    private void SetExpandedState()
    {
        if (Expanded)
        {
            MaxMinButton.Text = "Select Tracks - Open";
            TrackLayout.IsVisible = false;
            TrackButtons.IsVisible = false;
            FilterCollection.IsVisible = false; 
            Expanded = false;
        }
        else
        {
            Task.Run(async () => await this.GetTracks());
            MaxMinButton.Text = "Select Tracks - Close";
            TrackLayout.IsVisible = true;
            TrackButtons.IsVisible = true;
            FilterCollection.IsVisible = true;  
            Expanded = true;
        }
        if (this.OnExpanded != null)
        {
            this.OnExpanded(Expanded);
        }
    }

    private void OnFilterChanged(object sender, CheckedChangedEventArgs e)
    {
        ApplyFilters();
    }


    private void ApplyFilters()
    {
        bool audio = AudioFilter.IsChecked;
        bool midi = MidiFilter.IsChecked;
        bool group = GroupFilter.IsChecked;

        bool anyFilter = audio || midi || group;

        foreach (var track in Tracks)
        {

            var ui = _uiMap.FirstOrDefault(x => x.Value.btn.Text == track.Name).Value;
            if (ui.btn == null)
                continue;

            bool show = false;

            // audio filter
            if (audio && track.ChannelType == MidiChannelType.Audio)
                show = true;

            // midi filter
            if (midi && (track.ChannelType == MidiChannelType.MidiChannel ||
                         track.ChannelType == MidiChannelType.Synth))
                show = true;

            // group filter
            if (group && track.ChannelType == MidiChannelType.GroupChannel)
                show = true;

            // If NO filters are checked → show everything
            if (!anyFilter)
                show = true;
            
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ui.container.IsVisible = show;
                ui.rec.IsVisible = show;    
                ui.sel.IsVisible = show;
                ui.border.IsVisible = show;
                ui.btn.IsVisible = show;
            });
        }
    }

}