using System.Collections.Generic;
using Cpm.Core.Services.Scenarios;

namespace Cpm.Core.Services.Context
{
    public class ScenarioContextState
    {
        public IReadOnlyDictionary<Season, string> Seasons { get; set; }
        public FieldState[] Fields { get; set; }
    }
}