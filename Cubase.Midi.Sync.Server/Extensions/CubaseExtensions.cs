using Cubase.Midi.Sync.Server.Constants;
using System.Diagnostics;

namespace Cubase.Midi.Sync.Server.Extensions
{
    public static class CubaseExtensions
    {
        public static Process GetCubaseService()
        {
            return Process.GetProcessesByName(CubaseServerConstants.CubaseExeName)
                          .FirstOrDefault();
        }
    }
}
