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
                throw new FileNotFoundException($"{filePath} Cubase 15 key commands XML file not found.");
            }
            var knownCommands = new CubaseKnownCollection();
            var doc = XDocument.Load(filePath);
            var list = new CubaseKeyCommandCollection();

            foreach (var element in doc.Root.Elements())
            {
                if (element.Attribute("name")?.Value == "Categories")
                {
                    ProcessCatgeories(element, list, knownCommands);
                }
            }
            return list;
        }

        private void ProcessCatgeories(XElement catElement, CubaseKeyCommandCollection commands, CubaseKnownCollection knownCommands)
        {
            foreach (var element in catElement.Elements())
            {
                if (element.Name.LocalName.ToLower() == "item")
                {
                    ProcessCatItem(element, commands, knownCommands);
                }
            }
        }

        private void ProcessCatItem(XElement catItemElement, CubaseKeyCommandCollection commands, CubaseKnownCollection knownCommands)
        {
            var categoryName = "";
            foreach (var element in catItemElement.Elements())
            {
                if (element.Name.LocalName.ToLower() == "string")
                {
                    // category name
                    categoryName = element.Attribute("value")?.Value ?? "Unknown";
                }
                if (element.Name.LocalName.ToLower() == "list")
                {
                    // commands list
                    foreach (var cmdItem in element.Elements("item"))
                    {
                        ProcessCommandItem(cmdItem, categoryName, commands, knownCommands);
                    }
                }
            }
        }

        private void ProcessCommandItem(XElement cmdItemElement, string categoryName, CubaseKeyCommandCollection commands, CubaseKnownCollection knownCommands)
        {
            string name = "";
            string key = "";
            string action = "";

            foreach (var element in cmdItemElement.Elements("string"))
            {
                var elName = element.Attribute("name")?.Value?.ToLowerInvariant();
                switch (elName)
                {
                    case "name":
                        name = element.Attribute("value")?.Value ?? "";
                        break;
                    case "key":
                        key = element.Attribute("value")?.Value ?? "";
                        break;
                }
            }

            foreach (var list in cmdItemElement.Elements("list"))
            {
                var keys = list.Descendants("item")
                    .Select(i => i.Attribute("value")?.Value)
                    .Where(v => !string.IsNullOrEmpty(v))
                    .ToList();
                if (keys.Any())
                    key = string.Join(", ", keys);
            }

            commands.Add(new CubaseKeyCommand()
            {
                Category = categoryName,
                Name = name,
                Key = key,
                Action = ActionEvent.Create(GetAreaName(categoryName), key),
                CubaseCommand = knownCommands.GetCommandByName(name)
            });

            CubaseAreaTypes GetAreaName(string categoryName)
            {
                if (!Enum.TryParse<CubaseAreaTypes>(categoryName, out var areaType))
                {
                    return CubaseAreaTypes.Keys;
                }
                return areaType;
            }
        }

    }
}

