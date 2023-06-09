﻿using B3DigitalModel;
using B3DigitalService;
using Microsoft.AspNetCore.Mvc;

namespace B3DigitalTest.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class BestPriceController : ControllerBase
    {
        ICalculateBestPrice iCalculateBestPrice { get; }

        public BestPriceController(ICalculateBestPrice calculateBestPrice) 
        {
            iCalculateBestPrice = calculateBestPrice;
        }


        [HttpGet("{criptoType}/{side}/{quantity}")]
        public async Task<IActionResult> Get(CriptoType criptoType, Side side, int quantity)
        {
            try
            {
                var result = iCalculateBestPrice.CalculatePrice(criptoType, side, quantity);

                return Ok(result);
            }
            catch (Exception e) 
            {
                return BadRequest(e.Message);
            }
        }
    }
}
