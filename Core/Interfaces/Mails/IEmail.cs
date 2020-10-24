using System;
using FluentEmail.Core;

namespace Core.Interfaces.Mails
{
    public interface IEmail
    {
        public IFluentEmail Prepare(string mailData, IServiceProvider serviceProvider);
    }
}