using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DomainModels;
using Core.Interfaces.Mails;
using Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Core.Services
{
    public class SendMailService : ISendMailService
    {
        private readonly ILogger<SendMailService> _logger;
        private readonly IMailBuilderService _mailBuilderService;
        
        public SendMailService(ILogger<SendMailService> logger, IMailBuilderService mailBuilderService)
        {
            _logger = logger;
            _mailBuilderService = mailBuilderService;
        }
        
        // TODO: Kolekcja zamiast listy
        public async Task SendMail(List<ScheduledMailModel> mailsList)
        {
            foreach (var mail in mailsList)
            {
                _logger.LogInformation($"Try to send mail {mail.EmailType}");
                
                try
                {
                    await _mailBuilderService
                        .BuildMail(mail.EmailType, mail.Params)
                        .SendAsync();
                    _logger.LogInformation($"Mail sent.");
                }
                catch (Exception e)
                {
                    _logger.LogInformation($"Not sent: {e.Message}");
                }
            }
        }
    }
}