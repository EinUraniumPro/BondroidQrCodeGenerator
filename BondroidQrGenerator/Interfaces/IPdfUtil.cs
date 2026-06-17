using SkiaSharp;

namespace BondroidQRHelper.Interfaces;

/// <summary>
/// Utility to render the PDF with the QR-Codes and Labels
/// </summary>
public interface IPdfUtil
{
    /// <summary>
    /// Creates a multipage PDF document containing a grid of QR codes.
    /// </summary>
    /// <param name="qrBitmaps">A list of QR code bitmaps to render.</param>
    /// <param name="outputPath">The file path where the PDF will be saved.</param>
    /// <param name="columns">The number of columns in the grid per page.</param>
    /// <param name="margin">The outer margin of the page in PDF points.</param>
    /// <param name="cellPadding">The padding inside each grid cell in PDF points.</param>
    void CreateQrGridPdf(
        IReadOnlyList<SKBitmap?> qrBitmaps,
        string outputPath,
        int columns = 3,
        float margin = 5,
        float cellPadding = 10);
}
