using Cubase.Midi.Sync.WindowManager.Services.Cubase;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.WindowManager.Registration
{
    public static class RegisterCubaseWindowsRegistration
    {
        public static IServiceCollection RegisterCubaseWindows(this IServiceCollection serviceProvider)
        {
            return serviceProvider.AddTransient<ICubaseWindowsService, CubaseWindowsService>();
        }
    }
}
