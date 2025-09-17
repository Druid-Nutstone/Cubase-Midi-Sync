using Cubase.Midi.Sync.Common.Mixer;
using Cubase.Midi.Sync.Server.Services.Mixer;
using Microsoft.AspNetCore.Mvc;

namespace Cubase.Midi.Sync.Server.Controllers
{
    [Route("api/[controller]")]
    public class MixerController : Controller
    {
        
        private readonly IMixerService mixerService;

        public MixerController(IMixerService mixerService)
        {
           this.mixerService = mixerService;    
        }

        [HttpPost]
        public async Task<IActionResult> SetCubaseMixer([FromBody] CubaseMixer cubaseMixer)
        {
            var result = await mixerService.MixerCommand(cubaseMixer);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetCubaseMixer()
        {
            var result = await mixerService.GetMixer();
            return Ok(result);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
