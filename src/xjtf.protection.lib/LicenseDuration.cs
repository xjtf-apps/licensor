namespace xjtf.protection.lib;

public static partial class SoftwareProtection
{
    public abstract class LicenseDuration
    {
        public abstract bool IsValidCurrently();
    }

    public sealed class IndefiniteLicenseDuration : LicenseDuration
    {
        public override bool IsValidCurrently() => true;
    }

    public sealed class ExpiringLicenseDuration : LicenseDuration
    {
        private readonly DateTime ExpiresUTC;

        public override bool IsValidCurrently() => ExpiresUTC > DateTime.UtcNow;
    }

    public sealed class ExpiredLicense : LicenseDuration
    {
        public override bool IsValidCurrently() => false;
    }
}