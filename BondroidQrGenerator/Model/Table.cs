using CsvHelper.Configuration.Attributes;

namespace BondroidQRHelper.Model;

/// <summary>
/// Model of the Table of the CSV-File
/// </summary>
public sealed record Table
{
    /// <summary>
    /// Bondroid Id of the Table
    /// </summary>
    [Name("ID")]
    public int Id { get; set; } = -1;

    /// <summary>
    /// Area, where the Table is placed
    /// </summary>
    [Name("Bereich")]
    public string? Area { get; set; }

    /// <summary>
    /// Name of the Table
    /// </summary>
    [Name("Tisch")]
    public string? TableName { get; set; }

    /// <summary>
    /// String to encode into an QR-Code as Required in the Bondroid App
    /// </summary>
    [Name("QR Text")]
    public string? QrCodeText { get; set; }
}
