namespace xjtf.protection.lib;

public static partial class SoftwareProtection
{
    public sealed class License
    {
        public LicenseDuration Duration { get; }
        public IReadOnlyList<EnabledFeature> Features { get; }
        public bool IsValidCurrently => Duration.IsValidCurrently();

        internal License(LicenseDuration duration, IReadOnlyList<EnabledFeature> features)
        {
            Duration = duration;
            Features = features;
        }
    }
}
