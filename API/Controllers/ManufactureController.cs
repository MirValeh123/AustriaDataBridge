using Application.Services;
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

    }
}
