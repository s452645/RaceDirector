using backend.Services.Hardware;
using backend.Services.Hardware.Comms;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers.Hardware
{

    [Route("api/[controller]")]
    [ApiController]
    public class ElevatorController : Controller
    {
        private readonly HardwareCommunicationService _commsService;

        public ElevatorController(HardwareCommunicationService commsService)
        {
            _commsService = commsService;
        }


        // POST: api/Elevator/command
        [HttpPost("command")]
        public ActionResult PostCommand(string command)
        {
            _commsService.SendMessage(command);
            return Ok();
        }
    }
}
