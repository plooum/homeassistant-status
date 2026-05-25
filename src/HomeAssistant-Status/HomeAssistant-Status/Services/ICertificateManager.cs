using System.Security.Cryptography.X509Certificates;

namespace HomeAssistant_Status.Services;

public interface ICertificateManager
{
    public X509Certificate2 GetOrCreateCertificate();
}