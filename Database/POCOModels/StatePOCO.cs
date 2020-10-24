using System;
using System.Linq.Expressions;
using Core.DomainModels;
using Core.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Database.POCOModels
{
    public class StatePOCO
    {
        public const string Name = "StateMail";
        
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        
        public static Expression<Func<StatePOCO, string>> MapToDomainModel =>
            state => state.Value;
    }
}