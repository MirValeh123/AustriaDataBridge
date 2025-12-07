using Application.External.Taxograf.Models;
using System.Net.Http;
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
                    Interval = TimeSpan.FromMinutes(1),
                    LastRun = DateTime.MinValue
                });
                endpoints.Add(new ScheduledEndpoint
                {
                    Name = "HourlyUploadBatchXml",
                    Url = "api/Sftp/uploadBatchXml",
                    Method = "PUT",
                    Interval = TimeSpan.FromMinutes(1),
                    LastRun = DateTime.MinValue
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
                    Name = "DailyUploadBatchXml",
                    Url = "api/Sftp/uploadBatchXml",
                    Method = "PUT",
                    Interval = TimeSpan.FromDays(1),
                    LastRun = DateTime.MinValue
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
            using var request = new HttpRequestMessage(method, fullUri)
            {
                // PUT üçün body tələb olunmursa boş content göndəririk
                Content = method == HttpMethod.Put ? new StringContent(string.Empty) : null
            };

            using var response = await client.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            _logger.LogInformation("Scheduled endpoint '{Name}' üçün HTTP sorğu tamamlandı. StatusCode: {StatusCode}", endpoint.Name, response.StatusCode);
        }
    }
}
