using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core.Tasks
{
    public class ScheduleRunner : IHostedService, IDisposable
    {
        private const int IntervalSeconds = 10;
        private readonly ILogger<ScheduleRunner> _logger;
        private readonly IReadMailService _readMailService;
        private readonly ISendMailService _sendMailService;
        private Timer _timer;

        public ScheduleRunner(ILogger<ScheduleRunner> logger, IReadMailService readMailService, ISendMailService sendMailService)
        {
            _logger = logger;
            _readMailService = readMailService;
            _sendMailService = sendMailService;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Scheduler running.");
            _timer = new Timer(DoWork, null, TimeSpan.Zero,TimeSpan.FromSeconds(IntervalSeconds));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            var mailsList = _readMailService.ReadMail();
            
            _logger.LogInformation("Starting sending mails");
            await _sendMailService.SendMail(mailsList);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Scheduler is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}