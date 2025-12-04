using Cubase.Midi.Sync.Common.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Server.Tests.Tests.ScriptParsers
{
    [TestClass]
    public class ScriptParseTests
    {
        [TestMethod]
        public async Task Can_Parse_Script()
        {
            var parser = new ScriptParser();
            var lines = new List<string>()
            {
                "DisableRecord()",
                "let tracks = GetTracks('Telecaster','Vocal  1')",
                "foreach ($track in tracks)", 
                "  EnableRecord($track)",
                "endforeach" 
            }; 
            var ast = parser.Parse(lines, err => Console.WriteLine(err.Message));
            var cubaseApi = new TestCubaseApi();
            var runner = new ScriptRunner(cubaseApi);
            await runner.ExecuteAsync(ast);
        }
    }
}
