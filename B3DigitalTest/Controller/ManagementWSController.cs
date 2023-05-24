using B3DigitalService;
using Microsoft.AspNetCore.Mvc;

namespace B3DigitalTest.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagementWSController : ControllerBase
    {
        IWebSocketService iWebSocketService { get; }
        public ManagementWSController(IWebSocketService webSocketService)
        {
            iWebSocketService = webSocketService;
        }

        [HttpPut("PowerOn")]
        public async Task<IActionResult> PowerOn(CancellationToken cancellationToken)
        {
            if (!await iWebSocketService.CheckIsRunning())
            {
                await iWebSocketService.StartAsync(cancellationToken);

                return Ok("OK");
            }
            else 
            {
                return BadRequest("WS all ready running!");
            }
        }

        [HttpPut("PowerOff")]
        public async Task<IActionResult> PowerOff(CancellationToken cancellationToken)
        {
            await iWebSocketService.StopAsync(cancellationToken);

            return Ok("OK");
        }
    }
}
