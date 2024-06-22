using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer.Certificates;

public class SigningAudienceCertificate : IDisposable
{
    private readonly RSA rsa;

    public SigningAudienceCertificate()
    {
        rsa = RSA.Create();
    }

    public void Dispose()
    {
        rsa?.Dispose();
    }

    public SigningCredentials GetAudienceSigningKey()
    {
        var privateXmlKey = File.ReadAllText("./private_key.pem");
        rsa.ImportFromPem(privateXmlKey);

        return new SigningCredentials(
            new RsaSecurityKey(rsa),
            SecurityAlgorithms.RsaSha256);
    }
}