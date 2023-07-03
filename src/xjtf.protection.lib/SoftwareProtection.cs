namespace xjtf.protection.lib;

public static partial class SoftwareProtection
{
    public class VerificationResult
    {
        internal static readonly FailedVerificationResult Failed = new();
        internal sealed class FailedVerificationResult : VerificationResult { }
        internal sealed class PositiveVerificationResult : VerificationResult { internal License License { get; } }

        public void Deconstruct(out License? license, out IReadOnlyList<EnabledFeature> features)
        {
            license = null;
            features = new List<EnabledFeature>();

            if (this is FailedVerificationResult) return;
            if (this is PositiveVerificationResult result)
            {
                license = result.License;
                features = license.Features;
            }
        }
    }
}
