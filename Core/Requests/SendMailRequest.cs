using Core.DomainModels;
using MediatR;

namespace Core.Requests
{
    public class SendMailRequest : IRequest
    {
        public ScheduledMailModel Mail;
    }
}