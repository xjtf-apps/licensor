using System.Text;
using System.Text.Json;
using System.Security.Cryptography;

namespace xjtf.licensor.lib;

public static partial class SoftwareLicensing
{
    public static MemoryStream GenerateLicense(Cryptography crypto, DateTime? expiresUtc, IDictionary<string, string>? features = null)
    {
        features ??= new Dictionary<string, string>();
        features.Add("ValidUntil", expiresUtc?.ToString() ?? "Indefinite");
        var signingPayload = JsonSerializer.Serialize(features, new JsonSerializerOptions() { WriteIndented = true });
        var signingPayloadBytes = Encoding.UTF8.GetBytes(signingPayload);

        var signatureBytes = crypto._algorithm.SignData(signingPayloadBytes, HashAlgorithmName.SHA256);
        var signature = Convert.ToBase64String(signatureBytes);
        features.Add("Signature", signature);

        var payload = JsonSerializer.Serialize(features, new JsonSerializerOptions() { WriteIndented = true });
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var format =
           $"""
            -----BEGIN PAYLOAD-----
            {Convert.ToBase64String(payloadBytes)}
            -----END PAYLOAD-----
            {crypto.GetPublicKeyPem()}
            """;
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(format));
        return stream;
    }
}
