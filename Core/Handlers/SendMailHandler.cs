using System.Threading;
using System.Threading.Tasks;
using Core.Requests;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Handlers
{
    public class SendMailHandler : AsyncRequestHandler<SendMailRequest>
    {
        private readonly ILogger<SendMailHandler> _logger;

        public SendMailHandler(ILogger<SendMailHandler> logger)
        {
            _logger = logger;
        }
        
        protected override Task Handle(SendMailRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Try to send mail {request.Mail.EmailType}");
            
            // TODO
            // connect fluent mail
            // 
            
            return Task.CompletedTask;
        }
    }
}