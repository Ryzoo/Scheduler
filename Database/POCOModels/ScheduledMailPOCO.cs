using System;
using System.Linq.Expressions;
using Core.DomainModels;
using Core.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Database.POCOModels
{
    public class ScheduledMailPOCO
    {
        public const string Name = "ScheduledMail";
        
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Params { get; set; }
        public EmailType EmailType { get; set; }
        public EmailStatus Status { get; set; }
        public DateTime StatusChangedAt { get; set; }
        
        public static Expression<Func<ScheduledMailPOCO, ScheduledMailModel>> ToDomainModel =>
            mail => new ScheduledMailModel();
        public static Func<ScheduledMailModel, ScheduledMailPOCO> FromDomainModel =>
            mail => new ScheduledMailPOCO()
            {
                Id = mail.Id,
                Params = mail.Params,
                EmailType = mail.EmailType,
                Status = mail.Status,
                StatusChangedAt = mail.StatusChangedAt,
            };
        
    }
}