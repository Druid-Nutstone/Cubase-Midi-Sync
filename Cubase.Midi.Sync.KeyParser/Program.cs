// See https://aka.ms/new-console-template for more information
using Cubase.Midi.Sync.Common;
using HtmlAgilityPack;

var htmlFile = "C:\\Users\\david\\Documents\\Steinberg\\Cubase\\MIDI Remote\\Driver Scripts\\README_v1.html";

var doc = new HtmlDocument();
doc.Load(htmlFile);

var liNodes = doc.DocumentNode.SelectNodes("//div[@id='table_of_commands']//li[not(ul)]");
var list = new List<string>();
char quote = '"';
foreach (var liNode in liNodes)
{
    var id = liNode.GetAttributes().FirstOrDefault(x => x.Name == "id");
    var snippet = liNode.GetAttributes().FirstOrDefault(x => x.Name == "snippet");
    var command = GetCommandFromIdAndSnippet(id.Value, snippet.Value);
    var commandEntry = $"            this.Add(new CubaseKnownCommand({quote}{command.CommandBinding}{quote}, {quote}{command.CommandDescription}{quote}, {quote}{command.CommandName}{quote}));";
    list.Add(commandEntry);
}

File.WriteAllLines("C:\\deleteme\\cubaseknowncommands.txt", list);

CubaseKnownCommand GetCommandFromIdAndSnippet(string id, string snippet)
{
    var idBits = id.Split('/', StringSplitOptions.RemoveEmptyEntries);
    var snippetBits = snippet.Split(",");

    var commandBinding = idBits[0].Trim().Replace("&amp;", "&");
    var commandDescription = idBits[1].Replace("$amp;", "&").Trim();
    if (idBits.Length > 2)
    {
        commandDescription = idBits[1].Trim();
        for (int i = 2; i < idBits.Length; i++)
        {
            commandDescription += $"/{idBits[i]}".Trim();
        }
        commandDescription = commandDescription.Replace("&amp;", "&").Trim();
    }
    // var commandDescription = string.Join(' ', idBits.Skip(1)).Replace("&amp;", "&").Trim();
    var commandName = snippetBits[1].Replace("'", "").Trim().Replace("&amp", "&");

    return new CubaseKnownCommand(commandBinding, commandDescription, commandName);
} 




