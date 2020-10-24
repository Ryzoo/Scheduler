using System.Threading.Tasks;
using Core.Enums;
using FluentEmail.Core;

namespace Core.Interfaces.Mails
{
    public interface IMailBuilderService
    {
        public IFluentEmail BuildMail(EmailType type, string parameters);
    }
}