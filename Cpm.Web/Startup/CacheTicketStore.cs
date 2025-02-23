using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;

namespace Cpm.Web.Startup
{
    public class CacheTicketStore : ITicketStore
    {
        private const string Prefix = "CacheTicketStore-";
        private IMemoryCache _cache;

        public CacheTicketStore()
        {
        }

        public void SetCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var key = Prefix + Guid.NewGuid();
            await RenewAsync(key, ticket);
            return key;
        }
        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            if (_cache != null)
            {
                var options = new MemoryCacheEntryOptions();
                var expiresUtc = ticket.Properties.ExpiresUtc;
                if (expiresUtc.HasValue)
                {
                    options.SetAbsoluteExpiration(expiresUtc.Value);
                }
                options.SetSlidingExpiration(TimeSpan.FromMinutes(60));
                _cache.Set(key, ticket, options);
            }
            return Task.FromResult(0);
        }
        public Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            if (_cache == null) return Task.FromResult<AuthenticationTicket>(null);

            _cache.TryGetValue(key, out AuthenticationTicket ticket);

            return Task.FromResult(ticket);
        }
        public Task RemoveAsync(string key)
        {
            _cache?.Remove(key);
            return Task.FromResult(0);
        }
    }
}