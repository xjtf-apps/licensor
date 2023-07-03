namespace xjtf.protection.lib;

public static partial class SoftwareProtection
{
    public class EnabledFeature
    {
        public static explicit operator FeatureKey(EnabledFeature feature)
        {
            throw new NotImplementedException();
        }
    }

    public class FeatureKey
    {
        public string UniqueFeatureName { get; }
    }
}