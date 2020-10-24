using Core.Interfaces.Mails;
using Newtonsoft.Json;

namespace Core.Mails.Types
{
    public class WelcomeParams : IEmailTemplateParams
    {
        public string UserName;
        public string UserEmail;
    }
    
    public class WelcomeMail : BaseMail
    {
        public WelcomeMail()
        {
            Subject = "Welcome!";
            Template = "WelcomeMailTemplate";
        }

        protected override IEmailTemplateParams PrepareParams(string param)
        {
            return JsonConvert.DeserializeObject<WelcomeParams>(param);
        }
    }
}