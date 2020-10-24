using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DomainModels;

namespace Core.Interfaces.Repositories
{
    public interface IScheduledMailRepository
    {
        public Task<IReadOnlyCollection<ScheduledMailModel>> GetAll();
        public Task AddMany(IReadOnlyCollection<ScheduledMailModel> elements);
        public Task<bool> IsAnyPending();
        public Task<int> CountOfSentInMinute(DateTime minute);
        public Task<IReadOnlyCollection<ScheduledMailModel>> GetLastToSend(int lastCount);
    }
}