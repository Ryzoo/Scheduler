namespace Core.CSV.Models
{
    public class WelcomeMailDataCsvHeaders
    {
        public const string UserName = "Name";
        public const string UserEmail = "Email";
    }
    public class WelcomeMailDataCsvModel : CsvModelBase<WelcomeMailDataCsvModel>
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
    }
}