using Application.External.Taxograf.Models;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Services
{
    public class ScheduledRequestService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ScheduledRequestService> _logger;
        private readonly List<ScheduledEndpoint> _scheduledEndpoints;

        public ScheduledRequestService(
            IServiceScopeFactory serviceScopeFactory,
            IConfiguration configuration,
            ILogger<ScheduledRequestService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scheduledEndpoints = LoadScheduledEndpoints();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ScheduledRequestService başladı. {Count} endpoint qeydiyyatdan keçdi.", _scheduledEndpoints.Count);

            while (!stoppingToken.IsCancellationRequested)
            {
                var tasks = _scheduledEndpoints
                    .Where(endpoint => ShouldExecute(endpoint))
                    .Select(endpoint => ExecuteEndpointAsync(endpoint, stoppingToken))
                    .ToArray();

                if (tasks.Length > 0)
                {
                    await Task.WhenAll(tasks);
                }

                // Hər dəqiqə yoxla
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private List<ScheduledEndpoint> LoadScheduledEndpoints()
        {
            var endpoints = new List<ScheduledEndpoint>();

            // Saatlıq sorğu
            var hourlyEnabled = _configuration.GetValue<bool>("ScheduledRequests:Hourly:Enabled", true);
            if (hourlyEnabled)
            {
                endpoints.Add(new ScheduledEndpoint
                {
                    Name = "HourlyManufactureBatchAsXml",
                    Url = "api/Manufacture/getBatchForManufacturingAsXml",
                    Method = "GET",
                    Interval = TimeSpan.FromHours(1),
                    LastRun = DateTime.MinValue
                });
                endpoints.Add(new ScheduledEndpoint
                {
                    Name = "HourlySentToManufacturerCallback",
                    Url = "api/Manufacture/sentToManufacturerCallback",
                    Method = "PUT",
                    Interval = TimeSpan.FromHours(1),
                    LastRun = DateTime.MinValue,
                    RequiresBody = true
                });
                _logger.LogInformation("Saatlıq sorğu aktivləşdirildi.");
            }

            // Günlük sorğu
            var dailyEnabled = _configuration.GetValue<bool>("ScheduledRequests:Daily:Enabled", true);
            if (dailyEnabled)
            {
                endpoints.Add(new ScheduledEndpoint
                {
                    Name = "DailyManufactureBatchAsXml",
                    Url = "api/Manufacture/getBatchForManufacturingAsXml",
                    Method = "GET",
                    Interval = TimeSpan.FromDays(1),
                    LastRun = DateTime.MinValue
                });
                endpoints.Add(new ScheduledEndpoint
                {
                    Name = "DailySentToManufacturerCallback",
                    Url = "api/Manufacture/sentToManufacturerCallback",
                    Method = "PUT",
                    Interval = TimeSpan.FromDays(1),
                    LastRun = DateTime.MinValue,
                    RequiresBody = true
                });
                _logger.LogInformation("Günlük sorğu aktivləşdirildi.");
            }

            return endpoints;
        }

        private bool ShouldExecute(ScheduledEndpoint endpoint)
        {
            var timeSinceLastRun = DateTime.UtcNow - endpoint.LastRun;
            return timeSinceLastRun >= endpoint.Interval;
        }

        private async Task ExecuteEndpointAsync(ScheduledEndpoint endpoint, CancellationToken cancellationToken)
        {
            endpoint.LastRun = DateTime.UtcNow;

            using var scope = _serviceScopeFactory.CreateScope();
            var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            var client = httpClientFactory.CreateClient("ScheduledRequests");

            var baseUrl = _configuration.GetValue<string>("ScheduledRequests:BaseUrl");
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                _logger.LogError("ScheduledRequests:BaseUrl configuration is missing or empty.");
                return;
            }

            var baseUri = new Uri(baseUrl);
            var fullUri = new Uri(baseUri, endpoint.Url);

            _logger.LogInformation("Scheduled endpoint '{Name}' üçün HTTP sorğu göndərilir: {Url} ({Method})", endpoint.Name, fullUri, endpoint.Method);

            var method = new HttpMethod(endpoint.Method);
            using var request = new HttpRequestMessage(method, fullUri);

            // PUT sorğusu üçün body hazırla
            if (endpoint.RequiresBody && method == HttpMethod.Put)
            {
                var requestBody = new
                {
                    cardNumbers = new List<string>(),
                    jobNumber = string.Empty
                };

                var jsonContent = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _logger.LogDebug("Request body: {Body}", jsonContent);
            }

            using var response = await client.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            _logger.LogInformation("Scheduled endpoint '{Name}' üçün HTTP sorğu tamamlandı. StatusCode: {StatusCode}", endpoint.Name, response.StatusCode);
        }
    }
}