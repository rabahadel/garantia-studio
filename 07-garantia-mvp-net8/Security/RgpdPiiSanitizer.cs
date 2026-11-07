using System.Text.RegularExpressions;

namespace GarantIA.Security;

/// <summary>
/// Anonymiseur RGPD / Évitement HDS en mémoire vive (Zero-Data Retention).
/// Masque à la volée les NIR (13 à 15 caractères) et les adresses email via des expressions régulières compilées à la génération.
/// </summary>
public static partial class RgpdPiiSanitizer
{
    public const string NirMask = "[NIR-MASQUE-RGPD]";
    public const string EmailMask = "[EMAIL-MASQUE-RGPD]";

    [GeneratedRegex(@"\b[12](?:[\s.-]?\d{2}){2}[\s.-]?(?:\d{2}|2[ABab])(?:[\s.-]?\d{3}){2}(?:[\s.-]?\d{2})?\b|\b[12]\d{12,14}\b", RegexOptions.CultureInvariant)]
    private static partial Regex NirRegex();

    [GeneratedRegex(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b", RegexOptions.CultureInvariant)]
    private static partial Regex EmailRegex();

    /// <summary>
    /// Anonymise à la volée un texte brut en remplaçant les NIR et adresses email par des jetons RGPD.
    /// </summary>
    public static string AnonymiserTexteBrut(string rawText)
    {
        if (string.IsNullOrWhiteSpace(rawText))
        {
            return string.Empty;
        }

        string sansNir = NirRegex().Replace(rawText, NirMask);
        return EmailRegex().Replace(sansNir, EmailMask);
    }
}
