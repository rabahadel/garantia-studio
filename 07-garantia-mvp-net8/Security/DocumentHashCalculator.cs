using System.Security.Cryptography;
using System.Text;

namespace GarantIA.Security;

/// <summary>
/// Calculateur d'empreinte cryptographique SHA-256 pour le cache FinOps d'analyses documentaires.
/// </summary>
public static class DocumentHashCalculator
{
    /// <summary>
    /// Calcule l'empreinte SHA-256 hexadécimale en majuscules (64 caractères ASCII) d'un contenu textuel.
    /// </summary>
    public static string ComputeSha256Hex(string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content);

        byte[] bytes = Encoding.UTF8.GetBytes(content);
        byte[] hashBytes = SHA256.HashData(bytes);
        return Convert.ToHexString(hashBytes);
    }
}
