using Application.External.Taxograf.Models;
using Application.Services.Abstract;
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
                    Name = "HourlyManufactureBatch",
                    Url = "/Manufacture/getBatchForManufacturing",
                    Method = "GET",
                    Interval = TimeSpan.FromHours(1),
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
                    Name = "DailyManufactureBatch",
                    Url = "/Manufacture/getBatchForManufacturing",
                    Method = "GET",
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
            if (!endpoint.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            endpoint.LastRun = DateTime.UtcNow;

            using var scope = _serviceScopeFactory.CreateScope();
            var manufactureService = scope.ServiceProvider.GetRequiredService<IManufactureService>();
            var response = await manufactureService.GetBatchForManufacturingAsXMLAsync();
        }
    }
}
