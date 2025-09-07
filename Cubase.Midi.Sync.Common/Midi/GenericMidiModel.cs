using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Cubase.Midi.Sync.Common.Midi
{
    public class GenericMidiModel : List<MidiCtrl>
    {
        public GenericMidiModel() { }

        public static GenericMidiModel LoadFromFile()
        {
            XDocument doc = XDocument.Load(CubaseServiceConstants.GenericMidiFilePath);

            var controls = doc.Descendants("ctrl")
                .Select(x => new MidiCtrl
                {
                    Name = (string)x.Element("name"),
                    Status = (int)x.Element("stat"),
                    Channel = (int)x.Element("chan"),
                    Addr = (int)x.Element("addr"),
                    Max = (int)x.Element("max"),
                    Flags = (int)x.Element("flags")
                })
                .ToList();
            var midiModel = new GenericMidiModel();
            midiModel.AddRange(controls);
            return midiModel;
        }
    }

    public class MidiCtrl
    {
        public string Name { get; set; }
        public int Status { get; set; }
        public int Channel { get; set; }
        public int Addr { get; set; }
        public int Max { get; set; }
        public int Flags { get; set; }
    }
}
