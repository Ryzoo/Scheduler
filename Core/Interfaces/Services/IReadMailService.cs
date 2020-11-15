using System.Collections.Generic;
using Core.DomainModels;

namespace Core.Interfaces.Services
{
    public interface IReadMailService
    {
        public List<ScheduledMailModel> ReadMail();
    }
}