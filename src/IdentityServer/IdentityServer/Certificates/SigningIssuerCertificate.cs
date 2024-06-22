using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer.Certificates;

public class SigningIssuerCertificate : IDisposable
{
    private readonly RSA rsa;

    public SigningIssuerCertificate()
    {
        rsa = RSA.Create();
    }

    public void Dispose()
    {
        rsa?.Dispose();
    }

    public RsaSecurityKey GetIssuerSigningKey()
    {
        var publicXmlKey = File.ReadAllText("./public_key.pem");
        rsa.ImportFromPem(publicXmlKey);

        return new RsaSecurityKey(rsa);
    }
}