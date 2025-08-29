using Cubase.Midi.Sync.Command.Builder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;



public class CubaseKeyCommandParser
{
    public string filePath { get; set; } = string.Empty;    

    public static CubaseKeyCommandParser Create()
    {
        // C:\Users\david\AppData\Roaming\Steinberg\Cubase 14_64
        var keyLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Steinberg", "Cubase 14_64", "Key Commands.xml");
        return new CubaseKeyCommandParser() { filePath = keyLocation };   
    } 
    
    public CubaseKeyCommandCollection Parse()
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Cubase 14 key commands XML file not found.", filePath);

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
                        Action = action
                    });
                }
            }
        }

        return list;
    }
}
