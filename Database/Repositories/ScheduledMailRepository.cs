using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DomainModels;
using Core.Enums;
using Core.Interfaces.Repositories;
using Database.POCOModels;
using MongoDB.Driver;

namespace Database.Repositories
{
    public class ScheduledMailRepository : IScheduledMailRepository
    {
        private readonly DatabaseContext _context;

        public ScheduledMailRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<ScheduledMailModel>> GetAll()
        {
            return await _context.ScheduledMails
                .Find(_ => true)
                .Project(ScheduledMailPOCO.ToDomainModel)
                .ToListAsync();
        }

        public async Task AddMany(IReadOnlyCollection<ScheduledMailModel> elements)
        {
            await _context.ScheduledMails
                .InsertManyAsync(elements.Select(ScheduledMailPOCO.FromDomainModel));
        }

        public async Task<bool> IsAnyPending()
        {
            var pendingCount = await _context.ScheduledMails
                .Find(x => x.Status == EmailStatus.Pending)
                .CountDocumentsAsync();

            return pendingCount > 0;
        }

        public async Task<int> CountOfSentInMinute(DateTime minute)
        {
            var sentEmails = await _context.ScheduledMails
                .Find(x => x.Status == EmailStatus.Sent)
                .Project(ScheduledMailPOCO.ToDomainModel)
                .ToListAsync();

            return sentEmails
                .Count(x => x.StatusChangedAt.Minute == minute.Minute);
        }

        public async Task<IReadOnlyCollection<ScheduledMailModel>> GetLastToSend(int lastCount)
        {
            return await _context.ScheduledMails
                .Find(x => x.Status == EmailStatus.New)
                .Limit(lastCount)
                .Project(ScheduledMailPOCO.ToDomainModel)
                .ToListAsync();
        }

        public async Task ChangeStatus(string id, EmailStatus status)
        {
            var mail = await _context.ScheduledMails
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (mail == null)
                throw new Exception($"Mail with {id} not exist.");

            mail.Status = status;
            await _context.ScheduledMails
                .ReplaceOneAsync(x => x.Id == id, mail);
        }
    }
}