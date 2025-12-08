using Application.External.Taxograf.Models;
using Application.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManufactureController : ControllerBase
    {
        private readonly IManufactureService _manufactureService;

        public ManufactureController(IManufactureService manufactureService)
        {
            _manufactureService = manufactureService;
        }

        [HttpGet("getBatchForManufacturing")]
        public async Task<IActionResult> GetBatch() => Ok(await _manufactureService.GetBatchForManufacturingAsync());

        [HttpGet("getBatchForManufacturingAsXml")]
        public async Task<IActionResult> GetBatchXMl() => Ok(await _manufactureService.GetBatchForManufacturingAsXMLAsync());

        [HttpPut("setReadyOnManufacturerCallback")]
        public async Task<IActionResult> SetReadyOnManufacturerCallback([FromBody] SetReadyOnManufacturerCallbackRequest request)
        {
            await _manufactureService.SetReadyOnManufacturerCallbackAsync(request);
            return NoContent();
        }

        [HttpPut("sentToManufacturerCallback")]
        public async Task<IActionResult> SentToManufacturerCallback([FromBody] SentToManufacturerCallbackRequest request)
        {
            await _manufactureService.SentToManufacturerCallback(request);
            return NoContent();
        }

    }
}
