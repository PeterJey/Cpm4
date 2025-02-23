namespace Cpm.Core.ViewModels
{
    public class OptionVm
    {
        public string Value { get; }
        public string Description { get; }

        public OptionVm(string value, string description)
        {
            Value = value;
            Description = description;
        }
    }
}