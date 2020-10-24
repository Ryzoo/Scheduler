using Core.Interfaces.Mails;

namespace Core.Mails.Types
{
    public abstract class BaseMail : IEmail
    {
        protected string From = "";
        protected string To;
        protected string Subject;
        protected string Template;

        public void SetFrom(string email)
        {
            From = email;
        }
    }
}