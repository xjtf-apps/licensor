using System.Security.Cryptography;

namespace xjtf.licensor.lib;

public static partial class SoftwareLicensing
{
    public sealed class Cryptography : IDisposable
    {
        internal readonly ECDsa _algorithm;
        public Cryptography() { _algorithm = ECDsa.Create(); }
        public Cryptography(string privateKeyPem) : this()
        {
            _algorithm.ImportFromPem(privateKeyPem);
        }
        public void Dispose() => ((IDisposable)_algorithm).Dispose();
        public string GetPrivateKeyPem() => _algorithm.ExportPkcs8PrivateKeyPem();
        public string GetPublicKeyPem() => _algorithm.ExportSubjectPublicKeyInfoPem();
    }
}
