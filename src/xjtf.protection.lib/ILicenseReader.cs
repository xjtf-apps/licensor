namespace xjtf.protection.lib;

public static partial class SoftwareProtection
{
    public interface ILicenseReader
    {
        byte[] ReadUtf8LicenseBytes();
    }
}