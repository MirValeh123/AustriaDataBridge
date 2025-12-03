using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Services
{
    public class ScheduledRequestService : BackgroundService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly TimeSpan _interval;

        public ScheduledRequestService(IHttpClientFactory httpClientFactory, IConfiguration configuration, TimeSpan interval)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
            // Interval-i konfiqurasiyadan al (default: 1 saat)
            var intervalInHours = _configuration.GetValue<int>("ServiceIntervalHours", 1);
            _interval = TimeSpan.FromHours(intervalInHours);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
