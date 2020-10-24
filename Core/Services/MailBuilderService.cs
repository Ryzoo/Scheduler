using System;
using System.Threading.Tasks;
using Core.Enums;
using Core.Interfaces.Mails;
using Core.Mails.Types;
using FluentEmail.Core;
using Newtonsoft.Json;

namespace Core.Services
{
    public class MailBuilderService : IMailBuilderService
    {
        private readonly IServiceProvider _serviceProvider;

        public MailBuilderService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public IFluentEmail BuildMail(EmailType type, string parameters)
        {
            var email = GetMailByType(type);
            return email
                .Prepare(parameters, _serviceProvider );
        }

        private BaseMail GetMailByType(EmailType type)
        {
            switch (type)
            {
                case EmailType.WelcomeMail:
                    return new WelcomeMail();
            }

            throw new Exception("Mail type not found");
        }
    }
}