using System.Collections.Generic;
using System.Linq;

namespace Cpm.Core.Farms
{
    public class ModelResult : IModelResult
    {
        public bool Success => !Errors.Any();

        public ICollection<string> Errors { get; set; }

        public FarmModel Model { get; set; }

        public ModelResult()
        {
            Errors = new List<string>();
        }
    }
}