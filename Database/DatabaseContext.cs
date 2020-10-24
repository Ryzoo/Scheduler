using Core.Interfaces.Database;
using Core.Settings;
using Database.POCOModels;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Database
{
    public class DatabaseContext : IDatabaseContext
    {
        public IMongoCollection<ScheduledMailPOCO> ScheduledMails { get; }
        public IMongoCollection<StatePOCO> State { get; }

        public DatabaseContext(IOptions<DbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.Database);
            
            ScheduledMails = database.GetCollection<ScheduledMailPOCO>(ScheduledMailPOCO.Name);
            State = database.GetCollection<StatePOCO>(StatePOCO.Name);
        }
    }
}