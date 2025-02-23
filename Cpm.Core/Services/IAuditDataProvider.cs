using System;

namespace Cpm.Core.Services
{
    public interface IAuditDataProvider
    {
        string GetUserField();
        DateTime GetTimestampField();
    }
}