using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Midi;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Server.Services.Commands;
using Cubase.Midi.Sync.Server.Services.Cubase;
using Microsoft.AspNetCore.Mvc;

namespace Cubase.Midi.Sync.Server.Controllers
{
    [Route("api/[controller]")]
    public class CubaseController : Controller
    {
        private readonly ICommandService commandService;
        private readonly ICubaseService cubaseService;

        public CubaseController(ICommandService commandService, ICubaseService cubaseService)
        {
           this.commandService = commandService;    
           this.cubaseService = cubaseService;
        }

        [HttpGet]
        [Route("test")]
        public async Task<IActionResult> TestConnection()
        {
            return Ok();
        }


        [HttpGet]
        [Route("commands")]
        public async Task<IActionResult> GetCommands()
        {
            var commands = await commandService.GetCommands();
            return Ok(commands);
        }

        [HttpGet]
        [Route("tracks")]
        public async Task<IActionResult> GetTracks()
        {
            var tracks = await cubaseService.GetTracks();
            return Ok(tracks);
        }

        [HttpPost]
        [Route("tracks/selected")]
        public async Task<IActionResult> SetSelectedTracks([FromBody] List<MidiChannel> midiChannels)
        {
            var tracks = await cubaseService.SetSelectedTracks(midiChannels);
            return Ok(tracks);
        }

        [HttpPost]
        [Route("execute")]
        public async Task<IActionResult> ExecureCubaseCommand([FromBody] CubaseActionRequest cubaseActionRequest)
        {
            var response = await cubaseService.ExecuteAction(cubaseActionRequest);
            return Ok(response);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
