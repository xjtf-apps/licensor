using static xjtf.protection.lib.SoftwareProtection;

namespace xjtf.protection.lib;

public static class FeaturesExtensions
{
    public static bool IsFeatureEnabled(this IReadOnlyList<EnabledFeature> features, string featureKey)
    {
        return features
            .Select(feature => (FeatureKey)feature)
            .Select(fk => fk.UniqueFeatureName)
            .Contains(featureKey);
    }
}
