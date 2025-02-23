using System;

namespace Cpm.Core
{
    public class IdHelper
    {
        public static string NewId()
        {
            return Guid.NewGuid().ToString("D");
        }
    }
}