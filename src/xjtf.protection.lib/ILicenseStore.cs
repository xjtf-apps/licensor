namespace xjtf.protection.lib;

public static partial class SoftwareProtection
{
    internal interface ILicenseStore
    {
        License? GetLicense();
        internal void SetLicense(License license);
    }

    internal sealed class LicenseStore : ILicenseStore
    {
        private License? _license;
        License? ILicenseStore.GetLicense() => _license;
        void ILicenseStore.SetLicense(License license) => _license = license;
    }
}