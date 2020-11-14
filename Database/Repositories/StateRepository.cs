using System.ComponentModel;
using System.Threading.Tasks;
using Core.Enums;
using Core.Interfaces.Repositories;
using Database.POCOModels;

namespace Database.Repositories
{
    public class StateRepository : IStateRepository
    {
        private readonly DatabaseContext _context;

        public StateRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<T> GetState<T>(StateType key)
        {
            var state = await _context.State
                .Find(x => x.Key == key.ToString())
                .Project(StatePOCO.MapToDomainModel)
                .FirstOrDefaultAsync();

            return (T) (!string.IsNullOrEmpty(state)
                ? TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(state)
                : null);
        }

        public async Task SetState<T>(StateType key, string value)
        {
            var currentState = await _context.State
                .Find(x => x.Key == key.ToString())
                .FirstOrDefaultAsync();

            if (currentState != null)
            {
                currentState.Value = value;
                await _context.State.ReplaceOneAsync(x => x.Id == currentState.Id, currentState);
            }
            else
            {
                var newState = new StatePOCO()
                {
                    Key = key.ToString(),
                    Value = value
                };
                await _context.State.InsertOneAsync(newState);
            }
        }
    }
}