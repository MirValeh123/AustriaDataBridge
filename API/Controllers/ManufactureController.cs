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
        public async Task<IActionResult> GetBatch()
        {
            var result = await _manufactureService.GetBatchForManufacturingAsync();
            return Ok(result);
        }
    }
}
