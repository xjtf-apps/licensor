namespace xjtf.protection.lib;

public static partial class SoftwareProtection
{
    public interface ILicenseAccessor
    {
        LicenseDuration Duration { get; }
        IReadOnlyList<EnabledFeature> Features { get; }
        bool IsValidCurrently => Duration.IsValidCurrently();
    }

    internal sealed class LicenseAccessor : ILicenseAccessor
    {
        private readonly License? _license;

        public LicenseAccessor(IServiceProvider serviceProvider)
        {
            var licenseReader = serviceProvider.GetRequiredService<ILicenseReader>();
            var licenseStore = serviceProvider.GetRequiredService<ILicenseStore>();
            var license = licenseStore.GetLicense();

            if (license == null)
            {
                var keyBytes = licenseReader.ReadUtf8LicenseBytes();
                license = DecodeLicenseFromKey(keyBytes);
                licenseStore.SetLicense(license);
            }

            _license = licenseStore.GetLicense();
        }

        public LicenseDuration Duration => _license?.Duration ?? new ExpiredLicense();
        public IReadOnlyList<EnabledFeature> Features => _license?.Features ?? new List<EnabledFeature>();
    }

    internal static License DecodeLicenseFromKey(byte[] keyBytes)
    {
        throw new NotImplementedException();
    }
}