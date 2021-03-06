﻿using System;
using Core.Enums;

namespace Core.DomainModels
{
    public class ScheduledMailModel
    {
        public string Id { get; set; }
        public string Params { get; set; }
        public EmailType EmailType { get; set; }
        public EmailStatus Status { get; set; }
        public DateTime StatusChangedAt { get; set; }
    }
}