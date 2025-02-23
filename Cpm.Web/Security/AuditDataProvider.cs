using System;
using System.Linq;
using Cpm.Core;
using Cpm.Core.Services;
using Microsoft.AspNetCore.Http;

namespace Cpm.Web.Security
{
    public class AuditDataProvider : IAuditDataProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditDataProvider(
            IHttpContextAccessor httpContextAccessor
            )
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserField()
        {
            return _httpContextAccessor?.HttpContext?.User?.Claims
                       ?.FirstOrDefault(x => x.Type == ClaimTypes.LongName)
                       ?.Value 
                   ?? "?";
        }

        public DateTime GetTimestampField()
        {
            return Clock.Now.ToUniversalTime();
        }
    }
}