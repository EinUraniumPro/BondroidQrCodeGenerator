using BondroidQRHelper.Model;
using SkiaSharp;

namespace BondroidQRHelper.Interfaces;

/// <summary>
/// Utility to generate the QR-Code
/// </summary>
public interface IQrCodeUtil
{
    /// <summary>
    /// Generates a QR code bitmap from the given Table data.
    /// </summary>
    /// <param name="qrCodeText">The QR code text to render into a QR code.</param>
    /// <returns>A rendered SKBitmap representing the QR code.</returns>
    SKBitmap GenerateQrCode(string qrCodeText);

    /// <summary>
    /// Combines a QR code bitmap and a text label bitmap vertically into a single image.
    /// </summary>
    /// <param name="qrBitmap">The main QR code bitmap.</param>
    /// <param name="labelBitmap">The text label bitmap to display below the QR code.</param>
    /// <param name="spacing">The vertical spacing between the QR code and the label.</param>
    /// <param name="paddingBottom">Additional padding at the bottom of the combined image.</param>
    /// <returns>A new SKBitmap containing both images.</returns>
    SKBitmap CombineQrCodeAndLabel(
        SKBitmap qrBitmap,
        SKBitmap labelBitmap,
        int spacing = 0,
        int paddingBottom = 0);
}
