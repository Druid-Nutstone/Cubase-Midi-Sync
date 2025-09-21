using Cubase.Midi.Sync.Common.Mixer;
using Cubase.Midi.Sync.Server.Services.Mixer;
using Microsoft.AspNetCore.Mvc;

namespace Cubase.Midi.Sync.Server.Controllers
{
    [Route("api/[controller]")]
    public class MixerController : Controller
    {
        
        private readonly IMixerService mixerService;

        private readonly ILogger<MixerController> logger;   

        public MixerController(IMixerService mixerService, ILogger<MixerController> logger)
        {
           this.mixerService = mixerService;    
           this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SetCubaseMixer([FromBody] CubaseMixer cubaseMixer)
        {
            this.logger.LogInformation($"Setting Mixer command {cubaseMixer.Command.ToString()} {cubaseMixer.ButtonText} from controller");
            var result = await mixerService.MixerCommand(cubaseMixer);
            if (result.Count > 0)
            { 
               return Ok(result);
            }
            else
            {
                this.logger.LogError($"The mixer command return 0 results");
                return BadRequest("The mixer command returned no results!");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCubaseMixer()
        {
            this.logger.LogInformation("Loading Mixer from controller");
            var result = await mixerService.GetMixer();
            return Ok(result);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
