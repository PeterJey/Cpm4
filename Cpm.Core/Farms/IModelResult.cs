using System.Collections.Generic;

namespace Cpm.Core.Farms
{
    public interface IModelResult
    {
        bool Success { get; }
        ICollection<string> Errors { get; }
        FarmModel Model { get; }
    }
}