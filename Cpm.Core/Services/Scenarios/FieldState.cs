using System.Collections.Generic;

namespace Cpm.Core.Services.Scenarios
{
    public class FieldState
    {
        public bool IsVisible { get; private set; }
        public string Algorithm { get; private set; }
        public IReadOnlyDictionary<string, string> Settings { get; private set; }

        public static FieldState Default = new FieldState(
            new Dictionary<string, string>
            {
                {"WeekOffset", "0"}
            },
            "Static", 
            true
            );

        public FieldState(IReadOnlyDictionary<string, string> settings, string algorithm, bool isVisible)
        {
            IsVisible = isVisible;
            Algorithm = algorithm;
            Settings = new Dictionary<string, string>(settings);
        }

        public FieldState ChangeAlgorithm(string algorithm)
        {
            var state = Duplicate();
            state.Algorithm = algorithm;
            return state;
        }

        private FieldState Duplicate()
        {
            return new FieldState(Settings, Algorithm, IsVisible);
        }

        public FieldState ChangeSettings(string name, string value)
        {
            var state = Duplicate();
            var dict = new Dictionary<string, string>(Settings)
            {
                [name] = value
            };
            state.Settings = dict;
            return state;
        }

        public FieldState ChangeIsVisible(bool isVisible)
        {
            var state = Duplicate();
            state.IsVisible = isVisible;
            return state;
        }
    }
}