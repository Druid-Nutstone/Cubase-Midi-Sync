using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Keys;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.Responses;
using Cubase.Midi.Sync.Server.Constants;
using Cubase.Midi.Sync.Server.Services.Keyboard;
using static Cubase.Midi.Sync.Server.Services.Keyboard.KeyboardService;

namespace Cubase.Midi.Sync.Server.Services.Area.Transport
{
    public class TransportAreaService : IAreaService
    {
        private Dictionary<CubaseActionName, Func<CubaseActionRequest, CubaseActionResponse>> actionHandlers;

        private CubaseMappingCollection keyboardHandlers;

        private readonly IKeyboardService keyboardService;

        public TransportAreaService(IKeyboardService keyboardService) 
        { 
           this.keyboardService = keyboardService;  
           actionHandlers = new Dictionary<CubaseActionName, Func<CubaseActionRequest, CubaseActionResponse>>
           {
               { CubaseActionName.TransportPlay, HandlePlay },
               // Add more action handlers as needed
           };
           keyboardHandlers = CubaseMappingCollection.LoadFromFile(CubaseServerConstants.KeyMappingFileLocation);
        }
        
        public CubaseActionResponse ProcessAction(CubaseActionRequest request)
        {
            if (keyboardHandlers.ContainsCubaseKey(request.Action))
            {
                 return HandleKeyboardEvent(keyboardHandlers.GetCubaseKey(request.Action));
            }
            else if (actionHandlers.ContainsKey(request.Action))
            {
                return actionHandlers[request.Action](request);
            }
            else
            {
                return CubaseActionResponse.CreateError($"No handler for action {request.Action}");
            }
        }

        [Obsolete("Use ProcessKeyboardAction instead")]
        private CubaseActionResponse HandlePlay(CubaseActionRequest request)
        {
            return CubaseActionResponse.CreateSuccess();
        }

        private CubaseActionResponse HandleKeyboardEvent(VirtualKey virtualKey)
        {
            var keyPressed = this.keyboardService.SendKey(virtualKey);  
            return keyPressed ? CubaseActionResponse.CreateSuccess() : CubaseActionResponse.CreateError("Failed to send key event");    
        }
    }
}
