using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Enums;
using Core.Interfaces.Mails;
using Core.Interfaces.Repositories;
using Core.Requests;
using FluentEmail.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Handlers
{
    public class SendMailHandler : AsyncRequestHandler<SendMailRequest>
    {
        private readonly ILogger<SendMailHandler> _logger;
        private readonly IMailBuilderService _mailBuilderService;
        private readonly IScheduledMailRepository _mailRepository;

        public SendMailHandler(ILogger<SendMailHandler> logger, IMailBuilderService mailBuilderService, IScheduledMailRepository mailRepository)
        {
            _logger = logger;
            _mailBuilderService = mailBuilderService;
            _mailRepository = mailRepository;
        }
        
        protected override async Task Handle(SendMailRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Try to send mail {request.Mail.EmailType}");

            try
            {
                await _mailRepository.ChangeStatus(request.Mail.Id, EmailStatus.Pending);
                await _mailBuilderService
                        .BuildMail(request.Mail.EmailType, request.Mail.Params)
                        .SendAsync();
                await _mailRepository.ChangeStatus(request.Mail.Id, EmailStatus.Sent);
                _logger.LogInformation($"Mail sent.");
            }
            catch (Exception e)
            {
                await _mailRepository.ChangeStatus(request.Mail.Id, EmailStatus.New);
                _logger.LogInformation($"Not sent: {e.Message}");
            }
        }
    }
}