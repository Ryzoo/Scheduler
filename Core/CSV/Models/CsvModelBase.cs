using System;
using Core.DomainModels;
using Core.Enums;
using Newtonsoft.Json;

namespace Core.CSV.Models
{
    public abstract class CsvModelBase<T>
    {
        public static Func<T, ScheduledMailModel> ToDomainModel =>
            mail => new ScheduledMailModel()
            {
                Params = JsonConvert.SerializeObject(mail),
                Status = EmailStatus.New,
                EmailType = EmailType.WelcomeMail,
                StatusChangedAt = DateTime.UtcNow,
            };
    }
}