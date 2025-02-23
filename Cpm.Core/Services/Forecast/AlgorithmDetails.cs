namespace Cpm.Core.Services.Forecast
{
    public class AlgorithmDetails
    {
        public string Name { get; }
        public string Description { get; }
        public IAlgorithm Algorithm { get; }

        public AlgorithmDetails(string name, string description, IAlgorithm algorithm)
        {
            Name = name;
            Description = description;
            Algorithm = algorithm;
        }
    }
}