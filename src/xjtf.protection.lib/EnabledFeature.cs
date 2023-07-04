namespace xjtf.protection.lib;

public static partial class SoftwareProtection
{
    public class EnabledFeature
    {
        private readonly string _name;
        private readonly string? _value;

        internal EnabledFeature(string name, string? value)
        {
            _name = name;
            _value = value;
        }

        public static explicit operator FeatureKey(EnabledFeature feature)
        {
            return new FeatureKey { UniqueFeatureName = feature._name };
        }

        public static explicit operator FeatureValue(EnabledFeature feature)
        {
            return new FeatureValue { Value = feature._value };
        }
    }

    public class FeatureKey
    {
        public string UniqueFeatureName { get; internal init; }
    }

    public class FeatureValue
    {
        public string? Value { get; internal init; }
    }
}