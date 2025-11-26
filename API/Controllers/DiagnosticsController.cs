using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosticsController : ControllerBase
    {
        private readonly MongoDbContext _mongoDbContext;
        private readonly ILogger<DiagnosticsController> _logger;

        public DiagnosticsController(MongoDbContext mongoDbContext, ILogger<DiagnosticsController> logger)
        {
            _mongoDbContext = mongoDbContext;
            _logger = logger;
        }

        /// <summary>
        /// Lightweight endpoint used to verify TracingMiddleware + Mongo connectivity.
        /// </summary>
        [HttpGet("trace-test")]
        public async Task<IActionResult> TraceTest()
        {
            var pingResult = await _mongoDbContext.Database.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));

            var mongoOk = pingResult.TryGetValue("ok", out var okValue) && okValue == 1;

            var response = new DiagnosticsResponse
            {
                MongoConnected = mongoOk,
                MongoMessage = pingResult.ToJson(),
                TraceIdentifier = HttpContext.TraceIdentifier,
                TimestampUtc = DateTime.UtcNow
            };

            _logger.LogInformation("Diagnostics trace-test executed. Trace={TraceId} MongoOk={MongoOk}", response.TraceIdentifier, response.MongoConnected);

            return Ok(response);
        }


        [HttpGet("test-exception")]
        public IActionResult ThrowTest()
        {
            throw new InvalidOperationException("Test exception from controller");
        }

        private sealed class DiagnosticsResponse
        {
            public bool MongoConnected { get; set; }
            public string TraceIdentifier { get; set; } = string.Empty;
            public DateTime TimestampUtc { get; set; }
            public string MongoMessage { get; set; } = string.Empty;
        }
    }
}

