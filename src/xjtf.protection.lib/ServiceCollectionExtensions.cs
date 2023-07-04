using static xjtf.protection.lib.SoftwareProtection;

namespace xjtf.protection.lib;

public static class ServiceCollectionExtensions
{
    public static void AddSoftwareProtection(this IServiceCollection services)
    {
        //services.AddSingleton<ILicenseReader>();
        services.AddSingleton<ILicenseStore, LicenseStore>();
        services.AddTransient<ILicenseAccessor, LicenseAccessor>();
    }
}