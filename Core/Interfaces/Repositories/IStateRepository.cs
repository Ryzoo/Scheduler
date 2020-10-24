using System.Threading.Tasks;
using Core.Enums;

namespace Core.Interfaces.Repositories
{
    public interface IStateRepository
    {
        public Task<T> GetState<T>(StateType key);
        public Task SetState<T>(StateType key, string value);
    }
}