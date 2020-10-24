namespace Core.Mails.Types
{
    public class WelcomeMail : BaseMail
    {
        public WelcomeMail()
        {
            Template = "WelcomeMailTemplate";
        }
    }
}