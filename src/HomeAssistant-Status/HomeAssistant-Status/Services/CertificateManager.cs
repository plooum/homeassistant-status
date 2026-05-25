using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace HomeAssistant_Status.Services;

public class CertificateManager(
    ILogger<CertificateManager> logger,
    IPasswordGenerator passwordGenerator) : ICertificateManager
{
    private static readonly string CertPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "server.pfx");
    
    public X509Certificate2 CreateCertificate()
    {
        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Create Certificate...");
        
        var certPassword = passwordGenerator.GetNewPassword();
        if (File.Exists(CertPath))
        {
            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("Deleting previous Certificate...");
            
            File.Delete(CertPath);
        }

        if (logger.IsEnabled(LogLevel.Debug))
            logger.LogDebug("Creating new Certificate...");
        
        using var rsa = RSA.Create(2048);

        var request = new CertificateRequest(
            "CN=localhost", 
            rsa, 
            HashAlgorithmName.SHA256, 
            RSASignaturePadding.Pkcs1);

        // Ajouter les extensions nécessaires pour un certificat serveur
        request.CertificateExtensions.Add(
            new X509EnhancedKeyUsageExtension(
                new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") }, // Server Authentication
                false));

        // Ajouter le Subject Alternative Name (SAN) pour éviter les erreurs strictes des navigateurs
        var sanBuilder = new SubjectAlternativeNameBuilder();
        sanBuilder.AddDnsName("localhost");
        sanBuilder.AddIpAddress(IPAddress.Loopback);
        request.CertificateExtensions.Add(sanBuilder.Build());
        DateTimeOffset notBefore = DateTimeOffset.UtcNow;
        // Un an de validité
        DateTimeOffset notAfter = notBefore.AddYears(1);

        using var generatedCert = request.CreateSelfSigned(notBefore, notAfter);
        // Exporter au format PFX avec la clé privée et le mot de passe
        byte[] pfxBytes = generatedCert.Export(X509ContentType.Pfx, certPassword);
        
        if (logger.IsEnabled(LogLevel.Debug))
            logger.LogDebug("Writing Certificate to {certpath}...", CertPath);
        File.WriteAllBytes(CertPath, pfxBytes);
        
        // Recharger le certificat depuis le fichier pour s'assurer que la clé privée est correctement liée
        return new (CertPath, certPassword);
    }
}