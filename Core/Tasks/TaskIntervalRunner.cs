using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Requests;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core.Tasks
{
    public class TaskIntervalRunner<T> : IHostedService, IDisposable
        where T : IRequest, new()
    {
        private const int IntervalSeconds = 10;
        private readonly ILogger<TaskIntervalRunner<T>> _logger;
        private readonly IMediator _mediator;
        private Timer _timer;

        public TaskIntervalRunner(ILogger<TaskIntervalRunner<T>> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{typeof(T)} running.");
            _timer = new Timer(DoWork, null, TimeSpan.Zero,TimeSpan.FromSeconds(IntervalSeconds));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            await _mediator.Send(new T());
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{typeof(T)} is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}