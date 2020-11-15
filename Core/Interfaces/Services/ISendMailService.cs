using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DomainModels;

namespace Core.Interfaces.Services
{
    public interface ISendMailService
    {
        public Task SendMail(List<ScheduledMailModel> mailsList);
    }
}