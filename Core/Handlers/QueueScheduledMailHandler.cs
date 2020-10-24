using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces.Repositories;
using Core.Requests;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Handlers
{
    public class QueueScheduledMailHandler : AsyncRequestHandler<QueueScheduledMailRequest>
    {
        private const int MaxEmailPerMinute = 100;
        private readonly ILogger<ReadMailFileHandler> _logger;
        private readonly IScheduledMailRepository _mailRepository;
        private readonly IMediator _mediator;

        public QueueScheduledMailHandler(ILogger<ReadMailFileHandler> logger, IScheduledMailRepository mailRepository,
            IMediator mediator)
        {
            _logger = logger;
            _mailRepository = mailRepository;
            _mediator = mediator;
        }

        protected override async Task Handle(QueueScheduledMailRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("SendScheduledMailHandler handled");

            if (await _mailRepository.IsAnyPending())
                return;

            var countOfSentInMinute = await _mailRepository.CountOfSentInMinute(DateTime.UtcNow);
            var actualTryToSendCount = MaxEmailPerMinute - countOfSentInMinute;
            var mailToSend = await _mailRepository.GetLastToSend(actualTryToSendCount);

            _logger.LogInformation($"Try to send {actualTryToSendCount}");

            foreach (var mail in mailToSend)
                await _mediator.Send(new SendMailRequest()
                {
                    Mail = mail
                }, cancellationToken);
        }
    }
}