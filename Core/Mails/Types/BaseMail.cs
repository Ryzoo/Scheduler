using System;
using System.IO;
using Core.Interfaces.Mails;
using FluentEmail.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Mails.Types
{
    public abstract class BaseMail : IEmail
    {
        protected string To = "to@mail.com";
        protected string Subject;
        protected string Template;

        public IFluentEmail Prepare(string mailData, IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            return scope.ServiceProvider.GetRequiredService<IFluentEmail>()
                .To(To)
                .Subject(Subject)
                .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/../Core/Mails/Template/{Template}.cshtml",
                    PrepareParams(mailData));
        }

        protected abstract IEmailTemplateParams PrepareParams(string param);
    }
}