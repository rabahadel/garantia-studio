namespace GarantIA.Domain;

/// <summary>
/// Value Object représentant l'écart déterministe entre un niveau de garantie proposé et un niveau actuel.
/// </summary>
public readonly record struct EcartGarantie(
    TendanceComparaison Tendance,
    decimal DeltaValeur,
    string ResumeCourt);
