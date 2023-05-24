using B3DigitalModel;
using B3DigitalService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace B3DigitalTest.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisFindController : ControllerBase
    {
        IDistributedCache iDistributedCache { get; }

        public RedisFindController(IDistributedCache distributedCache) 
        {
            iDistributedCache = distributedCache;
        }

        [HttpGet("State/{criptoType}")]
        public async Task<IActionResult> GetState(CriptoType criptoType, CancellationToken cancelation)
        {
            try
            {
                var result = await iDistributedCache.GetAsync(criptoType.ToString(), cancelation);

                if (result != null) 
                {
                    var obj = Encoding.UTF8.GetString(result);

                    var data = JsonConvert.DeserializeObject<QuoteInfo>(obj);

                    return Ok(data);
                }
                else 
                {
                    return BadRequest("Key not found!");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("Calculo/{id}")]
        public async Task<IActionResult> GetCalculo(Guid id, CancellationToken cancelation)
        {
            try
            {
                var result = await iDistributedCache.GetAsync(id.ToString(), cancelation);

                if (result != null)
                {
                    var obj = Encoding.UTF8.GetString(result);

                    var data = JsonConvert.DeserializeObject<BestPricePayload>(obj);

                    return Ok(data);
                }
                else
                {
                    return BadRequest("Key not found!");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
