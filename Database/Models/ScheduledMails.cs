using System;
using Core.Enums;
using LinqToDB.Mapping;

namespace Database
{
    public class ScheduledMails
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Params { get; set; }
        public EmailType EmailType { get; set; }
        public EmailStatus Status { get; set; }
        public DateTime StatusChangedAt { get; set; }
    }
}