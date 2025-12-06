using Cubase.Midi.Sync.Common.Midi;
using Cubase.Midi.Sync.UI.CubaseService.WebSocket;
using Cubase.Midi.Sync.UI.Models;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace Cubase.Midi.Sync.UI.Controls;

public partial class TrackSelectorView : ContentView
{

    private bool Expanded = true;

    private IMidiWebSocketResponse midiWebSocketResponse;


    private readonly object _mergeLock = new();
    private readonly Dictionary<int, TrackModel> _pending = new();
    private bool _updateScheduled = false;

     private readonly Dictionary<int, (Grid container, Button btn, BoxView sel, BoxView rec)> _uiMap
    = new Dictionary<int, (Grid, Button, BoxView, BoxView)>();

    public TrackSelectorView()
    {
        this.InitializeComponent();
        MaxMinButton.Clicked += MaxMinButton_Clicked;
    }

    private void MaxMinButton_Clicked(object? sender, EventArgs e)
    {
        if (Expanded)
        {
            MaxMinButton.Text = "Select Tracks - Open";
            TrackLayout.IsVisible = false;
            Expanded = false;
        }
        else
        {
            MaxMinButton.Text = "Select Tracks - Close";
            TrackLayout.IsVisible = true;
            Expanded = true;
        }
    }

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

    public async Task Initialise(IMidiWebSocketResponse midiWebSocketResponse)
    {
        this.midiWebSocketResponse = midiWebSocketResponse; 
        this.Tracks = new ObservableCollection<TrackModel>();
        midiWebSocketResponse.RegisterdTrackHandler(this.TracksUpdatedHandler);
        await this.UpdateTracks();
    }

    private void BuildUI()
    {
        TrackLayout.Children.Clear();
        _uiMap.Clear();

        if (Tracks == null) return;

        foreach (var track in Tracks)
        {
            var (container, btn, sel, rec) = CreateTrackUI(track);

            TrackLayout.Children.Add(container);

            // store by index so updates can find controls quickly
            _uiMap[track.Index] = (container, btn, sel, rec);
        }
    }

    private (Grid container, Button btn, BoxView sel, BoxView rec) CreateTrackUI(TrackModel track)
    {
        // --- Container (2 rows: button + indicators) ---
        var container = new Grid
        {
            WidthRequest = 120,
            HeightRequest = 120,
            Padding = 4,
            RowDefinitions =
        {
            new RowDefinition { Height = GridLength.Star },
            new RowDefinition { Height = new GridLength(20) }
        }
        };

        // --- Button (Row 0) ---
        var btn = new Button
        {
            Text = track.Name,
            BackgroundColor = track.IsSelected ? Colors.AntiqueWhite : Colors.LightGray,
            TextColor = track.IsSelected ? Colors.White : Colors.Black,
            CornerRadius = 4,
            Padding = new Thickness(6),
            FontSize = 13,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            LineBreakMode = LineBreakMode.WordWrap,
        };

        btn.Clicked += (s, e) =>
        {
            track.IsSelected = !track.IsSelected;
            btn.BackgroundColor = track.IsSelected ? Colors.AntiqueWhite : Colors.LightGray;
        };

        container.Add(btn);
        Grid.SetRow(btn, 0);

        // --- Indicators (Row 1) ---
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

        // selected indicator
        var selectedIndicator = new BoxView
        {
            WidthRequest = 12,
            HeightRequest = 12,
            BackgroundColor = track.TrackSelectedEnabled ? Colors.LimeGreen : Colors.Transparent
        };
        indicators.Add(selectedIndicator);
        Grid.SetColumn(selectedIndicator, 0);

        // record-enable indicator
        var recordIndicator = new BoxView
        {
            WidthRequest = 12,
            HeightRequest = 12,
            BackgroundColor = track.TrackRecordEnabled ? Colors.Red : Colors.Transparent
        };
        indicators.Add(recordIndicator);
        Grid.SetColumn(recordIndicator, 1);

        container.Add(indicators);
        Grid.SetRow(indicators, 1);

        // Bold text if either flag is true
        btn.FontAttributes =
            (track.TrackSelectedEnabled || track.TrackRecordEnabled) ? FontAttributes.Bold : FontAttributes.None;

        return (container, btn, selectedIndicator, recordIndicator);
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




    public IEnumerable<TrackModel> GetSelectedTracks() =>
        Tracks?.Where(t => t.IsSelected) ?? Enumerable.Empty<TrackModel>();
}