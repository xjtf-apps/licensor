# xjtf licensor

License generator and verifier for your .NET apps

## Generate a license

> Install-Package xjtf.licensor.lib

```csharp
using var crypto = new SoftwareLicensing.Cryptography();
var features = new Dictionary<string,string> { { "FeatureA", "" } };
var stream = SoftwareLicensing.GenerateLicense(crypto, DateTime.UtcNow.AddDays(30), features);
```

Note that the example uses an implicit random cryptographic key. You would most likely want to use a predetermined private key. You can pass your own key to the Cryptography class constructor, or first generate it with the Crypography class and load it later from some store.

If you pass null as the argument for the date and time of expiry, an indefinite license will be created.

The features dictionary can be used to pass arbitrary data to the application which will be loading the license.

## Load and verify the license

> Install-Package xjtf.protection.lib

```csharp
// 1a. configure the app dependency injection

public void ConfigureServices(IServiceCollection services)
{
    // ...
    services.AddSoftwareProtection();
    services.AddSingleton<SoftwareProtection.ILicenseReader>(new CustomLicenseReader());
    // ...
}

// 1b. provide your own mechanism of loading the license

public class CustomLicenseReader : SoftwareProtection.ILicenseReader
{
    public byte[] ReadUtf8LicenseBytes()
    {
        throw new NotImplementedException();
    }
}

// 2. request the license accessor in some DI context

public class SomeDataService
{
    public SomeDataService(SoftwareProtection.ILicenseAccessor license)
    {
        if (!license.IsCurrentlyValid)
            throw new NotSupportedException();

        if (license.Features.IsFeatureEnabled("FeatureA"))
        {
            // ...
        }
    } 
}
```

Note that, when providing your own ILicenseReader, which you must, you may choose a different DI service lifetime if required. The license reader will be called the first time you instantiate a license accessor, while on subsequent occassions a cached license will be used.

We currently don't have a mechanism to revoke the license privileges, so it's your responsibility to check that the license is valid as often as possible.

For one, it would be a good idea to check it the license is valid sometime during app startup.

## Security

The system uses an industry-practiced elliptic curve algorithm to sign the generated license, which is later validated against license payload integrity.

Please don't distribute the license generator package with your end-user bits, only the protection package.

## Contributing

Feel free to open a issue or PR!

## Thanks

Software inspired by previous work and this [blog post](https://ayende.com/blog/199617-A/using-encryption-to-verify-a-license-key).