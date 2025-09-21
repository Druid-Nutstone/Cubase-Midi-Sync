using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Cubase.Midi.Sync.Common.Keys
{

    public class CubaseKeyCommandParser
    {

        public CubaseKeyCommandCollection Parse(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"{filePath} Cubase 14 key commands XML file not found.");
            }
            var knownCommands = new CubaseKnownCollection();
            var doc = XDocument.Load(filePath);
            var list = new CubaseKeyCommandCollection();

            foreach (var categoryItem in doc.Descendants("list"))
            {
                if (categoryItem.Attribute("name")?.Value != "Categories")
                    continue;

                foreach (var cat in categoryItem.Elements("item"))
                {
                    string categoryName = cat.Element("string")?.Attribute("value")?.Value ?? "Unknown";

                    var commandsList = cat.Element("list");
                    if (commandsList == null) continue;

                    foreach (var cmdItem in commandsList.Elements("item"))
                    {
                        string name = cmdItem.Element("string")?.Attribute("value")?.Value ?? "";
                        string key = "";
                        string action = "";

                        foreach (var s in cmdItem.Elements("string"))
                        {
                            var attr = s.Attribute("name")?.Value;
                            if (attr == "Name") name = s.Attribute("value")?.Value ?? "";
                            if (attr == "Key") key = s.Attribute("value")?.Value ?? "";
                            if (attr == "Action") action = s.Attribute("value")?.Value ?? "";
                        }

                        list.Add(new CubaseKeyCommand
                        {
                            Category = categoryName,
                            Name = name,
                            Key = key,
                            Action = action,
                            CubaseCommand = knownCommands.GetCommandByName(name)
                        });
                    }
                }
            }

            return list;
        }
    }
}
