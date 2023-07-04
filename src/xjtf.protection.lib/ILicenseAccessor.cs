using System.Text.Json;

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
                license = DecodeLicenseContents(keyBytes);
                licenseStore.SetLicense(license);
            }

            _license = licenseStore.GetLicense();
        }

        public LicenseDuration Duration => _license?.Duration ?? new ExpiredLicenseDuration();
        public IReadOnlyList<EnabledFeature> Features => _license?.Features ?? new List<EnabledFeature>();
    }

    internal static License DecodeLicenseContents(byte[] licenseBytes)
    {
        // -----BEGIN PAYLOAD-----
        // -----END PAYLOAD-----
        // -----BEGIN PUBLIC KEY-----
        // -----END PUBLIC KEY-----

        using var crypto = ECDsa.Create();
        var licenseString = Encoding.UTF8.GetString(licenseBytes);
        var licensePemIndex = licenseString.IndexOf("-----BEGIN PUBLIC KEY-----");
        var licensePayload = licenseString[..licensePemIndex].Replace("-----BEGIN PAYLOAD-----", "").Replace("-----END PAYLOAD-----", "");
        var licensePem = licenseString[licensePemIndex..];
        crypto.ImportFromPem(licensePem);

        var licensePayloadContents =
            JsonSerializer.Deserialize<Dictionary<string, string>>(Encoding.UTF8.GetString(Convert.FromBase64String(licensePayload)))!;
        var licensePayloadSignature =
            Convert.FromBase64String(licensePayloadContents["Signature"]);
        var licenseExpiry =
            licensePayloadContents["ValidUntil"] == "Indefinite"
            ? (DateTime?)null : DateTime.Parse(licensePayloadContents["ValidUntil"]);

        var signingPayloadBytes =
            Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(
                    licensePayloadContents
                        .Where(kvp => kvp.Key != "Signature")
                            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value), new JsonSerializerOptions() { WriteIndented = true }));


        if (crypto.VerifyData(signingPayloadBytes, licensePayloadSignature!, HashAlgorithmName.SHA256))
        {
            var licenseDuration = licenseExpiry == null
                ? (LicenseDuration)new IndefiniteLicenseDuration()
                : new ExpiringLicenseDuration((DateTime)licenseExpiry);

            var features = licensePayloadContents
                .Where(kvp => kvp.Key != "Signature" && kvp.Key != "ValidUntil")
                .Select(kvp => new EnabledFeature(kvp.Key, kvp.Value))
                .ToList();

            return new License(licenseDuration, features);
        }
        else throw new InvalidDataException("The cryptographic signature of the license payload cannot be verified!");
    }
}