using xjtf.protection.lib;

namespace xjtf.licensor;

public class IntegrationTest
{
    private readonly SoftwareLicensing.Cryptography _crypto = new();

    [Fact]
    public void GeneratedLicenseValidSignedAndContentfull()
    {
        var dateTimeExpires = DateTime.UtcNow.AddDays(1);
        var features = new Dictionary<string, string>
        {
            { "FeatureA", "" },
            { "FeatureB", "" },
            { "FeatureC", "" }
        };
        var licenseStream = SoftwareLicensing.GenerateLicense(_crypto, dateTimeExpires, features);
        var licenseText = Encoding.UTF8.GetString(licenseStream.ToArray());

        var services = new ServiceCollection();
        services.AddSoftwareProtection();
        services.AddSingleton<SoftwareProtection.ILicenseReader>(new LicenseReader(licenseText));
        var provider = services.BuildServiceProvider();
        var accessor = provider.GetRequiredService<SoftwareProtection.ILicenseAccessor>();

        Assert.NotNull(accessor);
        Assert.True(accessor.IsValidCurrently);
        Assert.True(accessor.Features.IsFeatureEnabled("FeatureA"));
        Assert.True(accessor.Features.IsFeatureEnabled("FeatureB"));
        Assert.True(accessor.Features.IsFeatureEnabled("FeatureC"));
    }
}

public class LicenseReader : SoftwareProtection.ILicenseReader
{
    private readonly string _licenseText;
    public LicenseReader(string licenseText) => _licenseText = licenseText;
    public byte[] ReadUtf8LicenseBytes() => Encoding.UTF8.GetBytes(_licenseText);
}