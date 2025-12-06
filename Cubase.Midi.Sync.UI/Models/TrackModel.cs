using Cubase.Midi.Sync.Common.Midi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.UI.Models
{
    public class TrackModel : INotifyPropertyChanged
    {
        public int Index { get; set; }
        public string Name { get; set; } = "";
        public bool TrackRecordEnabled { get; set; }
        public bool TrackSelectedEnabled { get; set; }
        public bool IsSelected { get; set; }

        public static TrackModel FromMidiChannel(MidiChannel midi)
        {
            return new TrackModel
            {
                Index = midi.Index,
                Name = midi.Name,
                TrackRecordEnabled = midi.RecordEnable.Value,
                TrackSelectedEnabled = midi.Selected.Value
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }



}
