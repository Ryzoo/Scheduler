using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;

namespace Database
{
    public class DatabaseContext : DataConnection
    {
        public ITable<ScheduledMails> ScheduledMails => GetTable<ScheduledMails>();
        public ITable<State> State => GetTable<State>();
        public DatabaseContext(LinqToDbConnectionOptions<DatabaseContext> options) :base(options)
        {}
    }
}