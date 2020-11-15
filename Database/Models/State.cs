using LinqToDB.Mapping;

namespace Database
{
    public class State
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}