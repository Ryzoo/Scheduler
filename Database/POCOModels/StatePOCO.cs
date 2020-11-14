using System;
using System.Linq.Expressions;

namespace Database.POCOModels
{
    public class StatePOCO
    {
        public const string Name = "StateMail";

        public string Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        
        public static Expression<Func<StatePOCO, string>> MapToDomainModel =>
            state => state.Value;
    }
}