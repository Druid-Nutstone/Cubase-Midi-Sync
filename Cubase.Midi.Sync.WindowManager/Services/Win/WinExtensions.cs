using Cubase.Midi.Sync.WindowManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.WindowManager.Services.Win
{
    public static class WinExtensions
    {
        public static IEnumerable<(nint, string)> GetWindows(this IEnumerable<(nint, string)> windowCollection, IEnumerable<string> windowNames)
        {
            return windowCollection
                .Where(w => windowNames.Any(name => w.Item2.StartsWith(name, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
