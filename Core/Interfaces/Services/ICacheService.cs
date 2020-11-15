using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface ICacheService
    {
        public int GetReadLineCount();
        public void SetReadLineCount(int count);
    }
}