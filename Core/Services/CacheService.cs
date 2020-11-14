using Core.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Core.Services
{
    public class CacheService : ICacheService
    {
        private const string ReadLineCount = "readLineCount";
        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }
        
        public int GetReadLineCount()
        {
            return _cache.TryGetValue(ReadLineCount, out int previousReadLine) ? previousReadLine : 0;
        }
        
        public void SetReadLineCount(int count)
        {
            _cache.Set(ReadLineCount, count);
        }
    }
}    